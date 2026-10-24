using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class GetStatisticsResponse : PlayFabResultCommon
	{
		public Dictionary<string, StatisticColumnCollection> ColumnDetails;

		public EntityKey Entity;

		public Dictionary<string, EntityStatisticValue> Statistics;
	}
}
