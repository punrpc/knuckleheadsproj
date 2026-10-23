using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

namespace Ragdoll
{
	[DefaultExecutionOrder(50)]
	public class Tutorial : MonoBehaviour
	{
		public enum Kind
		{
			Reach = 0,
			Run = 1,
			Jump = 2,
			ArmJump = 3,
			Vault = 4,
			Climb = 5,
			Lift = 6,
			Kick = 7,
			Punch = 8,
			Block = 9
		}

		[Serializable]
		public class Step
		{
			public Kind kind;

			[TextArea]
			public string text;

			public int count;

			public Collider area;

			public Transform target;

			public TMP_Text sign;

			public GameObject gate;

			public Transform checkpoint;
		}

		[Header("Steps")]
		public Step[] steps;

		[Header("Feedback")]
		public AudioSource ding;

		public GameObject poof;

		[HideInInspector]
		public int at;

		[HideInInspector]
		public int got;

		[HideInInspector]
		public bool finished;

		private const string Menu = "MainMenu";

		private readonly Color todo = new Color(1f, 1f, 1f, 1f);

		private readonly Color done = new Color(0.35f, 1f, 0.45f, 1f);

		private float runRatio = 1.4f;

		private float armJumpBoost = 1.1f;

		private float liftRise = 0.15f;

		private float kickLegs = 0.3f;

		private float fistReach = 0.1f;

		private float gateTime = 1f;

		private float swingRest = 0.45f;

		private float swingWind = 0.35f;

		private float swingStrike = 0.12f;

		private float swingBack = 0.25f;

		private float swingPull = 0.15f;

		private float dodgeGap = 0.3f;

		private float swingImpulse = 6f;

		private float swingSpeed = 5f;

		private float crateMass = 50f;

		private float crateCarry = 25f;

		private readonly List<Rigidbody> crates = new List<Rigidbody>();

		private Player p;

		private bool replay;

		private bool posed;

		private Vector3 pelvis0;

		private Vector3 rig0;

		private Quaternion rigRot0;

		private Quaternion pelvisRot0;

		private float lift0;

		private float run;

		private bool seen;

		private bool counted;

		private int jumps;

		private float liftFrom;

		private Vector3 lastPelvis;

		private Collider[] targetCols = new Collider[0];

		private Collider legL;

		private Collider legR;

		private HandGrab handL;

		private HandGrab handR;

		private readonly Vector3[] histL = new Vector3[3];

		private readonly Vector3[] histR = new Vector3[3];

		private int histAt;

		private bool touchL;

		private bool touchR;

		private float stepFrom;

		private float tutorialFrom;

		private bool left;

		private bool awaitingLeave;

		private TextMeshPro hud;

		private bool hudPlaced;

		private bool stepDone;

		private float freshAt = -9f;

		private const float HudDistance = 0.85f;

		private const float HudWidth = 0.95f;

		private const float HudHeight = 0.4f;

		private const float HudTall = 0.66f;

		private const float HudDrop = 0.12f;

		private const float RestDistance = 1.25f;

		private const float RestDrop = 0.55f;

		private const float RestScale = 0.75f;

		private const float FreshTime = 2f;

		private const float HudFollow = 9f;

		private const float ReachMargin = 0.5f;

		private string shown;

		private Vector3 gloveHome;

		private Vector3 aim;

		private Vector3 wound;

		private float swingAt;

		private int swingPhase;

		private Transform fistPivot;

		private bool fistMade;

		private float driftSince = -1f;

		private float driftSeen = -99f;

		private float driftDoneAt = -99f;

		private Vector2 driftFrom;

		private bool driftX;

		private bool driftY;

		private bool driftDone;

		private bool driftPress;

		private string shownExtra = "";

		private const float SkipAfter = 600f;

		private const float DriftTouch = 1.5f;

		private const float DriftSteady = 8f;

		private const float DriftStill = 0.03f;

		private const float DriftShow = 10f;

		private const float DriftThanks = 4f;

