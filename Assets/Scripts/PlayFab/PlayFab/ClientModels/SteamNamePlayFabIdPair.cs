using System;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class SteamNamePlayFabIdPair : PlayFabBaseModel
	{
		public string PlayFabId;

		public string SteamName;
	}
}
