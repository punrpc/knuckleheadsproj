using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Client;
using Photon.Realtime;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace Ragdoll
{
	public class NetworkManager : MonoBehaviour
	{
		public class Plan
		{
			public string session;

			public string kind = "dev";

			public bool visible = true;

			public int players;

			public bool mustExist;

			public bool drop;

			public string code;

			public string state = "lobby";

			public string title = "";

			public int wins;

			public int teams;

			public string maps = "";

			public int cap;
		}

		public class Room
		{
			public string session;

			public string code;

			public string title;

			public int players;

			public int max;

			public bool playing;

			public string match = "";

			public int mode;
		}

		[Serializable]
		private class AuthenticationRequest
		{
			public string UserId;

			public string OculusId;

			public string Nonce;

			public string IntegToken;
		}

		[Serializable]
		private class AuthenticationResponse
		{
			public bool Success;

			public string UserId;

			public string Ticket;

			public long ExpiresAt;
		}

		[Header("Session")]
		public string session = "dev";

		public bool connectOnPlay = true;

		[Header("Players")]
		[FormerlySerializedAs("avatarPrefab")]
		public NetworkPlayer networkPlayerPrefab;

		public Player localBody;

		[Header("Spawn")]
		public Transform[] spawnPoints;

		public float spawnRing = 1.2f;

		public int spawnSlots = 8;

		[Header("Drop Goo Gotch")]
		public Transform dropZone;

		public TMP_Text connectingText;

		public float dropHeight = 25f;

		public const int Protocol = 36;

		public const string Lost = "CONNECTION LOST";

		public const string AuthFailed = "AUTHENTICATION FAILED\n<size=70%>TRY UPDATING YOUR HEADSET</size>";

		private const float SendWait = 3f;

		public const int MaxRoom = 8;

		public static Plan next;

		[HideInInspector]
		public NetworkRunner runner;

		[HideInInspector]
		public Recorder mic;

		[HideInInspector]
		public NetworkPlayer localAvatar;

		[HideInInspector]
		public string status = "off";

		[HideInInspector]
		public float joinedAt = -1f;

		[HideInInspector]
		public int spawnTries;

		[HideInInspector]
		public string kind = "";

		[HideInInspector]
		public string code = "";

		[HideInInspector]
		public string error = "";

		[HideInInspector]
		public bool listed;

		[HideInInspector]
		public int roomsSeen;

		[HideInInspector]
		public bool quickWanted;

		public readonly List<Room> rooms = new List<Room>();

		private bool leaving;

		private bool gutted;

		private FusionVoiceClient voice;

		private int voiceTries;

		private float browseAfter;

		private const float BrowseRetry = 5f;

		private const int RejoinWait = 2000;

		private float voiceRetryAt = -1f;

		private bool voiceSigning;

		private short voiceRefused;

		private const int VoiceTries = 5;

		private float leaveAt = -1f;

		private string leaveTo;

		private bool veiled;

		private bool dropReady;

		private bool holdBody;

		private Vector3 holdAt;

		private float holdFloor;

		private Rigidbody holdOn;

		private Collider[] heldMine;

		private Collider[] heldFloor;

		private bool placed;

		private float holdOnY;

		private bool spawnWait;

		private float spawnWaitFrom;

		private int missingAtJoin;

		private float arriveWait = 3f;

		private bool showText;

		private string seenGo;

		private float clockOffset;

		private NetworkRunner browser;

		private bool browsing;

		private string titleSent;

		private static string metaName = "";

		private static string carried = "";

		private PlayerRef handedTo = PlayerRef.None;

		private bool authenticating;

		private static readonly SemaphoreSlim authLock = new SemaphoreSlim(1, 1);

		private const string AuthBaseUrl = "https://dre1mer.org";

		private const string PcFusion = "a06a7807-0143-4472-9db5-20caacde203c";

		private const string PcVoice = "d788a71e-3fd2-434e-b3f8-b84a8859a0b3";

		private static string ticketUserId;

		private static string photonTicket;

		private static float ticketRefreshAt;

		public static NetworkManager main;

		public const string Stuck = "LOADING TOOK TOO LONG";

		private const float LoadStuck = 45f;

		private float stuckSince = -1f;

		private bool travelled;

		private readonly List<KeyValuePair<string, string>> wrote = new List<KeyValuePair<string, string>>();

		private readonly Dictionary<string, string> seen = new Dictionary<string, string>();

		private readonly HashSet<string> flying = new HashSet<string>();

		private string refused;

		public static readonly string[] RegionCodes = new string[8] { "cae", "us", "usw", "sa", "eu", "asia", "jp", "au" };

		public static readonly string[] RegionNames = new string[8] { "CANADA EAST", "US EAST", "US WEST", "S. AMERICA", "EUROPE", "ASIA", "JAPAN", "AUSTRALIA" };

		private bool wasHost;

		private readonly List<NetworkPlayer> hostedWith = new List<NetworkPlayer>();

		private static AsyncOperation sceneLoad;

		public static string MetaName => metaName;

		public static float Clock => Time.time + (main ? main.clockOffset : 0f);

		public bool Busy => status == "connecting" || quickWanted;

		public bool Connecting => status == "connecting";

		public bool Travelling => leaving;

		public bool InRoom => (bool)runner && runner.IsRunning && joinedAt >= 0f;

		public bool IsHost => InRoom && runner.IsSharedModeMasterClient;

		public static int RegionIndex => Mathf.Max(Array.IndexOf(RegionCodes, Prefs.region), 0);

		public static string RegionName => RegionNames[RegionIndex];

		public bool Open => InRoom && runner.SessionInfo.IsValid && runner.SessionInfo.IsVisible;

		private static async Task<AuthenticationValues> GetAuthentication()
		{
			await authLock.WaitAsync();
			try
			{
				if (!string.IsNullOrEmpty(photonTicket) && Time.realtimeSinceStartup < ticketRefreshAt)
				{
					return AuthenticateWithPhoton();
				}
				ResetAuthentication();
				if (!Core.IsInitialized())
				{
					Request<PlatformInitialize> init = Core.AsyncInitialize();
					EnsureMetaCallbacks();
					await AwaitMeta(init, "meta init");
				}
				EnsureMetaCallbacks();
				await AwaitMeta(Entitlements.IsUserEntitledToApplication(), "entitlement check");
				User user = await AwaitMeta(Users.GetLoggedInUser(), "get meta user");
				if (user == null || user.ID == 0)
				{
					throw new InvalidOperationException("empty id?");
				}
				metaName = ShortName(user.DisplayName);
				if (metaName == "")
				{
					metaName = ShortName(user.OculusID);
				}
				OrgScopedID org = await AwaitMeta(Users.GetOrgScopedID(user.ID), "get org scope");
				if (org == null || org.ID == 0)
				{
					throw new InvalidOperationException("empty scope?");
				}
				Debug.Log("getting nonce.");
				UserProof proof = await AwaitMeta(Users.GetUserProof(), "get nonce");
				if (proof == null || string.IsNullOrEmpty(proof.Value))
				{
					throw new InvalidOperationException("empty nonce?");
				}
				Debug.Log("getting token.");
				string token = await AwaitMeta(DeviceApplicationIntegrity.GetIntegrityToken(proof.Value), "get token");
				if (string.IsNullOrEmpty(token))
				{
					throw new InvalidOperationException("empty token?");
				}
				await AuthenticateWithServer(org.ID.ToString(), user.ID.ToString(), proof.Value, token);
				return AuthenticateWithPhoton();
			}
			finally
			{
				authLock.Release();
			}
		}

		private static async Task AuthenticateWithServer(string userid, string oculusid, string nonce, string integrity)
		{
			Debug.Log("authing with server.");
			AuthenticationRequest body = new AuthenticationRequest
			{
				UserId = userid,
				OculusId = oculusid,
				Nonce = nonce,
				IntegToken = integrity
			};
			using (UnityWebRequest request = new UnityWebRequest(AuthBaseUrl + "/knuckle/auth/bootstrap", "POST"))
			{
				request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(body)));
				request.downloadHandler = new DownloadHandlerBuffer();
				request.SetRequestHeader("Content-Type", "application/json");
				request.timeout = 35;
				UnityWebRequestAsyncOperation operation = request.SendWebRequest();
				while (!operation.isDone)
				{
					await Task.Yield();
				}
				if (request.result != UnityWebRequest.Result.Success)
				{
					throw new InvalidOperationException(request.responseCode + " " + request.error);
				}
				AuthenticationResponse response = JsonUtility.FromJson<AuthenticationResponse>(request.downloadHandler.text);
				if (response == null || !response.Success || response.UserId != userid || string.IsNullOrWhiteSpace(response.Ticket) || response.ExpiresAt <= 0)
				{
					throw new InvalidOperationException("bootstrap returned invalid authentication.");
				}
				long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				if (DateTimeOffset.TryParse(request.GetResponseHeader("Date"), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset date))
				{
					now = date.ToUnixTimeSeconds();
				}
				ticketUserId = response.UserId;
				photonTicket = response.Ticket;
				ticketRefreshAt = Time.realtimeSinceStartup + Mathf.Max(response.ExpiresAt - now - 60, 0f);
			}
		}

		private static AuthenticationValues AuthenticateWithPhoton()
		{
			Debug.Log("authing with photon.");
			Dictionary<string, object> data = new Dictionary<string, object>(2)
			{
				["UserId"] = ticketUserId,
				["Ticket"] = photonTicket
			};
			AuthenticationValues auth = new AuthenticationValues
			{
				UserId = ticketUserId,
				AuthType = CustomAuthenticationType.Custom
			};
			auth.SetAuthPostData(data);
			return auth;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetAuthentication()
		{
			ticketUserId = null;
			photonTicket = null;
			ticketRefreshAt = 0f;
		}

		private static void CheckAuthentication(ShutdownReason reason)
		{
			if (reason == ShutdownReason.InvalidAuthentication || reason == ShutdownReason.CustomAuthenticationFailed || reason == ShutdownReason.AuthenticationTicketExpired)
			{
				ResetAuthentication();
			}
		}

		private static async Task<AuthenticationValues> SignIn()
		{
			// AUTH DISABLED (temporary): the shipped flow below checks the Meta entitlement and
			// trades a Meta user proof + integrity token for a ticket from the original game's
			// backend, then uses Photon custom auth. Until our own backend exists, connect to
			// Photon anonymously. See Docs/BACKEND_AUTH.md (in the Unity project folder) for what is needed to turn it back on.
			// return await GetAuthentication();
			AssignRandomName();
			await Task.CompletedTask;
			return new AuthenticationValues { UserId = GameManager.MyId };
		}

		// No Meta lookup while backend auth is off: every launch gets a random
		// "PLAYER####" name, used for the nickname and the "NAME'S ROOM" title.
		private static void AssignRandomName()
		{
			if (metaName == "")
			{
				metaName = "PLAYER" + UnityEngine.Random.Range(1000, 10000);
			}
		}

		private async Task<AuthenticationValues> SignInRetry()
		{
			for (int tries = 1; ; tries++)
			{
				try
				{
					return await SignIn();
				}
				catch (Exception e)
				{
					if (tries > 2 || !this || leaving)
					{
						throw;
					}
					Debug.LogWarning("NetworkManager sign-in try " + tries + " failed: " + e.Message);
				}
				await Task.Delay(tries * 2000);
			}
		}

		private static void EnsureMetaCallbacks()
		{
			CallbackRunner callbacks = UnityEngine.Object.FindObjectOfType<CallbackRunner>();
			if (!callbacks)
			{
				GameObject go = new GameObject("Oculus.Platform.CallbackRunner");
				UnityEngine.Object.DontDestroyOnLoad(go);
				go.AddComponent<CallbackRunner>();
			}
			else if (!callbacks.enabled)
			{
				callbacks.enabled = true;
			}
		}

		private static async Task<T> AwaitMeta<T>(Request<T> request, string step)
		{
			TaskCompletionSource<T> completion = new TaskCompletionSource<T>();
			request.OnComplete(delegate(Message<T> message)
			{
				if (message.IsError)
				{
					completion.TrySetException(new InvalidOperationException(step + " " + message.GetError().Message));
				}
				else
				{
					completion.TrySetResult(message.Data);
				}
			});
			Task done = await Task.WhenAny(completion.Task, Task.Delay(20000));
			if (done != completion.Task)
			{
				throw new TimeoutException(step + " timed out.");
			}
			return await completion.Task;
		}

		private static async Task AwaitMeta(Request request, string step)
		{
			TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();
			request.OnComplete(delegate(Message message)
			{
				if (message.IsError)
				{
					completion.TrySetException(new InvalidOperationException(step + " " + message.GetError().Message));
				}
				else
				{
					completion.TrySetResult(true);
				}
			});
			Task done = await Task.WhenAny(completion.Task, Task.Delay(20000));
			if (done != completion.Task)
			{
				throw new TimeoutException(step + " timed out.");
			}
			await completion.Task;
		}

		private void Awake()
		{
			main = this;
		}

		private async void Start()
		{
			string env = Environment.GetEnvironmentVariable("PCGAME_SESSION");
			if (!string.IsNullOrEmpty(env))
			{
				session = env;
			}
			error = carried;
			carried = "";
			Plan plan = next;
			next = null;
			if (plan == null && !connectOnPlay && string.IsNullOrEmpty(env))
			{
				return;
			}
			string delay = Environment.GetEnvironmentVariable("PCGAME_JOIN_DELAY");
			if (!string.IsNullOrEmpty(delay))
			{
				await Task.Delay((int)(float.Parse(delay) * 1000f));
			}
			await Connect(plan ?? new Plan
			{
				session = session
			});
		}

		public async Task<bool> Connect(Plan plan)
		{
			if ((bool)runner || leaving || authenticating)
			{
				return false;
			}
			Prefs.Load();
			if (MenuTV.TutorialOn && !Prefs.tutorialDone && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PCGAME_SESSION")))
			{
				error = "FINISH THE TUTORIAL FIRST";
				Player.Note("connect refused: the tutorial is not finished");
				return false;
			}
			status = "connecting";
			authenticating = true;
			error = "";
			AuthenticationValues auth;
			try
			{
				await StopBrowsing();
				auth = await SignInRetry();
			}
			finally
			{
				authenticating = false;
			}
			if (!this || leaving)
			{
				return false;
			}
			kind = plan.kind;
			if (plan.code != null)
			{
				code = plan.code;
			}
			if (plan.drop)
			{
				Veil(on: true);
			}
			GameObject go = new GameObject("Runner");
			go.transform.SetParent(transform, worldPositionStays: false);
			runner = go.AddComponent<NetworkRunner>();
			runner.ProvideInput = false;
			mic = go.AddComponent<Recorder>();
			mic.RecordWhenJoined = false;
			mic.RecordingEnabled = false;
			mic.TransmitEnabled = false;
			mic.VoiceDetection = true;
			mic.VoiceDetectionThreshold = 0.02f;
			mic.MicrophoneType = Recorder.MicType.Photon;
			mic.SetAndroidNativeMicrophoneSettings(aec: true, agc: true, ns: true);
			voice = go.AddComponent<VoiceFollow>();
			voice.PrimaryRecorder = mic;
			voice.AutoConnectAndJoin = auth != null;
			voice.Client.StateChanged += VoiceChanged;
			voice.Client.OpResponseReceived += VoiceRefused;
			voice.SpeakerLinked += VoiceLinked;
			voiceTries = 0;
			voiceRefused = 0;
			voiceRetryAt = -1f;
			int objects = SceneObjects();
			StartGameArgs args = new StartGameArgs
			{
				GameMode = GameMode.Shared,
				AuthValues = auth,
				SessionName = plan.session,
				IsVisible = plan.visible,
				SessionProperties = new Dictionary<string, SessionProperty>
				{
					{ "v", Protocol },
					{ "go", "" },
					{ "win", "" },
					{ "start", "" },
					{ "match", "" },
					{ "state", plan.state },
					{ "title", plan.title ?? "" },
					{ "wins", (plan.wins > 0) ? plan.wins : 3 },
					{ "rules", plan.teams + "|" + plan.maps + "|" + ((plan.cap > 0) ? plan.cap : ((plan.players > 0) ? plan.players : MaxRoom)) },
					{ "nos", objects }
				},
				SceneManager = go.AddComponent<NetworkSceneManagerDefault>()
			};
			args.PlayerCount = (plan.players > 0) ? Mathf.Min(plan.players, MaxRoom) : MaxRoom;
			args.CustomPhotonAppSettings = App();
			UnityEngine.SceneManagement.Scene active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			if (active.buildIndex >= 0)
			{
				args.Scene = SceneRef.FromIndex(active.buildIndex);
			}
			Hold(on: true);
			StartGameResult result = await runner.StartGame(args);
			if (!result.Ok)
			{
				CheckAuthentication(result.ShutdownReason);
				status = "failed: " + result.ShutdownReason + " " + result.ErrorMessage;
				error = (result.ShutdownReason == ShutdownReason.MaxCcuReached) ? "SERVERS FULL" : ((result.ShutdownReason == ShutdownReason.CustomAuthenticationFailed || result.ShutdownReason == ShutdownReason.InvalidAuthentication) ? "SIGN-IN REJECTED" : "COULD NOT CONNECT");
				Debug.LogWarning("NetworkManager " + status);
				Drop();
				Veil(on: false);
				return false;
			}
			SessionInfo info = runner.SessionInfo;
			int players = info.PlayerCount;
			bool sameGame = info.Properties != null && info.Properties.TryGetValue("v", out SessionProperty version) && (int)version == Protocol;
			if ((plan.mustExist && players < 2) || !sameGame)
			{
				error = sameGame ? "ROOM NOT FOUND" : "DIFFERENT GAME VERSION";
				status = "off";
				Debug.Log("NetworkManager left '" + info.Name + "': " + error);
				await runner.Shutdown(destroyGameObject: false);
				Drop();
				Veil(on: false);
				return false;
			}
			int cap = Cap(info.Properties);
			if (players >= 2 && cap >= 1 && info.PlayerCount > cap)
			{
				error = "THAT ROOM IS FULL";
				status = "off";
				Debug.Log("NetworkManager left '" + info.Name + "': " + info.PlayerCount + " players, the room takes " + cap);
				await runner.Shutdown(destroyGameObject: false);
				Drop();
				Veil(on: false);
				return false;
			}
			if (info.Properties.TryGetValue("nos", out SessionProperty hostObjects) && (int)hostObjects != objects)
			{
				Player.Note("this map's network objects differ from the host's (mine " + objects + ", host " + (int)hostObjects + "), objects may swap: everyone needs the same build");
				Debug.LogError("NetworkManager scene objects differ from the session's: mine " + objects + ", session " + (int)hostObjects);
			}
			joinedAt = Time.time;
			session = info.Name;
			if (kind == "public" || kind == "private")
			{
				kind = info.IsVisible ? "public" : "private";
			}
			if (session.StartsWith("kh-") && session.Length >= 9)
			{
				code = session.Substring(3, 6);
			}
			status = "in '" + session + "' " + info.Region + " players " + info.PlayerCount;
			Debug.Log("NetworkManager " + status);
			if (!plan.drop)
			{
				spawnWait = true;
				spawnWaitFrom = Time.time;
				missingAtJoin = Missing();
				return true;
			}
			Hold(on: false);
			dropReady = true;
			return true;
		}

		private void Drop()
		{
			float was = joinedAt;
			wrote.Clear();
			seen.Clear();
			flying.Clear();
			if ((bool)voice)
			{
				voice.Client.StateChanged -= VoiceChanged;
				voice.Client.OpResponseReceived -= VoiceRefused;
			}
			if ((bool)runner)
			{
				UnityEngine.Object.Destroy(runner.gameObject);
			}
			runner = null;
			mic = null;
			voice = null;
			joinedAt = -1f;
			dropReady = false;
			Ignore(on: false);
			spawnWait = false;
			titleSent = null;
			handedTo = PlayerRef.None;
			if (!status.StartsWith("failed"))
			{
				status = "off";
			}
			gutted |= was >= 0f;
			if (was >= 0f && GameManager.match == null)
			{
				Player.Note("the session in '" + session + "' ended and took this scene's network objects, reloading " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
				GoTo(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, error);
			}
		}

		public async void CreateRoom(bool open, int size = 8, int wins = 3, int teams = 0, string maps = "")
		{
			if ((bool)runner || Busy)
			{
				return;
			}
			GameManager.hostId = "";
			for (int tries = 0; tries < 3; tries++)
			{
				code = UnityEngine.Random.Range(0, 1000000).ToString("D6");
				Plan plan = new Plan
				{
					session = "kh-" + code,
					kind = open ? "public" : "private",
					visible = open,
					players = MaxRoom,
					drop = true,
					wins = wins,
					teams = teams,
					maps = maps,
					cap = size
				};
				if (!(await Connect(plan)) || runner.SessionInfo.PlayerCount <= 1)
				{
					return;
				}
				await runner.Shutdown(destroyGameObject: false);
				Drop();
			}
			error = "COULD NOT MAKE A ROOM";
			Veil(on: false);
		}

		private void Veil(bool on)
		{
			veiled = on;
			if (on)
			{
				showText = true;
			}
			if ((bool)localBody)
			{
				localBody.blackout = on ? 1f : 0f;
			}
		}

		private void ShowConnecting()
		{
			if (!connectingText)
			{
				return;
			}
			float alpha = localBody ? localBody.dark : (veiled ? 1f : 0f);
			if (!veiled && alpha < 0.01f)
			{
				showText = false;
			}
			bool show = showText && !leaving && (!GameManager.main || !GameManager.main.Showing);
			if (show != connectingText.gameObject.activeSelf)
			{
				connectingText.gameObject.SetActive(show);
			}
			if (show)
			{
				connectingText.alpha = alpha;
			}
		}

		public async void JoinCode(string typed)
		{
			if ((bool)runner || Busy || typed.Length != 6)
			{
				return;
			}
			Room room = rooms.Find((Room r) => r.code == typed);
			if (room != null && room.playing)
			{
				error = "THAT ROOM IS IN A MATCH";
				return;
			}
			GameManager.hostId = "";
			code = typed;
			await Connect(new Plan
			{
				session = "kh-" + typed,
				kind = "private",
				visible = false,
				mustExist = true,
				drop = true
			});
		}

		public async void JoinRoom(Room room)
		{
			if ((bool)runner || Busy || room == null)
			{
				return;
			}
			if (room.players >= room.max)
			{
				error = "THAT ROOM IS FULL";
				return;
			}
			GameManager.hostId = "";
			code = room.code;
			if (!room.playing)
			{
				await Connect(new Plan
				{
					session = room.session,
					kind = "public",
					visible = true,
					mustExist = true,
					drop = true
				});
				return;
			}
			if (GameManager.Decode(room.match) == null)
			{
				error = "THAT ROOM IS IN A MATCH";
				return;
			}
			Player.Note("joining " + room.title + " in the middle of a match");
			await StopBrowsing();
			Follow(room.match, mustExist: true);
		}

		public void QuickJoin()
		{
			if ((bool)runner || Busy)
			{
				return;
			}
			if (!browser || listed)
			{
				Quick();
			}
			else
			{
				quickWanted = true;
			}
		}

		private async void Quick()
		{
			quickWanted = false;
			Room room = rooms.Find((Room r) => !r.playing && r.players < r.max);
			if (room == null)
			{
				CreateRoom(open: true);
				return;
			}
			GameManager.hostId = "";
			code = room.code;
			Player.Note("quick join picked " + room.title + " (" + room.players + "/" + room.max + ")");
			await Connect(new Plan
			{
				session = room.session,
				kind = "public",
				visible = true,
				drop = true
			});
		}

		private static FusionAppSettings App()
		{
			FusionAppSettings settings = PhotonAppSettings.Global.AppSettings.GetCopy();
			settings.FixedRegion = RegionCodes[RegionIndex];
			return settings;
		}

		public void ChangeRegion(int step)
		{
			if (leaving)
			{
				return;
			}
			int count = RegionCodes.Length;
			Prefs.region = RegionCodes[((RegionIndex + step) % count + count) % count];
			Prefs.Save();
			Player.Note("region set to " + RegionName + " (" + Prefs.region + ")");
			rooms.Clear();
			if ((bool)browser)
			{
				_ = StopBrowsing();
			}
			if ((bool)runner && !leaving)
			{
				Leave();
			}
		}

		public void SetOpen(bool open)
		{
			if (IsHost && runner.SessionInfo.IsValid)
			{
				runner.SessionInfo.IsVisible = open;
				kind = open ? "public" : "private";
				Player.Note("room is now " + kind);
			}
		}

		public void Browse(bool on)
		{
			if (browsing && !browser)
			{
				browsing = false;
			}
			if (browsing == on)
			{
				return;
			}
			if (!on)
			{
				browsing = false;
				_ = StopBrowsing();
			}
			else if (!(Time.time < browseAfter))
			{
				browsing = true;
				StartBrowsing();
			}
		}

		private async void StartBrowsing()
		{
			if ((bool)browser || (bool)runner)
			{
				browsing = false;
				return;
			}
			GameObject go = new GameObject("Browser");
			go.transform.SetParent(transform, worldPositionStays: false);
			NetworkRunner made = go.AddComponent<NetworkRunner>();
			NetworkEvents events = go.AddComponent<NetworkEvents>();
			if (events.OnSessionListUpdate == null)
			{
				events.OnSessionListUpdate = new NetworkEvents.SessionListUpdateEvent();
			}
			events.OnSessionListUpdate.AddListener(Listed);
			made.AddCallbacks(events);
			browser = made;
			listed = false;
			AuthenticationValues auth = await SignIn();
			if (!this || browser != made || !browsing)
			{
				return;
			}
			StartGameResult result = await made.JoinSessionLobby(SessionLobby.Shared, null, auth, App());
			if (browser != made)
			{
				return;
			}
			if (result.Ok)
			{
				if (error == AuthFailed)
				{
					error = "";
				}
				return;
			}
			CheckAuthentication(result.ShutdownReason);
			Debug.LogWarning("NetworkManager could not list rooms: " + result.ShutdownReason + " " + result.ErrorMessage);
			browser = null;
			browsing = false;
			browseAfter = Time.time + BrowseRetry;
			if ((bool)made)
			{
				UnityEngine.Object.Destroy(made.gameObject);
			}
			if (quickWanted)
			{
				Quick();
			}
		}

		private async Task StopBrowsing()
		{
			NetworkRunner was = browser;
			browsing = false;
			quickWanted = false;
			browser = null;
			listed = false;
			if ((bool)was)
			{
				await was.Shutdown();
				await Task.Yield();
			}
		}

		private void Listed(NetworkRunner from, List<SessionInfo> list)
		{
			if (from != browser)
			{
				return;
			}
			rooms.Clear();
			foreach (SessionInfo info in list)
			{
				if (info == null || !info.IsValid || !info.IsVisible || info.Name == null || !info.Name.StartsWith("kh-") || info.Name.Length < 9 || info.Properties == null || !info.Properties.TryGetValue("v", out SessionProperty version) || !version.IsInt || (int)version != Protocol)
				{
					continue;
				}
				int cap = Cap(info.Properties);
				Room r = new Room
				{
					session = info.Name,
					code = info.Name.Substring(3, 6),
					players = info.PlayerCount,
					max = (cap > 0) ? Mathf.Min(cap, info.MaxPlayers) : info.MaxPlayers
				};
				r.playing = info.Properties.TryGetValue("state", out SessionProperty state) && state.IsString && (string)state == "playing";
				r.match = (info.Properties.TryGetValue("match", out SessionProperty match) && match.IsString) ? ((string)match) : "";
				r.title = (info.Properties.TryGetValue("title", out SessionProperty title) && title.IsString && (string)title != "") ? ((string)title) : ("ROOM " + r.code);
				r.mode = (info.Properties.TryGetValue("rules", out SessionProperty rules) && rules.IsString && int.TryParse(Rules((string)rules)[0], out int mode)) ? mode : 0;
				if (r.playing && r.mode < 0)
				{
					continue;
				}
				int index = rooms.FindIndex((Room o) => o.code == r.code);
				if (index < 0)
				{
					rooms.Add(r);
				}
				else if (rooms[index].playing && (!r.playing || Round(r.match) > Round(rooms[index].match)))
				{
					rooms[index] = r;
				}
			}
			rooms.Sort((Room a, Room b) => Rank(b).CompareTo(Rank(a)));
			listed = true;
			roomsSeen++;
			if (quickWanted)
			{
				Quick();
			}
		}

		private static int Rank(Room r)
		{
			if (!r.playing)
			{
				return (r.players < r.max) ? (r.players + 1000) : 500;
			}
			return r.players;
		}

		private void NameRoom()
		{
			if (!IsHost || !localAvatar || !localAvatar.Object || !localAvatar.Object.IsValid)
			{
				return;
			}
			string name;
			if (metaName == "")
			{
				if (localAvatar.Paint < 1)
				{
					return;
				}
				name = GameManager.ColorName(localAvatar.Paint);
			}
			else
			{
				name = metaName;
			}
			string title = name + "'S ROOM";
			if (!(title == titleSent))
			{
				titleSent = title;
				if (Property("title") != title)
				{
					Queue("title", title);
					SendProperties();
					Player.Note("room title " + title);
				}
			}
		}

		// TODO(recon): nothing in the Android build calls PcName, PcFusion or PcVoice; their caller is probably a PC-only (#if) path that was compiled out (token 0x6000397)
		private static string PcName()
		{
			Prefs.Load();
			string name = ShortName(Prefs.nick);
			return name != "" ? name : "KNUCKLEHEAD" + UnityEngine.Random.Range(1000, 10000);
		}

		private static string ShortName(string raw)
		{
			if (string.IsNullOrWhiteSpace(raw))
			{
				return "";
			}
			StringBuilder name = new StringBuilder();
			foreach (char c in raw.Trim())
			{
				if (c >= ' ' && c <= '~' && c != '<' && c != '>')
				{
					name.Append(char.ToUpperInvariant(c));
					if (name.Length >= 12)
					{
						break;
					}
				}
			}
			return name.ToString().Trim();
		}

		private void HandHost()
		{
			bool host = IsHost;
			if (host && !wasHost)
			{
				hostedWith.Clear();
				foreach (NetworkPlayer avatar in NetworkPlayer.avatars)
				{
					if ((bool)avatar && (bool)avatar.Object && avatar.Object.IsValid && !avatar.HasStateAuthority)
					{
						hostedWith.Add(avatar);
					}
				}
			}
			wasHost = host;
			if (!host)
			{
				return;
			}
			string hostId = GameManager.hostId;
			if (string.IsNullOrEmpty(hostId) || hostId == GameManager.MyId)
			{
				return;
			}
			foreach (NetworkPlayer avatar in NetworkPlayer.avatars)
			{
				if (!avatar || !avatar.Object || !avatar.Object.IsValid || avatar.Uid.Value != hostId || hostedWith.Contains(avatar))
				{
					continue;
				}
				PlayerRef owner = avatar.Object.StateAuthority;
				if (owner != PlayerRef.None && owner != handedTo && Present(owner))
				{
					handedTo = owner;
					runner.SetMasterClient(owner);
					Player.Note("handing the host back to " + avatar.name + " (player " + owner.PlayerId + ")");
				}
				break;
			}
		}

		public void Leave()
		{
			if (!leaving)
			{
				GameManager.hostId = "";
				leaving = true;
				leaveAt = Time.time;
				leaveTo = null;
			}
		}

		public void StartMatch()
		{
			if (IsHost && !leaving && (bool)GameManager.main)
			{
				string go = GameManager.main.FirstHop(session, kind, RoomWins(), RoomCap(), RoomTeams(), RoomMaps());
				Debug.Log("NetworkManager starting the match: " + go);
				Hop(go);
			}
		}

		public void Follow(string go, bool mustExist = false)
		{
			if (leaving || string.IsNullOrEmpty(go))
			{
				return;
			}
			GameManager.Hop hop = GameManager.Decode(go);
			if (hop == null || GameManager.followed.Contains(hop.id))
			{
				return;
			}
			GameManager.followed.Add(hop.id);
			GameManager.hostId = hop.host;
			string title = Property("title");
			Plan plan;
			if (hop.round >= 1)
			{
				GameManager.match = hop;
				if (hop.lobby == GameManager.podiumLobby)
				{
					GameManager.podium.Clear();
				}
				plan = new Plan
				{
					session = hop.session,
					kind = "game",
					visible = hop.lobbyKind == "public",
					state = "playing",
					title = title ?? "",
					maps = hop.maps,
					players = hop.size,
					wins = hop.wins,
					teams = hop.teams,
					mustExist = mustExist
				};
			}
			else
			{
				GameManager.match = null;
				plan = new Plan
				{
					session = hop.lobby,
					kind = hop.lobbyKind,
					visible = hop.lobbyKind == "public",
					drop = true,
					code = (hop.lobby.StartsWith("kh-") && hop.lobby.Length > 8) ? hop.lobby.Substring(3, 6) : "",
					title = title ?? "",
					maps = hop.maps,
					players = MaxRoom,
					cap = hop.size,
					wins = hop.wins,
					teams = hop.teams
				};
			}
			next = plan;
			leaving = true;
			leaveAt = Time.time;
			leaveTo = hop.scene;
			if ((bool)GameManager.main)
			{
				GameManager.main.Leaving(hop);
			}
			Player.Note((hop.round > 0) ? ("round " + hop.round + " on " + hop.scene + " in '" + hop.session + "'") : ("back to the lobby '" + hop.lobby + "'"));
		}

		private static int Round(string match)
		{
			return GameManager.Decode(match ?? "")?.round ?? (-1);
		}

		public bool Rejoin(GameManager.Hop hop)
		{
			if ((bool)runner || leaving || hop == null || authenticating || hop.round < 1)
			{
				return false;
			}
			Player.Note("could not join round " + hop.round + " (" + status + "), " + (gutted ? ("reloading " + hop.scene + " to try '") : "trying '") + hop.session + "' once more");
			error = "";
			status = "connecting";
			Plan plan = new Plan
			{
				session = hop.session,
				kind = "game",
				visible = hop.lobbyKind == "public",
				state = "playing",
				players = hop.size,
				wins = hop.wins,
				teams = hop.teams,
				maps = hop.maps
			};
			RejoinLater(plan, gutted ? hop : null);
			return true;
		}

		private async void RejoinLater(Plan plan, GameManager.Hop reload)
		{
			await Task.Delay(RejoinWait);
			if (!this || leaving || (bool)runner)
			{
				return;
			}
			if (reload == null)
			{
				await Connect(plan);
				return;
			}
			next = plan;
			leaving = true;
			leaveAt = Time.time;
			leaveTo = reload.scene;
			if ((bool)GameManager.main)
			{
				GameManager.main.Leaving(reload);
			}
		}

		public void GoTo(string scene, string why = "")
		{
			if (!leaving)
			{
				carried = why;
				next = null;
				leaving = true;
				leaveAt = Time.time;
				leaveTo = scene;
			}
		}

		public string Property(string key)
		{
			if (!InRoom)
			{
				return null;
			}
			foreach (KeyValuePair<string, string> w in wrote)
			{
				if (w.Key == key && flying.Contains(key))
				{
					return string.IsNullOrEmpty(w.Value) ? null : w.Value;
				}
			}
			if (runner.SessionInfo.IsValid && runner.SessionInfo.Properties != null)
			{
				seen.Clear();
				foreach (KeyValuePair<string, SessionProperty> property in runner.SessionInfo.Properties)
				{
					if (property.Value.IsString)
					{
						seen[property.Key] = (string)property.Value;
					}
				}
			}
			if (!seen.TryGetValue(key, out string value))
			{
				return null;
			}
			return string.IsNullOrEmpty(value) ? null : value;
		}

		public void SetProperty(string key, string value)
		{
			Queue(key, value);
			SendProperties();
		}

		public void Hop(string go)
		{
			Queue("match", "");
			Queue("go", go);
			SendProperties();
			Follow(go);
		}

		private void Queue(string key, string value)
		{
			wrote.RemoveAll((KeyValuePair<string, string> w) => w.Key == key);
			flying.Remove(key);
			wrote.Add(new KeyValuePair<string, string>(key, value ?? ""));
		}

		private void SendProperties()
		{
			Dictionary<string, SessionProperty> all;
			if (wrote.Count == 0 || !InRoom || !runner.SessionInfo.IsValid)
			{
				return;
			}
			if (!IsHost)
			{
				wrote.Clear();
				flying.Clear();
				return;
			}
			all = new Dictionary<string, SessionProperty>();
			if (runner.SessionInfo.Properties != null)
			{
				foreach (KeyValuePair<string, SessionProperty> property in runner.SessionInfo.Properties)
				{
					all[property.Key] = property.Value;
				}
			}
			wrote.RemoveAll((KeyValuePair<string, string> w) => all.TryGetValue(w.Key, out SessionProperty current) && current.IsString && (string)current == w.Value);
			flying.RemoveWhere((string k) => !wrote.Exists((KeyValuePair<string, string> w) => w.Key == k));
			if (wrote.Count == 0 || wrote.TrueForAll((KeyValuePair<string, string> w) => flying.Contains(w.Key)))
			{
				return;
			}
			foreach (KeyValuePair<string, string> w in wrote)
			{
				all[w.Key] = w.Value;
			}
			all["v"] = Protocol;
			if (!runner.SessionInfo.UpdateCustomProperties(all))
			{
				string keys = string.Join(",", wrote.ConvertAll((KeyValuePair<string, string> w) => w.Key));
				if (keys != refused)
				{
					Player.Note("session refused properties " + keys + " (Fusion allows 10 keys, 500 bytes)");
				}
				refused = keys;
				Debug.LogWarning("NetworkManager could not set session properties " + wrote.Count);
				return;
			}
			refused = null;
			foreach (KeyValuePair<string, string> w in wrote)
			{
				flying.Add(w.Key);
			}
		}

		private async void Travel()
		{
			await StopBrowsing();
			if ((bool)runner && runner.IsRunning)
			{
				await runner.Shutdown(destroyGameObject: false);
			}
			if ((bool)this && !travelled)
			{
				travelled = true;
				Load((!string.IsNullOrEmpty(leaveTo)) ? leaveTo : UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
			}
		}

		public static void Load(string scene)
		{
			if (sceneLoad == null || sceneLoad.isDone)
			{
				UnityEngine.Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.Low;
				sceneLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
			}
		}

		private void Watchdog()
		{
			bool stuck = leaving || authenticating || (status == "connecting" && !InRoom) || (InRoom && (!localAvatar || spawnWait || dropReady));
			if (!stuck)
			{
				stuckSince = -1f;
				return;
			}
			if (stuckSince < 0f)
			{
				stuckSince = Time.time;
			}
			if (Time.time - stuckSince < (authenticating ? 90f : LoadStuck))
			{
				return;
			}
			Player.Note("loading stuck for " + (Time.time - stuckSince).ToString("F0") + " s:" + (leaving ? (" leaving for " + leaveTo + ((leaveAt >= 0f) ? " (travel not started)" : " (shutting down)")) : "") + (authenticating ? " signing in" : "") + " status '" + status + "', " + (runner ? ("runner " + (runner.IsRunning ? "running" : "not running")) : "no runner") + (InRoom ? ", in the room" : "") + (localAvatar ? ", avatar" : ", no avatar") + (spawnWait ? ", waiting for map objects" : "") + (dropReady ? ", drop ready" : ""));
			stuckSince = Time.time;
			if (!leaving)
			{
				error = authenticating ? "SIGN-IN TIMED OUT" : Stuck;
				status = "failed: " + error;
				Drop();
				Veil(on: false);
				return;
			}
			if ((bool)runner)
			{
				UnityEngine.Object.Destroy(runner.gameObject);
			}
			runner = null;
			leaveAt = -1f;
			travelled = true;
			Load((!string.IsNullOrEmpty(leaveTo)) ? leaveTo : UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
		}

		public void DropIn()
		{
			DropIn(Mathf.Max(runner.LocalPlayer.PlayerId - 1, 0) % spawnSlots);
		}

		public void DropIn(int first)
		{
			Player body = localBody;
			if (!body || !body.playSpace || !body.pelvis)
			{
				return;
			}
			Vector3 pelvis = body.pelvis.position;
			Vector3 zone = dropZone ? dropZone.position : pelvis;
			float sea = Water.main ? (Water.main.Surface + 0.3f) : float.MinValue;
			if (!Ground(zone, zone.y + 3f, out float floor))
			{
				floor = body.playSpace.position.y;
			}
			Vector3 spot = zone;
			for (int i = 0; i < spawnSlots; i++)
			{
				int slot = (first + i) % spawnSlots;
				Vector3 at = zone;
				if (slot >= 1)
				{
					at = zone + Quaternion.Euler(0f, (float)(slot - 1) * 360f / (float)(spawnSlots - 1), 0f) * Vector3.forward * spawnRing;
				}
				if (Ground(at, zone.y + 3f, out float y) && !(y < sea) && !Blocked(new Vector3(at.x, y + 0.35f, at.z), new Vector3(at.x, y + 1.5f, at.z), body))
				{
					floor = y;
					Player.Note("drop spot slot " + slot + " at " + at.ToString("F1"));
					spot = at;
					break;
				}
			}
			float room = FallRoom(spot, floor, body);
			Vector3 shift = new Vector3(spot.x - pelvis.x, room + (floor - body.playSpace.position.y), spot.z - pelvis.z);
			body.LetGo("join");
			body.Shift(shift);
			body.playSpace.position += shift;
			body.dropping = true;
			Player.Note("dropped in from " + room.ToString("F1") + " m");
		}

		private float FallRoom(Vector3 spot, float floor, Player driver)
		{
			float room = dropHeight;
			RaycastHit[] hits = Physics.SphereCastAll(new Vector3(spot.x, floor + 0.5f, spot.z), 0.35f, Vector3.up, dropHeight + 2f, -1, QueryTriggerInteraction.Ignore);
			foreach (RaycastHit hit in hits)
			{
				if (!(hit.distance <= 0f) && !hit.collider.transform.IsChildOf(driver.transform))
				{
					room = Mathf.Min(room, hit.distance - 1.1f);
				}
			}
			return Mathf.Max(room, 0.2f);
		}

		private void SpawnPoint()
		{
			Player body = localBody;
			if (!body || !body.playSpace || !body.pelvis)
			{
				return;
			}
			Vector3 pelvis = body.pelvis.position;
			float floor = holdFloor;
			if (!holdBody && !Ground(pelvis, pelvis.y, out floor))
			{
				floor = body.playSpace.position.y;
			}
			int count = spawnPoints.Length;
			int hash = runner.SessionInfo.Name.GetHashCode();
			int me = Mathf.Max(runner.LocalPlayer.PlayerId - 1, 0);
			int start = (Mathf.Max(runner.LocalPlayer.PlayerId - 1, 0) + (int)((uint)hash % (uint)count)) % count;
			for (int i = 0; i < count; i++)
			{
				int index = (start + i) % count;
				Transform point = spawnPoints[index];
				if (!point)
				{
					continue;
				}
				Vector3 spot = point.position;
				if (!Ground(spot, spot.y + 1.5f, out float y))
				{
					y = spot.y;
				}
				Vector3 side = Quaternion.Euler(0f, point.eulerAngles.y, 0f) * Vector3.right;
				if (me >= count)
				{
					Vector3 beside = spot + (float)(me / count) * 0.7f * side;
					if (Ground(beside, spot.y + 1.5f, out float besideY) && Mathf.Abs(besideY - y) < 0.3f && !Blocked(new Vector3(beside.x, besideY + 0.35f, beside.z), new Vector3(beside.x, besideY + 1.5f, beside.z)))
					{
						spot = beside;
						y = besideY;
					}
				}
				if (Blocked(new Vector3(spot.x, y + 0.35f, spot.z), new Vector3(spot.x, y + 1.5f, spot.z), body) || InRock(new Vector3(spot.x, y + 1f, spot.z)))
				{
					Player.Note("spawn " + point.name + " is blocked by " + Blocker(new Vector3(spot.x, y + 0.35f, spot.z), new Vector3(spot.x, y + 1.5f, spot.z), body) + ", trying the next one");
					continue;
				}
				Vector3 shift = new Vector3(spot.x - pelvis.x, y - floor, spot.z - pelvis.z);
				body.Shift(shift);
				body.playSpace.position += shift;
				body.MoveSpawn(shift);
				placed = true;
				Player.Note("spawned at " + point.name + " (player " + runner.LocalPlayer.PlayerId + ", point " + (index + 1) + " of " + count + ")");
				return;
			}
			SpreadSpawn();
		}

		private void SpreadSpawn()
		{
			Player body = localBody;
			if (!body || !body.playSpace || !body.pelvis || spawnRing <= 0f)
			{
				return;
			}
			Vector3 pelvis = body.pelvis.position;
			if (!Ground(pelvis, pelvis.y, out float floor))
			{
				return;
			}
			List<Vector3> spots = FreeSpots(pelvis, floor, body);
			if (spots.Count == 0)
			{
				return;
			}
			Vector3 spot = spots[Mathf.Max(runner.LocalPlayer.PlayerId - 1, 0) % spots.Count];
			Vector3 shift = new Vector3(spot.x - pelvis.x, spot.y - floor, spot.z - pelvis.z);
			if (!(shift.sqrMagnitude < 0.0001f))
			{
				body.Shift(shift);
				body.playSpace.position += shift;
				body.MoveSpawn(shift);
			}
		}

		private List<Vector3> FreeSpots(Vector3 home, float homeY, Player driver)
		{
			List<Vector3> grid = new List<Vector3>();
			for (int x = -8; x <= 8; x++)
			{
				for (int z = -8; z <= 8; z++)
				{
					grid.Add(home + new Vector3(x, 0f, z) * (spawnRing * 0.42f));
				}
			}
			grid.Sort((Vector3 a, Vector3 b) => (a - home).sqrMagnitude.CompareTo((b - home).sqrMagnitude));
			List<Vector3> free = new List<Vector3>();
			foreach (Vector3 spot in grid)
			{
				if (!Ground(spot, homeY + 1.5f, out float y) || Mathf.Abs(y - homeY) > 1.2f || Blocked(new Vector3(spot.x, y + 0.35f, spot.z), new Vector3(spot.x, y + 1.5f, spot.z), driver) || InRock(new Vector3(spot.x, y + 1f, spot.z)))
				{
					continue;
				}
				bool apart = true;
				foreach (Vector3 other in free)
				{
					if ((other - new Vector3(spot.x, y, spot.z)).sqrMagnitude < spawnRing * spawnRing * 0.39f)
					{
						apart = false;
					}
				}
				if (apart)
				{
					free.Add(new Vector3(spot.x, y, spot.z));
				}
			}
			return free;
		}

		private static bool Ground(Vector3 at, float from, out float y)
		{
			y = float.NegativeInfinity;
			RaycastHit[] hits = Physics.RaycastAll(new Vector3(at.x, from, at.z), Vector3.down, 6f, -1, QueryTriggerInteraction.Ignore);
			foreach (RaycastHit hit in hits)
			{
				if (!Loose(hit.collider) && hit.point.y > y)
				{
					y = hit.point.y;
				}
			}
			return y > float.MinValue;
		}

		private static bool Loose(Collider c)
		{
			Rigidbody body = c.attachedRigidbody;
			if (!body)
			{
				return false;
			}
			Grabbable grabbable = body.GetComponent<Grabbable>();
			if (((bool)grabbable && !grabbable.holdsYou) || (bool)body.GetComponentInParent<Player>() || (bool)body.GetComponentInParent<AvatarBody>())
			{
				return true;
			}
			return c.GetComponentInParent<Rope>();
		}

		private static bool InRock(Vector3 at)
		{
			bool backfaces = Physics.queriesHitBackfaces;
			Physics.queriesHitBackfaces = true;
			bool inside = Physics.Raycast(at, Vector3.up, out RaycastHit hit, 50f, -1, QueryTriggerInteraction.Ignore) && !hit.collider.attachedRigidbody && Vector3.Dot(hit.normal, Vector3.up) > 0f;
			Physics.queriesHitBackfaces = backfaces;
			return inside;
		}

		private static string Blocker(Vector3 a, Vector3 b, Player self)
		{
			foreach (Collider c in Physics.OverlapCapsule(a, b, 0.3f, -1, QueryTriggerInteraction.Ignore))
			{
				if (!c.attachedRigidbody)
				{
					return c.name + " (static)";
				}
				if ((bool)self && c.attachedRigidbody.GetComponentInParent<Player>() != self)
				{
					return c.name + " on " + c.attachedRigidbody.name;
				}
			}
			return "rock above";
		}

		private static bool Blocked(Vector3 a, Vector3 b, Player self = null)
		{
			foreach (Collider c in Physics.OverlapCapsule(a, b, 0.3f, -1, QueryTriggerInteraction.Ignore))
			{
				if (!c.attachedRigidbody || ((bool)self && c.attachedRigidbody.GetComponentInParent<Player>() != self))
				{
					return true;
				}
			}
			return false;
		}

		private void Talk()
		{
			if (!mic || !voice || voice.Client == null)
			{
				Player.voiceLive = false;
				Player.sharedMic = 0f;
				return;
			}
			bool live = voice.Client.InRoom;
			bool send = live && Prefs.mic && !Player.ownMic && Player.MicAllowed();
			if (send != mic.RecordingEnabled)
			{
				mic.RecordingEnabled = send;
			}
			mic.TransmitEnabled = send;
			Player.voiceLive = live;
			Player.sharedMic = mic.IsCurrentlyTransmitting ? 1f : 0f;
			if (voiceRetryAt >= 0f && Time.time >= voiceRetryAt && !voiceSigning && InRoom && !leaving)
			{
				Revoice();
			}
		}

		private void VoiceChanged(ClientState from, ClientState to)
		{
			if (!voice)
			{
				return;
			}
			switch (to)
			{
			case ClientState.Joined:
				voiceRefused = 0;
				Player.Note("voice chat joined " + voice.Client.CurrentRoom.Name + " with " + voice.Client.CurrentRoom.PlayerCount + " of " + (InRoom ? runner.SessionInfo.PlayerCount : 0) + " players" + ((voiceTries > 0) ? (" after " + voiceTries + " retries") : ""));
				break;
			case ClientState.Leaving:
				if (!leaving)
				{
					Player.Note("voice chat is leaving its room while the session is " + (runner.IsInSession ? "live" : "gone"));
				}
				break;
			case ClientState.Disconnected:
			{
				DisconnectCause cause = voice.Client.DisconnectedCause;
				Player.Note("voice chat disconnected from " + from + ": " + cause + ", now " + voice.Client.State);
				if (cause != DisconnectCause.None && (cause != DisconnectCause.DisconnectByClientLogic || voiceRefused != 0) && voiceTries < VoiceTries && !leaving)
				{
					voiceRetryAt = Time.time + Mathf.Min(Mathf.Pow(3f, voiceTries) * 3f, 60f);
				}
				break;
			}
			case ClientState.ConnectedToMasterServer:
				if (!leaving)
				{
					Player.Note("voice chat reached the master server" + (runner.SessionInfo.IsValid ? "" : " during a session write") + ", now " + voice.Client.State);
				}
				break;
			}
		}

		private void VoiceRefused(OperationResponse response)
		{
			if (response.ReturnCode != 0)
			{
				voiceRefused = response.ReturnCode;
				Player.Note("voice chat op " + response.OperationCode + " refused " + response.ReturnCode + ": " + response.DebugMessage);
			}
		}

		private static void VoiceLinked(Speaker speaker)
		{
			NetworkPlayer owner = speaker.GetComponentInParent<NetworkPlayer>();
			Player.Note("voice chat p#" + speaker.RemoteVoice.PlayerId + " plays on " + (owner ? owner.name : speaker.name));
		}

		private async void Revoice()
		{
			voiceRetryAt = -1f;
			voiceSigning = true;
			voiceTries++;
			FusionVoiceClient was = voice;
			AuthenticationValues auth = await SignIn();
			voiceSigning = false;
			if ((bool)was && !(was != voice) && InRoom && voice.Client.State == ClientState.Disconnected)
			{
				voice.UseFusionAuthValues = false;
				voice.Client.AuthValues = auth;
				Player.Note("voice chat retry " + voiceTries + " with a fresh sign-in" + (voice.ConnectAndJoinRoom() ? "" : " could not start"));
			}
		}

		private void Update()
		{
			Watchdog();
			if ((bool)runner && runner.IsRunning && runner.RemoteRenderTime > 0f)
			{
				float offset = runner.RemoteRenderTime - Time.time;
				clockOffset = (Mathf.Abs(offset - clockOffset) > 0.5f) ? offset : Mathf.Lerp(clockOffset, offset, 0.02f);
			}
			ShowConnecting();
			SendProperties();
			Talk();
			if (spawnWait)
			{
				Arrive();
			}
			if (dropReady && (!localBody || localBody.dark >= 0.99f))
			{
				dropReady = false;
				DropIn();
				Veil(on: false);
			}
			if (leaving)
			{
				if ((bool)localBody)
				{
					localBody.blackout = 1f;
				}
				bool sent = wrote.TrueForAll((KeyValuePair<string, string> w) => flying.Contains(w.Key));
				if (leaveAt >= 0f && Time.time > leaveAt + 0.45f && (sent || Time.time > leaveAt + SendWait))
				{
					leaveAt = -1f;
					Travel();
				}
				return;
			}
			if (joinedAt >= 0f && (!runner || !runner.IsRunning))
			{
				Player.Note("lost the connection to '" + session + "' (" + status + ")");
				error = Lost;
				Drop();
				return;
			}
			string go = Property("go");
			if (go != null && go != seenGo)
			{
				seenGo = go;
				GameManager.Hop hop = GameManager.Decode(go);
				if (hop != null && !GameManager.followed.Contains(hop.id))
				{
					Follow(go);
					return;
				}
				if (IsHost && GameManager.match == null)
				{
					Queue("go", null);
					SendProperties();
				}
			}
			NameRoom();
			HandHost();
			if (!localAvatar && (bool)runner && runner.IsRunning && (bool)networkPlayerPrefab && (bool)localBody && !(joinedAt < 0f) && !(Time.time < joinedAt + 0.5f) && !runner.IsSceneManagerBusy && runner.CanSpawn)
			{
				localAvatar = runner.Spawn(networkPlayerPrefab, localBody.pelvis ? localBody.pelvis.position : transform.position, Quaternion.identity, runner.LocalPlayer);
				spawnTries++;
				if ((bool)localAvatar)
				{
					Debug.Log("NetworkManager avatar spawned after " + spawnTries + " tries, " + (Time.time - joinedAt).ToString("F2") + " s in");
				}
			}
		}

		private void Hold(bool on)
		{
			Ignore(on: false);
			if (!on)
			{
				holdBody = false;
				return;
			}
			holdOn = null;
			if (!localBody || !localBody.pelvis)
			{
				holdBody = false;
				return;
			}
			holdBody = localBody.playSpace;
			if (!holdBody)
			{
				return;
			}
			Vector3 at = localBody.pelvis.position;
			holdAt = at;
			holdFloor = float.NegativeInfinity;
			Rigidbody under = null;
			RaycastHit[] hits = Physics.RaycastAll(at, Vector3.down, 6f, -1, QueryTriggerInteraction.Ignore);
			foreach (RaycastHit hit in hits)
			{
				if (!Loose(hit.collider) && !(hit.point.y <= holdFloor))
				{
					holdFloor = hit.point.y;
					under = hit.collider.attachedRigidbody;
				}
			}
			holdBody = (bool)under && !under.isKinematic && (bool)under.GetComponentInParent<NetworkObject>();
			holdOn = under;
			holdOnY = under ? under.position.y : 0f;
			if (holdBody)
			{
				heldMine = localBody.GetComponentsInChildren<Collider>();
				heldFloor = under.GetComponentsInChildren<Collider>();
				Ignore(on: true);
			}
		}

		private void Ignore(bool on)
		{
			if (heldMine == null || heldFloor == null)
			{
				return;
			}
			foreach (Collider mine in heldMine)
			{
				foreach (Collider floor in heldFloor)
				{
					if ((bool)mine && (bool)floor)
					{
						Physics.IgnoreCollision(mine, floor, on);
					}
				}
			}
			if (!on)
			{
				heldMine = null;
				heldFloor = null;
			}
		}

		private void FixedUpdate()
		{
			if (holdBody && (bool)localBody && (bool)localBody.pelvis)
			{
				localBody.Shift(holdAt - localBody.pelvis.position);
			}
		}

		private void Arrive()
		{
			int missing = Missing();
			float behind = float.MaxValue;
			if (missing == 0)
			{
				behind = Behind();
			}
			if ((missing != 0 || behind > 0.15f) && Time.time < spawnWaitFrom + arriveWait)
			{
				return;
			}
			spawnWait = false;
			string ride = holdOn ? (", " + holdOn.name + " under me moved " + (holdOn.position.y - holdOnY).ToString("F2") + " m during the join" + (holdBody ? "" : " (not held)")) : "";
			Player.Note(missingAtJoin + " map objects were still on their way at join, " + ((missing > 0) ? (missing + " never arrived, spawning anyway after ") : "all here after ") + (Time.time - spawnWaitFrom).ToString("F2") + " s" + ((missing == 0 && behind > 0.15f) ? (", a platform still " + behind.ToString("F2") + " m from the host's") : "") + ride);
			placed = false;
			if (spawnPoints == null || spawnPoints.Length == 0)
			{
				SpreadSpawn();
			}
			else
			{
				SpawnPoint();
			}
			if (!placed && holdBody && (bool)holdOn && (bool)localBody)
			{
				Vector3 rode = Vector3.up * (holdOn.position.y - holdOnY);
				localBody.Shift(rode);
				localBody.playSpace.position += rode;
				localBody.MoveSpawn(rode);
				Player.Note("no spawn point was free, rode " + holdOn.name + " " + rode.y.ToString("F2") + " m instead");
			}
			Hold(on: false);
		}

		private static float Behind()
		{
			UnityEngine.SceneManagement.Scene active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			float behind = 0f;
			foreach (NetRope rope in UnityEngine.Object.FindObjectsByType<NetRope>(FindObjectsSortMode.None))
			{
				if (!(rope.gameObject.scene != active) && !rope.GetComponent<Rope>() && !rope.GetComponent<HingeJoint>())
				{
					behind = Mathf.Max(behind, rope.Behind());
				}
			}
			return behind;
		}

		private static int Missing()
		{
			UnityEngine.SceneManagement.Scene active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			int missing = 0;
			foreach (NetworkObject obj in UnityEngine.Object.FindObjectsByType<NetworkObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			{
				if (obj.gameObject.scene == active && !obj.gameObject.activeInHierarchy)
				{
					missing++;
				}
			}
			return missing;
		}

		private static int SceneObjects()
		{
			UnityEngine.SceneManagement.Scene active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			uint keys = 0u;
			int count = 0;
			foreach (NetworkObject obj in UnityEngine.Object.FindObjectsByType<NetworkObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			{
				if (!(obj.gameObject.scene != active))
				{
					keys ^= obj.SortKey;
					count++;
				}
			}
			return unchecked((int)keys + count * 486187739);
		}

		public bool Present(PlayerRef player)
		{
			if (!runner || !runner.IsRunning)
			{
				return false;
			}
			foreach (PlayerRef active in runner.ActivePlayers)
			{
				if (active == player)
				{
					return true;
				}
			}
			return false;
		}

		public int Players()
		{
			if (!runner || !runner.IsRunning || !runner.SessionInfo.IsValid)
			{
				return 0;
			}
			return runner.SessionInfo.PlayerCount;
		}

		public int RoomWins()
		{
			if (!InRoom || !runner.SessionInfo.IsValid || runner.SessionInfo.Properties == null || !runner.SessionInfo.Properties.TryGetValue("wins", out SessionProperty wins))
			{
				return 3;
			}
			if (wins.IsInt)
			{
				return (int)wins;
			}
			if (wins.IsString)
			{
				if (!int.TryParse((string)wins, out int parsed))
				{
					return 3;
				}
				return parsed;
			}
			return 3;
		}

		private static string[] Rules(string text)
		{
			string[] parts = (text ?? "").Split('|');
			if (parts.Length > 2)
			{
				return parts;
			}
			return new string[3] { "0", "", "0" };
		}

		private static int Cap(IReadOnlyDictionary<string, SessionProperty> properties)
		{
			if (properties == null || !properties.TryGetValue("rules", out SessionProperty rules) || !rules.IsString)
			{
				return 0;
			}
			if (!int.TryParse(Rules((string)rules)[2], out int cap))
			{
				return 0;
			}
			return cap;
		}

		public int RoomTeams()
		{
			if (!int.TryParse(Rules(Property("rules"))[0], out int teams))
			{
				return 0;
			}
			return teams;
		}

		public string RoomMaps()
		{
			return Rules(Property("rules"))[1];
		}

		public int RoomCap()
		{
			bool parsed = int.TryParse(Rules(Property("rules"))[2], out int cap);
			int max = MaxPlayers();
			if (!parsed || cap <= 0)
			{
				return max;
			}
			return Mathf.Min(cap, Mathf.Max(max, 1));
		}

		public void UpdateRoom(bool open, int cap, int wins, int teams, string maps)
		{
			if (IsHost)
			{
				if (Open != open)
				{
					SetOpen(open);
				}
				Queue("rules", Mathf.Max(teams, -1) + "|" + maps + "|" + Mathf.Clamp(cap, Mathf.Max(Players(), 2), MaxRoom));
				SendProperties();
				Queue("wins", Mathf.Clamp(wins, 1, 8).ToString());
				SendProperties();
				Player.Note("room settings now: " + (open ? "public" : "private") + ", " + cap + " players, first to " + wins + ", " + ((teams > 0) ? "teams (auto split)" : ((teams == 0) ? "free for all" : "rounds")) + ", maps " + (string.IsNullOrEmpty(maps) ? "all" : maps));
			}
		}

		public int MaxPlayers()
		{
			if (!runner || !runner.IsRunning || !runner.SessionInfo.IsValid)
			{
				return 0;
			}
			return runner.SessionInfo.MaxPlayers;
		}
	}
}
