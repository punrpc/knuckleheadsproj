using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class GetLeaderboardAroundEntityRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public EntityKey Entity;

		public string LeaderboardName;

		public uint MaxSurroundingEntries;

		public uint? Version;
	}
}
