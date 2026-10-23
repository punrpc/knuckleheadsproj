using UnityEngine;

namespace Ragdoll
{
	public class Grabbable : MonoBehaviour
	{
		[Header("Grabbable")]
		public bool holdsYou;

		public float breakForce = float.PositiveInfinity;

		public float gripForce = -1f;

		public bool pressWhileTouching;

		public float reach = -1f;

		public float friction = 0.01f;

		public float climbTime;

		private Collider[] cols;

		private PhysicsMaterial[] own;

		private PhysicsMaterial slick;

		private void Start()
		{
			foreach (Collider col in GetComponentsInChildren<Collider>())
			{
				Player.IgnoreFingers(col);
			}
			Rigidbody rb = GetComponent<Rigidbody>();
			if (rb)
			{
				rb.maxDepenetrationVelocity = 1f;
			}
		}

		public void Slip(bool on)
		{
			if (friction < 0f)
			{
				return;
			}
			if (on ? holdsYou : cols == null)
			{
				return;
			}
			if (cols == null)
			{
				cols = GetComponentsInChildren<Collider>();
				own = new PhysicsMaterial[cols.Length];
				for (int i = 0; i < cols.Length; i++)
				{
					own[i] = cols[i].sharedMaterial;
				}
				slick = new PhysicsMaterial("Carried");
				slick.staticFriction = friction;
				slick.dynamicFriction = friction;
				slick.frictionCombine = PhysicsMaterialCombine.Minimum;
			}
			for (int j = 0; j < cols.Length; j++)
			{
				if (cols[j])
				{
					cols[j].sharedMaterial = on ? slick : own[j];
				}
			}
		}
	}
}
