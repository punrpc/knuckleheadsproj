using System;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class BattleNetAccountPlayFabIdPair : PlayFabBaseModel
	{
		public string BattleNetAccountId;

		public string PlayFabId;
	}
}