		private void Start()
		{
			p = Player.main;
			Prefs.Load();
			replay = Prefs.tutorialDone;
			if (ding)
			{
				Prefs.Track(ding);
			}
			if (p)
			{
				if (p.leftLowerLeg)
				{
					legL = p.leftLowerLeg.GetComponent<Collider>();
				}
				if (p.rightLowerLeg)
				{
					legR = p.rightLowerLeg.GetComponent<Collider>();
				}
				if (p.leftHand)
				{
					handL = p.leftHand.GetComponent<HandGrab>();
				}
				if (p.rightHand)
				{
					handR = p.rightHand.GetComponent<HandGrab>();
				}
			}
			foreach (Step step in steps)
			{
				if (step.sign)
				{
					step.sign.gameObject.SetActive(false);
				}
			}
			foreach (NetProp prop in UnityEngine.Object.FindObjectsByType<NetProp>(FindObjectsSortMode.None))
			{
				if (prop.gameObject.scene != gameObject.scene || !prop.name.StartsWith("Crate"))
				{
					continue;
				}
				Rigidbody rb = prop.GetComponent<Rigidbody>();
				if (rb)
				{
					rb.mass = crateMass;
					crates.Add(rb);
				}
			}
			tutorialFrom = Time.time;
			MakeHud();
			if (steps.Length != 0)
			{
				Begin(0);
			}
		}

		private void FixedUpdate()
		{
			if (!p || finished || at >= steps.Length)
			{
				return;
			}
			foreach (Rigidbody crate in crates)
			{
				if (crate)
				{
					crate.mass = Carrying(crate) ? crateCarry : crateMass;
				}
			}
			if (!posed && p.spawnKnown && p.grounded)
			{
				Pose();
			}
			if (p.drowned || p.ghost || !p.pelvis)
			{
				return;
			}
			int reached = Reached();
			if (reached > at)
			{
				Player.Note("tutorial reached station " + reached + " " + steps[reached].kind);
				Begin(reached);
			}
			else if (!stepDone && posed && Ahead())
			{
				p.SendHome("tutorial: finish station " + at + " " + steps[at].kind + " first");
			}
			if (stepDone)
			{
				return;
			}
			Step s = steps[at];
			Vector3 pos = p.pelvis.position;
			bool inside = !s.area || In(s.area, pos);
			switch (s.kind)
			{
			case Kind.Reach:
				if (s.area && In(s.area, pos))
				{
					Pass();
				}
				break;
			case Kind.Run:
			{
				Vector3 moved = pos - lastPelvis;
				moved.y = 0f;
				if (inside && p.grounded && p.moveWish.magnitude >= p.speed * runRatio)
				{
					run += moved.magnitude;
					got = (int)run;
					if (run >= Need(s))
					{
						Pass();
					}
				}
				break;
			}
			case Kind.Jump:
			case Kind.ArmJump:
				if (p.jumpCount != jumps)
				{
					jumps = p.jumpCount;
					if (inside && (s.kind == Kind.Jump || p.jumpBoost >= armJumpBoost))
					{
						Count();
					}
				}
				break;
			case Kind.Vault:
				if (p.ledgeL || p.ledgeR)
				{
					seen = true;
				}
				if (seen && p.grounded && s.area && In(s.area, pos))
				{
					Pass();
				}
				break;
			case Kind.Climb:
				if (p.hanging && p.Grabbing)
				{
					seen = true;
				}
				if (seen && p.grounded && s.area && In(s.area, pos))
				{
					Pass();
				}
				break;
			case Kind.Lift:
				if (!p.Lifting)
				{
					seen = false;
					break;
				}
				if (!seen)
				{
					seen = true;
					liftFrom = pos.y;
				}
				if (!counted && inside && pos.y - liftFrom >= liftRise)
				{
					counted = true;
					Count();
				}
				break;
			case Kind.Kick:
				if (Prefs.noKick)
				{
					Pass();
					break;
				}
				if (p.kicking && p.airLegs >= kickLegs && !p.hanging && !counted && (targetCols.Length == 0 || Kicks()))
				{
					counted = true;
					Count();
				}
				if (p.grounded && !p.kicking)
				{
					counted = false;
				}
				break;
			case Kind.Punch:
				if (inside)
				{
					Fist(p.leftHand, histL, ref touchL, XRNode.LeftHand);
					Fist(p.rightHand, histR, ref touchR, XRNode.RightHand);
				}
				histAt = (histAt + 1) % histL.Length;
				break;
			}
			lastPelvis = pos;
		}

		private void Update()
		{
			if (!p)
			{
				return;
			}
			if (!fistMade)
			{
				Fist();
			}
			if (finished)
			{
				if (!left && p.dark >= 0.99f)
				{
					left = true;
					NetworkManager.Load(Menu);
				}
				return;
			}
			if (at >= steps.Length)
			{
				return;
			}
			Step s = steps[at];
			if (s.kind == Kind.Block && !stepDone)
			{
				Swing(s);
			}
			Drift();
			if (at < steps.Length)
			{
				Sign(s);
			}
		}

