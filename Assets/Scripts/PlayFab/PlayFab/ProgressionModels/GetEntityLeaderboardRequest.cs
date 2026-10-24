using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class GetEntityLeaderboardRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public string LeaderboardName;

		public uint PageSize;

		public uint? StartingPosition;

		public uint? Version;
	}
}
