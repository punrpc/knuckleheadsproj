using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Ragdoll
{
	// Not part of the shipped game: an on-screen list of the flat-screen (no headset)
	// controls from Player.Desk and friends. Shown only while Player.main is not in VR;
	// Insert hides/shows it. Uses the game's own fonts and menu colours.
	public class DeskControlsHud : MonoBehaviour
	{
		private static readonly string[,] Rows =
		{
			{ "CLICK", "Lock mouse" },
			{ "MOUSE", "Look" },
			{ "W A S D", "Move" },
			{ "SHIFT", "Sprint" },
			{ "CTRL", "Crouch" },
			{ "F (HOLD)", "Charged jump" },
			{ "SPACE", "Wind up punch" },
			{ "Q", "Raise hands" },
			{ "LMB / RMB", "Grab left / right" },
			{ "E", "Grab both" },
			{ "R", "Recentre" },
			{ "Y / N / S", "Answer prompts / settings" },
			{ "ESC", "Unlock mouse, hold to leave" },
		};

		// MenuTV.MapOn / GameManager.MapYellow, and the dark grey of the Barrier stripes
		private static readonly Color KeyColor = new Color(0.128f, 0.723f, 0.931f, 1f);
		private static readonly Color TitleColor = new Color(1f, 0.76f, 0.16f, 1f);
		private static readonly Color PanelColor = new Color(0.157f, 0.157f, 0.157f, 0.86f);
		private static readonly Color HintColor = new Color(0.62f, 0.62f, 0.66f, 1f);

		private static bool hidden;

		private GameObject panel;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Boot()
		{
			GameObject go = new GameObject("Desk Controls HUD");
			DontDestroyOnLoad(go);
			go.AddComponent<DeskControlsHud>();
		}

		private void Start()
		{
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 500;
			CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920f, 1080f);
			scaler.matchWidthOrHeight = 1f;

			TMP_FontAsset title = Font("Supersonic Rocketship SDF");
			TMP_FontAsset body = Font("Inter_24pt-Bold SDF");

			panel = new GameObject("Panel", typeof(RectTransform), typeof(Image));
			panel.transform.SetParent(transform, false);
			panel.GetComponent<Image>().color = PanelColor;
			panel.GetComponent<Image>().raycastTarget = false;
			RectTransform box = (RectTransform)panel.transform;
			box.anchorMin = box.anchorMax = box.pivot = new Vector2(0f, 1f);
			box.anchoredPosition = new Vector2(24f, -24f);

			const float pad = 22f;
			const float keyWidth = 170f;
			const float actionWidth = 370f;
			const float rowHeight = 30f;
			float y = -pad;
			Label("PC CONTROLS", title, 40f, TitleColor, TextAlignmentOptions.TopLeft, new Vector2(pad, y), keyWidth + actionWidth);
			y -= 54f;
			for (int i = 0; i < Rows.GetLength(0); i++)
			{
				Label(Rows[i, 0], body, 22f, KeyColor, TextAlignmentOptions.TopLeft, new Vector2(pad, y), keyWidth);
				Label(Rows[i, 1].ToUpperInvariant(), body, 22f, Color.white, TextAlignmentOptions.TopLeft, new Vector2(pad + keyWidth, y), actionWidth);
				y -= rowHeight;
			}
			y -= 10f;
			Label("INSERT TO HIDE", body, 18f, HintColor, TextAlignmentOptions.TopLeft, new Vector2(pad, y), keyWidth + actionWidth);
			y -= 24f;
			box.sizeDelta = new Vector2(pad * 2f + keyWidth + actionWidth, -y + pad - 8f);
			Apply();
		}

		private void Update()
		{
			if (Keyboard.current != null && Keyboard.current.insertKey.wasPressedThisFrame)
			{
				hidden = !hidden;
			}
			Apply();
		}

		private void Apply()
		{
			if (!panel)
			{
				return;
			}
			bool show = !hidden && Player.main && !Player.main.inVR;
			if (panel.activeSelf != show)
			{
				panel.SetActive(show);
			}
		}

		private void Label(string text, TMP_FontAsset font, float size, Color color, TextAlignmentOptions align, Vector2 at, float width)
		{
			GameObject go = new GameObject(text, typeof(RectTransform));
			go.transform.SetParent(panel.transform, false);
			TextMeshProUGUI label = go.AddComponent<TextMeshProUGUI>();
			if (font)
			{
				label.font = font;
			}
			label.text = text;
			label.fontSize = size;
			label.color = color;
			label.alignment = align;
			label.textWrappingMode = TextWrappingModes.NoWrap;
			label.raycastTarget = false;
			RectTransform rect = (RectTransform)go.transform;
			rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 1f);
			rect.anchoredPosition = at;
			rect.sizeDelta = new Vector2(width, size * 1.4f);
		}

		// The game's TMP fonts aren't in Resources; they're loaded with the scene's text.
		private static TMP_FontAsset Font(string name)
		{
			foreach (TMP_FontAsset font in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
			{
				if (font.name == name)
				{
					return font;
				}
			}
			return TMP_Settings.defaultFontAsset;
		}
	}
}
