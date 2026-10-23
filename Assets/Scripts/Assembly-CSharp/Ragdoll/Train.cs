using UnityEngine;

namespace Ragdoll
{
	public class Train : MonoBehaviour
	{
		[Header("Track")]
		public Transform nose;

		public float[] tracks = new float[2] { 9f, -9f };

		public float westCap = -35.5f;

		public float eastCap = 20.4f;

		public float westMouth = -23.7f;

		public float eastMouth = 9.3f;

		public float speed = 22f;

		public float lead = 40f;

		public float killDepth = 3f;

		public float flop = 6f;

		[Header("Schedule")]
		public float firstTrain = 10f;

		public float gap = 16f;

		public float minGap = 6f;

		[Header("Look")]
		public Material paint;

		public Color[] paints = new Color[8]
		{
			new Color(0.86f, 0.22f, 0.22f, 1f),
			new Color(0.2f, 0.45f, 0.9f, 1f),
			new Color(0.2f, 0.75f, 0.3f, 1f),
			new Color(0.95f, 0.8f, 0.15f, 1f),
			new Color(0.6f, 0.3f, 0.85f, 1f),
			new Color(0.95f, 0.5f, 0.12f, 1f),
			new Color(0.15f, 0.75f, 0.75f, 1f),
			new Color(0.95f, 0.4f, 0.7f, 1f)
		};

		[Header("Lamps")]
		public Color headColor = new Color(1f, 0.92f, 0.76f, 1f);

		public Color tailColor = new Color(1f, 0.08f, 0.04f, 1f);

		public float headBright = 40f;

		public float headRange = 26f;

		public float headCone = 35f;

		public float tailBright = 2f;

		public float tailRange = 6f;

		[Header("Sound")]
		public AudioSource rumble;

		public AudioSource horn;

		[HideInInspector]
		public float forceTime = -1f;

		public static Train main;

		private static readonly int LampsId = Shader.PropertyToID("_TrainLamps");

		private static readonly int LampPosId = Shader.PropertyToID("_TrainLampPos");

		private static readonly int LampDirId = Shader.PropertyToID("_TrainLampDir");

		private static readonly int LampColorId = Shader.PropertyToID("_TrainLampColor");

		private Rigidbody body;

		private Collider[] cols;

		private Renderer[] shown;

		private MaterialPropertyBlock block;

		private Rigidbody[] carried;

		private Vector3[] slotPos;

		private Quaternion[] slotRot;

		private Quaternion rest;

		private Vector3 restNose;

		private float height;

		private float front;

		private float back;

		private float side;

		private float low;

		private float high;

		private float shownTime = -1f;

		private float rate = 1f;

		private float endAt = -1f;

		private float rumbleLevel;

		private float hornLevel;

		private int seed;

		private string seededFor;

		private int run = -1;

		private int lane;

		private Player rider;

		private int copiesSeen = -1;

		private bool horned;

		private bool visible = true;

		private bool lampsOn;

		private Vector3 headLocal;

		private Vector3 tailLocal;

		private Vector3 noseLocal;

		private readonly Vector4[] lampPos = new Vector4[2];

		private readonly Vector4[] lampDir = new Vector4[2];

		private readonly Vector4[] lampColor = new Vector4[2];

		private const float SideReach = 0.3f;

		private const float TrackTop = 0.6f;

		private const float TrackSide = 0.3f;

		private float Length => front + back;

		private float Travel => eastCap - westCap + lead + Length + 1f;

		private void Awake()
		{
			main = this;
			if (rumble)
			{
				rumbleLevel = rumble.volume;
			}
			if (horn)
			{
				hornLevel = horn.volume;
			}
		}

