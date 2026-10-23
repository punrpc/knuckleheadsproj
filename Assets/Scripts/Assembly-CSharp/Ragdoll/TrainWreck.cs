using System.Collections.Generic;
using UnityEngine;

namespace Ragdoll
{
	[DefaultExecutionOrder(50)]
	public class TrainWreck : MonoBehaviour
	{
		private class Tile
		{
			public Transform root;

			public Transform[] props;

			public Vector3[] propPos;

			public Quaternion[] propRot;

			public Vector3[] propScale;

			public int lap = int.MinValue;

			public Renderer ground;

			public GameObject holder;

			public GameObject tunnel;

			public bool inTunnel;

			public Rigidbody body;
		}

		[Header("Desert")]
		public GameObject piece;

		public int pieces = 6;

		public float speed = 30f;

		public float floorY;

		public float trackClear = 5f;

		[Header("Train")]
		public Transform[] cars;

		public Transform[] couplers;

		public float carDrop = 1.5f;

		public float leftBehind = 30f;

		public float wobble = 1f;

		public float occlusion = 0.6f;

		[Header("Off The Train")]
		public float carMargin = 0.3f;

		public float propBack = 40f;

		[Header("Tunnel")]
		public GameObject tunnel;

		public int tunnelEvery = 20;

		public int tunnelLength = 8;

		[Header("Sound")]
		public AudioSource rumble;

		public AudioSource horn;

		[Header("Speed")]
		public ParticleSystem[] dust;

		public static TrainWreck main;

		public const int SolidLayer = 11;

		private readonly List<Tile> tiles = new List<Tile>();

		private readonly List<Collider> carCols = new List<Collider>();

		private readonly List<int> carOf = new List<int>();

		private readonly List<NetProp> props = new List<NetProp>();

		private readonly List<Vector3> propHome = new List<Vector3>();

		private readonly List<Quaternion> propHomeRot = new List<Quaternion>();

		private Rigidbody[] carBodies;

		private Vector3[] carHome;

		private Quaternion[] carHomeRot;

		private float[] looseAt;

		private Vector3[] carStep;

		private Vector3 ahead = Vector3.right;

		private Vector3 tileHome;

		private float length;

		private float shownTime = float.NaN;

		private float rate = 1f;

		private float propLow;

		private float propHigh;

		private Rigidbody floorBody;

		private Vector3 floorHome;

		private float roofTop;

		private float groundAlong;

		private GameObject tunnelBuilt;

		private Vector3 tunnelOffset;

		private float trainFront;

		private float trainBack;

		private int hornedFor = int.MinValue;

		private float rumbleLevel;

		private float hornLevel;

		private float rolling;

		private float tunnelK;

		private float heldTime;

		private bool wasLive;

		private bool liveSnap;

		private float lastTime;

		private const float HornLead = 3f;

		private const int TunnelLead = 8;

		private const float Bob = 0.012f;

		private const float Sway = 0.4f;

		private const float Jolt = 0.06f;

		private const float BumpGap = 110f;

		private float trainLeft;

		private float trainRight;

		private bool dragged;

		private float draggedAt;

		private bool underPass = true;

		private Player passMe;

		private Collider pelvisCol;

		private Collider chestCol;

		private readonly List<Collider> passCols = new List<Collider>();

		private bool[,] passing;

		private List<Collider>[] carUnder;

		private const float PlateWide = 3.5f;

		private const float PlateThick = 0.25f;

		private const float Stand = 4f;

		private const float Touch = 0.05f;

		private const float DragFast = 20f;

		private const float DeadFast = 8f;

		private const float Grit = 0.9f;

		private const float PassReach = 0.6f;

		private const float DieBehind = 12f;

		private const float GripMargin = 1.5f;

		private float[] carBottom;

		private float[] carLo;

		private float[] carHi;

		private readonly HashSet<Rigidbody> desert = new HashSet<Rigidbody>();

		private Rigidbody tunnelBody;

		private int tunnelAt = int.MinValue;

		private PhysicsMaterial sand;

		private bool frozen;

		private bool snapped;

		private bool caught;

		private bool lifted;

		private bool roofed;

		private bool smacked;

		private Rigidbody smackBone;

		private Vector3 smackAt;

		private string hitBy;

		private const int Solid = 2048;

		private const float RailTop = 0.217f;

		private const float Sink = 0.3f;

		private static readonly float[] RailEdges = new float[4] { 1.17f, 1.24f, 1.406f, 1.547f };

		private const float Tall = 2f;

		private const float Trunk = 0.4f;

		private const float ArchFrom = 2f;

		private const float ArchLow = 0.3f;

		private const float ArchSpan = 120f;

		private const float ArchStep = 8f;

		private const float Wall = 2f;

		private const float Skin = 0.1f;

		private const float Kiss = 0.01f;

		private const float Headroom = 1.6f;

		private void Awake()
		{
			main = this;
			VertexAO.sceneScale = occlusion;
		}

		private void OnValidate()
		{
			if (!Application.isPlaying)
			{
				VertexAO.Rescale();
			}
		}

		private void OnDestroy()
		{
			foreach (Rigidbody rb in desert)
			{
				VertexAO.skip.Remove(rb);
			}
			if (main == this)
			{
				main = null;
				VertexAO.sceneScale = 1f;
			}
		}

		private void Start()
		{
			if (cars != null && cars.Length >= 2 && cars[0] && cars[cars.Length - 1])
			{
				Vector3 run = cars[0].position - cars[cars.Length - 1].position;
				run.y = 0f;
				if (run.sqrMagnitude > 0.01f)
				{
					ahead = run.normalized;
				}
			}
			Cars();
			Desert();
			trainFront = float.MinValue;
			trainBack = float.MaxValue;
			trainLeft = float.MaxValue;
			trainRight = float.MinValue;
			Vector3 side = Vector3.Cross(Vector3.up, ahead);
			foreach (Collider c in carCols)
			{
				float a = Vector3.Dot(c.bounds.min - tileHome, ahead);
				float b = Vector3.Dot(c.bounds.max - tileHome, ahead);
				trainFront = Mathf.Max(trainFront, Mathf.Max(a, b));
				trainBack = Mathf.Min(trainBack, Mathf.Min(a, b));
				float l = Vector3.Dot(c.bounds.min, side);
				float r = Vector3.Dot(c.bounds.max, side);
				trainLeft = Mathf.Min(trainLeft, Mathf.Min(l, r));
				trainRight = Mathf.Max(trainRight, Mathf.Max(l, r));
			}
			Bottoms();
			Unders();
			if (rumble)
			{
				rumbleLevel = rumble.volume;
				rumble.volume = 0f;
				if (!rumble.isPlaying)
				{
					rumble.Play();
				}
			}
			if (horn)
			{
				hornLevel = horn.volume;
			}
			foreach (NetProp prop in Object.FindObjectsByType<NetProp>(FindObjectsSortMode.None))
			{
				if (!(prop.gameObject.scene != gameObject.scene))
				{
					props.Add(prop);
					propHome.Add(prop.transform.position);
					propHomeRot.Add(prop.transform.rotation);
				}
			}
		}

