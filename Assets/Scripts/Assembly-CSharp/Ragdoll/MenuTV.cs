using System;
using System.Collections.Generic;
using System.Globalization;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

namespace Ragdoll
{
	public class MenuTV : MonoBehaviour
	{
		[Serializable]
		public class Cassette
		{
			public Rigidbody tape;

			public GameObject page;

			public bool roomOnly;

			public UnityEvent onPlay;
		}

		private class Home
		{
			public Rigidbody rb;

			public Vector3 pos;

			public Quaternion rot;

			public NetProp net;

			public bool settled;

			public bool roomTape;

			public Collider[] cols;
		}

		private class Tape
		{
			public Rigidbody rb;

			public string kind;

			public bool inserted;

			public bool clear = true;

			public float slideAt;

			public Vector3 fromPos;

			public Quaternion fromRot;

			public Vector3 longAxis;

			public Vector3 labelAxis;

			public float depth;

			public bool ignoring;

			public int mode;

			public float progress;

			public float progStart;

			public float autoTo;

			public float snapAt = -9f;

			public bool leaving;

			public Vector3 handStart;

			public HandGrab by;

			public Cassette entry;

			public NetProp net;

			public Grabbable grab;

			public Renderer[] shows;

			public Collider[] cols;

			public bool visible = true;

			public readonly List<Collider> stuckMine = new List<Collider>();

			public readonly List<Collider> stuckProp = new List<Collider>();
		}

		private class RoomPage
		{
			public GameObject menu;

			public GameObject keypad;

			public GameObject busy;

			public GameObject room;

			public GameObject error;

			public GameObject create;

			public GameObject peoplePanel;

			public GameObject confirm;

			public GameObject picker;

			public GameObject[] personRows;

			public TMP_Text[] personName;

			public TMP_Text[] personState;

			public TMP_Text code;

			public TMP_Text roomCode;

			public TMP_Text players;

			public TMP_Text host;

			public TMP_Text errorText;

			public TMP_Text status;

			public TMP_Text empty;

			public TMP_Text pageText;

			public TMP_Text newKind;

			public TMP_Text kindHint;

			public TMP_Text privacy;

			public TMP_Text newPlayers;

			public TMP_Text newWins;

			public TMP_Text newMaps;

			public TMP_Text newMode;

			public Button[] mapButtons;

			public TMP_Text[] mapNames;

			public TMP_Text makeLabel;

			public GameObject editButton;

			public Button privacyButton;

			public GameObject[] rows;

			public TMP_Text[] rowName;

			public TMP_Text[] rowCount;

			public Button[] rowButton;

			public int seen = -1;

			public int seenPage = -1;

			public bool seenListed;
		}

		private class Btn
		{
			public Button button;

			public RectTransform face;

			public Image image;

			public Color color;

			public Vector2 rest;
		}

		private class Pointer
		{
			public XRNode node;

			public InputDevice device;

			public LineRenderer line;

			public Transform dot;

			public bool held;

			public Btn hover;

			public Btn down;
		}

		[Header("Setup")]
		public Transform recorder;

		public Material screenOn;

		public string gameScene = "TestScene";

		public string tutorialScene = "Tutorial";

		public bool tutorialOn;

		[Header("Tapes")]
		public Cassette[] cassettes;

		[Header("Screen")]
		public Camera screenCamera;

		public RectTransform cursor;

		public Transform volumeDots;

		public Transform turnDots;

		public Color dotOn = new Color(1f, 0.9f, 0.12f, 1f);

		public Color dotOff = new Color(0.32f, 0.45f, 0.75f, 1f);

		[Header("Sounds")]
		public AudioClip tapeIn;

		public AudioClip monitorOn;

		public AudioClip monitorOff;

		public float soundVolume = 1f;

		[Header("Respawn")]
		public GameObject poof;

		[Header("Changelog")]
		public TextAsset changelog;

		[Header("Liv Camera")]
		public GameObject livTablet;

		public Transform livSpot;

		private readonly List<Home> homes = new List<Home>();

		private float personGap = 0.4f;

		private float tapeDrop = 0.3f;

		private const int ScreenLayer = 5;

		private const float PressAt = 0.6f;

		private const float LetAt = 0.4f;

		private Player player;

		private MeshRenderer rend;

		private Material[] offMats;

		private Material[] onMats;

		private Material screenMat;

		private Mesh screenMesh;

		private RenderTexture rt;

		private Image[] volumeBars;

		private Image[] turnBars;

		private readonly List<Btn> buttons = new List<Btn>();

		private readonly List<Tape> tapes = new List<Tape>();

		private readonly List<Collider> passHand = new List<Collider>();

		private readonly List<Collider> passRec = new List<Collider>();

		private readonly List<HandGrab> passBy = new List<HandGrab>();

		private float passClear = 0.03f;

		private readonly Pointer[] pointers = new Pointer[2];

		private bool deskHeld;

		private Btn deskHover;

		private Btn deskDown;

		private Material lineMat;

		private bool lit;

		private AudioSource screenAudio;

		private AudioSource slotAudio;

		private Vector3 screenNormal;

		private Vector3 screenRight;

		private Vector3 screenUp;

		private float screenPlane;

		private float minRight;

		private float minUp;

		private float screenW;

		private float screenH;

		private int rtW = 320;

		private int rtH = 296;

		private Vector3 slotCenter;

		private Vector3 slotOut;

		private Vector3 slotAcross;

		private Vector3 slotUp;

		private float stickOut = 0.35f;

		private float snapTime = 0.12f;

		private float autoSpeed = 0.4f;

		private float railBreak = 0.3f;

		private const int Free = 0;

		private const int Rail = 1;

		private const int Auto = 2;

		private const int In = 3;

		private float reachMax = 4f;

		private float power;

		private float powerGoal;

		private float powerOn = 0.45f;

		private float powerOff = 0.3f;

		private string shown;

		private float leaveAt = -1f;

		private NetworkManager boot;

		private string typed = "";

		private bool keypad;

		private bool creating;

		private bool people;

		private int peopleSeen = -1;

		private readonly List<NetworkPlayer> shownPeople = new List<NetworkPlayer>();

		private bool newOpen = true;

		private int newPlayers = 8;

		private int newWins = 8;

		private int newMode;

		private static readonly string[] Modes = { "NORMAL", "TEAMS", "ROUNDS" };

		private const int TeamsMode = 1;

		private const int RoundsMode = 2;

		private static readonly string[] ModeTints = { "#D9E4EC", "#A6FF8F", "#FFB347" };

		private bool picking;

		private bool editing;

		private bool tutorialDue;

		private Transform welcome;

		private int welcomeStep;

		private TMP_Text welcomeTitle;

		private TMP_Text welcomeLine;

		private GameObject welcomeKeys;

		private GameObject welcomeChoice;

		private Btn welcomeDone;

		private readonly List<Btn> welcomeButtons = new List<Btn>();

		private string typedName = "";

		private bool welcomeTyping;

		private const int NameStep = 1;

		private const int TutorialStep = 2;

		private const int NameMax = 12;

		private const float PanelScale = 0.0028f;

		private const float PanelAhead = 1.15f;

		private const float RayLength = 4f;

		private const string KeyRows = "1234567890|QWERTYUIOP|ASDFGHJKL|ZXCVBNM";

		private const float OceanAway = 150f;

		private const float OceanEye = 2.2f;

		private Vector3 welcomeShift;

		private Vector3 welcomeHome;

		private const string SettingsKind = "SettingsTape";

		private TMP_Text deadValue;

		private readonly float[] deadSteps = { 0.15f, 0.25f, 0.35f, 0.5f };

		private readonly List<string> skipped = new List<string>();

		private static readonly Color MapOn = new Color(0.128f, 0.723f, 0.931f, 1f);

		private static readonly Color MapOff = Color.white;

		private static readonly Color MapOnShade = new Color(0.092f, 0.459f, 0.584f, 1f);

		private static readonly Color MapOffShade = new Color(0.468f, 0.473f, 0.554f, 1f);

		private int listPage;

		private readonly List<NetworkManager.Room> shownRooms = new List<NetworkManager.Room>();

		private readonly Dictionary<GameObject, RoomPage> roomPages = new Dictionary<GameObject, RoomPage>();

		private Image[] musicBars;

		private Image[] sfxBars;

		private GameObject soundPanel;

		private GameObject controlsPanel;

		private GameObject gamePanel;

		private GameObject soundMark;

		private GameObject controlsMark;

		private GameObject gameMark;

		private GameObject notesPrev;

		private GameObject notesNext;

		private TMP_Text moveValue;

		private TMP_Text turnValue;

		private TMP_Text hapticsValue;

		private TMP_Text sticksValue;

		private TMP_Text speedLabel;

		private TMP_Text voicesValue;

		private TMP_Text micValue;

		private TMP_Text myVoiceValue;

		private TMP_Text regionValue;

		private TMP_Text kickValue;

		private TMP_Text livValue;

		private TMP_Text notesText;

		private TMP_Text notesPage;

		private TMP_Text shopTeeth;

		private TMP_Text shopStatus;

		private TMP_Text[] shopPrices;

		private int shopTeethShown = -1;

		private int tab;

		private float canvasScale = 1f;

		private int turnIndex = 2;

		private int notesAt = 1;

		private int livShown = -2;

		private readonly float[] turnSpeeds = { 90f, 135f, 180f, 240f, 300f };

		private NetworkManager.Room confirmRoom;

		public static bool TutorialOn { get; private set; }

		private void Awake()
		{
			rend = GetComponent<MeshRenderer>();
			offMats = rend.sharedMaterials;
			int slot = ScreenSlot(offMats);
			MeshFilter mf = GetComponent<MeshFilter>();
			screenMesh = ScreenMesh(mf.sharedMesh, slot);
			mf.sharedMesh = screenMesh;
			if (screenCamera && screenCamera.transform.parent)
			{
				Transform holder = screenCamera.transform.parent;
				holder.localScale = holder.localScale / Mathf.Max(holder.lossyScale.x, 0.0001f);
			}
			rt = screenCamera ? screenCamera.targetTexture : null;
			if (!rt)
			{
				rt = new RenderTexture(rtW, rtH, 16, RenderTextureFormat.ARGB32);
				rt.filterMode = FilterMode.Point;
				if (screenCamera)
				{
					screenCamera.targetTexture = rt;
				}
			}
			rtW = rt.width;
			rtH = rt.height;
			screenMat = new Material(screenOn);
			screenMat.SetTexture("_MainTex", rt);
			screenMat.SetVector("_Cells", new Vector4(rtW, rtH, 0f, 0f));
			screenMat.SetFloat("_Power", 0f);
			Material off = offMats[slot];
			if (off && off.HasProperty("_TopColor"))
			{
				screenMat.SetColor("_OffTop", off.GetColor("_TopColor"));
			}
			if (off && off.HasProperty("_BottomColor"))
			{
				screenMat.SetColor("_OffBottom", off.GetColor("_BottomColor"));
			}
			onMats = (Material[])offMats.Clone();
			onMats[slot] = screenMat;
			if (recorder)
			{
				MeasureSlot();
			}
			screenAudio = Speaker(gameObject);
			slotAudio = recorder ? Speaker(recorder.gameObject) : screenAudio;
			TutorialOn = tutorialOn;
			if (cassettes != null)
			{
				foreach (Cassette c in cassettes)
				{
					if (c != null && !tutorialOn && c.tape && Plays(c, "PlayTutorial"))
					{
						c.tape.gameObject.SetActive(false);
					}
					else
					{
						AddTape(c);
					}
				}
			}
			Prefs.Load();
			int index = Array.IndexOf(turnSpeeds, PlayerPrefs.GetFloat("turnSpeed", 180f));
			turnIndex = index >= 0 ? index : 2;
			BindScreen();
		}

