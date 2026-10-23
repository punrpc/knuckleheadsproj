using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Ragdoll
{
	public class AvatarBody : MonoBehaviour
	{
		public class AvatarHit : MonoBehaviour
		{
			[HideInInspector]
			public AvatarBody owner;

			[HideInInspector]
			public int index;

			private void OnCollisionEnter(Collision c)
			{
				if (!owner || !c.rigidbody)
				{
					return;
				}
				if (c.rigidbody.GetComponentInParent<AvatarBody>())
				{
					return;
				}
				owner.Impact(index, c.impulse.magnitude);
			}
		}

		public class FistWatch : MonoBehaviour
		{
			[HideInInspector]
			public AvatarHit touching;

			[HideInInspector]
			public Vector3 local;

			[HideInInspector]
			public Vector3 push;

			[HideInInspector]
			public float at = -9f;

			[HideInInspector]
			public bool touchOnly;

			private readonly Dictionary<AvatarBody, float> lastTouch = new Dictionary<AvatarBody, float>();

			private readonly Dictionary<AvatarBody, bool> landed = new Dictionary<AvatarBody, bool>();

			private readonly Dictionary<AvatarBody, float> landedAt = new Dictionary<AvatarBody, float>();

			private static bool rehit = true;

			private const float RehitGap = 0.35f;

			private Rigidbody body;

			private Player me;

			private int spent;

			private void FixedUpdate()
			{
				if (!swingRule || spent == 0 || touchOnly)
				{
					return;
				}
				if (!body)
				{
					body = GetComponent<Rigidbody>();
				}
				if (!me)
				{
					me = body ? body.GetComponentInParent<Player>() : null;
				}
				if (!me || me.SwingSpeed(body, 1) >= 1.5f)
				{
					return;
				}
				spent = 0;
				landed.Clear();
			}

			private bool Swung(AvatarHit hit, Rigidbody fist, ref float speed, out float swing)
			{
				swing = -1f;
				if (!me)
				{
					me = fist.GetComponentInParent<Player>();
				}
				if (!me)
				{
					return true;
				}
				swing = me.SwingSpeed(fist, Player.SwingSteps);
				if (landed.TryGetValue(hit.owner, out bool done) && done)
				{
					if (speed >= 3.5f && swing >= 3.5f)
					{
						Player.Note("punch NOT counted: " + fist.name + " hit " + NetworkPlayer.Names[hit.index] + " at " + speed.ToString("F1") + " m/s, this swing already landed on them (the hand must slow under " + 1.5f + " m/s before the next punch)");
					}
					return false;
				}
				if (speed >= 3.5f && swing < 3.5f)
				{
					Player.Note("push, not a punch: " + fist.name + " on " + NetworkPlayer.Names[hit.index] + ", hand " + speed.ToString("F1") + " m/s, swing " + swing.ToString("F1") + " m/s");
				}
				speed = Mathf.Min(speed, swing);
				return true;
			}

			private void OnCollisionStay(Collision c)
			{
				AvatarHit hit = c.collider ? c.collider.GetComponent<AvatarHit>() : null;
				if (!hit || !hit.owner)
				{
					return;
				}
				hit.owner.Touched(hit.index);
				if (touchOnly)
				{
					return;
				}
				lastTouch[hit.owner] = Time.fixedTime;
				Vector3 off = c.collider.bounds.center - transform.position;
				if (off.sqrMagnitude < 1E-06f)
				{
					return;
				}
				Vector3 point = c.contactCount > 0 ? c.GetContact(0).point : transform.position;
				touching = hit;
				local = hit.transform.InverseTransformPoint(point);
				push = off.normalized * (c.impulse.magnitude / Time.fixedDeltaTime);
				at = Time.time;
			}

			private void OnCollisionEnter(Collision c)
			{
				AvatarHit hit = c.collider ? c.collider.GetComponent<AvatarHit>() : null;
				if (!hit || !hit.owner)
				{
					return;
				}
				hit.owner.Touched(hit.index);
				if (touchOnly)
				{
					return;
				}
				Rigidbody fist = GetComponent<Rigidbody>();
				if (!fist)
				{
					return;
				}
				bool fresh = !lastTouch.TryGetValue(hit.owner, out float last) || Time.fixedTime - last > Time.fixedDeltaTime * 1.5f;
				lastTouch[hit.owner] = Time.fixedTime;
				if (fresh && (!swingRule || !hit.owner.IsLocalFist(fist)))
				{
					landed[hit.owner] = false;
				}
				bool arm = hit.index == 4 || hit.index == 5 || hit.index == 7 || hit.index == 8;
				float handSpeed = hit.owner.SpeedBefore(fist);
				float speed = handSpeed;
				float swing = -1f;
				bool swung = false;
				if (swingRule && hit.owner.IsLocalFist(fist))
				{
					if (!Swung(hit, fist, ref speed, out swing))
					{
						return;
					}
					swung = true;
				}
				else if (landed.TryGetValue(hit.owner, out bool done) && done)
				{
					float closing = (hit.owner.VelocityBefore(fist) - hit.owner.bodies[0].linearVelocity).magnitude;
					float landedTime = -9f;
					if (rehit && !arm && closing >= 3f && landedAt.TryGetValue(hit.owner, out landedTime) && Time.fixedTime - landedTime >= RehitGap)
					{
						if (hit.owner.IsLocalFist(fist))
						{
							Player.Note("punch counts again in the same contact: " + fist.name + " closing at " + closing.ToString("F1") + " m/s, " + (Time.fixedTime - landedTime).ToString("F2") + " s after its last hit");
						}
					}
					else
					{
						if (speed < 3f || !hit.owner.IsLocalFist(fist))
						{
							return;
						}
						Player.Note("punch NOT counted: " + fist.name + " hit " + NetworkPlayer.Names[hit.index] + " at " + speed.ToString("F1") + " m/s, this contact already landed a hit (closing " + closing.ToString("F1") + " m/s, " + (landedTime < 0f ? "-" : (Time.fixedTime - landedTime).ToString("F2") + " s after it") + ")");
						return;
					}
				}
				int verdict = hit.owner.Report(hit.index, c.impulse.magnitude, speed, fist.position, fist, arm && !fresh, swing, handSpeed);
				if (verdict < 0 || (verdict != 1 && arm))
				{
					return;
				}
				landed[hit.owner] = true;
				landedAt[hit.owner] = Time.fixedTime;
				if (swung)
				{
					spent++;
				}
			}
		}

		public static readonly float[] Radius = new float[13]
		{
			0.24f, 0.25f, 0.225f, 0.07f, 0.05f, 0.04f, 0.07f, 0.05f, 0.04f, 0.09f,
			0.06f, 0.09f, 0.06f
		};

		public static readonly float[] Height = new float[13]
		{
			0.48f, 0.5f, 0.473f, 0.395f, 0.292f, 0.15f, 0.395f, 0.292f, 0.15f, 0.357f,
			0.269f, 0.357f, 0.269f
		};

		public static readonly float[] CenterY = new float[13]
		{
			0.105f, 0.109f, 0.208f, 0.198f, 0.146f, 0.08f, 0.198f, 0.146f, 0.08f, 0.103f,
			0.11f, 0.103f, 0.11f
		};

		public static readonly float[] CenterZ = new float[13]
		{
			0f, 0f, 0f, 0f, 0f, 0.007f, 0f, 0f, 0.007f, 0f,
			0f, 0f, 0f
		};

		[Header("Hits")]
		public float hitScale = 0.7f;

		public float fistMass = 3f;

		public float maxSpeed = 20f;

		[Header("Copy")]
		public float follow = 0.06f;

		public float grip = 0.5f;

		public float coreGrip = 0.9f;

		public float impactFrom = 3f;

		public float coreImpactFrom = 25f;

		public float impactHold = 0.25f;

		public float impactEase = 0.35f;

		public float snapAt = 3f;

		public float drag = 3f;

		public float koHold = 0.4f;

		public float koWait = 1f;

		public float koCatch = 0.15f;

		public float koEase = 1f;

		public float koTrack = 0.15f;

		public float koTrackTime = 0.25f;

		public float knockMax = 40f;

		public float carryGrip;

		public float carryMass = 0.5f;

		public float climbMass = 20f;

		public float releaseFree = 3f;

		public float lined = 0.3f;

		public float deepFrom = 0.12f;

		public float poseSpeed = 12f;

		private const float MaxFollow = 50f;

		private const float TouchAccel = 15f;

		private const float TouchSpeed = 3f;

		private const float TouchTurn = 20f;

		private const float TouchSpin = 6f;

		private static readonly float[] TouchScale = new float[13]
		{
			1f, 1f, 1f, 2f, 4f, 8f, 2f, 4f, 8f, 1f,
			1.5f, 1f, 1.5f
		};

		public float poseFollow = 0.05f;

		public float poseSpin = 60f;

		public Action<int, float, float, Vector3, Vector3, Rigidbody, int> onHit;

		public static readonly List<AvatarBody> copies = new List<AvatarBody>();

		public static bool peaceful;

		public static bool truce;

		public const float PropSpeed = 3.5f;

		public const float KickSpeed = 1.5f;

		public const float KickMass = 11f;

		public const float KickMax = 60f;

		public const float KickMin = 30f;

		public const float PropMax = 15f;

		private static bool swingRule = true;

		public const float HatMax = 3f;

		private const float BackHit = 0.3f;

		public static int copiesChanged;

		[HideInInspector]
		public bool physical = true;

		[HideInInspector]
		public bool driven;

		[HideInInspector]
		public bool holdingMe;

		private readonly bool[] pinned = new bool[13];

		[HideInInspector]
		public bool streamed;

		[HideInInspector]
		public float impactAt = -9f;

		[HideInInspector]
		public int hits;

		[HideInInspector]
		public string lastHit = "";

		[HideInInspector]
		public Transform[] bones;

		[HideInInspector]
		public Rigidbody[] bodies;

		[HideInInspector]
		public Collider[] cols;

		private Player localBody;

		private readonly HandGrab[] localGrabs = new HandGrab[2];

		private readonly List<Rigidbody> watchedBodies = new List<Rigidbody>();

		private readonly float[] impactTimes = new float[13]
		{
			-9f, -9f, -9f, -9f, -9f, -9f, -9f, -9f, -9f, -9f,
			-9f, -9f, -9f
		};

		private float touchedAt = -9f;

		private bool gentle;

		private static int gentleMode = 5;

		private const float LookAhead = 0.2f;

		private const float NearMargin = 0.15f;

		private readonly Vector3[] towardMe = new Vector3[13];

		private bool linked;

		private readonly Dictionary<Rigidbody, Vector3[]> velocityHistory = new Dictionary<Rigidbody, Vector3[]>();

		private readonly List<Vector2> sentHurt = new List<Vector2>();

		private float seenKoLeft = -1f;

		private float seenKoAt;

		private float lastSentAt = -9f;

		private float lastSent;

		private float sentLife = 1.3f;

		private readonly Vector3[] goalPos = new Vector3[13];

		private readonly Quaternion[] goalRot = new Quaternion[13];

		private readonly Vector3[] goalVel = new Vector3[13];

		private readonly Vector3[] goalSpin = new Vector3[13];

		private float goalAt = -1f;

		private bool wasCarried;

		private bool wasLimp;

		private bool passThrough;

		private bool catchingUp;

		private bool gliding;

		private bool goalJumped;

		private static bool glideRule = true;

		private static bool kinematicWorld = true;

		private const float SnapFar = 8f;

		private const float CatchFrom = 1f;

		private const float CatchTime = 0.1f;

		private const float CatchMax = 40f;

		private readonly int[] parentOf = new int[13];

		private bool parentsRead;

		private bool fistsOn;

		private bool collidesWithWorld;

		private float passThroughUntil;

		private float limpUntil = -9f;

		private bool loose;

		private float predictedAt = -9f;

		private float poppedAt = -9f;

		private float predictedHead;

		private float releasedAt = -9f;

		private float freeUntil = -9f;

		private readonly List<Collider> worldColliders = new List<Collider>();

		private static readonly RaycastHit[] trapHits = new RaycastHit[16];

		private static readonly Collider[] stallHits = new Collider[16];

		private const float StallGap = 1f;

		private const float StallSpeed = 1f;

		private const int StallSteps = 3;

		private const float StallReach = 0.2f;

		private int stalledSteps;

		private const float CarrySlip = 4f;

		private const float CarryFast = 6f;

		private bool carryFast;

		private const float TrapGap = 0.75f;

		private Collider[] localColliders;

		private Bounds[] localBounds;

		private Player localFighter;

		private Player localSounds;

		private NetworkPlayer netAvatar;

		private float[] baseMass;

		private readonly ConfigurableJoint[] joints = new ConfigurableJoint[13];

		private readonly ConfigurableJointMotion[] jointLimits = new ConfigurableJointMotion[39];

		private bool posed;

		private float massScale = 1f;

		public bool Held => HeldByLocal();

		public static bool IsKick(bool fist, float impulse)
		{
			return !fist && impulse >= KickMin;
		}

		public static float PropImpulse(float mass, float speed)
		{
			return Mathf.Min(Mathf.Clamp(Mathf.Sqrt(mass / 3f), 1f, 2f) * 3f * speed, PropMax);
		}

		public void Pin(int bone, bool on)
		{
			if (bone < 0 || bone >= pinned.Length)
			{
				return;
			}
			pinned[bone] = on;
		}

		private void Awake()
		{
			bones = NetworkPlayer.Bind(transform);
			if (bones == null)
			{
				return;
			}
			Grabbable grabbable = GetComponent<Grabbable>();
			if (grabbable)
			{
				grabbable.pressWhileTouching = true;
			}
			if (grabbable && peaceful)
			{
				grabbable.enabled = false;
			}
			Player me = FindLocalBody();
			Transform[] mine = me ? NetworkPlayer.Bind(me.transform) : null;
			bodies = new Rigidbody[NetworkPlayer.Bones];
			cols = new Collider[NetworkPlayer.Bones];
			baseMass = new float[NetworkPlayer.Bones];
			for (int i = 0; i < NetworkPlayer.Bones; i++)
			{
				GameObject go = bones[i].gameObject;
				Rigidbody rb = go.GetComponent<Rigidbody>();
				if (!rb)
				{
					rb = go.AddComponent<Rigidbody>();
				}
				rb.isKinematic = true;
				rb.useGravity = false;
				rb.interpolation = RigidbodyInterpolation.None;
				CapsuleCollider capsule = go.GetComponent<CapsuleCollider>();
				if (!capsule)
				{
					capsule = go.AddComponent<CapsuleCollider>();
				}
				CapsuleCollider like = mine != null ? mine[i].GetComponent<CapsuleCollider>() : null;
				rb.mass = like && like.attachedRigidbody ? like.attachedRigidbody.mass : 5f;
				baseMass[i] = rb.mass;
				capsule.direction = like ? like.direction : 1;
				capsule.radius = like ? like.radius : Radius[i];
				capsule.height = like ? like.height : Height[i];
				capsule.center = like ? like.center : new Vector3(0f, CenterY[i], CenterZ[i]);
				AvatarHit hit = go.GetComponent<AvatarHit>();
				if (!hit)
				{
					hit = go.AddComponent<AvatarHit>();
				}
				hit.owner = this;
				hit.index = i;
				bodies[i] = rb;
				cols[i] = capsule;
			}
			for (int j = 0; j < NetworkPlayer.Bones; j++)
			{
				for (int k = j + 1; k < NetworkPlayer.Bones; k++)
				{
					Physics.IgnoreCollision(cols[j], cols[k], true);
				}
				Rigidbody parent = ParentBody(j);
				if (!parent)
				{
					continue;
				}
				ConfigurableJoint joint = bones[j].GetComponent<ConfigurableJoint>();
				if (!joint)
				{
					joint = bones[j].gameObject.AddComponent<ConfigurableJoint>();
				}
				ConfigurableJoint likeJoint = mine != null ? mine[j].GetComponent<ConfigurableJoint>() : null;
				joint.xMotion = ConfigurableJointMotion.Locked;
				joint.yMotion = ConfigurableJointMotion.Locked;
				joint.zMotion = ConfigurableJointMotion.Locked;
				joint.enableCollision = false;
				if (likeJoint)
				{
					joint.autoConfigureConnectedAnchor = false;
					joint.anchor = likeJoint.anchor;
					joint.connectedAnchor = likeJoint.connectedAnchor;
					joint.axis = likeJoint.axis;
					joint.secondaryAxis = likeJoint.secondaryAxis;
					joint.angularXMotion = likeJoint.angularXMotion;
					joint.angularYMotion = likeJoint.angularYMotion;
					joint.angularZMotion = likeJoint.angularZMotion;
					joint.lowAngularXLimit = likeJoint.lowAngularXLimit;
					joint.highAngularXLimit = likeJoint.highAngularXLimit;
					joint.angularYLimit = likeJoint.angularYLimit;
					joint.angularZLimit = likeJoint.angularZLimit;
					joints[j] = joint;
					jointLimits[j * 3] = likeJoint.angularXMotion;
					jointLimits[j * 3 + 1] = likeJoint.angularYMotion;
					jointLimits[j * 3 + 2] = likeJoint.angularZMotion;
				}
				else
				{
					joint.anchor = Vector3.zero;
					joint.autoConfigureConnectedAnchor = true;
					joint.angularXMotion = ConfigurableJointMotion.Free;
					joint.angularYMotion = ConfigurableJointMotion.Free;
					joint.angularZMotion = ConfigurableJointMotion.Free;
				}
				joint.connectedBody = parent;
				if (joints[j])
				{
					joint.angularXMotion = ConfigurableJointMotion.Free;
					joint.angularYMotion = ConfigurableJointMotion.Free;
					joint.angularZMotion = ConfigurableJointMotion.Free;
				}
			}
			foreach (AvatarBody other in copies)
			{
				if (!other || other.cols == null)
				{
					continue;
				}
				for (int l = 0; l < cols.Length; l++)
				{
					for (int m = 0; m < other.cols.Length; m++)
					{
						Physics.IgnoreCollision(cols[l], other.cols[m], true);
					}
				}
			}
			copies.Add(this);
			copiesChanged++;
			SetPhysical(physical);
		}

		private void OnDestroy()
		{
			copies.Remove(this);
			copiesChanged++;
		}

		private static bool IsCore(int i)
		{
			return i < 3 || i > 8;
		}

		private bool CoreFar()
		{
			for (int i = 0; i < NetworkPlayer.Bones; i++)
			{
				if (IsCore(i) && (goalPos[i] - bodies[i].position).sqrMagnitude > lined * lined)
				{
					return true;
				}
			}
			return false;
		}

		private void Reseat()
		{
			if (goalAt < 0f || HeldByLocal())
			{
				return;
			}
			if (!parentsRead)
			{
				parentsRead = true;
				for (int i = 0; i < NetworkPlayer.Bones; i++)
				{
					parentOf[i] = -1;
					Rigidbody connected = joints[i] ? joints[i].connectedBody : null;
					for (int j = 0; j < NetworkPlayer.Bones; j++)
					{
						if (connected && bodies[j] == connected)
						{
							parentOf[i] = j;
						}
					}
				}
			}
			for (int k = 0; k < NetworkPlayer.Bones; k++)
			{
				int p = parentOf[k];
				if (p < 0 || p > k)
				{
					continue;
				}
				Quaternion inverse = Quaternion.Inverse(goalRot[p]);
				Rigidbody parent = bodies[p];
				Rigidbody child = bodies[k];
				Vector3 position = parent.position;
				Quaternion rotation = parent.rotation;
				child.position = position + rotation * (inverse * (goalPos[k] - goalPos[p]));
				child.rotation = parent.rotation * (inverse * goalRot[k]);
				child.transform.SetPositionAndRotation(child.position, child.rotation);
				child.linearVelocity = parent.linearVelocity;
				child.angularVelocity = parent.angularVelocity;
			}
		}

		public void Impact(int i, float impulse)
		{
			if (!driven || (IsCore(i) ? coreImpactFrom : impactFrom) > impulse || HeldByLocal())
			{
				return;
			}
			impactTimes[i] = Time.time;
			impactAt = Time.time;
		}

		private Rigidbody ParentBody(int i)
		{
			Transform t = bones[i];
			while (true)
			{
				Transform parent = t.parent;
				if (!parent || parent == transform)
				{
					return null;
				}
				for (int j = 0; j < NetworkPlayer.Bones; j++)
				{
					if (bones[j] == parent)
					{
						return bodies[j];
					}
				}
				t = parent;
			}
		}

		public void Drive(Vector3[] pos, Quaternion[] rot)
		{
			if (bodies == null)
			{
				return;
			}
			float dt = goalAt > 0f ? Time.time - goalAt : 0f;
			bool jumped = (pos[0] - goalPos[0]).sqrMagnitude > snapAt * snapAt;
			for (int i = 0; i < NetworkPlayer.Bones; i++)
			{
				Vector3 velocity = dt > 0.0001f ? (pos[i] - goalPos[i]) / dt : Vector3.zero;
				goalVel[i] = driven && !jumped ? Vector3.Lerp(goalVel[i], velocity, 0.5f) : Vector3.zero;
				Vector3 spin = Vector3.zero;
				if (dt > 0.0001f && !jumped)
				{
					(rot[i] * Quaternion.Inverse(goalRot[i])).ToAngleAxis(out float angle, out Vector3 axis);
					if (angle > 180f)
					{
						angle -= 360f;
					}
					if (float.IsFinite(axis.x))
					{
						spin = axis * (angle * Mathf.Deg2Rad / dt);
					}
				}
				goalSpin[i] = driven && !jumped ? Vector3.Lerp(goalSpin[i], spin, 0.5f) : Vector3.zero;
				goalPos[i] = pos[i];
				goalRot[i] = rot[i];
			}
			goalAt = Time.time;
			if (!driven)
			{
				StartDriving();
			}
			else if (jumped)
			{
				goalJumped = true;
			}
		}

		public float Stray()
		{
			if (goalAt < 0f || bodies == null)
			{
				return 0f;
			}
			return Vector3.Distance(goalPos[0], bodies[0].position);
		}

		private static bool World(Collider c)
		{
			if (c.isTrigger || c.gameObject.layer == 11)
			{
				return false;
			}
			Rigidbody rb = c.attachedRigidbody;
			if (!rb)
			{
				return true;
			}
			if (Drawbridge.Surface(rb, rb.position, out Vector3 _))
			{
				return false;
			}
			if (c.GetComponentInParent<Hazard>())
			{
				return true;
			}
			if (Platform(rb))
			{
				return true;
			}
			if (!kinematicWorld)
			{
				return false;
			}
			if (rb.GetComponentInParent<Player>() || rb.GetComponentInParent<AvatarBody>() || rb.GetComponentInParent<NetProp>() || c.GetComponentInParent<Train>())
			{
				return false;
			}
			if (rb.isKinematic)
			{
				return true;
			}
			return rb.GetComponentInParent<Rope>();
		}

		private void StartGlide(string why)
		{
			if (!gliding)
			{
				Player.Note("copy of " + name + " is " + Stray().ToString("F1") + " m from where the network has it" + why + ", gliding through to it");
			}
			bool through = passThrough;
			catchingUp = true;
			gliding = true;
			stalledSteps = 0;
			if (!through)
			{
				CollideWithLocal(false);
			}
			Collider[] array = cols;
			foreach (Collider c in array)
			{
				if (c)
				{
					c.excludeLayers = -1;
				}
			}
		}

		private void EndGlide()
		{
			if (!gliding)
			{
				return;
			}
			gliding = false;
			Collider[] array = cols;
			foreach (Collider c in array)
			{
				if (c)
				{
					c.excludeLayers = 0;
				}
			}
		}

		private void StartDriving()
		{
			driven = true;
			Collider[] all = UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsSortMode.None);
			foreach (Collider world in all)
			{
				if (!World(world))
				{
					continue;
				}
				worldColliders.Add(world);
				Collider[] array = cols;
				foreach (Collider c in array)
				{
					Physics.IgnoreCollision(c, world, true);
				}
			}
			for (int i = 0; i < NetworkPlayer.Bones; i++)
			{
				Rigidbody rb = bodies[i];
				bones[i].SetPositionAndRotation(goalPos[i], goalRot[i]);
				rb.position = goalPos[i];
				rb.rotation = goalRot[i];
				rb.isKinematic = false;
				rb.useGravity = false;
				rb.interpolation = RigidbodyInterpolation.Interpolate;
				rb.linearDamping = drag;
				rb.angularDamping = drag;
				rb.maxDepenetrationVelocity = 1f;
				rb.maxAngularVelocity = poseSpin;
				rb.excludeLayers = 1 << 11;
			}
		}

		private void FollowGoal()
		{
			if (goalAt < 0f)
			{
				return;
			}
			bool carried = CarriedByLocal();
			float stray = Vector3.Distance(goalPos[0], bodies[0].position);
			if (!carried && Time.time > freeUntil && stray > (glideRule && !goalJumped ? SnapFar : snapAt))
			{
				Player.Note("copy of " + name + " snapped " + stray.ToString("F1") + " m to where the network has it" + (goalJumped ? " (the network moved it that far at once)" : ""));
				goalJumped = false;
				Snap();
				return;
			}
			if (stray <= snapAt)
			{
				goalJumped = false;
			}
			float mass;
			if (carried)
			{
				mass = carryMass;
			}
			else
			{
				if (!streamed && Time.time < freeUntil && (goalPos[0] - bodies[0].position).sqrMagnitude < lined * lined && bodies[0].linearVelocity.sqrMagnitude < 0.25f)
				{
					freeUntil = Time.time;
					limpUntil = Time.time;
				}
				mass = ClimbedByLocal() ? climbMass : 1f;
			}
			if (mass != massScale)
			{
				if (mass == climbMass && OverlapsLocal(0.03f))
				{
					Player.Note("climbing " + name + " with my body 3+ cm inside its copy, the copy turns " + climbMass.ToString("F0") + "x heavy");
				}
				massScale = mass;
				for (int i = 0; i < NetworkPlayer.Bones; i++)
				{
					bodies[i].mass = mass * baseMass[i];
				}
			}
			if (wasCarried != carried)
			{
				wasCarried = carried;
				if (!netAvatar)
				{
					netAvatar = GetComponent<NetworkPlayer>();
				}
				if (carried)
				{
					HoldToHand();
				}
				else if (!netAvatar || netAvatar.KnockedOut)
				{
					Free();
				}
				else
				{
					freeUntil = Time.time;
					limpUntil = Time.time;
				}
			}
			else if (carried)
			{
				HoldToHand();
			}
			bool falling = AwaitingFall();
			if (falling)
			{
				limpUntil = Mathf.Max(limpUntil, Time.time + impactHold);
				for (int j = 0; j < NetworkPlayer.Bones; j++)
				{
					impactTimes[j] = Mathf.Max(impactTimes[j], Time.time);
				}
			}
			bool limp = carried || Time.time < limpUntil;
			bool unheld = !carried && limp;
			if (unheld)
			{
				if (Player.Buried(bodies[0].position, null, out float top))
				{
					Vector3 lift = Vector3.up * (top + 0.25f - bodies[0].position.y);
					Rigidbody[] array = bodies;
					foreach (Rigidbody rb in array)
					{
						rb.position += lift;
						rb.transform.position = rb.position;
					}
					Player.Note("copy of " + name + " was " + ((lift.y - 0.25f) * 100f).ToString("F0") + " cm inside the ground while limp, lifted out");
				}
				if (Time.time > freeUntil && Trapped(out Collider wall))
				{
					Player.Note("copy of " + name + " was stuck " + Vector3.Distance(goalPos[0], bodies[0].position).ToString("F1") + " m from where the network has it, behind " + wall.name + ", moved through");
					Snap();
					return;
				}
			}
			if (loose != unheld)
			{
				loose = unheld;
				NetProp.Loose(cols, unheld);
			}
			if (wasLimp != limp)
			{
				wasLimp = limp;
				if (limp)
				{
					Reseat();
				}
				else if (CoreFar())
				{
					catchingUp = true;
					if (!passThrough)
					{
						CollideWithLocal(false);
					}
				}
				CollisionDetectionMode mode = limp ? CollisionDetectionMode.ContinuousSpeculative : CollisionDetectionMode.Discrete;
				for (int k = 0; k < NetworkPlayer.Bones; k++)
				{
					bodies[k].useGravity = limp;
					bodies[k].collisionDetectionMode = mode;
					ConfigurableJoint joint = joints[k];
					if (joint)
					{
						joint.angularXMotion = limp ? jointLimits[k * 3] : ConfigurableJointMotion.Free;
						joint.angularYMotion = limp ? jointLimits[k * 3 + 1] : ConfigurableJointMotion.Free;
						joint.angularZMotion = limp ? jointLimits[k * 3 + 2] : ConfigurableJointMotion.Free;
					}
				}
			}
			float damping = unheld ? 0f : drag;
			if (bodies[0].linearDamping != damping)
			{
				Rigidbody[] array2 = bodies;
				foreach (Rigidbody rb2 in array2)
				{
					rb2.linearDamping = damping;
				}
			}
			if (gliding && (carried || limp || holdingMe))
			{
				EndGlide();
			}
			if (catchingUp && !CoreFar())
			{
				catchingUp = false;
				EndGlide();
			}
			if (carried || holdingMe || peaceful)
			{
				if (!passThrough)
				{
					CollideWithLocal(false);
				}
				bool fists = holdingMe && !carried && !peaceful;
				if (fistsOn != fists)
				{
					Fists(fists);
				}
				passThroughUntil = Time.time + 0.5f;
			}
			else if (!passThrough && OverlapsLocal(deepFrom))
			{
				Player.Note("ghosting: my body sank over " + (deepFrom * 100f).ToString("F0") + " cm into " + name + ", we pass through each other (punches too) until apart");
				CollideWithLocal(false);
				passThroughUntil = Time.time + 0.2f;
			}
			else if (passThrough && Time.time > passThroughUntil && !catchingUp && !OverlapsLocal(0.01f))
			{
				CollideWithLocal(true);
			}
			if (limp)
			{
				if (!collidesWithWorld)
				{
					CollideWithWorld(true);
				}
			}
			else if (collidesWithWorld && Time.time > passThroughUntil)
			{
				CollideWithWorld(false);
			}
			Pose(!limp);
			if (posed && !carried && !holdingMe && !HeldByLocal() && !ClimbedByLocal())
			{
				if (glideRule && !gliding && stray > CatchFrom)
				{
					StartGlide("");
				}
				else if (!gliding && Stalled(out string against))
				{
					if (!glideRule)
					{
						Player.Note("copy of " + name + " stalled " + (goalPos[0] - bodies[0].position).magnitude.ToString("F1") + " m from where the network has it, against " + against + ", moved through");
						Snap();
						return;
					}
					StartGlide(", stalled against " + against);
				}
			}
			else
			{
				stalledSteps = 0;
			}
			gentle = gentleMode != 0 && !passThrough && (Time.fixedTime - touchedAt <= Time.fixedDeltaTime * 1.5f || NearLocal(Reach()));
			linked = gentle && gentleMode == 5 && (holdingMe || ClimbedByLocal() || CarriedByLocal());
			if (gentle && gentleMode == 5 && !linked)
			{
				TowardMe();
			}
			if (posed)
			{
				float k2 = 1f / Mathf.Max(poseFollow, Time.fixedDeltaTime);
				for (int l = 0; l < NetworkPlayer.Bones; l++)
				{
					Rigidbody rb3 = bodies[l];
					float speed = gliding ? Mathf.Clamp(Vector3.Distance(goalPos[l], rb3.position) / CatchTime, poseSpeed, CatchMax) : poseSpeed;
					if (!pinned[l])
					{
						Vector3 carry = Vector3.ClampMagnitude(goalVel[l], MaxFollow);
						Vector3 pull = Vector3.ClampMagnitude((goalPos[l] - rb3.position) * k2, speed);
						rb3.linearVelocity = Gentle(l, carry + pull);
					}
					(goalRot[l] * Quaternion.Inverse(rb3.rotation)).ToAngleAxis(out float angle, out Vector3 axis);
					if (angle > 180f)
					{
						angle -= 360f;
					}
					if (float.IsFinite(axis.x))
					{
						rb3.angularVelocity = GentleSpin(l, Vector3.ClampMagnitude(goalSpin[l] + axis * (k2 * (angle * Mathf.Deg2Rad)), poseSpin));
					}
				}
				return;
			}
			for (int m = 0; m < NetworkPlayer.Bones; m++)
			{
				float now = Time.time;
				if (falling && IsCore(m))
				{
					Rigidbody rb4 = bodies[m];
					Vector3 track = (goalPos[m] - rb4.position) / koTrackTime + goalVel[m];
					Vector3 velocity = rb4.linearVelocity;
					rb4.linearVelocity = Gentle(m, new Vector3(Mathf.Lerp(velocity.x, track.x, koTrack), velocity.y, Mathf.Lerp(velocity.z, track.z, koTrack)));
					continue;
				}
				float g = carried ? carryGrip : (IsCore(m) ? coreGrip : grip);
				float since = now - impactTimes[m];
				float ease = since < impactHold ? 0f : Mathf.Clamp01((since - impactHold) / Mathf.Max(impactEase, 0.01f));
				float ko = carried ? 1f : Mathf.Clamp01((Time.time - releasedAt) / koEase);
				float hold = g * ease * ko;
				if (!(hold > 0f))
				{
					continue;
				}
				Rigidbody rb5 = bodies[m];
				Vector3 want = (goalPos[m] - rb5.position) / follow + goalVel[m];
				rb5.linearVelocity = Gentle(m, Vector3.Lerp(rb5.linearVelocity, want, hold));
				(goalRot[m] * Quaternion.Inverse(rb5.rotation)).ToAngleAxis(out float angle2, out Vector3 axis2);
				if (angle2 > 180f)
				{
					angle2 -= 360f;
				}
				if (float.IsFinite(axis2.x))
				{
					rb5.angularVelocity = GentleSpin(m, Vector3.Lerp(rb5.angularVelocity, axis2 * (angle2 * Mathf.Deg2Rad / follow), hold));
				}
			}
		}

		private void Snap()
		{
			stalledSteps = 0;
			EndGlide();
			if (!passThrough)
			{
				CollideWithLocal(false);
			}
			passThroughUntil = Mathf.Max(passThroughUntil, Time.time + 0.2f);
			for (int i = 0; i < NetworkPlayer.Bones; i++)
			{
				bodies[i].position = goalPos[i];
				bodies[i].rotation = goalRot[i];
				bodies[i].linearVelocity = goalVel[i];
				bodies[i].angularVelocity = Vector3.zero;
			}
		}

		private bool Trapped(out Collider wall)
		{
			wall = null;
			Vector3 position = bodies[0].position;
			Vector3 to = goalPos[0] - position;
			float distance = to.magnitude;
			if (distance < TrapGap)
			{
				return false;
			}
			bool backfaces = Physics.queriesHitBackfaces;
			Physics.queriesHitBackfaces = true;
			int count = Physics.RaycastNonAlloc(position, to / distance, trapHits, distance, ~(1 << 11), QueryTriggerInteraction.Ignore);
			Physics.queriesHitBackfaces = backfaces;
			for (int i = 0; i < count; i++)
			{
				Collider c = trapHits[i].collider;
				if (!c || (c.attachedRigidbody && !c.attachedRigidbody.isKinematic) || c.GetComponentInParent<AvatarBody>() || c.GetComponentInParent<Player>())
				{
					continue;
				}
				wall = c;
				return true;
			}
			return false;
		}

		private void HoldToHand()
		{
			Rigidbody hand = null;
			HandGrab[] array = localGrabs;
			foreach (HandGrab g in array)
			{
				if (g && g.grabbing && !g.holdsYou && g.held && g.held.transform.IsChildOf(transform))
				{
					hand = g.GetComponent<Rigidbody>();
				}
			}
			if (!hand)
			{
				return;
			}
			Vector3 handVelocity = hand.linearVelocity;
			float pelvis = bodies[0].linearVelocity.magnitude;
			float fastest = 0f;
			Rigidbody[] array2 = bodies;
			foreach (Rigidbody rb in array2)
			{
				Vector3 off = rb.linearVelocity - handVelocity;
				fastest = Mathf.Max(fastest, off.magnitude);
				if (off.sqrMagnitude > CarrySlip * CarrySlip)
				{
					rb.linearVelocity = handVelocity + off.normalized * CarrySlip;
				}
			}
			bool fast = pelvis > CarryFast || fastest > 12f;
			if (fast && !carryFast)
			{
				Player.Note("carrying " + name + ": copy pelvis " + pelvis.ToString("F1") + " m/s, my hand " + handVelocity.magnitude.ToString("F1") + " m/s, my pelvis " + (localFighter && localFighter.pelvis ? localFighter.pelvis.linearVelocity.magnitude.ToString("F1") : "?") + " m/s, fastest bone off my hand " + fastest.ToString("F1") + " m/s, held to " + CarrySlip.ToString("F0"));
			}
			carryFast = fast;
		}

		private bool Stalled(out string against)
		{
			against = "nothing";
			Vector3 to = goalPos[0] - bodies[0].position;
			float distance = to.magnitude;
			if (distance <= StallGap || Vector3.Dot(to / distance, bodies[0].linearVelocity) >= StallSpeed)
			{
				stalledSteps = 0;
				return false;
			}
			if (++stalledSteps < StallSteps)
			{
				return false;
			}
			stalledSteps = 0;
			float best = float.MaxValue;
			for (int i = 0; i < NetworkPlayer.Bones; i++)
			{
				if (!cols[i])
				{
					continue;
				}
				int count = Physics.OverlapSphereNonAlloc(bodies[i].position, StallReach, stallHits, ~(1 << 11), QueryTriggerInteraction.Ignore);
				for (int j = 0; j < count; j++)
				{
					Collider c = stallHits[j];
					if (!c || c.transform.IsChildOf(transform) || Physics.GetIgnoreCollision(c, cols[i]))
					{
						continue;
					}
					Vector3 position = bodies[i].position;
					Vector3 closest = ((c is MeshCollider mesh && !mesh.convex) || c is TerrainCollider) ? c.bounds.ClosestPoint(position) : c.ClosestPoint(position);
					float d = (closest - position).sqrMagnitude;
					if (d >= best)
					{
						continue;
					}
					best = d;
					against = c.name + (c.GetComponentInParent<Player>() ? " (my body)" : (!c.attachedRigidbody ? " (static)" : (c.attachedRigidbody.isKinematic ? " (kinematic)" : " (prop)")));
				}
			}
			return true;
		}

		public void Touched(int bone)
		{
			touchedAt = Time.fixedTime;
		}

		private void TowardMe()
		{
			float k = 1f / Mathf.Max(poseFollow, Time.fixedDeltaTime);
			Vector3 me = localFighter && localFighter.pelvis ? localFighter.pelvis.position : transform.position;
			for (int i = 0; i < NetworkPlayer.Bones; i++)
			{
				towardMe[i] = Vector3.zero;
				if (!cols[i] || localColliders == null)
				{
					continue;
				}
				float pull = Mathf.Min(k * Vector3.Distance(goalPos[i], bodies[i].position), poseSpeed);
				float speed = Mathf.Max(Mathf.Min(goalVel[i].magnitude, MaxFollow) + pull, bodies[i].linearVelocity.magnitude);
				Bounds bounds = cols[i].bounds;
				float reach = speed * LookAhead + bounds.extents.magnitude + NearMargin;
				float best = reach * reach;
				Vector3 away = me - bounds.center;
				Vector3 dir = Vector3.zero;
				bool found = false;
				Collider[] array = localColliders;
				foreach (Collider c in array)
				{
					if (!c || !c.enabled || c.isTrigger)
					{
						continue;
					}
					Vector3 d = c.bounds.ClosestPoint(bounds.center) - bounds.center;
					float sq = d.sqrMagnitude;
					if (!(sq >= best))
					{
						best = sq;
						found = true;
						dir = sq > 1E-06f ? d : away;
					}
				}
				if (found && dir.sqrMagnitude > 1E-06f)
				{
					towardMe[i] = dir.normalized;
				}
			}
		}

		private Vector3 Gentle(int bone, Vector3 velocity)
		{
			if (!gentle)
			{
				return velocity;
			}
			if (gentleMode == 5 && !linked)
			{
				return Inward(bone, velocity);
			}
			Vector3 braked = Brake(bodies[bone].linearVelocity, velocity);
			if (Vector3.Dot(velocity, braked) >= velocity.sqrMagnitude)
			{
				return velocity;
			}
			Vector3 step = Vector3.ClampMagnitude(velocity - braked, TouchScale[bone] * TouchAccel * Time.fixedDeltaTime);
			return Vector3.ClampMagnitude(braked + step, Mathf.Max(braked.magnitude, TouchScale[bone] * TouchSpeed));
		}

		private Vector3 Inward(int bone, Vector3 velocity)
		{
			Vector3 dir = towardMe[bone];
			if (dir == Vector3.zero)
			{
				return velocity;
			}
			float into = Vector3.Dot(velocity, dir);
			float now = Mathf.Max(Vector3.Dot(dir, bodies[bone].linearVelocity), 0f);
			if (into <= now)
			{
				return velocity;
			}
			float allowed = Mathf.Min(into, now + TouchScale[bone] * TouchAccel * Time.fixedDeltaTime);
			allowed = Mathf.Min(allowed, Mathf.Max(now, TouchScale[bone] * TouchSpeed));
			return velocity + dir * (allowed - into);
		}

		private Vector3 GentleSpin(int bone, Vector3 spin)
		{
			if (!gentle)
			{
				return spin;
			}
			if (gentleMode == 5 && !linked && towardMe[bone] == Vector3.zero)
			{
				return spin;
			}
			Vector3 braked = Brake(bodies[bone].angularVelocity, spin);
			if (Vector3.Dot(spin, braked) >= spin.sqrMagnitude)
			{
				return spin;
			}
			Vector3 step = Vector3.ClampMagnitude(spin - braked, TouchScale[bone] * TouchTurn * Time.fixedDeltaTime);
			return Vector3.ClampMagnitude(braked + step, Mathf.Max(braked.magnitude, TouchScale[bone] * TouchSpin));
		}

		private static Vector3 Brake(Vector3 was, Vector3 want)
		{
			float magnitude = was.magnitude;
			if (magnitude < 0.0001f)
			{
				return was;
			}
			Vector3 dir = was / magnitude;
			float change = Vector3.Dot(want - was, dir);
			if (change >= 0f)
			{
				return was;
			}
			return was + dir * Mathf.Max(change, -magnitude);
		}

		private float Reach()
		{
			float k = 1f / Mathf.Max(poseFollow, Time.fixedDeltaTime);
			float fastest = 0f;
			for (int i = 0; i < NetworkPlayer.Bones; i++)
			{
				float pull = Mathf.Min(k * Vector3.Distance(goalPos[i], bodies[i].position), poseSpeed);
				float speed = Mathf.Max(Mathf.Min(goalVel[i].magnitude, MaxFollow) + pull, bodies[i].linearVelocity.magnitude);
				fastest = Mathf.Max(fastest, speed);
			}
			return fastest * Time.fixedDeltaTime + 0.05f;
		}

		private bool NearLocal(float reach)
		{
			if (localColliders == null)
			{
				return false;
			}
			Bounds bounds = cols[0].bounds;
			for (int i = 1; i < cols.Length; i++)
			{
				bounds.Encapsulate(cols[i].bounds);
			}
			bounds.Expand(reach * 2f);
			Collider[] array = localColliders;
			foreach (Collider c in array)
			{
				if (c && c.enabled && !c.isTrigger && bounds.Intersects(c.bounds))
				{
					return true;
				}
			}
			return false;
		}

		private bool AwaitingFall()
		{
			if (predictedAt < 0f)
			{
				return false;
			}
			bool confirmed = netAvatar && netAvatar.KnockedOut;
			bool caught = confirmed && goalPos[2].y < bodies[2].position.y + koCatch && goalPos[2].y < predictedHead - koCatch;
			if (Time.time > predictedAt + koWait + NetworkPlayer.viewLag || caught)
			{
				Player.Note("copy of " + name + ": predicted KO ended after " + ((Time.time - predictedAt) * 1000f).ToString("F0") + " ms, " + (caught ? "the network body caught up" : (confirmed ? "the network body stayed up" : "the network never confirmed it")) + ", " + (Vector3.Distance(goalPos[0], bodies[0].position) * 100f).ToString("F0") + " cm away");
				predictedAt = -9f;
				if (confirmed)
				{
					releasedAt = Time.time + impactHold;
				}
			}
			return predictedAt >= 0f && confirmed;
		}

		private void Pose(bool on)
		{
			if (posed != on)
			{
				posed = on;
			}
		}

		public void Wake()
		{
			freeUntil = Time.time;
			limpUntil = Time.time;
		}

		public void Rejoin()
		{
			freeUntil = Time.time;
			for (int i = 0; i < NetworkPlayer.Bones; i++)
			{
				impactTimes[i] = Mathf.Min(impactTimes[i], Time.time);
			}
			if (!netAvatar)
			{
				netAvatar = GetComponent<NetworkPlayer>();
			}
			if (netAvatar && netAvatar.KnockedOut)
			{
				return;
			}
			limpUntil = Time.time;
		}

		public bool Overturn(string why)
		{
			if (netAvatar && netAvatar.KnockedOut)
			{
				return false;
			}
			if (poppedAt >= 0f && netAvatar)
			{
				netAvatar.popAt = -9f;
			}
			poppedAt = -9f;
			if (predictedAt < 0f)
			{
				return false;
			}
			Player.Note("copy of " + name + ": predicted KO overturned after " + ((Time.time - predictedAt) * 1000f).ToString("F0") + " ms, " + why);
			predictedAt = -9f;
			return true;
		}

		public bool Confirm()
		{
			if (poppedAt < 0f)
			{
				return false;
			}
			Player.Note("copy of " + name + ": predicted KO confirmed after " + ((Time.time - poppedAt) * 1000f).ToString("F0") + " ms");
			poppedAt = -9f;
			return true;
		}

		public void Free()
		{
			freeUntil = Time.time + releaseFree;
			limpUntil = Mathf.Max(limpUntil, Time.time + releaseFree);
			for (int i = 0; i < NetworkPlayer.Bones; i++)
			{
				impactTimes[i] = Mathf.Max(impactTimes[i], freeUntil - impactHold);
			}
		}

		public void SetPhysical(bool on)
		{
			physical = on;
			if (cols == null)
			{
				return;
			}
			Collider[] array = cols;
			foreach (Collider c in array)
			{
				if (c)
				{
					c.enabled = on;
				}
			}
			if (on)
			{
				Unshare();
			}
		}

		private void Unshare()
		{
			Grabbable[] all = UnityEngine.Object.FindObjectsByType<Grabbable>(FindObjectsSortMode.None);
			foreach (Grabbable g in all)
			{
				if (LocalOnly(g) || HostOnly(g) || (peaceful && Prop(g)))
				{
					Ignore(g.GetComponentsInChildren<Collider>(), cols);
				}
			}
		}

		public static void Unshare(Collider[] props)
		{
			foreach (AvatarBody copy in copies)
			{
				if (copy && copy.physical && copy.cols != null)
				{
					Ignore(props, copy.cols);
				}
			}
		}

		public bool Loose()
		{
			return loose;
		}

		public static bool LocalOnly(Grabbable g)
		{
			if (!Prop(g) || g.GetComponent<NetworkObject>())
			{
				return false;
			}
			return !g.GetComponent<Rigidbody>().isKinematic;
		}

		public static bool HostOnly(Grabbable g)
		{
			NetProp prop = g ? g.GetComponent<NetProp>() : null;
			return prop && prop.hostOnly;
		}

		public static bool Prop(Grabbable g)
		{
			if (!g || !g.GetComponent<Rigidbody>() || g.GetComponentInParent<Player>())
			{
				return false;
			}
			return !g.GetComponentInParent<AvatarBody>();
		}

		private static void Ignore(Collider[] props, Collider[] body)
		{
			foreach (Collider prop in props)
			{
				if (!prop || !prop.enabled)
				{
					continue;
				}
				foreach (Collider c in body)
				{
					if (c && c.enabled)
					{
						Physics.IgnoreCollision(prop, c, true);
					}
				}
			}
		}

		private void FixedUpdate()
		{
			if (driven)
			{
				FollowGoal();
			}
			if (!localBody)
			{
				localBody = FindLocalBody();
				if (!localBody)
				{
					return;
				}
				localFighter = localBody.GetComponent<Player>();
				localColliders = localBody.GetComponentsInChildren<Collider>();
				Rigidbody[] array = localBody.bodies;
				foreach (Rigidbody rb in array)
				{
					Watch(rb);
				}
			}
			if (localGrabs[0] == null && localBody.leftHand)
			{
				localGrabs[0] = localBody.leftHand.GetComponent<HandGrab>();
			}
			if (localGrabs[1] == null && localBody.rightHand)
			{
				localGrabs[1] = localBody.rightHand.GetComponent<HandGrab>();
			}
			HandGrab[] array2 = localGrabs;
			foreach (HandGrab g in array2)
			{
				if (!g)
				{
					continue;
				}
				if (g.held && !g.holdsYou)
				{
					Watch(g.held);
				}
				if (g.lastHeld && g.Wields(g.lastHeld))
				{
					Watch(g.lastHeld);
				}
			}
			if (localBody.Body && !watchedBodies.Contains(localBody.Body))
			{
				Watch(localBody.Body, true);
			}
			watchedBodies.RemoveAll((Rigidbody r) => !r);
			foreach (Rigidbody watched in watchedBodies)
			{
				RecordVelocity(watched);
			}
		}

		private static Player FindLocalBody()
		{
			NetworkManager manager = UnityEngine.Object.FindFirstObjectByType<NetworkManager>();
			if (manager && manager.localBody)
			{
				return manager.localBody;
			}
			Player[] all = UnityEngine.Object.FindObjectsByType<Player>(FindObjectsSortMode.None);
			foreach (Player p in all)
			{
				if (!p.GetComponent<SlapDummy>())
				{
					return p;
				}
			}
			return null;
		}

		private void Watch(Rigidbody rb, bool touchOnly = false)
		{
			if (!rb || watchedBodies.Contains(rb))
			{
				return;
			}
			watchedBodies.Add(rb);
			FistWatch watch = rb.GetComponent<FistWatch>();
			if (!watch)
			{
				watch = rb.gameObject.AddComponent<FistWatch>();
			}
			watch.touchOnly = touchOnly;
		}

		public bool Armed(Rigidbody rb)
		{
			if (IsLocalFist(rb) || IsLocalKick(rb))
			{
				return true;
			}
			if (!rb)
			{
				return false;
			}
			HandGrab[] array = localGrabs;
			foreach (HandGrab g in array)
			{
				if (g && g.Wields(rb))
				{
					return true;
				}
			}
			return false;
		}

		private void RecordVelocity(Rigidbody rb)
		{
			if (!rb)
			{
				return;
			}
			if (!velocityHistory.TryGetValue(rb, out Vector3[] history))
			{
				history = new Vector3[3];
				velocityHistory[rb] = history;
			}
			history[2] = history[1];
			history[1] = history[0];
			history[0] = rb.linearVelocity;
		}

		public bool IsLocalFist(Rigidbody rb)
		{
			if (!localBody || !rb)
			{
				return false;
			}
			return rb == localBody.leftHand || rb == localBody.rightHand;
		}

		public bool IsLocalKick(Rigidbody rb)
		{
			if (!localFighter)
			{
				return false;
			}
			return localFighter.Kicks(rb);
		}

		public float SpeedBefore(Rigidbody rb)
		{
			return VelocityBefore(rb).magnitude;
		}

		public Vector3 VelocityBefore(Rigidbody rb)
		{
			if (!rb)
			{
				return Vector3.zero;
			}
			if (!velocityHistory.TryGetValue(rb, out Vector3[] history))
			{
				return rb.linearVelocity;
			}
			Vector3 average = (history[0] + history[1] + history[2]) / 3f;
			if (average.sqrMagnitude < 1E-06f)
			{
				return Vector3.zero;
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
				return Vector3.zero;
			}
			return sum / count;
		}

		public int Report(int bone, float impulse, float speed, Vector3 point, Rigidbody by, bool touching, float swing = -1f, float handSpeed = -1f)
		{
			if (peaceful || truce || !Armed(by) || Gripping(by))
			{
				return -1;
			}
			if (!netAvatar)
			{
				netAvatar = GetComponent<NetworkPlayer>();
			}
			if (netAvatar && GameManager.SameTeam(netAvatar.Uid.Value, GameManager.MyId))
			{
				return -1;
			}
			bool fist = IsLocalFist(by) || !by;
			bool kick = !fist && IsLocalKick(by);
			if (speed > maxSpeed)
			{
				if (fist)
				{
					Player.Note("punch NOT counted: " + (by ? by.name : "?") + " hit " + NetworkPlayer.Names[bone] + " read as " + speed.ToString("F1") + " m/s, over maxSpeed " + maxSpeed);
				}
				return -1;
			}
			if (localFighter && (localFighter.knockedOut || localFighter.dead))
			{
				return -1;
			}
			if (kick && speed < KickSpeed)
			{
				Player.Note("kick NOT counted: " + by.name + " hit " + NetworkPlayer.Names[bone] + " at " + speed.ToString("F1") + " m/s, under " + KickSpeed);
			}
			float min;
			if (fist)
			{
				if (swingRule)
				{
					if (speed < 3.5f)
					{
						return -1;
					}
				}
				else if (speed < 2f && impulse < 1f)
				{
					return -1;
				}
				min = Player.punchRamp ? 3.5f : (localFighter ? localFighter.minSpeed : 3.5f);
				impulse = fistMass * speed;
			}
			else
			{
				if (speed < (kick ? KickSpeed : PropSpeed))
				{
					return -1;
				}
				min = localFighter ? localFighter.minSpeed : 3.5f;
				if (kick)
				{
					impulse = Mathf.Clamp(speed * KickMass, KickMin, KickMax);
				}
				else
				{
					bool hat = by.GetComponentInParent<LooseHat>();
					float mass = by.mass;
					impulse = hat ? Mathf.Min(PropImpulse(mass, speed), HatMax) : PropImpulse(mass, speed);
				}
			}
			Vector3 dir = VelocityBefore(by) - (localFighter && (kick || fist) ? localFighter.rideCarry : Vector3.zero);
			if (dir.sqrMagnitude < 1E-06f)
			{
				dir = bones[bone].position - point;
			}
			int verdict = JudgeHit(bone, impulse, speed, dir, kick, fist);
			if (touching && verdict != 1)
			{
				if (fist && speed >= 3f)
				{
					Player.Note("punch NOT counted: " + (by ? by.name : "?") + " hit " + NetworkPlayer.Names[bone] + " at " + speed.ToString("F1") + " m/s, an arm while already touching them");
				}
				return -1;
			}
			hits++;
			bool arm = bone == 4 || bone == 5 || bone == 7 || bone == 8;
			bool hurt = verdict == 0 && localFighter && speed >= min && !arm;
			lastHit = NetworkPlayer.Names[bone] + " " + impulse.ToString("F1") + " Ns at " + speed.ToString("F1") + " m/s by " + (by ? by.name : "?") + (verdict == 1 ? " BLOCKED" : (verdict == 2 ? " KO" : ""));
			if (kick)
			{
				Player.Note("kick landed: " + lastHit);
			}
			if (fist)
			{
				string swingText = swing >= 0f ? " (swing " + swing.ToString("F1") + " m/s, hand " + handSpeed.ToString("F1") + " m/s)" : "";
				string why = "";
				if (verdict == 2 || (verdict == 0 && arm))
				{
					why = " (" + (verdict == 2 ? "" : "arm, cannot KO, ") + "no block: " + ((localFighter && speed < min) ? "too slow" : ((netAvatar && netAvatar.GuardUp) ? (FromBehind(dir) ? "from behind" : "hands not up in front of the face") : "guard used up")) + ")";
				}
				Player.Note("punch landed: " + lastHit + swingText + why);
			}
			if (hurt && netAvatar && !localFighter.Brush(kick, speed))
			{
				Sent(localFighter.Hurt(bone == 2, false, impulse, speed, kick, fist));
			}
			onHit?.Invoke(bone, impulse, speed, point, dir.normalized, by, verdict);
			PlayHitFeedback(impulse, speed, dir.normalized, point, by, verdict);
			if (verdict == 2 || hurt)
			{
				Player.HitAt(point, false);
			}
			return verdict;
		}

		private bool FromBehind(Vector3 dir)
		{
			Vector3 facing = (goalAt >= 0f ? goalRot[2] : bodies[2].rotation) * Vector3.forward;
			if (facing.x * facing.x + facing.z * facing.z < 0.0001f || dir.x * dir.x + dir.z * dir.z < 0.0001f)
			{
				return false;
			}
			Vector3 from = new Vector3(dir.x, 0f, dir.z).normalized;
			Vector3 front = new Vector3(facing.x, 0f, facing.z).normalized;
			return Vector3.Dot(from, front) > BackHit;
		}

		private int JudgeHit(int bone, float impulse, float speed, Vector3 dir, bool kick, bool fist)
		{
			if (!netAvatar)
			{
				netAvatar = GetComponent<NetworkPlayer>();
			}
			if (netAvatar && netAvatar.KnockedOut)
			{
				sentHurt.Clear();
				seenKoLeft = -1f;
			}
			if (!netAvatar || !localFighter || netAvatar.KnockedOut || netAvatar.Dead)
			{
				return 0;
			}
			float min = fist && Player.punchRamp ? 3.5f : localFighter.minSpeed;
			if (min > speed)
			{
				return 0;
			}
			if (!kick && Guarded(dir))
			{
				return 1;
			}
			if (bone == 4 || bone == 5 || bone == 7 || bone == 8)
			{
				return 0;
			}
			if (localFighter.Brush(kick, speed))
			{
				return 0;
			}
			if (!kick && netAvatar.GuardUp && FromBehind(dir))
			{
				return 0;
			}
			float hurt = Counted(localFighter.Hurt(bone == 2, false, impulse, speed, kick, fist));
			if (!(netAvatar.KoLeft > 0f) || !(hurt > 0f))
			{
				return 0;
			}
			if (hurt < netAvatar.KoLeft - Unconfirmed())
			{
				return 0;
			}
			return Time.time > netAvatar.popAt + 1.5f ? 2 : 0;
		}

		private float Counted(float hurt)
		{
			if (Time.time < lastSentAt + 0.12f)
			{
				return hurt - lastSent;
			}
			return hurt;
		}

		private void Sent(float hurt)
		{
			float counted = Counted(hurt);
			if (Time.time >= lastSentAt + 0.12f || lastSent < hurt)
			{
				lastSent = hurt;
				lastSentAt = Time.time;
			}
			if (counted > 0f)
			{
				sentHurt.Add(new Vector2(Time.time, counted));
			}
		}

		private float Unconfirmed()
		{
			float koLeft = netAvatar.KoLeft;
			if (seenKoLeft >= 0f)
			{
				float landed = seenKoLeft + localFighter.Recovery() * (Time.time - seenKoAt) - koLeft;
				while (landed > 0f && sentHurt.Count > 0)
				{
					Vector2 sent = sentHurt[0];
					if (sent.y > landed + 0.5f)
					{
						sentHurt[0] = new Vector2(sent.x, sent.y - landed);
						break;
					}
					landed -= sent.y;
					sentHurt.RemoveAt(0);
				}
			}
			seenKoLeft = koLeft;
			seenKoAt = Time.time;
			float total = 0f;
			for (int i = sentHurt.Count - 1; i >= 0; i--)
			{
				if (Time.time > sentHurt[i].x + sentLife)
				{
					sentHurt.RemoveAt(i);
				}
				else
				{
					total += sentHurt[i].y;
				}
			}
			return total;
		}

		private bool Guarded(Vector3 dir)
		{
			if (!netAvatar.GuardUp || FromBehind(dir))
			{
				return false;
			}
			if (!(cols[2] is CapsuleCollider head))
			{
				return false;
			}
			Vector3 position;
			Quaternion rotation;
			if (goalAt >= 0f)
			{
				rotation = goalRot[2];
				position = goalPos[2];
			}
			else
			{
				rotation = bodies[2].rotation;
				position = bodies[2].position;
			}
			Vector3 center = rotation * head.center;
			Vector3 handL = goalAt >= 0f ? goalPos[5] : bodies[5].position;
			Vector3 handR = goalAt >= 0f ? goalPos[8] : bodies[8].position;
			return localFighter.Guard(position + center, rotation, handL, handR);
		}

		private void PlayHitFeedback(float impulse, float speed, Vector3 dir, Vector3 point, Rigidbody by, int verdict)
		{
			if (!localBody)
			{
				return;
			}
			HandGrab hand = HandHolding(by);
			if (verdict == 1)
			{
				Player.BlockAt(point);
				if (hand)
				{
					Player.Hand(hand.node, 0.5f, 0.08f);
				}
				return;
			}
			if (!localSounds)
			{
				localSounds = localBody.GetComponent<Player>();
			}
			if (localSounds)
			{
				localSounds.Punch(IsLocalFist(by) ? by : null, speed, impulse);
			}
			if (hand)
			{
				float strength = IsLocalFist(by) && Player.punchRamp ? Mathf.InverseLerp(3.5f, 4.5f, speed) : Mathf.InverseLerp(2f, 7f, speed);
				Player.Hand(hand.node, Mathf.Lerp(0.2f, 0.7f, strength), 0.05f);
			}
			if (verdict != 2 || !netAvatar)
			{
				return;
			}
			netAvatar.popAt = Time.time;
			predictedAt = Time.time;
			predictedHead = goalAt >= 0f ? goalPos[2].y : bodies[2].position.y;
			poppedAt = Time.time;
			Player.KnockAt(bodies[2].position);
			if (hand)
			{
				Player.Hand(hand.node, 1f, 0.25f);
			}
		}

		private void Fists(bool on)
		{
			fistsOn = on;
			Player me = localBody ? localBody : FindLocalBody();
			if (!me)
			{
				return;
			}
			Rigidbody[] hands = new Rigidbody[2] { me.leftHand, me.rightHand };
			foreach (Rigidbody hand in hands)
			{
				if (!hand)
				{
					continue;
				}
				Collider[] components = hand.GetComponents<Collider>();
				foreach (Collider c in components)
				{
					Collider[] array = cols;
					foreach (Collider mine in array)
					{
						AvatarHit hit = mine.GetComponent<AvatarHit>();
						if (!hit || hit.index < 3 || hit.index > 8)
						{
							Physics.IgnoreCollision(mine, c, !on);
						}
					}
				}
			}
		}

		private void CollideWithLocal(bool on)
		{
			fistsOn = false;
			passThrough = !on;
			Player me = localBody ? localBody : FindLocalBody();
			if (!me)
			{
				return;
			}
			localColliders = me.GetComponentsInChildren<Collider>();
			Collider[] array = localColliders;
			foreach (Collider c in array)
			{
				Collider[] array2 = cols;
				foreach (Collider mine in array2)
				{
					Physics.IgnoreCollision(mine, c, !on);
				}
			}
		}

		private bool OverlapsLocal(float depth)
		{
			if (localColliders == null)
			{
				return false;
			}
			if (localBounds == null || localBounds.Length != localColliders.Length)
			{
				localBounds = new Bounds[localColliders.Length];
			}
			Bounds mine = cols[0].bounds;
			for (int i = 1; i < cols.Length; i++)
			{
				mine.Encapsulate(cols[i].bounds);
			}
			bool any = false;
			for (int j = 0; j < localColliders.Length; j++)
			{
				Collider c = localColliders[j];
				localBounds[j] = (c && c.enabled && !c.isTrigger) ? c.bounds : new Bounds(Vector3.one * 1000000f, Vector3.zero);
				any |= mine.Intersects(localBounds[j]);
			}
			if (!any)
			{
				return false;
			}
			Collider[] array = cols;
			foreach (Collider col in array)
			{
				Bounds bounds = col.bounds;
				Transform t = col.transform;
				for (int k = 0; k < localColliders.Length; k++)
				{
					if (bounds.Intersects(localBounds[k]))
					{
						Collider other = localColliders[k];
						Transform o = other.transform;
						if (Physics.ComputePenetration(col, t.position, t.rotation, other, o.position, o.rotation, out Vector3 _, out float distance) && distance > depth)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private static bool Platform(Rigidbody rb)
		{
			if (!rb.GetComponent<NetRope>())
			{
				return false;
			}
			return !rb.GetComponentInParent<Rope>();
		}

		private void CollideWithWorld(bool on)
		{
			collidesWithWorld = on;
			foreach (Collider world in worldColliders)
			{
				if (!world)
				{
					continue;
				}
				Collider[] array = cols;
				foreach (Collider c in array)
				{
					Physics.IgnoreCollision(c, world, !on);
				}
			}
		}

		public void DropCarry()
		{
			HandGrab[] array = localGrabs;
			foreach (HandGrab g in array)
			{
				if (g && g.grabbing && !g.holdsYou && g.held && g.held.transform.IsChildOf(transform))
				{
					g.releaseWhy = "woke";
					g.Release();
				}
			}
		}

		public void DropHold()
		{
			if (localFighter && !localFighter.grounded)
			{
				return;
			}
			HandGrab[] array = localGrabs;
			foreach (HandGrab g in array)
			{
				if (g && g.grabbing && g.holdsYou && g.held && g.held.transform.IsChildOf(transform))
				{
					g.releaseWhy = "ko";
					g.Release();
					g.regrabUntil = Time.time + 0.3f;
				}
			}
		}

		private bool Gripping(Rigidbody by)
		{
			HandGrab g = by ? by.GetComponent<HandGrab>() : null;
			if (!g || !g.grabbing || !g.held)
			{
				return false;
			}
			return g.held.transform.IsChildOf(transform);
		}

		private bool HeldByLocal()
		{
			HandGrab[] array = localGrabs;
			foreach (HandGrab g in array)
			{
				if (g && g.grabbing && g.held && g.held.transform.IsChildOf(transform))
				{
					return true;
				}
			}
			return false;
		}

		private bool ClimbedByLocal()
		{
			HandGrab[] array = localGrabs;
			foreach (HandGrab g in array)
			{
				if (g && g.grabbing && g.holdsYou && g.held && g.held.transform.IsChildOf(transform))
				{
					return true;
				}
			}
			return false;
		}

		private bool CarriedByLocal()
		{
			HandGrab[] array = localGrabs;
			foreach (HandGrab g in array)
			{
				if (g && g.grabbing && !g.holdsYou && g.held && g.held.transform.IsChildOf(transform))
				{
					return true;
				}
			}
			return false;
		}

		private HandGrab HandHolding(Rigidbody by)
		{
			if (!by)
			{
				return null;
			}
			HandGrab own = by.GetComponent<HandGrab>();
			if (own)
			{
				return own;
			}
			HandGrab[] array = localGrabs;
			foreach (HandGrab g in array)
			{
				if (g && (g.held == by || g.lastHeld == by))
				{
					return g;
				}
			}
			return null;
		}

		public void Knock(Vector3 push)
		{
			if (!driven)
			{
				return;
			}
			Pose(false);
			float until = Time.time + koHold;
			for (int i = 0; i < NetworkPlayer.Bones; i++)
			{
				impactTimes[i] = until;
			}
			impactAt = until;
			limpUntil = until + impactHold;
			push = Vector3.ClampMagnitude(push, knockMax);
			bodies[2].AddForce(push, ForceMode.Impulse);
			bodies[1].AddForce(push * 0.5f, ForceMode.Impulse);
		}
	}
}
