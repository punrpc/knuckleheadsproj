using UnityEngine;

namespace Ragdoll
{
	public class HandTouch : MonoBehaviour
	{
		[HideInInspector]
		public bool touching;

		[HideInInspector]
		public bool holdsYou;

		[HideInInspector]
		public Collider holds;

		[HideInInspector]
		public bool rope;

		[HideInInspector]
		public bool carried;

		[HideInInspector]
		public bool resting;

		[HideInInspector]
		public Transform holder;

		[HideInInspector]
		public bool self;

		[HideInInspector]
		public Collider selfCol;

		[HideInInspector]
		public float selfPush;

		[HideInInspector]
		public Transform own;

		public float heavy = 20f;

		public float linger = 0.1f;

		public float underImpulse = 1.5f;

		public float worldMass = 25f;

		private Player mine;

		[HideInInspector]
		public bool world;

		private float untilWorld;

		[HideInInspector]
		public Collider worldCol;

		[HideInInspector]
		public Vector3 worldPoint;

		[HideInInspector]
		public float worldUntil;

		[HideInInspector]
		public bool worldTop;

		[HideInInspector]
		public bool under;

		private float untilUnder;

		[HideInInspector]
		public string lastHit = "";

		[HideInInspector]
		public float lastImpulse;

		[HideInInspector]
		public string hitName = "";

		private float impAcc;

		private float until;

		private float untilHolds;

		private float untilRope;

		private float untilSelf;

		private float pushAcc;

		private float untilRest;

		private void OnCollisionStay(Collision c)
		{
			if (own && c.transform.IsChildOf(own))
			{
				untilSelf = Time.time + 0.1f;
				selfCol = c.collider;
				pushAcc += c.impulse.magnitude;
				if (!carried)
				{
					return;
				}
				if (!(c.impulse.magnitude > underImpulse))
				{
					return;
				}
				if (c.rigidbody && c.rigidbody.GetComponent<HandTouch>())
				{
					return;
				}
				int count = c.contactCount;
				for (int i = 0; i < count; i++)
				{
					if (c.GetContact(i).normal.y < -0.5f)
					{
						untilUnder = Time.time + linger;
						return;
					}
				}
				return;
			}
			Rigidbody rb = c.rigidbody;
			Vector3 impulse = c.impulse;
			if (impulse.magnitude > impAcc)
			{
				impAcc = impulse.magnitude;
				hitName = c.collider.name;
			}
			Grabbable grabbable = c.collider.GetComponentInParent<Grabbable>();
			if (grabbable && grabbable.holdsYou)
			{
				untilHolds = Time.time + 0.1f;
				if (!Player.wallTouch || c.collider.GetComponentInParent<Rope>() || c.collider.GetComponentInParent<AvatarBody>() || c.collider.GetComponentInParent<NetworkPlayer>())
				{
					untilRope = Time.time + 0.1f;
					holds = c.collider;
					if (!carried)
					{
						return;
					}
				}
				else if (!carried && rb && !rb.isKinematic && rb.mass < heavy)
				{
					return;
				}
			}
			else if (rb && !rb.isKinematic && rb.mass < heavy)
			{
				return;
			}
			until = Time.time + linger;
			if (!rb || rb.isKinematic || Heavy(rb))
			{
				untilWorld = Time.time + linger;
			}
			if ((!rb || rb.isKinematic) && c.contactCount >= 1 && (!grabbable || grabbable.enabled) && !TrainWreck.Moving(rb))
			{
				worldCol = c.collider;
				worldPoint = c.GetContact(0).point;
				worldTop = false;
				worldUntil = Time.time + linger;
				for (int j = 0; j < c.contactCount; j++)
				{
					if (c.GetContact(j).normal.y > 0.5f)
					{
						worldTop = true;
						break;
					}
				}
			}
			lastHit = c.collider.name;
			if (!carried)
			{
				return;
			}
			int contactCount = c.contactCount;
			for (int k = 0; k < contactCount; k++)
			{
				ContactPoint contact = c.GetContact(k);
				if (contact.normal.y > 0.5f && (!holder || contact.point.y > holder.position.y))
				{
					untilRest = Time.time + linger;
					return;
				}
			}
		}

		private bool Heavy(Rigidbody other)
		{
			if (worldMass <= 0f || other.mass < worldMass)
			{
				return false;
			}
			if (other.GetComponentInParent<Player>())
			{
				return false;
			}
			if (!mine && own)
			{
				mine = own.GetComponent<Player>();
			}
			if (!mine)
			{
				return true;
			}
			return !mine.Holds(other);
		}

		private void FixedUpdate()
		{
			touching = Time.time < until;
			world = Time.time < untilWorld;
			under = Time.time < untilUnder;
			resting = Time.time < untilRest;
			holdsYou = Time.time < untilHolds;
			rope = Time.time < untilRope;
			selfPush = pushAcc;
			pushAcc = 0f;
			lastImpulse = impAcc;
			impAcc = 0f;
			self = Time.time < untilSelf;
		}
	}
}
