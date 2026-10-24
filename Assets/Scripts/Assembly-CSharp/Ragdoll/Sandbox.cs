using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using Object = UnityEngine.Object;

namespace Ragdoll
{
	public class Sandbox : MonoBehaviour
	{
		public enum Kind
		{
			Part = 0,
			Build = 1,
			Motor = 2,
			Remote = 3
		}

		private enum Mode
		{
			Spawn = 0,
			Glue = 1,
			Freeze = 2,
			Delete = 3
		}

		private struct Piece
		{
			public Transform t;

			public float mass;
		}

		private struct Dab
		{
			public Transform blob;

			public Transform a;

			public Transform b;
		}

		private class Spin
		{
			public Transform hub;

			public Transform plate;

			public Transform button;

			public HingeJoint joint;

			public Sandbox driver;

			public Vector3 rest;

			public Vector3 ahead;

			public float drive;

			public float reach;

			public float sign;

			public bool toy;

			public bool poked;
		}

		public const int MaxPieces = 32;

		private const int MaxParts = 60;

		private const float GlueSpeed = 6f;

		private const float Reach = 15f;

		private const float StrandWidth = 0.02f;

		private const float DotSize = 0.05f;

		private const float PressAt = 0.6f;

		private const float LetAt = 0.3f;

		private const float StrayFar = 40f;

		private const float Sunk = 6f;

		private const float PressDepth = 0.02f;

		private const float SpawnShot = 6f;

		private const float MenuDistance = 0.6f;

		private const float MenuDrop = 0.15f;

		private const float PreviewRadius = 0.038f;

		private const float HoverGrow = 1.08f;

		private const float PreviewSpin = 90f;

		private const float MaxSpin = 900f;

		private const float TipSpeed = 6f;

		private const float Torque = 40f;

		private const float Brake = 4f;

		private const float ToyDrive = 0.5f;

		private const float PowerDead = 0.05f;

		private const float RemoteReach = 10f;

		private const float Rolls = 1f;

		private const float SteerDead = 8f;

		private const float SteerFull = 40f;

		private const float SideMin = 0.1f;

		private const float SteerPower = 0.4f;

		private static readonly string[] Hints = new string[4] { "TRIGGER: SPAWN WHERE YOU POINT", "HOLD TRIGGER ON A PART, LET GO ON ANOTHER", "TRIGGER: FREEZE OR UNFREEZE", "TRIGGER: DELETE A PART OR A GLUE LINE" };

		private static readonly Color Ice = new Color(0.55f, 0.9f, 1f, 1f);

		private static readonly int topId = Shader.PropertyToID("_TopColor");

		private static readonly int bottomId = Shader.PropertyToID("_BottomColor");

		private static readonly Collider[] near = new Collider[64];

		private static readonly RaycastHit[] rays = new RaycastHit[16];

		private static readonly List<Sandbox> parts = new List<Sandbox>();

		private static readonly Sandbox[] busy = new Sandbox[2];

		private static readonly List<Spin> spins = new List<Spin>();

		private static Material strandLook;

		private static float spunAt = -1f;

		public Kind kind;

		public GameObject[] shapes;

		private readonly List<Piece> pieces = new List<Piece>();

		private readonly List<Dab> dabs = new List<Dab>();

		private readonly List<Spin> paired = new List<Spin>();

		private readonly List<Transform> cables = new List<Transform>();

		private Rigidbody rb;

		private Transform tip;

		private Transform beam;

		private Transform dot;

		private Transform from;

		private Transform menu;

		private Transform preview;

		private Transform cable;

		private Transform goLight;

		private Transform backLight;

		private Transform[] lights;

		private RectTransform mark;

		private RectTransform modeMark;

		private RectTransform[] items;

		private RectTransform[] tabs;

		private bool[] poked;

		private bool[] tabPoked;

		private TMP_Text label;

		private TMP_Text hint;

		private GameObject shown;

		private Vector3 fromAt;

		private Vector3 home;

		private Quaternion homeTurn;

		private Mode mode;

		private int pick;

		private float spin;

		private bool pressed;

		private bool frozen;

		private bool open;

		private bool buttonDown;

		private bool reverse;

		private bool recentring;

		private RectTransform canvasRect;

		private RectTransform hovered;

		private readonly LineRenderer[] lasers = new LineRenderer[2];

		private readonly Transform[] laserDots = new Transform[2];

		private readonly bool[] laserHeld = new bool[2];

		private MaterialPropertyBlock tint;

		public int Count => pieces.Count;

		public bool Frozen => frozen;

		public Rigidbody Body => rb;

		private void Awake()
		{
			rb = GetComponent<Rigidbody>();
			if (kind == Kind.Motor)
			{
				Assemble();
				return;
			}
			home = transform.position;
			homeTurn = transform.rotation;
			if (kind == Kind.Build)
			{
				items = new RectTransform[shapes.Length];
				poked = new bool[shapes.Length];
				tabs = new RectTransform[Hints.Length];
				tabPoked = new bool[Hints.Length];
				lights = new Transform[Hints.Length];
			}
			foreach (Transform t in GetComponentsInChildren<Transform>(true))
			{
				if (t.name == "Tip")
				{
					tip = t;
				}
				if (t.name == "Beam")
				{
					beam = t;
				}
				if (t.name == "Dot")
				{
					dot = t;
				}
				if (t.name == "Menu")
				{
					menu = t;
				}
				if (t.name == "Preview")
				{
					preview = t;
				}
				if (t.name == "Canvas")
				{
					canvasRect = t as RectTransform;
				}
				if (t.name == "Mark")
				{
					mark = t as RectTransform;
				}
				if (t.name == "Mode Mark")
				{
					modeMark = t as RectTransform;
				}
				if (t.name == "Name")
				{
					label = t.GetComponent<TMP_Text>();
				}
				if (t.name == "Hint")
				{
					hint = t.GetComponent<TMP_Text>();
				}
				if (t.name == "Cable")
				{
					cable = t;
				}
				if (t.name == "Go Light")
				{
					goLight = t;
				}
				if (t.name == "Back Light")
				{
					backLight = t;
				}
				if (t.name == "Strand" && t.GetComponent<Renderer>())
				{
					strandLook = t.GetComponent<Renderer>().sharedMaterial;
				}
				if (items != null && t.name.StartsWith("Item ") && int.TryParse(t.name.Substring(5), out int item) && item < items.Length)
				{
					items[item] = t as RectTransform;
				}
				if (tabs != null && t.name.StartsWith("Tab ") && int.TryParse(t.name.Substring(4), out int tab) && tab < tabs.Length)
				{
					tabs[tab] = t as RectTransform;
				}
				if (lights != null && t.name.StartsWith("Mode Light ") && int.TryParse(t.name.Substring(11), out int light) && light < lights.Length)
				{
					lights[light] = t;
				}
			}
			foreach (Transform t in new Transform[5] { beam, dot, menu, cable, backLight })
			{
				if (t)
				{
					t.gameObject.SetActive(false);
				}
			}
			if (kind == Kind.Build)
			{
				Use(Mode.Spawn);
			}
			if (kind != Kind.Part || !rb)
			{
				return;
			}
			foreach (Transform child in transform)
			{
				pieces.Add(new Piece
				{
					t = child,
					mass = rb.mass / transform.childCount
				});
			}
			parts.Add(this);
		}

