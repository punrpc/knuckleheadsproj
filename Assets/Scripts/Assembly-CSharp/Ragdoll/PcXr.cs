using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Ragdoll
{
	public class PcXr : MonoBehaviour
	{
		private const float Gap = 6f;

		private const int MaxTries = 25;

		private const string Help = "VR DIDN'T START\n\nPut your headset on and open SteamVR.\nIf it still doesn't work: SteamVR > Settings > OpenXR >\n\"Set SteamVR as OpenXR Runtime\", then restart the game.";

		private const float SteamGrace = 3f;

		private const float SteamMax = 60f;

		private const float PollGap = 0.5f;

		private int tries;

		private bool trying;

		private bool steam;

		private bool steamUp;

		private bool launched;

		private bool looked;

		private string steamJson;

		private float steamFrom = -1f;

		private float steamUpAt = -1f;

		private float polledAt = -9f;

		private float nextTry = 0.5f;

		private string shown = "";

		private GUIStyle style;

		private Camera spectator;

		private Camera spectatorFrom;

		private bool spectatorLive;

		private const float SpecFov = 95f;

		private const float SpecFollow = 25f;

		private const float SpecTurn = 8f;

		private const float SpecSnap = 2f;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Boot()
		{
			if (Application.platform != RuntimePlatform.WindowsPlayer)
			{
				return;
			}
			GameObject go = new GameObject("PC XR");
			Object.DontDestroyOnLoad(go);
			go.AddComponent<PcXr>();
		}

		private static bool Running()
		{
			XRGeneralSettings settings = XRGeneralSettings.Instance;
			XRManagerSettings manager = settings ? settings.Manager : null;
			if (!manager || !manager.activeLoader)
			{
				return false;
			}
			return XRSettings.isDeviceActive;
		}

		private void Update()
		{
			if (Running())
			{
				if (shown != "")
				{
					Debug.Log("PC XR: VR is running" + (steam ? " through SteamVR" : ""));
				}
				shown = "";
			}
			else if (!trying && tries < MaxTries && !(Time.unscaledTime < nextTry) && (tries != 0 || SteamReady()))
			{
				StartCoroutine(Retry());
			}
		}

		private string Steam()
		{
			if (!looked)
			{
				looked = true;
				steamJson = SteamVr();
			}
			return steamJson;
		}

		private bool SteamReady()
		{
			if (Steam() == null)
			{
				return true;
			}
			if (steamFrom < 0f)
			{
				steamFrom = Time.unscaledTime;
			}
			if (Time.unscaledTime > steamFrom + SteamMax)
			{
				return true;
			}
			if (Time.unscaledTime > polledAt + PollGap)
			{
				polledAt = Time.unscaledTime;
				Process[] found = Process.GetProcessesByName("vrserver");
				bool up = found.Length > 0;
				foreach (Process process in found)
				{
					process.Dispose();
				}
				if (!up)
				{
					steamUp = false;
					if (!launched)
					{
						try
						{
							launched = true;
							Process.Start(new ProcessStartInfo("steam://run/250820")
							{
								UseShellExecute = true
							});
							Debug.Log("PC XR: SteamVR was not running, asked Steam to start it");
						}
						catch (Exception e)
						{
							Debug.LogWarning("PC XR: could not start SteamVR: " + e.Message);
						}
					}
				}
				else
				{
					if (!steamUp)
					{
						steamUpAt = Time.unscaledTime;
						Debug.Log("PC XR: SteamVR is running, starting VR in " + SteamGrace + " s");
					}
					steamUp = true;
				}
			}
			return steamUp && Time.unscaledTime > steamUpAt + SteamGrace;
		}

		private IEnumerator Retry()
		{
			trying = true;
			tries++;
			if (tries >= 2 && !steam)
			{
				string json = Steam();
				if (json != null && json != Environment.GetEnvironmentVariable("XR_RUNTIME_JSON"))
				{
					Environment.SetEnvironmentVariable("XR_RUNTIME_JSON", json);
					steam = true;
					Debug.Log("PC XR: VR did not start with the default OpenXR runtime, trying SteamVR at " + json);
				}
			}
			shown = (tries == 1) ? "" : (Help + "\n\n(trying again... " + tries + ")");
			yield return null;
			yield return null;
			XRManagerSettings m = XRGeneralSettings.Instance ? XRGeneralSettings.Instance.Manager : null;
			if (m)
			{
				if (m.isInitializationComplete)
				{
					m.DeinitializeLoader();
				}
				yield return m.InitializeLoader();
				if (m.activeLoader)
				{
					m.StartSubsystems();
					Debug.Log("PC XR: VR started on retry " + tries);
				}
				else
				{
					Debug.LogWarning("PC XR: retry " + tries + " could not start VR");
				}
			}
			if (tries >= MaxTries && !Running())
			{
				shown = Help;
			}
			trying = false;
			nextTry = Time.unscaledTime + Gap;
		}

		private static string SteamVr()
		{
			string paths = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "openvr", "openvrpaths.vrpath");
			if (File.Exists(paths))
			{
				Match match = Regex.Match(File.ReadAllText(paths), "\"runtime\"\\s*:\\s*\\[\\s*\"([^\"]+)\"");
				if (match.Success)
				{
					string json = Path.Combine(match.Groups[1].Value.Replace("\\\\", "\\"), "steamxr_win64.json");
					if (File.Exists(json))
					{
						return json;
					}
				}
			}
			string fallback = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\SteamVR\\steamxr_win64.json";
			return File.Exists(fallback) ? fallback : null;
		}

		private void LateUpdate()
		{
			Player main = Player.main;
			Camera from = (main && main.cam) ? main.cam.GetComponent<Camera>() : null;
			bool on = from && Running();
			if (on)
			{
				if (!spectator)
				{
					GameObject go = new GameObject("Spectator Camera");
					go.transform.SetParent(transform, false);
					spectator = go.AddComponent<Camera>();
					spectatorFrom = null;
				}
				if (spectatorFrom != from)
				{
					spectatorFrom = from;
					spectator.CopyFrom(from);
					UniversalAdditionalCameraData data = spectator.GetUniversalAdditionalCameraData();
					JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(from.GetUniversalAdditionalCameraData()), data);
					data.allowXRRendering = false;
					data.renderType = CameraRenderType.Base;
					data.cameraStack?.Clear();
					spectator.stereoTargetEye = StereoTargetEyeMask.None;
					spectator.targetTexture = null;
					spectator.fieldOfView = SpecFov;
					spectator.depth = 100f;
					spectator.targetDisplay = 0;
					spectatorLive = false;
				}
			}
			if (spectator && spectator.enabled != on)
			{
				spectator.enabled = on;
				XRSettings.gameViewRenderMode = on ? GameViewRenderMode.None : GameViewRenderMode.LeftEye;
				spectatorLive = false;
			}
			if (!on)
			{
				return;
			}
			spectator.cullingMask = from.cullingMask;
			spectator.nearClipPlane = from.nearClipPlane;
			spectator.farClipPlane = from.farClipPlane;
			Transform t = spectator.transform;
			Vector3 forward = from.transform.forward;
			Quaternion turn = (forward.sqrMagnitude > 0.0001f) ? Quaternion.LookRotation(forward, Vector3.up) : t.rotation;
			if (spectatorLive && !((t.position - from.transform.position).sqrMagnitude > SpecSnap * SpecSnap))
			{
				float dt = Time.unscaledDeltaTime;
				Vector3 position = t.position;
				Vector3 target = from.transform.position;
				float follow = 1f - Mathf.Exp(-SpecFollow * dt);
				Quaternion rotation = t.rotation;
				float turnBy = 1f - Mathf.Exp(-SpecTurn * dt);
				t.SetPositionAndRotation(Vector3.Lerp(position, target, follow), Quaternion.Slerp(rotation, turn, turnBy));
				return;
			}
			t.SetPositionAndRotation(from.transform.position, turn);
			spectatorLive = true;
		}

		private void OnGUI()
		{
			if (shown == "")
			{
				return;
			}
			if (style == null)
			{
				style = new GUIStyle(GUI.skin.label)
				{
					alignment = TextAnchor.MiddleCenter,
					wordWrap = true,
					fontStyle = FontStyle.Bold
				};
				style.normal.textColor = Color.white;
			}
			style.fontSize = Mathf.Max(Screen.height / 22, 14);
			GUI.color = Color.black;
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
			GUI.color = Color.white;
			GUI.Label(new Rect(Screen.width * 0.05f, 0f, Screen.width * 0.9f, Screen.height), shown, style);
		}
	}
}
