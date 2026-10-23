using UnityEngine;

namespace Ragdoll
{
	[ExecuteAlways]
	public class Paint : MonoBehaviour
	{
		[Header("Paint")]
		public bool shuffle;

		public int pick = -1;

		public Renderer[] parts;

		public Material second;

		private static readonly Color[] palette = new Color[10]
		{
			new Color(1f, 0.345f, 0.196f, 1f),
			new Color(0.9f, 0.2f, 0.2f, 1f),
			new Color(0.2f, 0.45f, 0.95f, 1f),
			new Color(0.25f, 0.75f, 0.35f, 1f),
			new Color(1f, 0.8f, 0.2f, 1f),
			new Color(0.15f, 0.75f, 0.75f, 1f),
			new Color(0.6f, 0.35f, 0.9f, 1f),
			new Color(0.62f, 0.15f, 0.2f, 1f),
			new Color(0.62f, 0.64f, 0.66f, 1f),
			new Color(1f, 0.45f, 0.65f, 1f)
		};

		private int shown = -1;

		private int local = -1;

		private MaterialPropertyBlock block;

		private void OnEnable()
		{
			shown = -1;
			if (!shuffle || !Application.isPlaying)
			{
				Show(Fixed());
			}
		}

		private void Update()
		{
			if (!shuffle || !Application.isPlaying)
			{
				return;
			}
			NetRope rope = null;
			if (parts != null && parts.Length != 0 && (bool)parts[0])
			{
				rope = parts[0].GetComponent<NetRope>();
			}
			int index;
			if ((bool)rope && (bool)rope.Object && rope.Object.IsValid)
			{
				if (rope.Tint < 1)
				{
					return;
				}
				index = rope.Tint % palette.Length;
			}
			else
			{
				if (local < 0)
				{
					local = Seed((GameManager.match != null) ? GameManager.match.round : 0);
				}
				index = local;
			}
			if (index != shown)
			{
				Show(index);
			}
		}

		private int Fixed()
		{
			if (pick >= 0)
			{
				return pick % palette.Length;
			}
			Vector3 p = base.transform.position;
			uint h = 2166136261u;
			h = (h ^ (uint)Mathf.RoundToInt(p.x * 2f)) * 16777619;
			h = (h ^ (uint)Mathf.RoundToInt(p.z * 2f)) * 16777619;
			return (int)(h % (uint)palette.Length);
		}

		private int Seed(int round)
		{
			Vector3 p = base.transform.position;
			uint h = 2166136261u;
			h = (h ^ (uint)Mathf.RoundToInt(p.x * 2f)) * 16777619;
			h = (h ^ (uint)Mathf.RoundToInt(p.z * 2f)) * 16777619;
			h = (h ^ (uint)round) * 16777619;
			h ^= h >> 13;
			return (int)(h % (uint)palette.Length);
		}

		private void Show(int index)
		{
			shown = index;
			if (parts == null)
			{
				return;
			}
			if (block == null)
			{
				block = new MaterialPropertyBlock();
			}
			Color main = palette[index];
			Color alt = palette[(index * 7 + 3) % palette.Length];
			Renderer[] array = parts;
			foreach (Renderer r in array)
			{
				if (!r)
				{
					continue;
				}
				Material[] mats = r.sharedMaterials;
				for (int i = 0; i < mats.Length && (i == 0 || (bool)second); i++)
				{
					Color c = ((bool)second && mats[i] == second) ? alt : main;
					block.SetColor("_TopColor", c);
					block.SetColor("_BottomColor", new Color(c.r * 0.66f, c.g * 0.66f, c.b * 0.66f, 1f));
					r.SetPropertyBlock(block, i);
				}
			}
		}
	}
}
