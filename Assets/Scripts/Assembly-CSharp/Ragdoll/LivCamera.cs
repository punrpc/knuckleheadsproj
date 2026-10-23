using Liv.Lck;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Tablet;
using Liv.Lck.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace Ragdoll
{
	[DefaultExecutionOrder(100)]
	public class LivCamera : MonoBehaviour
	{
		[Header("Setup")]
		public LckQualityConfig quality;

		public const int Layer = 7;

		public const int TouchLayer = 9;

		public const int HandleLayer = 10;

		public static LivCamera main;

		public static Camera firstPerson;

		private Rigidbody rb;

		private LCKCameraController controller;

		private LckTabletFollow follow;

		private ILckService lck;

		private Player bound;

		private Vector3 lastHead;

		private bool held;

		private bool preview = true;

		private readonly GameObject[] down = new GameObject[2];

		private readonly bool[] holdDown = new bool[2];

		private readonly Vector3[] was = new Vector3[2];

		private readonly bool[] had = new bool[2];

		private readonly Transform[] ends = new Transform[2];

		private const float TipRadius = 0.013f;

		private const float Release = 0.025f;

		public static bool Works()
		{
			GraphicsDeviceType gfx = SystemInfo.graphicsDeviceType;
			if (Application.platform == RuntimePlatform.Android)
			{
				return gfx == GraphicsDeviceType.Vulkan || gfx == GraphicsDeviceType.OpenGLES3;
			}
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
			{
				return gfx == GraphicsDeviceType.Vulkan || gfx == GraphicsDeviceType.Direct3D11 || gfx == GraphicsDeviceType.OpenGLCore;
			}
			return false;
		}

		public static int State(Transform spot)
		{
			if (!main)
			{
				return Works() ? 0 : (-1);
			}
			if (!spot)
			{
				return 1;
			}
			return ((main.rb.position - spot.position).sqrMagnitude < 0.0025f) ? 2 : 1;
		}

		public static void Press(GameObject prefab, Transform spot)
		{
			if (!prefab || !spot || ((bool)main && main.held))
			{
				return;
			}
			int state = State(spot);
			if (state < 0)
			{
				return;
			}
			if (state == 1)
			{
				main.Move(spot.position, spot.rotation);
				return;
			}
			if (state == 2)
			{
				Object.Destroy(main.gameObject);
				main = null;
				return;
			}
			Player me = Player.main;
			if ((bool)me && (bool)me.cam)
			{
				Ear(me.cam);
			}
			if (!LckDiContainer.Instance.HasService<ILckService>())
			{
				LckServiceInitializer.ConfigureServices(LckDiContainer.Instance, prefab.GetComponent<LivCamera>().quality, null);
			}
			Object.Instantiate(prefab, spot.position, spot.rotation);
		}

		private static void Ear(Transform cam)
		{
			AudioListener listener = cam.GetComponent<AudioListener>();
			if ((bool)listener && !listener.GetComponent<LivEar>())
			{
				listener.gameObject.AddComponent<LivEar>();
			}
		}

		private void Awake()
		{
			main = this;
			Object.DontDestroyOnLoad(base.gameObject);
			rb = GetComponent<Rigidbody>();
			controller = GetComponent<LCKCameraController>();
			follow = GetComponent<LckTabletFollow>();
			lck = Works() ? LckDiContainer.Instance.GetService<ILckService>() : null;
			if (lck != null)
			{
				lck.SetPreviewActive(true);
			}
			Transform fp = base.transform.Find("Cameras/First Person Camera");
			firstPerson = fp ? fp.GetComponentInChildren<Camera>(true) : null;
			LCKCameraController.ColliderButtonsInUse = false;
		}

		private void OnDestroy()
		{
			if (main == this)
			{
				main = null;
				firstPerson = null;
			}
			LCKCameraController.ColliderButtonsInUse = false;
		}

		private void Update()
		{
			Player me = Player.main;
			if (!me || !me.cam || !me.head)
			{
				return;
			}
			if (me != bound)
			{
				Bind(me);
				return;
			}
			Vector3 head = me.cam.position;
			bool show = held || (rb.position - head).sqrMagnitude < 2.25f;
			if (preview != show && lck != null)
			{
				preview = show;
				lck.SetPreviewActive(show);
			}
			Vector3 delta = head - lastHead;
			lastHead = head;
			if (!held && !(delta.sqrMagnitude < 4f) && !((delta + (rb.position - head)).sqrMagnitude > 9f))
			{
				Move(delta + rb.position, rb.rotation);
			}
		}

		private void FixedUpdate()
		{
			Player me = Player.main;
			if ((bool)me && me == bound)
			{
				Poke(me);
			}
			bool holding = (bool)me && me.Holds(rb);
			if (held != holding)
			{
				held = holding;
				if ((bool)follow)
				{
					follow.enabled = !held;
				}
				rb.constraints = held ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeAll;
				rb.linearVelocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				LCKCameraController.ColliderButtonsInUse = false;
			}
		}

		private void Bind(Player me)
		{
			Up(0, false);
			Up(1, false);
			had[0] = false;
			had[1] = false;
			ends[0] = Tip(me.leftHand);
			ends[1] = Tip(me.rightHand);
			bound = me;
			lastHead = me.cam.position;
			held = false;
			if ((bool)follow)
			{
				follow.enabled = true;
			}
			rb.constraints = RigidbodyConstraints.FreezeAll;
			if ((bool)controller)
			{
				controller.HmdTransform = me.cam;
			}
			Ear(me.cam);
			LCKCameraController.ColliderButtonsInUse = false;
			HandTouch touch = GetComponent<HandTouch>();
			if ((bool)touch)
			{
				Object.Destroy(touch);
			}
			MenuTV tv = Object.FindFirstObjectByType<MenuTV>();
			if ((bool)tv && (bool)tv.livSpot)
			{
				Move(tv.livSpot.position, tv.livSpot.rotation);
				return;
			}
			Vector3 head = me.head.position;
			Vector3 forward = Vector3.ProjectOnPlane(me.cam.forward, Vector3.up);
			if (forward.sqrMagnitude < 0.01f)
			{
				forward = Vector3.ProjectOnPlane(me.playSpace.forward, Vector3.up);
			}
			forward = forward.normalized;
			Vector3 pos = head + forward * 0.6f + Vector3.Cross(Vector3.up, forward) * 0.3f + Vector3.down * 0.25f;
			Move(pos, Quaternion.LookRotation(Vector3.ProjectOnPlane(head - pos, Vector3.up)));
		}

		private void Poke(Player me)
		{
			for (int i = 0; i < 2; i++)
			{
				Rigidbody hand = (i == 0) ? me.leftHand : me.rightHand;
				Transform target = (i == 0) ? me.handTargetL : me.handTargetR;
				HandGrab grab = hand ? hand.GetComponent<HandGrab>() : null;
				if (!hand || !target || !ends[i] || ((bool)grab && grab.grabbing))
				{
					Up(i, false);
					had[i] = false;
					continue;
				}
				Vector3 tip = target.position + target.rotation * Quaternion.Euler(me.handKey) * hand.transform.InverseTransformPoint(ends[i].position);
				if ((bool)down[i])
				{
					BoxCollider box = down[i].GetComponent<BoxCollider>();
					if ((bool)box && box.enabled && down[i].activeInHierarchy)
					{
						Vector3 local = box.transform.InverseTransformPoint(tip) - box.center;
						float slack = Release / Mathf.Max(box.transform.lossyScale.x, 1E-05f);
						if (local.z < (0f - box.size.z) * 0.5f - slack || Mathf.Abs(local.x) > slack + box.size.x * 0.5f || Mathf.Abs(local.y) > slack + box.size.y * 0.5f)
						{
							Up(i, true);
						}
					}
					else
					{
						Up(i, false);
					}
				}
				else if (had[i])
				{
					Vector3 from = rb.position + rb.rotation * was[i];
					Vector3 dir = tip - from;
					float d2 = dir.sqrMagnitude;
					if (d2 > 1E-08f && d2 < 0.09f && Physics.SphereCast(from, TipRadius, dir, out RaycastHit hit, Mathf.Sqrt(d2), 1 << Layer, QueryTriggerInteraction.Collide) && Vector3.Dot(hit.normal, hit.collider.transform.forward) < -0.5f)
					{
						Press(i, hit.collider.gameObject, grab);
					}
				}
				was[i] = Quaternion.Inverse(rb.rotation) * (tip - rb.position);
				had[i] = true;
			}
		}

		private void Press(int i, GameObject target, HandGrab grab)
		{
			down[i] = target;
			PointerEventData data = Pointer(target);
			ExecuteEvents.Execute(target, data, ExecuteEvents.pointerEnterHandler);
			ExecuteEvents.Execute(target, data, ExecuteEvents.pointerDownHandler);
			holdDown[i] = target.GetComponent<LckDoubleButtonTrigger>();
			if (!holdDown[i])
			{
				ExecuteEvents.Execute(target, data, ExecuteEvents.pointerUpHandler);
				ExecuteEvents.Execute(target, data, ExecuteEvents.pointerClickHandler);
			}
			if ((bool)grab)
			{
				Player.Hand(grab.node, 0.25f, 0.03f);
			}
		}

		private void Up(int i, bool done)
		{
			GameObject target = down[i];
			if ((bool)target)
			{
				down[i] = null;
				PointerEventData data = Pointer(target);
				if (holdDown[i] && (done || !target.GetComponent<LckStreamButton>()))
				{
					ExecuteEvents.Execute(target, data, ExecuteEvents.pointerUpHandler);
				}
				ExecuteEvents.Execute(target, data, ExecuteEvents.pointerExitHandler);
				holdDown[i] = false;
			}
		}

		private static PointerEventData Pointer(GameObject target)
		{
			return new PointerEventData(EventSystem.current)
			{
				button = PointerEventData.InputButton.Left,
				pointerEnter = target,
				pointerPress = target,
				rawPointerPress = target,
				eligibleForClick = true
			};
		}

		private static Transform Tip(Rigidbody hand)
		{
			if (!hand)
			{
				return null;
			}
			foreach (Transform t in hand.GetComponentsInChildren<Transform>(true))
			{
				if (t.name == "Liv Tip")
				{
					return t;
				}
			}
			return null;
		}

		private void Move(Vector3 pos, Quaternion rot)
		{
			rb.linearVelocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			base.transform.SetPositionAndRotation(pos, rot);
			rb.position = pos;
			rb.rotation = rot;
			Up(0, false);
			Up(1, false);
			had[0] = false;
			had[1] = false;
		}
	}
}
