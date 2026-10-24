using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class ListLeaderboardDefinitionsRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public int? PageSize;

		public string SkipToken;
	}
}
