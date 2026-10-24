using System;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class LeaderboardColumn : PlayFabBaseModel
	{
		public LinkedStatisticColumn LinkedStatisticColumn;

		public string Name;

		public LeaderboardSortDirection SortDirection;
	}
}
