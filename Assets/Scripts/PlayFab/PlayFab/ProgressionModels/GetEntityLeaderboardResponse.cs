using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class GetEntityLeaderboardResponse : PlayFabResultCommon
	{
		public List<LeaderboardColumn> Columns;

		public uint EntryCount;

		public DateTime? NextReset;

		public List<EntityLeaderboardEntry> Rankings;

		public uint Version;
	}
}
