using System.Collections.Generic;
using UnityEngine;

namespace Ragdoll
{
	public class Hazard : MonoBehaviour
	{
		private struct Touch
		{
			public Hazard by;

			public float at;

			public Vector3 outward;
		}

		[Header("Spin")]
		public float spin;

		public Vector3 axis = Vector3.right;

		public bool topple;

		[Header("Belt")]
		public float belt;

		public float beltWidth = 1.4f;

		public float beltBend = 3.5f;

		[Header("Crush")]
		public bool crush;

		[Header("Wire")]
		public bool wire;

		public float push = 3f;

		public float lift = 1.5f;

		public float hold = 25f;

		[Header("Slip")]
		public bool slick;

		public float slipKick = 3.5f;

		[Header("Fan")]
		public float fan;

		public float fanHeight = 4f;

		public Transform blades;

		[Header("Ring")]
		public bool keepIn;

		[Header("Shock")]
		public bool shock;

		private static readonly Dictionary<Rigidbody, Touch> touches = new Dictionary<Rigidbody, Touch>();

		private static string pinched;

		private static Vector3 pinchedAt;

		private static bool wired;

		private static Vector3 wirePush;

		private static Hazard slipped;

		private static Vector3 slipWay;

		private static bool slipSpent;

		private static float oilAt = -9f;

		private const float SlipSpeed = 0.6f;

		private const float DryTraction = 0.5f;

		private const float FanRise = 10f;

		private const float BladeSpin = 600f;

		private const float ToppleSpeed = 2.5f;

		private const float ToppleUp = 1f;

		private const float NpcWireGap = 0.6f;

		private const float NpcWireOut = 3f;

		private const float NpcWireUp = 3f;

		private const float NpcWirePass = 1f;

		private const float RingEdge = 0.3f;

		private readonly Dictionary<Player, float> passing = new Dictionary<Player, float>();

		private readonly List<Player> passed = new List<Player>();

		private static readonly Dictionary<Player, float> npcWired = new Dictionary<Player, float>();

		private static readonly Collider[] blown = new Collider[96];

		private static readonly Dictionary<Player, Rigidbody[]> parts = new Dictionary<Player, Rigidbody[]>();

		private readonly HashSet<Rigidbody> pushed = new HashSet<Rigidbody>();

		private readonly HashSet<Player> lifted = new HashSet<Player>();

		private BoxCollider column;

		private static readonly List<Hazard> rings = new List<Hazard>();

		private Bounds ringBox;

		private Rigidbody body;

		private Quaternion start;

		private Vector3 middle;

		private float angle = float.NaN;

		private float ring;

		private readonly List<Rigidbody> lanes = new List<Rigidbody>();

		private readonly List<Vector3> laneRest = new List<Vector3>();

		private readonly List<Vector3> laneDir = new List<Vector3>();

		private readonly List<Rigidbody> corners = new List<Rigidbody>();

		private readonly List<Quaternion> cornerRest = new List<Quaternion>();

		private readonly List<Mesh> arcs = new List<Mesh>();

		private const float CornerReach = 0.55f;

		private const float CoreDrop = 0.12f;

		private const float CoreShrink = 0.97f;

		private const int CoreWedges = 24;

		private static readonly int[] WedgeTris = new int[36]
		{
			0, 1, 3, 0, 3, 2, 4, 6, 7, 4,
			7, 5, 0, 4, 5, 0, 5, 1, 2, 3,
			7, 2, 7, 6, 0, 2, 6, 0, 6, 4,
			1, 5, 7, 1, 7, 3
		};

		private const float SpinRush = 0.5f;

		private Renderer shown;

		private int shownSlot = -1;

		private MaterialPropertyBlock block;

		private float scroll;

		private float lap;

		private static readonly Dictionary<Rigidbody, Hazard> laneOwner = new Dictionary<Rigidbody, Hazard>();

		private static float Rush => 1f + (GameManager.BeltRush - 1f) * SpinRush;

		private bool Parked => topple && GameManager.match != null && !(GameManager.SinceStart > 0f);

		private float Speed => belt * GameManager.BeltRush;

