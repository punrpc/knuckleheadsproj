using System;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class GetTwitchResponse : PlayFabResultCommon
	{
		public string ClientID;

		public bool Created;
	}
}
