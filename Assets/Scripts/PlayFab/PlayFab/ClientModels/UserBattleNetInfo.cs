using System;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class UserBattleNetInfo : PlayFabBaseModel
	{
		public string BattleNetAccountId;

		public string BattleNetBattleTag;
	}
}