		private void Awake()
		{
			if (keepIn)
			{
				Ring();
			}
			start = transform.localRotation;
			Renderer r = GetComponentInChildren<Renderer>();
			middle = r ? r.bounds.center : transform.position;
			if (belt != 0f && r)
			{
				ring = Mathf.Max(r.bounds.extents.x, r.bounds.extents.z) - beltWidth * 0.5f;
				float bend = Mathf.Max(beltBend, beltWidth * 0.5f);
				float straight = ring - bend;
				lap = (straight * 2f + bend * (Mathf.PI * 0.5f)) * 4f;
				block = new MaterialPropertyBlock();
				Material[] mats = r.sharedMaterials;
				for (int i = 0; i < mats.Length; i++)
				{
					if (mats[i] && mats[i].HasProperty("_Inner"))
					{
						shown = r;
						shownSlot = i;
						r.GetPropertyBlock(block, i);
						block.SetFloat("_Speed", 0f);
						block.SetFloat("_Scroll", 0f);
						block.SetFloat("_Inner", ring - beltWidth * 0.5f);
						block.SetFloat("_Bend", beltBend);
						block.SetFloat("_Width", beltWidth);
						block.SetVector("_Center", middle);
						r.SetPropertyBlock(block, i);
					}
				}
				Lanes(r.bounds);
			}
			if (spin == 0f)
			{
				return;
			}
			body = GetComponent<Rigidbody>();
			if (!body)
			{
				body = gameObject.AddComponent<Rigidbody>();
			}
			body.isKinematic = true;
			body.useGravity = false;
			body.interpolation = RigidbodyInterpolation.Interpolate;
			MeshCollider mc = GetComponent<MeshCollider>();
			if (mc && !mc.convex && mc.sharedMesh && Mathf.Abs(Vector3.Dot(transform.TransformDirection(axis).normalized, Vector3.up)) > 0.9f)
			{
				Core(mc.sharedMesh);
			}
		}

		private void Core(Mesh mesh)
		{
			Vector3 up = transform.InverseTransformDirection(Vector3.up);
			int a = (Mathf.Abs(up.x) > Mathf.Abs(up.y)) ? ((Mathf.Abs(up.x) > Mathf.Abs(up.z)) ? 0 : 2) : ((Mathf.Abs(up.y) > Mathf.Abs(up.z)) ? 1 : 2);
			int u = (a + 1) % 3;
			int v = (a + 2) % 3;
			Bounds b = mesh.bounds;
			float top = (up[a] > 0f) ? b.max[a] : (0f - b.min[a]);
			if (top <= CoreDrop)
			{
				return;
			}
			float outer = 0f;
			float inner = float.MaxValue;
			foreach (Vector3 p in mesh.vertices)
			{
				float r = new Vector2(p[u] - b.center[u], p[v] - b.center[v]).magnitude;
				inner = Mathf.Min(inner, r);
				outer = Mathf.Max(outer, r);
			}
			float rimIn = inner + outer * (1f - CoreShrink);
			float rimOut = outer * CoreShrink;
			if (rimOut <= rimIn)
			{
				return;
			}
			float high = (up[a] < 0f) ? (0f - (top - CoreDrop)) : (top - CoreDrop);
			float low = (up[a] > 0f) ? b.min[a] : b.max[a];
			GameObject core = new GameObject(name + " Core");
			core.layer = gameObject.layer;
			core.transform.SetParent(transform, false);
			for (int i = 0; i < CoreWedges; i++)
			{
				Vector3[] pts = new Vector3[8];
				for (int k = 0; k < 8; k++)
				{
					float r = ((k & 2) == 0) ? rimIn : rimOut;
					Vector3 p = b.center;
					float t = (i + (k >> 2)) * Mathf.PI * 2f / CoreWedges;
					p[u] += r * Mathf.Cos(t);
					p[v] += r * Mathf.Sin(t);
					p[a] = ((k & 1) == 0) ? low : high;
					pts[k] = p;
				}
				Mesh wedge = new Mesh();
				wedge.name = $"{name} Core {i}";
				wedge.vertices = pts;
				wedge.triangles = WedgeTris;
				MeshCollider mc = core.AddComponent<MeshCollider>();
				mc.sharedMesh = wedge;
				mc.convex = true;
			}
		}

