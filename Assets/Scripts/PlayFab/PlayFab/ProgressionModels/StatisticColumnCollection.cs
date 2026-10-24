using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class StatisticColumnCollection : PlayFabBaseModel
	{
		public List<StatisticColumn> Columns;
	}
}
