using System;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class LinkedStatisticColumn : PlayFabBaseModel
	{
		public string LinkedStatisticColumnName;

		public string LinkedStatisticName;
	}
}
