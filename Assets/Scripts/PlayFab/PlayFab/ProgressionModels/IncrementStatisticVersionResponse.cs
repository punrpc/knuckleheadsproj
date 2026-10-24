using System;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class IncrementStatisticVersionResponse : PlayFabResultCommon
	{
		public uint Version;
	}
}
