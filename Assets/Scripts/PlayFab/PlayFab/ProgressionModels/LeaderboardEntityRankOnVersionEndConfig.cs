using System;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class LeaderboardEntityRankOnVersionEndConfig : PlayFabBaseModel
	{
		public EventType EventType;

		public int RankLimit;
	}
}
