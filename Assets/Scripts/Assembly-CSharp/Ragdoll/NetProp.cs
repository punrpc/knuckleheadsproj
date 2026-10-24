using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.XR;

namespace Ragdoll
{
	[RequireComponent(typeof(Rigidbody))]
	public class NetProp : NetworkBehaviour, IStateAuthorityChanged, IPublicFacingInterface
	{
		[Header("Claim")]
		public float claimWait = 0.5f;

		public bool hostOnly;

		[Header("Held")]
		public float follow;

		[Header("Pinned")]
		public bool pinned;

		public float breakImpulse = 45f;

		public float breakSpeed = 5f;

		public float breakPush = 0.5f;

		public float breakLoad;

		public float breakTime = 50f;

		[Header("Hitting")]
		public float armFrom = 2.5f;

		public float armTo = 1.5f;

		public float armHold = 0.3f;

		private Rigidbody rb;

		private HandGrab[] localHands;

		private float claimedAt = -9f;

		private Collider[] colliders;

		private Grabbable grab;

		private Collider[] holderColliders;

		private AvatarBody holderCopy;

		private NetworkId holderId;

		private bool armed = true;

		private bool hostSim;

		private int copiesSeen = -1;

		private float fastUntil = -9f;

		private float swingReach = -1f;

		private float keepLocalUntil = -9f;

		private float keepLocal = 1f;

		private float restSpeed = 0.5f;

		private NetworkTransform networkTransform;

		private PropertyReader<NetworkBool> heldReader;

		private PropertyReader<NetworkId> byReader;

		private PropertyReader<int> boneReader;

		private PropertyReader<Vector3> gripReader;

		private PropertyReader<Quaternion> turnReader;

		private PropertyReader<Vector3> tossReader;

		private PropertyReader<Vector3> tossSpinReader;

		private Vector3 shownToss;

		private Vector3 shownTossSpin;

		private bool shownHeld;

		private NetworkId shownBy;

		private int shownBone;

		private Vector3 shownGrip;

		private Quaternion shownTurn = Quaternion.identity;

		private float looseUntil = -9f;

		private float looseMax = 2.5f;

		private float settleSpeed = 0.3f;

		private float slowSince = -1f;

		private bool trackKnown;

		private Collider[] droppedColliders;

		private AvatarBody droppedCopy;

		private float droppedUntil;

		private float dropClear = 2f;

		private float minSpinDrag = 0.5f;

		private float looseFrom = -9f;

		private bool wasHeldHere;

		private bool ownNoted;

		private bool wasSharing;

		private float grabbedAt;

		private Vector3 trackPos;

		private Vector3 trackVel;

		private Collider[] bodyColliders;

		private bool ghosting;

		private float ghostFrom = 0.08f;

		private int localTouch = -1;

		private Quaternion netRot = Quaternion.identity;

		private bool netRotKnown;

		private float looseToss = 1.5f;

		private Vector3 blendFrom;

		private Quaternion blendFromRot = Quaternion.identity;

		private float blendAt = -9f;

		private float blendTime = 0.25f;

		private float settleLerp = 0.15f;

		private Vector3 netPos;

		private bool attached;

		private float attachGap = 0.06f;

		private Vector3 grabFrom;

		private Quaternion grabFromRot = Quaternion.identity;

		private float grabAt = -9f;

		private bool ghostFast;

		private float ghostOn = 2f;

		private float ghostOff = 0.7f;

		private float speedSmooth = 0.1f;

		private float netSpeed;

		private Vector3 lastNetPos;

		private bool netKnown;

		private Rigidbody lastRiding;

		[HideInInspector]
		public Rigidbody riding;

		private readonly List<Collider> fastStuckA = new List<Collider>();

		private readonly List<Collider> fastStuckB = new List<Collider>();

		private readonly List<Collider> stuckLocal = new List<Collider>();

		private readonly List<Collider> stuckMine = new List<Collider>();

		private static readonly List<NetProp> live = new List<NetProp>();

		private float load;

		private Vector3 restUp = Vector3.up;

		private float strength = 1f;

		private readonly List<Component> standing = new List<Component>();

		private readonly List<Rigidbody> pressing = new List<Rigidbody>();

		private readonly List<Rigidbody> carriers = new List<Rigidbody>();

		private static readonly List<Collider> pressCols = new List<Collider>();

		private static readonly Collider[] onTop = new Collider[32];

		private const float PlayerMass = 70f;

		private const float LoadSmooth = 0.25f;

		private const float TouchGap = 0.08f;

		private const int Still = 16;

		private readonly Vector3[] stillPos = new Vector3[Still];

		private readonly Quaternion[] stillRot = new Quaternion[Still];

		private int stillAt;

		private int stillCount;

		private Vector3 netVel;

		private Vector3 netSpin;

		private bool netVelKnown;

		private const float MaxCorrection = 10f;

		private const float StuckGap = 0.25f;

		private const float CrossGap = 0.04f;

		private const float StuckSpeed = 1f;

		private const float FreeDepth = 0.02f;

		private const float SoftDepenetration = 1f;

		private static readonly Collider[] wedged = new Collider[24];

		private static readonly RaycastHit[] crossing = new RaycastHit[16];

		private string crossed = "";

		private static readonly Collider[] nearby = new Collider[24];

		[Networked]
		public NetworkBool Held { get; set; }

		[Networked]
		public NetworkId By { get; set; }

		[Networked]
		public int Bone { get; set; }

		[Networked]
		public Vector3 Grip { get; set; }

		[Networked]
		public Quaternion Turn { get; set; }

		[Networked]
		public Vector3 Toss { get; set; }

		[Networked]
		public Vector3 TossSpin { get; set; }

		[Networked]
		public NetworkBool Broken { get; set; }

		[Networked]
		public NetworkBool Resting { get; set; }

		[Networked]
		public float Wear { get; set; }

		public bool ShownHeld => shownHeld;

		public bool Bolted => pinned && (!Object || !Object.IsValid || !Broken);

