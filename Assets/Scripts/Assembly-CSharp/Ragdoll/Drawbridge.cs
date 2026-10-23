using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ragdoll
{
	public class Drawbridge : MonoBehaviour
	{
		[Serializable]
		public class Leaf
		{
			public Rigidbody hinge;

			public Vector3 axis = Vector3.right;

			public float sign = 1f;
		}

		[Serializable]
		public class Car
		{
			public Rigidbody body;

			public Renderer paint;

			public Transform[] wheels;
		}

		private class Rider
		{
			public NetProp prop;

			public Rigidbody body;

			public Grabbable grab;

			public int leaf;

			public Vector3 pos;

			public Quaternion rot;
		}

		[Header("Bridge")]
		public Leaf[] leaves;

		public float maxAngle = 70f;

		public float firstRaise = 60f;

		public float period = 60f;

		public float raiseTime = 6f;

		public float holdTime = 30f;

		public float lowerTime = 12f;

		[Header("Boat")]
		public Transform boat;

		public Transform boatFrom;

		public Transform boatTo;

		public float boatSpeed = 10f;

		public ParticleSystem[] wake;

		[Header("Sound")]
		public AudioSource motor;

		public AudioSource horn;

		[Header("Cars")]
		public Car[] cars;

		public int paintSlot;

		public Color[] paints;

		public float grip = 6f;

		public float carWeight = 0.35f;

		[Header("Look")]
		public float occlusion = 0.6f;

		[HideInInspector]
		public float forceTime = -1f;

		private Quaternion[] rest;

		private Quaternion[] restWorld;

		private float shownAngle;

		private float spinRate;

		private static Drawbridge main;

		private float shownTime = -1f;

		private float endAt = -1f;

		private const float ClunkAt = 5.75f;

		private float rate = 1f;

		private int moving;

		private bool soundReady;

		private bool horned;

		private float motorLevel = 1f;

		private float hornLevel = 1f;

		private const float HornDistance = 150f;

		private const float PassAfter = 3f;

		private Quaternion boatTurn = Quaternion.identity;

		private bool boatShown = true;

		private float bobSeed;

		private readonly List<Rider> riders = new List<Rider>();

		private readonly List<NetProp> loose = new List<NetProp>();

		private readonly List<Rigidbody> looseBodies = new List<Rigidbody>();

		private Transform[] carGround;

		private Vector3[] carLocal;

		private float[] wheelSize;

		private static readonly RaycastHit[] below = new RaycastHit[16];

		private static readonly Collider[] touching = new Collider[32];

		public float ShownAngle => shownAngle;

		public static bool Surface(Rigidbody rb, Vector3 at, out Vector3 velocity)
		{
			velocity = Vector3.zero;
			Drawbridge bridge = main;
			if (!bridge || !rb || bridge.rest == null)
			{
				return false;
			}
			for (int i = 0; i < bridge.leaves.Length; i++)
			{
				Rigidbody hinge = bridge.leaves[i].hinge;
				if (hinge != rb)
				{
					continue;
				}
				Quaternion parent = hinge.transform.parent ? hinge.transform.parent.rotation : Quaternion.identity;
				Vector3 axis = parent * bridge.rest[i] * bridge.leaves[i].axis.normalized;
				Vector3 spin = axis * (bridge.spinRate * bridge.leaves[i].sign * Mathf.Deg2Rad);
				velocity = Vector3.Cross(spin, at - hinge.position);
				return true;
			}
			return false;
		}

		private void Start()
		{
			main = this;
			rest = new Quaternion[leaves.Length];
			restWorld = new Quaternion[leaves.Length];
			for (int i = 0; i < leaves.Length; i++)
			{
				Rigidbody hinge = leaves[i].hinge;
				if (hinge)
				{
					hinge.isKinematic = true;
					hinge.interpolation = RigidbodyInterpolation.Interpolate;
					rest[i] = hinge.transform.localRotation;
					restWorld[i] = hinge.rotation;
					VertexAO.skip.Add(hinge);
				}
			}
			VertexAO.sceneScale = occlusion;
			Mount();
			if (boat && boatFrom && boatTo)
			{
				boatTurn = Quaternion.Inverse(Quaternion.LookRotation((boatTo.position - boatFrom.position).normalized)) * boat.rotation;
			}
			bobSeed = UnityEngine.Random.value * 10f;
			if (motor)
			{
				motorLevel = motor.volume;
			}
			if (horn)
			{
				hornLevel = horn.volume;
			}
			int count = (cars != null) ? cars.Length : 0;
			carGround = new Transform[count];
			carLocal = new Vector3[count];
			wheelSize = new float[count];
			for (int j = 0; j < count; j++)
			{
				if (cars[j].body)
				{
					cars[j].body.centerOfMass = carWeight * Vector3.up;
				}
				wheelSize[j] = 0.4f;
				if (cars[j].wheels != null && cars[j].wheels.Length != 0 && cars[j].wheels[0])
				{
					Renderer wheel = cars[j].wheels[0].GetComponent<Renderer>();
					if (wheel)
					{
						wheelSize[j] = Mathf.Max(wheel.bounds.extents.x, wheel.bounds.extents.y, wheel.bounds.extents.z);
					}
				}
			}
			Paint();
		}

		private void OnValidate()
		{
			if (!Application.isPlaying)
			{
				VertexAO.Rescale();
			}
		}

		private void OnDestroy()
		{
			if (main == this)
			{
				main = null;
			}
			VertexAO.sceneScale = 1f;
			if (leaves == null)
			{
				return;
			}
			foreach (Leaf leaf in leaves)
			{
				if (leaf.hinge)
				{
					VertexAO.skip.Remove(leaf.hinge);
				}
			}
		}

		private void Mount()
		{
			foreach (NetProp prop in UnityEngine.Object.FindObjectsByType<NetProp>(FindObjectsSortMode.None))
			{
				if (!prop.pinned)
				{
					loose.Add(prop);
					looseBodies.Add(prop.GetComponent<Rigidbody>());
					continue;
				}
				Rigidbody body = prop.GetComponent<Rigidbody>();
				int leaf = LeafTouching(prop);
				if (leaf >= 0)
				{
					Rigidbody hinge = leaves[leaf].hinge;
					riders.Add(new Rider
					{
						prop = prop,
						body = body,
						grab = prop.GetComponent<Grabbable>(),
						leaf = leaf,
						pos = Quaternion.Inverse(restWorld[leaf]) * (body.position - hinge.position),
						rot = Quaternion.Inverse(restWorld[leaf]) * body.rotation
					});
				}
			}
		}

		private int LeafTouching(NetProp p)
		{
			foreach (Collider c in p.GetComponentsInChildren<Collider>())
			{
				Bounds bounds = c.bounds;
				int hits = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents + Vector3.one * 0.15f, touching, Quaternion.identity, -1, QueryTriggerInteraction.Ignore);
				for (int i = 0; i < hits; i++)
				{
					for (int j = 0; j < leaves.Length; j++)
					{
						if (leaves[j].hinge && touching[i].attachedRigidbody == leaves[j].hinge)
						{
							return j;
						}
					}
				}
			}
			return -1;
		}

		private int LeafUnder(Vector3 from, Vector3 down, float far, Rigidbody self, out RaycastHit ground)
		{
			ground = default(RaycastHit);
			int hits = Physics.RaycastNonAlloc(from, down, below, far, -1, QueryTriggerInteraction.Ignore);
			float best = float.MaxValue;
			for (int i = 0; i < hits; i++)
			{
				if (below[i].rigidbody == self || below[i].distance >= best || (below[i].rigidbody && (below[i].rigidbody.GetComponentInParent<Player>() || below[i].rigidbody.GetComponentInParent<AvatarBody>())))
				{
					continue;
				}
				best = below[i].distance;
				ground = below[i];
			}
			if (!ground.collider)
			{
				return -2;
			}
			for (int j = 0; j < leaves.Length; j++)
			{
				if (leaves[j].hinge && ground.rigidbody == leaves[j].hinge)
				{
					return j;
				}
			}
			return -1;
		}

		private float Now()
		{
			if (forceTime >= 0f)
			{
				return forceTime;
			}
			if (forceTime < -1.5f)
			{
				return -1f;
			}
			NetworkManager network = NetworkManager.main;
			if (!network || !network.InRoom)
			{
				return Time.timeSinceLevelLoad;
			}
			return GameManager.RoundTime;
		}

		private float Smoothed()
		{
			float dt = Time.fixedDeltaTime;
			float now = Now();
			if (shownTime < 0f || (endAt >= 0f && now >= 0f))
			{
				shownTime = now;
				endAt = -1f;
				rate = 1f;
				return now;
			}
			if (now < 0f)
			{
				if (endAt < 0f)
				{
					endAt = shownTime;
				}
				shownTime += dt;
				return shownTime;
			}
			float ahead = shownTime + dt;
			if (Mathf.Abs(now - ahead) > 1f && Mathf.Abs(Angle(now) - Angle(shownTime)) < 0.01f)
			{
				shownTime = now;
				rate = 1f;
				return now;
			}
			rate = Mathf.MoveTowards(rate, Mathf.Clamp(now - ahead, -0.5f, 0.5f) + 1f, dt * 0.5f);
			shownTime += dt * rate;
			return shownTime;
		}

		private float Phase(float t, out float hold)
		{
			hold = holdTime;
			if (firstRaise > t)
			{
				return -1f;
			}
			float grow = GameManager.HoldGrow;
			float overtime = GameManager.OvertimeFrom;
			float start = firstRaise;
			int late = 0;
			while (true)
			{
				hold = holdTime + grow * late;
				float next = start + Mathf.Max(hold + period - holdTime, 1f);
				if (next > t)
				{
					break;
				}
				if (start >= overtime)
				{
					late++;
				}
				start = next;
			}
			return t - start;
		}

		private float Angle(float t)
		{
			float p = Phase(t, out float hold);
			if (p < 0f)
			{
				return 0f;
			}
			float began = t - p;
			if (endAt >= 0f && endAt >= began && endAt - began < raiseTime + hold)
			{
				hold = Mathf.Max(endAt - began - raiseTime, 0f);
			}
			if (p < raiseTime)
			{
				return Smooth(p / raiseTime) * maxAngle;
			}
			if (p < raiseTime + hold)
			{
				return maxAngle;
			}
			if (p < raiseTime + hold + lowerTime)
			{
				return Smooth(1f - (p - raiseTime - hold) / lowerTime) * maxAngle;
			}
			return 0f;
		}

		private static float Smooth(float x)
		{
			x = Mathf.Clamp01(x);
			return x * x * (3f - 2f * x);
		}

		private void FixedUpdate()
		{
			float angle = Angle(Smoothed());
			spinRate = (angle - shownAngle) / Time.fixedDeltaTime;
			for (int i = 0; i < leaves.Length; i++)
			{
				Rigidbody hinge = leaves[i].hinge;
				if (hinge)
				{
					Quaternion turn = rest[i] * Quaternion.AngleAxis(angle * leaves[i].sign, leaves[i].axis);
					if (hinge.transform.parent)
					{
						turn = hinge.transform.parent.rotation * turn;
					}
					hinge.MoveRotation(turn);
					Carry(i, hinge.position, turn * Quaternion.Inverse(restWorld[i]));
				}
			}
			Sound(angle);
			shownAngle = angle;
			for (int j = 0; j < carGround.Length; j++)
			{
				Grip(j);
			}
			Ride();
		}

		private void Ride()
		{
			for (int i = 0; i < loose.Count; i++)
			{
				NetProp prop = loose[i];
				Rigidbody body = looseBodies[i];
				if (!prop || !body)
				{
					continue;
				}
				int nearest = -1;
				float best = float.MaxValue;
				for (int j = 0; j < leaves.Length; j++)
				{
					if (leaves[j].hinge)
					{
						float d = (leaves[j].hinge.position - body.position).sqrMagnitude;
						if (d < best)
						{
							best = d;
							nearest = j;
						}
					}
				}
				Rigidbody riding = null;
				if (nearest >= 0)
				{
					Vector3 up = leaves[nearest].hinge.rotation * Quaternion.Inverse(restWorld[nearest]) * Vector3.up;
					if (LeafUnder(body.worldCenterOfMass, -up, 1.6f, body, out RaycastHit _) == nearest)
					{
						riding = leaves[nearest].hinge;
					}
				}
				prop.riding = riding;
			}
		}

		private void Sound(float angle)
		{
			int dir;
			if (angle > shownAngle + 0.0001f)
			{
				dir = 1;
			}
			else if (angle < shownAngle + -0.0001f)
			{
				dir = -1;
			}
			else
			{
				if (!soundReady)
				{
					soundReady = true;
					moving = 0;
				}
				else if (angle <= 0.001f || maxAngle + -0.001f <= angle)
				{
					moving = 0;
				}
				return;
			}
			if (!soundReady)
			{
				soundReady = true;
			}
			else if (dir != moving && motor && motor.clip)
			{
				motor.pitch = Mathf.Clamp(5.75f / Mathf.Max((dir == 1) ? raiseTime : lowerTime, 0.1f), 0.6f, 1f);
				motor.volume = motorLevel * Prefs.sfx;
				motor.Play();
			}
			moving = dir;
		}

		private void Carry(int leaf, Vector3 pivot, Quaternion turn)
		{
			for (int i = riders.Count - 1; i >= 0; i--)
			{
				Rider rider = riders[i];
				if (rider.leaf != leaf)
				{
					continue;
				}
				if (rider.prop && rider.body && rider.prop.Bolted)
				{
					if (rider.body.isKinematic)
					{
						rider.body.MovePosition(pivot + turn * rider.pos);
						rider.body.MoveRotation(turn * rider.rot);
					}
					continue;
				}
				if (rider.body && Player.main)
				{
					Player.main.GaveWay(rider.body);
				}
				if (rider.grab)
				{
					rider.grab.holdsYou = false;
				}
				riders.RemoveAt(i);
			}
		}

		private void Grip(int i)
		{
			Rigidbody body = cars[i].body;
			if (!body)
			{
				return;
			}
			Transform t = body.transform;
			int leaf = LeafUnder(body.worldCenterOfMass, -t.up, 1.6f, body, out RaycastHit hit);
			Transform ground = (leaf >= 0) ? leaves[leaf].hinge.transform : ((leaf == -1) ? hit.transform : null);
			if (ground != carGround[i])
			{
				carGround[i] = ground;
				if (ground)
				{
					carLocal[i] = ground.InverseTransformPoint(body.transform.position);
				}
			}
			if (ground && !body.isKinematic)
			{
				Vector3 reach = body.transform.forward * wheelSize[i] * 1.5f;
				Axle(body, body.worldCenterOfMass + reach);
				Axle(body, body.worldCenterOfMass - reach);
			}
		}

		private void Axle(Rigidbody b, Vector3 at)
		{
			Vector3 side = b.transform.right;
			Vector3 velocity = b.GetPointVelocity(at);
			float slip = Mathf.Clamp(grip * Vector3.Dot(side, velocity), 0f - Physics.gravity.magnitude, Physics.gravity.magnitude);
			b.AddForceAtPosition(side * slip * b.mass * -0.5f, at);
		}

		private void Update()
		{
			Sail(shownTime);
			for (int i = 0; i < carGround.Length; i++)
			{
				Roll(i);
			}
		}

		private void Roll(int i)
		{
			Transform ground = carGround[i];
			Car car = cars[i];
			if (!ground || !car.body || car.wheels == null)
			{
				return;
			}
			Transform t = car.body.transform;
			Vector3 local = ground.InverseTransformPoint(t.position);
			Vector3 moved = ground.TransformVector(local - carLocal[i]);
			carLocal[i] = local;
			float spin = Vector3.Dot(moved, t.forward) / Mathf.Max(wheelSize[i], 0.05f);
			if (Mathf.Abs(spin * Mathf.Rad2Deg) < 0.01f)
			{
				return;
			}
			foreach (Transform wheel in car.wheels)
			{
				if (wheel)
				{
					wheel.Rotate(t.right, spin * Mathf.Rad2Deg, Space.World);
				}
			}
		}

		private void Sail(float t)
		{
			if (!boat || !boatFrom || !boatTo)
			{
				return;
			}
			float distance = Vector3.Distance(boatFrom.position, boatTo.position);
			float crossing = distance / Mathf.Max(boatSpeed, 0.1f);
			float phase = Phase(t, out float hold);
			float into = (firstRaise > t) ? (t - firstRaise) : phase;
			float lead = raiseTime + 3f + crossing * -0.5f;
			float sailed = into - lead;
			if (sailed > crossing)
			{
				sailed = into - Mathf.Max(period + hold - holdTime, 1f) - lead;
			}
			bool show = sailed >= 0f && t >= 0f && sailed <= crossing;
			if (show != boatShown)
			{
				boatShown = show;
				boat.gameObject.SetActive(show);
				foreach (ParticleSystem trail in wake)
				{
					if (trail)
					{
						if (show)
						{
							trail.Play();
						}
						else
						{
							trail.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
						}
					}
				}
			}
			if (!show)
			{
				horned = false;
				return;
			}
			if (!horned)
			{
				float hornAt = (distance * 0.5f + -150f) / Mathf.Max(boatSpeed, 0.1f);
				if (sailed >= hornAt)
				{
					horned = true;
					if (horn && horn.clip && sailed < hornAt + 1f)
					{
						horn.volume = hornLevel * Prefs.sfx;
						horn.Play();
					}
				}
			}
			Vector3 dir = (boatTo.position - boatFrom.position).normalized;
			Vector3 pos = boatFrom.position + dir * (sailed * boatSpeed);
			float bob = Time.time + bobSeed;
			pos.y += Mathf.Sin(bob * 1.1f) * 0.08f;
			boat.position = pos;
			boat.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(Mathf.Sin(bob * 0.9f) * 1.2f, 0f, Mathf.Sin(bob * 0.7f) * 2f) * boatTurn;
		}

		private void Paint()
		{
			if (cars == null || cars.Length == 0 || paints == null || paints.Length == 0)
			{
				return;
			}
			string key = (GameManager.match != null) ? (GameManager.match.session + "/" + GameManager.match.round) : 0.ToString();
			uint hash = 2166136261u;
			foreach (char c in key)
			{
				hash = (hash ^ c) * 16777619;
			}
			System.Random random = new System.Random((int)hash);
			int[] order = new int[paints.Length];
			for (int i = 0; i < order.Length; i++)
			{
				order[i] = i;
			}
			for (int j = order.Length - 1; j > 0; j--)
			{
				int k = random.Next(j + 1);
				int swap = order[j];
				order[j] = order[k];
				order[k] = swap;
			}
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			for (int l = 0; l < cars.Length; l++)
			{
				Renderer paint = cars[l].paint;
				if (paint)
				{
					int slot = Mathf.Clamp(paintSlot, 0, paint.sharedMaterials.Length - 1);
					Color color = paints[order[l % order.Length]];
					paint.GetPropertyBlock(block, slot);
					block.SetColor("_BaseColor", color);
					block.SetColor("_Color", color);
					block.SetColor("_TopColor", color);
					block.SetColor("_BottomColor", Color.Lerp(color, Color.white, 0.25f));
					paint.SetPropertyBlock(block, slot);
				}
			}
		}
	}
}
