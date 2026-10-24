using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class UpdateLeaderboardEntriesRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public List<LeaderboardEntryUpdate> Entries;

		public string LeaderboardName;
	}
}
