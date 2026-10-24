using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class EntityStatistics : PlayFabBaseModel
	{
		public EntityKey EntityKey;

		public List<EntityStatisticValue> Statistics;
	}
}
