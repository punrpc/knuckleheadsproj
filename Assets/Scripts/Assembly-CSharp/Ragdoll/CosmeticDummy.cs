using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

namespace Ragdoll
{
	public class CosmeticDummy : MonoBehaviour
	{
		[Serializable]
		public class Arrow
		{
			public int slot;

			public int step;

			public Renderer button;

			public Collider zone;
		}

		[Header("Cosmetics")]
		public Cosmetics cosmetics = new Cosmetics();

		[Header("Parts")]
		public Transform body;

		public Arrow[] arrows;

		public TextMeshPro[] labels;

		public LineRenderer[] rays;

		[Header("Idle")]
		public float swayAmount = 1f;

		public float swaySpeed = 1f;

		public bool blink = true;

		[Header("Pointer")]
		public float reach = 6f;

		public float pokeReach = 0.05f;

		[Header("Arrow Colors")]
		public Color idle = new Color(0.95f, 0.95f, 0.95f, 1f);

		public Color lit = new Color(1f, 0.85f, 0.2f, 1f);

		[Header("Podium")]
		public AudioClip riseSound;

		public GameObject splash;

		public GameObject ripples;

		public GameObject fireworks;

		private static readonly string[] swayBones = { "UpperTorso", "Head", "UpperArm.L", "UpperArm.R", "LowerArm.L", "LowerArm.R" };

		private readonly XRNode[] nodes = { XRNode.LeftHand, XRNode.RightHand };

		private readonly InputDevice[] devices = new InputDevice[2];

		private readonly int[] hover = { -1, -1, -1 };

		private int seenPaints = -1;

		private readonly bool[] held = new bool[3];

		private readonly RaycastHit[] hits = new RaycastHit[32];

		private bool[] poked;

		private Vector3[] restScale;

		private Transform[] sway;

		private Quaternion[] rest;

		private Renderer face;

		private Renderer closedEyes;

		private MaterialPropertyBlock block;

		private float seed;

		private float blinkAt;

		public const int ColorSlot = 4;

		private static readonly string[] Places = { "1ST", "2ND", "3RD" };

		private const float NameGap = 0.06f;

		private const float NameHeight = 0.22f;

		private const float Sink = 5.6f;

		private const float IslandSink = 7.5f;

		private const float SplashEvery = 0.45f;

		private const float RingScale = 3f;

		private float sink;

		private float splashAt;

		private GameObject ring;

		private const float RiseTime = 6f;

		private const float RiseWait = 1f;

		private const float FadeIn = 0.6f;

		private const float FadeOut = 1.5f;

		private const float RumbleVolume = 0.9f;

		private const float Shake = 0.02f;

		private static int risenFor;

		private Transform stand;

		private Transform island;

		private bool podiumBuilt;

		private Transform podium;

		private Vector3 podiumHome;

		private float riseAt = -1f;

		private AudioSource rumble;

		private void Start()
		{
			Prefs.Load();
			block = new MaterialPropertyBlock();
			poked = new bool[arrows.Length];
			restScale = new Vector3[arrows.Length];
			for (int i = 0; i < arrows.Length; i++)
			{
				restScale[i] = arrows[i].button.transform.localScale;
				Glow(i);
			}
			if (cosmetics.Move(Prefs.outfit))
			{
				Prefs.Save();
			}
			for (int slot = 0; slot < Cosmetics.Slots.Length; slot++)
			{
				cosmetics.Wear(slot, Prefs.outfit[slot]);
			}
			Refresh();
			Idle();
			Podium();
		}

		private void Podium()
		{
			foreach (Transform t in FindObjectsByType<Transform>(FindObjectsSortMode.None))
			{
				if (t.name == "Podium" && t.gameObject.scene == gameObject.scene)
				{
					stand = t;
				}
			}
			if (!stand)
			{
				return;
			}
			island = (stand.parent && stand.parent.GetComponent<Island>()) ? stand.parent : stand;
			island.gameObject.SetActive(false);
		}