		private int Reached()
		{
			if (stepDone && at + 1 < steps.Length)
			{
				Transform cp = steps[at + 1].checkpoint;
				if (!cp || Past(cp))
				{
					return at + 1;
				}
			}
			return at;
		}

		private bool Carrying(Rigidbody crate)
		{
			if (p.Holds(crate) && p.grounded && !p.hanging && p.head && !Reaching(handL, crate))
			{
				return !Reaching(handR, crate);
			}
			return false;
		}

		private bool Reaching(HandGrab hand, Rigidbody crate)
		{
			if (hand && hand.grabbing && hand.held == crate)
			{
				return hand.transform.position.y > p.head.position.y;
			}
			return false;
		}

		private bool Ahead()
		{
			for (int i = at + 1; i < steps.Length; i++)
			{
				Transform cp = steps[i].checkpoint;
				if (cp && Past(cp))
				{
					return true;
				}
			}
			return false;
		}

		private bool Past(Transform cp)
		{
			if (Vector3.Dot(p.pelvis.position - cp.position, cp.forward) >= -ReachMargin)
			{
				return p.pelvis.position.y >= cp.position.y - 1f;
			}
			return false;
		}

		private void Begin(int i)
		{
			at = i;
			got = 0;
			run = 0f;
			seen = false;
			counted = false;
			touchL = false;
			touchR = false;
			stepDone = false;
			freshAt = Time.time;
			shown = null;
			stepFrom = Time.time;
			Step s = steps[i];
			if (p)
			{
				jumps = p.jumpCount;
				if (p.pelvis)
				{
					lastPelvis = p.pelvis.position;
				}
			}
			targetCols = s.target ? s.target.GetComponentsInChildren<Collider>(true) : new Collider[0];
			if (s.kind == Kind.Block && s.target)
			{
				gloveHome = s.target.position;
				swingPhase = 0;
				swingAt = Time.time;
			}
			if ((s.kind == Kind.Reach || s.kind == Kind.Vault || s.kind == Kind.Climb) && !s.area)
			{
				Debug.LogWarning("Tutorial step " + i + " (" + s.kind + ") needs an area to finish");
			}
			Checkpoint(s.checkpoint);
		}

		private void MakeHud()
		{
			hud = new GameObject("Tutorial Text").AddComponent<TextMeshPro>();
			TMP_Text source = null;
			foreach (Step step in steps)
			{
				if (!source && step.sign)
				{
					source = step.sign;
				}
			}
			if (!source && GameManager.main)
			{
				source = GameManager.main.title;
			}
			if (source)
			{
				hud.font = source.font;
				hud.fontSharedMaterial = source.fontSharedMaterial;
			}
			hud.rectTransform.sizeDelta = new Vector2(HudWidth, HudHeight);
			hud.enableAutoSizing = true;
			hud.fontSizeMin = 0.3f;
			hud.fontSizeMax = 0.6f;
			hud.fontStyle = FontStyles.Bold;
			hud.alignment = TextAlignmentOptions.Center;
			hud.color = todo;
			hud.text = "";
		}

		private void LateUpdate()
		{
			if (!hud || !p || !p.cam)
			{
				return;
			}
			Transform cam = p.cam;
			Vector3 forward = cam.forward;
			if (forward.x * forward.x + forward.z * forward.z < 0.0001f)
			{
				forward = cam.up;
			}
			forward = new Vector3(forward.x, 0f, forward.z).normalized;
			float age = Time.time - freshAt;
			bool fresh = age < FreshTime;
			Vector3 want = cam.position + forward * (fresh ? HudDistance : RestDistance) + Vector3.down * (fresh ? HudDrop : RestDrop);
			Transform t = hud.transform;
			float follow = 1f - Mathf.Exp(-HudFollow * Time.deltaTime);
			if (!hudPlaced || (t.position - want).sqrMagnitude > 9f)
			{
				hudPlaced = true;
				t.position = want;
			}
			else
			{
				t.position = Vector3.Lerp(t.position, want, follow);
			}
			t.localScale = Vector3.one * Mathf.Lerp(t.localScale.x, fresh ? 1f : RestScale, follow);
			Vector3 look = t.position - cam.position;
			if (look.sqrMagnitude > 0.0001f)
			{
				t.rotation = Quaternion.LookRotation(look.normalized, Vector3.up);
			}
		}

		private int Need(Step s)
		{
			return Mathf.Max(1, s.count);
		}

