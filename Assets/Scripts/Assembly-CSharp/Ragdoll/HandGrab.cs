using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Ragdoll
{
	public class HandGrab : MonoBehaviour
	{
		private static bool exactRestore = true;

		private static bool surfaceGrip = true;

		[Header("Hand")]
		public XRNode node = XRNode.RightHand;

		public Transform own;

		public float reach = 0.09f;

		public float heavyMass = 20f;

		public float throwWindow = 1.5f;

		[HideInInspector]
		public Rigidbody lastHeld;

		[HideInInspector]
		public float lastHeldAt = -99f;

		[Header("Stamina")]
		public float gripStamina = 7f;

		private float wallGrip = 2f;

		private const float TrainGrip = 10f;

		private float grabbedAt;

		private HandTouch touch;

		private GameObject worldHold;

		private bool topHold;

		private Collider worldWall;

		public float tiredSlip = 0.35f;

		public float lowStamina = 0.3f;

		[Header("Haptics")]
		public float grabBuzz = 0.25f;

		public float tiredBuzz = 0.7f;

		private float nextBuzz;

		[HideInInspector]
		public float stamina = 1f;

		public bool forceGrab;

		[HideInInspector]
		public bool grabbing;

		[HideInInspector]
		public string lastTry = "";

		[HideInInspector]
		public string lastRelease = "";

		[HideInInspector]
		public float lastTryAt = -1f;

		[HideInInspector]
		public float lastReleaseAt = -1f;

		[HideInInspector]
		public string releaseWhy;

		[HideInInspector]
		public bool holdsYou;

		[HideInInspector]
		public Rigidbody held;

		[HideInInspector]
		public float regrabUntil = -9f;

		[HideInInspector]
		public ArticulationBody heldLink;

		[HideInInspector]
		public Rope heldRope;

		private float ropeTilt = 45f;

		[HideInInspector]
		public HandTouch heldTouch;

		[HideInInspector]
		public Grabbable grabbable;

		[HideInInspector]
		public bool shared;

		public bool secondWeld = true;

		public bool passLegs;

		[HideInInspector]
		public bool heavySoft;

		public float heavyGrip;

		public float heavyDrag = 0.5f;

		public float heavySpinDrag = 1f;

		private static PhysicsMaterial heavyMat;

		[HideInInspector]
		public Quaternion gripRot = Quaternion.identity;

		private Rigidbody rb;

		private Collider col;

		private Joint joint;

		public float gripSpring = 100000f;

		public float gripDamp = 5000f;

		public float gripForce = 8000f;

		[HideInInspector]
		public bool mutual;

		private float mutualGrip = 150f;

		public float gripRamp = 0.2f;

		private float looseGap = 0.35f;

		public float gripTurn = 30f;

		public float gripTurnDamp = 3f;

		public float gripTurnForce = 10f;

		public float secondForce = 600f;

		private float rampTo;

		[HideInInspector]
		public bool tugging;

		private bool reeling;

		private Rigidbody slippedOff;

		private bool dropped;

		private float tugGrip = 250f;

		private float tugBreak = 0.35f;

		private float tugReach = 0.6f;

		private float tugWeld = 0.04f;

		private Vector3 tugAnchor;

		private float gripStart;

		private ConfigurableJoint softJoint;

		private InputDevice device;

		private readonly Collider[] hits = new Collider[64];

		private Collider[] ownCols;

		private readonly List<Collider[]> ignored = new List<Collider[]>();

		private readonly List<Collider[]> pending = new List<Collider[]>();

		private float pressedAt = -9f;

		private float pressGrace = 2.5f;

		private float gripGrab = 0.3f;

		private float gripHold = -1f;

		private float lastGrip;

		private readonly Vector3[] vel = new Vector3[12];

		private readonly Vector3[] spin = new Vector3[12];

		private int velAt;

		private bool velKnown;

		private bool swungSincePress;

		private float punchedAt = -9f;

		private const float PunchGrab = 0.25f;

		private const float PressWindow = 0.3f;

		[HideInInspector]
		public float tiredUntil;

		private Vector3 copyAt;

		private bool copyKnown;

		private const float CopyJump = 1f;

		private const float CopyStray = 1.5f;

		private float grabbedY;

		private Player mine;

		private Player downed;

		private bool rampDone;

		private bool wasWant;

		private readonly List<Joint> grips = new List<Joint>();

		public bool second => grabbing && shared && !holdsYou && !heavySoft;

		public bool onWall => grabbing && worldHold && held && held.gameObject == worldHold;

		public bool ledge => grabbing && topHold && worldHold && held && held.gameObject == worldHold;

		public Vector3 TugAnchor => tugAnchor;

		public Vector3 PinAt
		{
			get
			{
				if (softJoint && softJoint.connectedBody)
				{
					return softJoint.connectedBody.transform.TransformPoint(softJoint.connectedAnchor);
				}
				return transform.position;
			}
		}

		private void Awake()
		{
			rb = GetComponent<Rigidbody>();
			col = GetComponent<Collider>();
		}

		private bool Punching()
		{
			Player player = own ? own.GetComponent<Player>() : null;
			Rigidbody body = rb ? rb : GetComponent<Rigidbody>();
			if (player && body)
			{
				return player.SwingSpeed(body, 6) >= 3.5f;
			}
			return false;
		}

		public float Swing(int steps)
		{
			if (!velKnown)
			{
				return -1f;
			}
			float best = 0f;
			for (int i = 1; i <= steps && i <= vel.Length; i++)
			{
				best = Mathf.Max(best, vel[(velAt + vel.Length - i) % vel.Length].sqrMagnitude);
			}
			return Mathf.Sqrt(best);
		}

		private void FixedUpdate()
		{
			if (device.isValid && device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceVelocity, out Vector3 velocity))
			{
				vel[velAt] = velocity;
				spin[velAt] = device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceAngularVelocity, out Vector3 angular) ? angular : Vector3.zero;
				velKnown = true;
				velAt = (velAt + 1) % vel.Length;
			}
			bool want = forceGrab || Grip() > (grabbing ? gripHold : gripGrab);
			bool fresh = want && !wasWant;
			if (fresh)
			{
				pressedAt = Time.time;
			}
			bool punching = !grabbing && Punching();
			if (punching)
			{
				punchedAt = Time.time;
			}
			if (!want || fresh)
			{
				swungSincePress = false;
			}
			else if (punching)
			{
				swungSincePress = true;
			}
			wasWant = want;
			if (want)
			{
				if (OutCold())
				{
					dropped = true;
				}
				if (!grabbing && (forceGrab || Time.time < regrabUntil || (Time.time <= pressedAt + PressWindow && (fresh || Time.time >= punchedAt + PunchGrab))))
				{
					TryGrab(fresh);
				}
				else if (grabbing && OutCold())
				{
					releaseWhy = "out cold";
					Release();
				}
				else if (grabbing && held && CopyGone())
				{
					releaseWhy = "copy";
					Release();
				}
			}
			else
			{
				slippedOff = null;
				dropped = false;
				if (grabbing)
				{
					releaseWhy = "grip";
					Release();
				}
			}
			Stamina();
			Loose();
			if (grabbing && (!joint || (!held && !heldLink)))
			{
				releaseWhy = "joint";
				Release();
			}
			if (grabbing && softJoint)
			{
				Ramp();
			}
			if (grabbing && (tugging || reeling))
			{
				float stretch = Stretch();
				float far = tugging ? Reach() : stretch;
				if (far > (tugging ? tugReach : tugBreak))
				{
					if (Felt())
					{
						Player.Note("tug grip on " + held.name + " slipped, " + (tugging ? "controller " : "hand ") + far.ToString("F2") + " m from the grip");
					}
					slippedOff = held;
					releaseWhy = "tug";
					Release();
				}
				else if (reeling && stretch <= tugWeld)
				{
					Weld();
				}
			}
			Restore();
		}

		private void Stamina()
		{
			bool drain = false;
			bool rest = true;
			float time = gripStamina;
			if (grabbing && holdsYou)
			{
				drain = !OnGround() || (held && held.GetComponentInParent<AvatarBody>());
				rest = false;
				bool climb = grabbable && grabbable.climbTime > 0f;
				bool wall = worldHold && held && held.gameObject == worldHold;
				time = climb ? grabbable.climbTime : ((wall && !topHold) ? wallGrip : gripStamina);
			}
			if (drain && time > 0f)
			{
				stamina -= Time.fixedDeltaTime / time;
				if (stamina < lowStamina && Time.time >= nextBuzz && Felt())
				{
					nextBuzz = Time.time + 0.1f;
					Player.Hand(node, tiredBuzz * Mathf.Lerp(0.5f, 1f, 1f - stamina / lowStamina), 0.12f);
				}
				if (stamina <= 0f)
				{
					stamina = 0f;
					releaseWhy = "tired";
					tiredUntil = Time.time + tiredSlip;
					if (Felt())
					{
						Player.Hand(node, 1f, 0.3f);
					}
					Release();
				}
			}
			else if (rest && OnGround())
			{
				stamina = 1f;
			}
		}

		private bool OnGround()
		{
			if (!mine && own)
			{
				mine = own.GetComponent<Player>();
			}
			if (!mine)
			{
				return true;
			}
			return mine.grounded && mine.onFloor && !TrainWreck.Moving(mine.Standing);
		}

		private bool CopyGone()
		{
			AvatarBody body = held.GetComponentInParent<AvatarBody>();
			if (!body)
			{
				copyKnown = false;
				return false;
			}
			NetworkPlayer np = body.GetComponent<NetworkPlayer>();
			float jump = copyKnown ? Vector3.Distance(held.position, copyAt) : 0f;
			float stray = holdsYou ? body.Stray() : 0f;
			copyAt = held.position;
			copyKnown = true;
			if (jump > CopyJump)
			{
				if (Felt())
				{
					Player.Note("let go of " + body.name + ": its copy jumped " + jump.ToString("F1") + " m in one step");
				}
				return true;
			}
			if (stray > CopyStray)
			{
				if (Felt())
				{
					Player.Note("let go of " + body.name + ": its copy is " + stray.ToString("F1") + " m from where the network has it");
				}
				return true;
			}
			if (np)
			{
				return np.Dead;
			}
			return false;
		}

		private static bool DeadCopy(Grabbable g)
		{
			AvatarBody body = g.GetComponentInParent<AvatarBody>();
			NetworkPlayer np = body ? body.GetComponent<NetworkPlayer>() : null;
			if (np && np.Object && np.Object.IsValid)
			{
				return np.Dead;
			}
			return false;
		}

		private bool Felt()
		{
			if (!own)
			{
				return true;
			}
			return !own.GetComponent<SlapDummy>();
		}

		private float Pelvis()
		{
			if (!mine && own)
			{
				mine = own.GetComponent<Player>();
			}
			if (mine && mine.pelvis)
			{
				return mine.pelvis.position.y;
			}
			return transform.position.y;
		}

		private void Loose()
		{
			if (!grabbing || !holdsYou || !softJoint || !softJoint.connectedBody || !held)
			{
				return;
			}
			Vector3 pin = softJoint.connectedBody.transform.TransformPoint(softJoint.connectedAnchor);
			Vector3 hand = transform.TransformPoint(softJoint.anchor);
			float gap = Vector3.Distance(pin, hand);
			if (gap < looseGap)
			{
				return;
			}
			Rigidbody body = rb ? rb : GetComponent<Rigidbody>();
			Player.Note((node == XRNode.LeftHand ? "left" : "right") + " grip on " + held.name + " came loose: hand " + gap.ToString("F2") + " m from its pin, held " + (held.isKinematic ? "kinematic" : "dynamic") + " at " + held.linearVelocity.magnitude.ToString("F1") + " m/s, hand at " + (body ? body.linearVelocity.magnitude.ToString("F1") : "?") + " m/s, joint on " + softJoint.connectedBody.name + ", cap " + softJoint.yDrive.maximumForce.ToString("F0") + Owner() + ((held.gameObject == worldHold && worldWall) ? (", hold on " + worldWall.name + " scale " + worldWall.transform.lossyScale.ToString("F1") + (worldWall.attachedRigidbody ? (" body " + worldWall.attachedRigidbody.name + (worldWall.attachedRigidbody.isKinematic ? " kinematic" : " dynamic") + " at " + worldWall.attachedRigidbody.linearVelocity.magnitude.ToString("F1") + " m/s") : " static") + ", touched " + (Time.time - (touch ? (touch.worldUntil - touch.linger) : Time.time)).ToString("F2") + " s ago") : ""));
			releaseWhy = "loose";
			Release();
		}

		private string Owner()
		{
			NetRope rope = held.GetComponentInParent<NetRope>();
			if (rope && rope.Object && rope.Object.IsValid)
			{
				if (rope.HasStateAuthority)
				{
					return ", mine";
				}
				return ", owned by player " + rope.Object.StateAuthority.PlayerId;
			}
			NetProp prop = held.GetComponentInParent<NetProp>();
			if (prop && prop.Object && prop.Object.IsValid)
			{
				if (prop.HasStateAuthority)
				{
					return ", mine";
				}
				return ", owned by player " + prop.Object.StateAuthority.PlayerId;
			}
			return "";
		}

		private bool OutCold()
		{
			if (!mine && own)
			{
				mine = own.GetComponent<Player>();
			}
			if (mine && (mine.knockedOut || mine.dead))
			{
				return true;
			}
			if (!downed && own)
			{
				downed = own.GetComponent<Player>();
			}
			if (downed)
			{
				return downed.flopped || downed.rising;
			}
			return false;
		}

		private void Ramp()
		{
			float k = (gripRamp > 0f) ? Mathf.Clamp01((Time.time - gripStart) / gripRamp) : 1f;
			if (rampDone && k >= 1f)
			{
				return;
			}
			rampDone = k >= 1f;
			JointDrive drive = softJoint.xDrive;
			drive.maximumForce = Mathf.Lerp(0.1f, 1f, k) * rampTo;
			softJoint.xDrive = drive;
			softJoint.yDrive = drive;
			softJoint.zDrive = drive;
		}

		private float Grip()
		{
			if (!device.isValid)
			{
				device = InputDevices.GetDeviceAtXRNode(node);
			}
			if (device.isValid && device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.grip, out float value))
			{
				lastGrip = value;
				return value;
			}
			return grabbing ? lastGrip : 0f;
		}

		private void Sweep()
		{
			foreach (Joint j in grips)
			{
				if (j)
				{
					Object.DestroyImmediate(j);
					if (j)
					{
						Object.Destroy(j);
					}
				}
			}
			grips.Clear();
			joint = null;
			softJoint = null;
		}

		private void TryGrab(bool pressed)
		{
			if (!col)
			{
				return;
			}
			Sweep();
			if (worldHold)
			{
				Object.Destroy(worldHold);
				worldHold = null;
			}
			Bounds area = col.bounds;
			Vector3 at = area.center;
			area.Expand(reach * 2f);
			int count = Physics.OverlapBoxNonAlloc(area.center, area.extents, hits, Quaternion.identity, -1, QueryTriggerInteraction.Collide);
			lastTryAt = Time.time;
			lastTry = "none of " + count + ((count < hits.Length) ? "" : " FULL");
			Grabbable g = null;
			Rigidbody body = null;
			Collider c = null;
			ArticulationBody link = null;
			bool pick = false;
			float best = 1f;
			for (int i = 0; i < count; i++)
			{
				if (hits[i].isTrigger && hits[i].gameObject.layer != 10)
				{
					continue;
				}
				if (own && hits[i].transform.IsChildOf(own))
				{
					continue;
				}
				Grabbable hg = hits[i].GetComponentInParent<Grabbable>();
				if ((hg && !hg.enabled) || (hg && DeadCopy(hg)))
				{
					continue;
				}
				Rigidbody hb = hits[i].attachedRigidbody;
				bool ok = !hg && Pickable(hb);
				if (!hg && !ok)
				{
					continue;
				}
				ArticulationBody hl = hb ? null : hits[i].attachedArticulationBody;
				if (!hb && !hl)
				{
					continue;
				}
				Vector3 p = (hits[i] is MeshCollider mc && !mc.convex) ? hits[i].bounds.ClosestPoint(at) : hits[i].ClosestPoint(at);
				float d = Vector3.Distance(p, at);
				if (d < Mathf.Max(reach, hg ? hg.reach : (-1f)) && d < best)
				{
					best = d;
					body = hb;
					g = hg;
					c = hits[i];
					pick = ok;
					link = hl;
				}
			}
			if (!g && !pick && (OutCold() || Time.time < tiredUntil || (dropped && !pressed)))
			{
				lastTry = OutCold() ? "out cold" : ((Time.time < tiredUntil) ? "too tired" : "let go by the game, squeeze again");
				return;
			}
			if (!g && !pick && !WorldHold(out g, out body, out c))
			{
				return;
			}
			if (!g)
			{
				g = Adopt(body);
			}
			if (OutCold())
			{
				lastTry = "out cold";
				return;
			}
			if (g.holdsYou && (Time.time < tiredUntil || stamina <= 0f))
			{
				lastTry = "too tired";
				if (Felt() && Time.time >= nextBuzz)
				{
					nextBuzz = Time.time + 0.4f;
					Player.Hand(node, 1f, 0.2f);
				}
				return;
			}
			if (g.pressWhileTouching && !pressed && Time.time > pressedAt + pressGrace && Time.time > regrabUntil)
			{
				lastTry = "held, not pressed";
				return;
			}
			if (g.pressWhileTouching && (Punching() || (swungSincePress && !pressed)) && (g.GetComponentInParent<AvatarBody>() || g.GetComponentInParent<SlapDummy>()))
			{
				lastTry = "punching, not grabbing";
				return;
			}
			if (body && body == slippedOff && !pressed)
			{
				lastTry = "slipped off, squeeze again";
				return;
			}
			if (dropped && !pressed)
			{
				lastTry = "let go by the game, squeeze again";
				return;
			}
			lastTry = "got " + c.name + " " + best.ToString("F3") + " of " + count;
			if (surfaceGrip && !g.holdsYou && c && !c.isTrigger && col && (rb || GetComponent<Rigidbody>()))
			{
				Rigidbody hand = rb ? rb : GetComponent<Rigidbody>();
				if (Physics.ComputePenetration(col, col.transform.position, col.transform.rotation, c, c.transform.position, c.transform.rotation, out Vector3 direction, out float distance) && distance > 0.005f)
				{
					hand.position += direction * distance;
					transform.position += direction * distance;
				}
			}
			for (int j = pending.Count - 1; j >= 0; j--)
			{
				ignored.Add(pending[j]);
				pending.RemoveAt(j);
			}
			if (!g.holdsYou)
			{
				HandGrab other = Other();
				bool both = body && other && other.grabbing && other.held == body && !other.holdsYou;
				bool heavy = !both && heavyGrip > 0f && body.mass >= heavyMass;
				if ((both && !secondWeld) || heavy)
				{
					float force = heavy ? ((g.gripForce > 0f) ? g.gripForce : heavyGrip) : secondForce;
					ConfigurableJoint soft = gameObject.AddComponent<ConfigurableJoint>();
					soft.connectedBody = body;
					soft.anchor = Vector3.zero;
					soft.xMotion = ConfigurableJointMotion.Free;
					soft.yMotion = ConfigurableJointMotion.Free;
					soft.zMotion = ConfigurableJointMotion.Free;
					soft.angularXMotion = ConfigurableJointMotion.Free;
					soft.angularYMotion = ConfigurableJointMotion.Free;
					soft.angularZMotion = ConfigurableJointMotion.Free;
					JointDrive drive = new JointDrive
					{
						positionSpring = gripSpring,
						positionDamper = gripDamp,
						maximumForce = force
					};
					soft.xDrive = drive;
					soft.yDrive = drive;
					soft.zDrive = drive;
					soft.rotationDriveMode = RotationDriveMode.Slerp;
					soft.slerpDrive = new JointDrive
					{
						positionSpring = gripTurn,
						positionDamper = gripTurnDamp,
						maximumForce = gripTurnForce
					};
					soft.targetPosition = Vector3.zero;
					soft.targetRotation = Quaternion.identity;
					soft.enablePreprocessing = false;
					joint = soft;
					softJoint = soft;
					heavySoft = heavy;
					grips.Add(soft);
					gripStart = Time.time;
					rampTo = heavy ? force : ((g.gripForce > 0f) ? g.gripForce : secondForce);
					gripRot = Quaternion.Inverse(body.rotation) * transform.rotation;
					Ramp();
				}
				else
				{
					joint = gameObject.AddComponent<FixedJoint>();
					joint.connectedBody = body;
					grips.Add(joint);
				}
				shared = both;
				if (both)
				{
					gripRot = Quaternion.Inverse(body.rotation) * transform.rotation;
				}
			}
			else
			{
				ConfigurableJoint cj = gameObject.AddComponent<ConfigurableJoint>();
				Rope rope = c ? c.GetComponentInParent<Rope>() : null;
				if (rope && body)
				{
					Vector3 up = body.transform.up;
					Vector3 side = Vector3.Cross(up, transform.forward);
					if (side.sqrMagnitude < 0.01f)
					{
						side = Vector3.Cross(up, transform.right);
					}
					cj.axis = transform.InverseTransformDirection(up);
					cj.secondaryAxis = transform.InverseTransformDirection(side.normalized);
				}
				if (link)
				{
					cj.connectedArticulationBody = link;
				}
				else
				{
					cj.connectedBody = body;
				}
				cj.autoConfigureConnectedAnchor = !link;
				cj.anchor = Vector3.zero;
				if (link)
				{
					cj.connectedAnchor = link.transform.InverseTransformPoint(transform.position);
				}
				else if (rope && body && c)
				{
					Vector3 up2 = body.transform.up;
					Vector3 mid = c.bounds.center;
					Vector3 ext = c.bounds.extents;
					float limit = Mathf.Max(Mathf.Abs(up2.x) * ext.x + Mathf.Abs(up2.y) * ext.y + Mathf.Abs(up2.z) * ext.z - 0.02f, 0f);
					float along = Mathf.Clamp(Vector3.Dot(up2, at - mid), 0f - limit, limit);
					cj.autoConfigureConnectedAnchor = false;
					cj.anchor = transform.InverseTransformPoint(at);
					cj.connectedAnchor = body.transform.InverseTransformPoint(mid + up2 * along);
				}
				ConfigurableJointMotion linear = link ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free;
				cj.xMotion = linear;
				cj.yMotion = linear;
				cj.zMotion = linear;
				if (link)
				{
					cj.linearLimit = new SoftJointLimit
					{
						limit = 0.02f
					};
					cj.linearLimitSpring = new SoftJointLimitSpring
					{
						spring = gripSpring,
						damper = gripDamp
					};
				}
				cj.angularXMotion = rope ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked;
				cj.angularYMotion = rope ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;
				cj.angularZMotion = rope ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;
				if (rope)
				{
					cj.angularYLimit = new SoftJointLimit
					{
						limit = ropeTilt
					};
					cj.angularZLimit = new SoftJointLimit
					{
						limit = ropeTilt
					};
				}
				JointDrive drive2 = new JointDrive
				{
					positionSpring = gripSpring,
					positionDamper = gripDamp,
					maximumForce = gripForce
				};
				cj.xDrive = drive2;
				cj.yDrive = drive2;
				cj.zDrive = drive2;
				cj.targetPosition = Vector3.zero;
				cj.enablePreprocessing = false;
				joint = cj;
				softJoint = cj;
				grips.Add(cj);
				gripStart = Time.time;
				rampTo = gripForce;
				Ramp();
			}
			joint.enableCollision = false;
			joint.breakForce = g.breakForce;
			held = body;
			heldLink = link;
			heldRope = c ? c.GetComponentInParent<Rope>() : null;
			grabbing = true;
			holdsYou = g.holdsYou;
			grabbedAt = Time.time;
			grabbedY = Pelvis();
			if (Felt())
			{
				Player.Hand(node, grabBuzz, 0.04f);
			}
			grabbable = g;
			g.Slip(true);
			if (own && ownCols == null)
			{
				ownCols = own.GetComponentsInChildren<Collider>();
			}
			if (!holdsYou)
			{
				heldTouch = held.gameObject.AddComponent<HandTouch>();
				heldTouch.own = own;
				heldTouch.carried = true;
				heldTouch.holder = transform;
				if (ownCols == null)
				{
					return;
				}
				foreach (Collider a in held.GetComponentsInChildren<Collider>())
				{
					foreach (Collider b in ownCols)
					{
						if (Player.fingers.Contains(b))
						{
							continue;
						}
						if (b.transform == transform || b.transform.IsChildOf(transform))
						{
							Ignore(a, b);
						}
						else if (transform.parent && b.transform == transform.parent)
						{
							IgnoreWhileInside(a, b);
						}
						else if (passLegs && Low(b))
						{
							Ignore(a, b);
						}
					}
				}
				return;
			}
			List<Collider> mineCols = new List<Collider>();
			foreach (Collider x in GetComponentsInChildren<Collider>())
			{
				if (!Player.fingers.Contains(x))
				{
					mineCols.Add(x);
				}
			}
			Collider[] handCols = mineCols.ToArray();
			if (heldRope)
			{
				foreach (Collider r in heldRope.GetComponentsInChildren<Collider>())
				{
					foreach (Collider m in handCols)
					{
						Ignore(r, m);
					}
				}
			}
			else if (c)
			{
				foreach (Collider m2 in handCols)
				{
					Ignore(c, m2);
				}
			}
		}

		private void Ignore(Collider a, Collider b)
		{
			if (a && b)
			{
				Physics.IgnoreCollision(a, b, true);
				ignored.Add(new Collider[2] { a, b });
			}
		}

		private void IgnoreWhileInside(Collider a, Collider b)
		{
			if (!a || !b)
			{
				return;
			}
			Bounds bounds = a.bounds;
			bounds.Expand(0.02f);
			if (bounds.Intersects(b.bounds))
			{
				Physics.IgnoreCollision(a, b, true);
				pending.Add(new Collider[2] { a, b });
			}
		}

		private void Restore()
		{
			if (!grabbing && ignored.Count > 0)
			{
				pending.AddRange(ignored);
				ignored.Clear();
			}
			for (int i = pending.Count - 1; i >= 0; i--)
			{
				Collider a = pending[i][0];
				Collider b = pending[i][1];
				if (a && b)
				{
					Bounds ab = a.bounds;
					Bounds bb = b.bounds;
					ab.Expand(0.02f);
					if (ab.Intersects(bb) && (!exactRestore || Inside(a, b)))
					{
						continue;
					}
					Physics.IgnoreCollision(a, b, false);
				}
				pending.RemoveAt(i);
			}
		}

		private static bool Inside(Collider a, Collider b)
		{
			if (Physics.ComputePenetration(a, a.transform.position, a.transform.rotation, b, b.transform.position, b.transform.rotation, out Vector3 _, out float distance))
			{
				return distance > 0.005f;
			}
			return false;
		}

		private void Unignore(Collider wall)
		{
			if (!wall)
			{
				return;
			}
			for (int i = ignored.Count - 1; i >= 0; i--)
			{
				if (!(ignored[i][0] != wall))
				{
					if (ignored[i][1])
					{
						Physics.IgnoreCollision(wall, ignored[i][1], false);
					}
					ignored.RemoveAt(i);
				}
			}
			for (int j = pending.Count - 1; j >= 0; j--)
			{
				if (!(pending[j][0] != wall))
				{
					if (pending[j][1])
					{
						Physics.IgnoreCollision(wall, pending[j][1], false);
					}
					pending.RemoveAt(j);
				}
			}
		}

		private bool Low(Collider o)
		{
			Rigidbody body = o.attachedRigidbody;
			if (!body || !own)
			{
				return false;
			}
			Player player = own.GetComponent<Player>();
			Player owner = own.GetComponent<Player>();
			if (owner && body == owner.Body)
			{
				return true;
			}
			if (!player)
			{
				return false;
			}
			return body == player.pelvis || body == player.leftUpperLeg || body == player.leftLowerLeg || body == player.rightUpperLeg || body == player.rightLowerLeg;
		}

		private HandGrab Other()
		{
			if (!own)
			{
				return null;
			}
			foreach (HandGrab hand in own.GetComponentsInChildren<HandGrab>())
			{
				if (hand != this)
				{
					return hand;
				}
			}
			return null;
		}

		public void Mutual(bool on)
		{
			if (!grabbing || !holdsYou || !softJoint || mutual == on)
			{
				return;
			}
			mutual = on;
			if (on)
			{
				if (Felt())
				{
					Player.Hand(node, tiredBuzz, 0.15f);
				}
				rampTo = mutualGrip;
				gripStart = Time.time - gripRamp;
			}
			else
			{
				rampTo = gripForce;
				gripStart = Time.time;
			}
			rampDone = false;
			Ramp();
		}

		public void Tug(bool on)
		{
			if (!grabbing || !held || holdsYou || heldLink || tugging == on)
			{
				return;
			}
			tugging = on;
			if (on)
			{
				if (!reeling)
				{
					float breakForce = joint ? joint.breakForce : float.PositiveInfinity;
					Sweep();
					ConfigurableJoint cj = gameObject.AddComponent<ConfigurableJoint>();
					cj.connectedBody = held;
					cj.anchor = Vector3.zero;
					cj.xMotion = ConfigurableJointMotion.Free;
					cj.yMotion = ConfigurableJointMotion.Free;
					cj.zMotion = ConfigurableJointMotion.Free;
					cj.angularXMotion = ConfigurableJointMotion.Free;
					cj.angularYMotion = ConfigurableJointMotion.Free;
					cj.angularZMotion = ConfigurableJointMotion.Free;
					JointDrive drive = new JointDrive
					{
						positionSpring = gripSpring,
						positionDamper = gripDamp,
						maximumForce = tugGrip
					};
					cj.xDrive = drive;
					cj.yDrive = drive;
					cj.zDrive = drive;
					cj.rotationDriveMode = RotationDriveMode.Slerp;
					cj.slerpDrive = new JointDrive
					{
						positionSpring = gripTurn,
						positionDamper = gripTurnDamp,
						maximumForce = gripTurnForce
					};
					cj.targetPosition = Vector3.zero;
					cj.targetRotation = Quaternion.identity;
					cj.enablePreprocessing = false;
					cj.enableCollision = false;
					cj.breakForce = breakForce;
					tugAnchor = held.transform.InverseTransformPoint(transform.position);
					joint = cj;
					softJoint = cj;
					grips.Add(cj);
				}
				else
				{
					reeling = false;
				}
				rampTo = tugGrip;
				gripStart = Time.time - gripRamp;
			}
			else
			{
				reeling = true;
				rampTo = gripForce;
				gripStart = Time.time;
			}
			rampDone = false;
		}

		private void Weld()
		{
			float breakForce = joint ? joint.breakForce : float.PositiveInfinity;
			Sweep();
			FixedJoint fixedJoint = gameObject.AddComponent<FixedJoint>();
			fixedJoint.connectedBody = held;
			fixedJoint.enableCollision = false;
			fixedJoint.breakForce = breakForce;
			joint = fixedJoint;
			grips.Add(fixedJoint);
			reeling = false;
			rampDone = false;
		}

		private float Reach()
		{
			if (!mine && own)
			{
				mine = own.GetComponent<Player>();
			}
			Transform target = mine ? ((node == XRNode.LeftHand) ? mine.handTargetL : mine.handTargetR) : null;
			if (held && target)
			{
				return Vector3.Distance(target.position, held.transform.TransformPoint(tugAnchor));
			}
			return Stretch();
		}

		private float Stretch()
		{
			if (!held)
			{
				return 0f;
			}
			return Vector3.Distance(transform.position, held.transform.TransformPoint(tugAnchor));
		}

		public void Harden()
		{
			if (!softJoint || !held || holdsYou || heavySoft || tugging || reeling)
			{
				return;
			}
			if (heavyGrip > 0f && held.mass >= heavyMass)
			{
				heavySoft = true;
				rampTo = heavyGrip;
				JointDrive drive = softJoint.xDrive;
				drive.maximumForce = heavyGrip;
				softJoint.xDrive = drive;
				softJoint.yDrive = drive;
				softJoint.zDrive = drive;
				rampDone = true;
				return;
			}
			float breakForce = softJoint.breakForce;
			Object.DestroyImmediate(softJoint);
			if (softJoint)
			{
				Object.Destroy(softJoint);
			}
			FixedJoint fixedJoint = gameObject.AddComponent<FixedJoint>();
			fixedJoint.connectedBody = held;
			fixedJoint.enableCollision = false;
			fixedJoint.breakForce = breakForce;
			grips.Remove(softJoint);
			grips.Add(fixedJoint);
			joint = fixedJoint;
			softJoint = null;
			rampDone = false;
		}

		public void Soften()
		{
			if (!grabbing || !held || holdsYou || softJoint || tugging)
			{
				return;
			}
			float breakForce = joint ? joint.breakForce : float.PositiveInfinity;
			Sweep();
			ConfigurableJoint cj = gameObject.AddComponent<ConfigurableJoint>();
			cj.connectedBody = held;
			cj.anchor = Vector3.zero;
			cj.xMotion = ConfigurableJointMotion.Free;
			cj.yMotion = ConfigurableJointMotion.Free;
			cj.zMotion = ConfigurableJointMotion.Free;
			cj.angularXMotion = ConfigurableJointMotion.Free;
			cj.angularYMotion = ConfigurableJointMotion.Free;
			cj.angularZMotion = ConfigurableJointMotion.Free;
			JointDrive drive = new JointDrive
			{
				positionSpring = gripSpring,
				positionDamper = gripDamp,
				maximumForce = secondForce
			};
			cj.xDrive = drive;
			cj.yDrive = drive;
			cj.zDrive = drive;
			cj.rotationDriveMode = RotationDriveMode.Slerp;
			cj.slerpDrive = new JointDrive
			{
				positionSpring = gripTurn,
				positionDamper = gripTurnDamp,
				maximumForce = gripTurnForce
			};
			cj.targetPosition = Vector3.zero;
			cj.targetRotation = Quaternion.identity;
			cj.enablePreprocessing = false;
			cj.enableCollision = false;
			cj.breakForce = breakForce;
			joint = cj;
			softJoint = cj;
			grips.Add(cj);
			rampTo = secondForce;
			rampDone = false;
			gripStart = Time.time - gripRamp;
			Ramp();
		}

		public void Drop(string why)
		{
			if (grabbing)
			{
				slippedOff = held;
				dropped = true;
				releaseWhy = why;
				Release();
			}
		}

		public void GaveWay()
		{
			if (grabbing)
			{
				slippedOff = held;
				releaseWhy = "gave way";
				Release();
			}
		}

		public void Rebind(Transform npc)
		{
			if (!wasWant || dropped || swungSincePress || !col || !npc)
			{
				return;
			}
			if (grabbing && (!onWall || !OnGround()))
			{
				return;
			}
			Bounds box = col.bounds;
			Vector3 center = box.center;
			box.Expand(reach + reach);
			Grabbable grab = npc.GetComponentInChildren<Grabbable>();
			float far = Mathf.Max(reach, grab ? grab.reach : -1f);
			int n = Physics.OverlapBoxNonAlloc(box.center, box.extents, hits, Quaternion.identity, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < n; i++)
			{
				if (!hits[i].transform.IsChildOf(npc))
				{
					continue;
				}
				Vector3 p = hits[i] is MeshCollider mesh && !mesh.convex ? hits[i].bounds.ClosestPoint(center) : hits[i].ClosestPoint(center);
				if (Vector3.Distance(p, center) < far)
				{
					if (grabbing)
					{
						releaseWhy = "rebind";
						Release();
					}
					TryGrab(true);
					return;
				}
			}
		}

		public void Release()
		{
			lastRelease = releaseWhy ?? "other";
			copyKnown = false;
			lastReleaseAt = Time.time;
			if (holdsYou && held && Felt() && (lastRelease != "grip" || mutual || Time.time > grabbedAt + 1f))
			{
				Player.Note((node == XRNode.LeftHand ? "left" : "right") + " hand let go of " + held.name + " (climb) after " + (Time.time - grabbedAt).ToString("F1") + " s: " + lastRelease + (mutual ? ", weak grip" : "") + ", stamina " + stamina.ToString("F2") + ", pelvis rose " + ((Pelvis() - grabbedY) * 100f).ToString("F0") + " cm" + Owner());
			}
			if (holdsYou && held && Felt() && lastRelease != "grip")
			{
				Player.Hand(node, tiredBuzz, 0.15f);
			}
			releaseWhy = null;
			HandGrab other = Other();
			bool both = other && other.grabbing && other.grabbable && other.grabbable == grabbable && grabbable;
			Sweep();
			rampDone = false;
			if (held && !both && rb && !holdsYou)
			{
				Vector3 velocity = rb.linearVelocity;
				Vector3 angular = Vector3.zero;
				Player player = own ? own.GetComponent<Player>() : null;
				if (velKnown && player && player.playSpace)
				{
					int fastest = 0;
					for (int i = 1; i < vel.Length; i++)
					{
						if (vel[i].sqrMagnitude > vel[fastest].sqrMagnitude)
						{
							fastest = i;
						}
					}
					velocity = player.playSpace.rotation * vel[fastest] + player.spaceVel;
					angular = rb.angularVelocity;
				}
				if (held.mass < heavyMass && !held.isKinematic)
				{
					held.linearVelocity = velocity;
					held.angularVelocity = angular;
				}
			}
			if (held && !holdsYou)
			{
				lastHeld = held;
				lastHeldAt = Time.time;
			}
			if (heldTouch)
			{
				Object.Destroy(heldTouch);
			}
			if (grabbable && !both)
			{
				grabbable.Slip(false);
			}
			else if (both && held && other.held == held)
			{
				other.Harden();
				other.shared = false;
			}
			grabbable = null;
			joint = null;
			heldTouch = null;
			held = null;
			heldLink = null;
			heldRope = null;
			GameObject hold = worldHold;
			tugging = false;
			reeling = false;
			holdsYou = false;
			mutual = false;
			heavySoft = false;
			shared = false;
			grabbing = false;
			if (hold)
			{
				Unignore(worldWall);
				Object.Destroy(worldHold);
				worldHold = null;
				worldWall = null;
			}
		}

		private bool WorldHold(out Grabbable g, out Rigidbody body, out Collider c)
		{
			g = null;
			body = null;
			c = null;
			if (!touch)
			{
				touch = GetComponent<HandTouch>();
			}
			if (!touch || !touch.worldCol || Time.time > touch.worldUntil)
			{
				return false;
			}
			c = touch.worldCol;
			GameObject hold = new GameObject("Hold");
			hold.transform.SetParent(c.transform, true);
			hold.transform.position = touch.worldPoint;
			body = hold.AddComponent<Rigidbody>();
			body.isKinematic = true;
			body.useGravity = false;
			g = hold.AddComponent<Grabbable>();
			g.holdsYou = true;
			if (TrainWreck.Car(c))
			{
				g.climbTime = TrainGrip;
			}
			worldHold = hold;
			worldWall = c;
			topHold = touch.worldTop;
			return true;
		}

		private static bool Pickable(Rigidbody body)
		{
			if (!body || body.isKinematic || body.GetComponentInParent<Player>())
			{
				return false;
			}
			return !body.GetComponentInParent<Rope>();
		}

		private Grabbable Adopt(Rigidbody body)
		{
			Grabbable g = body.gameObject.AddComponent<Grabbable>();
			bool heavy = body.mass >= heavyMass;
			if (heavy)
			{
				g.friction = -1f;
				if (!heavyMat)
				{
					heavyMat = new PhysicsMaterial("Heavy");
					heavyMat.staticFriction = 1.2f;
					heavyMat.dynamicFriction = 1f;
					heavyMat.frictionCombine = PhysicsMaterialCombine.Maximum;
				}
			}
			foreach (Collider c in body.GetComponentsInChildren<Collider>())
			{
				Player.IgnoreFingers(c);
				if (heavy && !c.sharedMaterial)
				{
					c.sharedMaterial = heavyMat;
				}
			}
			if (heavy)
			{
				body.linearDamping = Mathf.Max(body.linearDamping, heavyDrag);
				body.angularDamping = Mathf.Max(body.angularDamping, heavySpinDrag);
			}
			body.maxDepenetrationVelocity = 1f;
			return g;
		}

		public bool Wields(Rigidbody rb)
		{
			if (!rb)
			{
				return false;
			}
			if (grabbing && !holdsYou && held == rb)
			{
				return true;
			}
			if (lastHeld == rb)
			{
				return Time.time < lastHeldAt + throwWindow;
			}
			return false;
		}

		public Vector3 HeldPoint()
		{
			if (held)
			{
				return held.position;
			}
			if (heldLink)
			{
				return heldLink.transform.position;
			}
			return transform.position;
		}

		private void OnJointBreak(float force)
		{
		}
	}
}
