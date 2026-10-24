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

		[Serializable]
		public class Bundle
		{
			public string name;

			public string head;

			public string torso;

			public string legs;

			public string face;

			public string Item(int slot)
			{
				switch (slot)
				{
					case 0:
						return head;
					case 1:
						return torso;
					case 2:
						return legs;
					default:
						return face;
				}
			}
		}

		private class Tag
		{
			public int slot;

			public Rigidbody rb;

			public TextMeshPro price;

			public LineRenderer cord;

			public Transform top;

			public Vector3 home;

			public Quaternion homeTurn;

			public Transform hole;

			public float rest;

			public bool pulled;

			public XRNode hand = XRNode.Head;

			public float denyUntil = -1f;

			public int need;

			public int key = int.MinValue;
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

		[Header("Bundles")]
		public Bundle[] bundles = new Bundle[10]
		{
			new Bundle { name = "SPORTY", head = "Baseball Hat", torso = "Tank Top", legs = "Shorts", face = "" },
			new Bundle { name = "DJ", head = "Headphones", torso = "Hoodie", legs = "Sneakers", face = "Sunglasses" },
			new Bundle { name = "CHEF", head = "Chef Hat", torso = "Apron", legs = "Shorts", face = "Mustache" },
			new Bundle { name = "SNOW DAY", head = "Beanie", torso = "Scarf", legs = "Rain Boots", face = "Ski Goggles" },
			new Bundle { name = "BUILDER", head = "Hard Hat Knuck", torso = "Hi Vis Vest", legs = "Rain Boots", face = "" },
			new Bundle { name = "PARTY", head = "Party Hat", torso = "Bow Tie", legs = "Tutu", face = "Clown Nose" },
			new Bundle { name = "COWBOY", head = "Cowboy Hat", torso = "Scarf", legs = "Rain Boots", face = "Mustache" },
			new Bundle { name = "GENTLEMAN", head = "Top Hat", torso = "Bow Tie", legs = "Shorts", face = "Mustache" },
			new Bundle { name = "SOLDIER", head = "Military Hat", torso = "Backpack", legs = "Sneakers", face = "Eyepatch" },
			new Bundle { name = "VIKING", head = "Viking Helmet", torso = "Cape", legs = "Rain Boots", face = "Eyepatch" }
		};

		[Header("Shop")]
		public Rigidbody brush;

		public GameObject paintSplat;

		public AudioClip paintSound;

		public AudioClip denySound;

		public GameObject buyBurst;

		public static XRNode brushHand = XRNode.Head;

		private static readonly string[] swayBones = { "UpperTorso", "Head", "UpperArm.L", "UpperArm.R", "LowerArm.L", "LowerArm.R" };

		private readonly XRNode[] nodes = { XRNode.LeftHand, XRNode.RightHand };

		private readonly InputDevice[] devices = new InputDevice[2];

		private readonly int[] hover = { -1, -1, -1 };

		private int seen = int.MinValue;

		private int bundle = -1;

		private int brushPaint = 1;

		private bool brushHeld;

		private bool brushTouching;

		private float hintAlpha;

		private XRNode hintHand = XRNode.Head;

		private Renderer bristles;

		private int bristleIndex;

		private Transform brushTip;

		private Collider paintZone;

		private TextMeshPro brushHint;

		private Tag[] tags;

		private const float TagPull = 0.1f;

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

		public const int BundleSlot = 4;

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
			cosmetics.Cover(body);
			bristles = brush ? brush.GetComponentInChildren<Renderer>() : null;
			if (bristles)
			{
				Material[] mats = bristles.sharedMaterials;
				for (int i = 0; i < mats.Length; i++)
				{
					if (mats[i] && mats[i].name == "Brush")
					{
						bristleIndex = i;
					}
				}
			}
			brushTip = brush ? brush.transform.Find("Tip") : null;
			Transform zone = transform.Find("Paint Zone");
			paintZone = zone ? zone.GetComponent<Collider>() : null;
			Transform hint = transform.Find("Brush Hint");
			brushHint = hint ? hint.GetComponent<TextMeshPro>() : null;
			if (brushHint)
			{
				brushHint.gameObject.SetActive(false);
			}
			tags = new Tag[Cosmetics.Slots.Length];
			for (int i = 0; i < tags.Length; i++)
			{
				Transform tag = transform.Find("Price Tag " + Cosmetics.Slots[i]);
				if (!tag)
				{
					continue;
				}
				Transform hole = tag.Find("Hole");
				Transform cord = transform.Find("Tag Cord " + Cosmetics.Slots[i]);
				tags[i] = new Tag
				{
					slot = i,
					rb = tag.GetComponent<Rigidbody>(),
					price = tag.GetComponentInChildren<TextMeshPro>(true),
					cord = cord ? cord.GetComponent<LineRenderer>() : null,
					top = transform.Find("Tag Top " + Cosmetics.Slots[i]),
					home = tag.position,
					homeTurn = tag.rotation,
					hole = hole ? hole : tag
				};
				if (tags[i].top)
				{
					tags[i].rest = Vector3.Distance(tags[i].hole.position, tags[i].top.position);
				}
			}
			brushPaint = Prefs.color < 1 ? 1 : Prefs.color;
			TintBrush();
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
						parts = parts.ToArray(),
						cover = item.cover
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
			if (slot == BundleSlot)
			{
				if (bundles == null || bundles.Length == 0)
				{
					return "";
				}
				bundle = bundle < 0 ? (step > 0 ? 0 : bundles.Length - 1) : ((bundle + step) % bundles.Length + bundles.Length) % bundles.Length;
				for (int s = 0; s < Cosmetics.Slots.Length; s++)
				{
					Show(s, bundles[bundle].Item(s) ?? "");
				}
				Prefs.Save();
				Refresh();
				Player.Note("wardrobe bundle -> " + bundles[bundle].name);
				return bundles[bundle].name;
			}
			int count = cosmetics.List(slot).Count;
			string pick = cosmetics.worn[slot];
			for (int i = 0; i < count + 1; i++)
			{
				pick = cosmetics.Step(slot, pick, step);
				if (pick != "Crown" || Account.Champion)
				{
					break;
				}
			}
			Show(slot, pick);
			Prefs.Save();
			Refresh();
			Player.Note("wardrobe " + Cosmetics.Slots[slot] + " -> " + (pick == "" ? "None" : pick) + (Account.Owned(pick) ? "" : " (locked)"));
			return pick;
		}

		private void Show(int slot, string item)
		{
			cosmetics.Wear(slot, item);
			string worn = cosmetics.worn[slot];
			if (!Account.Owned(worn))
			{
				return;
			}
			Prefs.outfit[slot] = worn;
			if (Player.main)
			{
				Player.main.Wear(slot, worn);
			}
		}

		private void Refresh()
		{
			if (cosmetics.worn[0] == "Crown" && Account.ChampionKnown && !Account.Champion)
			{
				cosmetics.Wear(0, Prefs.outfit[0] == "Crown" ? "" : Prefs.outfit[0]);
			}
			else if (cosmetics.worn[0] == "" && Account.Champion && Prefs.outfit[0] == "Crown")
			{
				cosmetics.Wear(0, "Crown");
			}
			for (int i = 0; i < labels.Length && i < BundleSlot && i < cosmetics.worn.Length; i++)
			{
				if (labels[i])
				{
					labels[i].text = Label(cosmetics.worn[i]);
				}
			}
			if (labels.Length > BundleSlot && labels[BundleSlot])
			{
				Bundle match = null;
				if (bundles != null)
				{
					foreach (Bundle b in bundles)
					{
						bool same = true;
						for (int s = 0; same && s < Cosmetics.Slots.Length; s++)
						{
							same = (b.Item(s) ?? "") == cosmetics.worn[s];
						}
						if (same)
						{
							match = b;
							break;
						}
					}
				}
				bool locked = false;
				for (int s = 0; s < Cosmetics.Slots.Length; s++)
				{
					locked |= !Account.Owned(cosmetics.worn[s]);
				}
				bool gold = match != null && locked;
				labels[BundleSlot].text = match != null ? match.name : "BUNDLES";
				labels[BundleSlot].color = new Color(1f, gold ? 0.88f : 1f, gold ? 0.3f : 1f, 1f);
			}
			if (tags == null)
			{
				return;
			}
			foreach (Tag t in tags)
			{
				if (t != null)
				{
					t.key = int.MinValue;
				}
			}
		}

		private static string Label(string item)
		{
			if (item == "")
			{
				return "NONE";
			}
			if (item == "Crown")
			{
				return "<color=#FFE14D>#1 CROWN</color>";
			}
			if (!Account.Owned(item))
			{
				return "<color=#FF8C8C>(LOCKED)</color> " + item.ToUpper();
			}
			return item.ToUpper();
		}

		private void Tags()
		{
			if (tags == null)
			{
				return;
			}
			foreach (Tag t in tags)
			{
				if (t == null || !t.rb)
				{
					continue;
				}
				string item = cosmetics.worn[t.slot];
				bool show = item != "" && Account.Cost(item) >= 1 && !Account.Owned(item);
				if (show != t.rb.gameObject.activeSelf)
				{
					if (!show)
					{
						Release(t);
					}
					t.rb.gameObject.SetActive(show);
					if (t.cord)
					{
						t.cord.gameObject.SetActive(show);
					}
					t.key = int.MinValue;
				}
				if (!show || !t.price)
				{
					continue;
				}
				bool denied = Time.time < t.denyUntil;
				int key = item.GetHashCode() * 31 + (denied ? t.need + 1 : 0) + (Account.Unlocking ? 7 : 0);
				if (key == t.key)
				{
					continue;
				}
				t.key = key;
				if (denied)
				{
					t.price.text = "<color=#E53935>" + (t.need > 0 ? "NEED\n" + t.need : "SHOP\nOFFLINE") + "</color>";
				}
				else
				{
					t.price.text = Account.Unlocking ? "..." : Account.Cost(item) + "\n<size=40%>PULL TO BUY</size>";
				}
			}
		}

		private void Release(Tag t)
		{
			Player me = Player.main;
			if (me)
			{
				Rigidbody[] hands = new Rigidbody[2] { me.leftHand, me.rightHand };
				foreach (Rigidbody hand in hands)
				{
					HandGrab grab = hand ? hand.GetComponent<HandGrab>() : null;
					if (grab && grab.grabbing && grab.held == t.rb)
					{
						grab.Drop("price tag");
					}
				}
			}
			t.pulled = false;
			t.rb.linearVelocity = Vector3.zero;
			t.rb.angularVelocity = Vector3.zero;
			t.rb.position = t.home;
			t.rb.rotation = t.homeTurn;
			t.rb.transform.SetPositionAndRotation(t.home, t.homeTurn);
			Cord(t);
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

		public static bool Busy(XRNode hand)
		{
			return brushHand == hand;
		}

		private void OnDisable()
		{
			brushHand = XRNode.Head;
		}

		private void TintBrush()
		{
			if (!bristles)
			{
				return;
			}
			Color[] palette = Palette();
			if (brushPaint < 1 || brushPaint > palette.Length)
			{
				return;
			}
			Color c = palette[brushPaint - 1];
			bristles.GetPropertyBlock(block, bristleIndex);
			block.SetColor("_TopColor", c);
			block.SetColor("_BottomColor", Color.Lerp(c, Color.white, 0.35f));
			block.SetColor("_BaseColor", c);
			bristles.SetPropertyBlock(block, bristleIndex);
		}

		private void Brush(Player me)
		{
			if (!brush)
			{
				return;
			}
			XRNode node = XRNode.Head;
			bool holding = me && me.Holds(brush, out node);
			brushHand = holding ? node : XRNode.Head;
			if (!holding)
			{
				brushHeld = true;
				brushTouching = false;
				Hint(me, false);
				return;
			}
			int h = node == XRNode.LeftHand ? 0 : 1;
			if (!devices[h].isValid)
			{
				devices[h] = InputDevices.GetDeviceAtXRNode(nodes[h]);
			}
			bool press = devices[h].TryGetFeatureValue(CommonUsages.secondaryButton, out bool down) && down;
			if (press && !brushHeld)
			{
				NextPaint(node);
			}
			brushHeld = press;
			bool touching = false;
			if (paintZone && brushTip && (paintZone.ClosestPoint(brushTip.position) - brushTip.position).sqrMagnitude < pokeReach * pokeReach)
			{
				if (!brushTouching)
				{
					PaintMe(node);
				}
				touching = true;
			}
			brushTouching = touching;
			Hint(me, true);
		}

		private void NextPaint(XRNode hand)
		{
			Color[] palette = Palette();
			int n = palette.Length;
			if (n == 0)
			{
				return;
			}
			for (int i = 1; i <= n; i++)
			{
				int pick = (brushPaint + i - 1) % n + 1;
				if (!NetworkPlayer.TakenByOthers(pick))
				{
					brushPaint = pick;
					break;
				}
			}
			TintBrush();
			Player.Hand(hand, 0.15f, 0.03f);
		}

		private void PaintMe(XRNode hand)
		{
			Color[] palette = Palette();
			if (brushPaint < 1 || brushPaint > palette.Length)
			{
				return;
			}
			if (NetworkPlayer.TakenByOthers(brushPaint))
			{
				Player.Hand(hand, 0.1f, 0.06f);
				return;
			}
			Color c = palette[brushPaint - 1];
			Prefs.color = brushPaint;
			Prefs.Save();
			Player main = Player.main;
			if (main && !(NetworkManager.main && NetworkManager.main.InRoom))
			{
				main.cosmetics.Tint(main.transform, c);
			}
			if (paintSplat)
			{
				GameObject fx = Instantiate(paintSplat, brushTip.position, Quaternion.identity);
				foreach (ParticleSystem ps in fx.GetComponentsInChildren<ParticleSystem>())
				{
					ParticleSystem.MainModule m = ps.main;
					m.startColor = c;
				}
				Destroy(fx, 3f);
			}
			if (paintSound)
			{
				AudioSource.PlayClipAtPoint(paintSound, brushTip.position, Prefs.sfx);
			}
			Player.Hand(hand, 0.5f, 0.08f);
			Player.Note("wardrobe paint -> " + GameManager.ColorName(brushPaint));
		}

		private void Hint(Player me, bool holding)
		{
			if (!brushHint)
			{
				return;
			}
			Transform cam = me ? me.cam : null;
			float want = 0f;
			if (holding && cam)
			{
				Vector3 to = brush.position - cam.position;
				if (to.magnitude < 2.5f && Vector3.Angle(to, cam.forward) < 70f)
				{
					want = 1f;
				}
			}
			hintAlpha = Mathf.MoveTowards(hintAlpha, want, Time.deltaTime / 0.15f);
			bool show = hintAlpha > 0f && cam;
			if (show != brushHint.gameObject.activeSelf)
			{
				brushHint.gameObject.SetActive(show);
			}
			if (!show)
			{
				return;
			}
			if (hintHand != brushHand && brushHand != XRNode.Head)
			{
				hintHand = brushHand;
				brushHint.text = "PRESS <color=#FFE14D>" + (hintHand == XRNode.LeftHand ? "Y" : "B") + "</color> TO CHANGE COLOR\nTAP THE DUMMY TO PAINT YOURSELF";
			}
			float a = Mathf.Clamp01(hintAlpha);
			float ease = 3f * a * a - 2f * a * a * a;
			brushHint.transform.localScale = Vector3.one * Mathf.Lerp(ease, 1f, 0.4f);
			Vector3 at = bristles ? new Vector3(bristles.bounds.center.x, bristles.bounds.max.y + 0.16f, bristles.bounds.center.z) : brush.position + Vector3.up * 0.2f;
			brushHint.transform.SetPositionAndRotation(at, Quaternion.LookRotation(at - cam.position));
		}

		private void FixedUpdate()
		{
			if (tags == null)
			{
				return;
			}
			Player me = Player.main;
			foreach (Tag t in tags)
			{
				if (t != null && t.rb && t.rb.gameObject.activeInHierarchy)
				{
					Pull(me, t);
				}
			}
		}

		private void Pull(Player me, Tag t)
		{
			XRNode node = XRNode.Head;
			HandGrab grab = null;
			if (me && me.Holds(t.rb, out node))
			{
				Rigidbody hand = node == XRNode.LeftHand ? me.leftHand : me.rightHand;
				grab = hand ? hand.GetComponent<HandGrab>() : null;
			}
			if (!grab)
			{
				t.pulled = false;
				return;
			}
			if (t.pulled || !t.top)
			{
				return;
			}
			if (Vector3.Distance(t.hole.position, t.top.position) - t.rest < TagPull)
			{
				return;
			}
			t.pulled = true;
			grab.Drop("price tag");
			Buy(t, node);
		}

		private static void Cord(Tag t)
		{
			if (!t.cord || !t.top || !t.hole)
			{
				return;
			}
			t.cord.SetPosition(0, t.top.position);
			t.cord.SetPosition(1, t.hole.position);
		}

		private static Vector3 HandPoint(Player me, XRNode node)
		{
			Transform target = node == XRNode.LeftHand ? me.handTargetL : me.handTargetR;
			if (target)
			{
				return target.position;
			}
			return (node == XRNode.LeftHand ? me.leftHand : me.rightHand).position;
		}

		private void Buy(Tag t, XRNode node)
		{
			string item = cosmetics.worn[t.slot];
			int cost = Account.Cost(item);
			if (cost < 1 || Account.Owned(item) || Account.Unlocking)
			{
				Player.Hand(node, 0.1f, 0.05f);
				return;
			}
			if (!Account.Ready && !Account.Practice)
			{
				Deny(t, node, 0);
				return;
			}
			if (Account.Teeth < cost)
			{
				Deny(t, node, cost - Account.Teeth);
				return;
			}
			Vector3 at = t.rb.position;
			Account.Unlock(item, ok =>
			{
				if (!this)
				{
					return;
				}
				if (!ok)
				{
					Deny(t, node, Mathf.Max(0, cost - Account.Teeth));
					return;
				}
				if (cosmetics.worn[t.slot] == item)
				{
					Prefs.outfit[t.slot] = item;
					Prefs.Save();
					if (Player.main)
					{
						Player.main.Wear(t.slot, item);
					}
				}
				Player.Note("wardrobe bought " + item + " for " + cost);
				Celebrate(at, node);
				Refresh();
			});
		}

		private void Deny(Tag t, XRNode node, int need)
		{
			t.need = need;
			t.key = int.MinValue;
			t.denyUntil = Time.time + 2f;
			if (denySound)
			{
				AudioSource.PlayClipAtPoint(denySound, t.home, Prefs.sfx);
			}
			Player.Hand(node, 0.15f, 0.15f);
		}

		private void Celebrate(Vector3 at, XRNode node)
		{
			Transform around = sway != null && sway.Length > 1 && sway[1] ? sway[1] : body;
			Account.Purchased();
			Effect(buyBurst, at, 0.8f, 4f);
			Effect(fireworks, transform.position + Vector3.up * 3.2f, 0.8f, 6f);
			Account.Spray(around, 20, false);
			Player.Hand(node, 0.8f, 0.25f);
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
			int now = Account.OwnedCount * 31 + (Account.Champion ? 1 : 0) + (Account.Ready ? 2 : 0);
			if (now != seen)
			{
				seen = now;
				Refresh();
			}
			Brush(main);
			Tags();
			if (tags != null)
			{
				foreach (Tag t in tags)
				{
					if (t != null && t.rb && t.rb.gameObject.activeInHierarchy)
					{
						Cord(t);
					}
				}
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