		private void Count()
		{
			got++;
			Player.Both(0.2f, 0.05f);
			if (got >= Need(steps[at]))
			{
				Pass();
			}
		}

		private void Pass()
		{
			if (finished || at >= steps.Length || stepDone)
			{
				return;
			}
			Step s = steps[at];
			stepDone = true;
			freshAt = Time.time;
			if (ding)
			{
				ding.Play();
			}
			Player.Both(0.5f, 0.1f);
			if (s.gate)
			{
				StartCoroutine(Open(s.gate));
			}
			Player.Note("tutorial step " + at + " " + s.kind + " done after " + (Time.time - stepFrom).ToString("F0") + " s");
			if (s.kind == Kind.Block && s.target)
			{
				s.target.position = gloveHome;
			}
			if (at + 1 >= steps.Length)
			{
				at = steps.Length;
				awaitingLeave = true;
				if (hud)
				{
					hud.text = "WELL DONE!\nHOLD THE LEFT MENU BUTTON TO PLAY";
					hud.color = todo;
				}
				Player.Note("tutorial steps all done, waiting for the menu button hold");
			}
		}

		public bool Leave()
		{
			if (finished)
			{
				return false;
			}
			if (!awaitingLeave)
			{
				Player.Note("tutorial left at station " + at + ((at < steps.Length) ? (" " + steps[at].kind) : "") + (replay ? " (replay)" : " (skipped)"));
			}
			Finish();
			return true;
		}

		private void Drift()
		{
			InputDevice device = InputDevices.GetDeviceAtXRNode(Prefs.swapSticks ? XRNode.LeftHand : XRNode.RightHand);
			bool press = Button(XRNode.LeftHand) || Button(XRNode.RightHand);
			if (press && !driftPress && Asking())
			{
				driftDone = true;
				driftDoneAt = Time.time;
				Prefs.Load();
				if (driftX)
				{
					Prefs.noTurn = true;
				}
				if (driftY)
				{
					Prefs.noKick = true;
				}
				Prefs.Save();
				Player.Note("stick drift: turned off" + (driftX ? " turning" : "") + (driftY ? " dropkicks" : ""));
			}
			driftPress = press;
			if (driftDone || !device.isValid || !device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
			{
				return;
			}
			bool x = !Prefs.noTurn && Mathf.Abs(axis.x) > Prefs.stickDead;
			bool y = !Prefs.noKick && Mathf.Abs(axis.y) > Prefs.stickDead;
			bool canTouch = device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool touching);
			if (!(x || y) || (canTouch ? touching : (driftSince >= 0f && (axis - driftFrom).sqrMagnitude > DriftStill * DriftStill)))
			{
				driftSince = -1f;
				return;
			}
			if (driftSince < 0f)
			{
				driftSince = Time.time;
				driftFrom = axis;
				return;
			}
			if (Time.time - driftSince < (canTouch ? DriftTouch : DriftSteady))
			{
				return;
			}
			if (!driftX && !driftY)
			{
				Player.Note("stick drift: stick at " + axis.ToString("F2") + (canTouch ? " with the thumb off it" : " and not moving"));
			}
			driftX |= x;
			driftY |= y;
			driftSeen = Time.time;
		}

		private bool Asking()
		{
			if (driftDone)
			{
				return false;
			}
			if (driftX || driftY)
			{
				return Time.time - driftSeen < DriftShow;
			}
			return false;
		}

		private static bool Button(XRNode node)
		{
			InputDevice device = InputDevices.GetDeviceAtXRNode(node);
			if (!device.isValid)
			{
				return false;
			}
			return device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool pressed) && pressed;
		}

		private string Extra()
		{
			string what = driftX ? (driftY ? "TURNING AND DROPKICKS" : "TURNING") : "DROPKICKS";
			if (driftDone && Time.time - driftDoneAt < DriftThanks)
			{
				return what + " TURNED OFF\nCHANGE IT IN SETTINGS";
			}
			if (Asking())
			{
				return "STICK DRIFT?\nPRESS B TO TURN OFF " + what + "\nCHANGE IT LATER IN SETTINGS";
			}
			if (Time.time - tutorialFrom >= SkipAfter)
			{
				return "TAKING A WHILE?\nHOLD THE LEFT MENU BUTTON\nTO SKIP THE TUTORIAL AND PLAY";
			}
			return "";
		}

		[ContextMenu("Skip Step")]
		public void Skip()
		{
			if (!finished && at < steps.Length)
			{
				Player.Note("tutorial step " + at + " " + steps[at].kind + " skipped after " + (Time.time - stepFrom).ToString("F0") + " s");
				Pass();
			}
		}

