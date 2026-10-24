using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class GetLeaderboardDefinitionResponse : PlayFabResultCommon
	{
		public List<LeaderboardColumn> Columns;

		public DateTime Created;

		public string EntityType;

		public LeaderboardEventEmissionConfig EventEmissionConfig;

		public DateTime? LastResetTime;

		public string Name;

		public int SizeLimit;

		public uint Version;

		public VersionConfiguration VersionConfiguration;
	}
}
