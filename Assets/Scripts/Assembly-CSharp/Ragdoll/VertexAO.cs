using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Ragdoll
{
	public class VertexAO : ScriptableRendererFeature
	{
		private struct Shape
		{
			public float dist;

			public Vector4 a;

			public Vector4 b;

			public Vector4 c;
		}

		private class Nearer : IComparer<Shape>
		{
			public int Compare(Shape x, Shape y)
			{
				return x.dist.CompareTo(y.dist);
			}
		}

		public const int Max = 32;

		public static float sceneScale = 1f;

		public static readonly HashSet<Rigidbody> skip = new HashSet<Rigidbody>();

		[Header("Look")]
		[Range(0f, 1f)]
		public float strength = 0.8f;

		[Range(0f, 1f)]
		public float direct = 0.3f;

		[Range(1.5f, 6f)]
		public float range = 3f;

		public bool showAO;

		[Header("Occluders")]
		[Range(0f, 32f)]
		public int maxCapsules = 24;

		[Range(0f, 32f)]
		public int maxBoxes = 24;

		public float maxDistance = 15f;

		public float minSize = 0.08f;

		public float boxScale = 1.1f;

		public int rescanFrames = 30;

		private static readonly int CapAId = Shader.PropertyToID("_AOCapA");

		private static readonly int CapBId = Shader.PropertyToID("_AOCapB");

		private static readonly int Box0Id = Shader.PropertyToID("_AOBox0");

		private static readonly int Box1Id = Shader.PropertyToID("_AOBox1");

		private static readonly int Box2Id = Shader.PropertyToID("_AOBox2");

		private static readonly int CapCountId = Shader.PropertyToID("_AOCapCount");

		private static readonly int BoxCountId = Shader.PropertyToID("_AOBoxCount");

		private static readonly int StrengthId = Shader.PropertyToID("_AOStrength");

		private static readonly int DirectId = Shader.PropertyToID("_AODirect");

		private static readonly int DebugId = Shader.PropertyToID("_AODebug");

		private static readonly int RangeId = Shader.PropertyToID("_AORange");

		private static readonly int BoundsMinId = Shader.PropertyToID("_AOBoundsMin");

		private static readonly int BoundsMaxId = Shader.PropertyToID("_AOBoundsMax");

		private readonly Vector4[] capA = new Vector4[32];

		private readonly Vector4[] capB = new Vector4[32];

		private readonly Vector4[] box0 = new Vector4[32];

		private readonly Vector4[] box1 = new Vector4[32];

		private readonly Vector4[] box2 = new Vector4[32];

		private readonly List<Collider> candidates = new List<Collider>();

		private readonly List<Shape> caps = new List<Shape>();

		private readonly List<Shape> boxes = new List<Shape>();

		private readonly Nearer nearer = new Nearer();

		private int scannedAt = -9999;

		private int builtAt = -1;

		public static void Rescale()
		{
			if (!Application.isPlaying)
			{
				TrainWreck wreck = Object.FindFirstObjectByType<TrainWreck>();
				Drawbridge bridge = wreck ? null : Object.FindFirstObjectByType<Drawbridge>();
				sceneScale = wreck ? wreck.occlusion : (bridge ? bridge.occlusion : 1f);
			}
		}

		public override void Create()
		{
			Upload(0, 0);
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (builtAt != Time.frameCount)
			{
				builtAt = Time.frameCount;
				Camera cam = Camera.main ? Camera.main : renderingData.cameraData.camera;
				Gather(cam.transform.position);
			}
		}

		protected override void Dispose(bool disposing)
		{
			Off();
		}

		private void OnValidate()
		{
			if (!base.isActive)
			{
				Off();
			}
			scannedAt = -9999;
			builtAt = -1;
		}

		private static void Off()
		{
			Shader.SetGlobalFloat(StrengthId, 0f);
			Shader.SetGlobalFloat(DebugId, 0f);
			Shader.SetGlobalFloat(CapCountId, 0f);
			Shader.SetGlobalFloat(BoxCountId, 0f);
		}

		private void Gather(Vector3 eye)
		{
			if (Time.frameCount - scannedAt >= rescanFrames || Time.frameCount < scannedAt)
			{
				Scan();
			}
			caps.Clear();
			boxes.Clear();
			float maxSq = maxDistance * maxDistance;
			for (int i = candidates.Count - 1; i >= 0; i--)
			{
				Collider c = candidates[i];
				if (!c)
				{
					candidates.RemoveAt(i);
					continue;
				}
				if (!c.enabled || !c.gameObject.activeInHierarchy)
				{
					continue;
				}
				float dist = (c.bounds.ClosestPoint(eye) - eye).sqrMagnitude;
				if (dist > maxSq)
				{
					continue;
				}
				Transform t = c.transform;
				Vector3 scale = t.lossyScale;
				if (c is CapsuleCollider capsule)
				{
					int dir = capsule.direction;
					float across = dir switch
					{
						0 => Mathf.Max(Mathf.Abs(scale.y), Mathf.Abs(scale.z)),
						1 => Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z)),
						_ => Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y)),
					};
					float radius = capsule.radius;
					Vector3 axis = Vector3.zero;
					axis[dir] = Mathf.Max(0f, capsule.height * 0.5f - capsule.radius);
					Vector3 a = t.TransformPoint(capsule.center + axis);
					Vector3 b = t.TransformPoint(capsule.center - axis);
					caps.Add(new Shape
					{
						dist = dist,
						a = Point(a, across * radius),
						b = Point(b, 0f)
					});
					continue;
				}
				if (c is SphereCollider sphere)
				{
					float r = Mathf.Max(Mathf.Abs(scale.x), Mathf.Max(Mathf.Abs(scale.y), Mathf.Abs(scale.z))) * sphere.radius;
					Vector3 p = t.TransformPoint(sphere.center);
					caps.Add(new Shape
					{
						dist = dist,
						a = Point(p, r),
						b = Point(p, 0f)
					});
					continue;
				}
				Vector3 center;
				Vector3 size;
				if (c is BoxCollider box)
				{
					center = box.center;
					size = box.size;
				}
				else
				{
					if (!(c is MeshCollider mesh) || !mesh.sharedMesh)
					{
						continue;
					}
					center = mesh.sharedMesh.bounds.center;
					size = mesh.sharedMesh.bounds.size;
				}
				Vector3 half = size * 0.5f * boxScale;
				Vector3 pos = t.TransformPoint(center);
				Vector3 right = t.right;
				Vector3 up = t.up;
				Vector3 forward = t.forward;
				boxes.Add(new Shape
				{
					dist = dist,
					a = Point(right / Mathf.Max(Mathf.Abs(scale.x) * half.x, 0.001f), pos.x),
					b = Point(up / Mathf.Max(Mathf.Abs(scale.y) * half.y, 0.001f), pos.y),
					c = Point(forward / Mathf.Max(Mathf.Abs(scale.z) * half.z, 0.001f), pos.z)
				});
			}
			caps.Sort(nearer);
			boxes.Sort(nearer);
			int capCount = Mathf.Min(caps.Count, maxCapsules);
			int boxCount = Mathf.Min(boxes.Count, maxBoxes);
			Vector3 lo = Vector3.one * float.MaxValue;
			Vector3 hi = Vector3.one * float.MinValue;
			for (int i = 0; i < capCount; i++)
			{
				capA[i] = caps[i].a;
				capB[i] = caps[i].b;
				Vector3 pad = Vector3.one * caps[i].a.w * range;
				Vector3 a = caps[i].a;
				Vector3 b = caps[i].b;
				lo = Vector3.Min(lo, Vector3.Min(a, b) - pad);
				hi = Vector3.Max(hi, Vector3.Max(a, b) + pad);
			}
			for (int i = 0; i < boxCount; i++)
			{
				box0[i] = boxes[i].a;
				box1[i] = boxes[i].b;
				box2[i] = boxes[i].c;
				Vector3 center = new Vector3(boxes[i].a.w, boxes[i].b.w, boxes[i].c.w);
				float reach = 1f / Mathf.Min(((Vector3)boxes[i].a).magnitude, Mathf.Min(((Vector3)boxes[i].b).magnitude, ((Vector3)boxes[i].c).magnitude));
				Vector3 pad = Vector3.one * reach * range;
				lo = Vector3.Min(lo, center - pad);
				hi = Vector3.Max(hi, center + pad);
			}
			if (capCount + boxCount == 0)
			{
				lo = Vector3.zero;
				hi = Vector3.zero;
			}
			Shader.SetGlobalVector(BoundsMinId, lo);
			Shader.SetGlobalVector(BoundsMaxId, hi);
			Upload(capCount, boxCount);
		}

		private static Vector4 Point(Vector3 p, float w)
		{
			return new Vector4(p.x, p.y, p.z, w);
		}

		private void Scan()
		{
			scannedAt = Time.frameCount;
			candidates.Clear();
			foreach (Collider c in Object.FindObjectsByType<Collider>(FindObjectsSortMode.None))
			{
				if (c.isTrigger || !c.attachedRigidbody || skip.Contains(c.attachedRigidbody) || (bool)c.GetComponentInParent<Rope>())
				{
					continue;
				}
				Hazard hazard = Hazard.Of(c.attachedRigidbody);
				if (((bool)hazard && hazard.belt != 0f) || c.bounds.extents.magnitude < minSize)
				{
					continue;
				}
				if (c is CapsuleCollider || c is SphereCollider || c is BoxCollider || c is MeshCollider)
				{
					candidates.Add(c);
				}
			}
		}

		private void Upload(int capCount, int boxCount)
		{
			Shader.SetGlobalVectorArray(CapAId, capA);
			Shader.SetGlobalVectorArray(CapBId, capB);
			Shader.SetGlobalVectorArray(Box0Id, box0);
			Shader.SetGlobalVectorArray(Box1Id, box1);
			Shader.SetGlobalVectorArray(Box2Id, box2);
			Shader.SetGlobalFloat(CapCountId, base.isActive ? ((float)capCount) : 0f);
			Shader.SetGlobalFloat(BoxCountId, base.isActive ? ((float)boxCount) : 0f);
			Shader.SetGlobalFloat(StrengthId, base.isActive ? (strength * sceneScale) : 0f);
			Shader.SetGlobalFloat(DirectId, direct);
			Shader.SetGlobalFloat(RangeId, range);
			Shader.SetGlobalFloat(DebugId, base.isActive ? (showAO ? 1f : 0f) : 0f);
		}
	}
}
