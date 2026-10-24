using System.Collections.Generic;
using UnityEngine;

namespace Ragdoll
{
	public class Floater : MonoBehaviour
	{
		[Header("Float")]
		public float ride = 0.3f;

		public float swell = 0.12f;

		public float period = 7f;

		public float roll = 2f;

		[Header("Flight")]
		public bool flight;

		public float descent = 0.03f;

		public float sink = 1f;

		public float stiffness = 3f;

		public float wander = 1.5f;

		public float upright = 2f;

		public Rigidbody envelope;

		public Rigidbody basket;

		public Rope[] ropes;

		[Header("Formation")]
		public bool formations = true;

		[HideInInspector]
		public float testTime = -1f;

		public static int forceLayout = -1;

		private static int arrangedScene = -1;

		private const float FreeCeiling = 35f;

		private const float FreeRise = 2.5f;

		private const float FreeSettle = 0.8f;

		private const float FreeGrip = 4f;

		private static readonly Vector2[][] layouts = new Vector2[4][]
		{
			new Vector2[5]
			{
				new Vector2(-8f, -4f),
				new Vector2(-8f, 4f),
				new Vector2(0f, -4f),
				new Vector2(0f, 4f),
				new Vector2(8f, -4f)
			},
			new Vector2[5]
			{
				new Vector2(-16f, 0f),
				new Vector2(-8f, 0f),
				new Vector2(8f, 0f),
				new Vector2(0f, 0f),
				new Vector2(16f, 0f)
			},
			new Vector2[5]
			{
				new Vector2(-4f, -12f),
				new Vector2(-4f, -4f),
				new Vector2(-4f, 12f),
				new Vector2(-4f, 4f),
				new Vector2(-4f, 20f)
			},
			new Vector2[5]
			{
				new Vector2(-8f, -8f),
				new Vector2(-8f, 0f),
				new Vector2(0f, 8f),
				new Vector2(0f, 0f),
				new Vector2(8f, 8f)
			}
		};

		private static readonly int[,] chainLinks = new int[5, 2]
		{
			{ 0, 1 },
			{ 1, 3 },
			{ 3, 2 },
			{ 1, 3 },
			{ 2, 4 }
		};

		private static readonly float[] chainSide = new float[5] { 0f, -0.6f, 0f, 0.6f, 0f };

		private static readonly Collider[] near = new Collider[96];

		private static readonly HashSet<Object> riders = new HashSet<Object>();

		private Vector3 home;

		private Quaternion rest;

		private float phase;

		private Rigidbody body;

		private float load;

		private float ropeMass;

		private int seen;

		private float hanging;

		private NetRope net;

		private Quaternion envRest;

		private void Awake()
		{
			if (!flight || !formations || !Application.isPlaying)
			{
				return;
			}
			if (arrangedScene == gameObject.scene.handle)
			{
				return;
			}
			arrangedScene = gameObject.scene.handle;
			Arrange(transform.parent, forceLayout >= 0 ? forceLayout : (GameManager.match != null ? GameManager.match.round : 0));
		}

