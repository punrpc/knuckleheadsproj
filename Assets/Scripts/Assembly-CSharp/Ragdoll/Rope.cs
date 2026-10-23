using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ragdoll
{
	[ExecuteAlways]
	public class Rope : MonoBehaviour
	{
		public class RopeTouch : MonoBehaviour
		{
			[HideInInspector]
			public float lastImpulse;

			[HideInInspector]
			public string hitName = "";

			private float acc;

			private string accName = "";

			private void OnCollisionStay(Collision c)
			{
				float impulse = c.impulse.magnitude;
				if (impulse > acc)
				{
					acc = impulse;
					accName = c.collider.name;
				}
			}

			private void FixedUpdate()
			{
				hitName = accName;
				lastImpulse = acc;
				acc = 0f;
				accName = "";
			}

			private void OnCollisionEnter(Collision c)
			{
				Rope rope = GetComponentInParent<Rope>();
				if (rope)
				{
					rope.Struck(GetComponent<Rigidbody>(), c);
				}
			}
		}

		[Header("Build")]
		public float length = 6f;

		public int segments = 16;

		public float radius = 0.03f;

		public float segmentMass = 12f;

		public static float massOverride;

		public float bendSpring = 60f;

		public float bendDamp = 6f;

		public float maxSpeed = 12f;

		public float maxSpin = 20f;

		public Transform end;

		public Rigidbody start;

		public float slack = 0.06f;

		[Header("Snap")]
		public float snapWear;

		public float snapSpeed = 3f;

		public AudioClip snapSound;

		public float snapVolume = 1f;

		[HideInInspector]
		public float wear;

		[HideInInspector]
		public int snapped = -1;

		public Action<int, float> onHit;

		private static Player local;

		[Header("Climb")]
		public float climbTime;

		[Header("Look")]
		public Mesh piece;

		public Material ropeMaterial;

		public float visualRadius;

		public MeshFilter skin;

		private static Mesh defaultPiece;

		private static Material defaultMaterial;

		private const string MeshName = "Rope Mesh";

		private Mesh built;

		private void OnEnable()
		{
			RebuildVisual();
		}

		private void OnDisable()
		{
			if (!built)
			{
				return;
			}
			if (Application.isPlaying)
			{
				Destroy(built);
			}
			else
			{
				DestroyImmediate(built);
			}
			built = null;
		}

		private void Start()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			Top();
			Rigidbody bottom = end ? Anchor(end) : null;
			Rigidbody[] links = Links();
			if (links.Length == 0)
			{
				Debug.LogWarning("Rope '" + name + "' has no segments: press Build on the Rope component", this);
				return;
			}
			Player player = UnityEngine.Object.FindFirstObjectByType<Player>();
			Collider[] mine = player ? player.GetComponentsInChildren<Collider>() : new Collider[0];
			Collider[] hits = new Collider[16];
			Physics.SyncTransforms();
			foreach (Rigidbody link in links)
			{
				link.maxLinearVelocity = maxSpeed;
				link.maxAngularVelocity = maxSpin;
				Collider col = link.GetComponent<Collider>();
				if (col)
				{
					foreach (Collider c in mine)
					{
						if (!c)
						{
							continue;
						}
						string part = c.attachedRigidbody ? c.attachedRigidbody.name : c.name;
						if (!part.Contains("Arm") && !part.StartsWith("Hand"))
						{
							Physics.IgnoreCollision(col, c, true);
						}
					}
				}
				CapsuleCollider capsule = col as CapsuleCollider;
				if (capsule)
				{
					Vector3 axis = link.transform.up * Mathf.Max(capsule.height * 0.5f - capsule.radius, 0f);
					int count = Physics.OverlapCapsuleNonAlloc(link.position + axis, link.position - axis, capsule.radius, hits, -1, QueryTriggerInteraction.Ignore);
					for (int i = 0; i < count; i++)
					{
						if (!hits[i].transform.IsChildOf(transform) && !hits[i].GetComponentInParent<Player>() && !hits[i].GetComponentInParent<AvatarBody>())
						{
							Physics.IgnoreCollision(capsule, hits[i], true);
						}
					}
					Rigidbody anchor = (link == links[links.Length - 1]) ? bottom : ((link == links[0] && start) ? start : null);
					if (anchor)
					{
						foreach (Collider c2 in anchor.GetComponentsInChildren<Collider>())
						{
							Physics.IgnoreCollision(capsule, c2, true);
						}
					}
				}
				if (!link.GetComponent<RopeTouch>())
				{
					link.gameObject.AddComponent<RopeTouch>();
				}
				Grabbable grab = link.GetComponent<Grabbable>();
				if (grab)
				{
					grab.pressWhileTouching = true;
					grab.climbTime = climbTime;
				}
			}
		}

		[ContextMenu("Build")]
		public void Build()
		{
			Clear();
			Rigidbody top = Top();
			float scale = Mathf.Max(transform.lossyScale.y, 0.0001f);
			Vector3[] hang = end ? Hang(transform.position, end.position, TotalLength(), Mathf.Max(segments, 1)) : null;
			Vector3 side = end ? Vector3.Cross(Vector3.up, end.position - transform.position) : Vector3.zero;
			side = (side.sqrMagnitude > 0.0001f) ? side.normalized : Vector3.forward;
			Rigidbody prev = null;
			float size = length / segments;
			for (int i = 0; i < segments; i++)
			{
				float last = size;
				GameObject link = new GameObject("Rope " + i);
				link.transform.SetParent(transform, false);
				if (hang == null)
				{
					link.transform.localPosition = Vector3.down * (size * (i + 0.5f));
					link.transform.localRotation = Quaternion.identity;
				}
				else
				{
					size = Vector3.Distance(hang[i], hang[i + 1]) / scale;
					link.transform.SetPositionAndRotation((hang[i] + hang[i + 1]) * 0.5f, Quaternion.LookRotation(side, (hang[i] - hang[i + 1]).normalized));
				}
				CapsuleCollider capsule = link.AddComponent<CapsuleCollider>();
				capsule.direction = 1;
				capsule.radius = radius;
				capsule.height = size;
				Rigidbody rb = link.AddComponent<Rigidbody>();
				rb.mass = (massOverride > 0f) ? massOverride : segmentMass;
				rb.linearDamping = 0.3f;
				rb.angularDamping = 0.5f;
				rb.interpolation = RigidbodyInterpolation.Interpolate;
				rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
				rb.solverIterations = 120;
				rb.solverVelocityIterations = 16;
				rb.maxDepenetrationVelocity = 1f;
				rb.maxLinearVelocity = maxSpeed;
				rb.maxAngularVelocity = maxSpin;
				ConfigurableJoint joint = link.AddComponent<ConfigurableJoint>();
				joint.connectedBody = prev ? prev : top;
				joint.autoConfigureConnectedAnchor = false;
				joint.anchor = new Vector3(0f, size * 0.5f, 0f);
				joint.connectedAnchor = prev ? new Vector3(0f, last * -0.5f, 0f) : top.transform.InverseTransformPoint(transform.position);
				joint.xMotion = ConfigurableJointMotion.Locked;
				joint.yMotion = ConfigurableJointMotion.Locked;
				joint.zMotion = ConfigurableJointMotion.Locked;
				joint.angularXMotion = ConfigurableJointMotion.Free;
				joint.angularYMotion = ConfigurableJointMotion.Free;
				joint.angularZMotion = ConfigurableJointMotion.Free;
				joint.rotationDriveMode = RotationDriveMode.Slerp;
				joint.slerpDrive = new JointDrive
				{
					positionSpring = bendSpring,
					positionDamper = bendDamp,
					maximumForce = float.MaxValue
				};
				joint.enableCollision = false;
				joint.enablePreprocessing = false;
				joint.projectionMode = JointProjectionMode.PositionAndRotation;
				joint.projectionDistance = 0.005f;
				joint.projectionAngle = 30f;
				link.AddComponent<Grabbable>().holdsYou = true;
				if (hang != null && i == segments - 1)
				{
					Rigidbody anchor = Anchor(end);
					bool fixedEnd = anchor.isKinematic;
					ConfigurableJoint tie = (fixedEnd ? link : anchor.gameObject).AddComponent<ConfigurableJoint>();
					tie.connectedBody = fixedEnd ? anchor : rb;
					tie.autoConfigureConnectedAnchor = false;
					tie.anchor = fixedEnd ? new Vector3(0f, size * -0.5f, 0f) : anchor.transform.InverseTransformPoint(end.position);
					tie.connectedAnchor = fixedEnd ? anchor.transform.InverseTransformPoint(end.position) : new Vector3(0f, size * -0.5f, 0f);
					tie.xMotion = ConfigurableJointMotion.Locked;
					tie.yMotion = ConfigurableJointMotion.Locked;
					tie.zMotion = ConfigurableJointMotion.Locked;
					tie.enableCollision = false;
					tie.enablePreprocessing = false;
					tie.projectionMode = JointProjectionMode.PositionAndRotation;
					tie.projectionDistance = 0.005f;
					tie.projectionAngle = 30f;
				}
				prev = rb;
			}
			Physics.SyncTransforms();
			RebuildVisual();
			if (Application.isPlaying)
			{
				Start();
			}
		}

		[ContextMenu("Clear")]
		public void Clear()
		{
			Rigidbody anchor = end ? end.GetComponentInParent<Rigidbody>() : null;
			if (anchor)
			{
				foreach (ConfigurableJoint joint in anchor.GetComponents<ConfigurableJoint>())
				{
					if (!joint.connectedBody || joint.connectedBody.transform.IsChildOf(transform))
					{
						if (Application.isPlaying)
						{
							Destroy(joint);
						}
						else
						{
							DestroyImmediate(joint);
						}
					}
				}
			}
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				Transform child = transform.GetChild(i);
				if (Application.isPlaying)
				{
					Destroy(child.gameObject);
				}
				else
				{
					DestroyImmediate(child.gameObject);
				}
			}
			if (built)
			{
				if (Application.isPlaying)
				{
					Destroy(built);
				}
				else
				{
					DestroyImmediate(built);
				}
				built = null;
			}
		}

		[ContextMenu("Rebuild Visual")]
		public void RebuildVisual()
		{
			Rigidbody[] links = Links();
			if (skin && skin.sharedMesh && links.Length != 0)
			{
				SkinVisual(links);
				return;
			}
			Transform old = transform.Find(MeshName);
			Mesh mesh = piece ? piece : defaultPiece;
			Material material = ropeMaterial ? ropeMaterial : defaultMaterial;
			if (piece && !defaultPiece)
			{
				defaultPiece = piece;
			}
			if (ropeMaterial && !defaultMaterial)
			{
				defaultMaterial = ropeMaterial;
			}
			if (links.Length == 0 || !mesh)
			{
				if (old)
				{
					if (Application.isPlaying)
					{
						Destroy(old.gameObject);
					}
					else
					{
						DestroyImmediate(old.gameObject);
					}
				}
				return;
			}
			GameObject go = old ? old.gameObject : new GameObject(MeshName);
			if (!old)
			{
				go.transform.SetParent(transform, false);
				go.hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
			}
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
			if (!smr)
			{
				smr = go.AddComponent<SkinnedMeshRenderer>();
			}
			Bounds bounds = mesh.bounds;
			Vector3 size = bounds.size;
			int axis = (size.x >= size.y && size.x >= size.z) ? 0 : ((size.y >= size.z) ? 1 : 2);
			float low = bounds.min[axis];
			float span = Mathf.Max(size[axis], 0.0001f);
			Vector3[] vertices = mesh.vertices;
			float pieceRadius = float.MaxValue;
			for (int i = 0; i < vertices.Length; i++)
			{
				float u = (axis != 0) ? vertices[i].x : vertices[i].y;
				float w = (axis != 2) ? vertices[i].z : vertices[i].y;
				float r = Mathf.Sqrt(u * u + w * w);
				if (r < pieceRadius && r > 0.0005f)
				{
					pieceRadius = r;
				}
			}
			if (pieceRadius == float.MaxValue)
			{
				pieceRadius = Mathf.Max((axis != 0) ? size.x : size.y, (axis != 2) ? size.z : size.y, 0.0001f) * 0.5f;
			}
			float thickness = (visualRadius > 0f) ? visualRadius : radius;
			Vector3[] normals = mesh.normals;
			Vector2[] uv = mesh.uv;
			int[] triangles = mesh.triangles;
			int n = links.Length;
			Vector3[] verts = new Vector3[vertices.Length * n];
			Vector3[] norms = new Vector3[vertices.Length * n];
			Vector2[] uvs = new Vector2[vertices.Length * n];
			Vector2[] along = new Vector2[vertices.Length * n];
			BoneWeight[] weights = new BoneWeight[vertices.Length * n];
			int[] tris = new int[triangles.Length * n];
			Transform[] bones = new Transform[n + 1];
			bones[0] = (start ? (Component)start : this).transform;
			for (int j = 0; j < n; j++)
			{
				bones[j + 1] = links[j].transform;
			}
			float scale = thickness / pieceRadius;
			int at = 0;
			for (int k = 0; k < n; k++)
			{
				Transform t = links[k].transform;
				CapsuleCollider capsule = links[k].GetComponent<CapsuleCollider>();
				float h = capsule ? capsule.height : (length / n);
				for (int v = 0; v < vertices.Length; v++)
				{
					Vector3 p = vertices[v];
					float a = (axis == 0) ? p.x : ((axis == 1) ? p.y : p.z);
					float f = (a - low) / span;
					float x = scale * ((axis != 0) ? p.x : p.y);
					float z = scale * ((axis != 2) ? p.z : p.y);
					Vector3 world = t.TransformPoint(new Vector3(x, h * (0.5f - f), z));
					Quaternion rot = t.rotation;
					bool joinNext = f > 0.999f && k + 1 < n && k + 1 != snapped;
					bool joinPrev = k != 0 && f < 0.001f && k != snapped;
					if (joinNext || joinPrev)
					{
						Transform upper = joinNext ? t : links[k - 1].transform;
						Transform lower = joinNext ? links[k + 1].transform : t;
						CapsuleCollider upperCap = upper.GetComponent<CapsuleCollider>();
						CapsuleCollider lowerCap = lower.GetComponent<CapsuleCollider>();
						float upperH = upperCap ? upperCap.height : (length / n);
						float lowerH = lowerCap ? lowerCap.height : (length / n);
						Vector3 joint = (upper.TransformPoint(new Vector3(0f, upperH * -0.5f, 0f)) + lower.TransformPoint(new Vector3(0f, lowerH * 0.5f, 0f))) * 0.5f;
						rot = Quaternion.Slerp(upper.rotation, lower.rotation, 0.5f);
						world = joint + rot * new Vector3(x, 0f, z);
					}
					int idx = at + v;
					verts[idx] = transform.InverseTransformPoint(world);
					Vector3 normal;
					if (normals != null && normals.Length == vertices.Length)
					{
						Vector3 nv = normals[v];
						normal = new Vector3((axis != 0) ? nv.x : nv.y, 0f - nv[axis], (axis != 2) ? nv.z : nv.y);
					}
					else
					{
						normal = Vector3.up;
					}
					norms[idx] = transform.InverseTransformDirection(rot * normal).normalized;
					uvs[idx] = (uv != null && uv.Length == vertices.Length) ? uv[v] : Vector2.zero;
					along[idx] = new Vector2((f + k) / n, 0f);
					weights[idx] = Weigh(k, f, n);
				}
				for (int l = 0; l < triangles.Length; l++)
				{
					tris[k * triangles.Length + l] = triangles[l] + k * vertices.Length;
				}
				at += vertices.Length;
			}
			Matrix4x4[] bindposes = new Matrix4x4[n + 1];
			for (int m = 0; m <= n; m++)
			{
				bindposes[m] = bones[m].worldToLocalMatrix * transform.localToWorldMatrix;
			}
			if (!built)
			{
				built = new Mesh();
				built.name = MeshName;
				built.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
			}
			built.Clear();
			built.vertices = verts;
			built.normals = norms;
			built.uv = uvs;
			built.uv2 = along;
			built.boneWeights = weights;
			built.bindposes = bindposes;
			built.triangles = tris;
			built.RecalculateBounds();
			smr.sharedMesh = built;
			smr.bones = bones;
			smr.rootBone = transform;
			smr.updateWhenOffscreen = true;
			smr.sharedMaterial = material;
			smr.shadowCastingMode = ShadowCastingMode.On;
		}

		private void SkinVisual(Rigidbody[] links)
		{
			Transform old = transform.Find(MeshName);
			GameObject go = old ? old.gameObject : new GameObject(MeshName);
			if (!old)
			{
				go.transform.SetParent(transform, false);
				go.hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
			}
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
			if (!smr)
			{
				smr = go.AddComponent<SkinnedMeshRenderer>();
			}
			Mesh source = skin.sharedMesh;
			int n = links.Length;
			Vector3[] tops = new Vector3[n];
			Vector3[] bottoms = new Vector3[n];
			for (int i = 0; i < n; i++)
			{
				CapsuleCollider capsule = links[i].GetComponent<CapsuleCollider>();
				float h = capsule ? capsule.height : (length / n);
				tops[i] = links[i].transform.TransformPoint(new Vector3(0f, h * 0.5f, 0f));
				bottoms[i] = links[i].transform.TransformPoint(new Vector3(0f, h * -0.5f, 0f));
			}
			Vector3[] vertices = source.vertices;
			Vector3[] normals = source.normals;
			Vector3[] verts = new Vector3[vertices.Length];
			Vector3[] norms = new Vector3[vertices.Length];
			BoneWeight[] weights = new BoneWeight[vertices.Length];
			for (int v = 0; v < vertices.Length; v++)
			{
				Vector3 p = skin.transform.TransformPoint(vertices[v]);
				int best = 0;
				float bestF = 0f;
				float bestD = float.MaxValue;
				for (int j = 0; j < n; j++)
				{
					Vector3 seg = bottoms[j] - tops[j];
					float f = Mathf.Clamp01(Vector3.Dot(p - tops[j], seg) / Mathf.Max(seg.sqrMagnitude, 1E-06f));
					float d = (p - (tops[j] + seg * f)).sqrMagnitude;
					if (d < bestD)
					{
						bestD = d;
						best = j;
						bestF = f;
					}
				}
				weights[v] = Weigh(best, bestF, n);
				verts[v] = transform.InverseTransformPoint(p);
				norms[v] = (normals != null && normals.Length == vertices.Length) ? transform.InverseTransformDirection(skin.transform.TransformDirection(normals[v])).normalized : Vector3.up;
			}
			Transform[] bones = new Transform[n + 1];
			bones[0] = (start ? (Component)start : this).transform;
			for (int k = 0; k < n; k++)
			{
				bones[k + 1] = links[k].transform;
			}
			Matrix4x4[] bindposes = new Matrix4x4[n + 1];
			for (int m = 0; m <= n; m++)
			{
				bindposes[m] = bones[m].worldToLocalMatrix * transform.localToWorldMatrix;
			}
			if (!built)
			{
				built = new Mesh();
				built.name = MeshName;
				built.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
			}
			built.Clear();
			built.indexFormat = (vertices.Length > 65000) ? IndexFormat.UInt32 : IndexFormat.UInt16;
			built.vertices = verts;
			built.normals = norms;
			built.uv = source.uv;
			built.boneWeights = weights;
			built.bindposes = bindposes;
			built.subMeshCount = source.subMeshCount;
			for (int s = 0; s < source.subMeshCount; s++)
			{
				built.SetTriangles(source.GetTriangles(s), s);
			}
			built.RecalculateBounds();
			smr.sharedMesh = built;
			smr.bones = bones;
			smr.rootBone = transform;
			smr.updateWhenOffscreen = true;
			smr.sharedMaterials = skin.GetComponent<Renderer>().sharedMaterials;
			smr.shadowCastingMode = ShadowCastingMode.On;
			Material shared = smr.sharedMaterial;
			if (shared && shared.HasProperty("_FadeStart"))
			{
				float y0 = tops[0].y;
				float y1 = bottoms[n - 1].y;
				float lo = Mathf.Min(y0, y1);
				float hi = Mathf.Max(y0, y1);
				MaterialPropertyBlock block = new MaterialPropertyBlock();
				smr.GetPropertyBlock(block);
				block.SetFloat("_FadeStart", lo + (hi - lo) * 0.35f);
				block.SetFloat("_FadeEnd", lo + (hi - lo) * 0.85f);
				smr.SetPropertyBlock(block);
			}
		}

		private BoneWeight Weigh(int i, float f, int n)
		{
			BoneWeight w = default(BoneWeight);
			if (f < 0.5f)
			{
				w.boneIndex0 = i + 1;
				if (i == snapped)
				{
					w.weight0 = 1f;
				}
				else
				{
					w.weight0 = f + 0.5f;
					w.boneIndex1 = i;
					w.weight1 = 0.5f - f;
				}
			}
			else if (i + 1 >= n || i + 1 == snapped)
			{
				w.boneIndex0 = i + 1;
				w.weight0 = 1f;
			}
			else
			{
				w.boneIndex0 = i + 1;
				w.weight0 = 1.5f - f;
				w.boneIndex1 = i + 2;
				w.weight1 = f - 0.5f;
			}
			return w;
		}

		public void Struck(Rigidbody link, Collision c)
		{
			if (snapWear <= 0f || snapped >= 0)
			{
				return;
			}
			float speed = c.relativeVelocity.magnitude;
			if (speed < snapSpeed)
			{
				return;
			}
			if (!ByLocal(c.collider) && !OwnProp(c.collider))
			{
				return;
			}
			Player player = c.collider.GetComponentInParent<Player>();
			if (player && (player.flopped || player.knockedOut))
			{
				return;
			}
			int at = Array.IndexOf(Links(), link);
			if (at >= 0)
			{
				onHit?.Invoke(at, speed);
				Wear(at, speed);
			}
		}

		public void Wear(int at, float amount)
		{
			wear += amount;
			if (wear >= snapWear)
			{
				Snap(at);
			}
		}

		public void Snap(int at)
		{
			Rigidbody[] links = Links();
			if (snapped >= 0 || links.Length == 0)
			{
				return;
			}
			at = Mathf.Clamp(at, 0, links.Length - 1);
			Rigidbody above = (at == 0) ? Top() : links[at - 1];
			foreach (Joint joint in links[at].GetComponents<Joint>())
			{
				if (joint.connectedBody == above)
				{
					Destroy(joint);
				}
			}
			snapped = at;
			Loosen(links);
			if (snapSound)
			{
				AudioSource.PlayClipAtPoint(snapSound, links[at].position, snapVolume * Prefs.sfx);
			}
			foreach (Rigidbody rb in links)
			{
				rb.WakeUp();
			}
			if (end)
			{
				Rigidbody anchor = end.GetComponentInParent<Rigidbody>();
				if (anchor)
				{
					anchor.WakeUp();
				}
			}
			RebuildVisual();
			Player.Note(name + " snapped at link " + at);
		}

		private void Loosen(Rigidbody[] links)
		{
			Rigidbody anchor = end ? end.GetComponentInParent<Rigidbody>() : null;
			Transform parent = (anchor ? (Component)anchor : this).transform.parent;
			if (!parent)
			{
				return;
			}
			Collider[] around = parent.GetComponentsInChildren<Collider>();
			for (int i = 0; i < links.Length; i++)
			{
				Collider col = links[i].GetComponent<Collider>();
				if (!col)
				{
					continue;
				}
				links[i].maxDepenetrationVelocity = 2f;
				Rigidbody keep = (i == links.Length - 1) ? anchor : ((i == 0) ? start : null);
				foreach (Collider other in around)
				{
					if (!other || other.GetComponentInParent<Rope>())
					{
						continue;
					}
					if (keep && other.attachedRigidbody == keep)
					{
						continue;
					}
					Physics.IgnoreCollision(col, other, false);
				}
			}
		}

		private bool OwnProp(Collider c)
		{
			Rigidbody rb = c.attachedRigidbody;
			if (!rb || rb.isKinematic)
			{
				return false;
			}
			if (rb.GetComponentInParent<Rope>() || rb.GetComponentInParent<Player>() || rb.GetComponentInParent<LooseHat>())
			{
				return false;
			}
			Grabbable grab = rb.GetComponent<Grabbable>();
			if (!grab || grab.holdsYou)
			{
				return false;
			}
			NetProp prop = rb.GetComponent<NetProp>();
			if (prop && prop.Object && prop.Object.IsValid)
			{
				return prop.HasStateAuthority;
			}
			return onHit == null;
		}

		private static bool ByLocal(Collider c)
		{
			Player player = c.GetComponentInParent<Player>();
			if (player)
			{
				if (player.dummy)
				{
					return false;
				}
				return player.playSpace;
			}
			Rigidbody rb = c.attachedRigidbody;
			if (!rb)
			{
				return false;
			}
			if (!local)
			{
				foreach (Player p in UnityEngine.Object.FindObjectsByType<Player>(FindObjectsSortMode.None))
				{
					if (p.playSpace && !p.dummy)
					{
						local = p;
					}
				}
			}
			if (!local)
			{
				return false;
			}
			HandGrab left = local.leftHand ? local.leftHand.GetComponent<HandGrab>() : null;
			HandGrab right = local.rightHand ? local.rightHand.GetComponent<HandGrab>() : null;
			if (left && left.grabbing && left.held == rb)
			{
				return true;
			}
			if (right && right.grabbing)
			{
				return right.held == rb;
			}
			return false;
		}

		public float TotalLength()
		{
			if (!end)
			{
				return length;
			}
			return Vector3.Distance(transform.position, end.position) * (slack + 1f);
		}

		private Rigidbody Top()
		{
			Rigidbody own = GetComponent<Rigidbody>();
			if (start)
			{
				if (own)
				{
					if (Application.isPlaying)
					{
						Destroy(own);
					}
					else
					{
						DestroyImmediate(own);
					}
				}
				return start;
			}
			Rigidbody rb = own ? own : gameObject.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;
			return rb;
		}

		private static Rigidbody Anchor(Transform at)
		{
			Rigidbody rb = at.GetComponentInParent<Rigidbody>();
			if (rb)
			{
				return rb;
			}
			Rigidbody added = at.gameObject.AddComponent<Rigidbody>();
			added.isKinematic = true;
			added.useGravity = false;
			return added;
		}

		private static Vector3[] Hang(Vector3 a, Vector3 b, float total, int count)
		{
			Vector3 flat = new Vector3(b.x - a.x, 0f, b.z - a.z);
			float rise = b.y - a.y;
			Vector3[] points = new Vector3[count + 1];
			float d = Mathf.Sqrt(flat.x * flat.x + flat.z * flat.z);
			if (d < 0.01f || Vector3.Distance(a, b) + 0.0001f >= total)
			{
				for (int i = 0; i <= count; i++)
				{
					points[i] = Vector3.Lerp(a, b, (float)i / (float)count);
				}
				return points;
			}
			float span = Mathf.Sqrt(total * total - rise * rise);
			float lo = Mathf.Log(d / 100f);
			float hi = Mathf.Log(d * 1000f);
			for (int j = 0; j < 60; j++)
			{
				float mid = (lo + hi) * 0.5f;
				float c = Mathf.Exp(mid);
				if (2f * c * Sinh(d / (2f * c)) > span)
				{
					lo = mid;
				}
				else
				{
					hi = mid;
				}
			}
			float k = Mathf.Exp((lo + hi) * 0.5f);
			float x0 = d * 0.5f - Atanh(Mathf.Clamp(rise / total, -0.9999f, 0.9999f)) * k;
			float s0 = Sinh(-x0 / k);
			float c0 = Cosh(-x0 / k);
			Vector3 dir = flat / d;
			for (int l = 0; l <= count; l++)
			{
				float x = x0 + k * Asinh(s0 + (float)l * total / (float)count / k);
				float y = k * (Cosh((x - x0) / k) - c0);
				points[l] = a + dir * x + Vector3.up * y;
			}
			points[0] = a;
			points[count] = b;
			return points;
		}

		private static float Sinh(float x)
		{
			return (Mathf.Exp(x) - Mathf.Exp(0f - x)) * 0.5f;
		}

		private static float Cosh(float x)
		{
			return (Mathf.Exp(x) + Mathf.Exp(0f - x)) * 0.5f;
		}

		private static float Asinh(float x)
		{
			float r = Mathf.Log(Mathf.Abs(x) + Mathf.Sqrt(x * x + 1f));
			return (x >= 0f) ? r : (0f - r);
		}

		private static float Atanh(float x)
		{
			return Mathf.Log((x + 1f) / (1f - x)) * 0.5f;
		}

		public Rigidbody[] Links()
		{
			List<Rigidbody> list = new List<Rigidbody>();
			foreach (Transform child in transform)
			{
				Rigidbody rb = child.GetComponent<Rigidbody>();
				if (rb)
				{
					list.Add(rb);
				}
			}
			return list.ToArray();
		}
	}
}
