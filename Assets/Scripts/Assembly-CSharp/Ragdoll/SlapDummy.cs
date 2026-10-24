using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Ragdoll
{
	[RequireComponent(typeof(Player))]
	public class SlapDummy : MonoBehaviour
	{
		public enum Task
		{
			Fight = 0,
			ClimbRope = 1,
			Vault = 2
		}

		public enum Kind
		{
			Dummy = 0,
			Small = 1,
			Normal = 2,
			Big = 3,
			Fat = 4
		}

		public class Bump : MonoBehaviour
		{
			[HideInInspector]
			public SlapDummy dummy;

			private void OnCollisionEnter(Collision c)
			{
				if (!dummy || !c.rigidbody || c.rigidbody.transform.IsChildOf(dummy.transform) || c.rigidbody.GetComponentInParent<Rope>())
				{
					return;
				}
				Rigidbody mine = GetComponent<Rigidbody>();
				if (dummy.npc)
				{
					dummy.Touched(mine, c);
					return;
				}
				if (dummy.Punching)
				{
					if (dummy.IsYou(c.rigidbody) && dummy.IsPunchArm(mine) && dummy.SwingLive)
					{
						float speed = dummy.SpeedBefore(mine);
						dummy.lastLandedBy = base.name + " on " + c.rigidbody.name;
						dummy.lastLandedSpeed = speed;
						if (dummy.Landed(c.rigidbody, c.impulse.magnitude, speed, mine.position))
						{
							dummy.EndSwing();
						}
					}
					return;
				}
				if (dummy.Recovering)
				{
					return;
				}
				bool fist = dummy.IsFist(c.rigidbody);
				Rigidbody by;
				if (fist)
				{
					by = c.rigidbody;
				}
				else
				{
					if (c.rigidbody.GetComponentInParent<LooseHat>())
					{
						return;
					}
					by = dummy.Wielder(c.rigidbody);
				}
				if (!by)
				{
					return;
				}
				float hitSpeed = dummy.SpeedBefore(c.rigidbody);
				if (!fist && hitSpeed < AvatarBody.PropSpeed)
				{
					return;
				}
				dummy.lastHitBy = c.rigidbody.name;
				dummy.Struck(mine, fist ? c.impulse.magnitude : AvatarBody.PropImpulse(c.rigidbody.mass, hitSpeed), hitSpeed, c.rigidbody.position, by);
			}
		}

		[Header("Npc")]
		public Kind kind;

		[Header("Neutral")]
		public bool neutral;

		public bool aggressive;

		public bool limpWhenGrabbed;

		public bool climbable = true;

		[Header("Blocking")]
		public bool blocks = true;

		public float blockRange = 0.8f;

		public float blockSpeed = 2f;

		[Header("Fighting")]
		public bool fight = true;

		public float sightRange = 12f;

		public float reach = 0.4f;

		public float punchFrom = 0.8f;

		public float walkSpeed = 1.1f;

		public float standBand = 0.2f;

		public float leadMax = 0.45f;

		public float punchEvery = 2f;

		public float punchTime = 0.9f;

		public float punchGuard = 0.5f;

		public float stepBack = 1.4f;

		public float stepBackFor = 1.5f;

		public float downReach = 1.6f;

		public float hitRange = 0.27f;

		[Range(0f, 1f)]
		public float airSteer = 0.4f;

		[Header("Swing")]
		public float windTo = 140f;

		public float through = 30f;

		public float dwell = 0.3f;

		public float swingReach = 0.56f;

		public float swingCock = 0.3f;

		public float swingPast = 0.12f;

		public float swingRaise = 0.38f;

		public float swingDrop = 0.15f;

		public float swingSlop = 25f;

		public float armLoose = 0.6f;

		public bool swingFromShoulder;

		public float shoulderLead = 0.25f;

		public float idleArmTone = 0.45f;

		public float recover = 0.6f;

		public float returnSlew = 1.4f;

		public float raiseSlew = 1.1f;

		public float raiseTime = 1.2f;

		public float raiseForce = 0.08f;

		public float aimSlew = 150f;

		[Range(0.1f, 0.8f)]
		public float wind = 0.35f;

		public float windLift = 0.22f;

		[Header("Hands")]
		public float guardUp = 0.1f;

		public float guardOut = 0.24f;

		public float guardIn = 0.03f;

		public float armReach = 0.5f;

		public float handSlew = 3f;

		public float swingSlew = 5f;

		public float handSpring = 9000f;

		public float handForce = 1500f;

		public Vector3 wristTilt;

		[Range(0f, 1f)]
		public float fist = 1f;

		[Header("Pose")]
		public bool posed;

		public Vector3 guardPosL;

		public Vector3 guardPosR;

		public Quaternion guardRotL = Quaternion.identity;

		public Quaternion guardRotR = Quaternion.identity;

		[Header("Climbing")]
		public Task task;

		public bool followUp;

		public float followHeight = 0.8f;

		public float ropeNear = 3f;

		public float ropeStep = 1.4f;

		public float ropeStand = 0.15f;

		public float grabAbove = 0.35f;

		public float vaultRaise = 0.5f;

		public float vaultPull = 0.9f;

		public float vaultWalk = 0.6f;

		public float lookUp = -70f;

		public float lookDown = 35f;

		[HideInInspector]
		public int landings;

		[HideInInspector]
		public float lastHit;

		[HideInInspector]
		public float lastHitSpeed;

		[HideInInspector]
		public float maxHit;

		[HideInInspector]
		public float maxHitSpeed;

		[HideInInspector]
		public float lastLanded;

		[HideInInspector]
		public float lastLandedSpeed;

		[HideInInspector]
		public string lastHitBy = "";

		[HideInInspector]
		public string maxHitBy = "";

		[HideInInspector]
		public string lastLandedBy = "";

		[HideInInspector]
		public StringBuilder hitLog = new StringBuilder();

		public static readonly List<SlapDummy> alive = new List<SlapDummy>();

		private Player body;

		private Player targetBody;

		private Player self;

		private Player target;

		private Player fighter;

		private Player[] others;

		private float nextOthers;

		private float aggroAt;

		private int wasRescues;

		private Grabbable grabbable;

		private bool blockRight;

		private bool blocking;

		private Vector3 blockPoint;

		private Player rig;

		private Player glove;

		private Rope rope;

		private HandGrab[] grabs;

		private HandGrab[] yourGrabs;

		private Task doing;

		private int climbStep;

		private bool punchRight;

		private bool punchSpent;

		private bool closing;

		private bool regrabbed;

		private bool wasSwinging;

		private bool nextRight;

		private bool flung;

		private bool clingThis;

		private bool jabSpent;

		private bool jabbing;

		private float punchPower = 1f;

		private float clingUntil;

		private float clingAt;

		private ConfigurableJoint clingJoint;

		private const float ClingBreak = 2500f;

		private const float ClingReach = 0.14f;

		private const float HugReach = 0.5f;

		private const float HugBreak = 9000f;

		private const float JabEvery = 0.3f;

		private const float Fling = 5f;

		private const float WindPull = 60f;

		private const float StrikePull = 90f;

		private const float Twisting = 25f;

		private const float StuckFor = 1.5f;

		private const float StuckGain = 0.3f;

		private const float LeapApex = 0.9f;

		private const float LeapTop = 3.4f;

		private const float NpcSteps = 0.2f;

		private float stuckFrom = -1f;

		private float stuckAt;

		private bool leaping;

		private float slop;

		private float swungAt = -99f;

		private float guardFrom = -99f;

		private Vector3 aimL;

		private Vector3 aimR;

		private Player drive;

		private bool wasFlopped;

		private bool handKnownL;

		private bool handKnownR;

		private float nextPunch;

		private float punchUntil;

		private float backUntil;

		private float floppedAt;

		private float nextGetUp;

		private float climbAt;

		private float grabbedAt;

		private float grabbedUntil;

		private Vector3 handLocalL;

		private Vector3 handLocalR;

		private Vector3 faceLocal = Vector3.forward;

		private bool npc;

		private int id;

		private static int made;

		private float power = 1f;

		private float push = 1f;

		private float getUp = 0.8f;

		private float rest = 1f;

		private float burstGap = 0.1f;

		private int burstMin = 1;

		private int burstMax = 1;

		private int burstLeft;

		private int kos;

		private float koAt = -99f;

		private float pounceY;

		private static readonly List<Collider> partCols = new List<Collider>();

		private Player.Babble mouth;

		private float talkUntil = -1f;

		private float talkNext;

		private const float TalkMin = 0.5f;

		private const float TalkMax = 1.6f;

		private const float QuietMin = 2.5f;

		private const float QuietMax = 6f;

		public const float BlabVolume = 0.6f;

		private ConfigurableJoint haul;

		private int haulWay = -1;

		private bool haulNow;

		private float haulAt;

		private float claimAt;

		private float claimKey = -1f;

		private float quitKey = -1f;

		private static float tossAt = -99f;

		private int haulMoves;

		private Player tossed;

		private Collider[] passMine;

		private Collider[] passTheirs;

		private static SlapDummy hauler;

		private static SlapDummy helper;

		private static float scanKey = -1f;

		private static Rigidbody scanFloor;

		private static bool scanOnBody;

		private static readonly float[] brinkAt = new float[16];

		private static readonly Vector3[] standAt = new Vector3[16];

		private static readonly Vector3[] landAt = new Vector3[16];

		private static readonly Vector3[] edgeAt = new Vector3[16];

		private static float spentKey = -1f;

		private bool gone;

		private bool wasKO;

		private float goneAt;

		private bool pouncing;

		private bool launched;

		private bool spentL;

		private bool spentR;

		private bool slamSpent;

		private bool scaling;

		private bool scaleOver;

		private Vector3 scaleTop;

		private Vector3 scaleWay;

		private float scaleFaceY;

		private float scaleBest;

		private float scaleBestAt;

		private float scaleRig;

		private const float ClimbTop = 4.5f;

		private const float ClimbFace = 1.2f;

		private const float ClimbUp = 2.6f;

		private const float ClimbHug = 0.6f;

		private const float ClimbStall = 0.8f;

		private const float OverSpeed = 2.2f;

		private const float ClearSpace = 0.5f;

		private static readonly Collider[] roomHits = new Collider[12];

		private float pounceAt;

		private float nextPounce;

		private float overUntil;

		private bool shoulderKnown;

		private bool apart;

		private Vector3 shoulderL;

		private Vector3 shoulderR;

		private float armLen = 0.875f;

		private readonly Rigidbody[] aimParts = new Rigidbody[4];

		private Player aimFor;

		private const float SlapImpulse = 13.5f;

		private const float SlapSpeed = 4.5f;

		private const float SlapMin = 2f;

		private const float Linger = 1.2f;

		private const float HeldKo = 9f;

		private const float LeapFar = 6f;

		private const float GapFar = 8f;

		private const float GapReach = 9f;

		private const float GapStep = 0.4f;

		private const float GapLip = 0.3f;

		private const float GapFloor = 40f;

		private Vector3 leapAt;

		private Vector3 leapBase;

		private bool leapSpot;

		private const float PunchBonus = 0.8f;

		private const float GapStepDown = 0.6f;

		private static readonly RaycastHit[] gapHits = new RaycastHit[12];

		private const int Ways = 16;

		private const float HaulNear = 0.5f;

		private const float HaulCrouch = 0.5f;

		private const float HaulPace = 0.6f;

		private const float HaulSpring = 100000f;

		private const float HaulDamp = 5000f;

		private const float HaulStretch = 0.35f;

		private const float HaulMax = 10f;

		private const float HaulFar = 1.2f;

		private const float HaulSide = 0.3f;

		private const float HaulArrive = 0.3f;

		private const float TossOver = 0.3f;

		private const float TossAir = 1.2f;

		private const float HaulDeep = 1.1f;

		private const float FetchMax = 3f;

		private const float BigGrab = 0.22f;

		private const float HaulRing = 2.5f;

		private const float Settled = 0.5f;

		private const float LipClear = 0.4f;

		private const float HaulAssist = 15f;

		private const float SmallSolo = 0.25f;

		private const float SmallTeam = 0.65f;

		private const float BigKoSpeed = 8.5f;

		private const float BigKoLift = 3f;

		private float haulLean;

		private const float PounceNear = 0.9f;

		private const float PounceFar = 2.2f;

		private const float PounceLift = 1f;

		private const float PounceLead = 0.2f;

		private const float Separate = 0.8f;

		private const float Ledge = 2.5f;

		private readonly Dictionary<Rigidbody, Vector3[]> before = new Dictionary<Rigidbody, Vector3[]>();

		public float impactHold = 0.05f;

		
		public Task Doing => doing;

		public int ClimbStep => climbStep;

		public bool Punching => Time.time < punchUntil;

		public bool Recovering => Time.time < punchUntil + punchGuard;

		public bool Walking => rig && rig.testWish.sqrMagnitude > 0f;

		public bool Blocking => blocking;

		public bool PunchRight => punchRight;

		public bool Pouncing => pouncing;

		public bool Scaling => scaling;

		public bool Clinging => clingJoint != null;

		public bool Hauling => haul != null;

		public bool Talking => Time.time < talkUntil;

		public bool Quiet { get; private set; }

		public bool Jabbing => jabbing;

		public bool Dressed { get; private set; }

		public int Kos
		{
			get
			{
				return kos;
			}
			set
			{
				kos = value;
			}
		}

		public bool Gone => gone;

		public Player Target => target;

		public Player Body => body;

		public bool SwingLive
		{
			get
			{
				if (!(Time.time < punchUntil))
				{
					return false;
				}
				return Mathf.Clamp01(1f - (punchUntil - Time.time) / Mathf.Max(punchTime, 0.01f)) >= wind;
			}
		}

		public static float Pitch(Kind k)
		{
			switch (k)
			{
				case Kind.Small:
					return 1.45f;
				case Kind.Normal:
					return 1f;
				case Kind.Big:
					return 0.7f;
				case Kind.Fat:
					return 1.25f;
				default:
					return 1f;
			}
		}

		public static bool Hauls(Player p)
		{
			if (!hauler || !hauler.isActiveAndEnabled || hauler.target != p)
			{
				return false;
			}
			if (hauler.haul)
			{
				return true;
			}
			return Time.time < hauler.claimAt + FetchMax;
		}

		public bool IsYou(Rigidbody rb)
		{
			if (!rb || !target)
			{
				return false;
			}
			return rb.transform.IsChildOf(target.transform);
		}

		public bool IsPunchArm(Rigidbody rb)
		{
			if (!rb || !body)
			{
				return false;
			}
			if (punchRight)
			{
				return rb == body.rightHand || rb == body.rightForearm;
			}
			return rb == body.leftHand || rb == body.leftForearm;
		}

		private bool IsArm(Rigidbody rb)
		{
			if (!rb || !body)
			{
				return false;
			}
			return rb == body.rightHand || rb == body.rightForearm || rb == body.leftHand || rb == body.leftForearm;
		}

		public bool IsFist(Rigidbody rb)
		{
			if (!rb)
			{
				return false;
			}
			Player p = rb.GetComponentInParent<Player>();
			if (!p || p == body || p.dummy || p.Ragdolled())
			{
				return false;
			}
			return rb == p.leftHand || rb == p.rightHand || rb == p.leftForearm || rb == p.rightForearm;
		}

		public Rigidbody Wielder(Rigidbody rb)
		{
			if (!rb || rb.GetComponentInParent<Player>())
			{
				return null;
			}
			if (npc)
			{
				return Wields(target, rb);
			}
			if (others != null)
			{
				Player[] array = others;
				foreach (Player o in array)
				{
					if (o && o != body)
					{
						Rigidbody w = Wields(o, rb);
						if (w)
						{
							return w;
						}
					}
				}
			}
			return null;
		}

		private static Rigidbody Wields(Player o, Rigidbody rb)
		{
			if (!o)
			{
				return null;
			}
			HandGrab left = o.leftHand ? o.leftHand.GetComponent<HandGrab>() : null;
			if (left && left.Wields(rb))
			{
				return o.leftHand;
			}
			HandGrab right = o.rightHand ? o.rightHand.GetComponent<HandGrab>() : null;
			if (right && right.Wields(rb))
			{
				return o.rightHand;
			}
			return null;
		}

		public void Aggro(Rigidbody by)
		{
			aggressive = true;
			aggroAt = Time.time;
			if (npc || target || !by)
			{
				return;
			}
			Player p = by.GetComponentInParent<Player>();
			if (p && p != self)
			{
				target = p;
				targetBody = p;
			}
		}

		public void ResetHits()
		{
			maxHit = 0f;
			maxHitBy = "";
			lastHit = 0f;
			lastHitBy = "";
			hitLog.Length = 0;
		}

		private void OnEnable()
		{
			if (!alive.Contains(this))
			{
				alive.Add(this);
			}
		}

		private void Start()
		{
			grabbable = GetComponent<Grabbable>();
			if (!grabbable)
			{
				grabbable = gameObject.AddComponent<Grabbable>();
			}
			grabbable.pressWhileTouching = true;
			grabbable.holdsYou = climbable;
			grabbable.reach = 0.2f;
			grabbable.gripForce = 1200f;
			body = GetComponent<Player>();
			self = GetComponent<Player>();
			fighter = Player.Of(this);
			if (!body || !self || !body.pelvis)
			{
				enabled = false;
				return;
			}
			rig = self;
			npc = kind != Kind.Dummy;
			if (kind != Kind.Dummy)
			{
				Setup();
			}
			if (rig)
			{
				rig.grabCursor = false;
				rig.ignoreInput = true;
				rig.aiOn = true;
				rig.aiYaw = rig.playSpace.eulerAngles.y;
				rig.speed = walkSpeed;
				rig.airControl = airSteer;
				rig.maxSeparation = 0.8f;
				self.handSpring = handSpring;
				self.handForce = handForce;
				self.elbowFromHand = 0f;
				if (!npc)
				{
					self.flopSpeed = 5.5f;
				}
				drive = GetComponent<Player>();
				if (drive)
				{
					drive.armDamp = 0.5f;
					drive.legTone = 0.5f;
					drive.driveLegDamp = 2.5f;
				}
				self.holdOff = true;
			}
			glove = GetComponent<Player>();
			if (glove)
			{
				glove.fist = fist;
			}
			Rigidbody[] bodies = body.bodies;
			foreach (Rigidbody rb in bodies)
			{
				if (rb)
				{
					rb.gameObject.AddComponent<Bump>().dummy = this;
				}
			}
			if (!npc)
			{
				Player[] all = UnityEngine.Object.FindObjectsByType<Player>(FindObjectsSortMode.None);
				Player found = null;
				foreach (Player p in all)
				{
					if (p != self)
					{
						found = p;
						break;
					}
				}
				if (!found)
				{
					return;
				}
				target = found;
				targetBody = found;
			}
			Retarget();
		}

		private void Setup()
		{
			followUp = false;
			task = Task.Fight;
			id = 1001 + made++;
			aggressive = true;
			sightRange = 40f;
			idleArmTone = 0.35f;
			switch (kind)
			{
				case Kind.Fat:
					walkSpeed = 1.9f;
					self.turnRate = 300f;
					fist = 0f;
					reach = 0.35f;
					punchFrom = 0.15f;
					punchTime = 0.5f;
					stepBack = 0.9f;
					stepBackFor = 0.5f;
					wind = 0.35f;
					windTo = 110f;
					through = 30f;
					swingCock = 0.15f;
					swingPast = 0.1f;
					swingRaise = 0.15f;
					swingDrop = 0.05f;
					swingSlew = 5f;
					swingSlop = 30f;
					burstMin = 1;
					burstMax = 2;
					rest = 0.9f;
					burstGap = 0.15f;
					self.koDamage = 50f;
					blocks = false;
					self.knockoutTime = KoTime(kind);
					self.guardLeft = 0;
					self.guardHits = 0;
					self.flopSpeed = 9f;
					self.sturdy = 3f;
					getUp = 1f;
					handSpring = 8000f;
					handForce = 1500f;
					break;
				case Kind.Big:
					walkSpeed = 1.4f;
					self.turnRate = 120f;
					fist = 1f;
					reach = 0.95f;
					punchFrom = 0.35f;
					punchTime = 1.1f;
					stepBack = 1.2f;
					stepBackFor = 0.6f;
					wind = 0.5f;
					windTo = 160f;
					through = 45f;
					swingCock = 0.35f;
					swingPast = 0.2f;
					swingRaise = 0.5f;
					swingDrop = 0.25f;
					swingSlew = 6.5f;
					swingSlop = 15f;
					burstMin = 1;
					burstMax = 1;
					rest = 0.9f;
					burstGap = 0f;
					self.koDamage = 60f;
					blocks = false;
					self.knockoutTime = KoTime(kind);
					self.guardLeft = 0;
					self.guardHits = 0;
					self.flopSpeed = 10f;
					self.sturdy = 2.5f;
					getUp = 1.2f;
					handSpring = 15000f;
					handForce = 3000f;
					break;
				case Kind.Small:
					walkSpeed = 3.4f;
					self.turnRate = 480f;
					fist = 0f;
					reach = 0.3f;
					punchFrom = 0.15f;
					punchTime = 0.32f;
					stepBack = 0.9f;
					stepBackFor = 0.5f;
					wind = 0.3f;
					windTo = 90f;
					through = 30f;
					swingCock = 0.15f;
					swingPast = 0.1f;
					swingRaise = 0.12f;
					swingDrop = 0.05f;
					swingSlew = 6f;
					swingSlop = 25f;
					burstMin = 3;
					burstMax = 5;
					rest = 0.5f;
					burstGap = 0.08f;
					self.koDamage = 22f;
					blocks = false;
					self.knockoutTime = KoTime(kind);
					self.guardLeft = 0;
					self.guardHits = 0;
					self.flopSpeed = 5f;
					self.sturdy = 0.8f;
					getUp = 0.4f;
					handSpring = 6000f;
					handForce = 800f;
					break;
				default:
					walkSpeed = 2.2f;
					self.turnRate = 260f;
					fist = 0.25f;
					reach = 0.65f;
					punchFrom = 0.25f;
					punchTime = 0.6f;
					stepBack = 1.2f;
					stepBackFor = 1f;
					wind = 0.35f;
					windTo = 140f;
					through = 30f;
					swingCock = 0.3f;
					swingPast = 0.12f;
					swingRaise = 0.38f;
					swingDrop = 0.15f;
					swingSlew = 6f;
					swingSlop = 25f;
					burstMin = 2;
					burstMax = 3;
					rest = 0.7f;
					burstGap = 0.1f;
					self.koDamage = 40f;
					blocks = true;
					self.knockoutTime = KoTime(kind);
					self.guardLeft = 1;
					self.guardHits = 1;
					self.flopSpeed = 6.5f;
					self.sturdy = 1f;
					getUp = 0.8f;
					handSpring = 9000f;
					handForce = 1500f;
					break;
			}
			Strength(kind, out power, out push);
			climbable = false;
			if (grabbable)
			{
				grabbable.holdsYou = false;
			}
			self.stepVolume = 0.2f;
			Shoulders();
			Outfit();
		}

		private void Outfit()
		{
			Cosmetics cosmetics = self.cosmetics;
			for (int i = 0; i < Cosmetics.Slots.Length; i++)
			{
				List<Cosmetics.Item> list = cosmetics.List(i);
				string item = "";
				if (list.Count >= 1 && Random.value < (i == 0 ? 0.8f : (i == 3 ? 0.5f : 0.6f)))
				{
					item = list[Random.Range(0, list.Count)].name;
				}
				self.Wear(i, item);
			}
			Dressed = true;
		}

		public void End(string why)
		{
			if (npc && !gone)
			{
				Vanish(why);
			}
		}

		public static bool Hazard(Vector3 p)
		{
			if (Train.Escape(p).sqrMagnitude > 0f || global::Ragdoll.Hazard.OutOfRing(p))
			{
				return true;
			}
			Water main = Water.main;
			if (main && !main.acid && p.y < main.SurfaceAt(p) - 0.1f)
			{
				return true;
			}
			foreach (Water acid in Water.acids)
			{
				if (acid && acid.Burns(p + Vector3.up * 0.15f))
				{
					return true;
				}
			}
			return false;
		}

		public void Kicked(Rigidbody bone, float speed, Vector3 from, Vector3 dir)
		{
			if (gone || !bone || speed < AvatarBody.KickSpeed)
			{
				return;
			}
			Hit hit = fighter.TakeHit(bone, Mathf.Clamp(speed * AvatarBody.KickMass, AvatarBody.KickMin, AvatarBody.KickMax), speed, from, -1, Time.time, 2, true, false, true);
			if (haul && (hit == Hit.Hurt || hit == Hit.KnockedOut))
			{
				Drop("hit");
			}
			Vector3 away = dir;
			if (dir.sqrMagnitude < 0.0001f)
			{
				away = new Vector3(body.chest.position.x - from.x, 0f, body.chest.position.z - from.z);
			}
			if (hit >= Hit.Hurt && !fighter.Brush(true, speed))
			{
				fighter.Launch(away, Mathf.Clamp(speed * AvatarBody.KickMass, AvatarBody.KickMin, AvatarBody.KickMax));
			}
			if (Application.isEditor && hitLog.Length < 40000)
			{
				hitLog.Append("      t ").Append(Time.time.ToString("F2")).Append(" KICKED ").Append(kind).Append(" on ").Append(bone.name)
					.Append(" at ")
					.Append(speed.ToString("F1"))
					.Append(" m/s -> ")
					.Append(hit)
					.Append('\n');
			}
		}

		public static float KoTime(Kind k)
		{
			return (k == Kind.Big || k == Kind.Fat) ? 6f : 5f;
		}

		public static void Strength(Kind k, out float power, out float push)
		{
			switch (k)
			{
				case Kind.Small:
					power = 0.9f;
					push = 0.3f;
					break;
				case Kind.Normal:
					power = 1.6f;
					push = 1f;
					break;
				case Kind.Big:
					power = 2.3f;
					push = 1.5f;
					break;
				case Kind.Fat:
					power = 1.1f;
					push = 0.8f;
					break;
				default:
					power = 1f;
					push = 1f;
					break;
			}
		}

		public static Hit Smack(Kind k, Player victim, Rigidbody bone, Vector3 point, Vector3 dir, int by)
		{
			if (!victim || victim.dummy || victim.dead || AvatarBody.peaceful || AvatarBody.truce)
			{
				return Hit.None;
			}
			float power;
			float push;
			switch (k)
			{
				case Kind.Small:
					power = 0.9f;
					push = 0.3f;
					break;
				case Kind.Normal:
					power = 1.6f;
					push = 1f;
					break;
				case Kind.Big:
					power = 2.3f;
					push = 1.5f;
					break;
				case Kind.Fat:
					power = 1.1f;
					push = 0.8f;
					break;
				default:
					power = 1f;
					push = 1f;
					break;
			}
			Rigidbody part = (victim.IsArm(bone) && !victim.guarding && victim.chest) ? victim.chest : bone;
			Hit hit = victim.TakeHit(part, power * SlapImpulse, SlapSpeed, point, -1, Time.time, by, false, true, true, true);
			if (hit == Hit.Hurt || hit == Hit.KnockedOut)
			{
				part.AddForceAtPosition(dir * (push * SlapImpulse), point, ForceMode.Impulse);
				if (push >= 1f)
				{
					victim.Shove(dir * (push * 40.5f));
				}
				Player.HitAt(point, true);
				if (k == Kind.Big)
				{
					if (hit == Hit.Hurt)
					{
						victim.Launch(dir, 45f);
					}
					else
					{
						victim.Launch(dir, 60f, BigKoSpeed, BigKoLift);
					}
				}
				return hit;
			}
			if (hit == Hit.Blocked)
			{
				if (k == Kind.Big)
				{
					victim.Shove(dir * 20f);
				}
				return Hit.Blocked;
			}
			return hit;
		}

		private void Shoulders()
		{
			if (!rig || !rig.playSpace || !body.rightUpperArm || !body.leftUpperArm)
			{
				return;
			}
			Vector3 head = rig.cam ? rig.cam.localPosition : Vector3.up * rig.flatHeight;
			shoulderR = rig.playSpace.InverseTransformPoint(body.rightUpperArm.position) - head;
			shoulderL = rig.playSpace.InverseTransformPoint(body.leftUpperArm.position) - head;
			shoulderL.z = 0f;
			shoulderR.z = 0f;
			shoulderKnown = true;
			float hand = body.rightHand.GetComponent<CapsuleCollider>() ? body.rightHand.GetComponent<CapsuleCollider>().height * body.rightHand.transform.lossyScale.y : 0.15f;
			armLen = Vector3.Distance(body.rightUpperArm.position, body.rightForearm.position) + Vector3.Distance(body.rightForearm.position, body.rightHand.position) + hand;
			float scale = armLen / 0.875f;
			guardUp *= scale;
			guardOut *= scale;
			guardIn *= scale;
			armReach = armLen * 0.85f;
			swingReach = armLen * (kind == Kind.Big ? 1f : 0.95f);
		}

		private void Apart()
		{
			apart = true;
			if (kind == Kind.Normal)
			{
				return;
			}
			Rigidbody[] arms = new Rigidbody[4] { body.leftForearm, body.rightForearm, body.leftHand, body.rightHand };
			Rigidbody[] core = new Rigidbody[2] { body.pelvis, body.chest };
			foreach (Rigidbody arm in arms)
			{
				if (!arm)
				{
					continue;
				}
				Collider[] components = arm.GetComponents<Collider>();
				foreach (Collider a in components)
				{
					foreach (Rigidbody part in core)
					{
						if (!part)
						{
							continue;
						}
						Collider[] components2 = part.GetComponents<Collider>();
						foreach (Collider b in components2)
						{
							Physics.IgnoreCollision(a, b, true);
						}
					}
				}
			}
		}

		private static bool Valid(Player p)
		{
			if (!p || p.dummy || p.dead || p.drowned)
			{
				return false;
			}
			return p.isActiveAndEnabled;
		}

		private void Retarget()
		{
			if (!Valid(target))
			{
				target = Valid(Player.main) ? Player.main : null;
				targetBody = target;
				aimFor = null;
			}
		}

		private void Remember(Rigidbody rb)
		{
			if (!rb)
			{
				return;
			}
			if (!before.TryGetValue(rb, out Vector3[] history))
			{
				history = new Vector3[3];
				before[rb] = history;
			}
			history[2] = history[1];
			history[1] = history[0];
			history[0] = rb.linearVelocity;
		}

		private void RememberHand(Rigidbody hand)
		{
			HandGrab g = hand ? hand.GetComponent<HandGrab>() : null;
			if (!g)
			{
				return;
			}
			if (g.held && !g.holdsYou)
			{
				Remember(g.held);
			}
			else if (g.lastHeld && g.Wields(g.lastHeld))
			{
				Remember(g.lastHeld);
			}
		}

		public float SpeedBefore(Rigidbody rb)
		{
			if (!rb)
			{
				return 0f;
			}
			if (!before.TryGetValue(rb, out Vector3[] history))
			{
				return rb.linearVelocity.magnitude;
			}
			Vector3 average = (history[0] + history[1] + history[2]) / 3f;
			if (average.sqrMagnitude < 1E-06f)
			{
				return 0f;
			}
			Vector3 dir = average.normalized;
			Vector3 sum = Vector3.zero;
			int count = 0;
			for (int i = 0; i < 3; i++)
			{
				if (!(history[i].sqrMagnitude < 1E-06f) && !(Vector3.Dot(dir, history[i].normalized) < 0.2f))
				{
					sum += history[i];
					count++;
				}
			}
			if (count == 0)
			{
				return 0f;
			}
			return (sum / count).magnitude;
		}

		public void Struck(Rigidbody bone, float impulse, float speed, Vector3 from, Rigidbody by, bool punch = false)
		{
			if (gone)
			{
				return;
			}
			if (npc && punch && (by == null || (Player.Of(by) && !Player.Of(by).dummy)))
			{
				impulse += 2.4f;
				speed += PunchBonus;
			}
			lastHit = impulse;
			lastHitSpeed = speed;
			if (impulse > maxHit)
			{
				maxHit = impulse;
				maxHitSpeed = speed;
				maxHitBy = lastHitBy;
			}
			Rigidbody part = bone;
			if (npc && punch && fighter.IsArm(bone) && !fighter.guarding && body.chest)
			{
				part = body.chest;
			}
			Hit hit = fighter.TakeHit(part, impulse, speed, from, -1, npc ? Time.time : -1f, npc ? 1 : -1, false, npc && punch, true);
			if (haul && (hit == Hit.Hurt || hit == Hit.KnockedOut))
			{
				Drop("hit");
			}
			Player hitter = hit >= Hit.Hurt ? Player.Of(by) : null;
			if (hitter)
			{
				hitter.LandPunch(speed);
			}
			if (Application.isEditor && impulse >= 3f && hitLog.Length < 40000)
			{
				hitLog.Append("      t ").Append(Time.time.ToString("F2")).Append(" ON HIM by ").Append(lastHitBy).Append(' ')
					.Append(impulse.ToString("F1"))
					.Append(" Ns at ")
					.Append(speed.ToString("F1"))
					.Append(" m/s -> ")
					.Append(hit)
					.Append(' ')
					.Append(fighter.lastHit)
					.Append('\n');
			}
			if (speed >= fighter.minSpeed && !neutral)
			{
				Aggro(by);
			}
		}

		public bool Landed(Rigidbody yourBone, float impulse, float speed, Vector3 from)
		{
			lastLanded = impulse;
			landings++;
			Player you = Player.Of(yourBone);
			Hit hit = you ? you.TakeHit(yourBone, impulse, speed, from, -1, -1f, -1, false, false, true) : Hit.None;
			if (Application.isEditor && impulse >= 3f && hitLog.Length < 40000)
			{
				hitLog.Append("      t ").Append(Time.time.ToString("F2")).Append(" ON YOU by his ").Append(lastLandedBy).Append(' ')
					.Append(impulse.ToString("F1"))
					.Append(" Ns at ")
					.Append(speed.ToString("F1"))
					.Append(" m/s -> ")
					.Append(hit)
					.Append('\n');
			}
			bool landed = !punchSpent && hit > Hit.Blocked;
			if (landed)
			{
				punchSpent = true;
				backUntil = Time.time + stepBackFor;
			}
			return landed;
		}

		public Hit Slap(Player victim, Rigidbody bone, Vector3 point, Vector3 dir, float measured)
		{
			if (!victim || victim.dummy || victim.dead || gone || AvatarBody.peaceful || AvatarBody.truce)
			{
				return Hit.None;
			}
			Hit hit = Smack(kind, victim, bone, point, dir, id);
			lastLandedSpeed = measured;
			landings++;
			lastLanded = power * SlapImpulse;
			if (Application.isEditor && hitLog.Length < 40000)
			{
				hitLog.Append("      t ").Append(Time.time.ToString("F2")).Append(" ON YOU ").Append(kind).Append(' ')
					.Append(lastLandedBy)
					.Append(" on ")
					.Append(bone.name)
					.Append(" at ")
					.Append(measured.ToString("F1"))
					.Append(" m/s -> ")
					.Append(hit)
					.Append(' ')
					.Append(victim.lastHit)
					.Append('\n');
			}
			return hit;
		}

		private bool Live(Rigidbody mine)
		{
			if (SwingLive && IsPunchArm(mine))
			{
				return true;
			}
			if (pouncing && launched && IsArm(mine))
			{
				return true;
			}
			if (!jabbing)
			{
				return false;
			}
			return mine == body.rightHand || mine == body.rightForearm;
		}

		public void Touched(Rigidbody mine, Collision c)
		{
			if (gone || !c.rigidbody)
			{
				return;
			}
			Vector3 point = c.contactCount > 0 ? c.GetContact(0).point : mine.position;
			if (!Live(mine))
			{
				if (pouncing && launched && !slamSpent && (mine == body.pelvis || mine == body.chest || mine == body.head) && target && (c.rigidbody == target.head || c.rigidbody == target.chest) && c.relativeVelocity.magnitude >= 2f)
				{
					slamSpent = true;
					lastLandedBy = "body slam " + mine.name;
					Slap(target, c.rigidbody, point, Flat(c.rigidbody.position - mine.position).normalized, c.relativeVelocity.magnitude);
					return;
				}
				Player kicker = Player.Of(c.rigidbody);
				if (kicker && kicker != body && !kicker.dummy && kicker.Kicks(c.rigidbody))
				{
					Kicked(mine, c.relativeVelocity.magnitude, c.rigidbody.position, Flat(c.rigidbody.linearVelocity));
					return;
				}
				bool fistHit = IsFist(c.rigidbody);
				Rigidbody by;
				if (fistHit)
				{
					by = c.rigidbody;
				}
				else
				{
					if (c.rigidbody.GetComponentInParent<LooseHat>())
					{
						return;
					}
					by = Wielder(c.rigidbody);
				}
				if (!by)
				{
					return;
				}
				float speed = SpeedBefore(c.rigidbody);
				if (!fistHit)
				{
					if (speed < AvatarBody.PropSpeed)
					{
						return;
					}
					lastHitBy = c.rigidbody.name;
					Struck(mine, AvatarBody.PropImpulse(c.rigidbody.mass, speed), speed, c.rigidbody.position, by, false);
					return;
				}
				lastHitBy = c.rigidbody.name;
				Player puncher = Player.Of(c.rigidbody);
				Rigidbody hand;
				if (puncher && c.rigidbody == puncher.leftForearm)
				{
					hand = puncher.leftHand;
				}
				else if (puncher && c.rigidbody == puncher.rightForearm)
				{
					hand = puncher.rightHand;
				}
				else
				{
					hand = c.rigidbody;
				}
				float swing = puncher ? Mathf.Min(speed, puncher.SwingSpeed(hand, Player.SwingSteps)) : speed;
				Struck(mine, swing * 3f, swing, c.rigidbody.position, by, true);
				return;
			}
			SlapDummy other = IsYou(c.rigidbody) ? null : c.rigidbody.GetComponentInParent<SlapDummy>();
			if (!IsYou(c.rigidbody) && (!other || other == this || !other.npc || other.gone))
			{
				return;
			}
			float handSpeed = SpeedBefore(mine);
			if (handSpeed < SlapMin)
			{
				return;
			}
			bool left = mine == body.leftHand || mine == body.leftForearm;
			if (jabbing)
			{
				if (jabSpent)
				{
					return;
				}
				jabSpent = true;
			}
			else if (pouncing)
			{
				if (left ? spentL : spentR)
				{
					return;
				}
				if (left)
				{
					spentL = true;
				}
				else
				{
					spentR = true;
				}
			}
			else if (punchSpent)
			{
				return;
			}
			lastLandedBy = mine.name;
			if (!other)
			{
				Vector3 flat = Flat(target.chest.position - body.chest.position);
				Vector3 dir = flat.sqrMagnitude > 0.0001f ? flat.normalized : body.chest.transform.forward;
				Hit hit = Slap(target, c.rigidbody, point, dir, handSpeed);
				if (pouncing || jabbing || hit == Hit.None)
				{
					return;
				}
			}
			else
			{
				Vector3 flat2 = Flat(c.rigidbody.position - body.chest.position);
				Vector3 dir2 = flat2.sqrMagnitude > 0.0001f ? flat2.normalized : body.chest.transform.forward;
				other.Struck(c.rigidbody, power * SlapImpulse, SlapSpeed, mine.position, mine, true);
				c.rigidbody.AddForceAtPosition(dir2 * (push * SlapImpulse), point, ForceMode.Impulse);
				if (Application.isEditor && hitLog.Length < 40000)
				{
					hitLog.Append("      t ").Append(Time.time.ToString("F2")).Append(" ON NPC ").Append(other.kind).Append(' ')
						.Append(c.rigidbody.name)
						.Append(" ko ")
						.Append(other.fighter.knockedOut)
						.Append('\n');
				}
				if (pouncing || jabbing)
				{
					return;
				}
			}
			punchSpent = true;
			EndSwing();
		}

		private void FixedUpdate()
		{
			if (!rig || !self)
			{
				return;
			}
			if (npc)
			{
				if (!apart)
				{
					Apart();
				}
				if (Ending())
				{
					return;
				}
				Blab();
				rig.testLean = Vector3.zero;
				if (tossed && (tossed.dead || (!tossed.flopped && !Overlaps())))
				{
					Pass(tossed, false);
					tossed = null;
				}
			}
			if (glove)
			{
				glove.fist = fist;
			}
			Remember(body.leftHand);
			Remember(body.rightHand);
			Remember(body.leftForearm);
			Remember(body.rightForearm);
			if (!npc)
			{
				if (Time.time >= nextOthers)
				{
					nextOthers = Time.time + 2f;
					others = UnityEngine.Object.FindObjectsByType<Player>(FindObjectsSortMode.None);
				}
				Player[] array = others;
				foreach (Player o in array)
				{
					if (o && o != body)
					{
						Remember(o.leftHand);
						Remember(o.rightHand);
						Remember(o.leftForearm);
						Remember(o.rightForearm);
						RememberHand(o.leftHand);
						RememberHand(o.rightHand);
					}
				}
				if (aggressive)
				{
					Player foe = target ? target.GetComponent<Player>() : null;
					if ((foe && foe.knockedOut) || self.rescues != wasRescues)
					{
						aggressive = false;
					}
				}
			}
			else
			{
				Retarget();
				if (target)
				{
					Remember(target.leftHand);
					Remember(target.rightHand);
					Remember(target.leftForearm);
					Remember(target.rightForearm);
					RememberHand(target.leftHand);
					RememberHand(target.rightHand);
				}
			}
			wasRescues = self.rescues;
			self.sweepOff = doing == Task.Fight && SwingLive;
			if (SwingLive)
			{
				swungAt = Time.time;
			}
			float recovered = recover > 0f ? Mathf.Clamp01((Time.time - swungAt) / recover) : 1f;
			if (wasSwinging && !SwingLive)
			{
				self.SoftenArms(recover);
				if (npc && kind == Kind.Big && !punchSpent)
				{
					nextPunch += PunchBonus;
					overUntil = Time.time + 0.4f;
				}
			}
			wasSwinging = SwingLive;
			float raised = raiseTime > 0f ? Mathf.Clamp01((Time.time - guardFrom) / raiseTime) : 1f;
			bool live = SwingLive;
			self.handForce = handForce * Mathf.Lerp(armLoose, 1f, live ? 0f : recovered) * Mathf.Lerp(raiseForce, 1f, raised * raised);
			if (grabs == null)
			{
				grabs = GetComponentsInChildren<HandGrab>(true);
			}
			HandGrab[] array2 = grabs;
			foreach (HandGrab g in array2)
			{
				if (g && g.enabled == (doing == Task.Fight))
				{
					g.enabled = doing != Task.Fight;
				}
			}
			self.holdOff = !npc && doing == Task.Fight && !fighter.knockedOut && !self.flopped && !self.rising && !self.airborne;
			if (yourGrabs == null && limpWhenGrabbed)
			{
				List<HandGrab> list = new List<HandGrab>();
				HandGrab[] all = UnityEngine.Object.FindObjectsByType<HandGrab>(FindObjectsSortMode.None);
				foreach (HandGrab g2 in all)
				{
					if (g2 && !g2.transform.IsChildOf(transform))
					{
						list.Add(g2);
					}
				}
				yourGrabs = list.ToArray();
			}
			bool grabbed = false;
			if (limpWhenGrabbed)
			{
				HandGrab[] array3 = yourGrabs;
				foreach (HandGrab g3 in array3)
				{
					if (g3 && g3.grabbing && g3.held && g3.held.transform.IsChildOf(transform))
					{
						grabbed = true;
						break;
					}
				}
			}
			if (npc && fighter.knockedOut && Held())
			{
				float until = Mathf.Min(Time.time + 1f, koAt + HeldKo);
				self.knockedOutUntil = Mathf.Max(self.knockedOutUntil, until);
				self.riseLockedUntil = Mathf.Max(self.riseLockedUntil, until);
			}
			if (fighter.knockedOut)
			{
				self.limpArmScale = 0.12f;
			}
			else if (npc)
			{
				self.limpArmScale = idleArmTone;
			}
			if (grabbable)
			{
				grabbable.holdsYou = climbable && !fighter.knockedOut && !self.flopped && !self.rising && self.grounded;
			}
			if (grabbed && limpWhenGrabbed)
			{
				self.Knock(Vector3.zero);
				grabbedUntil = Time.time + 0.8f;
			}
			if (haul && Haul())
			{
				return;
			}
			if ((clingJoint || clingUntil > 0f) && Cling())
			{
				return;
			}
			if (scaling && Scale())
			{
				return;
			}
			if (pouncing && Pounce())
			{
				return;
			}
			if (self.flopped)
			{
				if (!wasFlopped)
				{
					floppedAt = Time.time;
				}
				wasFlopped = true;
				float wait = npc ? getUp : 0.8f;
				if (self.grounded && Time.time > floppedAt + wait && Time.time > nextGetUp && Time.time > grabbedUntil && !fighter.knockedOut)
				{
					nextGetUp = Time.time + wait;
					self.Jump(1f);
				}
				rig.testWish = Vector3.zero;
				rig.aiHandL = false;
				rig.aiHandR = false;
				handKnownL = false;
				handKnownR = false;
				guardFrom = Time.time;
				return;
			}
			wasFlopped = false;
			bool calm = npc && (AvatarBody.peaceful || AvatarBody.truce || GameManager.Waiting);
			if (neutral || !fight || !aggressive || !target || !targetBody || calm)
			{
				rig.testWish = Vector3.zero;
				guardFrom = Time.time;
				if (npc)
				{
					fighter.limpWhenIdle = true;
					Hang();
					return;
				}
				fighter.limpWhenIdle = doing == Task.Fight;
				rig.aiHandL = false;
				rig.aiHandR = false;
				handKnownL = false;
				handKnownR = false;
				HangArms();
				return;
			}
			fighter.limpOnRise = true;
			fighter.limpWhenIdle = npc;
			if (Climbing())
			{
				return;
			}
			Vector3 to = Flat(targetBody.pelvis.position - rig.playSpace.position);
			float dist = to.magnitude;
			float gap = Flat(targetBody.chest.position - body.chest.position).magnitude;
			if (dist < 0.01f || dist > sightRange)
			{
				rig.testWish = Vector3.zero;
				Guard();
				return;
			}
			if (!fighter.knockedOut)
			{
				float yaw = Quaternion.LookRotation(to / dist, Vector3.up).eulerAngles.y;
				if (npc && dist > 1.5f && kind == Kind.Small)
				{
					yaw += Mathf.Sin(Time.time * 9.424778f) * 20f;
				}
				if (npc && Time.time < overUntil)
				{
					yaw += 40f;
				}
				if (npc && kind == Kind.Big && dist < reach + 1.5f)
				{
					yaw += Mathf.Asin(Mathf.Clamp(Shoulder(Punching ? punchRight : nextRight).x / dist, -0.8f, 0.8f)) * (0f - Mathf.Rad2Deg);
				}
				if (npc && Punching)
				{
					yaw += Twist();
				}
				rig.aiYaw = yaw;
			}
			bool down = target.flopped || target.rising || (npc && target.knockedOut);
			bool claimed = false;
			bool helping = false;
			bool othersHaul = false;
			if (npc && down)
			{
				claimed = Claim();
				helping = npc && down && !claimed && Help();
				if (npc && down && !claimed && !helping && hauler && hauler != this && hauler.target == target)
				{
					othersHaul = !hauler.gone;
				}
			}
			float range = down ? ((claimed || helping) ? HaulNear : (othersHaul ? HaulRing : downReach)) : (Time.time < backUntil ? stepBack : reach);
			rig.speed = Mathf.Min(walkSpeed, Mathf.Abs(dist - range) / Mathf.Max(Time.deltaTime, Time.fixedDeltaTime));
			float lead = Flat(body.pelvis.position - rig.playSpace.position).magnitude;
			if (gap < range - standBand || dist < range - standBand)
			{
				rig.testWish = Vector3.back;
				closing = false;
			}
			else if (lead > leadMax)
			{
				rig.testWish = Vector3.zero;
			}
			else
			{
				if (!closing && dist > range + standBand)
				{
					closing = true;
				}
				if (dist <= range && closing)
				{
					closing = false;
					rig.testWish = Vector3.zero;
				}
				else if (closing && gap > range)
				{
					rig.testWish = Vector3.forward;
				}
				else
				{
					rig.testWish = Vector3.zero;
				}
			}
			if (npc)
			{
				Steer();
			}
			if (fighter.knockedOut)
			{
				rig.aiHandL = false;
				rig.aiHandR = false;
				return;
			}
			if (npc && !claimed && !helping && Stuck(range))
			{
				return;
			}
			Vector3 face = Flat(rig.playSpace.InverseTransformPoint(targetBody.chest.position));
			if (face.sqrMagnitude > 0.0001f)
			{
				faceLocal = face.normalized;
			}
			if (Punching)
			{
				Throw();
				return;
			}
			if (npc && down)
			{
				if (claimed || helping)
				{
					Fetch();
				}
				else if (kind == Kind.Normal)
				{
					Taunt();
				}
				else
				{
					Guard();
				}
				return;
			}
			Guard();
			if (down)
			{
				return;
			}
			if (npc && (kind == Kind.Fat || kind == Kind.Small) && TryPounce(dist, face))
			{
				return;
			}
			if (Time.time < backUntil)
			{
				return;
			}
			Vector3 aim;
			if (npc)
			{
				if (!self.grounded)
				{
					return;
				}
				aim = AimPoint();
			}
			else
			{
				aim = targetBody.chest.position;
			}
			Vector3 shoulder = rig.playSpace.TransformPoint(Shoulder(!npc || kind != Kind.Big || nextRight));
			if (Flat(aim - shoulder).magnitude > swingReach + punchFrom + (npc ? 0.15f : 0f))
			{
				return;
			}
			if (Time.time < nextPunch)
			{
				return;
			}
			punchSpent = false;
			flung = false;
			punchUntil = Time.time + punchTime;
			punchPower = Random.Range(0.6f, 1.4f);
			slop = Random.Range(0f - swingSlop, swingSlop);
			if (!npc)
			{
				nextPunch = Time.time + punchEvery;
				punchRight = Random.value > 0.5f;
				return;
			}
			if (burstLeft > 0)
			{
				punchRight = !punchRight;
			}
			else
			{
				burstLeft = Random.Range(burstMin, burstMax + 1);
				punchRight = (npc && kind == Kind.Big) ? nextRight : (Random.value > 0.5f);
			}
			burstLeft--;
			nextRight = Random.value > 0.5f;
			if (burstLeft >= 1)
			{
				nextPunch = Time.time + punchTime + burstGap;
				return;
			}
			nextPunch = Time.time + punchTime + rest;
			if (burstLeft == 0)
			{
				backUntil = Time.time + punchTime + stepBackFor;
			}
		}


		private bool Ending()
		{
			if (!gone)
			{
				if (fighter.knockedOut && !wasKO)
				{
					kos++;
					koAt = Time.time;
				}
				wasKO = fighter.knockedOut;
				if (!gone && self.rescues != wasRescues && wasRescues >= 0)
				{
					Vanish("fell");
				}
				if (!gone)
				{
					return false;
				}
			}
			self.knockedOutUntil = Mathf.Max(self.knockedOutUntil, Time.time + 1f);
			self.riseLockedUntil = Mathf.Max(self.riseLockedUntil, Time.time + 1f);
			if (!(Time.time < goneAt + Linger))
			{
				Player.HitAt(body.pelvis.position, true);
				Component root = transform.parent ? (Component)transform.parent : this;
				UnityEngine.Object.Destroy(root.gameObject);
				enabled = false;
			}
			return true;
		}

		private void Vanish(string why)
		{
			gone = true;
			goneAt = Time.time;
			bool left = why == "left";
			Quiet = left;
			if (!left)
			{
				GameManager.Died();
			}
			Drop("gone");
			pouncing = false;
			rig.aiHandL = false;
			rig.aiHandR = false;
			rig.testWish = Vector3.zero;
			fighter.limpWhenIdle = true;
			alive.Remove(this);
			if (Application.isEditor && hitLog.Length < 40000)
			{
				hitLog.Append("      t ").Append(Time.time.ToString("F2")).Append(" GONE ").Append(kind).Append(' ')
					.Append(why)
					.Append('\n');
			}
		}

		private void Steer()
		{
			Vector3 escape = Train.main ? Train.Escape(Feet()) : Vector3.zero;
			if (escape.sqrMagnitude > 0f)
			{
				rig.speed = walkSpeed;
				rig.testWish = Flat(rig.playSpace.InverseTransformDirection(escape)).normalized;
				return;
			}
			if (rig.testWish.sqrMagnitude <= 0f)
			{
				return;
			}
			Vector3 move = rig.playSpace.rotation * rig.testWish.normalized;
			Vector3 probe = body.pelvis.position + Flat(move) * 0.6f + Vector3.up * 0.3f;
			if (!Physics.Raycast(probe, Vector3.down, out RaycastHit hit, 2.5f, ~4, QueryTriggerInteraction.Ignore) || TrainWreck.Desert(hit.rigidbody) || (Hazard(hit.point) && !Hazard(Feet())))
			{
				rig.testWish = Vector3.zero;
				return;
			}
			Vector3 away = Vector3.zero;
			foreach (SlapDummy other in alive)
			{
				if (!other || other == this || !other.body || !other.body.pelvis)
				{
					continue;
				}
				Vector3 off = Flat(body.pelvis.position - other.body.pelvis.position);
				float d = off.magnitude;
				if (d < Separate && d > 0.001f)
				{
					away += off / d * (d / (0f - Separate) + 1f);
				}
			}
			if (away.sqrMagnitude > 0f)
			{
				rig.testWish += rig.playSpace.InverseTransformDirection(away) * 1.5f;
				rig.testWish.y = 0f;
			}
		}

		private Vector3 Feet()
		{
			Vector3 position = body.pelvis.position;
			if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 2f, ~4, QueryTriggerInteraction.Ignore))
			{
				return hit.point;
			}
			return position;
		}

		private bool Stuck(float want)
		{
			float gap = Flat(targetBody.pelvis.position - body.pelvis.position).magnitude;
			float rise = targetBody.pelvis.position.y - body.pelvis.position.y;
			if ((gap <= standBand + want + 0.5f && rise <= 1f) || Time.time < punchUntil || pouncing || !self.grounded || self.rising || stuckFrom < 0f || gap < stuckFrom - StuckGain)
			{
				stuckFrom = gap;
				stuckAt = Time.time;
				return false;
			}
			if (Time.time < stuckAt + StuckFor)
			{
				return false;
			}
			stuckFrom = -1f;
			if (WallTop(out scaleTop, out scaleWay))
			{
				StartScale();
				return true;
			}
			bool leapable = Leapable();
			leapSpot = !leapable;
			if (!leapable && !GapLanding(out leapAt))
			{
				return false;
			}
			pouncing = true;
			launched = false;
			leaping = true;
			clingThis = kind == Kind.Fat || (kind == Kind.Small && Random.value < 0.5f);
			slamSpent = false;
			spentL = false;
			spentR = false;
			pounceAt = Time.time;
			rig.testWish = Vector3.zero;
			if (Application.isEditor && hitLog.Length < 40000)
			{
				hitLog.Append("      t ").Append(Time.time.ToString("F2")).Append(" STUCK, LEAP from ").Append(gap.ToString("F2")).Append(" m, ")
					.Append(rise.ToString("F2"))
					.Append(" m up")
					.Append(leapSpot ? (" to the floor across the gap at " + leapAt.ToString("F1")) : "")
					.Append('\n');
			}
			return true;
		}

		private bool Leapable()
		{
			float rise = target.pelvis.position.y - body.pelvis.position.y;
			if (rise > 3f)
			{
				return false;
			}
			float apex = Mathf.Clamp(Mathf.Max(rise, 0f) + LeapApex, LeapApex, LeapTop);
			float up = Mathf.Sqrt(apex * 19.62f) / 9.81f;
			float down = Mathf.Sqrt(Mathf.Max(apex - rise, 0.05f) * 2f / 9.81f);
			return Flat(target.pelvis.position - body.pelvis.position).magnitude / (up + down) <= LeapFar;
		}

		private bool Held()
		{
			Player main = Player.main;
			if (!main || main == body)
			{
				return false;
			}
			Rigidbody[] bodies = body.bodies;
			foreach (Rigidbody rb in bodies)
			{
				if (rb && main.Holds(rb))
				{
					return true;
				}
			}
			return false;
		}

		private bool Wet()
		{
			Water main = Water.main;
			Vector3 position = body.pelvis.position;
			if (main && !main.acid && position.y < main.SurfaceAt(position) + 0.1f)
			{
				return true;
			}
			foreach (Water acid in Water.acids)
			{
				if (acid && acid.Inside(position) && position.y < acid.SurfaceAt(position) + 0.3f)
				{
					return true;
				}
			}
			return false;
		}

		private bool GapLanding(out Vector3 land)
		{
			land = Vector3.zero;
			leapBase = Feet();
			float baseY = leapBase.y;
			float targetY = target.pelvis.position.y;
			float bodyY = body.pelvis.position.y;
			Vector3 across = Flat(target.pelvis.position - body.pelvis.position);
			float far = Mathf.Min(across.magnitude, GapReach);
			if (far < 0.8f)
			{
				return false;
			}
			float top = Mathf.Max(baseY, target.pelvis.position.y) + 2f;
			if (far < GapStep)
			{
				return false;
			}
			Vector3 dir = across / across.magnitude;
			float cast = top - baseY + 2.5f;
			float low = baseY - 2.5f;
			float high = baseY + LeapTop - GapStep;
			bool gap = false;
			for (float t = GapStep; t <= far; t += GapStep)
			{
				if (GapGround(new Vector3(leapBase.x + dir.x * t, top, leapBase.z + dir.z * t), cast, out RaycastHit hit))
				{
					Rigidbody rb = hit.rigidbody;
					if (hit.normal.y > 0.6f && hit.point.y > low && hit.point.y < high && !Hazard(hit.point) && !TrainWreck.Desert(rb) && (!rb || rb.isKinematic || !(rb.mass < GapFloor)))
					{
						float y = hit.point.y;
						if (!(y < baseY - GapStepDown) || targetY < bodyY - GapStepDown)
						{
							if (!gap && !(y < baseY - GapStepDown) && y < baseY + GapStepDown)
							{
								gap = false;
								continue;
							}
							land = hit.point + dir * GapLip;
							Vector3 over = land + Vector3.up;
							if (Physics.Linecast(body.chest.position, over, out RaycastHit block, ~4, QueryTriggerInteraction.Ignore) && !block.collider.GetComponentInParent<Player>() && !block.collider.GetComponentInParent<NetworkPlayer>() && !block.collider.GetComponentInParent<NetworkNpc>() && !block.collider.GetComponentInParent<Rope>() && (land + Vector3.up - block.point).sqrMagnitude > 0.25f)
							{
								return false;
							}
							float rise = land.y - baseY;
							float apex = Mathf.Clamp(Mathf.Max(rise, 0f) + LeapApex, LeapApex, LeapTop);
							float up = Mathf.Sqrt(apex * 19.62f) / 9.81f;
							float down = Mathf.Sqrt(Mathf.Max(apex - rise, 0.05f) * 2f / 9.81f);
							return Flat(land - leapBase).magnitude / (up + down) <= GapFar;
						}
					}
				}
				gap = true;
			}
			return false;
		}

		private static bool GapGround(Vector3 from, float dist, out RaycastHit hit)
		{
			int count = Physics.RaycastNonAlloc(from, Vector3.down, gapHits, dist, ~4, QueryTriggerInteraction.Ignore);
			hit = default(RaycastHit);
			if (count < 1)
			{
				return false;
			}
			float best = float.MaxValue;
			for (int i = 0; i < count; i++)
			{
				Collider c = gapHits[i].collider;
				if (c && !(gapHits[i].distance >= best) && !c.GetComponentInParent<Player>() && !c.GetComponentInParent<NetworkPlayer>() && !c.GetComponentInParent<NetworkNpc>() && !c.GetComponentInParent<Rope>())
				{
					best = gapHits[i].distance;
					hit = gapHits[i];
				}
			}
			return best < float.MaxValue;
		}

		private Vector3 LeapVelocity()
		{
			if (leapSpot)
			{
				Vector3 forward = body.chest.transform.forward;
				return Arc(leapAt - leapBase, LeapApex, Flat(forward).normalized, GapFar);
			}
			Vector3 goal = target.pelvis.position + target.pelvis.linearVelocity * 0.3f - body.pelvis.position;
			float over = Mathf.Max(Clearance(Feet(), target.pelvis.position) + 0.5f - Mathf.Max(goal.y, 0f), LeapApex);
			Vector3 forward2 = body.chest.transform.forward;
			return Arc(goal, over, Flat(forward2).normalized, LeapFar);
		}

		private float Clearance(Vector3 from, Vector3 to)
		{
			Vector3 flat = new Vector3(to.x - from.x, 0f, to.z - from.z);
			float dist = flat.magnitude;
			if (dist < GapStep)
			{
				return 0f;
			}
			float end = dist - GapStep;
			float top = Mathf.Max(from.y, to.y) + LeapTop + 1f;
			float best = 0f;
			Vector3 dir = flat / dist;
			for (float d = GapStep; d < end; d += GapStep)
			{
				if (GapGround(new Vector3(from.x + dir.x * d, top, from.z + dir.z * d), top - from.y + 0.5f, out RaycastHit hit))
				{
					best = Mathf.Max(best, hit.point.y - from.y);
				}
			}
			return Mathf.Min(best, LeapTop);
		}

		private bool Facing(Vector3 pelvis)
		{
			float y = Mathf.Max(scaleFaceY, Mathf.Min(pelvis.y, scaleTop.y - 0.2f));
			int count = Physics.RaycastNonAlloc(new Vector3(pelvis.x, y, pelvis.z), scaleWay, gapHits, ClimbFace, ~4, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < count; i++)
			{
				Collider c = gapHits[i].collider;
				if (c && !Other(c))
				{
					return true;
				}
			}
			return false;
		}

		private static bool Other(Collider c)
		{
			if (!c.GetComponentInParent<Player>() && !c.GetComponentInParent<NetworkPlayer>() && !c.GetComponentInParent<NetworkNpc>())
			{
				return c.GetComponentInParent<Rope>();
			}
			return true;
		}

		private bool WallTop(out Vector3 top, out Vector3 way)
		{
			top = Vector3.zero;
			way = Flat(targetBody.pelvis.position - body.pelvis.position);
			if (way.sqrMagnitude < 0.01f)
			{
				return false;
			}
			way = way.normalized;
			Vector3 feet = Feet();
			if (!Physics.Raycast(feet + Vector3.up * 0.6f, way, out RaycastHit wall, ClimbFace, ~4, QueryTriggerInteraction.Ignore) || Mathf.Abs(wall.normal.y) > 0.4f || Other(wall.collider))
			{
				return false;
			}
			if (wall.rigidbody && !wall.rigidbody.isKinematic && wall.rigidbody.mass < 40f)
			{
				return false;
			}
			if (!GapGround(new Vector3(wall.point.x + way.x * 0.35f, feet.y + ClimbTop + 0.6f, wall.point.z + way.z * 0.35f), ClimbTop + 0.6f, out RaycastHit ledge))
			{
				return false;
			}
			float rise = ledge.point.y - feet.y;
			if (rise < 0.7f || rise > ClimbTop)
			{
				return false;
			}
			if (ledge.normal.y < 0.6f || Hazard(ledge.point) || TrainWreck.Desert(ledge.rigidbody))
			{
				return false;
			}
			if (ledge.rigidbody && !ledge.rigidbody.isKinematic && ledge.rigidbody.mass < 40f)
			{
				return false;
			}
			if (Physics.Raycast(wall.point + way * -0.3f + Vector3.up * 0.2f, Vector3.up, out RaycastHit roof, rise + ClimbFace, ~4, QueryTriggerInteraction.Ignore) && !Other(roof.collider))
			{
				return false;
			}
			int count = Physics.OverlapCapsuleNonAlloc(ledge.point + Vector3.up * 0.45f, ledge.point + Vector3.up * 1.5f, 0.25f, roomHits, ~4, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < count; i++)
			{
				if (roomHits[i] && !Other(roomHits[i]))
				{
					return false;
				}
			}
			top = ledge.point;
			scaleFaceY = wall.point.y;
			return true;
		}

		private void StartScale()
		{
			scaling = true;
			scaleOver = false;
			scaleBest = body.pelvis.position.y;
			scaleBestAt = Time.time;
			scaleRig = rig.playSpace.position.y - body.pelvis.position.y;
			rig.testWish = Vector3.zero;
			if (Application.isEditor && hitLog.Length < 40000)
			{
				hitLog.Append("      t ").Append(Time.time.ToString("F2")).Append(" CLIMB ").Append((scaleTop.y - Feet().y).ToString("F2")).Append(" m up at ")
					.Append(scaleTop.ToString("F1"))
					.Append('\n');
			}
		}

		private void EndScale()
		{
			scaling = false;
			scaleOver = false;
			rig.testWish = Vector3.zero;
		}

		private float FootY()
		{
			float y = body.pelvis.position.y;
			Rigidbody[] legs = new Rigidbody[2] { body.leftLowerLeg, body.rightLowerLeg };
			foreach (Rigidbody leg in legs)
			{
				if (!leg)
				{
					continue;
				}
				leg.GetComponents(partCols);
				foreach (Collider c in partCols)
				{
					if (c && !c.isTrigger)
					{
						y = Mathf.Min(y, c.bounds.min.y);
					}
				}
			}
			return y;
		}

		private bool Scale()
		{
			if (!target || fighter.knockedOut || self.flopped || Wet())
			{
				EndScale();
				return false;
			}
			Vector3 pelvis = body.pelvis.position;
			if (pelvis.y > scaleBest + 0.05f)
			{
				scaleBest = pelvis.y;
				scaleBestAt = Time.time;
			}
			else if (Time.time > scaleBestAt + ClimbStall)
			{
				EndScale();
				return false;
			}
			rig.aiYaw = Quaternion.LookRotation(scaleWay, Vector3.up).eulerAngles.y;
			float foot = FootY();
			if (!scaleOver && foot >= scaleTop.y + 0.1f)
			{
				scaleOver = true;
			}
			if (scaleOver)
			{
				if (Vector3.Dot(Flat(pelvis - scaleTop), scaleWay) > 0.3f)
				{
					EndScale();
					return false;
				}
			}
			else if (!Facing(pelvis))
			{
				EndScale();
				return false;
			}
			Vector3 want;
			if (scaleOver)
			{
				float lift = (foot < scaleTop.y + 0.3f) ? 0.4f : Mathf.Min(body.pelvis.linearVelocity.y, 0f);
				want = scaleWay * OverSpeed + Vector3.up * lift;
			}
			else
			{
				want = Vector3.up * ClimbUp + scaleWay * ClimbHug;
			}
			Vector3 change = want - body.pelvis.linearVelocity;
			Rigidbody[] bodies = body.bodies;
			foreach (Rigidbody rb in bodies)
			{
				if (rb && !rb.isKinematic)
				{
					rb.AddForce(change, ForceMode.VelocityChange);
				}
			}
			Vector3 rigAt = rig.playSpace.position;
			rig.playSpace.position = new Vector3(rigAt.x, pelvis.y + scaleRig, rigAt.z);
			float phase = Mathf.Repeat(Time.time * 3f, 1f);
			Vector3 grip = body.chest.position + Vector3.up * armLen * 0.9f + scaleWay * 0.25f;
			Pull(body.rightHand, grip + ((phase < 0.5f) ? (Vector3.up * 0.25f) : Vector3.zero), WindPull);
			Pull(body.leftHand, grip + ((phase < 0.5f) ? Vector3.zero : (Vector3.up * 0.25f)), WindPull);
			return true;
		}

		private bool TryPounce(float spaceDist, Vector3 face)
		{
			if (Time.time < nextPounce)
			{
				return false;
			}
			if (!self.grounded || self.rising || fighter.knockedOut || Time.time < punchUntil)
			{
				return false;
			}
			if (spaceDist < PounceNear || spaceDist > PounceFar)
			{
				return false;
			}
			if (Vector3.Angle(Vector3.forward, face) > Twisting)
			{
				return false;
			}
			leaping = false;
			pouncing = true;
			launched = false;
			clingThis = kind == Kind.Fat || (kind == Kind.Small && Random.value < 0.5f);
			slamSpent = false;
			spentL = false;
			spentR = false;
			pounceAt = Time.time;
			rig.testWish = Vector3.zero;
			if (Application.isEditor && hitLog.Length < 40000)
			{
				hitLog.Append("      t ").Append(Time.time.ToString("F2")).Append(" POUNCE from ").Append(spaceDist.ToString("F2")).Append('\n');
			}
			return true;
		}

		private bool Pounce()
		{
			if (!target || fighter.knockedOut)
			{
				EndPounce();
				return false;
			}
			float since = Time.time - pounceAt;
			if (launched)
			{
				if (since > 0.35000002f && (self.grounded || self.flopped || (body.pelvis.linearVelocity.y < 0f && body.pelvis.position.y < pounceY - 0.2f) || Wet()))
				{
					EndPounce();
					return false;
				}
				rig.testWish = Vector3.forward;
				if (clingThis && Grip())
				{
					return true;
				}
				float phase = Mathf.Repeat(since * 8f, 1f);
				rig.playSpace.InverseTransformPoint(target.head.position);
				bool swap = phase >= 0.5f;
				Pull(swap ? body.leftHand : body.rightHand, target.head.position, StrikePull);
				Pull(swap ? body.rightHand : body.leftHand, rig.playSpace.TransformPoint(Shoulder(swap) + new Vector3(swap ? 0.1f : -0.1f, 0.2f, 0.25f)), WindPull);
				return true;
			}
			if (since > 0.65f || self.flopped)
			{
				EndPounce();
				return false;
			}
			Pull(body.rightHand, rig.playSpace.TransformPoint(Shoulder(true) + Vector3.up * armLen * 0.9f), WindPull);
			Pull(body.leftHand, rig.playSpace.TransformPoint(Shoulder(false) + Vector3.up * armLen * 0.9f), WindPull);
			if (since >= 0.2f && self.grounded && !self.jumped)
			{
				self.Jump(1f);
			}
			if (!self.jumped || self.grounded)
			{
				return true;
			}
			launched = true;
			Rigidbody[] bodies;
			if (leaping)
			{
				pounceY = body.pelvis.position.y;
				Vector3 leap = LeapVelocity();
				Vector3 change = leap - body.pelvis.linearVelocity;
				bodies = body.bodies;
				foreach (Rigidbody rb in bodies)
				{
					if (rb && !rb.isKinematic)
					{
						rb.AddForce(change, ForceMode.VelocityChange);
					}
				}
				Vector3 flat = Flat(leap);
				if (flat.sqrMagnitude > 0.0001f)
				{
					rig.aiYaw = Quaternion.LookRotation(flat, Vector3.up).eulerAngles.y;
				}
				rig.speed = flat.magnitude;
				rig.testWish = Vector3.forward;
				return true;
			}
			pounceY = body.pelvis.position.y;
			float air = (Mathf.Max(body.pelvis.linearVelocity.y, 0f) + PounceLift) * 2f / 9.81f;
			Vector3 gap = Flat(target.head.position - body.pelvis.position + target.pelvis.linearVelocity * air);
			float speed = Mathf.Clamp(gap.magnitude / Mathf.Max(air, 0.1f), 1.5f, 4.5f);
			Vector3 dir = (gap.sqrMagnitude > 0.0001f) ? gap.normalized : Flat(body.chest.transform.forward).normalized;
			bodies = body.bodies;
			foreach (Rigidbody rb2 in bodies)
			{
				if (rb2 && !rb2.isKinematic)
				{
					rb2.AddForce(dir * speed + Vector3.up, ForceMode.VelocityChange);
				}
			}
			rig.aiYaw = Quaternion.LookRotation(dir, Vector3.up).eulerAngles.y;
			rig.speed = speed;
			rig.testWish = Vector3.forward;
			return true;
		}

		private bool Grip()
		{
			if (!targetBody)
			{
				return false;
			}
			bool fat = kind == Kind.Fat;
			Rigidbody holder = fat ? body.chest : body.leftHand;
			Rigidbody part = Part(holder, fat ? HugReach : ClingReach, true, out Vector3 _);
			if (!part)
			{
				return false;
			}
			clingJoint = holder.gameObject.AddComponent<ConfigurableJoint>();
			clingJoint.connectedBody = part;
			clingJoint.xMotion = ConfigurableJointMotion.Locked;
			clingJoint.yMotion = ConfigurableJointMotion.Locked;
			clingJoint.zMotion = ConfigurableJointMotion.Locked;
			clingJoint.enableCollision = false;
			clingJoint.breakForce = fat ? HugBreak : ClingBreak;
			clingAt = Time.time;
			float now = Time.time;
			float hold = (kind == Kind.Fat) ? Random.Range(3.5f, 5f) : Random.Range(2.2f, 3.6f);
			jabSpent = false;
			clingUntil = now + hold;
			EndPounce();
			if (Application.isEditor && hitLog.Length < 40000)
			{
				hitLog.Append("      t ").Append(Time.time.ToString("F2")).Append(" CLING on ").Append(part.name).Append('\n');
			}
			return true;
		}

		private void Blab()
		{
			if (fighter.knockedOut)
			{
				talkUntil = -1f;
				return;
			}
			if (Time.time >= talkNext && !(Time.time < talkUntil))
			{
				float now = Time.time;
				float talk = Random.Range(TalkMin, TalkMax);
				talkUntil = now + talk;
				talkNext = now + talk + Random.Range(QuietMin, QuietMax);
			}
			Player main = Player.main;
			if (!main || main == body || !body.head)
			{
				return;
			}
			if (mouth == null || !mouth.source)
			{
				mouth = new Player.Babble();
				mouth.source = main.SpeakerOn(body.head.transform);
			}
			mouth.Tick(Time.time < talkUntil, main.blabbers, main.blabberVolume * BlabVolume, Pitch(kind));
		}

		private Rigidbody Part(Rigidbody holder, float reach, bool head, out Vector3 at)
		{
			at = holder.position;
			Rigidbody found = null;
			float best = reach;
			for (int i = 0; i < 5; i++)
			{
				Rigidbody part;
				switch (i)
				{
					case 0:
						part = targetBody.chest;
						break;
					case 1:
						part = targetBody.leftUpperArm;
						break;
					case 2:
						part = targetBody.rightUpperArm;
						break;
					case 3:
						part = targetBody.pelvis;
						break;
					default:
						part = head ? targetBody.head : null;
						break;
				}
				if (!part)
				{
					continue;
				}
				part.GetComponents(partCols);
				foreach (Collider c in partCols)
				{
					if (!c.isTrigger && c.enabled)
					{
						Vector3 p = c.ClosestPoint(holder.position);
						float d = (p - holder.position).magnitude;
						if (d < best)
						{
							at = p;
							best = d;
							found = part;
						}
					}
				}
			}
			return found;
		}

		private bool Claim()
		{
			bool near = target.knockedOut && !target.carriedHold && !target.trainHeld && !TrainWreck.Dragged(target) && quitKey != target.knockedOutUntil && spentKey != target.knockedOutUntil && Time.time > tossAt + TossAir && Flat(body.pelvis.position - target.pelvis.position).magnitude < FetchMax;
			bool taken;
			if (hauler && hauler != this && !hauler.gone && hauler.isActiveAndEnabled)
			{
				taken = !near || hauler.claimKey == target.knockedOutUntil;
			}
			else
			{
				taken = !near;
			}
			if (taken || !Pick())
			{
				if (hauler == this && !haul)
				{
					hauler = null;
				}
				return false;
			}
			if (hauler != this || claimKey != target.knockedOutUntil)
			{
				haulLean = 0f;
				claimAt = Time.time;
				handKnownL = false;
				handKnownR = false;
			}
			hauler = this;
			claimKey = target.knockedOutUntil;
			return true;
		}

		private bool Help()
		{
			bool near = hauler && hauler != this && !hauler.gone && hauler.isActiveAndEnabled && hauler.kind == Kind.Small && hauler.target == target && hauler.claimKey == target.knockedOutUntil && hauler.haulWay >= 0 && target.knockedOut && quitKey != target.knockedOutUntil && Flat(body.pelvis.position - target.pelvis.position).magnitude < FetchMax;
			bool taken;
			if (helper && helper != this && !helper.gone && helper.isActiveAndEnabled)
			{
				taken = !near || helper.claimKey == target.knockedOutUntil;
			}
			else
			{
				taken = !near;
			}
			if (taken)
			{
				if (helper == this && !haul)
				{
					helper = null;
				}
				return false;
			}
			if (helper != this || claimKey != target.knockedOutUntil)
			{
				haulLean = 0f;
				claimAt = Time.time;
				handKnownL = false;
				handKnownR = false;
			}
			helper = this;
			claimKey = target.knockedOutUntil;
			haulWay = hauler.haulWay;
			haulNow = hauler.haulNow;
			return true;
		}

		private float Strain()
		{
			if (kind != Kind.Small)
			{
				return 1f;
			}
			if (helper && helper != this && !helper.gone && helper.haul && helper.target == target)
			{
				return SmallTeam;
			}
			return SmallSolo;
		}

		private bool Pick()
		{
			if (scanKey != target.knockedOutUntil)
			{
				if (!target.grounded || Flat(target.pelvis.linearVelocity).magnitude > 0.5f)
				{
					return false;
				}
				scanKey = target.knockedOutUntil;
				scanFloor = GameManager.Brinks(target.pelvis.position, brinkAt, standAt, landAt, edgeAt);
				scanOnBody = scanFloor;
				if (scanFloor)
				{
					for (int i = 0; i < Ways; i++)
					{
						standAt[i] = scanFloor.transform.InverseTransformPoint(standAt[i]);
						landAt[i] = scanFloor.transform.InverseTransformPoint(landAt[i]);
						edgeAt[i] = scanFloor.transform.InverseTransformPoint(edgeAt[i]);
					}
				}
			}
			if (scanOnBody && !scanFloor)
			{
				return false;
			}
			Vector3 away = Flat(body.pelvis.position - target.pelvis.position);
			Vector3 dir = (away.sqrMagnitude > 0.0001f) ? away.normalized : Vector3.zero;
			haulWay = -1;
			float best = float.MaxValue;
			for (int j = 0; j < Ways; j++)
			{
				if (brinkAt[j] < 0f || brinkAt[j] >= best)
				{
					continue;
				}
				Vector3 land = Placed(landAt[j]);
				Vector3 stand = Placed(standAt[j]);
				bool now = brinkAt[j] < HaulFar;
				if (!now)
				{
					Vector3 way = Flat(land - stand);
					if (way.sqrMagnitude < 0.0001f || Vector3.Dot(dir, way.normalized) < HaulSide)
					{
						continue;
					}
				}
				best = brinkAt[j];
				haulNow = now;
				haulWay = j;
			}
			return haulWay >= 0;
		}

		private static Vector3 Placed(Vector3 local)
		{
			if (!scanFloor)
			{
				return local;
			}
			return scanFloor.transform.TransformPoint(local);
		}

		private void Fetch()
		{
			if (Time.time > claimAt + FetchMax)
			{
				quitKey = target.knockedOutUntil;
				if (helper == this)
				{
					helper = null;
					return;
				}
				spentKey = quitKey;
				if (haulWay >= 0 && Flat(target.pelvis.position - body.pelvis.position).magnitude < armLen + 0.5f)
				{
					Toss();
					return;
				}
				if (hauler == this)
				{
					hauler = null;
				}
				return;
			}
			bool fat = kind == Kind.Fat;
			Rigidbody holder = fat ? body.chest : body.leftHand;
			Rigidbody part = Part(holder, float.MaxValue, false, out Vector3 at);
			if (!part)
			{
				return;
			}
			float lean = fat ? (body.chest.position.y - at.y + -0.25f) : (body.leftUpperArm.position.y - at.y + armLen * -0.6f);
			haulLean = Mathf.Clamp(Mathf.Max(lean, haulLean), 0.25f, HaulDeep);
			rig.testLean = Vector3.down * haulLean;
			Vector3 from = (fat ? body.chest : body.leftUpperArm).position;
			Vector3 face = Flat(at - body.chest.position);
			if (face.sqrMagnitude > 0.0001f)
			{
				rig.aiYaw = Quaternion.LookRotation(face, Vector3.up).eulerAngles.y;
			}
			rig.speed = walkSpeed * HaulPace;
			Vector3 step = Flat(at - from);
			rig.testWish = (step.magnitude > (fat ? 0.3f : armLen * 0.55f)) ? Flat(rig.playSpace.InverseTransformDirection(step)).normalized : Vector3.zero;
			Steer();
			if (!fat)
			{
				Lift(body.leftHand);
				Lift(body.leftForearm);
				Lift(body.leftUpperArm);
				Pull(body.leftHand, at, StrikePull);
				GuardHand(true);
			}
			if (Vector3.Distance(at, holder.position) < (fat ? HaulNear : (kind == Kind.Big ? BigGrab : ClingReach)))
			{
				Hold(holder, part);
			}
		}

		private void Hold(Rigidbody holder, Rigidbody part)
		{
			haul = holder.gameObject.AddComponent<ConfigurableJoint>();
			haul.connectedBody = part;
			haul.xMotion = ConfigurableJointMotion.Free;
			haul.yMotion = ConfigurableJointMotion.Free;
			haul.zMotion = ConfigurableJointMotion.Free;
			float max = (kind == Kind.Small) ? 600f : ((kind == Kind.Normal) ? 900f : 1500f);
			JointDrive drive = default(JointDrive);
			drive.positionSpring = HaulSpring;
			drive.positionDamper = HaulDamp;
			drive.maximumForce = max;
			haul.xDrive = drive;
			haul.yDrive = drive;
			haul.zDrive = drive;
			haul.connectedMassScale = Mathf.Min(holder.mass * 5f / Mathf.Max(part.mass, 0.01f), 1f);
			haul.enableCollision = false;
			haul.enablePreprocessing = false;
			haul.breakForce = max * 3f;
			haulAt = Time.time;
			haulMoves = target.recovers + target.rescues;
			Pass(target, true);
			tossed = target;
			stuckFrom = -1f;
			if (Application.isEditor && hitLog.Length < 40000)
			{
				hitLog.Append("      t ").Append(Time.time.ToString("F2")).Append(" HAUL grab ").Append(part.name).Append(" edge ")
					.Append(brinkAt[haulWay].ToString("F1"))
					.Append(" m")
					.Append(haulNow ? " (toss now)" : "")
					.Append('\n');
			}
			Player.Note("a " + kind.ToString() + ((helper == this) ? " grabbed my knocked out body to help drag it" : (" grabbed my knocked out body to throw it " + (haulNow ? "off the edge right here" : (brinkAt[haulWay].ToString("F1") + " m away")))));
		}

		private float Stretch()
		{
			if (!haul || !haul.connectedBody)
			{
				return float.MaxValue;
			}
			return Vector3.Distance(haul.transform.TransformPoint(haul.anchor), haul.connectedBody.transform.TransformPoint(haul.connectedAnchor));
		}

		private bool Haul()
		{
			if (helper == this)
			{
				return Assist();
			}
			bool calm = AvatarBody.peaceful || AvatarBody.truce || GameManager.Waiting;
			bool lost = gone || fighter.knockedOut || self.flopped || !Valid(target) || !target.knockedOut || target.carriedHold || target.trainHeld || TrainWreck.Dragged(target) || target.recovers + target.rescues != haulMoves || (scanOnBody && !scanFloor) || haulWay < 0;
			if (lost || calm)
			{
				Drop("let go");
				return false;
			}
			if (Stretch() > HaulStretch)
			{
				Toss();
				return true;
			}
			fighter.limpWhenIdle = true;
			rig.testLean = Vector3.down * haulLean;
			Vector3 face = Flat(target.chest.position - body.chest.position);
			if (face.sqrMagnitude > 0.0001f)
			{
				rig.aiYaw = Quaternion.LookRotation(face, Vector3.up).eulerAngles.y;
			}
			rig.speed = walkSpeed * HaulPace * Strain();
			Vector3 stand = Placed(standAt[haulWay]);
			Vector3 go = Flat(stand - body.pelvis.position);
			float dist = go.magnitude;
			if (!(dist > HaulArrive) || haulNow)
			{
				rig.testWish = Vector3.zero;
				Toss();
				return true;
			}
			rig.testWish = Flat(rig.playSpace.InverseTransformDirection(go)).normalized;
			Steer();
			if (rig.testWish.sqrMagnitude <= 0f || Time.time > haulAt + HaulMax)
			{
				Toss();
				return true;
			}
			if (stuckFrom < 0f || dist < stuckFrom - StuckGain)
			{
				stuckFrom = dist;
				stuckAt = Time.time;
			}
			else if (Time.time > stuckAt + StuckFor)
			{
				Toss();
				return true;
			}
			Vector3 want = go / dist * rig.speed;
			Rigidbody[] bodies = target.bodies;
			foreach (Rigidbody rb in bodies)
			{
				if (rb && !rb.isKinematic)
				{
					float dt = Time.fixedDeltaTime;
					Vector3 velocity = rb.linearVelocity;
					Vector3 accel = new Vector3((want.x - velocity.x) / dt, want.y / dt, (want.z - velocity.z) / dt);
					rb.AddForce(Vector3.ClampMagnitude(accel, HaulAssist), ForceMode.Acceleration);
				}
			}
			if (kind != Kind.Fat)
			{
				Lift(body.leftHand);
				Lift(body.leftForearm);
				Lift(body.leftUpperArm);
			}
			return true;
		}

		private bool Assist()
		{
			bool calm = AvatarBody.peaceful || AvatarBody.truce || GameManager.Waiting;
			bool lost = !hauler || hauler == this || hauler.gone || !hauler.isActiveAndEnabled || hauler.target != target || hauler.claimKey != claimKey || (!hauler.haul && !(Time.time < hauler.claimAt + FetchMax)) || gone || fighter.knockedOut || self.flopped || !Valid(target) || !target.knockedOut || Stretch() > HaulStretch;
			if (lost || calm)
			{
				Drop("helper let go");
				return false;
			}
			fighter.limpWhenIdle = true;
			rig.testLean = Vector3.down * haulLean;
			Vector3 face = Flat(target.chest.position - body.chest.position);
			if (face.sqrMagnitude > 0.0001f)
			{
				rig.aiYaw = Quaternion.LookRotation(face, Vector3.up).eulerAngles.y;
			}
			bool still = !hauler.haul || !(hauler.rig.testWish.sqrMagnitude > 0f);
			Vector3 stand = Placed(standAt[hauler.haulWay]);
			Vector3 from = hauler.body.pelvis.position;
			rig.speed = hauler.rig.speed;
			Vector3 go = Flat(stand - from);
			if (!still && go.sqrMagnitude > 0.0001f)
			{
				rig.testWish = Flat(rig.playSpace.InverseTransformDirection(go)).normalized;
			}
			else
			{
				rig.testWish = Vector3.zero;
			}
			if (!still)
			{
				Steer();
			}
			if (kind != Kind.Fat)
			{
				Lift(body.leftHand);
				Lift(body.leftForearm);
				Lift(body.leftUpperArm);
			}
			return true;
		}

		private void Toss()
		{
			Vector3 land = Placed(landAt[haulWay]);
			Vector3 stand = Placed(standAt[haulWay]);
			Vector3 way = Flat(land - stand);
			Vector3 dir = (way.sqrMagnitude > 0.0001f) ? way.normalized : Flat(target.pelvis.position - body.pelvis.position).normalized;
			if (helper && helper != this && helper.haul)
			{
				helper.Drop("toss");
			}
			if (!GameManager.Deadly(land, dir, out float surface))
			{
				Drop("no toss");
				return;
			}
			Vector3 from = target.pelvis.position;
			float over = Mathf.Max(body.head.position.y - target.pelvis.position.y, 0f) + TossOver;
			if (Physics.Raycast(target.pelvis.position + Vector3.up * 0.3f, Vector3.up, out RaycastHit hit, over + 0.6f, -1, QueryTriggerInteraction.Ignore) && !hit.collider.transform.IsChildOf(target.transform) && !hit.collider.transform.IsChildOf(transform))
			{
				over = Mathf.Max(hit.distance, 0.2f);
			}
			Vector3 velocity = Arc(new Vector3(land.x - from.x, surface - from.y, land.z - from.z), over, dir, LeapFar);
			Vector3 edge = Placed(edgeAt[haulWay]);
			float toEdge = Flat(edge - target.pelvis.position).magnitude;
			float toLand = Flat(land - target.pelvis.position).magnitude;
			float up = Mathf.Max(velocity.y, 0.5f);
			float rise = (up + up) / 9.81f;
			float fall = (up + Mathf.Sqrt(up * up + Mathf.Max(target.pelvis.position.y - surface, 0f) * 19.62f)) / 9.81f;
			float clear = (toEdge + LipClear) / rise;
			float speed = Mathf.Min(Mathf.Max(toLand / fall, clear), Mathf.Max((toLand + Flat(land - edge).magnitude) / fall, clear), 6f);
			Vector3 flat = Flat(velocity);
			if (flat.sqrMagnitude > 0.0001f)
			{
				dir = flat.normalized;
			}
			Player victim = target;
			Drop("toss");
			Pass(victim, true);
			tossed = victim;
			victim.knockedOutUntil = Mathf.Max(victim.knockedOutUntil, Time.time + TossAir);
			quitKey = victim.knockedOutUntil;
			victim.riseLockedUntil = Mathf.Max(victim.riseLockedUntil, Time.time + TossAir);
			tossAt = Time.time;
			Vector3 throwV = dir * speed + Vector3.up * velocity.y;
			Vector3 change = throwV - victim.pelvis.linearVelocity;
			Rigidbody[] bodies = victim.bodies;
			foreach (Rigidbody rb in bodies)
			{
				if (rb && !rb.isKinematic)
				{
					rb.AddForce(change, ForceMode.VelocityChange);
				}
			}
			Player.Note("a " + kind.ToString() + " threw my knocked out body at " + throwV.magnitude.ToString("F1") + " m/s toward " + land.ToString("F1"));
		}

		private void Drop(string why)
		{
			if (haul)
			{
				haul.xDrive = default(JointDrive);
				haul.yDrive = default(JointDrive);
				haul.zDrive = default(JointDrive);
				UnityEngine.Object.Destroy(haul);
				quitKey = target ? target.knockedOutUntil : -1f;
				if (helper != this)
				{
					spentKey = quitKey;
				}
				if (Application.isEditor && hitLog.Length < 40000)
				{
					hitLog.Append("      t ").Append(Time.time.ToString("F2")).Append(" HAUL drop ").Append(why).Append('\n');
				}
			}
			haul = null;
			haulLean = 0f;
			if (hauler == this)
			{
				hauler = null;
			}
			if (helper == this)
			{
				helper = null;
			}
			stuckFrom = -1f;
			handKnownL = false;
			handKnownR = false;
		}

		private void Pass(Player p, bool off)
		{
			if (!p)
			{
				return;
			}
			if (off || passMine == null)
			{
				passMine = body.GetComponentsInChildren<Collider>();
				passTheirs = p.GetComponentsInChildren<Collider>();
			}
			Collider[] array = passMine;
			foreach (Collider mine in array)
			{
				if (!mine)
				{
					continue;
				}
				Collider[] array2 = passTheirs;
				foreach (Collider theirs in array2)
				{
					if (theirs)
					{
						Physics.IgnoreCollision(mine, theirs, off);
					}
				}
			}
		}

		private bool Overlaps()
		{
			if (passMine == null || passTheirs == null)
			{
				return false;
			}
			Collider[] array = passMine;
			foreach (Collider mine in array)
			{
				Bounds bounds = mine.bounds;
				Collider[] array2 = passTheirs;
				foreach (Collider theirs in array2)
				{
					if (bounds.Intersects(theirs.bounds))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static Vector3 Arc(Vector3 goal, float over, Vector3 fallback, float far = 6f)
		{
			Vector3 flat = Flat(goal);
			Vector3 dir = (flat.sqrMagnitude > 0.0001f) ? flat.normalized : fallback;
			float apex = Mathf.Clamp(Mathf.Max(goal.y, 0f) + over, over, LeapTop);
			float up = Mathf.Sqrt(apex * 19.62f);
			float time = up / 9.81f + Mathf.Sqrt(Mathf.Max(apex - goal.y, 0.05f) * 2f / 9.81f);
			float speed = Mathf.Clamp(flat.magnitude / time, 1f, far);
			return dir * speed + Vector3.up * up;
		}

		private bool Cling()
		{
			if (!clingJoint || fighter.knockedOut || !target || target.knockedOut || target.dead || Time.time > clingUntil)
			{
				LetGoOf();
				return false;
			}
			rig.testWish = Vector3.zero;
			rig.aiHandL = true;
			rig.aiHandPosL = rig.playSpace.InverseTransformPoint(body.leftHand.position);
			handKnownL = false;
			float phase = Mathf.Repeat((Time.time - clingAt) / JabEvery, 1f);
			bool strike = phase >= 0.4f;
			if (!strike)
			{
				jabSpent = false;
			}
			Vector3 shoulder = body.rightUpperArm.position;
			Vector3 aim = targetBody.chest.position;
			float best = float.MaxValue;
			for (int i = 0; i < 4; i++)
			{
				Rigidbody part;
				switch (i)
				{
					case 0:
						part = targetBody.head;
						break;
					case 1:
						part = targetBody.chest;
						break;
					case 2:
						part = targetBody.rightUpperArm;
						break;
					default:
						part = targetBody.leftUpperArm;
						break;
				}
				if (!part)
				{
					continue;
				}
				part.GetComponents(partCols);
				foreach (Collider c in partCols)
				{
					if (!c.isTrigger && c.enabled)
					{
						Vector3 p = c.ClosestPoint(shoulder);
						float d = (p - shoulder).sqrMagnitude;
						if (d < best)
						{
							aim = p;
							best = d;
						}
					}
				}
			}
			Vector3 to = aim - shoulder;
			Vector3 dir = (to.sqrMagnitude > 0.0001f) ? to.normalized : body.chest.transform.forward;
			if (strike && !jabbing)
			{
				float kick = Random.Range(0.8f, 1.2f) * 7f;
				body.rightHand.AddForce(dir * kick, ForceMode.VelocityChange);
				body.rightForearm.AddForce(dir * kick * 0.7f, ForceMode.VelocityChange);
			}
			jabbing = strike;
			Lift(body.rightHand);
			Lift(body.rightForearm);
			Lift(body.rightUpperArm);
			if (strike)
			{
				Pull(body.rightHand, aim + dir * 0.12f, StrikePull);
				Pull(body.rightForearm, shoulder + to * 0.6f, 45f);
			}
			else
			{
				Pull(body.rightHand, shoulder - dir * 0.12f + Vector3.up * 0.05f, WindPull);
				Pull(body.rightForearm, shoulder - dir * 0.06f, 30f);
			}
			return true;
		}

		private void LetGoOf()
		{
			if (clingJoint)
			{
				UnityEngine.Object.Destroy(clingJoint);
			}
			clingJoint = null;
			clingUntil = 0f;
			jabbing = false;
			rig.aiHandL = false;
			handKnownL = false;
			handKnownR = false;
			nextPounce = Time.time + Random.Range(2f, 3.2f);
		}

		private void EndPounce()
		{
			pouncing = false;
			launched = false;
			leaping = false;
			rig.speed = walkSpeed;
			nextPounce = Time.time + Random.Range(2f, 3.2f);
		}

		private void Hang()
		{
			Vector3 left = Shoulder(false);
			Reach(false, new Vector3(left.x + -0.06f, left.y - armLen * 0.8f, left.z + 0.08f), Fist(false), armLen);
			Vector3 right = Shoulder(true);
			Reach(true, new Vector3(right.x + 0.06f, right.y - armLen * 0.8f, right.z + 0.08f), Fist(true), armLen);
		}

		private void Taunt()
		{
			float wave = Mathf.Sin(Time.time * 37.699112f);
			Vector3 left = Shoulder(false);
			Reach(false, new Vector3(left.x + -0.1f, left.y + (armLen * 0.25f + wave * 0.08f), left.z + 0.15f), Fist(false), armLen);
			Vector3 right = Shoulder(true);
			Reach(true, new Vector3(right.x + 0.1f, right.y + (armLen * 0.25f - wave * 0.08f), right.z + 0.15f), Fist(true), armLen);
		}


		private void Connect()
		{
			if (punchSpent || !targetBody)
			{
				return;
			}
			Rigidbody hand = punchRight ? body.rightHand : body.leftHand;
			if (!hand)
			{
				return;
			}
			if (Mathf.Clamp01(1f - (punchUntil - Time.time) / Mathf.Max(punchTime, 0.01f)) < wind)
			{
				return;
			}
			Vector3 to = targetBody.chest.position - hand.position;
			if (to.magnitude > hitRange)
			{
				return;
			}
			if (Vector3.Dot(hand.linearVelocity, to.normalized) < fighter.minSpeed)
			{
				return;
			}
			punchSpent = true;
			landings++;
			lastLanded = hitRange;
			lastLandedBy = (punchRight ? "right" : "left") + " fist, in range";
			backUntil = Time.time + stepBackFor;
		}

		private bool Climbing()
		{
			Task want;
			if (task == Task.Fight)
			{
				if (!followUp || !(targetBody.pelvis.position.y - body.pelvis.position.y > followHeight))
				{
					if (doing != Task.Fight)
					{
						StopClimb();
					}
					return false;
				}
				rope = NearestRope();
				want = rope ? Task.ClimbRope : Task.Vault;
			}
			else
			{
				want = task;
			}
			if (doing != want)
			{
				doing = want;
				climbStep = 0;
				climbAt = Time.time;
				if (want == Task.ClimbRope && !rope)
				{
					rope = NearestRope();
				}
				rig.aiHandL = false;
				rig.aiHandR = false;
			}
			if (doing == Task.ClimbRope)
			{
				ClimbRope();
			}
			else
			{
				Vault();
			}
			return true;
		}

		private Rope NearestRope()
		{
			float best = ropeNear;
			Rope nearest = null;
			Rope[] all = UnityEngine.Object.FindObjectsByType<Rope>(FindObjectsSortMode.None);
			foreach (Rope r in all)
			{
				float d = Flat(r.transform.position - body.pelvis.position).magnitude;
				if (d < best)
				{
					nearest = r;
					best = d;
				}
			}
			return nearest;
		}

		private void StopClimb()
		{
			rig.testPull = 0f;
			rig.testGrabL = false;
			rig.testGrabR = false;
			rig.testRaise = 0f;
			rig.Look(0f);
			rig.testWish = Vector3.zero;
			doing = Task.Fight;
			climbStep = 0;
		}

		private void ClimbRope()
		{
			if (!rope)
			{
				StopClimb();
				return;
			}
			Vector3 off = Flat(rope.transform.position - rig.playSpace.position);
			float d = off.magnitude;
			if (d > 0.01f)
			{
				rig.aiYaw = Quaternion.LookRotation(off / d, Vector3.up).eulerAngles.y;
			}
			if (climbStep == 0)
			{
				rig.Look(lookUp);
				rig.testGrabL = false;
				rig.testGrabR = false;
				rig.testWish = (d > ropeStand) ? Vector3.forward : Vector3.zero;
				if (d <= ropeStand)
				{
					climbStep = 1;
					climbAt = Time.time;
				}
				return;
			}
			float since = Time.time - climbAt;
			rig.testWish = Vector3.zero;
			float left = body.leftHand.position.y;
			float reachY = body.chest.position.y + grabAbove;
			float right = body.rightHand.position.y;
			if (climbStep == 1)
			{
				rig.Look(lookUp);
				if (since > 0.5f && !rig.testGrabL && (since > 2.5f || (right > reachY && left > reachY)))
				{
					rig.testGrabL = true;
					rig.testGrabR = true;
					grabbedAt = Time.time;
				}
				if (rig.testGrabL && Time.time > grabbedAt + 0.4f)
				{
					rig.Look(lookDown);
				}
				if (rig.testGrabL && Time.time > grabbedAt + 0.4f + ropeStep)
				{
					climbStep = 2;
					climbAt = Time.time;
				}
				return;
			}
			bool above = ((climbStep & 1) != 0) ? (right > reachY) : (left > reachY);
			if (since < 0.15f)
			{
				if ((climbStep & 1) != 0)
				{
					rig.testGrabR = false;
				}
				else
				{
					rig.testGrabL = false;
				}
				rig.Look(lookUp);
				regrabbed = false;
				return;
			}
			if (!regrabbed)
			{
				if (since > 0.5f && (since > 2.5f || above))
				{
					if ((climbStep & 1) != 0)
					{
						rig.testGrabR = true;
					}
					else
					{
						rig.testGrabL = true;
					}
					regrabbed = true;
					grabbedAt = Time.time;
				}
				return;
			}
			if (Time.time < grabbedAt + 0.4f)
			{
				return;
			}
			if (Time.time < grabbedAt + 0.4f + ropeStep)
			{
				rig.Look(lookDown);
				return;
			}
			climbStep++;
			climbAt = Time.time;
		}

		private void Vault()
		{
			Vector3 to = Flat(targetBody.pelvis.position - rig.playSpace.position);
			if (to.sqrMagnitude > 0.01f)
			{
				rig.aiYaw = Quaternion.LookRotation(to.normalized, Vector3.up).eulerAngles.y;
			}
			if (climbStep == 1)
			{
				float since = Time.time - climbAt;
				rig.testWish = Vector3.zero;
				rig.testPull = vaultPull;
				if (since > 2.5f)
				{
					climbStep = 2;
					climbAt = Time.time;
				}
				return;
			}
			if (climbStep != 0)
			{
				rig.testPull = 0f;
				rig.testRaise = 0f;
				float since2 = Time.time - climbAt;
				rig.testWish = Vector3.forward * vaultWalk;
				if (since2 > 2f)
				{
					StopClimb();
				}
				return;
			}
			rig.testPull = 0f;
			rig.testRaise = vaultRaise;
			float since3 = Time.time - climbAt;
			rig.testWish = Vector3.forward * vaultWalk;
			if (self.holding || since3 > 3f)
			{
				climbStep = 1;
				climbAt = Time.time;
			}
		}

		private void HangArms()
		{
			if (!drive || !body.chest)
			{
				return;
			}
			Vector3 down = -body.chest.transform.up;
			AimArm(false, down, down);
			AimArm(true, down, down);
			self.limpArmScale = idleArmTone;
		}

		private void AimArms()
		{
			if (!drive || !body.chest)
			{
				return;
			}
			AimArm(false, ToTarget(false), -body.chest.transform.up);
			AimArm(true, ToTarget(true), -body.chest.transform.up);
		}

		private Vector3 ToTarget(bool right)
		{
			Vector3 from = rig.playSpace.TransformPoint(Shoulder(right));
			Vector3 to = rig.playSpace.TransformPoint(right ? handLocalR : handLocalL) - from;
			if (to.sqrMagnitude > 0.0001f)
			{
				return to.normalized;
			}
			return -body.chest.transform.up;
		}

		private void AimArm(bool right, Vector3 upperWant, Vector3 foreWant)
		{
			Vector3 aim = right ? aimR : aimL;
			if (aim.sqrMagnitude < 0.0001f)
			{
				aim = upperWant;
			}
			aim = Vector3.RotateTowards(aim, upperWant, aimSlew * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f).normalized;
			if (right)
			{
				aimR = aim;
			}
			else
			{
				aimL = aim;
			}
			drive.Point(right ? body.rightUpperArm : body.leftUpperArm, aim);
			drive.Point(right ? body.rightForearm : body.leftForearm, Vector3.Slerp(aim, foreWant, 0.35f).normalized);
		}

		private void Guard()
		{
			Block();
			GuardHand(false);
			GuardHand(true);
		}

		private void Block()
		{
			blocking = false;
			if (!blocks || !targetBody || !body.head)
			{
				return;
			}
			Vector3 head = body.head.position;
			float best = blockRange;
			Rigidbody threat = null;
			for (int i = 0; i < 2; i++)
			{
				Rigidbody hand = (i == 0) ? targetBody.leftHand : targetBody.rightHand;
				if (!hand)
				{
					continue;
				}
				Vector3 to = head - hand.position;
				float d = to.magnitude;
				if (!(d >= best) && !(d < 0.05f) && !(Vector3.Dot(to / d, hand.linearVelocity) < blockSpeed))
				{
					best = d;
					threat = hand;
				}
			}
			if (threat)
			{
				Vector3 local = rig.playSpace.InverseTransformPoint(Vector3.Lerp(head, threat.position, 0.5f));
				blockPoint = local;
				blocking = true;
				blockRight = local.x >= 0f;
			}
		}

		private void GuardHand(bool right)
		{
			if (blocking && blockRight == right)
			{
				Reach(right, blockPoint);
				return;
			}
			if (posed)
			{
				Reach(right, right ? guardPosR : guardPosL, right ? guardRotR : guardRotL, armReach);
				return;
			}
			Vector3 shoulder = Shoulder(right);
			Reach(right, new Vector3(right ? (shoulder.x - guardIn) : (shoulder.x + guardIn), shoulder.y + guardUp, shoulder.z + guardOut));
		}

		private void OnDisable()
		{
			alive.Remove(this);
			if (Application.isEditor && Application.isPlaying && hitLog.Length != 0)
			{
				File.WriteAllText(Application.dataPath + "/../DummyHits.txt", hitLog.ToString());
			}
		}

		private void Reach(bool right, Vector3 local)
		{
			Reach(right, local, Fist(right), armReach);
		}

		private void Reach(bool right, Vector3 local, Quaternion rot)
		{
			Reach(right, local, rot, armReach);
		}

		private void Reach(bool right, Vector3 local, float radius)
		{
			Reach(right, local, Fist(right), radius);
		}

		private Quaternion Fist(bool right)
		{
			Vector3 face = (faceLocal.sqrMagnitude > 0.0001f) ? faceLocal : Vector3.forward;
			Quaternion key = self ? Quaternion.Inverse(Quaternion.Euler(self.handKey)) : Quaternion.identity;
			Vector3 fore = face;
			if (body)
			{
				Rigidbody arm = right ? body.rightForearm : body.leftForearm;
				Rigidbody hand = right ? body.rightHand : body.leftHand;
				if (arm && hand)
				{
					Vector3 along = rig.playSpace.InverseTransformDirection(hand.position - arm.position);
					if (along.sqrMagnitude > 0.0004f)
					{
						fore = along.normalized;
					}
				}
			}
			Vector3 side = Vector3.ProjectOnPlane(Vector3.up, fore);
			if (side.sqrMagnitude < 0.01f)
			{
				side = Vector3.ProjectOnPlane(-face, fore);
			}
			side = side.normalized;
			return Quaternion.LookRotation(Vector3.Cross(right ? -side : side, fore), fore) * Quaternion.Euler(wristTilt) * key;
		}

		private void Reach(bool right, Vector3 local, Quaternion rot, float radius)
		{
			Vector3 shoulder = Shoulder(right);
			Vector3 want = shoulder + Vector3.ClampMagnitude(local - shoulder, radius);
			float slew;
			if ((pouncing && launched) || clingJoint)
			{
				slew = swingSlew * 1.4f;
			}
			else if (Time.time < punchUntil && punchRight == right && SwingLive)
			{
				slew = swingSlew;
			}
			else if (punchRight == right && recover > 0f && Time.time < swungAt + recover)
			{
				float t = (Time.time - swungAt) / recover;
				slew = Mathf.Lerp(returnSlew, handSlew, t * t * t);
			}
			else if (raiseTime > 0f && Time.time < guardFrom + raiseTime)
			{
				float t2 = (Time.time - guardFrom) / raiseTime;
				slew = Mathf.Lerp(raiseSlew, handSlew, t2 * t2 * t2);
			}
			else
			{
				slew = handSlew;
			}
			if (!right)
			{
				if (!handKnownL)
				{
					handLocalL = rig.playSpace.InverseTransformPoint(body.leftHand.position);
					handKnownL = true;
				}
				handLocalL = Vector3.MoveTowards(handLocalL, want, slew * Time.fixedDeltaTime);
				rig.aiHandL = true;
				rig.aiHandPosL = handLocalL;
				rig.aiHandRotL = Quaternion.RotateTowards(rig.aiHandRotL, rot, Time.fixedDeltaTime * 360f);
				return;
			}
			if (!handKnownR)
			{
				handLocalR = rig.playSpace.InverseTransformPoint(body.rightHand.position);
				handKnownR = true;
			}
			handLocalR = Vector3.MoveTowards(handLocalR, want, slew * Time.fixedDeltaTime);
			rig.aiHandR = true;
			rig.aiHandPosR = handLocalR;
			rig.aiHandRotR = Quaternion.RotateTowards(rig.aiHandRotR, rot, Time.fixedDeltaTime * 360f);
		}

		private float Twist()
		{
			float progress = Mathf.Clamp01(1f - (punchUntil - Time.time) / Mathf.Max(punchTime, 0.01f));
			float twist = (progress < wind) ? Mathf.Lerp(0f, Twisting, progress / Mathf.Max(wind, 0.01f)) : Mathf.Lerp(Twisting, -20f, Mathf.Clamp01((progress - wind) / Mathf.Max(1f - wind, 0.01f)));
			return punchRight ? twist : (0f - twist);
		}

		private static void Lift(Rigidbody rb)
		{
			if (rb)
			{
				rb.AddForce(Physics.gravity * -0.9f, ForceMode.Acceleration);
			}
		}

		private void Pull(Rigidbody rb, Vector3 to, float k)
		{
			if (!rb)
			{
				return;
			}
			Rigidbody upper = (rb == body.leftHand || rb == body.leftForearm || rb == body.leftUpperArm) ? body.leftUpperArm : body.rightUpperArm;
			if (upper)
			{
				to = upper.position + Vector3.ClampMagnitude(to - upper.position, armLen * 1.15f);
			}
			rb.AddForce((to - rb.position) * k - Mathf.Sqrt(k) * 0.6f * rb.linearVelocity, ForceMode.Acceleration);
		}

		private void Throw()
		{
			if (!npc)
			{
				GuardHand(!punchRight);
			}
			float progress = Mathf.Clamp01(1f - (punchUntil - Time.time) / Mathf.Max(punchTime, 0.01f));
			float hold = dwell / Mathf.Max(punchTime, 0.01f);
			float side = punchRight ? 1f : -1f;
			float t = Mathf.Clamp01((progress - wind) / (Mathf.Clamp(1f - hold, wind + 0.05f, 1f) - wind));
			float ease = t * t * (3f - 2f * t);
			float yaw = (progress < wind) ? Mathf.Lerp(20f, windTo, progress / wind) : Mathf.Lerp(windTo, 0f - through, ease);
			Vector3 shoulder = Shoulder(punchRight);
			Vector3 aim = rig.playSpace.InverseTransformPoint(AimPoint());
			float rise = aim.y - shoulder.y;
			float height = (progress < wind) ? Mathf.Lerp(rise, rise + swingRaise, progress / wind) : Mathf.Lerp(rise + swingRaise, rise - swingDrop, ease);
			float dx = aim.x - shoulder.x;
			float dz = aim.z - shoulder.z;
			float heading = (dx * dx + dz * dz > 0.0001f) ? (Mathf.Atan2(dx, dz) * Mathf.Rad2Deg) : 0f;
			float far = Mathf.Min(Mathf.Sqrt(dx * dx + dz * dz) + swingPast, swingReach);
			float outward = (progress < wind) ? Mathf.Lerp(far * 0.6f, swingCock, progress / wind) : Mathf.Lerp(swingCock, far, ease);
			Vector3 swing = Quaternion.Euler(0f, side * yaw + (heading + slop), 0f) * Vector3.forward;
			Vector3 hand = shoulder + swing * Mathf.Max(outward, 0.05f) + Vector3.up * height;
			if (!npc)
			{
				if (swingFromShoulder && drive)
				{
					Vector3 point = rig.playSpace.TransformPoint(hand) - rig.playSpace.TransformPoint(shoulder);
					if (point.sqrMagnitude > 0.0001f)
					{
						drive.Point(punchRight ? body.rightUpperArm : body.leftUpperArm, point.normalized);
					}
				}
				Reach(punchRight, hand, Fist(punchRight), swingReach);
				rig.testWish = (progress < wind) ? (Vector3.back * 0.25f) : Vector3.forward;
				return;
			}
			Rigidbody fist = punchRight ? body.rightHand : body.leftHand;
			if (progress >= wind && !flung)
			{
				flung = true;
				Vector3 fling = Quaternion.Euler(0f, heading + slop + side * Mathf.Lerp(windTo, 0f - through, 0.35f), 0f) * Vector3.forward;
				Vector3 to = rig.playSpace.TransformPoint(shoulder + fling * far + Vector3.up * rise) - fist.position;
				if (to.sqrMagnitude > 0.0001f)
				{
					fist.AddForce(to.normalized * Fling * punchPower, ForceMode.VelocityChange);
				}
			}
			Rigidbody fore = punchRight ? body.rightForearm : body.leftForearm;
			Rigidbody upper = punchRight ? body.rightUpperArm : body.leftUpperArm;
			Vector3 goal = rig.playSpace.TransformPoint(hand);
			float pull = (progress < wind) ? WindPull : (punchPower * StrikePull);
			Lift(fist);
			Lift(fore);
			Lift(upper);
			Pull(fist, goal, pull);
			Pull(fore, Vector3.Lerp(rig.playSpace.TransformPoint(shoulder), goal, 0.6f), pull * 0.5f);
			if (!(progress < wind) && !(Flat(targetBody.pelvis.position - rig.playSpace.position).magnitude < reach + standBand))
			{
				rig.testWish = Vector3.forward;
			}
			else
			{
				rig.testWish = Vector3.zero;
			}
		}

		private Vector3 AimPoint()
		{
			Vector3 shoulder = rig.playSpace.TransformPoint(Shoulder(punchRight));
			if (aimFor != targetBody)
			{
				aimFor = targetBody;
				aimParts[0] = targetBody.head;
				aimParts[1] = targetBody.leftUpperArm;
				aimParts[2] = targetBody.rightUpperArm;
				aimParts[3] = (npc && (kind == Kind.Fat || kind == Kind.Small)) ? targetBody.pelvis : null;
			}
			Vector3 best = targetBody.chest.position;
			float bestDistance = Vector3.Distance(shoulder, best);
			Rigidbody[] array = aimParts;
			foreach (Rigidbody part in array)
			{
				if (part)
				{
					float d = Vector3.Distance(shoulder, part.position);
					if (d < bestDistance)
					{
						best = part.position;
						bestDistance = d;
					}
				}
			}
			return best;
		}

		public void EndSwing()
		{
			punchUntil = Mathf.Min(punchUntil, Time.time + impactHold);
			if (self)
			{
				self.SoftenArms(recover);
			}
		}

		private static Vector3 Flat(Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		public Vector3 Shoulder(bool right)
		{
			if (!shoulderKnown)
			{
				Player me = self ? self : GetComponent<Player>();
				Player r = rig ? rig : (me ? me : null);
				float y = r ? (r.flatHeight - 0.25f) : 1.46f;
				return new Vector3(right ? 0.315f : -0.315f, y, 0f);
			}
			Vector3 head = rig.cam ? rig.cam.localPosition : (Vector3.up * rig.flatHeight);
			return head + (right ? shoulderR : shoulderL);
		}
	}
}
