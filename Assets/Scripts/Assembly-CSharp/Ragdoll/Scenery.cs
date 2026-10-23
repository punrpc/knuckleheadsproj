using UnityEngine;

namespace Ragdoll
{
	public class Scenery : MonoBehaviour
	{
		private Rigidbody body;

		private void Awake()
		{
			body = GetComponent<Rigidbody>();
		}

		private void OnCollisionEnter(Collision c)
		{
			if ((bool)TrainWreck.main)
			{
				TrainWreck.main.Touched(c, body);
			}
		}

		private void OnCollisionStay(Collision c)
		{
			if ((bool)TrainWreck.main)
			{
				TrainWreck.main.Touched(c, body);
			}
		}
	}
}
