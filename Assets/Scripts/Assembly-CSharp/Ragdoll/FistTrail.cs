using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ragdoll
{
	public class FistTrail : MonoBehaviour
	{
		[HideInInspector]
		public Transform[] fists;

		[HideInInspector]
		public Transform body;

		[HideInInspector]
		public Func<int, float> grip;

		[HideInInspector]
		public Func<bool> limp;

		[HideInInspector]
		public Action<int, float> onSwing;

		private TrailRenderer[] trails;

		private Vector3[] last;

		private float[] speed;

		private bool[] swung;

		private bool known;

		private static Material mat;

		private const float From = 2.2f;

		private const float Full = 6f;

		private const float Life = 0.14f;

		private const float Width = 0.12f;

		private const float Smooth = 0.03f;

		private const float Snap = 25f;

		private const float FistAt = 0.4f;

		private const float SwingAt = 4f;

		private const float SwingReset = 2f;

		private static readonly Color Slow = new Color(1f, 1f, 1f, 0.5f);

		private static readonly Color Fast = new Color(1f, 1f, 1f, 0.95f);

		private void LateUpdate()
		{
			if (fists == null || !body)
			{
				return;
			}
			if (trails == null)
			{
				Make();
			}
			float dt = Time.deltaTime;
			if (dt < 0.0001f)
			{
				return;
			}
			bool isLimp = limp != null && limp();
			float blend = 1f - Mathf.Exp((0f - dt) / Smooth);
			for (int i = 0; i < fists.Length; i++)
			{
				TrailRenderer trail = trails[i];
				if (!trail)
				{
					continue;
				}
				Vector3 rel = trail.transform.position - body.position;
				float v = known ? ((rel - last[i]).magnitude / dt) : 0f;
				last[i] = rel;
				speed[i] = Mathf.Lerp(speed[i], (v > Snap) ? 0f : v, blend);
				trail.emitting = !isLimp && speed[i] > From && (grip == null || grip(i) >= FistAt);
				if (trail.emitting && speed[i] >= SwingAt && !swung[i])
				{
					swung[i] = true;
					onSwing?.Invoke(i, speed[i]);
				}
				else if (swung[i] && (speed[i] < SwingReset || isLimp))
				{
					swung[i] = false;
				}
				float f = Mathf.Clamp01((speed[i] - From) / (Full - From));
				Color c = Color.Lerp(Slow, Fast, f);
				trail.startColor = c;
				trail.endColor = new Color(c.r, c.g, c.b, 0f);
				trail.widthMultiplier = Width * Mathf.Lerp(0.6f, 1f, f);
				trail.time = Life * Mathf.Lerp(0.7f, 1.3f, f);
			}
			known = true;
		}

		private void Make()
		{
			if (!mat)
			{
				mat = Resources.Load<Material>("Effects/FistTrail");
			}
			trails = new TrailRenderer[fists.Length];
			last = new Vector3[fists.Length];
			speed = new float[fists.Length];
			swung = new bool[fists.Length];
			for (int i = 0; i < fists.Length; i++)
			{
				Transform fist = fists[i];
				if ((bool)fist)
				{
					GameObject go = new GameObject("Fist Trail");
					go.layer = fist.gameObject.layer;
					go.transform.SetParent(fist, false);
					go.transform.localPosition = Centre(fist);
					TrailRenderer trail = go.AddComponent<TrailRenderer>();
					trail.sharedMaterial = mat;
					trail.time = Life;
					trail.minVertexDistance = 0.015f;
					trail.widthMultiplier = Width;
					trail.widthCurve = AnimationCurve.Linear(0f, 1f, 1f, 0.35f);
					trail.textureMode = LineTextureMode.Stretch;
					trail.alignment = LineAlignment.View;
					trail.numCapVertices = 2;
					trail.shadowCastingMode = ShadowCastingMode.Off;
					trail.receiveShadows = false;
					trail.emitting = false;
					trails[i] = trail;
				}
			}
		}

		private static Vector3 Centre(Transform fist)
		{
			foreach (Collider c in fist.GetComponents<Collider>())
			{
				if (c.enabled && !c.isTrigger)
				{
					return fist.InverseTransformPoint(c.bounds.center);
				}
			}
			foreach (Transform child in fist)
			{
				if (child.name.StartsWith("Glove"))
				{
					return child.localPosition;
				}
			}
			return Vector3.zero;
		}

		private void OnDestroy()
		{
			if (trails == null)
			{
				return;
			}
			TrailRenderer[] array = trails;
			foreach (TrailRenderer trail in array)
			{
				if ((bool)trail)
				{
					UnityEngine.Object.Destroy(trail.gameObject);
				}
			}
		}
	}
}
