using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class GetPlayFabIDsFromBattleNetAccountIdsResult : PlayFabResultCommon
	{
		public List<BattleNetAccountPlayFabIdPair> Data;
	}
}
