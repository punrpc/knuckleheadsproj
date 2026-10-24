using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class GetStatisticsForEntitiesResponse : PlayFabResultCommon
	{
		public Dictionary<string, StatisticColumnCollection> ColumnDetails;

		public List<EntityStatistics> EntitiesStatistics;
	}
}
