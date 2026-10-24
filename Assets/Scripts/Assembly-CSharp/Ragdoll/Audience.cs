using System.Collections.Generic;
using UnityEngine;

namespace Ragdoll
{
	public class Audience : MonoBehaviour
	{
		public Color[] tints;

		public float bounce = 0.08f;

		public Vector2 speed = new Vector2(4f, 6.5f);

		public AudioClip cheer;

		public float cheerVolume = 0.7f;

		private static Audience main;

		private AudioSource cheering;

		private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

		private readonly List<Material> made = new List<Material>();

		private Transform[] fans = new Transform[0];

		private Vector3[] home = new Vector3[0];

		private float[] rate = new float[0];

		private float[] phase = new float[0];

		private void Start()
		{
			main = this;
			cheering = gameObject.AddComponent<AudioSource>();
			cheering.playOnAwake = false;
			cheering.spatialBlend = 0f;
			cheering.volume = cheerVolume;
			Dictionary<string, Material> shared = new Dictionary<string, Material>();
			List<Transform> list = new List<Transform>();
			foreach (Renderer r in GetComponentsInChildren<Renderer>())
			{
				int i = list.Count;
				list.Add(r.transform);
				if (tints == null || tints.Length == 0 || !r.sharedMaterial)
				{
					continue;
				}
				int t = Mathf.Min((int)(Hash(i, 3) * tints.Length), tints.Length - 1);
				string key = r.sharedMaterial.GetInstanceID() + "/" + t;
				if (!shared.TryGetValue(key, out Material m))
				{
					m = new Material(r.sharedMaterial);
					m.SetColor(BaseColorId, tints[t]);
					shared[key] = m;
					made.Add(m);
				}
				r.sharedMaterial = m;
			}
			fans = list.ToArray();
			home = new Vector3[fans.Length];
			rate = new float[fans.Length];
			phase = new float[fans.Length];
			for (int i = 0; i < fans.Length; i++)
			{
				home[i] = fans[i].localPosition;
				rate[i] = Mathf.Lerp(speed.x, speed.y, Hash(i, 1));
				phase[i] = Hash(i, 2) * Mathf.PI * 2f;
			}
		}

		private void Update()
		{
			float time = Time.time;
			for (int i = 0; i < fans.Length; i++)
			{
				if (fans[i])
				{
					fans[i].localPosition = home[i] + Vector3.up * (bounce * (Mathf.Sin(time * rate[i] + phase[i]) * 0.5f + 0.5f));
				}
			}
		}

		public static void Cheer()
		{
			if (main && main.cheer && main.cheering && !main.cheering.isPlaying)
			{
				main.cheering.clip = main.cheer;
				main.cheering.Play();
			}
		}

		private void OnDestroy()
		{
			if (main == this)
			{
				main = null;
			}
			foreach (Material m in made)
			{
				if (m)
				{
					Object.Destroy(m);
				}
			}
		}

		private static float Hash(int i, int salt)
		{
			uint h = (uint)i * 0x165667B1u + (uint)salt * 0x27D4EB2Fu;
			h ^= h >> 13;
			h *= 0x4BF19F61u;
			h ^= h >> 16;
			return (h & 0xFFFFFF) / 16777216f;
		}
	}
}