		private void ShowPodium()
		{
			if (!island)
			{
				return;
			}
			bool here = GameManager.PodiumHere();
			if (here == island.gameObject.activeSelf)
			{
				return;
			}
			island.gameObject.SetActive(here);
			if (!here)
			{
				if (!podium)
				{
					return;
				}
				podium.position = podiumHome;
				if (rumble)
				{
					Destroy(rumble);
				}
				if (ring)
				{
					Destroy(ring);
				}
				podium = null;
				riseAt = -1f;
				return;
			}
			if (podiumBuilt)
			{
				return;
			}
			podiumBuilt = true;
			for (int i = 0; i < sway.Length; i++)
			{
				if (sway[i])
				{
					sway[i].localRotation = rest[i];
				}
			}
			if (face)
			{
				face.enabled = true;
			}
			if (closedEyes)
			{
				closedEyes.enabled = false;
			}
			StringBuilder shown = new StringBuilder();
			for (int i = 0; i < Places.Length && i < GameManager.podium.Count; i++)
			{
				Transform block = stand.Find(Places[i]);
				if (block)
				{
					Stand(GameManager.podium[i], stand, block, Places[i]);
					shown.Append(Places[i]).Append(' ').Append(GameManager.podium[i].name).Append("  ");
				}
			}
			Player.Note("podium shows " + shown);
			if (risenFor != GameManager.podiumRound)
			{
				risenFor = GameManager.podiumRound;
				podium = island;
				sink = podium == stand ? Sink : IslandSink;
				podiumHome = podium.position;
				podium.position = podiumHome + Vector3.down * sink;
			}
		}

		private float WaterLine()
		{
			if (!Water.main)
			{
				return podiumHome.y - 1.2f;
			}
			return Water.main.Surface;
		}

		private Vector3 Edge(float angle)
		{
			Renderer r = podium.GetComponent<Renderer>();
			Bounds b = r ? r.bounds : new Bounds(podium.position, Vector3.one * 4f);
			return new Vector3(b.center.x + Mathf.Cos(angle) * b.extents.x * 0.85f, WaterLine() + 0.05f, b.center.z + Mathf.Sin(angle) * b.extents.z * 0.85f);
		}

		private void Effect(GameObject prefab, Vector3 at, float scale, float life)
		{
			if (!prefab)
			{
				return;
			}
			GameObject fx = Instantiate(prefab, at, Quaternion.identity);
			fx.transform.localScale = Vector3.one * scale;
			if (life > 0f)
			{
				Destroy(fx, life);
			}
		}

		private void Celebrate()
		{
			Transform top = podium.name == "Podium" ? podium : podium.Find("Podium");
			Vector3 at = top ? top.position : podiumHome;
			Effect(fireworks, at + new Vector3(0f, 7f, 0f), 1.4f, 6f);
			Effect(fireworks, at + new Vector3(-2.5f, 5.5f, 1f), 1.1f, 6f);
			Effect(fireworks, at + new Vector3(2.5f, 6f, -1f), 1.1f, 6f);
			for (int i = 0; i < 4; i++)
			{
				Effect(splash, Edge(i * Mathf.PI * 0.5f + 0.4f), 1.6f, 4f);
			}
		}

		private void Rise()
		{
			if (!podium)
			{
				return;
			}
			if (riseAt < 0f)
			{
				Player main = Player.main;
				if (main && main.dark > 0.05f)
				{
					return;
				}
				riseAt = Time.time + RiseWait;
				return;
			}
			float e = Time.time - riseAt;
			if (e < 0f)
			{
				return;
			}
			if (!rumble && riseSound)
			{
				rumble = podium.gameObject.AddComponent<AudioSource>();
				rumble.clip = riseSound;
				rumble.playOnAwake = false;
				rumble.spatialBlend = 1f;
				rumble.rolloffMode = AudioRolloffMode.Linear;
				rumble.minDistance = 4f;
				rumble.maxDistance = 40f;
				rumble.volume = 0f;
				rumble.Play();
				Player.Note(podium.name + " rising");
				if (ripples && !ring)
				{
					Renderer r = podium.GetComponent<Renderer>();
					Vector3 c = r ? r.bounds.center : podiumHome;
					ring = Instantiate(ripples, new Vector3(c.x, WaterLine() + 0.03f, c.z), Quaternion.identity);
					ring.transform.localScale = Vector3.one * RingScale;
				}
				splashAt = e;
			}
			if (e < RiseTime && e >= splashAt)
			{
				splashAt = e + SplashEvery;
				Effect(splash, Edge(UnityEngine.Random.Range(0f, Mathf.PI * 2f)), UnityEngine.Random.Range(0.9f, 1.4f), 4f);
			}
			float nx = Mathf.PerlinNoise(Time.time * 23f, 0.3f);
			float nz = Mathf.PerlinNoise(0.7f, Time.time * 23f);
			float t = Mathf.Clamp01(e / RiseTime);
			float ease = t * t * (3f - 2f * t);
			podium.position = podiumHome + Vector3.down * ((1f - ease) * sink) + new Vector3(nx - 0.5f, 0f, nz - 0.5f) * ((1f - t) * Shake * 2f);
			if (rumble)
			{
				rumble.volume = Mathf.Clamp01((RiseTime + 0.5f - e) / FadeOut) * (Mathf.Clamp01(e / FadeIn) * (Prefs.sfx * RumbleVolume));
			}
			if (e < RiseTime + 0.5f)
			{
				return;
			}
			podium.position = podiumHome;
			if (rumble)
			{
				Destroy(rumble);
			}
			if (ring)
			{
				Destroy(ring);
			}
			Celebrate();
			podium = null;
		}

