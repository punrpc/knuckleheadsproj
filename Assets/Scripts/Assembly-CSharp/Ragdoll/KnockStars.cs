using UnityEngine;

namespace Ragdoll
{
	public class KnockStars : MonoBehaviour
	{
		[Header("Stars")]
		public float above = 0.33f;

		[HideInInspector]
		public Transform head;

		[HideInInspector]
		public bool on;

		[HideInInspector]
		public bool dead;

		private ParticleSystem stars;

		private Collider headCol;

		private bool shown;

		private static GameObject prefab;

		private static Material deadLook;

		private Material faceLook;

		private bool deadShown;

		private Renderer face;

		private Renderer blink;

		private Renderer dizzy;

		private bool faceFound;

		private float blinkAt;

		private const float BlinkMin = 2f;

		private const float BlinkMax = 6f;

		private const float BlinkFor = 0.12f;

		private void LateUpdate()
		{
			if (!head)
			{
				return;
			}
			Face();
			if (on && !stars)
			{
				Make();
			}
			if (!stars)
			{
				return;
			}
			Vector3 top = ((bool)headCol && headCol.enabled) ? headCol.bounds.center : head.position;
			stars.transform.SetPositionAndRotation(top + Vector3.up * above, Quaternion.identity);
			if (on)
			{
				if (!shown)
				{
					stars.Play(true);
					shown = true;
				}
			}
			else if (shown)
			{
				stars.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				shown = false;
			}
		}

		private void Face()
		{
			if (!faceFound)
			{
				faceFound = true;
				Transform f = head.Find("Face");
				if (!f)
				{
					return;
				}
				face = f.GetComponent<Renderer>();
				faceLook = face ? face.sharedMaterial : null;
				if (!deadLook)
				{
					deadLook = Resources.Load<Material>("Player/DeadEyes");
				}
				Transform b = f.Find("Blink");
				blink = b ? b.GetComponent<Renderer>() : null;
				Transform d = f.Find("Dizzy");
				dizzy = d ? d.GetComponent<Renderer>() : null;
				blinkAt = Time.time + Random.Range(BlinkMin, BlinkMax);
			}
			if (!face)
			{
				return;
			}
			if (Time.time >= blinkAt + BlinkFor)
			{
				blinkAt = Time.time + Random.Range(BlinkMin, BlinkMax);
			}
			bool showDead = dead && (bool)deadLook;
			if (deadShown != showDead)
			{
				deadShown = showDead;
				face.sharedMaterial = showDead ? deadLook : faceLook;
			}
			bool showFace = true;
			bool showBlink = false;
			bool showDizzy = false;
			if (!showDead)
			{
				if (on && (bool)dizzy)
				{
					showFace = false;
					showDizzy = true;
				}
				else if ((bool)blink)
				{
					showBlink = Time.time >= blinkAt;
					showFace = !showBlink;
				}
			}
			face.enabled = showFace;
			if ((bool)blink)
			{
				blink.enabled = showBlink;
			}
			if ((bool)dizzy)
			{
				dizzy.enabled = showDizzy;
			}
		}

		private void Make()
		{
			if (!prefab)
			{
				prefab = Resources.Load<GameObject>("Effects/KnockStars");
			}
			if ((bool)prefab)
			{
				stars = Object.Instantiate(prefab).GetComponent<ParticleSystem>();
				headCol = head.GetComponent<Collider>();
			}
		}

		private void OnDestroy()
		{
			if ((bool)stars)
			{
				Object.Destroy(stars.gameObject);
			}
		}
	}
}