		public bool OnCarrier => riding || carriers.Exists(r => r != null);

		private bool KeepingLocal
		{
			get
			{
				if (HasStateAuthority)
				{
					return false;
				}
				if (!HeldByOther && Time.time < keepLocalUntil)
				{
					return true;
				}
				return !holderCopy && Time.time < looseUntil;
			}
		}

		private bool HeldByOther => Held && !Mine(By);

		public bool OtherHolds
		{
			get
			{
				if (!Object || !Object.IsValid)
				{
					return false;
				}
				return HeldByOther;
			}
		}

		public bool TeamToss
		{
			get
			{
				if (!Object || !Object.IsValid || Held || Resting || Mine(By))
				{
					return false;
				}
				if (!Runner.TryFindObject(By, out NetworkObject holder) || !holder.TryGetComponent(out NetworkPlayer player))
				{
					return false;
				}
				return GameManager.SameTeam(player.Uid.Value, GameManager.MyId);
			}
		}

		private static bool Mine(NetworkId id)
		{
			if (!NetworkPlayer.mine || !NetworkPlayer.mine.Object)
			{
				return false;
			}
			return NetworkPlayer.mine.Object.Id == id;
		}

		private void Awake()
		{
			restUp = Quaternion.Inverse(transform.rotation) * Vector3.up;
			strength = Random.Range(0.85f, 1.15f);
			if (pinned)
			{
				GetComponent<Rigidbody>().isKinematic = true;
			}
		}

		public override void Spawned()
		{
			rb = GetComponent<Rigidbody>();
			rb.angularDamping = Mathf.Max(rb.angularDamping, minSpinDrag);
			if (pinned)
			{
				rb.isKinematic = !Broken;
			}
			grab = GetComponent<Grabbable>();
			colliders = GetComponentsInChildren<Collider>();
			networkTransform = GetComponent<NetworkTransform>();
			if ((bool)networkTransform)
			{
				networkTransform.CustomForecast = ForecastInHand;
				networkTransform.CustomApplyCorrection = CorrectInHand;
			}
			heldReader = GetPropertyReader<NetworkBool>("Held");
			byReader = GetPropertyReader<NetworkId>("By");
			boneReader = GetPropertyReader<int>("Bone");
			gripReader = GetPropertyReader<Vector3>("Grip");
			turnReader = GetPropertyReader<Quaternion>("Turn");
			tossReader = GetPropertyReader<Vector3>("Toss");
			tossSpinReader = GetPropertyReader<Vector3>("TossSpin");
			live.Add(this);
		}

		private void TouchLocal(bool on)
		{
			if (colliders == null)
			{
				return;
			}
			if (localTouch != (on ? 1 : 0))
			{
				localTouch = on ? 1 : 0;
				stuckLocal.Clear();
				stuckMine.Clear();
				foreach (Grabbable grabbable in UnityEngine.Object.FindObjectsByType<Grabbable>(FindObjectsSortMode.None))
				{
					if (!AvatarBody.LocalOnly(grabbable) || grabbable.GetComponent<Rigidbody>().isKinematic)
					{
						continue;
					}
					foreach (Collider other in grabbable.GetComponentsInChildren<Collider>())
					{
						if (!other || !other.enabled)
						{
							continue;
						}
						foreach (Collider mine in colliders)
						{
							if (!mine || !mine.enabled)
							{
								continue;
							}
							bool stuck = on && Inside(other, mine);
							Physics.IgnoreCollision(other, mine, stuck || !on);
							if (stuck)
							{
								stuckLocal.Add(other);
								stuckMine.Add(mine);
							}
						}
					}
				}
				if (stuckLocal.Count > 0)
				{
					Player.Note(name + " is simulated here but inside " + stuckLocal[0].name + ", passing through until clear");
				}
			}
			for (int i = stuckLocal.Count - 1; i >= 0; i--)
			{
				Collider other = stuckLocal[i];
				Collider mine = stuckMine[i];
				if (other && mine && Inside(other, mine))
				{
					continue;
				}
				if (other && mine)
				{
					Physics.IgnoreCollision(other, mine, false);
				}
				stuckLocal.RemoveAt(i);
				stuckMine.RemoveAt(i);
			}
		}

		private void GhostFast(bool on)
		{
			if (ghostFast != on)
			{
				ghostFast = on;
				fastStuckA.Clear();
				fastStuckB.Clear();
				foreach (Grabbable grabbable in UnityEngine.Object.FindObjectsByType<Grabbable>(FindObjectsSortMode.None))
				{
					if (!AvatarBody.Prop(grabbable) || AvatarBody.LocalOnly(grabbable) || grabbable.transform == transform)
					{
						continue;
					}
					NetProp other = grabbable.GetComponent<NetProp>();
					if (!other)
					{
						continue;
					}
					if (on)
					{
						if (other.Object && other.Object.IsValid && other.Object.StateAuthority == Object.StateAuthority)
						{
							continue;
						}
					}
					else if (other.ghostFast)
					{
						continue;
					}
					foreach (Collider mine in colliders)
					{
						if (!mine || !mine.enabled)
						{
							continue;
						}
						foreach (Collider theirs in grabbable.GetComponentsInChildren<Collider>())
						{
							if (!theirs || !theirs.enabled)
							{
								continue;
							}
							if (on)
							{
								Physics.IgnoreCollision(mine, theirs, true);
								continue;
							}
							bool stuck = Inside(mine, theirs);
							Physics.IgnoreCollision(mine, theirs, stuck);
							if (stuck)
							{
								fastStuckA.Add(mine);
								fastStuckB.Add(theirs);
							}
						}
					}
				}
				if (on)
				{
					Player.Note(name + (holderCopy ? " held by player " + Object.StateAuthority.PlayerId : " flies at " + netSpeed.ToString("F1") + " m/s under player " + Object.StateAuthority.PlayerId) + ", passing through props here");
				}
			}
			for (int i = fastStuckA.Count - 1; i >= 0; i--)
			{
				Collider mine = fastStuckA[i];
				Collider theirs = fastStuckB[i];
				if (mine && theirs && Inside(mine, theirs))
				{
					continue;
				}
				if (mine && theirs)
				{
					Physics.IgnoreCollision(mine, theirs, false);
				}
				fastStuckA.RemoveAt(i);
				fastStuckB.RemoveAt(i);
			}
		}