		private void Cars()
		{
			int count = (cars != null) ? cars.Length : 0;
			carBodies = new Rigidbody[count];
			carHome = new Vector3[count];
			carHomeRot = new Quaternion[count];
			looseAt = new float[count];
			carStep = new Vector3[count];
			for (int i = 0; i < count; i++)
			{
				looseAt[i] = -1f;
				if (!cars[i])
				{
					continue;
				}
				carHome[i] = cars[i].position;
				carHomeRot[i] = cars[i].rotation;
				Rigidbody rb = cars[i].GetComponent<Rigidbody>();
				if (!rb)
				{
					rb = cars[i].gameObject.AddComponent<Rigidbody>();
				}
				rb.isKinematic = true;
				rb.interpolation = RigidbodyInterpolation.Interpolate;
				carBodies[i] = rb;
				foreach (Collider c in cars[i].GetComponentsInChildren<Collider>())
				{
					if (!c.isTrigger)
					{
						carCols.Add(c);
						carOf.Add(i);
						roofTop = Mathf.Max(roofTop, c.bounds.max.y);
					}
				}
			}
		}

		private void Desert()
		{
			if (!piece)
			{
				return;
			}
			sand = new PhysicsMaterial("Sand");
			sand.staticFriction = Grit;
			sand.dynamicFriction = Grit;
			sand.frictionCombine = PhysicsMaterialCombine.Maximum;
			Renderer r = piece.GetComponent<Renderer>();
			Bounds b = r ? r.bounds : Bounds(piece);
			length = Mathf.Max(Mathf.Abs(Vector3.Dot(b.size, ahead)), 1f);
			tileHome = piece.transform.position;
			groundAlong = Vector3.Dot(b.center - tileHome, ahead);
			Solids(piece.transform.Find("Props"));
			Vector3 side = Vector3.Cross(Vector3.up, ahead);
			for (int i = 0; i < pieces; i++)
			{
				GameObject go = piece;
				if (i != 0)
				{
					go = Object.Instantiate(piece, piece.transform.parent);
					go.name = piece.name + " " + i;
				}
				Transform holder = go.transform.Find("Props");
				Tile tile = new Tile();
				tile.root = go.transform;
				tile.ground = go.GetComponent<Renderer>();
				tile.holder = holder ? holder.gameObject : null;
				List<Transform> list = new List<Transform>();
				foreach (Transform child in holder ? holder : go.transform)
				{
					Vector3 size = Bounds(child.gameObject).size;
					if (size.magnitude > 0f && Mathf.Max(size.x, size.z) < length * Trunk)
					{
						list.Add(child);
					}
				}
				tile.props = list.ToArray();
				tile.propPos = new Vector3[list.Count];
				tile.propRot = new Quaternion[list.Count];
				tile.propScale = new Vector3[list.Count];
				for (int j = 0; j < list.Count; j++)
				{
					tile.propPos[j] = list[j].localPosition;
					tile.propRot[j] = list[j].localRotation;
					tile.propScale[j] = list[j].localScale;
					if (i == 0)
					{
						float off = Mathf.Abs(Vector3.Dot(list[j].position - tileHome, side));
						if (j == 0)
						{
							propLow = off;
							propHigh = off;
						}
						else
						{
							propLow = Mathf.Min(propLow, off);
							propHigh = Mathf.Max(propHigh, off);
						}
					}
				}
				tiles.Add(tile);
			}
			foreach (Tile t in tiles)
			{
				t.body = t.root.GetComponent<Rigidbody>();
				if (!t.body)
				{
					t.body = t.root.gameObject.AddComponent<Rigidbody>();
				}
				Kinematic(t.body);
				t.root.gameObject.AddComponent<Scenery>();
			}
			propLow = Mathf.Max(propLow, trackClear);
			propHigh = Mathf.Max(propHigh, propLow + 1f);
			GameObject floor = new GameObject("Desert Floor");
			floor.transform.SetParent(transform, false);
			BoxCollider box = floor.AddComponent<BoxCollider>();
			box.sharedMaterial = sand;
			float ring = length * pieces;
			floor.transform.position = new Vector3(tileHome.x, floorY + -0.5f, tileHome.z);
			floor.transform.rotation = Quaternion.LookRotation(Vector3.Cross(ahead, Vector3.up), Vector3.up);
			box.size = new Vector3(ring * 2f, 1f, 400f);
			floorBody = floor.AddComponent<Rigidbody>();
			floorBody.isKinematic = true;
			floorBody.useGravity = false;
			floorHome = floor.transform.position;
			Rails(floor.transform, ring);
			desert.Add(floorBody);
			VertexAO.skip.Add(floorBody);
			foreach (ParticleSystem d in dust)
			{
				if (d)
				{
					ParticleSystem.VelocityOverLifetimeModule velocity = d.velocityOverLifetime;
					velocity.enabled = true;
					velocity.space = ParticleSystemSimulationSpace.World;
					velocity.x = new ParticleSystem.MinMaxCurve(0f - ahead.x * speed);
					velocity.y = new ParticleSystem.MinMaxCurve(0f);
					velocity.z = new ParticleSystem.MinMaxCurve(0f - ahead.z * speed);
				}
			}
		}

		private void Kinematic(Rigidbody rb)
		{
			rb.isKinematic = true;
			rb.useGravity = false;
			rb.interpolation = RigidbodyInterpolation.None;
			rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			desert.Add(rb);
			VertexAO.skip.Add(rb);
		}

		private void Rails(Transform floor, float ring)
		{
			Vector3 side = Vector3.Cross(Vector3.up, ahead);
			Transform track = piece.transform.Find("Track");
			Vector3 centre = track ? Bounds(track.gameObject).center : tileHome;
			float across = Vector3.Dot(centre, side);
			for (int i = 0; i + 1 < RailEdges.Length; i += 2)
			{
				for (int s = -1; s <= 1; s += 2)
				{
					GameObject rail = new GameObject("Rail");
					rail.layer = SolidLayer;
					rail.transform.SetParent(floor, false);
					Vector3 at = floor.position;
					float inner = RailEdges[i];
					float outer = RailEdges[i + 1];
					Quaternion turn = Quaternion.LookRotation(ahead, Vector3.up);
					rail.transform.SetPositionAndRotation(ahead * Vector3.Dot(at, ahead) + side * (across + (inner + outer) * s * 0.5f) + Vector3.up * (floorY + (RailTop - (RailTop + Sink) * 0.5f)), turn);
					BoxCollider box = rail.AddComponent<BoxCollider>();
					box.size = new Vector3(outer - inner, RailTop + Sink, ring * 2f);
					box.sharedMaterial = sand;
				}
			}
		}

