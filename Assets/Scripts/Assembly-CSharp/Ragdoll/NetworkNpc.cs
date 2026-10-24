using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Ragdoll
{
	public class NetworkNpc : NetworkBehaviour, IStateAuthorityChanged, IPublicFacingInterface
	{
		public class Bone : MonoBehaviour
		{
			public NetworkNpc npc;

			public int index;

			private void OnCollisionEnter(Collision c)
			{
				if (npc)
				{
					npc.Touch(index, c);
				}
			}
		}

		public GameObject[] prefabs;

		public static readonly List<NetworkNpc> all = new List<NetworkNpc>();

		private const int Bones = 13;

		private const int Live = 1;

		private const int RightArm = 2;

		private const int BothArms = 4;

		private const int Out = 8;

		private const int Gone = 16;

		private const int Hush = 32;

		private const int Kept = 64;

		private const int Talk = 128;

		private const int Flop = 256;

		private const float Closer = 2f;

		private const int Crowded = 3;

		private const float Follow = 18f;

		private const float Snap = 1.5f;

		private const float HitGap = 0.2f;

		private const float SlapMin = 2f;

		private const float LiveGrace = 0.25f;

		private const float AskGap = 1f;

		private const float Reach = 0.45f;

		private const float Shunned = 20f;

		private static readonly List<Collider> shunCols = new List<Collider>();

		private static readonly string[] BoneNames = new string[13]
		{
			"LowerTorso", "UpperTorso", "Head", "UpperArm.L", "LowerArm.L", "Hand.L", "UpperArm.R", "LowerArm.R", "Hand.R", "UpperLeg.L",
			"LowerLeg.L", "UpperLeg.R", "LowerLeg.R"
		};

		private readonly Rigidbody[] bones = new Rigidbody[13];

		private SlapDummy dummy;

		private Player body;

		private Transform puppet;

		private Cosmetics look;

		private int dressedAs;

		private bool lookKept;

		private bool posed;

		private bool wasLive;

		private int hitSwing = -1;

		private int liveState;

		private int liveSwing;

		private float liveUntil = -9f;

		private float hitAt = -9f;

		private float askedAt = -9f;

		private bool wasGone;

		private Collider[] cols;

		private KnockStars stars;

		private Player.Babble mouth;

		private bool wasOut;

		private readonly Vector3[] keptAt = new Vector3[13];

		private readonly Quaternion[] keptTurn = new Quaternion[13];

		private int keptHas;

		private Vector3 keptVel;

		private Vector3 rootVel;

		private Vector3 lastRoot;

		private float lastRootAt = -1f;

		private int reposeFrame = -1;

		private bool rebind;

		private GameObject fading;

		private readonly List<Renderer> veiled = new List<Renderer>();

		[Networked]
		public int Kind { get; set; }

		[Networked]
		public int Look { get; set; }

		[Networked]
		public int State { get; set; }

		[Networked]
		public int Swing { get; set; }

		[Networked]
		public int Kos { get; set; }

		[Networked]
		public PlayerRef Want { get; set; }

		[Networked]
		public PlayerRef Spawner { get; set; }

		[Networked]
		public Vector3 Root { get; set; }

		[Networked]
		[Capacity(39)]
		public NetworkArray<int> Pose => default;

		public bool Alive
		{
			get
			{
				if (Object && Object.IsValid)
				{
					return (State & Gone) == 0;
				}
				return false;
			}
		}

		public bool Down
		{
			get
			{
				if (Object && Object.IsValid)
				{
					return (State & (Out | Gone | Flop)) != 0;
				}
				return false;
			}
		}

		public static int Dropped(PlayerRef who)
		{
			int count = 0;
			foreach (NetworkNpc npc in all)
			{
				if (npc && npc.Object && npc.Object.IsValid && npc.Alive && npc.Spawner == who)
				{
					count++;
				}
			}
			return count;
		}

		private static int Held(PlayerRef who)
		{
			int count = 0;
			foreach (NetworkNpc npc in all)
			{
				if (npc && npc.Object && npc.Object.IsValid && npc.Alive && npc.Object.StateAuthority == who)
				{
					count++;
				}
			}
			return count;
		}

		public static int Owned()
		{
			int count = 0;
			foreach (NetworkNpc npc in all)
			{
				if (npc && npc.Object && npc.Object.IsValid && npc.HasStateAuthority)
				{
					count += npc.Alive ? 1 : 0;
				}
			}
			return count;
		}

		public static int Remaining()
		{
			all.RemoveAll((NetworkNpc npc) => !npc);
			int count = 0;
			foreach (NetworkNpc npc2 in all)
			{
				if (npc2)
				{
					count += npc2.Alive ? 1 : 0;
				}
			}
			return count;
		}

		private GameObject Prefab()
		{
			if (prefabs == null || prefabs.Length == 0)
			{
				return null;
			}
			return prefabs[Mathf.Clamp(Kind - 1, 0, prefabs.Length - 1)];
		}

		public override void Spawned()
		{
			all.Add(this);
			GameObject pre = Prefab();
			if (pre)
			{
				if (HasStateAuthority)
				{
					Real(pre, transform.position, transform.rotation);
				}
				else
				{
					Puppet(pre);
				}
			}
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			all.Remove(this);
			if (fading)
			{
				Destroy(fading);
			}
			if (dummy)
			{
				Destroy(dummy.transform.root.gameObject);
			}
			if (puppet)
			{
				if (bones[0])
				{
					Player.HitAt(bones[0].position, true);
				}
				Destroy(puppet.gameObject);
			}
		}

		public void StateAuthorityChanged()
		{
			if (HasStateAuthority && !dummy)
			{
				TakeOver();
			}
			else if (!HasStateAuthority && dummy)
			{
				GiveUp();
			}
		}

		private void Real(GameObject pre, Vector3 at, Quaternion turn)
		{
			GameObject copy = Instantiate(pre, at, turn);
			dummy = copy.GetComponentInChildren<SlapDummy>();
			body = dummy ? dummy.GetComponent<Player>() : null;
			lookKept = false;
			wasLive = false;
			Find(copy.transform);
		}

		private void TakeOver()
		{
			GameObject pre = Prefab();
			if (!pre)
			{
				return;
			}
			int state = State;
			if ((state & Gone) != 0)
			{
				if (puppet)
				{
					Destroy(puppet.gameObject);
				}
				puppet = null;
				Runner.Despawn(Object);
				return;
			}
			Vector3 at = (posed && bones[0]) ? bones[0].position : ((Root == Vector3.zero) ? transform.position : Root);
			keptHas = 0;
			if (posed)
			{
				for (int i = 0; i < Bones; i++)
				{
					if (bones[i])
					{
						keptAt[i] = bones[i].position;
						keptTurn[i] = bones[i].rotation;
						keptHas |= 1 << i;
					}
				}
			}
			keptVel = rootVel;
			if (fading)
			{
				Destroy(fading);
			}
			fading = null;
			if (puppet && keptHas != 0)
			{
				fading = puppet.gameObject;
				foreach (Collider col in fading.GetComponentsInChildren<Collider>())
				{
					col.enabled = false;
				}
			}
			else if (puppet)
			{
				Destroy(puppet.gameObject);
			}
			puppet = null;
			posed = false;
			Player me = Player.main;
			Vector3 face = (me && me.pelvis) ? (me.pelvis.position - at) : Vector3.forward;
			face.y = 0f;
			Quaternion turn = (face.sqrMagnitude > 0.01f) ? Quaternion.LookRotation(face) : Quaternion.identity;
			Vector3 offset = Vector3.up * 0.5f;
			foreach (Rigidbody rb in pre.GetComponentsInChildren<Rigidbody>(true))
			{
				if (rb.name == BoneNames[0])
				{
					offset = rb.transform.position - pre.transform.position;
				}
			}
			Vector3 spot = at - turn * offset;
			if (Physics.Raycast(at + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 2f, -1, QueryTriggerInteraction.Ignore) && hit.point.y > spot.y)
			{
				spot.y = hit.point.y;
			}
			Real(pre, spot, turn);
			veiled.Clear();
			if (keptHas != 0 && dummy)
			{
				foreach (Renderer r in dummy.transform.root.GetComponentsInChildren<Renderer>())
				{
					if (r.enabled)
					{
						r.enabled = false;
						veiled.Add(r);
					}
				}
			}
			if (keptHas != 0)
			{
				foreach (Rigidbody bone in bones)
				{
					if (bone && !bone.isKinematic)
					{
						bone.linearVelocity = keptVel;
					}
				}
			}
			if (dummy)
			{
				dummy.Kos = Kos;
			}
			if (body && (State & Out) != 0)
			{
				body.knockoutTime = SlapDummy.KoTime((SlapDummy.Kind)Mathf.Clamp(Kind, 1, 4));
				body.KnockOut(at, false);
			}
			reposeFrame = (keptHas != 0 && dummy) ? Time.frameCount : (-1);
			rebind = (State & Out) != 0;
			Player.Note("took over a " + (SlapDummy.Kind)Mathf.Clamp(Kind, 1, 4) + " (" + Kos + " knockouts), I'm the nearest player");
		}

		private void Repose()
		{
			reposeFrame = -1;
			if (fading)
			{
				Destroy(fading);
			}
			fading = null;
			foreach (Renderer r in veiled)
			{
				if (r)
				{
					r.enabled = true;
				}
			}
			veiled.Clear();
			if (!dummy || !body)
			{
				return;
			}
			for (int i = 0; i < Bones; i++)
			{
				Rigidbody bone = bones[i];
				if (bone && ((keptHas >> i) & 1) != 0)
				{
					bone.transform.SetPositionAndRotation(keptAt[i], keptTurn[i]);
					bone.position = keptAt[i];
					bone.rotation = keptTurn[i];
					bone.linearVelocity = keptVel;
					bone.angularVelocity = Vector3.zero;
				}
			}
			body.Reframe();
			Player me = Player.main;
			if (!rebind || !me)
			{
				return;
			}
			rebind = false;
			Physics.SyncTransforms();
			if (Near(me, me.leftHand, 1f))
			{
				me.Regrab(true, dummy.transform.root);
			}
			if (Near(me, me.rightHand, 1f))
			{
				me.Regrab(false, dummy.transform.root);
			}
		}

		private bool Reaching(Player me)
		{
			if (!posed || !me)
			{
				return false;
			}
			if (!Near(me, me.leftHand, me.gripL))
			{
				return Near(me, me.rightHand, me.gripR);
			}
			return true;
		}

		private bool Near(Player me, Rigidbody hand, float grip)
		{
			if (grip < 0.5f || !hand)
			{
				return false;
			}
			if (me.SwingSpeed(hand, 6) >= 3.5f)
			{
				return false;
			}
			foreach (Rigidbody bone in bones)
			{
				if (bone && (bone.position - hand.position).sqrMagnitude < Reach * Reach)
				{
					return true;
				}
			}
			return false;
		}

		private void GiveUp()
		{
			if (fading)
			{
				Destroy(fading);
			}
			fading = null;
			veiled.Clear();
			reposeFrame = -1;
			if (dummy)
			{
				Destroy(dummy.transform.root.gameObject);
			}
			dummy = null;
			body = null;
			for (int i = 0; i < Bones; i++)
			{
				bones[i] = null;
			}
			GameObject pre = Prefab();
			if (pre)
			{
				Puppet(pre);
			}
		}

		private void Find(Transform root)
		{
			Dictionary<string, Rigidbody> byName = new Dictionary<string, Rigidbody>();
			foreach (Rigidbody rb in root.GetComponentsInChildren<Rigidbody>(true))
			{
				if (!byName.ContainsKey(rb.name))
				{
					byName[rb.name] = rb;
				}
			}
			for (int i = 0; i < Bones; i++)
			{
				byName.TryGetValue(BoneNames[i], out bones[i]);
			}
		}

		private void Puppet(GameObject pre)
		{
			posed = false;
			GameObject hold = new GameObject("Npc Puppet Hold");
			hold.SetActive(false);
			GameObject copy = Instantiate(pre, transform.position, transform.rotation, hold.transform);
			copy.name = pre.name + " Puppet";
			Player player = copy.GetComponentInChildren<Player>(true);
			look = player ? player.cosmetics : null;
			dressedAs = 0;
			foreach (Joint j in copy.GetComponentsInChildren<Joint>(true))
			{
				DestroyImmediate(j);
			}
			for (int pass = 0; pass < 3; pass++)
			{
				MonoBehaviour[] scripts = copy.GetComponentsInChildren<MonoBehaviour>(true);
				if (scripts.Length == 0)
				{
					break;
				}
				for (int k = scripts.Length - 1; k >= 0; k--)
				{
					if (scripts[k])
					{
						DestroyImmediate(scripts[k]);
					}
				}
			}
			copy.AddComponent<Grabbable>().enabled = false;
			Find(copy.transform);
			for (int i = 0; i < Bones; i++)
			{
				Rigidbody rb = bones[i];
				if (rb)
				{
					rb.transform.SetParent(copy.transform, true);
					rb.isKinematic = true;
					rb.useGravity = false;
					rb.interpolation = RigidbodyInterpolation.Interpolate;
					rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
					Bone bone = rb.gameObject.AddComponent<Bone>();
					bone.npc = this;
					bone.index = i;
				}
			}
			foreach (Renderer r in copy.GetComponentsInChildren<Renderer>(true))
			{
				if (r.name == "Blink" || r.name == "Dizzy")
				{
					r.enabled = false;
				}
			}
			stars = copy.AddComponent<KnockStars>();
			stars.head = bones[2] ? bones[2].transform : null;
			copy.SetActive(false);
			copy.transform.SetParent(transform, true);
			Destroy(hold);
			cols = copy.GetComponentsInChildren<Collider>(true);
			puppet = copy.transform;
			Dress();
		}

		private void Dress()
		{
			int code = Look;
			if (!(look is null) && code != 0 && code != dressedAs)
			{
				dressedAs = code;
				Wear(look, code, null);
			}
		}

		private static void Wear(Cosmetics c, int code, Player wearer)
		{
			for (int slot = 0; slot < Cosmetics.Slots.Length; slot++)
			{
				List<Cosmetics.Item> items = c.List(slot);
				int pick = (code >> (slot * 5)) & 0x1F;
				string item = (pick == 0 || pick > items.Count) ? "" : items[pick - 1].name;
				if (wearer)
				{
					wearer.Wear(slot, item);
				}
				else
				{
					c.Wear(slot, item);
				}
			}
		}

		private static int Outfit(Cosmetics c)
		{
			int code = 0x100000;
			for (int slot = 0; slot < Cosmetics.Slots.Length; slot++)
			{
				int index = c.List(slot).FindIndex((Cosmetics.Item it) => it.name == c.worn[slot]);
				code |= ((index + 1) & 0x1F) << (slot * 5);
			}
			return code;
		}

		public override void FixedUpdateNetwork()
		{
			if (!HasStateAuthority || puppet)
			{
				return;
			}
			if (!dummy || !body || !bones[0])
			{
				Runner.Despawn(Object);
				return;
			}
			Vector3 root = bones[0].position;
			Root = root;
			for (int i = 0; i < Bones; i++)
			{
				if (!bones[i])
				{
					continue;
				}
				if (i != 0)
				{
					Vector3 d = bones[i].position - root;
					Pose.Set(i * 3, (Mm(d.x) & 0xFFFF) | (Mm(d.y) << 16));
					Pose.Set(i * 3 + 1, Mm(d.z));
				}
				Pose.Set(i * 3 + 2, NetworkPlayer.PackTurn(bones[i].rotation));
			}
			bool live = dummy.SwingLive || dummy.Pouncing || dummy.Jabbing;
			int state = live ? Live : 0;
			if (dummy.Jabbing || (dummy.SwingLive && dummy.PunchRight))
			{
				state |= RightArm;
			}
			if (dummy.Pouncing)
			{
				state |= BothArms;
			}
			if (body.knockedOut)
			{
				state |= Out;
			}
			if (body.Ragdolled())
			{
				state |= Flop;
			}
			if (Grasped())
			{
				state |= Kept;
			}
			if (dummy.Gone)
			{
				state |= Gone;
			}
			if (dummy.Quiet)
			{
				state |= Hush;
			}
			if (dummy.Talking)
			{
				state |= Talk;
			}
			if (live && !wasLive)
			{
				Swing++;
			}
			wasLive = live;
			State = state;
			Kos = dummy.Kos;
			if (dummy.Dressed && !lookKept)
			{
				lookKept = true;
				if (Look == 0)
				{
					Look = Outfit(body.cosmetics);
				}
				else
				{
					Wear(body.cosmetics, Look, body);
				}
			}
			if (dummy.Gone)
			{
				return;
			}
			Player me = Player.main;
			if (me && me.pelvis && !me.dead && !me.drowned)
			{
				Vector3 gap = me.pelvis.position - root;
				gap.y = 0f;
				PlayerRef near = Nearest(root, gap.magnitude - Closer, true);
				if (body.knockedOut || dummy.Hauling || (State & Kept) != 0)
				{
					Want = Runner.LocalPlayer;
				}
				else if (!Busy())
				{
					Want = (near != PlayerRef.None) ? near : Runner.LocalPlayer;
				}
			}
			else
			{
				PlayerRef near2 = Nearest(root, float.MaxValue, false);
				Want = near2;
				if (near2 == PlayerRef.None)
				{
					dummy.End("left");
				}
			}
		}

		private bool Busy()
		{
			if (body.flopped || dummy.Punching || dummy.Pouncing || dummy.Clinging || dummy.Hauling)
			{
				return true;
			}
			return dummy.Scaling;
		}

		private bool Grasped()
		{
			Player me = Player.main;
			if (!me)
			{
				return false;
			}
			foreach (Rigidbody bone in bones)
			{
				if (bone && me.Holds(bone))
				{
					return true;
				}
			}
			return false;
		}

		private PlayerRef Nearest(Vector3 from, float limit, bool crowd)
		{
			PlayerRef best = PlayerRef.None;
			float bestDistance = limit;
			foreach (NetworkPlayer avatar in NetworkPlayer.avatars)
			{
				if (!avatar || !avatar.Object || !avatar.Object.IsValid || (bool)avatar.Dead || (bool)avatar.Watching)
				{
					continue;
				}
				PlayerRef who = avatar.Object.StateAuthority;
				Vector3 root = avatar.Root;
				if (who == PlayerRef.None || who == Runner.LocalPlayer || root == Vector3.zero || (crowd && Held(who) >= Crowded))
				{
					continue;
				}
				float distance = new Vector2(root.x - from.x, root.z - from.z).magnitude;
				if (distance < bestDistance)
				{
					bestDistance = distance;
					best = who;
				}
			}
			return best;
		}

		private static int Mm(float metres)
		{
			return Mathf.Clamp(Mathf.RoundToInt(metres * 1000f), -32767, 32767);
		}

		private Vector3 Spot(Vector3 root, int bone)
		{
			if (bone == 0)
			{
				return root;
			}
			int xy = Pose[bone * 3];
			int z = Pose[bone * 3 + 1];
			return new Vector3(root.x + (float)(short)xy * 0.001f, root.y + (float)(xy >> 16) * 0.001f, root.z + (float)z * 0.001f);
		}

		private void FixedUpdate()
		{
			if (reposeFrame >= 0 && Time.frameCount > reposeFrame + 1)
			{
				Repose();
			}
			if (!puppet || !Object || !Object.IsValid)
			{
				return;
			}
			Dress();
			int state = State;
			if (stars)
			{
				stars.on = (state & (Out | Gone)) == Out;
			}
			if ((state & (Out | Gone)) == Out && !wasOut && posed && bones[2])
			{
				Player.KnockAt(bones[2].position);
			}
			wasOut = (state & (Out | Gone)) == Out;
			Player me = Player.main;
			if (me && bones[2] && posed)
			{
				if (mouth == null || !mouth.source)
				{
					mouth = new Player.Babble
					{
						source = me.SpeakerOn(bones[2].transform)
					};
				}
				mouth.Tick((state & (Talk | Out | Gone)) == Talk, me.blabbers, me.blabberVolume * 0.6f, SlapDummy.Pitch((SlapDummy.Kind)Mathf.Clamp(Kind, 1, 4)));
			}
			if ((state & Gone) != 0 && !wasGone)
			{
				wasGone = true;
				if (posed && (state & Hush) == 0)
				{
					GameManager.Died();
				}
			}
			if ((state & Live) != 0)
			{
				liveState = state;
				liveSwing = Swing;
				liveUntil = Time.time + LiveGrace;
			}
			Player main = Player.main;
			bool ask = false;
			if ((state & (Gone | Kept)) == 0)
			{
				ask = Want == Runner.LocalPlayer && (state & Out) == 0;
				if ((state & Out) != 0)
				{
					ask = Reaching(main);
				}
			}
			if (ask && !HasStateAuthority && main && !main.dead && !main.drowned && Time.time > askedAt + AskGap)
			{
				askedAt = Time.time;
				Object.RequestStateAuthority();
			}
			Vector3 root = Root;
			if (root == Vector3.zero)
			{
				return;
			}
			float now = Time.time;
			if (root == lastRoot)
			{
				if (now - lastRootAt > 0.3f)
				{
					rootVel = Vector3.zero;
				}
			}
			else
			{
				rootVel = (lastRootAt >= 0f && now - lastRootAt < 0.3f && (root - lastRoot).sqrMagnitude < Snap * Snap) ? ((root - lastRoot) / Mathf.Max(now - lastRootAt, Runner.DeltaTime)) : Vector3.zero;
				lastRoot = root;
				lastRootAt = Time.time;
			}
			float k = 1f - Mathf.Exp(Time.fixedDeltaTime * (0f - Follow));
			for (int i = 0; i < Bones; i++)
			{
				Rigidbody bone = bones[i];
				if (!bone)
				{
					continue;
				}
				Vector3 at = Spot(root, i);
				Quaternion turn = NetworkPlayer.UnpackTurn(Pose[i * 3 + 2]);
				if (posed && !((at - bone.position).sqrMagnitude > Snap * Snap))
				{
					bone.MovePosition(Vector3.Lerp(bone.position, at, k));
					bone.MoveRotation(Quaternion.Slerp(bone.rotation, turn, k));
					continue;
				}
				bone.transform.SetPositionAndRotation(at, turn);
				if (posed)
				{
					bone.position = at;
					bone.rotation = turn;
				}
			}
			if (!posed)
			{
				puppet.gameObject.SetActive(true);
				Shun();
			}
			posed = true;
		}

		private bool LiveArm(int bone, int s)
		{
			if ((s & (Live | Out | Gone)) != Live)
			{
				return false;
			}
			bool left = bone == 4 || bone == 5;
			bool right = bone == 7 || bone == 8;
			if ((s & BothArms) != 0)
			{
				return left || right;
			}
			if ((s & RightArm) != 0)
			{
				return right;
			}
			return left;
		}

		private void Shun()
		{
			Player me = Player.main;
			foreach (Rigidbody rb in FindObjectsByType<Rigidbody>(FindObjectsSortMode.None))
			{
				if (!rb || rb.mass < Shunned || rb.transform.IsChildOf(transform) || Player.Of(rb) || rb.GetComponentInParent<NetworkNpc>() || (me && me.Holds(rb)))
				{
					continue;
				}
				rb.GetComponentsInChildren(true, shunCols);
				foreach (Collider col in shunCols)
				{
					if (col && col.attachedRigidbody == rb)
					{
						Ignore(col);
					}
				}
			}
		}

		private void Ignore(Collider col)
		{
			if (cols == null || !col)
			{
				return;
			}
			foreach (Collider own in cols)
			{
				if (own)
				{
					Physics.IgnoreCollision(own, col, true);
				}
			}
		}

		private void Touch(int bone, Collision c)
		{
			Player me = Player.main;
			Rigidbody other = c.rigidbody;
			if (other && !other.isKinematic && !Player.Of(other) && (!me || !me.Holds(other)))
			{
				Ignore(c.collider);
				return;
			}
			if (!me || me.dead || !c.rigidbody || Player.Of(c.rigidbody) != me)
			{
				return;
			}
			Rigidbody rb = c.rigidbody;
			Vector3 point = (c.contactCount > 0) ? c.GetContact(0).point : rb.position;
			if ((rb == me.leftLowerLeg || rb == me.rightLowerLeg) && me.Kicks(rb))
			{
				float speed = c.relativeVelocity.magnitude;
				if (speed >= 1.5f && Time.time > hitAt + HitGap && (State & Gone) == 0)
				{
					hitAt = Time.time;
					Vector3 velocity = rb.linearVelocity;
					RPC_Kicked(bone, speed, rb.position, new Vector3(velocity.x, 0f, velocity.z));
					Player.Note("kicked another player's " + (SlapDummy.Kind)Mathf.Clamp(Kind, 1, 4) + " on " + BoneNames[bone] + " at " + speed.ToString("F1") + " m/s");
				}
				return;
			}
			if (!me.Ragdolled() && (rb == me.leftHand || rb == me.rightHand || rb == me.leftForearm || rb == me.rightForearm))
			{
				Rigidbody hand = (rb == me.leftForearm) ? me.leftHand : ((rb == me.rightForearm) ? me.rightHand : rb);
				float swing = me.SwingSpeed(hand, 6);
				if (swing >= 3.5f && Time.time > hitAt + HitGap && (State & Gone) == 0)
				{
					hitAt = Time.time;
					me.LandPunch(swing);
					RPC_Struck(bone, swing * 3f, swing, hand.position);
					return;
				}
			}
			int s;
			int swingId;
			if (Time.time < liveUntil)
			{
				swingId = liveSwing;
				s = liveState;
			}
			else
			{
				s = State;
				swingId = Swing;
			}
			if (!LiveArm(bone, s) || (State & (Out | Gone)) != 0 || swingId == hitSwing)
			{
				return;
			}
			float hitSpeed = c.relativeVelocity.magnitude;
			if (!(hitSpeed < SlapMin))
			{
				hitSwing = swingId;
				Vector3 chest = me.chest.position;
				Vector3 center = bones[0] ? bones[0].position : point;
				Vector3 dir = new Vector3(chest.x - center.x, 0f, chest.z - center.z);
				dir = (dir.sqrMagnitude > 0.0001f) ? dir.normalized : (-c.relativeVelocity.normalized);
				Hit hit = SlapDummy.Smack((SlapDummy.Kind)Mathf.Clamp(Kind, 1, 4), me, rb, point, dir, (int)Object.Id.Raw);
				Player.Note("hit by another player's " + (SlapDummy.Kind)Mathf.Clamp(Kind, 1, 4) + " on " + rb.name + " at " + c.relativeVelocity.magnitude.ToString("F1") + " m/s -> " + hit + " | " + me.lastHit);
			}
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void RPC_Struck(int bone, float impulse, float speed, Vector3 from)
		{
			if (bone >= 0 && bone < Bones && dummy && bones[bone])
			{
				dummy.Struck(bones[bone], impulse, speed, from, null, true);
			}
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void RPC_Kicked(int bone, float speed, Vector3 from, Vector3 dir)
		{
			if (bone >= 0 && bone < Bones && dummy && bones[bone])
			{
				dummy.Kicked(bones[bone], speed, from, dir);
			}
		}
	}
}