		private void Hit(Collision c)
		{
			if (!pinned || !Object || !Object.IsValid || Broken || !c.rigidbody)
			{
				return;
			}
			float impulse = c.impulse.magnitude;
			float speed = c.relativeVelocity.magnitude;
			if (impulse < breakImpulse || speed < breakSpeed)
			{
				return;
			}
			Player player = c.rigidbody.GetComponentInParent<Player>();
			if (!player && (bool)c.rigidbody.GetComponentInParent<AvatarBody>())
			{
				return;
			}
			if ((bool)player && player.StandingLeg(c.rigidbody))
			{
				return;
			}
			NetProp prop = c.rigidbody.GetComponent<NetProp>();
			if (!((bool)player && player == Player.main) && !((bool)prop && prop.SimulatedHere()) && !HasStateAuthority)
			{
				return;
			}
			Vector3 at = (c.contactCount > 0) ? c.GetContact(0).point : rb.worldCenterOfMass;
			Vector3 away = rb.worldCenterOfMass - c.rigidbody.worldCenterOfMass;
			away.y = Mathf.Max(away.y, 0f);
			Vector3 push = away.normalized * impulse * breakPush;
			Player.Note(c.rigidbody.name + " hit " + name + " at " + impulse.ToString("F0") + " Ns " + speed.ToString("F1") + " m/s, breaking it off");
			if (HasStateAuthority)
			{
				Break(push, at);
			}
			else
			{
				RPC_Break(push, at);
			}
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void RPC_Break(Vector3 push, Vector3 at)
		{
			Break(push, at);
		}

		private void Break(Vector3 push, Vector3 at)
		{
			if (Broken)
			{
				return;
			}
			Broken = true;
			rb.isKinematic = false;
			Unwedge();
			rb.WakeUp();
			if (!LocalHandHolding() && !NetworkPlayer.HeldByOther(Object.Id))
			{
				rb.AddForceAtPosition(push, at, ForceMode.Impulse);
			}
		}

		private void Unwedge()
		{
			foreach (Collider mine in colliders)
			{
				if (!mine || !mine.enabled || mine.isTrigger)
				{
					continue;
				}
				int count = Physics.OverlapBoxNonAlloc(mine.bounds.center, mine.bounds.extents, wedged, Quaternion.identity, -1, QueryTriggerInteraction.Ignore);
				for (int i = 0; i < count; i++)
				{
					Collider other = wedged[i];
					if (!other || other.transform.IsChildOf(transform))
					{
						continue;
					}
					NetProp prop = other.GetComponentInParent<NetProp>();
					if (!prop || !prop.pinned || !other.attachedRigidbody || !other.attachedRigidbody.isKinematic)
					{
						continue;
					}
					if (Physics.ComputePenetration(mine, mine.transform.position, mine.transform.rotation, other, other.transform.position, other.transform.rotation, out Vector3 _, out float depth) && depth > 0.001f)
					{
						Physics.IgnoreCollision(mine, other, true);
					}
				}
			}
		}

		private bool SimulatedHere()
		{
			if (!Object || !Object.IsValid)
			{
				return false;
			}
			if (HasStateAuthority || (bool)LocalHandHolding())
			{
				return true;
			}
			return KeepingLocal;
		}

		private bool CopyIgnores(Collider a, Collider b, bool loose)
		{
			if (loose || hostSim)
			{
				return Inside(a, b);
			}
			if (!armed)
			{
				return true;
			}
			return a.bounds.Intersects(b.bounds);
		}

		private static bool Inside(Collider a, Collider b)
		{
			if (Physics.ComputePenetration(a, a.transform.position, a.transform.rotation, b, b.transform.position, b.transform.rotation, out Vector3 _, out float depth))
			{
				return depth > 0.01f;
			}
			return false;
		}

		public static void Loose(Collider[] body, bool on)
		{
			if (body == null)
			{
				return;
			}
			foreach (NetProp prop in live)
			{
				if (!prop || prop.colliders == null)
				{
					continue;
				}
				foreach (Collider mine in prop.colliders)
				{
					if (!mine || !mine.enabled)
					{
						continue;
					}
					foreach (Collider part in body)
					{
						if (part && part.enabled)
						{
							Physics.IgnoreCollision(mine, part, prop.CopyIgnores(mine, part, on));
						}
					}
				}
			}
		}

		public static void Unshare(Collider[] props)
		{
			foreach (NetProp prop in live)
			{
				if (prop && prop.localTouch == 0)
				{
					Pair(props, prop.colliders, true);
				}
			}
		}

		private static void Pair(Collider[] a, Collider[] b, bool ignore)
		{
			foreach (Collider first in a)
			{
				if (!first || !first.enabled)
				{
					continue;
				}
				foreach (Collider second in b)
				{
					if (second && second.enabled)
					{
						Physics.IgnoreCollision(first, second, ignore);
					}
				}
			}
		}

		private void ReadShown()
		{
			if (!HasStateAuthority && TryGetSnapshotsBuffers(out NetworkBehaviourBuffer from, out NetworkBehaviourBuffer to, out float alpha))
			{
				shownHeld = heldReader.Read(from) && heldReader.Read(to) && byReader.Read(from) == byReader.Read(to);
				shownBy = byReader.Read(to);
				shownBone = boneReader.Read(to);
				shownGrip = shownHeld ? Vector3.Lerp(gripReader.Read(from), gripReader.Read(to), alpha) : gripReader.Read(to);
				shownTurn = shownHeld ? Quaternion.Slerp(turnReader.Read(from), turnReader.Read(to), alpha) : turnReader.Read(to);
				shownToss = tossReader.Read(to);
				shownTossSpin = tossSpinReader.Read(to);
			}
			else
			{
				shownHeld = Held;
				shownBy = By;
				shownBone = Bone;
				shownGrip = Grip;
				shownTurn = Turn;
				shownToss = Toss;
				shownTossSpin = TossSpin;
			}
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			live.Remove(this);
			SetHolder(default);
		}

		public override void FixedUpdateNetwork()
		{
			if (!HasStateAuthority)
			{
				return;
			}
			HandGrab hand = LocalHandHolding();
			Resting = !hand && Stopped();
			if (Held && !hand)
			{
				Toss = rb.linearVelocity;
				TossSpin = rb.angularVelocity;
			}
			Held = (bool)hand;
			if (!hand || !NetworkPlayer.mine || !NetworkPlayer.mine.Object)
			{
				return;
			}
			Transform grip = hand.transform;
			By = NetworkPlayer.mine.Object.Id;
			Bone = (hand.node == XRNode.LeftHand) ? 5 : 8;
			Grip = Quaternion.Inverse(grip.rotation) * (rb.position - grip.position);
			Turn = Quaternion.Inverse(grip.rotation) * rb.rotation;
		}

		private void Loosen()
		{
			if (!grab || !grab.holdsYou)
			{
				return;
			}
			grab.holdsYou = false;
			if (Player.main)
			{
				Player.main.GaveWay(rb);
			}
		}

		private void FixedUpdate()
		{
			if (!Object || !Object.IsValid)
			{
				return;
			}
			if (pinned && rb.isKinematic == (bool)Broken)
			{
				rb.isKinematic = !Broken;
				if (Broken)
				{
					Unwedge();
					rb.WakeUp();
				}
			}
			if (pinned && Broken)
			{
				Loosen();
			}
			if (pinned && (bool)networkTransform && networkTransform.enabled == Bolted)
			{
				networkTransform.enabled = !Bolted;
			}
			if (pinned && breakLoad > 0f && HasStateAuthority && !Broken)
			{
				Strain();
			}
			if (Object.StateAuthority == PlayerRef.None && Runner.IsSharedModeMasterClient)
			{
				Claim(force: true);
			}
			HandGrab hand = LocalHandHolding();
			bool heldHere = hand;
			rb.maxDepenetrationVelocity = heldHere ? Physics.defaultMaxDepenetrationVelocity : SoftDepenetration;
			bool local = heldHere || HasStateAuthority || KeepingLocal;
			TouchLocal(local);
			Vector3 pos = riding ? (Quaternion.Inverse(riding.rotation) * (rb.position - riding.position)) : rb.position;
			Vector3 moved = riding ? (riding.rotation * (pos - lastNetPos)) : (pos - lastNetPos);
			float speed = (netKnown && riding == lastRiding) ? (moved.magnitude / Time.fixedDeltaTime) : netSpeed;
			lastNetPos = pos;
			lastRiding = riding;
			netKnown = true;
			netSpeed = Mathf.Lerp(netSpeed, speed, 1f - Mathf.Exp(-Time.fixedDeltaTime / speedSmooth));
			GhostFast(!local && ((bool)holderCopy || netSpeed > (ghostFast ? ghostOff : ghostOn)));
			Ghost(heldHere);
			if (!heldHere && !HasStateAuthority && !holderCopy && !KeepingLocal && netVelKnown && !rb.isKinematic)
			{
				Unstick();
			}
			NoteHolding(heldHere);
			if (heldHere && !HasStateAuthority && !NetworkPlayer.Holds(Object.StateAuthority, Object.Id))
			{
				Claim(force: true);
			}
			if (heldHere && !HasStateAuthority)
			{
				keepLocalUntil = Time.time + keepLocal;
			}
			foreach (HandGrab localHand in localHands)
			{
				if ((bool)localHand && localHand.grabbing && localHand.held == rb)
				{
					localHand.Tug(!HasStateAuthority);
				}
			}
			bool sharing = heldHere && !HasStateAuthority && HeldByOther;
			if (wasSharing != sharing)
			{
				wasSharing = sharing;
				if (sharing)
				{
					Player.Note("tug " + name + ", player " + Object.StateAuthority.PlayerId + " holds it");
				}
			}
			ReadShown();
			NetworkId holder = (!HasStateAuthority && shownHeld && !Mine(shownBy)) ? shownBy : default(NetworkId);
			if (holder != holderId)
			{
				netRotKnown = false;
				attached = false;
				if ((bool)holderCopy && !holder.IsValid)
				{
					blendFrom = rb.position;
					blendFromRot = rb.rotation;
					blendAt = Time.time;
				}
				if (holder.IsValid)
				{
					grabFrom = rb.position;
					grabFromRot = rb.rotation;
					grabAt = Time.time;
				}
				if ((bool)holderCopy && !holder.IsValid && shownToss.magnitude > looseToss)
				{
					looseUntil = Time.time + looseMax;
					looseFrom = Time.time;
					rb.WakeUp();
					rb.linearVelocity = shownToss;
					rb.angularVelocity = shownTossSpin;
				}
				trackKnown = false;
				SetHolder(holder);
			}
			ClearDropped();
			if (Time.time < looseUntil)
			{
				if (rb.linearVelocity.sqrMagnitude < settleSpeed * settleSpeed && rb.angularVelocity.sqrMagnitude < 1f)
				{
					float now = Time.time;
					if (slowSince < 0f)
					{
						slowSince = now;
					}
					else if (now > slowSince + 0.2f)
					{
						looseUntil = -9f;
					}
				}
				else
				{
					slowSince = -1f;
				}
			}
			if ((bool)holderCopy && !HasStateAuthority && HolderHandPose(out Vector3 handPos, out Quaternion handRot, out Vector3 _))
			{
				float dt = Time.fixedDeltaTime;
				trackVel = trackKnown ? Vector3.Lerp(trackVel, (handPos - trackPos) / dt, 0.5f) : Vector3.zero;
				trackPos = handPos;
				trackKnown = true;
				TrackHand(handPos, handRot, trackVel, dt);
			}
			bool swung = SwungFast();
			bool host = HasStateAuthority && Runner.IsSharedModeMasterClient;
			if (armed == swung && hostSim == host && copiesSeen == AvatarBody.copiesChanged)
			{
				return;
			}
			armed = swung;
			hostSim = host;
			copiesSeen = AvatarBody.copiesChanged;
			foreach (AvatarBody copy in AvatarBody.copies)
			{
				if (!copy || copy.cols == null)
				{
					continue;
				}
				bool copyLoose = copy.Loose();
				foreach (Collider mine in colliders)
				{
					foreach (Collider part in copy.cols)
					{
						if ((bool)mine && (bool)part)
						{
							Physics.IgnoreCollision(mine, part, CopyIgnores(mine, part, copyLoose));
						}
					}
				}
			}
			Ignore(holderColliders, on: true);
		}

		private void Strain()
		{
			float dt = Time.fixedDeltaTime;
			float mass = 0f;
			foreach (Rigidbody body in pressing)
			{
				if ((bool)body)
				{
					mass += body.mass;
				}
			}
			int players = PlayersOn();
			float tilt = Vector3.Cross(transform.rotation * restUp, Vector3.up).magnitude;
			float weight = mass + players * PlayerMass;
			load = Mathf.Lerp(load, weight * (tilt * Physics.gravity.magnitude), 1f - Mathf.Exp(-dt / LoadSmooth));
			float ratio = load / breakLoad;
			Wear += dt * Mathf.Min(ratio * (ratio * ratio), 20f) / Mathf.Max(breakTime * strength, 0.1f);
			if (Wear >= 1f)
			{
				Player.Note(name + " gave way under " + load.ToString("F0") + " N with " + standing.Count + " players on it");
				Break(Vector3.zero, rb.worldCenterOfMass);
			}
		}

		private void Load(Collision c)
		{
			if (breakLoad <= 0f || !pinned || !c.rigidbody || c.rigidbody.isKinematic)
			{
				return;
			}
			if (!Object || !Object.IsValid || !HasStateAuthority || Broken)
			{
				return;
			}
			if (c.impulse.sqrMagnitude < 0.0001f)
			{
				return;
			}
			if ((bool)c.rigidbody.GetComponentInParent<Player>() || (bool)c.rigidbody.GetComponentInParent<AvatarBody>())
			{
				return;
			}
			if (!pressing.Contains(c.rigidbody))
			{
				pressing.Add(c.rigidbody);
			}
		}

		private int PlayersOn()
		{
			standing.Clear();
			foreach (Collider mine in colliders)
			{
				Touching(mine);
			}
			foreach (Rigidbody body in pressing)
			{
				if (!body)
				{
					continue;
				}
				body.GetComponentsInChildren(pressCols);
				foreach (Collider part in pressCols)
				{
					Touching(part);
				}
			}
			for (int i = pressing.Count - 1; i >= 0; i--)
			{
				if (!pressing[i] || !pressing[i].IsSleeping())
				{
					pressing.RemoveAt(i);
				}
			}
			return standing.Count;
		}

		private void Touching(Collider mine)
		{
			if (!mine || !mine.enabled || mine.isTrigger)
			{
				return;
			}
			Vector3 center = mine.bounds.center;
			Vector3 half = mine.bounds.extents;
			MeshCollider mesh = mine as MeshCollider;
			BoxCollider box = mine as BoxCollider;
			Quaternion rot = Quaternion.identity;
			if ((bool)mesh && (bool)mesh.sharedMesh)
			{
				Bounds bounds = mesh.sharedMesh.bounds;
				center = mine.transform.TransformPoint(bounds.center);
				half = Vector3.Scale(bounds.extents, mine.transform.lossyScale);
				rot = mine.transform.rotation;
			}
			else if ((bool)box)
			{
				center = mine.transform.TransformPoint(box.center);
				half = Vector3.Scale(box.size * 0.5f, mine.transform.lossyScale);
				rot = mine.transform.rotation;
			}
			half = new Vector3(Mathf.Abs(half.x), Mathf.Abs(half.y), Mathf.Abs(half.z)) + Vector3.one * TouchGap;
			int count = Physics.OverlapBoxNonAlloc(center, half, onTop, rot, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < count; i++)
			{
				Rigidbody body = onTop[i].attachedRigidbody;
				if (!body)
				{
					continue;
				}
				Component who = body.GetComponentInParent<Player>();
				if (!who)
				{
					AvatarBody copy = body.GetComponentInParent<AvatarBody>();
					NetworkPlayer avatar = copy ? copy.GetComponent<NetworkPlayer>() : null;
					if ((bool)copy && !((bool)avatar && avatar.HasStateAuthority))
					{
						who = copy;
					}
				}
				if ((bool)who && !standing.Contains(who))
				{
					standing.Add(who);
				}
			}
		}

		private bool Crosses(Vector3 from, Vector3 to)
		{
			Vector3 delta = to - from;
			float distance = delta.magnitude;
			if (distance < 0.001f)
			{
				return false;
			}
			bool backfaces = Physics.queriesHitBackfaces;
			Physics.queriesHitBackfaces = true;
			int count = Physics.RaycastNonAlloc(from, delta / distance, crossing, distance, -1, QueryTriggerInteraction.Ignore);
			Physics.queriesHitBackfaces = backfaces;
			for (int i = 0; i < count; i++)
			{
				Collider hit = crossing[i].collider;
				if ((bool)hit && !hit.transform.IsChildOf(transform) && Solid(hit))
				{
					crossed = hit.name;
					return true;
				}
			}
			return false;
		}

		private bool FreeAt(Vector3 pos, Quaternion rot)
		{
			Quaternion delta = rot * Quaternion.Inverse(rb.rotation);
			foreach (Collider mine in colliders)
			{
				if (!mine || !mine.enabled || mine.isTrigger)
				{
					continue;
				}
				Vector3 at = pos + delta * (mine.transform.position - rb.position);
				Quaternion turn = delta * mine.transform.rotation;
				int count = Physics.OverlapSphereNonAlloc(at, mine.bounds.extents.magnitude, nearby, -1, QueryTriggerInteraction.Ignore);
				for (int i = 0; i < count; i++)
				{
					Collider other = nearby[i];
					if (!other || other.transform.IsChildOf(transform) || !Solid(other))
					{
						continue;
					}
					if (Physics.ComputePenetration(mine, at, turn, other, other.transform.position, other.transform.rotation, out Vector3 _, out float depth) && depth > FreeDepth)
					{
						return false;
					}
				}
			}
			return true;
		}

		private static bool Solid(Collider c)
		{
			Rigidbody body = c.attachedRigidbody;
			if (!body)
			{
				return true;
			}
			if ((bool)body.GetComponentInParent<Player>() || (bool)body.GetComponentInParent<AvatarBody>() || (bool)c.GetComponentInParent<NetProp>())
			{
				return false;
			}
			if (body.isKinematic)
			{
				return true;
			}
			return body.GetComponent<NetRope>();
		}

		private void Unstick()
		{
			Vector3 gap = netPos - rb.position;
			float distance = gap.magnitude;
			if (distance < CrossGap || netVel.sqrMagnitude > StuckSpeed * StuckSpeed)
			{
				return;
			}
			Vector3 center = rb.worldCenterOfMass - rb.position;
			bool cross = Crosses(rb.position + center, netPos + center);
			if (!cross)
			{
				if (distance < StuckGap || !rb.SweepTest(gap / distance, out RaycastHit hit, distance, QueryTriggerInteraction.Ignore) || !Solid(hit.collider) || Vector3.Dot(gap / distance, hit.normal) >= -0.5f)
				{
					return;
				}
				crossed = hit.collider.name;
			}
			if (!FreeAt(netPos, netRot))
			{
				return;
			}
			Player.Note(name + " was stuck " + (distance * 100f).ToString("F0") + " cm from where player " + Object.StateAuthority.PlayerId + " has it, behind " + crossed + ", moved through from " + rb.position.ToString("F2") + " to " + netPos.ToString("F2"));
			rb.position = netPos;
			rb.rotation = netRot;
			rb.linearVelocity = netVel;
			rb.angularVelocity = netSpin;
			transform.SetPositionAndRotation(netPos, netRot);
		}

		private void NoteHolding(bool heldHere)
		{
			if (wasHeldHere != heldHere)
			{
				wasHeldHere = heldHere;
				if (heldHere)
				{
					grabbedAt = Time.time;
					ownNoted = HasStateAuthority;
					Player.Note("grab " + name + " owner " + Object.StateAuthority.PlayerId + (HasStateAuthority ? " (me)" : "") + (Held ? (" held by " + By) : ""));
				}
				else
				{
					Player.Note("release " + name + (HasStateAuthority ? " owned" : (" NOT owned, owner " + Object.StateAuthority.PlayerId + " keeps its own copy")));
				}
			}
			else if (heldHere)
			{
				if (!ownNoted)
				{
					if (HasStateAuthority)
					{
						ownNoted = true;
						Player.Note("own " + name + " " + ((Time.time - grabbedAt) * 1000f).ToString("F0") + " ms after grab");
					}
				}
				else if (!HasStateAuthority)
				{
					ownNoted = false;
					Player.Note("lost " + name + " to player " + Object.StateAuthority.PlayerId + " while holding");
				}
			}
		}

		private bool SwungFast()
		{
			if (localHands == null)
			{
				LocalHandHolding();
			}
			bool wields = false;
			foreach (HandGrab hand in localHands)
			{
				if ((bool)hand)
				{
					wields |= hand.Wields(rb);
				}
			}
			if (!wields)
			{
				return false;
			}
			if (swingReach < 0f)
			{
				swingReach = MeasureReach();
			}
			float speed = Mathf.Max(rb.linearVelocity.magnitude, rb.angularVelocity.magnitude * swingReach);
			if (speed > (armed ? armTo : armFrom))
			{
				fastUntil = Time.time + armHold;
			}
			return Time.time < fastUntil;
		}

		private float MeasureReach()
		{
			float reach = 0f;
			foreach (Collider mine in colliders)
			{
				if ((bool)mine)
				{
					reach = Mathf.Max(reach, Vector3.Distance(mine.bounds.center, rb.worldCenterOfMass) + mine.bounds.extents.magnitude);
				}
			}
			return reach;
		}

		private void OnCollisionEnter(Collision c)
		{
			Load(c);
			Hit(c);
			if (Time.time < looseUntil && Time.time > looseFrom + 0.15f)
			{
				looseUntil = -9f;
			}
			if (Carrier(c) || !Object || !Object.IsValid || HasStateAuthority || !Takeable() || !c.rigidbody)
			{
				return;
			}
			if (c.relativeVelocity.magnitude < 1f)
			{
				return;
			}
			NetProp other = c.rigidbody.GetComponent<NetProp>();
			if ((bool)other && other.SimulatedHere() && (bool)other.LocalHandHolding())
			{
				Player.Note(other.name + " hit " + name + " at " + c.relativeVelocity.magnitude.ToString("F1") + " m/s, claiming it");
				Claim(force: false);
			}
		}

		private void OnCollisionExit(Collision c)
		{
			if ((bool)c.rigidbody)
			{
				carriers.Remove(c.rigidbody);
			}
		}

		private void OnCollisionStay(Collision c)
		{
			Load(c);
			if ((bool)c.rigidbody && !c.rigidbody.isKinematic && !carriers.Contains(c.rigidbody) && (bool)c.rigidbody.GetComponent<NetRope>())
			{
				carriers.Add(c.rigidbody);
			}
			if (Carrier(c) || !Object || !Object.IsValid || HasStateAuthority || !Takeable() || !c.rigidbody)
			{
				return;
			}
			Player player = c.rigidbody.GetComponentInParent<Player>();
			if ((bool)player && !(player != Player.main) && (c.rigidbody == player.leftHand || c.rigidbody == player.rightHand || c.rigidbody == player.leftForearm || c.rigidbody == player.rightForearm))
			{
				Claim(force: false);
			}
		}

		private bool Carrier(Collision c)
		{
			if (!Object || !Object.IsValid || HasStateAuthority || !Takeable() || !c.rigidbody)
			{
				return false;
			}
			NetRope rope = c.rigidbody.GetComponent<NetRope>();
			if (!rope || !rope.Object || !rope.Object.IsValid || !rope.HasStateAuthority)
			{
				return false;
			}
			Claim(force: false);
			return true;
		}

		public void StateAuthorityChanged()
		{
			stillCount = 0;
			if (HasStateAuthority && (bool)rb && !rb.isKinematic && !KeepingLocal && netVelKnown)
			{
				rb.linearVelocity = netVel;
				rb.angularVelocity = Vector3.ClampMagnitude(netSpin, 20f);
			}
		}

		private bool Takeable()
		{
			if (!Resting || NetworkPlayer.HeldByOther(Object.Id))
			{
				return false;
			}
			return !pinned || (bool)Broken;
		}

		private bool Stopped()
		{
			stillPos[stillAt] = rb.position;
			stillRot[stillAt] = rb.rotation;
			stillAt = (stillAt + 1) % Still;
			if (stillCount < Still)
			{
				stillCount++;
			}
			if (rb.isKinematic || rb.IsSleeping())
			{
				return true;
			}
			if (stillCount < Still)
			{
				return false;
			}
			for (int i = 0; i < Still; i++)
			{
				if ((stillPos[i] - rb.position).sqrMagnitude > 0.0001f || Quaternion.Angle(stillRot[i], rb.rotation) > 1f)
				{
					return false;
				}
			}
			return true;
		}

		public void Claim(bool force)
		{
			if (HasStateAuthority || Time.time < claimedAt + claimWait)
			{
				return;
			}
			if (hostOnly && !Runner.IsSharedModeMasterClient)
			{
				return;
			}
			if (!force && NetworkPlayer.HeldByOther(Object.Id))
			{
				return;
			}
			claimedAt = Time.time;
			stillCount = 0;
			Object.RequestStateAuthority();
		}

		private HandGrab LocalHandHolding()
		{
			if (localHands == null)
			{
				NetworkManager manager = UnityEngine.Object.FindFirstObjectByType<NetworkManager>();
				localHands = ((bool)manager && (bool)manager.localBody) ? manager.localBody.GetComponentsInChildren<HandGrab>() : new HandGrab[0];
			}
			foreach (HandGrab hand in localHands)
			{
				if ((bool)hand && hand.grabbing && hand.held == rb)
				{
					return hand;
				}
			}
			return null;
		}

		private void SetHolder(NetworkId id)
		{
			Collider[] wasColliders = holderColliders;
			AvatarBody wasCopy = holderCopy;
			holderId = id;
			holderCopy = null;
			holderColliders = null;
			if (id.IsValid && (bool)Runner && Runner.TryFindObject(id, out NetworkObject holder) && (bool)holder)
			{
				holderCopy = holder.GetComponent<AvatarBody>();
				holderColliders = holderCopy ? holderCopy.GetComponentsInChildren<Collider>() : null;
			}
			if (droppedColliders != null && droppedCopy != holderCopy)
			{
				Ignore(droppedColliders, on: false);
			}
			droppedColliders = null;
			droppedCopy = null;
			if (wasColliders != null && wasCopy != holderCopy)
			{
				droppedColliders = wasColliders;
				droppedCopy = wasCopy;
				droppedUntil = Time.time + dropClear;
			}
			Ignore(holderColliders, on: true);
		}

		private void IgnoreHolder(bool on)
		{
			Ignore(holderColliders, on);
		}

		private void Ghost(bool heldHere)
		{
			if (bodyColliders == null)
			{
				NetworkManager manager = UnityEngine.Object.FindFirstObjectByType<NetworkManager>();
				if (!manager || !manager.localBody)
				{
					return;
				}
				bodyColliders = manager.localBody.GetComponentsInChildren<Collider>();
			}
			bool fast = !HasStateAuthority && !heldHere && !KeepingLocal && netSpeed > (ghosting ? ghostOff : ghostOn);
			if (!ghosting && !HasStateAuthority && !heldHere && (fast || Inside(bodyColliders, ghostFrom)))
			{
				Ignore(bodyColliders, on: true);
				ghosting = true;
				Player.Note(name + (fast ? " flies at " + netSpeed.ToString("F1") + " m/s" : " is " + (ghostFrom * 100f).ToString("F0") + "+ cm inside me") + " but is owned by player " + Object.StateAuthority.PlayerId + ", passing through me until " + (fast ? "it slows down" : "clear"));
			}
			else if (ghosting && !heldHere && !fast && !Inside(bodyColliders, 0.01f))
			{
				Ignore(bodyColliders, on: false);
				ghosting = false;
			}
		}

		private bool Inside(Collider[] others, float depthFrom)
		{
			foreach (Collider mine in colliders)
			{
				if (!mine || !mine.enabled)
				{
					continue;
				}
				Bounds bounds = mine.bounds;
				foreach (Collider other in others)
				{
					if (!other || !other.enabled || other.isTrigger || !bounds.Intersects(other.bounds))
					{
						continue;
					}
					if (Physics.ComputePenetration(mine, mine.transform.position, mine.transform.rotation, other, other.transform.position, other.transform.rotation, out Vector3 _, out float depth) && depth > depthFrom)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void Ignore(Collider[] others, bool on)
		{
			if (others == null || colliders == null)
			{
				return;
			}
			foreach (Collider mine in colliders)
			{
				foreach (Collider other in others)
				{
					if ((bool)mine && (bool)other)
					{
						Physics.IgnoreCollision(mine, other, on);
					}
				}
			}
		}

		private void ClearDropped()
		{
			if (droppedColliders == null)
			{
				return;
			}
			if (Time.time < droppedUntil)
			{
				bool touching = false;
				foreach (Collider mine in colliders)
				{
					if (!mine)
					{
						continue;
					}
					Bounds bounds = mine.bounds;
					bounds.Expand(0.04f);
					foreach (Collider other in droppedColliders)
					{
						if ((bool)other && other.enabled)
						{
							touching |= bounds.Intersects(other.bounds);
						}
					}
				}
				if (touching)
				{
					return;
				}
			}
			Ignore(droppedColliders, on: false);
			droppedColliders = null;
			droppedCopy = null;
		}

		private bool HolderHandPose(out Vector3 pos, out Quaternion rot, out Vector3 vel)
		{
			pos = Vector3.zero;
			rot = Quaternion.identity;
			vel = Vector3.zero;
			if (!holderCopy || holderCopy.bones == null || shownBone < 0 || shownBone > 12)
			{
				return false;
			}
			Rigidbody body = holderCopy.driven ? holderCopy.bodies[shownBone] : null;
			Transform bone = holderCopy.bones[shownBone];
			Vector3 at = body ? body.position : bone.position;
			Quaternion turn = body ? body.rotation : bone.rotation;
			if (netRotKnown)
			{
				turn = netRot * Quaternion.Inverse(shownTurn);
			}
			pos = at + turn * shownGrip;
			rot = turn * shownTurn;
			if (netRotKnown && !attached)
			{
				if (Vector3.Distance(at, netPos - turn * shownGrip) > attachGap)
				{
					pos = netPos;
					rot = netRot;
				}
				else
				{
					attached = true;
				}
			}
			float t = (Time.time - grabAt) / blendTime;
			if (t < 1f)
			{
				float blend = t * t * (3f - 2f * t);
				pos = Vector3.Lerp(grabFrom, pos, blend);
				rot = Quaternion.Slerp(grabFromRot, rot, blend);
			}
			vel = body ? body.GetPointVelocity(pos) : Vector3.zero;
			return true;
		}

		private void TrackHand(Vector3 pos, Quaternion rot, Vector3 vel, float dt)
		{
			if (rb.isKinematic)
			{
				return;
			}
			float lag = Mathf.Max(follow, dt);
			rb.linearVelocity = vel + Vector3.ClampMagnitude((pos - rb.position) / lag, MaxCorrection);
			(rot * Quaternion.Inverse(rb.rotation)).ToAngleAxis(out float angle, out Vector3 axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			rb.angularVelocity = float.IsFinite(axis.x) ? (axis * (angle * Mathf.Deg2Rad / lag)) : Vector3.zero;
		}

		private void ForecastInHand(NetworkTransform nt, float dt, AbstractPhysicsBody body, float ticks, ref KinematicSnapshot local, ref KinematicSnapshot remote, out KinematicSnapshot result)
		{
			if (KeepingLocal || Bolted)
			{
				result = local;
				return;
			}
			netRot = remote.WorldRotation;
			netPos = remote.WorldPosition;
			netVel = remote.LinearVelocity;
			netSpin = remote.AngularVelocity;
			netVelKnown = true;
			if ((bool)holderCopy && nt.TryGetSnapshotsBuffers(out NetworkBehaviourBuffer from, out NetworkBehaviourBuffer to, out float alpha))
			{
				NetworkTRSPData a = from.ReinterpretState<NetworkTRSPData>();
				NetworkTRSPData b = to.ReinterpretState<NetworkTRSPData>();
				netPos = Vector3.Lerp(a.Position, b.Position, alpha);
				netRot = Quaternion.Slerp(a.Rotation, b.Rotation, alpha);
			}
			netRotKnown = holderCopy;
			if (HolderHandPose(out Vector3 pos, out Quaternion rot, out Vector3 vel))
			{
				result = remote;
				result.WorldPosition = pos;
				result.WorldRotation = rot;
				result.LinearVelocity = vel;
				result.IsSleeping = false;
				result.AngularVelocity = Vector3.zero;
				return;
			}
			NetworkTransformHelpers.ComputeExtrapolatedSnapshot(nt, dt, body, ticks, ref local, ref remote, out result);
			float t = (Time.time - blendAt) / blendTime;
			if (t < 1f)
			{
				float blend = t * t * (3f - 2f * t);
				result.WorldPosition = Vector3.Lerp(blendFrom, result.WorldPosition, blend);
				result.WorldRotation = Quaternion.Slerp(blendFromRot, result.WorldRotation, blend);
			}
		}

		private void CorrectInHand(NetworkTransform nt, float dt, float fps, AbstractPhysicsBody body, float since, ref KinematicSnapshot local, ref KinematicSnapshot remote, ref KinematicSnapshot result)
		{
			if (KeepingLocal || Bolted)
			{
				return;
			}
			if ((bool)holderCopy)
			{
				float lag = Mathf.Max(follow, dt);
				body.LinearVelocity = Vector3.ClampMagnitude((result.WorldPosition - local.WorldPosition) / lag, MaxCorrection) + result.LinearVelocity;
				(result.WorldRotation * Quaternion.Inverse(local.WorldRotation)).ToAngleAxis(out float angle, out Vector3 axis);
				if (angle > 180f)
				{
					angle -= 360f;
				}
				body.AngularVelocity = float.IsFinite(axis.x) ? (axis * (angle * Mathf.Deg2Rad / lag)) : Vector3.zero;
				return;
			}
			NetworkTransformHelpers.ApplyCorrection(nt, dt, fps, body, since, ref local, ref remote, ref result);
			if (!remote.IsSleeping && !(remote.LinearVelocity.sqrMagnitude < settleSpeed * settleSpeed && remote.AngularVelocity.sqrMagnitude < 1f))
			{
				return;
			}
			if (nt.PhysicsSettings.ImpactCorrectionTimeComplete < since)
			{
				body.Position = Vector3.Lerp(body.Position, result.WorldPosition, settleLerp);
				body.Rotation = Quaternion.Slerp(body.Rotation, result.WorldRotation, settleLerp);
			}
		}
	}
}