		private void OnDestroy()
		{
			parts.Remove(this);
			Claim(false, XRNode.RightHand);
			if (menu && menu.parent != transform)
			{
				Destroy(menu.gameObject);
			}
		}

		public static bool Busy(XRNode hand)
		{
			return busy[Side(hand)];
		}

		private static int Side(XRNode hand)
		{
			return hand != XRNode.LeftHand ? 1 : 0;
		}

		private void Claim(bool held, XRNode hand)
		{
			for (int i = 0; i < busy.Length; i++)
			{
				if (busy[i] == this)
				{
					busy[i] = null;
				}
			}
			if (held)
			{
				busy[Side(hand)] = this;
			}
		}

		private void Update()
		{
			if (kind != Kind.Build && kind != Kind.Remote)
			{
				return;
			}
			if (!rb)
			{
				return;
			}
			Player me = Player.main;
			XRNode hand = XRNode.RightHand;
			bool held = me && me.Holds(rb, out hand);
			Claim(held, hand);
			InputDevice d = InputDevices.GetDeviceAtXRNode(hand);
			float trigger = 0f;
			bool button = false;
			if (held)
			{
				trigger = d.isValid && d.TryGetFeatureValue(CommonUsages.trigger, out float t) ? t : 0f;
				button = d.isValid && d.TryGetFeatureValue(CommonUsages.secondaryButton, out bool b) && b;
			}
			bool tap = button && !buttonDown;
			buttonDown = button;
			bool on = held && trigger > (pressed ? LetAt : PressAt);
			if (kind == Kind.Build)
			{
				Build(held, on, tap, hand);
			}
			else
			{
				Remote(held, trigger, tap, hand, Steer(held, d));
			}
			pressed = on;
		}

		private void FixedUpdate()
		{
			if (!rb)
			{
				return;
			}
			if (kind == Kind.Part)
			{
				Turn();
				if (rb.position.y < Sea() - Sunk)
				{
					Destroy(gameObject);
				}
			}
			else
			{
				Stray();
			}
		}

		private static float Sea()
		{
			Water water = Water.Get();
			return water ? water.Surface : -9999f;
		}

		private static Sandbox Of(Collider c)
		{
			if (!c || !c.attachedRigidbody)
			{
				return null;
			}
			return c.attachedRigidbody.GetComponent<Sandbox>();
		}

		private int PieceOf(Collider c)
		{
			for (int i = 0; i < pieces.Count; i++)
			{
				if (pieces[i].t && c.transform.IsChildOf(pieces[i].t))
				{
					return i;
				}
			}
			return -1;
		}

		private int DabOf(Collider c)
		{
			for (int i = 0; i < dabs.Count; i++)
			{
				if (dabs[i].blob == c.transform)
				{
					return i;
				}
			}
			return -1;
		}

		private void Build(bool held, bool on, bool tap, XRNode hand)
		{
			if (tap)
			{
				Open(!open);
			}
			if (!held && open)
			{
				Open(false);
			}
			RaycastHit h = default;
			bool hit = false;
			Sandbox s = null;
			if (open && Panel(hand))
			{
				on = pressed;
			}
			else if (held && Ray(Reach, mode == Mode.Delete, out h))
			{
				s = Of(h.collider);
				hit = true;
			}
			int piece = s && s.kind == Kind.Part ? s.PieceOf(h.collider) : -1;
			int dab = s && mode == Mode.Delete ? s.DabOf(h.collider) : -1;
			Vector3 a = Vector3.zero;
			Vector3 b = hit ? h.point : tip.position + tip.forward * Reach;
			bool aim = piece >= 0 || dab >= 0;
			bool line = false;
			if (mode == Mode.Freeze)
			{
				a = tip.position;
				if (on)
				{
					if (!pressed)
					{
						if (piece >= 0)
						{
							s.Freeze(!s.frozen);
							Player.Note((s.frozen ? "froze " : "thawed ") + s.name + " (" + s.Count + " pieces)");
						}
						Player.Hand(hand, piece >= 0 ? 0.8f : 0.15f, 0.08f);
					}
					line = true;
				}
			}
			else if (mode == Mode.Glue)
			{
				if (on)
				{
					if (!pressed)
					{
						from = piece >= 0 ? s.pieces[piece].t : null;
						if (from)
						{
							fromAt = from.InverseTransformPoint(h.point);
						}
						Player.Hand(hand, from ? 0.4f : 0.1f, 0.05f);
					}
					if (from)
					{
						a = from.TransformPoint(fromAt);
						line = true;
					}
				}
				else
				{
					if (pressed && from)
					{
						bool ok = piece >= 0 && Join(from, from.TransformPoint(fromAt), s.pieces[piece].t, h.point);
						Player.Hand(hand, ok ? 0.9f : 0.15f, ok ? 0.15f : 0.05f);
					}
					from = null;
				}
			}
			else if (mode == Mode.Spawn)
			{
				if (on && !pressed)
				{
					Player.Hand(hand, Spawn(hit, h) ? 0.7f : 0.1f, 0.08f);
				}
				aim = hit;
			}
			else if (on && !pressed)
			{
				if (dab >= 0)
				{
					s.Unglue(dab);
				}
				else if (piece >= 0)
				{
					s.Poof(piece);
				}
				Player.Hand(hand, aim ? 1f : 0.1f, 0.1f);
			}
			if (dot)
			{
				if (dot.gameObject.activeSelf != aim)
				{
					dot.gameObject.SetActive(aim);
				}
				if (aim)
				{
					dot.position = h.point;
				}
			}
			if (line)
			{
				Stretch(beam, a, b);
			}
			else if (beam && beam.gameObject.activeSelf)
			{
				beam.gameObject.SetActive(false);
			}
		}

