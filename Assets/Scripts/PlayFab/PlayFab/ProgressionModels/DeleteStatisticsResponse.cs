using System;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class DeleteStatisticsResponse : PlayFabResultCommon
	{
		public EntityKey Entity;
	}
}