		private void OnDisable()
		{
			touches.Clear();
		}

		private void OnDestroy()
		{
			rings.Remove(this);
			parts.Clear();
			npcWired.Clear();
			foreach (Rigidbody lane in lanes)
			{
				if (lane)
				{
					laneOwner.Remove(lane);
				}
			}
			foreach (Rigidbody corner in corners)
			{
				if (corner)
				{
					laneOwner.Remove(corner);
				}
			}
			foreach (Mesh arc in arcs)
			{
				if (arc)
				{
					Object.Destroy(arc);
				}
			}
		}

		private void Update()
		{
			if (blades)
			{
				blades.Rotate(Vector3.forward, Time.deltaTime * BladeSpin, Space.Self);
			}
			if (belt == 0f || !shown)
			{
				return;
			}
			scroll = Mathf.Repeat(scroll + Speed * Time.deltaTime, Mathf.Max(lap, 0.01f));
			shown.GetPropertyBlock(block, shownSlot);
			block.SetFloat("_Scroll", scroll);
			shown.SetPropertyBlock(block, shownSlot);
		}

		private void Lanes(Bounds b)
		{
			foreach (Collider col in GetComponents<Collider>())
			{
				col.enabled = false;
			}
			Vector3[] dirs = new Vector3[4]
			{
				Vector3.right,
				Vector3.forward,
				Vector3.left,
				Vector3.back
			};
			float half = ring - beltWidth * 0.5f;
			foreach (Vector3 dir in dirs)
			{
				Vector3 side = Vector3.Cross(dir, Vector3.up);
				Vector3 at = new Vector3(middle.x + dir.x * ring, b.center.y, middle.z + dir.z * ring);
				Rigidbody lane = Part(name + " Lane", at, Quaternion.LookRotation(side, Vector3.up), new Vector3(beltWidth, b.size.y, half * 2f), null);
				lanes.Add(lane);
				laneRest.Add(at);
				laneDir.Add(side);
				Quaternion turn = Quaternion.LookRotation(side, Vector3.up);
				Rigidbody corner = Part(name + " Corner", new Vector3(middle.x + (dir.x + side.x) * half, b.center.y, middle.z + (dir.z + side.z) * half), turn, Vector3.zero, Arc(b.size.y));
				corners.Add(corner);
				cornerRest.Add(turn);
			}
		}

		private Mesh Arc(float height)
		{
			List<Vector3> verts = new List<Vector3>();
			List<int> tris = new List<int>();
			for (int i = 0; i <= 12; i++)
			{
				float t = i * Mathf.PI * 0.5f / 12f;
				float c = Mathf.Cos(t);
				float s = Mathf.Sin(t);
				Vector2 rim = new Vector2(c, s) * (beltWidth / Mathf.Max(c, s));
				Vector2 hub = Vector2.zero;
				verts.Add(new Vector3(hub.x, height * 0.5f, hub.y));
				verts.Add(new Vector3(rim.x, height * 0.5f, rim.y));
				verts.Add(new Vector3(hub.x, (0f - height) * 0.5f, hub.y));
				verts.Add(new Vector3(rim.x, (0f - height) * 0.5f, rim.y));
			}
			for (int j = 0; j < 12; j++)
			{
				int o = j * 4;
				tris.AddRange(new int[12]
				{
					o, o + 4, o + 5, o, o + 5, o + 1, o + 2, o + 7, o + 6, o + 2,
					o + 3, o + 7
				});
			}
			Mesh mesh = new Mesh();
			mesh.name = name + " Corner";
			mesh.SetVertices(verts);
			mesh.SetTriangles(tris, 0);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			arcs.Add(mesh);
			return mesh;
		}

