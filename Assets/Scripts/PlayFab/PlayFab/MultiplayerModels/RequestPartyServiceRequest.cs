using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class RequestPartyServiceRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public PartyNetworkConfiguration NetworkConfiguration;

		public string PartyId;

		public string PlayFabId;

		public List<string> PreferredRegions;
	}
}
