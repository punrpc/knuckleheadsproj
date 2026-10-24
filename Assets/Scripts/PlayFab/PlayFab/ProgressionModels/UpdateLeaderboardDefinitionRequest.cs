using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class UpdateLeaderboardDefinitionRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public LeaderboardEventEmissionConfig EventEmissionConfig;

		public string Name;

		public int? SizeLimit;

		public VersionConfiguration VersionConfiguration;
	}
}
