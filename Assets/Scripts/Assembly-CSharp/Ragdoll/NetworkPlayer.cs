using System.Collections.Generic;
using Fusion;
using Photon.Voice.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ragdoll
{
	public class NetworkPlayer : NetworkBehaviour
	{
		private struct PendingHit
		{
			public PlayerRef source;

			public int bone;

			public int verdict;

			public float impulse;

			public float speed;

			public float at;

			public float best;

			public Vector3 local;

			public Vector3 dir;

			public bool kick;

			public bool fist;
		}

		public const int Bones = 13;

		public static readonly string[] Names = new string[13] { "LowerTorso", "UpperTorso", "Head", "UpperArm.L", "LowerArm.L", "Hand.L", "UpperArm.R", "LowerArm.R", "Hand.R", "UpperLeg.L", "LowerLeg.L", "UpperLeg.R", "LowerLeg.R" };

		[Header("Contact")]
		public float hitPush = 1f;

		public float hitKick = 3f;

		public float kickFrom = 8f;

		public float shoveFrom = 300f;

		public float pushGain = 2f;

		private const float HandBackMax = 10f;

		public float pushMax = 250f;

		public float hangMax = 800f;

		public float pressSmooth = 0.05f;

		[Header("Forecast")]
		public float maxLead = 0.15f;

		public float maxShift = 0.4f;

		public float shiftRate = 1.5f;

		public float uprightFrom = 0.35f;

		public float uprightFull = 0.55f;

		public float uprightEase = 0.5f;

		public float riseBlend = 0.8f;

		[Header("Grab")]
		public float grabSpring = 600f;

		public float grabDamp = 60f;

		public float grabMax = 250f;

		public float liftMax = 150f;

		public float hangRamp = 0.3f;

		[Header("Fingers")]
		public float curlRate = 12f;

		[Header("Carry")]
		[Header("Cosmetics")]
		public Cosmetics cosmetics = new Cosmetics();

		[Header("Colors")]
		public Color[] colors;

		public static readonly HashSet<string> muted = new HashSet<string>();

		[HideInInspector]
		public float popAt = -9f;

		[HideInInspector]
		public bool mirroring;

		[HideInInspector]
		public int shown;

		[HideInInspector]
		public string lastHit = "";

		[HideInInspector]
		public bool carryShown;

		[HideInInspector]
		public Vector3[] carryPos = new Vector3[Bones];

		[HideInInspector]
		public Quaternion[] carryRot = new Quaternion[Bones];

		public static NetworkPlayer mine;

		public static NetworkPlayer testCarrier;

		public static bool carriedNow;

		private Transform[] bones;

		private NetworkString<_16> dressedHead;

		private NetworkString<_16> dressedTorso;

		private NetworkString<_16> dressedLegs;

		private NetworkString<_16> dressedFace;

		private int dressedPaint = -1;

		private bool wasBlab;

		private bool dressed;

		private Speaker speech;

		private AudioSource speechSource;

		private float speechBlend = 1f;

		private float speechVolume = 1f;

		private const float GhostVolume = 0.4f;

		private const float VoiceReach = 40f;

		private static readonly AnimationCurve VoiceFalloff = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.05f, 1f), new Keyframe(0.25f, 0.6f), new Keyframe(0.6f, 0.35f), new Keyframe(1f, 0.3f));

		private Transform[] localBones;

		private Player localBody;

		private Player fighter;

		private Player drown;

		private AvatarBody copy;

		private PropertyReader<Vector3> rootReader;

		private ArrayReader<int> poseReader;

		private ArrayReader<Vector3> carryPosReader;

		private ArrayReader<Quaternion> carryRotReader;

		private bool wasShownCarried;

		private readonly Vector3[] lyingPos = new Vector3[Bones];

		private readonly Quaternion[] lyingRot = new Quaternion[Bones];

		private bool wasLying;

		private float standUpAt = -9f;

		private float teleportGap = 2f;

		private AvatarBody releasedCopy;

		private AvatarBody lastCarried;

		private bool riding;

		private Vector3 carryFrom;

		private Vector3 carriedFrom;

		private Vector3 dropCheckPos;

		private AvatarBody dropCheck;

		private float dropCheckAt;

		private float settledSince = -1f;

		private float carryMax = 6f;

		private float carryRest = 0.4f;

		private float throwFrom = 2.5f;

		private Player glove;

		private Transform[] fingerBones;

		private Quaternion[] fingerOpen;

		private Quaternion[] fingerClosed;

		private bool[] fingerIsThumb;

		private bool[] fingerIsLeft;

		private Vector4 shownCurl;

		private float releasedAt;

		private int lastTick = -1;

		private Rigidbody[] localBodies;

		private HandGrab leftGrab;

		private HandGrab rightGrab;

		private Player rig;

		private Player localRagdoll;

		private Vector3 ownPull;

		private float hangBlend;

		private float nextLogAt;

		private KnockStars stars;

		private readonly Vector3[] showPos = new Vector3[Bones];

		private readonly Quaternion[] showRot = new Quaternion[Bones];

		private Vector3 rootVelocity;

		private Vector3 lastRootPos;

		private Vector3 forecastShift;

		private bool rootKnown;

		private float uprightBlend = 1f;

		private bool wasKnockedOut;

		private int seenDeaths;

		private Grabbable grabbable;

		private float anchorBlend;

		private Vector3 anchorShift;

		private float anchorRate = 4f;

		private float anchorSpeed = 6f;

		private Vector3 shownRoot;

		private bool shownRootKnown;

		private float kneeHeight = 0.3f;

		private Quaternion rigidFix = Quaternion.identity;

		private Vector3 rigidMove;

		private bool rigidHeld;

		private readonly NetPress[] outgoingPresses = new NetPress[3];

		private readonly NetPress[] outgoingTugs = new NetPress[2];

		private string tugNote;

		private float tugPeak;

		private Vector3 tugShove;

		private float tugFrom = 100f;

		private float tugSpring = 600f;

		public static readonly List<NetworkPlayer> avatars = new List<NetworkPlayer>();

		private static bool anchorGrips = true;

		private static bool latestPose = true;

		private readonly Vector3[] latestFrom = new Vector3[Bones];

		private readonly Quaternion[] latestFromRot = new Quaternion[Bones];

		private readonly Vector3[] latestTo = new Vector3[Bones];

		private readonly Quaternion[] latestToRot = new Quaternion[Bones];

		private readonly Vector3[] latestShown = new Vector3[Bones];

		private readonly Quaternion[] latestShownRot = new Quaternion[Bones];

		private Vector3 latestRoot;

		private int latestTick = -1;

		private float latestAt;

		private float latestSpan;

		private bool latestKnown;

		private float latestAlpha;

		private float latestGap;

		private float latestEvery = 0.03125f;

		private static float latestPredict = 0.1f;

		private static bool fusionSmooth = true;

		private static float fusionRate = 20f;

		private static float fusionCap = 0.15f;

		private const float MaxPredictSpeed = 8f;

		private readonly Vector3[] latestVel = new Vector3[Bones];

		private readonly Vector3[] latestCarryFrom = new Vector3[Bones];

		private readonly Quaternion[] latestCarryFromRot = new Quaternion[Bones];

		private readonly Vector3[] latestCarryTo = new Vector3[Bones];

		private readonly Quaternion[] latestCarryToRot = new Quaternion[Bones];

		private readonly Vector3[] latestCarryShown = new Vector3[Bones];

		private readonly Quaternion[] latestCarryShownRot = new Quaternion[Bones];

		private bool latestCarry;

		private static bool attachClimbers = true;

		private static float climbMassScale = 0f;

		private float climbHold = 400f;

		private readonly ConfigurableJoint[] climbJoints = new ConfigurableJoint[2];

		private readonly int[] climbBone = new int[2] { -1, -1 };

		private readonly Vector3[] climbLocal = new Vector3[2];

		private static bool reachAll = true;

		private static bool floorLift = true;

		public static float pressIn;

		public static float pressOut;

		public static float rtt;

		public static float viewLag;

		private static int statUpdates;

		private static int statTicks;

		private static int statLate;

		private static int statFrames;

		private static int statPredicted;

		private static float statWorstGap;

		private static float statFrom;

		public static int pressBone = -1;

		public static int taken;

		public static string lastTaken = "";

		private static bool yawAnchor = true;

		private Transform climbCore;

		private Vector3 climbRoot;

		private Quaternion climbTurn = Quaternion.identity;

		private const float ClimbSmooth = 0.06f;

		private Transform ghostHead;

		private Transform ghostL;

		private Transform ghostR;

		private Material ghostMat;

		private int ghostPaint = -1;

		private Material ghostFace;

		private MaterialPropertyBlock ghostBlock;

		private readonly List<Renderer> ghostParts = new List<Renderer>();

		private float ghostAlpha;

		private bool ghostTried;

		private readonly Vector3[] ghostBase = new Vector3[3] { Vector3.one, Vector3.one, Vector3.one };

		private const float GhostShow = 0.85f;

		private const float GhostFade = 0.4f;

		private const float GhostFollow = 18f;

		private TextMeshPro tagText;

		private float tagAlpha;

		private string tagName;

		private const float TagNear = 3.2f;

		private const float TagFar = 4f;

		private const float TagFade = 6f;

		private const float TagAbove = 0.62f;

		private bool shownWatching;

		private readonly List<Renderer> hidden = new List<Renderer>();

		private Player.Babble mouth;

		private AudioSource hipSound;

		private AudioSource handSoundL;

		private AudioSource handSoundR;

		private int heardJumps;

		private readonly List<PendingHit> pending = new List<PendingHit>();

		private float hitReach = 0.3f;

		private float hitLoose = 0.6f;

		private float hitWait = 0.35f;

		private float hitSlack = 0.15f;

		private float hitWaitMax = 1f;

		private float hitRewind = 0.25f;

		private float pushAt = -9f;

		private float pushed;

		private int pushBy = -1;

		private const int Past = 64;

		private readonly Matrix4x4[] pastPose = new Matrix4x4[Past * Bones];

		private readonly float[] pastAt = new float[Past];

		private int pastNext;

		[Networked]
		public Vector3 Root { get; set; }

		[Networked]
		[Capacity(39)]
		public NetworkArray<int> Pose => default;

		[Networked]
		[Capacity(3)]
		public NetworkArray<NetPress> Presses => default;

		[Networked]
		[Capacity(2)]
		public NetworkArray<NetPress> Tugs => default;

		[Networked]
		public int HitsTaken { get; set; }

		[Networked]
		public NetworkBool KnockedOut { get; set; }

		[Networked]
		public NetworkBool Dead { get; set; }

		[Networked]
		public int Deaths { get; set; }

		[Networked]
		public NetworkBool Watching { get; set; }

		[Networked]
		public float GhostSize { get; set; }

		[Networked]
		public Vector3 GhostHead { get; set; }

		[Networked]
		public Quaternion GhostHeadTurn { get; set; }

		[Networked]
		public Vector3 GhostL { get; set; }

		[Networked]
		public Quaternion GhostLTurn { get; set; }

		[Networked]
		public Vector3 GhostR { get; set; }

		[Networked]
		public Quaternion GhostRTurn { get; set; }

		[Networked]
		public NetworkId HoldL { get; set; }

		[Networked]
		public NetworkId HoldR { get; set; }

		[Networked]
		public Vector3 GripL { get; set; }

		[Networked]
		public Vector3 GripR { get; set; }

		[Networked]
		public NetworkBool Limp { get; set; }

		[Networked]
		public NetworkString<_16> Head { get; set; }

		[Networked]
		public NetworkString<_16> Torso { get; set; }

		[Networked]
		public NetworkString<_16> Legs { get; set; }

		[Networked]
		public NetworkString<_16> Face { get; set; }

		[Networked]
		public int Paint { get; set; }

		[Networked]
		public NetworkString<_16> Uid { get; set; }

		[Networked]
		public NetworkString<_16> Nick { get; set; }

		[Networked]
		public int Level { get; set; }

		[Networked]
		public float KoLeft { get; set; }

		[Networked]
		public NetworkBool GuardUp { get; set; }

		[Networked]
		public NetworkBool Talking { get; set; }

		[Networked]
		public NetworkBool Blabber { get; set; }

		[Networked]
		public int Jumps { get; set; }

		[Networked]
		public Vector4 Curl { get; set; }

		[Networked]
		public NetworkId CarryId { get; set; }

		[Networked]
		public int CarrySeq { get; set; }

		[Networked]
		public NetworkBool CarryDone { get; set; }

		[Networked]
		[Capacity(13)]
		public NetworkArray<Vector3> CarryVel => default;

		[Networked]
		public int TookBack { get; set; }

		[Networked]
		public int WantUp { get; set; }

		[Networked]
		public int Owed { get; set; }

		[Networked]
		[Capacity(13)]
		public NetworkArray<Vector3> CarryPos => default;

		[Networked]
		[Capacity(13)]
		public NetworkArray<Quaternion> CarryRot => default;

		public static bool HeldByOther(NetworkId id)
		{
			if (!id.IsValid)
			{
				return false;
			}
			foreach (NetworkPlayer a in avatars)
			{
				if (a && a != mine && a.Object && a.Object.IsValid && (a.HoldL == id || a.HoldR == id))
				{
					return true;
				}
			}
			return false;
		}

		public static bool Holds(PlayerRef who, NetworkId id)
		{
			if (!id.IsValid)
			{
				return false;
			}
			foreach (NetworkPlayer a in avatars)
			{
				if (a && a.Object && a.Object.IsValid && a.Object.StateAuthority == who && (a.HoldL == id || a.HoldR == id))
				{
					return true;
				}
			}
			return false;
		}

		private static NetworkId Holding(HandGrab g)
		{
			if (!g || !g.grabbing)
			{
				return default;
			}
			Component part = g.heldRope ? (Component)g.heldRope : g.held;
			NetworkObject obj = part ? part.GetComponentInParent<NetworkObject>() : null;
			if (!obj || !obj.IsValid)
			{
				return default;
			}
			return obj.Id;
		}

		private static Vector3 GripPoint(HandGrab g, Transform hand)
		{
			if (!g || !g.grabbing || g.heldRope || !g.held || !hand)
			{
				return Vector3.zero;
			}
			return g.held.transform.InverseTransformPoint(hand.position);
		}

		private void AttachClimber()
		{
			for (int i = 0; i < 2; i++)
			{
				NetPress press = Presses[i];
				int hand = i == 0 ? 5 : 8;
				bool off = true;
				if (attachClimbers && copy && copy.physical && copy.bodies != null && !copy.Held && mine && mine.Object && mine.localBodies != null && mine.localBones != null && press.grab && press.avatar == mine.Object.Id && press.bone >= 0 && press.bone <= 12 && mine.localBodies[press.bone])
				{
					off = KnockedOut || Dead;
				}
				ConfigurableJoint joint = climbJoints[i];
				if (off)
				{
					if (joint)
					{
						Destroy(joint);
					}
					climbJoints[i] = null;
					climbBone[i] = -1;
					if (copy)
					{
						copy.Pin(hand, false);
					}
					continue;
				}
				Rigidbody body = mine.localBodies[press.bone];
				if (joint && joint.connectedBody == body && climbBone[i] == press.bone && (climbLocal[i] - press.local).sqrMagnitude < 0.0004f)
				{
					continue;
				}
				if (joint)
				{
					Destroy(joint);
				}
				Rigidbody grip = copy.bodies[hand];
				Vector3 world = mine.localBones[press.bone].TransformPoint(press.local);
				ConfigurableJoint added = grip.gameObject.AddComponent<ConfigurableJoint>();
				added.connectedBody = body;
				added.autoConfigureConnectedAnchor = false;
				added.anchor = Vector3.zero;
				added.connectedAnchor = body.transform.InverseTransformPoint(world);
				added.xMotion = ConfigurableJointMotion.Free;
				added.yMotion = ConfigurableJointMotion.Free;
				added.zMotion = ConfigurableJointMotion.Free;
				JointDrive drive = new JointDrive
				{
					positionSpring = 20000f,
					positionDamper = 800f,
					maximumForce = climbHold
				};
				added.xDrive = drive;
				added.yDrive = drive;
				added.zDrive = drive;
				added.targetPosition = Vector3.zero;
				added.enableCollision = false;
				added.enablePreprocessing = false;
				added.connectedMassScale = climbMassScale;
				climbJoints[i] = added;
				climbBone[i] = press.bone;
				climbLocal[i] = press.local;
				copy.Pin(hand, true);
			}
		}

		private bool Latest()
		{
			Vector3 root = Root;
			if (root == Vector3.zero)
			{
				return false;
			}
			int tick = Object.LastReceiveTick;
			bool update = true;
			bool jump = true;
			if (latestKnown)
			{
				if (tick == latestTick)
				{
					update = false;
				}
				else
				{
					jump = (root - latestRoot).sqrMagnitude > teleportGap * teleportGap;
				}
			}
			if (update)
			{
				NetworkArrayReadOnly<int> pose = Pose.ToReadOnly();
				latestGap = latestKnown ? Mathf.Max(Runner.DeltaTime * (tick - latestTick), 0.001f) : 0f;
				for (int i = 0; i < Bones; i++)
				{
					Vector3 spot = Spot(root, pose, i);
					Quaternion turn = UnpackTurn(pose[i * 3 + 2]);
					if (jump)
					{
						latestVel[i] = Vector3.zero;
					}
					else
					{
						latestVel[i] = Vector3.ClampMagnitude((spot - latestTo[i]) / latestGap, MaxPredictSpeed);
					}
					latestTo[i] = spot;
					latestToRot[i] = turn;
					latestFrom[i] = jump ? latestTo[i] : latestShown[i];
					latestFromRot[i] = jump ? latestToRot[i] : latestShownRot[i];
				}
				if (latestKnown && !jump)
				{
					float gap = Time.time - latestAt;
					latestEvery += (Mathf.Clamp(gap, 0.01f, 0.12f) - latestEvery) * 0.2f;
					statUpdates++;
					statTicks += tick - latestTick;
					statWorstGap = Mathf.Max(statWorstGap, gap);
					if (gap > 0.08f)
					{
						statLate++;
					}
				}
				Vector3 carried = CarryPos[0];
				bool carryJump = !latestCarry || carried == Vector3.zero || (carried - latestCarryTo[0]).sqrMagnitude > teleportGap * teleportGap;
				bool snap = jump || carryJump;
				for (int i = 0; i < Bones; i++)
				{
					Vector3 pos = CarryPos[i];
					Quaternion rot = CarryRot[i];
					latestCarryFrom[i] = snap ? pos : latestCarryShown[i];
					latestCarryFromRot[i] = snap ? rot : latestCarryShownRot[i];
					latestCarryTo[i] = pos;
					latestCarryToRot[i] = rot;
				}
				latestCarry = carried != Vector3.zero;
				latestSpan = jump ? 0f : Mathf.Clamp(Mathf.Max(latestEvery, Runner.DeltaTime * (tick - latestTick)), 0.01f, 0.12f);
				latestRoot = root;
				latestTick = tick;
				latestAt = Time.time;
				latestKnown = true;
				shown++;
			}
			float age = Time.time - latestAt;
			float ahead;
			if (latestSpan > 0f)
			{
				latestAlpha = Mathf.Clamp01(age / latestSpan);
				ahead = Mathf.Clamp(age - latestSpan, 0f, latestPredict);
			}
			else
			{
				latestAlpha = 1f;
				ahead = 0f;
			}
			statFrames++;
			if (ahead > 0f)
			{
				statPredicted++;
			}
			if (fusionSmooth)
			{
				ahead = Mathf.Min(age, fusionCap);
				float k = latestSpan > 0f ? 1f - Mathf.Exp(-fusionRate * Time.deltaTime) : 1f;
				for (int i = 0; i < Bones; i++)
				{
					latestShown[i] = Vector3.Lerp(latestShown[i], latestTo[i] + latestVel[i] * ahead, k);
					latestShownRot[i] = Quaternion.Slerp(latestShownRot[i], latestToRot[i], k);
					showPos[i] = latestShown[i];
					showRot[i] = latestShownRot[i];
				}
				viewLag = 1f / fusionRate + rtt * 0.5f;
			}
			else
			{
				for (int i = 0; i < Bones; i++)
				{
					latestShown[i] = Vector3.Lerp(latestFrom[i], latestTo[i], latestAlpha) + latestVel[i] * ahead;
					latestShownRot[i] = Quaternion.Slerp(latestFromRot[i], latestToRot[i], latestAlpha);
					showPos[i] = latestShown[i];
					showRot[i] = latestShownRot[i];
				}
				viewLag = (1f - latestAlpha) * latestSpan + (age + rtt * 0.5f - ahead);
			}
			return true;
		}

		private bool HeldBody(int slot, out Vector3 point)
		{
			point = Vector3.zero;
			if (!anchorGrips)
			{
				return false;
			}
			NetworkId id = slot == 0 ? HoldL : HoldR;
			Vector3 grip = slot == 0 ? GripL : GripR;
			if (!id.IsValid || grip == Vector3.zero)
			{
				return false;
			}
			if (!Runner.TryFindObject(id, out NetworkObject obj) || !obj)
			{
				return false;
			}
			if (!obj.GetComponent<NetRope>() || obj.GetComponent<Rope>() || obj.GetComponent<NetworkPlayer>())
			{
				return false;
			}
			point = obj.transform.TransformPoint(grip);
			return true;
		}

		public override void Spawned()
		{
			if (HasStateAuthority)
			{
				Watching = GameManager.watching || GameManager.Outsider;
			}
			if (HasStateAuthority && CarrySeq == 0)
			{
				CarrySeq = (Random.Range(1, 1000) + Object.StateAuthority.PlayerId * 1000) * 1000;
			}
			seenDeaths = Deaths;
			bones = Bind(transform);
			foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
			{
				r.enabled = !HasStateAuthority;
			}
			copy = GetComponent<AvatarBody>();
			if (copy)
			{
				copy.SetPhysical(!HasStateAuthority);
				copy.onHit = SendHit;
			}
			name = "Avatar " + Object.StateAuthority;
			if (!HasStateAuthority && Watching)
			{
				Unseen();
			}
			rootReader = GetPropertyReader<Vector3>(nameof(Root));
			poseReader = GetArrayReader<int>(nameof(Pose));
			carryPosReader = GetArrayReader<Vector3>(nameof(CarryPos));
			carryRotReader = GetArrayReader<Quaternion>(nameof(CarryRot));
			avatars.Add(this);
			grabbable = GetComponent<Grabbable>();
			if (HasStateAuthority)
			{
				mine = this;
				Uid = GameManager.MyId;
				Nick = NetworkManager.MetaName;
				DiscordLog.Joined(Runner);
				NetworkManager manager = FindFirstObjectByType<NetworkManager>();
				localBody = manager && manager.localBody ? manager.localBody : null;
				if (localBody)
				{
					localBones = Bind(localBody.transform);
					fighter = localBody.GetComponent<Player>();
					drown = localBody.GetComponent<Player>();
					glove = localBody.GetComponent<Player>();
				}
				if (localBones != null)
				{
					localBodies = new Rigidbody[Bones];
					for (int i = 0; i < Bones; i++)
					{
						localBodies[i] = localBones[i].GetComponent<Rigidbody>();
					}
					leftGrab = localBones[5].GetComponent<HandGrab>();
					rightGrab = localBones[8].GetComponent<HandGrab>();
					localRagdoll = localBody.GetComponent<Player>();
					rig = localRagdoll ? localRagdoll : null;
				}
				mirroring = localBones != null;
				return;
			}
			stars = gameObject.AddComponent<KnockStars>();
			stars.head = bones != null ? bones[2] : null;
			if (bones != null)
			{
				FistTrail trail = gameObject.AddComponent<FistTrail>();
				trail.fists = new Transform[] { bones[5], bones[8] };
				trail.body = bones[1];
				trail.grip = i => i == 0 ? shownCurl.x : shownCurl.z;
				trail.limp = () => Limp || KnockedOut || Dead;
				trail.onSwing = (i, fast) =>
				{
					if (Player.main)
					{
						Player.main.SwingSound(i == 0 ? handSoundL : handSoundR, fast);
					}
				};
			}
			BindFingers();
		}

		private static int Mm(float metres)
		{
			return Mathf.Clamp(Mathf.RoundToInt(metres * 1000f), -32767, 32767);
		}

		private static Vector3 Spot(Vector3 root, NetworkArrayReadOnly<int> pose, int bone)
		{
			if (bone == 0)
			{
				return root;
			}
			int xy = pose[bone * 3];
			int z = pose[bone * 3 + 1];
			return new Vector3(root.x + (short)xy * 0.001f, root.y + (xy >> 16) * 0.001f, root.z + z * 0.001f);
		}

		public static int PackTurn(Quaternion q)
		{
			float ax = Mathf.Abs(q.x);
			float ay = Mathf.Abs(q.y);
			float az = Mathf.Abs(q.z);
			float aw = Mathf.Abs(q.w);
			int big = 0;
			float most = ax;
			if (ay > most)
			{
				big = 1;
				most = ay;
			}
			if (az > most)
			{
				big = 2;
				most = az;
			}
			if (aw > most)
			{
				big = 3;
			}
			float lead = big == 0 ? q.x : big == 1 ? q.y : big == 2 ? q.z : q.w;
			float sign = lead < 0f ? -1f : 1f;
			float a = big == 0 ? q.y : q.x;
			float b = big < 2 ? q.z : q.y;
			float c = big == 3 ? q.z : q.w;
			return (Ten(a * sign) << 20) | (big << 30) | (Ten(b * sign) << 10) | Ten(c * sign);
		}

		public static Quaternion UnpackTurn(int packed)
		{
			int big = (packed >> 30) & 3;
			float a = Unten(packed >> 20);
			float b = Unten(packed >> 10);
			float c = Unten(packed);
			float d = Mathf.Sqrt(Mathf.Max(0f, 1f - a * a - b * b - c * c));
			switch (big)
			{
				case 0:
					return new Quaternion(d, a, b, c);
				case 1:
					return new Quaternion(a, d, b, c);
				case 2:
					return new Quaternion(a, b, d, c);
				default:
					return new Quaternion(a, b, c, d);
			}
		}

		public static int PackSpeed(Vector3 v)
		{
			float most = Mathf.Max(Mathf.Abs(v.x), Mathf.Max(Mathf.Abs(v.y), Mathf.Abs(v.z)));
			int range = most < 5.1f ? 0 : most < 10.2f ? 1 : most < 20.4f ? 2 : 3;
			float scale = 100f / (1 << range);
			return (Notch(v.x * scale) << 20) | (range << 30) | (Notch(v.y * scale) << 10) | Notch(v.z * scale);
		}

		public static Vector3 UnpackSpeed(int packed)
		{
			float scale = (1 << ((packed >> 30) & 3)) * 0.01f;
			return new Vector3(scale * (((packed >> 20) & 0x3FF) - 512), scale * (((packed >> 10) & 0x3FF) - 512), scale * ((packed & 0x3FF) - 512));
		}

		private static int Notch(float v)
		{
			return Mathf.Clamp(Mathf.RoundToInt(v), -511, 511) + 512;
		}

		private static int Ten(float v)
		{
			return Mathf.Clamp(Mathf.RoundToInt((v * 1.4142135f + 1f) * 511.5f), 0, 1023);
		}

		private static float Unten(int bits)
		{
			return ((bits & 0x3FF) / 511.5f - 1f) * 0.70710677f;
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			if (mine == this)
			{
				DiscordLog.Left(runner);
			}
			avatars.Remove(this);
			foreach (Transform part in new Transform[] { ghostHead, ghostL, ghostR })
			{
				if (part)
				{
					Destroy(part.gameObject);
				}
			}
			if (ghostMat)
			{
				Destroy(ghostMat);
			}
			if (ghostFace)
			{
				Destroy(ghostFace);
			}
			if (rig)
			{
				rig.shoveForce = Vector3.zero;
				rig.tether = Vector3.zero;
			}
		}

		public override void FixedUpdateNetwork()
		{
			if (!HasStateAuthority || localBones == null)
			{
				return;
			}
			HoldL = Holding(leftGrab);
			HoldR = Holding(rightGrab);
			GripL = GripPoint(leftGrab, localBones[5]);
			GripR = GripPoint(rightGrab, localBones[8]);
			Vector3 root = localBones[0].position;
			Root = root;
			for (int i = 0; i < Bones; i++)
			{
				if (i != 0)
				{
					Vector3 offset = localBones[i].position - root;
					Pose.Set(i * 3, (Mm(offset.x) & 0xFFFF) | (Mm(offset.y) << 16));
					Pose.Set(i * 3 + 1, Mm(offset.z));
				}
				Pose.Set(i * 3 + 2, PackTurn(localBones[i].rotation));
			}
			Player body = NetworkManager.main ? NetworkManager.main.localBody : null;
			Vector3 headAt = Vector3.zero;
			Vector3 leftAt = Vector3.zero;
			Vector3 rightAt = Vector3.zero;
			float size = 0f;
			Quaternion headTurn = Quaternion.identity;
			Quaternion leftTurn = Quaternion.identity;
			Quaternion rightTurn = Quaternion.identity;
			bool ghost = body && body.GhostPose(out headAt, out headTurn, out leftAt, out leftTurn, out rightAt, out rightTurn, out size);
			GhostSize = ghost ? size : 0f;
			if (ghost)
			{
				GhostHead = headAt;
				GhostHeadTurn = headTurn;
				GhostL = leftAt;
				GhostLTurn = leftTurn;
				GhostR = rightAt;
				GhostRTurn = rightTurn;
			}
			for (int i = 0; i < outgoingPresses.Length; i++)
			{
				Presses.Set(i, outgoingPresses[i]);
			}
			for (int i = 0; i < outgoingTugs.Length; i++)
			{
				Tugs.Set(i, outgoingTugs[i]);
			}
			AvatarBody carried = CarriedIn(leftGrab);
			if (!carried)
			{
				carried = CarriedIn(rightGrab);
			}
			if (carried && !lastCarried)
			{
				carryFrom = carried.bodies[0].position;
				if (releasedCopy != carried)
				{
					CarrySeq++;
					CarryDone = false;
				}
				Player.Note("carrying " + carried.name + " from " + carryFrom.ToString("F1") + " (carry " + CarrySeq + ")");
			}
			if (!carried && lastCarried)
			{
				NetworkPlayer dropped = lastCarried.GetComponent<NetworkPlayer>();
				Vector3 at = lastCarried.bodies[0].position;
				Vector3 velocity = lastCarried.bodies[0].linearVelocity;
				Vector3 ownVelocity = localBodies[0] ? localBodies[0].linearVelocity : Vector3.zero;
				float handSpeed = (velocity - ownVelocity).magnitude;
				Player.Note("dropped " + lastCarried.name + " at " + at.ToString("F1") + " after moving it " + (at - carryFrom).magnitude.ToString("F1") + " m, leaving my hand at " + handSpeed.ToString("F1") + " m/s" + (dropped && dropped.KnockedOut ? "" : " (it woke up)"));
				if (handSpeed < throwFrom && lastCarried == releasedCopy)
				{
					CarryDone = true;
					Player.Note("handing " + lastCarried.name + " back at " + at.ToString("F1") + " (let go, not thrown)");
				}
			}
			lastCarried = carried;
			if (carried)
			{
				if (releasedCopy && releasedCopy != carried)
				{
					releasedCopy.streamed = false;
				}
				releasedCopy = carried;
				releasedCopy.streamed = true;
				releasedAt = Time.time;
				settledSince = -1f;
				NetworkPlayer held = carried.GetComponent<NetworkPlayer>();
				if (held && held.Object && held.Object.IsValid && held.WantUp == CarrySeq)
				{
					Player.Note(carried.name + " wants up, letting go");
					carried.DropCarry();
				}
			}
			else if (releasedCopy)
			{
				NetworkPlayer other = releasedCopy.GetComponent<NetworkPlayer>();
				bool tookBack = other && other.Object && other.Object.IsValid && other.TookBack == CarrySeq;
				if (tookBack || !(other && other.Object && other.Object.IsValid))
				{
					Vector3 at = releasedCopy.bodies[0].position;
					if (tookBack)
					{
						Player.Note(releasedCopy.name + " took its body back at " + at.ToString("F1") + ", " + (at - carryFrom).magnitude.ToString("F1") + " m from the pickup, its own body " + (other.Root - at).magnitude.ToString("F1") + " m from my copy");
						dropCheck = releasedCopy;
						dropCheckPos = at;
						dropCheckAt = Time.time + 2.5f;
						releasedCopy.Rejoin();
					}
					releasedCopy.streamed = false;
					releasedCopy = null;
				}
				else if (!CarryDone)
				{
					if (!Settled(releasedCopy))
					{
						releasedCopy.Free();
						settledSince = -1f;
					}
					else if (settledSince < 0f)
					{
						settledSince = Time.time;
					}
					bool rested = settledSince >= 0f && Time.time > settledSince + 0.3f;
					if (rested || other.WantUp == CarrySeq || Time.time > releasedAt + carryMax)
					{
						CarryDone = true;
						Player.Note("handing " + releasedCopy.name + " back at " + releasedCopy.bodies[0].position.ToString("F1") + (rested ? " (came to rest)" : other.WantUp == CarrySeq ? " (it wants up)" : " (carry timeout)"));
					}
				}
			}
			if (carried)
			{
				CarryDone = false;
			}
			NetworkPlayer streamed = releasedCopy ? releasedCopy.GetComponent<NetworkPlayer>() : null;
			if (streamed && !streamed.Object)
			{
				streamed = null;
			}
			CarryId = streamed ? streamed.Object.Id : default;
			if (streamed || CarryPos[0] != Vector3.zero)
			{
				for (int i = 0; i < Bones; i++)
				{
					CarryPos.Set(i, streamed ? releasedCopy.bodies[i].position : Vector3.zero);
					CarryRot.Set(i, streamed ? releasedCopy.bodies[i].rotation : Quaternion.identity);
					CarryVel.Set(i, streamed ? releasedCopy.bodies[i].linearVelocity : Vector3.zero);
				}
			}
			rtt = (float)Runner.GetPlayerRtt(Runner.LocalPlayer);
			if (Time.time > nextLogAt && avatars.Count >= 2)
			{
				nextLogAt = Time.time + 2f;
				float now = Time.time;
				float elapsed = Mathf.Max(now - statFrom, 0.1f);
				int others = avatars.Count - 1;
				Debug.Log("[Net] rtt " + (rtt * 1000f).ToString("F0") + " ms, other players shown " + (viewLag * 1000f).ToString("F0") + " ms behind, " + (statUpdates / (elapsed * others)).ToString("F1") + " updates/s each, " + (statUpdates < 1 ? 0f : (float)statTicks / statUpdates).ToString("F2") + " ticks per update, " + statLate + " gaps over 80 ms, worst gap " + (statWorstGap * 1000f).ToString("F0") + " ms, predicting " + (statFrames < 1 ? 0f : statPredicted * 100f / statFrames).ToString("F0") + "% of frames");
				statFrom = Time.time;
				statUpdates = 0;
				statTicks = 0;
				statLate = 0;
				statFrames = 0;
				statPredicted = 0;
				statWorstGap = 0f;
			}
			if (fighter)
			{
				KnockedOut = fighter.knockedOut;
			}
			if (fighter)
			{
				Limp = fighter.Ragdolled();
			}
			if (fighter)
			{
				Talking = fighter.talking;
				Blabber = Prefs.myBlabber;
				Jumps = fighter.jumpCount;
			}
			if (fighter)
			{
				KoLeft = fighter.Shielded() ? 0f : fighter.Threshold() - fighter.damage;
			}
			if (fighter)
			{
				GuardUp = fighter.guardLeft > 0;
			}
			if (glove)
			{
				Curl = new Vector4(glove.gripL, glove.thumbL, glove.gripR, glove.thumbR);
			}
			if (drown)
			{
				Dead = drown.drowned;
			}
			if (drown)
			{
				Deaths = drown.deaths;
			}
			Watching = GameManager.watching || GameManager.Outsider;
			Owed = GameManager.Owed();
			int level = Account.Ready ? Account.Level : 0;
			if (Level != level)
			{
				Level = level;
			}
			if (!localBody)
			{
				return;
			}
			string[] worn = localBody.cosmetics.worn;
			if (Head != worn[0])
			{
				Head = worn[0];
			}
			if (Torso != worn[1])
			{
				Torso = worn[1];
			}
			if (Legs != worn[2])
			{
				Legs = worn[2];
			}
			if (Face != worn[3])
			{
				Face = worn[3];
			}
			if (Paint == 0 || Clash())
			{
				Paint = FreePaint();
			}
			else if (colors != null && Prefs.color >= 1 && Prefs.color <= colors.Length && Paint != Prefs.color && !Taken(Prefs.color) && !GameManager.Claimed(Prefs.color))
			{
				Paint = Prefs.color;
			}
			if (localBody.cosmetics.color != Colour(Shown()))
			{
				localBody.cosmetics.Tint(localBody.transform, Colour(Shown()));
			}
		}

		private int FreePaint()
		{
			if (colors == null || colors.Length == 0)
			{
				return 0;
			}
			int count = colors.Length;
			int saved = GameManager.PaintOf(GameManager.MyId);
			if (saved >= 1 && saved <= count && !Taken(saved))
			{
				return saved;
			}
			if (Prefs.color >= 1 && Prefs.color <= count && !Taken(Prefs.color) && !GameManager.Claimed(Prefs.color))
			{
				return Prefs.color;
			}
			int seed = Runner.SessionInfo.IsValid ? Runner.SessionInfo.Name.GetHashCode() : 0;
			int me = Object.StateAuthority.PlayerId;
			int start = (seed + me * 7) % count;
			for (int pass = 0; pass < 2; pass++)
			{
				for (int k = 0; k < count; k++)
				{
					int paint = (start + k) % count + 1;
					if (!Taken(paint) && (pass == 1 || !GameManager.Claimed(paint)))
					{
						return paint;
					}
				}
			}
			return start + 1;
		}

		public static bool TakenByOthers(int paint)
		{
			foreach (NetworkPlayer a in avatars)
			{
				if (a && a.Object && a.Object.IsValid && !a.HasStateAuthority && a.Paint == paint)
				{
					return true;
				}
			}
			return false;
		}

		private bool Taken(int paint)
		{
			foreach (NetworkPlayer a in avatars)
			{
				if (a != this && a && a.Object && a.Object.IsValid && a.Paint == paint && (Owns(a, paint) || !Owns(this, paint)))
				{
					return true;
				}
			}
			return false;
		}

		private static bool Owns(NetworkPlayer a, int paint)
		{
			if (paint < 1 || a.Uid.Length < 1)
			{
				return false;
			}
			return GameManager.PaintOf(a.Uid.Value) == paint;
		}

		private bool Clash()
		{
			foreach (NetworkPlayer a in avatars)
			{
				if (a != this && a && a.Object && a.Object.IsValid && a.Paint == Paint)
				{
					if (Owns(a, Paint) != Owns(this, Paint))
					{
						if (Owns(a, Paint))
						{
							return true;
						}
					}
					else if (a.Object.StateAuthority.PlayerId < Object.StateAuthority.PlayerId)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void Dress()
		{
			if (!dressed)
			{
				dressed = true;
				for (int i = 0; i < Cosmetics.Slots.Length; i++)
				{
					cosmetics.Wear(i, "");
				}
			}
			if (Head != dressedHead)
			{
				dressedHead = Head;
				cosmetics.Wear(0, Head.Value);
			}
			if (Torso != dressedTorso)
			{
				dressedTorso = Torso;
				cosmetics.Wear(1, Torso.Value);
			}
			if (Legs != dressedLegs)
			{
				dressedLegs = Legs;
				cosmetics.Wear(2, Legs.Value);
			}
			if (Face != dressedFace)
			{
				dressedFace = Face;
				cosmetics.Wear(3, Face.Value);
			}
			if (Shown() != dressedPaint)
			{
				dressedPaint = Shown();
				cosmetics.Tint(transform, Colour(dressedPaint));
			}
		}

		private int Shown()
		{
			int team = GameManager.TeamOf(Uid.Value);
			return team > 0 ? GameManager.TeamPaint(team) : Paint;
		}

		private static string UidOf(PlayerRef who)
		{
			foreach (NetworkPlayer a in avatars)
			{
				if (a && a.Object && a.Object.IsValid && a.Object.StateAuthority == who)
				{
					return a.Uid.Value;
				}
			}
			return "";
		}

		public Color Colour(int paint)
		{
			if (colors == null || paint < 1 || colors.Length == 0)
			{
				return Color.white;
			}
			return colors[(paint - 1) % colors.Length];
		}

		public Transform HeadBone()
		{
			return bones != null ? bones[2] : null;
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void RPC_Wear(string item)
		{
			if (localBody)
			{
				localBody.HatOn(item);
			}
		}

		private void FixedUpdate()
		{
			if (localBones == null || !Object || !Object.IsValid)
			{
				return;
			}
			Remember();
			if (pending.Count > 0)
			{
				SettleHits();
			}
			NetworkPlayer carrier = testCarrier ? testCarrier : FindCarrier(Object.Id);
			if (carrier && carrier.CarrySeq == TookBack)
			{
				carrier = null;
			}
			if (carrier && fighter && fighter.knockedOut && Time.time >= fighter.knockedOutUntil && WantUp != carrier.CarrySeq)
			{
				WantUp = carrier.CarrySeq;
				Player.Note("knockout over while carried by " + carrier.name + ", asking to be let go");
			}
			if (carrier && fighter && !fighter.knockedOut)
			{
				WantUp = carrier.CarrySeq;
				TookBack = carrier.CarrySeq;
				if (riding)
				{
					foreach (Rigidbody body in localBodies)
					{
						if (body)
						{
							body.linearVelocity = Vector3.zero;
						}
					}
				}
				Player.Note("awake while " + carrier.name + " carries me, keeping my body");
				carrier = null;
			}
			if (fighter)
			{
				fighter.carriedHold = carrier;
			}
			if (carrier && carrier.CarryDone)
			{
				TakeBack(carrier);
			}
			else if (carrier)
			{
				Ride(carrier);
			}
			else
			{
				Unride();
			}
			if (carriedNow != (bool)carrier && localBodies[0])
			{
				string note;
				if (carrier)
				{
					carriedFrom = localBodies[0].position;
					note = "being carried by " + carrier.name + " from " + carriedFrom.ToString("F1");
				}
				else
				{
					note = "carry ended, my pelvis at " + localBodies[0].position.ToString("F1") + ", moved " + (localBodies[0].position - carriedFrom).magnitude.ToString("F1") + " m while carried" + (fighter && fighter.knockedOut ? "" : " (woke up)");
				}
				Player.Note(note);
			}
			carriedNow = carrier;
			if (dropCheck && Time.time > dropCheckAt)
			{
				float moved = (dropCheck.bodies[0].position - dropCheckPos).magnitude;
				Player.Note(dropCheck.name + " 2.5 s later is " + moved.ToString("F1") + " m from where it came to rest" + (moved > 1f ? ", SNAPPED BACK" : ""));
				dropCheck = null;
			}
			GatherPresses();
			GatherTugs();
			ApplyTugs();
			Vector3 pulls = Vector3.zero;
			Vector3 pushes = Vector3.zero;
			if ((!drown || !drown.drowned) && !AvatarBody.peaceful && !AvatarBody.truce)
			{
				pushes = ApplyPresses(out pulls) - pulls;
			}
			Vector3 flat = new Vector3(pushes.x, 0f, pushes.z);
			float flatSize = flat.magnitude;
			if (rig)
			{
				Vector3 tugFlat = new Vector3(tugShove.x, 0f, tugShove.z);
				float tugSize = tugFlat.magnitude;
				Vector3 shove = flatSize > shoveFrom ? flat * ((flatSize - shoveFrom) / flatSize) : Vector3.zero;
				Vector3 tug = tugSize > tugFrom ? tugFlat * ((tugSize - tugFrom) / tugSize) : Vector3.zero;
				rig.shoveForce = new Vector3(ownPull.x, 0f, ownPull.z) + (shove + new Vector3(pulls.x, 0f, pulls.z)) + tug;
			}
			if (rig)
			{
				rig.tether = new Vector3(tugShove.x, 0f, tugShove.z) / grabMax;
			}
		}

		private void GatherPresses()
		{
			float keep = Mathf.Exp(-Time.fixedDeltaTime / Mathf.Max(pressSmooth, 0.001f));
			ownPull = Vector3.zero;
			if ((fighter && (fighter.knockedOut || fighter.dead)) || (drown && drown.drowned))
			{
				for (int i = 0; i < outgoingPresses.Length; i++)
				{
					outgoingPresses[i] = default;
				}
				pressOut = 0f;
				return;
			}
			AvatarBody.AvatarHit left = HeldPart(leftGrab);
			AvatarBody.AvatarHit right = HeldPart(rightGrab);
			int hands = (left ? 1 : 0) + (right ? 1 : 0);
			float hang = hands > 0 && localRagdoll && localRagdoll.hanging && !localRagdoll.grounded ? 1f : 0f;
			hangBlend = Mathf.MoveTowards(hangBlend, hang, Time.fixedDeltaTime / Mathf.Max(hangRamp, 0.01f));
			float weight = 0f;
			if (hands > 0)
			{
				weight = hangBlend * Mathf.Min(-Physics.gravity.y * localBody.TotalMass() / hands, hangMax);
			}
			Transform targetL = null;
			if (localRagdoll)
			{
				targetL = localRagdoll.handTargetL;
			}
			PressWithHand(0, leftGrab, left, localBones[5], targetL, weight, 1f - keep);
			Transform targetR = null;
			if (localRagdoll)
			{
				targetR = localRagdoll.handTargetR;
			}
			PressWithHand(1, rightGrab, right, localBones[8], targetR, weight, 1f - keep);
			ownPull = Vector3.ClampMagnitude(ownPull, hangMax);
			AvatarBody.FistWatch best = null;
			for (int i = 0; i < Bones; i++)
			{
				if (i == 5 || i == 8)
				{
					continue;
				}
				AvatarBody.FistWatch watch = localBones[i].GetComponent<AvatarBody.FistWatch>();
				if (IsFreshTouch(watch) && (!best || watch.push.sqrMagnitude > best.push.sqrMagnitude))
				{
					best = watch;
				}
			}
			if (best)
			{
				SetPress(2, best.touching, best.local, best.push * pushGain, pushMax, 1f - keep, false, Vector3.zero);
			}
			else
			{
				SetPress(2, null, Vector3.zero, Vector3.zero, pushMax, 1f - keep, false, Vector3.zero);
			}
			pressOut = 0f;
			foreach (NetPress press in outgoingPresses)
			{
				pressOut = Mathf.Max(pressOut, press.force.magnitude);
			}
		}

		private void GatherTugs()
		{
			tugShove = Vector3.zero;
			for (int i = 0; i < outgoingTugs.Length; i++)
			{
				HandGrab grab = i == 0 ? leftGrab : rightGrab;
				Transform hand = null;
				if (localRagdoll)
				{
					hand = i == 0 ? localRagdoll.handTargetL : localRagdoll.handTargetR;
				}
				NetProp prop = null;
				if (grab && grab.tugging && grab.held)
				{
					prop = grab.held.GetComponent<NetProp>();
				}
				NetPress press = default;
				if (prop && hand && prop.Object && prop.Object.IsValid && !prop.HasStateAuthority)
				{
					press = new NetPress
					{
						avatar = prop.Object.Id,
						bone = -1,
						local = grab.TugAnchor,
						target = hand.position,
						grab = true
					};
					tugShove += TugPull(prop.transform.TransformPoint(grab.TugAnchor) - hand.position, Vector3.zero, liftMax);
				}
				outgoingTugs[i] = press;
			}
		}

		private void ApplyTugs()
		{
			float total = 0f;
			string note = null;
			foreach (NetworkPlayer a in avatars)
			{
				if (a == this || !a.Object || !a.Object.IsValid)
				{
					continue;
				}
				for (int i = 0; i < a.Tugs.Length; i++)
				{
					NetPress tug = a.Tugs[i];
					if (!tug.grab || !Runner.TryFindObject(tug.avatar, out NetworkObject obj) || !obj)
					{
						continue;
					}
					NetProp prop = obj.GetComponent<NetProp>();
					Rigidbody body = prop && prop.HasStateAuthority ? prop.GetComponent<Rigidbody>() : null;
					if (!body)
					{
						continue;
					}
					Vector3 at = prop.transform.TransformPoint(tug.local);
					Vector3 pull = TugPull(tug.target - at, body.GetPointVelocity(at), Mathf.Min(liftMax, body.mass * Physics.gravity.y * -0.5f));
					body.AddForceAtPosition(pull, at, ForceMode.Force);
					if ((leftGrab && leftGrab.held == body) || (rightGrab && rightGrab.held == body))
					{
						tugShove += pull;
					}
					total += pull.magnitude;
					note = prop.name + " by player " + a.Object.StateAuthority.PlayerId;
				}
			}
			if (note == null)
			{
				if (tugNote != null)
				{
					Player.Note("tug on " + tugNote + " ended, peak " + tugPeak.ToString("F0") + " N");
				}
				tugPeak = 0f;
			}
			else
			{
				if (tugNote == null)
				{
					Player.Note("tugged on " + note);
				}
				tugPeak = Mathf.Max(tugPeak, total);
			}
			tugNote = note;
		}

		private void PressWithHand(int slot, HandGrab grab, AvatarBody.AvatarHit held, Transform hand, Transform target, float weight, float smoothing)
		{
			if (!held || !target)
			{
				AvatarBody.FistWatch watch = hand.GetComponent<AvatarBody.FistWatch>();
				bool swinging = localRagdoll && localRagdoll.SwingSpeed(localBodies[slot == 0 ? 5 : 8], 6) >= 3.5f;
				if (!swinging && IsFreshTouch(watch))
				{
					SetPress(slot, watch.touching, watch.local, watch.push * pushGain, pushMax, smoothing, false, Vector3.zero);
				}
				else
				{
					SetPress(slot, null, Vector3.zero, Vector3.zero, pushMax, smoothing, false, Vector3.zero);
				}
				return;
			}
			NetworkPlayer other = RemoteAvatar(held);
			bool theyGrab = other.IsGrabbing(Object.Id);
			NetPress was = outgoingPresses[slot];
			bool same = was.grab && was.avatar == other.Object.Id;
			bool first = same ? (bool)was.first : !theyGrab;
			bool yields = theyGrab && !Keeps(other, first);
			if (!same && grab.holdsYou)
			{
				Player.Note((slot == 0 ? "left" : "right") + " hand climbing " + other.name + " by " + held.name + (theyGrab ? ", they were already holding me" : ", grabbed first"));
			}
			if (yields && !grab.mutual)
			{
				Player.Note("holding " + other.name + " while they hold me, they grabbed first so my grip only pulls now");
			}
			grab.Mutual(yields);
			Vector3 aim = Vector3.Lerp(target.position, grab.transform.position, hangBlend);
			if (grab.holdsYou && !yields)
			{
				ownPull -= GrabPull(aim - grab.transform.position, Vector3.zero, grabMax, liftMax);
			}
			Vector3 pin = grab.holdsYou ? grab.PinAt : grab.transform.position;
			Vector3 local = held.transform.InverseTransformPoint(pin);
			SetPress(slot, held, local, Vector3.down * (yields ? 0f : weight), hangMax, smoothing, true, held.transform.InverseTransformPoint(aim));
			outgoingPresses[slot].first = first;
			Transform core = other.bones[CoreOf(held.index)];
			Quaternion yaw = Yaw(core.rotation);
			outgoingPresses[slot].root = Quaternion.Inverse(yaw) * (localBones[0].position - core.position);
			outgoingPresses[slot].turn = PackTurn(Quaternion.Inverse(yaw) * localBones[0].rotation);
		}

		private void Ride(NetworkPlayer carrier)
		{
			for (int i = 0; i < Bones; i++)
			{
				Rigidbody body = localBodies[i];
				if (!body)
				{
					continue;
				}
				body.detectCollisions = false;
				body.position = carrier.carryPos[i];
				body.rotation = carrier.carryRot[i];
				body.linearVelocity = Vector3.ClampMagnitude(carrier.CarryVel[i], 10f);
				body.angularVelocity = Vector3.zero;
			}
			riding = true;
		}

		private void TakeBack(NetworkPlayer carrier)
		{
			bool streamed = carrier.CarryPos[0] != Vector3.zero;
			float ahead = streamed ? Vector3.Distance(carrier.CarryPos[0], carrier.carryPos[0]) : 0f;
			for (int i = 0; i < Bones; i++)
			{
				Rigidbody body = localBodies[i];
				if (!body)
				{
					continue;
				}
				body.position = streamed ? carrier.CarryPos[i] : carrier.carryPos[i];
				body.rotation = streamed ? carrier.CarryRot[i] : carrier.carryRot[i];
				body.linearVelocity = Vector3.ClampMagnitude(carrier.CarryVel[i], 10f);
				body.angularVelocity = Vector3.zero;
			}
			Unride();
			TookBack = carrier.CarrySeq;
			Player.Note("took my body back from " + carrier.name + " at " + localBodies[0].position.ToString("F1") + " moving " + carrier.CarryVel[0].magnitude.ToString("F1") + " m/s, " + ahead.ToString("F1") + " m ahead of the shown carry");
		}

		public void KeepBody()
		{
			NetworkPlayer carrier = testCarrier ? testCarrier : FindCarrier(Object.Id);
			if (carrier && carrier.CarrySeq != TookBack)
			{
				WantUp = carrier.CarrySeq;
				TookBack = carrier.CarrySeq;
			}
			Unride();
			if (fighter)
			{
				fighter.carriedHold = false;
			}
		}

		private void Unride()
		{
			if (!riding)
			{
				return;
			}
			riding = false;
			foreach (Rigidbody body in localBodies)
			{
				if (body)
				{
					body.detectCollisions = true;
				}
			}
		}

		private bool Settled(AvatarBody copy)
		{
			foreach (Rigidbody body in copy.bodies)
			{
				if (body && body.linearVelocity.sqrMagnitude > carryRest * carryRest)
				{
					return false;
				}
			}
			return true;
		}

		private static AvatarBody CarriedIn(HandGrab hand)
		{
			AvatarBody.AvatarHit part = HeldPart(hand);
			return part && !hand.holdsYou ? part.owner : null;
		}

		private static NetworkPlayer FindCarrier(NetworkId id)
		{
			foreach (NetworkPlayer a in avatars)
			{
				if (a && a.carryShown && a.CarryId == id)
				{
					return a;
				}
			}
			return null;
		}

		private bool IsGrabbing(NetworkId id)
		{
			for (int i = 0; i < 2; i++)
			{
				NetPress press = Presses[i];
				if (press.grab && press.avatar == id)
				{
					return true;
				}
			}
			return false;
		}

		private bool Keeps(NetworkPlayer other, bool first)
		{
			bool mineFirst = first || Firsts(other.Object.Id);
			bool theirsFirst = other.Firsts(Object.Id);
			if (mineFirst != theirsFirst)
			{
				return mineFirst;
			}
			return Object.Id.Raw < other.Object.Id.Raw;
		}

		private bool Firsts(NetworkId id)
		{
			for (int i = 0; i < 2; i++)
			{
				NetPress press = HasStateAuthority ? outgoingPresses[i] : Presses[i];
				if (press.grab && press.first && press.avatar == id)
				{
					return true;
				}
			}
			return false;
		}

		private static NetworkPlayer FindAvatar(NetworkId id)
		{
			foreach (NetworkPlayer a in avatars)
			{
				if (a && a.Object && a.Object.Id == id)
				{
					return a;
				}
			}
			return null;
		}

		private static AvatarBody.AvatarHit HeldPart(HandGrab hand)
		{
			AvatarBody.AvatarHit part = hand && hand.grabbing && hand.held ? hand.held.GetComponent<AvatarBody.AvatarHit>() : null;
			return RemoteAvatar(part) ? part : null;
		}

		private static NetworkPlayer RemoteAvatar(AvatarBody.AvatarHit part)
		{
			NetworkPlayer avatar = part && part.owner ? part.owner.GetComponent<NetworkPlayer>() : null;
			return avatar && avatar.Object && avatar.Object.IsValid ? avatar : null;
		}

		private static bool IsFreshTouch(AvatarBody.FistWatch watch)
		{
			return watch && watch.touching && Time.time - watch.at < Time.fixedDeltaTime * 2.5f;
		}

		private Vector3 TugPull(Vector3 offset, Vector3 vel, float lift)
		{
			Vector3 pull = Vector3.ClampMagnitude(offset * tugSpring - vel * grabDamp, grabMax);
			pull.y = Mathf.Min(pull.y, lift);
			return pull;
		}

		private Vector3 GrabPull(Vector3 offset, Vector3 vel, float cap, float lift)
		{
			Vector3 pull = Vector3.ClampMagnitude(offset * grabSpring - vel * grabDamp, cap);
			pull.y = Mathf.Min(pull.y, lift);
			return pull;
		}

		private void SetPress(int slot, AvatarBody.AvatarHit part, Vector3 local, Vector3 force, float cap, float smoothing, bool grab, Vector3 target)
		{
			NetworkPlayer other = RemoteAvatar(part);
			NetPress press = outgoingPresses[slot];
			if (!other)
			{
				press.force = Vector3.Lerp(press.force, Vector3.zero, smoothing);
				press.grab = false;
				outgoingPresses[slot] = press.force.sqrMagnitude < 1f ? default : press;
				return;
			}
			Vector3 kept = press.avatar == other.Object.Id && press.bone == part.index ? press.force : Vector3.zero;
			press.avatar = other.Object.Id;
			press.bone = part.index;
			press.local = local;
			press.force = Vector3.Lerp(kept, Vector3.ClampMagnitude(force, cap), smoothing);
			press.target = target;
			press.grab = grab;
			outgoingPresses[slot] = press;
		}

		private static int CoreOf(int bone)
		{
			return bone >= 1 && bone <= 8 ? 1 : 0;
		}

		private static Quaternion Yaw(Quaternion q)
		{
			if (!yawAnchor)
			{
				return q;
			}
			Vector3 ahead = q * Vector3.forward;
			if (ahead.x * ahead.x + ahead.z * ahead.z < 0.04f)
			{
				ahead = q * Vector3.up;
			}
			ahead.y = 0f;
			if (ahead.sqrMagnitude > 0.0001f)
			{
				return Quaternion.LookRotation(ahead.normalized, Vector3.up);
			}
			return Quaternion.identity;
		}

		private void Rigid(float weight)
		{
			Vector3 pivot = showPos[0];
			Quaternion turn = Quaternion.Slerp(Quaternion.identity, rigidFix, weight);
			Vector3 moved = pivot + rigidMove * weight;
			for (int i = 0; i < Bones; i++)
			{
				showPos[i] = moved + turn * (showPos[i] - pivot);
				showRot[i] = turn * showRot[i];
			}
		}

		private float FootY(int bone)
		{
			CapsuleCollider capsule = null;
			if (copy && copy.bones != null && copy.bones[bone])
			{
				capsule = copy.bones[bone].GetComponent<CapsuleCollider>();
			}
			if (!capsule)
			{
				return showPos[bone].y - 0.45f;
			}
			Vector3 axis = capsule.direction == 0 ? Vector3.right : capsule.direction == 1 ? Vector3.up : Vector3.forward;
			Vector3 scale = capsule.transform.lossyScale;
			float half = Mathf.Max(capsule.height * 0.5f - capsule.radius, 0f);
			Quaternion turn = showRot[bone];
			Vector3 top = turn * Vector3.Scale(axis * half + capsule.center, scale);
			Vector3 bottom = turn * Vector3.Scale(capsule.center - axis * half, scale);
			float radius = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z)) * capsule.radius;
			return Mathf.Min(top.y, bottom.y) + showPos[bone].y - radius;
		}

		private static float FloorUnder(Vector3 at)
		{
			float floor = float.MinValue;
			foreach (RaycastHit hit in Physics.RaycastAll(at + Vector3.up * 0.5f, Vector3.down, 4f, -1, QueryTriggerInteraction.Ignore))
			{
				Rigidbody body = hit.collider.attachedRigidbody;
				Grabbable grabbable = body ? body.GetComponent<Grabbable>() : null;
				if (body && (body.GetComponentInParent<AvatarBody>() || body.GetComponentInParent<Player>() || (grabbable && !grabbable.holdsYou)))
				{
					continue;
				}
				if (hit.point.y > floor)
				{
					floor = hit.point.y;
				}
			}
			return floor;
		}

		private Vector3 ApplyPresses(out Vector3 pulls)
		{
			pulls = Vector3.zero;
			Vector3 total = Vector3.zero;
			pressIn = 0f;
			pressBone = -1;
			bool knocked = fighter && fighter.knockedOut;
			float strongest = 0f;
			foreach (NetworkPlayer a in avatars)
			{
				if (a == this || !a.Object || !a.Object.IsValid || a.Dead || a.KnockedOut)
				{
					continue;
				}
				for (int i = 0; i < a.Presses.Length; i++)
				{
					NetPress press = a.Presses[i];
					if (press.avatar != Object.Id || (uint)press.bone > 12u)
					{
						continue;
					}
					if (!press.grab && press.force.sqrMagnitude < 1f)
					{
						continue;
					}
					if (knocked && press.grab)
					{
						continue;
					}
					Rigidbody body = localBodies[press.bone];
					if (!body)
					{
						continue;
					}
					Vector3 at = localBones[press.bone].TransformPoint(press.local);
					Vector3 force = Vector3.ClampMagnitude(press.force, hangMax);
					if (press.grab)
					{
						Vector3 goal = localBones[press.bone].TransformPoint(press.target);
						Vector3 pull = GrabPull(goal - at, body.GetPointVelocity(at) - localBodies[0].linearVelocity, grabMax, liftMax);
						force += pull;
						pulls += pull;
					}
					body.AddForceAtPosition(force, at, ForceMode.Force);
					total += force;
					pressIn += force.magnitude;
					if (force.magnitude > strongest)
					{
						strongest = force.magnitude;
						pressBone = press.bone;
					}
				}
			}
			return total;
		}

		private void Tag()
		{
			Transform cam = null;
			if (Player.main)
			{
				cam = Player.main.cam;
			}
			bool ghost = GhostSize > 0f;
			Transform anchor;
			if (ghost)
			{
				anchor = ghostAlpha > 0f ? ghostHead : null;
			}
			else
			{
				anchor = Dead || Watching ? null : bones[2];
			}
			Vector3 at = anchor ? anchor.position + Vector3.up * TagAbove * (ghost ? GhostSize : 1f) : Vector3.zero;
			float want = cam && anchor ? Mathf.InverseLerp(TagFar, TagNear, Vector3.Distance(cam.position, anchor.position + Vector3.up * TagAbove)) : 0f;
			if (ghost)
			{
				want *= ghostAlpha / GhostShow;
			}
			tagAlpha = Mathf.MoveTowards(tagAlpha, want, Time.deltaTime * TagFade);
			if (tagAlpha <= 0f)
			{
				if (tagText && tagText.gameObject.activeSelf)
				{
					tagText.gameObject.SetActive(false);
				}
				return;
			}
			if (!tagText)
			{
				GameObject tag = new GameObject("Name Tag");
				tag.transform.SetParent(transform, false);
				tagText = tag.AddComponent<TextMeshPro>();
				TMP_FontAsset font = GameManager.main && GameManager.main.title ? GameManager.main.title.font : TMP_Settings.defaultFontAsset;
				if (font)
				{
					tagText.font = font;
					tagText.fontSharedMaterial = font.material;
				}
				tagText.rectTransform.sizeDelta = new Vector2(1.4f, 0.25f);
				tagText.fontSize = 0.9f;
				tagText.fontStyle = FontStyles.Bold;
				tagText.alignment = TextAlignmentOptions.Center;
				tagText.textWrappingMode = TextWrappingModes.NoWrap;
				tagText.overflowMode = TextOverflowModes.Overflow;
				tagText.outlineWidth = 0.22f;
				tagText.outlineColor = new Color32(20, 20, 30, 255);
				tagName = null;
			}
			string badge = Level > 0 ? "<size=70%>" + Account.Badge(Level) + "</size>  " : "";
			string label = badge + (Nick.Length > 0 ? Nick.Value.ToUpperInvariant() : "PLAYER " + Object.StateAuthority.PlayerId);
			if (label != tagName)
			{
				tagName = label;
				tagText.text = label;
			}
			if (!tagText.gameObject.activeSelf)
			{
				tagText.gameObject.SetActive(true);
			}
			tagText.color = new Color(1f, 1f, 1f, tagAlpha);
			Transform shownTag = tagText.transform;
			shownTag.position = at;
			Vector3 look = at - cam.position;
			if (look.sqrMagnitude > 0.0001f)
			{
				shownTag.rotation = Quaternion.LookRotation(look.normalized, Vector3.up);
			}
		}

		private void Haunt()
		{
			Player me = Player.main;
			float want = 0f;
			if (GhostSize > 0f)
			{
				if (me && me.ghost)
				{
					want = GhostShow;
				}
				else if (GameManager.main)
				{
					want = GameManager.main.winner != null ? GhostShow : 0f;
				}
			}
			ghostAlpha = Mathf.MoveTowards(ghostAlpha, want, Time.deltaTime * GhostShow / GhostFade);
			if (ghostAlpha <= 0f)
			{
				ShowGhost(false);
				return;
			}
			if (!ghostHead && !BuildGhost())
			{
				return;
			}
			if (ghostMat && Shown() != ghostPaint)
			{
				ghostPaint = Shown();
				ghostMat.SetColor("_BaseColor", Colour(ghostPaint));
			}
			bool wasShown = ghostHead.gameObject.activeSelf;
			ShowGhost(true);
			float k = 1f;
			if (wasShown)
			{
				k = 1f - Mathf.Exp(Time.deltaTime * -GhostFollow);
			}
			Follow(ghostHead, GhostHead, GhostHeadTurn, ghostBase[0], GhostSize, k);
			Follow(ghostL, GhostL, GhostLTurn, ghostBase[1], GhostSize, k);
			Follow(ghostR, GhostR, GhostRTurn, ghostBase[2], GhostSize, k);
			ghostBlock.SetFloat("_Alpha", ghostAlpha);
			foreach (Renderer part in ghostParts)
			{
				if (part)
				{
					part.SetPropertyBlock(ghostBlock);
				}
			}
		}

		private static void Follow(Transform t, Vector3 at, Quaternion turn, Vector3 rest, float size, float k)
		{
			if (!t)
			{
				return;
			}
			t.SetPositionAndRotation(Vector3.Lerp(t.position, at, k), Quaternion.Slerp(t.rotation, turn, k));
			if (size > 0f)
			{
				t.localScale = Vector3.Lerp(t.localScale, rest * size, k);
			}
		}

		private void ShowGhost(bool on)
		{
			foreach (Transform part in new Transform[] { ghostHead, ghostL, ghostR })
			{
				if (part && part.gameObject.activeSelf != on)
				{
					part.gameObject.SetActive(on);
				}
			}
		}

		private bool BuildGhost()
		{
			if (ghostTried || !copy || copy.bones == null || copy.bones.Length < Bones || !copy.bones[2] || !copy.bones[5] || !copy.bones[8])
			{
				return false;
			}
			ghostTried = true;
			SkinnedMeshRenderer skin = null;
			foreach (SkinnedMeshRenderer candidate in copy.GetComponentsInChildren<SkinnedMeshRenderer>(true))
			{
				if (candidate.sharedMesh && System.Array.IndexOf(candidate.bones, copy.bones[2]) >= 0 && System.Array.IndexOf(candidate.bones, copy.bones[5]) >= 0)
				{
					skin = candidate;
				}
			}
			Material look = Resources.Load<Material>("Player/GhostHand");
			if (!skin || !look)
			{
				return false;
			}
			ghostPaint = Shown();
			Color color = Colour(ghostPaint);
			ghostMat = new Material(look);
			Material skinMat = skin.sharedMaterial;
			if (skinMat && skinMat.HasProperty("_BaseMap"))
			{
				ghostMat.SetTexture("_BaseMap", skinMat.GetTexture("_BaseMap"));
			}
			ghostMat.SetColor("_BaseColor", color);
			ghostBlock = new MaterialPropertyBlock();
			ghostHead = Cut(skin, copy.bones[2], "Ghost Head " + name);
			ghostL = Cut(skin, copy.bones[5], "Ghost Hand.L " + name);
			ghostR = Cut(skin, copy.bones[8], "Ghost Hand.R " + name);
			if (ghostHead)
			{
				ghostBase[0] = ghostHead.localScale;
			}
			if (ghostL)
			{
				ghostBase[1] = ghostL.localScale;
			}
			if (ghostR)
			{
				ghostBase[2] = ghostR.localScale;
			}
			Transform face = copy.bones[2].Find("Face");
			MeshRenderer faceRenderer = null;
			if (face)
			{
				faceRenderer = face.GetComponent<MeshRenderer>();
			}
			MeshFilter faceFilter = null;
			if (face)
			{
				faceFilter = face.GetComponent<MeshFilter>();
			}
			if (ghostHead && faceRenderer && faceFilter && faceFilter.sharedMesh)
			{
				ghostFace = new Material(look);
				Material eyes = Resources.Load<Material>("Player/DeadEyes");
				Material source = eyes ? eyes : faceRenderer.sharedMaterial;
				if (source && source.HasProperty("_BaseMap"))
				{
					ghostFace.SetTexture("_BaseMap", source.GetTexture("_BaseMap"));
				}
				ghostFace.SetColor("_BaseColor", new Color(1f, 1f, 1f, 1f));
				GameObject ghostFaceObject = new GameObject("Ghost Face");
				ghostFaceObject.transform.SetParent(ghostHead, false);
				ghostFaceObject.transform.localPosition = face.localPosition;
				ghostFaceObject.transform.localRotation = face.localRotation;
				ghostFaceObject.transform.localScale = face.localScale;
				ghostFaceObject.AddComponent<MeshFilter>().sharedMesh = faceFilter.sharedMesh;
				MeshRenderer faceCopy = ghostFaceObject.AddComponent<MeshRenderer>();
				faceCopy.sharedMaterial = ghostFace;
				faceCopy.shadowCastingMode = ShadowCastingMode.Off;
				ghostParts.Add(faceCopy);
			}
			return ghostHead && ghostL && ghostR;
		}

		private Transform Cut(SkinnedMeshRenderer skin, Transform bone, string label)
		{
			Transform[] skinBones = skin.bones;
			List<int> kept = new List<int>();
			int root = -1;
			for (int i = 0; i < skinBones.Length; i++)
			{
				if (skinBones[i] && skinBones[i].IsChildOf(bone))
				{
					if (skinBones[i] == bone)
					{
						root = kept.Count;
					}
					kept.Add(i);
				}
			}
			if (root < 0)
			{
				return null;
			}
			Mesh mesh = Player.HandMesh(skin.sharedMesh, kept, root, 0.06f);
			GameObject cut = new GameObject(label);
			Transform[] parts = new Transform[kept.Count];
			for (int i = 0; i < kept.Count; i++)
			{
				parts[i] = (i != root ? new GameObject(skinBones[kept[i]].name) : cut).transform;
			}
			for (int i = 0; i < kept.Count; i++)
			{
				if (i == root)
				{
					continue;
				}
				Transform source = skinBones[kept[i]];
				int parent = kept.IndexOf(System.Array.IndexOf(skinBones, source.parent));
				parts[i].SetParent(parent < 0 ? cut.transform : parts[parent], false);
				parts[i].localPosition = source.localPosition;
				parts[i].localRotation = source.localRotation;
				parts[i].localScale = source.localScale;
			}
			cut.transform.localScale = bone.lossyScale;
			SkinnedMeshRenderer shown = cut.AddComponent<SkinnedMeshRenderer>();
			shown.sharedMesh = mesh;
			shown.bones = parts;
			shown.rootBone = cut.transform;
			Material[] materials = new Material[mesh.subMeshCount];
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i] = ghostMat;
			}
			shown.sharedMaterials = materials;
			shown.updateWhenOffscreen = true;
			shown.shadowCastingMode = ShadowCastingMode.Off;
			ghostParts.Add(shown);
			cut.SetActive(false);
			return cut.transform;
		}

		private void Unseen()
		{
			bool watching = Watching;
			if (!watching && shownWatching == watching)
			{
				return;
			}
			if (shownWatching != watching)
			{
				shownWatching = watching;
				if (copy)
				{
					copy.SetPhysical(!watching);
				}
				Player.Note(name + (watching ? " is spectating, hidden" : " is playing again"));
			}
			GetComponentsInChildren(true, hidden);
			foreach (Renderer part in hidden)
			{
				if (part)
				{
					part.forceRenderingOff = watching;
				}
			}
		}

		public override void Render()
		{
			if (Deaths > seenDeaths)
			{
				Player.Note("death sound for " + name);
				GameManager.Died();
			}
			seenDeaths = Deaths;
			if (HasStateAuthority || bones == null)
			{
				return;
			}
			AttachClimber();
			Unseen();
			Haunt();
			Tag();
			Sounds();
			Dress();
			if (!latestPose)
			{
				viewLag = Runner.LocalRenderTime - Runner.RemoteRenderTime;
			}
			if (stars)
			{
				stars.on = KnockedOut;
				stars.dead = Dead;
			}
			if (grabbable)
			{
				grabbable.holdsYou = !KnockedOut;
			}
			if (KnockedOut && !wasKnockedOut && (!copy || !copy.Confirm()) && Time.time > popAt + 1.5f)
			{
				popAt = Time.time;
				Player.KnockAt(bones[2].position);
			}
			if (!KnockedOut && wasKnockedOut && copy)
			{
				copy.DropCarry();
			}
			if (KnockedOut && copy)
			{
				copy.DropHold();
			}
			wasKnockedOut = KnockedOut;
			NetworkPlayer carrier = FindCarrier(Object.Id);
			if (carrier && carrier.CarrySeq == TookBack)
			{
				carrier = null;
			}
			if (carrier)
			{
				carryShown = false;
				wasShownCarried = true;
				rootKnown = false;
				shownRootKnown = false;
				forecastShift = Vector3.zero;
				for (int i = 0; i < Bones; i++)
				{
					showPos[i] = carrier.carryPos[i];
					showRot[i] = carrier.carryRot[i];
				}
				if (copy)
				{
					copy.holdingMe = false;
				}
				ShowPose();
				return;
			}
			if (wasShownCarried)
			{
				wasShownCarried = false;
				if (copy)
				{
					copy.Free();
				}
			}
			NetworkBehaviourBuffer from = default;
			NetworkBehaviourBuffer to = default;
			float alpha = 0f;
			if (latestPose)
			{
				if (!Latest())
				{
					return;
				}
			}
			else
			{
				if (!TryGetSnapshotsBuffers(out from, out to, out alpha))
				{
					return;
				}
				Vector3 rootFrom = rootReader.Read(from);
				Vector3 rootTo = rootReader.Read(to);
				NetworkArrayReadOnly<int> poseFrom = poseReader.Read(from);
				NetworkArrayReadOnly<int> poseTo = poseReader.Read(to);
				if (rootTo == Vector3.zero)
				{
					return;
				}
				bool blend = rootFrom != Vector3.zero && (rootTo - rootFrom).sqrMagnitude < teleportGap * teleportGap;
				if (to.Tick != lastTick)
				{
					lastTick = to.Tick;
					shown++;
				}
				for (int i = 0; i < Bones; i++)
				{
					Vector3 spot = Spot(rootTo, poseTo, i);
					Quaternion turn = UnpackTurn(poseTo[i * 3 + 2]);
					if (blend)
					{
						spot = Vector3.Lerp(Spot(rootFrom, poseFrom, i), spot, alpha);
					}
					showPos[i] = spot;
					if (blend)
					{
						turn = Quaternion.Slerp(UnpackTurn(poseFrom[i * 3 + 2]), turn, alpha);
					}
					showRot[i] = turn;
				}
			}
			if (rootKnown && (showPos[0] - lastRootPos).sqrMagnitude > teleportGap * teleportGap)
			{
				rootKnown = false;
				shownRootKnown = false;
				rootVelocity = Vector3.zero;
				forecastShift = Vector3.zero;
			}
			if (rootKnown && Time.deltaTime > 0.0001f)
			{
				Vector3 velocity = (showPos[0] - lastRootPos) / Time.deltaTime;
				rootVelocity = Vector3.Lerp(rootVelocity, velocity, 1f - Mathf.Exp(-Time.deltaTime / 0.1f));
			}
			lastRootPos = showPos[0];
			rootKnown = true;
			float upright = 0f;
			if (!KnockedOut && !Dead)
			{
				upright = Mathf.InverseLerp(uprightFrom, uprightFull, showPos[2].y - showPos[0].y);
			}
			uprightBlend = upright < uprightBlend ? upright : Mathf.MoveTowards(uprightBlend, upright, Time.deltaTime / Mathf.Max(uprightEase, 0.01f));
			Vector3 lead = Vector3.ClampMagnitude(new Vector3(rootVelocity.x, 0f, rootVelocity.z) * (uprightBlend * Mathf.Min(viewLag, maxLead)), maxShift);
			forecastShift = Vector3.MoveTowards(forecastShift, lead, shiftRate * Time.deltaTime);
			Vector3 handShift = Vector3.zero;
			Quaternion coreTurn = Quaternion.identity;
			Vector3 coreRoot = Vector3.zero;
			Transform core = null;
			int held = 0;
			int hands = 0;
			int onMine = 0;
			int cores = 0;
			bool anchored = false;
			if (!Dead && !KnockedOut)
			{
				for (int i = 0; i < 2; i++)
				{
					NetPress press = Presses[i];
					if (!press.grab || press.bone < 0 || press.bone > 12)
					{
						continue;
					}
					NetworkPlayer avatar = FindAvatar(press.avatar);
					if (!avatar || avatar == this)
					{
						continue;
					}
					if (avatar != mine && avatar.IsGrabbing(Object.Id) && !Keeps(avatar, false))
					{
						continue;
					}
					if (avatar == mine && copy && copy.Held)
					{
						continue;
					}
					Transform[] list = avatar == mine ? mine.localBones : avatar.bones;
					Transform bone = list != null ? list[press.bone] : null;
					if (!bone)
					{
						continue;
					}
					held++;
					onMine += avatar == mine ? 1 : 0;
					if (press.root == Vector3.zero)
					{
						Vector3 at = bone.TransformPoint(press.local);
						hands++;
						handShift += at - showPos[i == 0 ? 5 : 8];
						continue;
					}
					Transform pressCore = (avatar == mine ? mine.localBones : avatar.bones)[CoreOf(press.bone)];
					if (cores >= 1 && pressCore != core)
					{
						continue;
					}
					Quaternion turn = UnpackTurn(press.turn);
					if (cores != 0)
					{
						turn = Quaternion.Slerp(coreTurn, turn, 0.5f);
					}
					coreTurn = turn;
					core = pressCore;
					cores++;
					coreRoot += press.root;
				}
				for (int i = 0; i < 2; i++)
				{
					if (HeldBody(i, out Vector3 point))
					{
						held++;
						hands++;
						handShift += point - showPos[i == 0 ? 5 : 8];
					}
				}
				if (cores >= 1)
				{
					Vector3 root = coreRoot / cores;
					if (rigidHeld && climbCore)
					{
						if (core != climbCore)
						{
							Quaternion was = Yaw(climbCore.rotation);
							Quaternion now = Yaw(core.rotation);
							climbRoot = Quaternion.Inverse(now) * (climbCore.position + was * climbRoot - core.position);
							climbTurn = Quaternion.Inverse(now) * was * climbTurn;
						}
						float k = 1f - Mathf.Exp(-Time.deltaTime / ClimbSmooth);
						climbRoot = Vector3.Lerp(climbRoot, root, k);
						climbTurn = Quaternion.Slerp(climbTurn, coreTurn, k);
					}
					else
					{
						climbRoot = root;
						climbTurn = coreTurn;
					}
					climbCore = core;
					anchored = true;
				}
			}
			if ((anchored || rigidHeld) && climbCore)
			{
				Quaternion yaw = Yaw(climbCore.rotation);
				rigidFix = yaw * climbTurn * Quaternion.Inverse(showRot[0]);
				rigidMove = climbCore.position + yaw * climbRoot - showPos[0];
			}
			if (anchored)
			{
				rigidHeld = true;
			}
			else if (rigidHeld)
			{
				rigidHeld = anchorBlend > 0f;
			}
			if (rigidHeld)
			{
				Rigid(anchorBlend);
			}
			if (held >= 1)
			{
				Vector3 shift = hands > 0 ? handShift / hands : Vector3.zero;
				anchorShift = anchorBlend > 0f ? Vector3.MoveTowards(anchorShift, shift, anchorRate * Time.deltaTime) : shift;
			}
			if (copy)
			{
				copy.holdingMe = onMine > 0;
			}
			anchorBlend = Mathf.MoveTowards(anchorBlend, held > 0 ? 1f : 0f, Time.deltaTime * 4f);
			Vector3 offset = Vector3.Lerp(forecastShift, anchorShift, anchorBlend);
			if (anchorBlend > 0f)
			{
				if (floorLift)
				{
					float left = FootY(10);
					float right = FootY(12);
					Vector3 foot = left < right ? showPos[10] : showPos[12];
					float floor = FloorUnder(foot + offset);
					if (floor > float.MinValue)
					{
						offset.y += Mathf.Max(floor - Mathf.Min(left, right) - offset.y, 0f) * anchorBlend;
					}
				}
				else
				{
					Vector3 foot = showPos[10].y < showPos[12].y ? showPos[10] : showPos[12];
					offset.y = Mathf.Max(offset.y, Mathf.Min(FloorUnder(foot) + kneeHeight - foot.y, 0f));
				}
			}
			for (int i = 0; i < Bones; i++)
			{
				showPos[i] += offset;
			}
			if (anchorBlend > 0f && shownRootKnown)
			{
				Vector3 step = showPos[0] - shownRoot;
				float most = anchorSpeed * Time.deltaTime;
				if (step.sqrMagnitude > most * most)
				{
					Vector3 fix = shownRoot + step.normalized * most - showPos[0];
					for (int i = 0; i < Bones; i++)
					{
						showPos[i] += fix;
					}
				}
			}
			Vector3 pin = Train.Pin(showPos);
			if (pin != Vector3.zero)
			{
				for (int i = 0; i < Bones; i++)
				{
					showPos[i] += pin;
				}
			}
			shownRoot = showPos[0];
			shownRootKnown = true;
			BlendStandUp();
			carryShown = false;
			if (CarryId.IsValid && latestPose)
			{
				if (latestCarry)
				{
					for (int i = 0; i < Bones; i++)
					{
						latestCarryShown[i] = Vector3.Lerp(latestCarryFrom[i], latestCarryTo[i], latestAlpha);
						latestCarryShownRot[i] = Quaternion.Slerp(latestCarryFromRot[i], latestCarryToRot[i], latestAlpha);
						carryPos[i] = offset + latestCarryShown[i];
						carryRot[i] = latestCarryShownRot[i];
					}
					carryShown = true;
				}
			}
			else if (CarryId.IsValid)
			{
				NetworkArrayReadOnly<Vector3> posFrom = carryPosReader.Read(from);
				NetworkArrayReadOnly<Vector3> posTo = carryPosReader.Read(to);
				NetworkArrayReadOnly<Quaternion> rotFrom = carryRotReader.Read(from);
				NetworkArrayReadOnly<Quaternion> rotTo = carryRotReader.Read(to);
				if (posTo[0] != Vector3.zero)
				{
					bool blend = posFrom[0] != Vector3.zero;
					for (int i = 0; i < Bones; i++)
					{
						carryPos[i] = offset + (blend ? Vector3.Lerp(posFrom[i], posTo[i], alpha) : posTo[i]);
						carryRot[i] = blend ? Quaternion.Slerp(rotFrom[i], rotTo[i], alpha) : rotTo[i];
					}
					carryShown = true;
				}
			}
			ShowPose();
		}

		private void Sounds()
		{
			Player main = Player.main;
			if (!main || bones[0] == null)
			{
				return;
			}
			if (mouth == null)
			{
				mouth = new Player.Babble
				{
					source = main.SpeakerOn(bones[2])
				};
				hipSound = main.SpeakerOn(bones[0]);
				handSoundL = main.SpeakerOn(bones[5]);
				handSoundR = main.SpeakerOn(bones[8]);
				heardJumps = Jumps;
			}
			if (Jumps != heardJumps)
			{
				heardJumps = Jumps;
				main.PlayJump(hipSound);
			}
			if (!speech)
			{
				speech = GetComponentInChildren<Speaker>(true);
				if (speech)
				{
					speechSource = speech.GetComponent<AudioSource>();
					speechBlend = speechSource.spatialBlend;
					speechVolume = speechSource.volume;
					speechSource.rolloffMode = AudioRolloffMode.Custom;
					speechSource.maxDistance = VoiceReach;
					speechSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, VoiceFalloff);
				}
			}
			bool blab = Blabbered();
			if (wasBlab != blab)
			{
				wasBlab = blab;
				Player.Note(name + (blab ? " now blabbers" : " now uses their real voice") + " (their setting " + (Blabber ? "blabber" : "real") + ", my other voices " + (Prefs.realVoice ? "real" : "blabber") + ", muted " + (Uid.Length > 0 && muted.Contains(Uid.Value)) + ")");
			}
			float volume = 1f;
			bool spatial = true;
			if (Watching || (Dead && GameManager.match != null))
			{
				volume = main.dead ? 1f : GameManager.watching ? 1f : GhostVolume;
				spatial = false;
			}
			if (speechSource)
			{
				speechSource.mute = blab;
				speechSource.spatialBlend = spatial ? speechBlend : 0f;
				speechSource.volume = volume * speechVolume;
			}
			if (mouth.source)
			{
				mouth.source.spatialBlend = spatial ? 1f : 0f;
			}
			mouth.Tick(blab && Talking, main.blabbers, volume * main.blabberVolume, 1f);
		}

		public bool Blabbered()
		{
			if (!Prefs.realVoice || Blabber)
			{
				return true;
			}
			return Uid.Length >= 1 && muted.Contains(Uid.Value);
		}

		private void BindFingers()
		{
			NetworkManager manager = FindFirstObjectByType<NetworkManager>();
			Player body = null;
			if (manager && manager.localBody)
			{
				body = manager.localBody.GetComponent<Player>();
			}
			if (!body)
			{
				return;
			}
			int count = body.leftFingers.Count + body.rightFingers.Count;
			fingerBones = new Transform[count];
			fingerOpen = new Quaternion[count];
			fingerClosed = new Quaternion[count];
			fingerIsThumb = new bool[count];
			fingerIsLeft = new bool[count];
			Transform[] all = GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < count; i++)
			{
				int lefts = body.leftFingers.Count;
				Player.Digit digit = i < lefts ? body.leftFingers[i] : body.rightFingers[i - lefts];
				if (!digit.bone)
				{
					continue;
				}
				foreach (Transform part in all)
				{
					if (part.name == digit.bone.name)
					{
						fingerBones[i] = part;
						break;
					}
				}
				fingerOpen[i] = digit.open;
				fingerClosed[i] = digit.closed;
				fingerIsThumb[i] = digit.thumb;
				fingerIsLeft[i] = i < lefts;
			}
		}

		private void ShowFingers()
		{
			if (fingerBones == null)
			{
				return;
			}
			shownCurl = Vector4.MoveTowards(shownCurl, Curl, curlRate * Time.deltaTime);
			for (int i = 0; i < fingerBones.Length; i++)
			{
				if (!fingerBones[i])
				{
					continue;
				}
				float curl = fingerIsLeft[i] ? (fingerIsThumb[i] ? shownCurl.y : shownCurl.x) : (fingerIsThumb[i] ? shownCurl.w : shownCurl.z);
				fingerBones[i].localRotation = Quaternion.Slerp(fingerOpen[i], fingerClosed[i], curl);
			}
		}

		private void BlendStandUp()
		{
			if (KnockedOut || (anchorBlend <= 0f && showPos[2].y - showPos[0].y < uprightFrom))
			{
				for (int i = 0; i < Bones; i++)
				{
					lyingPos[i] = showPos[i];
					lyingRot[i] = showRot[i];
				}
				wasLying = true;
				return;
			}
			if (wasLying)
			{
				wasLying = false;
				standUpAt = (showPos[0] - lyingPos[0]).sqrMagnitude > teleportGap * teleportGap ? -9f : Time.time;
			}
			float t = Mathf.Clamp01((Time.time - standUpAt) / Mathf.Max(riseBlend, 0.01f));
			if (t >= 1f)
			{
				return;
			}
			float k = t * t * (3f - 2f * t);
			for (int i = 0; i < Bones; i++)
			{
				showPos[i] = Vector3.Lerp(lyingPos[i], showPos[i], k);
				showRot[i] = Quaternion.Slerp(lyingRot[i], showRot[i], k);
			}
		}

		private void ShowPose()
		{
			ShowFingers();
			if (copy && copy.physical)
			{
				copy.Drive(showPos, showRot);
				return;
			}
			transform.position = showPos[0];
			Apply(bones, showPos, showRot);
		}

		private void SendHit(int bone, float impulse, float speed, Vector3 point, Vector3 dir, Rigidbody by, int verdict)
		{
			Vector3 local = bones[bone].InverseTransformPoint(point);
			bool fist = copy && copy.IsLocalFist(by);
			RPC_Hit(bone, impulse, speed, local, dir, verdict, fist, point);
		}

		[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
		public void RPC_Landed(int bone, Vector3 local, PlayerRef by, RpcInfo info = default(RpcInfo))
		{
			if ((uint)bone > 12u || Runner.LocalPlayer == by)
			{
				return;
			}
			Transform part = null;
			if (HasStateAuthority && localBodies != null && localBodies[bone])
			{
				part = localBodies[bone].transform;
			}
			else if (bones != null)
			{
				part = bones[bone];
			}
			if (part)
			{
				Player.HitAt(part.TransformPoint(local), true);
			}
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void RPC_Hit(int bone, float impulse, float speed, Vector3 local, Vector3 dir, int verdict, NetworkBool fist, Vector3 world, RpcInfo info = default(RpcInfo))
		{
			if (AvatarBody.peaceful || AvatarBody.truce || !localBody || (uint)bone > 12u || localBodies == null || (drown && drown.drowned) || (copy && copy.maxSpeed < speed) || !localBodies[bone] || GameManager.SameTeam(UidOf(info.Source), Uid.Value))
			{
				if (verdict == 2)
				{
					RPC_Overturn(info.Source);
				}
				return;
			}
			PendingHit hit = new PendingHit
			{
				source = info.Source,
				bone = bone,
				verdict = verdict,
				impulse = impulse,
				speed = speed,
				at = Time.time,
				best = float.MaxValue,
				local = local,
				dir = dir,
				kick = AvatarBody.IsKick(fist, impulse),
				fist = fist
			};
			bool knocks = verdict == 2 || (fighter && fighter.WouldKnockOut(localBodies[bone], impulse, speed, info.Source.PlayerId, hit.kick, hit.fist));
			if (!fist && reachAll)
			{
				float near = WasThere(bone, local, world, out float ago);
				if (near < float.MaxValue && near > hitLoose)
				{
					Player.Note("dodged a hit from player " + info.Source.PlayerId + " on " + Names[bone] + ": you were never within " + near.ToString("F2") + " m of where their screen hit you" + (verdict == 2 ? " (their KO is overturned)" : ""));
					if (verdict == 2)
					{
						RPC_Overturn(info.Source);
					}
					return;
				}
				if (near < float.MaxValue && near > hitRewind)
				{
					TakeHit(hit, " (not a fist, you were within " + near.ToString("F2") + " m of it " + (ago * 1000f).ToString("F0") + " ms ago)");
				}
				else
				{
					TakeHit(hit, "");
				}
				return;
			}
			float gap = -1f;
			if (fist && (!knocks || reachAll))
			{
				gap = FistGap(hit);
			}
			if (gap > hitReach)
			{
				if (WasThere(bone, local, world, out float ago) <= hitRewind)
				{
					TakeHit(hit, " (you were there " + (ago * 1000f).ToString("F0") + " ms ago, where their screen showed you)");
					return;
				}
				hit.best = gap;
				pending.Add(hit);
				return;
			}
			TakeHit(hit, "");
		}

		private void Remember()
		{
			if (!HasStateAuthority || localBodies == null)
			{
				return;
			}
			int first = pastNext * Bones;
			for (int i = 0; i < Bones; i++)
			{
				if (localBodies[i])
				{
					pastPose[first + i] = localBodies[i].transform.localToWorldMatrix;
				}
			}
			pastAt[pastNext] = Time.time;
			pastNext = (pastNext + 1) % Past;
		}

		private float WasThere(int bone, Vector3 local, Vector3 world, out float ago)
		{
			ago = 0f;
			float window = Mathf.Clamp(viewLag + rtt + hitSlack, hitWait, hitWaitMax);
			float best = float.MaxValue;
			for (int i = 0; i < Past; i++)
			{
				float age = Time.time - pastAt[i];
				if (pastAt[i] <= 0f || age > window)
				{
					continue;
				}
				float distance = Vector3.Distance(pastPose[i * Bones + bone].MultiplyPoint3x4(local), world);
				if (distance < best)
				{
					best = distance;
					ago = age;
				}
			}
			return best;
		}

		private float FistGap(PendingHit hit)
		{
			Rigidbody body = localBodies[hit.bone];
			if (!body)
			{
				return -1f;
			}
			Vector3 at = body.transform.TransformPoint(hit.local);
			foreach (NetworkPlayer a in avatars)
			{
				if (a == this || !a.Object || !a.Object.IsValid)
				{
					continue;
				}
				if (a.Object.StateAuthority == hit.source && a.bones != null)
				{
					return Mathf.Min(Vector3.Distance(a.bones[5].position, at), Vector3.Distance(a.bones[8].position, at));
				}
			}
			return -1f;
		}

		private float Unpushed(PendingHit hit, Hit result)
		{
			if (result == Hit.Blocked || hit.impulse <= 0f)
			{
				return 1f;
			}
			float fresh;
			if (Mathf.Abs(hit.at - pushAt) < 0.12f && hit.source.PlayerId == pushBy)
			{
				fresh = Mathf.Max(hit.impulse - pushed, 0f);
				pushed = Mathf.Max(pushed, hit.impulse);
				pushAt = Mathf.Max(pushAt, hit.at);
			}
			else
			{
				if (hit.at < pushAt)
				{
					return 1f;
				}
				fresh = hit.impulse;
				pushed = hit.impulse;
				pushAt = hit.at;
			}
			pushBy = hit.source.PlayerId;
			return fresh / hit.impulse;
		}

		private void SettleHits()
		{
			for (int i = pending.Count - 1; i >= 0; i--)
			{
				PendingHit hit = pending[i];
				float gap = FistGap(hit);
				if (gap >= 0f)
				{
					hit.best = Mathf.Min(hit.best, gap);
				}
				pending[i] = hit;
				float age = Time.time - hit.at;
				if (gap >= 0f && gap <= hitReach)
				{
					pending.RemoveAt(i);
					TakeHit(hit, " (landed " + (age * 1000f).ToString("F0") + " ms after it arrived, when their fist reached you here)");
				}
				else if (age > Mathf.Clamp(viewLag + hitSlack, hitWait, hitWaitMax))
				{
					pending.RemoveAt(i);
					if (hit.best <= hitLoose)
					{
						TakeHit(hit, " (their fist only came within " + hit.best.ToString("F2") + " m here)", false);
					}
					else
					{
						Player.Note("dodged a hit from player " + hit.source.PlayerId + " on " + Names[hit.bone] + ": their fist never came within " + hit.best.ToString("F2") + " m of you here" + (hit.verdict == 2 ? " (their KO is overturned)" : ""));
						if (hit.verdict == 2)
						{
							RPC_Overturn(hit.source);
						}
					}
				}
			}
		}

		private void TakeHit(PendingHit hit, string why, bool touched = true)
		{
			Rigidbody body = localBodies[hit.bone];
			if (!body || (drown && drown.drowned))
			{
				if (hit.verdict == 2)
				{
					RPC_Overturn(hit.source);
				}
				return;
			}
			int verdict = hit.verdict;
			float impulse = hit.impulse;
			float speed = hit.speed;
			HitsTaken++;
			Vector3 at = body.transform.TransformPoint(hit.local);
			bool repeat = !fighter || fighter.knockedOut || fighter.SameStrike(hit.at, hit.source.PlayerId);
			Hit result = Hit.None;
			if (fighter)
			{
				result = fighter.TakeHit(body, impulse, speed, at, verdict, hit.at, hit.source.PlayerId, hit.kick, hit.fist, touched);
			}
			float fresh = touched ? Unpushed(hit, result) : 0f;
			float push = hit.kick ? Mathf.Min(impulse, AvatarBody.KickMin) : impulse;
			body.AddForceAtPosition(hit.dir.normalized * push * hitPush * (!hit.kick || fresh >= 1f ? fresh : 0f), at, ForceMode.Impulse);
			if (rig && fresh > 0f && impulse >= kickFrom && result != Hit.Blocked)
			{
				rig.Shove(hit.dir.normalized * impulse * hitKick * fresh);
			}
			bool blocked = result == Hit.Blocked;
			if (touched && !repeat && hit.kick && speed >= fighter.minSpeed && !fighter.Brush(true, speed))
			{
				blocked = false;
				if (result != Hit.None)
				{
					if (result == Hit.Blocked)
					{
						blocked = true;
					}
					else
					{
						fighter.Launch(hit.dir, impulse);
					}
				}
			}
			bool overturned = false;
			if (blocked && (verdict == 1 || !fighter.IsArm(body)))
			{
				RPC_Blocked(at, hit.source, verdict != 1);
			}
			else if (verdict == 2 && result != Hit.KnockedOut)
			{
				RPC_Overturn(hit.source);
				overturned = true;
			}
			if (!overturned && verdict == 0 && fighter && speed >= fighter.minSpeed && (hit.bone > 8 || ((1 << hit.bone) & 0x1B0) == 0))
			{
				RPC_Landed(hit.bone, hit.local, hit.source);
			}
			lastHit = Names[hit.bone] + " " + impulse.ToString("F1") + " Ns at " + speed.ToString("F1") + " m/s -> " + result;
			Player.Note("hit taken from player " + hit.source.PlayerId + ": " + lastHit + " (verdict " + verdict + ")" + why + (fresh < 1f ? " (pushed " + (fresh * 100f).ToString("F0") + "%)" : "") + (fighter ? " | " + fighter.lastHit : ""));
			lastTaken = lastHit;
			taken++;
		}

		[Rpc(RpcSources.StateAuthority, RpcTargets.Proxies)]
		public void RPC_Teeth(int count)
		{
			if (bones != null && bones[1])
			{
				Account.Spray(bones[1], Mathf.Clamp(count, 1, 40), true);
			}
		}

		[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
		public void RPC_Go(string game)
		{
			NetworkManager manager = FindFirstObjectByType<NetworkManager>();
			if (manager)
			{
				manager.Follow(game, false);
			}
		}

		[Rpc(RpcSources.StateAuthority, RpcTargets.Proxies)]
		public void RPC_Blocked(Vector3 at, PlayerRef by, NetworkBool overturned)
		{
			if (Runner.LocalPlayer == by && (!overturned || !copy || !copy.Overturn("they were guarding on their side")))
			{
				return;
			}
			Player.BlockAt(at);
		}

		[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
		public void RPC_Glass(int pane, Vector3 point, Vector3 push)
		{
			Glass.Break(pane, point, push);
		}

		[Rpc(RpcSources.StateAuthority, RpcTargets.Proxies)]
		public void RPC_Bolt(int slot, Vector3 spot)
		{
			Storm.Told(slot, spot);
		}

		[Rpc(RpcSources.StateAuthority, RpcTargets.Proxies)]
		public void RPC_Overturn(PlayerRef by)
		{
			if (Runner.LocalPlayer == by && copy)
			{
				copy.Overturn("their side did not knock them out");
			}
		}

		public static Transform[] Bind(Transform root)
		{
			Transform[] all = root.GetComponentsInChildren<Transform>(true);
			Transform[] found = new Transform[Bones];
			for (int i = 0; i < Bones; i++)
			{
				foreach (Transform part in all)
				{
					if (part.name == Names[i])
					{
						found[i] = part;
						break;
					}
				}
				if (!found[i])
				{
					return null;
				}
			}
			return found;
		}

		public static void Read(Transform[] from, Vector3[] pos, Quaternion[] rot)
		{
			for (int i = 0; i < Bones; i++)
			{
				pos[i] = from[i].position;
				rot[i] = from[i].rotation;
			}
		}

		public static void Apply(Transform[] to, Vector3[] pos, Quaternion[] rot)
		{
			for (int i = 0; i < Bones; i++)
			{
				to[i].position = pos[i];
				to[i].rotation = rot[i];
			}
		}
	}
}
