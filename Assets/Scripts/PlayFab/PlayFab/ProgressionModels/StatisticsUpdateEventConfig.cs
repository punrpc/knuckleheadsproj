using System;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class StatisticsUpdateEventConfig : PlayFabBaseModel
	{
		public EventType EventType;
	}
}
