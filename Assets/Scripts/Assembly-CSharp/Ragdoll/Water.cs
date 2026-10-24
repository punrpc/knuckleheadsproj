using System.Collections.Generic;
using UnityEngine;

namespace Ragdoll
{
	[ExecuteAlways]
	public class Water : MonoBehaviour
	{
		private class Shape
		{
			public Transform at;

			public Collider col;

			public Vector3[] local;

			public float radius;

			public bool world;
		}

		public static Water main;

		public static readonly List<Water> acids = new List<Water>();

		[Header("Float")]
		public float buoyancy = 1.8f;

		public float drag = 2.5f;

		public float spinDrag = 1f;

		public float minBand = 0.08f;

		public float rescan = 0.5f;

		[Header("Acid")]
		public bool acid;

		[Header("Waves")]
		public float wave = 0.04f;

		public float waveLength = 3f;

		public float waveSpeed = 0.7f;

		[Header("Fog")]
		public Color fogColor = new Color(0f, 0f, 0f, 0f);

		public float fogTop = 8f;

		public float fogBottom = 2f;

		public float fogFar;

		private static readonly int FogId = Shader.PropertyToID("_HeightFog");

		private static readonly int FogColorId = Shader.PropertyToID("_HeightFogColor");

		private Rigidbody[] all = new Rigidbody[0];

		private AvatarBody[] copies = new AvatarBody[0];

		private readonly Dictionary<Rigidbody, List<Shape>> shapes = new Dictionary<Rigidbody, List<Shape>>();

		private float nextScan;

		private Bounds area;

		private float fogFrom;

		public float Surface => transform.position.y;

