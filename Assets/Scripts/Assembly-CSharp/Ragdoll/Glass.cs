using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ragdoll
{
	public class Glass : MonoBehaviour
	{
		[Header("Glass")]
		public float breakSpeed = 7f;

		public float burst = 2.5f;

		public float pieceLife = 3f;

		public Material intact;

		public AudioClip[] shatter;

		private static readonly List<Glass> all = new List<Glass>();

		private static bool sorted;

		private Renderer[] pieces;

		private GameObject pane;

		private BoxCollider box;

		private bool broken;

		private bool fading;

		private float brokeAt;

		private Vector3[] sizes;

		private void Awake()
		{
			pieces = GetComponentsInChildren<Renderer>();
			if (pieces.Length == 0)
			{
				return;
			}
			sizes = new Vector3[pieces.Length];
			for (int i = 0; i < pieces.Length; i++)
			{
				sizes[i] = pieces[i].transform.localScale;
			}
			Bounds local = new Bounds(transform.InverseTransformPoint(pieces[0].bounds.center), Vector3.zero);
			foreach (Renderer r in pieces)
			{
				Bounds b = r.bounds;
				for (int k = 0; k < 8; k++)
				{
					Vector3 corner = b.center + new Vector3((k & 1) == 0 ? -b.extents.x : b.extents.x, (k & 2) == 0 ? -b.extents.y : b.extents.y, (k & 4) == 0 ? -b.extents.z : b.extents.z);
					local.Encapsulate(transform.InverseTransformPoint(corner));
				}
				r.enabled = false;
				foreach (Collider col in r.GetComponents<Collider>())
				{
					col.enabled = false;
				}
			}
			pane = GameObject.CreatePrimitive(PrimitiveType.Cube);
			pane.name = "Pane";
			Object.Destroy(pane.GetComponent<Collider>());
			pane.transform.SetParent(transform, false);
			pane.transform.localPosition = local.center;
			pane.transform.localRotation = Quaternion.identity;
			pane.transform.localScale = local.size;
			MeshRenderer mr = pane.GetComponent<MeshRenderer>();
			mr.sharedMaterial = intact ? intact : pieces[0].sharedMaterial;
			mr.shadowCastingMode = ShadowCastingMode.Off;
			Rigidbody rb = GetComponent<Rigidbody>();
			if (!rb)
			{
				rb = gameObject.AddComponent<Rigidbody>();
			}
			rb.isKinematic = true;
			rb.useGravity = false;
			box = gameObject.AddComponent<BoxCollider>();
			box.center = local.center;
			box.size = local.size;
			all.Add(this);
			sorted = false;
		}

		private void OnDestroy()
		{
			all.Remove(this);
		}

		private void OnCollisionEnter(Collision c)
		{
			Rigidbody rb = c.rigidbody;
			if (broken || !rb)
			{
				return;
			}
			if (c.relativeVelocity.magnitude < breakSpeed)
			{
				return;
			}
			if (!Mine(rb))
			{
				return;
			}
			Vector3 point = c.contactCount > 0 ? c.GetContact(0).point : rb.position;
			Vector3 push = (rb.linearVelocity.sqrMagnitude > 0.01f ? rb.linearVelocity : box.bounds.center - point).normalized;
			NetworkPlayer mine = NetworkPlayer.mine;
			if (mine && mine.Object && mine.Object.IsValid)
			{
				mine.RPC_Glass(Index(), point, push);
				return;
			}
			Break(point, push);
		}

		private static bool Mine(Rigidbody rb)
		{
			Player me = Player.main;
			if (me && rb.transform.IsChildOf(me.transform))
			{
				return true;
			}
			NetProp prop = rb.GetComponentInParent<NetProp>();
			if (prop && prop.Object && prop.Object.IsValid)
			{
				return prop.HasStateAuthority;
			}
			return !NetworkPlayer.mine;
		}

		private int Index()
		{
			if (!sorted)
			{
				all.Sort((Glass a, Glass b) => string.CompareOrdinal(Path(a.transform), Path(b.transform)));
				sorted = true;
			}
			return all.IndexOf(this);
		}

		private static string Path(Transform t)
		{
			string path = t.GetSiblingIndex().ToString("D3");
			for (Transform p = t.parent; p; p = p.parent)
			{
				path = p.GetSiblingIndex().ToString("D3") + "/" + path;
			}
			return path;
		}

		public static void Break(int index, Vector3 point, Vector3 push)
		{
			if (!sorted && all.Count > 0)
			{
				all[0].Index();
			}
			if (index < 0 || index >= all.Count)
			{
				return;
			}
			all[index].Break(point, push);
		}

		public void Break(Vector3 point, Vector3 push)
		{
			if (broken || !pane)
			{
				return;
			}
			broken = true;
			brokeAt = Time.time;
			pane.SetActive(false);
			box.enabled = false;
			AudioClip clip = (shatter != null && shatter.Length != 0) ? shatter[Random.Range(0, shatter.Length)] : null;
			if (clip)
			{
				AudioSource.PlayClipAtPoint(clip, point, Prefs.sfx);
			}
			foreach (Renderer r in pieces)
			{
				if (!r)
				{
					continue;
				}
				r.enabled = true;
				GameObject go = r.gameObject;
				BoxCollider col = go.GetComponent<BoxCollider>();
				if (!col)
				{
					col = go.AddComponent<BoxCollider>();
				}
				col.enabled = true;
				Rigidbody rb = go.GetComponent<Rigidbody>();
				if (!rb)
				{
					rb = go.AddComponent<Rigidbody>();
				}
				rb.isKinematic = false;
				rb.mass = 0.2f;
				Vector3 away = r.bounds.center - point;
				rb.linearVelocity = push * burst + away.normalized * Random.Range(0.3f, 1f) * burst / Mathf.Max(away.magnitude * 4f, 1f);
				rb.angularVelocity = Random.insideUnitSphere * 10f;
			}
		}

		private void Update()
		{
			if (!broken)
			{
				return;
			}
			if (Time.time < brokeAt + pieceLife)
			{
				return;
			}
			float k = 1f - Mathf.Clamp01((Time.time - brokeAt - pieceLife) / 0.6f);
			for (int i = 0; i < pieces.Length; i++)
			{
				Renderer r = pieces[i];
				if (!r)
				{
					continue;
				}
				if (!fading)
				{
					Rigidbody rb = r.GetComponent<Rigidbody>();
					if (rb)
					{
						rb.isKinematic = true;
					}
					Collider col = r.GetComponent<Collider>();
					if (col)
					{
						col.enabled = false;
					}
				}
				if (k <= 0f)
				{
					Object.Destroy(r.gameObject);
				}
				else
				{
					r.transform.localScale = sizes[i] * k;
				}
			}
			fading = true;
			if (k <= 0f)
			{
				enabled = false;
			}
		}
	}
}