		private Rigidbody Part(string label, Vector3 at, Quaternion facing, Vector3 size, Mesh mesh)
		{
			GameObject go = new GameObject(label);
			go.layer = gameObject.layer;
			go.transform.SetParent(transform.parent, false);
			go.transform.SetPositionAndRotation(at, facing);
			if (mesh)
			{
				go.AddComponent<MeshCollider>().sharedMesh = mesh;
			}
			else
			{
				go.AddComponent<BoxCollider>().size = size;
			}
			Rigidbody rb = go.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;
			rb.centerOfMass = Vector3.zero;
			laneOwner[rb] = this;
			return rb;
		}

		public static Hazard Of(Rigidbody rb)
		{
			if (!rb)
			{
				return null;
			}
			Hazard hazard = rb.GetComponent<Hazard>();
			if (hazard)
			{
				return hazard;
			}
			return laneOwner.TryGetValue(rb, out hazard) ? hazard : null;
		}

		private void FixedUpdate()
		{
			Settle();
			if (fan > 0f)
			{
				Blow();
			}
			if (passing.Count > 0)
			{
				Unpass();
			}
			if (keepIn)
			{
				KeepIn();
			}
			float speed = Speed;
			for (int i = 0; i < lanes.Count; i++)
			{
				Rigidbody lane = lanes[i];
				if (lane)
				{
					lane.position = laneRest[i] - laneDir[i] * (speed * Time.fixedDeltaTime);
					lane.MovePosition(laneRest[i]);
				}
			}
			for (int j = 0; j < corners.Count; j++)
			{
				Rigidbody corner = corners[j];
				if (corner)
				{
					float degrees = -speed / (beltWidth * CornerReach) * Mathf.Rad2Deg * Time.fixedDeltaTime;
					corner.rotation = cornerRest[j];
					corner.MoveRotation(Quaternion.AngleAxis(degrees, Vector3.up) * cornerRest[j]);
				}
			}
			if (!body || spin == 0f)
			{
				return;
			}
			float clock = (topple && GameManager.match != null) ? (Parked ? 0f : GameManager.SinceStart) : NetworkManager.Clock;
			float target = Mathf.Repeat(spin * (clock + GameManager.BeltLead * 0.5f), 360f);
			if (float.IsNaN(angle))
			{
				angle = target;
			}
			float ahead = Mathf.Repeat(Mathf.Sign(spin) * (target - angle), 360f);
			float rate = Mathf.Abs(spin) * Rush * Time.fixedDeltaTime;
			float step = (ahead > 180f) ? (rate * SpinRush) : Mathf.Min(ahead, rate * 2f);
			angle = Mathf.Repeat(angle + Mathf.Sign(spin) * step, 360f);
			Quaternion rot = start * Quaternion.AngleAxis(angle, axis);
			Rigidbody rigidbody = body;
			if (transform.parent)
			{
				rot = transform.parent.rotation * rot;
			}
			rigidbody.MoveRotation(rot);
		}

		private void Blow()
		{
			if (!column)
			{
				column = GetComponent<BoxCollider>();
			}
			if (!column)
			{
				return;
			}
			Vector3 center = transform.TransformPoint(column.center);
			Vector3 half = Vector3.Scale(column.size, transform.lossyScale) * 0.5f;
			int count = Physics.OverlapBoxNonAlloc(center, half, blown, transform.rotation, -1, QueryTriggerInteraction.Ignore);
			pushed.Clear();
			lifted.Clear();
			float bottom = center.y - half.y;
			for (int i = 0; i < count; i++)
			{
				Rigidbody rb = blown[i].attachedRigidbody;
				if (!rb || rb.isKinematic || !pushed.Add(rb))
				{
					continue;
				}
				Player p = rb.GetComponentInParent<Player>();
				if (!p)
				{
					if (rb.linearVelocity.y < FanRise)
					{
						rb.AddForce(Vector3.up * FanPush(rb.worldCenterOfMass.y - bottom), ForceMode.Acceleration);
					}
				}
				else if (lifted.Add(p) && p.pelvis)
				{
					Lift(p, FanPush(p.pelvis.position.y - bottom));
				}
			}
		}

		private float FanPush(float height)
		{
			return fan * (1f - Mathf.Clamp01(height / Mathf.Max(fanHeight, 0.1f)));
		}

