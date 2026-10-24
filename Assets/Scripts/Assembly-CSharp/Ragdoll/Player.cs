using System;
using System.Collections.Generic;
using System.IO;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

namespace Ragdoll
{
	[DefaultExecutionOrder(45)]
	public class Player : MonoBehaviour
	{
		[Serializable]
		public class Digit
		{
			public Transform bone;

			public bool thumb;

			public Quaternion open = Quaternion.identity;

			public Quaternion closed = Quaternion.identity;
		}

		public class Babble
		{
			public AudioSource source;

			private float until;

			public void Tick(bool talking, AudioClip[] clips, float volume, float pitch = 1f)
			{
				if (!talking || !source || clips == null || clips.Length == 0 || Time.time < until)
				{
					return;
				}
				AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
				source.pitch = UnityEngine.Random.Range(0.9f, 1.2f) * pitch;
				source.PlayOneShot(clip, volume);
				until = Time.time + clip.length / source.pitch + UnityEngine.Random.Range(0f, 0.05f);
			}
		}

		public class BallTouch : MonoBehaviour
		{
			public bool wall;

			private float until;

			private void OnCollisionStay(Collision c)
			{
				int count = c.contactCount;
				for (int i = 0; i < count; i++)
				{
					if (c.GetContact(i).normal.y < 0.5f)
					{
						until = Time.time + 0.1f;
					}
				}
			}

			private void FixedUpdate()
			{
				wall = Time.time < until;
			}
		}

		[Header("References")]
		public Transform playSpace;

		public Transform cam;

		public Transform view;

		public Transform controllerL;

		public Transform controllerR;

		public Transform headTarget;

		public Transform handTargetL;

		public Transform handTargetR;

		public Transform poseRoot;

		[Header("Bones")]
		public Rigidbody pelvis;

		public Rigidbody chest;

		public Rigidbody head;

		public Rigidbody leftUpperArm;

		public Rigidbody leftForearm;

		public Rigidbody rightUpperArm;

		public Rigidbody rightForearm;

		public Rigidbody leftHand;

		public Rigidbody rightHand;

		public Rigidbody leftUpperLeg;

		public Rigidbody leftLowerLeg;

		public Rigidbody rightUpperLeg;

		public Rigidbody rightLowerLeg;

		public Rigidbody[] bodies;

		public ConfigurableJoint[] joints;

		[Header("Fingers")]
		public List<Digit> leftFingers = new List<Digit>();

		public List<Digit> rightFingers = new List<Digit>();

		[Header("Moving")]
		public float speed = 3f;

		public float sprintMult = 1.8f;

		public float turnSpeed = 180f;

		[HideInInspector]
		public bool dropping;

		private bool dropAir;

		[Header("Fighting")]
		public float minSpeed = 3f;

		public float koDamage = 50f;

		public float headMul = 2f;

		public float bodyMul = 1.5f;

		public float knockoutTime = 3f;

		public int guardHits = 3;

		public float blockCooldown = 0.6f;

		public float guardRecover = 5f;

		[Header("Drowning")]
		public float respawnAfter = 4f;

		[Header("Cosmetics")]
		public Cosmetics cosmetics = new Cosmetics();

		public float hatKnockDamage = 25f;

		public int hatKnockHits = 3;

		[Header("Sounds")]
		public AudioClip[] steps;

		public AudioClip[] smacks;

		public AudioClip[] punches;

		public AudioClip knockout;

		public AudioClip block;

		public AudioClip dizzy;

		public AudioClip fall;

		public AudioClip[] swings;

		public AudioClip[] jumps;

		public AudioClip[] blabbers;

		[Header("Volume")]
		public float stepVolume = 0.7f;

		public float smackVolume = 2.5f;

		public float punchVolume = 1f;

		public float knockVolume = 1f;

		public float blockVolume = 1f;

		public float dizzyVolume = 0.15f;

		public float fallVolume = 1f;

		public float swingVolume = 0.5f;

		public float jumpVolume = 0.6f;

		public float blabberVolume = 0.8f;

		[Header("Performance")]
		[Range(0f, 1f)]
		public float foveation = 0.5f;

		[HideInInspector]
		public bool dummy;

		[HideInInspector]
		public bool drowns;

		[HideInInspector]
		public bool ghostOn;

		[HideInInspector]
		public Tracker headTracker;

		[HideInInspector]
		public Tracker trackL;

		[HideInInspector]
		public Tracker trackR;

		[HideInInspector]
		public float relaxK;

		[HideInInspector]
		public bool loose;

		[HideInInspector]
		public float hillScaleL = 1f;

		[HideInInspector]
		public float hillErrL;

		[HideInInspector]
		public float hillErrUL;

		[HideInInspector]
		public float uprightScale = 1f;

		[HideInInspector]
		public bool propped;

		[HideInInspector]
		public bool pullingUp;

		[HideInInspector]
		public float shBlendL;

		[HideInInspector]
		public float shBlendR;

		[HideInInspector]
		public bool limpArms;

		[HideInInspector]
		public float riseLockedUntil;

		[HideInInspector]
		public float autoRiseUntil;

		[HideInInspector]
		public Vector3 smoothL;

		[HideInInspector]
		public Vector3 smoothR;

		[HideInInspector]
		public float spaceCarry = 1f;

		[HideInInspector]
		public Vector3 rawGoalL;

		[HideInInspector]
		public Vector3 goalL;

		[HideInInspector]
		public Vector3 goalR;

		[HideInInspector]
		public int placedL;

		[HideInInspector]
		public int placedR;

		[HideInInspector]
		public float reachCutL;

		[HideInInspector]
		public Vector3 leadHips;

		[HideInInspector]
		public Vector3 hipHome;

		[HideInInspector]
		public Vector3 hipOffset;

		[HideInInspector]
		public Vector3 handGap;

		[HideInInspector]
		public int gapStamp;

		[HideInInspector]
		public Vector3 headGap;

		[HideInInspector]
		public bool dangling;

		[HideInInspector]
		public bool gripping;

		[HideInInspector]
		public bool feetOff;

		[HideInInspector]
		public bool onFloor;

		[HideInInspector]
		public bool ridingY;

		[HideInInspector]
		public float pelvisGoalY;

		[HideInInspector]
		public int goalBranch;

		[HideInInspector]
		public float goalStance;

		[HideInInspector]
		public float headAbovePelvis;

		[HideInInspector]
		public string lastSnap = "";

		[HideInInspector]
		public bool grounded;

		[HideInInspector]
		public float floorY;

		[HideInInspector]
		public bool holding;

		[HideInInspector]
		public bool selfPush;

		[HideInInspector]
		public bool airborne;

		[HideInInspector]
		public Vector3 neck;

		[HideInInspector]
		public float traction = 1f;

		[HideInInspector]
		public bool feetFloor;

		[HideInInspector]
		public bool selfProp;

		[HideInInspector]
		public bool floorHeld;

		[HideInInspector]
		public float turnL;

		[HideInInspector]
		public float turnR;

		[HideInInspector]
		public bool hanging;

		[HideInInspector]
		public bool ballMode;

		[HideInInspector]
		public float ballSquat;

		[HideInInspector]
		public float crouch;

		[HideInInspector]
		public bool armsLive;

		[HideInInspector]
		public bool plantedL;

		[HideInInspector]
		public bool plantedR;

		[HideInInspector]
		public bool tied;

		[HideInInspector]
		public bool heldL;

		[HideInInspector]
		public bool heldR;

		[HideInInspector]
		public float jumpCharge;

		[HideInInspector]
		public float jumpPower;

		[HideInInspector]
		public float jumpBoost = 1f;

		[HideInInspector]
		public bool kicking;

		[HideInInspector]
		public float flopBlackAt = -99f;

		[HideInInspector]
		public float airLegs;

		[HideInInspector]
		public bool jumped;

		[HideInInspector]
		public bool landing;

		[HideInInspector]
		public bool rising;

		[HideInInspector]
		public float riseProgress = 1f;

		[HideInInspector]
		public bool slipping;

		[HideInInspector]
		public float slamL = -1f;

		[HideInInspector]
		public float slamR = -1f;

		[HideInInspector]
		public float pressL = -1f;

		[HideInInspector]
		public float pressR = -1f;

		[HideInInspector]
		public float vyL;

		[HideInInspector]
		public float vyR;

		[HideInInspector]
		public bool flopped;

		[HideInInspector]
		public bool bodyDead;

		[HideInInspector]
		public bool away;

		[HideInInspector]
		public bool parkedL;

		[HideInInspector]
		public bool parkedR;

		[HideInInspector]
		public bool airHandL;

		[HideInInspector]
		public bool airHandR;

		[HideInInspector]
		public float frameTurnL;

		[HideInInspector]
		public float frameTurnR;

		[HideInInspector]
		public int unstuck;

		[HideInInspector]
		public int limps;

		[HideInInspector]
		public float reachCutR;

		[HideInInspector]
		public float gapRNow;

		[HideInInspector]
		public float cutRNow;

		[HideInInspector]
		public float stretchRNow;

		[HideInInspector]
		public float pushRNow;

		[HideInInspector]
		public float stoppedR = -1f;

		[HideInInspector]
		public float braceNow = 1f;

		[HideInInspector]
		public bool onRNow;

		[HideInInspector]
		public bool contactRNow;

		[HideInInspector]
		public bool selfRNow;

		[HideInInspector]
		public bool lockRNow;

		[HideInInspector]
		public bool ledgeL;

		[HideInInspector]
		public bool ledgeR;

		[HideInInspector]
		public int rescues;

		[HideInInspector]
		public int recovers;

		[HideInInspector]
		public float recoveredAt = -99f;

		[HideInInspector]
		public float wallDistL = -1f;

		[HideInInspector]
		public float wallDistR = -1f;

		[HideInInspector]
		public bool latchL;

		[HideInInspector]
		public bool latchR;

		[HideInInspector]
		public float gripL;

		[HideInInspector]
		public float gripR;

		[HideInInspector]
		public float thumbL;

		[HideInInspector]
		public float thumbR;

		[HideInInspector]
		public float damage;

		[HideInInspector]
		public bool guarding;

		[HideInInspector]
		public int guardLeft = 3;

		[HideInInspector]
		public bool limpWhenIdle;

		[HideInInspector]
		public bool limpOnRise;

		[HideInInspector]
		public bool knockedOut;

		[HideInInspector]
		public bool dead;

		[HideInInspector]
		public float knockedOutUntil;

		public bool carriedHold;

		public bool trainHeld;

		[HideInInspector]
		public float lastHitAt;

		[HideInInspector]
		public string lastHit = "";

		[HideInInspector]
		public float dark;

		[HideInInspector]
		public float blackout;

		[HideInInspector]
		public bool hidden;

		[HideInInspector]
		public int cutTriangles;

		[HideInInspector]
		public int localRenders;

		[HideInInspector]
		public int otherRenders;

		[HideInInspector]
		public bool ballGrounded;

		[HideInInspector]
		public bool passive;

		[HideInInspector]
		public float squat;

		[HideInInspector]
		public float legs;

		[HideInInspector]
		public bool drowned;

		[HideInInspector]
		public float diedAt = -99f;

		[HideInInspector]
		public int deaths;

		[HideInInspector]
		public bool testDrown;

		[HideInInspector]
		public bool spawnKnown;

		[HideInInspector]
		public Vector3 spawnRig;

		[HideInInspector]
		public Vector3 spawnPelvis;

		private float spawnLift;

		[HideInInspector]
		public Quaternion spawnRigRot = Quaternion.identity;

		[HideInInspector]
		public Quaternion spawnPelvisRot = Quaternion.identity;

		[HideInInspector]
		public bool on;

		[HideInInspector]
		public bool testGripL;

		[HideInInspector]
		public bool testGripR;

		[HideInInspector]
		public Vector2 testTurn;

		[HideInInspector]
		public int mode;

		[HideInInspector]
		public bool easing;

		[HideInInspector]
		public Vector3 shoveForce;

		[HideInInspector]
		public Vector3 tether;

		[HideInInspector]
		public Vector3 shoveVel;

		[HideInInspector]
		public bool ridingHead;

		public static Player main;

		[HideInInspector]
		public bool talking;

		[HideInInspector]
		public int jumpCount;

		[HideInInspector]
		public float micLevel;

		private AudioSource hipSource;

		private Babble mouth;

		private int lastSwing = -1;

		private int lastJump = -1;

		private float whooshFrom = 4f;

		private float whooshFull = 9f;

		private float talkStart = 0.3f;

		private float loudFor;

		private bool wasTalking;

		private AudioClip micClip;

		private string micName;

		private static bool micAsked;

		private readonly float[] micBuf = new float[1600];

		private float talkLevel = 0.02f;

		private float talkHold = 0.35f;

		private float talkUntil;

		public static bool voiceLive;

		public static bool ownMic;

		public static float sharedMic;

		private Rigidbody floorBody;

		[HideInInspector]
		public Vector3 lastPelvis;

		[HideInInspector]
		public bool pelvisKnown;

		[HideInInspector]
		public Vector3 lastHead;

		[HideInInspector]
		public bool headKnown;

		[HideInInspector]
		public Vector3 rode;

		[HideInInspector]
		public int leashBranch;

		[HideInInspector]
		public float leashGy;

		[HideInInspector]
		public float leashGg;

		[HideInInspector]
		public float leashExpected;

		[HideInInspector]
		public float leashRise;

		[HideInInspector]
		public Vector3 leashBias;

		[HideInInspector]
		public Vector3 rawGap;

		[HideInInspector]
		public Vector3 moveWish;

		[HideInInspector]
		public Vector3 moveAccel;

		[HideInInspector]
		public Vector3 testWish;

		[HideInInspector]
		public bool ignoreInput;

		[HideInInspector]
		public bool frozen;

		private bool floating;

		private readonly List<Renderer> floatHid = new List<Renderer>();

		private readonly List<Rigidbody> floatGrav = new List<Rigidbody>();

		[HideInInspector]
		public bool aiOn;

		[HideInInspector]
		public float aiYaw;

		[HideInInspector]
		public bool aiHandL;

		[HideInInspector]
		public bool aiHandR;

		[HideInInspector]
		public Vector3 aiHandPosL;

		[HideInInspector]
		public Vector3 aiHandPosR;

		[HideInInspector]
		public Quaternion aiHandRotL = Quaternion.identity;

		[HideInInspector]
		public Quaternion aiHandRotR = Quaternion.identity;

		[HideInInspector]
		public float moveTime;

		[HideInInspector]
		public Vector3 moveNow;

		[HideInInspector]
		public Vector3 spaceVel;

		[HideInInspector]
		public float spaceTime;

		[HideInInspector]
		public Vector3 leashGap;

		[HideInInspector]
		public Vector3 bodyGap;

		[HideInInspector]
		public float testPull;

		[HideInInspector]
		public float testPullRate = 1.5f;

		[HideInInspector]
		public float testRaise;

		[HideInInspector]
		public bool testGrab;

		[HideInInspector]
		public bool testGrabL;

		[HideInInspector]
		public bool testGrabR;

		[HideInInspector]
		public bool inVR;

		[HideInInspector]
		public bool fakeVR;

		[HideInInspector]
		public bool ghost;

		[HideInInspector]
		public bool testJump;

		[HideInInspector]
		public float testPitch;

		[HideInInspector]
		public Vector3 testLean;

		[NonSerialized]
		public float strength = 1f;

		[NonSerialized]
		public float armDamp = 4f;

		[NonSerialized]
		public float legTone = 0.5f;

		[NonSerialized]
		public float driveLegDamp = 2.5f;

		[NonSerialized]
		public float legScale = 1f;

		[NonSerialized]
		public float armScale = 1f;

		[NonSerialized]
		public bool holdOff;

		[NonSerialized]
		public float handSpring = 20000f;

		[NonSerialized]
		public float handForce = 3000f;

		[NonSerialized]
		public float restGap = 0.06f;

		[NonSerialized]
		public float legAir = 0.15f;

		[NonSerialized]
		public bool sweepOff;

		[NonSerialized]
		public float limpArmScale = 0.12f;

		[NonSerialized]
		public float handPushAir = 400f;

		[NonSerialized]
		public float handPushSpin = 300f;

		[NonSerialized]
		public float handPushSpinDamp = 6f;

		[NonSerialized]
		public Vector3 handKey = new Vector3(0f, 0f, 180f);

		[NonSerialized]
		public float elbowFromHand = 0.7f;

		[NonSerialized]
		public float elbowFromRoll = 2f;

		private static bool wristPoleFix = true;

		private float wristPole = 0.6f;

		[NonSerialized]
		public bool naturalElbow = true;

		[NonSerialized]
		public float limpTime = 1.2f;

		[NonSerialized]
		public float limpStrength = 0.05f;

		[NonSerialized]
		public float flopSpeed = 9f;

		[NonSerialized]
		public float flopArm = 0.3f;

		[NonSerialized]
		public float fist = -1f;

		[NonSerialized]
		public float maxSeparation = 0.35f;

		[NonSerialized]
		public float airControl;

		[NonSerialized]
		public float flatHeight = 1.66f;

		[NonSerialized]
		public bool grabCursor = true;

		private static readonly List<Collider> mine = new List<Collider>();

		private Collider[] selfCols;

		private readonly List<Collider> slipWalls = new List<Collider>();

		private float slipReach = 0.45f;

		private float slipDepth = 3f;

		private Rigidbody[] passL;

		private Rigidbody[] passR;

		private readonly List<Collider> passA = new List<Collider>();

		private readonly List<Collider> wallA = new List<Collider>();

		private readonly List<Collider> wallB = new List<Collider>();

		private Rigidbody[] armL;

		private Rigidbody[] armR;

		private const float FallFollow = 1f;

		private float stickCrouch;

		private const float CrouchFrom = 0.5f;

		private const float CrouchFull = 0.9f;

		private const float CrouchDepth = 0.55f;

		private Collider crossL;

		private Collider crossR;

		private bool swept;

		private static readonly RaycastHit[] armHits = new RaycastHit[16];

		private readonly List<Collider> passB = new List<Collider>();

		private float passClear = 0.03f;

		private static bool forearmPass = true;

		private static bool selfClash = true;

		public static bool feetGround = true;

		public static bool secondFollows;

		public static bool propPullOn = true;

		public static int forcePark;

		public static bool leadOnJump;

		public static bool airHands = true;

		public static bool wallTouch = true;

		public static bool neckRide = true;

		public static bool armPass = true;

		public static bool sweepLimbs = true;

		public static bool aheadFree = true;

		public static bool armAimAir = true;

		public static float teleportAt = -9f;

		public static readonly List<Collider> fingers = new List<Collider>();

		public static float forceCurl = -1f;

		private static readonly int colorId = Shader.PropertyToID("_Color");

		private static readonly int radiusId = Shader.PropertyToID("_Radius");

		private static readonly int softId = Shader.PropertyToID("_Soft");

		private static readonly int aspectId = Shader.PropertyToID("_Aspect");

		private static readonly int blurId = Shader.PropertyToID("_Blur");

		private static readonly int dimId = Shader.PropertyToID("_Dim");

		private static GameObject spark;

		private static Player blockSounds;

		private static GameObject[] pops;

		private static GameObject hitFx;

		private static Player knockSounds;

		private float elbowRoll = 12f;

		private static bool wristSoft = true;

		private float wristSpin = 3000f;

		private float wristSpinDamp = 60f;

		private float wristTorque = 20f;

		private float hipSwing = 85f;

		private float armProjectAngle = 180f;

		private float limitSpring = 400f;

		private float limitDamp = 20f;

		private float jointSlack = 0.05f;

		private float elbowLimitSpring = 5000f;

		private float elbowLimitDamp = 100f;

		private float elbowProjectAngle = 30f;

		private float spring = 3000f;

		private float damper = 80f;

		private float maxForce = 100000f;

		private float armStrength = -1f;

		private float looseDamper = 1f;

		private float appliedArmStrength = -2f;

		private float armTone = 4f;

		private float handTone = 0.05f;

		private float driveHandForce = 8f;

		private float armForce = 40f;

		private float legForce = 300f;

		private float neckTone;

		private Rigidbody[] rbOf;

		private Rigidbody[] rbParent;

		private float hunch = 14f;

		private float headStoop = 0.4f;

		private Transform[] poseBones;

		private Vector3[] aimDir;

		private Quaternion[] aimRot;

		private bool[] hasAimRot;

		private bool[] aimLocal;

		private Quaternion[] aimParent;

		private Quaternion[] startLocal;

		private bool[] isChest;

		private Quaternion[] toJoint;

		private bool[] isArm;

		private bool[] isHand;

		private bool[] isNeck;

		private bool[] isLeg;

		private float applied = -1f;

		private float appliedTone = -1f;

		private float appliedSpring = -1f;

		private float appliedDamper = -1f;

		private float appliedArmDamp = -1f;

		private float appliedHandTone = -1f;

		private float appliedNeck = -1f;

		private float appliedLeg = -1f;

		private float appliedArm = -1f;

		private float capNowL = -1f;

		private float capNowR = -1f;

		private float handSlack = 0.03f;

		private Matrix4x4 flopFromRig = Matrix4x4.identity;

		private Quaternion flopRotFromRig = Quaternion.identity;

		private bool flopFrameKnown;

		private float handHold = 500f;

		private float handPush = 400f;

		private float handPushGrip = 60f;

		private float handPushTug = 150f;

		private float handPushSecond = 100f;

		private float handVmax = 3f;

		private float handPushTorque = 200f;

		private float handPushTorqueCarry = 40f;

		private float handPushCarry = 700f;

		private float hexaSpring = 5000f;

		private float hexaDamp = 1000f;

		private float hexaForce = 1500f;

		private float hexaSpin = 3000f;

		private float hexaSpinDamp = 200f;

		private float hexaTorque = 75f;

		private float hexaVmax = 20f;

		private float hexaSpinFeed = 1f;

		private float hexaSpinMax = 40f;

		private Quaternion hexaTurnL;

		private Quaternion hexaTurnR;

		private float hexaFeed = 1f;

		private float handPushLedge = 550f;

		private float ledgeAbove = 0.1f;

		private float ledgeLift = 0.05f;

		private float ledgeSnap = 0.06f;

		private float mantleBelow = 0.25f;

		private float ledgeMass = 25f;

		private bool offLedgeGrip;

		private float floorPush = 150f;

		private float floorHand = 0.25f;

		private float floorHip = 0.5f;

		private float carryBrace = 1f;

		private float carryBraceMax = 2.5f;

		private float handPushSelf = 60f;

		private float selfPropHold = 0.5f;

		private float relockSettle = 0.15f;

		private static bool armFix = true;

		private static bool slipRelease = true;

		private float tieK = 1f;

		private float legHang = 0.12f;

		private float handVfree = 2f;

		private float handVgap = 0.12f;

		private float boneMaxSpeed = 15f;

		private float boneMaxSpin = 30f;

		private float armGrip = 1f;

		private float muscleSpring = 1500f;

		private float muscleCap = 700f;

		private float gripMuscle = 4000f;

		private float gripMuscleDamp = 500f;

		private bool gripWalls;

		private float pumpNow = 1f;

		private float swingFrom = 0.8f;

		private float swingFull = 2.2f;

		private float swingSmooth = 0.3f;

		private float swingCap = 6f;

		private float swingL;

		private float swingR;

		private Vector3 lastRelL;

		private Vector3 lastRelR;

		private bool swingKnown;

		private float armWalkFrom = 0.4f;

		private float armWalkFull = 1.2f;

		private float snapFrom = 0.7f;

		private float snapReset = 0.3f;

		private bool snapReady = true;

		private float armUp;

		private float armUpWindow = 0.1f;

		private float armUpHold = 0.25f;

		private readonly Vector3[] armTrail = new Vector3[32];

		private int armTrailAt;

		private float jumpSwingFrom = 1f;

		private float jumpSwingFull = 3f;

		private float jumpSwingMult = 1.25f;

		private float liftSpeed = 0.2f;

		private float liftMax = 0.35f;

		private float liftDrop = 1.5f;

		private float liftCarry = 0.8f;

		private float liftL;

		private float liftR;

		private float liftNow;

		private float liftFrom;

		private float liftFromY;

		private bool lifting;

		private bool liftUsed;

		private Rigidbody lastTop;

		private Rigidbody braced;

		private float legTuck = 0.65f;

		private float kneesUp = 0.25f;

		private float flopArmScale = 0.05f;

		private float hipSideHold = 0.3f;

		private float hipSpinHold = 0.35f;

		private float hipSpinKick = 1f;

		private float kickLean = 75f;

		private float kickTurn = 420f;

		private float hipTurn = 220f;

		private Vector3 rollRefL;

		private Vector3 rollRefR;

		private float rollAngL;

		private float rollAngR;

		private Vector3 bChestToShL;

		private Vector3 bChestToShR;

		private Quaternion facing = Quaternion.identity;

		private float spinNow = -1f;

		private float ropeWeight = 1f;

		private float rescueAfter = 1.5f;

		private float rescueCooldown = 4f;

		private float rescueBack = 3f;

		private float limpArmDamp = 4f;

		private float riseArmScale = 0.5f;

		private float limpMaxSpeed = 6f;

		private float limpMaxSpin = 10f;

		private float limpFade = 0.45f;

		private float unstickAfter = 0.4f;

		private float unstickGap = 0.12f;

		private float wrappedL;

		private float wrappedR;

		private bool limpKnown;

		private float armScaleWas = 1f;

		private float armDampWas = 4f;

		private float limpK;

		private float armScaleNow = 1f;

		private float armDampNow = 4f;

		private float rescuedAt = -100f;

		private float lostFloorSince = -1f;

		private bool airNow;

		private Vector3 safePelvis;

		private Vector3 safeRig;

		private readonly Vector3[] trailPelvis = new Vector3[16];

		private readonly Vector3[] trailRig = new Vector3[16];

		private int trailAt;

		private int trailCount;

		private float nextTrail;

		private bool safeKnown;

		private float armRamp = 5f;

		private float handPushSpinDampGrip = 30f;

		private float selfPushMin = 2f;

		private float selfPushHold = 0.75f;

		private float handTurn = 900f;

		private float handPress = 0.015f;

		private float handSnap = 0.35f;

		private float handMax = 12f;

		private float handEase = 0.06f;

		private static bool fastHands = true;

		private float fastEase = 0.01f;

		private float easeSlowAt = 1f;

		private float easeFastAt = 3f;

		private float catchK = 1.3f;

		private float catchBase = 1.5f;

		private float handJump = 3f;

		private readonly float[] swingTimes = new float[64];

		private readonly Vector3[] swingPosL = new Vector3[64];

		private readonly Vector3[] swingPosR = new Vector3[64];

		private int swingAt;

		private int swingSeen;

		private const float SwingWindow = 0.03f;

		private const float SwingJump = 20f;

		public const float PunchFrom = 3.5f;

		public const float PunchFull = 4.5f;

		public static bool punchRamp = true;

		public const float PunchReset = 1.5f;

		public const int SwingSteps = 6;

		private Vector3 smoothLocalL;

		private Vector3 smoothLocalR;

		private Quaternion easedRotL = Quaternion.identity;

		private Quaternion easedRotR = Quaternion.identity;

		private Transform easedL;

		private Transform easedR;

		private float placeSpeed = 5f;

		private float placeEase;

		private float placeEaseUntil = -99f;

		private float lead = 0.012f;

		private float hipLead = 0.055f;

		private float trackLead = 0.06f;

		private float accelLead = 0.5f;

		private float accelEase = 10f;

		private float leadMax = 0.05f;

		private float headSpring = 6000f;

		private float headDamp = 300f;

		private float headForce = 1000f;

		private float headSpin = 400f;

		private float headSpinDamp = 40f;

		private float hipSpring = 6000f;

		private float hipDamp = 1200f;

		private float hipForce = 2000f;

		private float hipForceHold = 4000f;

		private float ballHipDamp = 600f;

		private float ballHipForce = 600f;

		private float hipSide = 6000f;

		private float hipSideDamp = 1000f;

		private float hipSideForce = 1500f;

		private float hipSpin = 8000f;

		private float hipSpinDamp = 600f;

		private float hangSpin = 0.25f;

		private float hangTorque = 1e9f;

		private bool wasHanging;

		private float hipReach = 2.5f;

		private float airGap = 0.2f;

		private float footOff = 0.06f;

		private float elbowOut = 2.3f;

		private float elbowOutRaised = 0.5f;

		private float raisedFrom;

		private float raisedFull = 0.5f;

		private float elbowStick = 2f;

		private float elbowHandTop = 0.7f;

		private float elbowHandBand = 0.4f;

		private float wristFollow = 1f;

		private float elbowTurn = 340f;

		private float elbowTwistMax = 80f;

		private float tractionFriction = 0.6f;

		private float tuckDepth = 0.35f;

		private float dangleCrouch = 0.5f;

		private float hipMinHang = 0.35f;

		private float hipLeadMax = 0.2f;

		private float dangleKeep = 0.3f;

		private float propPull = 0.12f;

		private float propPullKeep = 0.04f;

		private float handoverTurn = 200f;

		private float handoverTurnTime = 0.6f;

		private bool wasSecondL;

		private Quaternion hexaOffL = Quaternion.identity;

		private Quaternion hexaOffR = Quaternion.identity;

		private Vector3 hexaLastL;

		private Vector3 hexaLastR;

		private bool hexaKnownL;

		private bool hexaKnownR;

		private bool hexa;

		private ConfigurableJoint hexaL;

		private ConfigurableJoint hexaR;

		private bool useHexaL;

		private bool useHexaR;

		private Vector3 goalPosL;

		private Vector3 goalPosR;

		private Quaternion goalRotL = Quaternion.identity;

		private Quaternion goalRotR = Quaternion.identity;

		private float selfPropUntilL = -9f;

		private float selfPropUntilR = -9f;

		private bool wasSecondR;

		private float handoverL = -9f;

		private float handoverR = -9f;

		private bool pullL;

		private bool pullR;

		private float relockTurn = 12f;

		private float relockSnap = 3f;

		private float relockWait = 1f;

		private float touchEndL = -9f;

		private float touchEndR = -9f;

		private bool wasOnL;

		private bool wasOnR;

		private float pullUp = 0.35f;

		private float pullRate = 0.5f;

		private float holdSlip = 0.08f;

		private float recoverDist = 0.25f;

		private float recoverTime = 2f;

		private float snapDist = 0.5f;

		private float targetJump = 0.3f;

		private static bool rearmJumps = true;

		private static bool reachFromHead = true;

		private float stuckGap = 0.4f;

		private float stuckBehind = 0.1f;

		private float stuckInside = 0.12f;

		private Vector3 armJumpL;

		private Vector3 armJumpR;

		private bool armJumpKnown;

		private float elbowReach = 0.55f;

		private float elbowDrop = 0.28f;

		private float maxReach = 0.97f;

		private readonly FrameTiming[] timing = new FrameTiming[1];

		private float cpuSum;

		private float gpuSum;

		private float gpuWorst;

		private float mainSum;

		private float renderSum;

		private float waitSum;

		private float foveateAt;

		private readonly List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();

		private int timed;

		private int gcSeen;

		private ProfilerRecorder drawRecorder;

		private ProfilerRecorder trisRecorder;

		private ProfilerRecorder setPassRecorder;

		private ProfilerRecorder castersRecorder;

		private ProfilerRecorder gcRecorder;

		private bool recording;

		private float drawSum;

		private float trisSum;

		private float setPassSum;

		private float castersSum;

		private float gcSum;

		private float appGpuSum;

		private float appGpuWorst;

		private float appCpuSum;

		private int counted;

		private int metered;

		private int perfMetrics = -1;

		private float straightFrom = 2f;

		private float straightRange = 10f;

		private float walkFrom = 0.3f;

		private float walkFull = 0.8f;

		private float strideMax = 1.5f;

		private float cadenceMin = 1f;

		private float cadenceMax = 2f;

		private float stepHeight = 0.1f;

		private float footClear = -0.005f;

		private float replantAt = 0.35f;

		private float legDamp = 0.5f;

		private float footGrip = 0.8f;

		private float dropReach = 0.25f;

		private float handGrip = 1.2f;

		private float standUpRate = 1.2f;

		private float tone = 1f;

		private float limpUntil;

		private Quaternion wristL;

		private Quaternion wristR;

		private bool centred;

		private Vector3 leadHead;

		private Vector3 leadL;

		private Vector3 leadR;

		private Vector3 lastHeadLocal;

		private Vector3 lastHandLocalL;

		private Vector3 lastHandLocalR;

		private Vector3 headVel;

		private Vector3 handVelL;

		private Vector3 handVelR;

		private bool leadRead;

		private RaycastHit[] hits = new RaycastHit[32];

		private readonly RaycastHit[] restHits = new RaycastHit[8];

		private readonly Collider[] nearBuffer = new Collider[16];

		private Vector3[] restPos;

		private Quaternion[] restRot;

		private float clearHeight = 0.8f;

		private float clearRadius = 0.15f;

		private Vector3 elbowStickL;

		private Vector3 elbowStickR;

		private bool wristRead;

		private Vector3 bChestToHead;

		private Vector3 bChestToPelvis;

		private Vector3 bindFwdLocal;

		private Vector3 bindUpLocal;

		private Quaternion bPelvisRel;

		private Quaternion headK;

		private Vector3 legUL;

		private Vector3 legLL;

		private Vector3 legUR;

		private Vector3 legLR;

		private float bHeadAbovePelvis;

		private float standHeight;

		private Quaternion relUL = Quaternion.identity;

		private Quaternion relLL = Quaternion.identity;

		private Quaternion relUR = Quaternion.identity;

		private Quaternion relLR = Quaternion.identity;

		private Transform endL;

		private Transform endR;

		private float ankleL;

		private float ankleR;

		private Vector3 plantL;

		private Vector3 plantR;

		private float floorL;

		private float floorR;

		private Vector3 hipOffL;

		private Vector3 hipOffR;

		private Vector3 goalPos;

		private Quaternion goalRot = Quaternion.identity;

		private float lostSince = -1f;

		private float hitchAt = -9f;

		private const float HangBelow = 0.4f;

		private float neckStand;

		private float standRise = 0.3f;

		private bool neckStandKnown;

		private Vector3 lastNeckWorld;

		private bool neckKnown;

		private bool poseRead;

		private Vector3 lastFwd = Vector3.forward;

		private float gait;

		private Vector3 lastGoal;

		private Vector3 walkVel;

		private bool goalKnown;

		private Rigidbody anchorHead;

		private Rigidbody anchorHip;

		private Rigidbody anchorL;

		private Rigidbody anchorR;

		private ConfigurableJoint tieL;

		private ConfigurableJoint tieR;

		private HandTouch touchL;

		private HandTouch touchR;

		private HandTouch touchFL;

		private HandTouch touchFR;

		private HandTouch touchUL;

		private HandTouch touchHead;

		private HandGrab grabL;

		private HandGrab grabR;

		private HandTouch touchUR;

		private float selfPushUntil;

		private Vector3 accelNow;

		private ConfigurableJoint tieHip;

		private ConfigurableJoint tieHead;

		private float tractionNow = 1f;

		private float sideNow = -1f;

		private bool wasHolding;

		private int hipYNow = -1;

		private Vector3 gripPointL;

		private Vector3 gripPointR;

		private float holdGoalY = float.NegativeInfinity;

		private float hangBase = float.NegativeInfinity;

		private float eyeRadius = 0.08f;

		private readonly RaycastHit[] eyeCasts = new RaycastHit[8];

		private Vector3 parkOffset = new Vector3(0.09f, -0.26f, 0.13f);

		private Vector3 parkEuler = new Vector3(20f, -60f, 0f);

		private float parkFreeAfter = 0.25f;

		private float parkAfter = 2.5f;

		private float jumpSpeed = 2.6f;

		private float jumpAirTime = 0.25f;

		private float jumpTap = 0.6f;

		private float jumpHold = 1.3f;

		private float jumpAskCharge = 1f;

		private float legKick = 0.7f;

		private float airLegPower = 45f;

		private float airLegDead = 0.15f;

		private float kickTail = 0.25f;

		private Collider[] kickCols = new Collider[0];

		private bool feetOn;

		private float flopLie = 2f;

		private float flopRecover = 3f;

		private float kickedAt = -99f;

		private float landedAt = -1f;

		private bool seatedOnce;

		private int stepsSeen;

		private int stepsWorst;

		private int frames;

		private int doubleSteps;

		private int noSteps;

		private float frameSum;

		private float frameWorst;

		private float perfAt;

		private float outSince = -1f;

		private float outWorst;

		private bool outNoted;

		private string outState = "";

		private float respawnCheckAt = -1f;

		private float lastSpeed;

		private float lastSlide;

		private float flungNoteAt;

		private float fallTop;

		public static readonly float SteepY = 0.6427876f;

		private float getUpSpeed = 1.4f;

		private float getUpHold = 0.2f;

		private float riseFrom;

		private float riseTime = 0.6f;

		private float riseStart = 0.15f;

		private float riseDamp = 800f;

		private float riseDampMax = 1500f;

		private float landBrace = 2.5f;

		private float airLockAfter = 0.15f;

		private float airSince = -1f;

		private bool wasAirborne;

		private float handPushHeap = 60f;

		private float risingAt = -1f;

		private float swingForce = 300f;

		private float anchorMaxSpeed;

		private float tieEase = 0.35f;

		private float tieEaseUntil = -99f;

		private float icePush = 175f;

		private float flopSettle = 0.5f;

		private float jumpUntil = -1f;

		private float floppedAt = -1f;

		private bool riseDue;

		private float gotUpAt = -1f;

		private bool jumpAsk;

		private float jumpAskUntil;

		private float jumpBuffer = 0.2f;

		private JointDrive headSpinSaved;

		private JointDrive headPosSaved;

		private bool headSpinOff;

		private float trackedSinceL;

		private float trackedSinceR;

		private float lostSinceL;

		private float lostSinceR;

		private Collider[] parkColsL;

		private Collider[] parkColsR;

		private Collider capL;

		private Collider capR;

		private float appliedPushL;

		private float sideCapL = float.MaxValue;

		private float sideCapR = float.MaxValue;

		private static float dragShare = 0.45f;

		private float appliedPushR;

		private float appliedSpinL = -1f;

		private float appliedSpinR = -1f;

		private bool appliedLockL;

		private bool appliedLockR;

		private float appliedDampL = -1f;

		private float appliedDampR = -1f;

		private Quaternion relockOffL = Quaternion.identity;

		private Quaternion relockOffR = Quaternion.identity;

		private float relockAtL = -9f;

		private float relockAtR = -9f;

		private float relockEase = 0.25f;

		private bool pinnedL;

		private bool pinnedR;

		private Vector3 pinRigL;

		private Vector3 pinRigR;

		private Vector3 pinOffL;

		private Vector3 pinOffR;

		private float unpinRate = 2f;

		private float airHandSpring = 2000f;

		private float airHandDamp = 40f;

		private bool ropeFreeL;

		private bool ropeFreeR;

		private float airHandSpin = 8f;

		private float airHandSpinDamp = 0.05f;

		private float airHandTorque = 10f;

		private readonly List<Rigidbody> reInterp = new List<Rigidbody>();

		private float reInterpAt = -1f;

		private float frameSlew = 1800f;

		private float frameSlewSoft = 260f;

		private float softFor = 0.7f;

		private float softUntil = -99f;

		private Quaternion prevUrL = Quaternion.identity;

		private Quaternion prevFrL = Quaternion.identity;

		private Quaternion prevUrR = Quaternion.identity;

		private Quaternion prevFrR = Quaternion.identity;

		private bool frameKnownL;

		private bool frameKnownR;

		private bool wasContactL;

		private bool wasContactR;

		private float contactEndL = -9f;

		private float contactEndR = -9f;

		private bool cappedLimp;

		private float closeRate = 12f;

		private float mittenRadius = 0.032f;

		private Vector3 massCapsuleCenter = new Vector3(0f, 0.08f, 0.007f);

		private float massCapsuleHeight = 0.15f;

		private float massCapsuleRadius = 0.04f;

		private float tipPad = 0.007f;

		private UnityEngine.XR.InputDevice devL;

		private UnityEngine.XR.InputDevice devR;

		private float koGrow = 1.5f;

		private float koMax = 6f;

		private float koForget = 45f;

		private float koShield = 1f;

		private float shieldUntil = -9f;

		private bool shieldOnRise;

		private float knockShove = 1.6f;

		private Vector2 kickLaunch = new Vector2(3f, 5.5f);

		private float kickLift = 1.5f;

		private float kickHard = 5f;

		private float knockLift = 0.2f;

		private float hurtShove = 0.8f;

		public const float HitDebounce = 0.12f;

		private float slapFrom = 2f;

		private float hitCap = 24f;

		private float armMul = 0.5f;

		private float softCap = 0.5f;

		private float anyKO = 90f;

		private float anySpeed = 10f;

		private float bigFall = 12f;

		private float damageDecay = 4f;

		private float koWeaken;

		private float koFloor = 0.4f;

		private float koAgree = 0.97f;

		private float guardHigh = 0.2f;

		private float guardLow = 0.25f;

		private float guardFront = 0.35f;

		private float guardSide = 0.25f;

		private float reviveLoose = 0.6f;

		private KnockStars stars;

		private int koCount;

		private float lastKOAt = -99f;

		private float lastHurt;

		private int lastHitFrom = -1;

		private float shoveAt = -9f;

		private float shoveRamp;

		private int shoveFrom = -1;

		private bool knockOwed;

		private float guardAt = -99f;

		private float blockAt = -99f;

		private CapsuleCollider headCap;

		private float looseUntil = -9f;

		private float veilClear = 1.25f;

		private float shutRadius = -0.62f;

		private float soft = 0.5f;

		private float darkest = 1f;

		private float blur = 0.03f;

		private float blurRadius = 0.3f;

		private float blurSoft = 0.9f;

		private float blurDim = 0.55f;

		private float shutIn = 0.12f;

		private float clearOut = 0.7f;

		private float follow = 0.25f;

		private float maxDown = 6f;

		private float flopDepth = 0.75f;

		private float muffledHz = 450f;

		private float clearHz = 22000f;

		private Color waterTint = new Color(0.05f, 0.3f, 0.45f, 0.5f);

		private float waterMuffle = 0.8f;

		private float deathBlink = 0.7f;

		private float dizzyFade = 2f;

		private bool wasDead;

		private AudioSource dizzySource;

		private bool dizzyOn;

		private float dizzyLevel;

		private bool blackShut;

		private float blackIn = 0.35f;

		private AudioSource fallSource;

		private float fallLevel;

		private float fallFrom = 7f;

		private float fallFull = 16f;

		private float fallFade = 0.25f;

		private float flopSeen = -99f;

		private Camera veilCam;

		private Renderer veil;

		private Material mat;

		private UniversalAdditionalCameraData camData;

		private bool copying;

		private AudioLowPassFilter muffle;

		private bool wasOut;

		private float depth;

		private float downSince = -99f;

		private float stepGap = 0.12f;

		private float stepPitch = 0.12f;

		private float smackFrom = 1.2f;

		private float smackFull = 5f;

		private float smackFloor = 0.7f;

		private float smackGap = 0.08f;

		private float smackPitch = 0.07f;

		private float heavyFrom = 6f;

		private float heavyFull = 20f;

		private float heavyPitch = 0.1f;

		private float near = 4f;

		private float far = 25f;

		private AudioSource footL;

		private AudioSource footR;

		private AudioSource handL;

		private AudioSource handR;

		private bool wasL;

		private bool wasR;

		private float nextStep;

		private float nextSmack;

		private int lastStep = -1;

		private int lastSmack = -1;

		private float hideWithin = 0.5f;

		private float showBeyond = 0.65f;

		private float clothesClip = 0.15f;

		private float bodyAway = 0.2f;

		private float bodyBack = 0.15f;

		private bool bodyHidden;

		private SkinnedMeshRenderer armsOnly;

		private readonly List<SkinnedMeshRenderer> sleeves = new List<SkinnedMeshRenderer>();

		private readonly List<SkinnedMeshRenderer> sleeveOf = new List<SkinnedMeshRenderer>();

		private MaterialPropertyBlock sleeveBlock;

		private Renderer[] wearing = new Renderer[0];

		private float headWeight = 0.5f;

		private SkinnedMeshRenderer full;

		private Collider shinL;

		private Collider shinR;

		private float sunkSince = -1f;

		private float sunkWorst;

		private bool sunkNoted;

		private int hatHits;

		private SkinnedMeshRenderer headless;

		private Camera eye;

		private float radius = 0.25f;

		private float mass = 10f;

		private float grip = 1f;

		private float chase = 8f;

		private float maxSpeed = 4.5f;

		private float spin = 60f;

		private float push = 400f;

		private float maxPush = 800f;

		private float maxTorque = 400f;

		private float ballSpring = 8000f;

		private float damp = 1500f;

		private float tuck = 0.5f;

		private float crouchLow = 0.12f;

		private float vaultTuck = 0.15f;

		private float jumpShare = 2f;

		private float chargeSquat = 0.25f;

		private Rigidbody rb;

		private SphereCollider col;

		private ConfigurableJoint joint;

		private BallTouch touch;

		private float rest;

		private float sag;

		private bool wasJumped;

		private readonly Collider[] around = new Collider[8];

		private float underFor = 0.3f;

		private Water water;

		private float underSince = -1f;

		private bool handedOver;

		private bool fellEarly;

		private float nextFind;

		private float riseTo = 1.6f;

		private float back = 0.8f;

		private float pullOut = 1.2f;

		private float pullSpeed = 3f;

		private float pullFor;

		private float pullAt;

		private Vector3 pullFrom;

		private float gripAt = 0.6f;

		private float minScale = 0.3f;

		private float maxScale = 6f;

		private GameObject handModelL;

		private GameObject handModelR;

		private Material handMaterialL;

		private Material handMaterialR;

		private Vector3 handOffsetL = new Vector3(0f, 0f, 0.035f);

		private Vector3 handOffsetR = new Vector3(0f, 0f, 0.035f);

		private Vector3 handEulerL = new Vector3(50f, 0f, 0f);

		private Vector3 handEulerR = new Vector3(50f, 0f, 0f);

		private float handModelScale = 1f;

		private bool showLive;

		private bool hideWhileGrabbing = true;

		private bool hideBattery = true;

		private Material handMaterial;

		private float handRadius = 0.03f;

		private float handLength = 0.12f;

		private UnityEngine.XR.InputDevice ghostLeft;

		private UnityEngine.XR.InputDevice ghostRight;

		private Vector3 target;

		private Vector3 anchorWorld;

		private Vector3 mid0;

		private Vector3 midLocal;

		private float sep0;

		private float scale0;

		private bool staleL;

		private bool staleR;

		private GameObject shapeL;

		private GameObject shapeR;

		private Mesh ghostMeshL;

		private Mesh ghostMeshR;

		private Material ghostMat;

		private MaterialPropertyBlock ghostBlock;

		private float fadeL;

		private float fadeR;

		private float ghostFadeIn = 0.4f;

		private float wristFade = 0.06f;

		private Vector3 headFromCam = new Vector3(0f, -0.08f, -0.06f);

		private Quaternion headTurnFromCam = Quaternion.identity;

		private bool headFromCamKnown;

		private List<Digit> ghostFingersL = new List<Digit>();

		private List<Digit> ghostFingersR = new List<Digit>();

		private Vector3 glide;

		private float glideFade = 2.5f;

		private float gripSlip = 0.12f;

		private float glideMax = 8f;

		private float glideSmooth = 0.05f;

		private HandGrab ghostGrabL;

		private HandGrab ghostGrabR;

		private float accel = 10f;

		private float decel = 12f;

		private float deadzone = 0.15f;

		private float shoveDrag = 4f;

		private float shoveMax = 2.5f;

		private float sink = -0.3386196f;

		private Vector3 neckPivot = new Vector3(0f, -0.11f, -0.05f);

		private float followRate = 25f;

		private float followDeadY = 0.03f;

		private float followSmooth = 0.06f;

		private float liftFollow = 0.06f;

		private Vector3 followVel;

		private float followVelY;

		private float lastNeckLocalY;

		private bool neckLocalKnown;

		private float leashRate = 8f;

		private float leashSmooth = 12f;

		private float leashMax = 6f;

		private float headClose = 3f;

		private float headHang = 0.5f;

		private float hangUntil = -10f;

		private float rideVelY;

		private float landSmooth = 0.05f;

		private float squatFollow = 15f;

		private float squatShown;

		private float leashDead = 0.1f;

		private float leashDeadY = 0.015f;

		private float leashFollow = 40f;

		private float leashFollowY = 10f;

		private Vector3 homeEase;

		private bool homed;

		private float airVel;

		private Vector3 rideVel;

		[HideInInspector]
		public Vector3 rideCarry;

		private bool falling;

		private float slipMin = 0.4f;

		private float slipRide = 0.5f;

		private float slipRideSpeed = 0.3f;

		private float biasRate = 1f;

		private float biasMax = 0.08f;

		private Vector3 bias;

		private bool wasTracking;

		private Vector3 slack;

		private float armsTired;

		private float lastLandedAt = -9f;

		private float tirePerHit = 0.08f;

		private float tireStart = 0.5f;

		private float tireFrom = 2.5f;

		private float tireGap = 0.15f;

		private float tireRest = 1f;

		private float tireRecover = 0.3f;

		private float tiredEase = 0.2f;

		[NonSerialized]
		public float turnRate = 200f;

		[NonSerialized]
		public float sturdy = 1f;

		private const float LegLength = 0.4256f;

		private float stuckFrom = 0.12f;

		private float stuckRange = 0.15f;

		private float deskPull = 0.6f;

		private float deskRaise = 0.5f;

		private float deskPullNow;

		private float deskRaiseNow;

		private float armAngleNow = -55f;

		private float angL = -55f;

		private float angR = -55f;

		private float reachL = 0.5f;

		private float reachR = 0.5f;

		private bool deskGrabL;

		private bool deskGrabR;

		private Vector3 homeL;

		private Vector3 homeR;

		private bool homesKnown;

		private float flatCrouch = 1.0292f;

		private float look = 0.08f;

		private float maxPitch = 80f;

		private UnityEngine.XR.InputDevice leftDevice;

		private UnityEngine.XR.InputDevice rightDevice;

		private float yaw;

		private float pitch;

		private float peak;

		private float nextLog;

		private float worst;

		private float jumpChargeTime = 0.6f;

		private float kickFrom = 0.1f;

		private float kickFull = 0.4f;

		private bool jumpHeld;

		private float jumpSince;

		private bool heldRecentre;

		private float headPinUp = 0.06f;

		private float headPinRate = 25f;

		private float pinFade = 0.6f;

		private float pinFadeUntil = -9f;

		private float raiseNarrow = 0.25f;

		private float shoulderDrop = 0.25f;

		private float handFollow;

		private static StreamWriter noteFile;

		private Collider standCol;

		private float standTop;

		private float standAt = -9f;

		private bool fellNoted;

		private static readonly RaycastHit[] buriedHits = new RaycastHit[16];

		private static readonly Collider[] buriedCols = new Collider[16];

		private MeshRenderer[] onHead = new MeshRenderer[0];

		private bool vanished;

		private bool BodyFirst => playSpace;

		public bool TieHeldL => heldL;

		public bool TieHeldR => heldR;

		public bool Lifting => lifting;

		public bool Grabbing => (grabL && grabL.grabbing) || (grabR && grabR.grabbing);

		public Vector3 StickR => elbowStickR;

		public Rigidbody Body => rb;

		public Rigidbody Standing => grounded ? floorBody : null;

		public bool GhostPose(out Vector3 headAt, out Quaternion headTurn, out Vector3 leftAt, out Quaternion leftTurn, out Vector3 rightAt, out Quaternion rightTurn, out float size)
		{
			headAt = leftAt = rightAt = Vector3.zero;
			headTurn = leftTurn = rightTurn = Quaternion.identity;
			size = 0f;
			if (!ghost || !on || easing || !cam || !controllerL || !controllerR)
			{
				return false;
			}
			size = cam.lossyScale.x;
			headAt = cam.TransformPoint(headFromCam);
			headTurn = cam.rotation * headTurnFromCam;
			Transform left = shapeL ? shapeL.transform : controllerL;
			Transform right = shapeR ? shapeR.transform : controllerR;
			leftAt = left.position;
			leftTurn = left.rotation;
			rightAt = right.position;
			rightTurn = right.rotation;
			return true;
		}

		private void Awake()
		{
			dummy = GetComponent<SlapDummy>();
			if (dummy && cam)
			{
				flatHeight = cam.localPosition.y;
			}
			if (!dummy)
			{
				headTracker = new Tracker(cam, XRNode.Head);
				trackL = new Tracker(controllerL, XRNode.LeftHand);
				trackR = new Tracker(controllerR, XRNode.RightHand);
				trackL.extrapolate = trackR.extrapolate = true;
			}
			AwakeFighter();
			AwakeSounds();
			AwakeBones();
			AwakeGlove();
			drowns = playSpace;
		}

		private void OnEnable()
		{
			if (!dummy)
			{
				OnEnableKnockOutView();
				OnEnableHeadHide();
				Application.onBeforeRender += FollowTrackers;
			}
		}

		private void OnDisable()
		{
			if (!dummy)
			{
				OnDisableKnockOutView();
				OnDisableHeadHide();
				OnDisableBall();
				Application.onBeforeRender -= FollowTrackers;
			}
		}

		private void Start()
		{
			if (!dummy)
			{
				StartHeadHide();
			}
			if (!dummy && playSpace)
			{
				StartOutfit();
			}
			if (!dummy && playSpace)
			{
				cosmetics.NearClip(clothesClip);
			}
			if (!dummy && playSpace)
			{
				turnSpeed = PlayerPrefs.GetFloat("turnSpeed", turnSpeed);
				Prefs.Apply();
			}
			StartRig();
			StartBody();
			if (!dummy)
			{
				StartBall();
			}
			if (drowns)
			{
				StartDrown();
			}
			StartDrive();
		}

		private void FixedUpdate()
		{
			stepsSeen++;
			if (dropping)
			{
				if (!grounded)
				{
					dropAir = true;
				}
				else if (dropAir && pelvis && pelvis.linearVelocity.y > -1f)
				{
					dropping = false;
				}
			}
			RestArms();
			FixedUpdateFighter();
			FixedUpdateSounds();
			FixedUpdateBody();
			if (!dummy)
			{
				FixedUpdateBall();
			}
			if (drowns)
			{
				FixedUpdateDrown();
			}
			FixedUpdateDrive();
		}

		private void Update()
		{
			Mic();
			if (!dummy)
			{
				Tracker.frameAt = Time.time;
				Tracker.frameDt += (Time.deltaTime - Tracker.frameDt) * 0.1f;
				FollowTrackers();
			}
			UpdateRig();
			if (ghostOn)
			{
				UpdateGhost();
			}
			if (!dummy)
			{
				Watch();
			}
		}

		public static void Note(string text)
		{
			string line = "[Ev] " + Time.time.ToString("F2") + " " + text;
			Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "{0}", line);
			if (Application.isEditor)
			{
				return;
			}
			if (noteFile == null)
			{
				string dir = Application.persistentDataPath + "/Notes";
				Directory.CreateDirectory(dir);
				string[] files = Directory.GetFiles(dir, "notes_*.txt");
				Array.Sort(files);
				for (int i = 0; i < files.Length - 9; i++)
				{
					File.Delete(files[i]);
				}
				noteFile = new StreamWriter(dir + "/notes_" + DateTime.Now.ToString("MMdd_HHmmss") + ".txt", true);
				noteFile.AutoFlush = true;
			}
			noteFile.WriteLine(DateTime.Now.ToString("HH:mm:ss.ff") + " " + line);
		}

		private void Foveate()
		{
			if (!inVR || Application.isEditor || Time.unscaledTime < foveateAt)
			{
				return;
			}
			foveateAt = Time.unscaledTime + 5f;
			SubsystemManager.GetSubsystems(displays);
			foreach (XRDisplaySubsystem display in displays)
			{
				if (!display.running || Mathf.Approximately(display.foveatedRenderingLevel, foveation))
				{
					continue;
				}
				display.foveatedRenderingLevel = foveation;
				Note("foveation level " + foveation.ToString("F2") + " set, now " + display.foveatedRenderingLevel.ToString("F2") + " (only works without post-processing and HDR)");
			}
		}

		private void Meter()
		{
			if (!recording)
			{
				recording = true;
				drawRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
				trisRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
				setPassRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
				castersRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Shadow Casters Count");
				gcRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
				return;
			}
			drawSum += drawRecorder.Valid ? drawRecorder.LastValue : 0;
			trisSum += trisRecorder.Valid ? trisRecorder.LastValue : 0;
			setPassSum += setPassRecorder.Valid ? setPassRecorder.LastValue : 0;
			castersSum += castersRecorder.Valid ? castersRecorder.LastValue : 0;
			gcSum += gcRecorder.Valid ? gcRecorder.LastValue : 0;
			counted++;
			if (perfMetrics < 0)
			{
				perfMetrics = 0;
				if (Application.isEditor)
				{
					return;
				}
				try
				{
					perfMetrics = OVRPlugin.IsPerfMetricsSupported(OVRPlugin.PerfMetrics.App_GpuTime_Float) ? 1 : 0;
				}
				catch (Exception e)
				{
					Note("perf metrics failed: " + e.Message);
				}
				Note("perf metrics " + (perfMetrics == 1 ? "on" : "not supported") + ", counters draw " + drawRecorder.Valid + " tris " + trisRecorder.Valid + " gc " + gcRecorder.Valid);
			}
			if (perfMetrics != 1)
			{
				return;
			}
			float? gpu = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.App_GpuTime_Float);
			if (!gpu.HasValue)
			{
				return;
			}
			float gpuMs = gpu.Value < 1f ? gpu.Value * 1000f : gpu.Value;
			float? cpu = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.App_CpuTime_Float);
			float cpuMs = cpu.HasValue ? (cpu.Value < 1f ? cpu.Value * 1000f : cpu.Value) : 0f;
			metered++;
			appGpuSum += gpuMs;
			appCpuSum += cpuMs;
			appGpuWorst = Mathf.Max(appGpuWorst, gpuMs);
		}

		private string MeterText()
		{
			string text = "";
			if (counted >= 1)
			{
				text = text + ", draws " + (drawSum / counted).ToString("F0") + " setpass " + (setPassSum / counted).ToString("F0") + " tris " + (trisSum / counted / 1000f).ToString("F0") + "k casters " + (castersSum / counted).ToString("F0") + " gc " + (gcSum / counted / 1024f).ToString("F1") + " KB/frame";
			}
			if (metered >= 1)
			{
				float? gpuUtil = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.System_GpuUtilPercentage_Float);
				float? cpuUtil = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.System_CpuUtilAveragePercentage_Float);
				float? cpuWorst = OVRPlugin.GetPerfMetricsFloat(OVRPlugin.PerfMetrics.System_CpuUtilWorstPercentage_Float);
				int? dropped = OVRPlugin.GetPerfMetricsInt(OVRPlugin.PerfMetrics.Compositor_DroppedFrameCount_Int);
				text = text + ", app gpu " + (appGpuSum / metered).ToString("F1") + " ms (worst " + appGpuWorst.ToString("F1") + ") app cpu " + (appCpuSum / metered).ToString("F1") + " ms, gpu util " + (gpuUtil.HasValue ? gpuUtil.Value.ToString("F2") : "?") + " cpu util " + (cpuUtil.HasValue ? cpuUtil.Value.ToString("F2") : "?") + " worst core " + (cpuWorst.HasValue ? cpuWorst.Value.ToString("F2") : "?") + " dropped " + (dropped.HasValue ? dropped.Value.ToString() : "?");
			}
			return text;
		}

		private void Watch()
		{
			Foveate();
			float dt = Time.unscaledDeltaTime;
			frames++;
			frameSum += dt;
			frameWorst = Mathf.Max(frameWorst, dt);
			FrameTimingManager.CaptureFrameTimings();
			float gpu = -1f;
			if (FrameTimingManager.GetLatestTimings(1, timing) != 0)
			{
				FrameTiming t = timing[0];
				timed++;
				cpuSum += (float)t.cpuFrameTime;
				mainSum += (float)t.cpuMainThreadFrameTime;
				renderSum += (float)t.cpuRenderThreadFrameTime;
				waitSum += (float)t.cpuMainThreadPresentWaitTime;
				gpu = (float)t.gpuFrameTime;
				gpuSum += gpu;
				gpuWorst = Mathf.Max(gpuWorst, gpu);
			}
			Meter();
			int gc = GC.CollectionCount(0);
			if (dt > 0.05f && Time.unscaledTime > 5f && Time.unscaledTime > hitchAt + 1f && !Application.isEditor)
			{
				hitchAt = Time.unscaledTime;
				Note("hitch " + (dt * 1000f).ToString("F0") + " ms" + (gc == gcSeen ? "" : ", garbage collection ran") + (Time.timeSinceLevelLoad < 5f ? ", " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + " loaded " + Time.timeSinceLevelLoad.ToString("F1") + " s ago" : "") + (gpu >= 0f ? ", gpu " + gpu.ToString("F0") + " ms" : ""));
			}
			gcSeen = gc;
			stepsWorst = Mathf.Max(stepsWorst, stepsSeen);
			if (stepsSeen >= 2)
			{
				doubleSteps++;
			}
			else if (stepsSeen == 0)
			{
				noSteps++;
			}
			stepsSeen = 0;
			if (Time.unscaledTime > perfAt)
			{
				if (perfAt > 0f && frames >= 1)
				{
					Note("perf fps " + (frames / Mathf.Max(frameSum, 0.001f)).ToString("F0") + " worst frame " + (frameWorst * 1000f).ToString("F0") + " ms, physics steps per frame max " + stepsWorst + " (2+ in " + (doubleSteps * 100f / Mathf.Max(frames, 1)).ToString("F0") + "%, 0 in " + (noSteps * 100f / Mathf.Max(frames, 1)).ToString("F0") + "%)" + (inVR ? " vr" : " desk") + (timed > 0 ? ", cpu " + (cpuSum / timed).ToString("F1") + " ms (main " + (mainSum / timed).ToString("F1") + " render " + (renderSum / timed).ToString("F1") + " present wait " + (waitSum / timed).ToString("F1") + ") gpu " + (gpuSum / timed).ToString("F1") + " ms (worst " + gpuWorst.ToString("F0") + ")" : "") + MeterText() + (ghost ? " ghost" : "") + " in " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
				}
				timed = 0;
				cpuSum = gpuSum = renderSum = waitSum = gpuWorst = mainSum = 0f;
				drawSum = trisSum = setPassSum = castersSum = gcSum = 0f;
				appGpuSum = appGpuWorst = appCpuSum = 0f;
				counted = metered = 0;
				stepsWorst = frames = doubleSteps = noSteps = 0;
				frameSum = frameWorst = 0f;
				perfAt = Time.unscaledTime + 10f;
			}
			if (respawnCheckAt > 0f && Time.time > respawnCheckAt && pelvis)
			{
				respawnCheckAt = -1f;
				Note("after respawn: pelvis " + pelvis.position.ToString("F1") + (grounded ? " standing" : " NOT on the ground") + ", speed " + pelvis.linearVelocity.magnitude.ToString("F1") + " m/s");
			}
			if (pelvis && !drowned)
			{
				float speed = pelvis.linearVelocity.magnitude;
				Vector3 vel = pelvis.linearVelocity;
				float slide = new Vector3(vel.x - rideCarry.x, 0f, vel.z - rideCarry.z).magnitude;
				bool burst = speed > 8f && lastSpeed < 5f;
				if (Time.time > flungNoteAt && (burst || (slide > 8f && lastSlide <= 8f)))
				{
					flungNoteAt = Time.time + 1f;
					Note("flung to " + speed.ToString("F1") + " m/s" + (burst ? " in one frame" : ", " + slide.ToString("F1") + " m/s against the floor carry") + " | " + State() + " | last hit " + lastHit);
				}
				lastSpeed = speed;
				lastSlide = slide;
			}
			if (head && playSpace && !ghost && !drowned)
			{
				Sunk();
				Vector3 headAt = head.transform.position;
				Vector3 neckAt = Neck();
				float side = Mathf.Sqrt((headAt.x - neckAt.x) * (headAt.x - neckAt.x) + (headAt.z - neckAt.z) * (headAt.z - neckAt.z));
				float up = Mathf.Abs(headAt.y - neckAt.y);
				float gap = Mathf.Max(side, up);
				if (gap > 0.3f)
				{
					if (outSince < 0f)
					{
						outWorst = 0f;
						outSince = Time.time;
						outState = State();
					}
					outWorst = Mathf.Max(outWorst, gap);
					if (!outNoted && Time.time - outSince > 0.15f)
					{
						outNoted = true;
						Note("outofbody start side " + side.ToString("F2") + " up " + up.ToString("F2") + " began " + outState + " | now " + State());
					}
					return;
				}
				if (outSince < 0f)
				{
					return;
				}
				if (outNoted)
				{
					Note("outofbody end after " + (Time.time - outSince).ToString("F2") + " s worst " + outWorst.ToString("F2") + " m | " + State());
				}
				outNoted = false;
			}
			outSince = -1f;
		}

		private void Sunk()
		{
			if (!shinL && leftLowerLeg)
			{
				shinL = leftLowerLeg.GetComponent<Collider>();
			}
			if (!shinR && rightLowerLeg)
			{
				shinR = rightLowerLeg.GetComponent<Collider>();
			}
			if (!shinL || !shinR)
			{
				return;
			}
			float bottom = Mathf.Min(shinL.bounds.min.y, shinR.bounds.min.y);
			if (grounded)
			{
				float sunk = floorY - bottom;
				if (sunk >= 0.1f && !flopped)
				{
					if (sunkSince < 0f)
					{
						sunkSince = Time.time;
						sunkWorst = 0f;
					}
					sunkWorst = Mathf.Max(sunkWorst, sunk);
					if (sunkNoted || Time.time - sunkSince < 0.3f)
					{
						return;
					}
					sunkNoted = true;
					Note("feet sunk " + (sunk * 100f).ToString("F0") + " cm into the floor | pelvis " + (pelvis.position.y - floorY).ToString("F2") + " m above it (stand " + standHeight.ToString("F2") + "), crouch " + crouch.ToString("F2") + ", squat " + ballSquat.ToString("F2") + ", goal branch " + goalBranch + " | " + State());
					return;
				}
			}
			if (sunkNoted)
			{
				Note("feet out of the floor after " + (Time.time - sunkSince).ToString("F1") + " s, worst " + (sunkWorst * 100f).ToString("F0") + " cm");
			}
			sunkNoted = false;
			sunkSince = -1f;
		}

		private string State()
		{
			return (grounded ? "grounded" : "air") + (jumped ? " jumped" : "") + (landing ? " landing" : "") + (flopped ? " flopped" : "") + (rising ? " rising" : "") + (holding ? " holding" : "") + (gripping ? " gripping" : "") + (hanging ? " hanging" : "") + (knockedOut ? " KO" : "") + " stick " + moveWish.magnitude.ToString("F1") + " rig " + moveNow.magnitude.ToString("F1") + " body " + (pelvis ? pelvis.linearVelocity.magnitude : 0f).ToString("F1") + " m/s carry " + rideCarry.magnitude.ToString("F1") + (floorBody ? " on " + floorBody.name : "");
		}

		private void LateUpdate()
		{
			if (!dummy)
			{
				LateUpdateKnockOutView();
				FallWind();
			}
			LateUpdateGlove();
			TrackSwing();
		}

		[BeforeRenderOrder(90)]
		private void FollowTrackers()
		{
			if (ignoreInput)
			{
				headTracker.Clear();
				trackL.Clear();
				trackR.Clear();
				return;
			}
			headTracker.Follow();
			if (trackL.enabled)
			{
				trackL.Follow();
			}
			if (trackR.enabled)
			{
				trackR.Follow();
			}
		}

		private void AwakeBones()
		{
			Limits();
			mine.Clear();
			foreach (Rigidbody body in bodies)
			{
				if (body)
				{
					mine.AddRange(body.GetComponents<Collider>());
				}
			}
			for (int i = 0; i < mine.Count; i++)
			{
				for (int j = i + 1; j < mine.Count; j++)
				{
					Physics.IgnoreCollision(mine[i], mine[j], true);
				}
			}
		}

		private void Limits()
		{
			foreach (ConfigurableJoint j in joints)
			{
				if (!j)
				{
					continue;
				}
				j.projectionMode = JointProjectionMode.PositionAndRotation;
				j.projectionDistance = jointSlack;
				j.projectionAngle = 10f;
				Rigidbody rb = j.GetComponent<Rigidbody>();
				if (rb == leftForearm || rb == rightForearm || rb == leftUpperArm || rb == rightUpperArm)
				{
					SoftJointLimitSpring limit = new SoftJointLimitSpring { spring = limitSpring, damper = limitDamp };
					j.angularXLimitSpring = limit;
					j.angularYZLimitSpring = limit;
					j.projectionAngle = armProjectAngle;
				}
				if (rb == leftForearm || rb == rightForearm)
				{
					j.angularYLimit = new SoftJointLimit { limit = elbowRoll };
					j.angularXLimitSpring = new SoftJointLimitSpring { spring = elbowLimitSpring, damper = elbowLimitDamp };
					j.projectionAngle = elbowProjectAngle;
				}
				else if (rb == leftHand || rb == rightHand)
				{
					j.projectionAngle = armProjectAngle;
				}
				else if (rb == leftUpperLeg || rb == rightUpperLeg)
				{
					j.angularZLimit = new SoftJointLimit { limit = hipSwing };
				}
			}
		}

		public float TotalMass()
		{
			float total = 0f;
			foreach (Rigidbody body in bodies)
			{
				if (body)
				{
					total += body.mass;
				}
			}
			return total;
		}

		private void StartDrive()
		{
			int count = joints.Length;
			poseBones = new Transform[count];
			aimDir = new Vector3[count];
			aimRot = new Quaternion[count];
			hasAimRot = new bool[count];
			aimLocal = new bool[count];
			aimParent = new Quaternion[count];
			startLocal = new Quaternion[count];
			isChest = new bool[count];
			toJoint = new Quaternion[count];
			isArm = new bool[count];
			isHand = new bool[count];
			isNeck = new bool[count];
			isLeg = new bool[count];
			rbOf = new Rigidbody[count];
			rbParent = new Rigidbody[count];
			for (int i = 0; i < count; i++)
			{
				ConfigurableJoint j = joints[i];
				rbOf[i] = j.GetComponent<Rigidbody>();
				rbParent[i] = j.connectedBody;
				isArm[i] = j.name.IndexOf("arm", StringComparison.OrdinalIgnoreCase) >= 0;
				isHand[i] = j.name.IndexOf("hand", StringComparison.OrdinalIgnoreCase) >= 0;
				isNeck[i] = j.name == "Head";
				isChest[i] = j.transform == (chest ? chest.transform : null);
				isLeg[i] = j.name.IndexOf("leg", StringComparison.OrdinalIgnoreCase) >= 0;
				startLocal[i] = j.transform.localRotation;
				poseBones[i] = poseRoot ? Find(poseRoot, j.name) : null;
				Vector3 forward = Vector3.Cross(j.axis, j.secondaryAxis).normalized;
				Vector3 up = Vector3.Cross(forward, j.axis).normalized;
				toJoint[i] = Quaternion.LookRotation(forward, up);
			}
		}

		public void Loose(bool on)
		{
			loose = on;
			if (!on)
			{
				applied = -1f;
				capNowL = capNowR = -1f;
				return;
			}
			foreach (ConfigurableJoint j in joints)
			{
				if (j)
				{
					j.slerpDrive = new JointDrive
					{
						positionSpring = 0f,
						positionDamper = looseDamper,
						maximumForce = float.MaxValue
					};
				}
			}
		}

		private void FixedUpdateDrive()
		{
			if (loose)
			{
				return;
			}
			ApplyDrives();
			for (int i = 0; i < joints.Length; i++)
			{
				Quaternion want;
				if (hasAimRot[i])
				{
					Quaternion parent = aimLocal[i] ? aimParent[i] : joints[i].transform.parent.rotation;
					hasAimRot[i] = false;
					aimLocal[i] = false;
					want = Quaternion.Inverse(parent) * aimRot[i];
				}
				else if (aimDir[i].sqrMagnitude > 0.0001f)
				{
					Transform t = joints[i].transform;
					Quaternion toLocal = Quaternion.Inverse(t.parent.rotation);
					want = toLocal * Quaternion.FromToRotation(t.up, aimDir[i].normalized) * t.rotation;
					aimDir[i] = Vector3.zero;
				}
				else
				{
					want = poseBones[i] ? poseBones[i].localRotation : startLocal[i];
				}
				if (relaxK > 0.001f && (isArm[i] || isHand[i]))
				{
					want = Quaternion.Slerp(want, startLocal[i], relaxK);
				}
				if (hunch != 0f)
				{
					if (isChest[i])
					{
						want *= Quaternion.Euler(hunch, 0f, 0f);
					}
					else if (isNeck[i])
					{
						want *= Quaternion.Euler(0f - headStoop * hunch, 0f, 0f);
					}
				}
				Aim(joints[i], i, want);
			}
		}

		public void PointRot(Rigidbody bone, Quaternion world)
		{
			for (int i = 0; i < joints.Length; i++)
			{
				if (joints[i].transform == bone.transform)
				{
					aimRot[i] = world;
					hasAimRot[i] = true;
					break;
				}
			}
		}

		public void Point(Rigidbody bone, Vector3 dir)
		{
			for (int i = 0; i < joints.Length; i++)
			{
				if (joints[i].transform == bone.transform)
				{
					aimDir[i] = dir;
					break;
				}
			}
		}

		private void ApplyDrives()
		{
			if (Mathf.Approximately(appliedNeck, neckTone) && Mathf.Approximately(appliedLeg, legTone * legScale) && Mathf.Approximately(appliedArm, armScale) && Mathf.Approximately(applied, strength) && Mathf.Approximately(appliedTone, armTone) && Mathf.Approximately(appliedSpring, spring) && Mathf.Approximately(appliedDamper, damper) && Mathf.Approximately(appliedArmDamp, armDamp) && Mathf.Approximately(appliedHandTone, handTone) && Mathf.Approximately(appliedArmStrength, armStrength))
			{
				return;
			}
			applied = strength;
			appliedTone = armTone;
			appliedSpring = spring;
			appliedDamper = damper;
			appliedArmStrength = armStrength;
			appliedArmDamp = armDamp;
			appliedHandTone = handTone;
			appliedLeg = legTone * legScale;
			appliedNeck = neckTone;
			appliedArm = armScale;
			for (int i = 0; i < joints.Length; i++)
			{
				float k;
				float damp;
				if (isNeck[i])
				{
					k = damp = neckTone;
				}
				else if (isHand[i])
				{
					k = damp = handTone;
				}
				else
				{
					k = isArm[i] ? armTone * armScale : (isLeg[i] ? legTone * legScale : 1f);
					damp = isArm[i] ? armDamp : (isLeg[i] ? driveLegDamp : 1f);
				}
				float scale;
				float power;
				if (isArm[i])
				{
					scale = armScale;
					power = armStrength < 0f ? strength : armStrength;
				}
				else if (isHand[i])
				{
					scale = 1f;
					power = armStrength < 0f ? strength : armStrength;
				}
				else
				{
					scale = 1f;
					power = strength;
				}
				ConfigurableJoint j = joints[i];
				JointDrive drive = new JointDrive
				{
					positionSpring = k * power * spring,
					positionDamper = damp * damper * scale,
					maximumForce = isHand[i] ? driveHandForce : (isArm[i] ? armForce * armScale : (isLeg[i] ? legForce : maxForce))
				};
				j.slerpDrive = drive;
			}
		}

		private void Aim(ConfigurableJoint j, int i, Quaternion want)
		{
			j.targetRotation = Quaternion.Inverse(toJoint[i]) * Quaternion.Inverse(want) * startLocal[i] * toJoint[i];
		}

		private static Transform Find(Transform root, string name)
		{
			if (root.name == name)
			{
				return root;
			}
			for (int i = 0; i < root.childCount; i++)
			{
				Transform found = Find(root.GetChild(i), name);
				if (found)
				{
					return found;
				}
			}
			return null;
		}

		private Vector3 ToSpace(Vector3 world)
		{
			if (!playSpace)
			{
				return world;
			}
			return playSpace.InverseTransformPoint(world);
		}

		private Vector3 FromSpace(Vector3 local)
		{
			if (!playSpace)
			{
				return local;
			}
			float ahead = Mathf.Clamp(Time.time - spaceTime, 0f, 0.04f);
			return playSpace.TransformPoint(local) + spaceVel * (ahead * spaceCarry);
		}

		private void Ease(ref Transform eased, ref Quaternion local, Transform raw, Vector3 at)
		{
			if (!raw)
			{
				return;
			}
			Quaternion space = playSpace ? playSpace.rotation : Quaternion.identity;
			Quaternion toSpace = Quaternion.Inverse(space);
			Quaternion rotation = raw.rotation;
			if (!eased)
			{
				eased = new GameObject(raw.name + ".eased").transform;
				eased.SetParent(transform, false);
				local = toSpace * rotation;
				eased.SetPositionAndRotation(at, raw.rotation);
				return;
			}
			float ease = HandEase();
			float k = ease > 0f ? 1f - Mathf.Exp(-Time.fixedDeltaTime / ease) : 1f;
			local = Quaternion.Slerp(local, toSpace * rotation, k);
			eased.SetPositionAndRotation(at, space * local);
		}

		private Vector3 Step(Vector3 from, Vector3 to, float k, float room)
		{
			if ((to - from).sqrMagnitude > handJump * handJump)
			{
				return to;
			}
			return Vector3.MoveTowards(from, Vector3.Lerp(from, to, k), room);
		}

		private Vector3 AnchorStep(Vector3 from, Vector3 to)
		{
			if (anchorMaxSpeed <= 0f)
			{
				return to;
			}
			return Vector3.MoveTowards(from, to, anchorMaxSpeed * Time.fixedDeltaTime);
		}

		public void SoftenArms(float seconds)
		{
			softUntil = Mathf.Max(softUntil, Time.time + seconds);
		}

		private Vector3 Fallen(Vector3 world)
		{
			if (!playSpace || !chest)
			{
				return world;
			}
			if (!flopFrameKnown)
			{
				FlopFrame();
			}
			Transform chestT = chest.transform;
			return chestT.TransformPoint(flopFromRig.MultiplyPoint3x4(playSpace.InverseTransformPoint(world)));
		}

		private Quaternion Fallen(Quaternion world)
		{
			if (!playSpace || !chest)
			{
				return world;
			}
			if (!flopFrameKnown)
			{
				FlopFrame();
			}
			return chest.transform.rotation * flopRotFromRig * Quaternion.Inverse(playSpace.rotation) * world;
		}

		private void FlopFrame()
		{
			if (!playSpace || !chest)
			{
				return;
			}
			flopFromRig = chest.transform.worldToLocalMatrix * playSpace.localToWorldMatrix;
			flopRotFromRig = Quaternion.Inverse(chest.transform.rotation) * playSpace.rotation;
			flopFrameKnown = true;
		}

		private void Kick()
		{
			kicking = false;
			if (flopped)
			{
				return;
			}
			if (Mathf.Abs(airLegs) < airLegDead)
			{
				return;
			}
			if (grounded && Time.time > jumpUntil)
			{
				return;
			}
			kicking = true;
			kickedAt = Time.time;
			Vector3 fwd = new Vector3(lastFwd.x, 0f, lastFwd.z);
			if (fwd.sqrMagnitude < 0.0001f)
			{
				return;
			}
			Vector3 push = fwd.normalized * (airLegPower * airLegs);
			leftLowerLeg.AddForce(push * leftLowerLeg.mass, ForceMode.Force);
			rightLowerLeg.AddForce(push * rightLowerLeg.mass, ForceMode.Force);
			chest.AddForce(-push * (leftLowerLeg.mass + rightLowerLeg.mass), ForceMode.Force);
		}

		public bool Kicks(Rigidbody rb)
		{
			if (!rb)
			{
				return false;
			}
			if (!kicking && !(Time.time < kickedAt + kickTail))
			{
				return false;
			}
			return rb == leftLowerLeg || rb == rightLowerLeg;
		}

		private void Feet(bool on)
		{
			if (feetOn == on)
			{
				return;
			}
			if (kickCols.Length == 0)
			{
				return;
			}
			feetOn = on;
			foreach (Collider c in kickCols)
			{
				c.enabled = on;
			}
			if (!on)
			{
				return;
			}
			Collider[] all = GetComponentsInChildren<Collider>();
			for (int i = 0; i < kickCols.Length; i++)
			{
				foreach (Collider other in all)
				{
					if (other != kickCols[i] && other.enabled)
					{
						Physics.IgnoreCollision(kickCols[i], other, true);
					}
				}
			}
		}

		public bool Holds(Rigidbody rb)
		{
			if (!rb)
			{
				return false;
			}
			if (grabL && grabL.held == rb)
			{
				return true;
			}
			return grabR && grabR.held == rb;
		}

		public bool Holds(Rigidbody rb, out XRNode hand)
		{
			hand = grabR && grabR.held == rb ? XRNode.RightHand : XRNode.LeftHand;
			return Holds(rb);
		}

		public void LetGo(string why = null)
		{
			if (grabL)
			{
				if (why != null && grabL.grabbing)
				{
					grabL.releaseWhy = why;
				}
				grabL.Release();
			}
			if (grabR)
			{
				if (why != null && grabR.grabbing)
				{
					grabR.releaseWhy = why;
				}
				grabR.Release();
			}
		}

		public void GaveWay(Rigidbody rb)
		{
			if (grabL && grabL.held == rb)
			{
				grabL.GaveWay();
			}
			if (grabR && grabR.held == rb)
			{
				grabR.GaveWay();
			}
		}

		public void Regrab(bool left, Transform npc)
		{
			HandGrab g = left ? grabL : grabR;
			if (g)
			{
				g.Rebind(npc);
			}
		}

		public void Jump(float charge = 1f)
		{
			jumpAsk = true;
			jumpAskCharge = charge;
			jumpAskUntil = Time.time + jumpBuffer;
		}

		private bool PushingDown()
		{
			if (!playSpace)
			{
				return false;
			}
			bool left = Pushing(leftHand, handTargetL, touchL, touchFL, handVelL, ref slamL, ref pressL, ref vyL);
			bool right = Pushing(rightHand, handTargetR, touchR, touchFR, handVelR, ref slamR, ref pressR, ref vyR);
			return left | right;
		}

		private bool Pushing(Rigidbody hand, Transform target, HandTouch t, HandTouch tf, Vector3 vel, ref float slam, ref float press, ref float vy)
		{
			Vector3 world = playSpace.TransformDirection(vel);
			vy = Mathf.Lerp(vy, world.y, Mathf.Clamp01(Time.fixedDeltaTime / 0.08f));
			if (vy < -getUpSpeed)
			{
				slam = Time.time;
			}
			bool touching = (t && (t.touching || t.holdsYou)) || (tf && (tf.touching || tf.holdsYou));
			if (touching && hand && target && target.position.y <= hand.position.y + 0.02f && Time.time < slam + 0.5f)
			{
				if (press < 0f)
				{
					press = Time.time;
				}
				return Time.time >= press + getUpHold;
			}
			press = -1f;
			return false;
		}

		private void StartBody()
		{
			if (dummy)
			{
				Heft();
			}
			Slicken();
			Unclash();
			float lowest = float.MaxValue;
			foreach (Rigidbody body in bodies)
			{
				if (!body)
				{
					continue;
				}
				foreach (Collider c in body.GetComponents<Collider>())
				{
					lowest = Mathf.Min(lowest, c.bounds.min.y);
				}
			}
			if (lowest < float.MaxValue)
			{
				standHeight = pelvis.transform.position.y - lowest;
			}
			endL = Tip(leftLowerLeg.transform);
			endR = Tip(rightLowerLeg.transform);
			if (lowest < float.MaxValue)
			{
				if (endL)
				{
					ankleL = Mathf.Max(endL.position.y - lowest, 0f);
				}
				if (endR)
				{
					ankleR = Mathf.Max(endR.position.y - lowest, 0f);
				}
			}
			Quaternion toPelvis = Quaternion.Inverse(pelvis.rotation);
			hipOffL = toPelvis * (leftUpperLeg.position - pelvis.position);
			hipOffR = toPelvis * (rightUpperLeg.position - pelvis.position);
			foreach (Rigidbody body in bodies)
			{
				if (!body || Kind(body) != 1)
				{
					continue;
				}
				foreach (Collider c in body.GetComponents<Collider>())
				{
					c.enabled = false;
				}
			}
			List<Collider> kick = new List<Collider>();
			if (leftLowerLeg)
			{
				kick.AddRange(leftLowerLeg.GetComponents<Collider>());
			}
			if (rightLowerLeg)
			{
				kick.AddRange(rightLowerLeg.GetComponents<Collider>());
			}
			kickCols = kick.ToArray();
			restPos = new Vector3[bodies.Length];
			restRot = new Quaternion[bodies.Length];
			Quaternion inverse = Quaternion.Inverse(pelvis.rotation);
			for (int i = 0; i < bodies.Length; i++)
			{
				if (!bodies[i])
				{
					continue;
				}
				restPos[i] = inverse * (bodies[i].position - pelvis.position);
				restRot[i] = inverse * bodies[i].rotation;
			}
			foreach (Rigidbody rb in new Rigidbody[] { leftHand, rightHand, leftForearm, rightForearm })
			{
				if (rb)
				{
					rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
				}
			}
			foreach (Rigidbody body in bodies)
			{
				if (!body)
				{
					continue;
				}
				body.solverIterations = 20;
				body.solverIterations = 30;
				body.solverVelocityIterations = 10;
				body.interpolation = RigidbodyInterpolation.Interpolate;
				body.maxDepenetrationVelocity = 2f;
				body.maxLinearVelocity = boneMaxSpeed;
				body.maxAngularVelocity = boneMaxSpin;
			}
			anchorHead = Anchor("Head Anchor", head);
			tieHead = Tie(head, anchorHead);
			JointDrive headDrive = Drive(headSpring, headDamp, headForce);
			tieHead.xDrive = headDrive;
			tieHead.zDrive = headDrive;
			tieHead.slerpDrive = Drive(headSpin, headSpinDamp, float.MaxValue);
			anchorHip = Anchor("Hip Anchor", pelvis);
			ConfigurableJoint hip = Tie(pelvis, anchorHip);
			JointDrive side = Drive(hipSide, hipSideDamp, hipSideForce);
			hip.xDrive = side;
			hip.yDrive = Drive(hipSpring, hipDamp, hipForce);
			hip.zDrive = side;
			hip.slerpDrive = Drive(hipSpin, hipSpinDamp, float.MaxValue);
			tieHip = hip;
			anchorL = Anchor("Hand.L Anchor", leftHand);
			anchorR = Anchor("Hand.R Anchor", rightHand);
			hexa = pelvis && !GetComponent<SlapDummy>();
			tieL = Tie(leftHand, anchorL);
			tieR = Tie(rightHand, anchorR);
			if (hexa)
			{
				hexaL = Chest(leftHand, out hexaOffL);
				hexaR = Chest(rightHand, out hexaOffR);
			}
			capL = leftHand.GetComponent<Collider>();
			capR = rightHand.GetComponent<Collider>();
			touchL = Feel(leftHand);
			touchR = Feel(rightHand);
			passL = new Rigidbody[] { leftHand, leftForearm };
			passR = new Rigidbody[] { rightHand, rightForearm };
			armL = new Rigidbody[] { leftUpperArm, leftForearm, leftHand };
			armR = new Rigidbody[] { rightUpperArm, rightForearm, rightHand };
			touchFL = Feel(leftForearm);
			touchFR = Feel(rightForearm);
			touchUL = Feel(leftUpperArm);
			touchHead = Feel(head);
			foreach (Rigidbody body in bodies)
			{
				if (body)
				{
					body.collisionDetectionMode = CollisionDetectionMode.Discrete;
				}
			}
			grabL = Grip(leftHand, XRNode.LeftHand);
			grabR = Grip(rightHand, XRNode.RightHand);
			touchUR = Feel(rightUpperArm);
		}

		private static Vector3 Taut(HandGrab g, Vector3 hand, Vector3 gap)
		{
			if (!g || !g.grabbing)
			{
				return gap;
			}
			Rope rope = g.heldRope;
			if (!rope)
			{
				return gap;
			}
			Vector3 toRope = rope.transform.position - hand;
			if (toRope.sqrMagnitude < 0.0001f)
			{
				return gap;
			}
			Vector3 dir = toRope.normalized;
			return dir * Mathf.Max(0f, Vector3.Dot(gap, dir));
		}

		private bool Aloft(Collider c)
		{
			Rigidbody rb = c.attachedRigidbody;
			if (!rb)
			{
				return false;
			}
			bool leftHolds = grabL && grabL.grabbing && !grabL.holdsYou;
			bool rightHolds = grabR && grabR.grabbing && !grabR.holdsYou;
			if (!leftHolds && !rightHolds)
			{
				return false;
			}
			AvatarBody.AvatarHit hit = rb.GetComponent<AvatarBody.AvatarHit>();
			Transform root = (hit && hit.owner) ? hit.owner.transform : rb.transform;
			if (!Carries(grabL, root) && !Carries(grabR, root))
			{
				return false;
			}
			if (hit)
			{
				return true;
			}
			return !Resting(c);
		}

		private bool Carries(HandGrab g, Transform root)
		{
			if (!g || !g.grabbing || g.holdsYou || !g.held)
			{
				return false;
			}
			return g.held.transform.IsChildOf(root);
		}

		private bool Resting(Collider c)
		{
			Bounds bounds = c.bounds;
			Vector3 center = new Vector3(bounds.center.x, bounds.min.y + 0.1f, bounds.center.z);
			Vector3 half = new Vector3(bounds.extents.x * 0.8f, 0.02f, bounds.extents.z * 0.8f);
			int count = Physics.BoxCastNonAlloc(center, half, Vector3.down, restHits, Quaternion.identity, restGap + 0.1f, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < count; i++)
			{
				Collider other = restHits[i].collider;
				if (other.attachedRigidbody && other.attachedRigidbody == c.attachedRigidbody)
				{
					continue;
				}
				if (other.transform.IsChildOf(transform))
				{
					continue;
				}
				if (!Holds(other.attachedRigidbody))
				{
					return true;
				}
			}
			return false;
		}

		private bool Carried(Rigidbody hand, Collider c)
		{
			HandGrab g = hand == leftHand ? grabL : grabR;
			if (!g || !g.grabbing)
			{
				return false;
			}
			if (g.held && c.attachedRigidbody == g.held)
			{
				return true;
			}
			if (g.heldLink && c.attachedArticulationBody == g.heldLink)
			{
				return true;
			}
			if (!g.heldRope)
			{
				return false;
			}
			return c.transform.IsChildOf(g.heldRope.transform);
		}

		private HandGrab Grip(Rigidbody hand, XRNode node)
		{
			HandGrab grab = hand.gameObject.AddComponent<HandGrab>();
			grab.node = node;
			grab.own = transform;
			return grab;
		}

		private HandTouch Feel(Rigidbody bone)
		{
			HandTouch touch = bone.gameObject.AddComponent<HandTouch>();
			touch.own = transform;
			return touch;
		}

		private Rigidbody Anchor(string name, Rigidbody bone)
		{
			GameObject go = new GameObject(name);
			go.transform.SetPositionAndRotation(bone.transform.position, bone.transform.rotation);
			Rigidbody rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;
			return rb;
		}

		private ConfigurableJoint Tie(Rigidbody bone, Rigidbody anchor)
		{
			ConfigurableJoint j = bone.gameObject.AddComponent<ConfigurableJoint>();
			j.connectedBody = anchor;
			j.autoConfigureConnectedAnchor = false;
			j.anchor = Vector3.zero;
			j.connectedAnchor = Vector3.zero;
			j.rotationDriveMode = RotationDriveMode.Slerp;
			j.enableCollision = false;
			return j;
		}

		private void Seat()
		{
			if (anchorL && leftHand)
			{
				anchorL.position = leftHand.position;
				anchorL.rotation = leftHand.rotation;
			}
			if (anchorR && rightHand)
			{
				anchorR.position = rightHand.position;
				anchorR.rotation = rightHand.rotation;
			}
			if (leftHand)
			{
				smoothL = leftHand.position;
				smoothLocalL = ToSpace(smoothL);
			}
			if (rightHand)
			{
				smoothR = rightHand.position;
				smoothLocalR = ToSpace(smoothR);
			}
			tieEaseUntil = Time.time + tieEase;
			hexaKnownL = hexaKnownR = false;
			placeEaseUntil = Time.time + placeEase;
			if (leftHand)
			{
				goalPosL = leftHand.position;
				goalRotL = leftHand.rotation;
			}
			if (rightHand)
			{
				goalPosR = rightHand.position;
				goalRotR = rightHand.rotation;
			}
		}

		private void SoftHold()
		{
			if (!tieL || !tieR)
			{
				return;
			}
			heldL = heldR = true;
			Mode(tieL, true, true, 0f, handPushTorque);
			Mode(tieR, true, true, 0f, handPushTorque);
			appliedPushL = -1f;
			appliedPushR = -1f;
		}

		private float GroundUnder(Vector3 at)
		{
			Vector3 from = new Vector3(at.x, Mathf.Min(at.y + 3f, neck.y), at.z);
			int count = Physics.RaycastNonAlloc(from, Vector3.down, hits, 20f, -1, QueryTriggerInteraction.Ignore);
			float best = float.MinValue;
			for (int i = 0; i < count; i++)
			{
				Rigidbody rb = hits[i].collider.attachedRigidbody;
				if (rb && !rb.isKinematic && !rb.GetComponent<NetRope>())
				{
					continue;
				}
				if (hits[i].collider.gameObject.layer == 11)
				{
					continue;
				}
				if (hits[i].collider.transform.IsChildOf(transform))
				{
					continue;
				}
				if (hits[i].point.y > best)
				{
					best = hits[i].point.y;
				}
			}
			return best;
		}

		private bool Clear(Vector3 pelvisPos)
		{
			int count = Physics.OverlapCapsuleNonAlloc(pelvisPos, pelvisPos + Vector3.up * clearHeight, clearRadius, nearBuffer, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < count; i++)
			{
				if (!nearBuffer[i].transform.IsChildOf(transform))
				{
					return false;
				}
			}
			return true;
		}

		private ConfigurableJoint Chest(Rigidbody hand, out Quaternion offset)
		{
			ConfigurableJoint j = pelvis.gameObject.AddComponent<ConfigurableJoint>();
			j.connectedBody = hand;
			j.autoConfigureConnectedAnchor = false;
			j.anchor = Vector3.zero;
			j.connectedAnchor = Vector3.zero;
			j.rotationDriveMode = RotationDriveMode.Slerp;
			j.enableCollision = true;
			offset = Quaternion.Inverse(Quaternion.Inverse(pelvis.rotation) * hand.rotation);
			return j;
		}

		private void Steer(Rigidbody anchor, Vector3 position, Quaternion rotation)
		{
			anchor.MovePosition(position);
			anchor.MoveRotation(rotation);
			if (anchor == anchorL)
			{
				goalPosL = position;
				goalRotL = rotation;
			}
			else if (anchor == anchorR)
			{
				goalPosR = position;
				goalRotR = rotation;
			}
		}

		private void Target(ConfigurableJoint j, Vector3 goal, Quaternion goalRot, Quaternion offset, ref Vector3 last, ref Quaternion lastTurn, ref bool known, bool holding)
		{
			if (!j || !pelvis)
			{
				return;
			}
			Quaternion toPelvis = Quaternion.Inverse(pelvis.rotation);
			Vector3 pos = toPelvis * (goal - pelvis.position);
			Quaternion rot = toPelvis * goalRot;
			Vector3 vel = known ? (pos - last) / Time.fixedDeltaTime : Vector3.zero;
			if (vel.sqrMagnitude > hexaVmax * hexaVmax)
			{
				vel = Vector3.zero;
			}
			Vector3 spin = Vector3.zero;
			if (known)
			{
				(rot * Quaternion.Inverse(lastTurn)).ToAngleAxis(out float angle, out Vector3 axis);
				if (angle > 180f)
				{
					angle -= 360f;
				}
				if (float.IsFinite(axis.x))
				{
					spin = axis * (angle * Mathf.Deg2Rad / Time.fixedDeltaTime);
				}
				if (spin.sqrMagnitude > hexaSpinMax * hexaSpinMax)
				{
					spin = Vector3.zero;
				}
			}
			last = pos;
			lastTurn = rot;
			known = true;
			j.targetPosition = pos;
			j.targetVelocity = vel * hexaFeed;
			j.targetRotation = rot * offset;
			j.targetAngularVelocity = holding ? spin * hexaSpinFeed : Vector3.zero;
		}

		private static JointDrive Drive(float spring, float damp, float max)
		{
			JointDrive drive = default;
			drive.positionSpring = spring;
			drive.positionDamper = damp;
			drive.maximumForce = max;
			return drive;
		}

		private void Unstick()
		{
			if (!chest || unstickAfter <= 0f)
			{
				return;
			}
			Wrapped(leftHand, leftForearm, handTargetL, ref wrappedL, ref elbowStickL, -1f);
			Wrapped(rightHand, rightForearm, handTargetR, ref wrappedR, ref elbowStickR, 1f);
		}

		private void Wrapped(Rigidbody hand, Rigidbody fore, Transform target, ref float since, ref Vector3 stick, float side)
		{
			if (hand && fore && target)
			{
				Transform chestT = chest.transform;
				Vector3 handLocal = chestT.InverseTransformPoint(hand.position);
				Vector3 foreLocal = chestT.InverseTransformPoint(fore.position);
				Vector3 targetLocal = chestT.InverseTransformPoint(target.position);
				if ((foreLocal.z < -0.08f || handLocal.z < -0.05f) && targetLocal.z > 0.05f && Vector3.Distance(hand.position, target.position) > unstickGap)
				{
					since += Time.fixedDeltaTime;
					if (since < unstickAfter)
					{
						return;
					}
					since = 0f;
					Transform frame = chest.transform;
					Vector3 from = stick.sqrMagnitude > 0.0001f ? stick.normalized : -frame.up;
					Vector3 to = (frame.right * side * 0.6f - frame.up).normalized;
					stick = Vector3.Slerp(from, to, 0.6f);
					unstuck++;
					return;
				}
			}
			since = 0f;
		}

		private void Ceiling(bool limp)
		{
			bool moving = limp && dummy && chest;
			if (cappedLimp == limp && !moving)
			{
				return;
			}
			cappedLimp = limp;
			float carried = moving ? chest.linearVelocity.magnitude : 0f;
			Rigidbody[] arms = new Rigidbody[6] { leftUpperArm, leftForearm, leftHand, rightUpperArm, rightForearm, rightHand };
			foreach (Rigidbody body in arms)
			{
				if (body)
				{
					body.maxLinearVelocity = limp ? carried + limpMaxSpeed : boneMaxSpeed;
					body.maxAngularVelocity = limp ? limpMaxSpin : boneMaxSpin;
				}
			}
		}

		public void Recap()
		{
			foreach (Rigidbody body in bodies)
			{
				if (body)
				{
					body.maxLinearVelocity = boneMaxSpeed;
				}
			}
			bool limp = cappedLimp;
			cappedLimp = !cappedLimp;
			Ceiling(limp);
		}

		private void Hands(bool on)
		{
			heldL = heldR = false;
			Mode(tieL, on, false, handPush, handPushTorque);
			Mode(tieR, on, false, handPush, handPushTorque);
		}

		private void Tendon(Rigidbody upper, Rigidbody hand, Vector3 gap, bool firm, Vector3 pulled)
		{
			if (!chest || !upper || !hand)
			{
				return;
			}
			Vector3 force = gap * (firm ? gripMuscle : muscleSpring);
			if (firm)
			{
				Vector3 along = hand.position - upper.position;
				if (along.sqrMagnitude > 1e-04f)
				{
					along.Normalize();
					Vector3 rel = hand.linearVelocity - pulled - chest.GetPointVelocity(upper.position);
					force += along * (gripMuscleDamp * Vector3.Dot(rel, along));
				}
			}
			force = Vector3.ClampMagnitude(force, muscleCap);
			chest.AddForceAtPosition(force, upper.position);
			hand.AddForce(-force);
		}

		private Vector3 Pin(bool pin, ref bool pinned, ref Vector3 rigAt, ref Vector3 off)
		{
			if (!BodyFirst || !playSpace)
			{
				pinned = false;
				off = Vector3.zero;
				return Vector3.zero;
			}
			if (pin)
			{
				if (!pinned)
				{
					pinned = true;
					rigAt = playSpace.position - off;
				}
				off = playSpace.position - rigAt;
				return off;
			}
			pinned = false;
			off = Vector3.MoveTowards(off, Vector3.zero, unpinRate * Time.fixedDeltaTime);
			return off;
		}

		private Quaternion Ease(ref Quaternion off, float at)
		{
			if (relockEase > 0f)
			{
				float t = (Time.time - at) / relockEase;
				if (t < 1f && t >= 0f)
				{
					return Quaternion.Slerp(off, Quaternion.identity, t);
				}
			}
			off = Quaternion.identity;
			return Quaternion.identity;
		}

		private void Mode(ConfigurableJoint j, bool on, bool held)
		{
			Mode(j, on, held, handPush, handPushTorque);
		}

		private Vector3 HoldAxis(HandTouch h, HandTouch f)
		{
			Collider held = null;
			if (h && h.rope && h.holds)
			{
				held = h.holds;
			}
			else if (f && f.rope && f.holds)
			{
				held = f.holds;
			}
			if (!held)
			{
				return Vector3.up;
			}
			CapsuleCollider capsule = held as CapsuleCollider;
			if (!capsule)
			{
				return held.transform.up;
			}
			Vector3 axis;
			if (capsule.direction == 0)
			{
				axis = Vector3.right;
			}
			else if (capsule.direction == 1)
			{
				axis = Vector3.up;
			}
			else
			{
				axis = Vector3.forward;
			}
			return held.transform.TransformDirection(axis).normalized;
		}

		private void AirHand(Rigidbody hand, Rigidbody anchor, Rigidbody upper, Vector3 slide, float cap)
		{
			if (!hand || !anchor || !chest)
			{
				return;
			}
			Vector3 force = airHandSpring * (anchor.position - hand.position) - airHandDamp * (hand.linearVelocity - anchor.linearVelocity);
			if (slide.sqrMagnitude > 0.5f)
			{
				Vector3 along = slide * Vector3.Dot(slide, force);
				Vector3 across = force - along;
				force = Vector3.ClampMagnitude(along, cap) + Vector3.ClampMagnitude(across, handPushGrip);
			}
			else
			{
				force = Vector3.ClampMagnitude(force, cap);
			}
			hand.AddForce(force);
			chest.AddForceAtPosition(-force, (upper ? upper : chest).position);
			(anchor.rotation * Quaternion.Inverse(hand.rotation)).ToAngleAxis(out float angle, out Vector3 axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			if (!float.IsFinite(axis.x))
			{
				return;
			}
			Vector3 torque = Vector3.ClampMagnitude(axis.normalized * (angle * Mathf.Deg2Rad * airHandSpin) - hand.angularVelocity * airHandSpinDamp, airHandTorque);
			hand.AddTorque(torque);
			chest.AddTorque(-torque);
		}

		private void HexaMode(ConfigurableJoint j, bool on, bool held, float push, float spin)
		{
			j.xMotion = ConfigurableJointMotion.Free;
			j.yMotion = ConfigurableJointMotion.Free;
			j.zMotion = ConfigurableJointMotion.Free;
			j.angularXMotion = ConfigurableJointMotion.Free;
			j.angularYMotion = ConfigurableJointMotion.Free;
			j.angularZMotion = ConfigurableJointMotion.Free;
			float force = held ? push : hexaForce;
			JointDrive drive = default;
			JointDrive side = default;
			if (on)
			{
				drive = Drive(hexaSpring, hexaDamp, force);
				side = Drive(hexaSpring, hexaDamp, Mathf.Min(force, j == hexaL ? sideCapL : sideCapR));
			}
			j.xDrive = side;
			j.yDrive = drive;
			j.zDrive = side;
			HandGrab grab = j == hexaL ? grabL : grabR;
			bool holdsYou = true;
			if (grab && grab.grabbing)
			{
				holdsYou = grab.holdsYou;
			}
			if (!on)
			{
				j.slerpDrive = default;
				return;
			}
			float torque = hexaTorque;
			if (held)
			{
				torque = holdsYou ? Mathf.Min(hexaTorque, spin) : spin;
			}
			j.slerpDrive = Drive(hexaSpin, hexaSpinDamp, torque);
		}

		private void Mode(ConfigurableJoint j, bool on, bool held, float push, float spin, bool lockTurn = false, float damp = -1f)
		{
			ConfigurableJoint twin = null;
			if (hexa)
			{
				if (j == tieL)
				{
					twin = hexaL;
				}
				else if (j == tieR)
				{
					twin = hexaR;
				}
			}
			bool live = on;
			if (twin)
			{
				bool use = on && (j == tieL ? useHexaL : useHexaR);
				HexaMode(twin, use, held, push, spin);
				live = !use && on;
			}
			ConfigurableJointMotion motion = live ? (held ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Limited) : ConfigurableJointMotion.Free;
			bool firmWrist = !wristSoft;
			j.linearLimit = new SoftJointLimit
			{
				limit = handSlack,
				contactDistance = handSlack * 0.5f
			};
			j.linearLimitSpring = new SoftJointLimitSpring
			{
				spring = 0f,
				damper = 0f
			};
			j.xMotion = motion;
			j.yMotion = motion;
			j.zMotion = motion;
			ConfigurableJointMotion turn = (live && (!held || lockTurn) && (!live || held || firmWrist)) ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
			j.angularXMotion = turn;
			j.angularYMotion = turn;
			j.angularZMotion = turn;
			JointDrive drive = live ? Drive(handSpring, handHold, held ? push : handForce) : default;
			j.xDrive = drive;
			j.yDrive = drive;
			j.zDrive = drive;
			if (live && held)
			{
				j.slerpDrive = Drive(handPushSpin, damp < 0f ? handPushSpinDamp : damp, spin);
			}
			else if (!live || held || firmWrist)
			{
				j.slerpDrive = default;
			}
			else
			{
				j.slerpDrive = Drive(wristSpin, wristSpinDamp, wristTorque);
			}
		}

		public void Reframe()
		{
			if (flopped)
			{
				FlopFrame();
			}
		}

		public void Knock(Vector3 push)
		{
			limpUntil = Time.time + limpTime;
			if (!knockedOut && !dead)
			{
				riseDue = true;
			}
			if (!flopped)
			{
				flopped = true;
				floppedAt = Time.time;
				FlopFrame();
			}
			if (push.sqrMagnitude <= 0f)
			{
				return;
			}
			foreach (Rigidbody body in bodies)
			{
				if (body)
				{
					body.AddForce(push / sturdy * body.mass, ForceMode.Impulse);
				}
			}
		}

		public void Deaden()
		{
			if (bodyDead)
			{
				return;
			}
			bodyDead = true;
			if (grabL)
			{
				grabL.releaseWhy = "drown";
			}
			if (grabR)
			{
				grabR.releaseWhy = "drown";
			}
			LetGo();
			if (!flopped)
			{
				flopped = true;
				floppedAt = Time.time;
				FlopFrame();
			}
			jumpAsk = false;
			rising = false;
			jumped = false;
			landing = false;
			holding = false;
			selfPush = false;
			gripping = false;
			hanging = false;
			heldL = false;
			heldR = false;
			autoRiseUntil = 0f;
			tone = 0f;
			riseDue = false;
			lostFloorSince = -1f;
			handGap = Vector3.zero;
			lostSince = -1f;
			Hands(false);
			tied = false;
			Loosen(tieHip);
			Loosen(tieHead);
			strength = 0f;
			armStrength = -1f;
			Loose(true);
		}

		private static void Loosen(ConfigurableJoint j)
		{
			if (!j)
			{
				return;
			}
			j.xDrive = default;
			j.yDrive = default;
			j.zDrive = default;
			j.slerpDrive = default;
		}

		public void Revive(Vector3 pelvisPos, Quaternion pelvisRot)
		{
			if (!bodyDead)
			{
				return;
			}
			bodyDead = false;
			Unslip();
			flopped = false;
			floppedAt = -1f;
			flopFrameKnown = false;
			rising = false;
			risingAt = -1f;
			riseLockedUntil = 0f;
			autoRiseUntil = 0f;
			gotUpAt = Time.time;
			tone = 1f;
			limpUntil = 0f;
			riseDue = false;
			limpArms = false;
			Ceiling(false);
			jumpUntil = -1f;
			lostFloorSince = -1f;
			lostSince = -1f;
			trailAt = 0;
			trailCount = 0;
			spinNow = -1f;
			hipYNow = -1;
			safeKnown = false;
			neckKnown = false;
			tractionNow = -1f;
			sideNow = -1f;
			wasHanging = false;
			if (tieHip)
			{
				JointDrive side = Drive(hipSide, hipSideDamp, hipSideForce);
				tieHip.xDrive = side;
				tieHip.zDrive = side;
				tieHip.yDrive = Drive(hipSpring, hipDamp, hipForce);
				tieHip.slerpDrive = Drive(hipSpin, hipSpinDamp, float.MaxValue);
			}
			if (tieHead)
			{
				JointDrive drive = Drive(headSpring, headDamp, headForce);
				tieHead.xDrive = drive;
				tieHead.zDrive = drive;
				tieHead.slerpDrive = Drive(headSpin, headSpinDamp, float.MaxValue);
				headSpinOff = false;
			}
			strength = 1f;
			armStrength = -1f;
			loose = false;
			applied = -1f;
			capNowL = -1f;
			capNowR = -1f;
			tied = false;
			tieEaseUntil = Time.time + tieEase;
			Recover(pelvisPos, pelvisRot, "revive");
			if (playSpace)
			{
				Resnap();
			}
		}

		private void FixedUpdateBody()
		{
			if (!anchorHip)
			{
				return;
			}
			if (bodyDead)
			{
				ReInterp();
				Brace(null);
				return;
			}
			bool live = !(Time.time < limpUntil) && !(playSpace && inVR && !armsLive);
			float toneGoal = limpStrength;
			float toneRate = 20f;
			if (live && !flopped)
			{
				toneGoal = 1f;
				toneRate = standUpRate;
			}
			tone = Mathf.MoveTowards(tone, toneGoal, toneRate * Time.fixedDeltaTime);
			strength = tone;
			armStrength = ((flopped || rising) && live) ? Mathf.Max(flopArm, tone) : -1f;
			uprightScale = landing ? landBrace : 1f;
			if (!view && headTarget)
			{
				view = headTarget.parent;
			}
			Wrists();
			ReadPose();
			if (playSpace && inVR && !ignoreInput)
			{
				if (headTracker != null && headTracker.enabled)
				{
					headTracker.Follow();
				}
				if (trackL != null && trackL.enabled)
				{
					trackL.Follow();
				}
				if (trackR != null && trackR.enabled)
				{
					trackR.Follow();
				}
			}
			if (!inVR && controllerL && controllerR)
			{
				if (aiHandL)
				{
					controllerL.localPosition = aiHandPosL;
				}
				if (aiHandR)
				{
					controllerR.localPosition = aiHandPosR;
				}
				TrackSwing();
			}
			Park(ref parkedL, ref trackedSinceL, ref lostSinceL, trackL, handTargetL, ref parkColsL, leftHand, leftForearm, ref lastHandLocalL, ref handVelL, true);
			Park(ref parkedR, ref trackedSinceR, ref lostSinceR, trackR, handTargetR, ref parkColsR, rightHand, rightForearm, ref lastHandLocalR, ref handVelR, false);
			leadHead = Lead(headTarget, ref lastHeadLocal, ref headVel, lead);
			leadL = Lead(handTargetL, ref lastHandLocalL, ref handVelL, lead);
			leadR = Lead(handTargetR, ref lastHandLocalR, ref handVelR, lead);
			if (useHexaL)
			{
				leadL = Vector3.zero;
			}
			if (useHexaR)
			{
				leadR = Vector3.zero;
			}
			Vector3 accelGoal = playSpace ? moveAccel * (accelLead * TotalMass() / Mathf.Max(hipSide, 1f)) : Vector3.zero;
			accelNow = Vector3.Lerp(accelNow, accelGoal, 1f - Mathf.Exp(-accelEase * Time.fixedDeltaTime));
			Vector3 headDrift = playSpace ? Flat(playSpace.TransformDirection(headVel)) : Vector3.zero;
			if ((airNow || ridingHead) && playSpace && headKnown)
			{
				leadHead = head.position - lastHead;
				leadL = leadHead;
				leadR = leadHead;
			}
			Vector3 wishLead = playSpace ? Flat(moveWish) * (hipLead - lead) : Vector3.zero;
			leadRead = true;
			leadHips = Flat(leadHead) + wishLead + headDrift * (trackLead - lead) + accelNow;
			bool lostL = trackL != null && !trackL.present && !trackL.tracking;
			bool lostR = trackR != null && !trackR.present && !trackR.tracking;
			armsLive = (playSpace && !inVR) || ((!lostL || parkedL) && (!lostR || parkedR));
			bool driving = false;
			if (armsLive && live && (!flopped || rising) && handTargetL)
			{
				centred = handTargetR;
				driving = centred && !limpArms;
			}
			else
			{
				centred = false;
			}
			bool sweep = sweepLimbs && (!driving || fallTop > flopSpeed);
			if (swept != sweep)
			{
				swept = sweep;
				for (int i = 0; i < bodies.Length; i++)
				{
					Rigidbody body = bodies[i];
					if (body)
					{
						body.collisionDetectionMode = sweep ? CollisionDetectionMode.ContinuousSpeculative : CollisionDetectionMode.Discrete;
					}
				}
			}
			Unstick();
			if (!limpKnown)
			{
				limpKnown = true;
				armScaleWas = armScale;
				armDampWas = armDamp;
				armScaleNow = armScale;
				armDampNow = armDamp;
			}
			float limpStep = Time.fixedDeltaTime / Mathf.Max(limpFade, 0.01f);
			float scaleGoal;
			float kGoal;
			if (limpArms)
			{
				scaleGoal = (rising && !flopped) ? Mathf.Max(limpArmScale, riseArmScale) : limpArmScale;
				kGoal = 1f;
			}
			else
			{
				scaleGoal = armScaleWas;
				kGoal = 0f;
			}
			limpK = Mathf.MoveTowards(limpK, kGoal, limpStep);
			relaxK = limpK;
			float dampGoal = limpArms ? limpArmDamp : armDampWas;
			armScaleNow = Mathf.MoveTowards(armScaleNow, scaleGoal, Mathf.Max(armScaleWas, 1f) * limpStep);
			armDampNow = Mathf.MoveTowards(armDampNow, dampGoal, Mathf.Max(armDampWas, limpArmDamp) * limpStep);
			armScale = armScaleNow;
			armDamp = armDampNow;
			Ceiling(limpArms);
			if (tied != driving)
			{
				if (driving)
				{
					Seat();
					Hands(true);
					tied = driving;
					SoftHold();
					if (rearmJumps)
					{
						if (!grabL || !grabL.grabbing)
						{
							Rearm(leftUpperArm, leftForearm, leftHand, handTargetL, wristL, leadL, capL, anchorL);
						}
						if (!grabR || !grabR.grabbing)
						{
							Rearm(rightUpperArm, rightForearm, rightHand, handTargetR, wristR, leadR, capR, anchorR);
						}
					}
					elbowStickL = Vector3.zero;
					elbowStickR = Vector3.zero;
					rollRefL = Vector3.zero;
					rollRefR = Vector3.zero;
					rollAngL = 0f;
					rollAngR = 0f;
				}
				else
				{
					Hands(false);
					tied = driving;
				}
				if (leftUpperArm && leftForearm)
				{
					prevUrL = leftUpperArm.rotation;
					prevFrL = leftForearm.rotation;
					frameKnownL = true;
				}
				if (rightUpperArm && rightForearm)
				{
					prevUrR = rightUpperArm.rotation;
					prevFrR = rightForearm.rotation;
					frameKnownR = true;
				}
				softUntil = Time.time + softFor;
			}
			if (driving)
			{
				ArmJump();
			}
			else
			{
				armJumpKnown = false;
			}
			Hips();
			Head();
			ReInterp();
			if (!driving)
			{
				holding = false;
				handGap = Vector3.zero;
				Brace(null);
				return;
			}
			Vector3 pinL = Pin(false, ref pinnedL, ref pinRigL, ref pinOffL);
			Vector3 pinR = Pin(false, ref pinnedR, ref pinRigR, ref pinOffR);
			bool holdL = grabL && grabL.grabbing && grabL.holdsYou;
			bool holdR = grabR && grabR.grabbing && grabR.holdsYou;
			bool wasLifting = lifting;
			if (jumpHeld && (holdL || holdR))
			{
				lifting = !flopped;
				if (lifting)
				{
					liftUsed = true;
					if (!wasLifting)
					{
						liftFrom = Time.time;
						liftFromY = pelvis.position.y;
					}
				}
			}
			else
			{
				lifting = false;
			}
			if (wasLifting && !lifting && !dummy && Time.time > liftFrom + 0.3f)
			{
				Note("hold-jump lift " + (Time.time - liftFrom).ToString("F1") + " s, pelvis rose " + ((pelvis.position.y - liftFromY) * 100f).ToString("F0") + " cm, carry " + (liftNow * liftCarry).ToString("F2") + " g" + ((holdL || holdR) ? "" : ", hands off"));
			}
			liftL = Mathf.MoveTowards(holdL ? liftL : 0f, (holdL && lifting) ? liftMax : 0f, (lifting ? liftSpeed : liftDrop) * Time.fixedDeltaTime);
			liftR = Mathf.MoveTowards(holdR ? liftR : 0f, (holdR && lifting) ? liftMax : 0f, (lifting ? liftSpeed : liftDrop) * Time.fixedDeltaTime);
			liftNow = Mathf.MoveTowards(liftNow, lifting ? 1f : 0f, Time.fixedDeltaTime * ((lifting ? liftSpeed : liftDrop) / Mathf.Max(liftMax, 0.01f)));
			Vector3 aimL = handTargetL.position + leadL - pinL + Vector3.down * liftL;
			Vector3 aimR = handTargetR.position + leadR - pinR + Vector3.down * liftR;
			if (flopped)
			{
				aimL = Fallen(aimL);
				aimR = Fallen(aimR);
			}
			aimL = Unbend(leftUpperArm, leftForearm, leftHand, Reachable(leftUpperArm, leftForearm, leftHand, aimL));
			aimR = Unbend(rightUpperArm, rightForearm, rightHand, Reachable(rightUpperArm, rightForearm, rightHand, aimR));
			Vector3 placeL = aimL;
			if (!(BodyFirst && heldL) && !sweepOff)
			{
				placeL = Stop(leftHand, capL, aimL);
			}
			Vector3 placeR = aimR;
			if (!(BodyFirst && heldR) && !sweepOff)
			{
				placeR = Stop(rightHand, capR, aimR);
			}
			goalL = placeL;
			goalR = placeR;
			reachCutL = handTargetL ? Vector3.Distance(handTargetL.position, aimL) : 0f;
			reachCutR = handTargetR ? Vector3.Distance(handTargetR.position, aimR) : 0f;
			cutRNow = Vector3.Distance(placeR, aimR);
			if (handEase > 0f || handMax > 0f)
			{
				float room = handMax > 0f ? handMax * Time.fixedDeltaTime : float.MaxValue;
				rawGoalL = placeL;
				smoothLocalL = Step(smoothLocalL, ToSpace(placeL), EaseK(leftHand), CatchRoom(leftHand, room));
				smoothLocalR = Step(smoothLocalR, ToSpace(placeR), EaseK(rightHand), CatchRoom(rightHand, room));
				smoothL = FromSpace(smoothLocalL);
				smoothR = FromSpace(smoothLocalR);
				placeL = smoothL;
				placeR = smoothR;
				Ease(ref easedL, ref easedRotL, handTargetL, smoothL);
				Ease(ref easedR, ref easedRotR, handTargetR, smoothR);
			}
			if (!heldL && !(grabL && grabL.grabbing))
			{
				if ((placeL - leftHand.transform.position).sqrMagnitude >= handSnap * handSnap)
				{
					placedL++;
				}
				Place(leftUpperArm, leftForearm, leftHand, easedL ? easedL : handTargetL, wristL, placeL);
			}
			if (!heldR && !(grabR && grabR.grabbing))
			{
				if ((placeR - rightHand.transform.position).sqrMagnitude >= handSnap * handSnap)
				{
					placedR++;
				}
				Place(rightUpperArm, rightForearm, rightHand, easedR ? easedR : handTargetR, wristR, placeR);
			}
			bool secondL = grabL && grabL.second && grabL.held && !secondFollows;
			bool secondR = grabR && grabR.second && grabR.held && !secondFollows;
			if (wasSecondL && !secondL && grabL && grabL.grabbing)
			{
				handoverL = Time.time;
			}
			if (wasSecondR && !secondR && grabR && grabR.grabbing)
			{
				handoverR = Time.time;
			}
			wasSecondL = secondL;
			wasSecondR = secondR;
			Quaternion rotL = secondL ? grabL.held.rotation * grabL.gripRot : Wrist(handTargetL) * Ease(ref relockOffL, relockAtL);
			Quaternion rotR = secondR ? grabR.held.rotation * grabR.gripRot : Wrist(handTargetR) * Ease(ref relockOffR, relockAtR);
			if (flopped)
			{
				rotL = Fallen(rotL);
				rotR = Fallen(rotR);
			}
			float turnL = Time.time < handoverL + handoverTurnTime ? handoverTurn : handTurn;
			float turnR = Time.time < handoverR + handoverTurnTime ? handoverTurn : handTurn;
			Steer(anchorL, AnchorStep(anchorL.position, placeL), Quaternion.RotateTowards(anchorL.rotation, rotL, turnL * Time.fixedDeltaTime));
			Steer(anchorR, AnchorStep(anchorR.position, placeR), Quaternion.RotateTowards(anchorR.rotation, rotR, turnR * Time.fixedDeltaTime));
			tieK = tieEase > 0f ? Mathf.Clamp01(1f - (tieEaseUntil - Time.time) / tieEase) : 1f;
			Held(aimL, aimR);
			if (hexa)
			{
				Target(hexaL, goalPosL, goalRotL, hexaOffL, ref hexaLastL, ref hexaTurnL, ref hexaKnownL, grabL && grabL.grabbing && !grabL.holdsYou);
				Target(hexaR, goalPosR, goalRotR, hexaOffR, ref hexaLastR, ref hexaTurnR, ref hexaKnownR, grabR && grabR.grabbing && !grabR.holdsYou);
			}
			Arm(leftUpperArm, leftForearm, leftHand, easedL ? easedL : handTargetL, heldL && (!(airHandL || ropeFreeL) || !armAimAir), (grabL && grabL.grabbing) || touchL.holdsYou || touchFL.holdsYou);
			Arm(rightUpperArm, rightForearm, rightHand, easedR ? easedR : handTargetR, heldR && (!(airHandR || ropeFreeR) || !armAimAir), (grabR && grabR.grabbing) || touchR.holdsYou || touchFR.holdsYou);
		}

		private void FellThrough(Collider floorCol, Vector3 position)
		{
			if (dummy)
			{
				return;
			}
			if (grounded && floorCol && !feetFloor)
			{
				standCol = floorCol;
				standTop = floorY;
				standAt = Time.time;
				fellNoted = false;
				return;
			}
			if (!standCol || fellNoted || Time.time > standAt + 1f || position.y > standTop + -0.6f)
			{
				return;
			}
			if (standCol.bounds.max.y < position.y + 0.5f)
			{
				return;
			}
			fellNoted = true;
			int ignored = 0;
			foreach (Collider own in SelfCols())
			{
				if (own && Physics.GetIgnoreCollision(own, standCol))
				{
					ignored++;
				}
			}
			Rigidbody body = standCol.attachedRigidbody;
			NetProp prop = standCol.GetComponentInParent<NetProp>();
			NetRope rope = standCol.GetComponentInParent<NetRope>();
			Fusion.NetworkObject obj = prop ? prop.Object : (rope ? rope.Object : null);
			string owner;
			if (!obj || !obj.IsValid)
			{
				owner = "";
			}
			else if (obj.HasStateAuthority)
			{
				owner = ", mine";
			}
			else
			{
				owner = ", owned by player " + obj.StateAuthority.PlayerId.ToString();
			}
			Note("fell through " + standCol.name + ": pelvis " + (standTop - position.y).ToString("F2") + " m under the top I stood on, "
				+ (body ? (body.isKinematic ? "kinematic" : "dynamic") + " at " + body.linearVelocity.magnitude.ToString("F1") + " m/s" : "static")
				+ (standCol.enabled ? "" : ", collider off") + ", " + ignored.ToString() + " of my colliders ignore it" + owner + " | " + State());
		}

		private bool Limb(Rigidbody rb)
		{
			return rb == leftHand || rb == rightHand || rb == leftForearm || rb == rightForearm || rb == leftUpperArm || rb == rightUpperArm;
		}

		private bool Soaked(float surface)
		{
			foreach (Collider c in SelfCols())
			{
				if (!c || c.isTrigger || !c.enabled)
				{
					continue;
				}
				Rigidbody body = c.attachedRigidbody;
				if (body == pelvis && c.bounds.min.y < surface + -0.05f)
				{
					return true;
				}
				if (body == chest && c.bounds.min.y < surface)
				{
					return true;
				}
			}
			return false;
		}

		private bool Dipped(Water acid)
		{
			foreach (Collider c in SelfCols())
			{
				if (!c || c.isTrigger || !c.enabled || Limb(c.attachedRigidbody))
				{
					continue;
				}
				Bounds bounds = c.bounds;
				if (acid.Inside(bounds.center) && bounds.min.y < acid.SurfaceAt(bounds.center))
				{
					return true;
				}
			}
			return false;
		}

		private Collider[] SelfCols()
		{
			if (selfCols == null)
			{
				List<Collider> list = new List<Collider>();
				foreach (Rigidbody body in bodies)
				{
					if (body)
					{
						list.AddRange(body.GetComponents<Collider>());
					}
				}
				selfCols = list.ToArray();
			}
			return selfCols;
		}

		private void Slip()
		{
			if (dummy || bodyDead || holding || gripping || hanging || !pelvis || !chest)
			{
				return;
			}
			if (pelvis.linearVelocity.y < -1f)
			{
				return;
			}
			foreach (RaycastHit hit in Physics.RaycastAll(pelvis.position, Vector3.down, slipDepth, -1, QueryTriggerInteraction.Ignore))
			{
				if (!hit.collider.transform.IsChildOf(transform))
				{
					return;
				}
			}
			List<Collider> walls = new List<Collider>();
			bool wedged = false;
			foreach (Rigidbody body in new Rigidbody[2] { pelvis, chest })
			{
				Collider[] ring = new Collider[8];
				for (int i = 0; i < 8; i++)
				{
					Vector3 dir = Quaternion.Euler(0f, i * 45f, 0f) * Vector3.forward;
					if (Physics.Raycast(body.position, dir, out RaycastHit hit, slipReach, -1, QueryTriggerInteraction.Ignore) && Wall(hit.collider))
					{
						ring[i] = hit.collider;
					}
				}
				for (int i = 0; i < 4; i++)
				{
					if (ring[i] && ring[i + 4])
					{
						if (!walls.Contains(ring[i]))
						{
							walls.Add(ring[i]);
						}
						if (!walls.Contains(ring[i + 4]))
						{
							walls.Add(ring[i + 4]);
						}
						wedged = true;
					}
				}
			}
			if (!wedged)
			{
				return;
			}
			bool added = false;
			foreach (Collider wall in walls)
			{
				if (slipWalls.Contains(wall))
				{
					continue;
				}
				slipWalls.Add(wall);
				foreach (Collider own in SelfCols())
				{
					if (own)
					{
						Physics.IgnoreCollision(own, wall, true);
					}
				}
				added = true;
			}
			if (added)
			{
				Note("wedged over nothing between " + string.Join(" and ", walls.ConvertAll((Collider w) => w.name)) + " at " + pelvis.position.ToString("F1") + ", slipping through");
			}
		}

		private static bool Wall(Collider c)
		{
			Rigidbody rb = c.attachedRigidbody;
			if (!rb)
			{
				return true;
			}
			if (!rb.isKinematic || rb.GetComponent<Hazard>() || rb.GetComponentInParent<NetProp>() || rb.GetComponentInParent<AvatarBody>())
			{
				return false;
			}
			return !rb.GetComponentInParent<Player>();
		}

		public static bool Buried(Vector3 at, List<Collider> skip, out float top)
		{
			top = at.y;
			bool backfaces = Physics.queriesHitBackfaces;
			Physics.queriesHitBackfaces = true;
			int count = Physics.RaycastNonAlloc(at, Vector3.up, buriedHits, 1.5f, -1, QueryTriggerInteraction.Ignore);
			Physics.queriesHitBackfaces = backfaces;
			if (count > 0)
			{
				float best = float.MaxValue;
				bool inside = false;
				for (int i = 0; i < count; i++)
				{
					RaycastHit hit = buriedHits[i];
					if (hit.collider.attachedRigidbody || (skip != null && skip.Contains(hit.collider)) || hit.distance >= best)
					{
						continue;
					}
					best = hit.distance;
					inside = Vector3.Dot(hit.normal, Vector3.up) > 0f;
					top = hit.point.y;
				}
				if (inside)
				{
					return true;
				}
			}
			int found = Physics.OverlapSphereNonAlloc(at, 0.01f, buriedCols, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < found; i++)
			{
				Collider c = buriedCols[i];
				if (c.attachedRigidbody || c is MeshCollider || (skip != null && skip.Contains(c)))
				{
					continue;
				}
				if (c.Raycast(new Ray(at + Vector3.up * 1.5f, Vector3.down.normalized), out RaycastHit hit, 1.5f))
				{
					top = hit.point.y;
					return true;
				}
			}
			return false;
		}

		private void Unbury()
		{
			if (dummy || bodyDead || !pelvis)
			{
				return;
			}
			if (!Buried(pelvis.position, slipWalls, out float top))
			{
				return;
			}
			float depth = top - pelvis.position.y;
			Shift(Vector3.up * (depth + 0.25f));
			Note("pelvis was " + (depth * 100f).ToString("F0") + " cm inside the ground while ragdolled, lifted out");
		}

		private void Cornered()
		{
			if (dummy || bodyDead || !pelvis)
			{
				return;
			}
			int count = Physics.RaycastNonAlloc(pelvis.position, Vector3.down, hits, standHeight * 0.5f, -1, QueryTriggerInteraction.Ignore);
			Rigidbody under = null;
			float best = float.MaxValue;
			for (int i = 0; i < count; i++)
			{
				if (hits[i].collider.transform.IsChildOf(transform) || hits[i].distance >= best)
				{
					continue;
				}
				best = hits[i].distance;
				under = hits[i].rigidbody;
			}
			Hazard hazard = (under && under.isKinematic) ? Hazard.Of(under) : null;
			if (!hazard || !hazard.Turning(under))
			{
				return;
			}
			Vector3 ride = hazard.Ride(pelvis.position);
			foreach (Rigidbody body in bodies)
			{
				if (body && !body.isKinematic)
				{
					body.linearVelocity = new Vector3(ride.x, body.linearVelocity.y, ride.z);
				}
			}
		}

		private void Unslip()
		{
			foreach (Collider wall in slipWalls)
			{
				if (!wall)
				{
					continue;
				}
				foreach (Collider own in SelfCols())
				{
					if (own)
					{
						Physics.IgnoreCollision(own, wall, false);
					}
				}
			}
			slipWalls.Clear();
		}

		private bool SelfInside(Vector3 point, Rigidbody hand, Rigidbody fore, Rigidbody upper)
		{
			foreach (Collider c in SelfCols())
			{
				if (!c || !c.enabled || c.isTrigger)
				{
					continue;
				}
				if (c is MeshCollider mesh && !mesh.convex)
				{
					continue;
				}
				Rigidbody rb = c.attachedRigidbody;
				if (rb == hand || rb == fore || rb == upper)
				{
					continue;
				}
				if ((c.ClosestPoint(point) - point).sqrMagnitude < 1e-06f)
				{
					return true;
				}
			}
			return false;
		}

		private void PassSelf(Rigidbody[] parts, Collider own)
		{
			if (!own)
			{
				return;
			}
			foreach (Rigidbody part in parts)
			{
				if (!part || own.attachedRigidbody == part)
				{
					continue;
				}
				foreach (Collider c in part.GetComponents<Collider>())
				{
					if (!c || Physics.GetIgnoreCollision(c, own))
					{
						continue;
					}
					Physics.IgnoreCollision(c, own, true);
					passA.Add(c);
					passB.Add(own);
				}
			}
		}

		private void ClearSelf()
		{
			for (int i = passA.Count - 1; i >= 0; i--)
			{
				Collider a = passA[i];
				Collider b = passB[i];
				if (a && b)
				{
					if (Physics.ComputePenetration(a, a.transform.position, a.transform.rotation, b, b.transform.position, b.transform.rotation, out _, out _))
					{
						continue;
					}
					Vector3 onB = b.ClosestPoint(a.bounds.center);
					Vector3 onA = a.ClosestPoint(b.bounds.center);
					if ((onB - onA).sqrMagnitude < passClear * passClear)
					{
						continue;
					}
					Physics.IgnoreCollision(a, b, false);
				}
				passA.RemoveAt(i);
				passB.RemoveAt(i);
			}
		}

		private Collider Blocker(Vector3 from, Vector3 to)
		{
			return Blocker(from, to, out _);
		}

		private Collider Blocker(Vector3 from, Vector3 to, out Vector3 normal)
		{
			normal = Vector3.zero;
			Vector3 d = to - from;
			float dist = d.magnitude;
			if (dist < 0.01f)
			{
				return null;
			}
			int count = Physics.RaycastNonAlloc(from, d / dist, armHits, dist, -1, QueryTriggerInteraction.Ignore);
			Collider best = null;
			float bestDist = float.MaxValue;
			for (int i = 0; i < count; i++)
			{
				Collider c = armHits[i].collider;
				if (armHits[i].distance >= bestDist)
				{
					continue;
				}
				if (c.transform.IsChildOf(transform) || c.GetComponentInParent<Rope>() || c.GetComponentInParent<AvatarBody>() || c.GetComponentInParent<NetworkPlayer>())
				{
					continue;
				}
				if (Carried(leftHand, c) || Carried(rightHand, c))
				{
					continue;
				}
				bestDist = armHits[i].distance;
				normal = armHits[i].normal;
				best = c;
			}
			return best;
		}

		private Collider Crossed(Rigidbody[] arm)
		{
			if (arm == null || !armPass || !arm[0] || !arm[1] || !arm[2])
			{
				return null;
			}
			Vector3 a = arm[0].position;
			Vector3 b = arm[1].position;
			Vector3 c = arm[2].position;
			Vector3 normal;
			Collider wall = Blocker(a, b, out normal);
			if (!wall)
			{
				wall = Blocker(b, c, out normal);
			}
			if (!wall)
			{
				return null;
			}
			if (!Face(wall, c, b, out Vector3 face) && !Face(wall, b, a, out face))
			{
				return null;
			}
			return Vector3.Dot(normal, face) < -0.5f ? wall : null;
		}

		private static bool Face(Collider c, Vector3 from, Vector3 to, out Vector3 normal)
		{
			normal = Vector3.zero;
			Vector3 d = to - from;
			float dist = d.magnitude;
			if (dist < 0.01f)
			{
				return false;
			}
			if (!c.Raycast(new Ray(from, d / dist), out RaycastHit hit, dist))
			{
				return false;
			}
			normal = hit.normal;
			return true;
		}

		private bool Wants(Rigidbody[] arm, Collider wall, Vector3 goal)
		{
			if (pelvis.position.y - floorY < standHeight * 0.85f)
			{
				return false;
			}
			Vector3 d = goal - arm[0].position;
			float dist = d.magnitude;
			if (!(dist > 0.001f))
			{
				return false;
			}
			return wall.Raycast(new Ray(arm[0].position, d / dist), out _, dist);
		}

		private void PassWall(Rigidbody[] arm, Collider wall, Vector3 goal)
		{
			if (!wall || Wants(arm, wall, goal))
			{
				return;
			}
			foreach (Rigidbody part in arm)
			{
				if (!part)
				{
					continue;
				}
				foreach (Collider c in part.GetComponentsInChildren<Collider>())
				{
					if (!c || Physics.GetIgnoreCollision(c, wall))
					{
						continue;
					}
					Physics.IgnoreCollision(c, wall, true);
					wallA.Add(c);
					wallB.Add(wall);
				}
			}
		}

		private void ClearWalls(Vector3 goalL, Vector3 goalR)
		{
			for (int i = wallA.Count - 1; i >= 0; i--)
			{
				Collider own = wallA[i];
				Collider wall = wallB[i];
				if (own && wall && wall.enabled)
				{
					bool left = own.transform.IsChildOf(leftUpperArm.transform);
					Rigidbody[] arm = left ? armL : armR;
					if (!Wants(arm, wall, left ? goalL : goalR))
					{
						if (Physics.ComputePenetration(own, own.transform.position, own.transform.rotation, wall, wall.transform.position, wall.transform.rotation, out _, out _))
						{
							continue;
						}
						if (Blocker(arm[0].position, arm[1].position, out _) == wall || Blocker(arm[1].position, arm[2].position, out _) == wall)
						{
							continue;
						}
					}
					Physics.IgnoreCollision(own, wall, false);
				}
				wallA.RemoveAt(i);
				wallB.RemoveAt(i);
			}
		}

		private void Held(Vector3 goalL, Vector3 goalR)
		{
			Vector3 posL = leftHand.transform.position;
			Vector3 posR = rightHand.transform.position;
			Vector3 gapL = posL - goalL;
			Vector3 gapR = posR - goalR;
			bool holdsYouL = grabL && grabL.grabbing && grabL.holdsYou;
			bool holdsYouR = grabR && grabR.grabbing && grabR.holdsYou;
			bool firmL = Firm(grabL);
			bool firmR = Firm(grabR);
			if (lifting)
			{
				for (int i = 0; i < bodies.Length; i++)
				{
					if (bodies[i])
					{
						bodies[i].AddForce(Vector3.up * (liftCarry * 9.81f * liftNow), ForceMode.Acceleration);
					}
				}
			}
			bool touchHoldsL = grabL && grabL.grabbing && !grabL.holdsYou && grabL.heldTouch && grabL.heldTouch.holdsYou;
			bool touchHoldsR = grabR && grabR.grabbing && !grabR.holdsYou && grabR.heldTouch && grabR.heldTouch.holdsYou;
			bool heldTouchingL = grabL && grabL.grabbing && grabL.heldTouch && grabL.heldTouch.touching;
			bool heldTouchingR = grabR && grabR.grabbing && grabR.heldTouch && grabR.heldTouch.touching;
			bool restingL = grabL && grabL.grabbing && !grabL.holdsYou && grabL.heldTouch && grabL.heldTouch.resting;
			bool restingR = grabR && grabR.grabbing && !grabR.holdsYou && grabR.heldTouch && grabR.heldTouch.resting;
			propped = restingL || restingR;
			pullL = propPullOn && restingL && gapL.y > (pullL ? propPullKeep : propPull);
			pullR = propPullOn && restingR && gapR.y > (pullR ? propPullKeep : propPull);
			bool clingL = touchHoldsL || holdsYouL;
			if (!clingL && restingL)
			{
				clingL = crouch > dangleCrouch || dangling || pullL;
			}
			bool clingR = touchHoldsR || holdsYouR;
			if (!clingR && restingR)
			{
				clingR = crouch > dangleCrouch || dangling || pullR;
			}
			gripping = clingL || clingR;
			pullingUp = (clingL && gapL.y * (firmL ? gripMuscle : muscleSpring) > muscleSpring * 0.12f) || (clingR && gapR.y * (firmR ? gripMuscle : muscleSpring) > muscleSpring * 0.12f);
			ledgeL = LedgeNear(leftHand, grabL);
			Rigidbody topL = ledgeL ? lastTop : null;
			ledgeR = LedgeNear(rightHand, grabR);
			Rigidbody topR = ledgeR ? lastTop : null;
			bool lowL = ledgeL && grabL && grabL.ledge && leftHand.position.y < head.position.y;
			bool lowR = ledgeR && grabR && grabR.ledge && rightHand.position.y < head.position.y;
			offLedgeGrip = (clingL && !lowL) || (clingR && !lowR);
			Brace(holding ? (topL ? topL : topR) : null);
			crossL = (grabL && grabL.grabbing) ? null : Crossed(armL);
			crossR = (grabR && grabR.grabbing) ? null : Crossed(armR);
			bool touchingL = !crossL && (touchL.touching || touchFL.touching || touchUL.touching);
			bool contactL = touchingL || ledgeL || clingL || heldTouchingL;
			bool touchingR = !crossR && (touchR.touching || touchFR.touching || touchUR.touching);
			bool contactR = touchingR || ledgeR || heldTouchingR || clingR;
			if (wasContactL && !contactL)
			{
				contactEndL = Time.time;
			}
			if (wasContactR && !contactR)
			{
				contactEndR = Time.time;
			}
			wasContactL = contactL;
			wasContactR = contactR;
			bool selfL = touchL.self;
			bool selfR = touchR.self;
			if (selfL && !SelfInside(goalL, leftHand, leftForearm, leftUpperArm))
			{
				PassSelf(passL, touchL.selfCol);
			}
			if (selfR && !SelfInside(goalR, rightHand, rightForearm, rightUpperArm))
			{
				PassSelf(passR, touchR.selfCol);
			}
			if (forearmPass && touchFL.self && !SelfInside(goalL, leftHand, leftForearm, leftUpperArm))
			{
				PassSelf(passL, touchFL.selfCol);
			}
			if (forearmPass && touchFR.self && !SelfInside(goalR, rightHand, rightForearm, rightUpperArm))
			{
				PassSelf(passR, touchFR.selfCol);
			}
			ClearSelf();
			PassWall(armL, crossL, goalL);
			PassWall(armR, crossR, goalR);
			ClearWalls(goalL, goalR);

			// Airborne: neither on the ground, holding on, nor gripping.
			if (grounded || holding)
			{
				airborne = false;
			}
			else
			{
				airborne = !gripping;
			}
			if (airborne && !wasAirborne)
			{
				airSince = Time.time;
			}
			wasAirborne = airborne;
			if (kicking)
			{
				legScale = legKick;
			}
			else if (airborne || flopped || rising)
			{
				legScale = legAir;
			}
			else
			{
				legScale = hanging ? legHang : 1f;
			}
			float armWant;
			if (flopped)
			{
				armWant = flopArmScale;
			}
			else
			{
				armWant = (gripping && !BodyFirst) ? armGrip : 1f;
			}
			armScale = Mathf.MoveTowards(armScale, armWant, armRamp * Time.fixedDeltaTime);
			bool floorPressL = FloorPress(leftHand, touchL, touchFL, anchorL, clingL, grabL);
			bool floorPressR = FloorPress(rightHand, touchR, touchFR, anchorR, clingR, grabR);
			if (BodyFirst)
			{
				bool worldL = World(touchL) || World(touchFL) || World(touchUL);
				bool worldR = World(touchR) || World(touchFR) || World(touchUR);
				bool ledgeHeldL = ledgeL && grabL && grabL.grabbing;
				if ((ledgeHeldL || clingL || (worldL && heldL)) && !floorPressL)
				{
					Tendon(leftUpperArm, leftHand, gapL, firmL, playSpace.TransformDirection(handVelL));
				}
				bool ledgeHeldR = ledgeR && grabR && grabR.grabbing;
				if ((ledgeHeldR || clingR || (worldR && heldR)) && !floorPressR)
				{
					Tendon(rightUpperArm, rightHand, gapR, firmR, playSpace.TransformDirection(handVelR));
				}
			}

			// Hexa (six-axis) hand joints when the hand is on something it can push off.
			bool grabbingL = grabL && grabL.grabbing;
			bool grabbingR = grabR && grabR.grabbing;
			bool kinematicL = grabbingL && !grabL.holdsYou && grabL.held && grabL.held.isKinematic;
			bool kinematicR = grabbingR && !grabR.holdsYou && grabR.held && grabR.held.isKinematic;
			bool wantHexaL = hexa && !kinematicL && (contactL || (grabbingL && !grabL.holdsYou && grabL.heldTouch && (grabL.heldTouch.touching || grabL.heldTouch.self)));
			bool wantHexaR = hexa && !kinematicR && (contactR || (grabbingR && !grabR.holdsYou && grabR.heldTouch && (grabR.heldTouch.touching || grabR.heldTouch.self)));
			if (useHexaL != wantHexaL)
			{
				useHexaL = wantHexaL;
				hexaKnownL = false;
				if (tied && !heldL)
				{
					Mode(tieL, true, false, handPush, handPushTorque);
				}
				appliedPushL = -1f;
			}
			if (useHexaR != wantHexaR)
			{
				useHexaR = wantHexaR;
				hexaKnownR = false;
				if (tied && !heldR)
				{
					Mode(tieR, true, false, handPush, handPushTorque);
				}
				appliedPushR = -1f;
			}

			// Free-air hands.
			bool airBody = BodyFirst && airborne;
			bool downBody = BodyFirst && (flopped || rising) && !airborne;
			bool ropeL = touchL.rope || touchFL.rope;
			bool ropeR = touchR.rope || touchFR.rope;
			bool riding = BodyFirst && !airborne && playSpace && ridingHead && !landing;
			bool riseHold = downBody && rising;
			bool freeArms = riseHold || airBody;
			bool airL = (freeArms ? !grabbingL : ((slipping || riding) && !contactL && !ropeL && !grabbingL)) && airHands && !holdOff;
			bool airR = (freeArms ? !grabbingR : ((slipping || riding) && !contactR && !ropeR && !grabbingR)) && airHands && !holdOff;
			airHandR = airR;
			airHandL = airL;
			ropeFreeL = ropeL && riding && !grabbingL && !contactL && airHands && !holdOff;
			ropeFreeR = ropeR && riding && !grabbingR && !contactR && airHands && !holdOff;
			bool onL = selfL || grabbingL || airBody || downBody || ropeL || airL || contactL;
			bool onR = selfR || grabbingR || airBody || downBody || ropeR || airR || contactR;
			if (wasOnL && !onL)
			{
				touchEndL = Time.time;
			}
			if (wasOnR && !onR)
			{
				touchEndR = Time.time;
			}
			wasOnL = onL;
			wasOnR = onR;
			turnL = anchorL ? Quaternion.Angle(leftHand.rotation, anchorL.rotation) : 0f;
			turnR = anchorR ? Quaternion.Angle(rightHand.rotation, anchorR.rotation) : 0f;
			float turnHoldL = (Time.time < touchEndL + relockWait) ? turnL : 0f;
			float turnHoldR = (Time.time < touchEndR + relockWait) ? turnR : 0f;
			if (airBody)
			{
				// The shipped build reads Time.time here and discards it.
				_ = Time.time;
			}
			bool selfLockL = !riding && !airBody && selfL && !contactL && !grabbingL && !airL && !ropeFreeL && !ropeL;
			bool selfLockR = !riding && !airBody && selfR && !contactR && !grabbingR && !airR && !ropeFreeR && !ropeR;
			bool lockL = selfLockL || (!onL && !riding && !airBody && !grabbingL);
			bool lockR = selfLockR || (!onR && !riding && !airBody && !grabbingR);
			if (holdOff)
			{
				if (heldL)
				{
					heldL = false;
					if (anchorL)
					{
						anchorL.position = leftHand.position;
					}
					Mode(tieL, true, false, handPush, handPushTorque);
				}
				if (heldR)
				{
					heldR = false;
					if (anchorR)
					{
						anchorR.position = rightHand.position;
					}
					Mode(tieR, true, false, handPush, handPushTorque);
				}
				onL = false;
				onR = false;
			}

			// Tie each hand to whatever it is on.
			bool wasHeldL = heldL;
			bool wasHeldR = heldR;
			float overL = -1f;
			if (!airBody && !grabbingL && !downBody && !airL && !ropeFreeL)
			{
				overL = (BodyFirst && contactL) ? -1f : (selfL ? -0.005f : 0.02f);
			}
			bool holdL = Hold(tieL, onL, gapL, ref heldL, overL, turnHoldL, lockL, Time.time - contactEndL, false, anchorL ? Vector3.Distance(leftHand.position, anchorL.position) : -1f);
			float overR = -1f;
			if (!airBody && !grabbingR && !downBody && !airR && !ropeFreeR)
			{
				overR = (BodyFirst && contactR) ? -1f : (selfR ? -0.005f : 0.02f);
			}
			bool holdR = Hold(tieR, onR, gapR, ref heldR, overR, turnHoldR, lockR, Time.time - contactEndR, false, anchorR ? Vector3.Distance(rightHand.position, anchorR.position) : -1f);
			if (!wasHeldL && heldL)
			{
				appliedLockL = lockL;
			}
			if (!wasHeldR && heldR)
			{
				appliedLockR = lockR;
			}
			if (wasHeldL && !heldL && turnL > relockSnap)
			{
				anchorL.rotation = leftHand.rotation;
				relockOffL = Quaternion.Inverse(Wrist(handTargetL)) * leftHand.rotation;
				relockAtL = Time.time;
			}
			if (wasHeldR && !heldR && turnR > relockSnap)
			{
				anchorR.rotation = rightHand.rotation;
				relockOffR = Quaternion.Inverse(Wrist(handTargetR)) * rightHand.rotation;
				relockAtR = Time.time;
			}

			// Push strength for each tie.
			bool heavyL = grabbingL && grabL.held && grabL.held.mass >= grabL.heavyMass;
			bool heavyR = grabbingR && grabR.held && grabR.held.mass >= grabR.heavyMass;
			if (!grabbingL)
			{
				selfPropUntilL = -9f;
			}
			if (!grabbingR)
			{
				selfPropUntilR = -9f;
			}
			if (grabbingL && !grabL.holdsYou && grabL.heldTouch && grabL.heldTouch.under)
			{
				selfPropUntilL = Time.time + selfPropHold;
			}
			if (grabbingR && !grabR.holdsYou && grabR.heldTouch && grabR.heldTouch.under)
			{
				selfPropUntilR = Time.time + selfPropHold;
			}
			bool propL = grabbingL && Time.time < selfPropUntilL;
			bool propR = grabbingR && Time.time < selfPropUntilR;
			selfProp = propL || propR;
			float pushL = 0f;
			if (heldL)
			{
				if (airL || (!grabbingL && airBody))
				{
					pushL = handPushAir;
				}
				else if ((clingL && !lowL) || ropeL)
				{
					pushL = handPushGrip;
				}
				else if (lowL)
				{
					pushL = handPushLedge;
				}
				else if (grabL && grabL.tugging)
				{
					pushL = handPushTug;
				}
				else if (useHexaL)
				{
					pushL = (grabL && grabL.second && !heavyL && !secondFollows) ? handPushSecond : hexaForce;
				}
				else if (heavyL)
				{
					pushL = handPushCarry;
				}
				else if (grabL && grabL.second && !secondFollows)
				{
					pushL = handPushSecond;
				}
				else
				{
					pushL = ledgeL ? handPushLedge : handPush;
				}
			}
			float pushR = 0f;
			if (heldR)
			{
				if (airR || (!grabbingR && airBody))
				{
					pushR = handPushAir;
				}
				else if ((clingR && !lowR) || ropeR)
				{
					pushR = handPushGrip;
				}
				else if (lowR)
				{
					pushR = handPushLedge;
				}
				else if (grabR && grabR.tugging)
				{
					pushR = handPushTug;
				}
				else if (useHexaR)
				{
					pushR = (grabR && grabR.second && !heavyR && !secondFollows) ? handPushSecond : hexaForce;
				}
				else if (heavyR)
				{
					pushR = handPushCarry;
				}
				else if (grabR && grabR.second && !secondFollows)
				{
					pushR = handPushSecond;
				}
				else
				{
					pushR = ledgeR ? handPushLedge : handPush;
				}
			}
			pushL *= Toward(leftHand, anchorL) * tieK;
			pushR *= Toward(rightHand, anchorR) * tieK;
			if (floorPressL)
			{
				pushL = Mathf.Min(pushL, floorPush);
			}
			if (floorPressR)
			{
				pushR = Mathf.Min(pushR, floorPush);
			}
			if (propL && !hexa)
			{
				pushL = Mathf.Min(pushL, handPushSelf);
			}
			if (propR && !hexa)
			{
				pushR = Mathf.Min(pushR, handPushSelf);
			}
			if (airL || ropeFreeL)
			{
				pushL = 0f;
			}
			if (airR || ropeFreeR)
			{
				pushR = 0f;
			}
			float spinL = 0f;
			float spinR = 0f;
			if (!airL && !ropeFreeL)
			{
				spinL = (grabbingL && grabL.held && grabL.held.mass >= grabL.heavyMass) ? handPushTorqueCarry : handPushTorque;
			}
			if (!airR && !ropeFreeR)
			{
				spinR = (grabbingR && grabR.held && grabR.held.mass >= grabR.heavyMass) ? handPushTorqueCarry : handPushTorque;
			}
			float dampL = grabbingL ? handPushSpinDampGrip : handPushSpinDamp;
			float dampR = grabbingR ? handPushSpinDampGrip : handPushSpinDamp;

			// Grounded hands dragging something sideways only get a share of the hip side force.
			float dragCapL = float.MaxValue;
			if (grounded && grabbingL && !grabL.holdsYou && sideNow > 0f)
			{
				dragCapL = dragShare * hipSideForce * sideNow;
			}
			float dragCapR = float.MaxValue;
			if (grounded && grabbingR && !grabR.holdsYou && sideNow > 0f)
			{
				dragCapR = dragShare * hipSideForce * sideNow;
			}
			if (Changed(dragCapL, sideCapL))
			{
				appliedPushL = -1f;
			}
			if (Changed(dragCapR, sideCapR))
			{
				appliedPushR = -1f;
			}
			sideCapL = dragCapL;
			sideCapR = dragCapR;
			if (Changed(pushL, appliedPushL) || spinL != appliedSpinL || appliedLockL != lockL || dampL != appliedDampL)
			{
				appliedPushL = pushL;
				appliedSpinL = spinL;
				appliedLockL = lockL;
				appliedDampL = dampL;
				if (heldL)
				{
					Mode(tieL, true, true, pushL, spinL, lockL, dampL);
				}
			}
			if (Changed(pushR, appliedPushR) || spinR != appliedSpinR || appliedLockR != lockR || dampR != appliedDampR)
			{
				appliedPushR = pushR;
				appliedSpinR = spinR;
				appliedLockR = lockR;
				appliedDampR = dampR;
				if (heldR)
				{
					Mode(tieR, true, true, pushR, spinR, lockR, dampR);
				}
			}
			if (tieK >= 1f && Time.time < tieEaseUntil + Time.fixedDeltaTime * 1.5f)
			{
				appliedPushR = -1f;
				appliedPushL = -1f;
			}
			lockRNow = lockR;
			onRNow = onR;
			selfRNow = selfR;
			contactRNow = contactR;
			pushRNow = pushR;
			gapRNow = gapR.magnitude;
			stretchRNow = anchorR ? Vector3.Distance(anchorR.position, rightHand.position) : 0f;
			if (airL && heldL)
			{
				AirHand(leftHand, anchorL, leftUpperArm, Vector3.zero, downBody ? handPushHeap : handPushAir);
			}
			if (airR && heldR)
			{
				AirHand(rightHand, anchorR, rightUpperArm, Vector3.zero, downBody ? handPushHeap : handPushAir);
			}
			if (ropeFreeL && heldL)
			{
				AirHand(leftHand, anchorL, leftUpperArm, HoldAxis(touchL, touchFL), handPushAir);
			}
			if (ropeFreeR && heldR)
			{
				AirHand(rightHand, anchorR, rightUpperArm, HoldAxis(touchR, touchFR), handPushAir);
			}

			// Where the held hands want the body to be.
			Vector3 gap = Vector3.zero;
			int count = 0;
			bool onWorldL = !crossL && (World(touchL) || World(touchFL));
			bool onWorldR = !crossR && (World(touchR) || World(touchFR));
			if (clingL)
			{
				gapL = Taut(grabL, leftHand.position, gapL);
			}
			if (clingR)
			{
				gapR = Taut(grabR, rightHand.position, gapR);
			}
			bool anchoredL = onWorldL || (ledgeL && grabbingL) || clingL;
			bool anchoredR = onWorldR || (ledgeR && grabbingR) || clingR;
			if (holdL && anchoredL)
			{
				gap += gapL;
				count++;
			}
			if (holdR && anchoredR)
			{
				gap += gapR;
				count++;
			}
			if (dangling && !BodyFirst)
			{
				float share = TotalMass() * 9.81f * ropeWeight / ((clingR ? 1 : 0) + (clingL ? 1 : 0));
				if (clingL && grabL.held)
				{
					grabL.held.AddForceAtPosition(Vector3.down * share, leftHand.position);
				}
				if (clingR && grabR.held)
				{
					grabR.held.AddForceAtPosition(Vector3.down * share, rightHand.position);
				}
			}
			if (touchL.selfPush + touchR.selfPush + touchFL.selfPush + touchFR.selfPush + touchUL.selfPush + touchUR.selfPush > selfPushMin)
			{
				selfPushUntil = Time.time + selfPushHold;
			}
			selfPush = Time.time < selfPushUntil;
			Vector3 handAtL = leftHand.transform.position;
			Vector3 handAtR = rightHand.transform.position;
			if (anchoredL)
			{
				gripPointL = handAtL;
			}
			if (anchoredR)
			{
				gripPointR = handAtR;
			}
			bool stillL = false;
			bool stillR = false;
			if (!grounded)
			{
				stillL = (handAtL - gripPointL).sqrMagnitude < holdSlip * holdSlip;
				stillR = (handAtR - gripPointR).sqrMagnitude < holdSlip * holdSlip;
			}
			holding = !flopped && (anchoredL || anchoredR || (holding && (stillL || stillR)));
			handGap = (count > 0) ? gap / count : Vector3.zero;
			gapStamp++;
			Vector3 lean = (touchHead && touchHead.touching && head) ? head.position - (headTarget.position + leadHead) : Vector3.zero;
			headGap = Flat(lean) + EyeGap();
		}

		private void Park(ref bool parked, ref float trackedSince, ref float lostSince, Tracker track, Transform target, ref Collider[] cols, Rigidbody hand, Rigidbody fore, ref Vector3 lastLocal, ref Vector3 vel, bool left)
		{
			HandGrab grab = hand ? hand.GetComponent<HandGrab>() : null;
			bool lost = playSpace && track != null && inVR && !track.tracking;
			if (!lost || (grab && grab.grabbing))
			{
				lostSince = Time.time;
			}
			bool due = false;
			if (lost)
			{
				due = !track.enabled || track.present ? Time.time >= lostSince + parkAfter : true;
			}
			bool want;
			if (target && target.parent && ((forcePark & (left ? 1 : 2)) != 0 || due))
			{
				trackedSince = Time.time;
				want = true;
			}
			else
			{
				want = parked && Time.time < trackedSince + parkFreeAfter;
			}
			if (parked != want)
			{
				parked = want;
				if (cols == null)
				{
					Collider[] handCols = hand ? hand.GetComponents<Collider>() : new Collider[0];
					Collider[] foreCols = fore ? fore.GetComponents<Collider>() : new Collider[0];
					cols = new Collider[handCols.Length + foreCols.Length];
					handCols.CopyTo(cols, 0);
					foreCols.CopyTo(cols, handCols.Length);
				}
				foreach (Collider c in cols)
				{
					if (c)
					{
						c.enabled = !parked;
					}
				}
				if (grab && parked && grab.grabbing)
				{
					grab.releaseWhy = "park";
					grab.Release();
				}
				if (!parked && playSpace)
				{
					lastLocal = playSpace.InverseTransformPoint(target.position);
					vel = Vector3.zero;
				}
			}
			if (!parked)
			{
				return;
			}
			Vector3 offset = parkOffset;
			Vector3 euler = parkEuler;
			if (left)
			{
				offset.x = -offset.x;
				euler.y = -euler.y;
				euler.z = -euler.z;
			}
			float yaw = view ? view.eulerAngles.y : 0f;
			Quaternion turn = Quaternion.Euler(0f, yaw, 0f);
			Vector3 at = Neck() + turn * offset;
			Quaternion rot = turn * Quaternion.Euler(euler);
			Transform parent = target.parent;
			parent.rotation = rot * Quaternion.Inverse(target.localRotation);
			parent.position = at - parent.rotation * target.localPosition;
			if (playSpace)
			{
				lastLocal = playSpace.InverseTransformPoint(target.position);
				vel = Vector3.zero;
			}
		}

		private bool EyeWorld(Collider c)
		{
			if (!c || c.transform.IsChildOf(transform) || c.GetComponentInParent<Rope>())
			{
				return false;
			}
			Rigidbody rb = c.attachedRigidbody;
			return !rb || rb.isKinematic || rb.mass >= 20f;
		}

		public Vector3 SweepEyes(Vector3 step)
		{
			if (!view)
			{
				return step;
			}
			float dist = step.magnitude;
			if (dist < 1e-05f)
			{
				return step;
			}
			Vector3 dir = step / dist;
			Vector3 from = view.position;
			float allowed = dist;
			int count = Physics.SphereCastNonAlloc(from, eyeRadius, dir, eyeCasts, dist, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < count; i++)
			{
				if (EyeWorld(eyeCasts[i].collider) && eyeCasts[i].distance < allowed)
				{
					allowed = eyeCasts[i].distance;
				}
			}
			count = Physics.RaycastNonAlloc(from, dir, eyeCasts, dist + eyeRadius, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < count; i++)
			{
				if (EyeWorld(eyeCasts[i].collider))
				{
					allowed = Mathf.Min(allowed, Mathf.Max(0f, eyeCasts[i].distance - eyeRadius));
				}
			}
			return dir * allowed;
		}

		private Vector3 EyeGap()
		{
			if (!view || !head)
			{
				return Vector3.zero;
			}
			Vector3 from = head.position;
			Vector3 d = view.position - from;
			float dist = d.magnitude;
			if (dist < 0.001f)
			{
				return Vector3.zero;
			}
			Vector3 dir = d / dist;
			int count = Physics.SphereCastNonAlloc(from, eyeRadius, dir, eyeCasts, dist, -1, QueryTriggerInteraction.Ignore);
			float best = -1f;
			for (int i = 0; i < count; i++)
			{
				if (EyeWorld(eyeCasts[i].collider) && (best < 0f || eyeCasts[i].distance < best))
				{
					best = eyeCasts[i].distance;
				}
			}
			if (best < 0f)
			{
				return Vector3.zero;
			}
			return -dir * (dist - best);
		}

		private float Toward(Rigidbody hand, Rigidbody anchor)
		{
			if (!hand || !anchor)
			{
				return 1f;
			}
			Vector3 gap = anchor.position - hand.position;
			if (gap.sqrMagnitude < handVgap * handVgap)
			{
				return 1f;
			}
			float over = Vector3.Dot(hand.linearVelocity, gap.normalized) - handVfree;
			if (over <= 0f)
			{
				return 1f;
			}
			return Mathf.Clamp(1f - over / Mathf.Max(handVmax, 0.1f), 0.1f, 1f);
		}

		private static bool World(HandTouch t)
		{
			return t && t.world;
		}

		private bool FloorPress(Rigidbody hand, HandTouch t, HandTouch tf, Rigidbody anchor, bool gripping, HandGrab grab)
		{
			if (!BodyFirst || !grounded || flopped || rising || hanging || gripping || floorPush < 0f)
			{
				return false;
			}
			if (!hand || !anchor || (grab && grab.grabbing))
			{
				return false;
			}
			if (!World(t) && !World(tf))
			{
				return false;
			}
			if (hand.position.y - floorY >= floorHand || pelvis.position.y - floorY <= floorHip)
			{
				return false;
			}
			return anchor.position.y < hand.position.y - 0.01f;
		}

		private bool LedgeNear(Rigidbody hand, HandGrab grab)
		{
			if (ledgeSnap <= 0f || !hand)
			{
				return false;
			}
			Rigidbody held = null;
			if (grab && grab.grabbing)
			{
				if (!grab.ledge && (!grab.held || grab.holdsYou || !grab.heldTouch || !grab.heldTouch.world || grab.heldTouch.under))
				{
					return false;
				}
				held = grab.held;
			}
			if (hand.position.y < pelvis.position.y + ledgeAbove)
			{
				return false;
			}
			return OnTop(hand, held, float.NegativeInfinity);
		}

		private bool OnTop(Rigidbody hand, Rigidbody held, float above)
		{
			int count = Physics.SphereCastNonAlloc(hand.position + Vector3.up * 0.02f, 0.04f, Vector3.down, hits, ledgeSnap + 0.02f, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < count; i++)
			{
				Collider c = hits[i].collider;
				Rigidbody rb = c.attachedRigidbody;
				if (rb && !rb.isKinematic && rb != held && (rb.mass < ledgeMass || Holds(rb) || rb.GetComponentInParent<Player>()))
				{
					continue;
				}
				if (c.transform.IsChildOf(transform) || c.GetComponentInParent<Rope>())
				{
					continue;
				}
				if (!(hits[i].normal.y > 0.5f) || !(hits[i].point.y > above))
				{
					continue;
				}
				lastTop = rb;
				return true;
			}
			return false;
		}

		private void Brace(Rigidbody want)
		{
			Rigidbody rb = want;
			if (want && (want == floorBody || want.GetComponentInParent<Player>() || want.GetComponentInParent<NetworkPlayer>() || Holds(want) || (wallTouch && want.GetComponent<NetRope>())))
			{
				rb = null;
			}
			NetProp prop = rb ? rb.GetComponent<NetProp>() : null;
			bool networked = prop && prop.Object && prop.Object.IsValid;
			if (networked && rb && ((prop.ShownHeld && !prop.HasStateAuthority) || prop.OnCarrier))
			{
				rb = null;
			}
			NetProp bracedProp = braced ? braced.GetComponent<NetProp>() : null;
			if (braced && bracedProp && bracedProp.Object && bracedProp.Object.IsValid && !bracedProp.HasStateAuthority)
			{
				rb = null;
			}
			if (rb == braced)
			{
				return;
			}
			if (braced)
			{
				braced.isKinematic = false;
				braced.WakeUp();
				braced = null;
			}
			if (!rb || rb.isKinematic)
			{
				return;
			}
			if (networked && !prop.HasStateAuthority)
			{
				prop.Claim(false);
				return;
			}
			rb.isKinematic = true;
			braced = rb;
		}

		private bool Vaulting()
		{
			if (!holding)
			{
				return false;
			}
			return OnTop(leftHand, null, floorY + 0.2f) || OnTop(rightHand, null, floorY + 0.2f);
		}

		private float Carrying()
		{
			float total = 0f;
			if (grabL && grabL.grabbing && !grabL.holdsYou && grabL.held)
			{
				total += grabL.held.mass;
			}
			if (grabR && grabR.grabbing && !grabR.holdsYou && grabR.held)
			{
				total += grabR.held.mass;
			}
			return total;
		}

		private static bool Changed(float now, float applied)
		{
			if ((now == 0f) != (applied == 0f))
			{
				return true;
			}
			return Mathf.Abs(now - applied) > Mathf.Max(now, applied) * 0.1f;
		}

		private bool Hold(ConfigurableJoint j, bool touching, Vector3 gap, ref bool held, float over, float turn = 0f, bool lockTurn = false, float sinceTouch = 99f, bool selfOnly = false, float slip = -1f)
		{
			float dist = gap.magnitude;
			float stretch = (slipRelease && slip >= 0f) ? slip : dist;
			if (!held && touching)
			{
				if (dist > handSlack + over)
				{
					held = true;
					Mode(j, true, true, handPush * tieK, handPushTorque, lockTurn);
				}
			}
			else if ((selfOnly || !touching) && held && relockSettle <= sinceTouch && stretch < handSlack + (selfOnly ? -0.01f : 0.01f))
			{
				if (relockTurn > turn || slipRelease)
				{
					held = false;
					Mode(j, true, false, handPush, handPushTorque, false);
				}
			}
			return held && dist > handSlack + 0.02f;
		}

		public Vector3 NeckNow()
		{
			return Neck();
		}

		private Vector3 Neck()
		{
			if (!headTarget)
			{
				return Vector3.zero;
			}
			if (view && headTarget.parent == view && playSpace)
			{
				Quaternion yaw = Quaternion.Euler(0f, view.eulerAngles.y, 0f);
				return view.position + view.rotation * neckPivot + yaw * (headTarget.localPosition - neckPivot);
			}
			return headTarget.position;
		}

		private void Head()
		{
			if (!headTarget)
			{
				return;
			}
			neck = Neck();
			anchorHead.MovePosition(headTarget.position + leadHead);
			if (!poseRead)
			{
				return;
			}
			Transform from = view ? view : headTarget;
			anchorHead.MoveRotation(from.rotation * headK);
		}

		private void Hips()
		{
			if (!poseRead)
			{
				return;
			}
			if (!headTarget)
			{
				return;
			}
			Transform look = view ? view : headTarget;
			Vector3 fwd = look.forward;
			if (fwd.x * fwd.x + fwd.z * fwd.z < 0.01f && view)
			{
				fwd = -view.up;
			}
			Vector3 flat = new Vector3(fwd.x, 0f, fwd.z);
			if (flat.x * flat.x + flat.z * flat.z < 0.01f)
			{
				flat = lastFwd;
			}
			lastFwd = flat.normalized;
			facing = Quaternion.LookRotation(lastFwd, Vector3.up) * Quaternion.Inverse(Quaternion.LookRotation(bindFwdLocal, bindUpLocal));
			neck = Neck();
			Vector3 goal = leadHips + (neck - facing * bChestToHead + facing * bChestToPelvis);
			Vector3 pos = pelvis.transform.position;
			Vector3 rodeWas = playSpace ? rode : Vector3.zero;
			if (playSpace)
			{
				rode = Vector3.zero;
			}
			bool snapped = false;
			if (!carriedHold && !trainHeld && !TrainWreck.Dragged(this))
			{
				snapped = !neckKnown || (neck - lastNeckWorld - rodeWas).sqrMagnitude > snapDist * snapDist;
			}
			neckKnown = true;
			lastNeckWorld = neck;
			float bestDist = float.MaxValue;
			Collider best = null;
			int count = Physics.RaycastNonAlloc(neck, Vector3.down, hits, hipReach + 2f, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < count; i++)
			{
				if (hits[i].collider.transform.IsChildOf(transform))
				{
					continue;
				}
				if (hits[i].collider.GetComponentInParent<Rope>())
				{
					continue;
				}
				if (Aloft(hits[i].collider))
				{
					continue;
				}
				if (hits[i].normal.y < SteepY)
				{
					continue;
				}
				if (hits[i].distance < bestDist && !Character(hits[i].collider))
				{
					bestDist = hits[i].distance;
					best = hits[i].collider;
				}
			}
			float mass = TotalMass();
			bool onGround = false;
			if (bestDist < float.MaxValue)
			{
				floorHeld = best && Holds(best.attachedRigidbody);
				floorBody = best ? best.attachedRigidbody : null;
				feetFloor = false;
				floorY = neck.y - bestDist;
				onGround = true;
			}
			else
			{
				floorHeld = false;
				floorBody = null;
				feetFloor = false;
				if (!holding && !gripping && feetGround && endL && endR)
				{
					float floorL = FloorAt(endL.position + Vector3.up * 0.05f, float.NegativeInfinity);
					float floorR = FloorAt(endR.position + Vector3.up * 0.05f, float.NegativeInfinity);
					bool onL = floorL > float.NegativeInfinity && endL.position.y - ankleL - floorL < footOff;
					bool onR = floorR > float.NegativeInfinity && endR.position.y - ankleR - floorR < footOff;
					if (onL || onR)
					{
						feetFloor = true;
						floorY = Mathf.Max(onL ? floorL : float.NegativeInfinity, onR ? floorR : float.NegativeInfinity);
						onGround = true;
					}
				}
			}
			bool near = onGround;
			if ((flopped || rising) && !carriedHold && !trainHeld)
			{
				if (onGround && pos.y - floorY < standHeight + airGap)
				{
					near = true;
				}
				else
				{
					float below = FloorAt(pos, float.NegativeInfinity);
					if (pos.y - below < standHeight + airGap)
					{
						floorY = below;
						near = true;
					}
				}
			}
			float height = pos.y - floorY;
			bool nearFloor = false;
			bool isGrounded = false;
			if (near && dummy && height < -0.4f)
			{
				float below = FloorAt(pos, float.NegativeInfinity);
				if (below > float.NegativeInfinity && pos.y - below < standHeight + airGap)
				{
					floorY = below;
					height = pos.y - below;
					nearFloor = true;
				}
				else
				{
					height = pos.y - floorY;
				}
			}
			else if (near)
			{
				nearFloor = true;
			}
			if (nearFloor)
			{
				if (Time.time < jumpUntil)
				{
					isGrounded = false;
				}
				else if (snapped)
				{
					isGrounded = true;
				}
				else
				{
					isGrounded = height < standHeight + airGap;
				}
			}
			grounded = isGrounded;
			if (!floating)
			{
				FellThrough(best, pos);
			}
			if (jumped && (grounded || flopped || holding || gripping))
			{
				jumped = false;
			}
			if (!jumped && grounded && airborne)
			{
				landing = true;
				landedAt = Time.time;
			}
			if (landing && (!grounded || pelvis.linearVelocity.y > -0.05f || Time.time > landedAt + 0.8f))
			{
				landing = false;
			}
			bool falling = false;
			bool bodyFirst = BodyFirst;
			if (!grounded)
			{
				if (bodyFirst && !holding)
				{
					if (gripping)
					{
						fallTop = 0f;
					}
					else
					{
						fallTop = flopped ? 0f : Mathf.Max(fallTop, -pelvis.linearVelocity.y);
						falling = true;
					}
				}
				else
				{
					fallTop = 0f;
				}
			}
			else
			{
				float top = fallTop;
				fallTop = 0f;
				if (top > 0f && top > flopSpeed && !flopped && !snapped && !dropping && !holding && !gripping)
				{
					if (!dummy)
					{
						Note("ragdoll from a hard landing at " + top.ToString("F1") + " m/s");
					}
					flopped = true;
					floppedAt = Time.time;
					riseLockedUntil = Time.time + flopLie;
					riseDue = true;
					autoRiseUntil = Time.time + flopLie + flopRecover;
					LetGo("fell");
					FlopFrame();
				}
			}
			if (flopped)
			{
				if (grounded)
				{
					if (flopBlackAt < floppedAt)
					{
						flopBlackAt = Time.time;
					}
					if (Time.time > floppedAt + flopSettle && Time.time > riseLockedUntil && !TrainWreck.Dragged(this) && !SlapDummy.Hauls(this)
						&& (jumpAsk || PushingDown() || (riseDue && !carriedHold && !(Time.time < limpUntil))))
					{
						if (!dummy)
						{
							Note("got up " + (Time.time - floppedAt).ToString("F1") + " s after going down (" + (jumpAsk ? "A" : (PushingDown() ? "push" : "auto")) + ")");
						}
						flopped = false;
						jumpAsk = false;
						gotUpAt = Time.time;
						rising = true;
						riseFrom = height;
						risingAt = Time.time;
						riseDue = false;
						flopFrameKnown = false;
					}
				}
				if (flopped)
				{
					if (!grounded)
					{
						Slip();
					}
					if (flopped && !carriedHold && !trainHeld)
					{
						Unbury();
					}
				}
			}
			riseProgress = rising ? Mathf.Clamp01(Mathf.InverseLerp(riseFrom, standHeight - 0.03f, height)) : (flopped ? 0f : 1f);
			bool stood = false;
			if (nearFloor && height > standHeight - 0.03f)
			{
				stood = Mathf.Abs(pelvis.linearVelocity.y) < 0.25f;
			}
			if (rising && (flopped || Time.time > risingAt + 1.5f || stood))
			{
				rising = false;
				if (stood && slipWalls.Count >= 1)
				{
					Unslip();
				}
				if (shieldOnRise && !flopped)
				{
					shieldOnRise = false;
					shieldUntil = Time.time + koShield;
				}
				bool holdingOn = hanging || gripping || (grabL && grabL.grabbing) || (grabR && grabR.grabbing);
				if (stood && !flopped && !holdingOn && playSpace)
				{
					Vector3 drift = pos - (goal - leadHips);
					if (drift.x * drift.x + drift.z * drift.z > 0.0004f)
					{
						Vector3 shift = new Vector3(drift.x, 0f, drift.z);
						playSpace.position += shift;
						rode += shift;
						goal += shift;
						neck += shift;
						anchorHip.position += shift;
					}
					Resnap();
					pinFadeUntil = -9f;
					tieEaseUntil = Time.time + tieEase;
				}
			}
			if (playSpace && grounded && !holding && !gripping && !flopped)
			{
				slipping = traction < slipRide;
				if (slipping && icePush > 0f && playSpace)
				{
					Vector3 wish = new Vector3(moveWish.x, 0f, moveWish.z);
					float want = Mathf.Clamp01(wish.magnitude / Mathf.Max(speed, 0.1f));
					Vector3 vel = pelvis.linearVelocity;
					float along = Vector3.Dot(new Vector3(vel.x, 0f, vel.z), wish.normalized) / Mathf.Max(speed, 0.1f);
					float push = want * Mathf.Clamp01(1f - along);
					if (push > 0.05f)
					{
						pelvis.AddForce(wish.normalized * (push * icePush), ForceMode.Force);
					}
				}
			}
			else
			{
				slipping = false;
			}
			if (hanging && playSpace && swingForce > 0f
				&& ((grabL && grabL.grabbing && grabL.heldRope) || (grabR && grabR.grabbing && grabR.heldRope)))
			{
				Vector3 wish = new Vector3(moveWish.x, 0f, moveWish.z);
				float k = Mathf.Clamp01(wish.magnitude / Mathf.Max(speed, 0.1f));
				if (k > 0.05f)
				{
					pelvis.AddForce(wish.normalized * (k * swingForce), ForceMode.Force);
				}
			}
			Kick();
			Feet(kicking || Time.time < kickedAt + kickTail);
			if (jumpAsk && !flopped && grounded && !gripping && !hanging && tone > 0.9f && !Vaulting())
			{
				jumpUntil = Time.time + jumpAirTime;
				float boost = Mathf.Lerp(1f, jumpSwingMult, Mathf.InverseLerp(jumpSwingFrom, jumpSwingFull, armUp));
				float charge = Mathf.Clamp01(jumpAskCharge);
				if (boost > 1.02f && !dummy)
				{
					Note("jump boosted x" + boost.ToString("F2") + " by an arm swing of " + armUp.ToString("F1") + " m/s");
				}
				jumpBoost = boost;
				jumpPower = boost * (jumpSpeed * Mathf.Lerp(jumpTap, jumpHold, charge));
				for (int i = 0; i < bodies.Length; i++)
				{
					if (bodies[i])
					{
						bodies[i].AddForce(Vector3.up * (jumpPower * bodies[i].mass), ForceMode.Impulse);
					}
				}
				grounded = false;
				jumped = true;
				if (!dummy && playSpace)
				{
					jumpCount++;
					PlayJump(hipSource);
				}
				falling = BodyFirst && !holding && !gripping;
				jumpAsk = false;
			}
			else if (jumpAsk && Time.time > jumpAskUntil && !dummy)
			{
				string why = (flopped ? " flopped" : "") + (grounded ? "" : " not-grounded") + (Vaulting() ? " hands on top of something (vault)" : "") + (gripping ? " gripping" : "") + (hanging ? " hanging" : "") + (tone > 0.9f ? "" : " tone " + tone.ToString("F2"));
				Note("jump press dropped after " + (jumpBuffer * 1000f).ToString("F0") + " ms:" + why);
			}
			if (Time.time > jumpAskUntil)
			{
				jumpAsk = false;
			}
			airNow = falling;
			if (nearFloor)
			{
				lostFloorSince = -1f;
			}
			else if (lostFloorSince < 0f)
			{
				lostFloorSince = Time.time;
			}
			else if (!floating && safeKnown && playSpace && Time.time - lostFloorSince > rescueAfter && Time.time - rescuedAt > rescueCooldown
				&& !(Water.main && pos.y > Water.main.Surface - 10f)
				&& !(knockedOut && Water.Over(pos))
				&& !Physics.Raycast(pos + Vector3.up * 0.2f, Vector3.down, 300f, -1, QueryTriggerInteraction.Ignore))
			{
				rescuedAt = Time.time;
				rescues++;
				SlapDummy slapDummy = dummy ? GetComponent<SlapDummy>() : null;
				if (slapDummy && slapDummy.kind != SlapDummy.Kind.Dummy)
				{
					lostFloorSince = -1f;
					Kill("fell off the map");
					return;
				}
				int back = Mathf.RoundToInt(rescueBack * 2f);
				Vector3 pelvisAt = safePelvis;
				Vector3 rigAt = safeRig;
				if (trailCount >= 1)
				{
					for (int k = Mathf.Min(trailCount, Mathf.Max(back, 1)); k >= 1; k--)
					{
						int at = (trailAt + trailPelvis.Length * 2 - k) % trailPelvis.Length;
						if (Physics.Raycast(trailPelvis[at] + Vector3.up * 0.5f, Vector3.down, 3f, -1, QueryTriggerInteraction.Ignore))
						{
							pelvisAt = trailPelvis[at];
							rigAt = trailRig[at];
							break;
						}
					}
				}
				playSpace.position = rigAt;
				Vector3 neckAt = Neck();
				playSpace.position += new Vector3(pelvisAt.x - neckAt.x, 0f, pelvisAt.z - neckAt.z);
				Recover(pelvisAt + Vector3.up * 0.05f, facing * bPelvisRel, "rescue, lost the floor");
				riseDue = flopped;
				Resnap();
				riseLockedUntil = 0f;
				lostFloorSince = -1f;
				autoRiseUntil = Time.time + 4f;
				return;
			}
			if (grounded && !holding && playSpace && pos.y - floorY > standHeight * 0.7f)
			{
				safePelvis = pos;
				safeRig = playSpace.position;
				safeKnown = true;
				if (Time.time >= nextTrail)
				{
					nextTrail = Time.time + 0.5f;
					trailPelvis[trailAt] = pos;
					trailRig[trailAt] = safeRig;
					trailAt = (trailAt + 1) % trailPelvis.Length;
					if (trailCount < trailPelvis.Length)
					{
						trailCount++;
					}
				}
			}
			onFloor = nearFloor && height > (holding ? -mantleBelow : -0.02f) && height < standHeight + airGap;
			float neckY = playSpace ? playSpace.InverseTransformPoint(neck).y : neck.y;
			if (grounded && !holding && !snapped)
			{
				if (neckStandKnown)
				{
					// Standing height rises at standRise per second, and only creeps down once the neck is back near it.
					float rise = Mathf.Min(neckY, neckStand + standRise * Time.fixedDeltaTime);
					float sink = neckStand - neckY < 0.1f ? Time.fixedDeltaTime * 0.01f : 0f;
					neckStand = Mathf.Max(rise, neckStand - sink);
				}
				else
				{
					neckStand = neckY;
				}
				neckStandKnown = true;
			}
			crouch = neckStandKnown ? Mathf.Clamp01((neckStand - neckY) / Mathf.Max(tuckDepth, 0.05f)) : 0f;
			bool dangleStart = !onFloor || crouch > dangleCrouch || pullingUp;
			bool dangleKeepOn = crouch > dangleKeep || pullingUp;
			dangling = gripping && (dangleStart || (dangleKeepOn && dangling));
			if (BodyFirst)
			{
				hanging = ((holding || gripping) && !onFloor) || dangling;
			}
			else
			{
				hanging = (holding && !grounded) || (gripping && crouch > 0.05f);
			}
			float gapL = 0f;
			if (endL)
			{
				gapL = endL.position.y - ankleL - FloorAt(endL.position + Vector3.up * 0.05f, floorY);
			}
			float gapR = 0f;
			if (endR)
			{
				gapR = endR.position.y - ankleR - FloorAt(endR.position + Vector3.up * 0.05f, floorY);
			}
			float footLimit = feetOff ? footOff * 0.3f : footOff;
			feetOff = gapL > footLimit && gapR > footLimit;
			if (grounded)
			{
				float friction = tractionFriction;
				if (best && best.sharedMaterial)
				{
					friction = best.sharedMaterial.dynamicFriction;
				}
				traction = Mathf.Clamp01(friction / Mathf.Max(tractionFriction, 0.01f));
			}
			if (!holding)
			{
				holdGoalY = float.NegativeInfinity;
				hangBase = float.NegativeInfinity;
			}
			float spinK = flopped ? 0f : (kicking ? hipSpinKick : (hanging ? hangSpin : ((holding || gripping) ? hipSpinHold : 1f)));
			if (tieHip && (Mathf.Abs(spinK - spinNow) > 0.01f || hanging != wasHanging))
			{
				spinNow = spinK;
				wasHanging = hanging;
				tieHip.slerpDrive = Drive(spinK * hipSpin, spinK * hipSpinDamp, hanging ? hangTorque : float.MaxValue);
			}
			int hipMode;
			bool hipOff = false;
			bool hipBall = false;
			bool hipHold = false;
			if (falling || flopped)
			{
				hipMode = 2;
				hipOff = true;
			}
			else if (ballMode)
			{
				hipMode = 3;
				hipBall = true;
			}
			else
			{
				hipHold = holding;
				hipMode = holding ? 1 : 0;
			}
			if (tieHip && hipMode != hipYNow)
			{
				hipYNow = hipMode;
				wasHolding = holding;
				tieHip.yDrive = hipOff ? default(JointDrive) : (hipBall ? Drive(0f, ballHipDamp, ballHipForce) : Drive(hipSpring, hipDamp, hipHold ? hipForceHold : hipForce));
			}
			if (rising && riseDamp > 0f)
			{
				Vector3 v = pelvis.linearVelocity;
				pelvis.AddForce(Vector3.ClampMagnitude(-new Vector3(v.x, 0f, v.z) * riseDamp, riseDampMax));
			}
			if (tieHip && rising && !hipOff && !hipBall)
			{
				float t = Mathf.Clamp01((Time.time - risingAt) / Mathf.Max(riseTime, 0.05f));
				tieHip.yDrive = Drive(hipSpring, hipDamp, (hipHold ? hipForceHold : hipForce) * Mathf.Lerp(riseStart, 1f, t));
				hipYNow = -1;
			}
			bool headRide = playSpace && ridingHead && !holding && !gripping;
			float trac = 0f;
			if (!headRide && !falling && !flopped && !rising && !slipping)
			{
				trac = traction;
			}
			float carry = Carrying();
			braceNow = Mathf.Min(carry * carryBrace / Mathf.Max(TotalMass(), 1f) + 1f, carryBraceMax);
			float side = 0f;
			if (!ballMode || holding || carry > 0f)
			{
				side = trac * ((holding || gripping || propped || carry > 0f) ? hipSideHold : 1f);
			}
			if (tieHip)
			{
				side = braceNow * side;
				if (Mathf.Abs(side - sideNow) > 0.02f)
				{
					sideNow = side;
					JointDrive sideDrive = Drive(trac * hipSide, trac * hipSideDamp, side * hipSideForce);
					tieHip.xDrive = sideDrive;
					tieHip.zDrive = sideDrive;
				}
			}
			if (playSpace && ridingHead)
			{
				trac = 0f;
			}
			if (tieHip && Mathf.Abs(braceNow * trac - tractionNow) > 0.02f)
			{
				tractionNow = braceNow * trac;
				if (tieHead)
				{
					JointDrive headDrive = Drive(trac * headSpring, trac * headDamp, braceNow * (trac * headForce));
					tieHead.xDrive = headDrive;
					tieHead.zDrive = headDrive;
				}
			}
			bool headLimp = flopped || rising;
			if (tieHead && headSpinOff != headLimp)
			{
				headSpinOff = headLimp;
				if (headLimp)
				{
					headSpinSaved = tieHead.slerpDrive;
					headPosSaved = tieHead.xDrive;
					tieHead.slerpDrive = default(JointDrive);
					tieHead.zDrive = default(JointDrive);
					tieHead.yDrive = default(JointDrive);
					tieHead.xDrive = default(JointDrive);
				}
				else
				{
					tieHead.slerpDrive = headSpinSaved;
					tieHead.zDrive = headPosSaved;
					tieHead.yDrive = headPosSaved;
					tieHead.xDrive = headPosSaved;
				}
			}
			float sag = mass * 9.81f / Mathf.Max(hipSpring, 1f);
			bool bodyGoal = BodyFirst;
			if (bodyGoal && (holding || gripping))
			{
				goalBranch = 1;
				if (!onFloor || dangling)
				{
					Vector3 lead = Vector3.ClampMagnitude(pelvis.linearVelocity * Time.fixedDeltaTime, hipLeadMax);
					goal = pos + lead;
					if (nearFloor)
					{
						goal.y = Mathf.Max(pos.y, floorY + hipMinHang);
					}
					if (holding && !offLedgeGrip && (ledgeL || ledgeR))
					{
						goal.y += ledgeLift;
					}
				}
				else
				{
					float stance = Mathf.Min(standHeight, BodyFirst ? standHeight - Mathf.Max(0f, neckStand - neckY) : neck.y - bHeadAbovePelvis - floorY);
					goal.y = Mathf.Max(sag + (floorY + stance), pos.y);
				}
				holdGoalY = float.NegativeInfinity;
				hangBase = float.NegativeInfinity;
			}
			else if (!bodyGoal && holding)
			{
				goalBranch = 2;
				goal.y = nearFloor ? sag + (floorY + (neck.y - bHeadAbovePelvis - floorY)) : float.NegativeInfinity;
				if (hanging)
				{
					if (crouch > 0f)
					{
						if (hangBase == float.NegativeInfinity)
						{
							hangBase = anchorHip.position.y;
						}
						goal.y = Mathf.Max(goal.y, Mathf.Min(hangBase + crouch * pullUp, anchorHip.position.y + pullRate * Time.fixedDeltaTime));
					}
				}
				else
				{
					hangBase = float.NegativeInfinity;
				}
				if (!gripping)
				{
					if (holdGoalY == float.NegativeInfinity)
					{
						holdGoalY = anchorHip.position.y;
					}
					goal.y = Mathf.Max(goal.y, holdGoalY);
				}
				if (float.IsInfinity(goal.y))
				{
					goal.y = anchorHip.position.y;
				}
				holdGoalY = goal.y;
			}
			else if (!grounded)
			{
				goalBranch = 4;
				if (!snapped)
				{
					goal.y = pos.y;
					if (falling)
					{
						goal.x = pos.x;
						goal.z = pos.z;
					}
				}
				else
				{
					goal.y = neck.y - bHeadAbovePelvis;
				}
			}
			else
			{
				float stance = Mathf.Min(standHeight, BodyFirst ? standHeight - Mathf.Max(0f, neckStand - neckY) : neck.y - bHeadAbovePelvis - floorY);
				goalBranch = 3;
				goalStance = stance - ballSquat;
				goal.y = sag + (floorY + goalStance);
				if (gripping || propped)
				{
					goal.y = Mathf.Max(goal.y, pos.y);
				}
				if (floorHeld)
				{
					goal.y = Mathf.Min(goal.y, pos.y + sag);
				}
			}
			if (flopped && !snapped)
			{
				goal = pos;
			}
			else if (snapped)
			{
				float ground = GroundUnder(goal);
				goal.y = Mathf.Max(goal.y, ground + standHeight * 0.9f);
				if (ground > goal.y - standHeight - airGap - 1f)
				{
					Recover(goal, facing * bPelvisRel, "camera jumped");
				}
				else
				{
					Note("camera jumped over no floor, kept the body at " + pos.ToString("F1"));
					goal.x = pos.x;
					goal.z = pos.z;
				}
				if (playSpace)
				{
					Resnap();
				}
				lastSnap = "t=" + Time.time.ToString("F2") + " goal=" + goal.ToString("F2") + " neck=" + neck.ToString("F2") + " pelvis=" + pelvis.position.ToString("F2");
				foreach (ConfigurableJoint j in GetComponentsInChildren<ConfigurableJoint>())
				{
					if (!j.connectedBody)
					{
						continue;
					}
					Vector3 a = j.transform.TransformPoint(j.anchor);
					Vector3 b = j.connectedBody.transform.TransformPoint(j.connectedAnchor);
					float gap = Vector3.Distance(a, b);
					if (gap > 0.1f)
					{
						lastSnap = lastSnap + " | " + j.name + "->" + j.connectedBody.name + " gap " + gap.ToString("F2") + " (rb " + j.GetComponent<Rigidbody>().position.ToString("F2") + " tf " + j.transform.position.ToString("F2") + ")";
					}
				}
				foreach (Rigidbody body in GetComponentsInChildren<Rigidbody>())
				{
					if (body.linearVelocity.magnitude > 5f)
					{
						lastSnap = lastSnap + " | fast " + body.name + " " + body.linearVelocity.magnitude.ToString("F0");
					}
				}
			}
			if (rising)
			{
				goal.x = pos.x;
				goal.z = pos.z;
			}
			anchorHip.MovePosition(goal);
			Quaternion hipRot = facing * bPelvisRel;
			if (kicking)
			{
				hipRot = Quaternion.AngleAxis(-(kickLean * Mathf.Clamp(airLegs, -1f, 1f)), Vector3.Cross(Vector3.up, lastFwd)) * hipRot;
			}
			if (hipTurn > 0f && !flopped && !rising)
			{
				hipRot = Quaternion.RotateTowards(anchorHip.rotation, hipRot, (kicking ? kickTurn : hipTurn) * Time.fixedDeltaTime);
			}
			anchorHip.MoveRotation(hipRot);
			hipHome = goal - leadHips;
			ridingY = goal.y == pos.y;
			headAbovePelvis = bHeadAbovePelvis;
			pelvisGoalY = goal.y - sag;
			if (BodyFirst)
			{
				hipHome.y = ridingY ? neck.y - bHeadAbovePelvis : goal.y - sag;
			}
			else
			{
				hipHome.y = (grounded || holding) ? hipHome.y - sag : neck.y - bHeadAbovePelvis;
			}
			hipOffset = hipHome - neck;
			float dist = Vector3.Distance(pos, goal);
			bool handsHigh = false;
			if (holding && !grounded)
			{
				handsHigh = Mathf.Max(leftHand.position.y, rightHand.position.y) > pos.y + 0.1f;
			}
			bool slick = playSpace && traction < slipRide;
			if (dummy && playSpace && (flopped || knockedOut) && !float.IsNaN(dist))
			{
				playSpace.position += new Vector3(pos.x - goal.x, 0f, pos.z - goal.z);
			}
			if (dummy && playSpace && !grounded && !holding && !gripping && pelvis.linearVelocity.y < -1f && !float.IsNaN(dist))
			{
				playSpace.position += Vector3.up * Mathf.Min(pos.y + bHeadAbovePelvis - neck.y, 0f);
			}
			if (float.IsNaN(dist) || (dist > recoverDist && !handsHigh && !holding && !gripping && !hanging && !flopped && !slick && Time.time > gotUpAt + 3f))
			{
				float now = Time.time;
				if (lostSince < 0f)
				{
					lostSince = now;
				}
				else if (now - lostSince > recoverTime)
				{
					if (dummy)
					{
						playSpace.position += new Vector3(pos.x - goal.x, 0f, pos.z - goal.z);
					}
					else if (Clear(goal) && GroundUnder(goal) > goal.y - standHeight - airGap - 1f)
					{
						Recover(goal, facing * bPelvisRel, "body lost " + dist.ToString("F2") + " m from its goal");
					}
					lostSince = -1f;
				}
			}
			else
			{
				lostSince = -1f;
			}
			Vector3 neckLocal = playSpace ? playSpace.InverseTransformPoint(neck) : neck;
			Vector3 goalVel = goalKnown ? new Vector3(neckLocal.x - lastGoal.x, 0f, neckLocal.z - lastGoal.z) / Time.fixedDeltaTime : Vector3.zero;
			if (playSpace)
			{
				goalVel = playSpace.TransformDirection(goalVel) + new Vector3(moveWish.x, 0f, moveWish.z);
			}
			lastGoal = neckLocal;
			goalKnown = true;
			float ease = 1f - Mathf.Exp(Time.fixedDeltaTime * -10f);
			goalPos = goal;
			walkVel = Vector3.Lerp(walkVel, goalVel, ease);
			if (grounded || holding)
			{
				goalPos.y -= sag;
			}
			goalRot = facing * bPelvisRel;
			Stride(walkVel, goalRot, lastFwd);
		}

		private void Stride(Vector3 v, Quaternion hips, Vector3 facing)
		{
			if (!endL || !endR)
			{
				return;
			}
			if (slipping)
			{
				v = pelvis.linearVelocity;
				v.y = 0f;
			}
			float speed = v.magnitude;
			float walk = walkFrom == walkFull ? 0f : Mathf.Clamp01((speed - walkFrom) / (walkFull - walkFrom));
			Vector3 dir = speed > walkFrom ? v / speed : facing;
			float cadence = Mathf.Clamp(speed / Mathf.Max(strideMax, 0.1f), cadenceMin, cadenceMax);
			float stride = 0f;
			bool walking = false;
			if (speed > walkFrom && grounded)
			{
				stride = Mathf.Min(speed / (cadence * 2f), strideMax * 0.5f);
				gait += cadence * Mathf.PI * 2f * Time.fixedDeltaTime;
				walking = true;
			}
			else if (!grounded)
			{
				walk = 0f;
			}
			Vector3 side = Vector3.Cross(dir, Vector3.up);
			Vector3 hipL = HipRef(leftUpperLeg, hipOffL);
			Vector3 hipR = HipRef(rightUpperLeg, hipOffR);
			Leg(leftUpperLeg, leftLowerLeg, endL, hipL, hips * relUL, hips * relLL, gait, walk, dir, side, stride, walking, ankleL, ref plantL, ref plantedL, ref floorL);
			Leg(rightUpperLeg, rightLowerLeg, endR, hipR, hips * relUR, hips * relLR, gait + Mathf.PI, walk, dir, side, stride, walking, ankleR, ref plantR, ref plantedR, ref floorR);
		}

		private Vector3 HipRef(Rigidbody upper, Vector3 off)
		{
			Vector3 at = goalPos;
			if (grounded)
			{
				at.y = Mathf.Max(at.y, floorY + crouchLow);
			}
			return at + goalRot * off;
		}

		private void Leg(Rigidbody upper, Rigidbody lower, Transform end, Vector3 hip, Quaternion restUpper, Quaternion restLower, float phase, float amp, Vector3 dir, Vector3 side, float len, bool walking, float ankle, ref Vector3 plant, ref bool planted, ref float floorAt)
		{
			float l1 = Vector3.Distance(upper.transform.position, lower.transform.position);
			float l2 = Vector3.Distance(lower.transform.position, end.position);
			if (l1 < 0.01f || l2 < 0.01f)
			{
				return;
			}
			if (kicking)
			{
				planted = false;
				float air = Mathf.Clamp(airLegs, -1f, 1f);
				float reach = (l1 + l2) * 0.95f;
				Solve(upper, lower, restUpper, restLower, side, reach * air, reach * (1f - Mathf.Abs(air)) + 0.05f, l1, l2);
				return;
			}
			if (hanging)
			{
				planted = false;
				float down = (l1 + l2) * (0.95f - legTuck * crouch);
				float x = Mathf.Lerp(-0.2f, kneesUp, crouch);
				if (lostFloorSince < 0f)
				{
					float gap = hip.y - (FloorAt(hip, floorY) + ankle + footClear);
					if (gap < down)
					{
						float reach = down;
						down = Mathf.Max(gap, 0.05f);
						x = Mathf.Min(Mathf.Sqrt(Mathf.Max(reach * reach - down * down, 0f)), (l1 + l2) * 0.75f);
					}
				}
				Solve(upper, lower, restUpper, restLower, side, x, down, l1, l2);
				return;
			}
			if (slipping)
			{
				planted = false;
			}
			float t = Mathf.Repeat(phase / (Mathf.PI * 2f), 1f);
			bool swing = t >= 0.5f;
			float forward;
			float floor;
			float lift = 0f;
			if (!grounded || (swing && walking))
			{
				planted = false;
				float s = (t - 0.5f) * 2f;
				forward = (s - 0.5f) * len;
				lift = Mathf.Sin(s * Mathf.PI) * (stepHeight * amp);
				floor = floorY;
			}
			else
			{
				if (!planted)
				{
					Vector3 at = walking ? hip + dir * (len * 0.5f) : end.position;
					floorAt = FloorAt(new Vector3(at.x, hip.y, at.z), floorY);
					if (floorAt < floorY - dropReach)
					{
						floorAt = FloorAt(hip, floorY);
						if (floorAt < floorY - dropReach)
						{
							floorAt = floorY;
						}
						at = hip;
					}
					plant = new Vector3(at.x, floorAt, at.z);
					planted = true;
				}
				forward = Vector3.Dot(plant - hip, dir);
				if (Mathf.Abs(forward) > Mathf.Max(replantAt, len * 0.5f + 0.1f))
				{
					planted = false;
				}
				floor = floorAt;
			}
			float drop = grounded ? hip.y - (floor + ankle + footClear) : (l1 + l2) * 0.95f;
			Solve(upper, lower, restUpper, restLower, side, forward, drop - lift, l1, l2);
		}

		private void Solve(Rigidbody upper, Rigidbody lower, Quaternion restUpper, Quaternion restLower, Vector3 side, float x, float down, float l1, float l2)
		{
			down = Mathf.Max(down, 0.05f);
			float d = Mathf.Sqrt(x * x + down * down);
			float reach = (l1 + l2) * 0.995f;
			if (d > reach)
			{
				x *= reach / d;
				down *= reach / d;
				d = reach;
			}
			float aim = Mathf.Atan2(x, down);
			float hipBend = Mathf.Acos(Mathf.Clamp((l1 * l1 + d * d - l2 * l2) / (2f * l1 * d), -1f, 1f));
			float knee = Mathf.Acos(Mathf.Clamp((l1 * l1 + l2 * l2 - d * d) / (2f * l1 * l2), -1f, 1f));
			float upperAngle = (aim + hipBend) * Mathf.Rad2Deg;
			float lowerAngle = upperAngle - (180f - knee * Mathf.Rad2Deg);
			PointRot(upper, Quaternion.AngleAxis(upperAngle, side) * restUpper);
			PointRot(lower, Quaternion.AngleAxis(lowerAngle, side) * restLower);
		}

		private bool Character(Collider c)
		{
			if (!c.attachedRigidbody)
			{
				return false;
			}
			Player p = c.GetComponentInParent<Player>();
			if (p)
			{
				if (!dummy && !p.dummy)
				{
					return false;
				}
				return !p.flopped && !p.knockedOut && !p.dead;
			}
			NetworkNpc npc = c.GetComponentInParent<NetworkNpc>();
			if (npc)
			{
				return !npc.Down;
			}
			NetworkPlayer np = dummy ? c.GetComponentInParent<NetworkPlayer>() : null;
			if (!np || !np.Object || !np.Object.IsValid)
			{
				return false;
			}
			return !np.Limp;
		}

		private float FloorAt(Vector3 at, float fallback)
		{
			int count = Physics.RaycastNonAlloc(at, Vector3.down, hits, hipReach, -1, QueryTriggerInteraction.Ignore);
			float best = float.MaxValue;
			for (int i = 0; i < count; i++)
			{
				if (hits[i].collider.transform.IsChildOf(transform))
				{
					continue;
				}
				if (hits[i].collider.GetComponentInParent<Rope>())
				{
					continue;
				}
				if (Aloft(hits[i].collider))
				{
					continue;
				}
				if (hits[i].normal.y < SteepY)
				{
					continue;
				}
				if (hits[i].distance < best && !Character(hits[i].collider))
				{
					best = hits[i].distance;
				}
			}
			return best < float.MaxValue ? at.y - best : fallback;
		}

		private static Transform Tip(Transform bone)
		{
			if (!bone)
			{
				return null;
			}
			foreach (Transform t in bone.GetComponentsInChildren<Transform>(true))
			{
				if (t != bone && t.name.EndsWith("_end"))
				{
					return t;
				}
			}
			return bone.childCount > 0 ? bone.GetChild(0) : null;
		}

		public void Shift(Vector3 delta)
		{
			foreach (Rigidbody r in GetComponentsInChildren<Rigidbody>())
			{
				if (r.interpolation != RigidbodyInterpolation.None)
				{
					r.interpolation = RigidbodyInterpolation.None;
					reInterp.Add(r);
				}
				Vector3 p = r.position + delta;
				r.position = p;
				r.transform.position = p;
				r.linearVelocity = Vector3.zero;
				r.angularVelocity = Vector3.zero;
			}
			reInterpAt = Time.fixedTime + Time.fixedDeltaTime * 2f;
			foreach (Rigidbody a in new Rigidbody[] { anchorL, anchorR, anchorHead, anchorHip })
			{
				if (a)
				{
					Vector3 p = a.position + delta;
					a.position = p;
					a.transform.position = p;
				}
			}
			if (neckKnown)
			{
				lastNeckWorld += delta;
			}
			plantedL = plantedR = false;
			plantL += delta;
			plantR += delta;
			gripPointL += delta;
			gripPointR += delta;
			safePelvis += delta;
			safeRig += delta;
			if (playSpace)
			{
				rode = Vector3.zero;
				Resnap();
			}
		}

		private void Spin(Vector3 pivot, float angle)
		{
			if (!pelvis || holding || gripping || hanging || flopped || knockedOut || dead || carriedHold || trainHeld)
			{
				return;
			}
			Quaternion q = Quaternion.AngleAxis(angle, Vector3.up);
			foreach (Rigidbody r in GetComponentsInChildren<Rigidbody>())
			{
				if (r.interpolation != RigidbodyInterpolation.None)
				{
					r.interpolation = RigidbodyInterpolation.None;
					reInterp.Add(r);
				}
				Turn(r, pivot, q);
			}
			reInterpAt = Time.fixedTime + Time.fixedDeltaTime * 2f;
			foreach (Rigidbody a in new Rigidbody[] { anchorL, anchorR, anchorHead, anchorHip })
			{
				if (a)
				{
					Turn(a, pivot, q);
				}
			}
			foreach (HandGrab g in new HandGrab[] { grabL, grabR })
			{
				if (g && g.grabbing && g.held && !g.holdsYou && !g.held.isKinematic && !g.held.transform.IsChildOf(transform))
				{
					Turn(g.held, pivot, q);
				}
			}
			if (neckKnown)
			{
				lastNeckWorld = pivot + q * (lastNeckWorld - pivot);
			}
			plantedL = plantedR = false;
			plantL = pivot + q * (plantL - pivot);
			plantR = pivot + q * (plantR - pivot);
			safePelvis = pivot + q * (safePelvis - pivot);
			safeRig = pivot + q * (safeRig - pivot);
			lastFwd = q * lastFwd;
		}

		private static void Turn(Rigidbody rb, Vector3 pivot, Quaternion q)
		{
			Vector3 pos = pivot + q * (rb.position - pivot);
			Quaternion rot = q * rb.rotation;
			rb.position = pos;
			rb.rotation = rot;
			rb.transform.SetPositionAndRotation(pos, rot);
			if (!rb.isKinematic)
			{
				rb.linearVelocity = q * rb.linearVelocity;
				rb.angularVelocity = q * rb.angularVelocity;
			}
		}

		private void ReInterp()
		{
			if (reInterpAt < 0f || Time.fixedTime < reInterpAt)
			{
				return;
			}
			foreach (Rigidbody r in reInterp)
			{
				if (r)
				{
					r.interpolation = RigidbodyInterpolation.Interpolate;
				}
			}
			reInterp.Clear();
			reInterpAt = -1f;
		}

		private void Recover(Vector3 pelvisPos, Quaternion pelvisRot, string why)
		{
			if (!dummy && pelvis && (pelvisPos - pelvis.position).sqrMagnitude > 0.25f)
			{
				Note("teleport body (" + why + ") " + (pelvisPos - pelvis.position).magnitude.ToString("F2") + " m from " + pelvis.position.ToString("F1") + " to " + pelvisPos.ToString("F1") + (cam ? ", camera at " + cam.position.ToString("F1") : ""));
			}
			recovers++;
			recoveredAt = Time.time;
			if (restPos == null)
			{
				return;
			}
			for (int i = 0; i < bodies.Length; i++)
			{
				Rigidbody rb = bodies[i];
				if (!rb)
				{
					continue;
				}
				Put(rb, pelvisPos + pelvisRot * restPos[i], pelvisRot * restRot[i]);
				rb.linearVelocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
			}
			Snap();
			heldL = heldR = false;
			if (grabL)
			{
				grabL.releaseWhy = "reset";
				grabL.Release();
			}
			if (grabR)
			{
				grabR.releaseWhy = "reset";
				grabR.Release();
			}
			plantedL = plantedR = false;
			if (tieL)
			{
				Mode(tieL, tied, false, handPush, handPushTorque);
			}
			if (tieR)
			{
				Mode(tieR, tied, false, handPush, handPushTorque);
			}
			appliedPushL = appliedPushR = 0f;
			Put(anchorHip, pelvisPos, pelvisRot);
			if (anchorHead && head)
			{
				Put(anchorHead, head.position, head.rotation);
			}
			Rearm(leftUpperArm, leftForearm, leftHand, handTargetL, wristL, leadL, capL, anchorL);
			Rearm(rightUpperArm, rightForearm, rightHand, handTargetR, wristR, leadR, capR, anchorR);
		}

		private void ArmJump()
		{
			if (!pelvis || !handTargetL || !handTargetR || !rearmJumps || !leftUpperArm || !rightUpperArm)
			{
				return;
			}
			Vector3 localL = pelvis.transform.InverseTransformPoint(handTargetL.position);
			Vector3 localR = pelvis.transform.InverseTransformPoint(handTargetR.position);
			Vector3 fwd = Vector3.Cross(rightUpperArm.position - leftUpperArm.position, Vector3.up);
			fwd.y = 0f;
			fwd.Normalize();
			Vector3 mid = (leftUpperArm.position + rightUpperArm.position) * 0.5f;
			Vector3 core = chest ? chest.worldCenterOfMass : mid;
			if (!grabL || !grabL.grabbing)
			{
				if ((armJumpKnown && (localL - armJumpL).sqrMagnitude > targetJump * targetJump) || Stuck(leftForearm, leftHand, handTargetL, fwd, mid, core))
				{
					Rearm(leftUpperArm, leftForearm, leftHand, handTargetL, wristL, leadL, capL, anchorL);
					prevUrL = leftUpperArm.rotation;
					prevFrL = leftForearm.rotation;
				}
			}
			if (!grabR || !grabR.grabbing)
			{
				if ((armJumpKnown && (localR - armJumpR).sqrMagnitude > targetJump * targetJump) || Stuck(rightForearm, rightHand, handTargetR, fwd, mid, core))
				{
					Rearm(rightUpperArm, rightForearm, rightHand, handTargetR, wristR, leadR, capR, anchorR);
					prevUrR = rightUpperArm.rotation;
					prevFrR = rightForearm.rotation;
				}
			}
			armJumpL = localL;
			armJumpR = localR;
			armJumpKnown = true;
		}

		private bool Stuck(Rigidbody fore, Rigidbody hand, Transform target, Vector3 fwd, Vector3 mid, Vector3 core)
		{
			if (!fore || !hand)
			{
				return false;
			}
			if ((hand.position - target.position).sqrMagnitude < stuckGap * stuckGap)
			{
				return false;
			}
			Vector3 at = fore.position;
			if (Vector3.Dot(fwd, fore.position - mid) < -stuckBehind)
			{
				return true;
			}
			return new Vector2(at.x - core.x, at.z - core.z).magnitude < stuckInside;
		}

		private void Rearm(Rigidbody upper, Rigidbody fore, Rigidbody hand, Transform target, Quaternion bindRel, Vector3 lead, Collider cap, Rigidbody anchor)
		{
			if (!upper || !fore || !hand || !anchor)
			{
				return;
			}
			if (target)
			{
				Vector3 goal = Reachable(upper, fore, hand, target.position + lead);
				goal = Stop(hand, cap, goal);
				Place(upper, fore, hand, target, bindRel, goal, true);
			}
			Put(anchor, hand.position, hand.rotation);
		}

		private static void Put(Rigidbody rb, Vector3 pos, Quaternion rot)
		{
			teleportAt = Time.time;
			rb.transform.SetPositionAndRotation(pos, rot);
			rb.position = pos;
			rb.rotation = rot;
		}

		private bool ArmPose(Rigidbody upper, Rigidbody fore, Rigidbody hand, Vector3 want, Quaternion handRot, Quaternion bindRel, out Vector3 ud, out Vector3 fd, out Quaternion ur, out Quaternion fr)
		{
			ud = fd = Vector3.zero;
			ur = fr = Quaternion.identity;
			if (!upper || !fore || !hand)
			{
				return false;
			}
			Vector3 shoulder = Shoulder(upper);
			float upperLen = Vector3.Distance(upper.transform.position, fore.transform.position);
			Vector3 goal = Reachable(upper, fore, hand, want);
			Quaternion bone = handRot * Quaternion.Inverse(bindRel);
			Vector3 elbow = ElbowPoint(upper, fore, hand, goal, bone);
			ud = elbow - shoulder;
			if (ud.sqrMagnitude < 0.0001f)
			{
				return false;
			}
			ud = ud.normalized;
			fd = goal - (shoulder + ud * upperLen);
			if (fd.sqrMagnitude < 0.0001f)
			{
				fd = ud;
			}
			fd = fd.normalized;
			Vector3 reach = goal - shoulder;
			bool left = upper == leftUpperArm;
			Vector3 stick = left ? elbowStickL : elbowStickR;
			Vector3 pole = Vector3.zero;
			if (reach.sqrMagnitude > 0.0001f && stick.sqrMagnitude > 0.0001f)
			{
				pole = Vector3.Cross(reach.normalized, stick);
			}
			if (pole.sqrMagnitude < 0.0001f)
			{
				pole = Vector3.Cross(ud, fd);
			}
			if (pole.sqrMagnitude < 0.0001f)
			{
				pole = Vector3.Cross(ud, Vector3.up);
			}
			if (pole.sqrMagnitude < 0.0001f)
			{
				pole = Vector3.Cross(ud, Vector3.right);
			}
			pole = pole.normalized;
			if (Vector3.Angle(Quaternion.AngleAxis(Vector3.Angle(ud, fd), pole) * ud, fd) > 1f)
			{
				pole = -pole;
			}
			ur = Quaternion.LookRotation(Vector3.Cross(pole, ud), ud);
			fr = Quaternion.LookRotation(Vector3.Cross(pole, fd), fd);
			if (wristFollow > 0f && (!playSpace || inVR))
			{
				Vector3 twist = Vector3.ProjectOnPlane(handRot * Quaternion.Inverse(bindRel) * Vector3.forward, fd);
				Vector3 roll = left ? rollRefL : rollRefR;
				float size = twist.magnitude;
				if (size > 0.08f)
				{
					Vector3 twistDir = twist / size;
					roll = roll.sqrMagnitude < 0.0001f ? twistDir : Vector3.RotateTowards(roll.normalized, twistDir, elbowTurn * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f);
				}
				if (left)
				{
					rollRefL = roll;
				}
				else
				{
					rollRefR = roll;
				}
				Vector3 flat = Vector3.ProjectOnPlane(roll, fd);
				if (flat.sqrMagnitude > 0.0001f)
				{
					float angle = Vector3.SignedAngle(fr * Vector3.forward, flat.normalized, fd);
					float was = left ? rollAngL : rollAngR;
					float now = was + Mathf.DeltaAngle(was, angle);
					if (left)
					{
						rollAngL = now;
					}
					else
					{
						rollAngR = now;
					}
					float limit = Mathf.Min(elbowTwistMax, elbowRoll);
					fr = Quaternion.AngleAxis(Mathf.Clamp(now * wristFollow, -limit, limit), fd) * fr;
				}
			}
			return true;
		}

		private void Arm(Rigidbody upper, Rigidbody fore, Rigidbody hand, Transform target, bool held, bool grab)
		{
			if (!target)
			{
				return;
			}
			bool left = upper == leftUpperArm;
			Vector3 want;
			Quaternion handRot;
			if (held && (grab || !armFix))
			{
				handRot = hand.transform.rotation;
				want = hand.transform.position;
			}
			else
			{
				want = target.position + (left ? leadL : leadR);
				handRot = Wrist(target);
			}
			if (flopped)
			{
				want = Fallen(want);
				handRot = Fallen(handRot);
			}
			if (!ArmPose(upper, fore, hand, want, handRot, left ? wristL : wristR, out Vector3 ud, out Vector3 fd, out Quaternion ur, out Quaternion fr))
			{
				return;
			}
			float step = (Time.time < softUntil ? frameSlewSoft : frameSlew) * Time.fixedDeltaTime;
			if (left)
			{
				frameTurnL = Mathf.Max(Quaternion.Angle(prevUrL, ur), Quaternion.Angle(prevFrL, fr));
				if (frameKnownL)
				{
					ur = Quaternion.RotateTowards(prevUrL, ur, step);
					fr = Quaternion.RotateTowards(prevFrL, fr, step);
				}
				frameKnownL = true;
				prevUrL = ur;
				prevFrL = fr;
			}
			else
			{
				frameTurnR = Mathf.Max(Quaternion.Angle(prevUrR, ur), Quaternion.Angle(prevFrR, fr));
				if (frameKnownR)
				{
					ur = Quaternion.RotateTowards(prevUrR, ur, step);
					fr = Quaternion.RotateTowards(prevFrR, fr, step);
				}
				frameKnownR = true;
				prevUrR = ur;
				prevFrR = fr;
			}
			PointRot(upper, ur);
			PointRot(fore, fr);
			PointRot(hand, handRot);
		}

		private void Place(Rigidbody upper, Rigidbody fore, Rigidbody hand, Transform target, Quaternion bindRel, Vector3 goal, bool force = false)
		{
			if (!force && placeSpeed > 0f && Time.time < placeEaseUntil)
			{
				float step = placeSpeed * Time.fixedDeltaTime;
				if ((goal - hand.transform.position).sqrMagnitude > step * step)
				{
					goal = hand.transform.position + (goal - hand.transform.position).normalized * step;
				}
			}
			if (!upper || !fore || !hand || !target)
			{
				return;
			}
			if (!force && (goal - hand.transform.position).sqrMagnitude < handSnap * handSnap)
			{
				return;
			}
			Quaternion handRot = Wrist(target);
			if (!ArmPose(upper, fore, hand, goal, handRot, bindRel, out Vector3 ud, out Vector3 fd, out Quaternion ur, out Quaternion fr))
			{
				return;
			}
			Vector3 shoulder = upper.transform.position;
			float upperLen = Vector3.Distance(shoulder, fore.transform.position);
			float foreLen = Vector3.Distance(fore.transform.position, hand.transform.position);
			Put(upper, shoulder, ur);
			Vector3 elbow = shoulder + upperLen * ud;
			Put(fore, elbow, fr);
			Put(hand, elbow + foreLen * fd, fr * bindRel);
			upper.angularVelocity = Vector3.zero;
			fore.linearVelocity = Vector3.zero;
			fore.angularVelocity = Vector3.zero;
			hand.linearVelocity = Vector3.zero;
			hand.angularVelocity = Vector3.zero;
			Put(hand == leftHand ? anchorL : anchorR, hand.position, hand.rotation);
			if (!force)
			{
				return;
			}
			if (hand == leftHand)
			{
				smoothL = hand.position;
				smoothLocalL = ToSpace(smoothL);
			}
			else
			{
				smoothR = hand.position;
				smoothLocalR = ToSpace(smoothR);
			}
		}

		private Vector3 Stop(Rigidbody hand, Collider col, Vector3 goal)
		{
			if (!col)
			{
				return goal;
			}
			Vector3 from = hand.transform.position;
			Vector3 move = goal - from;
			float dist = move.magnitude;
			if (dist < 0.0001f)
			{
				return goal;
			}
			Vector3 dir = move / dist;
			BoxCollider box = col as BoxCollider;
			CapsuleCollider capsule = col as CapsuleCollider;
			SphereCollider sphere = col as SphereCollider;
			int count;
			if (sphere)
			{
				count = Physics.SphereCastNonAlloc(sphere.transform.TransformPoint(sphere.center), sphere.radius * 0.95f, dir, hits, dist, -1, QueryTriggerInteraction.Ignore);
			}
			else if (box)
			{
				Vector3 center = box.transform.TransformPoint(box.center);
				Vector3 halfExtents = Vector3.Scale(box.size, box.transform.lossyScale) * 0.475f;
				count = Physics.BoxCastNonAlloc(center, halfExtents, dir, hits, box.transform.rotation, dist, -1, QueryTriggerInteraction.Ignore);
			}
			else if (capsule)
			{
				Vector3 axis = capsule.direction == 0 ? Vector3.right : (capsule.direction == 1 ? Vector3.up : Vector3.forward);
				float half = Mathf.Max(capsule.height * 0.5f - capsule.radius, 0f);
				Vector3 center = capsule.transform.TransformPoint(capsule.center);
				Vector3 offset = capsule.transform.TransformDirection(axis) * half;
				count = Physics.CapsuleCastNonAlloc(center + offset, center - offset, capsule.radius * 0.95f, dir, hits, dist, -1, QueryTriggerInteraction.Ignore);
			}
			else
			{
				return goal;
			}
			float nearest = dist;
			bool blocked = false;
			for (int i = 0; i < count; i++)
			{
				Collider c = hits[i].collider;
				if (c.transform.IsChildOf(transform))
				{
					continue;
				}
				if (hits[i].collider.GetComponentInParent<Rope>())
				{
					continue;
				}
				if (Carried(hand, hits[i].collider))
				{
					continue;
				}
				if (wallB.Contains(hits[i].collider) && Physics.GetIgnoreCollision(col, hits[i].collider))
				{
					continue;
				}
				if (hits[i].distance < nearest)
				{
					nearest = hits[i].distance;
					blocked = true;
				}
			}
			if (hand == rightHand)
			{
				stoppedR = blocked ? nearest : -1f;
			}
			if (!blocked)
			{
				return goal;
			}
			return from + dir * Mathf.Min(nearest + handPress, dist);
		}

		private Vector3 Lead(Transform target, ref Vector3 last, ref Vector3 vel, float seconds)
		{
			if (!playSpace || !target)
			{
				return Vector3.zero;
			}
			Vector3 local = playSpace.InverseTransformPoint(target.position);
			if (leadRead && !ignoreInput && (local - last).sqrMagnitude > targetJump * targetJump)
			{
				// The tracked point jumped (recentre / teleport): re-seat instead of leading into the jump.
				Seat();
				if (tied)
				{
					SoftHold();
				}
				if (!leadOnJump)
				{
					last = local;
					vel = Vector3.zero;
					return moveWish * seconds + Slide();
				}
			}
			Vector3 now = leadRead ? (local - last) / Time.fixedDeltaTime : Vector3.zero;
			last = local;
			vel = Vector3.Lerp(vel, now, 1f - Mathf.Exp(Time.fixedDeltaTime * -20f));
			Vector3 lead = Vector3.ClampMagnitude(playSpace.TransformDirection(vel) * seconds, leadMax);
			return moveWish * seconds + lead + Slide();
		}

		private Vector3 Slide()
		{
			if (!playSpace || ridingHead || traction < slipRide)
			{
				return Vector3.zero;
			}
			return spaceVel * Mathf.Clamp(Time.time - spaceTime, 0f, 0.05f);
		}

		private Vector3 Unbend(Rigidbody upper, Rigidbody fore, Rigidbody hand, Vector3 goal)
		{
			ConfigurableJoint elbow = fore ? fore.GetComponent<ConfigurableJoint>() : null;
			if (!elbow || !upper || !hand)
			{
				return goal;
			}
			Vector3 axis = fore.transform.TransformDirection(elbow.axis).normalized;
			Vector3 upperDir = Vector3.ProjectOnPlane(fore.position - upper.position, axis);
			Vector3 foreDir = Vector3.ProjectOnPlane(hand.position - fore.position, axis);
			float k = Mathf.Clamp01((-Vector3.SignedAngle(upperDir, foreDir, axis) - straightFrom) / straightRange);
			if (k <= 0f)
			{
				return goal;
			}
			Vector3 straight = fore.position + (fore.position - upper.position).normalized * Vector3.Distance(fore.position, hand.position);
			return Vector3.Lerp(goal, straight, k);
		}

		private Vector3 Reachable(Rigidbody upper, Rigidbody fore, Rigidbody hand, Vector3 target)
		{
			if (!upper || !fore || !hand)
			{
				return target;
			}
			Vector3 shoulderAt = upper.transform.position;
			Vector3 elbowAt = fore.transform.position;
			Vector3 handAt = hand.transform.position;
			Vector3 from = reachFromHead && poseRead ? HeadShoulder(upper) : Shoulder(upper);
			Vector3 toward = target - from;
			float dist = toward.magnitude;
			if (dist < 1e-4f)
			{
				return target;
			}
			float arm = Vector3.Distance(shoulderAt, elbowAt) + Vector3.Distance(elbowAt, handAt);
			if (dist <= arm * maxReach)
			{
				return target;
			}
			return from + toward * (arm * maxReach / dist);
		}

		private Vector3 HeadShoulder(Rigidbody upper)
		{
			return neck - facing * bChestToHead + facing * (upper == leftUpperArm ? bChestToShL : bChestToShR);
		}

		private Vector3 Shoulder(Rigidbody upper)
		{
			Vector3 at = upper.transform.position;
			bool left = upper == leftUpperArm;
			float blend = left ? shBlendL : shBlendR;
			if (blend <= 0f || !poseRead)
			{
				return at;
			}
			Vector3 bound = neck - facing * bChestToHead + facing * (left ? bChestToShL : bChestToShR);
			return Vector3.Lerp(at, bound, blend);
		}

		private Quaternion Wrist(Transform target)
		{
			return target.rotation * Quaternion.Euler(handKey);
		}

		private void ReadPose()
		{
			if (poseRead)
			{
				return;
			}
			SkinnedMeshRenderer skin = BodySkin();
			if (!skin || !skin.sharedMesh)
			{
				return;
			}
			Matrix4x4[] bindposes = skin.sharedMesh.bindposes;
			Transform[] bones = skin.bones;
			Matrix4x4 chestBind = Matrix4x4.identity;
			Matrix4x4 headBind = Matrix4x4.identity;
			Matrix4x4 pelvisBind = Matrix4x4.identity;
			Matrix4x4 shoulderL = Matrix4x4.identity;
			Matrix4x4 shoulderR = Matrix4x4.identity;
			Vector3 hipL = Vector3.zero;
			Vector3 hipR = Vector3.zero;
			Quaternion upperL = Quaternion.identity;
			Quaternion lowerL = Quaternion.identity;
			Quaternion upperR = Quaternion.identity;
			Quaternion lowerR = Quaternion.identity;
			bool hasChest = false;
			bool hasHead = false;
			bool hasPelvis = false;
			for (int i = 0; i < bones.Length; i++)
			{
				Transform bone = bones[i];
				if (!bone)
				{
					continue;
				}
				Matrix4x4 world = bindposes[i].inverse;
				if (bone == chest.transform)
				{
					chestBind = world;
					hasChest = true;
				}
				if (bone == head.transform)
				{
					headBind = world;
					hasHead = true;
				}
				if (bone == pelvis.transform)
				{
					pelvisBind = world;
					hasPelvis = true;
				}
				if (bone == leftUpperArm.transform)
				{
					shoulderL = world;
				}
				if (bone == rightUpperArm.transform)
				{
					shoulderR = world;
				}
				if (bone == leftUpperLeg.transform)
				{
					hipL = world.GetColumn(3);
					upperL = world.rotation;
				}
				if (bone == rightUpperLeg.transform)
				{
					hipR = world.GetColumn(3);
					upperR = world.rotation;
				}
				if (bone == leftLowerLeg.transform)
				{
					lowerL = world.rotation;
				}
				if (bone == rightLowerLeg.transform)
				{
					lowerR = world.rotation;
				}
			}
			if (!hasChest || !hasHead || !hasPelvis)
			{
				return;
			}
			Quaternion toPelvis = Quaternion.Inverse(pelvisBind.rotation);
			legUL = toPelvis * (upperL * Vector3.up);
			legLL = toPelvis * (lowerL * Vector3.up);
			legUR = toPelvis * (upperR * Vector3.up);
			legLR = toPelvis * (lowerR * Vector3.up);
			relUL = toPelvis * upperL;
			relLL = toPelvis * lowerL;
			relUR = toPelvis * upperR;
			relLR = toPelvis * lowerR;
			Matrix4x4 toChest = chestBind.inverse;
			bChestToHead = toChest.MultiplyPoint3x4(headBind.GetColumn(3));
			bChestToPelvis = toChest.MultiplyPoint3x4(pelvisBind.GetColumn(3));
			bChestToShL = toChest.MultiplyPoint3x4(shoulderL.GetColumn(3));
			bChestToShR = toChest.MultiplyPoint3x4(shoulderR.GetColumn(3));
			bHeadAbovePelvis = Vector3.Distance(headBind.GetColumn(3), pelvisBind.GetColumn(3));
			bPelvisRel = Quaternion.Inverse(chestBind.rotation) * pelvisBind.rotation;
			Vector3 up = bChestToHead.sqrMagnitude > 1e-4f ? bChestToHead.normalized : Vector3.up;
			Vector3 fwd = Vector3.Cross(toChest.MultiplyVector(hipR - hipL), up);
			if (fwd.sqrMagnitude < 1e-4f)
			{
				return;
			}
			bindUpLocal = up;
			bindFwdLocal = fwd.normalized;
			headK = Quaternion.Inverse(Quaternion.LookRotation(bindFwdLocal, bindUpLocal)) * (Quaternion.Inverse(chestBind.rotation) * headBind.rotation);
			poseRead = true;
		}

		private void Wrists()
		{
			if (wristRead)
			{
				return;
			}
			SkinnedMeshRenderer skin = BodySkin();
			if (!skin || !skin.sharedMesh)
			{
				return;
			}
			Matrix4x4[] bindposes = skin.sharedMesh.bindposes;
			Transform[] bones = skin.bones;
			Quaternion palmL = Quaternion.identity;
			Quaternion palmR = Quaternion.identity;
			Quaternion foreL = Quaternion.identity;
			Quaternion foreR = Quaternion.identity;
			for (int i = 0; i < bones.Length; i++)
			{
				Transform bone = bones[i];
				if (!bone)
				{
					continue;
				}
				Quaternion rot = bindposes[i].inverse.rotation;
				if (bone == leftHand.transform)
				{
					palmL = rot;
				}
				if (bone == rightHand.transform)
				{
					palmR = rot;
				}
				if (bone == leftForearm.transform)
				{
					foreL = rot;
				}
				if (bone == rightForearm.transform)
				{
					foreR = rot;
				}
			}
			wristL = Quaternion.Inverse(foreL) * palmL;
			wristR = Quaternion.Inverse(foreR) * palmR;
			wristRead = true;
		}

		private Vector3 ElbowPoint(Rigidbody upper, Rigidbody fore, Rigidbody hand, Vector3 goal, Quaternion bone)
		{
			if (!upper)
			{
				return Vector3.zero;
			}
			return Solve(upper, fore, hand, goal, bone);
		}

		private Vector3 Solve(Rigidbody upper, Rigidbody fore, Rigidbody hand, Vector3 goal, Quaternion bone)
		{
			if (!upper || !fore || !hand)
			{
				return Vector3.zero;
			}
			Vector3 sh = Shoulder(upper);
			float l1 = Vector3.Distance(upper.transform.position, fore.transform.position);
			float l2 = Vector3.Distance(fore.transform.position, hand.transform.position);
			Vector3 toGoal = goal - sh;
			float dist = toGoal.magnitude;
			if (dist < 1e-4f || l1 < 1e-4f)
			{
				return sh;
			}
			float reach = Mathf.Min(dist, (l1 + l2) * maxReach);
			Vector3 outward = Flat(sh - chest.transform.position);
			if (outward.sqrMagnitude < 1e-4f)
			{
				outward = Vector3.right;
			}
			Vector3 fwd = poseRead ? chest.transform.rotation * bindFwdLocal : Vector3.forward;
			Vector3 dir = toGoal / dist;
			float raised = 0f;
			if (naturalElbow)
			{
				raised = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(raisedFrom, raisedFull, dir.y));
			}
			Vector3 pole = Vector3.down + outward.normalized * Mathf.Lerp(elbowOut, elbowOutRaised, raised) - Flat(fwd).normalized * 0.15f;
			Vector3 stick = upper == leftUpperArm ? elbowStickL : elbowStickR;
			pole = Vector3.ProjectOnPlane(pole, dir);
			float handK = Mathf.Clamp01((elbowHandTop - dir.y) / Mathf.Max(elbowHandBand, 0.01f));
			if (centred && hand && handK > 0f)
			{
				// Let the hand's own orientation steer the elbow when the hand is held low.
				Vector3 shift = Vector3.zero;
				Vector3 fromHand = Vector3.ProjectOnPlane(goal - bone * Vector3.up * l2 - sh, dir);
				if (fromHand.sqrMagnitude > 4e-4f)
				{
					shift += fromHand.normalized * elbowFromHand * handK;
				}
				bool left = upper == leftUpperArm;
				Vector3 roll = (left ? -1f : 1f) * (bone * Vector3.right);
				Vector3 fromRoll = Vector3.ProjectOnPlane(Quaternion.AngleAxis(left ? -90f : 90f, dir) * roll, dir);
				if (fromRoll.sqrMagnitude > 0.01f)
				{
					shift += fromRoll.normalized * elbowFromRoll * handK;
				}
				float poleLen = pole.magnitude;
				if (poleLen > 1e-4f && wristPoleFix)
				{
					Vector3 n = pole / poleLen;
					float against = Vector3.Dot(shift, n);
					if (against < 0f)
					{
						shift -= n * against;
					}
					shift = Vector3.ClampMagnitude(shift, poleLen * wristPole);
				}
				pole += shift;
			}
			if (stick.sqrMagnitude > 1e-4f)
			{
				pole += Vector3.ProjectOnPlane(stick, dir) * ((1f - Mathf.Clamp01(pole.magnitude * 2f)) * elbowStick);
			}
			if (pole.sqrMagnitude < 0.01f)
			{
				pole = Vector3.ProjectOnPlane(outward.normalized, dir);
			}
			if (pole.sqrMagnitude < 0.01f)
			{
				pole = Vector3.ProjectOnPlane(stick, dir);
			}
			if (pole.sqrMagnitude < 0.01f)
			{
				pole = Vector3.Cross(dir, Vector3.up);
			}
			float along = (l1 * l1 - l2 * l2 + reach * reach) / (2f * reach);
			if (pole.sqrMagnitude < 1e-4f)
			{
				return sh + dir * along;
			}
			pole = pole.normalized;
			Vector3 side = Vector3.ProjectOnPlane(chest.transform.right * (upper == leftUpperArm ? -1f : 1f), dir);
			if (side.sqrMagnitude > 0.01f)
			{
				side.Normalize();
				float d = Vector3.Dot(pole, side);
				if (d < 0f)
				{
					// Never let the elbow point in across the body: mirror it back outside.
					pole = (pole - side * (2f * d)).normalized;
				}
			}
			if (stick.sqrMagnitude > 1e-4f)
			{
				Vector3 held = Vector3.ProjectOnPlane(stick, dir);
				if (held.sqrMagnitude > 1e-4f)
				{
					pole = Vector3.RotateTowards(held.normalized, pole, elbowTurn * Mathf.Deg2Rad * Time.fixedDeltaTime, 0f).normalized;
				}
			}
			float h = Mathf.Sqrt(Mathf.Max(l1 * l1 - along * along, 0f));
			if (upper == leftUpperArm)
			{
				elbowStickL = pole;
			}
			else
			{
				elbowStickR = pole;
			}
			return sh + dir * along + pole * h;
		}

		private void Unclash()
		{
			for (int i = 0; i < bodies.Length; i++)
			{
				if (!bodies[i])
				{
					continue;
				}
				Collider[] own = bodies[i].GetComponents<Collider>();
				for (int j = i + 1; j < bodies.Length; j++)
				{
					if (!bodies[j])
					{
						continue;
					}
					Collider[] theirs = bodies[j].GetComponents<Collider>();
					bool clash = Clashes(bodies[i], bodies[j]);
					foreach (Collider a in own)
					{
						foreach (Collider b in theirs)
						{
							Physics.IgnoreCollision(a, b, !clash);
						}
					}
				}
			}
		}

		private bool Clashes(Rigidbody a, Rigidbody b)
		{
			// Kind: 0 torso, 1 leg, 2 upper arm, 3 hand, 4 head, 5 forearm.
			int ka = Kind(a);
			int kb = Kind(b);
			bool noHead = ka != 4 && kb != 4;
			bool forearm = ka == 5 || kb == 5;
			int ga = ka == 4 ? 0 : (ka == 5 ? 2 : ka);
			int gb = kb == 4 ? 0 : (kb == 5 ? 2 : kb);
			if (ga == 2 || gb == 2)
			{
				// Arms collide with arms and hands; forearms (not upper arms) with the torso; never with head or legs.
				if (ga > 1 && gb > 1)
				{
					return true;
				}
				if (!noHead)
				{
					return false;
				}
				if (ga == 1 || gb == 1)
				{
					return false;
				}
				// Torso contact with the arms only counts on dummies, or when selfClash is on.
				return forearm && (dummy || selfClash);
			}
			if (ka == 3 || kb == 3)
			{
				// Hands collide with each other and the torso, not the head or legs.
				if (ga >= 2 && gb >= 2)
				{
					return true;
				}
				if (ga == 1 || gb == 1)
				{
					return false;
				}
				return noHead && (dummy || selfClash);
			}
			// Torso against torso only; legs collide with neither.
			return ga == 0 && gb == 0;
		}

		private int Kind(Rigidbody rb)
		{
			if (rb == head)
			{
				return 4;
			}
			if (rb == leftHand || rb == rightHand)
			{
				return 3;
			}
			if (rb == leftForearm || rb == rightForearm)
			{
				return 5;
			}
			if (rb == leftUpperArm || rb == rightUpperArm)
			{
				return 2;
			}
			if (rb == leftUpperLeg || rb == leftLowerLeg || rb == rightUpperLeg || rb == rightLowerLeg)
			{
				return 1;
			}
			return 0;
		}

		private void Slicken()
		{
			PhysicsMaterial feet = new PhysicsMaterial("RagdollFeet");
			feet.staticFriction = footGrip;
			feet.dynamicFriction = footGrip;
			feet.frictionCombine = PhysicsMaterialCombine.Minimum;
			PhysicsMaterial palms = new PhysicsMaterial("RagdollHands");
			palms.staticFriction = handGrip;
			palms.dynamicFriction = handGrip;
			palms.frictionCombine = PhysicsMaterialCombine.Maximum;
			foreach (Rigidbody hand in new Rigidbody[] { leftHand, rightHand })
			{
				if (!hand)
				{
					continue;
				}
				foreach (Collider c in hand.GetComponentsInChildren<Collider>())
				{
					c.sharedMaterial = palms;
				}
			}
			for (int i = 0; i < bodies.Length; i++)
			{
				if (!bodies[i] || bodies[i].name.IndexOf("leg", StringComparison.OrdinalIgnoreCase) < 0)
				{
					continue;
				}
				foreach (Collider c in bodies[i].GetComponents<Collider>())
				{
					c.sharedMaterial = feet;
				}
				bodies[i].angularDamping = legDamp;
			}
		}

		private static Vector3 Flat(Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		public static void IgnoreFingers(Collider c)
		{
			if (!c)
			{
				return;
			}
			foreach (Collider finger in fingers)
			{
				if (finger)
				{
					Physics.IgnoreCollision(c, finger, true);
				}
			}
		}

		[ContextMenu("1 Find finger bones (open = as posed now)")]
		private void FindBones()
		{
			// TODO(recon): editor-only Record/Dirty calls were inlined away in the build; placement inferred from the "find finger bones" literal (token 0x60004CF)
			Record("find finger bones");
			leftFingers.Clear();
			rightFingers.Clear();
			if (leftHand)
			{
				Collect(leftHand.transform, ".L", leftFingers);
			}
			if (rightHand)
			{
				Collect(rightHand.transform, ".R", rightFingers);
			}
			Dirty();
		}

		[ContextMenu("2 Capture CLOSED pose from the bones as posed now")]
		private void CaptureClosed()
		{
			// TODO(recon): editor-only Record/Dirty calls were inlined away in the build; placement inferred from the "capture closed pose" literal (token 0x60004D0)
			Record("capture closed pose");
			foreach (Digit d in leftFingers)
			{
				if (d.bone)
				{
					d.closed = d.bone.localRotation;
				}
			}
			foreach (Digit d in rightFingers)
			{
				if (d.bone)
				{
					d.closed = d.bone.localRotation;
				}
			}
			Dirty();
		}

		[ContextMenu("2b Capture OPEN pose from the bones as posed now")]
		private void CaptureOpen()
		{
			// TODO(recon): editor-only Record/Dirty calls were inlined away in the build; placement inferred from the "capture open pose" literal (token 0x60004D1)
			Record("capture open pose");
			foreach (Digit d in leftFingers)
			{
				if (d.bone)
				{
					d.open = d.bone.localRotation;
				}
			}
			foreach (Digit d in rightFingers)
			{
				if (d.bone)
				{
					d.open = d.bone.localRotation;
				}
			}
			Dirty();
		}

		[ContextMenu("3 Pose bones OPEN")]
		private void PoseOpen()
		{
			Pose(leftFingers, 0f, true);
			Pose(rightFingers, 0f, true);
		}

		[ContextMenu("3b Pose bones CLOSED")]
		private void PoseClosed()
		{
			Pose(leftFingers, 1f, true);
			Pose(rightFingers, 1f, true);
		}

		[ContextMenu("4 Mirror closed pose LEFT to RIGHT")]
		private void MirrorLeftToRight()
		{
			// TODO(recon): editor-only Record/Dirty calls were inlined away in the build; placement inferred from the "mirror closed pose" literal (token 0x60004D4)
			Record("mirror closed pose");
			foreach (Digit l in leftFingers)
			{
				if (!l.bone)
				{
					continue;
				}
				string other = l.bone.name.Replace(".L", ".R");
				foreach (Digit r in rightFingers)
				{
					if (!r.bone || r.bone.name != other)
					{
						continue;
					}
					// Mirror the left curl (open -> closed) across the hand's X axis.
					Quaternion curl = Quaternion.Inverse(l.open) * l.closed;
					r.closed = r.open * new Quaternion(curl.x, -curl.y, -curl.z, curl.w);
				}
			}
			Dirty();
		}

		private static void Collect(Transform hand, string side, List<Digit> into)
		{
			string[] glove = new string[] { "Glove001", "Glove002" };
			string[] thumb = new string[] { "Thumb001", "Thumb002", "Thumb003" };
			foreach (string name in glove)
			{
				Add(hand, name + side, false, into);
			}
			foreach (string name in thumb)
			{
				Add(hand, name + side, true, into);
			}
		}

		private static void Add(Transform hand, string name, bool thumb, List<Digit> into)
		{
			foreach (Transform t in hand.GetComponentsInChildren<Transform>(true))
			{
				if (t.name != name)
				{
					continue;
				}
				into.Add(new Digit
				{
					bone = t,
					thumb = thumb,
					open = t.localRotation,
					closed = t.localRotation
				});
				break;
			}
		}

		private void Record(string what)
		{
			// TODO(recon): body is empty in the player build; the editor-only undo recording (UnityEditor) was stripped (token 0x60004D7)
		}

		private void Dirty()
		{
			// TODO(recon): body is empty in the player build; the editor-only SetDirty (UnityEditor) was stripped (token 0x60004D8)
		}

		private void AwakeGlove()
		{
			if (dummy)
			{
				PinInertia(leftHand);
				PinInertia(rightHand);
				return;
			}
			fingers.Clear();
			AddColliders(leftFingers);
			AddColliders(rightFingers);
			PinInertia(leftHand);
			PinInertia(rightHand);
			foreach (Grabbable grabbable in UnityEngine.Object.FindObjectsByType<Grabbable>(FindObjectsSortMode.None))
			{
				foreach (Collider c in grabbable.GetComponentsInChildren<Collider>())
				{
					IgnoreFingers(c);
				}
			}
			foreach (Collider c in GetComponentsInChildren<Collider>())
			{
				if (c && !fingers.Contains(c))
				{
					IgnoreFingers(c);
				}
			}
			for (int i = 0; i < fingers.Count; i++)
			{
				for (int j = i + 1; j < fingers.Count; j++)
				{
					if (fingers[i] && fingers[j])
					{
						Physics.IgnoreCollision(fingers[i], fingers[j], true);
					}
				}
			}
		}

		private void PinInertia(Rigidbody hand)
		{
			if (!hand)
			{
				return;
			}
			// Measure the inertia of a capsule standing in for the gloved hand, far below the level, then copy it over.
			GameObject shape = new GameObject("MassShape");
			shape.transform.position = new Vector3(0f, -500f, 0f);
			Rigidbody body = shape.AddComponent<Rigidbody>();
			body.mass = hand.mass;
			CapsuleCollider capsule = shape.AddComponent<CapsuleCollider>();
			capsule.direction = 1;
			capsule.center = massCapsuleCenter;
			capsule.height = massCapsuleHeight;
			capsule.radius = massCapsuleRadius;
			body.ResetCenterOfMass();
			body.ResetInertiaTensor();
			Vector3 centre = body.centerOfMass;
			Vector3 tensor = body.inertiaTensor;
			Quaternion tensorRotation = body.inertiaTensorRotation;
			UnityEngine.Object.Destroy(shape);
			hand.automaticCenterOfMass = false;
			hand.automaticInertiaTensor = false;
			hand.centerOfMass = centre;
			hand.inertiaTensor = tensor;
			hand.inertiaTensorRotation = tensorRotation;
		}

		private void AddColliders(List<Digit> digits)
		{
			foreach (Digit digit in digits)
			{
				if (!digit.bone || digit.thumb)
				{
					continue;
				}
				if (digit.bone.GetComponent<Collider>())
				{
					continue;
				}
				Transform tip = digit.bone.childCount > 0 ? digit.bone.GetChild(0) : null;
				Vector3 local = tip ? digit.bone.InverseTransformPoint(tip.position) : new Vector3(0f, 0.04f, 0f);
				if (local.magnitude < 0.005f)
				{
					continue;
				}
				Vector3 dir = local.normalized;
				int axis;
				if (Mathf.Abs(dir.y) >= Mathf.Abs(dir.x) && Mathf.Abs(dir.y) >= Mathf.Abs(dir.z))
				{
					axis = 1;
				}
				else
				{
					axis = Mathf.Abs(dir.x) > Mathf.Abs(dir.z) ? 0 : 2;
				}
				CapsuleCollider capsule = digit.bone.gameObject.AddComponent<CapsuleCollider>();
				capsule.direction = axis;
				capsule.radius = mittenRadius;
				float length = local.magnitude + tipPad;
				capsule.height = length + mittenRadius;
				capsule.center = dir * ((length - mittenRadius) * 0.5f);
				fingers.Add(capsule);
			}
		}

		private void LateUpdateGlove()
		{
			float dt = Time.deltaTime;
			float wantGripL = 0f;
			float wantThumbL = 0f;
			float wantGripR = 0f;
			float wantThumbR = 0f;
			if (fist < 0f)
			{
				Read(ref devL, XRNode.LeftHand, out wantGripL, out wantThumbL);
				Read(ref devR, XRNode.RightHand, out wantGripR, out wantThumbR);
			}
			if (forceCurl >= 0f)
			{
				wantGripL = forceCurl;
				wantGripR = forceCurl;
			}
			if (fist >= 0f)
			{
				wantGripL = wantGripR = wantThumbL = wantThumbR = fist;
			}
			float step = dt * closeRate;
			gripL = Mathf.MoveTowards(gripL, wantGripL, step);
			gripR = Mathf.MoveTowards(gripR, wantGripR, step);
			thumbL = Mathf.MoveTowards(thumbL, wantThumbL, step);
			thumbR = Mathf.MoveTowards(thumbR, wantThumbR, step);
			Pose(leftFingers, gripL, thumbL, false);
			Pose(rightFingers, gripR, thumbR, false);
			if (shapeL && shapeL.activeInHierarchy)
			{
				Pose(ghostFingersL, gripL, thumbL, false);
			}
			if (shapeR && shapeR.activeInHierarchy)
			{
				Pose(ghostFingersR, gripR, thumbR, false);
			}
		}

		private static void Pose(List<Digit> digits, float curl, bool undo)
		{
			Pose(digits, curl, curl, false);
		}

		private static void Pose(List<Digit> digits, float fingers, float thumb, bool undo)
		{
			for (int i = 0; i < digits.Count; i++)
			{
				Digit digit = digits[i];
				if (digit.bone)
				{
					digit.bone.localRotation = Quaternion.Slerp(digit.open, digit.closed, digit.thumb ? thumb : fingers);
				}
			}
		}

		private static void Read(ref UnityEngine.XR.InputDevice device, XRNode node, out float grip, out float thumb)
		{
			grip = 0f;
			thumb = 0f;
			if (!device.isValid)
			{
				device = InputDevices.GetDeviceAtXRNode(node);
			}
			if (!device.isValid)
			{
				return;
			}
			if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.grip, out float g))
			{
				grip = g;
			}
			if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool pressed) && pressed)
			{
				thumb = 1f;
			}
		}

		private void AwakeFighter()
		{
			stars = gameObject.AddComponent<KnockStars>();
			stars.head = head ? head.transform : null;
			if (leftHand && rightHand && chest)
			{
				FistTrail trail = gameObject.AddComponent<FistTrail>();
				trail.fists = new Transform[] { leftHand.transform, rightHand.transform };
				trail.body = chest.transform;
				trail.grip = i => i == 0 ? gripL : gripR;
				trail.limp = Ragdolled;
				trail.onSwing = (i, fast) => SwingSound(i == 0 ? handL : handR, fast);
			}
			headCap = head ? head.GetComponent<CapsuleCollider>() : null;
			foreach (Rigidbody bone in bodies)
			{
				if (!bone)
				{
					continue;
				}
				Impact impact = bone.gameObject.AddComponent<Impact>();
				impact.owner = this;
				impact.bone = bone;
			}
		}

		private void FixedUpdateFighter()
		{
			if (knockedOut && Time.time >= knockedOutUntil && !carriedHold && !SlapDummy.Hauls(this))
			{
				knockedOut = false;
				riseDue = flopped;
				shieldOnRise = !dummy;
				autoRiseUntil = Time.time + 4f;
			}
			if (carriedHold)
			{
				handedOver = true;
			}
			else if ((!knockedOut && !flopped) || SlapDummy.Hauls(this))
			{
				handedOver = false;
			}
			limpArms = knockedOut || dead || limpWhenIdle || (limpOnRise && rising) || Time.time < looseUntil || (inVR && (away || (parkedL && parkedR)));
			stars.on = knockedOut && !dead;
			stars.dead = dead;
			damage = Mathf.MoveTowards(damage, 0f, damageDecay * Time.fixedDeltaTime);
			if (guardLeft < guardHits && Time.time > blockAt + guardRecover)
			{
				guardLeft = guardHits;
			}
			Vector3 at = headCap ? headCap.transform.TransformPoint(headCap.center) : head.worldCenterOfMass;
			guarding = !knockedOut && !dead && Guard(at, head.rotation, leftHand.position, rightHand.position);
		}

		public bool Guard(Vector3 head, Quaternion headRot, Vector3 handL, Vector3 handR)
		{
			Vector3 fwd = headRot * Vector3.forward;
			Vector3 flat = new Vector3(fwd.x, 0f, fwd.z);
			if (flat.sqrMagnitude < 0.0001f)
			{
				return false;
			}
			flat = flat.normalized;
			return Raised(handL - head, flat) && Raised(handR - head, flat);
		}

		private bool Raised(Vector3 d, Vector3 f)
		{
			if (d.y >= guardHigh || d.y <= -guardLow)
			{
				return false;
			}
			float ahead = Vector3.Dot(d, f);
			if (ahead <= 0.05f || ahead >= guardFront)
			{
				return false;
			}
			return Mathf.Abs(Vector3.Dot(d, Vector3.Cross(Vector3.up, f))) < guardSide;
		}

		public void ReviveFighter()
		{
			shieldUntil = -9f;
			shieldOnRise = false;
			knockedOut = false;
			knockedOutUntil = 0f;
			damage = 0f;
			koCount = 0;
			lastKOAt = -99f;
			guardLeft = guardHits;
			looseUntil = Time.time + reviveLoose;
		}

		public float Recovery()
		{
			return damageDecay;
		}

		public float Threshold()
		{
			float count = Time.time > lastKOAt + koForget ? 0f : koCount;
			return koDamage * Mathf.Max(koFloor, 1f - count * koWeaken);
		}

		private bool IsPlayer()
		{
			return !dummy;
		}

		private void OnDestroy()
		{
			if (!dummy)
			{
				return;
			}
			if (anchorHead)
			{
				Destroy(anchorHead.gameObject);
			}
			if (anchorHip)
			{
				Destroy(anchorHip.gameObject);
			}
			if (anchorL)
			{
				Destroy(anchorL.gameObject);
			}
			if (anchorR)
			{
				Destroy(anchorR.gameObject);
			}
		}

		private void Heft()
		{
			float k = TotalMass() / 65f;
			headSpring *= k;
			headDamp *= k;
			headForce *= k;
			headSpin *= k;
			headSpinDamp *= k;
			hipSpring *= k;
			hipDamp *= k;
			hipForce *= k;
			hipForceHold *= k;
			hipSide *= k;
			hipSideDamp *= k;
			hipSideForce *= k;
			hipSpin *= k;
			hipSpinDamp *= k;
			armForce *= k;
			legForce *= k;
			Transform tipL = Tip(leftLowerLeg.transform);
			Transform tipR = Tip(rightLowerLeg.transform);
			if (!tipL || !tipR)
			{
				return;
			}
			float reach = Vector3.Distance(leftUpperLeg.position, leftLowerLeg.position)
				+ Vector3.Distance(leftLowerLeg.position, tipL.position)
				+ Vector3.Distance(rightUpperLeg.position, rightLowerLeg.position)
				+ Vector3.Distance(rightLowerLeg.position, tipR.position);
			float scale = reach * 0.5f / LegLength;
			strideMax = scale * strideMax;
			stepHeight = scale * stepHeight;
			cadenceMax = Mathf.Min(cadenceMax / scale, 4.5f);
		}

		[ContextMenu("Knock Out")]
		private void KnockOutNow()
		{
			if (!Application.isPlaying || !chest)
			{
				return;
			}
			KnockOut(chest.position + chest.transform.forward, true);
		}

		public bool StandingLeg(Rigidbody rb)
		{
			if (!grounded || flopped || knockedOut || dead)
			{
				return false;
			}
			return rb == leftUpperLeg || rb == leftLowerLeg || rb == rightUpperLeg || rb == rightLowerLeg;
		}

		public bool IsArm(Rigidbody rb)
		{
			return rb == leftHand || rb == rightHand || rb == leftForearm || rb == rightForearm;
		}

		public Hit TakeImpact(Rigidbody bone, float impulse, float speed, Vector3 from, Rigidbody other = null)
		{
			if (!bone || knockedOut || dead || Shielded())
			{
				return Hit.None;
			}
			bool byBody = other && !other.isKinematic;
			if (!byBody)
			{
				if (dummy || dropping || IsArm(bone) || bigFall > speed)
				{
					return Hit.None;
				}
			}
			else if (AvatarBody.peaceful || AvatarBody.truce || dropping)
			{
				return Hit.None;
			}
			if (anyKO > impulse || anySpeed > speed || gripping || holding)
			{
				return Hit.None;
			}
			lastHit = "hard knock on " + bone.name + " " + impulse.ToString("F0") + " Ns at " + speed.ToString("F0") + " m/s"
				+ (byBody ? " by " + other.name : " (world" + (other ? " " + other.name : "") + ")");
			KnockOut(from, true);
			return Hit.KnockedOut;
		}

		public Hit TakeHit(Rigidbody bone, float impulse, float speed, Vector3 from, int verdict = -1, float at = -1f, int by = -1, bool kick = false, bool fist = false, bool touched = true, bool over = false)
		{
			bool ramp = fist && punchRamp;
			if (!bone || dead)
			{
				return Hit.None;
			}
			float k;
			if (!ramp)
			{
				if (slapFrom > speed)
				{
					return Hit.None;
				}
				k = Mathf.InverseLerp(slapFrom, minSpeed, speed);
			}
			else
			{
				if (speed < PunchFrom)
				{
					return Hit.None;
				}
				k = Mathf.InverseLerp(PunchFrom, PunchFull, speed);
			}
			if (knockedOut)
			{
				float t = at >= 0f ? at : Time.time;
				if (touched)
				{
					if (knockOwed && SameStrike(t, by))
					{
						Shove(t, by, float.MaxValue);
						PushAway(from, knockShove);
					}
					else
					{
						float shove = Shove(t, by, k);
						if (shove > 0f)
						{
							PushAway(from, shove * hurtShove);
						}
					}
					knockOwed = false;
				}
				lastHitFrom = by;
				lastHitAt = Mathf.Max(lastHitAt, t);
				return Hit.KnockedOut;
			}
			if (IsArm(bone))
			{
				if (verdict == 1)
				{
					Block(from);
					return Hit.Blocked;
				}
				damage = Mathf.Max(damage, Mathf.Min(damage + Hurt(false, true, impulse, speed, false, fist), Threshold() * softCap));
				lastHit = "blocked on " + bone.name + ", " + damage.ToString("F0") + "/" + Threshold().ToString("F0");
				return Hit.Blocked;
			}
			if (kick)
			{
				if (kickHard > speed)
				{
					float light = Hurt(bone == head, false, impulse, speed, true, false);
					float t = at >= 0f ? at : Time.time;
					float add;
					if (SameStrike(t, by))
					{
						add = Mathf.Max(light - lastHurt, 0f);
						lastHurt = Mathf.Max(light, lastHurt);
					}
					else
					{
						lastHurt = light;
						add = light;
					}
					lastHitFrom = by;
					lastHitAt = Mathf.Max(lastHitAt, t);
					damage = Mathf.Max(damage, Mathf.Min(damage + add, Threshold() * softCap));
					lastHit = "light kick on " + bone.name + ", " + damage.ToString("F0") + "/" + Threshold().ToString("F0");
					if (touched)
					{
						float shove = Shove(t, by, k);
						if (shove > 0f)
						{
							PushAway(from, shove * hurtShove);
						}
					}
					return Hit.Hurt;
				}
				if (verdict == 1)
				{
					Block(from);
					return Hit.Blocked;
				}
			}
			else
			{
				if (verdict == 1 || (guarding && guardLeft >= 1 && (verdict != 0 || WouldKnockOut(bone, impulse, speed, by, kick, fist))))
				{
					Block(from);
					return Hit.Blocked;
				}
				if (guarding && guardLeft >= 1 && minSpeed > speed)
				{
					lastHit = "light slap on a raised guard";
					return Hit.Blocked;
				}
			}
			float hurt = Hurt(bone == head, false, impulse, speed, kick, fist, over);
			if (!dummy)
			{
				Both(Mathf.Lerp(0.2f, 0.6f, hurt / Threshold()), 0.06f);
			}
			float when = at >= 0f ? at : Time.time;
			float pushed = touched ? Shove(when, by, k) : 0f;
			float dealt;
			if (SameStrike(when, by))
			{
				if (verdict != 2 && hurt - lastHurt <= 0f)
				{
					if (pushed > 0f)
					{
						PushAway(from, pushed * hurtShove);
					}
					return Hit.Hurt;
				}
				dealt = Mathf.Max(hurt - lastHurt, 0f);
				lastHurt = Mathf.Max(hurt, lastHurt);
			}
			else
			{
				lastHurt = hurt;
				dealt = hurt;
			}
			lastHitFrom = by;
			damage += dealt;
			lastHitAt = Mathf.Max(lastHitAt, when);
			if (minSpeed <= speed && !dummy)
			{
				Bump(dealt, from);
			}
			if (Shielded())
			{
				damage = Mathf.Min(damage, Threshold() * softCap);
			}
			bool spared;
			if (Shielded())
			{
				spared = true;
			}
			else if (verdict == 2 && damage >= Threshold() * koAgree)
			{
				spared = false;
			}
			else
			{
				spared = damage < Threshold();
			}
			lastHit = bone.name + " " + dealt.ToString("F0") + " dmg, " + damage.ToString("F0") + "/" + Threshold().ToString("F0");
			if (spared || knockedOut)
			{
				if (pushed > 0f)
				{
					PushAway(from, pushed * hurtShove);
				}
				return Hit.Hurt;
			}
			KnockOut(from, touched);
			knockOwed = !touched;
			if (touched)
			{
				Shove(when, by, float.MaxValue);
			}
			return Hit.KnockedOut;
		}

		public bool Shielded()
		{
			if (shieldOnRise && (Time.time < autoRiseUntil || rising))
			{
				return true;
			}
			return Time.time < shieldUntil;
		}

		private void OnApplicationFocus(bool has)
		{
			if (!Application.isEditor && Application.platform != RuntimePlatform.WindowsPlayer)
			{
				away = !has;
			}
		}

		private void OnApplicationPause(bool paused)
		{
			if (!Application.isEditor)
			{
				away = paused;
			}
		}

		public bool Brush(bool kick, float speed)
		{
			return kick && kickHard > speed;
		}

		public bool WouldKnockOut(Rigidbody bone, float impulse, float speed, int by = -1, bool kick = false, bool fist = false)
		{
			if (!bone || dead || knockedOut || IsArm(bone) || Shielded())
			{
				return false;
			}
			if (kick && kickHard > speed)
			{
				return false;
			}
			float hurt = Hurt(bone == head, false, impulse, speed, kick, fist);
			float add = Time.time < lastHitAt + HitDebounce && lastHitFrom == by ? hurt - lastHurt : hurt;
			if (add <= 0f)
			{
				return false;
			}
			return add + damage >= Threshold();
		}

		public float Hurt(bool onHead, bool onArm, float impulse, float speed, bool kick = false, bool fist = false, bool over = false)
		{
			float k = fist && punchRamp ? Mathf.InverseLerp(PunchFrom, PunchFull, speed) : Mathf.InverseLerp(slapFrom, kick ? kickHard : minSpeed, speed);
			float hit = over ? impulse : Mathf.Min(kick ? 60f : hitCap, impulse);
			return k * (hit * (onHead ? headMul : (onArm ? armMul : bodyMul)));
		}

		public Hit Block(Vector3 at)
		{
			if (guardLeft >= 1 && Time.time >= guardAt + blockCooldown)
			{
				guardLeft--;
				guardAt = Time.time;
			}
			blockAt = Time.time;
			lastHit = "guarded, " + guardLeft + " left";
			BlockAt(at);
			if (!dummy)
			{
				Both(0.3f, 0.05f);
			}
			return Hit.Blocked;
		}

		public void KnockOut(Vector3 from, bool shove = true)
		{
			if (!dummy)
			{
				Note("knocked out, last hit " + lastHit);
			}
			HatOff(from);
			if (Time.time > lastKOAt + koForget)
			{
				koCount = 0;
			}
			shieldOnRise = false;
			knockedOut = true;
			knockedOutUntil = Time.time + Mathf.Min(knockoutTime + koGrow * koCount++, koMax);
			lastKOAt = Time.time;
			damage = 0f;
			if (head)
			{
				KnockAt(head.position);
			}
			if (!dummy)
			{
				Both(1f, 0.4f);
			}
			LetGo("knocked out");
			riseLockedUntil = knockedOutUntil;
			Knock(shove ? Away(from, knockLift) * knockShove : Vector3.zero);
		}

		public bool SameStrike(float at, int by)
		{
			return Mathf.Abs(at - lastHitAt) < HitDebounce && lastHitFrom == by;
		}

		private float Shove(float at, int by, float ramp)
		{
			if (Mathf.Abs(at - shoveAt) < HitDebounce && shoveFrom == by)
			{
				float extra = Mathf.Max(ramp - shoveRamp, 0f);
				shoveRamp = Mathf.Max(shoveRamp, ramp);
				shoveAt = Mathf.Max(shoveAt, at);
				shoveFrom = by;
				return extra;
			}
			if (shoveAt > at)
			{
				return ramp;
			}
			shoveRamp = ramp;
			shoveAt = at;
			shoveFrom = by;
			return ramp;
		}

		public void Launch(Vector3 dir, float impulse, float speed = 0f, float lift = 0f)
		{
			if (dead || Shielded())
			{
				return;
			}
			Vector3 flat = new Vector3(dir.x, 0f, dir.z);
			if (flat.sqrMagnitude < 0.0001f)
			{
				Vector3 forward = chest.transform.forward;
				flat = new Vector3(-forward.x, 0f, -forward.z);
			}
			if (flat.sqrMagnitude < 0.0001f)
			{
				return;
			}
			flat = flat.normalized;
			float launch = speed > 0f ? speed : Mathf.Lerp(kickLaunch.x, kickLaunch.y, Mathf.Clamp01((impulse - 30f) / 30f));
			float rise = Mathf.Max(kickLift, lift);
			Note("dropkicked: launched " + launch.ToString("F1") + " m/s from a " + impulse.ToString("F0") + " Ns kick");
			Knock(Vector3.zero);
			foreach (Rigidbody body in bodies)
			{
				if (!body || body.isKinematic)
				{
					continue;
				}
				Vector3 v = body.linearVelocity;
				float along = Vector3.Dot(flat, v);
				if (along < launch)
				{
					v += flat * (launch - along);
				}
				body.linearVelocity = new Vector3(v.x, Mathf.Max(v.y, rise), v.z);
			}
		}

		private void PushAway(Vector3 from, float shove)
		{
			Vector3 dir = Away(from, knockLift);
			foreach (Rigidbody body in bodies)
			{
				if (body)
				{
					body.AddForce(dir * (shove / sturdy) * body.mass, ForceMode.Impulse);
				}
			}
		}

		private Vector3 Away(Vector3 from, float lift)
		{
			Vector3 away = chest.position - from;
			away.y = 0f;
			if (away.sqrMagnitude < 0.0001f)
			{
				away = -chest.transform.forward;
			}
			return (away.normalized + Vector3.up * lift).normalized;
		}

		public static Player Of(Component c)
		{
			if (!c)
			{
				return null;
			}
			Player player = c.GetComponentInParent<Player>();
			return player ? player : null;
		}

		private void AwakeKnockOutView()
		{
		}

		private void OnEnableKnockOutView()
		{
			RenderPipelineManager.beginCameraRendering += Only;
		}

		private void OnDisableKnockOutView()
		{
			RenderPipelineManager.beginCameraRendering -= Only;
		}

		private void Only(ScriptableRenderContext context, Camera c)
		{
			if (veil)
			{
				veil.forceRenderingOff = c != veilCam;
			}
		}

		private void Build()
		{
			veilCam = Camera.main;
			if (!veilCam)
			{
				return;
			}
			Shader shader = Shader.Find("Island/Vignette");
			if (!shader)
			{
				return;
			}
			GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			quad.name = "Blackout";
			Destroy(quad.GetComponent<Collider>());
			quad.transform.SetParent(veilCam.transform, false);
			quad.transform.localPosition = new Vector3(0f, 0f, veilCam.nearClipPlane + 0.012f);
			quad.transform.localRotation = Quaternion.identity;
			MeshFilter filter = quad.GetComponent<MeshFilter>();
			if (filter)
			{
				filter.mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 64f);
			}
			veil = quad.GetComponent<Renderer>();
			mat = new Material(shader);
			veil.sharedMaterial = mat;
			veil.shadowCastingMode = ShadowCastingMode.Off;
			veil.receiveShadows = false;
			veil.enabled = false;
			camData = veilCam.GetUniversalAdditionalCameraData();
			AudioListener listener = veilCam.GetComponent<AudioListener>();
			if (!listener)
			{
				listener = FindFirstObjectByType<AudioListener>();
			}
			if (!listener)
			{
				return;
			}
			muffle = listener.GetComponent<AudioLowPassFilter>();
			if (!muffle)
			{
				muffle = listener.gameObject.AddComponent<AudioLowPassFilter>();
			}
			muffle.cutoffFrequency = clearHz;
			muffle.enabled = false;
		}

		private void FallWind()
		{
			if (!fall || !veilCam || !pelvis)
			{
				return;
			}
			if (!fallSource)
			{
				GameObject go = new GameObject("Fall Wind");
				go.transform.SetParent(veilCam.transform, false);
				fallSource = go.AddComponent<AudioSource>();
				fallSource.playOnAwake = false;
				fallSource.spatialBlend = 0f;
				fallSource.loop = true;
				fallSource.clip = fall;
			}
			float down = dead ? 0f : -pelvis.linearVelocity.y;
			fallLevel = Mathf.MoveTowards(fallLevel, Mathf.Clamp01((down - fallFrom) / (fallFull - fallFrom)), Time.deltaTime / fallFade);
			fallSource.volume = fallLevel * (fallVolume * fallLevel) * Prefs.sfx;
			if (fallLevel > 0f && !fallSource.isPlaying)
			{
				fallSource.time = UnityEngine.Random.Range(0f, fall.length * 0.8f);
				fallSource.Play();
			}
			else if (fallLevel <= 0f && fallSource.isPlaying)
			{
				fallSource.Stop();
			}
		}

		private void StartDizzy()
		{
			if (!dizzy || !veilCam)
			{
				return;
			}
			if (!dizzySource)
			{
				GameObject go = new GameObject("Dizzy");
				go.transform.SetParent(veilCam.transform, false);
				dizzySource = go.AddComponent<AudioSource>();
				dizzySource.playOnAwake = false;
				dizzySource.spatialBlend = 0f;
				dizzySource.loop = true;
				dizzySource.bypassListenerEffects = true;
			}
			dizzySource.clip = dizzy;
			dizzyOn = true;
			dizzyLevel = 1f;
			dizzySource.volume = dizzyVolume * Prefs.sfx;
			if (!dizzySource.isPlaying)
			{
				dizzySource.Play();
			}
		}

		private void Dizzy()
		{
			if (!dizzySource || !dizzyOn)
			{
				return;
			}
			if (dead)
			{
				dizzyLevel = 0f;
			}
			else if (knockedOut || flopped || rising)
			{
				dizzyLevel = 1f;
			}
			else
			{
				dizzyLevel = Mathf.MoveTowards(dizzyLevel, 0f, Time.deltaTime / Mathf.Max(dizzyFade, 0.01f));
			}
			dizzySource.volume = dizzyVolume * dizzyLevel * Prefs.sfx;
			if (dizzyLevel <= 0f)
			{
				dizzyOn = false;
				dizzySource.Stop();
			}
		}

		private float EyeAspect()
		{
			if (XRSettings.enabled && XRSettings.eyeTextureHeight >= 1)
			{
				return (float)XRSettings.eyeTextureWidth / (float)XRSettings.eyeTextureHeight;
			}
			return veilCam ? veilCam.aspect : 1f;
		}

		private void LateUpdateKnockOutView()
		{
			if (!veil && Camera.main)
			{
				Build();
			}
			if (!veil)
			{
				return;
			}
			if (knockedOut && !wasOut)
			{
				dark = 1f;
				depth = 1f;
				downSince = Time.time;
				StartDizzy();
			}
			Dizzy();
			if (flopBlackAt > flopSeen)
			{
				flopSeen = flopBlackAt;
				if (!dead)
				{
					if (depth <= 0f)
					{
						downSince = Time.time;
					}
					depth = Mathf.Max(depth, flopDepth);
					dark = Mathf.Max(dark, flopDepth);
				}
			}
			if (knockedOut)
			{
				downSince = Time.time;
			}
			float want;
			bool clearing;
			if (depth > 0f && !dead && (knockedOut || flopped || rising) && Time.time < downSince + maxDown)
			{
				want = knockedOut ? 1f : depth * (1f - riseProgress);
				clearing = false;
			}
			else
			{
				if (dark < 0.002f)
				{
					depth = 0f;
				}
				want = 0f;
				clearing = true;
			}
			if (blackout > 0f || !clearing || dark < 0.002f)
			{
				blackShut = blackout > 0f;
			}
			float target = Mathf.Max(want, blackout);
			wasOut = knockedOut;
			if (!dead && wasDead)
			{
				dark = Mathf.Max(dark, deathBlink);
			}
			wasDead = dead;
			float rate;
			if (target > dark)
			{
				rate = blackShut && clearing ? blackIn : shutIn;
			}
			else
			{
				rate = clearing ? clearOut : follow;
			}
			dark = Mathf.MoveTowards(dark, Mathf.Clamp01(target), Time.deltaTime / Mathf.Max(rate, 0.01f));
			bool under = Water.main && veilCam && veilCam.transform.position.y < Water.main.Surface;
			bool show = dark > 0.002f || under;
			if (show != veil.enabled)
			{
				veil.enabled = show;
			}
			bool copy = false;
			if (show)
			{
				veil.transform.localPosition = new Vector3(0f, 0f, (veilCam.nearClipPlane + 0.012f) / Mathf.Max(veilCam.transform.lossyScale.z, 0.01f));
				float radius;
				if (under)
				{
					mat.SetColor(colorId, Color.Lerp(waterTint, new Color(0f, 0f, 0f, darkest), dark));
					radius = shutRadius;
				}
				else
				{
					mat.SetColor(colorId, new Color(0f, 0f, 0f, darkest));
					radius = Mathf.Lerp(veilClear, blackShut ? shutRadius : blurRadius, dark);
				}
				mat.SetFloat(radiusId, radius);
				mat.SetFloat(softId, under || blackShut ? soft : blurSoft);
				mat.SetFloat(blurId, under || blackShut ? 0f : blur * dark);
				mat.SetFloat(dimId, blurDim);
				mat.SetFloat(aspectId, EyeAspect());
				copy = !under && !blackShut;
			}
			if (camData && copying != copy)
			{
				copying = copy;
				camData.requiresColorOption = copy ? CameraOverrideOption.On : CameraOverrideOption.UsePipelineSettings;
			}
			if (!muffle)
			{
				return;
			}
			float muffled = Mathf.Max(dark, under ? waterMuffle : 0f);
			if (show != muffle.enabled)
			{
				muffle.enabled = show;
			}
			if (show)
			{
				muffle.cutoffFrequency = Mathf.Lerp(clearHz, muffledHz, muffled);
			}
		}

		private void AwakeSounds()
		{
			footL = Speaker(leftLowerLeg);
			footR = Speaker(rightLowerLeg);
			handL = Speaker(leftHand);
			handR = Speaker(rightHand);
			mouth = new Babble
			{
				source = Speaker(head)
			};
			hipSource = Speaker(pelvis);
			if (!dummy)
			{
				main = this;
			}
			Listen(leftHand, handL);
			Listen(rightHand, handR);
		}

		public AudioSource SpeakerOn(Transform on)
		{
			if (!on)
			{
				return null;
			}
			AudioSource source = on.gameObject.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.spatialBlend = 1f;
			source.rolloffMode = AudioRolloffMode.Linear;
			source.minDistance = near;
			source.maxDistance = far;
			Prefs.Track(source);
			return source;
		}

		private AudioSource Speaker(Rigidbody on)
		{
			if (!on)
			{
				return null;
			}
			AudioSource source = on.gameObject.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.spatialBlend = 1f;
			source.rolloffMode = AudioRolloffMode.Linear;
			source.minDistance = near;
			source.maxDistance = far;
			return source;
		}

		private void Listen(Rigidbody on, AudioSource through)
		{
			if (!on)
			{
				return;
			}
			Smack smack = on.gameObject.AddComponent<Smack>();
			smack.owner = this;
			smack.through = through;
		}

		private void FixedUpdateSounds()
		{
			if (!dummy && playSpace && leftHand && rightHand && pelvis)
			{
				mouth.Tick(talking && Prefs.myBlabber, blabbers, blabberVolume);
			}
			bool left = plantedL;
			bool right = plantedR;
			if (grounded && !flopped)
			{
				if (left && !wasL)
				{
					PlayStep(footL);
				}
				if (right && !wasR)
				{
					PlayStep(footR);
				}
			}
			wasL = left;
			wasR = right;
		}

		public bool Ragdolled()
		{
			return flopped || knockedOut || dead;
		}

		public void SwingSound(AudioSource at, float speed)
		{
			if (!at || swings == null || swings.Length == 0)
			{
				return;
			}
			at.pitch = UnityEngine.Random.Range(0.9f, 1.15f);
			AudioClip clip = Pick(swings, ref lastSwing);
			at.PlayOneShot(clip, swingVolume * Mathf.Lerp(0.5f, 1f, Mathf.InverseLerp(whooshFrom, whooshFull, speed)));
		}

		public void PlayJump(AudioSource at)
		{
			if (!at || jumps == null || jumps.Length == 0)
			{
				return;
			}
			at.pitch = UnityEngine.Random.Range(0.92f, 1.1f);
			AudioClip clip = Pick(jumps, ref lastJump);
			at.PlayOneShot(clip, jumpVolume);
		}

		public static bool Blabbering()
		{
			return !Prefs.realVoice || !voiceLive;
		}

		public static bool VoiceReady()
		{
			return voiceLive;
		}

		public static bool MicAllowed()
		{
			bool allowed = UnityEngine.Android.Permission.HasUserAuthorizedPermission("android.permission.RECORD_AUDIO");
			if (!allowed)
			{
				if (!micAsked)
				{
					UnityEngine.Android.Permission.RequestUserPermission("android.permission.RECORD_AUDIO");
				}
				micAsked = true;
			}
			return allowed;
		}

		private void Mic()
		{
			if (dummy || !playSpace)
			{
				return;
			}
			if (Prefs.mic && (!voiceLive || Prefs.myBlabber))
			{
				if (!micClip)
				{
					StartMic();
				}
				else
				{
					Hear();
				}
				ownMic = micClip;
			}
			else
			{
				if (micClip)
				{
					Microphone.End(micName);
				}
				micClip = null;
				ownMic = false;
				Heard(Prefs.mic ? sharedMic : 0f);
				if (!Prefs.mic)
				{
					talking = false;
					Started();
					return;
				}
			}
			talking = Time.time < talkUntil;
			Started();
		}

		private void Heard(float level)
		{
			micLevel = level;
			loudFor = talkLevel < level ? loudFor + Time.deltaTime : 0f;
			if (loudFor > talkStart)
			{
				talkUntil = Time.time + talkHold;
			}
		}

		private void Started()
		{
			if (talking && !wasTalking)
			{
				Note("talking, mic level " + micLevel.ToString("F3") + (voiceLive ? " (voice chat)" : " (own mic)"));
			}
			wasTalking = talking;
		}

		private void StartMic()
		{
			if (!MicAllowed())
			{
				return;
			}
			if (Microphone.devices.Length == 0)
			{
				return;
			}
			micName = Microphone.devices[0];
			micClip = Microphone.Start(micName, true, 1, 16000);
		}

		private void Hear()
		{
			int position = Microphone.GetPosition(micName);
			if (position < micBuf.Length)
			{
				return;
			}
			micClip.GetData(micBuf, position - micBuf.Length);
			float sum = 0f;
			for (int i = 0; i < micBuf.Length; i++)
			{
				sum += micBuf[i] * micBuf[i];
			}
			Heard(Mathf.Sqrt(sum / micBuf.Length));
		}

		private void PlayStep(AudioSource a)
		{
			if (!a || steps == null || steps.Length == 0)
			{
				return;
			}
			if (Time.time < nextStep)
			{
				return;
			}
			nextStep = Time.time + stepGap;
			a.pitch = 1f + UnityEngine.Random.Range(-stepPitch, stepPitch);
			AudioClip clip = Pick(steps, ref lastStep);
			a.PlayOneShot(clip, stepVolume);
		}

		public void PlayHit(AudioSource a, float speed, float impulse, bool body)
		{
			AudioClip[] clips = body && punches != null && punches.Length != 0 ? punches : smacks;
			if (!a || clips == null || clips.Length == 0)
			{
				return;
			}
			if (Time.time < nextSmack || smackFrom > speed)
			{
				return;
			}
			nextSmack = Time.time + smackGap;
			float heavy = Mathf.InverseLerp(heavyFrom, heavyFull, impulse);
			float hard = Mathf.InverseLerp(smackFrom, smackFull, speed);
			float volume = (clips == punches ? punchVolume : smackVolume) * Mathf.Lerp(smackFloor, 1f, hard);
			a.pitch = 1f - heavy * heavyPitch + UnityEngine.Random.Range(-smackPitch, smackPitch);
			a.PlayOneShot(Pick(clips, ref lastSmack), volume);
			if (heavy > 0f)
			{
				a.PlayOneShot(Pick(clips, ref lastSmack), heavy * volume);
			}
		}

		public void LandPunch(float speed)
		{
			if (tireFrom > speed || Time.time < lastLandedAt + tireGap)
			{
				return;
			}
			lastLandedAt = Time.time;
			float was = armsTired;
			armsTired = Mathf.Min(armsTired + Mathf.Lerp(0.6f, 1.4f, Mathf.InverseLerp(3f, 8f, speed)) * tirePerHit, 1f);
			if (was <= tireStart && armsTired > tireStart && !dummy)
			{
				Note("arms tired from punching");
			}
		}

		private void TrackSwing()
		{
			if (!playSpace || !controllerL || !controllerR)
			{
				return;
			}
			Vector3 l = playSpace.InverseTransformPoint(controllerL.position);
			Vector3 r = playSpace.InverseTransformPoint(controllerR.position);
			float gap = swingSeen > 0 ? Time.time - swingTimes[swingAt] : 0f;
			float dt = Mathf.Max(gap, Time.fixedDeltaTime);
			if (swingSeen > 0)
			{
				float jump = dt * SwingJump;
				if ((l - swingPosL[swingAt]).sqrMagnitude > jump * jump || (r - swingPosR[swingAt]).sqrMagnitude > jump * jump)
				{
					swingSeen = 0;
				}
			}
			swingAt = (swingAt + 1) % swingTimes.Length;
			swingTimes[swingAt] = Time.time;
			swingPosL[swingAt] = l;
			swingPosR[swingAt] = r;
			swingSeen++;
		}

		private float DeskSwing(Vector3[] ring, int steps)
		{
			int count = Mathf.Min(swingSeen, swingTimes.Length);
			if (count < 2)
			{
				return 0f;
			}
			float window = Time.fixedDeltaTime * (steps - 1);
			float best = 0f;
			float newest = swingTimes[swingAt];
			int len = swingTimes.Length;
			for (int i = 0; i < count - 1; i++)
			{
				int a = (len - i + swingAt) % len;
				if (newest - swingTimes[a] > window + 1e-4f)
				{
					break;
				}
				int j = i + 1;
				while (j < count - 1 && swingTimes[a] - swingTimes[(len - j + swingAt) % len] < SwingWindow)
				{
					j++;
				}
				int b = (len - j + swingAt) % len;
				float dt = swingTimes[a] - swingTimes[b];
				if (dt > 0f)
				{
					best = Mathf.Max(best, (ring[a] - ring[b]).magnitude / dt);
				}
			}
			return best;
		}

		public float SwingSpeed(Rigidbody hand, int steps)
		{
			if (!hand)
			{
				return 0f;
			}
			bool left = hand == leftHand;
			if (!left && hand != rightHand)
			{
				return 0f;
			}
			HandGrab grab = left ? grabL : grabR;
			if (grab)
			{
				float speed = grab.Swing(steps);
				if (speed >= 0f)
				{
					return speed;
				}
			}
			return DeskSwing(left ? swingPosL : swingPosR, steps);
		}

		private float CatchRoom(Rigidbody hand, float room)
		{
			if (!fastHands)
			{
				return room;
			}
			float speed = SwingSpeed(hand, 1);
			return Mathf.Min((speed * catchK + catchBase) * Time.fixedDeltaTime, room);
		}

		private float EaseK(Rigidbody hand)
		{
			float ease = HandEase();
			if (fastHands)
			{
				float speed = SwingSpeed(hand, 1);
				ease = Mathf.Lerp(ease, ease + fastEase - handEase, Mathf.InverseLerp(easeSlowAt, easeFastAt, speed));
			}
			return ease > 0f ? 1f - Mathf.Exp(-Time.fixedDeltaTime / ease) : 1f;
		}

		private float HandEase()
		{
			return Mathf.Lerp(handEase, tiredEase, Mathf.InverseLerp(tireStart, 1f, armsTired));
		}

		private void RestArms()
		{
			if (armsTired <= 0f || Time.time < lastLandedAt + tireRest)
			{
				return;
			}
			armsTired = Mathf.MoveTowards(armsTired, 0f, tireRecover * Time.fixedDeltaTime);
		}

		public void Punch(Rigidbody by, float speed, float impulse)
		{
			LandPunch(speed);
			if (speed >= minSpeed)
			{
				PlayHit(by == leftHand ? handL : handR, speed, impulse, true);
			}
		}

		public void PunchAt(Vector3 at)
		{
			if (punches != null && punches.Length != 0)
			{
				AudioSource.PlayClipAtPoint(Pick(punches, ref lastSmack), at, punchVolume * Prefs.sfx);
			}
		}

		public void Knockout(Vector3 at)
		{
			if (knockout)
			{
				AudioSource.PlayClipAtPoint(knockout, at, knockVolume * Prefs.sfx);
			}
		}

		public void PlayBlock(Vector3 at)
		{
			if (block)
			{
				AudioSource.PlayClipAtPoint(block, at, blockVolume * Prefs.sfx);
			}
		}

		private AudioClip Pick(AudioClip[] from, ref int last)
		{
			if (from.Length == 1)
			{
				return from[0];
			}
			int i = UnityEngine.Random.Range(0, from.Length);
			if (i == last)
			{
				i = (i + 1) % from.Length;
			}
			last = i;
			return from[i];
		}

		public bool IsMine(Transform t)
		{
			if (!t)
			{
				return false;
			}
			return t.IsChildOf(transform);
		}

		private void StartOutfit()
		{
			Prefs.Load();
			if (cosmetics.Move(Prefs.outfit))
			{
				Prefs.Save();
			}
			for (int i = 0; i < Cosmetics.Slots.Length; i++)
			{
				cosmetics.Wear(i, Prefs.outfit[i]);
			}
			NetworkManager network = UnityEngine.Object.FindFirstObjectByType<NetworkManager>();
			Color[] colors = network && network.networkPlayerPrefab ? network.networkPlayerPrefab.colors : null;
			int paint;
			int team = dummy ? 0 : GameManager.TeamOf(GameManager.MyId);
			if (team > 0)
			{
				paint = GameManager.TeamPaint(team);
				if (colors == null)
				{
					return;
				}
			}
			else
			{
				if (colors == null)
				{
					return;
				}
				paint = Prefs.color;
			}
			if (colors.Length == 0)
			{
				return;
			}
			Transform body = transform;
			int index = paint >= 1 && paint <= colors.Length ? paint - 1 : UnityEngine.Random.Range(0, colors.Length);
			cosmetics.Tint(body, colors[index]);
		}

		private SkinnedMeshRenderer BodySkin()
		{
			foreach (SkinnedMeshRenderer skin in GetComponentsInChildren<SkinnedMeshRenderer>(true))
			{
				if (!cosmetics.Owns(skin.transform))
				{
					return skin;
				}
			}
			return null;
		}

		public void Wear(int slot, string item)
		{
			cosmetics.Wear(slot, item);
			if (slot == 0)
			{
				hatHits = 0;
			}
		}

		public bool HatOn(string item)
		{
			if (cosmetics.worn[0] != "")
			{
				return false;
			}
			if (!cosmetics.Has(0, item))
			{
				return false;
			}
			Wear(0, item);
			Note("put on a " + item);
			return true;
		}

		public void HatOff(Vector3 from)
		{
			if (cosmetics.worn[0] == "" || !head || dummy)
			{
				return;
			}
			string hat = cosmetics.worn[0];
			Vector3 away = head.position - from;
			away.y = 0f;
			Vector3 dir = away.sqrMagnitude > 1e-4f ? away.normalized : -head.transform.forward;
			Vector3 at = head.position;
			Quaternion rot = head.rotation;
			Vector3 velocity = dir * 2.5f + head.linearVelocity + Vector3.up * 2.5f;
			Wear(0, "");
			LooseHat.Drop(hat, at, rot, velocity, this);
			Note("knocked off the " + hat);
		}

		private void Bump(float hurt, Vector3 from)
		{
			if (cosmetics.worn[0] != "")
			{
				hatHits++;
				if (hurt >= hatKnockDamage || hatHits >= hatKnockHits)
				{
					HatOff(from);
				}
			}
		}

		private void StartHeadHide()
		{
			full = BodySkin();
			if (!full || !head)
			{
				return;
			}
			int headBone = Array.IndexOf(full.bones, head.transform);
			if (headBone < 0)
			{
				return;
			}
			headless = EyeCopy(new int[] { headBone }, "Headless");
			List<int> notArms = new List<int>();
			for (int i = 0; i < full.bones.Length; i++)
			{
				Transform bone = full.bones[i];
				if (bone && ((leftUpperArm && bone.IsChildOf(leftUpperArm.transform)) || (rightUpperArm && bone.IsChildOf(rightUpperArm.transform))))
				{
					continue;
				}
				notArms.Add(i);
			}
			if (leftUpperArm && rightUpperArm)
			{
				armsOnly = EyeCopy(notArms.ToArray(), "Arms");
			}
			List<Renderer> worn = new List<Renderer>();
			for (int slot = 1; slot < 3; slot++)
			{
				foreach (Cosmetics.Item item in cosmetics.List(slot))
				{
					foreach (GameObject part in item.parts)
					{
						if (part)
						{
							worn.AddRange(part.GetComponentsInChildren<Renderer>(true));
						}
					}
				}
			}
			wearing = worn.ToArray();
			if (leftUpperArm && rightUpperArm)
			{
				foreach (Renderer renderer in wearing)
				{
					SkinnedMeshRenderer skin = renderer as SkinnedMeshRenderer;
					if (!skin || !skin.sharedMesh)
					{
						continue;
					}
					List<int> cut = new List<int>();
					for (int i = 0; i < skin.bones.Length; i++)
					{
						Transform bone = skin.bones[i];
						if (bone && (bone.IsChildOf(leftUpperArm.transform) || bone.IsChildOf(rightUpperArm.transform)))
						{
							continue;
						}
						cut.Add(i);
					}
					if (cut.Count == skin.bones.Length)
					{
						continue;
					}
					Mesh mesh = Headless(skin.sharedMesh, cut.ToArray(), "Sleeves");
					int indices = 0;
					for (int s = 0; s < mesh.subMeshCount; s++)
					{
						indices += (int)mesh.GetIndexCount(s);
					}
					if (indices == 0)
					{
						continue;
					}
					GameObject copy = Instantiate(skin.gameObject, full.transform.parent);
					copy.name = skin.gameObject.name + " Sleeves";
					copy.SetActive(true);
					SkinnedMeshRenderer sleeve = copy.GetComponent<SkinnedMeshRenderer>();
					sleeve.sharedMesh = mesh;
					sleeve.shadowCastingMode = ShadowCastingMode.Off;
					sleeve.updateWhenOffscreen = true;
					sleeve.enabled = false;
					sleeves.Add(sleeve);
					sleeveOf.Add(skin);
				}
			}
			onHead = head.GetComponentsInChildren<MeshRenderer>(true);
			eye = playSpace && cam ? cam.GetComponent<Camera>() : Camera.main;
		}

		private void OnEnableHeadHide()
		{
			RenderPipelineManager.beginCameraRendering += Before;
			RenderPipelineManager.endCameraRendering += After;
		}

		private void OnDisableHeadHide()
		{
			RenderPipelineManager.beginCameraRendering -= Before;
			RenderPipelineManager.endCameraRendering -= After;
		}

		private void Before(ScriptableRenderContext context, Camera cam)
		{
			if (!headless || vanished)
			{
				return;
			}
			bool mine = cam.cameraType == CameraType.Game && (cam == eye || cam == LivCamera.firstPerson || (!eye && cam == Camera.main));
			if (!mine)
			{
				otherRenders++;
				return;
			}
			localRenders++;
			float dist = Vector3.Distance(cam.transform.position, head.worldCenterOfMass);
			hidden = dist < (hidden ? showBeyond : hideWithin);
			if (!hidden)
			{
				return;
			}
			Vector3 off = head.worldCenterOfMass - cam.transform.position;
			bodyHidden = Mathf.Sqrt(off.x * off.x + off.z * off.z) > (bodyHidden ? bodyBack : bodyAway);
			if (bodyHidden && armsOnly)
			{
				armsOnly.enabled = true;
				foreach (Renderer renderer in wearing)
				{
					if (renderer)
					{
						renderer.forceRenderingOff = true;
					}
				}
				if (sleeveBlock == null)
				{
					sleeveBlock = new MaterialPropertyBlock();
				}
				for (int i = 0; i < sleeves.Count; i++)
				{
					SkinnedMeshRenderer source = sleeveOf[i];
					SkinnedMeshRenderer sleeve = sleeves[i];
					if (!source || !sleeve || !source.enabled || !source.gameObject.activeInHierarchy)
					{
						continue;
					}
					Material[] materials = source.sharedMaterials;
					sleeve.sharedMaterials = materials;
					for (int m = 0; m < materials.Length; m++)
					{
						source.GetPropertyBlock(sleeveBlock, m);
						sleeve.SetPropertyBlock(sleeveBlock, m);
					}
					sleeve.enabled = true;
				}
			}
			else
			{
				headless.enabled = true;
			}
			full.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
			foreach (MeshRenderer renderer in onHead)
			{
				if (renderer)
				{
					renderer.forceRenderingOff = true;
				}
			}
		}

		private void After(ScriptableRenderContext context, Camera cam)
		{
			if (!headless)
			{
				return;
			}
			if (!headless.enabled && !(armsOnly && armsOnly.enabled))
			{
				return;
			}
			headless.enabled = false;
			if (armsOnly && armsOnly.enabled)
			{
				armsOnly.enabled = false;
				foreach (Renderer renderer in wearing)
				{
					if (renderer)
					{
						renderer.forceRenderingOff = false;
					}
				}
				foreach (SkinnedMeshRenderer sleeve in sleeves)
				{
					if (sleeve)
					{
						sleeve.enabled = false;
					}
				}
			}
			full.shadowCastingMode = ShadowCastingMode.On;
			foreach (MeshRenderer renderer in onHead)
			{
				if (renderer)
				{
					renderer.forceRenderingOff = false;
				}
			}
		}

		private SkinnedMeshRenderer EyeCopy(int[] cut, string label)
		{
			GameObject copy = Instantiate(full.gameObject, full.transform.parent);
			copy.name = full.gameObject.name + " " + label;
			SkinnedMeshRenderer skin = copy.GetComponent<SkinnedMeshRenderer>();
			skin.sharedMesh = Headless(Cosmetics.Whole(full.sharedMesh), cut, label);
			skin.shadowCastingMode = ShadowCastingMode.Off;
			skin.enabled = false;
			return skin;
		}

		private Mesh Headless(Mesh source, int[] cut, string label)
		{
			Mesh mesh = Instantiate(source);
			mesh.name = source.name + " " + label;
			BoneWeight[] weights = source.boneWeights;
			bool[] gone = new bool[weights.Length];
			for (int i = 0; i < weights.Length; i++)
			{
				BoneWeight w = weights[i];
				float w0 = Array.IndexOf(cut, w.boneIndex0) >= 0 ? w.weight0 : 0f;
				float w1 = Array.IndexOf(cut, w.boneIndex1) >= 0 ? w.weight1 : 0f;
				float w2 = Array.IndexOf(cut, w.boneIndex2) >= 0 ? w.weight2 : 0f;
				float w3 = Array.IndexOf(cut, w.boneIndex3) >= 0 ? w.weight3 : 0f;
				gone[i] = w0 + w1 + w2 + w3 > headWeight;
			}
			List<int> kept = new List<int>();
			for (int s = 0; s < source.subMeshCount; s++)
			{
				int[] triangles = source.GetTriangles(s);
				kept.Clear();
				for (int t = 0; t < triangles.Length; t += 3)
				{
					if (gone[triangles[t]] || gone[triangles[t + 1]] || gone[triangles[t + 2]])
					{
						cutTriangles++;
						continue;
					}
					kept.Add(triangles[t]);
					kept.Add(triangles[t + 1]);
					kept.Add(triangles[t + 2]);
				}
				mesh.SetTriangles(kept, s);
			}
			return mesh;
		}

		private void StartBall()
		{
			if (!pelvis)
			{
				return;
			}
			Vector3 at = pelvis.position;
			float floor;
			if (Physics.Raycast(at, Vector3.down, out RaycastHit hit, 3f, -1, QueryTriggerInteraction.Ignore) && !hit.collider.transform.IsChildOf(transform))
			{
				floor = hit.point.y;
			}
			else
			{
				floor = at.y + -0.45f;
			}
			if (playSpace)
			{
				floor = playSpace.position.y - sink;
			}
			float total = TotalMass();
			sag = total * 9.81f / ballSpring;
			rest = sag + (at.y - floor - radius);
			GameObject ball = new GameObject("Ball");
			ball.transform.SetParent(transform, false);
			ball.transform.position = new Vector3(at.x, floor + radius, at.z);
			ball.layer = pelvis.gameObject.layer;
			col = ball.AddComponent<SphereCollider>();
			col.radius = radius;
			PhysicsMaterial feet = new PhysicsMaterial("BallFeet");
			feet.staticFriction = grip;
			feet.dynamicFriction = grip;
			feet.frictionCombine = PhysicsMaterialCombine.Minimum;
			col.sharedMaterial = feet;
			rb = ball.AddComponent<Rigidbody>();
			rb.mass = mass;
			rb.interpolation = RigidbodyInterpolation.Interpolate;
			rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
			rb.maxAngularVelocity = maxSpeed / radius + 2f;
			rb.maxDepenetrationVelocity = 1f;
			touch = ball.AddComponent<BallTouch>();
			foreach (Player player in UnityEngine.Object.FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			{
				foreach (Collider c in player.GetComponentsInChildren<Collider>(true))
				{
					if (c != col)
					{
						Physics.IgnoreCollision(col, c, true);
					}
				}
			}
			joint = pelvis.gameObject.AddComponent<ConfigurableJoint>();
			joint.connectedBody = rb;
			joint.autoConfigureConnectedAnchor = false;
			joint.anchor = Vector3.zero;
			joint.connectedAnchor = Vector3.zero;
			joint.xMotion = ConfigurableJointMotion.Locked;
			joint.zMotion = ConfigurableJointMotion.Locked;
			joint.yMotion = ConfigurableJointMotion.Free;
			joint.angularXMotion = ConfigurableJointMotion.Free;
			joint.angularYMotion = ConfigurableJointMotion.Free;
			joint.angularZMotion = ConfigurableJointMotion.Free;
			joint.yDrive = new JointDrive
			{
				positionSpring = ballSpring,
				positionDamper = damp,
				maximumForce = float.MaxValue
			};
			joint.targetPosition = new Vector3(0f, -rest, 0f);
			joint.enableCollision = false;
			ballMode = true;
		}

		private void OnDisableBall()
		{
			ballMode = false;
			if (joint)
			{
				UnityEngine.Object.Destroy(joint);
			}
			if (rb)
			{
				UnityEngine.Object.Destroy(rb.gameObject);
			}
			joint = null;
			rb = null;
		}

		private void FixedUpdateBall()
		{
			if (!rb || !joint)
			{
				return;
			}
			float hipY = pelvis.position.y;
			bool low = true;
			bool ledge = false;
			if (holding)
			{
				low = !(Mathf.Max(leftHand.position.y, rightHand.position.y) > hipY + 0.2f);
				ledge = ledgeL || ledgeR;
			}
			bool off = flopped || rising || hanging || gripping || ledge;
			if (passive != off && (off || BallClear()))
			{
				Passive(off);
			}
			ballMode = !passive;
			if (passive)
			{
				ballSquat = 0f;
				float tucked = hanging ? 1f - legTuck * crouch : 1f;
				rb.MovePosition(new Vector3(pelvis.position.x, pelvis.position.y - Drop() * tucked, pelvis.position.z));
				return;
			}
			ballSquat = squat;
			RaycastHit hit;
			ballGrounded = Physics.SphereCast(rb.position, radius * 0.9f, Vector3.down, out hit, radius * 0.25f, -1, QueryTriggerInteraction.Ignore) && !hit.collider.transform.IsChildOf(transform) && hit.normal.y >= SteepY;
			bool high = !low && hipY - floorY > rest + radius + 0.05f;
			float room = Mathf.Max(rest - tuck * crouch - (high ? vaultTuck : 0f) - 0.05f, 0f);
			squat = Mathf.Min(Mathf.Max(chargeSquat * Mathf.Clamp01(jumpCharge * 2.5f), stickCrouch * CrouchDepth), room);
			legs = Mathf.Max(rest - tuck * crouch - (high ? vaultTuck : 0f) - squat, Mathf.Clamp01(crouch) * (crouchLow - radius + sag - 0.05f) + 0.05f);
			joint.targetPosition = new Vector3(0f, -legs, 0f);
			if (jumped && !wasJumped)
			{
				rb.AddForce(Vector3.up * (jumpPower * mass * jumpShare), ForceMode.Impulse);
			}
			wasJumped = jumped;
			if (!ballGrounded || slipping)
			{
				return;
			}
			Vector3 hips = pelvis.position;
			Vector3 goal = hipHome + leadHips;
			Vector3 move = playSpace ? Flat(moveNow) : Vector3.zero;
			Vector3 floorVel = FloorVelocity(hit.rigidbody, hit.point);
			Vector3 want = Vector3.ClampMagnitude(move + Flat(goal - hips) * chase, maxSpeed * pumpNow);
			Vector3 vel = rb.linearVelocity;
			float brace = Mathf.Max(braceNow, 1f);
			Vector3 goalVel = floorVel + want;
			if (!touch.wall)
			{
				Vector3 roll = Vector3.Cross(Vector3.up, goalVel - floorVel) / radius;
				rb.AddTorque(Vector3.ClampMagnitude((roll - rb.angularVelocity) * (brace * spin), brace * maxTorque), ForceMode.Force);
			}
			float drive = (slipping ? 0f : traction) * push;
			rb.AddForce(Vector3.ClampMagnitude((goalVel - Flat(vel)) * (brace * drive), brace * maxPush), ForceMode.Force);
		}

		public void Snap()
		{
			if (!rb || !pelvis)
			{
				return;
			}
			rb.position = new Vector3(pelvis.position.x, BallY(), pelvis.position.z);
			rb.linearVelocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}

		private float Drop()
		{
			return rest - sag;
		}

		private float BallY()
		{
			float hang = pelvis.position.y - Drop();
			float floor = floorY + radius;
			if (grounded && floor - hang < 0.1f && hang < floor)
			{
				return floor;
			}
			return hang;
		}

		private bool BallClear()
		{
			int n = Physics.OverlapSphereNonAlloc(new Vector3(pelvis.position.x, BallY(), pelvis.position.z), radius * 0.9f, around, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < n; i++)
			{
				if (around[i] != col && !around[i].GetComponentInParent<Player>())
				{
					return false;
				}
			}
			return true;
		}

		private void Passive(bool on)
		{
			passive = on;
			col.enabled = !on;
			rb.isKinematic = on;
			joint.xMotion = on ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked;
			joint.zMotion = on ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked;
			joint.yDrive = on ? default(JointDrive) : new JointDrive
			{
				positionSpring = ballSpring,
				positionDamper = damp,
				maximumForce = float.MaxValue
			};
			if (!on)
			{
				rb.position = new Vector3(pelvis.position.x, BallY(), pelvis.position.z);
				rb.linearVelocity = pelvis.linearVelocity;
				rb.angularVelocity = Vector3.zero;
			}
		}

		private void StartDrown()
		{
			if (!spawnKnown)
			{
				spawnKnown = true;
				if (playSpace)
				{
					spawnRig = playSpace.position;
					spawnRigRot = playSpace.rotation;
				}
				if (pelvis)
				{
					spawnPelvis = pelvis.position;
					spawnPelvisRot = pelvis.rotation;
					float floor = FloorBelow(spawnPelvis, 0.5f, 4f);
					if (floor > float.MinValue)
					{
						spawnLift = spawnPelvis.y - floor;
					}
				}
			}
			if (playSpace)
			{
				ghostOn = true;
				Load();
				if (showLive)
				{
					Shapes(true);
				}
			}
		}

		private Vector3 SpawnShift()
		{
			Vector3 fwd = Flat(spawnRigRot * Vector3.forward);
			fwd = fwd.sqrMagnitude > 1e-4f ? fwd.normalized : Vector3.forward;
			for (int i = -1; i < 7; i++)
			{
				Vector3 offset = i == -1 ? Vector3.zero : Quaternion.Euler(0f, i * 360f / 7f, 0f) * fwd * 1.2f;
				Vector3 at = offset + spawnPelvis;
				if (Physics.Raycast(at + Vector3.up, Vector3.down, 3f, -1, QueryTriggerInteraction.Ignore) && !Occupied(at))
				{
					return offset;
				}
			}
			return SpawnElsewhere();
		}

		private float FloorBelow(Vector3 at, float from, float reach)
		{
			int n = Physics.RaycastNonAlloc(at + Vector3.up * from, Vector3.down, hits, reach, -1, QueryTriggerInteraction.Ignore);
			float best = float.MinValue;
			for (int i = 0; i < n; i++)
			{
				Collider c = hits[i].collider;
				if (c.transform.IsChildOf(transform) || c.GetComponentInParent<AvatarBody>() || c.GetComponentInParent<Rope>())
				{
					continue;
				}
				if (hits[i].normal.y < 0.9f)
				{
					continue;
				}
				if (hits[i].point.y > best)
				{
					best = hits[i].point.y;
				}
			}
			return best;
		}

		private Vector3 SpawnElsewhere()
		{
			float above = Water.main ? Water.main.Surface + 0.3f : float.MinValue;
			float lift = spawnLift > 0f ? spawnLift : standHeight + 0.3f;
			Vector3 best = Vector3.zero;
			float bestScore = float.MaxValue;
			for (int ring = 1; ring < 17; ring++)
			{
				int count = 8 + ring * 4;
				float dist = ring + ring;
				for (int i = 0; i < count; i++)
				{
					Vector3 dir = Quaternion.Euler(0f, i * 360f / count, 0f) * Vector3.forward;
					Vector3 probe = spawnPelvis + dist * dir;
					float floor = FloorBelow(probe, 3f, 40f);
					if (floor < above)
					{
						continue;
					}
					float bump = 0f;
					for (int k = 0; k < 4; k++)
					{
						Vector3 side = Quaternion.Euler(0f, k * 90f, 0f) * Vector3.forward;
						bump = Mathf.Abs(FloorBelow(new Vector3(probe.x + side.x * 0.35f, floor, probe.z + side.z * 0.35f), 1f, 1.3f) - floor);
						if (!(bump < 0.15f))
						{
							break;
						}
					}
					if (!(bump < 0.15f))
					{
						continue;
					}
					Vector3 at = new Vector3(probe.x, lift + floor, probe.z);
					if (Occupied(at))
					{
						continue;
					}
					Vector3 offset = at - spawnPelvis;
					float score = new Vector3(offset.x, offset.y * 2f, offset.z).sqrMagnitude;
					if (score < bestScore)
					{
						best = offset;
						bestScore = score;
					}
				}
			}
			return best;
		}

		private bool Occupied(Vector3 pelvisAt)
		{
			foreach (Collider c in Physics.OverlapCapsule(pelvisAt + Vector3.up * 0.1f, pelvisAt + Vector3.up * 1.1f, 0.35f, -1, QueryTriggerInteraction.Ignore))
			{
				if (!c.transform.IsChildOf(transform))
				{
					return true;
				}
			}
			return false;
		}

		public void MoveSpawn(Vector3 delta)
		{
			spawnRig += delta;
			spawnPelvis += delta;
		}

		private void FixedUpdateDrown()
		{
			if (floating)
			{
				underSince = -1f;
				return;
			}
			if (drowned)
			{
				if (Time.time >= diedAt + respawnAfter && GameManager.match == null && !GameManager.Travelling)
				{
					Respawn();
				}
				return;
			}
			if (!chest || !pelvis)
			{
				return;
			}
			bool aloft = !grounded && !hanging;
			if (GameManager.Waiting || !aloft)
			{
				fellEarly = aloft && GameManager.Waiting;
			}
			if (!water && Time.time >= nextFind)
			{
				nextFind = Time.time + 1f;
				water = Water.Get();
			}
			bool under = testDrown;
			if (water)
			{
				float surface = water.Surface;
				bool sunk = pelvis.position.y < surface && !grounded;
				under = sunk || testDrown || chest.worldCenterOfMass.y < surface + 0.1f;
				if (dummy)
				{
					under |= Soaked(surface);
				}
			}
			foreach (Water acid in Water.acids)
			{
				if (!acid)
				{
					continue;
				}
				if (acid.Burns(pelvis.position) || acid.Burns(chest.worldCenterOfMass) || (leftLowerLeg && acid.Burns(leftLowerLeg.worldCenterOfMass)) || (rightLowerLeg && acid.Burns(rightLowerLeg.worldCenterOfMass)) || (dummy && Dipped(acid)))
				{
					if (!dummy)
					{
						Note("touched acid");
					}
					Spare("touched acid");
					return;
				}
			}
			if (!under)
			{
				underSince = -1f;
				return;
			}
			if (underSince < 0f)
			{
				underSince = Time.time;
			}
			if (Time.time - underSince >= underFor)
			{
				if (!dummy)
				{
					Note("drowned at " + pelvis.position.ToString("F1"));
				}
				Spare("drowned");
			}
		}

		private void Spare(string why)
		{
			if (dummy)
			{
				SlapDummy slapDummy = GetComponent<SlapDummy>();
				if (slapDummy)
				{
					slapDummy.End(why);
				}
				underSince = -1f;
				return;
			}
			if ((carriedHold || handedOver) && GameManager.Rounds && playSpace)
			{
				// Thrown in by a teammate: back to the spawn without a death.
				bool carried = carriedHold;
				handedOver = false;
				underSince = -1f;
				testDrown = false;
				if (carried && NetworkPlayer.mine)
				{
					NetworkPlayer.mine.KeepBody();
				}
				trainHeld = false;
				SendHome("a teammate put me in: " + why, true);
				return;
			}
			bool live = !GameManager.Waiting && (!fellEarly || GameManager.RoundTime < 0f);
			if (!dummy && GameManager.match != null)
			{
				if (!live)
				{
					testDrown = false;
					fellEarly = false;
					underSince = -1f;
					Note("fell before the round went live, back to the spawn");
					Deaden();
					drowned = true;
					Respawn();
					return;
				}
				if (GameManager.RoundTime < 0f)
				{
					testDrown = false;
					fellEarly = false;
					underSince = -1f;
					Note("the round is already decided, out without a death");
					Spectate();
					return;
				}
			}
			Die();
		}

		public void Kill(string why)
		{
			if (!drowned && drowns)
			{
				if (!dummy)
				{
					Note(why);
				}
				Spare(why);
			}
		}

		public void Die()
		{
			if (drowned)
			{
				return;
			}
			drowned = true;
			deaths++;
			Audience.Cheer();
			diedAt = Time.time;
			underSince = -1f;
			testDrown = false;
			Deaden();
			dead = true;
			if (playSpace)
			{
				Begin((head ? head : pelvis).position);
			}
		}

		public void Vanish()
		{
			vanished = true;
			foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
			{
				if (r)
				{
					r.enabled = false;
				}
			}
			Note("joined mid-round, my body is hidden until the next round");
		}

		public void Spectate()
		{
			if (drowned)
			{
				return;
			}
			drowned = true;
			diedAt = Time.time;
			underSince = -1f;
			testDrown = false;
			Deaden();
			dead = true;
			if (playSpace)
			{
				Begin((head ? head : pelvis).position);
			}
		}

		public void SendHome(string why, bool clear = false)
		{
			if (!pelvis || !playSpace)
			{
				return;
			}
			LetGo(why);
			Vector3 delta = spawnPelvis + (clear ? SpawnShift() : Vector3.zero) - pelvis.position;
			Shift(delta);
			playSpace.position += delta;
			if (!dummy)
			{
				Note("sent back " + delta.magnitude.ToString("F1") + " m: " + why);
			}
		}

		public void Respawn()
		{
			if (!drowned)
			{
				return;
			}
			drowned = false;
			if (playSpace)
			{
				End();
			}
			LetGo("respawn");
			Vector3 shift = SpawnShift();
			if (playSpace)
			{
				playSpace.localScale = Vector3.one;
				playSpace.SetPositionAndRotation(spawnRig + shift, spawnRigRot);
				Face(spawnRigRot.eulerAngles.y);
				SeatCamera(true, spawnRigRot.eulerAngles.y, RestPoint(head, spawnPelvis + shift, spawnPelvisRot));
			}
			Revive(spawnPelvis + shift, spawnPelvisRot);
			if (!dummy)
			{
				if (playSpace)
				{
					LooseHat.Return(this);
					Wear(0, Prefs.outfit[0]);
				}
				if (!dummy)
				{
					Note("respawn at " + (spawnPelvis + shift).ToString("F1") + (shift.sqrMagnitude > 0f ? ", spawn was occupied, moved " + shift.magnitude.ToString("F1") + " m" : ""));
					respawnCheckAt = Time.time + 0.6f;
				}
			}
			shieldUntil = -9f;
			shieldOnRise = false;
			knockedOut = false;
			knockedOutUntil = 0f;
			damage = 0f;
			koCount = 0;
			lastKOAt = -99f;
			guardLeft = guardHits;
			looseUntil = Time.time + reviveLoose;
		}

		private void AwakeGhost()
		{
			Load();
		}

		private void Load()
		{
			if (!handModelL)
			{
				handModelL = Resources.Load<GameObject>("Player/Quest3/MetaQuestTouchPlus_Left");
			}
			if (!handModelR)
			{
				handModelR = Resources.Load<GameObject>("Player/Quest3/MetaQuestTouchPlus_Right");
			}
			if (!handMaterialL)
			{
				handMaterialL = Resources.Load<Material>("Player/Quest3/Quest3LeftMat");
			}
			if (!handMaterialR)
			{
				handMaterialR = Resources.Load<Material>("Player/Quest3/Quest3RightMat");
			}
		}

		private void StartGhost()
		{
			if (showLive)
			{
				Shapes(true);
			}
		}

		public void Begin(Vector3 body)
		{
			mode = 0;
			on = true;
			ghost = true;
			easing = true;
			staleL = staleR = true;
			pullAt = Time.time;
			pullFrom = playSpace.position;
			Vector3 eye = (cam ? cam : playSpace).position;
			float floor = Water.main ? Mathf.Max(Water.main.Surface, body.y) : body.y;
			Vector3 away = new Vector3(eye.x - body.x, 0f, eye.z - body.z);
			if (away.sqrMagnitude < 0.01f)
			{
				Vector3 fwd = (cam ? cam : playSpace).forward;
				away = new Vector3(-fwd.x, 0f, -fwd.z);
			}
			if (away.sqrMagnitude < 0.0001f)
			{
				away = Vector3.back;
			}
			away = away.normalized;
			Vector3 move = new Vector3(body.x, floor + riseTo, body.z) + away * back - eye;
			target = playSpace.position + move;
			pullFor = Mathf.Max(pullOut, move.magnitude * 1.5f / pullSpeed);
			glide = Vector3.zero;
			Shapes(true);
		}

		public void End()
		{
			on = false;
			testGripL = false;
			easing = false;
			mode = 0;
			testGripR = false;
			testTurn = Vector2.zero;
			playSpace.localScale = Vector3.one;
			glide = Vector3.zero;
			if (playSpace)
			{
				ghost = false;
			}
			if (!showLive)
			{
				Shapes(false);
			}
		}

		private void UpdateGhost()
		{
			Visible();
			if (!on)
			{
				if (cam && head && !flopped && !knockedOut && !dead)
				{
					Vector3 at = cam.InverseTransformPoint(head.position);
					Quaternion turn = Quaternion.Inverse(cam.rotation) * head.rotation;
					if (!headFromCamKnown)
					{
						headFromCam = at;
						headTurnFromCam = turn;
					}
					else
					{
						headFromCam += (at - headFromCam) * 0.02f;
						headTurnFromCam = Quaternion.Slerp(headTurnFromCam, turn, 0.02f);
					}
					headFromCamKnown = true;
				}
				return;
			}
			if (!controllerL || !controllerR)
			{
				return;
			}
			float dt = Time.deltaTime;
			float spin = testTurn.x;
			if (!ignoreInput)
			{
				spin += Prefs.Dead(GhostStick(ref ghostRight, XRNode.RightHand), true).x;
			}
			if (!Prefs.noTurn && Mathf.Abs(spin) > 0.001f && cam)
			{
				playSpace.RotateAround(cam.position, Vector3.up, spin * turnSpeed * dt);
			}
			bool gripLeft = testGripL || (!ignoreInput && GhostGrip(ref ghostLeft, XRNode.LeftHand) > gripAt);
			bool gripRight = testGripR || (!ignoreInput && GhostGrip(ref ghostRight, XRNode.RightHand) > gripAt);
			if (!gripLeft)
			{
				staleL = false;
			}
			if (!gripRight)
			{
				staleR = false;
			}
			int want = ((gripRight && !staleR) ? 2 : 0) | ((gripLeft && !staleL) ? 1 : 0);
			if (want != mode)
			{
				GhostHold(want);
			}
			Vector3 moved;
			if (mode == 0)
			{
				if (!easing)
				{
					playSpace.position += glide * dt;
					glide *= Mathf.Exp(-dt / glideFade);
					return;
				}
				float t = Mathf.Clamp01((Time.time - pullAt) / pullFor);
				playSpace.position = Vector3.Lerp(pullFrom, target, t * t * (3f - (t + t)));
				if (t >= 1f)
				{
					easing = false;
				}
				return;
			}
			if (mode != 3)
			{
				Vector3 hand = (mode == 1 ? controllerL : controllerR).position;
				moved = (anchorWorld - hand) * (1f - Mathf.Exp(-dt / gripSlip));
				playSpace.position += moved;
			}
			else
			{
				float sep = Vector3.Distance(controllerL.localPosition, controllerR.localPosition);
				float scale = Mathf.Clamp(scale0 * sep0 / Mathf.Max(sep, 0.01f), minScale, maxScale);
				playSpace.localScale = Vector3.one * scale;
				Vector3 mid = (controllerL.position + controllerR.position) * 0.5f;
				Vector3 local = playSpace.InverseTransformPoint(mid);
				moved = playSpace.TransformVector(midLocal - local);
				midLocal = local;
				playSpace.position += mid0 - mid;
			}
			if (dt > 0.0001f)
			{
				glide = Vector3.Lerp(glide, Vector3.ClampMagnitude(moved / dt, glideMax), 1f - Mathf.Exp(-dt / glideSmooth));
			}
		}

		private void Visible()
		{
			if (!shapeL && !shapeR)
			{
				return;
			}
			if (ghostMat && cosmetics != null && ghostMat.GetColor("_BaseColor") != cosmetics.color)
			{
				ghostMat.SetColor("_BaseColor", cosmetics.color);
			}
			bool live = showLive && playSpace && inVR;
			if (live && (!ghostGrabL || !ghostGrabR))
			{
				ghostGrabL = leftHand ? leftHand.GetComponent<HandGrab>() : null;
				ghostGrabR = rightHand ? rightHand.GetComponent<HandGrab>() : null;
			}
			bool ghosting = (on && !easing) || floating;
			bool showL = ghosting;
			bool showR = ghosting;
			if (live && !ghosting)
			{
				showL = handModelL && !(hideWhileGrabbing && ghostGrabL && ghostGrabL.grabbing);
				showR = handModelR && !(hideWhileGrabbing && ghostGrabR && ghostGrabR.grabbing);
			}
			float rate = Time.deltaTime / Mathf.Max(ghostFadeIn, 0.01f);
			fadeL = Mathf.MoveTowards(fadeL, showL ? 1f : 0f, rate);
			fadeR = Mathf.MoveTowards(fadeR, showR ? 1f : 0f, rate);
			Fade(shapeL, fadeL);
			Fade(shapeR, fadeR);
		}

		private void Fade(GameObject go, float k)
		{
			Show(go, k > 0f);
			if (k <= 0f || !go)
			{
				return;
			}
			Renderer r = go.GetComponent<Renderer>();
			if (!r)
			{
				return;
			}
			if (ghostBlock == null)
			{
				ghostBlock = new MaterialPropertyBlock();
			}
			ghostBlock.SetFloat("_Alpha", k);
			r.SetPropertyBlock(ghostBlock);
		}

		private static void Show(GameObject go, bool yes)
		{
			if (go && go.activeSelf != yes)
			{
				go.SetActive(yes);
			}
		}

		private void GhostHold(int want)
		{
			int was = mode;
			mode = want;
			if (want == 0)
			{
				return;
			}
			easing = false;
			if (want != 3)
			{
				anchorWorld = (want == 1 ? controllerL : controllerR).position;
				if (was == 3)
				{
					anchorWorld += glide * gripSlip;
				}
				return;
			}
			sep0 = Vector3.Distance(controllerL.localPosition, controllerR.localPosition);
			scale0 = playSpace.localScale.x;
			mid0 = (controllerL.position + controllerR.position) * 0.5f;
			midLocal = playSpace.InverseTransformPoint(mid0);
		}

		public void Float(bool on)
		{
			if (floating == on)
			{
				return;
			}
			floating = on;
			frozen = on;
			if (on)
			{
				floatGrav.Clear();
				foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
				{
					if (rb.useGravity)
					{
						rb.useGravity = false;
						floatGrav.Add(rb);
					}
					if (!rb.isKinematic)
					{
						rb.linearVelocity = Vector3.zero;
						rb.angularVelocity = Vector3.zero;
					}
				}
			}
			else
			{
				foreach (Rigidbody rb in floatGrav)
				{
					if (rb)
					{
						rb.useGravity = true;
					}
				}
				floatGrav.Clear();
			}
			if (on)
			{
				floatHid.Clear();
				foreach (Renderer r in GetComponentsInChildren<Renderer>())
				{
					if (!r.enabled || (controllerL && r.transform.IsChildOf(controllerL)) || (controllerR && r.transform.IsChildOf(controllerR)))
					{
						continue;
					}
					r.enabled = false;
					floatHid.Add(r);
				}
			}
			else
			{
				foreach (Renderer r in floatHid)
				{
					if (r)
					{
						r.enabled = true;
					}
				}
				floatHid.Clear();
			}
			Shapes(on || showLive || this.on);
		}

		private void Shapes(bool show)
		{
			if (!show)
			{
				if (shapeL)
				{
					UnityEngine.Object.Destroy(shapeL);
				}
				if (shapeR)
				{
					UnityEngine.Object.Destroy(shapeR);
				}
				shapeL = null;
				shapeR = null;
				fadeL = fadeR = 0f;
				return;
			}
			if (!shapeL)
			{
				shapeL = Shape(controllerL, "Controller.L", true);
			}
			if (!shapeR)
			{
				shapeR = Shape(controllerR, "Controller.R", false);
			}
		}

		private GameObject Shape(Transform hand, string name, bool isLeft)
		{
			if (!hand)
			{
				return null;
			}
			GameObject shape = HandShape(hand, name, isLeft);
			if (shape)
			{
				return shape;
			}
			GameObject model = isLeft ? handModelL : handModelR;
			GameObject go;
			if (!model)
			{
				go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
				go.name = name;
				UnityEngine.Object.Destroy(go.GetComponent<Collider>());
				go.transform.SetParent(hand, false);
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				go.transform.localScale = new Vector3(handRadius * 2f, handLength * 0.5f, handRadius * 2f);
				if (handMaterial)
				{
					go.GetComponent<Renderer>().sharedMaterial = handMaterial;
				}
			}
			else
			{
				go = Instantiate(model, hand, false);
				go.name = name;
				go.transform.localPosition = isLeft ? handOffsetL : handOffsetR;
				go.transform.localRotation = Quaternion.Euler(isLeft ? handEulerL : handEulerR);
				go.transform.localScale = model.transform.localScale * handModelScale;
				Material material = isLeft ? handMaterialL : handMaterialR;
				foreach (Renderer r in go.GetComponentsInChildren<Renderer>(true))
				{
					if (hideBattery && r.name.ToLower().Contains("battery"))
					{
						r.enabled = false;
					}
					else if (material)
					{
						r.sharedMaterial = material;
					}
				}
				foreach (Collider c in go.GetComponentsInChildren<Collider>(true))
				{
					UnityEngine.Object.Destroy(c);
				}
			}
			go.SetActive(false);
			return go;
		}

		public GameObject GhostFist(Transform parent, bool isLeft)
		{
			List<Digit> fingers = isLeft ? ghostFingersL : ghostFingersR;
			List<Digit> saved = new List<Digit>(fingers);
			GameObject fist = HandShape(parent, "Ghost Fist", isLeft);
			if (fist)
			{
				foreach (Digit d in fingers)
				{
					d.bone.localRotation = d.closed;
				}
				fist.SetActive(true);
			}
			fingers.Clear();
			fingers.AddRange(saved);
			return fist;
		}

		private GameObject HandShape(Transform controller, string name, bool isLeft)
		{
			Rigidbody hand = isLeft ? leftHand : rightHand;
			SkinnedMeshRenderer skin = full ? full : BodySkin();
			if (!hand || !skin || !skin.sharedMesh)
			{
				return null;
			}
			Transform[] bones = skin.bones;
			List<int> used = new List<int>();
			int root = -1;
			for (int i = 0; i < bones.Length; i++)
			{
				if (bones[i] && bones[i].IsChildOf(hand.transform))
				{
					if (bones[i] == hand.transform)
					{
						root = used.Count;
					}
					used.Add(i);
				}
			}
			if (root < 0)
			{
				return null;
			}
			GameObject go = new GameObject(name);
			go.layer = controller.gameObject.layer;
			go.transform.SetParent(controller, false);
			go.transform.localRotation = Quaternion.Euler(handKey);
			Vector3 handScale = hand.transform.lossyScale;
			Vector3 controllerScale = controller.lossyScale;
			go.transform.localScale = new Vector3(handScale.x / controllerScale.x, handScale.y / controllerScale.y, handScale.z / controllerScale.z);
			List<Digit> ghostFingers = isLeft ? ghostFingersL : ghostFingersR;
			List<Digit> fingers = isLeft ? leftFingers : rightFingers;
			ghostFingers.Clear();
			Transform[] copies = new Transform[used.Count];
			for (int i = 0; i < used.Count; i++)
			{
				copies[i] = i == root ? go.transform : new GameObject(bones[used[i]].name).transform;
			}
			for (int i = 0; i < used.Count; i++)
			{
				if (i == root)
				{
					continue;
				}
				Transform bone = bones[used[i]];
				int slot = used.IndexOf(Array.IndexOf(bones, bone.parent));
				copies[i].SetParent(slot < 0 ? go.transform : copies[slot], false);
				copies[i].localPosition = bone.localPosition;
				copies[i].localRotation = bone.localRotation;
				copies[i].localScale = bone.localScale;
				foreach (Digit d in fingers)
				{
					if (d.bone == bone)
					{
						copies[i].localRotation = d.open;
						ghostFingers.Add(new Digit
						{
							bone = copies[i],
							thumb = d.thumb,
							open = d.open,
							closed = d.closed
						});
					}
				}
			}
			Mesh mesh = isLeft ? ghostMeshL : ghostMeshR;
			if (!mesh)
			{
				mesh = HandMesh(skin.sharedMesh, used, root, wristFade);
				if (isLeft)
				{
					ghostMeshL = mesh;
				}
				else
				{
					ghostMeshR = mesh;
				}
			}
			if (!ghostMat)
			{
				Material source = Resources.Load<Material>("Player/GhostHand");
				Material shared = skin.sharedMaterial;
				if (source)
				{
					ghostMat = new Material(source);
					if (shared && shared.HasProperty("_BaseMap"))
					{
						ghostMat.SetTexture("_BaseMap", shared.GetTexture("_BaseMap"));
					}
					if (shared && shared.HasProperty("_BaseColor"))
					{
						ghostMat.SetColor("_BaseColor", cosmetics is not null ? cosmetics.color : shared.GetColor("_BaseColor"));
					}
				}
			}
			SkinnedMeshRenderer smr = go.AddComponent<SkinnedMeshRenderer>();
			smr.sharedMesh = mesh;
			smr.bones = copies;
			smr.rootBone = go.transform;
			Material[] materials;
			if (ghostMat)
			{
				materials = new Material[mesh.subMeshCount];
				for (int i = 0; i < materials.Length; i++)
				{
					materials[i] = ghostMat;
				}
			}
			else
			{
				materials = skin.sharedMaterials;
			}
			smr.sharedMaterials = materials;
			smr.updateWhenOffscreen = true;
			smr.shadowCastingMode = ShadowCastingMode.Off;
			go.SetActive(false);
			return go;
		}

		internal static Mesh HandMesh(Mesh source, List<int> used, int root, float fade)
		{
			int[] map = new int[source.bindposes.Length];
			for (int i = 0; i < map.Length; i++)
			{
				map[i] = -1;
			}
			for (int i = 0; i < used.Count; i++)
			{
				map[used[i]] = i;
			}
			BoneWeight[] weights = source.boneWeights;
			Vector3[] vertices = source.vertices;
			Vector3[] normals = source.normals;
			Vector4[] tangents = source.tangents;
			Vector2[] uv = source.uv;
			int[] remap = new int[vertices.Length];
			bool[] keep = new bool[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
			{
				remap[i] = -1;
				BoneWeight w = weights[i];
				float on = (map[w.boneIndex0] < 0 ? 0f : w.weight0) + (map[w.boneIndex1] < 0 ? 0f : w.weight1) + (map[w.boneIndex2] < 0 ? 0f : w.weight2) + (map[w.boneIndex3] < 0 ? 0f : w.weight3);
				keep[i] = on >= 0.5f;
			}
			List<Vector3> verts = new List<Vector3>();
			List<Vector3> norms = new List<Vector3>();
			List<Vector4> tans = new List<Vector4>();
			List<Vector2> uvs = new List<Vector2>();
			List<BoneWeight> boneWeights = new List<BoneWeight>();
			List<int>[] subs = new List<int>[source.subMeshCount];
			for (int s = 0; s < source.subMeshCount; s++)
			{
				subs[s] = new List<int>();
				int[] tris = source.GetTriangles(s);
				for (int t = 0; t < tris.Length; t += 3)
				{
					if (!keep[tris[t]] || !keep[tris[t + 1]] || !keep[tris[t + 2]])
					{
						continue;
					}
					for (int k = 0; k < 3; k++)
					{
						int v = tris[t + k];
						if (remap[v] < 0)
						{
							remap[v] = verts.Count;
							verts.Add(vertices[v]);
							if (normals.Length == vertices.Length)
							{
								norms.Add(normals[v]);
							}
							if (tangents.Length == vertices.Length)
							{
								tans.Add(tangents[v]);
							}
							if (uv.Length == vertices.Length)
							{
								uvs.Add(uv[v]);
							}
							boneWeights.Add(Remap(weights[v], map, root));
						}
						subs[s].Add(remap[v]);
					}
				}
			}
			Matrix4x4 rootToMesh = source.bindposes[used[root]].inverse;
			Vector3 origin = rootToMesh.MultiplyPoint3x4(Vector3.zero);
			Vector3 axis = rootToMesh.MultiplyVector(Vector3.up).normalized;
			float side = 0f;
			foreach (Vector3 p in verts)
			{
				side += Vector3.Dot(axis, p - origin);
			}
			if (side < 0f)
			{
				axis = -axis;
			}
			float low = float.MaxValue;
			foreach (Vector3 p in verts)
			{
				low = Mathf.Min(low, Vector3.Dot(axis, p - origin));
			}
			List<Color> colors = new List<Color>(verts.Count);
			float span = Mathf.Max(fade, 0.001f);
			foreach (Vector3 p in verts)
			{
				float along = Vector3.Dot(axis, p - origin);
				colors.Add(new Color(1f, 1f, 1f, Mathf.SmoothStep(0f, 1f, (along - low) / span)));
			}
			Mesh mesh = new Mesh();
			mesh.name = source.name + " Hand";
			mesh.SetVertices(verts);
			mesh.SetColors(colors);
			if (norms.Count == verts.Count)
			{
				mesh.SetNormals(norms);
			}
			if (tans.Count == verts.Count)
			{
				mesh.SetTangents(tans);
			}
			if (uvs.Count == verts.Count)
			{
				mesh.SetUVs(0, uvs);
			}
			mesh.subMeshCount = subs.Length;
			for (int s = 0; s < subs.Length; s++)
			{
				mesh.SetTriangles(subs[s], s);
			}
			mesh.boneWeights = boneWeights.ToArray();
			Matrix4x4[] bindposes = new Matrix4x4[used.Count];
			for (int i = 0; i < used.Count; i++)
			{
				bindposes[i] = source.bindposes[used[i]];
			}
			mesh.bindposes = bindposes;
			mesh.RecalculateBounds();
			return mesh;
		}

		private static BoneWeight Remap(BoneWeight w, int[] map, int root)
		{
			int[] index = new int[4] { w.boneIndex0, w.boneIndex1, w.boneIndex2, w.boneIndex3 };
			float[] weight = new float[4] { w.weight0, w.weight1, w.weight2, w.weight3 };
			int[] outIndex = new int[4];
			float[] outWeight = new float[4];
			float total = 0f;
			int count = 0;
			for (int i = 0; i < weight.Length; i++)
			{
				if (weight[i] <= 0f)
				{
					continue;
				}
				int bone = map[index[i]] < 0 ? root : map[index[i]];
				int k = Array.IndexOf(outIndex, bone, 0, count);
				if (k < 0)
				{
					outIndex[count] = bone;
					k = count++;
				}
				outWeight[k] += weight[i];
				total += weight[i];
			}
			BoneWeight result = default;
			if (count == 0 || total <= 0f)
			{
				result.boneIndex0 = root;
				result.weight0 = 1f;
				return result;
			}
			result.boneIndex0 = outIndex[0];
			result.weight0 = outWeight[0] / total;
			result.boneIndex1 = outIndex[1];
			result.weight1 = outWeight[1] / total;
			result.boneIndex2 = outIndex[2];
			result.weight2 = outWeight[2] / total;
			result.boneIndex3 = outIndex[3];
			result.weight3 = outWeight[3] / total;
			return result;
		}

		private static Vector2 GhostStick(ref UnityEngine.XR.InputDevice device, XRNode node)
		{
			if (!device.isValid)
			{
				device = InputDevices.GetDeviceAtXRNode(node);
			}
			if (device.isValid && device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 stick))
			{
				return stick;
			}
			return Vector2.zero;
		}

		private static float GhostGrip(ref UnityEngine.XR.InputDevice device, XRNode node)
		{
			if (!device.isValid)
			{
				device = InputDevices.GetDeviceAtXRNode(node);
			}
			if (!device.isValid)
			{
				return 0f;
			}
			return device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.grip, out float grip) ? grip : 0f;
		}

		private static Vector3 Flat2(Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		private void StartRig()
		{
			yaw = playSpace.eulerAngles.y;
			playSpace.position = new Vector3(playSpace.position.x, playSpace.position.y + sink, playSpace.position.z);
			if (!Application.isEditor)
			{
				UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings.XrPerformanceSettingsFeature.SetPerformanceLevelHint(UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings.PerformanceDomain.Cpu, UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings.PerformanceLevelHint.SustainedHigh);
			}
		}

		private void Measure()
		{
			if (!inVR || !cam)
			{
				return;
			}
			peak = Mathf.Max(peak, cam.localPosition.y);
			if (Time.time < nextLog)
			{
				return;
			}
			nextLog = Time.time + 1f;
			Vector3 handL = controllerL ? controllerL.localPosition : Vector3.zero;
			Vector3 handR = controllerR ? controllerR.localPosition : Vector3.zero;
			Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "CALIBRATE eye={0} peak={1} handL={2} handR={3}", cam.localPosition.y.ToString("F3"), peak.ToString("F3"), handL.ToString("F2"), handR.ToString("F2"));
			worst = 0f;
		}

		private void UpdateRig()
		{
			bool tracking = fakeVR || (headTracker != null && headTracker.tracking);
			inVR = tracking;
			if (trackL != null)
			{
				trackL.enabled = tracking;
			}
			if (trackR != null)
			{
				trackR.enabled = tracking;
			}
			bool nowTracking;
			if (!tracking)
			{
				nowTracking = false;
			}
			else if (!wasTracking)
			{
				neckStandKnown = false;
				if (head && !ghost && !drowned)
				{
					SeatCamera(!seatedOnce, playSpace.eulerAngles.y, head.position);
				}
				seatedOnce = true;
				nowTracking = inVR;
			}
			else
			{
				nowTracking = true;
			}
			wasTracking = nowTracking;
			if (ghost)
			{
				moveWish = Vector3.zero;
				moveNow = Vector3.zero;
				spaceVel = Vector3.zero;
				falling = false;
				spaceTime = Time.time;
				jumpHeld = false;
				return;
			}
			if (Time.deltaTime > worst)
			{
				worst = Time.deltaTime;
			}
			Measure();
			float dt = Time.deltaTime;
			rideCarry = Vector3.zero;
			rideVel = Vector3.zero;
			followVel = Vector3.zero;
			if (inVR)
			{
				Controllers(dt);
			}
			else
			{
				Desk(dt);
			}
			Recentre();
			JumpInput();
			Leash(dt);
			spaceVel = moveNow + rideVel + followVel + (falling ? Vector3.up * airVel : Vector3.zero);
			spaceTime = Time.time;
		}

		private void JumpInput()
		{
			bool pressed = !ignoreInput && Either(rightDevice, UnityEngine.XR.CommonUsages.primaryButton, UnityEngine.XR.CommonUsages.primary2DAxisClick);
			bool down;
			if (frozen || pressed)
			{
				down = !frozen;
			}
			else
			{
				down = (Keyboard.current != null && !ignoreInput && Keyboard.current.fKey.isPressed) || testJump;
			}
			if (down)
			{
				if (!jumpHeld)
				{
					liftUsed = false;
					jumpSince = Time.time;
				}
				else if (liftUsed)
				{
					jumpCharge = 0f;
					jumpHeld = true;
					return;
				}
				jumpCharge = Mathf.Clamp01((Time.time - jumpSince) / Mathf.Max(jumpChargeTime, 0.01f));
				jumpHeld = true;
				return;
			}
			if (jumpHeld && !liftUsed)
			{
				jumpAsk = true;
				jumpAskCharge = jumpCharge;
				jumpAskUntil = Time.time + jumpBuffer;
			}
			jumpCharge = 0f;
			jumpHeld = false;
		}

		private void Recentre()
		{
			if (dummy)
			{
				return;
			}
			bool left = Either(leftDevice, UnityEngine.XR.CommonUsages.secondaryButton, UnityEngine.XR.CommonUsages.primary2DAxisClick);
			bool right = rightDevice.isValid && rightDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out bool rightButton) && rightButton;
			// A button already claimed by the sandbox or a cosmetic dummy doesn't recentre.
			bool pressed = (left && !Sandbox.Busy(XRNode.LeftHand) && !CosmeticDummy.Busy(XRNode.LeftHand))
				|| (right && !Sandbox.Busy(XRNode.RightHand) && !CosmeticDummy.Busy(XRNode.RightHand));
			if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
			{
				pressed = true;
			}
			if (!pressed)
			{
				heldRecentre = left || right;
				return;
			}
			if (heldRecentre)
			{
				return;
			}
			heldRecentre = true;
			if (head && !ghost && !drowned)
			{
				SeatCamera(false, 0f, head.position);
			}
		}

		private void Leash(float dt)
		{
			if (!cam || !pelvis)
			{
				return;
			}
			if (flopped || rising)
			{
				pinFadeUntil = Time.time + pinFade;
				PinToHead(Time.deltaTime);
				headKnown = false;
				neckLocalKnown = false;
				pelvisKnown = false;
				followVelY = 0f;
				return;
			}

			// Ease the home point (neck + hip offset, in play space) toward where it is now.
			Vector3 home = playSpace.InverseTransformPoint(Neck() + hipOffset);
			if (!homed)
			{
				homeEase = home;
				homed = true;
			}
			float kY = 1f - Mathf.Exp(-leashFollowY * dt);
			float k = 1f - Mathf.Exp(-leashFollow * dt);
			homeEase = new Vector3(Mathf.Lerp(homeEase.x, home.x, k), Mathf.Lerp(homeEase.y, home.y, kY), Mathf.Lerp(homeEase.z, home.z, k));

			Vector3 pelvisAt = pelvis.transform.position;
			Vector3 homeAt = playSpace.TransformPoint(homeEase);
			Vector3 gap = pelvisAt - homeAt;
			Vector3 localGap = playSpace.InverseTransformDirection(gap);
			localGap.y = 0f;
			if (!holding && !selfPush && moveWish.sqrMagnitude < 0.01f && pelvis.linearVelocity.sqrMagnitude < 0.09f && localGap.magnitude < biasMax * 1.5f)
			{
				bias = Vector3.ClampMagnitude(Vector3.Lerp(bias, localGap, 1f - Mathf.Exp(-biasRate * dt)), biasMax);
			}
			rawGap = gap;
			leashBias = playSpace.TransformDirection(bias);
			bodyGap = gap - playSpace.TransformDirection(bias);

			// Crouching in the ball sinks the rig.
			float squatWas = squatShown;
			squatShown = Mathf.Lerp(squatShown, inVR ? ballSquat : 0f, 1f - Mathf.Exp(-squatFollow * dt));
			if (Mathf.Abs(squatWas - squatShown) > 1e-05f)
			{
				float drop = squatWas - squatShown;
				playSpace.position += Vector3.up * drop;
				rode += Vector3.up * drop;
				followVel += dt > 0.0001f ? Vector3.up * (drop / dt) : Vector3.zero;
			}

			Vector3 pelvisPos = pelvis.transform.position;
			Vector3 pelvisStep = pelvisKnown ? pelvisPos - lastPelvis : Vector3.zero;
			lastPelvis = pelvisPos;
			pelvisKnown = true;
			Vector3 headPos = head ? head.transform.position : pelvisPos;
			Vector3 headStep = headKnown ? headPos - lastHead : Vector3.zero;
			lastHead = headPos;
			headKnown = true;
			if (hanging)
			{
				hangUntil = Time.time + headHang;
			}
			ridingHead = airborne || hanging || landing || Time.time < hangUntil;
			bool slid = traction < slipRide;
			if (slid)
			{
				Vector3 slide = pelvis.linearVelocity;
				slide.y = 0f;
				_ = slide.magnitude; // computed and discarded in the shipped build
			}
			bool riding = ridingHead || slid;
			Vector3 step = riding ? headStep : pelvisStep;
			Vector3 headOff = Vector3.zero;
			if (riding && headTarget)
			{
				headOff = headPos - (neckRide ? Neck() : headTarget.position);
			}
			float close = Mathf.Exp(-headClose * dt);

			// How far the pelvis sits above/below where the head says it should be.
			float expected = Neck().y - headAbovePelvis;
			leashExpected = expected;
			leashGy = pelvisPos.y - expected;
			leashGg = pelvisGoalY - expected;
			float rise = leashGy;
			if (!ridingY && (Mathf.Abs(leashGg) <= followDeadY || Mathf.Sign(leashGg) != Mathf.Sign(rise)))
			{
				if (!(liftFollow > 0f && rise > liftFollow && pelvisStep.y > 0f && Grabbing && !jumped))
				{
					rise = 0f;
				}
			}
			float neckY = playSpace.InverseTransformPoint(Neck()).y;
			float neckDelta = neckLocalKnown ? neckY - lastNeckLocalY : 0f;
			lastNeckLocalY = neckY;
			neckLocalKnown = true;
			float lift;
			if (holding || gripping)
			{
				lift = riding ? headOff.y : 0f;
				step.y -= neckDelta;
			}
			else
			{
				lift = riding ? headOff.y : rise;
			}

			float moved;
			if (riding || ridingY || holding || gripping)
			{
				moved = (1f - Mathf.Exp(dt * -5f)) * lift + step.y;
				float vy = dt > 0.0001f ? moved / dt : 0f;
				if (landing && landSmooth > 0f)
				{
					rideVelY = Mathf.Lerp(rideVelY, vy, 1f - Mathf.Exp(-dt / landSmooth));
					moved = rideVelY * dt;
				}
				else
				{
					rideVelY = vy;
				}
				playSpace.position += Vector3.up * moved;
				rode += Vector3.up * moved;
				followVel = dt > 0.0001f ? Vector3.up * (moved / dt) : Vector3.zero;
				followVelY = 0f;
				leashBranch = 1;
			}
			else
			{
				float want = 0f;
				if (Mathf.Abs(lift) > followDeadY)
				{
					want = pelvisStep.y + (lift - Mathf.Sign(lift) * followDeadY) * (1f - Mathf.Exp(-followRate * dt));
				}
				moved = Smooth(want, dt);
				if (Mathf.Abs(moved) > 1e-05f)
				{
					playSpace.position += Vector3.up * moved;
					rode += Vector3.up * moved;
					followVel = Vector3.up * (moved / dt);
				}
				leashBranch = want != 0f ? 2 : 0;
			}
			leashRise = moved;
			falling = false;

			// Horizontal leash: drag the rig after the body.
			Vector3 headFlat = new Vector3(headGap.x, 0f, headGap.z);
			Vector3 pull = new Vector3(bodyGap.x, 0f, bodyGap.z) + headFlat;
			if (selfPush)
			{
				Vector3 wish = new Vector3(moveWish.x, 0f, moveWish.z);
				Vector3 kept = headFlat;
				if (wish.sqrMagnitude > 0.0001f)
				{
					Vector3 dir = wish.normalized;
					float along = Vector3.Dot(pull - headFlat, dir);
					if (along < 0f)
					{
						kept = headFlat + dir * along;
					}
				}
				pull = kept;
				if (pull.sqrMagnitude < 0.0001f)
				{
					slack.x = 0f;
					slack.z = 0f;
				}
			}
			if (ridingHead || slid)
			{
				float dx = headOff.x * (1f - close) + (step.x - (moveNow.x + rideCarry.x) * dt);
				float dz = headOff.z * (1f - close) + (step.z - (moveNow.z + rideCarry.z) * dt);
				Vector3 carry = new Vector3(dx, 0f, dz);
				playSpace.position += carry;
				rode += carry;
				rideVel = dt > 0.0001f ? carry / dt + rideCarry : Vector3.zero;
				pull = Vector3.zero;
				slack.x = 0f;
				slack.z = 0f;
			}
			leashGap = new Vector3(pull.x, headGap.y, pull.z);
			Vector3 target = leashGap;
			if (Mathf.Abs(target.y) < leashDeadY)
			{
				target.y = 0f;
			}
			if (new Vector2(pull.x - headGap.x, pull.z - headGap.z).magnitude < leashDead)
			{
				target.x = headGap.x;
				target.z = headGap.z;
			}
			slack = Vector3.Lerp(slack, target, 1f - Mathf.Exp(-leashSmooth * dt));
			playSpace.position += Vector3.ClampMagnitude(slack * leashRate, leashMax) * dt;

			// Never let the camera wander further than maxSeparation from the head.
			if (head)
			{
				Vector3 headAt = head.transform.position;
				Vector3 neck = Neck();
				Vector3 apart = new Vector3(headAt.x - neck.x, 0f, headAt.z - neck.z);
				float distance = apart.magnitude;
				if (distance > maxSeparation)
				{
					playSpace.position += apart * ((distance - maxSeparation) / distance);
					slack.x = 0f;
					slack.z = 0f;
				}
			}
		}

		public void Face(float degrees)
		{
			yaw = degrees;
			playSpace.rotation = Quaternion.Euler(0f, degrees, 0f);
		}

		public void Look(float degrees)
		{
			pitch = Mathf.Clamp(degrees, -maxPitch, maxPitch);
			testPitch = Mathf.Clamp(degrees, -maxPitch, maxPitch);
		}

		private void SeatCamera(bool turn, float facing, Vector3 headAt)
		{
			if (!playSpace || !cam || !inVR)
			{
				return;
			}
			neckStandKnown = false;
			float turned = 0f;
			if (turn)
			{
				turned = Mathf.DeltaAngle(cam.eulerAngles.y, facing);
				playSpace.RotateAround(cam.position, Vector3.up, turned);
			}
			Vector3 neck = Neck();
			Vector3 move = new Vector3(headAt.x - neck.x, 0f, headAt.z - neck.z);
			playSpace.position += move;
			Resnap();
			if (!dummy)
			{
				Note("seat camera on body: moved " + move.magnitude.ToString("F2") + " m, turned " + turned.ToString("F0") + " deg");
			}
		}

		private Vector3 RestPoint(Rigidbody bone, Vector3 pelvisPos, Quaternion pelvisRot)
		{
			if (restPos != null && bone)
			{
				for (int i = 0; i < bodies.Length; i++)
				{
					if (bodies[i] == bone)
					{
						return pelvisPos + pelvisRot * restPos[i];
					}
				}
			}
			return bone ? bone.position : pelvisPos;
		}

		public void Resnap()
		{
			homed = false;
			neckLocalKnown = false;
			pelvisKnown = false;
			headKnown = false;
			slack = Vector3.zero;
			followVelY = 0f;
			bias = Vector3.zero;
		}

		private float Smooth(float rise, float dt)
		{
			if (dt <= 0.0001f)
			{
				return 0f;
			}
			if (followSmooth <= 0f)
			{
				followVelY = rise / dt;
				return rise;
			}
			followVelY = Mathf.Lerp(followVelY, rise / dt, 1f - Mathf.Exp(-dt / followSmooth));
			return followVelY * dt;
		}

		private float ArmRise(float yL, float yR)
		{
			float now = Time.time;
			int at = armTrailAt;
			armTrail[at] = new Vector3(now, yL, yR);
			int n = armTrail.Length;
			armTrailAt = (at + 1) % n;
			if (n < 2)
			{
				return 0f;
			}
			for (int back = n - 1; back > 0; back--)
			{
				Vector3 then = armTrail[(at + back) % n];
				if (then.x <= 0f)
				{
					return 0f;
				}
				float age = now - then.x;
				if (age <= 0f)
				{
					return 0f;
				}
				if (age >= armUpWindow)
				{
					return Mathf.Clamp(Mathf.Min(yL - then.y, yR - then.z) / age, -swingCap, swingCap);
				}
			}
			return 0f;
		}

		private float Swing(float dt)
		{
			if (!cam || !controllerL || !controllerR || dt <= 0.0001f)
			{
				return 1f;
			}
			Vector3 relL = controllerL.localPosition - cam.localPosition;
			Vector3 relR = controllerR.localPosition - cam.localPosition;
			if (swingKnown)
			{
				float k = 1f - Mathf.Exp(-dt / swingSmooth);
				swingL = Mathf.Lerp(swingL, Mathf.Min((relL - lastRelL).magnitude / dt, swingCap), k);
				swingR = Mathf.Lerp(swingR, Mathf.Min((relR - lastRelR).magnitude / dt, swingCap), k);
				float rise = ArmRise(relL.y, relR.y);
				armUp = Mathf.Max(rise, armUp * Mathf.Exp(-dt / armUpHold));
			}
			lastRelL = relL;
			lastRelR = relR;
			swingKnown = true;
			float t = Mathf.InverseLerp(swingFrom, swingFull, Mathf.Min(swingL, swingR));
			return 1f + (sprintMult - 1f) * Mathf.Max(t, 0f);
		}

		private void Controllers(float dt)
		{
			float pump = Swing(dt);
			pumpNow = pump;
			Vector2 left = ignoreInput ? Vector2.zero : Stick(ref leftDevice, XRNode.LeftHand);
			Vector2 right = ignoreInput ? Vector2.zero : Stick(ref rightDevice, XRNode.RightHand);
			Vector2 moveStick = Prefs.swapSticks ? right : left;
			Vector2 turnStick = Prefs.swapSticks ? left : right;
			if (frozen)
			{
				moveStick = Vector2.zero;
			}
			Vector2 move = Prefs.Dead(moveStick, false);
			Vector2 turn = Prefs.Dead(turnStick, true);
			if (Prefs.armWalk && !ignoreInput && !frozen)
			{
				move = new Vector2(0f, Mathf.InverseLerp(armWalkFrom, armWalkFull, Mathf.Min(swingL, swingR)));
			}
			if (testWish.sqrMagnitude > 0f)
			{
				move = new Vector2(testWish.x, testWish.z);
			}
			float legs = 0f;
			if (!Prefs.noKick && Mathf.Abs(turn.y) > kickFrom)
			{
				legs = -(Mathf.Sign(turn.y) * Mathf.InverseLerp(kickFrom, kickFull, Mathf.Abs(turn.y)));
			}
			airLegs = legs;
			stickCrouch = grounded && !frozen && Mathf.Abs(turn.x) < -turn.y ? Mathf.InverseLerp(CrouchFrom, CrouchFull, -turn.y) : 0f;
			if (Prefs.noTurn)
			{
				snapReady = false;
			}
			else if (!Prefs.snapTurn)
			{
				if (turn.x != 0f && cam)
				{
					playSpace.RotateAround(cam.position, Vector3.up, turn.x * turnSpeed * dt);
				}
			}
			else
			{
				if (Mathf.Abs(turn.x) < snapReset)
				{
					snapReady = true;
				}
				if (snapReady && Mathf.Abs(turn.x) > snapFrom && cam)
				{
					snapReady = false;
					float angle = Prefs.SnapAngle(turnSpeed);
					playSpace.RotateAround(cam.position, Vector3.up, turn.x >= 0f ? angle : -angle);
					Spin(cam.position, turn.x >= 0f ? angle : -angle);
				}
			}
			float dead = Prefs.armWalk ? deadzone * deadzone : 0.0001f;
			Vector3 wish = Vector3.zero;
			if (move.y * move.y + move.x * move.x >= dead)
			{
				Transform basis = cam ? cam : playSpace;
				Vector3 dir = Quaternion.Euler(0f, basis.eulerAngles.y, 0f) * new Vector3(move.x, 0f, move.y);
				wish = Tethered(Vector3.ClampMagnitude(dir, 1f) * (pump * speed));
			}
			moveWish = Ramp(wish, dt);
			Move(dt);
			PinToHead(dt);
		}

		public void Unpin()
		{
			pinFadeUntil = -9f;
		}

		private void PinToHead(float dt)
		{
			if (!head || !cam)
			{
				return;
			}
			float k = 1f;
			if (!flopped && !rising)
			{
				if (!(pinFade > 0f))
				{
					return;
				}
				k = Mathf.Clamp01((pinFadeUntil - Time.time) / pinFade);
				if (k <= 0f)
				{
					return;
				}
			}
			Vector3 want = head.worldCenterOfMass + Vector3.up * headPinUp;
			playSpace.position += (want - cam.position) * (k * (1f - Mathf.Exp(-(headPinRate * dt))));
		}

		public void Shove(Vector3 impulse)
		{
			shoveVel = Vector3.ClampMagnitude(shoveVel + impulse / Mass(), shoveMax);
		}

		private float Mass()
		{
			return Mathf.Max(TotalMass(), 1f);
		}

		private void Move(float dt)
		{
			float turn = 0f;
			shoveVel = Vector3.ClampMagnitude((shoveVel + shoveForce / Mass() * dt) * Mathf.Exp(-(shoveDrag * dt)), shoveMax);
			moveNow = moveWish * Free() * Mathf.Max(traction, slipMin) + shoveVel;
			if ((gripping && !grounded) || hanging || flopped || rising)
			{
				moveNow = Vector3.zero;
				if (rising)
				{
					moveWish = Vector3.zero;
				}
			}
			if (airborne)
			{
				moveNow *= jumped ? 1f : airControl;
			}
			if (slipping)
			{
				moveNow = Vector3.zero;
			}
			float ahead;
			if (!aheadFree)
			{
				ahead = Mathf.Sqrt(rawGap.x * rawGap.x + rawGap.z * rawGap.z);
			}
			else
			{
				ahead = 0f;
				Vector3 flat = new Vector3(moveNow.x, 0f, moveNow.z);
				if (flat.sqrMagnitude > 0.0001f)
				{
					ahead = -Vector3.Dot(new Vector3(rawGap.x, 0f, rawGap.z), flat.normalized);
				}
			}
			moveNow *= 1f - Mathf.Clamp01((ahead - 0.2f) / Mathf.Max(maxSeparation - 0.2f, 0.05f));
			Vector3 ride = RideHeld();
			if (ride.sqrMagnitude < 1e-06f)
			{
				ride = RideFloor(out turn);
			}
			rideCarry = ride;
			playSpace.position += SweepEyes((ride + moveNow) * dt);
			if (cam && Mathf.Abs(turn) > 0.0001f)
			{
				playSpace.RotateAround(cam.position, Vector3.up, turn * 57.29578f * dt);
			}
			moveTime = Time.time;
		}

		private Vector3 RideHeld()
		{
			Vector3 sum = Vector3.zero;
			float count = 0f;
			if (Rides(grabL))
			{
				sum += grabL.held.GetComponentInParent<AvatarBody>().bodies[0].linearVelocity;
				count += 1f;
			}
			if (Rides(grabR))
			{
				sum += grabR.held.GetComponentInParent<AvatarBody>().bodies[0].linearVelocity;
				count += 1f;
			}
			if (count == 0f)
			{
				return Vector3.zero;
			}
			return new Vector3(sum.x / count, 0f, sum.z / count);
		}

		private static Vector3 FloorVelocity(Rigidbody rb, Vector3 at)
		{
			if (!rb)
			{
				return Vector3.zero;
			}
			if (TrainWreck.Ground(rb, out Vector3 velocity))
			{
				return velocity;
			}
			if (Drawbridge.Surface(rb, at, out velocity))
			{
				return velocity;
			}
			Hazard hazard = rb.isKinematic ? Hazard.Of(rb) : null;
			if (hazard)
			{
				return hazard.SurfaceVelocity(at);
			}
			if (rb.isKinematic || rb.GetComponentInParent<AvatarBody>() || rb.GetComponentInParent<Player>())
			{
				return Vector3.zero;
			}
			return rb.GetPointVelocity(at);
		}

		private Vector3 RideFloor(out float turn)
		{
			turn = 0f;
			if (!grounded)
			{
				return Vector3.zero;
			}
			Rigidbody rb = floorBody;
			if (!rb)
			{
				return Vector3.zero;
			}
			Vector3 at = new Vector3(pelvis.position.x, floorY, pelvis.position.z);
			if (TrainWreck.Ground(rb, out Vector3 velocity))
			{
				if (flopped || rising)
				{
					Vector3 own = pelvis.linearVelocity;
					return new Vector3(own.x, 0f, own.z);
				}
				if (gripping || hanging || HoldsStill(grabL) || HoldsStill(grabR))
				{
					return Vector3.zero;
				}
				return velocity;
			}
			if (Drawbridge.Surface(rb, at, out velocity))
			{
				if (gripping || hanging || HoldsStill(grabL) || HoldsStill(grabR))
				{
					return Vector3.zero;
				}
				return velocity;
			}
			Hazard hazard = rb.isKinematic ? Hazard.Of(rb) : null;
			if (hazard)
			{
				if (gripping || hanging || HoldsStill(grabL) || HoldsStill(grabR))
				{
					return Vector3.zero;
				}
				Vector3 surface = hazard.SurfaceVelocity(at);
				return new Vector3(surface.x, 0f, surface.z);
			}
			if (rb.isKinematic || rb.GetComponentInParent<AvatarBody>() || rb.GetComponentInParent<Player>())
			{
				return Vector3.zero;
			}
			Vector3 point = rb.GetPointVelocity(at);
			turn = rb.angularVelocity.y;
			return new Vector3(point.x, 0f, point.z);
		}

		private static bool HoldsStill(HandGrab g)
		{
			if (!g || !g.grabbing)
			{
				return false;
			}
			if (!g.held)
			{
				return true;
			}
			return g.held.isKinematic;
		}

		private static bool Rides(HandGrab g)
		{
			if (!g || !g.grabbing || !g.holdsYou || g.mutual || !g.held)
			{
				return false;
			}
			return g.held.GetComponentInParent<AvatarBody>();
		}

		private bool Firm(HandGrab g)
		{
			if (!g || !g.grabbing || !g.holdsYou)
			{
				return false;
			}
			if (!gripWalls && g.onWall)
			{
				return false;
			}
			if (!g.held)
			{
				return true;
			}
			return !g.held.GetComponentInParent<AvatarBody>();
		}

		private float Free()
		{
			if (moveWish.sqrMagnitude < 0.0001f)
			{
				return 1f;
			}
			float behind = -Vector3.Dot(bodyGap, moveWish.normalized);
			float from = holding ? stuckFrom + 0.25f : stuckFrom;
			return 1f - Mathf.Clamp01((behind - from) / Mathf.Max(stuckRange, 0.01f));
		}

		private Vector3 Tethered(Vector3 to)
		{
			float pull = Mathf.Min(tether.magnitude, 1f);
			if (pull <= 0.0001f)
			{
				return to;
			}
			Vector3 dir = tether.normalized;
			float along = Vector3.Dot(to, dir);
			if (along < 0f)
			{
				return to - dir * (pull * along);
			}
			return to;
		}

		private Vector3 Ramp(Vector3 to, float dt)
		{
			float rate = to.sqrMagnitude > moveWish.sqrMagnitude ? accel : decel;
			Vector3 next = Vector3.MoveTowards(moveWish, to, rate * dt);
			moveAccel = dt > 0f ? Vector3.ClampMagnitude((next - moveWish) / dt, Mathf.Max(accel, decel)) : Vector3.zero;
			return next;
		}

		private void Desk(float dt)
		{
			Keyboard k = null;
			if (!dummy)
			{
				k = Keyboard.current;
				if (k == null)
				{
					return;
				}
			}
			if (grabCursor && Cursor.lockState != CursorLockMode.Locked && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			if (k != null && k.escapeKey.wasPressedThisFrame)
			{
				Cursor.lockState = CursorLockMode.None;
			}
			if (Cursor.lockState == CursorLockMode.Locked && Mouse.current != null && !ignoreInput)
			{
				Vector2 delta = Mouse.current.delta.ReadValue();
				yaw += delta.x * look;
				pitch = Mathf.Clamp(pitch - delta.y * look, -maxPitch, maxPitch);
			}
			if (aiOn)
			{
				yaw = turnRate > 0f ? Mathf.MoveTowardsAngle(yaw, aiYaw, turnRate * dt) : aiYaw;
			}
			playSpace.rotation = Quaternion.Euler(0f, yaw, 0f);
			Vector3 keys = Vector3.zero;
			if (k != null)
			{
				if (k.wKey.isPressed)
				{
					keys.z += 1f;
				}
				if (k.sKey.isPressed)
				{
					keys.z -= 1f;
				}
				if (k.aKey.isPressed)
				{
					keys.x -= 1f;
				}
				if (k.dKey.isPressed)
				{
					keys.x += 1f;
				}
			}
			if (ignoreInput)
			{
				keys = Vector3.zero;
			}
			float pump = k != null && k.leftShiftKey.isPressed && !ignoreInput ? sprintMult : 1f;
			pumpNow = pump;
			Vector3 wish = frozen ? Vector3.zero : (testWish.sqrMagnitude > 0f ? testWish : keys);
			moveWish = Ramp(Tethered(playSpace.rotation * wish.normalized * (pump * speed)), dt);
			Move(dt);
			DeskClimb(k, dt);
			if (!cam)
			{
				return;
			}
			cam.localRotation = Quaternion.Euler(pitch, 0f, 0f);
			float height = k != null && k.leftCtrlKey.isPressed && !ignoreInput ? flatCrouch : flatHeight;
			cam.localPosition = new Vector3(0f, height, 0f) + neckPivot - cam.localRotation * neckPivot + testLean;
		}

		private void DeskClimb(Keyboard k, float dt)
		{
			if (!controllerL || !controllerR)
			{
				return;
			}
			if (!homesKnown)
			{
				homesKnown = true;
				homeL = new Vector3(-0.22f, 1.05f, 0.3f);
				homeR = new Vector3(0.22f, 1.05f, 0.3f);
			}
			float pullTo = testPull;
			float raiseTo;
			float pullRate;
			if (k != null && k.spaceKey.isPressed && !ignoreInput)
			{
				pullTo = deskPull;
			}
			if (k != null && k.qKey.isPressed && !ignoreInput)
			{
				raiseTo = deskRaise;
				pullRate = 1.5f;
			}
			else
			{
				raiseTo = testRaise;
				pullRate = ignoreInput ? testPullRate : 1.5f;
			}
			deskPullNow = Mathf.MoveTowards(deskPullNow, pullTo, pullRate * dt);
			deskRaiseNow = Mathf.MoveTowards(deskRaiseNow, raiseTo, dt * 1.5f);

			// Arms follow the look pitch: down by the sides when looking down, up overhead when looking up.
			float p = Mathf.Clamp(ignoreInput ? testPitch : pitch, -80f, 35f);
			float up = Mathf.Clamp01(p / 35f);
			float angle = p > 0f ? Mathf.Lerp(-55f, -60f, up) : Mathf.Lerp(-55f, 80f, p / -80f);
			armAngleNow = Mathf.MoveTowards(armAngleNow, angle, dt * 360f);
			HandGrab gL = leftHand.GetComponent<HandGrab>();
			HandGrab gR = rightHand.GetComponent<HandGrab>();
			bool holdL = gL && gL.grabbing && gL.holdsYou;
			bool holdR = gR && gR.grabbing && gR.holdsYou;
			float shoulderY = flatHeight - shoulderDrop;
			float rest = Mathf.Sqrt((homeR.y - shoulderY) * (homeR.y - shoulderY) + homeR.z * homeR.z);
			float reach = Mathf.Lerp(rest, 0.18f, up);
			float pulled = deskPull > 0f ? Mathf.Clamp01(deskPullNow / deskPull) : 0f;
			float pulledAngle = Mathf.Lerp(armAngleNow, -60f, pulled);
			float pulledReach = Mathf.Lerp(reach, 0.18f, pulled);
			if (gL && gL.grabbing)
			{
				angL = Mathf.Min(angL, holdL ? pulledAngle : armAngleNow);
				reachL = Mathf.Min(reachL, holdL ? pulledReach : reach);
			}
			else
			{
				angL = armAngleNow;
				reachL = reach;
			}
			if (gR && gR.grabbing)
			{
				angR = Mathf.Min(angR, holdR ? pulledAngle : armAngleNow);
				reachR = Mathf.Min(reachR, holdR ? pulledReach : reach);
			}
			else
			{
				angR = armAngleNow;
				reachR = reach;
			}
			Vector3 liftL = Vector3.up * (deskRaiseNow - deskPullNow);
			Vector3 liftR = liftL;
			if (holdL)
			{
				liftL = Vector3.up * deskRaiseNow;
			}
			if (holdR)
			{
				liftR = Vector3.up * deskRaiseNow;
			}
			controllerL.localPosition = liftL + DeskHandPlace(homeL.x, shoulderY, angL, reachL);
			controllerR.localPosition = liftR + DeskHandPlace(homeR.x, shoulderY, angR, reachR);
			if (handFollow != 0f)
			{
				controllerL.localRotation = Quaternion.Euler(handFollow * angL - 90f, 0f, 0f);
				controllerR.localRotation = Quaternion.Euler(handFollow * angR - 90f, 0f, 0f);
			}
			if (aiHandL)
			{
				controllerL.localPosition = aiHandPosL;
				controllerL.localRotation = aiHandRotL;
			}
			if (aiHandR)
			{
				controllerR.localPosition = aiHandPosR;
				controllerR.localRotation = aiHandRotR;
			}
			Mouse mouse = Mouse.current;
			CursorLockMode lockState = Cursor.lockState;
			bool grabL;
			bool grabR;
			if ((k != null && k.eKey.isPressed && !ignoreInput) || testGrab)
			{
				grabL = true;
				grabR = true;
			}
			else
			{
				grabL = testGrabL || (lockState == CursorLockMode.Locked && mouse != null && !ignoreInput && mouse.leftButton.isPressed);
				grabR = testGrabR || (lockState == CursorLockMode.Locked && mouse != null && !ignoreInput && mouse.rightButton.isPressed);
			}
			if (deskGrabL == grabL && deskGrabR == grabR)
			{
				return;
			}
			deskGrabL = grabL;
			deskGrabR = grabR;
			if (gL)
			{
				gL.forceGrab = grabL;
			}
			if (gR)
			{
				gR.forceGrab = grabR;
			}
		}

		private Vector3 DeskHandPlace(float x, float shoulderY, float angle, float reach)
		{
			float rad = angle * ((float)Math.PI / 180f);
			return new Vector3((1f - Mathf.Clamp01((angle + 55f) / 135f) * raiseNarrow) * x, shoulderY, 0f) + new Vector3(0f, Mathf.Sin(rad), Mathf.Cos(rad)) * reach;
		}

		private Vector2 Stick(ref UnityEngine.XR.InputDevice device, XRNode node)
		{
			if (!device.isValid)
			{
				device = InputDevices.GetDeviceAtXRNode(node);
			}
			return device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 axis) ? axis : Vector2.zero;
		}

		public static bool Either(UnityEngine.XR.InputDevice d, InputFeatureUsage<bool> button, InputFeatureUsage<bool> spare)
		{
			if (!d.isValid)
			{
				return false;
			}
			if (d.TryGetFeatureValue(button, out bool pressed))
			{
				return pressed;
			}
			return d.TryGetFeatureValue(spare, out bool held) && held;
		}

		public static void Hand(XRNode node, float amplitude, float seconds)
		{
			if (!Prefs.haptics)
			{
				return;
			}
			UnityEngine.XR.InputDevice device = InputDevices.GetDeviceAtXRNode(node);
			if (device.isValid)
			{
				device.SendHapticImpulse(0u, Mathf.Clamp01(amplitude), seconds);
			}
		}

		public static void Both(float amplitude, float seconds)
		{
			Hand(XRNode.LeftHand, amplitude, seconds);
			Hand(XRNode.RightHand, amplitude, seconds);
		}

		public static void BlockAt(Vector3 point)
		{
			if (!blockSounds)
			{
				blockSounds = UnityEngine.Object.FindFirstObjectByType<Player>();
			}
			if (blockSounds)
			{
				blockSounds.PlayBlock(point);
			}
			if (!spark)
			{
				spark = Resources.Load<GameObject>("Effects/Block");
			}
			if (spark)
			{
				UnityEngine.Object.Instantiate(spark, point, Quaternion.identity);
			}
		}

		public static void HitAt(Vector3 point, bool sound)
		{
			if (!hitFx)
			{
				hitFx = Resources.Load<GameObject>("Effects/Hit");
			}
			if (hitFx)
			{
				UnityEngine.Object.Instantiate(hitFx, point, Quaternion.identity);
			}
			if (!sound)
			{
				return;
			}
			if (!knockSounds)
			{
				knockSounds = UnityEngine.Object.FindFirstObjectByType<Player>();
			}
			if (knockSounds)
			{
				knockSounds.PunchAt(point);
			}
		}

		public static void KnockAt(Vector3 head)
		{
			if (!knockSounds)
			{
				knockSounds = UnityEngine.Object.FindFirstObjectByType<Player>();
			}
			if (knockSounds)
			{
				knockSounds.Knockout(head);
			}
			if (pops == null)
			{
				pops = new GameObject[2]
				{
					Resources.Load<GameObject>("Effects/Wham"),
					Resources.Load<GameObject>("Effects/Pow")
				};
			}
			GameObject pop = pops[UnityEngine.Random.Range(0, pops.Length)];
			if (pop)
			{
				UnityEngine.Object.Instantiate(pop, head + Vector3.up * 0.45f, Quaternion.identity);
			}
		}
	}
}
