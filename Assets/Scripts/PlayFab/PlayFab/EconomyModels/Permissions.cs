using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EconomyModels
{
	[Serializable]
	public class Permissions : PlayFabBaseModel
	{
		public List<string> SegmentIds;
	}
}
