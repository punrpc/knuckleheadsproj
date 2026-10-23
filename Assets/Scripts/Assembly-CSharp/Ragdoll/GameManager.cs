using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ragdoll
{
	[DefaultExecutionOrder(-50)]
	public class GameManager : MonoBehaviour
	{
		[Serializable]
		public class Map
		{
			public string scene;

			public string title;
		}

		[Serializable]
		public class Overtime
		{
			public string scene;

			public string water;

			public float rise;

			public bool cables;

			public bool inOrder;

			public float belts;

			public float hold;
		}

		public class Score
		{
			public string id;

			public int paint;

			public int wins;

			public int team;
		}

		public class Placing
		{
			public string name;

			public int paint;

			public int wins;

			public string[] outfit = new string[4] { "", "", "", "" };
		}

		public class Hop
		{
			public string id;

			public string session;

			public string scene;

			public int round;

			public string lobbyKind;

			public string lobby;

			public string host = "";

			public int wins = 3;

			public int size = 8;

			public bool crowd;

			public int expect;

			public int teams;

			public string maps = "";

			public List<Score> scores = new List<Score>();
		}

		public static int podiumRound;

		public static string podiumLobby = "";

		public static readonly List<Placing> podium = new List<Placing>();

		[Header("Maps")]
		public List<Map> maps = new List<Map>();

		public string lobbyScene = "MainMenu";

		[Header("Match")]
		public int winsNeeded = 8;

		public float showWinner = 4f;

		public float showChampion = 6f;

		public float waitForPlayers = 15f;

		public float giveUpAfter = 30f;

		public float countdown = 3f;

		[Header("Overtime")]
		public List<Overtime> overtime = new List<Overtime>();

		public float overtimeFrom = 60f;

		public float roundLength = 300f;

		public float sinkTime = 10f;

		public float timeUp = 30f;

		[Header("Crowd")]
		public GameObject[] crowd;

		[Header("Music")]
		public AudioSource music;

		public AudioClip[] roundMusic;

		public AudioClip roundsMusic;

		public Texture roundsIcon;

		public float musicFade = 1.5f;

		[Header("Death")]
		public AudioClip deathSound;

		public float deathVolume = 0.8f;

		[Header("Loading Screen")]
		public Transform screen;

		public TextMeshPro title;

		public TextMeshPro status;

		public TextMeshPro subtitle;

		public Renderer stripes;

		public Renderer logo;

		public AudioClip loadingMusic;

		public float loadingVolume = 0.6f;

		public float distance = 2.4f;

		public float fadeTime = 0.35f;

		[Header("Color Names")]
		public string[] colorNames = new string[10] { "RED", "ORANGE", "YELLOW", "LIME", "GREEN", "TEAL", "SKY", "BLUE", "PURPLE", "PINK" };

		public static GameManager main;

		private AudioSource deathAudio;

		private int steady;

		public static Hop match;

		public static bool crowded;

		public const int Crowd = 4;

		public static string hostId = "";

		public static bool watching;

		public static readonly HashSet<string> followed = new HashSet<string>();

		public static readonly string MyId = Guid.NewGuid().ToString("N").Substring(0, 8);

		[NonSerialized]
		public bool live;

		[NonSerialized]
		public string winner;

		private static string carryMap;

		private NetworkManager boot;

		private bool showing;

		private bool loading;

		private Vector3 titleHome;

		private Vector3 statusHome;

		private float titleSize;

		private float statusSize;

		private float subtitleMax;

		private Vector2 subtitleBox;

		private Vector3 subtitleHome;

		private Material stripesMat;

		private Material logoMat;

		private static Color backTint = new Color(0.4f, 0.4f, 0.4f, 1f);

		private static AudioSource loadSource;

		private static bool loadHooked;

		private static int backPick = -1;

		private static readonly Color MapYellow = new Color(1f, 0.76f, 0.16f, 1f);

		private static readonly Color[] BackTints = new Color[8]
		{
			new Color(1f, 0.36f, 0.36f, 1f),
			new Color(1f, 0.6f, 0.22f, 1f),
			new Color(0.3f, 0.95f, 0.55f, 1f),
			new Color(0.25f, 0.9f, 0.9f, 1f),
			new Color(0.38f, 0.7f, 1f, 1f),
			new Color(0.5f, 0.52f, 1f, 1f),
			new Color(0.78f, 0.46f, 1f, 1f),
			new Color(1f, 0.46f, 0.82f, 1f)
		};

		private float alpha;

		private float snapUntil;

		private bool recentring;

		private bool beta;

		private bool asking;

		private const float BetaHold = 3f;

		private const float BetaStep = 0.1f;

		private static float betaLeft = 3f;

		private Renderer runner;

		private Material runnerMat;

		private const float RunFrame = 0.07f;

		private const int RunFrames = 8;

		private float winAt = -1f;

		private bool sentWin;

		private bool champion;

		private bool deserted;

		private const string Left = "~";

		private const string TeamWin = "team:";

		private static readonly int[] TeamPaints = new int[4] { 1, 8, 3, 5 };

		private static readonly Dictionary<string, int> teamMemory = new Dictionary<string, int>();

		private static string teamLobby;

		private const string OutOfTime = "time";

		private const string Cleared = "clear";

		private const string Fallen = "fallen";

		private const float SpawnLead = 1.5f;

		private const float SpawnGap = 1.2f;

		private const float Settle = 4f;

		private const float SkyHeight = 12f;

		private const float SkyNear = 3.5f;

		private const float SkyFar = 7f;

		private const float SkyClose = 1.5f;

		private const float SkyTouch = 0.6f;

		private const float NpcTall = 2.3f;

		private const float SkySoft = 1.2f;

		private const int BrinkSteps = 15;

		private const float Wide = 0.35f;

		private const float TossRun = 1.6f;

		private const float StepUp = 0.6f;

		private const float StepGap = 0.4f;

		private const float StepDown = 1.3f;

		private const float DropWide = 0.7f;

		private static readonly Collider[] dropHits = new Collider[16];

		private static readonly string[] Waves = new string[9] { "S", "SS", "SN", "SSN", "SNF", "SSNN", "SNNB", "SSNFB", "SSNNFB" };

		private static readonly RaycastHit[] skyHits = new RaycastHit[16];

		private static NetworkObject npcPrefab;

		private int spawned;

		private float droppedAt = -99f;

		private Texture logoTex;

		private const int MineAlive = 3;

		private const int RoomAlive = 8;

		private bool over;

		private List<SlapDummy.Kind> mine;

		private const float ClockDistance = 2.2f;

		private const float ClockRise = 0.75f;

		private const float ClockFollow = 4f;

		private static readonly Color ClockRed = new Color(1f, 0.3f, 0.25f, 1f);

		private TextMeshPro clock;

		private TextMeshPro ping;

		private float pingShown;

		private bool pingBad;

		private const float PingBad = 0.2f;

		private const float PingOk = 0.16f;

		private const float PingEase = 1.5f;

		private static readonly Vector3 PingSpot = new Vector3(0.34f, 0.24f, 1.1f);

		private static readonly Color PingTint = new Color(1f, 0.38f, 0.3f, 1f);

		private int clockShown = -1;

		private bool hopSent;

		private static string rejoined = "";

		private int shownCount = -1;

		private float goAt = -1f;

		private float musicPeak = -1f;

		private float startedAt;

		private Overtime rule;

		private Water riser;

		private float riserBase;

		private float riserLift;

		private readonly Dictionary<NetRope, float> snapAt = new Dictionary<NetRope, float>();

		private bool sinking;

		public static bool BetaShowing
		{
			get
			{
				if (!main)
				{
					return false;
				}
				return main.beta;
			}
		}

		public static bool Asking
		{
			get
			{
				if (!main)
				{
					return false;
				}
				return main.asking;
			}
		}

		public bool Showing
		{
			get
			{
				if (!showing)
				{
					return alpha > 0f;
				}
				return true;
			}
		}

		public int Wins
		{
			get
			{
				if (match == null || match.wins <= 0)
				{
					return winsNeeded;
				}
				return match.wins;
			}
		}

		public static bool Teams
		{
			get
			{
				if (match == null)
				{
					return false;
				}
				return match.teams > 0;
			}
		}

		public static bool Rounds
		{
			get
			{
				if (match == null)
				{
					return false;
				}
				return match.teams < 0;
			}
		}

		public static float RoundTime
		{
			get
			{
				if (!main || !main.live)
				{
					return -1f;
				}
				return NetworkManager.Clock - main.startedAt;
			}
		}

		public static float SinceStart
		{
			get
			{
				if (!main || !(main.startedAt > 0f))
				{
					return float.NaN;
				}
				return NetworkManager.Clock - main.startedAt;
			}
		}

		public static bool Waiting
		{
			get
			{
				if (match == null || !main || main.live)
				{
					return false;
				}
				return main.winner == null;
			}
		}

		public static bool Snaps
		{
			get
			{
				if (!main || main.rule == null)
				{
					return false;
				}
				return main.rule.cables;
			}
		}

		public static float HoldGrow
		{
			get
			{
				if (!main || main.rule == null)
				{
					return 0f;
				}
				return main.rule.hold;
			}
		}

		public static float OvertimeFrom
		{
			get
			{
				if (!main)
				{
					return 0f;
				}
				return main.overtimeFrom;
			}
		}

		public static float BeltRush
		{
			get
			{
				if (!main || main.rule == null || main.rule.belts <= 1f)
				{
					return 1f;
				}
				float t = Mathf.Clamp01((RoundTime - main.overtimeFrom) / Mathf.Max((main.roundLength - main.overtimeFrom) * 0.6f, 1f));
				return Mathf.Lerp(1f, main.rule.belts, t * t * (3f - 2f * t));
			}
		}

		public static float BeltLead
		{
			get
			{
				if (!main || main.rule == null || main.rule.belts <= 1f)
				{
					return 0f;
				}
				float elapsed = RoundTime - main.overtimeFrom;
				if (elapsed <= 0f)
				{
					return 0f;
				}
				float ramp = Mathf.Max((main.roundLength - main.overtimeFrom) * 0.6f, 1f);
				float u = Mathf.Min(elapsed / ramp, 1f);
				return (Mathf.Max(elapsed - ramp, 0f) + (1f - 0.5f * u) * (u * (u * (ramp * u)))) * (main.rule.belts - 1f);
			}
		}

		public static bool Travelling
		{
			get
			{
				if (!main || !main.boot)
				{
					return false;
				}
				return main.boot.Travelling;
			}
		}

		public static bool Outsider
		{
			get
			{
				if (match == null)
				{
					return false;
				}
				return !Listed(MyId);
			}
		}

		private void Awake()
		{
			main = this;
			watching = false;
			deathAudio = gameObject.AddComponent<AudioSource>();
			deathAudio.playOnAwake = false;
			deathAudio.spatialBlend = 0f;
			boot = UnityEngine.Object.FindFirstObjectByType<NetworkManager>();
			Thin();
			if (music)
			{
				Prefs.Track(music);
			}
			if (!loadHooked)
			{
				loadHooked = true;
				SceneManager.sceneLoaded += delegate
				{
					if (!main && loadSource)
					{
						loadSource.Stop();
					}
				};
			}
			titleHome = title.transform.localPosition;
			statusHome = status.transform.localPosition;
			titleSize = title.fontSize;
			statusSize = status.fontSize;
			subtitleMax = subtitle.fontSizeMax;
			subtitleBox = subtitle.rectTransform.sizeDelta;
			subtitleHome = subtitle.transform.localPosition;
			if (stripes)
			{
				stripesMat = stripes.material;
			}
			if (logo)
			{
				logoMat = logo.material;
			}
			if (logoMat)
			{
				logoTex = logoMat.mainTexture;
			}
			Transform runnerAt = screen.Find("Runner");
			runner = runnerAt ? runnerAt.GetComponent<Renderer>() : null;
			if (runner)
			{
				runnerMat = runner.material;
			}
			screen.gameObject.SetActive(false);
			if (carryMap == null && betaLeft > 0f)
			{
				if (SceneManager.GetActiveScene().buildIndex == 0)
				{
					ShowBeta();
				}
				else
				{
					betaLeft = 0f;
				}
			}
			if (carryMap != null)
			{
				Loading(carryMap);
				alpha = 1f;
				snapUntil = Time.time + 1f;
				if (boot && boot.localBody)
				{
					boot.localBody.dark = 1f;
					boot.localBody.blackout = 1f;
				}
			}
		}

		private void Thin()
		{
			crowded = Crowded();
			if (crowd == null || crowd.Length == 0)
			{
				return;
			}
			Player.Note(((match == null) ? 0 : match.scores.Count) + " players in the match, " + crowd.Length + " crowd extras " + (crowded ? "kept" : "removed"));
			if (crowded)
			{
				return;
			}
			foreach (GameObject extra in crowd)
			{
				if (extra)
				{
					UnityEngine.Object.DestroyImmediate(extra);
				}
			}
			if (boot && boot.spawnPoints != null)
			{
				boot.spawnPoints = Array.FindAll(boot.spawnPoints, (Transform p) => p != null);
			}
		}

		private static bool Crowded()
		{
			if (match == null)
			{
				return TestCrowd();
			}
			return match.crowd;
		}

		private static bool TestCrowd()
		{
			string session = Environment.GetEnvironmentVariable("PCGAME_SESSION");
			if (string.IsNullOrEmpty(session))
			{
				return false;
			}
			return session.Contains("crowd");
		}

		private void Start()
		{
			string scene = SceneManager.GetActiveScene().name;
			rule = Rounds ? null : overtime.Find((Overtime o) => o.scene == scene);
			if (rule == null)
			{
				return;
			}
			foreach (Water water in UnityEngine.Object.FindObjectsByType<Water>(FindObjectsSortMode.None))
			{
				if ((rule.water == "") ? (water == Water.Get()) : (water.name == rule.water))
				{
					riser = water;
				}
			}
			if (riser)
			{
				riserBase = riser.transform.position.y;
				riserLift = riser.buoyancy;
			}
		}

		public static bool SnapDue(NetRope rope)
		{
			if (!main || main.rule == null || !main.rule.cables)
			{
				return false;
			}
			float t = RoundTime;
			if (t < main.overtimeFrom)
			{
				return false;
			}
			if (main.snapAt.Count == 0)
			{
				main.ListCables();
			}
			if (main.snapAt.TryGetValue(rope, out float at))
			{
				return t >= at;
			}
			return false;
		}

		private void ListCables()
		{
			List<List<NetRope>> groups = new List<List<NetRope>>();
			List<NetRope> last = new List<NetRope>();
			foreach (NetRope n in UnityEngine.Object.FindObjectsByType<NetRope>(FindObjectsSortMode.None))
			{
				Rope rope = n.GetComponent<Rope>();
				if (!rope || !n.Object || !n.Object.IsValid)
				{
					continue;
				}
				if (rope.snapWear <= 0f)
				{
					last.Add(n);
					continue;
				}
				List<NetRope> group = groups.Find((List<NetRope> g) => g[0].transform.parent == n.transform.parent);
				if (group == null)
				{
					group = new List<NetRope>();
					groups.Add(group);
				}
				group.Add(n);
			}
			foreach (List<NetRope> group in groups)
			{
				group.Sort((NetRope a, NetRope b) => a.Object.Id.Raw.CompareTo(b.Object.Id.Raw));
			}
			groups.Sort((List<NetRope> a, List<NetRope> b) => a[0].Object.Id.Raw.CompareTo(b[0].Object.Id.Raw));
			List<NetRope> order = new List<NetRope>();
			if (rule.inOrder)
			{
				groups.Sort((List<NetRope> a, List<NetRope> b) => string.CompareOrdinal(a[0].transform.parent ? a[0].transform.parent.name : "", b[0].transform.parent ? b[0].transform.parent.name : ""));
				foreach (List<NetRope> group in groups)
				{
					order.AddRange(group);
				}
			}
			for (int i = 0; !rule.inOrder; i++)
			{
				int before = order.Count;
				foreach (List<NetRope> group in groups)
				{
					if (i < group.Count)
					{
						order.Add(group[i]);
					}
				}
				if (order.Count == before)
				{
					break;
				}
			}
			float span = roundLength - overtimeFrom;
			for (int i = 0; i < order.Count; i++)
			{
				float f = (order.Count < 2) ? 1f : ((float)i / (float)(order.Count - 1));
				snapAt[order[i]] = overtimeFrom + span * f;
			}
			foreach (NetRope n in last)
			{
				float at = roundLength;
				foreach (List<NetRope> group in groups)
				{
					if (group[0].transform.parent != n.transform.parent)
					{
						continue;
					}
					at = 0f;
					foreach (NetRope r in group)
					{
						at = Mathf.Max(at, snapAt[r]);
					}
				}
				snapAt[n] = at;
			}
			Player.Note("overtime: " + order.Count + " cables snap between " + overtimeFrom.ToString("F0") + " and " + roundLength.ToString("F0") + " s, " + last.Count + " chains with their last cable");
		}

		private void Flood()
		{
			float t = RoundTime;
			if (!riser || t < 0f)
			{
				return;
			}
			float rise = Mathf.Clamp01((t - overtimeFrom) / Mathf.Max(roundLength - overtimeFrom, 1f));
			Vector3 at = riser.transform.position;
			riser.transform.position = new Vector3(at.x, riserBase + rise * rule.rise, at.z);
			float sink = Mathf.Clamp01((t - roundLength) / Mathf.Max(sinkTime, 0.1f));
			riser.buoyancy = riserLift * (1f - sink);
			if (sink > 0f && !sinking)
			{
				sinking = true;
				Player.Note("overtime over, " + riser.name + " stops floating things");
			}
		}

		public static void Died()
		{
			if (main && main.deathSound && main.deathAudio && match != null)
			{
				main.deathAudio.PlayOneShot(main.deathSound, main.deathVolume * Prefs.sfx);
			}
		}

		private void Update()
		{
			steady = (Time.unscaledDeltaTime < 0.02f) ? (steady + 1) : 0;
			if (!boot)
			{
				return;
			}
			if (showing && (loading || beta || asking) && boot.localBody)
			{
				boot.localBody.blackout = 1f;
			}
			if (boot.Travelling)
			{
				return;
			}
			if (match == null)
			{
				if (boot.InRoom && Adopt())
				{
					return;
				}
				bool early = steady < 30 && Time.timeSinceLevelLoad <= 8f;
				if (showing && loading && ((boot.localAvatar && !early) || boot.error != "" || (!boot.Busy && !boot.InRoom && Time.timeSinceLevelLoad > 2f)))
				{
					Hide();
				}
				return;
			}
			if (boot.error != "" && ((!live && winner == null) || boot.error == "CONNECTION LOST" || boot.error == "LOADING TOOK TOO LONG"))
			{
				if (match != null && rejoined != match.id && Listed(MyId) && boot.error != "AUTHENTICATION FAILED\n<size=70%>TRY UPDATING YOUR HEADSET</size>" && boot.Rejoin(match))
				{
					rejoined = match.id;
					return;
				}
				Player.Note("round connect failed, back to the lobby: " + boot.error + " (" + boot.status + ")");
				match = null;
				Hide();
				boot.GoTo(lobbyScene, boot.error);
				return;
			}
			if (boot.IsHost && winner == null)
			{
				Publish();
			}
			if (live || winner != null)
			{
				Flood();
				if (goAt >= 0f && Time.time > goAt + 0.8f)
				{
					goAt = -1f;
					Hide();
				}
				Clock();
				if (winner == null)
				{
					if (Rounds && !watching)
					{
						Drop();
					}
					string win = boot.Property("win");
					if (win == null)
					{
						if (boot.IsHost)
						{
							Judge();
						}
					}
					else
					{
						Declare(win);
					}
					return;
				}
				float since = Time.time - winAt;
				bool done = Decided() || deserted || over;
				if (done && !champion && !deserted && !over && since > showWinner)
				{
					champion = true;
					Champion();
				}
				if (boot.IsHost && !hopSent && since > showWinner + (done ? showChampion : 0f) && !Pending())
				{
					NextHop();
				}
				return;
			}
			Hold(true);
			if (!boot.InRoom || !boot.localAvatar)
			{
				return;
			}
			if (!watching && !Listed(MyId))
			{
				Watch();
			}
			if (boot.Property("win") == Left)
			{
				Hold(false);
				Declare(Left);
				return;
			}
			string start = boot.Property("start");
			if (start == null)
			{
				int count = Count(out int alive, out NetworkPlayer standing);
				int expect = (match.expect > 0) ? match.expect : match.scores.Count;
				if (!boot.IsHost || (count < Mathf.Max(1, expect) && Time.time < boot.joinedAt + waitForPlayers))
				{
					return;
				}
				if (count == 0 || Deserted())
				{
					boot.SetProperty("win", Left);
				}
				else
				{
					boot.SetProperty("start", (NetworkManager.Clock + countdown + 0.5f).ToString("R", CultureInfo.InvariantCulture));
				}
				return;
			}
			float at = float.Parse(start, CultureInfo.InvariantCulture);
			startedAt = at;
			float wait = at - NetworkManager.Clock;
			PlayMusic();
			if (wait > 0f)
			{
				int left = Mathf.Clamp(Mathf.CeilToInt(wait), 1, Mathf.CeilToInt(countdown));
				if (left != shownCount)
				{
					shownCount = left;
					Banner(left.ToString(), Color.white, MyTeam(), Round());
				}
				return;
			}
			startedAt = at;
			live = true;
			goAt = Time.time;
			Hold(false);
			if (!watching)
			{
				Banner("GO!", Color.white, MyTeam(), Round());
			}
			else
			{
				Banner("SPECTATING", Color.white, "YOU'LL PLAY NEXT ROUND", Round());
				goAt = Time.time + 3f;
			}
			Player.Note("round " + match.round + " (first to " + Wins + ") live on " + Title(match.scene) + ", " + Count(out int _, out NetworkPlayer _) + " players: " + Who());
		}

		private bool Pending()
		{
			Hop hop = Decode(boot.Property("go") ?? "");
			if (hop != null)
			{
				return !followed.Contains(hop.id);
			}
			return false;
		}

		private void Clock()
		{
			float t = RoundTime;
			int left = Mathf.CeilToInt(roundLength + timeUp - t);
			if (winner == null && t >= roundLength && left > 0)
			{
				if (!clock)
				{
					MakeClock();
				}
				if (!clock.gameObject.activeSelf)
				{
					clock.gameObject.SetActive(true);
					clockShown = -1;
					PlaceClock(true);
					Player.Note("round " + match.round + " passed " + roundLength.ToString("F0") + " s, it ends in " + left + " s");
				}
				if (left != clockShown)
				{
					clockShown = left;
					clock.text = "TIME RUNNING OUT\n" + left;
					clock.color = (left > 10) ? Color.white : ClockRed;
				}
			}
			else if (clock && clock.gameObject.activeSelf)
			{
				clock.gameObject.SetActive(false);
			}
		}

		private void MakeClock()
		{
			GameObject go = new GameObject("Round Clock");
			clock = go.AddComponent<TextMeshPro>();
			clock.font = title.font;
			clock.fontSharedMaterial = title.fontSharedMaterial;
			clock.rectTransform.sizeDelta = new Vector2(1.6f, 0.6f);
			clock.enableAutoSizing = true;
			clock.fontSizeMin = 0.3f;
			clock.fontSizeMax = 1.2f;
			clock.fontStyle = FontStyles.Bold;
			clock.alignment = TextAlignmentOptions.Center;
			clock.text = "";
			go.SetActive(false);
		}

		private void Ping()
		{
			Camera cam = null;
			if (boot && boot.InRoom && boot.localAvatar)
			{
				pingShown = Mathf.Lerp(pingShown, NetworkPlayer.rtt, 1f - Mathf.Exp(-PingEase * Time.unscaledDeltaTime));
				pingBad = pingShown > (pingBad ? PingOk : PingBad);
				if (pingBad)
				{
					cam = Camera.main;
				}
			}
			else
			{
				pingShown = 0f;
				pingBad = false;
			}
			if (!cam)
			{
				if (ping && ping.gameObject.activeSelf)
				{
					ping.gameObject.SetActive(false);
				}
				return;
			}
			if (!ping)
			{
				MakePing();
			}
			if (!ping.gameObject.activeSelf)
			{
				ping.gameObject.SetActive(true);
			}
			ping.text = "BAD PING " + Mathf.RoundToInt(pingShown * 1000f) + "ms";
			Transform head = cam.transform;
			ping.transform.SetPositionAndRotation(head.position + head.rotation * PingSpot, head.rotation);
		}

		private void MakePing()
		{
			GameObject go = new GameObject("Ping Warning");
			ping = go.AddComponent<TextMeshPro>();
			ping.font = title.font;
			ping.fontSharedMaterial = title.fontSharedMaterial;
			ping.rectTransform.sizeDelta = new Vector2(0.6f, 0.1f);
			ping.fontSize = 0.28f;
			ping.fontStyle = FontStyles.Bold;
			ping.alignment = TextAlignmentOptions.Right;
			ping.color = PingTint;
			ping.text = "";
		}

		private void PlaceClock(bool snap)
		{
			if (!clock || !clock.gameObject.activeSelf)
			{
				return;
			}
			Camera cam = Camera.main;
			if (!cam)
			{
				return;
			}
			Transform head = cam.transform;
			Vector3 forward = head.forward;
			Vector3 flat = (forward.x * forward.x + forward.z * forward.z < 0.0001f) ? Vector3.forward : new Vector3(forward.x, 0f, forward.z);
			flat = flat.normalized;
			Vector3 want = head.position + flat * ClockDistance + Vector3.up * ClockRise;
			Transform t = clock.transform;
			t.position = snap ? want : Vector3.Lerp(t.position, want, 1f - Mathf.Exp(-ClockFollow * Time.unscaledDeltaTime));
			t.rotation = Quaternion.LookRotation(t.position - head.position, Vector3.up);
		}

		private void LateUpdate()
		{
			PlaceClock(false);
			Ping();
			float dt = Time.unscaledDeltaTime;
			if (beta)
			{
				betaLeft -= Mathf.Min(dt, BetaStep);
				if (betaLeft <= 0f)
				{
					Hide();
				}
			}
			if (music && music.isPlaying)
			{
				if (musicPeak < 0f)
				{
					musicPeak = music.volume;
				}
				if (winner != null)
				{
					music.volume = Mathf.MoveTowards(music.volume, 0f, dt * musicPeak / Mathf.Max(musicFade, 0.01f));
				}
				if (music.volume <= 0f)
				{
					music.Stop();
				}
			}
			alpha = Mathf.MoveTowards(alpha, showing ? 1f : 0f, dt / fadeTime);
			LoadMusic(dt);
			bool on = alpha > 0f;
			if (on != screen.gameObject.activeSelf)
			{
				screen.gameObject.SetActive(on);
			}
			if (!on)
			{
				return;
			}
			Keep();
			title.alpha = alpha;
			status.alpha = alpha;
			subtitle.alpha = alpha;
			if (stripesMat)
			{
				stripesMat.SetFloat("_Alpha", alpha);
				stripesMat.SetFloat("_Clock", Time.unscaledTime);
				stripesMat.SetColor("_ColorA", backTint * 0.42f);
				stripesMat.SetColor("_ColorB", backTint * 0.33f);
			}
			if (logoMat)
			{
				logoMat.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
			}
			if (runnerMat && runner.gameObject.activeSelf)
			{
				float frame = Mathf.Floor(Time.unscaledTime / RunFrame) % (float)RunFrames;
				runnerMat.mainTextureOffset = new Vector2(frame / (float)RunFrames, 0f);
				runnerMat.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
			}
		}

		private void LoadMusic(float dt)
		{
			float want = (loading && loadingMusic) ? (loadingVolume * Prefs.music * alpha) : 0f;
			if (!loadSource)
			{
				if (want <= 0f)
				{
					return;
				}
				GameObject go = new GameObject("Loading Music");
				UnityEngine.Object.DontDestroyOnLoad(go);
				loadSource = go.AddComponent<AudioSource>();
				loadSource.playOnAwake = false;
				loadSource.loop = true;
				loadSource.spatialBlend = 0f;
				loadSource.bypassListenerEffects = true;
				loadSource.volume = 0f;
			}
			if (want > 0f && !loadSource.isPlaying)
			{
				loadSource.clip = loadingMusic;
				loadSource.volume = 0f;
				loadSource.Play();
			}
			loadSource.volume = Mathf.MoveTowards(loadSource.volume, want, dt / Mathf.Max(fadeTime, 0.01f) * Mathf.Max(loadingVolume, 0.01f));
			if (want <= 0f && loadSource.volume <= 0f && loadSource.isPlaying)
			{
				loadSource.Stop();
			}
		}

		private void Layout(bool load, bool fresh)
		{
			if (stripes)
			{
				stripes.gameObject.SetActive(load);
			}
			if (logo)
			{
				logo.gameObject.SetActive(load);
			}
			if (runner)
			{
				runner.gameObject.SetActive(false);
			}
			title.transform.localPosition = new Vector3(titleHome.x, load ? (-0.33f) : titleHome.y, titleHome.z);
			status.transform.localPosition = new Vector3(statusHome.x, load ? (-0.58f) : statusHome.y, statusHome.z);
			title.fontSize = load ? (titleSize * 0.75f) : titleSize;
			status.fontSize = load ? (statusSize * 0.85f) : statusSize;
			subtitle.fontSizeMax = subtitleMax;
			subtitle.rectTransform.sizeDelta = subtitleBox;
			subtitle.transform.localPosition = subtitleHome;
			if (load && fresh)
			{
				int pick = UnityEngine.Random.Range(0, BackTints.Length - 1);
				if (backPick >= 0 && pick >= backPick)
				{
					pick++;
				}
				backPick = pick;
				backTint = BackTints[pick];
			}
		}

		public void Loading(string map)
		{
			if (alpha <= 0f)
			{
				Place();
			}
			EndBeta();
			asking = false;
			showing = true;
			Layout(true, carryMap == null);
			if (logoMat)
			{
				logoMat.mainTexture = (Rounds && roundsIcon) ? roundsIcon : logoTex;
			}
			loading = true;
			title.text = "LOADING";
			title.color = Color.white;
			status.text = "[" + map + "]";
			status.color = MapYellow;
			subtitle.text = "";
			carryMap = map;
		}

		private void Banner(string head, Color color, string line, string sub)
		{
			if (alpha <= 0f)
			{
				Place();
			}
			EndBeta();
			asking = false;
			showing = true;
			Layout(false, false);
			loading = false;
			carryMap = null;
			if (boot && boot.localBody)
			{
				boot.localBody.blackout = 0f;
			}
			title.text = head;
			title.color = color;
			status.text = line;
			status.color = Color.white;
			subtitle.text = sub;
		}

		public void Ask(string head, string line, string sub)
		{
			Banner(head, Color.white, line, sub);
			subtitle.fontSizeMax = subtitleMax * 1.5f;
			subtitle.rectTransform.sizeDelta = new Vector2(subtitleBox.x, subtitleBox.y * 1.5f);
			subtitle.transform.localPosition = subtitleHome + Vector3.down * subtitleBox.y * 0.35f;
			asking = true;
			if (boot && boot.localBody)
			{
				boot.localBody.dark = 1f;
				boot.localBody.blackout = 1f;
			}
		}

		public void Answered()
		{
			if (asking)
			{
				Hide();
			}
		}

		private void Hide()
		{
			asking = false;
			EndBeta();
			showing = false;
			carryMap = null;
			if (boot && boot.localBody)
			{
				boot.localBody.blackout = 0f;
			}
		}

		private void ShowBeta()
		{
			showing = true;
			beta = true;
			Layout(false, false);
			if (runner)
			{
				runner.gameObject.SetActive(true);
			}
			loading = false;
			title.text = "GAME IS IN BETA";
			title.color = Color.white;
			status.text = "";
			subtitle.text = "";
			alpha = 1f;
			snapUntil = Time.time + 1f;
			if (boot && boot.localBody)
			{
				boot.localBody.dark = 1f;
				boot.localBody.blackout = 1f;
			}
		}

		private void EndBeta()
		{
			if (beta)
			{
				beta = false;
				betaLeft = 0f;
			}
		}

		public void Leaving(Hop hop)
		{
			Loading((hop.round > 0) ? Title(hop.scene) : "LOBBY");
		}

		private void Place()
		{
			Camera cam = Camera.main;
			if (cam)
			{
				Vector3 at = Goal(cam, out Quaternion facing);
				screen.SetPositionAndRotation(at, facing);
			}
		}

		private Vector3 Goal(Camera cam, out Quaternion facing)
		{
			Vector3 forward = cam.transform.forward;
			Vector3 flat = (forward.x * forward.x + forward.z * forward.z < 0.0001f) ? Vector3.forward : new Vector3(forward.x, 0f, forward.z);
			flat = flat.normalized;
			facing = Quaternion.LookRotation(flat);
			return cam.transform.position + flat * distance;
		}

		private void Keep()
		{
			Camera cam = Camera.main;
			if (!cam)
			{
				return;
			}
			if (Time.time < snapUntil)
			{
				Place();
				return;
			}
			Vector3 goal = Goal(cam, out Quaternion facing);
			Vector3 at = screen.position;
			Vector3 eye = cam.transform.position;
			Vector3 ahead = facing * Vector3.forward;
			if (!recentring)
			{
				Vector3 flat = new Vector3(at.x - eye.x, 0f, at.z - eye.z);
				if (Vector3.Angle(flat, ahead) > 50f || Mathf.Abs(flat.magnitude - distance) > 0.8f || Mathf.Abs(screen.position.y - goal.y) > 0.6f)
				{
					recentring = true;
				}
			}
			if (recentring)
			{
				float k = 1f - Mathf.Exp(-5f * Time.unscaledDeltaTime);
				screen.SetPositionAndRotation(Vector3.Lerp(screen.position, goal, k), Quaternion.Slerp(screen.rotation, facing, k));
				if ((screen.position - goal).sqrMagnitude < 0.0025f)
				{
					recentring = false;
				}
			}
		}

		private int Count(out int alive, out NetworkPlayer standing)
		{
			alive = 0;
			standing = null;
			int count = 0;
			foreach (NetworkPlayer a in NetworkPlayer.avatars)
			{
				if (!a || !a.Object || !a.Object.IsValid || (bool)a.Watching || !boot.Present(a.Object.StateAuthority))
				{
					continue;
				}
				if (!a.Dead)
				{
					alive++;
					standing = a;
				}
				count++;
			}
			return count;
		}

		private void Judge()
		{
			if (sentWin)
			{
				return;
			}
			if (Rounds)
			{
				Survive();
				return;
			}
			if (RoundTime >= roundLength + timeUp)
			{
				sentWin = true;
				boot.SetProperty("win", OutOfTime);
				return;
			}
			if (Count(out int alive, out NetworkPlayer standing) == 0)
			{
				if (match.scores.Count != 0)
				{
					Player.Note("nobody in round " + match.round + " is still playing, ending it");
					sentWin = true;
					boot.SetProperty("win", Left);
				}
				return;
			}
			int sides = Sides(false, out alive);
			int standingSides = Sides(true, out int team);
			if (sides <= 1)
			{
				if (match.scores.Count >= 2)
				{
					if (Deserted())
					{
						sentWin = true;
						boot.SetProperty("win", Left);
					}
					return;
				}
				if (standingSides != 0)
				{
					return;
				}
				sentWin = true;
				boot.SetProperty("win", "-");
				return;
			}
			if (standingSides >= 2)
			{
				return;
			}
			sentWin = true;
			string win;
			if (standingSides != 1)
			{
				win = "-";
			}
			else if (match.teams <= 0)
			{
				win = standing.Uid.Value;
			}
			else
			{
				win = (team > 0) ? (TeamWin + team) : "-";
			}
			boot.SetProperty("win", win);
		}

		private void Survive()
		{
			if (RoundTime >= roundLength + timeUp)
			{
				sentWin = true;
				boot.SetProperty("win", OutOfTime);
				return;
			}
			int count = Count(out int alive, out NetworkPlayer _);
			string win;
			if (count == 0 || alive == 0)
			{
				if (match.scores.Count == 0)
				{
					return;
				}
				sentWin = true;
				win = (count == 0) ? Left : Fallen;
			}
			else
			{
				if (RoundTime < 5.5f || NetworkNpc.Remaining() > 0 || Owing())
				{
					return;
				}
				sentWin = true;
				win = Cleared;
			}
			boot.SetProperty("win", win);
		}

		private bool Owing()
		{
			foreach (NetworkPlayer a in NetworkPlayer.avatars)
			{
				if (a && a.Object && a.Object.IsValid && !a.Watching && !a.Dead && boot.Present(a.Object.StateAuthority) && a.Owed != 1)
				{
					return true;
				}
			}
			return false;
		}

		public static int Owed()
		{
			if (!main || !Rounds || !main.live || watching || main.winner != null)
			{
				return 0;
			}
			Player body = main.boot ? main.boot.localBody : null;
			int owed = (!body || body.dead || body.drowned) ? 1 : (Mathf.Max(0, main.Mine().Count - main.spawned) + 1);
			return owed + NetworkNpc.Owned();
		}

		public static List<SlapDummy.Kind> Wave(int round)
		{
			string wave;
			if (round <= Waves.Length)
			{
				wave = Waves[Mathf.Max(round, 1) - 1];
			}
			else
			{
				int extra = round - Waves.Length;
				int big = Mathf.Min(extra / 2, 2);
				int fat = Mathf.Min(extra / 3, 1);
				wave = new string('S', 1 - fat) + new string('N', 3 - big) + new string('F', fat + 1) + new string('B', big + 1);
			}
			List<SlapDummy.Kind> list = new List<SlapDummy.Kind>();
			foreach (char c in wave)
			{
				list.Add((c == 'B') ? SlapDummy.Kind.Big : ((c == 'N') ? SlapDummy.Kind.Normal : ((c == 'F') ? SlapDummy.Kind.Fat : SlapDummy.Kind.Small)));
			}
			return list;
		}

		private void Drop()
		{
			int kind;
			Player body = boot.localBody;
			if (!boot.runner || !boot.runner.IsRunning || !body || body.dead || body.drowned || !body.pelvis)
			{
				return;
			}
			List<SlapDummy.Kind> list = Mine();
			int cap = Mathf.Clamp(RoomAlive / Mathf.Max(1, match.scores.Count), 1, MineAlive);
			if (spawned >= list.Count || RoundTime < SpawnLead || RoundTime < droppedAt + SpawnGap || NetworkNpc.Dropped(boot.runner.LocalPlayer) >= cap || NetworkNpc.Remaining() > 7)
			{
				return;
			}
			if (!npcPrefab)
			{
				npcPrefab = Resources.Load<NetworkObject>("NetworkNpc");
			}
			if (!npcPrefab)
			{
				return;
			}
			Vector3 at = Sky(body.pelvis.position, body.Standing, out string why);
			kind = (int)list[spawned];
			float dx = body.pelvis.position.x - at.x;
			float dz = body.pelvis.position.z - at.z;
			Quaternion facing = (dx * dx + dz * dz > 0.01f) ? Quaternion.LookRotation(new Vector3(dx, 0f, dz)) : Quaternion.identity;
			boot.runner.Spawn(npcPrefab, at, facing, null, delegate(NetworkRunner r, NetworkObject o)
			{
				NetworkNpc npc = o.GetComponent<NetworkNpc>();
				npc.Kind = kind;
				npc.Spawner = r.LocalPlayer;
			});
			spawned++;
			droppedAt = RoundTime;
			Player.Note("round " + match.round + ": dropped a " + list[spawned - 1] + " from the sky (" + spawned + "/" + list.Count + ", " + NetworkNpc.Dropped(boot.runner.LocalPlayer) + " of mine alive, cap " + cap + ") " + why);
		}

		private List<SlapDummy.Kind> Mine()
		{
			if (mine != null)
			{
				return mine;
			}
			mine = new List<SlapDummy.Kind>();
			List<string> ids = match.scores.ConvertAll((Score s) => s.id);
			ids.Sort(string.CompareOrdinal);
			if (ids.Count == 0)
			{
				ids.Add(MyId);
			}
			List<SlapDummy.Kind> wave = Wave(match.round);
			int players = ids.Count;
			int total = wave.Count + (wave.Count + 2) / 3 * (players - 1);
			System.Random random = new System.Random((int)Roll(match.id, match.round));
			int[] load = new int[ids.Count];
			for (int i = 0; i < total; i++)
			{
				int k = random.Next(ids.Count);
				while (load[k] >= (players - 1 + total) / players + 1)
				{
					k = (k + 1) % ids.Count;
				}
				load[k]++;
				if (ids[k] == MyId)
				{
					mine.Add(wave[i % wave.Count]);
				}
			}
			Player.Note("round " + match.round + ": " + mine.Count + " of " + total + " NPCs drop near me");
			return mine;
		}

		public static Vector3 Sky(Vector3 near, Rigidbody under, out string why)
		{
			bool backfaces = Physics.queriesHitBackfaces;
			Physics.queriesHitBackfaces = true;
			try
			{
				return Spot(near, under, out why);
			}
			finally
			{
				Physics.queriesHitBackfaces = backfaces;
			}
		}

		private static Vector3 Spot(Vector3 near, Rigidbody under, out string why)
		{
			Vector3 floor = Ground(near, 3f, out RaycastHit below) ? below.point : (near - Vector3.up * 0.9f);
			float lid = Mathf.Max(Lid(floor + Vector3.up * 0.1f, 40f), 0.3f);
			float top = floor.y + lid;
			float reach = top - floor.y + StepDown + 1f;
			Vector3 eye = floor + Vector3.up * Mathf.Min(lid, 1.7f);
			for (int i = 0; i < 56; i++)
			{
				float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
				float range = (i < 28) ? UnityEngine.Random.Range(SkyNear, SkyFar) : ((i < 44) ? UnityEngine.Random.Range(SkyClose, SkyNear) : UnityEngine.Random.Range(SkyTouch, SkyClose));
				if (!Ground(new Vector3(near.x + Mathf.Cos(angle) * range, top, near.z + Mathf.Sin(angle) * range), reach, out RaycastHit hit))
				{
					continue;
				}
				float rise = hit.point.y - floor.y;
				if (rise > 1.2f || rise < -StepDown || !Landing(hit, under) || Walled(eye, hit.point + Vector3.up) || !Walkable(floor, hit.point, under))
				{
					continue;
				}
				float room = Room(hit.point);
				if (room < 0f)
				{
					continue;
				}
				if (hit.rigidbody && !hit.rigidbody.isKinematic)
				{
					room = Mathf.Min(room, 1.2f);
				}
				float up = Mathf.Min(room, SkyHeight);
				float drop = Mathf.Max(up, 0.05f);
				if (!Taken(hit.point, drop))
				{
					why = "on " + hit.collider.name + " " + Vector3.Distance(Flat(hit.point), Flat(floor)).ToString("F1") + " m away, " + up.ToString("F1") + " m up";
					return hit.point + Vector3.up * drop;
				}
			}
			float overhead = Room(floor);
			if (overhead < 2f && Spawned(floor, out Vector3 at, out string name))
			{
				why = "no open floor near, at spawn " + name;
				return at;
			}
			why = "above me, no open floor near";
			return floor + Vector3.up * Mathf.Clamp(overhead, 2f, SkyHeight);
		}

		private static bool Spawned(Vector3 foot, out Vector3 at, out string name)
		{
			at = foot;
			name = "";
			NetworkManager net = NetworkManager.main;
			if (!net || net.spawnPoints == null || net.spawnPoints.Length < 1)
			{
				return false;
			}
			float best = float.MaxValue;
			foreach (Transform point in net.spawnPoints)
			{
				if (!point || !Ground(point.position + Vector3.up * 1.5f, 6f, out RaycastHit hit) || !Landing(hit, null))
				{
					continue;
				}
				float room = Room(hit.point);
				if (room < 0f)
				{
					continue;
				}
				float d = (hit.point - foot).sqrMagnitude;
				if (!(d >= best))
				{
					if (hit.rigidbody && !hit.rigidbody.isKinematic)
					{
						room = Mathf.Min(room, 1.2f);
					}
					at = hit.point + Vector3.up * Mathf.Clamp(room, 0.05f, SkyHeight);
					name = point.name;
					best = d;
				}
			}
			return best < float.MaxValue;
		}

		private static bool Taken(Vector3 at, float drop)
		{
			int count = Physics.OverlapCapsuleNonAlloc(at + Vector3.up * 0.8f, at + Vector3.up * (drop + NpcTall - DropWide), DropWide, dropHits, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < count; i++)
			{
				Collider c = dropHits[i];
				if (c && ((bool)c.GetComponentInParent<Player>() || (bool)c.GetComponentInParent<NetworkNpc>() || (bool)c.GetComponentInParent<AvatarBody>() || (bool)c.GetComponentInParent<NetworkPlayer>()))
				{
					return true;
				}
			}
			return false;
		}

		private static float Lid(Vector3 from, float dist)
		{
			int count = Physics.RaycastNonAlloc(from, Vector3.up, skyHits, dist, -1, QueryTriggerInteraction.Ignore);
			float lid = dist;
			for (int i = 0; i < count; i++)
			{
				Collider c = skyHits[i].collider;
				if (c && !(skyHits[i].distance >= lid) && !c.GetComponentInParent<Player>() && !c.GetComponentInParent<NetworkPlayer>() && !c.GetComponentInParent<AvatarBody>() && !c.GetComponentInParent<NetworkNpc>())
				{
					lid = skyHits[i].distance;
				}
			}
			return lid;
		}

		public static Rigidbody Brinks(Vector3 body, float[] dist, Vector3[] stand, Vector3[] land, Vector3[] edge)
		{
			bool backfaces = Physics.queriesHitBackfaces;
			Physics.queriesHitBackfaces = true;
			for (int i = 0; i < dist.Length; i++)
			{
				dist[i] = -1f;
			}
			try
			{
				if (!Ground(body + Vector3.up * 0.5f, 3f, out RaycastHit hit))
				{
					return null;
				}
				Rigidbody under = hit.rigidbody;
				for (int i = 0; i < dist.Length; i++)
				{
					Vector3 dir = Quaternion.Euler(0f, (float)i * 360f / (float)dist.Length, 0f) * Vector3.forward;
					dist[i] = Brink(hit.point, dir, under, out stand[i], out land[i], out edge[i]);
				}
				return under;
			}
			finally
			{
				Physics.queriesHitBackfaces = backfaces;
			}
		}

		public static bool Deadly(Vector3 at, Vector3 way, out float surface)
		{
			bool backfaces = Physics.queriesHitBackfaces;
			Physics.queriesHitBackfaces = true;
			try
			{
				return Lethal(at, way, out surface);
			}
			finally
			{
				Physics.queriesHitBackfaces = backfaces;
			}
		}

		private static float Brink(Vector3 from, Vector3 dir, Rigidbody under, out Vector3 stand, out Vector3 land, out Vector3 edge)
		{
			stand = from;
			land = from;
			edge = from;
			Vector3 at = from;
			Vector3 last = from;
			for (int i = 1; i <= BrinkSteps; i++)
			{
				Vector3 probe = from + dir * ((float)i * StepGap);
				probe.y = at.y;
				if (Ground(probe + Vector3.up * StepUp * 2f, 2.4f, out RaycastHit hit))
				{
					if (Mathf.Abs(hit.point.y - at.y) <= StepUp && Landing(hit, under))
					{
						last = at;
						at = hit.point;
						continue;
					}
					if (hit.point.y > at.y + StepUp)
					{
						return -1f;
					}
				}
				if (Walled(from + Vector3.up * 0.5f, probe + Vector3.up * 0.5f) || !Lethal(probe, dir, out float surface))
				{
					return -1f;
				}
				float run = 0f;
				while (run < TossRun && Lethal(probe + dir * (run + StepGap), dir, out float _))
				{
					run += StepGap;
				}
				land = probe + dir * (run * 0.5f);
				land.y = surface;
				edge = probe;
				stand = (i > 3) ? last : at;
				return (float)(i - 1) * StepGap;
			}
			return -1f;
		}

		private static bool Lethal(Vector3 at, Vector3 way, out float surface)
		{
			Vector3 side = Vector3.Cross(Vector3.up, way).normalized;
			if (Column(at, out surface) && Column(at + side * Wide, out float other))
			{
				return Column(at - side * Wide, out other);
			}
			return false;
		}

		private static bool Column(Vector3 q, out float surface)
		{
			bool ground = Ground(q + Vector3.up * 1.2f, 300f, out RaycastHit hit);
			foreach (Water acid in Water.acids)
			{
				if (acid && acid.Inside(q) && (!ground || hit.point.y < acid.SurfaceAt(q)))
				{
					surface = acid.SurfaceAt(q);
					return true;
				}
			}
			Water water = Water.main;
			if (water && !water.acid && (!ground || hit.point.y < water.SurfaceAt(q) - 0.3f))
			{
				surface = water.SurfaceAt(q);
				return true;
			}
			if (!ground)
			{
				surface = q.y;
				return false;
			}
			surface = hit.point.y;
			return TrainWreck.Moving(hit.rigidbody);
		}

		private static bool Ground(Vector3 from, float dist, out RaycastHit hit)
		{
			int count = Physics.RaycastNonAlloc(from, Vector3.down, skyHits, dist, -1, QueryTriggerInteraction.Ignore);
			hit = default(RaycastHit);
			float best = float.MaxValue;
			for (int i = 0; i < count; i++)
			{
				Collider c = skyHits[i].collider;
				if (c && !(skyHits[i].distance >= best) && !c.GetComponentInParent<Player>() && !c.GetComponentInParent<NetworkPlayer>() && !c.GetComponentInParent<AvatarBody>() && !c.GetComponentInParent<NetworkNpc>())
				{
					best = skyHits[i].distance;
					hit = skyHits[i];
				}
			}
			return best < float.MaxValue;
		}

		private static Vector3 Flat(Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		private static bool Walkable(Vector3 from, Vector3 to, Rigidbody under)
		{
			Vector3 d = Flat(to - from);
			int steps = Mathf.CeilToInt(d.magnitude / StepGap);
			float y = from.y;
			for (int i = 1; i <= steps; i++)
			{
				float k = (float)i / (float)steps;
				if (!Ground(new Vector3(from.x + d.x * k, y + 1.2f, from.z + d.z * k), 2.7f, out RaycastHit hit))
				{
					return false;
				}
				float rise = hit.point.y - y;
				if (rise > StepUp || rise < -StepDown || !Landing(hit, under))
				{
					return false;
				}
				y = hit.point.y;
			}
			return Mathf.Abs(to.y - y) <= StepUp;
		}

		private static float Room(Vector3 at)
		{
			if (!Physics.Raycast(at + Vector3.up * 0.2f, Vector3.up, out RaycastHit hit, SkyHeight + NpcTall, -1, QueryTriggerInteraction.Ignore))
			{
				return SkyHeight;
			}
			return hit.distance + 0.2f - NpcTall;
		}

		private static bool Landing(RaycastHit hit, Rigidbody under)
		{
			Rigidbody rb = hit.rigidbody;
			if (((bool)rb && !rb.isKinematic && rb != under) || TrainWreck.Desert(rb) || hit.collider.gameObject.layer == 11 || (bool)hit.collider.GetComponentInParent<Player>() || (bool)hit.collider.GetComponentInParent<NetworkPlayer>())
			{
				return false;
			}
			return !SlapDummy.Hazard(hit.point);
		}

		private static bool Walled(Vector3 from, Vector3 to)
		{
			Vector3 d = to - from;
			float length = d.magnitude;
			if (length < 0.01f)
			{
				return false;
			}
			int count = Physics.RaycastNonAlloc(from, d / length, skyHits, length, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < count; i++)
			{
				Collider c = skyHits[i].collider;
				if (c && (!c.attachedRigidbody || c.attachedRigidbody.isKinematic) && !c.GetComponentInParent<Player>() && !c.GetComponentInParent<NetworkPlayer>() && !c.GetComponentInParent<AvatarBody>() && !c.GetComponentInParent<NetworkNpc>())
				{
					return true;
				}
			}
			return false;
		}

		private int Sides(bool standingOnly, out int team)
		{
			team = 0;
			int sides = 0;
			int seen = 0;
			foreach (NetworkPlayer a in NetworkPlayer.avatars)
			{
				if (!a || !a.Object || !a.Object.IsValid || (bool)a.Watching || !boot.Present(a.Object.StateAuthority) || (standingOnly && (bool)a.Dead))
				{
					continue;
				}
				int side = -1;
				if (match.teams > 0)
				{
					int t = TeamOf(a.Uid.Value);
					if (t >= 1)
					{
						int bit = 1 << t;
						if ((bit & seen) != 0)
						{
							continue;
						}
						seen |= bit;
						side = t;
					}
				}
				team = side;
				sides++;
			}
			return sides;
		}

		private void OnDestroy()
		{
			AvatarBody.truce = false;
		}

		private static bool Listed(string id)
		{
			if (match == null)
			{
				return false;
			}
			return match.scores.Exists((Score s) => s.id == id);
		}

		private int Enlist()
		{
			int count = 0;
			foreach (NetworkPlayer a in NetworkPlayer.avatars)
			{
				if (!a || !a.Object || !a.Object.IsValid || !a.Watching || a.Uid.Length == 0 || Listed(a.Uid.Value))
				{
					continue;
				}
				Score score = Find(a.Uid.Value);
				if (match.teams >= 1)
				{
					score.team = (teamMemory.TryGetValue(a.Uid.Value, out int team) && team > 0) ? team : Smallest();
					score.wins = TeamScore(score.team);
				}
				Player.Note(Name(a.Uid.Value) + " was spectating, plays from round " + (match.round + 1));
				count++;
			}
			return count;
		}

		private bool Deserted()
		{
			if (match.scores.Count < 2 || Sides(false, out int _) > 1 || Time.time < boot.joinedAt + giveUpAfter)
			{
				return false;
			}
			if (match.teams < 1)
			{
				int watchers = 0;
				foreach (NetworkPlayer a in NetworkPlayer.avatars)
				{
					if (a && a.Object && a.Object.IsValid)
					{
						watchers += ((bool)a.Watching) ? 1 : 0;
					}
				}
				return boot.Players() - watchers < 2;
			}
			return boot.Players() <= NetworkPlayer.avatars.FindAll((NetworkPlayer p) => p && p.Object && p.Object.IsValid).Count;
		}

		private void Publish()
		{
			string text = Encode(match);
			if (boot.Property("match") != text)
			{
				boot.SetProperty("match", text);
			}
		}

		private bool Adopt()
		{
			if (!boot.localAvatar)
			{
				return false;
			}
			Hop hop = Decode(boot.Property("match") ?? "");
			if (hop == null || hop.round < 1)
			{
				return false;
			}
			match = hop;
			followed.Add(hop.id);
			hostId = hop.host;
			Player.Note("joined round " + hop.round + " on " + hop.scene + " during the match");
			if (!Listed(MyId))
			{
				Watch();
			}
			return true;
		}

		private void Watch()
		{
			watching = true;
			if (boot.localBody)
			{
				boot.localBody.Vanish();
			}
			if (boot.localBody)
			{
				boot.localBody.Spectate();
			}
			Player.Note("not in this match (round " + match.round + "), spectating until the next round");
		}

		private void Hold(bool on)
		{
			AvatarBody.truce = on;
			if (boot.localBody)
			{
				boot.localBody.frozen = on;
			}
		}

		private void PlayMusic()
		{
			if (!music || music.isPlaying)
			{
				return;
			}
			AudioClip clip = (Rounds && roundsMusic) ? roundsMusic : ((roundMusic != null && roundMusic.Length != 0) ? roundMusic[(match.round - 1) % roundMusic.Length] : null);
			if (clip)
			{
				music.clip = clip;
				music.Play();
			}
		}

		private void Declare(string id)
		{
			int team = 0;
			if (id.StartsWith(TeamWin))
			{
				team = int.TryParse(id.Substring(TeamWin.Length), out int parsed) ? parsed : 0;
			}
			winner = (id == "-" || id == Left || id == OutOfTime || id == Cleared || id == Fallen) ? "" : id;
			deserted = id == Left;
			over = id == Fallen;
			if (over)
			{
				podium.Clear();
			}
			if (clock)
			{
				clock.gameObject.SetActive(false);
			}
			live = false;
			goAt = -1f;
			winAt = Time.time;
			Tally();
			if (team >= 1)
			{
				foreach (Score s in match.scores)
				{
					if (s.team == team)
					{
						s.wins++;
					}
				}
			}
			else if (winner != "")
			{
				Find(winner).wins++;
			}
			string round = Round();
			int survived = match.round - 1;
			if (deserted)
			{
				Banner("EVERYONE LEFT", Color.white, Scores(true), "BACK TO THE LOBBY");
			}
			else if (over)
			{
				Banner("EVERYONE DIED!", ClockRed, "YOU SURVIVED " + survived + ((survived == 1) ? " ROUND" : " ROUNDS"), "BACK TO THE LOBBY");
			}
			else if (Rounds)
			{
				Banner("ROUND " + match.round + " CLEARED!", MapYellow, (id == OutOfTime) ? "TIME'S UP, YOU MADE IT" : "GET READY FOR THE NEXT ONE", round);
			}
			else if (id == OutOfTime)
			{
				Banner("TIME'S UP!", Color.white, Scores(true), round);
			}
			else if (winner == "")
			{
				Banner("NOBODY WINS!", Color.white, Scores(true), round);
			}
			else if (team < 1)
			{
				Banner(Name(winner) + " WINS!", Colour(winner), Scores(true), round);
			}
			else
			{
				Banner(TeamName(team) + " WINS!", TeamColour(team), Scores(true), round);
			}
			Player.Note("round " + match.round + (deserted ? " ended, everyone else left" : ((id == OutOfTime) ? " ran out of time, nobody wins" : (" won by " + ((winner == "") ? "nobody" : ((team <= 0) ? Name(winner) : TeamName(team)))))) + ", " + Scores(false));
		}

		private bool Decided()
		{
			if (Rounds)
			{
				return false;
			}
			return Leader() >= Wins;
		}

		private string Round()
		{
			if (Rounds)
			{
				return "ROUNDS   ROUND " + match.round;
			}
			return "ROUND " + match.round + "   FIRST TO " + Wins;
		}

		private int Leader()
		{
			int best = 0;
			foreach (Score s in match.scores)
			{
				best = Mathf.Max(best, s.wins);
			}
			return best;
		}

		private void Champion()
		{
			string head;
			Color color;
			if (match.teams < 1)
			{
				Score best = null;
				bool tie = false;
				foreach (Score s in match.scores)
				{
					if (best == null || s.wins > best.wins)
					{
						best = s;
						tie = false;
					}
					else
					{
						tie |= s.wins == best.wins;
					}
				}
				if (best != null && best.wins != 0 && !tie)
				{
					head = Name(best.id) + " WINS THE MATCH!";
					color = Colour(best.id);
				}
				else
				{
					head = "IT'S A TIE!";
					color = Color.white;
				}
			}
			else
			{
				List<KeyValuePair<int, int>> table = TeamTable(true);
				if (table.Count == 0 || table[0].Value == 0 || (table.Count >= 2 && table[0].Value == table[1].Value))
				{
					head = "IT'S A TIE!";
					color = Color.white;
				}
				else
				{
					head = TeamName(table[0].Key) + " WINS THE MATCH!";
					color = TeamColour(table[0].Key);
				}
			}
			Banner(head, color, Scores(true), "FINAL SCORE");
			Player.Note("match over, " + Scores(false));
			Podium();
		}

		private void Podium()
		{
			podium.Clear();
			podiumRound++;
			podiumLobby = match.lobby;
			List<Score> list = new List<Score>(match.scores);
			list.RemoveAll((Score s) => !Here(s.id));
			if (match.teams >= 1)
			{
				List<KeyValuePair<int, int>> table = TeamTable(true);
				if (table.Count >= 1 && (table.Count == 1 || table[1].Value < table[0].Value))
				{
					list.RemoveAll((Score s) => s.team != table[0].Key);
				}
			}
			list.Sort((Score a, Score b) => b.wins.CompareTo(a.wins));
			StringBuilder line = new StringBuilder();
			for (int i = 0; i < list.Count && podium.Count < 3; i++)
			{
				Score s = list[i];
				NetworkPlayer avatar = NetworkPlayer.avatars.Find((NetworkPlayer a) => a && a.Object && a.Object.IsValid && a.Uid.Value == s.id);
				string nick = avatar ? avatar.Nick.Value : "";
				Placing placing = new Placing();
				placing.name = (nick != "") ? nick : Name(s.id);
				placing.paint = (match.teams >= 1 && s.team > 0) ? TeamPaint(s.team) : s.paint;
				placing.wins = s.wins;
				if (avatar)
				{
					placing.outfit = new string[4]
					{
						avatar.Head.Value,
						avatar.Torso.Value,
						avatar.Legs.Value,
						avatar.Face.Value
					};
				}
				podium.Add(placing);
				line.Append(podium.Count).Append(". ").Append(placing.name)
					.Append(' ')
					.Append(placing.wins)
					.Append("  ");
			}
			Player.Note("podium: " + line.ToString());
		}

		public static bool PodiumHere()
		{
			NetworkManager net = NetworkManager.main;
			if (podium.Count < 1 || !net || !net.InRoom)
			{
				return false;
			}
			return net.session == podiumLobby;
		}

		private void NextHop()
		{
			hopSent = true;
			bool end = Decided() || deserted || over;
			foreach (Score s in match.scores)
			{
				if (s.team >= 1)
				{
					teamMemory[s.id] = s.team;
				}
			}
			int dropped = match.scores.RemoveAll((Score s) => !Here(s.id));
			if (dropped >= 1)
			{
				Player.Note(dropped + " player(s) left the match, dropped from the scores");
			}
			int expect = Count(out int _, out NetworkPlayer _);
			if (!end)
			{
				expect += Enlist();
			}
			bool split = true;
			if (match.teams >= 1 && match.scores.Count >= 2)
			{
				split = match.scores.Exists((Score s) => s.team != match.scores[0].team);
				if (!split && !end)
				{
					Player.Note("only one team is left in the match, back to the lobby");
				}
			}
			uint roll = Roll(match.id, match.round + 1);
			Hop hop = new Hop();
			hop.id = roll.ToString("x8");
			hop.lobby = match.lobby;
			hop.lobbyKind = match.lobbyKind;
			hop.host = MyId;
			hop.wins = match.wins;
			hop.size = match.size;
			hop.crowd = match.crowd || match.scores.Count > Crowd;
			hop.expect = expect;
			hop.maps = match.maps;
			hop.teams = match.teams;
			hop.scores = match.scores;
			if (end || !split)
			{
				hop.scene = lobbyScene;
				hop.round = 0;
				hop.session = match.lobby;
			}
			else
			{
				hop.round = match.round + 1;
				hop.scene = Pick(match.scene, match.maps, roll);
				hop.session = match.lobby + "-" + hop.id;
			}
			boot.Hop(Encode(hop));
		}

		public string FirstHop(string lobby, string lobbyKind, int wins, int size, int teams, string allowed)
		{
			Hop hop = new Hop();
			hop.id = NewId();
			hop.round = 1;
			hop.lobby = lobby;
			hop.lobbyKind = lobbyKind;
			hop.host = MyId;
			hop.wins = (wins > 0) ? wins : winsNeeded;
			hop.size = (size > 0) ? size : 8;
			hop.teams = (teams < 0) ? (-1) : teams;
			hop.maps = allowed ?? "";
			hop.scene = Pick(null, hop.maps, (uint)UnityEngine.Random.Range(0, int.MaxValue));
			hop.session = lobby + "-" + hop.id;
			foreach (NetworkPlayer a in NetworkPlayer.avatars)
			{
				if (a && a.Object && a.Object.IsValid && a.Uid.Length >= 1)
				{
					hop.scores.Add(new Score
					{
						id = a.Uid.Value,
						paint = a.Paint
					});
				}
			}
			if (hop.scores.Count == 0)
			{
				hop.scores.Add(new Score
				{
					id = MyId
				});
			}
			if (hop.teams >= 1)
			{
				Split(hop);
			}
			int count = (hop.expect = hop.scores.Count);
			int players = boot ? boot.Players() : 0;
			hop.crowd = Mathf.Max(count, players) > Crowd || TestCrowd();
			return Encode(hop);
		}

		private void Tally()
		{
			foreach (NetworkPlayer a in NetworkPlayer.avatars)
			{
				if (a && a.Object && a.Object.IsValid && a.Uid.Length >= 1 && !a.Watching)
				{
					Find(a.Uid.Value).paint = a.Paint;
				}
			}
		}

		private static void Split(Hop hop)
		{
			int teams = Mathf.Clamp(Mathf.CeilToInt((float)hop.scores.Count / (float)hop.teams), 2, TeamPaints.Length);
			List<Score> list = new List<Score>(hop.scores);
			for (int i = list.Count - 1; i >= 1; i--)
			{
				int j = UnityEngine.Random.Range(0, i + 1);
				Score swap = list[i];
				list[i] = list[j];
				list[j] = swap;
			}
			if (teamLobby != hop.lobby)
			{
				teamMemory.Clear();
				teamLobby = hop.lobby;
			}
			int cap = Mathf.CeilToInt((float)list.Count / (float)teams);
			int[] size = new int[teams + 1];
			foreach (Score s in list)
			{
				s.team = 0;
				if (teamMemory.TryGetValue(s.id, out int t) && t >= 1 && t <= teams && size[t] < cap)
				{
					s.team = t;
					size[t]++;
				}
			}
			foreach (Score s in list)
			{
				if (s.team <= 0)
				{
					int best = 1;
					for (int t = 2; t <= teams; t++)
					{
						if (size[t] < size[best])
						{
							best = t;
						}
					}
					s.team = best;
					size[best]++;
				}
			}
			foreach (Score s in list)
			{
				teamMemory[s.id] = s.team;
			}
		}

		private int Smallest()
		{
			int most = 0;
			foreach (Score s in match.scores)
			{
				most = Mathf.Max(most, s.team);
			}
			int best = 1;
			int fewest = int.MaxValue;
			int last = Mathf.Max(most, 2);
			for (int t = 1; t <= last; t++)
			{
				int here = 0;
				foreach (Score s in match.scores)
				{
					if (s.team == t)
					{
						here += Here(s.id) ? 1 : 0;
					}
				}
				if (here < fewest)
				{
					fewest = here;
					best = t;
				}
			}
			return best;
		}

		private int TeamScore(int team)
		{
			int best = 0;
			foreach (Score s in match.scores)
			{
				if (s.team == team)
				{
					best = Mathf.Max(best, s.wins);
				}
			}
			return best;
		}

		private List<KeyValuePair<int, int>> TeamTable(bool all = false)
		{
			List<KeyValuePair<int, int>> table = new List<KeyValuePair<int, int>>();
			foreach (Score s in match.scores)
			{
				if (s.team >= 1 && (all || Here(s.id)) && !table.Exists((KeyValuePair<int, int> e) => e.Key == s.team))
				{
					table.Add(new KeyValuePair<int, int>(s.team, TeamScore(s.team)));
				}
			}
			table.Sort((KeyValuePair<int, int> a, KeyValuePair<int, int> b) => b.Value.CompareTo(a.Value));
			return table;
		}

		public static int TeamOf(string id)
		{
			if (match == null || match.teams < 1 || string.IsNullOrEmpty(id))
			{
				return 0;
			}
			foreach (Score s in match.scores)
			{
				if (s.id == id)
				{
					return s.team;
				}
			}
			return 0;
		}

		public static bool SameTeam(string a, string b)
		{
			if (Rounds)
			{
				return true;
			}
			int team = TeamOf(a);
			if (team < 1)
			{
				return false;
			}
			return team == TeamOf(b);
		}

		public static int TeamPaint(int team)
		{
			if (team < 1)
			{
				return 0;
			}
			return TeamPaints[(team - 1) % TeamPaints.Length];
		}

		public static string TeamName(int team)
		{
			return ColorName(TeamPaint(team)) + " TEAM";
		}

		private Color TeamColour(int team)
		{
			if (boot && boot.networkPlayerPrefab)
			{
				return boot.networkPlayerPrefab.Colour(TeamPaint(team));
			}
			return Color.white;
		}

		private static string MyTeam()
		{
			int team = TeamOf(MyId);
			if (Rounds)
			{
				return "SURVIVE THE ROUND";
			}
			if (team > 0)
			{
				return "YOU'RE ON " + TeamName(team);
			}
			return "";
		}

		private Score Find(string id)
		{
			foreach (Score s in match.scores)
			{
				if (s.id == id)
				{
					return s;
				}
			}
			Score score = new Score
			{
				id = id
			};
			match.scores.Add(score);
			return score;
		}

		public static int PaintOf(string id)
		{
			if (match == null)
			{
				return 0;
			}
			foreach (Score s in match.scores)
			{
				if (s.id == id)
				{
					return s.paint;
				}
			}
			return 0;
		}

		public static bool Claimed(int paint)
		{
			if (match == null)
			{
				return false;
			}
			foreach (Score s in match.scores)
			{
				if (s.id != MyId && s.paint == paint)
				{
					return true;
				}
			}
			return false;
		}

		private string Name(string id)
		{
			return ColorName(PaintOf(id));
		}

		public static string ColorName(int paint)
		{
			if (paint < 1 || !main || main.colorNames.Length == 0)
			{
				return "PLAYER";
			}
			return main.colorNames[(paint - 1) % main.colorNames.Length];
		}

		private Color Colour(string id)
		{
			if (boot && boot.networkPlayerPrefab)
			{
				return boot.networkPlayerPrefab.Colour(PaintOf(id));
			}
			return Color.white;
		}

		private string Scores(bool wrap = false)
		{
			StringBuilder text;
			if (match.teams >= 1)
			{
				List<KeyValuePair<int, int>> table = TeamTable();
				text = new StringBuilder();
				for (int i = 0; i < table.Count; i++)
				{
					if (i != 0)
					{
						text.Append("   ");
					}
					text.Append(ColorName(TeamPaint(table[i].Key))).Append(' ').Append(table[i].Value);
				}
				return text.ToString();
			}
			List<Score> list = new List<Score>(match.scores);
			list.Sort((Score a, Score b) => b.wins.CompareTo(a.wins));
			text = new StringBuilder();
			list.RemoveAll((Score s) => !Here(s.id));
			for (int i = 0; i < list.Count; i++)
			{
				if (i != 0)
				{
					text.Append((i % 4 == 0 && wrap) ? "\n" : "   ");
				}
				text.Append(Name(list[i].id)).Append(' ').Append(list[i].wins);
			}
			return text.ToString();
		}

		private static string Who()
		{
			StringBuilder text = new StringBuilder();
			foreach (NetworkPlayer a in NetworkPlayer.avatars)
			{
				if (!a || !a.Object || !a.Object.IsValid)
				{
					continue;
				}
				if (text.Length >= 1)
				{
					text.Append(", ");
				}
				int team = (match != null && match.teams >= 1 && a.Uid.Length > 0) ? TeamOf(a.Uid.Value) : 0;
				text.Append("Player:").Append(a.Object.StateAuthority.PlayerId).Append(' ')
					.Append(ColorName((team > 0) ? TeamPaint(team) : a.Paint))
					.Append((team > 0) ? (" team " + team) : "")
					.Append(a.HasStateAuthority ? " (me)" : "");
			}
			return text.ToString();
		}

		private bool Here(string id)
		{
			if (id == MyId)
			{
				return true;
			}
			foreach (NetworkPlayer a in NetworkPlayer.avatars)
			{
				if (a && a.Object && a.Object.IsValid && a.Uid.Value == id && boot.Present(a.Object.StateAuthority))
				{
					return true;
				}
			}
			return false;
		}

		private string Pick(string avoid, string allowed, uint roll)
		{
			List<Map> pool = maps.FindAll((Map m) => Allowed(allowed, m.scene));
			if (pool.Count == 0)
			{
				pool = new List<Map>(maps);
			}
			if (pool.Count >= 2)
			{
				pool.RemoveAll((Map m) => m.scene == avoid);
			}
			if (pool.Count == 0)
			{
				return lobbyScene;
			}
			return pool[(int)(roll % (uint)pool.Count)].scene;
		}

		private static uint Roll(string id, int round)
		{
			string key = id + "/" + round;
			uint hash = 2166136261u;
			foreach (char c in key)
			{
				hash = (hash ^ c) * 16777619;
			}
			return hash & 0x7FFFFFFF;
		}

		public static bool Allowed(string allowed, string scene)
		{
			if (string.IsNullOrEmpty(allowed))
			{
				return true;
			}
			return Array.IndexOf(allowed.Split(';'), scene) >= 0;
		}

		private string Title(string scene)
		{
			foreach (Map map in maps)
			{
				if (map.scene == scene)
				{
					return map.title;
				}
			}
			return scene.ToUpper();
		}

		private static string NewId()
		{
			return UnityEngine.Random.Range(0, int.MaxValue).ToString("x8");
		}

		public static string Encode(Hop hop)
		{
			StringBuilder text = new StringBuilder();
			text.Append(hop.id).Append('|').Append(hop.session)
				.Append('|')
				.Append(hop.scene)
				.Append('|')
				.Append(hop.round)
				.Append('|')
				.Append(hop.lobbyKind)
				.Append('|')
				.Append(hop.lobby)
				.Append('|');
			for (int i = 0; i < hop.scores.Count; i++)
			{
				if (i != 0)
				{
					text.Append(',');
				}
				text.Append(hop.scores[i].id).Append(':').Append(hop.scores[i].paint)
					.Append(':')
					.Append(hop.scores[i].wins)
					.Append(':')
					.Append(hop.scores[i].team);
			}
			text.Append('|').Append(hop.host).Append('|')
				.Append(hop.wins)
				.Append('|')
				.Append(hop.size)
				.Append('|')
				.Append(hop.crowd ? 1 : 0)
				.Append('|')
				.Append(hop.expect)
				.Append('|')
				.Append(hop.teams)
				.Append('|')
				.Append(hop.maps);
			return text.ToString();
		}

		public static Hop Decode(string text)
		{
			string[] parts = text.Split('|');
			if (parts.Length != 14 || !int.TryParse(parts[3], out int round) || !int.TryParse(parts[8], out int wins) || !int.TryParse(parts[9], out int size) || !int.TryParse(parts[11], out int expect) || !int.TryParse(parts[12], out int teams))
			{
				return null;
			}
			Hop hop = new Hop();
			hop.id = parts[0];
			hop.session = parts[1];
			hop.scene = parts[2];
			hop.round = round;
			hop.lobbyKind = parts[4];
			hop.lobby = parts[5];
			hop.host = parts[7];
			hop.wins = wins;
			hop.size = size;
			hop.crowd = parts[10] == "1";
			hop.expect = expect;
			hop.teams = teams;
			hop.maps = parts[13];
			string[] entries = parts[6].Split(',');
			foreach (string entry in entries)
			{
				string[] fields = entry.Split(':');
				if (fields.Length == 4 && int.TryParse(fields[1], out int paint) && int.TryParse(fields[2], out int won) && int.TryParse(fields[3], out int team))
				{
					hop.scores.Add(new Score
					{
						id = fields[0],
						paint = paint,
						wins = won,
						team = team
					});
				}
			}
			return hop;
		}
	}
}