		private void Start()
		{
			foreach (Rigidbody rb in FindObjectsByType<Rigidbody>(FindObjectsSortMode.None))
			{
				if (!rb.GetComponent<Grabbable>() || rb.GetComponentInParent<Player>() || rb.GetComponentInParent<NetworkPlayer>() || rb.GetComponentInParent<NetRope>())
				{
					continue;
				}
				homes.Add(new Home
				{
					rb = rb,
					pos = rb.position,
					rot = rb.rotation,
					net = rb.GetComponent<NetProp>(),
					cols = rb.GetComponentsInChildren<Collider>(true),
					roomTape = tapes.Exists((Tape t) => t.rb == rb && t.entry.roomOnly)
				});
			}
			AvatarBody.peaceful = true;
			boot = FindFirstObjectByType<NetworkManager>();
			player = null;
			foreach (Player p in FindObjectsByType<Player>(FindObjectsSortMode.None))
			{
				if (p.playSpace && !p.dummy)
				{
					player = p;
				}
			}
			foreach (Camera cam in FindObjectsByType<Camera>(FindObjectsSortMode.None))
			{
				if (cam != screenCamera)
				{
					cam.cullingMask &= ~(1 << ScreenLayer);
				}
			}
			Prefs.Apply();
			ApplyTurn();
			if (((tutorialOn && !Prefs.tutorialDone) || NeedsName()) && boot && SceneManager.GetActiveScene().name != tutorialScene && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PCGAME_SESSION")))
			{
				if (player)
				{
					player.dark = 1f;
					player.blackout = 1f;
				}
				tutorialDue = true;
			}
			Shader shader = Shader.Find("Sprites/Default");
			if (!shader)
			{
				shader = Shader.Find("Universal Render Pipeline/Unlit");
			}
			lineMat = new Material(shader);
			pointers[0] = MakePointer(XRNode.LeftHand);
			pointers[1] = MakePointer(XRNode.RightHand);
		}

		private void OnDestroy()
		{
			AvatarBody.peaceful = false;
			if (screenMesh)
			{
				Destroy(screenMesh);
			}
			if (welcomeTyping)
			{
				Keyboard keyboard = Keyboard.current;
				if (keyboard != null)
				{
					keyboard.onTextInput -= Typed;
				}
			}
			welcomeTyping = false;
		}

		private static int ScreenSlot(Material[] mats)
		{
			int slot = 0;
			for (int i = 0; i < mats.Length; i++)
			{
				if (mats[i] && mats[i].name.Contains("Screen"))
				{
					slot = i;
				}
			}
			return slot;
		}

		public Mesh PreviewScreen(out int slot, out Vector3 lift)
		{
			slot = ScreenSlot(GetComponent<MeshRenderer>().sharedMaterials);
			Mesh mesh = ScreenMesh(GetComponent<MeshFilter>().sharedMesh, slot);
			mesh.hideFlags = HideFlags.DontSave;
			lift = screenNormal * 0.002f;
			return mesh;
		}

		private Mesh ScreenMesh(Mesh source, int slot)
		{
			Mesh mesh = Instantiate(source);
			int[] tris = mesh.GetTriangles(slot);
			Vector3[] verts = mesh.vertices;
			Vector3[] normals = mesh.normals;
			Vector2[] uv = mesh.uv;
			if (uv.Length != verts.Length)
			{
				uv = new Vector2[verts.Length];
			}
			HashSet<int> used = new HashSet<int>(tris);
			Vector3 sum = Vector3.zero;
			foreach (int i in used)
			{
				sum += normals[i];
			}
			screenNormal = sum.normalized;
			Vector3 up = transform.InverseTransformDirection(Vector3.up);
			Vector3 flat = up - screenNormal * Vector3.Dot(up, screenNormal);
			screenUp = flat.sqrMagnitude > 0.0001f ? flat.normalized : Vector3.Cross(screenNormal, Vector3.right).normalized;
			screenRight = Vector3.Cross(screenNormal, screenUp).normalized;
			screenPlane = float.MinValue;
			minRight = float.MaxValue;
			minUp = float.MaxValue;
			float maxRight = float.MinValue;
			float maxUp = float.MinValue;
			foreach (int i in used)
			{
				float x = Vector3.Dot(verts[i], screenRight);
				float y = Vector3.Dot(verts[i], screenUp);
				minRight = Mathf.Min(minRight, x);
				minUp = Mathf.Min(minUp, y);
				maxRight = Mathf.Max(maxRight, x);
				maxUp = Mathf.Max(maxUp, y);
				screenPlane = Mathf.Max(screenPlane, Vector3.Dot(verts[i], screenNormal));
			}
			screenW = maxRight - minRight;
			screenH = maxUp - minUp;
			foreach (int i in used)
			{
				uv[i] = new Vector2((Vector3.Dot(verts[i], screenRight) - minRight) / screenW, (Vector3.Dot(verts[i], screenUp) - minUp) / screenH);
			}
			mesh.uv = uv;
			return mesh;
		}

		private void MeasureSlot()
		{
			MeshFilter mf = recorder.GetComponent<MeshFilter>();
			MeshRenderer mr = recorder.GetComponent<MeshRenderer>();
			if (!mf || !mr)
			{
				return;
			}
			int hole = -1;
			for (int i = 0; i < mr.sharedMaterials.Length; i++)
			{
				if (mr.sharedMaterials[i] && mr.sharedMaterials[i].name.Contains("Hole"))
				{
					hole = i;
				}
			}
			Mesh mesh = mf.sharedMesh;
			if (hole < 0 || hole >= mesh.subMeshCount)
			{
				return;
			}
			Vector3[] verts = mesh.vertices;
			Vector3[] normals = mesh.normals;
			HashSet<int> used = new HashSet<int>(mesh.GetTriangles(hole));
			Vector3 normal = Vector3.zero;
			Bounds box = default;
			bool first = true;
			foreach (int i in used)
			{
				normal += normals[i];
				if (first)
				{
					box = new Bounds(verts[i], Vector3.zero);
					first = false;
				}
				else
				{
					box.Encapsulate(verts[i]);
				}
			}
			slotCenter = box.center;
			slotOut = Axis(normal);
			Vector3 size = box.size;
			Vector3 across = Vector3.zero;
			Vector3 up = Vector3.zero;
			for (int a = 0; a < 3; a++)
			{
				Vector3 axis = Vector3.zero;
				axis[a] = 1f;
				if (Mathf.Abs(Vector3.Dot(axis, slotOut)) > 0.9f)
				{
					continue;
				}
				if (across == Vector3.zero || size[a] > Vector3.Dot(size, across))
				{
					up = across;
					across = axis;
				}
				else
				{
					up = axis;
				}
			}
			slotAcross = across;
			Vector3 worldUp = recorder.InverseTransformDirection(Vector3.up);
			slotUp = Vector3.Dot(up, worldUp) < 0f ? -up : up;
		}

		private static Vector3 Axis(Vector3 d)
		{
			float x = Mathf.Abs(d.x);
			float y = Mathf.Abs(d.y);
			float z = Mathf.Abs(d.z);
			if (x >= y && x >= z)
			{
				return new Vector3(Mathf.Sign(d.x), 0f, 0f);
			}
			if (y >= z)
			{
				return new Vector3(0f, Mathf.Sign(d.y), 0f);
			}
			return new Vector3(0f, 0f, Mathf.Sign(d.z));
		}

		private void AddTape(Cassette c)
		{
			Rigidbody rb = c?.tape;
			if (!rb)
			{
				return;
			}
			Tape t = new Tape
			{
				rb = rb,
				kind = rb.name,
				entry = c,
				net = rb.GetComponent<NetProp>(),
				grab = rb.GetComponent<Grabbable>(),
				shows = rb.GetComponentsInChildren<Renderer>(true),
				cols = rb.GetComponentsInChildren<Collider>(true)
			};
			MeshFilter mf = rb.GetComponent<MeshFilter>();
			Vector3 size = mf ? mf.sharedMesh.bounds.size : Vector3.one;
			int lng = size.y > size.x ? 1 : 0;
			if (size.z > size[lng])
			{
				lng = 2;
			}
			int thin = size.y < size.x ? 1 : 0;
			if (size.z < size[thin])
			{
				thin = 2;
			}
			t.longAxis = Vector3.zero;
			t.longAxis[lng] = 1f;
			t.labelAxis = Vector3.zero;
			t.labelAxis[thin] = 1f;
			TMP_Text label = rb.GetComponentInChildren<TMP_Text>();
			if (label && rb.transform.InverseTransformPoint(label.transform.position)[thin] < 0f)
			{
				t.labelAxis = -t.labelAxis;
			}
			t.depth = size[3 - lng - thin] * rb.transform.lossyScale.x;
			tapes.Add(t);
		}

		private void FixedUpdate()
		{
			Rescue();
			if (!recorder)
			{
				return;
			}
			tapes.RemoveAll((Tape t) => !t.rb);
			ClearHands();
			foreach (Tape t in tapes)
			{
				if (t.entry.roomOnly)
				{
					bool inRoom = boot && boot.InRoom;
					Show(t, inRoom);
					if (t.grab)
					{
						t.grab.enabled = inRoom && boot.IsHost;
					}
					if (!inRoom)
					{
						continue;
					}
				}
				if (t.net && t.net.Object && t.net.Object.IsValid && !t.net.HasStateAuthority)
				{
					Follow(t);
					continue;
				}
				switch (t.mode)
				{
					case Free:
						FreeStep(t);
						continue;
					case Rail:
						RailStep(t);
						break;
					case Auto:
						t.progress = Mathf.MoveTowards(t.progress, t.autoTo, autoSpeed * Time.fixedDeltaTime);
						if (t.progress >= Full(t))
						{
							Click(t);
						}
						else if (t.autoTo < 0f && t.progress <= t.autoTo)
						{
							Eject(t);
						}
						break;
					case In:
					{
						HandGrab g = Holder(t.rb);
						if (g)
						{
							Board(t, g);
						}
						else if (shown != t.kind && Time.time >= t.slideAt + (tapeIn ? tapeIn.length : 0f))
						{
							Power(t.kind);
						}
						break;
					}
					default:
						Place(t);
						continue;
				}
				if (t.mode != Free)
				{
					Place(t);
				}
			}
		}

		private void Rescue()
		{
			float water = Water.main ? Water.main.Surface : -5f;
			foreach (Home h in homes)
			{
				Rigidbody rb = h.rb;
				if (!rb || rb.isKinematic)
				{
					continue;
				}
				if (!h.settled && rb.IsSleeping() && (!h.roomTape || rb.position.y > h.pos.y - tapeDrop))
				{
					h.settled = true;
					h.pos = rb.position;
					h.rot = rb.rotation;
				}
				float low = float.MaxValue;
				foreach (Collider c in h.cols)
				{
					if (c && c.enabled && !c.isTrigger)
					{
						low = Mathf.Min(low, c.bounds.min.y);
					}
				}
				bool fell = h.roomTape && rb.position.y < h.pos.y - tapeDrop && rb.linearVelocity.sqrMagnitude < 0.01f;
				if (low > water && !fell)
				{
					continue;
				}
				bool networked = h.net && h.net.Object && h.net.Object.IsValid;
				if (Holder(rb))
				{
					continue;
				}
				if (networked && (!h.net.HasStateAuthority || h.net.ShownHeld))
				{
					continue;
				}
				bool clear = false;
				float lift = 0f;
				Vector3 at = h.pos;
				for (; lift <= 2.5f; lift += 0.25f)
				{
					at = h.pos + Vector3.up * lift;
					clear = Clear(h, at, h.rot, out bool person);
					if (person || clear)
					{
						break;
					}
				}
				if (!clear)
				{
					continue;
				}
				rb.linearVelocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				rb.transform.SetPositionAndRotation(at, h.rot);
				rb.position = at;
				rb.rotation = h.rot;
				NetworkTransform nt = networked ? rb.GetComponent<NetworkTransform>() : null;
				if (nt)
				{
					nt.Teleport(at, h.rot);
				}
				if (poof)
				{
					Instantiate(poof, rb.worldCenterOfMass, Quaternion.identity);
				}
				Player.Note(rb.name + (low > water ? " fell off the table" : " fell off the map") + ", sent back home" + (at == h.pos ? "" : (" (" + lift.ToString("F2") + " m up, something is on its spot)")));
			}
		}

		private bool Clear(Home h, Vector3 pos, Quaternion rot, out bool person)
		{
			person = false;
			Transform root = h.rb.transform;
			Quaternion inv = Quaternion.Inverse(root.rotation);
			bool clear = true;
			foreach (Collider c in h.cols)
			{
				if (!c || !c.enabled || c.isTrigger)
				{
					continue;
				}
				Transform ct = c.transform;
				Vector3 cpos = pos + rot * (inv * (ct.position - root.position));
				Quaternion crot = rot * (inv * ct.rotation);
				Vector3 center = cpos + rot * (inv * (c.bounds.center - ct.position));
				foreach (Collider o in Physics.OverlapBox(center, c.bounds.extents + Vector3.one * personGap, Quaternion.identity, -1, QueryTriggerInteraction.Ignore))
				{
					if (o.GetComponentInParent<Player>() || o.GetComponentInParent<AvatarBody>())
					{
						person = true;
						return false;
					}
				}
				foreach (Collider o in Physics.OverlapSphere(center, c.bounds.extents.magnitude + 0.05f, -1, QueryTriggerInteraction.Ignore))
				{
					if (o.attachedRigidbody == h.rb)
					{
						continue;
					}
					if (!Physics.ComputePenetration(c, cpos, crot, o, o.transform.position, o.transform.rotation, out Vector3 _, out float dist) || dist < 0.04f)
					{
						continue;
					}
					if (o.GetComponentInParent<Player>() || o.GetComponentInParent<AvatarBody>())
					{
						person = true;
						return false;
					}
					clear = false;
				}
			}
			return clear;
		}

		private void Show(Tape t, bool on)
		{
			if (t.visible == on)
			{
				return;
			}
			t.visible = on;
			foreach (Renderer r in t.shows)
			{
				if (r)
				{
					r.enabled = on;
				}
			}
			foreach (Collider c in t.cols)
			{
				if (c)
				{
					c.enabled = on;
				}
			}
			if (on)
			{
				AvatarBody.Unshare(t.cols);
				if (!t.net)
				{
					NetProp.Unshare(t.cols);
				}
			}
			if (t.mode == Free)
			{
				t.rb.isKinematic = !on;
			}
		}

		private void FreeStep(Tape t)
		{
			if (t.stuckMine.Count > 0)
			{
				Unstick(t);
			}
			if (!Near(t, 1.6f))
			{
				t.clear = true;
				if (t.ignoring)
				{
					Ignore(t, false);
				}
				return;
			}
			HandGrab g = Holder(t.rb);
			if (g && t.clear && Near(t, 1f) && Lined(t) && !Busy(t))
			{
				Board(t, g);
			}
		}

		private void Follow(Tape t)
		{
			RailPose(t, Full(t), out Vector3 pos, out Quaternion rot);
			bool seated = (t.rb.position - pos).sqrMagnitude < 0.0009f && Quaternion.Angle(t.rb.rotation, rot) < 10f;
			if (t.mode == In)
			{
				if (!Holder(t.rb) && !seated)
				{
					if (shown == t.kind)
					{
						Power(null);
					}
					t.mode = Free;
					t.inserted = false;
					return;
				}
			}
			else
			{
				if (!seated)
				{
					t.mode = Free;
					t.inserted = false;
					return;
				}
				t.mode = In;
				t.inserted = true;
				t.progress = Full(t);
				t.slideAt = Time.time;
			}
			if (shown != t.kind && Time.time >= t.slideAt + (tapeIn ? tapeIn.length : 0f))
			{
				Power(t.kind, false);
			}
		}

		private void Board(Tape t, HandGrab g)
		{
			if (t.mode == Free)
			{
				t.progStart = Mathf.Clamp(t.depth * 0.5f - Along(t), 0f, Full(t) * 0.5f);
				t.snapAt = Time.time;
				t.fromPos = t.rb.position;
				t.fromRot = t.rb.rotation;
				t.rb.linearVelocity = Vector3.zero;
				t.rb.angularVelocity = Vector3.zero;
				t.rb.isKinematic = true;
				t.leaving = false;
				Ignore(t, true);
			}
			else
			{
				t.progStart = t.progress;
				t.leaving = t.mode == In;
				t.rb.isKinematic = true;
				if (!t.ignoring)
				{
					Ignore(t, true);
				}
			}
			Player.Note("tape " + t.kind + " boarded from mode " + t.mode + (t.leaving ? " (leaving)" : "") + " at " + t.progStart.ToString("F3") + " of " + Full(t).ToString("F3"));
			t.by = g;
			g.Soften();
			PassHand(g);
			t.handStart = HandPoint(g);
			t.progress = t.progStart;
			t.mode = Rail;
		}

		private void RailStep(Tape t)
		{
			HandGrab by = t.by;
			if (by && by.grabbing && by.held == t.rb)
			{
				foreach (HandGrab other in Hands())
				{
					if (other && other != by && other.grabbing && other.held == t.rb)
					{
						other.Drop("vhs");
					}
				}
				by.Soften();
				Vector3 o = Out();
				Vector3 moved = HandPoint(by) - t.handStart;
				float push = Vector3.Dot(moved, -o);
				float at = t.progStart + push;
				if (at < -0.04f || (moved + o * push).magnitude > railBreak)
				{
					Eject(t);
					return;
				}
				t.progress = Mathf.Clamp(at, 0f, Full(t));
				if (t.leaving && t.progress < Full(t) - 0.04f)
				{
					t.leaving = false;
				}
				if (shown == t.kind && t.progress < Full(t) - 0.005f)
				{
					Power(null);
				}
				if (!t.leaving && t.progress >= Full(t))
				{
					Click(t);
				}
				return;
			}
			t.autoTo = (t.progress >= Full(t) * 0.5f && !t.leaving) ? Full(t) : -0.01f;
			t.mode = Auto;
		}

		private void Click(Tape t)
		{
			foreach (HandGrab g in Hands())
			{
				if (g && g.grabbing && g.held == t.rb)
				{
					g.Drop("vhs");
				}
			}
			t.mode = In;
			t.inserted = true;
			t.progress = Full(t);
			t.slideAt = Time.time;
			t.by = null;
			if (tapeIn)
			{
				slotAudio.PlayOneShot(tapeIn, soundVolume);
			}
			Player.Note("tape " + t.kind + " in" + Body());
		}

		private string Body()
		{
			if (!player || !player.pelvis)
			{
				return "";
			}
			return ", body at " + player.pelvis.linearVelocity.magnitude.ToString("F1") + " m/s";
		}

		private void Eject(Tape t)
		{
			if (shown == t.kind)
			{
				Power(null);
			}
			t.by = null;
			int was = t.mode;
			t.mode = Free;
			t.inserted = false;
			HandGrab g = Holder(t.rb);
			if (g)
			{
				g.Harden();
			}
			t.rb.isKinematic = false;
			if (was != Free)
			{
				Player.Note("tape " + t.kind + " out" + Body());
			}
		}

		private void Place(Tape t)
		{
			RailPose(t, t.progress, out Vector3 pos, out Quaternion rot);
			float k = Mathf.Clamp01((Time.time - t.snapAt) / snapTime);
			if (k < 1f)
			{
				float ease = k * k * (3f - 2f * k);
				pos = Vector3.Lerp(t.fromPos, pos, ease);
				rot = Quaternion.Slerp(t.fromRot, rot, ease);
			}
			t.rb.MovePosition(pos);
			t.rb.MoveRotation(rot);
		}

		private float Full(Tape t)
		{
			return t.depth * (1f - stickOut);
		}

		private Vector3 Out()
		{
			return recorder.TransformDirection(slotOut).normalized;
		}

		private float Along(Tape t)
		{
			return Vector3.Dot(t.rb.worldCenterOfMass - recorder.TransformPoint(slotCenter), Out());
		}

		private Vector3 HandPoint(HandGrab g)
		{
			Transform hand = g.node == XRNode.LeftHand ? player.handTargetL : player.handTargetR;
			return hand ? hand.position : g.transform.position;
		}

		private static AudioSource Speaker(GameObject on)
		{
			AudioSource source = on.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.spatialBlend = 1f;
			source.rolloffMode = AudioRolloffMode.Linear;
			source.minDistance = 4f;
			source.maxDistance = 25f;
			return source;
		}

		private bool Busy(Tape except)
		{
			foreach (Tape t in tapes)
			{
				if (t != except && t.mode != Free)
				{
					return true;
				}
			}
			return false;
		}

		private HandGrab Holder(Rigidbody rb)
		{
			foreach (HandGrab g in Hands())
			{
				if (g && g.grabbing && g.held == rb)
				{
					return g;
				}
			}
			return null;
		}

		private IEnumerable<HandGrab> Hands()
		{
			if (!player)
			{
				yield break;
			}
			if (player.leftHand)
			{
				yield return player.leftHand.GetComponent<HandGrab>();
			}
			if (player.rightHand)
			{
				yield return player.rightHand.GetComponent<HandGrab>();
			}
		}

		private bool Near(Tape t, float grow)
		{
			Vector3 center = recorder.TransformPoint(slotCenter);
			Vector3 p = t.rb.worldCenterOfMass;
			Vector3 o = Out();
			Vector3 across = recorder.TransformDirection(slotAcross).normalized;
			Vector3 up = recorder.TransformDirection(slotUp).normalized;
			Vector3 d = p - center;
			float along = Vector3.Dot(d, o);
			return along > grow * -0.02f && along < t.depth * 0.5f + FaceGap(center, o) + grow * 0.06f && Mathf.Abs(Vector3.Dot(d, across)) < grow * 0.1f && Mathf.Abs(Vector3.Dot(d, up)) < grow * 0.08f;
		}

		private float FaceGap(Vector3 center, Vector3 outward)
		{
			float gap = 0f;
			foreach (Collider c in recorder.GetComponentsInChildren<Collider>())
			{
				if (c && c.enabled && !c.isTrigger)
				{
					gap = Mathf.Max(gap, Vector3.Dot(outward, c.ClosestPoint(center + outward) - center));
				}
			}
			return gap;
		}

		private bool Lined(Tape t)
		{
			Vector3 lng = t.rb.transform.TransformDirection(t.longAxis).normalized;
			Vector3 lbl = t.rb.transform.TransformDirection(t.labelAxis).normalized;
			Vector3 across = recorder.TransformDirection(slotAcross).normalized;
			if (Mathf.Abs(Vector3.Dot(lng, across)) <= 0.7f)
			{
				return false;
			}
			return Vector3.Dot(lbl, recorder.TransformDirection(slotUp).normalized) > 0.5f;
		}

		private void RailPose(Tape t, float progress, out Vector3 pos, out Quaternion rot)
		{
			pos = recorder.TransformPoint(slotCenter) + Out() * (t.depth * 0.5f - progress);
			Quaternion local = Quaternion.LookRotation(t.longAxis, t.labelAxis);
			Vector3 across = recorder.TransformDirection(slotAcross).normalized;
			Vector3 lng = t.rb.transform.TransformDirection(t.longAxis);
			Vector3 up = recorder.TransformDirection(slotUp).normalized;
			rot = Quaternion.LookRotation(Vector3.Dot(across, lng) < 0f ? -across : across, up) * Quaternion.Inverse(local);
		}

		private void PassHand(HandGrab g)
		{
			if (!g || !player)
			{
				return;
			}
			Rigidbody arm = g.node == XRNode.LeftHand ? player.leftForearm : player.rightForearm;
			foreach (Rigidbody rb in new Rigidbody[2] { g.GetComponent<Rigidbody>(), arm })
			{
				if (!rb)
				{
					continue;
				}
				foreach (Collider a in rb.GetComponents<Collider>())
				{
					foreach (Collider b in recorder.GetComponentsInChildren<Collider>())
					{
						if (!a || !b || a.isTrigger || b.isTrigger || Physics.GetIgnoreCollision(a, b))
						{
							continue;
						}
						Physics.IgnoreCollision(a, b, true);
						passHand.Add(a);
						passRec.Add(b);
						passBy.Add(g);
					}
				}
			}
		}

		private void ClearHands()
		{
			foreach (Tape t in tapes)
			{
				if (t.mode == Rail)
				{
					return;
				}
			}
			for (int i = passHand.Count - 1; i >= 0; i--)
			{
				Collider a = passHand[i];
				Collider b = passRec[i];
				if (a && b)
				{
					Vector3 at = passBy[i] ? HandPoint(passBy[i]) : a.bounds.center;
					if ((b.ClosestPoint(at) - at).sqrMagnitude < passClear * passClear)
					{
						continue;
					}
					if (Physics.ComputePenetration(a, a.transform.position, a.transform.rotation, b, b.transform.position, b.transform.rotation, out Vector3 _, out float _))
					{
						continue;
					}
					if ((b.ClosestPoint(a.bounds.center) - a.ClosestPoint(b.bounds.center)).sqrMagnitude < passClear * passClear)
					{
						continue;
					}
					Physics.IgnoreCollision(a, b, false);
				}
				passHand.RemoveAt(i);
				passRec.RemoveAt(i);
				passBy.RemoveAt(i);
			}
		}

		private void Ignore(Tape t, bool on)
		{
			t.ignoring = on;
			t.stuckMine.Clear();
			t.stuckProp.Clear();
			Collider[] mine = t.rb.GetComponentsInChildren<Collider>();
			foreach (Collider a in mine)
			{
				foreach (Collider b in recorder.GetComponentsInChildren<Collider>())
				{
					if (a && b)
					{
						Physics.IgnoreCollision(a, b, on);
					}
				}
			}
			foreach (Grabbable g in FindObjectsByType<Grabbable>(FindObjectsSortMode.None))
			{
				if (!AvatarBody.Prop(g) || g.GetComponent<Rigidbody>().isKinematic || g.transform == t.rb.transform || g.transform.IsChildOf(recorder))
				{
					continue;
				}
				foreach (Collider a in mine)
				{
					if (!a || !a.enabled)
					{
						continue;
					}
					foreach (Collider b in g.GetComponentsInChildren<Collider>())
					{
						if (!b || !b.enabled)
						{
							continue;
						}
						if (on)
						{
							Physics.IgnoreCollision(a, b, true);
							continue;
						}
						bool stuck = Overlapping(a, b);
						Physics.IgnoreCollision(a, b, stuck);
						if (stuck)
						{
							t.stuckMine.Add(a);
							t.stuckProp.Add(b);
						}
					}
				}
			}
		}

		private void Unstick(Tape t)
		{
			for (int i = t.stuckMine.Count - 1; i >= 0; i--)
			{
				Collider a = t.stuckMine[i];
				Collider b = t.stuckProp[i];
				if (a && b && Overlapping(a, b))
				{
					continue;
				}
				if (a && b)
				{
					Physics.IgnoreCollision(a, b, false);
				}
				t.stuckMine.RemoveAt(i);
				t.stuckProp.RemoveAt(i);
			}
		}

		private static bool Overlapping(Collider a, Collider b)
		{
			return Physics.ComputePenetration(a, a.transform.position, a.transform.rotation, b, b.transform.position, b.transform.rotation, out Vector3 _, out float depth) && depth > 0.005f;
		}

		private void Power(string kind, bool act = true)
		{
			shown = kind;
			AudioClip clip = kind == null ? (powerGoal > 0f ? monitorOff : null) : (powerGoal > 0f ? null : monitorOn);
			if (clip)
			{
				screenAudio.PlayOneShot(clip, soundVolume);
			}
			powerGoal = kind == null ? 0f : 1f;
			if (kind == null)
			{
				return;
			}
			Cassette playing = null;
			foreach (Cassette c in cassettes)
			{
				bool mine = c.tape && c.tape.name == kind;
				if (c.page)
				{
					c.page.SetActive(mine);
				}
				if (mine)
				{
					playing = c;
				}
			}
			keypad = false;
			if (act && playing != null && playing.onPlay != null)
			{
				playing.onPlay.Invoke();
			}
			if (!lit)
			{
				rend.sharedMaterials = onMats;
			}
			lit = true;
			if (screenCamera)
			{
				screenCamera.enabled = true;
			}
			ShowSettings();
			if (notesText && notesText.gameObject.activeInHierarchy)
			{
				ShowNotes(1);
			}
		}

		private void Update()
		{
			if (tutorialDue && !GameManager.BetaShowing)
			{
				tutorialDue = false;
				if (NeedsName())
				{
					Welcome(NameStep);
				}
				else if (!Pc())
				{
					Welcome(TutorialStep);
				}
				else
				{
					Prefs.tutorialDone = true;
					Prefs.Save();
					if (player)
					{
						player.blackout = 0f;
					}
				}
			}
			RefreshRooms();
			ShowShop();
			power = Mathf.MoveTowards(power, powerGoal, Time.deltaTime / (powerGoal > power ? powerOn : powerOff));
			screenMat.SetFloat("_Power", power);
			if (lit && power <= 0f && powerGoal <= 0f)
			{
				rend.sharedMaterials = offMats;
				if (screenCamera)
				{
					screenCamera.enabled = false;
				}
				lit = false;
			}
			if (!livValue)
			{
				return;
			}
			int liv = LivCamera.State(livSpot);
			if (liv != livShown)
			{
				livShown = liv;
				livValue.text = liv < 0 ? "UNSUPPORTED" : (liv == 0 ? "SPAWN" : (liv == 1 ? "BRING BACK" : "PUT AWAY"));
			}
		}

		private void LateUpdate()
		{
			if (welcomeStep > 0)
			{
				PointWelcome();
				return;
			}
			bool usable = shown != null && power >= 1f && leaveAt < 0f;
			bool show = false;
			Vector2 shownPx = Vector2.zero;
			if (player && player.inVR)
			{
				foreach (Pointer p in pointers)
				{
					Vector3 from = Vector3.zero;
					Vector3 dir = Vector3.forward;
					Vector3 at = Vector3.zero;
					Vector2 uv = Vector2.zero;
					if (usable && Aim(p, out from, out dir))
					{
						bool hit = HitScreen(from, dir, out uv, out at);
						SetLine(p, hit, from, at);
						if (hit)
						{
							Vector2 px = uv * new Vector2(rtW, rtH);
							if (!show || p.node == XRNode.RightHand)
							{
								shownPx = px;
							}
							Hover(p, Under(px));
							float trigger = p.device.TryGetFeatureValue(CommonUsages.trigger, out float squeeze) ? squeeze : 0f;
							bool down = p.held ? trigger > 0.4f : trigger > 0.6f;
							if (down)
							{
								if (!p.held)
								{
									p.down = p.hover;
									Press(p.down, true);
								}
							}
							else if (p.held)
							{
								Press(p.down, false);
								if (p.down != null && p.down == p.hover)
								{
									Click(p.down, p.node);
								}
								p.down = null;
							}
							p.held = down;
							show = true;
							continue;
						}
					}
					else
					{
						SetLine(p, false, from, at);
					}
					Hover(p, null);
					Press(p.down, false);
					p.down = null;
					p.held = p.device.isValid && p.device.TryGetFeatureValue(CommonUsages.trigger, out float held) && held > (p.held ? 0.4f : 0.6f);
				}
			}
			else
			{
				foreach (Pointer p in pointers)
				{
					SetLine(p, false, Vector3.zero, Vector3.zero);
				}
				Transform view = (player && player.cam) ? player.cam : (Camera.main ? Camera.main.transform : null);
				if (usable)
				{
					Mouse mouse = Mouse.current;
					if (view && HitScreen(view.position, view.forward, out Vector2 uv, out Vector3 _))
					{
						Vector2 px = uv * new Vector2(rtW, rtH);
						shownPx = px;
						Btn over = Under(px);
						if (over != deskHover)
						{
							Glow(deskHover, false);
							Glow(over, true);
							deskHover = over;
						}
						bool down = mouse != null && mouse.leftButton.isPressed;
						if (down)
						{
							if (!deskHeld)
							{
								deskDown = deskHover;
								Press(deskDown, true);
							}
						}
						else if (deskHeld)
						{
							Press(deskDown, false);
							if (deskDown != null && deskDown == deskHover)
							{
								Click(deskDown, XRNode.Head);
							}
							deskDown = null;
						}
						show = true;
						deskHeld = down;
						goto Cursor;
					}
				}
				Glow(deskHover, false);
				deskHover = null;
				Press(deskDown, false);
				deskDown = null;
				show = false;
				deskHeld = false;
			}
			Cursor:
			if (cursor && show != cursor.gameObject.activeSelf)
			{
				cursor.gameObject.SetActive(show);
			}
			if (show && cursor)
			{
				cursor.anchoredPosition = (shownPx + new Vector2(rtW, rtH) * -0.5f) / canvasScale;
			}
			if (leaveAt < 0f || !player)
			{
				return;
			}
			player.dark = Mathf.Max(player.dark, Mathf.Clamp01((Time.time - leaveAt) / 0.4f));
			if (Time.time > leaveAt + 0.45f)
			{
				leaveAt = float.PositiveInfinity;
				NetworkManager.Load(gameScene);
			}
		}

		private bool Aim(Pointer p, out Vector3 from, out Vector3 dir)
		{
			from = Vector3.zero;
			dir = Vector3.forward;
			Transform hand = p.node == XRNode.LeftHand ? player.controllerL : player.controllerR;
			if (!hand)
			{
				return false;
			}
			if (!p.device.isValid)
			{
				p.device = InputDevices.GetDeviceAtXRNode(p.node);
			}
			Quaternion turn = Quaternion.identity;
			Vector3 offset = Vector3.zero;
			if (p.device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 gripPos) && p.device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion gripRot) && p.device.TryGetFeatureValue(new InputFeatureUsage<Vector3>("PointerPosition"), out Vector3 aimPos) && p.device.TryGetFeatureValue(new InputFeatureUsage<Quaternion>("PointerRotation"), out Quaternion aimRot))
			{
				Quaternion inv = Quaternion.Inverse(gripRot);
				turn = inv * aimRot;
				offset = inv * (aimPos - gripPos);
			}
			from = hand.position + hand.rotation * offset;
			dir = hand.rotation * turn * Vector3.forward;
			return true;
		}