		private static void Lift(Player p, float push)
		{
			if (push <= 0f || p.pelvis.linearVelocity.y >= FanRise)
			{
				return;
			}
			if (!parts.TryGetValue(p, out Rigidbody[] bodies) || bodies.Length == 0 || !bodies[0])
			{
				bodies = p.GetComponentsInChildren<Rigidbody>();
				parts[p] = bodies;
			}
			foreach (Rigidbody rb in bodies)
			{
				if (rb && !rb.isKinematic)
				{
					rb.AddForce(Vector3.up * push, ForceMode.Acceleration);
				}
			}
		}

		private static void Settle()
		{
			Player me = Player.main;
			if (pinched != null)
			{
				if (me && !me.knockedOut && !me.dead)
				{
					Player.Note($"crushed by the grinder, {pinched} pinched");
					me.KnockOut(pinchedAt, false);
				}
				pinched = null;
			}
			if (slipSpent && me && Time.fixedTime > oilAt + Time.fixedDeltaTime * 2f && me.grounded && !me.flopped && !me.rising && me.traction >= DryTraction)
			{
				slipSpent = false;
			}
			if (slipped)
			{
				Hazard hazard = slipped;
				slipped = null;
				if (me && !me.flopped && !me.rising && !me.knockedOut && !me.dead)
				{
					hazard.Slip(me);
				}
			}
			if (wired)
			{
				wired = false;
				if (me && !me.flopped && !me.rising && !me.knockedOut && !me.dead)
				{
					Player.Note("barbed wire");
					me.LetGo(null);
					me.Knock(wirePush);
				}
			}
		}

		public float Angle()
		{
			return angle;
		}

		public bool Turning(Rigidbody rb)
		{
			return corners.Contains(rb);
		}

		public Vector3 Ride(Vector3 point)
		{
			Vector3 along = Along(point);
			if (along.sqrMagnitude > 0.0001f)
			{
				return along.normalized * Speed;
			}
			return Vector3.zero;
		}

		public Vector3 SurfaceVelocity(Vector3 point)
		{
			if (belt != 0f)
			{
				return Along(point) * Speed;
			}
			if (!body || Parked)
			{
				return Vector3.zero;
			}
			return Vector3.Cross(transform.TransformDirection(axis).normalized * (spin * Rush * Mathf.Deg2Rad), point - transform.position);
		}

		private void OnCollisionEnter(Collision c)
		{
			if (crush)
			{
				Crushing(c);
			}
			if (wire)
			{
				Wire(c);
			}
			if (topple)
			{
				Topple(c);
			}
			if (shock)
			{
				Shock(c);
			}
		}

		private void Topple(Collision c)
		{
			Player p = Player.Of(c.rigidbody);
			if (!p || p.flopped || p.knockedOut || p.dead || !p.pelvis || p.pelvis.isKinematic)
			{
				return;
			}
			if (p != Player.main && !p.dummy)
			{
				return;
			}
			Vector3 at = (c.contactCount > 0) ? c.GetContact(0).point : p.pelvis.position;
			if (c.contactCount >= 1 && Mathf.Abs(c.GetContact(0).normal.y) > 0.7f)
			{
				return;
			}
			Vector3 push = SurfaceVelocity(at);
			push.y = 0f;
			Vector3 toward = c.rigidbody.worldCenterOfMass - at;
			toward.y = 0f;
			if (Vector3.Dot(push, toward) <= 0f)
			{
				return;
			}
			p.LetGo("knocked over by the pipe");
			p.Knock(Vector3.ClampMagnitude(push, ToppleSpeed) + Vector3.up * ToppleUp);
			if (p == Player.main)
			{
				Player.Note("knocked over by " + name + " at " + push.magnitude.ToString("F1") + " m/s");
			}
		}

		private void Shock(Collision c)
		{
			Player p = Player.Of(c.rigidbody);
			if (!p || p.dead || p.ghost || p.knockedOut || !p.pelvis || p.pelvis.isKinematic || p.Shielded())
			{
				return;
			}
			if (p != Player.main && !p.dummy)
			{
				return;
			}
			if (p == Player.main)
			{
				Player.Note("touched " + name + " with " + c.rigidbody.name);
			}
			p.KnockOut(middle, true);
		}

