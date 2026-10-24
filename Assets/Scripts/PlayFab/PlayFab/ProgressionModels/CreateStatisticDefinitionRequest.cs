using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class CreateStatisticDefinitionRequest : PlayFabRequestCommon
	{
		public List<string> AggregationSources;

		public List<StatisticColumn> Columns;

		public Dictionary<string, string> CustomTags;

		public string EntityType;

		public StatisticsEventEmissionConfig EventEmissionConfig;

		public string Name;

		public VersionConfiguration VersionConfiguration;
	}
}