		private void Start()
		{
			body = GetComponent<Rigidbody>();
			body.isKinematic = true;
			body.useGravity = false;
			body.interpolation = RigidbodyInterpolation.Interpolate;
			body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			cols = GetComponentsInChildren<Collider>();
			shown = GetComponentsInChildren<Renderer>();
			block = new MaterialPropertyBlock();
			rest = transform.rotation;
			height = transform.position.y;
			Vector3 origin = transform.position;
			Vector3 toward = nose ? (nose.position - origin) : transform.forward;
			Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
			Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
			float alongForward = Vector3.Dot(toward, forward);
			float alongRight = Vector3.Dot(toward, right);
			restNose = (Mathf.Abs(alongForward) > Mathf.Abs(alongRight)) ? (forward * Mathf.Sign(alongForward)) : (right * Mathf.Sign(alongRight));
			Vector3 across = Vector3.Cross(Vector3.up, restNose);
			foreach (Renderer r in shown)
			{
				Bounds b = r.bounds;
				Vector3 c = b.center;
				Vector3 e = b.extents;
				for (int i = 0; i < 8; i++)
				{
					Vector3 d = c + new Vector3(((i & 1) == 0) ? (0f - e.x) : e.x, ((i & 2) == 0) ? (0f - e.y) : e.y, ((i & 4) == 0) ? (0f - e.z) : e.z) - origin;
					float along = Vector3.Dot(d, restNose);
					front = Mathf.Max(front, along);
					back = Mathf.Max(back, 0f - along);
					side = Mathf.Max(side, Mathf.Abs(Vector3.Dot(d, across)));
					low = Mathf.Min(low, d.y);
					high = Mathf.Max(high, d.y);
				}
			}
			float lampHeight = low + 1f;
			foreach (Renderer r2 in shown)
			{
				MeshFilter filter = r2.GetComponent<MeshFilter>();
				if (!filter || !filter.sharedMesh)
				{
					continue;
				}
				Material[] mats = r2.sharedMaterials;
				for (int j = 0; j < mats.Length && j < filter.sharedMesh.subMeshCount; j++)
				{
					if (mats[j] && mats[j].name == "Lights")
					{
						lampHeight = r2.transform.TransformPoint(filter.sharedMesh.GetSubMesh(j).bounds.center).y - origin.y;
					}
				}
			}
			headLocal = transform.InverseTransformPoint(origin + restNose * (front + 0.05f) + Vector3.up * lampHeight);
			tailLocal = transform.InverseTransformPoint(origin - restNose * (back + 0.05f) + Vector3.up * lampHeight);
			noseLocal = transform.InverseTransformDirection(restNose);
			VertexAO.skip.Add(body);
			Park();
		}

		private void LateUpdate()
		{
			bool on = run >= 0;
			if (lampsOn != on)
			{
				lampsOn = on;
				Shader.SetGlobalFloat(LampsId, on ? 1f : 0f);
			}
			if (run >= 0)
			{
				Vector3 forward = transform.TransformDirection(noseLocal);
				Vector3 head = transform.TransformPoint(headLocal);
				Vector3 tail = transform.TransformPoint(tailLocal);
				Vector3 beam = (forward + Vector3.up * -0.08f).normalized;
				lampPos[0] = new Vector4(head.x, head.y, head.z, headRange);
				lampDir[0] = new Vector4(beam.x, beam.y, beam.z, Mathf.Cos(headCone * Mathf.Deg2Rad));
				lampColor[0] = headColor.linear * headBright;
				lampPos[1] = new Vector4(tail.x, tail.y, tail.z, tailRange);
				lampDir[1] = new Vector4(0f - forward.x, 0f - forward.y, 0f - forward.z, -1f);
				lampColor[1] = tailColor.linear * tailBright;
				Shader.SetGlobalVectorArray(LampPosId, lampPos);
				Shader.SetGlobalVectorArray(LampDirId, lampDir);
				Shader.SetGlobalVectorArray(LampColorId, lampColor);
			}
		}

		private void OnDisable()
		{
			lampsOn = false;
			Shader.SetGlobalFloat(LampsId, 0f);
		}

		private void OnDestroy()
		{
			if (main == this)
			{
				main = null;
			}
			VertexAO.skip.Remove(body);
			if (carried != null)
			{
				Release();
			}
		}

