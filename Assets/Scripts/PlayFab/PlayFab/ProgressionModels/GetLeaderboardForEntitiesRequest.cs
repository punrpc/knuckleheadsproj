using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class GetLeaderboardForEntitiesRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public List<string> EntityIds;

		public string LeaderboardName;

		public uint? Version;
	}
}