		private bool HitScreen(Vector3 origin, Vector3 dir, out Vector2 uv, out Vector3 at)
		{
			uv = Vector2.zero;
			at = Vector3.zero;
			Vector3 o = transform.InverseTransformPoint(origin);
			Vector3 d = transform.InverseTransformVector(dir.normalized);
			float facing = Vector3.Dot(d, screenNormal);
			if (facing > -0.0001f)
			{
				return false;
			}
			float t = (screenPlane - Vector3.Dot(o, screenNormal)) / facing;
			if (t <= 0f || t > reachMax)
			{
				return false;
			}
			Vector3 p = o + d * t;
			uv = new Vector2((Vector3.Dot(p, screenRight) - minRight) / screenW, (Vector3.Dot(p, screenUp) - minUp) / screenH);
			if (uv.x < 0f || uv.x > 1f || uv.y < 0f || uv.y > 1f)
			{
				return false;
			}
			at = transform.TransformPoint(p);
			return true;
		}

		private Btn Under(Vector2 px)
		{
			Btn found = null;
			foreach (Btn b in buttons)
			{
				if (b.button && b.button.interactable && b.face.gameObject.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(b.face, px, screenCamera))
				{
					found = b;
				}
			}
			return found;
		}

		private void Hover(Pointer p, Btn over)
		{
			if (p.hover == over)
			{
				return;
			}
			Glow(p.hover, false);
			Glow(over, true);
			p.hover = over;
			if (over != null)
			{
				Player.Hand(p.node, 0.12f, 0.02f);
			}
		}

