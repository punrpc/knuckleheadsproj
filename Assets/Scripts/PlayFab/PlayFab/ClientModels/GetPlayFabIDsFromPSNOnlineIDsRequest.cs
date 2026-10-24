using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class GetPlayFabIDsFromPSNOnlineIDsRequest : PlayFabRequestCommon
	{
		public int? IssuerId;

		public List<string> PSNOnlineIDs;
	}
}