		private static void Arrange(Transform group, int round)
		{
			int layout = round % layouts.Length;
			if (!group)
			{
				return;
			}
			bool five = group.Find("Balloon 5");
			int count = five ? 5 : 4;
			if (layout == 0 && !five)
			{
				return;
			}
			Floater[] balloons = new Floater[count];
			for (int i = 0; i < balloons.Length; i++)
			{
				Transform t = group.Find("Balloon " + (i + 1));
				balloons[i] = t ? t.GetComponent<Floater>() : null;
				if (!balloons[i] || !balloons[i].basket)
				{
					return;
				}
			}
			Transform links = group.Find("Balloon Links");
			Rope[] chain = new Rope[count];
			for (int j = 0; j < chain.Length; j++)
			{
				Transform child = (links && j < links.childCount) ? links.GetChild(j) : null;
				chain[j] = child ? child.GetComponent<Rope>() : null;
				if (!chain[j] || !chain[j].end)
				{
					return;
				}
			}
			List<NetProp> planks = new List<NetProp>();
			List<int[]> pairs = new List<int[]>();
			List<Vector3> offsets = new List<Vector3>();
			List<Quaternion> turns = new List<Quaternion>();
			foreach (NetProp prop in Object.FindObjectsByType<NetProp>(FindObjectsSortMode.None))
			{
				if (prop.gameObject.scene == group.gameObject.scene)
				{
					planks.Add(prop);
				}
			}
			planks.Sort((NetProp x, NetProp y) => string.CompareOrdinal(x.name, y.name));
			for (int k = planks.Count - 1; k >= 0; k--)
			{
				if (!Span(balloons, planks[k].transform.position, out _, out _))
				{
					planks.RemoveAt(k);
				}
			}
			foreach (NetProp plank in planks)
			{
				Span(balloons, plank.transform.position, out int a, out int b);
				Quaternion frame = Frame(balloons[a].basket.position, balloons[b].basket.position, out Vector3 mid);
				pairs.Add(new int[2] { a, b });
				offsets.Add(Quaternion.Inverse(frame) * (plank.transform.position - mid));
				turns.Add(Quaternion.Inverse(frame) * plank.transform.rotation);
			}
			List<Transform> roots = new List<Transform>();
			List<int> rootOn = new List<int>();
			foreach (Player player in Object.FindObjectsByType<Player>(FindObjectsSortMode.None))
			{
				Transform root = player.transform.root;
				if (player.gameObject.scene != group.gameObject.scene || roots.Contains(root))
				{
					continue;
				}
				Span(balloons, root.position, out int on, out _);
				roots.Add(root);
				rootOn.Add(on);
			}
			Vector3[] moved = new Vector3[balloons.Length];
			for (int l = 0; l < balloons.Length; l++)
			{
				Transform t2 = balloons[l].transform;
				Vector2 spot = layouts[layout][l];
				Vector3 to = new Vector3(spot.x, t2.position.y, spot.y);
				moved[l] = to - t2.position;
				t2.position = to;
			}
			for (int m = 0; m < roots.Count; m++)
			{
				roots[m].position += moved[rootOn[m]];
			}
			Physics.SyncTransforms();
			for (int n = (layout == 0) ? 4 : 0; n < chain.Length; n++)
			{
				Tie(chain[n], balloons[chainLinks[n, 0]].basket, balloons[chainLinks[n, 1]].basket, chainSide[n]);
			}
			for (int num = 0; num < planks.Count; num++)
			{
				int[] pair = (layout == 0 || num >= 3) ? pairs[num] : new int[2]
				{
					chainLinks[num, 0],
					chainLinks[num, 1]
				};
				Quaternion frame2 = Frame(balloons[pair[0]].basket.position, balloons[pair[1]].basket.position, out Vector3 mid2);
				Transform t3 = planks[num].transform;
				t3.SetPositionAndRotation(mid2 + frame2 * offsets[num], frame2 * turns[num]);
				Rigidbody rb = t3.GetComponent<Rigidbody>();
				if (rb)
				{
					rb.position = t3.position;
					rb.rotation = t3.rotation;
				}
			}
			Physics.SyncTransforms();
			Player.Note(count + " balloons in formation " + layout + " for round " + round + ", " + planks.Count + " planks moved");
		}

		private static bool Span(Floater[] balloons, Vector3 at, out int a, out int b)
		{
			a = -1;
			b = -1;
			float best = float.MaxValue;
			float second = float.MaxValue;
			for (int i = 0; i < balloons.Length; i++)
			{
				Vector3 p = balloons[i].basket.position;
				float dx = p.x - at.x;
				float dz = p.z - at.z;
				float d = Mathf.Sqrt(dx * dx + dz * dz);
				if (d < best)
				{
					b = a;
					second = best;
					best = d;
					a = i;
				}
				else if (d < second)
				{
					second = d;
					b = i;
				}
			}
			return b >= 0 && second < 6.5f;
		}

		private static Quaternion Frame(Vector3 from, Vector3 to, out Vector3 mid)
		{
			mid = (from + to) * 0.5f;
			return Quaternion.LookRotation(to - from, Vector3.up);
		}