		private void Glow(Btn b, bool on)
		{
			if (b == null)
			{
				return;
			}
			b.face.localScale = Vector3.one * (on ? 1.08f : 1f);
			if (b.image)
			{
				b.image.color = on ? Color.Lerp(b.color, Color.white, 0.25f) : b.color;
			}
		}

		private void Press(Btn b, bool down)
		{
			if (b != null)
			{
				b.face.anchoredPosition = b.rest + (down ? new Vector2(0f, -4f) : Vector2.zero);
			}
		}

		private void Click(Btn b, XRNode node)
		{
			if (node != XRNode.Head)
			{
				Player.Hand(node, 0.35f, 0.05f);
			}
			Player.Note("TV click " + (b.button.onClick.GetPersistentEventCount() > 0 ? b.button.onClick.GetPersistentMethodName(0) : b.button.name) + " by " + node + " on " + shown);
			b.button.onClick.Invoke();
		}

		private void SetLine(Pointer p, bool on, Vector3 from, Vector3 to)
		{
			if (p.line.enabled != on)
			{
				p.line.enabled = on;
			}
			if (p.dot.gameObject.activeSelf != on)
			{
				p.dot.gameObject.SetActive(on);
			}
			if (on)
			{
				p.line.SetPosition(0, from);
				p.line.SetPosition(1, to);
				p.dot.position = to;
				Color c = p.hover == null ? new Color(1f, 0.85f, 0.2f, 1f) : new Color(0.35f, 1f, 0.45f, 1f);
				p.line.startColor = c;
				p.line.endColor = c;
				p.dot.GetComponent<Renderer>().material.color = c;
			}
		}

