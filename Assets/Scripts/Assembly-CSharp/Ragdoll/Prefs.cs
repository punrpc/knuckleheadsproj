using System.Collections.Generic;
using UnityEngine;

namespace Ragdoll
{
	public static class Prefs
	{
		public static float master = 1f;

		public static float music = 1f;

		public static float sfx = 1f;

		public static bool armWalk;

		public static bool snapTurn;

		public static bool noTurn;

		public static bool noKick;

		public static bool haptics = true;

		public static bool swapSticks;

		public static bool realVoice;

		public static bool mic = true;

		public static int color;

		public static bool myBlabber;

		public static bool tutorialDone;

		public static string region = "cae";

		public static float stickDead = 0.25f;

		public static readonly string[] outfit = new string[4] { "", "", "", "" };

		private static bool loaded;

		private static readonly Dictionary<AudioSource, float> baseVolume = new Dictionary<AudioSource, float>();

		public static void Load()
		{
			if (loaded)
			{
				return;
			}
			loaded = true;
			master = PlayerPrefs.GetFloat("volume", 1f);
			music = PlayerPrefs.GetFloat("music", 1f);
			sfx = PlayerPrefs.GetFloat("sfx", 1f);
			armWalk = PlayerPrefs.GetInt("armWalk", 0) == 1;
			snapTurn = PlayerPrefs.GetInt("snapTurn", 0) == 1;
			noTurn = PlayerPrefs.GetInt("noTurn", 0) == 1;
			noKick = PlayerPrefs.GetInt("noKick", 0) == 1;
			haptics = PlayerPrefs.GetInt("haptics", 1) == 1;
			swapSticks = PlayerPrefs.GetInt("swapSticks", 0) == 1;
			realVoice = PlayerPrefs.GetInt("realVoice", 1) == 1;
			if (PlayerPrefs.GetInt("voiceDefault", 0) == 0)
			{
				realVoice = true;
				PlayerPrefs.SetInt("voiceDefault", 1);
				PlayerPrefs.SetInt("realVoice", 1);
				PlayerPrefs.Save();
			}
			mic = PlayerPrefs.GetInt("mic", 1) == 1;
			color = PlayerPrefs.GetInt("color", 0);
			myBlabber = PlayerPrefs.GetInt("myBlabber", 0) == 1;
			tutorialDone = PlayerPrefs.GetInt("tutorial", 0) == 1;
			region = PlayerPrefs.GetString("region", "cae");
			stickDead = PlayerPrefs.GetFloat("stickDead", 0.25f);
			for (int i = 0; i < outfit.Length; i++)
			{
				outfit[i] = PlayerPrefs.GetString("outfit" + i, "");
			}
		}

		public static void Save()
		{
			PlayerPrefs.SetFloat("volume", master);
			PlayerPrefs.SetFloat("music", music);
			PlayerPrefs.SetFloat("sfx", sfx);
			PlayerPrefs.SetInt("armWalk", armWalk ? 1 : 0);
			PlayerPrefs.SetInt("snapTurn", snapTurn ? 1 : 0);
			PlayerPrefs.SetInt("noTurn", noTurn ? 1 : 0);
			PlayerPrefs.SetInt("noKick", noKick ? 1 : 0);
			PlayerPrefs.SetInt("haptics", haptics ? 1 : 0);
			PlayerPrefs.SetInt("swapSticks", swapSticks ? 1 : 0);
			PlayerPrefs.SetInt("realVoice", realVoice ? 1 : 0);
			PlayerPrefs.SetInt("mic", mic ? 1 : 0);
			PlayerPrefs.SetInt("color", color);
			PlayerPrefs.SetInt("myBlabber", myBlabber ? 1 : 0);
			PlayerPrefs.SetInt("tutorial", tutorialDone ? 1 : 0);
			PlayerPrefs.SetString("region", region);
			PlayerPrefs.SetFloat("stickDead", stickDead);
			for (int i = 0; i < outfit.Length; i++)
			{
				PlayerPrefs.SetString("outfit" + i, outfit[i]);
			}
			PlayerPrefs.Save();
			Apply();
		}

		public static Vector2 Dead(Vector2 v, bool axes)
		{
			float d = Mathf.Clamp(stickDead, 0.05f, 0.6f);
			if (axes)
			{
				return new Vector2(Dead(v.x, d), Dead(v.y, d));
			}
			float m = Mathf.Min(v.magnitude, 1f);
			if (m <= d)
			{
				return Vector2.zero;
			}
			return v.normalized * Mathf.InverseLerp(d, 1f, m);
		}

		private static float Dead(float x, float d)
		{
			if (Mathf.Abs(x) <= d)
			{
				return 0f;
			}
			return Mathf.Sign(x) * Mathf.InverseLerp(d, 1f, Mathf.Abs(x));
		}

		public static void Apply()
		{
			Load();
			AudioListener.volume = master;
			foreach (AudioSource a in Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			{
				if (!baseVolume.TryGetValue(a, out float b))
				{
					b = a.volume;
					baseVolume[a] = b;
				}
				a.volume = b * (IsMusic(a) ? music : sfx);
			}
		}

		public static void Track(AudioSource a)
		{
			if (!a || baseVolume.ContainsKey(a))
			{
				return;
			}
			Load();
			float v = a.volume;
			baseVolume[a] = v;
			a.volume = v * (IsMusic(a) ? music : sfx);
		}

		public static bool IsMusic(AudioSource a)
		{
			if (a.gameObject.name.Contains("Music"))
			{
				return true;
			}
			if (!a.clip)
			{
				return false;
			}
			return a.clip.name.Contains("Music") || a.clip.name.Contains("OST");
		}

		public static float SnapAngle(float turnSpeed)
		{
			return Mathf.Clamp(turnSpeed * 0.25f, 15f, 90f);
		}
	}
}