		private void Stand(GameManager.Placing who, Transform stand, Transform block, string place)
		{
			Renderer r = block.GetComponent<Renderer>();
			if (!r)
			{
				return;
			}
			Bounds b = r.bounds;
			Vector3 down = block.TransformDirection(Vector3.down);
			Vector3 flat = new Vector3(down.x, 0f, down.z);
			Vector3 face = flat.sqrMagnitude > 0.0001f ? flat.normalized : stand.forward;
			GameObject copy = Instantiate(body.gameObject, stand);
			copy.name = place + " " + who.name;
			copy.transform.SetPositionAndRotation(new Vector3(b.center.x, b.center.y + b.extents.y, b.center.z), Quaternion.LookRotation(face));
			Cosmetics worn = Copy(copy.transform);
			for (int slot = 0; slot < Cosmetics.Slots.Length; slot++)
			{
				worn.Wear(slot, who.outfit[slot]);
			}
			NetworkManager net = NetworkManager.main ? NetworkManager.main : FindFirstObjectByType<NetworkManager>();
			Color paint = (net && net.networkPlayerPrefab) ? net.networkPlayerPrefab.Colour(who.paint) : Color.white;
			worn.Tint(copy.transform, paint);
			TextMeshPro label = new GameObject(place + " Name").AddComponent<TextMeshPro>();
			label.transform.SetParent(stand, false);
			TextMeshPro style = labels.Length > 0 ? labels[0] : null;
			if (style)
			{
				label.font = style.font;
				label.fontSharedMaterial = style.fontSharedMaterial;
			}
			float width = 1f;
			MeshFilter mf = block.GetComponent<MeshFilter>();
			if (mf && mf.sharedMesh)
			{
				width = mf.sharedMesh.bounds.size.x * Mathf.Abs(block.lossyScale.x) * 0.82f;
			}
			label.rectTransform.sizeDelta = new Vector2(Mathf.Max(width, 0.5f), NameHeight);
			label.enableAutoSizing = true;
			label.enableWordWrapping = false;
			label.fontSizeMin = 0.2f;
			label.fontSizeMax = 2.2f;
			label.fontStyle = FontStyles.Bold;
			label.alignment = TextAlignmentOptions.Center;
			label.color = Color.white;
			label.text = who.name;
			if (!Number(block, place, out Vector3 at, out float low))
			{
				float reachOut = Mathf.Abs(Vector3.Dot(b.extents, new Vector3(Mathf.Abs(face.x), 0f, Mathf.Abs(face.z))));
				at = new Vector3(b.center.x, 0f, b.center.z) + face * reachOut;
				low = b.min.y + 0.6f;
			}
			float y = Mathf.Max(low - NameGap - 0.11f, b.min.y + 0.132f);
			label.transform.SetPositionAndRotation(new Vector3(at.x, y, at.z) + face * 0.02f, Quaternion.LookRotation(-face));
		}

		private static bool Number(Transform block, string place, out Vector3 at, out float low)
		{
			at = Vector3.zero;
			low = float.MaxValue;
			Renderer own = block.GetComponent<Renderer>();
			Shader skip = (own && own.sharedMaterial) ? own.sharedMaterial.shader : null;
			Vector3 sum = Vector3.zero;
			float area = 0f;
			foreach (MeshRenderer mr in block.parent.GetComponentsInChildren<MeshRenderer>(true))
			{
				if (!mr.name.StartsWith(place))
				{
					continue;
				}
				MeshFilter mf = mr.GetComponent<MeshFilter>();
				Mesh mesh = mf ? mf.sharedMesh : null;
				if (!mesh || !mesh.isReadable)
				{
					continue;
				}
				Vector3[] verts = mesh.vertices;
				Material[] mats = mr.sharedMaterials;
				for (int s = 0; s < mesh.subMeshCount; s++)
				{
					if (s >= mats.Length)
					{
						break;
					}
					if (!mats[s] || mats[s].shader == skip)
					{
						continue;
					}
					int[] tris = mesh.GetTriangles(s);
					for (int i = 0; i < tris.Length; i += 3)
					{
						Vector3 a = mr.transform.TransformPoint(verts[tris[i]]);
						Vector3 b = mr.transform.TransformPoint(verts[tris[i + 1]]);
						Vector3 c = mr.transform.TransformPoint(verts[tris[i + 2]]);
						float w = Vector3.Cross(b - a, c - a).magnitude * 0.5f;
						sum += (a + b + c) / 3f * w;
						area += w;
						low = Mathf.Min(low, Mathf.Min(a.y, Mathf.Min(b.y, c.y)));
					}
				}
			}
			if (area > 0f)
			{
				at = sum / area;
			}
			return area > 0f;
		}

