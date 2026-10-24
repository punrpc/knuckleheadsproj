using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class DeleteLeaderboardDefinitionRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public string Name;
	}
}
