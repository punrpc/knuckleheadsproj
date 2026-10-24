using System;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class IncrementLeaderboardVersionResponse : PlayFabResultCommon
	{
		public uint Version;
	}
}
