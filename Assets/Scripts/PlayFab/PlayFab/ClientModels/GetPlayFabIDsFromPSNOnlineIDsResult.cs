using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class GetPlayFabIDsFromPSNOnlineIDsResult : PlayFabResultCommon
	{
		public List<PSNOnlinePlayFabIdPair> Data;
	}
}
