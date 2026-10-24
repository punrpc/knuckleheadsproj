using System;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class LeaderboardEventEmissionConfig : PlayFabBaseModel
	{
		public LeaderboardEntityRankOnVersionEndConfig EntityRankOnVersionEndConfig;

		public LeaderboardVersionEndConfig VersionEndConfig;
	}
}