		private void FixedUpdate()
		{
			Ignore();
			float t = Smoothed();
			int k = Run(t, out float into);
			Player me = Player.main;
			if (me && me.pelvis && !me.drowned && carried == null && Deep(me.pelvis.position))
			{
				me.Kill("walked too deep into the train tunnel");
			}
			if (k < 0 || (k != run && GameManager.main && GameManager.main.winner != null))
			{
				Npcs(false, Vector3.zero, Vector3.zero);
				if (carried != null)
				{
					Release();
				}
				if (rumble && rumble.isPlaying)
				{
					rumble.Stop();
				}
				run = -1;
				Park();
				return;
			}
			if (k != run)
			{
				run = k;
				int hash = Hash(k);
				horned = false;
				lane = hash & 1;
				Paint(k);
			}
			float dir = (lane == 0) ? (-1f) : 1f;
			float start = (dir > 0f) ? (westCap - lead) : (eastCap + lead);
			float tip = start + dir * speed * into;
			Quaternion turn = Quaternion.AngleAxis(Vector3.SignedAngle(restNose, new Vector3(dir, 0f, 0f), Vector3.up), Vector3.up) * rest;
			float z = tracks[Mathf.Min(lane, tracks.Length - 1)];
			Vector3 ahead = new Vector3(dir, 0f, 0f);
			float step = speed * Time.fixedDeltaTime;
			Vector3 pos = new Vector3(tip - dir * front, height, z);
			if ((pos - body.position).sqrMagnitude > step * step * 16f || Quaternion.Angle(body.rotation, turn) > 1f)
			{
				body.position = pos;
				body.rotation = turn;
				transform.SetPositionAndRotation(pos, turn);
			}
			else
			{
				body.MovePosition(pos);
				body.MoveRotation(turn);
			}
			float tail = tip - dir * Length;
			Show(Mathf.Max(tip, tail) > westCap && Mathf.Min(tip, tail) < eastCap);
			Sound(into, Travel / speed, (dir > 0f) ? (westMouth - start) : (start - eastMouth));
			Npcs(true, pos, ahead);
			if (!me || !me.pelvis)
			{
				return;
			}
			Vector3 vel = ahead * (speed * rate);
			if (carried != null && !me.knockedOut && !me.drowned)
			{
				Release();
			}
			if (carried != null)
			{
				Carry(me, pos, turn, ahead, vel);
				Vector3 at = me.pelvis.position;
				bool past = (dir > 0f) ? (at.x > eastMouth + killDepth) : (at.x < westMouth - killDepth);
				if (!me.drowned && (past || Deep(at)))
				{
					me.Kill("hit by the train");
				}
				if ((dir > 0f) ? (tip > eastCap) : (tip < westCap))
				{
					Release();
				}
			}
			else if (visible && !me.dead && !me.drowned && me.bodies != null)
			{
				if (Struck(me, pos, ahead))
				{
					if (me.carriedHold && NetworkPlayer.mine)
					{
						NetworkPlayer.mine.KeepBody();
					}
					Grab(me, pos, turn, ahead, vel);
				}
				else
				{
					Brush(me, pos, ahead);
				}
			}
		}

		private int Run(float t, out float into)
		{
			into = -1f;
			if (firstTrain > t)
			{
				return -1;
			}
			float duration = Travel / speed;
			float start = firstTrain;
			for (int k = 0; k < 10000; k++)
			{
				float next = start + Mathf.Max(Mathf.Lerp(gap, minGap, Rush(start)), duration + 1f);
				if (next > t)
				{
					if (endAt >= 0f && start >= endAt)
					{
						return -1;
					}
					into = t - start;
					return (into <= duration) ? k : (-1);
				}
				start = next;
			}
			return -1;
		}

		private static float Rush(float at)
		{
			GameManager game = GameManager.main;
			if (!game)
			{
				return 0f;
			}
			float x = Mathf.Clamp01((at - GameManager.OvertimeFrom) / Mathf.Max((game.roundLength - GameManager.OvertimeFrom) * 0.6f, 1f));
			return x * x * (3f - 2f * x);
		}

		private int Hash(int k)
		{
			string key = (GameManager.match != null) ? (GameManager.match.session + "/" + GameManager.match.round) : "solo";
			if (key != seededFor)
			{
				seededFor = key;
				seed = -2128831035;
				foreach (char c in key)
				{
					seed = (seed ^ c) * 16777619;
				}
			}
			int h = (seed ^ k) * 16777619;
			h = (h ^ (h >> 13)) * 16777619;
			return h & 0x7FFFFFFF;
		}

		private bool Open(float t)
		{
			int k = Run(t, out float into);
			if (k < 0)
			{
				return false;
			}
			float dir = ((Hash(k) & 1) == 0) ? (-1f) : 1f;
			float start = (dir > 0f) ? (westCap - lead) : (eastCap + lead);
			float tip = start + dir * speed * into;
			float tail = tip - dir * Length;
			return Mathf.Max(tip, tail) > westCap && Mathf.Min(tip, tail) < eastCap;
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
			if (!network || (!network.InRoom && GameManager.match == null && !network.Travelling))
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
				rate = 1f;
				endAt = -1f;
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
			if (Mathf.Abs(now - ahead) > 1f && !Open(now) && !Open(shownTime))
			{
				shownTime = now;
				rate = 1f;
				return now;
			}
			rate = Mathf.MoveTowards(rate, Mathf.Clamp(now - ahead, -0.5f, 0.5f) + 1f, dt * 0.5f);
			shownTime += dt * rate;
			return shownTime;
		}

		private void Park()
		{
			Vector3 parked = new Vector3(eastCap + lead + Length + 20f, height, tracks[0]);
			if ((body.position - parked).sqrMagnitude > 1f)
			{
				body.position = parked;
				transform.position = parked;
			}
			Show(false);
		}

