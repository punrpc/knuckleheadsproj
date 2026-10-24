using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class ListStatisticDefinitionsResponse : PlayFabResultCommon
	{
		public int PageSize;

		public string SkipToken;

		public List<StatisticDefinition> StatisticDefinitions;
	}
}
