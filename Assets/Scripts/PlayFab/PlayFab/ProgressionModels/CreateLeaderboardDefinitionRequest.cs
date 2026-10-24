using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class CreateLeaderboardDefinitionRequest : PlayFabRequestCommon
	{
		public List<LeaderboardColumn> Columns;

		public Dictionary<string, string> CustomTags;

		public string EntityType;

		public LeaderboardEventEmissionConfig EventEmissionConfig;

		public string Name;

		public int SizeLimit;

		public VersionConfiguration VersionConfiguration;
	}
}