		private void Solids(Transform holder)
		{
			if (!holder)
			{
				return;
			}
			int capsules = 0;
			int hulls = 0;
			foreach (Transform t in holder)
			{
				MeshFilter filter = t.GetComponent<MeshFilter>();
				if (!filter || !filter.sharedMesh)
				{
					continue;
				}
				Mesh mesh = filter.sharedMesh;
				Bounds b = mesh.bounds;
				Vector3 size = b.size;
				Vector3 up = t.InverseTransformDirection(Vector3.up);
				int axis = (Mathf.Abs(up.x) >= Mathf.Max(Mathf.Abs(up.y), Mathf.Abs(up.z))) ? 0 : ((Mathf.Abs(up.y) >= Mathf.Abs(up.z)) ? 1 : 2);
				float thin = Mathf.Min(size[(axis + 1) % 3], size[(axis + 2) % 3]);
				GameObject solid = new GameObject("Solid");
				solid.layer = SolidLayer;
				solid.transform.SetParent(t, false);
				if (size[axis] > thin + thin || !mesh.isReadable)
				{
					CapsuleCollider capsule = solid.AddComponent<CapsuleCollider>();
					capsule.direction = axis;
					capsule.center = b.center;
					capsule.height = size[axis];
					capsule.radius = Mathf.Min(thin * Trunk, size[axis] * 0.5f);
					capsule.sharedMaterial = sand;
					capsules++;
				}
				else
				{
					MeshCollider hull = solid.AddComponent<MeshCollider>();
					hull.convex = true;
					hull.sharedMesh = mesh;
					hull.sharedMaterial = sand;
					hulls++;
				}
			}
			Player.Note("TrainWreck: desert props solid, " + hulls + " hulls and " + capsules + " capsules per tile");
		}

		private void Tunnels()
		{
			if (!tunnel || tunnelBuilt == tunnel)
			{
				return;
			}
			tunnelBuilt = tunnel;
			Vector3 offset = tunnel.transform.position - tileHome;
			tunnelOffset = offset - ahead * Vector3.Dot(offset, ahead);
			Stretch(tunnel);
			Walls(tunnel);
			foreach (Tile tile in tiles)
			{
				if (tile.tunnel)
				{
					Object.Destroy(tile.tunnel);
				}
				tile.tunnel = Object.Instantiate(tunnel, tunnel.transform.parent);
				tile.tunnel.name = tunnel.name + " " + tile.root.name;
				foreach (Collider c in tile.tunnel.GetComponentsInChildren<Collider>())
				{
					c.enabled = false;
				}
				tile.tunnel.transform.position = tile.root.position + tunnelOffset;
				tile.tunnel.SetActive(tile.inTunnel);
			}
			tunnel.SetActive(false);
		}

		private void Stretch(GameObject model)
		{
			float half = length * 0.5f;
			Vector3 at = model.transform.position;
			Vector3 side = Vector3.Cross(Vector3.up, ahead);
			float start = Vector3.Dot(at, ahead);
			float middle = (trainLeft + trainRight) * 0.5f;
			foreach (MeshFilter filter in model.GetComponentsInChildren<MeshFilter>())
			{
				Mesh mesh = filter.sharedMesh;
				if (!mesh || !mesh.isReadable)
				{
					continue;
				}
				Matrix4x4 toWorld = filter.transform.localToWorldMatrix;
				Matrix4x4 toLocal = filter.transform.worldToLocalMatrix;
				Vector3[] vertices = mesh.vertices;
				float lo = float.MaxValue;
				float hi = float.MinValue;
				for (int i = 0; i < vertices.Length; i++)
				{
					Vector3 w = toWorld.MultiplyPoint3x4(vertices[i]);
					if (w.y > roofTop + 0.5f && Mathf.Abs(Vector3.Dot(w, side) - middle) < 3f)
					{
						float d = Vector3.Dot(w, ahead) - start;
						lo = Mathf.Min(lo, d);
						hi = Mathf.Max(hi, d);
					}
				}
				float span = hi - lo;
				if (span < half || span > length + -0.02f)
				{
					continue;
				}
				for (int j = 0; j < vertices.Length; j++)
				{
					Vector3 w2 = toWorld.MultiplyPoint3x4(vertices[j]);
					float d2 = Vector3.Dot(w2, ahead) - start;
					float to = (d2 <= lo) ? (0f - half) : ((d2 >= hi) ? half : (half * ((d2 - lo) * 2f) / span - half));
					vertices[j] = toLocal.MultiplyPoint3x4(w2 + ahead * (to - d2));
				}
				Mesh stretched = Object.Instantiate(mesh);
				stretched.vertices = vertices;
				stretched.RecalculateBounds();
				filter.sharedMesh = stretched;
			}
		}