		private void Use(Mode m)
		{
			from = null;
			mode = m;
			if (modeMark && tabs[(int)m])
			{
				modeMark.anchoredPosition = tabs[(int)m].anchoredPosition;
			}
			if (hint)
			{
				hint.text = "B / Y: CLOSE   -   " + Hints[(int)m];
			}
			for (int i = 0; i < lights.Length; i++)
			{
				if (lights[i])
				{
					lights[i].gameObject.SetActive((int)m == i);
				}
			}
			if (beam)
			{
				beam.gameObject.SetActive(false);
			}
		}

		private void Open(bool on)
		{
			open = on;
			if (!menu)
			{
				return;
			}
			menu.gameObject.SetActive(on);
			if (on)
			{
				menu.SetParent(null, true);
				Show(pick);
				recentring = true;
				Follow(true);
				return;
			}
			menu.SetParent(transform, true);
			Hover(null);
			for (int i = 0; i < lasers.Length; i++)
			{
				Laser(i, false, Vector3.zero, Vector3.zero);
				laserHeld[i] = false;
			}
		}

		private void Follow(bool snap)
		{
			Player me = Player.main;
			Transform head = me && me.cam ? me.cam : (Camera.main ? Camera.main.transform : null);
			if (!head || !menu)
			{
				return;
			}
			Vector3 flat = Vector3.ProjectOnPlane(head.forward, Vector3.up);
			if (flat.sqrMagnitude < 0.0001f)
			{
				flat = Vector3.forward;
			}
			flat = flat.normalized;
			Vector3 at = head.position;
			Quaternion turn = Quaternion.LookRotation(flat);
			Vector3 off = Vector3.ProjectOnPlane(menu.position - at, Vector3.up);
			Vector3 target = at + flat * MenuDistance + Vector3.down * MenuDrop;
			if (!recentring && Vector3.Angle(flat, off) <= 40f && Mathf.Abs(off.magnitude - MenuDistance) <= 0.3f)
			{
				return;
			}
			recentring = true;
			float k = snap ? 1f : 1f - Mathf.Exp(Time.deltaTime * -5f);
			menu.SetPositionAndRotation(Vector3.Lerp(menu.position, target, k), Quaternion.Slerp(menu.rotation, turn, k));
			if ((menu.position - target).sqrMagnitude < 0.0025f)
			{
				recentring = false;
			}
		}

		private bool Panel(XRNode hand)
		{
			Player me = Player.main;
			if (!menu || !me)
			{
				return false;
			}
			Follow(false);
			spin += Time.deltaTime * PreviewSpin;
			if (shown)
			{
				shown.transform.localRotation = Quaternion.Euler(18f, spin, 0f);
			}
			Rigidbody mine = hand == XRNode.LeftHand ? me.leftHand : me.rightHand;
			XRNode buzz = hand == XRNode.LeftHand ? XRNode.RightHand : XRNode.LeftHand;
			RectTransform over = null;
			bool used = false;
			foreach (XRNode node in new XRNode[2] { XRNode.LeftHand, XRNode.RightHand })
			{
				RectTransform r = Point(node, me, out bool onMenu);
				if (r)
				{
					over = r;
				}
				if (onMenu)
				{
					used |= node == hand;
				}
			}
			Hover(over);
			for (int i = 0; i < items.Length; i++)
			{
				bool p = Poked(items[i], me, mine);
				if (p && !poked[i])
				{
					Pick(items[i], buzz);
				}
				poked[i] = p;
			}
			for (int i = 0; i < tabs.Length; i++)
			{
				bool p = Poked(tabs[i], me, mine);
				if (p && !tabPoked[i])
				{
					Pick(tabs[i], buzz);
				}
				tabPoked[i] = p;
			}
			return used;
		}

		private RectTransform Point(XRNode node, Player me, out bool onMenu)
		{
			onMenu = false;
			Transform grip = node == XRNode.LeftHand ? me.controllerL : me.controllerR;
			InputDevice d = InputDevices.GetDeviceAtXRNode(node);
			if (!grip || !canvasRect)
			{
				Laser(Side(node), false, Vector3.zero, Vector3.zero);
				return null;
			}
			Aim(d, grip, out Vector3 origin, out Vector3 dir);
			Plane plane = new Plane(menu.forward, canvasRect.position);
			Ray ray = new Ray(origin, dir);
			if (Vector3.Dot(ray.direction, menu.forward) > 0f && plane.Raycast(ray, out float enter) && enter <= 3f)
			{
				Vector3 at = ray.GetPoint(enter);
				Vector3 local = canvasRect.InverseTransformPoint(at);
				if (Mathf.Abs(local.x) <= canvasRect.rect.width * 0.5f && Mathf.Abs(local.y) <= canvasRect.rect.height * 0.5f)
				{
					onMenu = true;
					Laser(Side(node), true, ray.origin, at);
					RectTransform r = Under(local);
					float trigger = d.isValid && d.TryGetFeatureValue(CommonUsages.trigger, out float t) ? t : 0f;
					bool down = trigger > (laserHeld[Side(node)] ? LetAt : PressAt);
					if (!laserHeld[Side(node)] && down && r)
					{
						Pick(r, node);
					}
					laserHeld[Side(node)] = down;
					return r;
				}
			}
			Laser(Side(node), false, Vector3.zero, Vector3.zero);
			laserHeld[Side(node)] = false;
			return null;
		}

