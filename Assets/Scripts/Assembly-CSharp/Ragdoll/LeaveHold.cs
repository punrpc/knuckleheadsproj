using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Ragdoll
{
	public class LeaveHold : MonoBehaviour
	{
		public static LeaveHold main;

		private Canvas bar;

		private RectTransform fill;

		private TMP_Text barText;

		private bool pressWas;

		private float pressedAt = -1f;

		private bool allowed;

		private float buzzAt;

		private static bool forcePress;

		private static Sprite pill;

		private static readonly Color accent = new Color(1f, 0.88f, 0.3f, 1f);

		private const float Delay = 0.3f;

		private const float LeaveTime = 1.5f;

		private const float BarWidth = 300f;

		private const float BarHeight = 26f;

		private const int PillSize = 64;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Boot()
		{
			if (!main)
			{
				GameObject go = new GameObject("Leave Hold");
				Object.DontDestroyOnLoad(go);
				main = go.AddComponent<LeaveHold>();
			}
		}

		private void Update()
		{
			bool pressed = Pressed();
			if (pressed && !pressWas)
			{
				pressedAt = Time.time;
				allowed = !Object.FindFirstObjectByType<MenuTV>();
			}
			pressWas = pressed;
			if (!pressed)
			{
				pressedAt = -1f;
				HideBar();
				return;
			}
			if (pressedAt < 0f || !allowed)
			{
				HideBar();
				return;
			}
			Player me = Player.main;
			if (!me || !me.cam)
			{
				HideBar();
				return;
			}
			float k = (Time.time - pressedAt - Delay) / LeaveTime;
			if (k <= 0f)
			{
				return;
			}
			ShowBar(me, k);
			if (k >= 1f)
			{
				pressedAt = -1f;
				HideBar();
				Player.Hand(XRNode.LeftHand, 0.6f, 0.15f);
				Leave();
			}
		}

		private static bool Pressed()
		{
			if (forcePress)
			{
				return true;
			}
			if (Player.Either(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), UnityEngine.XR.CommonUsages.menuButton, UnityEngine.XR.CommonUsages.primary2DAxisClick))
			{
				return true;
			}
			return Keyboard.current != null && Keyboard.current.escapeKey.isPressed;
		}

		private void ShowBar(Player me, float k)
		{
			if (!bar)
			{
				BuildBar();
			}
			Transform t = bar.transform;
			if (t.parent != me.cam)
			{
				t.SetParent(me.cam, false);
				t.localPosition = new Vector3(0f, -0.06f, 0.45f);
				t.localRotation = Quaternion.identity;
			}
			if (!bar.gameObject.activeSelf)
			{
				Tutorial tutorial = Object.FindFirstObjectByType<Tutorial>();
				barText.text = !tutorial ? "LEAVING GAME" : (tutorial.at >= tutorial.steps.Length ? "FINISHING TUTORIAL" : (Prefs.tutorialDone ? "LEAVING TUTORIAL" : "SKIPPING TUTORIAL"));
				bar.gameObject.SetActive(true);
			}
			k = Mathf.Clamp01(k);
			fill.sizeDelta = new Vector2(Mathf.Lerp(BarHeight, BarWidth, k), BarHeight);
			if (Time.time >= buzzAt)
			{
				buzzAt = Time.time + 0.15f;
				Player.Hand(XRNode.LeftHand, 0.1f + k * 0.3f, 0.03f);
			}
		}

		private void HideBar()
		{
			if (bar && bar.gameObject.activeSelf)
			{
				bar.gameObject.SetActive(false);
			}
		}

		private void BuildBar()
		{
			GameObject go = new GameObject("Leave Bar", typeof(RectTransform));
			bar = go.AddComponent<Canvas>();
			bar.renderMode = RenderMode.WorldSpace;
			RectTransform rt = (RectTransform)go.transform;
			rt.sizeDelta = new Vector2(340f, 110f);
			rt.localScale = Vector3.one * 0.0008f;
			barText = Label(rt, new Vector2(0f, 24f), new Vector2(340f, 56f), 42f);
			Round(Box(rt, "Edge", new Vector2(0f, -24f), new Vector2(BarWidth + 10f, BarHeight + 10f), new Color(0f, 0f, 0f, 0.85f)));
			Round(Box(rt, "Track", new Vector2(0f, -24f), new Vector2(BarWidth, BarHeight), new Color(1f, 1f, 1f, 0.25f)));
			fill = Round(Box(rt, "Fill", new Vector2(-BarWidth / 2f, -24f), new Vector2(BarHeight, BarHeight), accent)).rectTransform;
			fill.pivot = new Vector2(0f, 0.5f);
			fill.anchoredPosition = new Vector2(-BarWidth / 2f, -24f);
			go.SetActive(false);
		}

		private static Image Box(RectTransform parent, string name, Vector2 at, Vector2 size, Color color)
		{
			GameObject go = new GameObject(name, typeof(RectTransform));
			RectTransform rt = (RectTransform)go.transform;
			rt.SetParent(parent, false);
			rt.anchoredPosition = at;
			rt.sizeDelta = size;
			Image img = go.AddComponent<Image>();
			img.color = color;
			img.raycastTarget = false;
			return img;
		}

		private static TMP_Text Label(RectTransform parent, Vector2 at, Vector2 size, float fontSize)
		{
			GameObject go = new GameObject("Text", typeof(RectTransform));
			RectTransform rt = (RectTransform)go.transform;
			rt.SetParent(parent, false);
			rt.anchoredPosition = at;
			rt.sizeDelta = size;
			TextMeshProUGUI label = go.AddComponent<TextMeshProUGUI>();
			label.fontSize = fontSize;
			label.enableAutoSizing = true;
			label.fontSizeMin = fontSize * 0.5f;
			label.fontSizeMax = fontSize;
			label.textWrappingMode = TextWrappingModes.NoWrap;
			label.color = Color.white;
			label.alignment = TextAlignmentOptions.Center;
			label.fontStyle = FontStyles.Bold;
			label.raycastTarget = false;
			TMP_Text source = GameManager.main ? GameManager.main.title : null;
			Tutorial tutorial = source ? null : Object.FindFirstObjectByType<Tutorial>();
			if (tutorial)
			{
				foreach (Tutorial.Step step in tutorial.steps)
				{
					if (!source && step.sign)
					{
						source = step.sign;
					}
				}
			}
			if (source && source.font)
			{
				label.font = source.font;
				label.fontSharedMaterial = source.fontSharedMaterial;
			}
			return label;
		}

		private static Image Round(Image img)
		{
			img.sprite = Pill();
			img.type = Image.Type.Sliced;
			img.pixelsPerUnitMultiplier = PillSize / img.rectTransform.sizeDelta.y;
			return img;
		}

		private static Sprite Pill()
		{
			if (!pill)
			{
				Texture2D tex = new Texture2D(PillSize, PillSize, TextureFormat.RGBA32, false);
				tex.wrapMode = TextureWrapMode.Clamp;
				float r = PillSize / 2f;
				for (int y = 0; y < PillSize; y++)
				{
					float dy = y + 0.5f - r;
					for (int x = 0; x < PillSize; x++)
					{
						float dx = x + 0.5f - r;
						tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(r - Mathf.Sqrt(dy * dy + dx * dx))));
					}
				}
				tex.Apply();
				pill = Sprite.Create(tex, new Rect(0f, 0f, PillSize, PillSize), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect, new Vector4(r - 1f, r - 1f, r - 1f, r - 1f));
			}
			return pill;
		}

		private void Leave()
		{
			Tutorial tutorial = Object.FindFirstObjectByType<Tutorial>();
			if (tutorial && tutorial.Leave())
			{
				return;
			}
			Player.Note("left the game by holding the menu button");
			GameManager.match = null;
			GameManager.hostId = "";
			string scene = GameManager.main ? GameManager.main.lobbyScene : "MainMenu";
			NetworkManager manager = NetworkManager.main;
			if (!manager)
			{
				NetworkManager.Load(scene);
				return;
			}
			manager.GoTo(scene, "");
		}
	}
}
