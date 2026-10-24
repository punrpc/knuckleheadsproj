using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class UpdateStatisticDefinitionRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public StatisticsEventEmissionConfig EventEmissionConfig;

		public string Name;

		public VersionConfiguration VersionConfiguration;
	}
}