		private void Walls(GameObject model)
		{
			MeshFilter source = null;
			foreach (MeshFilter filter in model.GetComponentsInChildren<MeshFilter>())
			{
				if (filter.sharedMesh && filter.sharedMesh.isReadable)
				{
					source = filter;
					break;
				}
			}
			if (!source)
			{
				return;
			}
			GameObject probe = new GameObject("Arch Probe");
			probe.transform.SetPositionAndRotation(source.transform.position, source.transform.rotation);
			probe.transform.localScale = source.transform.lossyScale;
			MeshCollider shell = probe.AddComponent<MeshCollider>();
			shell.sharedMesh = source.sharedMesh;
			Vector3 side = Vector3.Cross(Vector3.up, ahead);
			Vector3 at = model.transform.position;
			Vector3 centre = at + side * ((trainLeft + trainRight) * 0.5f - Vector3.Dot(at, side));
			Vector3 origin = new Vector3(centre.x, floorY + Tall, centre.z);
			List<Vector3> rim = new List<Vector3>();
			for (float a = 0f - ArchSpan; a <= ArchSpan + 0.01f; a += ArchStep)
			{
				float r = a * Mathf.Deg2Rad;
				Vector3 dir = (Vector3.up * Mathf.Cos(r) + side * Mathf.Sin(r)).normalized;
				if (shell.Raycast(new Ray(origin, dir), out RaycastHit hit, 30f) && hit.point.y > floorY + ArchLow)
				{
					rim.Add(hit.point);
				}
			}
			shell.enabled = false;
			Object.Destroy(probe);
			if (rim.Count < 2)
			{
				return;
			}
			Vector3 first = rim[0];
			rim.Insert(0, new Vector3(first.x, floorY + -0.5f, first.z));
			Vector3 last = rim[rim.Count - 1];
			rim.Add(new Vector3(last.x, floorY + -0.5f, last.z));
			GameObject walls = new GameObject("Tunnel Walls");
			walls.transform.SetParent(transform, false);
			walls.transform.SetPositionAndRotation(model.transform.position, Quaternion.identity);
			float depth = length * Mathf.Min(tunnelLength, tunnelEvery);
			float arch = float.MinValue;
			for (int i = 1; i < rim.Count; i++)
			{
				Vector3 p0 = rim[i - 1];
				Vector3 p1 = rim[i];
				Vector3 d = p1 - p0;
				if (d.sqrMagnitude < 0.0001f)
				{
					continue;
				}
				Vector3 n = Vector3.Cross(ahead, d).normalized;
				Vector3 mid = (p0 + p1) * 0.5f;
				if (Vector3.Dot(mid - origin, n) < 0f)
				{
					n = -n;
				}
				GameObject wall = new GameObject("Tunnel");
				wall.layer = SolidLayer;
				wall.transform.SetParent(walls.transform, false);
				wall.transform.SetPositionAndRotation(mid + n * (Wall * 0.5f - Skin), Quaternion.LookRotation(ahead, n));
				BoxCollider box = wall.AddComponent<BoxCollider>();
				box.size = new Vector3(Mathf.Sqrt(d.sqrMagnitude) + Skin, Wall, depth);
				box.sharedMaterial = sand;
				arch = Mathf.Max(arch, Mathf.Max(p0.y, p1.y));
			}
			tunnelBody = walls.AddComponent<Rigidbody>();
			Kinematic(tunnelBody);
			tunnelBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			walls.AddComponent<Scenery>();
			Player.Note("TrainWreck: tunnel walls from " + rim.Count + " rim points, arch " + arch.ToString("F2") + " m, skin " + Skin.ToString("F2"));
		}

		private bool IsTunnel(int seg)
		{
			if (!tunnel || tunnelEvery <= tunnelLength)
			{
				return false;
			}
			int block = Mathf.FloorToInt((float)seg / (float)tunnelEvery);
			if (block < 0)
			{
				return false;
			}
			int gap = tunnelEvery - tunnelLength;
			uint h = (uint)(block + 1) * 2654435761u;
			int start = block * tunnelEvery + (int)(h % (uint)Mathf.Max(gap - TunnelLead, 1)) + Mathf.Min(gap - 1, TunnelLead);
			if (start <= seg)
			{
				return start + tunnelLength > seg;
			}
			return false;
		}

		private int NextTunnel(float run)
		{
			if (!tunnel || tunnelEvery <= tunnelLength)
			{
				return int.MinValue;
			}
			float behind = trainBack + -12f - length;
			int first = Mathf.FloorToInt((behind + run) / length);
			for (int s = first - tunnelLength; s <= first + tunnelEvery; s++)
			{
				if (IsTunnel(s) && !IsTunnel(s - 1) && length * (s + tunnelLength) - run + length * -0.5f > behind)
				{
					return s;
				}
			}
			return int.MinValue;
		}

		private static void Place(Rigidbody rb, Transform root, Vector3 at, bool jump)
		{
			if (!rb || jump || !rb.gameObject.activeInHierarchy)
			{
				root.position = at;
				if (rb)
				{
					rb.position = at;
				}
			}
			else
			{
				rb.MovePosition(at);
			}
		}

		private void Show(Tile t, int seg)
		{
			bool inside = IsTunnel(seg);
			t.inTunnel = inside;
			if (t.tunnel)
			{
				t.tunnel.SetActive(inside);
			}
			if (t.ground)
			{
				t.ground.enabled = !inside;
			}
			if (t.holder)
			{
				t.holder.SetActive(!inside);
			}
		}

		private static Bounds Bounds(GameObject g)
		{
			Renderer[] renderers = g.GetComponentsInChildren<Renderer>();
			if (renderers.Length == 0)
			{
				return new Bounds(g.transform.position, Vector3.zero);
			}
			Bounds b = renderers[0].bounds;
			foreach (Renderer r in renderers)
			{
				b.Encapsulate(r.bounds);
			}
			return b;
		}

		private float Now()
		{
			frozen = false;
			NetworkManager network = NetworkManager.main;
			if (!network || (!network.InRoom && GameManager.match == null && !network.Travelling))
			{
				return Time.timeSinceLevelLoad;
			}
			float since = GameManager.SinceStart;
			frozen = float.IsNaN(since);
			float t;
			if (frozen)
			{
				t = heldTime;
			}
			else
			{
				if (!wasLive)
				{
					liveSnap = true;
				}
				heldTime = since;
				t = since;
			}
			wasLive = !frozen;
			return t;
		}

		private void Horn(float t)
		{
			if (!horn || !tunnelBuilt || tiles.Count == 0)
			{
				return;
			}
			float travelled = Mathf.Max(t, 0f) * speed;
			int seg = Mathf.FloorToInt((travelled + trainFront - groundAlong + length * 0.5f) / length) + 1;
			for (int s = seg; s < seg + 4; s++)
			{
				if (IsTunnel(s) && !IsTunnel(s - 1))
				{
					if (!((groundAlong + (length * s - travelled) + length * -0.5f - trainFront) / speed > HornLead) && s != hornedFor)
					{
						hornedFor = s;
						horn.volume = hornLevel * Prefs.sfx;
						horn.Play();
					}
					break;
				}
			}
		}

