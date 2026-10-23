using UnityEngine;
using UnityEngine.XR;

namespace Ragdoll
{
	public class Tracker
	{
		public Transform target;

		public XRNode node;

		public bool enabled = true;

		public bool tracking;

		public bool posed;

		public bool present;

		public bool extrapolate;

		private InputDevice device;

		private int frame = -1;

		private int repeat;

		public Tracker(Transform target, XRNode node)
		{
			this.target = target;
			this.node = node;
		}

		public void Follow()
		{
			if (!device.isValid)
			{
				device = InputDevices.GetDeviceAtXRNode(node);
			}
			present = device.isValid;
			bool has = device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos);
			posed = has;
			repeat = Time.frameCount == frame ? repeat + 1 : 0;
			frame = Time.frameCount;
			if (has)
			{
				if (extrapolate && repeat >= 1 && Time.inFixedTimeStep && device.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 vel))
				{
					pos += vel * (Time.fixedDeltaTime * repeat);
				}
				target.localPosition = pos;
			}
			if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot))
			{
				target.localRotation = rot;
			}
			bool gotTracked = device.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked);
			tracking = gotTracked ? (isTracked && has) : has;
		}

		public void Clear()
		{
			present = false;
			tracking = false;
		}
	}
}
