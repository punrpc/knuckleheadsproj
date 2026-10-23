using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Ragdoll
{
	public class LooseHat : NetworkBehaviour
	{
		[Header("Hats")]
		public List<Cosmetics.Item> hats = new List<Cosmetics.Item>();

		[Header("Put On")]
		public float putOnReach = 0.06f;

		private static LooseHat prefab;

		private Rigidbody body;

		private BoxCollider box;

		private string shown = "";

		private string offlineItem = "";

		private Player offlineOwner;

		private bool gone;

		[Networked]
		public NetworkString<_16> Item { get; set; }

		[Networked]
		public PlayerRef Owner { get; set; }

		public static void Drop(string item, Vector3 at, Quaternion rot, Vector3 velocity, Player from)
		{
			if (!prefab)
			{
				prefab = Resources.Load<LooseHat>("LooseHat");
			}
			if (!prefab)
			{
				return;
			}
			NetworkManager manager = FindFirstObjectByType<NetworkManager>();
			LooseHat hat;
			if (manager && manager.runner && manager.runner.IsRunning)
			{
				hat = manager.runner.Spawn(prefab, at, rot, null, delegate(NetworkRunner runner, NetworkObject obj)
				{
					LooseHat spawned = obj.GetComponent<LooseHat>();
					spawned.Item = item;
					spawned.Owner = runner.LocalPlayer;
				});
			}
			else
			{
				hat = Instantiate(prefab, at, rot);
				hat.offlineItem = item;
				hat.offlineOwner = from;
			}
			if (hat)
			{
				Rigidbody rb = hat.GetComponent<Rigidbody>();
				rb.linearVelocity = velocity;
				rb.angularVelocity = Random.insideUnitSphere * 6f;
			}
		}

		public static void Return(Player owner)
		{
			NetworkManager manager = FindFirstObjectByType<NetworkManager>();
			bool offline = !manager || !manager.runner || !manager.runner.IsRunning;
			foreach (LooseHat hat in FindObjectsByType<LooseHat>(FindObjectsSortMode.None))
			{
				if (offline)
				{
					if (hat.offlineOwner == owner)
					{
						hat.Gone();
					}
				}
				else if (hat.Object && hat.Object.IsValid && hat.Owner == manager.runner.LocalPlayer)
				{
					hat.Gone();
				}
			}
		}

		public override void Spawned()
		{
			Show(Item.Value);
		}

		private void Start()
		{
			if (!Object || !Object.IsValid)
			{
				Show(offlineItem);
			}
		}

		private void Show(string item)
		{
			if (shown == item || item == "")
			{
				return;
			}
			shown = item;
			body = GetComponent<Rigidbody>();
			box = GetComponent<BoxCollider>();
			bool first = true;
			Bounds bounds = default(Bounds);
			foreach (Cosmetics.Item hat in hats)
			{
				bool on = hat.name == item;
				foreach (GameObject part in hat.parts)
				{
					if (!part)
					{
						continue;
					}
					part.SetActive(on);
					if (!on)
					{
						continue;
					}
					foreach (MeshFilter mf in part.GetComponentsInChildren<MeshFilter>())
					{
						Bounds mb = mf.sharedMesh.bounds;
						for (int k = 0; k < 8; k++)
						{
							Vector3 corner = mb.center + new Vector3(((k & 1) == 0) ? (0f - mb.extents.x) : mb.extents.x, ((k & 2) == 0) ? (0f - mb.extents.y) : mb.extents.y, ((k & 4) == 0) ? (0f - mb.extents.z) : mb.extents.z);
							Vector3 p = transform.InverseTransformPoint(mf.transform.TransformPoint(corner));
							if (first)
							{
								bounds = new Bounds(p, Vector3.zero);
							}
							else
							{
								bounds.Encapsulate(p);
							}
							first = false;
						}
					}
				}
			}
			if (!first && box)
			{
				box.center = bounds.center;
				box.size = bounds.size;
			}
		}

		private void FixedUpdate()
		{
			bool offline = !Object || !Object.IsValid;
			if (!offline && Item != shown)
			{
				Show(Item.Value);
			}
			if (gone || !body)
			{
				return;
			}
			if (!offline && !HasStateAuthority)
			{
				return;
			}
			if (transform.position.y < -40f)
			{
				Gone();
				return;
			}
			HandGrab held = HeldBy();
			if (!held)
			{
				return;
			}
			string item = offline ? offlineItem : Item.Value;
			Player me = Player.main;
			if (me && me.head && me.cosmetics.worn[0] == "" && Reaches(me.head.transform))
			{
				held.Release();
				if (me.HatOn(item))
				{
					Gone();
				}
				return;
			}
			if (offline)
			{
				return;
			}
			foreach (NetworkPlayer np in FindObjectsByType<NetworkPlayer>(FindObjectsSortMode.None))
			{
				if (np.Object && np.Object.IsValid && np.HasStateAuthority)
				{
					continue;
				}
				if (np.Head.Length > 0)
				{
					continue;
				}
				Transform headBone = np.HeadBone();
				if (headBone && Reaches(headBone))
				{
					held.Release();
					np.RPC_Wear(item);
					Gone();
					break;
				}
			}
		}

		private bool Reaches(Transform head)
		{
			Vector3 top = Top(head);
			if (box && box.enabled)
			{
				if (Vector3.Distance(box.ClosestPoint(top), top) < putOnReach)
				{
					return box.bounds.center.y > top.y - 0.05f;
				}
				return false;
			}
			return Vector3.Distance(transform.position, top) < putOnReach;
		}

		private static Vector3 Top(Transform head)
		{
			return head.position + head.up * 0.36f;
		}

		private HandGrab HeldBy()
		{
			Player me = Player.main;
			if (!me)
			{
				return null;
			}
			foreach (HandGrab hand in me.GetComponentsInChildren<HandGrab>())
			{
				if (hand.grabbing && hand.held == body)
				{
					return hand;
				}
			}
			return null;
		}

		public void Gone()
		{
			if (gone)
			{
				return;
			}
			if (!Object || !Object.IsValid)
			{
				gone = true;
				Destroy(gameObject);
			}
			else if (HasStateAuthority)
			{
				gone = true;
				Runner.Despawn(Object);
			}
			else
			{
				RPC_Gone();
			}
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private void RPC_Gone()
		{
			Gone();
		}
	}
}
