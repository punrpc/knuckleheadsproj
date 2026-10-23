using UnityEngine;

namespace Ragdoll
{
	public class Smack : MonoBehaviour
	{
		[HideInInspector]
		public Player owner;

		[HideInInspector]
		public AudioSource through;

		private void OnCollisionEnter(Collision c)
		{
			if (!owner)
			{
				return;
			}
			if (owner.IsMine(c.collider.transform))
			{
				return;
			}
			if (c.collider.GetComponent<AvatarBody.AvatarHit>())
			{
				return;
			}
			owner.PlayHit(through, c.relativeVelocity.magnitude, c.impulse.magnitude, c.collider.GetComponentInParent<Player>());
		}
	}
}