		private static void Tie(Rope rope, Rigidbody from, Rigidbody to, float side)
		{
			Rigidbody old = rope.end.GetComponentInParent<Rigidbody>();
			if (old)
			{
				foreach (ConfigurableJoint joint in old.GetComponents<ConfigurableJoint>())
				{
					if (joint.connectedBody && joint.connectedBody.transform.IsChildOf(rope.transform))
					{
						Object.DestroyImmediate(joint);
					}
				}
			}
			bool kept = false;
			float mass = 0f;
			float depenetration = 0f;
			float linear = 0f;
			float angular = 0f;
			int iterations = 0;
			int velocityIterations = 0;
			CollisionDetectionMode detection = CollisionDetectionMode.Discrete;
			for (int i = rope.transform.childCount - 1; i >= 0; i--)
			{
				GameObject link = rope.transform.GetChild(i).gameObject;
				Rigidbody rb = link.GetComponent<Rigidbody>();
				if (!rb)
				{
					Object.DestroyImmediate(link);
					continue;
				}
				if (!kept)
				{
					mass = rb.mass;
					depenetration = rb.maxDepenetrationVelocity;
					linear = rb.maxLinearVelocity;
					angular = rb.maxAngularVelocity;
					iterations = rb.solverIterations;
					velocityIterations = rb.solverVelocityIterations;
					detection = rb.collisionDetectionMode;
				}
				Object.DestroyImmediate(rb.gameObject);
				kept = true;
			}
			rope.transform.position = from.transform.TransformPoint(Edge(from.transform, to.position, side));
			rope.start = from;
			rope.end.SetParent(to.transform, false);
			rope.end.localPosition = Edge(to.transform, from.position, side);
			rope.Build();
			if (!kept)
			{
				return;
			}
			foreach (Rigidbody link2 in rope.Links())
			{
				link2.mass = mass;
				link2.solverIterations = iterations;
				link2.solverVelocityIterations = velocityIterations;
				link2.maxDepenetrationVelocity = depenetration;
				link2.maxLinearVelocity = linear;
				link2.maxAngularVelocity = angular;
				link2.collisionDetectionMode = detection;
			}
		}

		private static Vector3 Edge(Transform basket, Vector3 toward, float side)
		{
			Vector3 local = basket.InverseTransformPoint(toward);
			if (Mathf.Abs(local.x) >= Mathf.Abs(local.y))
			{
				return new Vector3(local.x < 0f ? -0.975f : 0.975f, side, 0.1f);
			}
			return new Vector3(side, local.y < 0f ? -0.975f : 0.975f, 0.1f);
		}

		private void Start()
		{
			body = GetComponent<Rigidbody>();
			home = (flight && envelope) ? envelope.position : transform.position;
			rest = transform.rotation;
			phase = Mathf.Repeat(home.x * 1.37f + home.z * 2.11f, 20f);
			if (!flight)
			{
				Water water = Water.Get();
				if (water)
				{
					home.y = water.Surface + ride;
				}
				return;
			}
			net = envelope ? envelope.GetComponent<NetRope>() : null;
			if (envelope)
			{
				envelope.constraints = RigidbodyConstraints.None;
				envRest = envelope.rotation;
			}
			foreach (Rope rope in ropes)
			{
				if (!rope)
				{
					continue;
				}
				foreach (Rigidbody link in rope.Links())
				{
					ropeMass += link.mass;
				}
			}
		}

		private void FixedUpdate()
		{
			if (flight)
			{
				Fly();
				return;
			}
			Water water = Water.main;
			float clock = NetworkManager.Clock;
			float drift = water ? (water.SurfaceAt(home) - water.Surface) : 0f;
			float a = phase + clock * 6.2832f / Mathf.Max(period, 0.1f);
			Vector3 pos = home + Vector3.up * (drift + Mathf.Sin(a) * swell);
			Quaternion rot = rest * Quaternion.Euler(Mathf.Sin(a * 0.7f) * roll, 0f, Mathf.Sin(a * 1.3f + 1f) * roll * 0.6f);
			if (body)
			{
				body.MovePosition(pos);
				body.MoveRotation(rot);
			}
			else
			{
				transform.SetPositionAndRotation(pos, rot);
			}
		}

		private float Hanging()
		{
			float total = 0f;
			foreach (Rope rope in ropes)
			{
				if (!rope || rope.snapped < 0)
				{
					continue;
				}
				Rigidbody[] links = rope.Links();
				bool top = rope.start == envelope;
				bool bottom = !top && rope.end && rope.end.GetComponentInParent<Rigidbody>() == envelope;
				for (int i = 0; i < links.Length; i++)
				{
					if ((top && i < rope.snapped) || (bottom && i >= rope.snapped))
					{
						total += links[i].mass;
					}
				}
			}
			return total;
		}

