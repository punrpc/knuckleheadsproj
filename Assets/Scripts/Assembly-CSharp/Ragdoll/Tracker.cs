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

		public static float frameAt;

		public static float frameDt = 1f / 90f;

		private static bool stepSample = true;

		private const float StepLead = 0.5f;

		private const float StepJump = 0.3f;

		private InputDevice device;

		private int frame = -1;

		private int repeat;

		private Vector3 prevPos;

		private Vector3 nowPos;

		private Quaternion prevRot = Quaternion.identity;

		private Quaternion nowRot = Quaternion.identity;

		private float prevAt = -1f;

		private float nowAt = -1f;

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
			bool hasRot = device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot);
			posed = has;
			repeat = Time.frameCount == frame ? repeat + 1 : 0;
			frame = Time.frameCount;
			if (extrapolate && stepSample)
			{
				if (has && hasRot)
				{
					Sample(ref pos, ref rot);
				}
				if (has)
				{
					target.localPosition = pos;
				}
			}
			else if (has)
			{
				if (extrapolate && repeat >= 1 && Time.inFixedTimeStep && device.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 vel))
				{
					pos += vel * (Time.fixedDeltaTime * repeat);
				}
				target.localPosition = pos;
			}
			if (hasRot)
			{
				target.localRotation = rot;
			}
			bool gotTracked = device.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked);
			tracking = gotTracked ? (isTracked && has) : has;
		}

		private void Sample(ref Vector3 pos, ref Quaternion rot)
		{
			bool step = Time.inFixedTimeStep;
			if ((pos - nowPos).sqrMagnitude >= 1E-10f || Quaternion.Dot(rot, nowRot) <= 0.999999f)
			{
				prevPos = nowPos;
				prevRot = nowRot;
				prevAt = nowAt;
				nowPos = pos;
				nowRot = rot;
				if (step)
				{
					nowAt = frameAt + frameDt;
				}
			}
			if (!step)
			{
				nowAt = Time.time;
				return;
			}
			if ((nowPos - prevPos).sqrMagnitude > StepJump * StepJump)
			{
				return;
			}
			float t = Blend(prevAt, nowAt, Time.fixedTime + Time.fixedDeltaTime * StepLead);
			if (t < 0f)
			{
				return;
			}
			pos = Vector3.LerpUnclamped(prevPos, nowPos, t);
			rot = Quaternion.SlerpUnclamped(prevRot, nowRot, t);
		}

		public static float Blend(float prevAt, float nowAt, float stepAt)
		{
			if (prevAt < 0f)
			{
				return -1f;
			}
			float span = nowAt - prevAt;
			if (span < 0.001f || span > Time.maximumDeltaTime)
			{
				return -1f;
			}
			return Mathf.Clamp((stepAt - prevAt) / span, 0f, 2f);
		}

		public void Clear()
		{
			present = false;
			tracking = false;
		}
	}
}
