using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Ragdoll
{
	public class NetRope : NetworkBehaviour, IStateAuthorityChanged, IPublicFacingInterface
	{
		public const int Max = 32;

		[Header("Follow")]
		public float follow = 0.08f;

		public float grip = 0.5f;

		public float maxLead = 0.15f;

		public float claimWait = 0.5f;

		private Rope rope;

		private Rigidbody[] links;

		private HandGrab[] hands;

		private Player local;

		private float claimedAt = -9f;

		private const int Still = 16;

		private Vector3[][] stillPos;

		private int stillAt;

		private int stillCount;

		private NetRope[] group;

		private int passive = -1;

		[Networked]
		public Vector3 Origin { get; set; }

		[Networked]
		public int Unit { get; set; }

		[Networked]
		[Capacity(128)]
		public NetworkArray<int> Pose => default;

		[Networked]
		public int Count { get; set; }

		[Networked]
		public float Wear { get; set; }

		[Networked]
		public int SnapLink { get; set; }

		[Networked]
		public int Tint { get; set; }

		[Networked]
		public NetworkBool Resting { get; set; }

		public override void Spawned()
		{
			rope = GetComponent<Rope>();
			if ((bool)rope)
			{
				rope.onHit = (int at, float speed) => RPC_Hit(at, speed);
			}
			Rejoin();
			foreach (Rigidbody link in LinkBodies())
			{
				if ((bool)link)
				{
					link.maxDepenetrationVelocity = 1f;
				}
			}
		}

		private void Rejoin()
		{
			if (!transform.parent)
			{
				return;
			}
			foreach (Joint joint in transform.parent.GetComponentsInChildren<Joint>())
			{
				Rigidbody body = joint.connectedBody;
				if (!body || joint.transform.IsChildOf(transform) == body.transform.IsChildOf(transform))
				{
					continue;
				}
				Vector3 anchor = joint.connectedAnchor;
				joint.autoConfigureConnectedAnchor = false;
				joint.connectedBody = null;
				joint.connectedBody = body;
				joint.connectedAnchor = anchor;
			}
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			if ((bool)rope)
			{
				rope.onHit = null;
			}
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void RPC_Hit(int at, float speed)
		{
			if (!rope || SnapLink > 0)
			{
				return;
			}
			Wear += speed;
			rope.wear = Wear;
			if (Wear >= rope.snapWear)
			{
				SnapLink = at + 1;
			}
		}

		private void Start()
		{
			if ((bool)GetComponent<Rope>())
			{
				return;
			}
			Collider[] mine = GetComponentsInChildren<Collider>();
			HingeJoint hinge = GetComponent<HingeJoint>();
			Floater floater = GetComponentInParent<Floater>();
			foreach (NetRope other in Group())
			{
				if (other == this)
				{
					continue;
				}
				if ((bool)other.GetComponent<Rope>() && !hinge)
				{
					continue;
				}
				if ((bool)floater && other.GetComponentInParent<Floater>() != floater)
				{
					continue;
				}
				if ((bool)hinge && (bool)hinge.connectedBody && other.GetComponent<Rigidbody>() == hinge.connectedBody)
				{
					continue;
				}
				if (!hinge && (bool)other.GetComponent<HingeJoint>() && other.GetComponent<HingeJoint>().connectedBody == GetComponent<Rigidbody>())
				{
					continue;
				}
				foreach (Collider theirs in other.GetComponentsInChildren<Collider>())
				{
					foreach (Collider part in mine)
					{
						Physics.IgnoreCollision(part, theirs, true);
					}
				}
			}
		}

		public override void FixedUpdateNetwork()
		{
			if (!HasStateAuthority)
			{
				return;
			}
			if (Tint <= 0)
			{
				Tint = UnityEngine.Random.Range(1, 100000);
			}
			if ((bool)rope && SnapLink == 0 && GameManager.SnapDue(this))
			{
				SnapLink = rope.Links().Length / 2 + 1;
			}
			Rigidbody[] bodies = LinkBodies();
			Resting = !HeldByLocalHand() && Stopped(bodies);
			int count = Mathf.Min(bodies.Length, Max);
			if (bodies.Length > 0)
			{
				Vector3 origin = bodies[0].position;
				float spread = 0f;
				for (int i = 1; i < count; i++)
				{
					Vector3 offset = bodies[i].position - origin;
					spread = Mathf.Max(spread, Mathf.Max(Mathf.Abs(offset.x), Mathf.Max(Mathf.Abs(offset.y), Mathf.Abs(offset.z))));
				}
				int unit = (spread < 32f) ? 0 : ((spread < 320f) ? 1 : 2);
				float per = Per(unit);
				Origin = origin;
				Unit = unit;
				for (int i = 0; i < count; i++)
				{
					int at = i * 4;
					if (at != 0)
					{
						Vector3 offset = per * (bodies[i].position - origin);
						Pose.Set(at, (Short(offset.x) & 0xFFFF) | (Short(offset.y) << 16));
						Pose.Set(at + 1, Short(offset.z));
					}
					Pose.Set(at + 2, NetworkPlayer.PackTurn(bodies[i].rotation));
					Pose.Set(at + 3, NetworkPlayer.PackSpeed(bodies[i].linearVelocity));
				}
			}
			Count = count;
		}

		private void FixedUpdate()
		{
			if (!Object || !Object.IsValid)
			{
				return;
			}
			if ((bool)rope && SnapLink > 0 && rope.snapped < 0)
			{
				rope.Snap(SnapLink - 1);
			}
			if (Object.StateAuthority == PlayerRef.None && Runner.IsSharedModeMasterClient)
			{
				Claim("unowned");
			}
			if (HeldByLocalHand() && !OwnerHolds())
			{
				Claim("holding");
				return;
			}
			if (HasStateAuthority)
			{
				return;
			}
			if (Passive() && GroupResting() && (!rope || rope.snapped < 0))
			{
				return;
			}
			Rigidbody[] bodies = LinkBodies();
			int count = Mathf.Min(Count, bodies.Length);
			float lead = Mathf.Min((float)Runner.GetPlayerRtt(Runner.LocalPlayer) * 0.5f, maxLead);
			for (int i = 0; i < count; i++)
			{
				Rigidbody link = bodies[i];
				if (!link || link.isKinematic)
				{
					continue;
				}
				Vector3 speed = Speed(i);
				Vector3 spot = Spot(i);
				Vector3 pull = Vector3.ClampMagnitude((spot + lead * speed - link.position) / follow, 10f);
				link.linearVelocity = Vector3.Lerp(link.linearVelocity, speed + pull, grip);
				(NetworkPlayer.UnpackTurn(Pose[i * 4 + 2]) * Quaternion.Inverse(link.rotation)).ToAngleAxis(out float angle, out Vector3 axis);
				if (angle > 180f)
				{
					angle -= 360f;
				}
				if (float.IsFinite(axis.x))
				{
					link.angularVelocity = Vector3.Lerp(link.angularVelocity, axis * (angle * Mathf.Deg2Rad / follow), grip);
				}
			}
		}

		public float Behind()
		{
			if (!Object || !Object.IsValid || HasStateAuthority || Count < 1)
			{
				return 0f;
			}
			Rigidbody[] bodies = LinkBodies();
			float behind = 0f;
			for (int i = 0; i < Mathf.Min(Count, bodies.Length); i++)
			{
				if ((bool)bodies[i])
				{
					behind = Mathf.Max(behind, Vector3.Distance(Spot(i), bodies[i].position));
				}
			}
			return behind;
		}

		private Vector3 Spot(int i)
		{
			if (i == 0)
			{
				return Origin;
			}
			int packed = Pose[i * 4];
			int z = Pose[i * 4 + 1];
			return Origin + new Vector3((short)packed, packed >> 16, z) / Per(Unit);
		}

		private Vector3 Speed(int i)
		{
			return NetworkPlayer.UnpackSpeed(Pose[i * 4 + 3]);
		}

		private static float Per(int unit)
		{
			if (unit < 1)
			{
				return 1000f;
			}
			return (unit == 1) ? 100f : 10f;
		}

		private static int Short(float v)
		{
			return Mathf.Clamp(Mathf.RoundToInt(v), -32767, 32767);
		}

		private bool Passive()
		{
			if (passive < 0)
			{
				passive = 0;
				if ((bool)rope || (bool)GetComponent<HingeJoint>())
				{
					foreach (NetRope other in Group())
					{
						if ((bool)other && !other.GetComponent<Rope>() && !other.GetComponent<HingeJoint>())
						{
							passive = 1;
						}
					}
				}
			}
			return passive == 1;
		}

		public void StateAuthorityChanged()
		{
			stillCount = 0;
			if (!HasStateAuthority)
			{
				return;
			}
			Rigidbody[] bodies = LinkBodies();
			int count = Mathf.Min(Count, bodies.Length);
			for (int i = 0; i < count; i++)
			{
				if ((bool)bodies[i] && !bodies[i].isKinematic)
				{
					bodies[i].linearVelocity = Speed(i);
					bodies[i].angularVelocity = Vector3.zero;
				}
			}
		}

		private bool OwnerHolds()
		{
			PlayerRef owner = Root().Object.StateAuthority;
			foreach (NetRope member in Group())
			{
				if ((bool)member && (bool)member.Object && member.Object.IsValid && NetworkPlayer.Holds(owner, member.Object.Id))
				{
					return true;
				}
			}
			return false;
		}

		private NetRope Root()
		{
			NetRope root = this;
			foreach (NetRope member in Group())
			{
				if ((bool)member && (bool)member.Object && member.Object.IsValid && member.Object.Id.Raw < root.Object.Id.Raw)
				{
					root = member;
				}
			}
			return root;
		}

		private bool GroupResting()
		{
			foreach (NetRope member in Group())
			{
				if ((bool)member && (bool)member.Object && member.Object.IsValid && !member.Resting)
				{
					return false;
				}
			}
			return true;
		}

		private bool Stopped(Rigidbody[] bodies)
		{
			int count = Mathf.Min(bodies.Length, Max);
			if (stillPos == null || stillPos.Length != Still || count != stillPos[0].Length)
			{
				stillPos = new Vector3[Still][];
				for (int i = 0; i < Still; i++)
				{
					stillPos[i] = new Vector3[count];
				}
				stillCount = 0;
			}
			bool sleeping = true;
			for (int i = 0; i < count; i++)
			{
				stillPos[stillAt][i] = bodies[i] ? bodies[i].position : Vector3.zero;
				if ((bool)bodies[i] && !bodies[i].isKinematic)
				{
					sleeping &= bodies[i].IsSleeping();
				}
			}
			int last = stillAt;
			stillAt = (stillAt + 1) % Still;
			if (stillCount < Still)
			{
				stillCount++;
			}
			if (sleeping)
			{
				return true;
			}
			if (stillCount < Still)
			{
				return false;
			}
			for (int s = 0; s < Still; s++)
			{
				for (int i = 0; i < count; i++)
				{
					if ((stillPos[s][i] - stillPos[last][i]).sqrMagnitude > 0.0001f)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void Claim(string why)
		{
			if (Time.time < claimedAt + claimWait)
			{
				return;
			}
			bool owned = true;
			foreach (NetRope member in Group())
			{
				if ((bool)member && (bool)member.Object && member.Object.IsValid)
				{
					owned &= member.HasStateAuthority;
				}
			}
			if (owned)
			{
				return;
			}
			claimedAt = Time.time;
			Player.Note("claiming " + name + " group from player " + Object.StateAuthority.PlayerId + " (" + why + ")");
			foreach (NetRope member in Group())
			{
				if ((bool)member && (bool)member.Object && member.Object.IsValid && !member.HasStateAuthority)
				{
					member.Object.RequestStateAuthority();
				}
			}
		}

		private NetRope[] Group()
		{
			if (group == null)
			{
				List<NetRope> list = new List<NetRope> { this };
				NetRope[] all = UnityEngine.Object.FindObjectsByType<NetRope>(FindObjectsSortMode.None);
				for (int i = 0; i < list.Count; i++)
				{
					foreach (NetRope other in all)
					{
						if (!list.Contains(other) && (Joins(list[i].LinkBodies(), other.LinkBodies()) || Joins(other.LinkBodies(), list[i].LinkBodies())))
						{
							list.Add(other);
						}
					}
				}
				group = list.ToArray();
			}
			return group;
		}

		private static bool Joins(Rigidbody[] from, Rigidbody[] to)
		{
			foreach (Rigidbody body in from)
			{
				if (!body)
				{
					continue;
				}
				foreach (Joint joint in body.GetComponents<Joint>())
				{
					if ((bool)joint.connectedBody && Array.IndexOf(to, joint.connectedBody) >= 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private Rigidbody[] LinkBodies()
		{
			if (links == null || links.Length == 0)
			{
				if (!rope)
				{
					rope = GetComponent<Rope>();
				}
				Rigidbody body = GetComponent<Rigidbody>();
				if ((bool)rope)
				{
					links = rope.Links();
				}
				else
				{
					links = body ? new Rigidbody[1] { body } : new Rigidbody[0];
				}
			}
			return links;
		}

		private bool HeldByLocalHand()
		{
			if (hands == null)
			{
				NetworkManager manager = UnityEngine.Object.FindFirstObjectByType<NetworkManager>();
				local = manager ? manager.localBody : null;
				hands = local ? local.GetComponentsInChildren<HandGrab>() : new HandGrab[0];
			}
			foreach (HandGrab hand in hands)
			{
				if (!hand || !hand.grabbing)
				{
					continue;
				}
				if ((bool)rope && hand.heldRope == rope)
				{
					return true;
				}
				if ((bool)hand.held && (hand.held.transform == transform || hand.held.transform.parent == transform))
				{
					return true;
				}
			}
			return false;
		}
	}
}