		private Pointer MakePointer(XRNode node)
		{
			GameObject go = new GameObject(node == XRNode.LeftHand ? "TV Ray L" : "TV Ray R");
			LineRenderer line = go.AddComponent<LineRenderer>();
			line.material = lineMat;
			line.positionCount = 2;
			line.startWidth = 0.012f;
			line.endWidth = 0.005f;
			line.numCapVertices = 4;
			line.shadowCastingMode = ShadowCastingMode.Off;
			line.receiveShadows = false;
			line.enabled = false;
			GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			dot.name = "Dot";
			Destroy(dot.GetComponent<Collider>());
			dot.transform.SetParent(go.transform, false);
			dot.transform.localScale = Vector3.one * 0.025f;
			Renderer r = dot.GetComponent<Renderer>();
			r.material = new Material(lineMat);
			r.shadowCastingMode = ShadowCastingMode.Off;
			dot.SetActive(false);
			return new Pointer
			{
				node = node,
				line = line,
				dot = dot.transform
			};
		}

		private void BindScreen()
		{
			Canvas canvas = null;
			foreach (Cassette c in cassettes)
			{
				if (!canvas && c.page)
				{
					canvas = c.page.GetComponentInParent<Canvas>(true);
				}
			}
			if (canvas)
			{
				foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
				{
					Image image = button.targetGraphic as Image;
					if (!image)
					{
						image = button.GetComponent<Image>();
					}
					RectTransform face = (RectTransform)button.transform;
					buttons.Add(new Btn
					{
						button = button,
						face = face,
						image = image,
						color = image ? image.color : Color.white,
						rest = face.anchoredPosition
					});
				}
			}
			volumeBars = DotsOf(volumeDots);
			turnBars = DotsOf(turnDots);
			if (canvas)
			{
				canvasScale = Mathf.Max(canvas.scaleFactor, 0.01f);
				musicBars = DotsOf(Deep(canvas.transform, "Music Dots"));
				sfxBars = DotsOf(Deep(canvas.transform, "Effects Dots"));
				soundPanel = Deep(canvas.transform, "Sound Panel") ? Deep(canvas.transform, "Sound Panel").gameObject : null;
				controlsPanel = Deep(canvas.transform, "Controls Panel") ? Deep(canvas.transform, "Controls Panel").gameObject : null;
				soundMark = Deep(canvas.transform, "Sound Mark") ? Deep(canvas.transform, "Sound Mark").gameObject : null;
				controlsMark = Deep(canvas.transform, "Controls Mark") ? Deep(canvas.transform, "Controls Mark").gameObject : null;
				gamePanel = Deep(canvas.transform, "Game Panel") ? Deep(canvas.transform, "Game Panel").gameObject : null;
				gameMark = Deep(canvas.transform, "Game Mark") ? Deep(canvas.transform, "Game Mark").gameObject : null;
				regionValue = Text(canvas.transform, "Region Value");
				kickValue = Text(canvas.transform, "Kick Value");
				deadValue = Text(canvas.transform, "Dead Value");
				livValue = Text(canvas.transform, "Liv Value");
				moveValue = Text(canvas.transform, "Move Value");
				turnValue = Text(canvas.transform, "Turn Value");
				hapticsValue = Text(canvas.transform, "Haptics Value");
				voicesValue = Text(canvas.transform, "Voices Value");
				myVoiceValue = Text(canvas.transform, "My Voice Value");
				micValue = Text(canvas.transform, "Mic Value");
				sticksValue = Text(canvas.transform, "Sticks Value");
				speedLabel = Text(canvas.transform, "Speed Label");
				notesText = Text(canvas.transform, "Notes");
				notesPage = Text(canvas.transform, "Notes Page");
				Transform prev = Deep(canvas.transform, "Notes Prev");
				Transform next = Deep(canvas.transform, "Notes Next");
				notesPrev = prev ? prev.gameObject : null;
				notesNext = next ? next.gameObject : null;
				shopTeeth = Text(canvas.transform, "Shop Teeth");
				shopStatus = Text(canvas.transform, "Shop Status");
				shopPrices = new TMP_Text[Account.Packs.Length];
				for (int i = 0; i < shopPrices.Length; i++)
				{
					shopPrices[i] = Text(canvas.transform, "Shop Price " + (i + 1));
				}
				if (notesText && changelog)
				{
					notesText.enableAutoSizing = true;
					notesText.fontSizeMax = notesText.fontSize;
					notesText.fontSizeMin = 6f;
					notesText.text = Pages(changelog.text);
				}
			}
			foreach (Cassette c in cassettes)
			{
				if (c.page)
				{
					c.page.SetActive(false);
				}
			}
			if (cursor)
			{
				cursor.gameObject.SetActive(false);
			}
			if (screenCamera)
			{
				screenCamera.enabled = false;
			}
		}

		private static Image[] DotsOf(Transform row)
		{
			List<Image> dots = new List<Image>();
			if (row)
			{
				foreach (Transform child in row)
				{
					Image image = child.GetComponent<Image>();
					if (image)
					{
						dots.Add(image);
					}
				}
			}
			return dots.ToArray();
		}

		public void StartGame()
		{
			if (boot)
			{
				boot.StartMatch();
			}
		}

		private static bool Plays(Cassette c, string method)
		{
			for (int i = 0; i < c.onPlay.GetPersistentEventCount(); i++)
			{
				if (c.onPlay.GetPersistentMethodName(i) == method)
				{
					return true;
				}
			}
			return false;
		}

		public void PlayTutorial()
		{
			if (tutorialOn && boot && !boot.InRoom && !boot.Busy)
			{
				boot.GoTo(tutorialScene, "");
			}
		}

		public void CreateRoom()
		{
			creating = false;
			picking = false;
			if (editing)
			{
				editing = false;
				if (boot)
				{
					boot.UpdateRoom(newOpen, newPlayers, newWins, ModeRule(), MapList());
				}
			}
			else if (boot)
			{
				boot.CreateRoom(newOpen, newPlayers, newWins, ModeRule(), MapList());
			}
		}

		private int ModeRule()
		{
			if (newMode == RoundsMode)
			{
				return -1;
			}
			return newMode == TeamsMode ? 8 : 0;
		}

		public void OpenEdit()
		{
			if (!boot || !boot.InRoom || !boot.IsHost)
			{
				return;
			}
			picking = false;
			editing = true;
			newOpen = boot.Open;
			newPlayers = boot.RoomCap();
			newWins = boot.RoomWins();
			int teams = boot.RoomTeams();
			newMode = teams < 0 ? RoundsMode : (teams != 0 ? TeamsMode : 0);
			skipped.Clear();
			string maps = boot.RoomMaps();
			if (maps == "")
			{
				return;
			}
			foreach (GameManager.Map map in Maps())
			{
				if (!GameManager.Allowed(maps, map.scene))
				{
					skipped.Add(map.scene);
				}
			}
		}

		public void NextMode()
		{
			newMode = (newMode + 1) % Modes.Length;
		}

		public void OpenMaps()
		{
			picking = true;
		}

		public void CloseMaps()
		{
			picking = false;
		}

		public void ToggleMap(int index)
		{
			List<GameManager.Map> maps = Maps();
			if (index < 0 || index >= maps.Count)
			{
				return;
			}
			string scene = maps[index].scene;
			if (skipped.Contains(scene))
			{
				skipped.Remove(scene);
			}
			else if (skipped.Count < maps.Count - 1)
			{
				skipped.Add(scene);
			}
		}

		private static List<GameManager.Map> Maps()
		{
			return GameManager.main ? GameManager.main.maps : new List<GameManager.Map>();
		}

		private string MapList()
		{
			if (skipped.Count == 0)
			{
				return "";
			}
			List<string> keep = new List<string>();
			foreach (GameManager.Map map in Maps())
			{
				if (!skipped.Contains(map.scene))
				{
					keep.Add(map.scene);
				}
			}
			return skipped.Count >= keep.Count ? string.Join(";", keep) : "!" + string.Join(";", skipped);
		}

		private static int Kept(string list)
		{
			int kept = 0;
			foreach (GameManager.Map map in Maps())
			{
				if (GameManager.Allowed(list, map.scene))
				{
					kept++;
				}
			}
			return kept;
		}

		private void ShowMaps(RoomPage r)
		{
			List<GameManager.Map> maps = Maps();
			for (int i = 0; i < r.mapButtons.Length; i++)
			{
				GameObject slot = r.mapButtons[i].transform.parent.gameObject;
				if (slot.activeSelf != i < maps.Count)
				{
					r.mapButtons[i].transform.parent.gameObject.SetActive(i < maps.Count);
				}
				if (i >= maps.Count)
				{
					continue;
				}
				bool off = skipped.Contains(maps[i].scene);
				if (r.mapNames[i])
				{
					r.mapNames[i].text = maps[i].title;
				}
				Color tint = off ? MapOff : MapOn;
				foreach (Btn b in buttons)
				{
					if (b.button != r.mapButtons[i] || b.color == tint)
					{
						continue;
					}
					b.color = tint;
					Glow(b, b.face.localScale.x > 1f);
					Transform shadow = b.face.parent.Find("Shadow");
					Image shade = shadow ? shadow.GetComponent<Image>() : null;
					if (shade)
					{
						shade.color = off ? MapOffShade : MapOnShade;
					}
				}
			}
		}

		public void PlayersUp()
		{
			newPlayers = Mathf.Min(newPlayers + 1, 8);
		}

		public void PlayersDown()
		{
			int least = (editing && boot) ? Mathf.Max(boot.Players(), 2) : 2;
			newPlayers = Mathf.Max(newPlayers - 1, least);
		}

		public void WinsUp()
		{
			newWins = Mathf.Min(newWins + 1, 8);
		}

		public void WinsDown()
		{
			newWins = Mathf.Max(newWins - 1, 1);
		}

		public void OpenCreate()
		{
			picking = false;
			keypad = false;
			creating = true;
		}

		public void ToggleNewRoom()
		{
			newOpen = !newOpen;
		}

		public void OpenKeypad()
		{
			keypad = true;
			creating = false;
			typed = "";
		}

		public void Key(string digit)
		{
			if (typed.Length < 6)
			{
				typed += digit;
			}
		}

		public void KeyBack()
		{
			if (typed.Length > 0)
			{
				typed = typed.Substring(0, typed.Length - 1);
			}
		}

		public void JoinTyped()
		{
			if (boot && typed.Length == 6)
			{
				keypad = false;
				boot.JoinCode(typed);
			}
		}

		public void QuickJoin()
		{
			if (boot)
			{
				boot.QuickJoin();
			}
		}

		public void JoinListed(int row)
		{
			if (!boot || row < 0 || row >= shownRooms.Count || shownRooms[row] == null)
			{
				return;
			}
			NetworkManager.Room room = shownRooms[row];
			if (!room.playing)
			{
				boot.JoinRoom(room);
				return;
			}
			confirmRoom = room;
			Player.Note("asked whether to join " + room.title + " mid-match as a spectator");
		}

