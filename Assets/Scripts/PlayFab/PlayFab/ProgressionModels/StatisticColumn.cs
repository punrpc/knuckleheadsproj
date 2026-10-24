using System;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class StatisticColumn : PlayFabBaseModel
	{
		public StatisticAggregationMethod AggregationMethod;

		public string Name;
	}
}
