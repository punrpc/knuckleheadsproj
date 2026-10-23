using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Island : MonoBehaviour
{
	[Header("Shape Stuff")]
	public List<Vector2> points = new List<Vector2>();

	[Range(1f, 24f)]
	public int resolution = 8;

	public float depth = 3f;

	[Range(0f, 1f)]
	public float bottomTaper;

	[Header("Look Stuff")]
	public Material material;

	public float textureSize = 4f;

	public bool smoothSides = true;

	[Header("Color Stuff")]
	public bool customColors;

	public Color grassColor = new Color(0.24f, 0.84f, 0.55f, 1f);

	public Color dirtColor = new Color(1f, 0.55f, 0.12f, 1f);

	[Header("Crease Stuff")]
	public bool grassSkirt;

	public float skirtBaseY;

	public bool contactSkirt = true;

	public float contactMargin = 0.15f;

	public float contactInset = 0.05f;

	public Transform water;

	public float seaLevel = -9999f;

	[Header("Snap Stuff")]
	public bool snapToGrid;

	public float gridSize = 0.5f;

	private static readonly int grassId = Shader.PropertyToID("_GrassColor");

	private static readonly int dirtId = Shader.PropertyToID("_DirtColor");

	private static readonly int skirtId = Shader.PropertyToID("_SkirtBaseY");

	private MeshFilter filter;

	private MeshRenderer rend;

	private MeshCollider col;

	private MaterialPropertyBlock block;

	private Mesh mesh;

	private static readonly List<Island> others = new List<Island>();

	private static float seaNow = -9999f;

	private List<Vector2> outline = new List<Vector2>();

	private List<Vector2> bottomRing = new List<Vector2>();

	private List<Vector3> quadN = new List<Vector3>();

	private List<float> ringDist = new List<float>();

	private List<int> polyTris = new List<int>();

	private List<int> left = new List<int>();

	private List<Vector3> verts = new List<Vector3>();

	private List<Vector3> normals = new List<Vector3>();

	private List<Vector2> uv = new List<Vector2>();

	private List<Vector2> uv2 = new List<Vector2>();

	private List<Vector2> uv3 = new List<Vector2>();

	private List<Vector2> uv4 = new List<Vector2>();

	private List<Vector2> uv5 = new List<Vector2>();

	private readonly List<Vector3> creases = new List<Vector3>();

	public float creaseReach = 3f;

	private List<int> tris = new List<int>();

	public List<Vector2> Outline => outline;

	public float TopY => transform.position.y;

	public float Depth => depth;

	private void OnEnable()
	{
		Rebuild();
	}

	private void OnValidate()
	{
		// TODO(recon): body is empty in the player build; any editor-time rebuild scheduling was most likely inside #if UNITY_EDITOR and is not recoverable (token 0x6000005)
	}

	private void OnDestroy()
	{
		if (!Application.isPlaying)
		{
			DestroyImmediate(mesh);
		}
		else
		{
			Destroy(mesh);
		}
	}

	public void ResetShape()
	{
		points.Clear();
		for (int i = 0; i < 6; i++)
		{
			float a = (float)i / 6f * Mathf.PI * 2f;
			points.Add(new Vector2(Mathf.Cos(a) * 4f, Mathf.Sin(a) * 4f));
		}
	}

	public Vector2 Spline(int seg, float t)
	{
		int n = points.Count;
		Vector2 p0 = points[(seg + n - 1) % n];
		Vector2 p1 = points[seg];
		Vector2 p2 = points[(seg + 1) % n];
		Vector2 p3 = points[(seg + 2) % n];
		float t2 = t * t;
		float t3 = t2 * t;
		return 0.5f * (2f * p1 + (p2 - p0) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 + (3f * p1 - p0 - 3f * p2 + p3) * t3);
	}

	public void Rebuild()
	{
		if (points.Count <= 2)
		{
			ResetShape();
		}
		if (!filter)
		{
			filter = GetComponent<MeshFilter>();
			rend = GetComponent<MeshRenderer>();
			col = GetComponent<MeshCollider>();
		}
		if (!mesh)
		{
			mesh = new Mesh();
			mesh.name = "Island";
			mesh.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
		}
		if (snapToGrid)
		{
			gridSize = Mathf.Max(gridSize, 0.05f);
			for (int i = 0; i < points.Count; i++)
			{
				points[i] = new Vector2(Mathf.Round(points[i].x / gridSize), Mathf.Round(points[i].y / gridSize)) * gridSize;
			}
		}
		SampleOutline();
		Triangulate();
		GatherOthers();
		verts.Clear();
		normals.Clear();
		uv.Clear();
		uv2.Clear();
		uv3.Clear();
		uv4.Clear();
		uv5.Clear();
		tris.Clear();
		Cap(outline, 0f, true, -1f);
		Wall();
		Cap(bottomRing, -depth, false, 1000f);
		mesh.Clear();
		mesh.SetVertices(verts);
		mesh.SetNormals(normals);
		mesh.SetUVs(0, uv);
		mesh.SetUVs(1, uv2);
		mesh.SetUVs(2, uv3);
		mesh.SetUVs(3, uv4);
		mesh.SetUVs(4, uv5);
		mesh.SetTriangles(tris, 0);
		mesh.RecalculateBounds();
		filter.sharedMesh = mesh;
		rend.sharedMaterial = material;
		if (customColors || grassSkirt)
		{
			if (block == null)
			{
				block = new MaterialPropertyBlock();
			}
			if (customColors)
			{
				block.SetColor(grassId, grassColor);
				block.SetColor(dirtId, dirtColor);
			}
			block.SetFloat(skirtId, grassSkirt ? skirtBaseY : -9999f);
			rend.SetPropertyBlock(block);
		}
		else
		{
			rend.SetPropertyBlock(null);
		}
		col.sharedMesh = null;
		col.sharedMesh = mesh;
	}

	private void GatherOthers()
	{
		others.Clear();
		seaNow = seaLevel;
		if (!water)
		{
			GameObject w = GameObject.Find("Water");
			if (w)
			{
				water = w.transform;
			}
		}
		if (water)
		{
			seaNow = water.position.y;
		}
		if (!contactSkirt)
		{
			return;
		}
		foreach (Island o in Object.FindObjectsByType<Island>(FindObjectsSortMode.None))
		{
			if (!(o == this) && o.outline.Count >= 3)
			{
				others.Add(o);
			}
		}
	}

	private static float InsideDist(List<Vector2> poly, Vector2 p)
	{
		bool inside = false;
		float best = float.MaxValue;
		for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
		{
			Vector2 a = poly[i];
			Vector2 b = poly[j];
			Vector2 d = b - a;
			if (a.y > p.y != b.y > p.y && p.x < a.x + (p.y - a.y) * d.x / d.y)
			{
				inside = !inside;
			}
			float t = (d.sqrMagnitude > 0.0001f) ? Mathf.Clamp01(Vector2.Dot(p - a, d) / d.sqrMagnitude) : 0f;
			best = Mathf.Min((p - (a + d * t)).sqrMagnitude, best);
		}
		return inside ? Mathf.Sqrt(best) : (0f - Mathf.Sqrt(best));
	}

	private static bool InsideBy(List<Vector2> poly, Vector2 p, float inset)
	{
		bool inside = false;
		float best = float.MaxValue;
		for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
		{
			Vector2 a = poly[i];
			Vector2 b = poly[j];
			Vector2 d = b - a;
			if (a.y > p.y != b.y > p.y && p.x < a.x + (p.y - a.y) * d.x / d.y)
			{
				inside = !inside;
			}
			float t = (d.sqrMagnitude > 0.0001f) ? Mathf.Clamp01(Vector2.Dot(p - a, d) / d.sqrMagnitude) : 0f;
			best = Mathf.Min((p - (a + d * t)).sqrMagnitude, best);
		}
		return inside && best >= inset * inset;
	}

	private static bool InsidePolygon(List<Vector2> poly, Vector2 p, float margin)
	{
		bool inside = false;
		float best = float.MaxValue;
		for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
		{
			Vector2 a = poly[i];
			Vector2 b = poly[j];
			Vector2 d = b - a;
			if (a.y > p.y != b.y > p.y && p.x < a.x + (p.y - a.y) * d.x / d.y)
			{
				inside = !inside;
			}
			float t = (d.sqrMagnitude > 0.0001f) ? Mathf.Clamp01(Vector2.Dot(p - a, d) / d.sqrMagnitude) : 0f;
			best = Mathf.Min((p - (a + d * t)).sqrMagnitude, best);
		}
		return inside || best <= margin * margin;
	}

	private Island ContactIsland(Vector2 ringTop, Vector2 ringBottom, out float top, out float inside)
	{
		top = 0f;
		inside = -1f;
		Island best = null;
		for (int i = 0; i < others.Count; i++)
		{
			Island o = others[i];
			if (o.TopY < seaNow || o.TopY > TopY + 0.01f)
			{
				continue;
			}
			float d = Inside(o, WallAt(ringTop, ringBottom, o.TopY));
			if (!(d <= 0f) && (!best || o.TopY > top))
			{
				best = o;
				top = o.TopY;
				inside = d;
			}
		}
		return best;
	}

	private Vector3 WallAt(Vector2 ringTop, Vector2 ringBottom, float y)
	{
		float t = (depth > 0.0001f) ? Mathf.Clamp01((TopY - y) / depth) : 0f;
		Vector2 p = Vector2.Lerp(ringTop, ringBottom, t);
		return transform.TransformPoint(new Vector3(p.x, (0f - t) * depth, p.y));
	}

	private static float Inside(Island o, Vector3 worldPos)
	{
		Vector3 local = o.transform.InverseTransformPoint(worldPos);
		return InsideDist(o.outline, new Vector2(local.x, local.z));
	}

	private void SampleOutline()
	{
		outline.Clear();
		int n = points.Count;
		for (int i = 0; i < n; i++)
		{
			for (int s = 0; s < resolution; s++)
			{
				Vector2 p = Spline(i, (float)s / (float)resolution);
				if (outline.Count > 0 && (p - outline[outline.Count - 1]).sqrMagnitude < 0.0001f)
				{
					continue;
				}
				outline.Add(p);
			}
		}
		int count = outline.Count;
		if (count >= 4 && (outline[0] - outline[count - 1]).sqrMagnitude < 0.0001f)
		{
			outline.RemoveAt(count - 1);
			count--;
		}
		Vector2 center = Vector2.zero;
		float area = 0f;
		for (int i = 0; i < count; i++)
		{
			Vector2 a = outline[i];
			Vector2 b = outline[(i + 1) % count];
			area += a.x * b.y - a.y * b.x;
			center += a;
		}
		if (area > 0f)
		{
			outline.Reverse();
		}
		bottomRing.Clear();
		center /= (float)count;
		for (int i = 0; i < count; i++)
		{
			bottomRing.Add(center + (outline[i] - center) * (1f - bottomTaper));
		}
	}

	private void Triangulate()
	{
		polyTris.Clear();
		left.Clear();
		int count = outline.Count;
		for (int i = 0; i < count; i++)
		{
			left.Add(i);
		}
		while (left.Count > 3)
		{
			int n = left.Count;
			int clip = -1;
			for (int i = 0; i < n; i++)
			{
				int a = left[(i + n - 1) % n];
				int b = left[i];
				int c = left[(i + 1) % n];
				Vector2 pa = outline[a];
				Vector2 pb = outline[b];
				Vector2 pc = outline[c];
				if ((pb.x - pa.x) * (pc.y - pb.y) - (pb.y - pa.y) * (pc.x - pb.x) >= 0f)
				{
					continue;
				}
				bool ear = true;
				for (int k = 0; k < n; k++)
				{
					int v = left[k];
					if (v == c || v == a || v == b)
					{
						continue;
					}
					Vector2 p = outline[v];
					if (!((pb.x - pa.x) * (p.y - pa.y) - (pb.y - pa.y) * (p.x - pa.x) > 0f) && !((pc.x - pb.x) * (p.y - pb.y) - (pc.y - pb.y) * (p.x - pb.x) > 0f) && (pa.x - pc.x) * (p.y - pc.y) - (pa.y - pc.y) * (p.x - pc.x) <= 0f)
					{
						ear = false;
						break;
					}
				}
				if (ear)
				{
					polyTris.Add(a);
					polyTris.Add(b);
					polyTris.Add(c);
					clip = i;
					break;
				}
			}
			if (clip < 0)
			{
				polyTris.Add(left[0]);
				polyTris.Add(left[1]);
				polyTris.Add(left[2]);
				clip = 1;
			}
			left.RemoveAt(clip);
		}
		polyTris.Add(left[0]);
		polyTris.Add(left[1]);
		polyTris.Add(left[2]);
	}

	private void Cap(List<Vector2> ring, float y, bool up, float mask)
	{
		int start = verts.Count;
		Vector3 n = up ? Vector3.up : Vector3.down;
		for (int i = 0; i < ring.Count; i++)
		{
			Vector2 p = ring[i];
			verts.Add(new Vector3(p.x, y, p.y));
			normals.Add(n);
			uv.Add(p / textureSize);
			uv2.Add(new Vector2(mask, 999f));
			uv3.Add(new Vector2(-1f, 0f));
			uv4.Add(new Vector2(-999f, 0f));
			uv5.Add(new Vector2(0f, 0f));
		}
		for (int i = 0; i < polyTris.Count; i += 3)
		{
			tris.Add(polyTris[i] + start);
			tris.Add(polyTris[i + (up ? 1 : 2)] + start);
			tris.Add(polyTris[i + (up ? 2 : 1)] + start);
		}
	}

	private void Wall()
	{
		int n = outline.Count;
		quadN.Clear();
		ringDist.Clear();
		float dist = 0f;
		for (int i = 0; i < n; i++)
		{
			int j = (i + 1) % n;
			Vector3 a = new Vector3(outline[i].x, 0f, outline[i].y);
			Vector3 b = new Vector3(outline[j].x, 0f, outline[j].y);
			Vector3 c = new Vector3(bottomRing[i].x, 0f - depth, bottomRing[i].y);
			quadN.Add(Vector3.Cross(c - a, b - a).normalized);
			ringDist.Add(dist);
			dist += Vector2.Distance(outline[j], outline[i]);
		}
		ringDist.Add(dist);
		Creases(n);
		for (int i = 0; i < n; i++)
		{
			int j = (i + 1) % n;
			Vector3 q = quadN[i];
			Vector3 na = smoothSides ? (q + quadN[(i + n - 1) % n]).normalized : q;
			Vector3 nb = smoothSides ? (q + quadN[j]).normalized : q;
			float ti;
			float di;
			Island ri = ContactIsland(outline[i], bottomRing[i], out ti, out di);
			float tj;
			float dj;
			Island rj = ContactIsland(outline[j], bottomRing[j], out tj, out dj);
			if ((bool)ri && (bool)rj && ri != rj && Mathf.Abs(ti - tj) > 0.01f)
			{
				Island high = (ti > tj) ? ri : rj;
				float s = Crossing(high, i, j, high.TopY, ti > tj);
				Quad(i, j, 0f, s, na, nb, ri, ri);
				Quad(i, j, s, 1f, na, nb, rj, rj);
				continue;
			}
			Island qi = ri;
			Island qj = rj;
			if ((bool)ri && !rj)
			{
				qj = ri;
			}
			else if ((bool)rj && !ri)
			{
				qi = rj;
			}
			Quad(i, j, 0f, 1f, na, nb, qi, qj);
		}
	}

	private float Crossing(Island o, int i, int j, float y, bool insideAtI)
	{
		Vector3 a = WallAt(outline[i], bottomRing[i], y);
		Vector3 b = WallAt(outline[j], bottomRing[j], y);
		float lo = 0f;
		float hi = 1f;
		for (int k = 0; k < 12; k++)
		{
			float mid = (lo + hi) * 0.5f;
			if (Inside(o, Vector3.Lerp(a, b, mid)) > 0f == insideAtI)
			{
				lo = mid;
			}
			else
			{
				hi = mid;
			}
		}
		return (lo + hi) * 0.5f;
	}

	private Island LedgeUnder(Vector3 worldPos, float below, Island not)
	{
		Island best = null;
		for (int i = 0; i < others.Count; i++)
		{
			Island o = others[i];
			if (!(o == not) && !(o.TopY < seaNow) && !(o.TopY > below - 0.01f) && !(Inside(o, worldPos) <= 0f) && (!best || o.TopY > best.TopY))
			{
				best = o;
			}
		}
		return best;
	}

	private void Creases(int m)
	{
		creases.Clear();
		for (int k = 0; k < others.Count; k++)
		{
			Island o = others[k];
			if (o.TopY < seaNow)
			{
				continue;
			}
			float y = Mathf.Min(TopY, o.TopY);
			if (y < TopY - depth + 0.01f || y < o.TopY - o.depth + 0.01f)
			{
				continue;
			}
			for (int i = 0; i < m; i++)
			{
				int j = (i + 1) % m;
				float di = Inside(o, WallAt(outline[i], bottomRing[i], y));
				float dj = Inside(o, WallAt(outline[j], bottomRing[j], y));
				if (di > 0f == dj > 0f)
				{
					continue;
				}
				float s = Crossing(o, i, j, y, di > 0f);
				Island ledge = LedgeUnder(Vector3.Lerp(WallAt(outline[i], bottomRing[i], y), WallAt(outline[j], bottomRing[j], y), s), y, o);
				if ((bool)ledge)
				{
					creases.Add(new Vector3(Mathf.Lerp(ringDist[i], ringDist[i + 1], s), y, ledge.TopY));
				}
			}
		}
	}

	private Vector3 NearCrease(float along)
	{
		float total = ringDist[ringDist.Count - 1];
		Vector3 best = new Vector3(-999f, 0f, 0f);
		float bestDist = creaseReach;
		for (int i = 0; i < creases.Count; i++)
		{
			float at = creases[i].x;
			float[] wraps = new float[3] { 0f, total, 0f - total };
			foreach (float wrap in wraps)
			{
				float d = Mathf.Abs(along - (at + wrap));
				if (d < bestDist)
				{
					bestDist = d;
					best = new Vector3(at + wrap, creases[i].y, creases[i].z);
				}
			}
		}
		return best;
	}

	private void Quad(int i, int j, float s0, float s1, Vector3 na, Vector3 nb, Island ri, Island rj)
	{
		float d0 = Mathf.Lerp(ringDist[i], ringDist[i + 1], s0);
		float d1 = Mathf.Lerp(ringDist[i], ringDist[i + 1], s1);
		Vector2 t0 = Vector2.Lerp(outline[i], outline[j], s0);
		Vector2 t1 = Vector2.Lerp(outline[i], outline[j], s1);
		Vector2 b0 = Vector2.Lerp(bottomRing[i], bottomRing[j], s0);
		Vector2 b1 = Vector2.Lerp(bottomRing[i], bottomRing[j], s1);
		Vector3 n0 = Vector3.Slerp(na, nb, s0);
		Vector3 n1 = Vector3.Slerp(na, nb, s1);
		Vector3 p0 = new Vector3(t0.x, 0f, t0.y);
		Vector3 p1 = new Vector3(t1.x, 0f, t1.y);
		Vector3 p2 = new Vector3(b1.x, 0f - depth, b1.y);
		Vector3 p3 = new Vector3(b0.x, 0f - depth, b0.y);
		int v = verts.Count;
		verts.Add(p0);
		verts.Add(p1);
		verts.Add(p2);
		verts.Add(p3);
		normals.Add(n0);
		normals.Add(n1);
		normals.Add(n1);
		normals.Add(n0);
		uv.Add(new Vector2(d0, 0f) / textureSize);
		uv.Add(new Vector2(d1, 0f) / textureSize);
		uv.Add(new Vector2(d1, 0f - depth) / textureSize);
		uv.Add(new Vector2(d0, 0f - depth) / textureSize);
		float yi = ri ? ri.TopY : 0f;
		float yj = rj ? rj.TopY : 0f;
		float ci = ri ? (Inside(ri, WallAt(t0, b0, yi)) - contactInset) : (-1f);
		float cj = rj ? (Inside(rj, WallAt(t1, b1, yj)) - contactInset) : (-1f);
		float hi = ri ? (transform.TransformPoint(p0).y - yi) : 999f;
		float hj = rj ? (transform.TransformPoint(p1).y - yj) : 999f;
		uv2.Add(new Vector2(0f, hi));
		uv2.Add(new Vector2(0f, hj));
		uv2.Add(new Vector2(depth, rj ? (transform.TransformPoint(p2).y - yj) : 999f));
		uv2.Add(new Vector2(depth, ri ? (transform.TransformPoint(p3).y - yi) : 999f));
		uv3.Add(new Vector2(ci, hi));
		uv3.Add(new Vector2(cj, hj));
		uv3.Add(new Vector2(cj, hj));
		uv3.Add(new Vector2(ci, hi));
		Vector3 crease = NearCrease((d0 + d1) * 0.5f);
		uv4.Add(crease);
		uv4.Add(crease);
		uv4.Add(crease);
		uv4.Add(crease);
		uv5.Add(new Vector2(crease.z, d0));
		uv5.Add(new Vector2(crease.z, d1));
		uv5.Add(new Vector2(crease.z, d1));
		uv5.Add(new Vector2(crease.z, d0));
		tris.Add(v);
		tris.Add(v + 3);
		tris.Add(v + 2);
		tris.Add(v);
		tris.Add(v + 2);
		tris.Add(v + 1);
	}
}