		private void OnCollisionStay(Collision c)
		{
			if (crush)
			{
				Crushing(c);
			}
			if (wire)
			{
				Wire(c);
			}
			if (shock)
			{
				Shock(c);
			}
		}


		private void OnTriggerStay(Collider other)
		{
			if (slick)
			{
				Slick(other.attachedRigidbody);
			}
		}

		private void Slick(Rigidbody rb)
		{
			Player me = Player.main;
			if (!me || !rb || !me.IsMine(rb.transform))
			{
				return;
			}
			oilAt = Time.fixedTime;
			if (slipped || slipSpent || me.dead || me.knockedOut || me.flopped || me.rising || me.holding || me.gripping)
			{
				return;
			}
			Vector3 v = me.pelvis.linearVelocity;
			Vector3 flat = new Vector3(v.x, 0f, v.z);
			if (!(flat.magnitude < SlipSpeed))
			{
				slipped = this;
				slipWay = flat.normalized;
			}
		}

		private void Slip(Player me)
		{
			slipSpent = true;
			Player.Note("slipped on " + name + " at " + me.pelvis.linearVelocity.magnitude.ToString("F1") + " m/s");
			me.Knock(Vector3.zero);
			Vector3 kick = slipWay * slipKick + Vector3.up * slipKick * 0.6f;
			foreach (Rigidbody leg in new Rigidbody[4] { me.leftUpperLeg, me.leftLowerLeg, me.rightUpperLeg, me.rightLowerLeg })
			{
				if (leg)
				{
					leg.AddForce(kick, ForceMode.VelocityChange);
				}
			}
			Vector3 back = -slipWay * slipKick * 0.4f;
			foreach (Rigidbody top in new Rigidbody[2] { me.chest, me.head })
			{
				if (top)
				{
					top.AddForce(back, ForceMode.VelocityChange);
				}
			}
		}

		private Vector3 Along(Vector3 point)
		{
			float dx = point.x - middle.x;
			float dz = point.z - middle.z;
			float half = ring - beltWidth * 0.5f;
			if (Mathf.Abs(dx) > half && Mathf.Abs(dz) > half)
			{
				float cx = dx - Mathf.Sign(dx) * half;
				float cz = dz - Mathf.Sign(dz) * half;
				return new Vector3(0f - cz, 0f, cx) / (beltWidth * CornerReach);
			}
			if (Mathf.Abs(dx) >= Mathf.Abs(dz))
			{
				return (dx > 0f) ? Vector3.forward : Vector3.back;
			}
			return (dz > 0f) ? Vector3.left : Vector3.right;
		}

		private void Crushing(Collision c)
		{
			Player me = Player.main;
			Rigidbody rb = c.rigidbody;
			if (!me || !rb || c.contactCount == 0 || me.knockedOut || me.dead || !me.IsMine(rb.transform))
			{
				return;
			}
			Vector3 ax = transform.TransformDirection(axis).normalized;
			Vector3 r = c.GetContact(0).point - transform.position;
			Vector3 outward = (r - ax * Vector3.Dot(ax, r)).normalized;
			if (touches.TryGetValue(rb, out Touch t) && t.by && t.by != this && t.at == Time.fixedTime && Vector3.Dot(outward, t.outward) < -0.5f)
			{
				pinched = rb.name;
				pinchedAt = rb.position;
			}
			touches[rb] = new Touch
			{
				by = this,
				at = Time.fixedTime,
				outward = outward
			};
		}

		private void Wire(Collision c)
		{
			Player me = Player.main;
			Rigidbody rb = c.rigidbody;
			if (!rb)
			{
				return;
			}
			Vector3 at = (c.contactCount > 0) ? c.GetContact(0).point : rb.position;
			Vector3 away = middle - at;
			away.y = 0f;
			if (away.sqrMagnitude < 0.0001f)
			{
				return;
			}
			away.Normalize();
			if (!me || me.dead || !me.IsMine(rb.transform))
			{
				NpcWire(rb, away);
				return;
			}
			if (me.flopped || me.rising || me.knockedOut)
			{
				rb.AddForce(away * hold, ForceMode.Acceleration);
				return;
			}
			wired = true;
			wirePush = away * push + Vector3.up * lift;
		}