		private void Show(bool on)
		{
			if (visible == on)
			{
				return;
			}
			visible = on;
			foreach (Renderer r in shown)
			{
				if (r)
				{
					r.enabled = on;
				}
			}
		}

		private void Paint(int k)
		{
			if (!paint || paints.Length == 0)
			{
				return;
			}
			int count = paints.Length;
			int pick = (Hash(k) >> 1) % count;
			if (k >= 1 && pick == (Hash(k - 1) >> 1) % count)
			{
				pick = (pick + 1) % count;
			}
			Color color = paints[pick];
			foreach (Renderer r in shown)
			{
				Material[] mats = r.sharedMaterials;
				for (int i = 0; i < mats.Length; i++)
				{
					if (mats[i] == paint)
					{
						r.GetPropertyBlock(block, i);
						block.SetColor("_TopColor", color);
						block.SetColor("_BottomColor", color * 0.8f);
						block.SetColor("_BaseColor", color);
						r.SetPropertyBlock(block, i);
					}
				}
			}
		}

		private void Sound(float into, float duration, float toMouth)
		{
			if (rumble)
			{
				if (!rumble.isPlaying && rumble.clip)
				{
					rumble.time = into % rumble.clip.length;
					rumble.Play();
				}
				rumble.volume = Mathf.Clamp01(Mathf.Min(duration - into, into) / 1.5f) * rumbleLevel * Prefs.sfx;
			}
			if (horned)
			{
				return;
			}
			float hornAt = toMouth / speed + -0.5f;
			if (!(hornAt > into))
			{
				horned = true;
				if (horn && !(hornAt + 1f < into))
				{
					horn.volume = hornLevel * Prefs.sfx;
					horn.Play();
				}
			}
		}

		private void Ignore()
		{
			if (copiesSeen == AvatarBody.copiesChanged)
			{
				return;
			}
			copiesSeen = AvatarBody.copiesChanged;
			foreach (AvatarBody copy in AvatarBody.copies)
			{
				if (!copy || copy.cols == null)
				{
					continue;
				}
				for (int i = 0; i < cols.Length; i++)
				{
					for (int j = 0; j < copy.cols.Length; j++)
					{
						if (cols[i] && copy.cols[j])
						{
							Physics.IgnoreCollision(cols[i], copy.cols[j], true);
						}
					}
				}
			}
		}

		public static Vector3 Pin(Vector3[] pose)
		{
			Train train = main;
			if (!train || !train.visible || pose == null || train.run < 0 || pose.Length == 0)
			{
				return Vector3.zero;
			}
			Vector3 at = train.transform.position;
			float dir = (train.lane == 0) ? (-1f) : 1f;
			bool touching = false;
			foreach (Vector3 p in pose)
			{
				if (Mathf.Abs(p.z - at.z) < train.side + 0.15f)
				{
					float y = p.y - at.y;
					if (y > train.low + -0.1f)
					{
						float x = dir * (p.x - at.x) - train.front;
						if (x < 0.35f && y < train.high + 0.1f && x > -1f - train.Length)
						{
							touching = true;
						}
					}
				}
			}
			if (!touching)
			{
				return Vector3.zero;
			}
			float gap = dir * (pose[0].x - at.x) - train.front;
			if (gap >= 0.6f)
			{
				return Vector3.zero;
			}
			return new Vector3(dir * (0.6f - gap), 0f, 0f);
		}

		public static Vector3 Escape(Vector3 p)
		{
			Train train = main;
			if (!train || p.y > train.height + train.low + 0.6f)
			{
				return Vector3.zero;
			}
			foreach (float z in train.tracks)
			{
				if (Mathf.Abs(p.z - z) < train.side + 0.3f)
				{
					return new Vector3(0f, 0f, (z > 0f) ? (-1f) : 1f);
				}
			}
			return Vector3.zero;
		}

		private void Npcs(bool running, Vector3 at, Vector3 ahead)
		{
			for (int i = SlapDummy.alive.Count - 1; i >= 0; i--)
			{
				SlapDummy dummy = (i < SlapDummy.alive.Count) ? SlapDummy.alive[i] : null;
				Player npc = dummy ? dummy.Body : null;
				if (!npc || !npc.pelvis || dummy.Gone)
				{
					continue;
				}
				if (Deep(npc.pelvis.position))
				{
					npc.Kill("walked too deep into the train tunnel");
				}
				else if (running && visible && Struck(npc, at, ahead))
				{
					npc.Kill("hit by the train");
				}
			}
		}

		private bool Deep(Vector3 p)
		{
			if (p.y > 5f || !(p.x < eastMouth + killDepth) || !(p.x > westMouth - killDepth))
			{
				return false;
			}
			foreach (float z in tracks)
			{
				if (Mathf.Abs(p.z - z) < 2.2f)
				{
					return true;
				}
			}
			return false;
		}