		public void ConfirmJoin()
		{
			NetworkManager.Room room = confirmRoom;
			confirmRoom = null;
			if (!boot || room == null)
			{
				return;
			}
			NetworkManager.Room live = boot.rooms.Find((NetworkManager.Room r) => r.code == room.code);
			if (live == null)
			{
				Player.Note("room " + room.title + " is gone, not joining");
				return;
			}
			boot.JoinRoom(live);
		}

		public void CancelJoin()
		{
			confirmRoom = null;
		}

		public void PrevPage()
		{
			listPage--;
		}

		public void NextPage()
		{
			listPage++;
		}

		public void TogglePrivacy()
		{
			if (boot)
			{
				boot.SetOpen(!boot.Open);
			}
		}

		public void LeaveRoom()
		{
			if (boot)
			{
				boot.Leave();
			}
		}

		public void Back()
		{
			keypad = false;
			creating = false;
			picking = false;
			editing = false;
			confirmRoom = null;
			if (boot)
			{
				boot.error = "";
			}
		}

		private void RefreshRooms()
		{
			bool listing = false;
			foreach (Cassette c in cassettes)
			{
				if (!c.page || !c.page.activeSelf)
				{
					continue;
				}
				if (!roomPages.TryGetValue(c.page, out RoomPage r))
				{
					Transform page = c.page.transform;
					r = new RoomPage
					{
						menu = Child(page, "Menu"),
						keypad = Child(page, "Keypad"),
						busy = Child(page, "Busy"),
						room = Child(page, "Room"),
						error = Child(page, "Error"),
						confirm = Child(page, "Confirm"),
						code = Text(page, "Code"),
						roomCode = Text(page, "Room Code"),
						players = Text(page, "Players"),
						host = Text(page, "Host"),
						errorText = Text(page, "Error Text"),
						create = Child(page, "Create"),
						status = Text(page, "Status"),
						empty = Text(page, "Empty"),
						pageText = Text(page, "Page"),
						newKind = Text(page, "New Kind"),
						kindHint = Text(page, "Kind Hint"),
						privacy = Text(page, "Privacy Value"),
						newPlayers = Text(page, "New Players"),
						newWins = Text(page, "New Wins"),
						newMode = Text(page, "New Mode"),
						newMaps = Text(page, "New Maps"),
						picker = Child(page, "Map Picker")
					};
					Transform make = Deep(c.page.transform, "Make");
					r.makeLabel = make ? make.GetComponentInChildren<TMP_Text>(true) : null;
					Transform edit = Deep(c.page.transform, "Edit Room");
					r.editButton = edit ? edit.gameObject : null;
					FindMaps(c.page.transform, r);
					if (r.privacy)
					{
						r.privacyButton = r.privacy.GetComponentInParent<Button>(true);
					}
					r.peoplePanel = Child(c.page.transform, "People");
					FindPeople(c.page.transform, r);
					FindRows(c.page.transform, r);
					roomPages[c.page] = r;
				}
				if (!r.menu && !r.room)
				{
					continue;
				}
				string view;
				if (!boot)
				{
					view = "menu";
				}
				else if (boot.Busy)
				{
					view = "busy";
				}
				else if (boot.InRoom)
				{
					view = (editing && r.create) ? ((picking && r.picker) ? "maps" : "create") : "room";
				}
				else if (confirmRoom != null && r.confirm)
				{
					view = "confirm";
				}
				else if (boot.error != "")
				{
					view = "error";
				}
				else if (keypad && r.keypad)
				{
					view = "keypad";
				}
				else if (creating && r.create)
				{
					view = (picking && r.picker) ? "maps" : "create";
				}
				else
				{
					view = "menu";
				}
				if (boot && r.rows != null && shown != null && c.tape && c.tape.name == shown && !boot.InRoom && !boot.Connecting)
				{
					listing |= !boot.Travelling;
				}
				if (r.menu)
				{
					r.menu.SetActive(view == "menu");
				}
				if (r.keypad)
				{
					r.keypad.SetActive(view == "keypad");
				}
				if (r.create)
				{
					r.create.SetActive(view == "create");
				}
				if (r.picker)
				{
					r.picker.SetActive(view == "maps");
				}
				if (view == "maps")
				{
					ShowMaps(r);
				}
				if (r.busy)
				{
					r.busy.SetActive(view == "busy");
				}
				if (r.room)
				{
					r.room.SetActive(view == "room" && (!people || !r.peoplePanel));
				}
				if (r.peoplePanel)
				{
					r.peoplePanel.SetActive(view == "room" && people);
				}
				if (view != "room")
				{
					people = false;
				}
				if (view == "room" && people)
				{
					People(r);
				}
				if (r.error)
				{
					r.error.SetActive(view == "error");
				}
				if (r.confirm)
				{
					r.confirm.SetActive(view == "confirm");
				}
				if (r.code)
				{
					string shownCode = "";
					for (int i = 0; i < 6; i++)
					{
						shownCode = shownCode + (i < typed.Length ? typed[i].ToString() : "_") + (i == 5 ? "" : " ");
					}
					r.code.text = shownCode;
				}
				if (!boot)
				{
					continue;
				}
				if (view == "menu")
				{
					List(r);
				}
				if (r.status)
				{
					r.status.text = boot.quickWanted ? "FINDING A ROOM..." : "CONNECTING...";
				}
				if (r.newKind)
				{
					r.newKind.text = newOpen ? "PUBLIC" : "PRIVATE";
				}
				if (r.newPlayers)
				{
					r.newPlayers.text = newPlayers.ToString();
				}
				if (r.newWins)
				{
					r.newWins.text = newWins.ToString();
				}
				if (editing && !(boot.InRoom && boot.IsHost))
				{
					editing = false;
				}
				if (r.makeLabel)
				{
					r.makeLabel.text = editing ? "SAVE" : "CREATE ROOM";
				}
				if (r.editButton && r.editButton.activeSelf != boot.IsHost)
				{
					r.editButton.SetActive(boot.IsHost);
				}
				if (r.newMode)
				{
					r.newMode.text = Modes[newMode];
				}
				if (r.newMaps)
				{
					r.newMaps.text = skipped.Count == 0 ? "ALL" : ((Maps().Count - skipped.Count) + "/" + Maps().Count);
				}
				if (r.kindHint)
				{
					r.kindHint.text = newOpen ? "ANYONE CAN FIND IT IN THE LIST" : "ONLY PEOPLE WITH THE CODE CAN JOIN  <color=#FF8C8C>NO XP OR WINS</color>";
				}
				if (r.roomCode)
				{
					r.roomCode.text = boot.code != "" ? ("ROOM " + boot.code) : "ROOM";
				}
				if (r.privacy)
				{
					r.privacy.text = boot.Open ? "PUBLIC" : "PRIVATE";
				}
				if (r.privacyButton)
				{
					r.privacyButton.interactable = boot.IsHost;
				}
				if (r.players)
				{
					r.players.text = "PLAYERS " + boot.Players() + "/" + boot.RoomCap() + "   FIRST TO " + boot.RoomWins();
				}
				if (r.host)
				{
					r.host.text = (boot.IsHost ? "YOU ARE THE HOST" : "WAITING FOR THE HOST") + (boot.RoomTeams() > 0 ? "   TEAMS MODE" : (boot.RoomTeams() < 0 ? "   ROUNDS MODE" : "")) + ((boot.RoomMaps() == "") ? "" : ("   " + Kept(boot.RoomMaps()) + " MAPS"));
				}
				if (r.errorText)
				{
					r.errorText.text = boot.error;
				}
			}
			if (boot)
			{
				boot.Browse(listing);
			}
		}

		private static void FindMaps(Transform page, RoomPage r)
		{
			List<Button> buttons = new List<Button>();
			List<TMP_Text> names = new List<TMP_Text>();
			string name = "Map Toggle 1";
			while (true)
			{
				Transform toggle = Deep(page, name);
				if (!toggle)
				{
					break;
				}
				Button button = toggle.GetComponentInChildren<Button>(true);
				if (!button)
				{
					break;
				}
				buttons.Add(button);
				names.Add(toggle.GetComponentInChildren<TMP_Text>(true));
				name = "Map Toggle " + (buttons.Count + 1);
			}
			r.mapButtons = buttons.ToArray();
			r.mapNames = names.ToArray();
		}

		private static void FindPeople(Transform page, RoomPage r)
		{
			List<Transform> found = new List<Transform>();
			string name = "Person 1";
			while (true)
			{
				Transform row = Deep(page, name);
				if (!row)
				{
					break;
				}
				found.Add(row);
				name = "Person " + (found.Count + 1);
			}
			r.personRows = new GameObject[found.Count];
			r.personName = new TMP_Text[found.Count];
			r.personState = new TMP_Text[found.Count];
			for (int i = 0; i < found.Count; i++)
			{
				r.personRows[i] = found[i].gameObject;
				r.personName[i] = Text(found[i], "Name");
				r.personState[i] = Text(found[i], "Count");
			}
		}

		private void People(RoomPage r)
		{
			if (r.personRows == null)
			{
				return;
			}
			int hash = NetworkPlayer.muted.Count;
			foreach (NetworkPlayer p in NetworkPlayer.avatars)
			{
				if (p && p.Object && p.Object.IsValid)
				{
					hash = hash * 31 + p.Paint + (p.Blabber ? 7 : 0) + p.Object.InputAuthority.PlayerId * 131;
				}
			}
			if (hash == peopleSeen)
			{
				return;
			}
			peopleSeen = hash;
			shownPeople.Clear();
			foreach (NetworkPlayer p in NetworkPlayer.avatars)
			{
				if (p && p.Object && p.Object.IsValid && !p.HasStateAuthority)
				{
					shownPeople.Add(p);
				}
			}
			for (int i = 0; i < r.personRows.Length; i++)
			{
				NetworkPlayer p = i < shownPeople.Count ? shownPeople[i] : null;
				r.personRows[i].SetActive(p);
				if (!p)
				{
					continue;
				}
				bool muted = p.Uid.Length > 0 && NetworkPlayer.muted.Contains(p.Uid.Value);
				if (r.personName[i])
				{
					r.personName[i].text = GameManager.ColorName(p.Paint);
				}
				if (r.personState[i])
				{
					r.personState[i].text = muted ? "MUTED" : (p.Blabber ? "BLABBERS" : "VOICE");
				}
			}
		}

		private static void FindRows(Transform page, RoomPage r)
		{
			List<Transform> found = new List<Transform>();
			string name = "Row 1";
			while (true)
			{
				Transform row = Deep(page, name);
				if (!row)
				{
					break;
				}
				found.Add(row);
				name = "Row " + (found.Count + 1);
			}
			if (found.Count == 0)
			{
				return;
			}
			r.rows = new GameObject[found.Count];
			r.rowName = new TMP_Text[found.Count];
			r.rowCount = new TMP_Text[found.Count];
			r.rowButton = new Button[found.Count];
			for (int i = 0; i < found.Count; i++)
			{
				r.rows[i] = found[i].gameObject;
				r.rowName[i] = Text(found[i], "Name");
				r.rowCount[i] = Text(found[i], "Count");
				r.rowButton[i] = found[i].GetComponentInChildren<Button>(true);
			}
		}

		private void List(RoomPage r)
		{
			if (r.rows == null)
			{
				return;
			}
			int per = r.rows.Length;
			int pages = Mathf.Max((boot.rooms.Count + per - 1) / per, 1);
			listPage = Mathf.Clamp(listPage, 0, pages - 1);
			if (r.seen == boot.roomsSeen && r.seenPage == listPage && r.seenListed == boot.listed)
			{
				return;
			}
			r.seen = boot.roomsSeen;
			r.seenPage = listPage;
			r.seenListed = boot.listed;
			shownRooms.Clear();
			for (int i = 0; i < per; i++)
			{
				int at = i + listPage * per;
				NetworkManager.Room room = at < boot.rooms.Count ? boot.rooms[at] : null;
				shownRooms.Add(room);
				r.rows[i].SetActive(room != null);
				if (room == null)
				{
					continue;
				}
				if (r.rowName[i])
				{
					r.rowName[i].text = ModeTag(room.mode) + room.title;
				}
				if (r.rowCount[i])
				{
					r.rowCount[i].text = room.players >= room.max ? "FULL" : (room.playing ? ("PLAYING " + room.players + "/" + room.max) : (room.players + "/" + room.max));
				}
				if (r.rowName[i] && r.rowCount[i])
				{
					Squeeze(r.rowName[i], r.rowCount[i]);
				}
				if (r.rowButton[i])
				{
					r.rowButton[i].interactable = room.players < room.max;
				}
			}
			if (r.empty)
			{
				r.empty.gameObject.SetActive(boot.rooms.Count == 0);
				r.empty.text = boot.listed ? "NO ROOMS YET\nCREATE ONE!" : "LOOKING FOR ROOMS...";
			}
			if (r.pageText)
			{
				r.pageText.text = (listPage + 1) + "/" + pages;
			}
		}

