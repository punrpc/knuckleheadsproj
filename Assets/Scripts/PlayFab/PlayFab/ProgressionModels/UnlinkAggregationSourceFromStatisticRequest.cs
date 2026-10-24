using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class UnlinkAggregationSourceFromStatisticRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public string Name;

		public string SourceStatisticName;
	}
}