		private Cosmetics Copy(Transform to)
		{
			Cosmetics copy = new Cosmetics();
			for (int slot = 0; slot < Cosmetics.Slots.Length; slot++)
			{
				foreach (Cosmetics.Item item in cosmetics.List(slot))
				{
					List<GameObject> parts = new List<GameObject>();
					foreach (GameObject part in item.parts)
					{
						Transform t = part ? to.Find(PathFrom(body, part.transform)) : null;
						if (t)
						{
							parts.Add(t.gameObject);
						}
					}
					copy.List(slot).Add(new Cosmetics.Item
					{
						name = item.name,
						parts = parts.ToArray()
					});
				}
			}
			return copy;
		}

		private static string PathFrom(Transform root, Transform t)
		{
			string path = t.name;
			Transform p = t.parent;
			while (p && p != root)
			{
				path = p.name + "/" + path;
				p = p.parent;
			}
			return path;
		}

		public string Choose(int slot, int step)
		{
			if (slot == ColorSlot)
			{
				StepColor(step);
				Refresh();
				return "";
			}
			string pick = cosmetics.Step(slot, Prefs.outfit[slot], step);
			Prefs.outfit[slot] = pick;
			Prefs.Save();
			cosmetics.Wear(slot, pick);
			if (Player.main)
			{
				Player.main.Wear(slot, pick);
			}
			Refresh();
			Player.Note("wardrobe " + Cosmetics.Slots[slot] + " -> " + (pick == "" ? "None" : pick));
			return pick;
		}

		private void Refresh()
		{
			for (int i = 0; i < labels.Length && i < cosmetics.worn.Length; i++)
			{
				string worn = cosmetics.worn[i];
				if (labels[i])
				{
					labels[i].text = worn == "" ? "NONE" : worn.ToUpper();
				}
			}
			if (labels.Length <= ColorSlot || !labels[ColorSlot])
			{
				return;
			}
			Color[] palette = Palette();
			bool random = Prefs.color < 1 || Prefs.color > palette.Length;
			labels[ColorSlot].text = random ? "RANDOM" : GameManager.ColorName(Prefs.color);
			labels[ColorSlot].color = random ? Color.white : palette[Prefs.color - 1];
		}

		private static Color[] Palette()
		{
			NetworkManager net = NetworkManager.main ? NetworkManager.main : FindFirstObjectByType<NetworkManager>();
			if (net && net.networkPlayerPrefab && net.networkPlayerPrefab.colors != null)
			{
				return net.networkPlayerPrefab.colors;
			}
			return new Color[0];
		}

		private void StepColor(int step)
		{
			Color[] palette = Palette();
			int n = palette.Length;
			if (n == 0)
			{
				return;
			}
			int from = (Prefs.color >= 1 && Prefs.color <= n) ? Prefs.color : (step < 1 ? 1 : 0);
			for (int i = 1; i <= n; i++)
			{
				int index = ((from + step * i - 1) % n + n) % n;
				int pick = index + 1;
				if (NetworkPlayer.TakenByOthers(pick))
				{
					continue;
				}
				Prefs.color = pick;
				Prefs.Save();
				Player main = Player.main;
				if (main && !(NetworkManager.main && NetworkManager.main.InRoom))
				{
					main.cosmetics.Tint(main.transform, palette[index]);
				}
				Player.Note("wardrobe color -> " + GameManager.ColorName(pick));
				return;
			}
		}

