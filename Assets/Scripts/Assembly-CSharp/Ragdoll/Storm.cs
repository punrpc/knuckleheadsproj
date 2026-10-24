using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ragdoll
{
	public class Storm : MonoBehaviour
	{
		[Header("Rain")]
		public ParticleSystem rain;

		[Header("Lightning")]
		public Transform strikeOn;

		public GameObject bolt;

		public Material boltLine;

		public AudioClip thunderSound;

		private const float Slot = 2f;

		private const float Chance = 0.15f;

		private const float OvertimeChance = 0.75f;

		private const float FarChance = 0.2f;

		private const float Warn = 1.6f;

		private const float Radius = 2f;

		private const float AtPlayer = 0.1f;

		private const float OvertimeAtPlayer = 0.85f;

		private const float Grid = 1f;

		private const float LaunchOut = 4f;

		private const float LaunchUp = 6f;

		private const float RiseSpeed = 0.8f;

		private const float RiseGrip = 12f;

		private const float RiseRamp = 0.5f;

		private const float RiseHeight = 3f;

		private const float FlashTime = 0.5f;

		private const float RainAbove = 9f;

		private const float BoltTop = 45f;

		private const int BoltKinks = 16;

		private const float BoltWide = 0.55f;

		private const float Elevated = 0.6f;

		private const float HighCap = 6f;

		private const float HeightStep = 0.5f;

		private const float Lifted = 2f;

		private const float LiftGap = 1.4f;

		private const float StandHeight = 1f;

		private const float Cover = 0.5f;

		private const float AimCap = 0.95f;

		private const float Lead = 0.4f;

		private static readonly int ZenithId = Shader.PropertyToID("_ZenithColor");

		private static readonly int HorizonId = Shader.PropertyToID("_HorizonColor");

		private static readonly int CloudLitId = Shader.PropertyToID("_CloudLitColor");

		private static readonly int CloudShadeId = Shader.PropertyToID("_CloudShadeColor");

		private readonly List<Vector3> spots = new List<Vector3>();

		private readonly List<Collider> surfaces = new List<Collider>();

		private readonly Dictionary<int, Vector3> aimed = new Dictionary<int, Vector3>();

		private readonly List<Vector4> rising = new List<Vector4>();

		private readonly HashSet<Rigidbody> raised = new HashSet<Rigidbody>();

		private readonly HashSet<Player> floated = new HashSet<Player>();

		private static readonly Collider[] near = new Collider[128];

		private readonly Dictionary<Player, Rigidbody[]> parts = new Dictionary<Player, Rigidbody[]>();

		private readonly List<int> drop = new List<int>();

		private readonly List<float> spotSum = new List<float>();

		private readonly List<NetworkPlayer> alive = new List<NetworkPlayer>();

		private readonly List<float> aliveSum = new List<float>();

		private static readonly RaycastHit[] skyHits = new RaycastHit[32];

		private float baseY;

		private float skyY;

		private float skyReach;

		private int told = int.MinValue;

		private readonly HashSet<int> heard = new HashSet<int>();

		private static Storm active;

		private int fired = int.MinValue;

		private int glowed = int.MinValue;

		private Light sun;

		private float sunBase;

		private Color skyBase;

		private Color equatorBase;

		private Color groundBase;

		private Material sky;

		private Color zenithBase;

		private Color horizonBase;

		private Color litBase;

		private Color shadeBase;

		private float flashAt = -9f;

		private float flashSize = 1f;

		private float shown;

		private AudioSource thunderAudio;

		private LineRenderer line;

		private bool lineLit;

		private float thunderDue = -1f;

		private float thunderVolume;

		private void Start()
		{
			active = this;
			foreach (Light light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
			{
				if (light.type == LightType.Directional)
				{
					sun = light;
				}
			}
			if (sun)
			{
				sunBase = sun.intensity;
			}
			skyBase = RenderSettings.ambientSkyColor;
			equatorBase = RenderSettings.ambientEquatorColor;
			groundBase = RenderSettings.ambientGroundColor;
			if (RenderSettings.skybox)
			{
				sky = new Material(RenderSettings.skybox);
				RenderSettings.skybox = sky;
				zenithBase = sky.GetColor(ZenithId);
				horizonBase = sky.GetColor(HorizonId);
				litBase = sky.GetColor(CloudLitId);
				shadeBase = sky.GetColor(CloudShadeId);
			}
			thunderAudio = gameObject.AddComponent<AudioSource>();
			thunderAudio.spatialBlend = 0f;
			thunderAudio.playOnAwake = false;
			Spots();
		}

		private void Spots()
		{
			spots.Clear();
			surfaces.Clear();
			if (!strikeOn)
			{
				return;
			}
			Bounds box = default(Bounds);
			bool any = false;
			foreach (Collider c in strikeOn.GetComponentsInChildren<Collider>())
			{
				if (c.isTrigger)
				{
					continue;
				}
				surfaces.Add(c);
				if (any)
				{
					box.Encapsulate(c.bounds);
					continue;
				}
				box = c.bounds;
				any = true;
			}
			if (!any)
			{
				return;
			}
			int moved = 0;
			skyY = box.max.y + 10f;
			skyReach = box.size.y + 40f;
			for (float x = box.min.x + Grid * 0.5f; x < box.max.x; x += Grid)
			{
				for (float z = box.min.z + Grid * 0.5f; z < box.max.z; z += Grid)
				{
					if (!Ground(new Vector3(x, box.max.y + 1f, z), box.size.y + 2f, out Vector3 at, out Vector3 normal) || normal.y <= 0.8f || Burning(at))
					{
						continue;
					}
					if (SkyHit(at, out Vector3 top, out Vector3 topNormal) && top.y > at.y + Cover)
					{
						moved++;
						if (topNormal.y <= 0.8f || Burning(top))
						{
							continue;
						}
						at = top;
					}
					spots.Add(at);
				}
			}
			List<float> heights = new List<float>();
			foreach (Vector3 spot in spots)
			{
				heights.Add(spot.y);
			}
			heights.Sort();
			baseY = (heights.Count > 0) ? heights[heights.Count / 10] : transform.position.y;
			spotSum.Clear();
			float sum = 0f;
			foreach (Vector3 spot in spots)
			{
				sum += Weight(spot.y);
				spotSum.Add(sum);
			}
			Player.Note("storm: " + spots.Count + " lightning spots on " + surfaces.Count + " surfaces, " + moved + " under cover moved to the top, base " + baseY.ToString("F1") + " m");
		}

		private float Weight(float y)
		{
			float height = Mathf.Round((y - baseY) / HeightStep) * HeightStep;
			return 1f + Mathf.Clamp(height, 0f, HighCap) * Elevated;
		}

		private static int Pick(List<float> sums, float u)
		{
			float target = sums[sums.Count - 1] * u;
			for (int i = 0; i < sums.Count; i++)
			{
				if (sums[i] > target)
				{
					return i;
				}
			}
			return sums.Count - 1;
		}

		private bool SkyHit(Vector3 p, out Vector3 at, out Vector3 normal)
		{
			at = p;
			normal = Vector3.up;
			float from = Mathf.Max(skyY, p.y + 1f);
			int count = Physics.RaycastNonAlloc(new Vector3(p.x, from, p.z), Vector3.down, skyHits, skyReach + (from - skyY), -1, QueryTriggerInteraction.Ignore);
			float best = float.MaxValue;
			for (int i = 0; i < count; i++)
			{
				RaycastHit hit = skyHits[i];
				if (hit.distance < best && Solid(hit.collider))
				{
					best = hit.distance;
					at = hit.point;
					normal = hit.normal;
				}
			}
			return best < float.MaxValue;
		}

		private static bool Solid(Collider c)
		{
			Rigidbody body = c.attachedRigidbody;
			if (body && !body.isKinematic)
			{
				return false;
			}
			if (!c.GetComponentInParent<Player>() && !c.GetComponentInParent<NetworkPlayer>() && !c.GetComponentInParent<AvatarBody>() && !c.GetComponentInParent<NetProp>())
			{
				return !c.GetComponentInParent<Rope>();
			}
			return false;
		}

		private bool Covered(Vector3 p)
		{
			if (SkyHit(p, out Vector3 at, out Vector3 _))
			{
				return at.y > p.y + Cover;
			}
			return false;
		}

		private float Threat(Vector3 root, bool down, bool held)
		{
			if (!SkyHit(root, out Vector3 at, out Vector3 _) || at.y > root.y + Cover)
			{
				return 0f;
			}
			if (root.y - at.y > LiftGap && held)
			{
				return Weight(root.y - StandHeight) * Lifted;
			}
			if (down)
			{
				return 0f;
			}
			return Weight(root.y - StandHeight);
		}

		private static bool Deciding()
		{
			NetworkManager manager = NetworkManager.main;
			if (!manager || !manager.InRoom)
			{
				return true;
			}
			return manager.IsHost;
		}

		private static bool Held(NetworkId id)
		{
			if (!id.IsValid)
			{
				return false;
			}
			foreach (NetworkPlayer avatar in NetworkPlayer.avatars)
			{
				if (avatar && avatar.Object && avatar.Object.IsValid && (avatar.HoldL == id || avatar.HoldR == id))
				{
					return true;
				}
			}
			return false;
		}

		public static void Told(int k, Vector3 spot)
		{
			if (active && active.fired < k && (!Deciding() || active.told < k))
			{
				active.aimed[k] = spot;
				active.heard.Add(k);
			}
		}

		private void Tell(int k)
		{
			told = k;
			if (!heard.Contains(k) && NetworkManager.Clock < Due(k) - Warn)
			{
				aimed.Remove(k);
			}
			Vector3 spot = Aim(k);
			NetworkPlayer mine = NetworkPlayer.mine;
			if (mine && mine.Object && mine.Object.IsValid)
			{
				mine.RPC_Bolt(k, spot);
			}
		}

		private bool Ground(Vector3 from, float reach, out Vector3 at, out Vector3 normal)
		{
			at = from;
			normal = Vector3.up;
			Ray ray = new Ray(from, Vector3.down);
			float best = float.MaxValue;
			bool found = false;
			foreach (Collider surface in surfaces)
			{
				if (surface && surface.Raycast(ray, out RaycastHit hit, reach) && hit.distance < best)
				{
					best = hit.distance;
					at = hit.point;
					normal = hit.normal;
					found = true;
				}
			}
			return found;
		}

		private static bool Burning(Vector3 p)
		{
			foreach (Water acid in Water.acids)
			{
				if (acid && acid.Burns(p - Vector3.up))
				{
					return true;
				}
			}
			return false;
		}

		private static float Hash(int i, int salt)
		{
			uint h = (uint)i * 375809969u + (uint)salt * 668265263u;
			h ^= h >> 13;
			h *= 1274126177u;
			h ^= h >> 16;
			return (float)(h & 0xFFFFFF) / 16777216f;
		}

		private static bool Active()
		{
			if (AvatarBody.peaceful)
			{
				return false;
			}
			if (GameManager.match == null)
			{
				return true;
			}
			if (GameManager.main)
			{
				return GameManager.main.live;
			}
			return false;
		}

		private static float Overtime(float at)
		{
			GameManager main = GameManager.main;
			if (!main || !main.live)
			{
				return 0f;
			}
			float t = Mathf.Clamp01((GameManager.RoundTime - (NetworkManager.Clock - at) - main.overtimeFrom) / Mathf.Max((main.roundLength - main.overtimeFrom) * 0.6f, 1f));
			return t * t * (3f - 2f * t);
		}

		private static float Due(int k)
		{
			return k * Slot + (0.15f + Hash(k, 2) * 0.7f) * Slot;
		}

		private static bool Strikes(int k)
		{
			return Hash(k, 1) < Mathf.Lerp(Chance, OvertimeChance, Overtime(Due(k)));
		}

		private Vector3 Aim(int k)
		{
			if (!aimed.TryGetValue(k, out Vector3 spot))
			{
				spot = (spots.Count > 0) ? spots[Pick(spotSum, Hash(k, 3))] : transform.position;
				if (Deciding())
				{
					spot = Target(k, spot);
				}
				aimed[k] = spot;
			}
			return spot;
		}

		private Vector3 Target(int k, Vector3 p)
		{
			alive.Clear();
			foreach (NetworkPlayer avatar in NetworkPlayer.avatars)
			{
				if (avatar && avatar.Object && avatar.Object.IsValid && !avatar.Dead && !avatar.Watching)
				{
					alive.Add(avatar);
				}
			}
			if (alive.Count == 0)
			{
				return p;
			}
			alive.Sort((NetworkPlayer a, NetworkPlayer b) => a.Object.StateAuthority.PlayerId.CompareTo(b.Object.StateAuthority.PlayerId));
			aliveSum.Clear();
			float sum = 0f;
			float most = 0f;
			foreach (NetworkPlayer avatar in alive)
			{
				float threat = Threat(avatar.Root, avatar.KnockedOut, Held(avatar.Object.Id));
				sum += threat;
				most = Mathf.Max(most, threat);
				aliveSum.Add(sum);
			}
			if (sum <= 0f)
			{
				return p;
			}
			if (Hash(k, 4) >= Mathf.Min(most * Mathf.Lerp(AtPlayer, OvertimeAtPlayer, Overtime(Due(k))), AimCap))
			{
				return p;
			}
			Vector3 root = alive[Pick(aliveSum, Hash(k, 5))].Root;
			if (SkyHit(root, out Vector3 at, out Vector3 normal) && root.y - at.y <= LiftGap && normal.y > Elevated && !Burning(at))
			{
				return at;
			}
			return root - Vector3.up * StandHeight;
		}

		private void Update()
		{
			Thunder();
			Flash();
			float clock = NetworkManager.Clock;
			int first = Mathf.FloorToInt(clock / Slot) - 1;
			int last = Mathf.FloorToInt((clock + Warn) / Slot) + 1;
			rising.Clear();
			if (!Active())
			{
				return;
			}
			for (int k = first; k <= last; k++)
			{
				float due = Due(k);
				if (Strikes(k))
				{
					if (k <= fired)
					{
						continue;
					}
					if (k > told && clock >= due - Warn - Lead && Deciding())
					{
						Tell(k);
					}
					if (clock < due - Warn)
					{
						continue;
					}
					Vector3 spot = Aim(k);
					if (clock < due)
					{
						rising.Add(new Vector4(spot.x, spot.y, spot.z, Mathf.Clamp01((clock - due + Warn) / RiseRamp)));
						continue;
					}
					fired = k;
					if (clock - due < FlashTime)
					{
						Strike(spot);
					}
				}
				else if (k > glowed && clock >= due && clock - due < FlashTime && Hash(k, 6) < FarChance)
				{
					glowed = k;
					Glow(0.45f, 0.6f + Hash(k, 7) * 1.4f, 0.35f);
				}
			}
			Prune(first);
		}

		private void LateUpdate()
		{
			if (!rain)
			{
				return;
			}
			Transform head = (Player.main && Player.main.cam) ? Player.main.cam : (Camera.main ? Camera.main.transform : null);
			if (head)
			{
				rain.transform.position = head.position + Vector3.up * RainAbove;
			}
		}

		private void FixedUpdate()
		{
			raised.Clear();
			floated.Clear();
			foreach (Vector4 spot in rising)
			{
				Rise(spot, spot.w);
			}
		}

		private static float Lift(float rise, float ramp)
		{
			return Mathf.Max(0f, ((RiseSpeed - rise) * RiseGrip - Physics.gravity.y) * ramp);
		}

		private void Rise(Vector3 spot, float ramp)
		{
			int count = Physics.OverlapCapsuleNonAlloc(spot, spot + Vector3.up * RiseHeight, Radius, near, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < count; i++)
			{
				Rigidbody body = near[i].attachedRigidbody;
				if (!body || body.isKinematic || !raised.Add(body))
				{
					continue;
				}
				Player p = body.GetComponentInParent<Player>();
				if (!p)
				{
					body.AddForce(Vector3.up * Lift(body.linearVelocity.y, ramp), ForceMode.Acceleration);
				}
				else
				{
					if (!floated.Add(p) || !p.pelvis || p.dead)
					{
						continue;
					}
					float y = p.pelvis.position.y;
					if (SkyHit(p.pelvis.position, out Vector3 at, out Vector3 _) && at.y > y + Cover)
					{
						continue;
					}
					Vector3 pelvis = p.pelvis.position;
					if (Vector2.Distance(new Vector2(pelvis.x, pelvis.z), new Vector2(spot.x, spot.z)) > Radius)
					{
						continue;
					}
					float lift = Lift(p.pelvis.linearVelocity.y, ramp);
					if (!parts.TryGetValue(p, out Rigidbody[] bodies) || bodies.Length == 0 || !bodies[0])
					{
						bodies = p.GetComponentsInChildren<Rigidbody>();
						parts[p] = bodies;
					}
					foreach (Rigidbody part in bodies)
					{
						if (part && !part.isKinematic)
						{
							part.AddForce(lift * Vector3.up, ForceMode.Acceleration);
						}
					}
				}
			}
		}

		private void Prune(int first)
		{
			drop.Clear();
			foreach (int k in aimed.Keys)
			{
				if (k < first)
				{
					drop.Add(k);
				}
			}
			foreach (int k in drop)
			{
				aimed.Remove(k);
				heard.Remove(k);
			}
		}

		private void Strike(Vector3 spot)
		{
			if (bolt)
			{
				Object.Instantiate(bolt, spot, Quaternion.identity);
			}
			Bolt(spot);
			Glow(1f, 0f, 1f);
			Player main = Player.main;
			if (main && !main.dummy)
			{
				Zap(main, spot);
			}
			foreach (SlapDummy dummy in Object.FindObjectsByType<SlapDummy>(FindObjectsSortMode.None))
			{
				if (dummy.kind != SlapDummy.Kind.Dummy)
				{
					Player p = dummy.GetComponentInParent<Player>();
					if (!p)
					{
						p = dummy.GetComponentInChildren<Player>();
					}
					if (p)
					{
						Zap(p, spot);
					}
				}
			}
		}

		private void Zap(Player p, Vector3 spot)
		{
			if (p.dead || p.ghost || p.knockedOut || !p.pelvis || p.pelvis.isKinematic || p.Shielded())
			{
				return;
			}
			Vector3 at = p.pelvis.position;
			Vector3 flat = new Vector3(at.x - spot.x, 0f, at.z - spot.z);
			if (flat.magnitude > Radius || at.y < spot.y - 0.5f || at.y > spot.y + RiseHeight + 1f || Covered(at))
			{
				return;
			}
			if (p == Player.main)
			{
				Player.Note("struck by lightning " + flat.magnitude.ToString("F1") + " m from the bolt");
			}
			p.KnockOut(spot, true);
			Vector3 away = (flat.sqrMagnitude > 0.01f) ? flat.normalized : p.pelvis.transform.forward;
			Vector3 launch = new Vector3(away.x, 0f, away.z);
			launch = (launch.x * launch.x + launch.z * launch.z > 0.0001f) ? launch.normalized : Vector3.forward;
			foreach (Rigidbody body in p.bodies)
			{
				if (body && !body.isKinematic)
				{
					body.linearVelocity = launch * LaunchOut + Vector3.up * LaunchUp;
				}
			}
		}

		private void Glow(float size, float delay, float volume)
		{
			flashAt = Time.time;
			flashSize = size;
			if (thunderSound)
			{
				thunderVolume = volume;
				thunderDue = Time.time + delay;
			}
		}

		private void Thunder()
		{
			if (!(thunderDue < 0f) && !(Time.time < thunderDue))
			{
				thunderDue = -1f;
				thunderAudio.PlayOneShot(thunderSound, thunderVolume);
			}
		}

		private void Bolt(Vector3 spot)
		{
			if (!boltLine)
			{
				return;
			}
			if (!line)
			{
				GameObject go = new GameObject("Bolt");
				go.transform.SetParent(transform, false);
				line = go.AddComponent<LineRenderer>();
				line.sharedMaterial = boltLine;
				line.useWorldSpace = true;
				line.positionCount = BoltKinks + 1;
				line.textureMode = LineTextureMode.Stretch;
				line.shadowCastingMode = ShadowCastingMode.Off;
				line.receiveShadows = false;
				line.widthCurve = new AnimationCurve(new Keyframe(0f, BoltWide * 0.5f), new Keyframe(1f, BoltWide));
			}
			Vector3 top = spot + new Vector3(Random.Range(-8f, 8f), BoltTop, Random.Range(-8f, 8f));
			for (int i = 0; i <= BoltKinks; i++)
			{
				float t = (float)i / (float)BoltKinks;
				Vector3 point = Vector3.Lerp(top, spot, t);
				if (i % BoltKinks != 0)
				{
					point += new Vector3(Random.Range(-1.6f, 1.6f), Random.Range(-0.5f, 0.5f), Random.Range(-1.6f, 1.6f)) * (1f - t * 0.7f);
				}
				line.SetPosition(i, point);
			}
			lineLit = true;
		}

		private void Flash()
		{
			float t = Time.time - flashAt;
			float k;
			if (t < 0f || t > FlashTime)
			{
				k = 0f;
			}
			else if (t < 0.06f)
			{
				k = 1f;
			}
			else if (t < 0.12f)
			{
				k = 0.2f;
			}
			else if (t < 0.2f)
			{
				k = 0.8f;
			}
			else
			{
				k = 0.8f - Mathf.Clamp01((t - 0.2f) / 0.3f) * 0.8f;
			}
			float f = k * flashSize;
			if (line)
			{
				line.enabled = f > 0.5f && lineLit;
			}
			if (f == 0f)
			{
				lineLit = false;
				if (shown == 0f)
				{
					return;
				}
			}
			shown = f;
			if (sun)
			{
				sun.intensity = (1f + f * 2.5f) * sunBase;
			}
			float ambient = 1f + f * 1.5f;
			RenderSettings.ambientSkyColor = skyBase * ambient;
			RenderSettings.ambientEquatorColor = equatorBase * ambient;
			RenderSettings.ambientGroundColor = groundBase * ambient;
			if (sky)
			{
				Color white = new Color(0.85f, 0.9f, 1f);
				sky.SetColor(ZenithId, Color.Lerp(zenithBase, white, f * 0.7f));
				sky.SetColor(HorizonId, Color.Lerp(horizonBase, white, f * 0.6f));
				sky.SetColor(CloudLitId, Color.Lerp(litBase, white, f * 0.8f));
				sky.SetColor(CloudShadeId, Color.Lerp(shadeBase, white, f * 0.5f));
			}
		}

		private void OnDestroy()
		{
			if (active == this)
			{
				active = null;
			}
			if (sky)
			{
				Object.Destroy(sky);
			}
		}
	}
}