		private bool Struck(Player me, Vector3 at, Vector3 ahead)
		{
			if (!InTrain(me.pelvis, at, ahead) && !InTrain(me.chest, at, ahead))
			{
				return InTrain(me.head, at, ahead);
			}
			return true;
		}

		private bool InTrain(Rigidbody rb, Vector3 at, Vector3 ahead)
		{
			if (!rb)
			{
				return false;
			}
			Vector3 d = rb.position - at;
			float along = Vector3.Dot(d, ahead);
			if (!(along > front - Length + -0.3f) || !(along < front + 0.35f))
			{
				return false;
			}
			if (!(Mathf.Abs(Vector3.Dot(d, Vector3.Cross(Vector3.up, ahead))) < side + 0.3f) || !(d.y > low + -0.1f))
			{
				return false;
			}
			return d.y < high + 0.1f;
		}

		private void Brush(Player me, Vector3 at, Vector3 ahead)
		{
			Vector3 across = Vector3.Cross(Vector3.up, ahead);
			foreach (Rigidbody b in me.bodies)
			{
				if (b && InTrain(b, at, ahead))
				{
					float away = Mathf.Sign(Vector3.Dot(b.position - at, across));
					b.linearVelocity = Vector3.ClampMagnitude(ahead * (speed * 0.4f) + across * away * 5f, 15f);
				}
			}
		}

		private void Grab(Player me, Vector3 at, Quaternion turn, Vector3 ahead, Vector3 vel)
		{
			Vector3 across = Vector3.Cross(Vector3.up, ahead);
			me.KnockOut(at + ahead * front, false);
			rider = me;
			me.trainHeld = true;
			carried = me.bodies;
			slotPos = new Vector3[carried.Length];
			slotRot = new Quaternion[carried.Length];
			float nearest = float.MaxValue;
			foreach (Rigidbody b in carried)
			{
				if (b)
				{
					nearest = Mathf.Min(nearest, Vector3.Dot(b.position - at, ahead));
				}
			}
			float push = Mathf.Max(front + 0.3f - nearest, 0f);
			Quaternion back = Quaternion.Inverse(turn);
			for (int i = 0; i < carried.Length; i++)
			{
				Rigidbody b2 = carried[i];
				if (b2)
				{
					b2.position += ahead * push;
					b2.linearVelocity = vel;
					b2.angularVelocity = Vector3.zero;
					Vector3 d = b2.position - at;
					slotPos[i] = new Vector3(Vector3.Dot(d, ahead), d.y, Vector3.Dot(d, across));
					slotRot[i] = back * b2.rotation;
					b2.detectCollisions = false;
				}
			}
			Player.Note("hit by the train");
		}

		private void Carry(Player me, Vector3 at, Quaternion turn, Vector3 ahead, Vector3 vel)
		{
			me.knockedOutUntil = Mathf.Max(me.knockedOutUntil, Time.time + 0.5f);
			me.riseLockedUntil = Mathf.Max(me.riseLockedUntil, Time.time + 0.5f);
			Vector3 across = Vector3.Cross(Vector3.up, ahead);
			for (int i = 0; i < carried.Length; i++)
			{
				Rigidbody b = carried[i];
				if (!b)
				{
					continue;
				}
				b.maxLinearVelocity = vel.magnitude + 2f * flop;
				if (b == me.pelvis || b == me.chest)
				{
					b.position = at + ahead * slotPos[i].x + Vector3.up * slotPos[i].y + across * slotPos[i].z;
					b.rotation = turn * slotRot[i];
					b.linearVelocity = vel;
					b.angularVelocity = Vector3.zero;
					continue;
				}
				b.linearVelocity = vel + Vector3.ClampMagnitude(b.linearVelocity - vel, flop);
				float along = Vector3.Dot(b.position - at, ahead);
				if (along < front + 0.1f)
				{
					b.position += ahead * (front + 0.1f - along);
					float closing = Vector3.Dot(b.linearVelocity - vel, ahead);
					if (closing < 0f)
					{
						b.linearVelocity -= ahead * closing;
					}
				}
			}
		}

		private void Release()
		{
			foreach (Rigidbody b in carried)
			{
				if (b)
				{
					b.detectCollisions = true;
					b.linearVelocity = Vector3.ClampMagnitude(b.linearVelocity, 15f);
				}
			}
			if (rider)
			{
				rider.trainHeld = false;
				rider.Recap();
			}
			rider = null;
			carried = null;
		}
	}
}
