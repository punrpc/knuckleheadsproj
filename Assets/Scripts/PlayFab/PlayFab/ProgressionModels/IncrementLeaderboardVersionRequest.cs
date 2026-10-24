using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class IncrementLeaderboardVersionRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public string Name;
	}
}
