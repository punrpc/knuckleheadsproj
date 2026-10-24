using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class GetStatisticsForEntitiesRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public List<EntityKey> Entities;

		public List<string> StatisticNames;
	}
}
