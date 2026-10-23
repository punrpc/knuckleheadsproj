using System.Linq;
using System.Text;
using Fusion;
using UnityEngine;
using UnityEngine.Networking;

namespace Ragdoll
{
	// Not part of the shipped game: posts player joins/leaves to a Discord webhook.
	// Each client reports only its own player, so every event is posted once.
	public static class DiscordLog
	{
		// Anyone who unpacks the APK can read this URL and post to the channel with it;
		// regenerate the webhook in Discord if that ever happens.
		private const string WebhookUrl = "https://discord.com/api/webhooks/1563205591269380159/wAizjC9tEVFu7QmwWzS-gO1AnHJK0nnMSJ7Hx53YkGxSSuXzyVEt6dkEwxECaxG4CJ3U";

		public static void Joined(NetworkRunner runner)
		{
			Send(":green_circle: **" + NetworkManager.MetaName + "** joined `" + Room(runner) + "` (" + Region(runner) + ", " + Players(runner) + " in room)");
		}

		public static void Left(NetworkRunner runner)
		{
			Send(":red_circle: **" + NetworkManager.MetaName + "** left `" + Room(runner) + "`");
		}

		private static string Room(NetworkRunner runner)
		{
			return runner && runner.SessionInfo.IsValid ? runner.SessionInfo.Name : "?";
		}

		private static string Region(NetworkRunner runner)
		{
			return runner && runner.SessionInfo.IsValid ? runner.SessionInfo.Region : Prefs.region;
		}

		private static int Players(NetworkRunner runner)
		{
			return runner && runner.IsRunning ? runner.ActivePlayers.Count() : 0;
		}

		private static void Send(string text)
		{
			Debug.Log("[DiscordLog] " + text);
			// allowed_mentions with no parse types keeps room names from pinging @everyone
			string json = "{\"content\":\"" + Escape(text) + "\",\"allowed_mentions\":{\"parse\":[]}}";
			UnityWebRequest request = new UnityWebRequest(WebhookUrl, "POST");
			request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 10;
			request.SendWebRequest().completed += _ =>
			{
				if (request.result != UnityWebRequest.Result.Success)
				{
					Debug.LogWarning("[DiscordLog] webhook failed: " + request.responseCode + " " + request.error);
				}
				request.Dispose();
			};
		}

		private static string Escape(string s)
		{
			StringBuilder sb = new StringBuilder(s.Length + 8);
			foreach (char c in s)
			{
				switch (c)
				{
				case '"': sb.Append("\\\""); break;
				case '\\': sb.Append("\\\\"); break;
				case '\n': sb.Append("\\n"); break;
				case '\r': break;
				default:
					if (c < ' ')
					{
						sb.Append("\\u").Append(((int)c).ToString("x4"));
					}
					else
					{
						sb.Append(c);
					}
					break;
				}
			}
			return sb.ToString();
		}
	}
}
