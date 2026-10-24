using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class EntityLeaderboardEntry : PlayFabBaseModel
	{
		public string DisplayName;

		public EntityKey Entity;

		public DateTime LastUpdated;

		public string Metadata;

		public int Rank;

		public List<string> Scores;
	}
}