		private void Fly()
		{
			if (!envelope)
			{
				return;
			}
			if (net && net.Object && net.Object.IsValid && !net.HasStateAuthority)
			{
				return;
			}
			float dt = Time.fixedDeltaTime;
			float t = (testTime >= 0f) ? testTime : GameManager.RoundTime;
			int cut = 0;
			foreach (Rope rope in ropes)
			{
				if (rope && rope.snapped >= 0)
				{
					cut++;
				}
			}
			if (cut != seen)
			{
				seen = cut;
				hanging = Hanging();
				Player.Note(name + " has " + (ropes.Length - cut) + " of " + ropes.Length + " cables left");
			}
			bool held = ropes.Length == 0 || cut < ropes.Length;
			float floor = Water.main ? Water.main.Surface : (home.y - 30f);
			float mass;
			float extra;
			if (!held)
			{
				mass = envelope.mass;
				extra = ropeMass * 0.5f;
				float rise = Mathf.Clamp((home.y + FreeCeiling - envelope.position.y) * FreeSettle, 0f - FreeRise, FreeRise);
				envelope.AddForce(Vector3.up * ((envelope.mass + hanging) * (Physics.gravity.magnitude + Mathf.Clamp((rise - envelope.linearVelocity.y) * FreeGrip, -6f, 6f))));
			}
			else
			{
				load = Mathf.MoveTowards(load, Riders(), dt * 0.6f);
				float target = home.y - Mathf.Max(t, 0f) * (descent * (Mathf.Repeat(phase * 0.37f, 1f) + 0.5f)) - load * sink - (home.y - floor) * cut / (ropes.Length + 1f);
				target = Mathf.Max(target, floor + 6f);
				mass = envelope.mass;
				extra = ropeMass + (basket ? basket.mass : 0f);
				float accel = stiffness * (target - envelope.position.y) - 2f * Mathf.Sqrt(stiffness) * envelope.linearVelocity.y;
				envelope.AddForce(Vector3.up * ((mass + extra) * (Mathf.Clamp(accel, -3f, 3f) + Physics.gravity.magnitude)));
			}
			float clock = NetworkManager.Clock;
			Vector3 push = new Vector3(Mathf.Sin(clock * 0.13f + phase), 0f, Mathf.Cos(clock * 0.09f + phase * 1.7f)) * 0.3f;
			Vector3 off = envelope.position - home;
			off.y = 0f;
			Vector3 vel = envelope.linearVelocity;
			vel.y = 0f;
			float k = 0.3f / Mathf.Max(wander, 0.1f);
			envelope.AddForce((push - off * k - vel * (Mathf.Sqrt(k) * 1.4f)) * (mass + extra));
			(envelope.rotation * Quaternion.Inverse(envRest)).ToAngleAxis(out float angle, out Vector3 axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			Vector3 tilt = float.IsNaN(axis.x) ? Vector3.zero : (axis * (angle * Mathf.Deg2Rad));
			Vector3 level = tilt - Vector3.Project(tilt, Vector3.up);
			envelope.AddTorque(-level * upright - envelope.angularVelocity * (2f * Mathf.Sqrt(upright)), ForceMode.Acceleration);
		}

		private int Riders()
		{
			riders.Clear();
			if (!basket)
			{
				return 0;
			}
			int hits = Physics.OverlapBoxNonAlloc(basket.worldCenterOfMass + Vector3.up * 1.1f, new Vector3(2f, 1f, 2f), near, Quaternion.Euler(0f, basket.rotation.eulerAngles.y, 0f), -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < hits; i++)
			{
				Collider c = near[i];
				if (!c || c.attachedRigidbody == basket)
				{
					continue;
				}
				Player player = c.GetComponentInParent<Player>();
				if (player && !player.dummy)
				{
					riders.Add(player);
					continue;
				}
				AvatarBody avatar = c.GetComponentInParent<AvatarBody>();
				if (avatar)
				{
					riders.Add(avatar);
				}
			}
			return riders.Count;
		}
	}
}
