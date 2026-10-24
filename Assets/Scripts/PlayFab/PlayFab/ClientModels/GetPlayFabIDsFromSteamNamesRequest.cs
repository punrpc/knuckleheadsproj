using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class GetPlayFabIDsFromSteamNamesRequest : PlayFabRequestCommon
	{
		public List<string> SteamNames;
	}
}