		private bool TrainInTunnel()
		{
			foreach (Tile tile in tiles)
			{
				if (tile.inTunnel)
				{
					float centre = groundAlong + Vector3.Dot(tile.root.position - tileHome, ahead);
					if (centre - length * 0.5f < trainFront && centre + length * 0.5f > trainBack)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void Update()
		{
			if (rumble)
			{
				rumble.volume = rolling * (rumbleLevel * Prefs.sfx) * (tunnelK * 0.5f + 1f);
			}
		}

		private float Smoothed()
		{
			float dt = Time.fixedDeltaTime;
			float now = Now();
			if (float.IsNaN(shownTime) || liveSnap || (!frozen && Mathf.Abs(now - (dt + shownTime)) > 1f))
			{
				liveSnap = false;
				snapped = true;
				shownTime = now;
				rate = frozen ? 0f : 1f;
				return now;
			}
			float target;
			float step;
			if (frozen)
			{
				target = 0f;
				step = dt;
			}
			else
			{
				target = Mathf.Clamp(now - (dt + shownTime), -0.5f, 0.5f) + 1f;
				step = dt * 0.5f;
			}
			rate = Mathf.MoveTowards(rate, target, step);
			shownTime += dt * rate;
			return shownTime;
		}

		private void FixedUpdate()
		{
			float t = Smoothed();
			float dt = Time.fixedDeltaTime;
			rolling = (!snapped || frozen) ? Mathf.MoveTowards(rolling, (t > lastTime + dt * 0.25f) ? 1f : 0f, dt / 1.5f) : 1f;
			lastTime = t;
			Scroll(t);
			snapped = false;
			Treadmill();
			Horn(t);
			tunnelK = Mathf.MoveTowards(tunnelK, TrainInTunnel() ? 1f : 0f, dt + dt);
			Couplers(t);
			Player me = Player.main;
			if (me && me.pelvis && me.bodies != null)
			{
				Body(me);
			}
			Npcs();
			Props();
			caught = false;
			smacked = false;
		}

		private void Scroll(float t)
		{
			if (tiles.Count == 0)
			{
				return;
			}
			Tunnels();
			float ring = length * tiles.Count;
			float half = ring * 0.5f;
			float run = speed * t;
			for (int i = 0; i < tiles.Count; i++)
			{
				Tile tile = tiles[i];
				float along = Mathf.Repeat(half + (length * i - run), ring);
				int lap = Mathf.FloorToInt((half + (run - length * i)) / ring);
				bool jump = snapped || lap != tile.lap;
				Vector3 at = tileHome + ahead * (along - half);
				Place(tile.body, tile.root, at, jump);
				if (tile.tunnel)
				{
					tile.tunnel.transform.position = at + tunnelOffset;
				}
				if (lap != tile.lap)
				{
					tile.lap = lap;
					Shuffle(tile, i, lap);
					Show(tile, i + tiles.Count * lap);
				}
			}
			if (!tunnelBody)
			{
				return;
			}
			int seg = NextTunnel(run);
			bool on = seg != int.MinValue;
			if (on != tunnelBody.gameObject.activeSelf)
			{
				tunnelBody.gameObject.SetActive(on);
			}
			if (on)
			{
				float along2 = length * ((tunnelLength - 1) / 2f + seg) - run;
				bool jump2 = snapped || seg != tunnelAt;
				Place(tunnelBody, tunnelBody.transform, tileHome + tunnelOffset + ahead * along2, jump2);
				tunnelAt = seg;
			}
		}

		private void Shuffle(Tile tile, int index, int lap)
		{
			if (lap == 0)
			{
				for (int i = 0; i < tile.props.Length; i++)
				{
					tile.props[i].localPosition = tile.propPos[i];
					tile.props[i].localRotation = tile.propRot[i];
					tile.props[i].localScale = tile.propScale[i];
				}
				return;
			}
			Random.State state = Random.state;
			Random.InitState(index * 7919 + lap * 104729);
			Vector3 side = Vector3.Cross(Vector3.up, ahead);
			for (int j = 0; j < tile.props.Length; j++)
			{
				Transform prop = tile.props[j];
				float along = Random.value;
				float off = Random.Range(propLow, propHigh);
				float across = (Random.value < 0.5f) ? (0f - off) : off;
				Vector3 root = tile.root.position;
				Transform parent = prop.parent ? prop.parent : tile.root;
				Vector3 home = parent.TransformPoint(tile.propPos[j]);
				prop.position = new Vector3(root.x + (along + -0.5f) * length * ahead.x + side.x * across, home.y, root.z + (along + -0.5f) * length * ahead.z + side.z * across);
				prop.rotation = Quaternion.AngleAxis(Random.value * 360f, Vector3.up) * (parent.rotation * tile.propRot[j]);
				prop.localScale = tile.propScale[j] * Random.Range(0.85f, 1.15f);
			}
			Random.state = state;
		}

		private void Couplers(float t)
		{
			if (carBodies == null)
			{
				return;
			}
			if (couplers != null)
			{
				float now = Mathf.Max(t, 0f);
				for (int i = 0; i < couplers.Length && i + 1 < carBodies.Length; i++)
				{
					if (!couplers[i] || looseAt[i + 1] >= 0f || !Broken(couplers[i]))
					{
						continue;
					}
					for (int j = i + 1; j < carBodies.Length; j++)
					{
						if (looseAt[j] < 0f)
						{
							looseAt[j] = now;
						}
					}
					Player.Note("TrainWreck: coupler " + couplers[i].name + " gave way, " + (carBodies.Length - 1 - i) + " car(s) fall behind");
				}
			}
			for (int k = 0; k < carBodies.Length; k++)
			{
				if (!carBodies[k])
				{
					continue;
				}
				Vector3 home = carHome[k];
				if (looseAt[k] >= 0f)
				{
					float e = Mathf.Max(t - looseAt[k], 0f);
					float back = e * (e * (carDrop * 0.5f));
					if (back > length * tiles.Count)
					{
						carStep[k] = Vector3.zero;
						if (carBodies[k].gameObject.activeSelf)
						{
							carBodies[k].gameObject.SetActive(false);
						}
						continue;
					}
					home -= ahead * back;
					carStep[k] = ahead * Vector3.Dot(ahead, home - carBodies[k].position);
				}
				Rock(k, t, out float lift, out Quaternion tilt);
				carBodies[k].MovePosition(home + Vector3.up * lift);
				carBodies[k].MoveRotation(tilt * carHomeRot[k]);
			}
		}

		private void Rock(int k, float t, out float lift, out Quaternion tilt)
		{
			float along = Vector3.Dot(carHome[k], ahead) + speed * t;
			int bump = Mathf.FloorToInt(along / BumpGap);
			float amp = wobble * rolling;
			float jolt = 0f;
			float sway = 0f;
			if (bump != int.MinValue)
			{
				for (int i = bump - 1; i <= bump; i++)
				{
					float size = Hash(i, 1);
					if (size < 0.35f)
					{
						continue;
					}
					float e = (along - (i + 0.2f + Hash(i, 2) * 0.6f) * BumpGap) / Mathf.Max(speed, 0.01f);
					if (!(e < 0f) && !(e > 1.2f))
					{
						float b = Bounce(e);
						jolt += (Mathf.Clamp01(size) * 0.5f + 0.5f) * b;
						sway += (Hash(i, 3) * 2f - 1f) * b;
					}
				}
				jolt *= Jolt;
				sway *= 1.2f;
			}
			float w = t * 6.2831855f;
			lift = amp * ((Mathf.Sin(k * 1.9f + w * 1.6f) * 0.6f + Mathf.Sin(k * 0.8f + w * 2.7f) * 0.4f) * Bob + jolt);
			Quaternion roll = Quaternion.AngleAxis(amp * ((Mathf.Sin(k * 1.3f + w * 0.8f) * 0.7f + Mathf.Sin(k * 2.6f + w * 2.1f) * 0.3f) * Sway + sway), ahead);
			Quaternion pitch = Quaternion.AngleAxis(Mathf.Sin(w * 1.1f + k) * (amp * 0.15f), Vector3.Cross(Vector3.up, ahead));
			tilt = roll * pitch;
		}

		private static float Bounce(float e)
		{
			if (e < 0.08f)
			{
				float x = e / 0.08f;
				return x * x * (3f - 2f * x);
			}
			return Mathf.Exp((e + -0.08f) * -7f) * Mathf.Cos((e + -0.08f) * 16f);
		}

		private static float Hash(int i, int salt)
		{
			uint h = (uint)(i * 374761393 + salt * 668265263);
			h = (h ^ (h >> 13)) * 1274126177;
			h ^= h >> 16;
			return (float)(h & 0xFFFFFF) / 16777216f;
		}

		private static bool Broken(Transform coupler)
		{
			Rope[] ropes = coupler.GetComponentsInChildren<Rope>();
			if (ropes.Length == 0)
			{
				return false;
			}
			foreach (Rope rope in ropes)
			{
				if (rope.snapped < 0)
				{
					return false;
				}
			}
			return true;
		}

		private int CarAt(Vector3 p)
		{
			for (int i = 0; i < carCols.Count; i++)
			{
				Collider c = carCols[i];
				if (!c || !c.enabled || !c.gameObject.activeInHierarchy)
				{
					continue;
				}
				Bounds b = c.bounds;
				b.Expand(new Vector3(carMargin * 2f, 0f, carMargin * 2f));
				if (p.x >= b.min.x && p.x <= b.max.x && p.z >= b.min.z && p.z <= b.max.z && p.y >= b.min.y + -0.5f && p.y <= b.max.y + Stand)
				{
					return carOf[i];
				}
			}
			return -1;
		}

		private float Behind(int car)
		{
			if (car < 0 || looseAt[car] < 0f || !carBodies[car])
			{
				return 0f;
			}
			return Vector3.Dot(ahead, carHome[car] - carBodies[car].position);
		}

		private void Bottoms()
		{
			int count = (carBodies != null) ? carBodies.Length : 0;
			carBottom = new float[count];
			carLo = new float[count];
			carHi = new float[count];
			bool backfaces = Physics.queriesHitBackfaces;
			Physics.queriesHitBackfaces = true;
			List<float> hits = new List<float>();
			for (int k = 0; k < count; k++)
			{
				carBottom[k] = float.NaN;
				if (!carBodies[k])
				{
					continue;
				}
				Bounds box = default(Bounds);
				bool any = false;
				for (int i = 0; i < carCols.Count; i++)
				{
					if (carOf[i] != k || !carCols[i])
					{
						continue;
					}
					if (!any)
					{
						box = carCols[i].bounds;
						any = true;
					}
					else
					{
						box.Encapsulate(carCols[i].bounds);
					}
				}
				if (!any)
				{
					continue;
				}
				float a = Vector3.Dot(box.min, ahead) - Vector3.Dot(carBodies[k].position, ahead);
				float b = Vector3.Dot(box.max, ahead) - Vector3.Dot(carBodies[k].position, ahead);
				carLo[k] = Mathf.Min(a, b);
				carHi[k] = Mathf.Max(a, b);
				hits.Clear();
				Vector3 min = box.min;
				Vector3 size = box.size;
				for (int x = 1; x < 8; x++)
				{
					float px = min.x + size.x * Mathf.Clamp01(x / 8f);
					for (int z = 1; z < 4; z++)
					{
						float pz = min.z + size.z * Mathf.Clamp01(z / 4f);
						Ray ray = new Ray(new Vector3(px, floorY + 0.02f, pz), Vector3.up);
						float best = float.MaxValue;
						for (int j = 0; j < carCols.Count; j++)
						{
							if (carOf[j] == k && carCols[j] && carCols[j].Raycast(ray, out RaycastHit hit, roofTop + 2f))
							{
								best = Mathf.Min(best, hit.point.y);
							}
						}
						if (best < float.MaxValue)
						{
							hits.Add(best);
						}
					}
				}
				if (hits.Count != 0)
				{
					hits.Sort();
					float under = hits[hits.Count / 2];
					if (under > floorY + 0.15f && under < roofTop + -1.5f)
					{
						carBottom[k] = under - carBodies[k].position.y;
					}
					Player.Note("TrainWreck: car " + cars[k].name + " underside at " + under.ToString("F2") + " m" + (float.IsNaN(carBottom[k]) ? ", out of range, no under-train check" : ""));
				}
			}
			Physics.queriesHitBackfaces = backfaces;
		}

		private void Unders()
		{
			int count = (carBodies != null) ? carBodies.Length : 0;
			carUnder = new List<Collider>[count];
			for (int i = 0; i < count; i++)
			{
				carUnder[i] = new List<Collider>();
			}
			for (int j = 0; j < carCols.Count; j++)
			{
				int car = carOf[j];
				if (carCols[j] && carBodies[car] && !float.IsNaN(carBottom[car]) && carCols[j].bounds.min.y < carBodies[car].position.y + carBottom[car] + -0.01f)
				{
					carUnder[car].Add(carCols[j]);
				}
			}
			Rope[] ropes = GetComponentsInChildren<Rope>(true);
			foreach (Rope rope in ropes)
			{
				foreach (Rigidbody link in rope.GetComponentsInChildren<Rigidbody>(true))
				{
					link.maxLinearVelocity = rope.maxSpeed;
					link.maxAngularVelocity = rope.maxSpin;
				}
			}
			Vector3 side = Vector3.Cross(Vector3.up, ahead);
			for (int k = 0; k < count; k++)
			{
				if (!carBodies[k] || float.IsNaN(carBottom[k]) || carUnder[k].Count == 0)
				{
					continue;
				}
				Transform t = carBodies[k].transform;
				GameObject plate = new GameObject("Under Plate");
				plate.transform.SetParent(t, false);
				Vector3 scale = t.lossyScale;
				plate.transform.localScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
				float along = Vector3.Dot(t.position, ahead) + (carLo[k] + carHi[k]) * 0.5f;
				Vector3 at = t.position;
				plate.transform.SetPositionAndRotation(ahead * along + side * Vector3.Dot(at, side) + Vector3.up * (at.y + carBottom[k] + PlateThick * 0.5f), Quaternion.LookRotation(ahead, Vector3.up));
				BoxCollider box = plate.AddComponent<BoxCollider>();
				box.size = new Vector3(PlateWide, PlateThick, carHi[k] - carLo[k]);
				foreach (Rope rope2 in ropes)
				{
					foreach (Collider c in rope2.GetComponentsInChildren<Collider>(true))
					{
						Physics.IgnoreCollision(box, c, true);
					}
				}
			}
		}

		private bool UnderCar(Vector3 p)
		{
			for (int i = 0; i < carCols.Count; i++)
			{
				Collider c = carCols[i];
				int car = carOf[i];
				if (!c || !c.enabled || !c.gameObject.activeInHierarchy || !carBodies[car] || float.IsNaN(carBottom[car]))
				{
					continue;
				}
				Bounds b = c.bounds;
				if (!(p.x < b.min.x) && !(p.x > b.max.x) && !(p.z < b.min.z) && !(p.z > b.max.z) && p.y < carBodies[car].position.y + carBottom[car] + -0.05f)
				{
					return true;
				}
			}
			return false;
		}

		internal void Touched(Collision c, Rigidbody solid)
		{
			Player me = Player.main;
			Rigidbody rb = c.rigidbody;
			if (!me || !rb || !rb.transform.IsChildOf(me.transform) || Gap(c) > Kiss)
			{
				return;
			}
			caught = true;
			if (solid != tunnelBody || dragged || (rb != me.head && rb != me.chest && rb != me.pelvis))
			{
				return;
			}
			hitBy = "hit by the tunnel";
			if (!smacked)
			{
				smacked = true;
				smackBone = rb;
				smackAt = c.GetContact(0).point;
			}
		}

		private static float Gap(Collision c)
		{
			float gap = float.MaxValue;
			for (int i = 0; i < c.contactCount; i++)
			{
				gap = Mathf.Min(gap, c.GetContact(i).separation);
			}
			return gap;
		}

		private void Smack(Player me)
		{
			if (me.carriedHold && NetworkPlayer.mine)
			{
				NetworkPlayer.mine.KeepBody();
			}
			me.lastHit = "hit by the tunnel on " + smackBone.name;
			me.KnockOut(smackAt, false);
			Player.Note("hit by the tunnel on the " + smackBone.name + " at " + smackAt.y.ToString("F2") + " m, side " + Vector3.Dot(smackAt, Vector3.Cross(Vector3.up, ahead)).ToString("F2"));
		}

		private void Catch(Player me, bool onCar)
		{
			if (caught && !onCar && Moving(floorBody) && (dragged || me.flopped || me.dead || me.drowned))
			{
				lifted = true;
				foreach (Rigidbody b in me.bodies)
				{
					if (b)
					{
						b.maxLinearVelocity = speed + DeadFast;
					}
				}
			}
			else if (lifted)
			{
				lifted = false;
				if (!dragged)
				{
					me.Recap();
				}
			}
		}

		private void Body(Player me)
		{
			Vector3 p = me.pelvis.position;
			int car = CarAt(p);
			Cache(me);
			Drag(me, p, car);
			Pass(underPass && dragged);
			bool onCar = OnCar(p, car);
			Catch(me, onCar);
			roofed = onCar && me.flopped && Moving(floorBody) && Physics.Raycast(p, Vector3.up, Headroom, Solid, QueryTriggerInteraction.Ignore);
			if (!me.drowned && !me.dead)
			{
				if (car >= 0 && Behind(car) > leftBehind)
				{
					me.Kill("left behind on a loose car");
				}
				else if (smacked && !me.knockedOut && Moving(floorBody))
				{
					Smack(me);
				}
			}
			if (car < 0 || !onCar || looseAt[car] < 0f || !carBodies[car] || !carBodies[car].gameObject.activeSelf || !me.playSpace)
			{
				return;
			}
			me.playSpace.position += carStep[car];
			me.rode += carStep[car];
			float drop = carDrop * Time.fixedDeltaTime;
			foreach (Rigidbody b in me.bodies)
			{
				if (b && !b.isKinematic)
				{
					b.linearVelocity -= ahead * drop;
				}
			}
		}

		private void Cache(Player me)
		{
			if (passMe == me && passing != null)
			{
				return;
			}
			passMe = me;
			passCols.Clear();
			foreach (Collider c in me.GetComponentsInChildren<Collider>(true))
			{
				if (!c.isTrigger)
				{
					passCols.Add(c);
				}
			}
			pelvisCol = me.pelvis.GetComponent<Collider>();
			chestCol = me.chest.GetComponent<Collider>();
			passing = new bool[passCols.Count, carBodies.Length];
		}

		private void Pass(bool on)
		{
			Vector3 side = Vector3.Cross(Vector3.up, ahead);
			for (int k = 0; k < carBodies.Length; k++)
			{
				Rigidbody body = carBodies[k];
				if (!body || carUnder[k].Count == 0)
				{
					continue;
				}
				bool off = !on || !body.gameObject.activeInHierarchy;
				float along = Vector3.Dot(body.position, ahead);
				float bottom = body.position.y + carBottom[k];
				for (int i = 0; i < passCols.Count; i++)
				{
					Collider c = passCols[i];
					if (!c)
					{
						continue;
					}
					Bounds b = c.bounds;
					float d = Vector3.Dot(b.center, ahead) - along;
					float s = Vector3.Dot(b.center, side);
					bool near = d > carLo[k] + -PassReach && d < carHi[k] + PassReach && s > trainLeft + -PassReach && s < trainRight + PassReach;
					bool want = !off && c.enabled && c.gameObject.activeInHierarchy && near && (passing[i, k] || b.min.y < bottom);
					if (want == passing[i, k])
					{
						continue;
					}
					passing[i, k] = want;
					foreach (Collider u in carUnder[k])
					{
						if (u && u.enabled && u.gameObject.activeInHierarchy)
						{
							Physics.IgnoreCollision(c, u, want);
						}
					}
				}
			}
			for (int j = 0; j < passCols.Count; j++)
			{
				Collider c2 = passCols[j];
				if (!c2)
				{
					continue;
				}
				bool any = false;
				for (int l = 0; l < carBodies.Length; l++)
				{
					any |= passing[j, l];
				}
				LayerMask mask = any ? Solid : 0;
				if (c2.excludeLayers.value != mask.value)
				{
					c2.excludeLayers = mask;
				}
			}
		}

		private void Drag(Player me, Vector3 p, int car)
		{
			bool alive = !me.drowned && !me.dead;
			bool clings = Clings(me);
			bool onCar = OnCar(p, car);
			bool moving = Moving(floorBody);
			if (!dragged && moving && !clings && !onCar && (desert.Contains(me.Standing) || Touches(pelvisCol) || Touches(chestCol)))
			{
				dragged = true;
				draggedAt = Time.time;
				if (alive)
				{
					me.LetGo("swept off your feet");
					me.Knock(Vector3.zero);
				}
				Player.Note("swept off your feet " + (Vector3.Dot(p - tileHome, ahead) - trainBack).ToString("F1") + " m from the back, side " + Vector3.Dot(p, Vector3.Cross(Vector3.up, ahead)).ToString("F2") + ", height " + p.y.ToString("F2") + (UnderCar(p) ? ", under a car" : ""));
			}
			else if (dragged && (!moving || (alive && (clings || onCar))))
			{
				dragged = false;
				me.Recap();
				Player.Note("no longer dragged after " + (Time.time - draggedAt).ToString("F1") + " s" + (moving ? (clings ? ", holding on" : ", on a car") : ", ground stopped"));
			}
			if (dragged)
			{
				float cap = alive ? DragFast : (speed + DeadFast);
				foreach (Rigidbody b in me.bodies)
				{
					if (b)
					{
						b.maxLinearVelocity = cap;
					}
				}
			}
			if (alive && onCar && !me.knockedOut)
			{
				hitBy = null;
			}
			float back = trainBack - Vector3.Dot(p - tileHome, ahead);
			if (!clings && !onCar && alive && !(back < DieBehind))
			{
				me.Kill(hitBy ?? "left behind");
				Player.Note("left behind " + back.ToString("F1") + " m back" + (dragged ? (" after " + (Time.time - draggedAt).ToString("F1") + " s on the ground") : "") + ((hitBy != null) ? (", " + hitBy) : ""));
			}
		}

		private bool Touches(Collider c)
		{
			if (!c)
			{
				return false;
			}
			Bounds b = c.bounds;
			if (b.min.y < floorY + Touch)
			{
				return true;
			}
			return Physics.CheckBox(b.center, b.extents + Vector3.one * Touch, Quaternion.identity, Solid, QueryTriggerInteraction.Ignore);
		}

		private bool OnCar(Vector3 p, int car)
		{
			if (car < 0 || !carBodies[car])
			{
				return false;
			}
			if (float.IsNaN(carBottom[car]))
			{
				return true;
			}
			return p.y > carBodies[car].position.y + carBottom[car];
		}

		public static bool Desert(Rigidbody rb)
		{
			if (!main || !rb)
			{
				return false;
			}
			return main.desert.Contains(rb);
		}

		private void Npcs()
		{
			for (int i = SlapDummy.alive.Count - 1; i >= 0; i--)
			{
				SlapDummy dummy = (i < SlapDummy.alive.Count) ? SlapDummy.alive[i] : null;
				Player npc = dummy ? dummy.Body : null;
				if (!npc || !npc.pelvis || dummy.Gone)
				{
					continue;
				}
				Vector3 p = npc.pelvis.position;
				int car = CarAt(p);
				if (!OnCar(p, car))
				{
					if (trainBack - Vector3.Dot(p - tileHome, ahead) >= DieBehind)
					{
						npc.Kill("left behind");
					}
				}
				else if (Behind(car) > leftBehind)
				{
					npc.Kill("left behind on a loose car");
				}
			}
		}

		public static bool Moving(Rigidbody rb)
		{
			TrainWreck wreck = main;
			if (!wreck || !rb || !wreck.desert.Contains(rb))
			{
				return false;
			}
			return wreck.speed * wreck.rolling > 4f;
		}

		public static bool Dragged(Player me)
		{
			TrainWreck wreck = main;
			if (!wreck || (!wreck.dragged && !wreck.lifted && !wreck.roofed) || !me)
			{
				return false;
			}
			return me == Player.main;
		}

		private bool Clings(Player me)
		{
			Rigidbody[] hands = new Rigidbody[2] { me.leftHand, me.rightHand };
			foreach (Rigidbody hand in hands)
			{
				HandGrab grab = hand ? hand.GetComponent<HandGrab>() : null;
				if (!grab || !grab.grabbing || OnDesert(grab.held))
				{
					continue;
				}
				if (grab.held && CarOf(grab.held) >= 0)
				{
					return true;
				}
				if ((me.gripping || me.hanging) && CarAt(hand.position) >= 0 && !UnderCar(hand.position))
				{
					return true;
				}
				if (grab.holdsYou && NearTrain(grab.PinAt, GripMargin))
				{
					return true;
				}
			}
			return false;
		}

		private bool NearTrain(Vector3 p, float margin)
		{
			float along = Vector3.Dot(ahead, p - tileHome);
			if (!(along > trainBack - margin) || !(along < trainFront + margin))
			{
				return false;
			}
			float across = Vector3.Dot(p, Vector3.Cross(Vector3.up, ahead));
			if (!(across > trainLeft - margin) || !(across < trainRight + margin) || !(p.y > floorY + 0.2f))
			{
				return false;
			}
			return !UnderCar(p);
		}

		private bool OnDesert(Rigidbody rb)
		{
			if (!rb)
			{
				return false;
			}
			foreach (Rigidbody d in desert)
			{
				if (d && rb.transform.IsChildOf(d.transform))
				{
					return true;
				}
			}
			return false;
		}

		private int CarOf(Rigidbody rb)
		{
			for (int i = 0; i < carBodies.Length; i++)
			{
				if (carBodies[i] && (rb == carBodies[i] || rb.transform.IsChildOf(carBodies[i].transform)))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool Ground(Rigidbody rb, out Vector3 velocity)
		{
			TrainWreck wreck = main;
			velocity = Vector3.zero;
			if (!wreck || !rb || !wreck.desert.Contains(rb))
			{
				return false;
			}
			velocity = -(wreck.ahead * wreck.speed) * wreck.rolling;
			return true;
		}

		private void Treadmill()
		{
			if (floorBody)
			{
				floorBody.position = floorHome + ahead * (speed * rolling * Time.fixedDeltaTime);
				floorBody.MovePosition(floorHome);
			}
		}

		private void Props()
		{
			for (int i = 0; i < props.Count; i++)
			{
				NetProp prop = props[i];
				if (!prop || !prop.Object || !prop.Object.IsValid || !prop.HasStateAuthority)
				{
					continue;
				}
				Rigidbody rb = prop.GetComponent<Rigidbody>();
				if (rb && !rb.isKinematic && !(Vector3.Dot(ahead, propHome[i] - rb.position) <= propBack))
				{
					rb.position = propHome[i];
					rb.rotation = propHomeRot[i];
					rb.linearVelocity = Vector3.zero;
					rb.angularVelocity = Vector3.zero;
				}
			}
		}
	}
}