		private void LateUpdate()
		{
			ShowPodium();
			Rise();
			Sway();
			Blink();
			Player main = Player.main;
			if (main && main.cosmetics.color != cosmetics.color)
			{
				cosmetics.Tint(body, main.cosmetics.color);
			}
			int paints = 0;
			foreach (NetworkPlayer avatar in NetworkPlayer.avatars)
			{
				if (avatar)
				{
					paints = paints * 31 + avatar.Paint;
				}
			}
			if (paints != seenPaints)
			{
				seenPaints = paints;
				Refresh();
			}
			Poke(main);
			if (main && main.inVR)
			{
				Aim(main, 0);
				Aim(main, 1);
				return;
			}
			for (int h = 0; h < 2; h++)
			{
				if (rays[h].enabled)
				{
					rays[h].enabled = false;
				}
			}
			Transform view = (main && main.cam) ? main.cam : (Camera.main ? Camera.main.transform : null);
			int over = view ? Cast(main, view.position, view.forward) : -1;
			Hover(2, over);
			bool down = UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.isPressed;
			if (down && over >= 0 && !held[2])
			{
				Click(over, XRNode.Head);
			}
			held[2] = down;
		}

		private void Aim(Player me, int h)
		{
			int over = Point(me, h, out Vector3 from, out Vector3 dir) ? Cast(me, from, dir) : -1;
			LineRenderer ray = rays[h];
			if (ray.enabled != over >= 0)
			{
				ray.enabled = over >= 0;
			}
			if (over >= 0)
			{
				ray.SetPosition(0, from);
				ray.SetPosition(1, arrows[over].button.transform.position);
				if (over != hover[h])
				{
					Player.Hand(nodes[h], 0.12f, 0.02f);
				}
			}
			Hover(h, over);
			bool down = devices[h].TryGetFeatureValue(CommonUsages.trigger, out float trigger) && trigger > (held[h] ? 0.3f : 0.6f);
			if (down && !held[h] && over >= 0)
			{
				Click(over, nodes[h]);
			}
			held[h] = down;
		}

		private void Poke(Player me)
		{
			for (int i = 0; i < arrows.Length; i++)
			{
				bool left = me && Touching(arrows[i].zone, me.leftHand);
				bool right = me && Touching(arrows[i].zone, me.rightHand);
				if ((left || right) && !poked[i])
				{
					Click(i, left ? XRNode.LeftHand : XRNode.RightHand);
				}
				poked[i] = left || right;
			}
		}

		private bool Touching(Collider zone, Rigidbody hand)
		{
			if (!hand)
			{
				return false;
			}
			Vector3 p = hand.position;
			return (zone.ClosestPoint(p) - p).sqrMagnitude < pokeReach * pokeReach;
		}

		private void Click(int i, XRNode node)
		{
			Choose(arrows[i].slot, arrows[i].step);
			if (node != XRNode.Head)
			{
				Player.Hand(node, 0.35f, 0.05f);
			}
		}

		private void Hover(int who, int over)
		{
			if (hover[who] == over)
			{
				return;
			}
			int was = hover[who];
			hover[who] = over;
			if (was >= 0)
			{
				Glow(was);
			}
			if (over >= 0)
			{
				Glow(over);
			}
		}

		private void Glow(int i)
		{
			bool on = hover[0] == i || hover[1] == i || hover[2] == i;
			Renderer button = arrows[i].button;
			button.GetPropertyBlock(block);
			block.SetColor("_BaseColor", on ? lit : idle);
			button.SetPropertyBlock(block);
			button.transform.localScale = (on ? 1.12f : 1f) * restScale[i];
		}