		private static bool Aim(InputDevice d, Transform grip, out Vector3 from, out Vector3 dir)
		{
			Quaternion turn = Quaternion.identity;
			Vector3 offset = Vector3.zero;
			if (d.isValid && d.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 gripPos) && d.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion gripRot) && d.TryGetFeatureValue(new InputFeatureUsage<Vector3>("PointerPosition"), out Vector3 aimPos) && d.TryGetFeatureValue(new InputFeatureUsage<Quaternion>("PointerRotation"), out Quaternion aimRot))
			{
				Quaternion inv = Quaternion.Inverse(gripRot);
				turn = inv * aimRot;
				offset = inv * (aimPos - gripPos);
			}
			from = grip.position + grip.rotation * offset;
			dir = grip.rotation * turn * Vector3.forward;
			return true;
		}

		private RectTransform Under(Vector2 px)
		{
			foreach (RectTransform[] set in new RectTransform[2][] { tabs, items })
			{
				foreach (RectTransform r in set)
				{
					if (r)
					{
						Vector2 at = r.anchoredPosition;
						if (Mathf.Abs(px.x - at.x) <= r.rect.width * 0.5f && Mathf.Abs(px.y - at.y) <= r.rect.height * 0.5f)
						{
							return r;
						}
					}
				}
			}
			return null;
		}

		private void Pick(RectTransform r, XRNode buzz)
		{
			int tab = Array.IndexOf(tabs, r);
			int item = Array.IndexOf(items, r);
			if (tab >= 0 && tab != (int)mode)
			{
				Use((Mode)tab);
			}
			else
			{
				if (item < 0 || (item == pick && mode == Mode.Spawn))
				{
					return;
				}
				Show(item);
				Use(Mode.Spawn);
			}
			Player.Hand(buzz, 0.5f, 0.05f);
		}

		private void Hover(RectTransform r)
		{
			if (hovered == r)
			{
				return;
			}
			if (hovered)
			{
				hovered.localScale = Vector3.one;
			}
			hovered = r;
			if (hovered)
			{
				hovered.localScale = Vector3.one * HoverGrow;
			}
		}

		private void Laser(int i, bool on, Vector3 a, Vector3 b)
		{
			if (!lasers[i] && on)
			{
				GameObject go = new GameObject(i == 0 ? "Menu Ray L" : "Menu Ray R");
				go.transform.SetParent(transform, false);
				LineRenderer line = go.AddComponent<LineRenderer>();
				Renderer look = beam ? beam.GetComponent<Renderer>() : null;
				if (look)
				{
					line.sharedMaterial = look.sharedMaterial;
				}
				line.positionCount = 2;
				line.startWidth = 0.008f;
				line.endWidth = 0.004f;
				line.numCapVertices = 4;
				line.shadowCastingMode = ShadowCastingMode.Off;
				line.receiveShadows = false;
				lasers[i] = line;
				GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				Object.DestroyImmediate(ball.GetComponent<Collider>());
				ball.transform.SetParent(go.transform, false);
				ball.transform.localScale = Vector3.one * 0.018f;
				Renderer ballLook = ball.GetComponent<Renderer>();
				if (look)
				{
					ballLook.sharedMaterial = look.sharedMaterial;
				}
				ballLook.shadowCastingMode = ShadowCastingMode.Off;
				laserDots[i] = ball.transform;
			}
			if (!lasers[i])
			{
				return;
			}
			if (lasers[i].enabled != on)
			{
				lasers[i].enabled = on;
			}
			if (laserDots[i] && laserDots[i].gameObject.activeSelf != on)
			{
				laserDots[i].gameObject.SetActive(on);
			}
			if (on)
			{
				lasers[i].SetPosition(0, a);
				lasers[i].SetPosition(1, b);
				if (laserDots[i])
				{
					laserDots[i].position = b;
				}
			}
		}

		private bool Poked(RectTransform r, Player me, Rigidbody mine)
		{
			if (!r)
			{
				return false;
			}
			float halfX = r.rect.width * 0.5f * r.lossyScale.x;
			float halfY = r.rect.height * 0.5f * r.lossyScale.y;
			PhysicsScene scene = gameObject.scene.GetPhysicsScene();
			Rect rect = r.rect;
			int n = scene.OverlapBox(r.TransformPoint(new Vector3(rect.x + rect.width * 0.5f, rect.y + rect.height * 0.5f, 0f)), new Vector3(halfX, halfY, 0.03f), near, r.rotation, -1, QueryTriggerInteraction.Collide);
			for (int i = 0; i < n; i++)
			{
				Collider c = near[i];
				if (c.GetComponentInParent<Player>() != me)
				{
					continue;
				}
				if (!mine)
				{
					return true;
				}
				if (c.attachedRigidbody != mine && !c.transform.IsChildOf(mine.transform))
				{
					return true;
				}
			}
			return false;
		}

		private void Show(int i)
		{
			if (i < 0 || shapes == null || shapes.Length <= i || !shapes[i])
			{
				return;
			}
			pick = i;
			if (mark && items[i])
			{
				mark.anchoredPosition = items[i].anchoredPosition;
			}
			if (label)
			{
				label.text = shapes[i].name.ToUpper();
			}
			if (shown)
			{
				Object.Destroy(shown);
			}
			shown = preview ? Model(shapes[i], preview) : null;
		}

		private static GameObject Model(GameObject from, Transform parent)
		{
			GameObject root = new GameObject("Model");
			root.transform.SetParent(parent, false);
			Bounds bounds = default;
			bool any = false;
			foreach (MeshFilter mf in from.GetComponentsInChildren<MeshFilter>(true))
			{
				if (!mf.sharedMesh || !mf.GetComponent<MeshRenderer>())
				{
					continue;
				}
				Matrix4x4 m = from.transform.worldToLocalMatrix * mf.transform.localToWorldMatrix;
				Bounds b = mf.sharedMesh.bounds;
				for (int c = 0; c < 8; c++)
				{
					Vector3 p = m.MultiplyPoint3x4(b.center + new Vector3((c & 1) == 0 ? -b.extents.x : b.extents.x, (c & 2) == 0 ? -b.extents.y : b.extents.y, (c & 4) == 0 ? -b.extents.z : b.extents.z));
					if (any)
					{
						bounds.Encapsulate(p);
						continue;
					}
					bounds = new Bounds(p, Vector3.zero);
					any = true;
				}
			}
			foreach (MeshFilter mf in from.GetComponentsInChildren<MeshFilter>(true))
			{
				MeshRenderer look = mf.GetComponent<MeshRenderer>();
				if (!look || !mf.sharedMesh)
				{
					continue;
				}
				Matrix4x4 m = from.transform.worldToLocalMatrix * mf.transform.localToWorldMatrix;
				GameObject bit = new GameObject(mf.name);
				bit.transform.SetParent(root.transform, false);
				bit.transform.localPosition = (Vector3)m.GetColumn(3) - bounds.center;
				bit.transform.localRotation = m.rotation;
				bit.transform.localScale = m.lossyScale;
				bit.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
				MeshRenderer r = bit.AddComponent<MeshRenderer>();
				r.sharedMaterials = look.sharedMaterials;
				r.shadowCastingMode = ShadowCastingMode.Off;
			}
			root.transform.localScale = Vector3.one * (PreviewRadius / Mathf.Max(bounds.extents.magnitude, 0.001f));
			return root;
		}

		private bool Spawn(bool hit, RaycastHit h)
		{
			if (shapes == null || pick >= shapes.Length || !shapes[pick] || !tip)
			{
				return false;
			}
			Tidy();
			Vector3 flat = Vector3.ProjectOnPlane(tip.forward, Vector3.up);
			Quaternion turn = flat.sqrMagnitude > 0.001f ? Quaternion.LookRotation(flat) : Quaternion.identity;
			Vector3 at = hit ? h.point + h.normal * (Lift(shapes[pick], h.normal, turn) + 0.02f) : tip.position + tip.forward * 0.4f;
			GameObject made = Object.Instantiate(shapes[pick], at, turn);
			SceneManager.MoveGameObjectToScene(made, gameObject.scene);
			made.name = shapes[pick].name;
			Rigidbody body = made.GetComponent<Rigidbody>();
			if (body && !hit)
			{
				body.linearVelocity = tip.forward * SpawnShot;
			}
			Player.Note("spawned a " + made.name + (hit ? " at " + h.collider.name : ", fired it"));
			return true;
		}

		private static float Lift(GameObject from, Vector3 normal, Quaternion turn)
		{
			float lift = 0f;
			foreach (MeshFilter mf in from.GetComponentsInChildren<MeshFilter>(true))
			{
				if (!mf.sharedMesh || !mf.GetComponent<MeshRenderer>())
				{
					continue;
				}
				Matrix4x4 m = from.transform.worldToLocalMatrix * mf.transform.localToWorldMatrix;
				Vector3 e = mf.sharedMesh.bounds.extents;
				Vector3 s = m.lossyScale;
				Quaternion q = turn * m.rotation;
				float d = e.x * s.x * Mathf.Abs(Vector3.Dot(normal, q * Vector3.right)) + e.y * s.y * Mathf.Abs(Vector3.Dot(normal, q * Vector3.up)) + e.z * s.z * Mathf.Abs(Vector3.Dot(normal, q * Vector3.forward));
				lift = Mathf.Max(lift, d);
			}
			return lift;
		}

		private static bool Join(Transform fa, Vector3 pa, Transform fb, Vector3 pb)
		{
			Sandbox a = fa.parent ? fa.parent.GetComponent<Sandbox>() : null;
			Sandbox b = fb.parent ? fb.parent.GetComponent<Sandbox>() : null;
			if (!a || !b || fa == fb)
			{
				return false;
			}
			Sandbox s = a == b ? a : a.Weld(b);
			if (!s)
			{
				return false;
			}
			s.Tie(pa, pb, fa, fb);
			Player.Note("glued " + fa.name + " to " + fb.name + " across " + Vector3.Distance(pa, pb).ToString("F1") + " m (" + s.Count + " pieces)");
			return true;
		}

		private Sandbox Weld(Sandbox other)
		{
			if (!other || other == this || !rb || !other.rb)
			{
				return null;
			}
			if (pieces.Count + other.pieces.Count > MaxPieces)
			{
				return null;
			}
			if ((rb.linearVelocity - other.rb.linearVelocity).magnitude > GlueSpeed)
			{
				return null;
			}
			Player me = Player.main;
			bool heldA = me && me.Holds(rb);
			bool heldB = me && me.Holds(other.rb);
			bool flip = other.frozen != frozen ? other.frozen : (heldA != heldB ? heldB : other.rb.mass > rb.mass);
			Sandbox keep = flip ? other : this;
			Sandbox gone = flip ? this : other;
			Vector3 keepVelocity = keep.rb.linearVelocity;
			float keepMass = keep.rb.mass;
			Vector3 goneVelocity = gone.rb.linearVelocity;
			float goneMass = gone.rb.mass;
			Vector3 keepSpin = keep.rb.angularVelocity;
			if (me && me.Holds(gone.rb))
			{
				me.GaveWay(gone.rb);
			}
			Vector3 momentum = keepVelocity * keepMass + goneVelocity * goneMass;
			foreach (Piece p in gone.pieces)
			{
				p.t.SetParent(keep.transform, true);
				keep.pieces.Add(p);
			}
			foreach (Dab d in gone.dabs)
			{
				if (d.blob)
				{
					d.blob.SetParent(keep.transform, true);
				}
				keep.dabs.Add(d);
			}
			gone.pieces.Clear();
			gone.dabs.Clear();
			gone.rb.isKinematic = true;
			gone.rb.detectCollisions = false;
			Object.Destroy(gone.gameObject);
			keep.Mass();
			if (!keep.frozen)
			{
				keep.rb.linearVelocity = momentum / keep.rb.mass;
				keep.rb.angularVelocity = keepSpin;
			}
			keep.Settle();
			keep.Tint();
			Rejoin();
			return keep;
		}

		private void Tie(Vector3 pa, Vector3 pb, Transform fa, Transform fb)
		{
			GameObject glue = new GameObject("Glue");
			glue.transform.SetParent(transform, false);
			Vector3 d = pb - pa;
			float length = d.magnitude;
			glue.transform.SetPositionAndRotation((pa + pb) * 0.5f, length > 0.001f ? Quaternion.LookRotation(d) : transform.rotation);
			CapsuleCollider capsule = glue.AddComponent<CapsuleCollider>();
			capsule.isTrigger = true;
			capsule.direction = 2;
			capsule.radius = DotSize;
			capsule.height = length + 0.1f;
			float half = length * 0.5f;
			Bit(PrimitiveType.Cylinder, glue.transform, Vector3.zero, Quaternion.Euler(90f, 0f, 0f), new Vector3(StrandWidth, half, StrandWidth));
			Bit(PrimitiveType.Sphere, glue.transform, Vector3.forward * half, Quaternion.identity, Vector3.one * DotSize);
			Bit(PrimitiveType.Sphere, glue.transform, Vector3.back * half, Quaternion.identity, Vector3.one * DotSize);
			dabs.Add(new Dab
			{
				blob = glue.transform,
				a = fa,
				b = fb
			});
			Tint();
		}

		private static void Bit(PrimitiveType type, Transform parent, Vector3 at, Quaternion turn, Vector3 size)
		{
			GameObject bit = GameObject.CreatePrimitive(type);
			Object.DestroyImmediate(bit.GetComponent<Collider>());
			bit.transform.SetParent(parent, false);
			bit.transform.localPosition = at;
			bit.transform.localRotation = turn;
			bit.transform.localScale = size;
			Renderer look = bit.GetComponent<Renderer>();
			if (strandLook)
			{
				look.sharedMaterial = strandLook;
			}
			look.shadowCastingMode = ShadowCastingMode.Off;
		}

		private void Unglue(int dab)
		{
			if (dabs[dab].blob)
			{
				Object.Destroy(dabs[dab].blob.gameObject);
			}
			Player.Note("removed the glue between " + dabs[dab].a.name + " and " + dabs[dab].b.name);
			dabs.RemoveAt(dab);
			Split();
		}

		private void Poof(int piece)
		{
			Transform t = pieces[piece].t;
			for (int i = dabs.Count - 1; i >= 0; i--)
			{
				if (dabs[i].a == t || dabs[i].b == t)
				{
					if (dabs[i].blob)
					{
						Object.Destroy(dabs[i].blob.gameObject);
					}
					dabs.RemoveAt(i);
				}
			}
			pieces.RemoveAt(piece);
			Player.Note("deleted a " + t.name);
			if (pieces.Count == 0)
			{
				Object.Destroy(gameObject);
				return;
			}
			t.SetParent(null, true);
			Object.Destroy(t.gameObject);
			Split();
		}

		private void Split()
		{
			List<Piece> left = new List<Piece>(pieces);
			List<List<Piece>> groups = new List<List<Piece>>();
			while (left.Count > 0)
			{
				List<Piece> group = new List<Piece>();
				group.Add(left[0]);
				left.RemoveAt(0);
				for (int i = 0; i < group.Count; i++)
				{
					for (int j = left.Count - 1; j >= 0; j--)
					{
						if (Glued(group[i].t, left[j].t))
						{
							group.Add(left[j]);
							left.RemoveAt(j);
						}
					}
				}
				groups.Add(group);
			}
			for (int i = 1; i < groups.Count; i++)
			{
				Adopt(groups[i]);
			}
			Mass();
			Settle();
			Tint();
			Rejoin();
		}

		private bool Glued(Transform a, Transform b)
		{
			foreach (Dab d in dabs)
			{
				if ((d.a == a && d.b == b) || (d.a == b && d.b == a))
				{
					return true;
				}
			}
			return false;
		}

		private void Adopt(List<Piece> group)
		{
			GameObject go = new GameObject(group[0].t.name);
			go.SetActive(false);
			SceneManager.MoveGameObjectToScene(go, gameObject.scene);
			go.transform.SetPositionAndRotation(group[0].t.position, group[0].t.rotation);
			Rigidbody body = go.AddComponent<Rigidbody>();
			body.linearDamping = rb.linearDamping;
			body.angularDamping = rb.angularDamping;
			body.interpolation = rb.interpolation;
			body.collisionDetectionMode = rb.collisionDetectionMode;
			body.isKinematic = frozen;
			Sandbox box = go.AddComponent<Sandbox>();
			go.AddComponent<Grabbable>();
			foreach (Piece p in group)
			{
				p.t.SetParent(go.transform, true);
				pieces.Remove(p);
			}
			for (int i = dabs.Count - 1; i >= 0; i--)
			{
				Transform a = dabs[i].a;
				if (group.Exists((Piece p) => p.t == a))
				{
					if (dabs[i].blob)
					{
						dabs[i].blob.SetParent(go.transform, true);
					}
					box.dabs.Add(dabs[i]);
					dabs.RemoveAt(i);
				}
			}
			go.SetActive(true);
			box.rb = body;
			box.pieces.Clear();
			box.pieces.AddRange(group);
			box.frozen = frozen;
			box.Mass();
			box.Settle();
			box.Tint();
			if (!frozen)
			{
				body.linearVelocity = rb.GetPointVelocity(go.transform.position);
				body.angularVelocity = rb.angularVelocity;
			}
		}

		private void Mass()
		{
			float total = 0f;
			Vector3 sum = Vector3.zero;
			foreach (Piece p in pieces)
			{
				Renderer look = p.t.GetComponent<Renderer>();
				sum += transform.InverseTransformPoint(look ? look.bounds.center : p.t.position) * p.mass;
				total += p.mass;
			}
			rb.mass = Mathf.Max(total, 0.01f);
			rb.ResetCenterOfMass();
			rb.ResetInertiaTensor();
			rb.centerOfMass = sum / rb.mass;
		}

		private void Settle()
		{
			foreach (Collider c in GetComponentsInChildren<Collider>())
			{
				Player.IgnoreFingers(c);
			}
			if (!rb.isKinematic)
			{
				rb.WakeUp();
			}
		}

		private static void Stretch(Transform line, Vector3 a, Vector3 b)
		{
			if (!line)
			{
				return;
			}
			if (!line.gameObject.activeSelf)
			{
				line.gameObject.SetActive(true);
			}
			line.position = (a + b) * 0.5f;
			Vector3 d = b - a;
			if (d.sqrMagnitude > 1E-06f)
			{
				line.rotation = Quaternion.LookRotation(d) * Quaternion.Euler(90f, 0f, 0f);
			}
			line.localScale = new Vector3(line.localScale.x, d.magnitude * 0.5f, line.localScale.z);
		}

		private bool Ray(float reach, bool strands, out RaycastHit hit)
		{
			hit = default;
			if (!tip)
			{
				return false;
			}
			PhysicsScene scene = gameObject.scene.GetPhysicsScene();
			int n = scene.Raycast(tip.position, tip.forward, rays, reach, -1, strands ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore);
			Player me = Player.main;
			if (n < 1)
			{
				return false;
			}
			float best = float.MaxValue;
			for (int i = 0; i < n; i++)
			{
				Collider c = rays[i].collider;
				if (rays[i].rigidbody == rb || rays[i].distance >= best)
				{
					continue;
				}
				if (me && c.GetComponentInParent<Player>() == me)
				{
					continue;
				}
				if (c.isTrigger && (!Of(c) || Of(c).DabOf(c) < 0))
				{
					continue;
				}
				best = rays[i].distance;
				hit = rays[i];
			}
			return best < float.MaxValue;
		}

		private void Freeze(bool on)
		{
			frozen = on;
			rb.isKinematic = on;
			if (!on)
			{
				rb.WakeUp();
			}
			Tint();
		}

		private void Tint()
		{
			if (tint == null)
			{
				tint = new MaterialPropertyBlock();
			}
			foreach (Renderer r in GetComponentsInChildren<Renderer>())
			{
				Material m = r.sharedMaterial;
				if (frozen && m && m.HasProperty(topId))
				{
					tint.SetColor(topId, Color.Lerp(m.GetColor(topId), Ice, 0.6f));
					tint.SetColor(bottomId, Color.Lerp(m.GetColor(m.HasProperty(bottomId) ? bottomId : topId), Ice, 0.6f));
					r.SetPropertyBlock(tint);
				}
				else
				{
					r.SetPropertyBlock(null);
				}
			}
		}

		private void Stray()
		{
			Player me = Player.main;
			if (rb.isKinematic || (me && me.Holds(rb)))
			{
				return;
			}
			if (rb.position.y > home.y - 4f && (rb.position - home).sqrMagnitude < StrayFar * StrayFar)
			{
				return;
			}
			transform.SetPositionAndRotation(home, homeTurn);
			rb.position = home;
			rb.rotation = homeTurn;
			rb.linearVelocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}

		private static void Tidy()
		{
			int total = 0;
			foreach (Sandbox s in parts)
			{
				total += s.pieces.Count;
			}
			if (total < MaxParts)
			{
				return;
			}
			Player me = Player.main;
			foreach (Sandbox s in parts)
			{
				if (s.pieces.Count == 1 && !s.frozen && !(me && me.Holds(s.rb)))
				{
					Object.Destroy(s.gameObject);
					break;
				}
			}
		}

		private void Assemble()
		{
			Spin p = new Spin();
			List<Transform> kids = new List<Transform>();
			foreach (Transform child in transform)
			{
				if (child.GetComponent<Sandbox>() && child.childCount > 0)
				{
					kids.Add(child);
				}
			}
			foreach (Transform kid in kids)
			{
				Transform button = Find(kid, "Button");
				if (button)
				{
					p.hub = kid.GetChild(0);
					p.button = button;
					p.rest = button.localPosition;
				}
				else
				{
					p.plate = kid.GetChild(0);
				}
				kid.SetParent(transform.parent, true);
			}
			if (p.hub && p.plate)
			{
				spins.Add(p);
			}
			Object.Destroy(gameObject);
		}

		private static Transform Find(Transform root, string name)
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

		private static void Turn()
		{
			if (spunAt == Time.fixedTime)
			{
				return;
			}
			spunAt = Time.fixedTime;
			Player me = Player.main;
			for (int i = spins.Count - 1; i >= 0; i--)
			{
				Spin p = spins[i];
				if (!p.hub || !p.plate)
				{
					if (p.joint)
					{
						Object.Destroy(p.joint);
					}
					spins.RemoveAt(i);
					continue;
				}
				if (!p.joint)
				{
					Fit(p);
				}
				Poke(p, me);
				if (!p.joint)
				{
					continue;
				}
				float drive = p.driver ? p.drive : (p.toy ? p.sign * ToyDrive : 0f);
				float target = drive * Mathf.Min(TipSpeed / Mathf.Max(p.reach, 0.05f) * 57.29578f, MaxSpin);
				float force = drive == 0f ? Brake : Torque;
				JointMotor motor = p.joint.motor;
				if (Mathf.Abs(motor.targetVelocity - target) > 0.5f || motor.force != force)
				{
					motor.targetVelocity = target;
					motor.force = force;
					motor.freeSpin = false;
					p.joint.motor = motor;
				}
				if (drive != 0f)
				{
					Rigidbody body = p.joint.GetComponent<Rigidbody>();
					if (body && body.IsSleeping())
					{
						body.WakeUp();
					}
					if (p.joint.connectedBody && p.joint.connectedBody.IsSleeping())
					{
						p.joint.connectedBody.WakeUp();
					}
				}
			}
		}

		private static void Rejoin()
		{
			foreach (Spin p in spins)
			{
				if (p.hub && p.plate)
				{
					Fit(p);
				}
			}
		}

		private static void Fit(Spin p)
		{
			Sandbox a = p.hub.parent ? p.hub.parent.GetComponent<Sandbox>() : null;
			Sandbox b = p.plate.parent ? p.plate.parent.GetComponent<Sandbox>() : null;
			Rigidbody bodyA = a ? a.GetComponent<Rigidbody>() : null;
			Rigidbody bodyB = b ? b.GetComponent<Rigidbody>() : null;
			bool broken = !a || !b || a == b || !bodyA || !bodyB;
			if (p.joint)
			{
				if (broken || p.joint.gameObject != b.gameObject || p.joint.connectedBody != bodyA)
				{
					Object.DestroyImmediate(p.joint);
					p.joint = null;
					if (broken)
					{
						return;
					}
				}
			}
			else if (broken)
			{
				return;
			}
			p.reach = Radius(b, p.plate);
			if (p.joint)
			{
				return;
			}
			HingeJoint joint = b.gameObject.AddComponent<HingeJoint>();
			joint.connectedBody = bodyA;
			joint.anchor = b.transform.InverseTransformPoint(p.plate.position);
			joint.axis = b.transform.InverseTransformDirection(p.plate.up);
			joint.autoConfigureConnectedAnchor = true;
			joint.enableCollision = false;
			joint.useMotor = true;
			JointMotor motor = default;
			motor.targetVelocity = 0f;
			motor.force = Brake;
			motor.freeSpin = false;
			joint.motor = motor;
			p.joint = joint;
		}

		private static float Radius(Sandbox b, Transform plate)
		{
			float radius = 0.05f;
			foreach (Renderer r in b.GetComponentsInChildren<Renderer>())
			{
				Bounds bounds = r.bounds;
				for (int c = 0; c < 8; c++)
				{
					Vector3 corner = bounds.center + new Vector3((c & 1) == 0 ? -bounds.extents.x : bounds.extents.x, (c & 2) == 0 ? -bounds.extents.y : bounds.extents.y, (c & 4) == 0 ? -bounds.extents.z : bounds.extents.z);
					radius = Mathf.Max(radius, Vector3.ProjectOnPlane(corner - plate.position, plate.up).magnitude);
				}
			}
			return radius;
		}

		private static void Poke(Spin p, Player me)
		{
			if (!p.button || !me)
			{
				return;
			}
			PhysicsScene scene = p.hub.gameObject.scene.GetPhysicsScene();
			int n = scene.OverlapBox(p.button.position + p.button.up * 0.03f, new Vector3(0.05f, 0.04f, 0.05f), near, p.button.rotation, -1, QueryTriggerInteraction.Collide);
			bool touched = false;
			for (int i = 0; i < n; i++)
			{
				if (near[i].GetComponentInParent<Player>() == me)
				{
					touched = true;
					break;
				}
			}
			if (touched && !p.poked)
			{
				p.toy = !p.toy;
				p.button.localPosition = p.rest - (p.toy ? p.button.parent.InverseTransformVector(p.button.up * PressDepth) : Vector3.zero);
				Player.Note("motor " + (p.toy ? "switched on" : "switched off"));
			}
			p.poked = touched;
		}

		private static float Steer(bool held, InputDevice d)
		{
			if (!held || !d.isValid || !d.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion turn))
			{
				return 0f;
			}
			Vector3 side = turn * Vector3.right;
			float roll = Mathf.Asin(Mathf.Clamp(-side.y, -1f, 1f)) * 57.29578f;
			float amount = Mathf.Clamp01((Mathf.Abs(roll) - SteerDead) / (SteerFull - SteerDead));
			return roll < 0f ? -amount : amount;
		}

		private void Remote(bool held, float power, bool tap, XRNode hand, float steer)
		{
			RaycastHit h = default;
			Sandbox s = held && Ray(RemoteReach, false, out h) ? Of(h.collider) : null;
			List<Spin> found = s && s.kind == Kind.Part ? Motors(s, h.collider) : null;
			bool aim = found != null && found.Count > 0;
			if (dot)
			{
				if (dot.gameObject.activeSelf != aim)
				{
					dot.gameObject.SetActive(aim);
				}
				if (aim)
				{
					dot.position = h.point;
				}
			}
			if (tap)
			{
				if (aim)
				{
					Pair(found, hand);
				}
				else
				{
					reverse = !reverse;
					if (goLight)
					{
						goLight.gameObject.SetActive(!reverse);
					}
					if (backLight)
					{
						backLight.gameObject.SetActive(reverse);
					}
					Player.Hand(hand, 0.25f, 0.05f);
				}
			}
			paired.RemoveAll((Spin p) => !spins.Contains(p));
			float drive = power > PowerDead && held ? (reverse ? -power : power) : 0f;
			foreach (Spin p in paired)
			{
				float lean = Lean(p);
				if (held && (drive != 0f || (steer != 0f && lean != 0f)))
				{
					p.driver = this;
					p.drive = Mathf.Clamp(drive - lean * steer * SteerPower, -1f, 1f) * p.sign;
				}
				else if (p.driver == this)
				{
					p.drive = 0f;
					p.driver = null;
				}
			}
			Cables(held);
		}

		private static float Lean(Spin p)
		{
			Sandbox car = p.hub.parent ? p.hub.parent.GetComponent<Sandbox>() : null;
			if (!car || !car.rb)
			{
				return 0f;
			}
			Vector3 ahead = Vector3.ProjectOnPlane(p.hub.TransformDirection(p.ahead), Vector3.up);
			if (ahead.sqrMagnitude < 0.0001f)
			{
				return 0f;
			}
			Vector3 off = p.plate.position - car.rb.worldCenterOfMass;
			float side = Vector3.Dot(Vector3.Cross(Vector3.up, ahead.normalized), off);
			if (Mathf.Abs(side) < SideMin)
			{
				return 0f;
			}
			return Mathf.Sign(side);
		}

		private static List<Spin> Motors(Sandbox s, Collider c)
		{
			List<Spin> list = new List<Spin>();
			int piece = s.PieceOf(c);
			Transform plate = piece >= 0 ? s.pieces[piece].t : null;
			foreach (Spin p in spins)
			{
				if (p.plate == plate)
				{
					list.Add(p);
				}
			}
			if (list.Count > 0)
			{
				return list;
			}
			foreach (Spin p in spins)
			{
				if (p.hub && p.hub.parent == s.transform)
				{
					list.Add(p);
				}
			}
			if (list.Count > 0)
			{
				return list;
			}
			foreach (Spin p in spins)
			{
				if (p.plate && p.plate.parent == s.transform)
				{
					list.Add(p);
				}
			}
			return list;
		}

		private void Pair(List<Spin> list, XRNode hand)
		{
			bool all = list.TrueForAll((Spin p) => paired.Contains(p));
			Player me = Player.main;
			Vector3 ahead = me && me.cam ? Vector3.ProjectOnPlane(me.cam.forward, Vector3.up) : Vector3.forward;
			foreach (Spin p in list)
			{
				if (all)
				{
					paired.Remove(p);
					if (p.driver == this)
					{
						p.drive = 0f;
						p.driver = null;
					}
				}
				else if (!paired.Contains(p))
				{
					paired.Add(p);
					p.sign = Mathf.Sign(Vector3.Dot(ahead, Vector3.Cross(p.plate.up, Vector3.up)));
					p.ahead = p.hub.InverseTransformDirection(ahead);
				}
			}
			Player.Hand(hand, all ? 0.3f : 0.6f, 0.08f);
			Player.Note((all ? "unpaired " : "paired ") + list.Count + " motor" + (list.Count == 1 ? "" : "s"));
		}

		private void Cables(bool held)
		{
			if (!cable || !tip)
			{
				return;
			}
			while (cables.Count < paired.Count)
			{
				cables.Add(Object.Instantiate(cable, cable.parent));
			}
			for (int i = 0; i < cables.Count; i++)
			{
				if (held && i < paired.Count && paired[i].hub)
				{
					Stretch(cables[i], tip.position, paired[i].hub.position);
				}
				else if (cables[i].gameObject.activeSelf)
				{
					cables[i].gameObject.SetActive(false);
				}
			}
		}
	}
}
