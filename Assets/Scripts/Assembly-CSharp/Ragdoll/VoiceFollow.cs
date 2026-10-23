using Fusion;
using Photon.Voice.Fusion;

namespace Ragdoll
{
	public class VoiceFollow : FusionVoiceClient
	{
		private NetworkRunner runner;

		protected override bool LeaderInRoom
		{
			get
			{
				if ((bool)runner && runner.IsInSession)
				{
					return !string.IsNullOrEmpty(runner.SessionInfo.Name);
				}
				return false;
			}
		}

		protected override void Awake()
		{
			runner = GetComponent<NetworkRunner>();
			base.Awake();
		}

		protected override string GetVoiceRoomName()
		{
			return runner.SessionInfo.Name + "_voice";
		}
	}
}
