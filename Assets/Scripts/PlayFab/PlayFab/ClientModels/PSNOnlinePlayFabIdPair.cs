using System;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class PSNOnlinePlayFabIdPair : PlayFabBaseModel
	{
		public string PlayFabId;

		public string PSNOnlineId;
	}
}