		private void Finish()
		{
			finished = true;
			Prefs.Load();
			Prefs.tutorialDone = true;
			Prefs.Save();
			if (p)
			{
				p.blackout = 1f;
			}
			Player.Note("tutorial finished after " + (Time.time - tutorialFrom).ToString("F0") + " s" + (replay ? " (replay)" : ""));
		}

		private IEnumerator Open(GameObject gate)
		{
			Collider[] cols = gate.GetComponentsInChildren<Collider>();
			Bounds bounds = new Bounds(gate.transform.position, Vector3.zero);
			bool any = false;
			foreach (Collider col in cols)
			{
				if (any)
				{
					bounds.Encapsulate(col.bounds);
				}
				else
				{
					bounds = col.bounds;
				}
				any = true;
				col.enabled = false;
			}
			if (poof)
			{
				UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate(poof, bounds.center, Quaternion.identity), 3f);
			}
			Vector3 from = gate.transform.position;
			Vector3 to = from + Vector3.down * ((cols.Length > 0) ? bounds.size.y : 2f);
			for (float t = 0f; t < gateTime; t += Time.deltaTime)
			{
				gate.transform.position = Vector3.Lerp(from, to, t / gateTime);
				yield return null;
			}
			gate.SetActive(false);
		}

		private void Pose()
		{
			posed = true;
			pelvis0 = p.spawnPelvis;
			rig0 = p.spawnRig;
			rigRot0 = p.spawnRigRot;
			pelvisRot0 = p.spawnPelvisRot;
			lift0 = p.spawnPelvis.y - p.floorY;
			if (at < steps.Length)
			{
				Checkpoint(steps[at].checkpoint);
			}
		}

		private void Checkpoint(Transform cp)
		{
			if (!cp || !posed || !p)
			{
				return;
			}
			Quaternion turn = Quaternion.Euler(0f, cp.eulerAngles.y - rigRot0.eulerAngles.y, 0f);
			p.spawnPelvis = cp.position + Vector3.up * lift0;
			p.spawnRig = p.spawnPelvis + turn * (rig0 - pelvis0);
			p.spawnRigRot = turn * rigRot0;
			p.spawnPelvisRot = turn * pelvisRot0;
		}

		private static bool In(Collider area, Vector3 pos)
		{
			return (area.ClosestPoint(pos) - pos).sqrMagnitude < 0.0001f;
		}

		private bool Kicks()
		{
			foreach (Collider leg in new Collider[2] { legL, legR })
			{
				if (!leg || !leg.enabled)
				{
					continue;
				}
				Bounds b = leg.bounds;
				b.Expand(0.1f);
				foreach (Collider col in targetCols)
				{
					if (col && col.enabled && col.gameObject.activeInHierarchy && b.Intersects(col.bounds))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void Fist(Rigidbody hand, Vector3[] hist, ref bool touching, XRNode node)
		{
			if (!hand)
			{
				return;
			}
			Vector3 center = hand.worldCenterOfMass;
			bool hit = false;
			Vector3 point = default(Vector3);
			foreach (Collider col in targetCols)
			{
				if (col && col.enabled && col.gameObject.activeInHierarchy)
				{
					Vector3 q = col.ClosestPoint(center);
					if ((q - center).sqrMagnitude < fistReach * fistReach)
					{
						hit = true;
						point = q;
					}
				}
			}
			if (hit && !touching)
			{
				Vector3 sum = Vector3.zero;
				foreach (Vector3 v in hist)
				{
					sum += v;
				}
				float speed = Mathf.Min((sum / hist.Length).magnitude, p.SwingSpeed(hand, 6));
				if (speed >= 3.5f)
				{
					Count();
					Player.HitAt(point, true);
					Player.Hand(node, Mathf.Lerp(0.2f, 0.7f, Mathf.InverseLerp(2f, 7f, speed)), 0.05f);
				}
			}
			touching = hit;
			hist[histAt] = hand.linearVelocity;
		}

		private void Fist()
		{
			Step block = null;
			foreach (Step step in steps)
			{
				if (step.kind == Kind.Block && step.target)
				{
					block = step;
				}
			}
			if (block == null)
			{
				fistMade = true;
				return;
			}
			if (!fistPivot)
			{
				fistPivot = new GameObject("Fist Pivot").transform;
				fistPivot.SetParent(block.target, false);
			}
			GameObject fist = p.GhostFist(fistPivot, false);
			if (!fist)
			{
				return;
			}
			Renderer r = fist.GetComponentInChildren<Renderer>();
			Vector3 c = r ? fistPivot.InverseTransformPoint(r.bounds.center) : Vector3.zero;
			if (c.sqrMagnitude > 1E-06f)
			{
				fist.transform.localRotation = Quaternion.FromToRotation(c, Vector3.forward) * fist.transform.localRotation;
				fist.transform.localPosition = -Vector3.forward * c.magnitude;
			}
			fistMade = true;
			foreach (Renderer renderer in block.target.GetComponents<Renderer>())
			{
				renderer.enabled = false;
			}
		}

		private void Swing(Step s)
		{
			if (!s.target || !p.head)
			{
				return;
			}
			Transform glove = s.target;
			if (fistPivot)
			{
				fistPivot.rotation = Quaternion.LookRotation(p.head.position - glove.position, Vector3.up);
			}
			bool live = (!s.area || In(s.area, p.pelvis.position)) && !p.knockedOut && !p.dead && !p.flopped && !p.rising && !p.drowned;
			float t = Time.time - swingAt;
			switch (swingPhase)
			{
			case 0:
				glove.position = Vector3.MoveTowards(glove.position, gloveHome, 2f * Time.deltaTime);
				if (!live)
				{
					swingAt = Time.time;
				}
				else if (t >= swingRest)
				{
					swingPhase = 1;
					swingAt = Time.time;
					aim = p.head.position;
					wound = gloveHome + (gloveHome - aim).normalized * swingPull;
				}
				break;
			case 1:
				aim = p.head.position;
				wound = gloveHome + (gloveHome - aim).normalized * swingPull;
				glove.position = Vector3.Lerp(gloveHome, wound, t / swingWind);
				if (t >= swingWind)
				{
					swingPhase = 2;
					swingAt = Time.time;
				}
				break;
			case 2:
				glove.position = Vector3.Lerp(wound, aim, t / swingStrike);
				if (t >= swingStrike)
				{
					swingPhase = 3;
					swingAt = Time.time;
					Strike(glove.position);
				}
				break;
			default:
				glove.position = Vector3.Lerp(aim, gloveHome, t / swingBack);
				if (t >= swingBack)
				{
					swingPhase = 0;
					swingAt = Time.time;
				}
				break;
			}
		}

		private void Strike(Vector3 from)
		{
			if (Vector3.Distance(p.head.position, aim) > dodgeGap || p.knockedOut || p.dead || p.flopped || p.rising)
			{
				return;
			}
			if (p.WouldKnockOut(p.head, swingImpulse, swingSpeed))
			{
				return;
			}
			switch (p.TakeHit(p.head, swingImpulse, swingSpeed, from))
			{
			case Hit.Hurt:
				Player.HitAt(from, true);
				break;
			case Hit.Blocked:
				Count();
				break;
			}
		}

		private void Sign(Step s)
		{
			if (!hud)
			{
				return;
			}
			string text = Words(s.text);
			if (Need(s) >= 2)
			{
				text = text + "\n" + Mathf.Min(got, Need(s)) + "/" + Need(s);
			}
			if (stepDone && at + 1 < steps.Length)
			{
				text = "NICE!\nTHE WALL IS OPEN, KEEP GOING";
			}
			string extra = Extra();
			if (extra != shownExtra)
			{
				if (extra != "")
				{
					freshAt = Time.time;
				}
				shownExtra = extra;
			}
			if (extra != "")
			{
				text = text + "\n<size=85%><color=#FFD54A>" + extra + "</color></size>";
			}
			hud.rectTransform.sizeDelta = new Vector2(HudWidth, (extra != "") ? HudTall : HudHeight);
			if (text != shown)
			{
				shown = text;
				hud.text = text;
			}
			hud.color = (stepDone || (s.kind == Kind.Block && p.guarding)) ? done : todo;
		}

		private static string Words(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return "";
			}
			string move = Prefs.armWalk ? "SWING YOUR ARMS" : (Prefs.swapSticks ? "PUSH THE RIGHT STICK" : "PUSH THE LEFT STICK");
			string stick = Prefs.swapSticks ? "LEFT STICK" : "RIGHT STICK";
			if (Prefs.noTurn)
			{
				text = text.Replace("TURN WITH THE {STICK}", "TURN AROUND");
			}
			return text.Replace("{MOVE}", move).Replace("{STICK}", stick).ToUpperInvariant();
		}
	}
}
