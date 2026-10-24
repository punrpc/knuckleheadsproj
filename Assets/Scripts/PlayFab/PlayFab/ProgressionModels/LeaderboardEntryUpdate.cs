using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class LeaderboardEntryUpdate : PlayFabBaseModel
	{
		public string EntityId;

		public string Metadata;

		public List<string> Scores;
	}
}
