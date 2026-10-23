using UnityEngine;

namespace Ragdoll
{
	[ExecuteAlways]
	[RequireComponent(typeof(MeshFilter))]
	public class CapsuleMesh : MonoBehaviour
	{
		[Tooltip("Faces around the tube.")]
		[Header("Mesh")]
		[Range(3f, 64f)]
		public int sides = 20;

		[Tooltip("Rings in each rounded cap.")]
		[Range(1f, 16f)]
		public int capRings = 5;

		[Tooltip("Texture repeats per world metre along the length.")]
		public float uvPerMetre = 1f;

		[Tooltip("No collider: build from these instead (world units).")]
		public float radius = 0.06f;

		public float height = 2f;

		[Tooltip("0 = X, 1 = Y, 2 = Z. Ignored when there is a collider.")]
		[Range(0f, 2f)]
		public int direction = 1;

		private MeshFilter filter;

		private CapsuleCollider col;

		private Mesh mesh;

		private Vector4 builtShape;

		private Vector3 builtScale;

		private Vector3 builtCentre;

		private int builtSides;

		private int builtRings;

		private int builtDir;

		private float builtUv;

		private void OnEnable()
		{
			Rebuild();
		}

		private void OnValidate()
		{
			// TODO(recon): body is empty in the player build; any editor-time rebuild scheduling was most likely inside #if UNITY_EDITOR and is not recoverable (token 0x60005F0)
		}

		private void Update()
		{
			if (Changed())
			{
				Rebuild();
			}
		}

		private bool Changed()
		{
			Shape(out Vector4 shape, out Vector3 centre, out int dir);
			return shape != builtShape || centre != builtCentre || dir != builtDir || base.transform.lossyScale != builtScale || sides != builtSides || capRings != builtRings || uvPerMetre != builtUv || !mesh;
		}

		private void Shape(out Vector4 shape, out Vector3 centre, out int dir)
		{
			Vector3 scale = base.transform.lossyScale;
			if (!col)
			{
				col = GetComponent<CapsuleCollider>();
			}
			float r;
			float h;
			if ((bool)col)
			{
				dir = col.direction;
				centre = col.center;
				float a;
				float b;
				float along;
				if (dir == 0)
				{
					a = Mathf.Abs(scale.y);
					b = Mathf.Abs(scale.z);
					along = Mathf.Abs(scale.x);
				}
				else if (dir == 1)
				{
					a = Mathf.Abs(scale.x);
					b = Mathf.Abs(scale.z);
					along = Mathf.Abs(scale.y);
				}
				else
				{
					a = Mathf.Abs(scale.x);
					b = Mathf.Abs(scale.y);
					along = Mathf.Abs(scale.z);
				}
				r = Mathf.Max(a, b) * col.radius;
				h = along * col.height;
			}
			else
			{
				dir = direction;
				centre = Vector3.zero;
				r = radius;
				h = height;
			}
			shape = new Vector4(r, Mathf.Max(h, r * 2f), 0f, 0f);
		}

		public void Rebuild()
		{
			if (!filter)
			{
				filter = GetComponent<MeshFilter>();
			}
			if (!filter)
			{
				return;
			}
			Shape(out Vector4 shape, out Vector3 centre, out int dir);
			Vector3 scale = base.transform.lossyScale;
			float r = shape.x;
			if (r <= 0.0001f || shape.y <= 0.0001f)
			{
				return;
			}
			Vector3 axis;
			Vector3 side;
			switch (dir)
			{
			case 0:
				axis = Vector3.right;
				side = Vector3.up;
				break;
			case 1:
				axis = Vector3.up;
				side = Vector3.right;
				break;
			default:
				axis = Vector3.forward;
				side = Vector3.right;
				break;
			}
			Vector3 other = Vector3.Cross(axis, side);
			float half = shape.y * 0.5f;
			float body = Mathf.Max(half - r, 0f);
			int ring = sides + 1;
			int rows = capRings * 2 + 2;
			Vector3[] verts = new Vector3[rows * ring];
			Vector3[] normals = new Vector3[rows * ring];
			Vector2[] uv = new Vector2[rows * ring];
			int v = 0;
			for (int i = 0; i < rows; i++)
			{
				bool top = i > capRings;
				float angle = (float)(top ? (i - capRings - 1) : i) * (Mathf.PI / 2f) / (float)capRings - (top ? 0f : (Mathf.PI / 2f));
				float cos = Mathf.Cos(angle);
				float sin = Mathf.Sin(angle);
				float along = (top ? body : (0f - body)) + r * sin;
				for (int j = 0; j <= sides; j++)
				{
					float t = (float)j * (Mathf.PI * 2f) / (float)sides;
					Vector3 radial = side * Mathf.Cos(t) + other * Mathf.Sin(t);
					Vector3 n = (axis * sin + radial * cos).normalized;
					verts[v + j] = Div(axis * along + radial * (r * cos), scale) + centre;
					normals[v + j] = Vector3.Scale(scale, n).normalized;
					uv[v + j] = new Vector2((float)j / (float)sides, (half + along) * uvPerMetre);
				}
				v += ring;
			}
			int bands = capRings * 2 + 1;
			int[] tris = new int[bands * sides * 6];
			int k = 0;
			for (int b = 0; b < bands; b++)
			{
				for (int j = 0; j < sides; j++)
				{
					int a = b * ring + j;
					int c = a + ring;
					tris[k++] = a;
					tris[k++] = c;
					tris[k++] = a + 1;
					tris[k++] = a + 1;
					tris[k++] = c;
					tris[k++] = c + 1;
				}
			}
			float facing = 0f;
			for (int i = 0; i < tris.Length; i += 3)
			{
				Vector3 p0 = verts[tris[i]];
				Vector3 p1 = verts[tris[i + 1]];
				Vector3 p2 = verts[tris[i + 2]];
				facing += Vector3.Dot(Vector3.Cross(p1 - p0, p2 - p0), normals[tris[i]]);
			}
			if (facing < 0f)
			{
				for (int i = 0; i < tris.Length; i += 3)
				{
					int swap = tris[i + 1];
					tris[i + 1] = tris[i + 2];
					tris[i + 2] = swap;
				}
			}
			if (!mesh)
			{
				mesh = new Mesh();
				mesh.name = "Capsule (collider)";
				mesh.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
			}
			mesh.Clear();
			mesh.vertices = verts;
			mesh.normals = normals;
			mesh.uv = uv;
			mesh.triangles = tris;
			mesh.RecalculateBounds();
			filter.sharedMesh = mesh;
			builtShape = shape;
			builtCentre = centre;
			builtScale = scale;
			builtDir = dir;
			builtSides = sides;
			builtRings = capRings;
			builtUv = uvPerMetre;
		}

		private static Vector3 Div(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x / ((Mathf.Abs(b.x) < 1E-05f) ? 1E-05f : b.x), a.y / ((Mathf.Abs(b.y) < 1E-05f) ? 1E-05f : b.y), a.z / ((Mathf.Abs(b.z) < 1E-05f) ? 1E-05f : b.z));
		}
	}
}