		private static Vector3 Corner(Bounds b, int i)
		{
			Vector3 c = b.center;
			Vector3 e = b.extents;
			return new Vector3(c.x + ((i & 1) == 0 ? -e.x : e.x), c.y + ((i & 2) == 0 ? -e.y : e.y), c.z + ((i & 4) == 0 ? -e.z : e.z));
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Install()
		{
			Get();
		}

		public static Water Get()
		{
			if (!main)
			{
				GameObject go = GameObject.Find("Water");
				if (!go)
				{
					return null;
				}
				main = go.GetComponent<Water>();
				if (!main)
				{
					main = go.AddComponent<Water>();
				}
			}
			return main;
		}

		private void OnEnable()
		{
			fogFrom = Surface;
			Fog(true);
			if (!Application.isPlaying)
			{
				return;
			}
			if (!acid)
			{
				main = this;
				return;
			}
			Renderer r = GetComponent<Renderer>();
			area = r ? r.bounds : new Bounds(transform.position, Vector3.zero);
			if (!acids.Contains(this))
			{
				acids.Add(this);
			}
		}

		private void Update()
		{
			if (!Application.isPlaying)
			{
				Fog(true);
			}
		}

		private void OnValidate()
		{
			Fog(isActiveAndEnabled);
		}

		private void Fog(bool on)
		{
			if (fogColor.a <= 0f)
			{
				return;
			}
			float top = Application.isPlaying ? Mathf.Min(fogFrom, Surface) : Surface;
			Shader.SetGlobalVector(FogId, new Vector4(top + fogTop, Surface + fogBottom, fogFar, 0f));
			Shader.SetGlobalColor(FogColorId, on ? fogColor.linear : Color.clear);
		}

		private void OnDisable()
		{
			Fog(false);
			acids.Remove(this);
			if (main == this)
			{
				main = null;
			}
		}

		public bool Inside(Vector3 p)
		{
			if (!acid)
			{
				return true;
			}
			return p.x >= area.min.x && p.x <= area.max.x && p.z >= area.min.z && p.z <= area.max.z;
		}

		public static bool Over(Vector3 p)
		{
			foreach (Water w in acids)
			{
				if (w && w.Inside(p) && p.y > w.Surface - 10f)
				{
					return true;
				}
			}
			return false;
		}

		public bool Burns(Vector3 p)
		{
			return Inside(p) && Under(p);
		}

		public float SurfaceAt(Vector3 p)
		{
			if (wave <= 0f)
			{
				return Surface;
			}
			float k = 6.2832f / Mathf.Max(waveLength, 0.1f);
			float t = NetworkManager.Clock * waveSpeed;
			return Surface + wave * 0.5f * (Mathf.Sin(p.x * k + t * 2.1f) + Mathf.Sin(p.z * k * 0.77f + t * -1.7f));
		}

		public bool Under(Vector3 point)
		{
			return point.y < SurfaceAt(point);
		}

		private void FixedUpdate()
		{
			if (fogColor.a > 0f)
			{
				Fog(true);
			}
			if (Time.time >= nextScan)
			{
				nextScan = Time.time + rescan;
				all = Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
				copies = new AvatarBody[all.Length];
				for (int i = 0; i < all.Length; i++)
				{
					AvatarBody.AvatarHit hit = all[i].GetComponent<AvatarBody.AvatarHit>();
					if (hit)
					{
						copies[i] = hit.owner;
					}
				}
				shapes.Clear();
			}
			float surface = Surface;
			float g = -Physics.gravity.y;
			for (int j = 0; j < all.Length; j++)
			{
				Rigidbody rb = all[j];
				if (!rb || rb.isKinematic || !rb.gameObject.activeInHierarchy)
				{
					continue;
				}
				AvatarBody copy = copies[j];
				if ((object)copy != null && !copy.streamed)
				{
					continue;
				}
				if (rb.worldCenterOfMass.y - 3f > surface + wave)
				{
					continue;
				}
				if (Inside(rb.worldCenterOfMass))
				{
					Push(rb, g);
				}
			}
		}

		private void Push(Rigidbody rb, float g)
		{
			if (!shapes.TryGetValue(rb, out List<Shape> list) || list.Exists((Shape s) => !s.at))
			{
				list = Build(rb);
				shapes[rb] = list;
			}
			int count = 0;
			foreach (Shape shape in list)
			{
				count += shape.local.Length;
			}
			if (count == 0)
			{
				return;
			}
			bool sleeping = rb.IsSleeping();
			float lift = rb.mass * g * buoyancy / count;
			float damp = rb.mass * drag / count;
			float wet = 0f;
			foreach (Shape shape in list)
			{
				float band = Mathf.Max(shape.radius, minBand);
				for (int i = 0; i < shape.local.Length; i++)
				{
					Vector3 p = shape.world ? Corner(shape.col.bounds, i) : shape.at.TransformPoint(shape.local[i]);
					float depth = Mathf.Clamp01((band + (SurfaceAt(p) - p.y)) / (band + band));
					if (depth <= 0f)
					{
						continue;
					}
					wet += depth;
					if (!sleeping)
					{
						rb.AddForceAtPosition(Vector3.up * (lift * depth), p);
						Vector3 v = rb.GetPointVelocity(p);
						rb.AddForceAtPosition(-(v * (damp * depth)), p);
					}
				}
			}
			float frac = wet / count;
			if (frac <= 0f)
			{
				return;
			}
			if (sleeping)
			{
				if (frac * buoyancy > 1f)
				{
					rb.WakeUp();
				}
				return;
			}
			rb.AddTorque(-(rb.angularVelocity * (frac * spinDrag)), ForceMode.Acceleration);
		}

		private List<Shape> Build(Rigidbody rb)
		{
			List<Shape> list = new List<Shape>();
			foreach (Collider c in rb.GetComponentsInChildren<Collider>())
			{
				if (!c || !c.enabled || c.isTrigger || c.attachedRigidbody != rb)
				{
					continue;
				}
				Vector3 scale = c.transform.lossyScale;
				float big = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
				Shape shape = new Shape();
				shape.at = c.transform;
				BoxCollider box = c as BoxCollider;
				CapsuleCollider capsule = c as CapsuleCollider;
				SphereCollider sphere = c as SphereCollider;
				if (box)
				{
					Vector3 size = box.size;
					Vector3 h = size * 0.5f;
					shape.local = new Vector3[8];
					for (int i = 0; i < 8; i++)
					{
						shape.local[i] = box.center + new Vector3((i & 1) == 0 ? -h.x : h.x, (i & 2) == 0 ? -h.y : h.y, (i & 4) == 0 ? -h.z : h.z);
					}
					shape.radius = Mathf.Min(Mathf.Abs(scale.x) * size.x, Mathf.Abs(scale.y) * size.y, Mathf.Abs(scale.z) * size.z) * 0.5f;
				}
				else if (capsule)
				{
					Vector3 axis = capsule.direction == 0 ? Vector3.right : (capsule.direction == 1 ? Vector3.up : Vector3.forward);
					float half = Mathf.Max(capsule.height * 0.5f - capsule.radius, 0f);
					shape.local = new Vector3[5];
					for (int i = 0; i < 5; i++)
					{
						shape.local[i] = capsule.center + axis * (half * (i / 2f - 1f));
					}
					shape.radius = big * capsule.radius;
				}
				else if (sphere)
				{
					shape.local = new Vector3[1] { sphere.center };
					shape.radius = big * sphere.radius;
				}
				else
				{
					Bounds b = c.bounds;
					shape.world = true;
					shape.col = c;
					shape.local = new Vector3[8];
					shape.radius = Mathf.Min(b.size.x, b.size.y, b.size.z) * 0.5f;
				}
				list.Add(shape);
			}
			if (list.Count == 0)
			{
				list.Add(new Shape
				{
					at = rb.transform,
					local = new Vector3[1] { rb.centerOfMass },
					radius = 0.1f
				});
			}
			return list;
		}
	}
}
