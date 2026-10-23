using Liv.Lck;
using Liv.Lck.Collections;
using UnityEngine;

namespace Ragdoll
{
	public class LivEar : MonoBehaviour, ILckAudioSource
	{
		private static readonly AudioBuffer heard = new AudioBuffer(96000);

		private static readonly object gate = new object();

		private static bool on;

		public void GetAudioData(ILckAudioSource.AudioDataCallbackDelegate callback)
		{
			lock (gate)
			{
				callback(heard);
				heard.Clear();
			}
		}

		public void EnableCapture()
		{
			lock (gate)
			{
				heard.Clear();
			}
			on = true;
		}

		public void DisableCapture()
		{
			on = false;
			lock (gate)
			{
				heard.Clear();
			}
		}

		public bool IsCapturing()
		{
			return on;
		}

		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (!on)
			{
				return;
			}
			lock (gate)
			{
				heard.TryExtendFrom(data);
			}
		}
	}
}
