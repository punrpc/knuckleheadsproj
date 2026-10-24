using System;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class LeaderboardVersionEndConfig : PlayFabBaseModel
	{
		public EventType EventType;
	}
}