		private bool Point(Player me, int h, out Vector3 from, out Vector3 dir)
		{
			from = Vector3.zero;
			dir = Vector3.forward;
			Transform hand = h == 0 ? me.controllerL : me.controllerR;
			if (!hand)
			{
				return false;
			}
			if (!devices[h].isValid)
			{
				devices[h] = InputDevices.GetDeviceAtXRNode(nodes[h]);
			}
			Quaternion turn = Quaternion.identity;
			Vector3 offset = Vector3.zero;
			if (devices[h].TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 gripPos) && devices[h].TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion gripRot) && devices[h].TryGetFeatureValue(new InputFeatureUsage<Vector3>("PointerPosition"), out Vector3 aimPos) && devices[h].TryGetFeatureValue(new InputFeatureUsage<Quaternion>("PointerRotation"), out Quaternion aimRot))
			{
				Quaternion inv = Quaternion.Inverse(gripRot);
				turn = inv * aimRot;
				offset = inv * (aimPos - gripPos);
			}
			from = hand.position + hand.rotation * offset;
			dir = hand.rotation * turn * Vector3.forward;
			return true;
		}

		private int Cast(Player me, Vector3 from, Vector3 dir)
		{
			int count = Physics.RaycastNonAlloc(from, dir, hits, reach, -1, QueryTriggerInteraction.Collide);
			int best = -1;
			float nearest = float.MaxValue;
			for (int i = 0; i < count; i++)
			{
				RaycastHit hit = hits[i];
				if (hit.distance >= nearest)
				{
					continue;
				}
				if (me && hit.collider.transform.IsChildOf(me.transform))
				{
					continue;
				}
				int arrow = Find(hit.collider);
				if (hit.collider.isTrigger && arrow < 0)
				{
					continue;
				}
				nearest = hit.distance;
				best = arrow;
			}
			return best;
		}

		private int Find(Collider c)
		{
			for (int i = 0; i < arrows.Length; i++)
			{
				if (arrows[i].zone == c)
				{
					return i;
				}
			}
			return -1;
		}

		private void Idle()
		{
			sway = new Transform[swayBones.Length + 1];
			sway[0] = body;
			foreach (Transform t in body.GetComponentsInChildren<Transform>(true))
			{
				int i = Array.IndexOf(swayBones, t.name);
				if (i >= 0 && !sway[i + 1])
				{
					sway[i + 1] = t;
				}
			}
			rest = new Quaternion[sway.Length];
			for (int i = 0; i < sway.Length; i++)
			{
				if (sway[i])
				{
					rest[i] = sway[i].localRotation;
				}
			}
			seed = UnityEngine.Random.value * 100f;
			Transform faceBone = sway[2] ? sway[2].Find("Face") : null;
			face = faceBone ? faceBone.GetComponent<Renderer>() : null;
			Transform shut = faceBone ? faceBone.Find("Blink") : null;
			closedEyes = shut ? shut.GetComponent<Renderer>() : null;
			blinkAt = Time.time + UnityEngine.Random.Range(2f, 6f);
		}

		private void Blink()
		{
			if (!blink || !face)
			{
				return;
			}
			if (Time.time >= blinkAt + 0.12f)
			{
				blinkAt = Time.time + UnityEngine.Random.Range(2f, 6f);
			}
			bool shut = closedEyes && Time.time >= blinkAt;
			face.enabled = !shut;
			if (closedEyes)
			{
				closedEyes.enabled = shut;
			}
		}

		private void Sway()
		{
			for (int i = 0; i < sway.Length; i++)
			{
				if (sway[i])
				{
					sway[i].localRotation = rest[i];
				}
			}
			float a = swayAmount;
			float t = Time.time * swaySpeed + seed;
			Vector3 f = transform.forward;
			Vector3 r = transform.right;
			Vector3 u = transform.up;
			Tilt(sway[0], f, a * (Mathf.Sin(t * 0.9f) * 2f + Mathf.Sin(t * 2.3f + 1f) * 0.8f), r, a * 1.2f * Mathf.Sin(t * 0.7f + 2f), u, 0f);
			Tilt(sway[1], f, a * -1.2f * Mathf.Sin(t * 0.9f + 0.4f), r, a * 1.5f * Mathf.Sin(t * 1.6f), u, a * 3f * Mathf.Sin(t * 0.5f));
			Tilt(sway[2], f, a * 3f * Mathf.Sin(t * 0.8f + 0.3f), r, a * 3f * Mathf.Sin(t * 1.1f + 0.5f), u, a * 8f * Mathf.Sin(t * 0.37f + 1.3f));
			Tilt(sway[3], f, a * -3f * Mathf.Sin(t * 1.3f), r, a * 5f * Mathf.Sin(t * 0.95f + 0.2f), u, 0f);
			Tilt(sway[4], f, a * 3f * Mathf.Sin(t * 1.2f + 0.7f), r, a * 5f * Mathf.Sin(t * 1.05f + 1.1f), u, 0f);
			Tilt(sway[5], f, 0f, r, a * 6f * Mathf.Sin(t * 1.4f + 0.6f), u, 0f);
			Tilt(sway[6], f, 0f, r, a * 6f * Mathf.Sin(t * 1.35f + 1.9f), u, 0f);
		}

		private static void Tilt(Transform b, Vector3 f, float roll, Vector3 r, float pitch, Vector3 u, float yaw)
		{
			if (!b)
			{
				return;
			}
			b.rotation = Quaternion.AngleAxis(roll, f) * Quaternion.AngleAxis(pitch, r) * Quaternion.AngleAxis(yaw, u) * b.rotation;
		}
	}
}