		private static void Squeeze(TMP_Text name, TMP_Text count)
		{
			RectTransform rt = name.rectTransform;
			Vector2 size = count.GetPreferredValues(count.text);
			rt.offsetMax = new Vector2(-(size.x + 8f), rt.offsetMax.y);
			name.overflowMode = TextOverflowModes.Ellipsis;
		}

		private static string ModeTag(int teams)
		{
			int mode = teams < 0 ? RoundsMode : (teams != 0 ? TeamsMode : 0);
			return "<size=80%><color=" + ModeTints[mode] + ">" + Modes[mode] + "</color></size>  ";
		}

		private static GameObject Child(Transform page, string name)
		{
			Transform t = page.Find(name);
			return t ? t.gameObject : null;
		}

		private static TMP_Text Text(Transform page, string name)
		{
			foreach (TMP_Text t in page.GetComponentsInChildren<TMP_Text>(true))
			{
				if (t.name == name)
				{
					return t;
				}
			}
			return null;
		}

		public void VolumeUp()
		{
			Step(ref Prefs.master, 0.1f);
		}

		public void VolumeDown()
		{
			Step(ref Prefs.master, -0.1f);
		}

		public void MusicUp()
		{
			Step(ref Prefs.music, 0.1f);
		}

		public void MusicDown()
		{
			Step(ref Prefs.music, -0.1f);
		}

		public void EffectsUp()
		{
			Step(ref Prefs.sfx, 0.1f);
		}

		public void EffectsDown()
		{
			Step(ref Prefs.sfx, -0.1f);
		}

		public void ToggleMove()
		{
			Prefs.armWalk = !Prefs.armWalk;
			Prefs.Save();
			ShowSettings();
		}

		public void ToggleTurn()
		{
			if (Prefs.noTurn)
			{
				Prefs.snapTurn = false;
				Prefs.noTurn = false;
			}
			else if (Prefs.snapTurn)
			{
				Prefs.noTurn = true;
			}
			else
			{
				Prefs.snapTurn = true;
			}
			Prefs.Save();
			ShowSettings();
		}

		public void ToggleVoices()
		{
			bool was = Prefs.realVoice;
			Prefs.realVoice = !Prefs.realVoice;
			Player.Note("other voices set to " + (was ? "blabber" : "real voice"));
			Prefs.Save();
			ShowSettings();
		}

		public void ToggleMyVoice()
		{
			bool was = Prefs.myBlabber;
			Prefs.myBlabber = !Prefs.myBlabber;
			Player.Note("my voice set to " + (was ? "real voice" : "blabber"));
			Prefs.Save();
			ShowSettings();
		}

		public void OpenPeople()
		{
			people = true;
			peopleSeen = -1;
		}

		public void ClosePeople()
		{
			people = false;
		}

		public void TogglePerson(int row)
		{
			if (row < 0 || row >= shownPeople.Count || !shownPeople[row] || shownPeople[row].Uid.Length == 0)
			{
				return;
			}
			string id = shownPeople[row].Uid.Value;
			if (!NetworkPlayer.muted.Remove(id))
			{
				NetworkPlayer.muted.Add(id);
			}
			Player.Note((NetworkPlayer.muted.Contains(id) ? "muted " : "unmuted ") + shownPeople[row].name);
			peopleSeen = -1;
		}

		public void ToggleMic()
		{
			Prefs.mic = !Prefs.mic;
			Prefs.Save();
			ShowSettings();
		}

		public void ToggleSticks()
		{
			Prefs.swapSticks = !Prefs.swapSticks;
			Prefs.Save();
			ShowSettings();
		}

		public void ToggleHaptics()
		{
			Prefs.haptics = !Prefs.haptics;
			Prefs.Save();
			if (Prefs.haptics)
			{
				Player.Both(0.5f, 0.1f);
			}
			ShowSettings();
		}

		public void ShowSound()
		{
			tab = 0;
			ShowSettings();
		}

		public void ShowControls()
		{
			tab = 1;
			ShowSettings();
		}

		public void ShowGame()
		{
			tab = 2;
			ShowSettings();
		}

		public void SpawnLiv()
		{
			LivCamera.Press(livTablet, livSpot);
		}

		public void ToggleKick()
		{
			Prefs.noKick = !Prefs.noKick;
			Prefs.Save();
			ShowSettings();
		}

		public void RegionDown()
		{
			if (boot)
			{
				boot.ChangeRegion(-1);
			}
			ShowSettings();
		}

		public void RegionUp()
		{
			if (boot)
			{
				boot.ChangeRegion(1);
			}
			ShowSettings();
		}

		public void DeadDown()
		{
			StepDead(-1);
		}

		public void DeadUp()
		{
			StepDead(1);
		}

		private void StepDead(int by)
		{
			int at = Array.IndexOf(deadSteps, Prefs.stickDead);
			if (at < 0)
			{
				at = 1;
			}
			Prefs.stickDead = deadSteps[Mathf.Clamp(at + by, 0, deadSteps.Length - 1)];
			Prefs.Save();
			ShowSettings();
		}

