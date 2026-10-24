using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class StatisticDefinition : PlayFabBaseModel
	{
		public List<string> AggregationDestinations;

		public List<string> AggregationSources;

		public List<StatisticColumn> Columns;

		public DateTime Created;

		public string EntityType;

		public StatisticsEventEmissionConfig EventEmissionConfig;

		public DateTime? LastResetTime;

		public List<string> LinkedLeaderboardNames;

		public string Name;

		public uint Version;

		public VersionConfiguration VersionConfiguration;
	}
}
