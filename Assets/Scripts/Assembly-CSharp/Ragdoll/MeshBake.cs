using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ragdoll
{
	[ExecuteAlways]
	public class MeshBake : MonoBehaviour
	{
		private class Source
		{
			public Vector3[] vertices;

			public Vector3[] normals;

			public Vector2[] uv;

			public List<Vector2> uv3 = new List<Vector2>();

			public int[][] used;

			public int[][] indices;
		}

		private class Part
		{
			public Source source;

			public int sub;

			public Matrix4x4 world;
		}

		private class Group
		{
			public Material material;

			public Renderer first;

			public bool gradient;

			public bool meshGradient;

			public bool worldSpace;

			public Vector3 direction;

			public int vertexCount;

			public List<Part> parts = new List<Part>();
		}

		private class Pending
		{
			public Mesh mesh;

			public Vector3[] pos;

			public Vector3[] nor;

			public float[] open;
		}

		[Header("Bake")]
		public int sectors = 8;

		public bool bakeOnStart = true;

		public bool castShadows;

		public bool receiveShadows;

		public bool ambientOcclusion;

		public bool staticAO;

		public float aoEdge;

		public float aoFrom = -1f;

		public float aoTo = 4f;

		public float lightEdge;

		public float staticRange = 2.5f;

		public float staticStrength = 1f;

		[Header("Editor")]
		public bool previewInEditor;

		private static readonly int TopId = Shader.PropertyToID("_TopColor");

		private static readonly int BottomId = Shader.PropertyToID("_BottomColor");

		private static readonly int DirectionId = Shader.PropertyToID("_Direction");

		private static readonly int WorldSpaceId = Shader.PropertyToID("_WorldSpace");

		private static readonly int MeshGradientId = Shader.PropertyToID("_MeshGradient");

		private static readonly int StaticId = Shader.PropertyToID("_StaticAO");

		private static readonly Vector3[] Rays = Spread(16);

		private const float Blur = 0.3f;

		private const float RefineGap = 0.25f;

		private readonly List<GameObject> baked = new List<GameObject>();

		private readonly List<Renderer> hidden = new List<Renderer>();

		private readonly List<Material> made = new List<Material>();

		private readonly List<Mesh> thick = new List<Mesh>();

		private readonly HashSet<int> solid = new HashSet<int>();

		private readonly List<Pending> pending = new List<Pending>();

		private bool previewing;

		private void Start()
		{
			if (Application.isPlaying && bakeOnStart)
			{
				Bake();
			}
		}

		private void OnEnable()
		{
			// TODO(recon): body is empty in the player build; the editor preview logic was most likely inside #if UNITY_EDITOR and is not recoverable (token 0x600064F)
		}

		private void OnDisable()
		{
			if (previewing)
			{
				Clear();
			}
		}

		private void OnValidate()
		{
			// TODO(recon): body is empty in the player build; the editor preview logic was most likely inside #if UNITY_EDITOR and is not recoverable (token 0x6000651)
		}

		private void Queue()
		{
			// TODO(recon): body is empty in the player build; the editor preview logic was most likely inside #if UNITY_EDITOR and is not recoverable (token 0x6000652)
		}

		private void Preview()
		{
			// TODO(recon): body is empty in the player build; the editor preview logic was most likely inside #if UNITY_EDITOR and is not recoverable (token 0x6000653)
		}

		private void Clear()
		{
			foreach (Renderer r in hidden)
			{
				if (r)
				{
					r.forceRenderingOff = false;
				}
			}
			foreach (GameObject go in baked)
			{
				if (!go)
				{
					continue;
				}
				MeshFilter mf = go.GetComponent<MeshFilter>();
				if (mf && mf.sharedMesh)
				{
					DestroyImmediate(mf.sharedMesh);
				}
				DestroyImmediate(go);
			}
			foreach (Material m in made)
			{
				if (m)
				{
					DestroyImmediate(m);
				}
			}
			List<GameObject> stale = new List<GameObject>();
			foreach (Transform child in transform)
			{
				if ((child.gameObject.hideFlags & HideFlags.DontSave) != 0)
				{
					stale.Add(child.gameObject);
				}
			}
			foreach (GameObject go in stale)
			{
				DestroyImmediate(go);
			}
			baked.Clear();
			made.Clear();
			hidden.Clear();
			previewing = false;
		}

		private void OnDestroy()
		{
			foreach (GameObject go in baked)
			{
				if (!go)
				{
					continue;
				}
				MeshFilter mf = go.GetComponent<MeshFilter>();
				if (mf && mf.sharedMesh)
				{
					Drop(mf.sharedMesh);
				}
			}
			foreach (Material m in made)
			{
				if (m)
				{
					Drop(m);
				}
			}
			foreach (Mesh m in thick)
			{
				if (m)
				{
					Drop(m);
				}
			}
		}

		private static void Drop(Object o)
		{
			if (!Application.isPlaying)
			{
				DestroyImmediate(o);
			}
			else
			{
				Destroy(o);
			}
		}

		public void Bake()
		{
			if (baked.Count > 0)
			{
				return;
			}
			float start = Time.realtimeSinceStartup;
			List<MeshRenderer> renderers = new List<MeshRenderer>();
			bool any = false;
			Bounds bounds = default(Bounds);
			foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
			{
				MeshFilter mf = r.GetComponent<MeshFilter>();
				if (!r.enabled || !mf || !mf.sharedMesh || !mf.sharedMesh.isReadable || r.GetComponent<TMP_Text>())
				{
					continue;
				}
				if (r.GetComponent<Hazard>())
				{
					if (ambientOcclusion && aoEdge > 0f && !previewing)
					{
						Thicken(mf, r.localToWorldMatrix);
					}
					continue;
				}
				renderers.Add(r);
				if (any)
				{
					bounds.Encapsulate(r.bounds);
				}
				else
				{
					bounds = r.bounds;
					any = true;
				}
			}
			if (!any)
			{
				return;
			}
			if (staticAO)
			{
				Physics.SyncTransforms();
				solid.Clear();
				foreach (Collider c in Object.FindObjectsByType<Collider>(FindObjectsSortMode.None))
				{
					if (!c.isTrigger && !c.attachedRigidbody)
					{
						solid.Add(c.GetInstanceID());
					}
				}
			}
			Dictionary<Mesh, Source> sources = new Dictionary<Mesh, Source>();
			Dictionary<string, Group> groups = new Dictionary<string, Group>();
			Dictionary<string, Material> mats = new Dictionary<string, Material>();
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			int n = Mathf.Max(1, sectors);
			foreach (MeshRenderer r in renderers)
			{
				Mesh mesh = r.GetComponent<MeshFilter>().sharedMesh;
				if (!sources.TryGetValue(mesh, out Source src))
				{
					src = Read(mesh);
					sources[mesh] = src;
				}
				Vector3 center = r.bounds.center;
				float angle = Mathf.Atan2(center.z - bounds.center.z, center.x - bounds.center.x);
				int sector = (int)((angle + Mathf.PI) / (Mathf.PI * 2f) * n) % n;
				Material[] shared = r.sharedMaterials;
				for (int i = 0; i < mesh.subMeshCount && i < shared.Length; i++)
				{
					Material m = shared[i];
					if (!m)
					{
						continue;
					}
					r.GetPropertyBlock(block, i);
					if (block.isEmpty && r.HasPropertyBlock())
					{
						r.GetPropertyBlock(block);
					}
					bool hasTop = false;
					bool hasBottom = false;
					Color top = new Color(1f, 1f, 1f, 1f);
					Color bottom = new Color(1f, 1f, 1f, 1f);
					if (!block.isEmpty)
					{
						hasTop = block.HasColor(TopId);
						hasBottom = block.HasColor(BottomId);
						if (hasTop)
						{
							top = block.GetColor(TopId);
						}
						if (hasBottom)
						{
							bottom = block.GetColor(BottomId);
						}
					}
					string matKey = m.GetInstanceID() + (hasTop ? " t" + ColorUtility.ToHtmlStringRGBA(top) : "") + (hasBottom ? " b" + ColorUtility.ToHtmlStringRGBA(bottom) : "");
					string key = matKey + " " + sector + " " + r.gameObject.layer + " " + (int)r.shadowCastingMode + " " + r.receiveShadows;
					if (!groups.TryGetValue(key, out Group g))
					{
						if (!mats.TryGetValue(matKey, out Material mat))
						{
							mat = new Material(m);
							mat.name = m.name + " Baked";
							if (previewing)
							{
								mat.hideFlags = HideFlags.HideAndDontSave;
							}
							if (!ambientOcclusion)
							{
								mat.EnableKeyword("_NO_AO");
							}
							if (!receiveShadows)
							{
								mat.EnableKeyword("_NO_SHADOWS");
							}
							if (hasTop)
							{
								mat.SetColor(TopId, top);
							}
							if (hasBottom)
							{
								mat.SetColor(BottomId, bottom);
							}
							mats[matKey] = mat;
							made.Add(mat);
						}
						g = new Group
						{
							material = mat,
							first = r
						};
						g.gradient = m.HasProperty(MeshGradientId) && m.HasProperty(DirectionId);
						if (g.gradient)
						{
							g.meshGradient = m.GetFloat(MeshGradientId) > 0.5f;
							g.worldSpace = m.HasProperty(WorldSpaceId) && m.GetFloat(WorldSpaceId) > 0.5f;
							g.direction = ((Vector3)m.GetVector(DirectionId) + new Vector3(0f, 1E-06f, 0f)).normalized;
							mat.SetFloat(MeshGradientId, 1f);
						}
						groups[key] = g;
					}
					g.parts.Add(new Part
					{
						source = src,
						sub = i,
						world = r.localToWorldMatrix
					});
					g.vertexCount += src.used[i].Length;
				}
				hidden.Add(r);
			}
			int vertices = 0;
			foreach (KeyValuePair<string, Group> pair in groups)
			{
				Build(pair.Value);
				vertices += pair.Value.vertexCount;
			}
			if (staticAO)
			{
				Soften();
			}
			foreach (Renderer r in hidden)
			{
				if (previewing)
				{
					r.forceRenderingOff = true;
				}
				else
				{
					r.enabled = false;
				}
			}
			Debug.Log("MeshBake " + base.name + ": " + hidden.Count + " renderers into " + baked.Count + " meshes, " + vertices + " vertices, " + ((Time.realtimeSinceStartup - start) * 1000f).ToString("F0") + " ms");
		}

		private static Source Read(Mesh mesh)
		{
			Source s = new Source();
			s.vertices = mesh.vertices;
			s.normals = mesh.normals;
			s.uv = mesh.uv;
			mesh.GetUVs(3, s.uv3);
			int subs = mesh.subMeshCount;
			s.used = new int[subs][];
			s.indices = new int[subs][];
			int[] remap = new int[s.vertices.Length];
			for (int sub = 0; sub < subs; sub++)
			{
				for (int i = 0; i < remap.Length; i++)
				{
					remap[i] = -1;
				}
				int[] tris = mesh.GetTriangles(sub);
				List<int> used = new List<int>();
				int[] idx = new int[tris.Length];
				for (int i = 0; i < tris.Length; i++)
				{
					int v = tris[i];
					if (remap[v] < 0)
					{
						remap[v] = used.Count;
						used.Add(v);
					}
					idx[i] = remap[v];
				}
				s.used[sub] = used.ToArray();
				s.indices[sub] = idx;
			}
			return s;
		}

		private void Build(Group g)
		{
			int count = g.vertexCount;
			Vector3[] pos = new Vector3[count];
			Vector3[] nor = new Vector3[count];
			Vector2[] uv = g.gradient ? null : new Vector2[count];
			Vector2[] uv3 = g.gradient ? new Vector2[count] : null;
			int indexCount = 0;
			foreach (Part p in g.parts)
			{
				indexCount += p.source.indices[p.sub].Length;
			}
			int[] tris = new int[indexCount];
			Matrix4x4 toLocal = transform.worldToLocalMatrix;
			int vo = 0;
			int io = 0;
			foreach (Part p in g.parts)
			{
				Source s = p.source;
				Matrix4x4 m = toLocal * p.world;
				Matrix4x4 nm = m.inverse.transpose;
				bool hasN = s.normals != null && s.normals.Length == s.vertices.Length;
				bool hasUV = s.uv != null && s.uv.Length == s.vertices.Length;
				bool hasUV3 = s.uv3.Count == s.vertices.Length;
				int[] used = s.used[p.sub];
				for (int j = 0; j < used.Length; j++)
				{
					int vi = used[j];
					Vector3 v = s.vertices[vi];
					int k = vo + j;
					pos[k] = m.MultiplyPoint3x4(v);
					nor[k] = hasN ? nm.MultiplyVector(s.normals[vi]).normalized : Vector3.up;
					if (uv != null)
					{
						uv[k] = hasUV ? s.uv[vi] : Vector2.zero;
					}
					if (uv3 != null)
					{
						float t;
						if (g.meshGradient)
						{
							t = hasUV3 ? s.uv3[vi].x : 0f;
						}
						else if (g.worldSpace)
						{
							t = Vector3.Dot(p.world.MultiplyPoint3x4(v), g.direction);
						}
						else
						{
							t = Vector3.Dot(v, g.direction);
						}
						uv3[k] = new Vector2(t, 0f);
					}
				}
				int[] idx = s.indices[p.sub];
				bool flip = m.determinant < 0f;
				for (int j = 0; j < idx.Length; j += 3)
				{
					tris[io + j] = idx[j] + vo;
					tris[io + j + 1] = idx[j + (flip ? 2 : 1)] + vo;
					tris[io + j + 2] = idx[j + (flip ? 1 : 2)] + vo;
				}
				io += idx.Length;
				vo += used.Length;
			}
			Vector3[] verts = pos;
			Vector3[] norms = nor;
			Vector2[] uvs = uv;
			Vector2[] uv3s = uv3;
			int[] indices = tris;
			List<float> open = null;
			bool refine = (ambientOcclusion && aoEdge > 0f) || lightEdge > 0f;
			if (refine || staticAO)
			{
				List<Vector3> P = new List<Vector3>(pos);
				List<Vector3> N = new List<Vector3>(nor);
				List<Vector2> UV = (uv != null) ? new List<Vector2>(uv) : new List<Vector2>();
				List<Vector2> UV3 = (uv3 != null) ? new List<Vector2>(uv3) : new List<Vector2>();
				List<int> T = new List<int>(tris);
				Dictionary<long, int> mids = new Dictionary<long, int>();
				if (refine)
				{
					Dense(P, N, UV, UV3, T, transform.localToWorldMatrix, mids, null);
				}
				if (staticAO)
				{
					open = new List<float>();
					Shade(P, N, 0, open);
					if (aoEdge > 0f)
					{
						Dense(P, N, UV, UV3, T, transform.localToWorldMatrix, mids, open);
					}
				}
				verts = P.ToArray();
				norms = N.ToArray();
				uvs = (uv != null) ? UV.ToArray() : null;
				uv3s = (uv3 != null) ? UV3.ToArray() : null;
				indices = T.ToArray();
			}
			Mesh mesh = new Mesh();
			mesh.name = base.name + " " + g.material.name;
			if (previewing)
			{
				mesh.hideFlags = HideFlags.HideAndDontSave;
			}
			mesh.indexFormat = (verts.Length >= 65536) ? IndexFormat.UInt32 : IndexFormat.UInt16;
			mesh.vertices = verts;
			mesh.normals = norms;
			if (uvs != null)
			{
				mesh.uv = uvs;
			}
			if (uv3s != null)
			{
				mesh.SetUVs(3, uv3s);
			}
			mesh.SetTriangles(indices, 0, true);
			if (staticAO)
			{
				pending.Add(new Pending
				{
					mesh = mesh,
					pos = verts,
					nor = norms,
					open = open.ToArray()
				});
				g.material.SetFloat(StaticId, 1f);
			}
			else
			{
				mesh.UploadMeshData(true);
			}
			GameObject go = new GameObject(mesh.name);
			if (previewing)
			{
				go.hideFlags = HideFlags.HideAndDontSave;
			}
			go.layer = g.first.gameObject.layer;
			go.transform.SetParent(transform, false);
			go.AddComponent<MeshFilter>().sharedMesh = mesh;
			MeshRenderer mr = go.AddComponent<MeshRenderer>();
			mr.sharedMaterial = g.material;
			mr.shadowCastingMode = castShadows ? g.first.shadowCastingMode : ShadowCastingMode.Off;
			mr.receiveShadows = receiveShadows && g.first.receiveShadows;
			mr.lightProbeUsage = g.first.lightProbeUsage;
			mr.reflectionProbeUsage = g.first.reflectionProbeUsage;
			baked.Add(go);
		}

		private void Thicken(MeshFilter mf, Matrix4x4 world)
		{
			Mesh src = mf.sharedMesh;
			List<Vector3> pos = new List<Vector3>();
			List<Vector3> nor = new List<Vector3>();
			List<Vector2> uv = new List<Vector2>();
			List<Vector2> uv3 = new List<Vector2>();
			src.GetVertices(pos);
			src.GetNormals(nor);
			src.GetUVs(0, uv);
			src.GetUVs(3, uv3);
			Dictionary<long, int> mids = new Dictionary<long, int>();
			List<int>[] subs = new List<int>[src.subMeshCount];
			for (int i = 0; i < subs.Length; i++)
			{
				subs[i] = new List<int>(src.GetTriangles(i));
				Dense(pos, nor, uv, uv3, subs[i], world, mids, null);
			}
			Mesh mesh = new Mesh();
			mesh.name = src.name + " AO";
			mesh.indexFormat = (pos.Count >= 65536) ? IndexFormat.UInt32 : IndexFormat.UInt16;
			mesh.SetVertices(pos);
			if (nor.Count == pos.Count)
			{
				mesh.SetNormals(nor);
			}
			if (uv.Count == pos.Count)
			{
				mesh.SetUVs(0, uv);
			}
			if (uv3.Count == pos.Count)
			{
				mesh.SetUVs(3, uv3);
			}
			mesh.subMeshCount = subs.Length;
			for (int i = 0; i < subs.Length; i++)
			{
				mesh.SetTriangles(subs[i], i, false);
			}
			mesh.RecalculateBounds();
			mf.sharedMesh = mesh;
			thick.Add(mesh);
			Debug.Log("MeshBake " + base.name + ": " + mf.name + " " + src.vertexCount + " -> " + pos.Count + " vertices for AO");
		}

		private void Dense(List<Vector3> pos, List<Vector3> nor, List<Vector2> uv, List<Vector2> uv3, List<int> tris, Matrix4x4 world, Dictionary<long, int> mids, List<float> open)
		{
			bool hasN = nor.Count == pos.Count;
			bool hasUV = uv.Count == pos.Count;
			bool hasUV3 = uv3.Count == pos.Count;
			Matrix4x4 nm = world.inverse.transpose;
			float ao = ambientOcclusion ? (aoEdge * aoEdge) : 0f;
			float light = lightEdge * lightEdge;
			int[] v = new int[3];
			int[] m = new int[3];
			List<int> next = new List<int>();
			for (int pass = 0; pass < 32; pass++)
			{
				int before = pos.Count;
				int splits = 0;
				for (int t = 0; t < tris.Count; t += 3)
				{
					if (pos.Count >= 200000)
					{
						break;
					}
					Vector3 a = world.MultiplyPoint3x4(pos[tris[t]]);
					Vector3 b = world.MultiplyPoint3x4(pos[tris[t + 1]]);
					Vector3 c = world.MultiplyPoint3x4(pos[tris[t + 2]]);
					float limit;
					if (open != null)
					{
						float oa = Mathf.Max(open[tris[t]], 0f);
						float ob = Mathf.Max(open[tris[t + 1]], 0f);
						float oc = Mathf.Max(open[tris[t + 2]], 0f);
						if (Mathf.Max(oa, Mathf.Max(ob, oc)) - Mathf.Min(oa, Mathf.Min(ob, oc)) <= RefineGap)
						{
							continue;
						}
						limit = aoEdge * aoEdge;
					}
					else
					{
						limit = light;
						if (ao > 0f && Mathf.Max(a.y, Mathf.Max(b.y, c.y)) >= aoFrom && Mathf.Min(a.y, Mathf.Min(b.y, c.y)) <= aoTo)
						{
							Vector3 n = hasN ? nm.MultiplyVector(nor[tris[t]] + nor[tris[t + 1]] + nor[tris[t + 2]]) : Vector3.Cross(b - a, c - a);
							limit = (n.normalized.y >= 0.5f) ? ao : light;
						}
					}
					if (limit <= 0f)
					{
						continue;
					}
					float ab = (a - b).sqrMagnitude;
					float bc = (b - c).sqrMagnitude;
					float ca = (c - a).sqrMagnitude;
					int e = (ab >= bc && ab >= ca) ? 0 : ((bc >= ca) ? 1 : 2);
					if (Mathf.Max(ab, Mathf.Max(bc, ca)) <= limit)
					{
						continue;
					}
					int i0 = tris[t + e];
					int i1 = tris[t + (e + 1) % 3];
					long key = Key(i0, i1);
					if (mids.ContainsKey(key))
					{
						continue;
					}
					mids[key] = pos.Count;
					pos.Add((pos[i0] + pos[i1]) * 0.5f);
					if (hasN)
					{
						nor.Add((nor[i0] + nor[i1]).normalized);
					}
					if (hasUV)
					{
						uv.Add((uv[i0] + uv[i1]) * 0.5f);
					}
					if (hasUV3)
					{
						uv3.Add((uv3[i0] + uv3[i1]) * 0.5f);
					}
					splits++;
				}
				next.Clear();
				for (int t = 0; t < tris.Count; t += 3)
				{
					for (int k = 0; k < 3; k++)
					{
						v[k] = tris[t + k];
					}
					int split = 0;
					int lone = 0;
					for (int k = 0; k < 3; k++)
					{
						m[k] = mids.TryGetValue(Key(v[k], v[(k + 1) % 3]), out int mid) ? mid : -1;
						if (m[k] < 0)
						{
							lone = k;
						}
						else
						{
							split++;
						}
					}
					switch (split)
					{
					case 3:
						Tri(next, v[0], m[0], m[2]);
						Tri(next, m[0], v[1], m[1]);
						Tri(next, m[2], m[1], v[2]);
						Tri(next, m[0], m[1], m[2]);
						break;
					case 1:
					{
						int e = (m[0] >= 0) ? 0 : ((m[1] >= 0) ? 1 : 2);
						Tri(next, v[e], m[e], v[(e + 2) % 3]);
						Tri(next, m[e], v[(e + 1) % 3], v[(e + 2) % 3]);
						break;
					}
					case 0:
						Tri(next, v[0], v[1], v[2]);
						break;
					default:
					{
						int i = (lone + 1) % 3;
						int j = (i + 1) % 3;
						Tri(next, m[i], v[j], m[j]);
						Tri(next, v[i], m[i], m[j]);
						Tri(next, v[i], m[j], v[(i + 2) % 3]);
						break;
					}
					}
				}
				tris.Clear();
				tris.AddRange(next);
				if (open != null && splits > 0)
				{
					Shade(pos, nor, before, open);
				}
				if (splits == 0)
				{
					break;
				}
			}
		}

		private void Shade(List<Vector3> pos, List<Vector3> nor, int first, List<float> open)
		{
			Matrix4x4 world = transform.localToWorldMatrix;
			Matrix4x4 nm = world.inverse.transpose;
			int count = pos.Count - first;
			int rays = Rays.Length;
			int batch = Mathf.Max(1, Mathf.Min(count, 4096));
			NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(batch * rays, Allocator.TempJob);
			NativeArray<RaycastHit> hits = new NativeArray<RaycastHit>(batch * rays, Allocator.TempJob);
			QueryParameters query = new QueryParameters(-5, false, QueryTriggerInteraction.Ignore, true);
			for (int start = 0; start < count; start += batch)
			{
				int n = Mathf.Min(batch, count - start);
				for (int j = 0; j < n; j++)
				{
					int i = first + start + j;
					Vector3 normal = nm.MultiplyVector(nor[i]).normalized;
					Vector3 p = world.MultiplyPoint3x4(pos[i]);
					Quaternion q = Quaternion.FromToRotation(Vector3.up, normal);
					Vector3 origin = p + normal * 0.02f;
					for (int r = 0; r < rays; r++)
					{
						commands[j * rays + r] = new RaycastCommand(origin, q * Rays[r], query, staticRange);
					}
				}
				RaycastCommand.ScheduleBatch(commands.GetSubArray(0, n * rays), hits.GetSubArray(0, n * rays), 64, 1).Complete();
				for (int j = 0; j < n; j++)
				{
					float occlusion = 0f;
					int backfaces = 0;
					for (int r = 0; r < rays; r++)
					{
						RaycastHit hit = hits[j * rays + r];
						if (hit.colliderInstanceID == 0 || !solid.Contains(hit.colliderInstanceID))
						{
							continue;
						}
						occlusion += 1f - hit.distance / staticRange;
						if (Vector3.Dot(hit.normal, commands[j * rays + r].direction) > 0f)
						{
							backfaces++;
						}
					}
					open.Add((backfaces * 2 > rays) ? (-1f) : Mathf.Clamp01(1f - staticStrength * 2f * occlusion / rays));
				}
			}
			commands.Dispose();
			hits.Dispose();
		}

		private void Soften()
		{
			Matrix4x4 world = transform.localToWorldMatrix;
			Matrix4x4 nm = world.inverse.transpose;
			Dictionary<Vector3Int, List<int>> cells = new Dictionary<Vector3Int, List<int>>();
			List<Vector3> P = new List<Vector3>();
			List<Vector3> N = new List<Vector3>();
			List<float> O = new List<float>();
			foreach (Pending p in pending)
			{
				for (int i = 0; i < p.pos.Length; i++)
				{
					Vector3 w = world.MultiplyPoint3x4(p.pos[i]);
					Vector3Int cell = Vector3Int.FloorToInt(w / Blur);
					if (!cells.TryGetValue(cell, out List<int> list))
					{
						list = new List<int>();
						cells[cell] = list;
					}
					list.Add(P.Count);
					P.Add(w);
					N.Add(nm.MultiplyVector(p.nor[i]).normalized);
					O.Add(p.open[i]);
				}
			}
			float[] result = new float[P.Count];
			List<int> near = new List<int>();
			foreach (KeyValuePair<Vector3Int, List<int>> pair in cells)
			{
				near.Clear();
				for (int x = -1; x <= 1; x++)
				{
					for (int y = -1; y <= 1; y++)
					{
						for (int z = -1; z <= 1; z++)
						{
							if (!cells.TryGetValue(pair.Key + new Vector3Int(x, y, z), out List<int> list))
							{
								continue;
							}
							foreach (int j in list)
							{
								if (O[j] >= 0f)
								{
									near.Add(j);
								}
							}
						}
					}
				}
				foreach (int i in pair.Value)
				{
					float sum = 0f;
					float weight = 0f;
					for (int k = 0; k < near.Count; k++)
					{
						int j = near[k];
						float d = (P[j] - P[i]).sqrMagnitude;
						if (d >= Blur * Blur || Vector3.Dot(N[i], N[j]) < 0.7f)
						{
							continue;
						}
						float w = 1f - Mathf.Sqrt(d) / Blur;
						weight += w;
						sum += w * O[j];
					}
					result[i] = (weight > 0f) ? (sum / weight) : Mathf.Max(O[i], 0f);
				}
			}
			int offset = 0;
			foreach (Pending p in pending)
			{
				Color32[] colors = new Color32[p.pos.Length];
				for (int i = 0; i < colors.Length; i++)
				{
					colors[i] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Mathf.RoundToInt(result[offset + i] * 255f));
				}
				offset += colors.Length;
				p.mesh.colors32 = colors;
				p.mesh.UploadMeshData(true);
			}
			pending.Clear();
		}

		private static Vector3[] Spread(int count)
		{
			Vector3[] rays = new Vector3[count];
			for (int i = 0; i < count; i++)
			{
				float angle = i * 2.3999631f;
				float r = Mathf.Sqrt((i + 0.5f) / count);
				rays[i] = new Vector3(Mathf.Cos(angle) * r, Mathf.Sqrt(1f - r * r), Mathf.Sin(angle) * r);
			}
			return rays;
		}

		private static long Key(int a, int b)
		{
			return (a < b) ? (((long)a << 32) | (uint)b) : (((long)b << 32) | (uint)a);
		}

		private static void Tri(List<int> list, int a, int b, int c)
		{
			list.Add(a);
			list.Add(b);
			list.Add(c);
		}
	}
}
