using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Oculus.Platform;
using Oculus.Platform.Models;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Ragdoll
{
	public class Account : MonoBehaviour
	{
		[Serializable]
		private class LoginBody
		{
			public string CustomId;

			public string UserId;

			public string Nonce;

			public string IntegrityToken;

			public string Name;

			public int Build;
		}

		[Serializable]
		private class LoginReply
		{
			public string PlayFabId;

			public string SessionTicket;

			public string EntityId;

			public string EntityType;

			public string EntityToken;

			public int Xp;

			public int Teeth;

			public int Wins;
		}

		[Serializable]
		private class RoundReply
		{
			public bool Accepted;

			public int Xp;

			public int Teeth;

			public int Wins;

			public Challenge[] Challenges;

			public string[] Completed;
		}

		[Serializable]
		private class ChallengeReply
		{
			public Challenge[] Challenges;

			public Challenge Prize;
		}

		[Serializable]
		private class UnlockReply
		{
			public bool Ok;

			public int Teeth;

			public string Owned;
		}

		[Serializable]
		private class Challenge
		{
			public string Name;

			public int Goal;

			public int Progress;

			public int Reward;

			public bool Done;

			public string Title;
		}

		[Serializable]
		private class BanReply
		{
			public string Message;

			public string Expires;
		}

		[Serializable]
		private class BuyReply
		{
			public bool Ok;

			public int Amount;

			public int Teeth;

			public string Error;
		}

		private struct Flyer
		{
			public Transform tooth;

			public Transform around;

			public Vector3 centre;

			public Vector3 size;

			public Vector3 velocity;

			public Vector3 spin;

			public float angle;

			public float speed;

			public float radius;

			public float height;

			public float born;

			public bool dropped;

			public int bounces;

			public float landed;

			public Quaternion lie;
		}

		private struct Row
		{
			public string name;

			public string value;

			public bool me;

			public int score;
		}

		[Header("Level Up")]
		public AudioClip levelSound;

		public AudioClip fillSound;

		public AudioClip teethSound;

		public AudioClip purchaseSound;

		public GameObject levelBurst;

		public GameObject levelFireworks;

		public float volume = 0.8f;

		[Header("Gold Teeth")]
		public GameObject toothModel;

		public Material toothMaterial;

		public float toothSize = 0.08f;

		public static Account main;

		public static readonly string[] Packs = new string[4] { "teeth_500", "teeth_1500", "teeth_3500", "teeth_8000" };

		public const string Crown = "Crown";

		private const string Server = "https://dre1mer.org/knuckle/pf/";

		private const int RoundXp = 10;

		private const int WinXp = 25;

		private const int ChampionXp = 50;

		private const int WaveXp = 10;

		private const int TeethPerLevel = 100;

		private const float BoardRefresh = 60f;

		private const float FillDelay = 0.6f;

		private const float FillTime = 1.6f;

		private const float BeatFill = 1.1f;

		private const float BarWidth = 1f;

		private const float BarHeight = 0.07f;

		private const float LobbyWait = 2.5f;

		private const float PopHold = 2.8f;

		private const float PopFade = 0.3f;

		private const float PopDistance = 1.8f;

		private const float BurstTime = 0.55f;

		private const float SwirlTime = 1.9f;

		private const float LieTime = 6f;

		private const float ShrinkTime = 0.5f;

		private const float FallLimit = 9f;

		private static readonly Color Gold = new Color(1f, 0.85f, 0.24f, 1f);

		private static int ground;

		private static readonly Color Track = new Color(0.06f, 0.1f, 0.22f, 1f);

		private static readonly string[] Medals = new string[3] { "#FFE14D", "#F2FAFF", "#FFB066" };

		private static readonly string[] ChallengeNames = new string[6] { "PLAY 5 ROUNDS", "WIN 2 ROUNDS", "WIN A MATCH", "SURVIVE 5 WAVES", "PLAY 3 TEAM ROUNDS", "FINISH 3 ROUNDS ALIVE" };

		private static readonly int[] ChallengeGoals = new int[6] { 5, 2, 1, 5, 3, 3 };

		private static readonly int[] ChallengeRewards = new int[6] { 50, 100, 150, 100, 75, 75 };

		private static Challenge[] daily;

		private static long dailyDay = -1L;

		private static readonly int[] PackTeeth = new int[4] { 500, 1500, 3500, 8000 };

		private static readonly string[] PracticePrices = new string[4] { "$1.99", "$4.99", "$9.99", "$19.99" };

		private static readonly string[] prices = new string[4];

		private static string metaUser;

		private static bool buying;

		private static float claimAt = -1f;

		private static int claimTries;

		private static bool championKnown;

		private static bool dailyBusy;

		private static bool unlocking;

		private static readonly HashSet<string> owned = new HashSet<string>();

		private static readonly Dictionary<string, string> ids = new Dictionary<string, string>();

		private const string CatalogVersion = "Cosmetics";

		private static readonly Dictionary<string, int> Costs = new Dictionary<string, int>
		{
			{ "Traffic Cone", 250 },
			{ "Hard Hat Knuck", 250 },
			{ "Party Hat", 250 },
			{ "Tank Top", 250 },
			{ "Apron", 250 },
			{ "Bow Tie", 250 },
			{ "Scarf", 250 },
			{ "Rain Boots", 250 },
			{ "Mustache", 250 },
			{ "Clown Nose", 250 },
			{ "Eyepatch", 250 },
			{ "Top Hat", 500 },
			{ "Military Hat", 500 },
			{ "Headphones", 500 },
			{ "Chef Hat", 500 },
			{ "Cowboy Hat", 500 },
			{ "Hi Vis Vest", 500 },
			{ "Backpack", 500 },
			{ "Tutu", 500 },
			{ "Sneakers", 500 },
			{ "Ski Goggles", 500 },
			{ "Viking Helmet", 1000 },
			{ "Cape", 1000 },
			{ "Champion Belt", 1500 },
			{ "Luchador Mask", 1000 },
			{ "Luchador Pants", 500 }
		};

		private static readonly string[] PracticeNames = new string[12] { "SLAPPY", "BIG KNUCK", "NOODLE", "BONK", "WOBBLES", "PUNCHY", "TOAST", "GOOBER", "SPROCKET", "MUFFIN", "BEANS", "CHONK" };

		private int shownFrom;

		private int shownTo;

		private float showAt = -1f;

		private float hideAt = -1f;

		private int shownLevel;

		private float punchAt = -1f;

		private float teethAt = -1f;

		private int teethGain;

		private int matchFrom = -1;

		private int matchTeethFrom;

		private float lobbyAt = -1f;

		private float popAlpha;

		private int nextTap;

		private int tapStep;

		private AudioSource tapSource;

		private readonly List<Flyer> flying = new List<Flyer>();

		private float toothScale;

		private Transform popup;

		private Material popTrack;

		private Material popFill;

		private Transform popFillT;

		private TextMeshPro popTitle;

		private TextMeshPro popLevel;

		private TextMeshPro popGain;

		private TextMeshPro popTeeth;

		private TextMeshPro popNote;

		private readonly List<string> doneNotes = new List<string>();

		private readonly List<Challenge> beats = new List<Challenge>();

		private readonly List<Challenge> prizes = new List<Challenge>();

		private Challenge beat;

		private float beatAt = -1f;

		private float beatFillAt = -1f;

		private bool leveled;

		private Transform dailyBoard;

		private Transform rewardCrown;

		private Vector3 rewardHome;

		private TMP_Text[] rewardNames;

		private TMP_Text dailyStatus;

		private TMP_Text dailyResets;

		private TMP_Text[] dailyNames;

		private TMP_Text[] dailyCounts;

		private TMP_Text[] dailyRewards;

		private Image[] dailyFills;

		private GameObject[] dailyDone;

		private GameObject[] dailyOpen;

		private int dailyShown = -2;

		private string dailyKey = "";

		private Transform board;

		private TMP_Text[] levelNames;

		private TMP_Text[] levelValues;

		private TMP_Text[] winNames;

		private TMP_Text[] winValues;

		private TMP_Text boardStatus;

		private TMP_Text boardHeader;

		private TMP_Text boardLevel;

		private TMP_Text boardXp;

		private TMP_Text boardTeeth;

		private Image boardFill;

		private Transform boardTooth;

		private float fetchAt = -1f;

		private bool fetching;

		private AudioSource audioSource;

		public static bool Ready { get; private set; }

		public static bool Practice { get; private set; }

		public static string Status { get; private set; } = "SIGNING IN...";

		public static int Xp { get; private set; }

		public static int Teeth { get; private set; }

		public static int Wins { get; private set; }

		public static int Level => LevelOf(Xp, out _, out _);

		public static string ShopStatus { get; private set; } = "";

		public static bool Champion { get; private set; }

		public static bool ChampionKnown => championKnown;

		public static int OwnedCount => owned.Count;

		private static int Ground
		{
			get
			{
				if (ground == 0)
				{
					ground = ~LayerMask.GetMask("Player", "OwnHead", "Water", "UI", "Ignore Raycast", "TransparentFX", "LCK Tablet", "LCK Touch", "LCK Handle");
				}
				return ground;
			}
		}

		public static bool Unlocking => unlocking;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Clear()
		{
			main = null;
			Ready = false;
			Practice = false;
			Status = "SIGNING IN...";
			Xp = 0;
			Teeth = 0;
			Wins = 0;
			ShopStatus = "";
			metaUser = null;
			buying = false;
			claimAt = -1f;
			Array.Clear(prices, 0, prices.Length);
			Champion = false;
			championKnown = false;
			unlocking = false;
			owned.Clear();
			ids.Clear();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Boot()
		{
			if (!main)
			{
				PlayFabSettings.DisableFocusTimeCollection = true;
				PlayFabSettings.DisableDeviceInfo = true;
				Account prefab = Resources.Load<Account>("Account");
				main = prefab ? Instantiate(prefab) : new GameObject().AddComponent<Account>();
				main.name = "Account";
				DontDestroyOnLoad(main.gameObject);
			}
		}

		private async void Start()
		{
			audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.spatialBlend = 0f;
			audioSource.bypassListenerEffects = true;
			tapSource = gameObject.AddComponent<AudioSource>();
			tapSource.playOnAwake = false;
			tapSource.spatialBlend = 0f;
			tapSource.bypassListenerEffects = true;
			SceneManager.sceneLoaded += Loaded;
			Loaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
			await Login();
			fetchAt = 0f;
		}

		private void OnDestroy()
		{
			SceneManager.sceneLoaded -= Loaded;
		}

		public static int Need(int level)
		{
			return Mathf.Min(80 + level * 20, 400);
		}

		public static int LevelOf(int xp, out int into, out int need)
		{
			into = Mathf.Max(0, xp);
			int level = 1;
			need = Need(level);
			while (into >= need)
			{
				into -= need;
				level++;
				need = Need(level);
			}
			return level;
		}

		public static string Tint(int level)
		{
			if (level >= 50)
			{
				return "#FF7AD9";
			}
			if (level >= 25)
			{
				return "#FFE14D";
			}
			if (level >= 10)
			{
				return "#E8F7FF";
			}
			return "#FFB066";
		}

		public static string Badge(int level)
		{
			return "<color=" + Tint(level) + ">LV " + level + "</color>";
		}

		// Local build: there is no backend of our own yet (see Docs/BACKEND_AUTH.md), so Account runs
		// in the game's offline practice mode instead of signing in at the original developer's
		// server. Set this to false once Server points at our own login endpoint.
		private static readonly bool Offline = true;

		private static async Task Login()
		{
			if (Offline)
			{
				StartPractice();
				return;
			}
			Load(PlayerPrefs.GetString("owned", ""));
			int tries = 1;
			while (true)
			{
				try
				{
					await SignIn();
					return;
				}
				catch (Exception e)
				{
					Debug.LogWarning("Account sign-in try " + tries + " failed: " + e.Message);
					if (Status.StartsWith("BANNED") || tries >= 3)
					{
						if (!Status.StartsWith("BANNED"))
						{
							Status = "LEADERBOARDS OFFLINE";
						}
						if (ShopStatus == "SIGNING IN...")
						{
							ShopStatus = "SHOP OFFLINE";
						}
						Debug.LogError("PlayFab login failed at " + Server + "login after " + tries + " tries: " + e.Message);
						return;
					}
				}
				await Task.Delay(tries * 2000);
				tries++;
			}
		}

		// Not in the shipped game: it has the practice paths but nothing that enters them.
		private static void StartPractice()
		{
			Practice = true;
			Xp = PlayerPrefs.GetInt("practiceXp", 0);
			Teeth = PlayerPrefs.GetInt("practiceTeeth", 0);
			Wins = PlayerPrefs.GetInt("practiceWins", 0);
			Load(PlayerPrefs.GetString("practiceOwned", ""));
			Status = "";
			Ready = true;
			if (ShopStatus == "SIGNING IN...")
			{
				ShopStatus = "";
			}
		}

		private static async Task SignIn()
		{
			if (!Core.IsInitialized())
			{
				await Meta(Core.AsyncInitialize(), "meta init");
			}
			Callbacks();
			User user = await Meta(Users.GetLoggedInUser(), "get meta user");
			if (user == null || user.ID == 0)
			{
				throw new InvalidOperationException("empty user");
			}
			OrgScopedID org = await Meta(Users.GetOrgScopedID(user.ID), "get org scope");
			if (org == null || org.ID == 0)
			{
				throw new InvalidOperationException("empty org");
			}
			UserProof proof = await Meta(Users.GetUserProof(), "get nonce");
			if (proof == null || string.IsNullOrEmpty(proof.Value))
			{
				throw new InvalidOperationException("empty nonce");
			}
			string token = await Meta(DeviceApplicationIntegrity.GetIntegrityToken(proof.Value), "get token");
			LoginBody body = new LoginBody
			{
				CustomId = org.ID.ToString(),
				UserId = user.ID.ToString(),
				Nonce = proof.Value,
				IntegrityToken = token,
				Name = Clean(string.IsNullOrEmpty(user.DisplayName) ? user.OculusID : user.DisplayName),
				Build = NetworkManager.Protocol
			};
			LoginReply reply = JsonUtility.FromJson<LoginReply>(await Post("login", JsonUtility.ToJson(body)));
			if (reply == null || string.IsNullOrEmpty(reply.PlayFabId) || string.IsNullOrEmpty(reply.EntityToken))
			{
				throw new InvalidOperationException("login returned nothing");
			}
			PlayFabSettings.staticPlayer.PlayFabId = reply.PlayFabId;
			PlayFabSettings.staticPlayer.ClientSessionTicket = reply.SessionTicket;
			PlayFabSettings.staticPlayer.EntityId = reply.EntityId;
			PlayFabSettings.staticPlayer.EntityType = reply.EntityType;
			PlayFabSettings.staticPlayer.EntityToken = reply.EntityToken;
			metaUser = body.UserId;
			Xp = reply.Xp;
			Teeth = reply.Teeth;
			Wins = reply.Wins;
			Status = "";
			Ready = true;
			if (ShopStatus == "SIGNING IN...")
			{
				ShopStatus = "";
			}
			Refresh(body.Name);
			Daily();
			Prices();
			Restore();
		}

		private static void Refresh(string name)
		{
			PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest
			{
				StatisticNames = new List<string> { "xp", "wins_week" }
			}, delegate(GetPlayerStatisticsResult result)
			{
				if (result.Statistics == null)
				{
					return;
				}
				foreach (StatisticValue stat in result.Statistics)
				{
					if (stat.StatisticName == "xp")
					{
						Xp = Mathf.Max(Xp, stat.Value);
					}
					if (stat.StatisticName == "wins_week")
					{
						Wins = Mathf.Max(Wins, stat.Value);
					}
				}
			}, delegate(PlayFabError error)
			{
				Debug.LogWarning("Account statistics failed: " + error.GenerateErrorReport());
			});
			Catalog();
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				if (result.VirtualCurrency != null && result.VirtualCurrency.TryGetValue("GT", out var teeth))
				{
					Teeth = Mathf.Max(Teeth, teeth);
				}
				Owns(result.Inventory);
				Keep();
			}, delegate(PlayFabError error)
			{
				Debug.LogWarning("Account inventory failed: " + error.GenerateErrorReport());
			});
			string want = name;
			if (want == null || want.Length <= 2)
			{
				want = PlayerPrefs.GetString("pfName", "");
				if (want.Length <= 2)
				{
					want = "KNUCKLEHEAD" + Random.Range(1000, 10000);
				}
			}
			if (!(PlayerPrefs.GetString("pfName", "") == want))
			{
				Rename(want, want, 0);
			}
		}

		private static void Rename(string want, string name, int tries)
		{
			PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
			{
				DisplayName = name
			}, delegate
			{
				PlayerPrefs.SetString("pfName", want);
				PlayerPrefs.Save();
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NameNotAvailable && tries <= 3)
				{
					Rename(want, want + " " + Random.Range(10, 100), tries + 1);
				}
				else
				{
					Debug.LogWarning("Account display name failed: " + error.GenerateErrorReport());
				}
			});
		}

		private static async Task<string> Post(string path, string json)
		{
			using UnityWebRequest request = new UnityWebRequest(Server + path, "POST");
			request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 20;
			UnityWebRequestAsyncOperation operation = request.SendWebRequest();
			while (!operation.isDone)
			{
				await Task.Yield();
			}
			if (request.responseCode == 400 || request.responseCode == 403)
			{
				BanReply ban = JsonUtility.FromJson<BanReply>(request.downloadHandler.text ?? "{}");
				if (ban != null && !string.IsNullOrEmpty(ban.Message))
				{
					Ready = false;
					Status = "BANNED: " + ban.Message.ToUpperInvariant() + (string.IsNullOrEmpty(ban.Expires) ? "" : ("\nUNTIL " + ban.Expires.ToUpperInvariant()));
					throw new InvalidOperationException("banned");
				}
			}
			if (request.result != UnityWebRequest.Result.Success)
			{
				string body = ((request.downloadHandler != null) ? request.downloadHandler.text : "");
				if (body != null && body.Length > 300)
				{
					body = body.Substring(0, 300);
				}
				throw new InvalidOperationException(request.responseCode + " " + request.error + " " + body);
			}
			return request.downloadHandler.text;
		}

		private static void Callbacks()
		{
			CallbackRunner runner = UnityEngine.Object.FindFirstObjectByType<CallbackRunner>();
			if (!runner)
			{
				GameObject go = new GameObject("Oculus.Platform.CallbackRunner");
				DontDestroyOnLoad(go);
				go.AddComponent<CallbackRunner>();
			}
			else if (!runner.enabled)
			{
				runner.enabled = true;
			}
		}

		private static async Task<T> Meta<T>(Request<T> request, string step)
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
			if (await Task.WhenAny(completion.Task, Task.Delay(20000)) != completion.Task)
			{
				throw new TimeoutException(step + " timed out");
			}
			return await completion.Task;
		}

		private static string Clean(string raw)
		{
			if (string.IsNullOrEmpty(raw))
			{
				return "";
			}
			StringBuilder clean = new StringBuilder();
			string upper = raw.ToUpperInvariant();
			foreach (char c in upper)
			{
				if (c >= ' ' && c <= '~' && c != '<' && c != '>')
				{
					clean.Append(c);
				}
				if (clean.Length >= 12)
				{
					break;
				}
			}
			return clean.ToString().Trim();
		}

		public static void Round(bool won, bool champion, bool wave, int players, bool alive, string mode)
		{
			if (!Ready || !main)
			{
				return;
			}
			if (Practice)
			{
				int gain = RoundXp + (won ? WinXp : 0) + (champion ? ChampionXp : 0) + (wave ? WaveXp : 0);
				int level = LevelOf(Xp + gain, out _, out _);
				int before = Level;
				List<string> completed = new List<string>();
				Challenge[] picks = Picks();
				int bonus = 0;
				foreach (Challenge c in picks)
				{
					if (!c.Done && (c.Name == ChallengeNames[0] || (c.Name == ChallengeNames[1] && won) || (c.Name == ChallengeNames[2] && champion) || (c.Name == ChallengeNames[3] && wave) || (c.Name == ChallengeNames[4] && mode == "teams") || (c.Name == ChallengeNames[5] && alive)))
					{
						c.Progress = Mathf.Min(c.Progress + 1, c.Goal);
						if (c.Progress >= c.Goal)
						{
							c.Done = true;
							completed.Add(c.Name);
							bonus += c.Reward;
						}
					}
				}
				daily = picks;
				main.Apply(Xp + gain, Teeth + bonus + (level - before) * TeethPerLevel, Wins + (won ? 1 : 0), completed.ToArray());
				PlayerPrefs.SetInt("practiceXp", Xp);
				PlayerPrefs.SetInt("practiceTeeth", Teeth);
				PlayerPrefs.SetInt("practiceWins", Wins);
				PlayerPrefs.Save();
				return;
			}
			NetworkManager net = NetworkManager.main;
			if (GameManager.match != null)
			{
				if (GameManager.match.lobbyKind == "private")
				{
					return;
				}
			}
			else if ((bool)net && net.InRoom && !net.Open)
			{
				return;
			}
			Dictionary<string, object> args = new Dictionary<string, object>
			{
				{
					"Session",
					(GameManager.match != null && !string.IsNullOrEmpty(GameManager.match.session)) ? GameManager.match.session : (((bool)net && (bool)net.runner && net.runner.SessionInfo != null && net.runner.SessionInfo.IsValid) ? net.runner.SessionInfo.Name : "")
				},
				{
					"Round",
					(GameManager.match != null) ? GameManager.match.round : 0
				},
				{ "Won", won },
				{ "Champion", champion },
				{ "Alive", alive },
				{ "Mode", mode },
				{ "Wave", wave },
				{ "Players", players }
			};
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "round",
				FunctionParameter = args
			}, delegate(ExecuteCloudScriptResult result)
			{
				if (result.Error != null)
				{
					Debug.LogWarning("Account round script failed: " + result.Error.Error + " " + result.Error.Message);
					return;
				}
				RoundReply reply = JsonUtility.FromJson<RoundReply>(PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).SerializeObject(result.FunctionResult) ?? "{}");
				if (reply != null && reply.Accepted && (bool)main)
				{
					if (reply.Challenges != null && reply.Challenges.Length != 0)
					{
						daily = reply.Challenges;
					}
					main.Apply(reply.Xp, reply.Teeth, reply.Wins, reply.Completed);
				}
			}, delegate(PlayFabError error)
			{
				Debug.LogWarning("Account round failed: " + error.GenerateErrorReport());
			});
		}

		public static int Cost(string item)
		{
			if (string.IsNullOrEmpty(item))
			{
				return 0;
			}
			if (!Costs.TryGetValue(item, out var cost))
			{
				return 0;
			}
			return cost;
		}

		public static bool Owned(string item)
		{
			if (item == Crown)
			{
				return Champion;
			}
			if (Cost(item) == 0)
			{
				return true;
			}
			return owned.Contains(item);
		}

		public static void Unlock(string item, Action<bool> done)
		{
			int cost = Cost(item);
			if (!main || unlocking || cost < 1 || owned.Contains(item))
			{
				done?.Invoke(owned.Contains(item));
				return;
			}
			if (Practice)
			{
				bool ok = Teeth >= cost;
				if (ok)
				{
					Teeth -= cost;
					owned.Add(item);
					PlayerPrefs.SetInt("practiceTeeth", Teeth);
					PlayerPrefs.SetString("practiceOwned", string.Join(",", owned));
					PlayerPrefs.Save();
				}
				done?.Invoke(ok);
				return;
			}
			if (!Ready)
			{
				done?.Invoke(obj: false);
				return;
			}
			unlocking = true;
			PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
			{
				CatalogVersion = "Cosmetics",
				ItemId = Id(item),
				VirtualCurrency = "GT",
				Price = cost
			}, delegate
			{
				unlocking = false;
				owned.Add(item);
				Teeth = Mathf.Max(0, Teeth - cost);
				PlayerPrefs.SetString("owned", string.Join(",", owned));
				PlayerPrefs.Save();
				Wallet();
				done?.Invoke(obj: true);
			}, delegate(PlayFabError error)
			{
				unlocking = false;
				Debug.LogWarning("Account purchase of " + item + " failed: " + error.GenerateErrorReport());
				if (error.Error == PlayFabErrorCode.WrongPrice || error.Error == PlayFabErrorCode.ItemNotFound)
				{
					Catalog();
				}
				Wallet();
				done?.Invoke(obj: false);
			});
		}

		private static string Id(string item)
		{
			if (!ids.TryGetValue(item, out var id))
			{
				return item.Replace(" ", "");
			}
			return id;
		}

		private static void Catalog()
		{
			PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
			{
				CatalogVersion = "Cosmetics"
			}, delegate(GetCatalogItemsResult result)
			{
				if (result.Catalog == null || result.Catalog.Count == 0)
				{
					return;
				}
				Costs.Clear();
				ids.Clear();
				foreach (CatalogItem item in result.Catalog)
				{
					if (item.VirtualCurrencyPrices != null && item.VirtualCurrencyPrices.TryGetValue("GT", out var price))
					{
						string name = (string.IsNullOrEmpty(item.DisplayName) ? item.ItemId : item.DisplayName);
						Costs[name] = (int)price;
						ids[name] = item.ItemId;
					}
				}
			}, delegate(PlayFabError error)
			{
				Debug.LogWarning("Account catalog failed: " + error.GenerateErrorReport());
			});
		}

		private static void Owns(List<ItemInstance> inventory)
		{
			if (inventory == null)
			{
				return;
			}
			owned.Clear();
			foreach (ItemInstance item in inventory)
			{
				if (!(item.CatalogVersion != "Cosmetics"))
				{
					string name = (string.IsNullOrEmpty(item.DisplayName) ? item.ItemId : item.DisplayName);
					foreach (KeyValuePair<string, string> pair in ids)
					{
						if (pair.Value == item.ItemId)
						{
							name = pair.Key;
						}
					}
					owned.Add(name);
				}
			}
			PlayerPrefs.SetString("owned", string.Join(",", owned));
			PlayerPrefs.Save();
		}

		private static void Load(string list)
		{
			owned.Clear();
			if (string.IsNullOrEmpty(list))
			{
				return;
			}
			string[] items = list.Split(',', StringSplitOptions.None);
			foreach (string item in items)
			{
				if (item.Length > 0)
				{
					owned.Add(item);
				}
			}
		}

		private static void Keep()
		{
			string key = "kept2" + PlayFabSettings.staticPlayer.PlayFabId;
			if (PlayerPrefs.GetInt(key, 0) == 1)
			{
				return;
			}
			List<string> missing = new List<string>();
			string[] outfit = Prefs.outfit;
			foreach (string item in outfit)
			{
				if (Cost(item) >= 1 && !owned.Contains(item))
				{
					missing.Add(item);
				}
			}
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "keep",
				FunctionParameter = new Dictionary<string, object> {
				{
					"Items",
					string.Join(",", missing)
				} }
			}, delegate(ExecuteCloudScriptResult result)
			{
				if (result.Error == null)
				{
					UnlockReply reply = JsonUtility.FromJson<UnlockReply>(PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).SerializeObject(result.FunctionResult) ?? "{}");
					if (reply != null)
					{
						Load(reply.Owned);
						PlayerPrefs.SetString("owned", reply.Owned ?? "");
						PlayerPrefs.SetInt(key, 1);
						PlayerPrefs.Save();
					}
				}
			}, delegate(PlayFabError error)
			{
				Debug.LogWarning("Account keep failed: " + error.GenerateErrorReport());
			});
		}

		private static void Crowned(List<Row> wins)
		{
			if (wins != null)
			{
				championKnown = true;
				Champion = wins.Count > 0 && wins[0].me && wins[0].score > 0;
				Strip();
			}
		}

		public static void Strip()
		{
			if (championKnown && !Champion && Prefs.outfit[0] == Crown)
			{
				Prefs.outfit[0] = "";
				Prefs.Save();
				if ((bool)Player.main && Player.main.cosmetics.worn[0] == Crown)
				{
					Player.main.Wear(0, "");
				}
			}
		}

		public static string Price(int pack)
		{
			if (pack < 0 || pack >= Packs.Length)
			{
				return "";
			}
			if (Practice)
			{
				return PracticePrices[pack];
			}
			if (!string.IsNullOrEmpty(prices[pack]))
			{
				return prices[pack];
			}
			return "...";
		}

		public static void OpenShop()
		{
			if (buying)
			{
				return;
			}
			if (Practice)
			{
				ShopStatus = "PRACTICE SHOP - NO REAL MONEY";
				return;
			}
			if (!Ready)
			{
				ShopStatus = (Status.StartsWith("SIGNING") ? "SIGNING IN..." : "SHOP OFFLINE");
				return;
			}
			ShopStatus = "";
			if (Array.Exists(prices, string.IsNullOrEmpty))
			{
				Prices();
			}
			Restore();
		}

		public static void Buy(int pack)
		{
			if (!main || pack < 0 || pack >= Packs.Length || buying)
			{
				return;
			}
			if (Practice)
			{
				main.Bought(PackTeeth[pack], PackTeeth[pack] + Teeth);
				PlayerPrefs.SetInt("practiceTeeth", Teeth);
				PlayerPrefs.Save();
				return;
			}
			if (!Ready || string.IsNullOrEmpty(metaUser))
			{
				ShopStatus = "SHOP OFFLINE";
				return;
			}
			Request<Purchase> request = IAP.LaunchCheckoutFlow(Packs[pack]);
			if (request == null)
			{
				ShopStatus = "SHOP OFFLINE";
				return;
			}
			buying = true;
			ShopStatus = "OPENING STORE...";
			request.OnComplete(async delegate(Message<Purchase> message)
			{
				if (message.IsError)
				{
					string why = message.GetError().Message ?? "";
					bool cancelled = why.Contains("user_canceled");
					if (!cancelled)
					{
						Debug.LogWarning("Shop checkout " + Packs[pack] + " failed: " + why);
					}
					buying = false;
					ShopStatus = (cancelled ? "PURCHASE CANCELLED" : "PURCHASE FAILED");
					Restore();
				}
				else
				{
					await Claim((message.Data != null && !string.IsNullOrEmpty(message.Data.Sku)) ? message.Data.Sku : Packs[pack]);
					buying = false;
				}
			});
		}

		private static void Prices()
		{
			Request<ProductList> request = IAP.GetProductsBySKU(Packs);
			if (request == null)
			{
				return;
			}
			request.OnComplete(delegate(Message<ProductList> message)
			{
				if (message.IsError || message.Data == null)
				{
					Debug.LogWarning("Shop prices failed: " + (message.IsError ? message.GetError().Message : "no products"));
					return;
				}
				foreach (Product product in message.Data)
				{
					int i = Array.IndexOf(Packs, product.Sku);
					if (i >= 0)
					{
						prices[i] = product.FormattedPrice;
					}
				}
			});
		}

		private static async void Restore()
		{
			if (!Ready || Practice || buying || string.IsNullOrEmpty(metaUser))
			{
				return;
			}
			Request<PurchaseList> request = IAP.GetViewerPurchases();
			if (request == null)
			{
				return;
			}
			buying = true;
			try
			{
				PurchaseList purchases = await Meta(request, "get purchases");
				List<string> skus = new List<string>();
				foreach (Purchase purchase in purchases)
				{
					if (purchase != null && Array.IndexOf(Packs, purchase.Sku) >= 0)
					{
						skus.Add(purchase.Sku);
					}
				}
				foreach (string sku in skus)
				{
					await Claim(sku);
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning("Shop restore failed: " + e.Message);
			}
			buying = false;
		}

		private static async Task Claim(string sku)
		{
			if (Array.IndexOf(Packs, sku) < 0 || !main)
			{
				return;
			}
			ShopStatus = "ADDING TEETH...";
			string token = "";
			string nonce = "";
			string why = "";
			token = (await Meta(Users.GetAccessToken(), "get access token")) ?? "";
			UserProof proof = await Meta(Users.GetUserProof(), "get nonce");
			nonce = ((proof == null) ? "" : (proof.Value ?? ""));
			TaskCompletionSource<BuyReply> done = new TaskCompletionSource<BuyReply>();
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
			{
				FunctionName = "buy",
				FunctionParameter = new Dictionary<string, object>
				{
					{ "Sku", sku },
					{ "UserId", metaUser },
					{ "Token", token },
					{ "Nonce", nonce }
				}
			}, delegate(ExecuteCloudScriptResult result)
			{
				if (result.Error != null)
				{
					why = "script " + result.Error.Error;
					Debug.LogWarning("Shop buy script failed: " + result.Error.Error + " " + result.Error.Message);
					done.TrySetResult(null);
				}
				else
				{
					done.TrySetResult(JsonUtility.FromJson<BuyReply>(PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).SerializeObject(result.FunctionResult) ?? "{}"));
				}
			}, delegate(PlayFabError error)
			{
				why = "server " + error.Error;
				Debug.LogWarning("Shop buy failed: " + error.GenerateErrorReport());
				done.TrySetResult(null);
			});
			BuyReply reply = await done.Task;
			if (reply != null)
			{
				if (reply.Ok && (bool)main)
				{
					claimAt = -1f;
					main.Bought(reply.Amount, reply.Teeth);
					return;
				}
				if (!string.IsNullOrEmpty(reply.Error))
				{
					why = reply.Error;
					Debug.LogWarning("Shop buy refused " + sku + ": " + reply.Error);
				}
			}
			claimTries++;
			claimAt = Time.realtimeSinceStartup + Mathf.Min(Mathf.Pow(2f, claimTries - 1) * 5f, 60f);
			ShopStatus = "TEETH PENDING, RETRYING\n<size=70%>" + Reason(why) + "</size>";
			Wallet();
		}

		private static string Reason(string why)
		{
			if (string.IsNullOrEmpty(why))
			{
				return "";
			}
			string reason = why.ToUpperInvariant().Replace("<", "").Replace(">", "");
			if (reason.Length <= 48)
			{
				return reason;
			}
			return reason.Substring(0, 48);
		}

		private static void Wallet()
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				if (result.VirtualCurrency != null && result.VirtualCurrency.TryGetValue("GT", out var teeth))
				{
					Teeth = teeth;
				}
				Owns(result.Inventory);
			}, delegate(PlayFabError error)
			{
				Debug.LogWarning("Account inventory failed: " + error.GenerateErrorReport());
			});
		}

		public static void Purchased()
		{
			if ((bool)main)
			{
				main.Play(main.purchaseSound, 1f);
				main.Play(main.teethSound, 1f);
			}
		}

		private void Bought(int amount, int balance)
		{
			Teeth = Mathf.Max(0, balance);
			ShopStatus = "THANKS! +" + amount.ToString("N0", CultureInfo.InvariantCulture) + " GOLD TEETH";
			Play(purchaseSound, 1f);
			Fireworks();
			Shower(Mathf.Clamp(amount / 50, 20, 40));
		}

		private void Apply(int xp, int teeth, int wins, string[] completed)
		{
			bool notes = completed != null && completed.Length != 0;
			bool gained = xp > Xp || teeth > Teeth;
			if ((gained || notes) && matchFrom < 0)
			{
				matchFrom = Xp;
				matchTeethFrom = Teeth;
			}
			if (notes)
			{
				doneNotes.AddRange(completed);
			}
			Xp = Mathf.Max(Xp, xp);
			Teeth = teeth;
			Wins = wins;
		}

		private void Update()
		{
			if (claimAt > 0f && Time.realtimeSinceStartup >= claimAt && !buying)
			{
				claimAt = -1f;
				Restore();
			}
			Pop();
			Fill();
			Board();
			DailyBoard();
			Spin();
			Fly();
		}

		public static void Spray(Transform around, int count, bool loud)
		{
			if (!main || !main.toothModel || !around)
			{
				return;
			}
			if (loud && (bool)main.teethSound)
			{
				AudioSource.PlayClipAtPoint(main.teethSound, around.position, main.volume * Prefs.sfx);
			}
			if (main.toothScale <= 0f)
			{
				GameObject probe = Instantiate(main.toothModel);
				Renderer shape = probe.GetComponentInChildren<Renderer>();
				float big = (shape ? Mathf.Max(shape.bounds.size.x, shape.bounds.size.y, shape.bounds.size.z) : 0f);
				main.toothScale = ((big > 0f) ? (main.toothSize / big) : 1f);
				Destroy(probe);
			}
			float now = Time.time;
			for (int i = 0; i < count; i++)
			{
				GameObject tooth = Instantiate(main.toothModel, around.position + Random.insideUnitSphere * 0.08f, Random.rotation);
				Renderer[] renderers = tooth.GetComponentsInChildren<Renderer>();
				foreach (Renderer r in renderers)
				{
					if ((bool)main.toothMaterial)
					{
						r.sharedMaterial = main.toothMaterial;
					}
					r.shadowCastingMode = ShadowCastingMode.Off;
				}
				Vector2 dir = Random.insideUnitCircle.normalized;
				float outward = Random.Range(1.4f, 2.8f);
				main.flying.Add(new Flyer
				{
					tooth = tooth.transform,
					around = around,
					centre = around.position,
					size = tooth.transform.localScale * main.toothScale,
					velocity = new Vector3(dir.x * outward, Random.Range(2.6f, 4.2f), dir.y * outward),
					spin = Random.onUnitSphere * Random.Range(360f, 900f),
					angle = ((float)i + Random.value * 0.6f) / (float)count * Mathf.PI * 2f,
					speed = Random.Range(2.6f, 4.2f),
					radius = Random.Range(0.55f, 0.9f),
					height = Random.Range(-0.7f, 0.9f),
					born = now + (float)i * 0.012f,
					landed = -1f
				});
				tooth.transform.localScale = Vector3.zero;
			}
		}

		private void Fly()
		{
			float now = Time.time;
			float dt = Time.deltaTime;
			for (int i = flying.Count - 1; i >= 0; i--)
			{
				Flyer f = flying[i];
				if (!f.tooth)
				{
					flying.RemoveAt(i);
					continue;
				}
				float age = now - f.born;
				if (age < 0f)
				{
					continue;
				}
				if ((bool)f.around)
				{
					f.centre = f.around.position;
				}
				Vector3 pos = f.tooth.position;
				float scale;
				if (age < BurstTime)
				{
					pos += f.velocity * dt;
					f.velocity += Physics.gravity * dt;
					scale = Mathf.Clamp01(age / 0.12f);
				}
				else if (age < SwirlTime)
				{
					f.angle += dt * f.speed;
					Vector3 target = f.centre + new Vector3(Mathf.Cos(f.angle) * f.radius, 0f, Mathf.Sin(f.angle) * f.radius) + Vector3.up * (f.height + Mathf.Sin(f.angle + age * 4f) * 0.12f);
					pos = Vector3.Lerp(pos, target, 1f - Mathf.Exp(-7f * dt));
					scale = 1f;
				}
				else
				{
					if (f.landed >= 0f)
					{
						float lay = now - f.landed;
						f.tooth.rotation = Quaternion.Slerp(f.tooth.rotation, f.lie, 1f - Mathf.Exp(-12f * dt));
						float shrink = 1f - Mathf.Clamp01((lay - LieTime) / ShrinkTime);
						f.tooth.localScale = f.size * shrink;
						if (shrink <= 0f)
						{
							Destroy(f.tooth.gameObject);
							flying.RemoveAt(i);
						}
						else
						{
							flying[i] = f;
						}
						continue;
					}
					if (!f.dropped)
					{
						Vector3 side = new Vector3(Mathf.Cos(f.angle), 0f, Mathf.Sin(f.angle));
						Vector3 along = new Vector3(0f - side.z, 0f, side.x) * f.speed * f.radius * 0.6f;
						f.velocity = along + side * Random.Range(0.3f, 1.2f) + Vector3.up * Random.Range(0.5f, 1.6f);
						f.dropped = true;
					}
					f.velocity += Physics.gravity * dt;
					Vector3 step = f.velocity * dt;
					if (Physics.Raycast(pos, step.normalized, out var hit, step.magnitude + 0.03f, Ground, QueryTriggerInteraction.Ignore))
					{
						pos = hit.point + hit.normal * 0.02f;
						if (f.bounces < 2 && f.velocity.magnitude > 1.2f)
						{
							f.bounces++;
							f.velocity = Vector3.Reflect(f.velocity, hit.normal) * 0.35f;
							f.spin *= 0.5f;
						}
						else
						{
							Quaternion rest = (toothModel ? toothModel.transform.rotation : Quaternion.identity);
							f.lie = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * Quaternion.Euler(0f, 0f, 90f) * rest;
							f.landed = now;
						}
					}
					else
					{
						pos += step;
					}
					scale = 1f;
					if (age > SwirlTime + FallLimit)
					{
						Destroy(f.tooth.gameObject);
						flying.RemoveAt(i);
						continue;
					}
				}
				f.tooth.SetPositionAndRotation(pos, f.tooth.rotation * Quaternion.Euler(f.spin * dt));
				f.tooth.localScale = f.size * scale;
				flying[i] = f;
			}
		}

		private void Pop()
		{
			Player player = Player.main;
			GameManager gm = GameManager.main;
			if (!player || !player.cam || !gm || gm.Showing || SceneManager.GetActiveScene().name != gm.lobbyScene)
			{
				lobbyAt = -1f;
				return;
			}
			if (lobbyAt < 0f)
			{
				lobbyAt = Time.time;
			}
			if ((matchFrom < 0 && prizes.Count == 0) || Time.time < lobbyAt + LobbyWait || popAlpha > 0f || !Popup(gm))
			{
				return;
			}
			shownFrom = ((matchFrom >= 0) ? matchFrom : Xp);
			shownTo = Xp;
			matchFrom = -1;
			shownLevel = LevelOf(shownFrom, out _, out _);
			int level = LevelOf(shownTo, out _, out _);
			teethGain = (level - shownLevel) * TeethPerLevel;
			beats.Clear();
			foreach (string name in doneNotes)
			{
				Challenge found = ((daily != null) ? Array.Find(daily, (Challenge c) => c.Name == name) : null);
				beats.Add(found ?? new Challenge
				{
					Name = name,
					Goal = 1,
					Reward = 0
				});
			}
			doneNotes.Clear();
			beats.AddRange(prizes);
			prizes.Clear();
			beat = null;
			beatAt = -1f;
			beatFillAt = -1f;
			leveled = false;
			popTitle.text = "";
			popTeeth.text = "";
			popNote.text = "";
			if (shownTo > shownFrom)
			{
				int gain = shownTo - shownFrom;
				nextTap = shownFrom;
				tapStep = Mathf.Max(1, gain / 16);
				showAt = Time.time + FillDelay;
				hideAt = showAt + FillTime + PopHold;
				popGain.text = "+" + gain + " XP";
				Paint(shownFrom);
			}
			else
			{
				showAt = -1f;
				beatAt = Time.time + FillDelay;
				hideAt = beatAt + PopHold;
				popGain.text = "";
				popLevel.text = "";
				Bar(0f);
			}
			Place(snap: true);
		}

		private void Fill()
		{
			if (!popup)
			{
				return;
			}
			float time = Time.time;
			float want = ((showAt >= 0f || beatAt >= 0f || beatFillAt >= 0f || time < hideAt) ? 1f : 0f);
			popAlpha = Mathf.MoveTowards(popAlpha, want, Time.deltaTime / PopFade);
			if (popup.gameObject.activeSelf != popAlpha > 0f)
			{
				popup.gameObject.SetActive(popAlpha > 0f);
			}
			if (popAlpha <= 0f)
			{
				return;
			}
			Place(snap: false);
			if (showAt >= 0f && time >= showAt)
			{
				float t = Mathf.Clamp01((time - showAt) / FillTime);
				int xp = Mathf.RoundToInt(Mathf.Lerp(shownFrom, shownTo, 1f - (1f - t) * (1f - t)));
				if (xp >= nextTap && xp < shownTo)
				{
					nextTap = xp + tapStep;
					LevelOf(xp, out var into, out var need);
					Tap((float)into / (float)need);
				}
				int level = LevelOf(xp, out _, out _);
				if (level > shownLevel)
				{
					shownLevel = level;
					leveled = true;
					Celebrate("LEVEL UP!");
				}
				Paint(xp);
				if (t >= 1f)
				{
					showAt = -1f;
					if (beats.Count > 0)
					{
						beatAt = time + (leveled ? 2.6f : FillDelay);
						hideAt = Mathf.Max(hideAt, beatAt + PopHold);
					}
				}
			}
			if (beatAt >= 0f && time >= beatAt)
			{
				beatAt = -1f;
				if (beats.Count > 0)
				{
					beat = beats[0];
					beats.RemoveAt(0);
					popTitle.text = "";
					popTeeth.text = "";
					popLevel.text = "";
					popNote.text = beat.Name;
					popGain.text = (string.IsNullOrEmpty(beat.Title) ? (Mathf.Max(beat.Goal - 1, 0) + " / " + beat.Goal) : "");
					Bar((beat.Goal >= 2) ? ((float)(beat.Goal - 1) / (float)beat.Goal) : 0f);
					nextTap = 0;
					beatFillAt = time + 0.4f;
					hideAt = Mathf.Max(hideAt, beatFillAt + BeatFill + PopHold + 0.7f);
				}
			}
			if (beatFillAt >= 0f && time >= beatFillAt && beat != null)
			{
				float t2 = Mathf.Clamp01((time - beatFillAt) / BeatFill);
				float from = ((beat.Goal >= 2) ? ((float)(beat.Goal - 1) / (float)beat.Goal) : 0f);
				float fill = Mathf.Lerp(from, 1f, 1f - (1f - t2) * (1f - t2));
				int tick = Mathf.FloorToInt(t2 * 10f);
				if (t2 < 1f && tick >= nextTap)
				{
					nextTap = tick + 1;
					Tap(fill);
				}
				Bar(fill);
				if (t2 >= 1f)
				{
					beatFillAt = -1f;
					if (string.IsNullOrEmpty(beat.Title))
					{
						popGain.text = beat.Goal + " / " + beat.Goal;
					}
					teethGain = beat.Reward;
					Celebrate(string.IsNullOrEmpty(beat.Title) ? "CHALLENGE COMPLETED!" : beat.Title);
					if (beats.Count > 0)
					{
						beatAt = time + 2.6f;
						hideAt = Mathf.Max(hideAt, beatAt + PopHold);
					}
				}
			}
			if (punchAt >= 0f)
			{
				float since = time - punchAt;
				popTitle.transform.localScale = Vector3.one * (1f + Mathf.Exp(-5f * since) * 0.5f * Mathf.Abs(Mathf.Cos(since * 12f)));
				if (since > 1.4f)
				{
					punchAt = -1f;
				}
			}
			if (teethAt >= 0f && time >= teethAt)
			{
				teethAt = -1f;
				if (teethGain > 0)
				{
					popTeeth.text = "+" + teethGain + " GOLD TEETH";
					Shower(Mathf.Clamp(teethGain / 5, 12, 40));
				}
			}
			popTrack.SetColor("_Color", new Color(Track.r, Track.g, Track.b, popAlpha * 0.85f));
			popFill.SetColor("_Color", new Color(Gold.r, Gold.g, Gold.b, popAlpha));
			popTitle.alpha = popAlpha;
			popLevel.alpha = popAlpha;
			popGain.alpha = popAlpha;
			popTeeth.alpha = popAlpha;
			popNote.alpha = popAlpha;
		}

		private void Tap(float progress)
		{
			if ((bool)fillSound && (bool)tapSource)
			{
				tapSource.pitch = 1f + Mathf.Clamp01(progress) * 0.6f;
				tapSource.PlayOneShot(fillSound, volume * Prefs.sfx);
			}
		}

		private void Place(bool snap)
		{
			Player player = Player.main;
			if ((bool)player && (bool)player.cam)
			{
				Vector3 forward = player.cam.forward;
				Vector3 flat = new Vector3(forward.x, 0f, forward.z);
				flat = ((flat.sqrMagnitude > 0.0001f) ? flat.normalized : Vector3.forward);
				Vector3 at = player.cam.position + flat * PopDistance + Vector3.down * 0.12f;
				float t = (snap ? 1f : (1f - Mathf.Exp(Time.deltaTime * -4f)));
				popup.SetPositionAndRotation(Vector3.Lerp(popup.position, at, t), Quaternion.Slerp(popup.rotation, Quaternion.LookRotation(flat), t));
			}
		}

		private void Paint(int xp)
		{
			int level = LevelOf(xp, out var into, out var need);
			popLevel.text = Badge(level);
			Bar((float)into / (float)need);
		}

		private void Bar(float fraction)
		{
			fraction = Mathf.Clamp01(fraction);
			popFillT.localScale = new Vector3(Mathf.Max(fraction, 0.0001f), BarHeight, 1f);
			popFillT.localPosition = new Vector3(fraction * 0.5f - 0.5f, 0f, -0.001f);
		}

		private void Celebrate(string title)
		{
			popTitle.text = title;
			punchAt = Time.time;
			teethAt = Time.time + 0.7f;
			hideAt = Mathf.Max(hideAt, Time.time + PopHold + 0.7f);
			Play(levelSound, 1f);
			Fireworks();
		}

		private void Shower(int count)
		{
			Play(teethSound, 1f);
			Player player = Player.main;
			if ((bool)player && (bool)player.chest)
			{
				Spray(player.chest.transform, count, loud: false);
			}
			NetworkPlayer mine = NetworkPlayer.mine;
			if ((bool)mine && (bool)mine.Object && mine.Object.IsValid && (bool)mine.Runner && mine.Runner.IsRunning)
			{
				mine.RPC_Teeth(count);
			}
		}

		private void Fireworks()
		{
			Player player = Player.main;
			if ((bool)player && (bool)player.cam)
			{
				Vector3 head = player.cam.position;
				Vector3 forward = player.cam.forward;
				Vector3 flat = new Vector3(forward.x, 0f, forward.z);
				flat = ((flat.sqrMagnitude > 0.001f) ? flat.normalized : Vector3.forward);
				Vector3 side = Vector3.Cross(Vector3.up, flat);
				Effect(levelBurst, head + Vector3.down * 0.5f, 1f, 4f);
				Effect(levelFireworks, head + flat * 4f + Vector3.up * 2.5f + side * 1.5f, 1f, 6f);
				Effect(levelFireworks, head + flat * 4.5f + Vector3.up * 3f - side * 1.5f, 1.2f, 6f);
			}
		}

		private static void Effect(GameObject prefab, Vector3 at, float scale, float life)
		{
			if ((bool)prefab)
			{
				GameObject effect = Instantiate(prefab, at, Quaternion.identity);
				effect.transform.localScale = Vector3.one * scale;
				Destroy(effect, life);
			}
		}

		private void Play(AudioClip clip, float loud)
		{
			if ((bool)clip && (bool)audioSource)
			{
				audioSource.PlayOneShot(clip, volume * loud * Prefs.sfx);
			}
		}

		private bool Popup(GameManager gm)
		{
			if ((bool)popup)
			{
				return true;
			}
			if (!gm.title || !gm.subtitle)
			{
				return false;
			}
			popup = new GameObject("Level Up").transform;
			popup.SetParent(transform, worldPositionStays: false);
			Shader shader = ((gm.logo && (bool)gm.logo.sharedMaterial) ? gm.logo.sharedMaterial.shader : Shader.Find("Knuckleheads/LoadingLogo"));
			popTrack = new Material(shader)
			{
				renderQueue = 4006
			};
			popFill = new Material(shader)
			{
				renderQueue = 4007
			};
			Quad(popup, popTrack, new Vector3(1.03f, 0.1f, 1f), Vector3.zero);
			popFillT = Quad(popup, popFill, new Vector3(BarWidth, BarHeight, 1f), new Vector3(0f, 0f, -0.001f));
			popTitle = Label(popup, gm.title, new Vector3(0f, 0.2f, -0.002f), TextAlignmentOptions.Center, 2.2f);
			popLevel = Label(popup, gm.subtitle, new Vector3(-0.55f, 0f, -0.002f), TextAlignmentOptions.Right, 0.8f);
			popGain = Label(popup, gm.subtitle, new Vector3(0.55f, 0f, -0.002f), TextAlignmentOptions.Left, 0.8f);
			popTeeth = Label(popup, gm.subtitle, new Vector3(0f, -0.15f, -0.002f), TextAlignmentOptions.Center, 0.7f);
			popNote = Label(popup, gm.subtitle, new Vector3(0f, -0.27f, -0.002f), TextAlignmentOptions.Center, 0.55f);
			popTitle.color = Gold;
			popTeeth.color = Gold;
			popNote.color = new Color(0.55f, 0.95f, 1f, 1f);
			popup.gameObject.SetActive(value: false);
			return true;
		}

		private static Transform Quad(Transform parent, Material material, Vector3 scale, Vector3 at)
		{
			GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			Destroy(quad.GetComponent<Collider>());
			quad.transform.SetParent(parent, worldPositionStays: false);
			quad.transform.localPosition = at;
			quad.transform.localScale = scale;
			Renderer renderer = quad.GetComponent<Renderer>();
			renderer.sharedMaterial = material;
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			return quad.transform;
		}

		private static TextMeshPro Label(Transform parent, TextMeshPro like, Vector3 at, TextAlignmentOptions align, float size)
		{
			GameObject go = new GameObject("Label");
			go.transform.SetParent(parent, worldPositionStays: false);
			TextMeshPro label = go.AddComponent<TextMeshPro>();
			label.font = like.font;
			label.fontSharedMaterial = like.fontSharedMaterial;
			label.fontSize = size;
			label.alignment = align;
			label.textWrappingMode = TextWrappingModes.NoWrap;
			label.overflowMode = TextOverflowModes.Overflow;
			label.rectTransform.pivot = new Vector2((align == TextAlignmentOptions.Right) ? 1f : ((align == TextAlignmentOptions.Left) ? 0f : 0.5f), 0.5f);
			label.rectTransform.sizeDelta = new Vector2(1.2f, 0.12f);
			label.transform.localPosition = at;
			label.text = "";
			return label;
		}

		private void Loaded(Scene scene, LoadSceneMode mode)
		{
			GameObject leaderboard = GameObject.Find("Leaderboard");
			Transform ui = (leaderboard ? leaderboard.transform.Find("UI") : null);
			board = (ui ? ui : (leaderboard ? leaderboard.transform : null));
			if (!board)
			{
				return;
			}
			levelNames = Texts("Levels", "Name");
			levelValues = Texts("Levels", "Value");
			winNames = Texts("Wins", "Name");
			winValues = Texts("Wins", "Value");
			boardStatus = Text("Status");
			boardHeader = Text("Header");
			boardLevel = Text("You/Level");
			boardXp = Text("You/Xp");
			boardTeeth = Text("You/Teeth");
			Transform fill = board.Find("You/Bar/Fill");
			boardFill = (fill ? fill.GetComponent<Image>() : null);
			boardTooth = board.Find("You/Tooth");
			fetchAt = ((Ready || Status != "SIGNING IN...") ? 0f : (-1f));
			GameObject rewards = GameObject.Find("Rewards");
			rewardCrown = (rewards ? rewards.transform.Find("Crown") : null);
			if ((bool)rewardCrown)
			{
				rewardHome = rewardCrown.localPosition;
			}
			rewardNames = new TMP_Text[3];
			for (int i = 0; i < rewardNames.Length; i++)
			{
				rewardNames[i] = (rewards ? Find<TMP_Text>(rewards.transform, "UI/Rows/Row " + (i + 1) + "/Name") : null);
			}
			Show(null, null);
			GameObject challenges = GameObject.Find("Challenges");
			dailyBoard = (challenges ? challenges.transform.Find("UI") : null);
			dailyShown = -2;
			dailyKey = "";
			if ((bool)dailyBoard)
			{
				dailyStatus = Find<TMP_Text>(dailyBoard, "Status");
				dailyResets = Find<TMP_Text>(dailyBoard, "Resets");
				dailyNames = new TMP_Text[3];
				dailyCounts = new TMP_Text[3];
				dailyRewards = new TMP_Text[3];
				dailyFills = new Image[3];
				dailyDone = new GameObject[3];
				dailyOpen = new GameObject[3];
				for (int j = 0; j < 3; j++)
				{
					string row = "Rows/Row " + (j + 1) + "/";
					dailyNames[j] = Find<TMP_Text>(dailyBoard, row + "Name");
					dailyCounts[j] = Find<TMP_Text>(dailyBoard, row + "Count");
					dailyRewards[j] = Find<TMP_Text>(dailyBoard, row + "Reward");
					dailyFills[j] = Find<Image>(dailyBoard, row + "Bar/Fill");
					Transform done = dailyBoard.Find(row + "Done");
					Transform open = dailyBoard.Find(row + "Open");
					dailyDone[j] = (done ? done.gameObject : null);
					dailyOpen[j] = (open ? open.gameObject : null);
				}
			}
		}

		private static T Find<T>(Transform root, string path) where T : Component
		{
			Transform found = root.Find(path);
			if (!found)
			{
				return null;
			}
			return found.GetComponent<T>();
		}

		private static void Daily()
		{
			if (Practice)
			{
				if (daily == null)
				{
					daily = Picks();
				}
			}
			else
			{
				if (dailyBusy)
				{
					return;
				}
				dailyBusy = true;
				PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
				{
					FunctionName = "challenges"
				}, delegate(ExecuteCloudScriptResult result)
				{
					dailyBusy = false;
					if (result.Error != null || result.FunctionResult == null)
					{
						return;
					}
					ChallengeReply reply = JsonUtility.FromJson<ChallengeReply>(PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).SerializeObject(result.FunctionResult) ?? "{}");
					if (reply != null)
					{
						if (reply.Challenges != null && reply.Challenges.Length != 0)
						{
							daily = reply.Challenges;
						}
						if (reply.Prize != null && reply.Prize.Reward >= 1)
						{
							if ((bool)main)
							{
								main.prizes.Add(reply.Prize);
							}
							Wallet();
						}
					}
				}, delegate(PlayFabError error)
				{
					dailyBusy = false;
					Debug.LogWarning("Account challenges failed: " + error.GenerateErrorReport());
				});
			}
		}

		private static Challenge[] Picks()
		{
			long day = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 86400;
			if (daily != null && dailyDay == day)
			{
				return daily;
			}
			dailyDay = day;
			int[] order = new int[ChallengeNames.Length];
			for (int i = 0; i < order.Length; i++)
			{
				order[i] = i;
			}
			long seed = (day * 9301 + 49297) % 233280;
			for (int j = order.Length - 1; j > 0; j--)
			{
				seed = (seed * 9301 + 49297) % 233280;
				int k = (int)(seed * (j + 1) / 233280);
				int swap = order[j];
				order[j] = order[k];
				order[k] = swap;
			}
			Challenge[] picks = new Challenge[3];
			for (int l = 0; l < 3; l++)
			{
				int pick = order[l];
				picks[l] = new Challenge
				{
					Name = ChallengeNames[pick],
					Goal = ChallengeGoals[pick],
					Reward = ChallengeRewards[pick]
				};
			}
			return picks;
		}

		private void DailyBoard()
		{
			if (!dailyBoard)
			{
				return;
			}
			TimeSpan left = DateTime.UtcNow.Date.AddDays(1.0) - DateTime.UtcNow;
			int minutes = (int)left.TotalMinutes;
			if (minutes != dailyShown)
			{
				dailyShown = minutes;
				if ((bool)dailyResets)
				{
					dailyResets.text = "NEW CHALLENGES IN " + ((left.Hours > 0) ? (left.Hours + "H ") : "") + left.Minutes + "M";
				}
			}
			StringBuilder key = new StringBuilder(Ready ? "R" : Status);
			if (daily != null)
			{
				for (int i = 0; i < daily.Length; i++)
				{
					Challenge c = daily[i];
					key.Append('|').Append(c.Name).Append(c.Progress)
						.Append(c.Done);
				}
			}
			if (key.ToString() == dailyKey)
			{
				return;
			}
			dailyKey = key.ToString();
			bool any = Ready && daily != null && daily.Length != 0;
			if ((bool)dailyStatus)
			{
				dailyStatus.text = ((!Ready) ? Status : ((daily == null) ? "LOADING..." : ""));
			}
			for (int j = 0; j < 3; j++)
			{
				Challenge challenge = ((any && j < daily.Length) ? daily[j] : null);
				if ((bool)dailyNames[j])
				{
					dailyNames[j].text = ((challenge != null) ? challenge.Name : "");
				}
				if ((bool)dailyCounts[j])
				{
					dailyCounts[j].text = ((challenge == null) ? "" : (challenge.Done ? "DONE!" : (challenge.Progress + " / " + challenge.Goal)));
				}
				if ((bool)dailyRewards[j])
				{
					dailyRewards[j].text = ((challenge == null) ? "" : ("+" + challenge.Reward));
				}
				if ((bool)dailyFills[j])
				{
					dailyFills[j].fillAmount = ((challenge != null && challenge.Goal >= 1) ? ((float)challenge.Progress / (float)challenge.Goal) : 0f);
				}
				if ((bool)dailyDone[j])
				{
					dailyDone[j].SetActive(challenge != null && challenge.Done);
				}
				if ((bool)dailyOpen[j])
				{
					dailyOpen[j].SetActive(challenge == null || !challenge.Done);
				}
			}
		}

		private TMP_Text Text(string path)
		{
			Transform found = board.Find(path);
			if (!found)
			{
				return null;
			}
			return found.GetComponent<TMP_Text>();
		}

		private TMP_Text[] Texts(string column, string part)
		{
			TMP_Text[] texts = new TMP_Text[10];
			for (int i = 0; i < texts.Length; i++)
			{
				texts[i] = Text(column + "/Row " + (i + 1) + "/" + part);
			}
			return texts;
		}

		private void Board()
		{
			if (!board)
			{
				return;
			}
			if ((bool)boardTooth)
			{
				boardTooth.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * 2.4f) * 10f);
			}
			LevelOf(Xp, out var into, out var need);
			if ((bool)boardLevel)
			{
				boardLevel.text = (Ready ? ("YOU  " + Badge(Level)) : "YOU");
			}
			if ((bool)boardXp)
			{
				boardXp.text = (Ready ? (into + " / " + need + " XP") : "");
			}
			if ((bool)boardTeeth)
			{
				boardTeeth.text = (Ready ? Teeth.ToString("N0") : "-");
			}
			if ((bool)boardFill)
			{
				boardFill.fillAmount = (Ready ? ((float)into / (float)need) : 0f);
			}
			if (!(fetchAt < 0f) && !(Time.time < fetchAt) && !fetching)
			{
				fetchAt = Time.time + BoardRefresh;
				Fetch();
				if (Ready)
				{
					Daily();
				}
			}
		}

		private void Fetch()
		{
			if (!Ready)
			{
				Show(null, null);
				return;
			}
			if (Practice)
			{
				List<Row> fakeWins = Fake(levels: false);
				Crowned(fakeWins);
				Show(Fake(levels: true), fakeWins);
				return;
			}
			fetching = true;
			List<Row> levels = null;
			List<Row> wins = null;
			int left = 2;
			Action done = delegate
			{
				if (--left <= 0)
				{
					fetching = false;
					Crowned(wins);
					if ((bool)this)
					{
						Show(levels, wins);
					}
				}
			};
			Ranks("xp", levels: true, delegate(List<Row> rows)
			{
				levels = rows;
				done();
			});
			Ranks("wins_week", levels: false, delegate(List<Row> rows)
			{
				wins = rows;
				done();
			});
		}

		private static void Ranks(string name, bool levels, Action<List<Row>> done)
		{
			PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
			{
				StatisticName = name,
				StartPosition = 0,
				MaxResultsCount = 10
			}, delegate(GetLeaderboardResult result)
			{
				List<Row> rows = new List<Row>();
				if (result.Leaderboard != null)
				{
					foreach (PlayerLeaderboardEntry entry in result.Leaderboard)
					{
						int score = entry.StatValue;
						rows.Add(new Row
						{
							name = Clean(string.IsNullOrEmpty(entry.DisplayName) ? "KNUCKLEHEAD" : entry.DisplayName),
							value = (levels ? Badge(LevelOf(score, out _, out _)) : (score + ((score == 1) ? " WIN" : " WINS"))),
							me = (entry.PlayFabId == PlayFabSettings.staticPlayer.PlayFabId),
							score = score
						});
					}
				}
				done(rows);
			}, delegate(PlayFabError error)
			{
				Debug.LogWarning("Account leaderboard " + name + " failed: " + error.GenerateErrorReport());
				done(null);
			});
		}

		private static List<Row> Fake(bool levels)
		{
			List<KeyValuePair<string, int>> scores = new List<KeyValuePair<string, int>>();
			int salt = 0;
			for (int i = 0; i < PracticeNames.Length; i++)
			{
				int hash = (salt + PracticeNames[i].Length * 7919) % 997;
				scores.Add(new KeyValuePair<string, int>(PracticeNames[i], levels ? (hash * 9 + 150) : (hash % 40 + 2)));
				salt += 104729;
			}
			scores.Add(new KeyValuePair<string, int>("", levels ? Xp : Wins));
			scores.Sort((KeyValuePair<string, int> a, KeyValuePair<string, int> b) => b.Value.CompareTo(a.Value));
			List<Row> rows = new List<Row>();
			for (int j = 0; j < scores.Count && rows.Count < 10; j++)
			{
				bool me = scores[j].Key == "";
				string name = ((!me) ? scores[j].Key : ((NetworkManager.MetaName != "") ? NetworkManager.MetaName : "YOU"));
				int score = scores[j].Value;
				rows.Add(new Row
				{
					name = name,
					value = (levels ? Badge(LevelOf(score, out _, out _)) : (score + ((score == 1) ? " WIN" : " WINS"))),
					me = me,
					score = score
				});
			}
			return rows;
		}

		private void Show(List<Row> levels, List<Row> wins)
		{
			if ((bool)board)
			{
				if ((bool)boardStatus)
				{
					boardStatus.text = ((!Ready) ? Status : ((levels == null && wins == null) ? "LEADERBOARDS OFFLINE" : ""));
				}
				if ((bool)boardHeader)
				{
					boardHeader.text = (Practice ? "PRACTICE BOARD" : "LEADERBOARDS");
				}
				Fill(levelNames, levelValues, levels);
				Fill(winNames, winValues, wins);
				Holders(wins);
			}
		}

		private void Holders(List<Row> wins)
		{
			if (rewardNames == null)
			{
				return;
			}
			for (int i = 0; i < rewardNames.Length; i++)
			{
				if ((bool)rewardNames[i])
				{
					rewardNames[i].text = ((wins != null && i < wins.Count && wins[i].score > 0) ? wins[i].name : "NOBODY YET");
				}
			}
		}

		private void Spin()
		{
			if ((bool)rewardCrown)
			{
				rewardCrown.localRotation = Quaternion.Euler(0f, Time.time * 50f, 0f);
				rewardCrown.localPosition = rewardHome + Vector3.up * (Mathf.Sin(Time.time * 1.6f) * 0.06f);
			}
		}

		private static void Fill(TMP_Text[] names, TMP_Text[] values, List<Row> rows)
		{
			for (int i = 0; i < names.Length; i++)
			{
				bool empty = rows == null || i >= rows.Count;
				string open = (empty ? "" : (rows[i].me ? "<color=#FFE14D>" : ""));
				string close = (empty ? "" : (rows[i].me ? "</color>" : ""));
				string rank = ((i < Medals.Length) ? ("<color=" + Medals[i] + ">" + (i + 1) + ".</color> ") : (i + 1 + ". "));
				if ((bool)names[i])
				{
					names[i].text = (empty ? "" : (rank + open + rows[i].name + close));
				}
				if ((bool)values[i])
				{
					values[i].text = (empty ? "" : rows[i].value);
				}
			}
		}
	}
}