		private void Ring()
		{
			column = GetComponent<BoxCollider>();
			if (!column)
			{
				return;
			}
			Vector3 half = column.size * 0.5f;
			ringBox = new Bounds(transform.TransformPoint(column.center), Vector3.zero);
			for (int i = 0; i < 8; i++)
			{
				ringBox.Encapsulate(transform.TransformPoint(column.center + new Vector3(((i & 1) == 0) ? (0f - half.x) : half.x, ((i & 2) == 0) ? (0f - half.y) : half.y, ((i & 4) == 0) ? (0f - half.z) : half.z)));
			}
			column.enabled = false;
			rings.Add(this);
		}

		public static bool OutOfRing(Vector3 p)
		{
			foreach (Hazard ring in rings)
			{
				if (ring && (p.x < ring.ringBox.min.x + RingEdge || p.x > ring.ringBox.max.x - RingEdge || p.z < ring.ringBox.min.z + RingEdge || p.z > ring.ringBox.max.z - RingEdge))
				{
					return true;
				}
			}
			return false;
		}

		private void KeepIn()
		{
			if (!rings.Contains(this))
			{
				return;
			}
			Player me = Player.main;
			if (me && !me.dummy)
			{
				Out(me, ringBox);
			}
			for (int i = SlapDummy.alive.Count - 1; i >= 0; i--)
			{
				SlapDummy dummy = (i < SlapDummy.alive.Count) ? SlapDummy.alive[i] : null;
				if (dummy && !dummy.Gone && dummy.kind != SlapDummy.Kind.Dummy)
				{
					Player npc = dummy.Body;
					if (npc)
					{
						Out(npc, ringBox);
					}
				}
			}
		}

		private static void Out(Player p, Bounds inside)
		{
			if (p.dead || !p.pelvis || p.pelvis.isKinematic)
			{
				return;
			}
			Vector3 at = p.pelvis.position;
			if (at.x > inside.min.x && at.x < inside.max.x && at.z > inside.min.z && at.z < inside.max.z)
			{
				return;
			}
			if (p.grounded || !(at.y > inside.min.y))
			{
				p.Kill("fell out of the ring");
			}

		}

		private void Pass(Player p, bool on)
		{
			Collider[] mine = GetComponentsInChildren<Collider>();
			foreach (Collider a in p.GetComponentsInChildren<Collider>())
			{
				foreach (Collider b in mine)
				{
					if (a && b && !b.isTrigger)
					{
						Physics.IgnoreCollision(a, b, on);
					}
				}
			}
		}

		private void Unpass()
		{
			passed.Clear();
			foreach (KeyValuePair<Player, float> pass in passing)
			{
				if (!pass.Key || Time.time >= pass.Value)
				{
					passed.Add(pass.Key);
				}
			}
			foreach (Player p in passed)
			{
				if (p)
				{
					Pass(p, false);
				}
				passing.Remove(p);
				npcWired.Remove(p);
			}
		}

		private void NpcWire(Rigidbody rb, Vector3 inward)
		{
			if (rb.isKinematic)
			{
				return;
			}
			Player npc = rb.GetComponentInParent<Player>();
			if (!npc || !npc.dummy || npc.dead)
			{
				return;
			}
			if (npcWired.TryGetValue(npc, out float at) && Time.time < at + NpcWireGap)
			{
				rb.AddForce(inward * hold, ForceMode.Acceleration);
				return;
			}
			npcWired[npc] = Time.time;
			npc.LetGo("barbed wire");
			npc.Knock(Vector3.zero);
			Pass(npc, true);
			passing[npc] = Time.time + NpcWirePass;
			foreach (Rigidbody part in npc.GetComponentsInChildren<Rigidbody>())
			{
				if (part && !part.isKinematic)
				{
					part.linearVelocity = inward * NpcWireOut + Vector3.up * NpcWireUp;
				}
			}
		}

	}
}
