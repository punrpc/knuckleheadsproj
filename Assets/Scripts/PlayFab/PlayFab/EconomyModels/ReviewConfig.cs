using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EconomyModels
{
	[Serializable]
	public class ReviewConfig : PlayFabBaseModel
	{
		public List<CategoryRatingConfig> CategoryRatings;
	}
}
