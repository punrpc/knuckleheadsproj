using UnityEngine;

namespace Ragdoll
{
	public class Impact : MonoBehaviour
	{
		[HideInInspector]
		public Player owner;

		[HideInInspector]
		public Rigidbody bone;

		private void OnCollisionEnter(Collision c)
		{
			if (!owner)
			{
				return;
			}
			if (owner.dummy && (!c.rigidbody || c.rigidbody.isKinematic))
			{
				return;
			}
			if (c.collider.transform.IsChildOf(owner.transform))
			{
				return;
			}
			if (owner.Holds(c.rigidbody))
			{
				return;
			}
			if (c.collider.GetComponent<AvatarBody.AvatarHit>())
			{
				return;
			}
			if (c.collider.GetComponentInParent<NetworkNpc>())
			{
				return;
			}
			Player player = c.rigidbody ? c.rigidbody.GetComponentInParent<Player>() : null;
			if (player && (player.dummy || owner.dummy))
			{
				return;
			}
			NetProp prop = c.rigidbody ? c.rigidbody.GetComponent<NetProp>() : null;
			if (prop && (prop.OtherHolds || prop.TeamToss))
			{
				return;
			}
			owner.TakeImpact(bone, c.impulse.magnitude, c.relativeVelocity.magnitude, c.GetContact(0).point, c.rigidbody);
		}
	}
}