		private static bool Pc()
		{
			if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				return true;
			}
			if (Application.isEditor)
			{
				return Environment.GetEnvironmentVariable("PCGAME_ASKNAME") == "1";
			}
			return false;
		}

		private static bool NeedsName()
		{
			Prefs.Load();
			return Pc() && string.IsNullOrEmpty(Prefs.nick);
		}

		private void Welcome(int step)
		{
			if (!welcome && !BuildWelcome())
			{
				Player.Note("welcome panel could not be built");
				if (player)
				{
					player.blackout = 0f;
				}
				return;
			}
			welcomeStep = step;
			welcomeKeys.SetActive(step == NameStep);
			welcomeChoice.SetActive(step == TutorialStep);
			welcomeTitle.text = step == NameStep ? "ENTER A NAME" : "LOOKS LIKE YOU'RE NEW!";
			if (step == NameStep)
			{
				ShowName();
			}
			else
			{
				welcomeLine.text = "PLAY THE TUTORIAL?";
			}
			if (player)
			{
				player.blackout = 0f;
				player.Float(true);
				if (welcomeShift == Vector3.zero)
				{
					ToSea();
				}
			}
			Place();
			welcome.gameObject.SetActive(true);
			if (step == NameStep && !welcomeTyping)
			{
				Keyboard keyboard = Keyboard.current;
				if (keyboard != null)
				{
					keyboard.onTextInput += Typed;
					welcomeTyping = true;
				}
			}
			Player.Note("welcome panel: " + welcomeTitle.text + " " + welcomeLine.text);
		}

		private void CloseWelcome()
		{
			welcomeStep = 0;
			if (welcome)
			{
				welcome.gameObject.SetActive(false);
			}
			foreach (Pointer p in pointers)
			{
				if (p != null)
				{
					Hover(p, null);
					Press(p.down, false);
					p.down = null;
					SetLine(p, false, Vector3.zero, Vector3.zero);
				}
			}
			if (player && welcomeShift != Vector3.zero)
			{
				player.Shift(welcomeHome - player.pelvis.position);
				player.playSpace.position -= welcomeShift;
			}
			welcomeShift = Vector3.zero;
			if (player)
			{
				player.Float(false);
			}
			if (welcomeTyping)
			{
				Keyboard keyboard = Keyboard.current;
				if (keyboard != null)
				{
					keyboard.onTextInput -= Typed;
				}
			}
			welcomeTyping = false;
		}

		private void ToSea()
		{
			if (!player.cam || !player.playSpace)
			{
				return;
			}
			Vector3 eye = player.cam.position;
			Vector3 forward = player.cam.forward;
			Vector3 flat = forward.x * forward.x + forward.z * forward.z < 0.01f ? Vector3.forward : new Vector3(forward.x, 0f, forward.z);
			flat = flat.normalized;
			welcomeHome = player.pelvis.position;
			float sea = Water.main ? Water.main.Surface : eye.y - 1.6f;
			Vector3 to = eye + flat * OceanAway;
			to.y = sea + OceanEye;
			welcomeShift = to - eye;
			player.Shift(welcomeShift);
			player.playSpace.position += welcomeShift;
		}

		private bool BuildWelcome()
		{
			Transform root = null;
			foreach (Cassette c in cassettes)
			{
				Canvas canvas = c.page ? c.page.GetComponentInParent<Canvas>(true) : null;
				if (canvas)
				{
					root = canvas.transform;
					break;
				}
			}
			Transform key = root ? Deep(root, "Key 1") : null;
			Transform go = root ? Deep(root, "Key GO") : null;
			Transform del = root ? Deep(root, "Key DEL") : null;
			Transform yes = root ? Deep(root, "New Room") : null;
			Transform no = del;
			TMP_Text title = root ? Text(root, "Title") : null;
			TMP_Text line = root ? Text(root, "Code") : null;
			if (!key || !go || !del || !yes || !no || !title || !line)
			{
				return false;
			}
			GameObject panel = new GameObject("Welcome Panel", typeof(RectTransform));
			panel.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;
			RectTransform rect = (RectTransform)panel.transform;
			rect.sizeDelta = new Vector2(380f, 300f);
			rect.localScale = Vector3.one * PanelScale;
			welcome = panel.transform;
			welcomeTitle = Label(title, welcome, new Vector2(0f, 120f), new Vector2(380f, 44f));
			welcomeLine = Label(line, welcome, new Vector2(0f, 82f), new Vector2(360f, 34f));
			welcomeKeys = Group(welcome, "Keys");
			welcomeChoice = Group(welcome, "Choice");
			string[] rows = KeyRows.Split('|');
			for (int r = 0; r < rows.Length; r++)
			{
				float y = r * -36f + 40f;
				for (int i = 0; i < rows[r].Length; i++)
				{
					char c = rows[r][i];
					MakeKey(key, welcomeKeys.transform, new Vector2((i + (rows[r].Length - 1) * -0.5f) * 34f, y), new Vector2(30f, 30f), c.ToString(), () => TypeName(c));
				}
			}
			MakeKey(key, welcomeKeys.transform, new Vector2(-104f, -106f), new Vector2(130f, 30f), "SPACE", () => TypeName(' '));
			MakeKey(del, welcomeKeys.transform, new Vector2(18f, -106f), new Vector2(98f, 30f), "DEL", EraseName);
			welcomeDone = MakeKey(go, welcomeKeys.transform, new Vector2(122f, -106f), new Vector2(98f, 30f), "DONE", NameDone);
			MakeKey(yes, welcomeChoice.transform, new Vector2(-74f, 10f), new Vector2(124f, 50f), "YES", TutorialYes);
			MakeKey(no, welcomeChoice.transform, new Vector2(74f, 10f), new Vector2(124f, 50f), "NO", TutorialNo);
			foreach (Transform t in panel.GetComponentsInChildren<Transform>(true))
			{
				t.gameObject.layer = 0;
			}
			panel.SetActive(false);
			return true;
		}

		private static GameObject Group(Transform parent, string name)
		{
			GameObject go = new GameObject(name, typeof(RectTransform));
			go.transform.SetParent(parent, false);
			return go;
		}

		private static TMP_Text Label(TMP_Text from, Transform parent, Vector2 at, Vector2 size)
		{
			TMP_Text label = Instantiate(from.gameObject, parent).GetComponent<TMP_Text>();
			RectTransform rect = label.rectTransform;
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = size;
			rect.anchoredPosition = at;
			label.enableAutoSizing = true;
			label.fontSizeMin = 8f;
			label.fontSizeMax = size.y * 0.8f;
			label.alignment = TextAlignmentOptions.Center;
			return label;
		}

		private Btn MakeKey(Transform from, Transform parent, Vector2 at, Vector2 size, string label, UnityAction act)
		{
			RectTransform rect = (RectTransform)Instantiate(from.gameObject, parent).transform;
			rect.name = "Key " + label;
			float height = rect.rect.height;
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = size;
			rect.anchoredPosition = at;
			foreach (RectTransform child in rect.GetComponentsInChildren<RectTransform>(true))
			{
				if (child != rect && child.anchorMin == child.anchorMax)
				{
					child.sizeDelta = size;
				}
			}
			foreach (Image image in rect.GetComponentsInChildren<Image>(true))
			{
				image.pixelsPerUnitMultiplier = height / size.y * image.pixelsPerUnitMultiplier;
			}
			TMP_Text text = rect.GetComponentInChildren<TMP_Text>(true);
			if (text)
			{
				text.text = label;
				text.enableAutoSizing = true;
				text.fontSizeMin = 6f;
				text.fontSizeMax = size.y * 0.62f;
			}
			Button button = rect.GetComponentInChildren<Button>(true);
			button.onClick = new Button.ButtonClickedEvent();
			button.onClick.AddListener(act);
			button.interactable = true;
			Image face = button.targetGraphic as Image;
			if (!face)
			{
				face = button.GetComponent<Image>();
			}
			RectTransform faceRect = (RectTransform)button.transform;
			Btn b = new Btn
			{
				button = button,
				face = faceRect,
				image = face,
				color = face ? face.color : Color.white,
				rest = faceRect.anchoredPosition
			};
			welcomeButtons.Add(b);
			return b;
		}

		private void Place()
		{
			Transform view = (player && player.cam) ? player.cam : (Camera.main ? Camera.main.transform : null);
			if (!view)
			{
				return;
			}
			Vector3 forward = view.forward;
			Vector3 flat = forward.x * forward.x + forward.z * forward.z < 0.01f ? Vector3.forward : new Vector3(forward.x, 0f, forward.z);
			flat = flat.normalized;
			welcome.SetPositionAndRotation(view.position + flat * PanelAhead + Vector3.down * 0.12f, Quaternion.LookRotation(flat, Vector3.up));
		}

		private Btn OnPanel(Vector3 at)
		{
			foreach (Btn b in welcomeButtons)
			{
				if (b.button.interactable && b.face.gameObject.activeInHierarchy && b.face.rect.Contains(b.face.InverseTransformPoint(at)))
				{
					return b;
				}
			}
			return null;
		}

		private void PointWelcome()
		{
			if (player && player.inVR)
			{
				foreach (Pointer p in pointers)
				{
					if (!Aim(p, out Vector3 from, out Vector3 dir))
					{
						SetLine(p, false, Vector3.zero, Vector3.zero);
						continue;
					}
					Vector3 end = from + dir * RayLength;
					Btn over = null;
					if (Vector3.Dot(dir, welcome.forward) > 0f && new Plane(welcome.forward, welcome.position).Raycast(new Ray(from, dir), out float enter) && enter < RayLength)
					{
						end = from + dir * enter;
						over = OnPanel(end);
					}
					Hover(p, over);
					SetLine(p, true, from, end);
					float trigger = p.device.TryGetFeatureValue(CommonUsages.trigger, out float squeeze) ? squeeze : 0f;
					bool down = p.held ? trigger > LetAt : trigger > PressAt;
					if (down)
					{
						if (!p.held)
						{
							p.down = p.hover;
							Press(p.down, true);
						}
					}
					else if (p.held)
					{
						Press(p.down, false);
						if (p.down != null && p.down == p.hover)
						{
							Click(p.down, p.node);
						}
						p.down = null;
					}
					p.held = down;
					if (welcomeStep == 0)
					{
						return;
					}
				}
				return;
			}
			Transform view = (player && player.cam) ? player.cam : (Camera.main ? Camera.main.transform : null);
			Mouse mouse = Mouse.current;
			Btn hit = null;
			if (view && Vector3.Dot(view.forward, welcome.forward) > 0f && new Plane(welcome.forward, welcome.position).Raycast(new Ray(view.position, view.forward), out float distance))
			{
				hit = OnPanel(view.position + view.forward * distance);
			}
			if (hit != deskHover)
			{
				Glow(deskHover, false);
				Glow(hit, true);
				deskHover = hit;
			}
			bool pressed = mouse != null && mouse.leftButton.isPressed;
			if (pressed)
			{
				if (!deskHeld)
				{
					deskDown = deskHover;
					Press(deskDown, true);
				}
			}
			else if (deskHeld)
			{
				Press(deskDown, false);
				if (deskDown != null && deskDown == deskHover)
				{
					Click(deskDown, XRNode.Head);
				}
				deskDown = null;
			}
			deskHeld = pressed;
		}

		private void Typed(char c)
		{
			if (welcomeStep != NameStep)
			{
				return;
			}
			if (c == '\r' || c == '\n')
			{
				NameDone();
			}
			else if (c == '\b')
			{
				EraseName();
			}
			else if (c <= '\u007f' && (char.IsLetterOrDigit(c) || c == ' '))
			{
				TypeName(c);
			}
		}

		private void TypeName(char c)
		{
			if (typedName.Length >= NameMax || (c == ' ' && (typedName.Length == 0 || typedName.EndsWith(" "))))
			{
				return;
			}
			typedName += char.ToUpperInvariant(c);
			ShowName();
		}

		private void EraseName()
		{
			if (typedName.Length > 0)
			{
				typedName = typedName.Substring(0, typedName.Length - 1);
			}
			ShowName();
		}

		private void ShowName()
		{
			welcomeLine.text = typedName + (typedName.Length < NameMax ? "_" : "");
			if (welcomeDone != null)
			{
				welcomeDone.button.interactable = typedName.Trim().Length > 0;
			}
		}

		private void NameDone()
		{
			string name = typedName.Trim();
			if (name == "")
			{
				return;
			}
			Prefs.Load();
			Prefs.nick = name;
			Prefs.tutorialDone = true;
			Prefs.Save();
			Player.Note("new player picked the name " + name);
			CloseWelcome();
		}

		private void TutorialYes()
		{
			Player.Note("new player chose the tutorial");
			CloseWelcome();
			if (player)
			{
				player.blackout = 1f;
			}
			boot.GoTo(tutorialScene, "");
		}

		private void TutorialNo()
		{
			Prefs.Load();
			Prefs.tutorialDone = true;
			Prefs.Save();
			Player.Note("new player skipped the tutorial");
			CloseWelcome();
		}

		public void NotesPrev()
		{
			ShowNotes(notesAt - 1);
		}

		public void NotesNext()
		{
			ShowNotes(notesAt + 1);
		}

		private string Pages(string raw)
		{
			string text = raw.Replace("\r", "").Trim();
			int line = text.IndexOf('\n');
			return "<color=#" + ColorUtility.ToHtmlStringRGB(dotOn) + ">" + (line < 0 ? text : text.Substring(0, line)) + "</color>" + (line < 0 ? "" : text.Substring(line));
		}

		private void ShowNotes(int page)
		{
			if (!notesText)
			{
				return;
			}
			notesText.ForceMeshUpdate(true, false);
			int pages = Mathf.Max(notesText.textInfo.pageCount, 1);
			notesAt = Mathf.Clamp(page, 1, pages);
			notesText.pageToDisplay = notesAt;
			if (notesPage)
			{
				notesPage.text = notesAt + "/" + pages;
			}
			if (notesPage)
			{
				notesPage.gameObject.SetActive(pages > 1);
			}
			if (notesPrev)
			{
				notesPrev.SetActive(pages > 1);
			}
			if (notesNext)
			{
				notesNext.SetActive(pages > 1);
			}
		}

		private void Step(ref float level, float by)
		{
			level = Mathf.Clamp01(Mathf.Round((level + by) * 10f) / 10f);
			Prefs.Save();
			ShowSettings();
		}

		public void OpenShop()
		{
			Account.OpenShop();
		}

		public void BuyTeeth(int pack)
		{
			Account.Buy(pack);
		}

		private void ShowShop()
		{
			if (!shopTeeth || !shopTeeth.gameObject.activeInHierarchy)
			{
				return;
			}
			if (shopTeethShown != Account.Teeth)
			{
				shopTeethShown = Account.Teeth;
				shopTeeth.text = Account.Teeth.ToString("N0", CultureInfo.InvariantCulture);
			}
			if (shopStatus && shopStatus.text != Account.ShopStatus)
			{
				shopStatus.text = Account.ShopStatus;
			}
			for (int i = 0; i < shopPrices.Length; i++)
			{
				string price = Account.Price(i);
				if (shopPrices[i] && shopPrices[i].text != price)
				{
					shopPrices[i].text = price;
				}
			}
		}

		private static Transform Deep(Transform root, string name)
		{
			foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
			{
				if (t.name == name)
				{
					return t;
				}
			}
			return null;
		}

		private void Bars(Image[] bars, float level)
		{
			if (bars == null)
			{
				return;
			}
			int on = Mathf.RoundToInt(bars.Length * level);
			for (int i = 0; i < bars.Length; i++)
			{
				bars[i].color = i < on ? dotOn : dotOff;
			}
		}

		public void TurnUp()
		{
			StepTurn(1);
		}

		public void TurnDown()
		{
			StepTurn(-1);
		}

		private void ShowSettings()
		{
			Bars(volumeBars, Prefs.master);
			Bars(musicBars, Prefs.music);
			Bars(sfxBars, Prefs.sfx);
			for (int i = 0; i < turnBars.Length; i++)
			{
				turnBars[i].color = i > turnIndex ? dotOff : dotOn;
			}
			if (soundPanel)
			{
				soundPanel.SetActive(tab == 0);
			}
			if (controlsPanel)
			{
				controlsPanel.SetActive(tab == 1);
			}
			if (gamePanel)
			{
				gamePanel.SetActive(tab == 2);
			}
			if (soundMark)
			{
				soundMark.SetActive(tab == 0);
			}
			if (controlsMark)
			{
				controlsMark.SetActive(tab == 1);
			}
			if (gameMark)
			{
				gameMark.SetActive(tab == 2);
			}
			if (regionValue)
			{
				regionValue.text = NetworkManager.RegionName;
			}
			if (kickValue)
			{
				kickValue.text = Prefs.noKick ? "OFF" : "ON";
			}
			if (deadValue)
			{
				deadValue.text = Mathf.RoundToInt(Prefs.stickDead * 100f) + "%";
			}
			if (moveValue)
			{
				moveValue.text = Prefs.armWalk ? "ARM SWING" : "STICK";
			}
			if (turnValue)
			{
				turnValue.text = Prefs.noTurn ? "OFF" : (Prefs.snapTurn ? "SNAP" : "SMOOTH");
			}
			if (hapticsValue)
			{
				hapticsValue.text = Prefs.haptics ? "ON" : "OFF";
			}
			if (sticksValue)
			{
				sticksValue.text = Prefs.swapSticks ? "RIGHT" : "LEFT";
			}
			if (voicesValue)
			{
				voicesValue.text = Prefs.realVoice ? "REAL VOICE" : "BLABBER";
			}
			if (myVoiceValue)
			{
				myVoiceValue.text = Prefs.myBlabber ? "BLABBER" : "REAL VOICE";
			}
			if (micValue)
			{
				micValue.text = Prefs.mic ? "ON" : "OFF";
			}
			if (speedLabel)
			{
				speedLabel.text = Prefs.snapTurn ? "SNAP ANGLE" : "TURN SPEED";
			}
		}

		private void ApplyTurn()
		{
			if (player)
			{
				player.turnSpeed = turnSpeeds[turnIndex];
			}
		}

		private void StepTurn(int by)
		{
			turnIndex = Mathf.Clamp(turnIndex + by, 0, turnSpeeds.Length - 1);
			PlayerPrefs.SetFloat("turnSpeed", turnSpeeds[turnIndex]);
			PlayerPrefs.Save();
			ApplyTurn();
			ShowSettings();
		}
	}
}
