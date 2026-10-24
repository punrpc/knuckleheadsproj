using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class ListLeaderboardDefinitionsResponse : PlayFabResultCommon
	{
		public List<LeaderboardDefinition> LeaderboardDefinitions;

		public int PageSize;

		public string SkipToken;
	}
}
