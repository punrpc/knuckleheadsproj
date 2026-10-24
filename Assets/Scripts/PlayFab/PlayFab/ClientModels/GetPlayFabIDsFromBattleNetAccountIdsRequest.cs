using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class GetPlayFabIDsFromBattleNetAccountIdsRequest : PlayFabRequestCommon
	{
		public List<string> BattleNetAccountIds;
	}
}
