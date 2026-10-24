using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EconomyModels
{
	[Serializable]
	public class Review : PlayFabBaseModel
	{
		public Dictionary<string, int> CategoryRatings;

		public int HelpfulNegative;

		public int HelpfulPositive;

		public bool IsInstalled;

		public string ItemId;

		public string ItemVersion;

		public string Locale;

		public int Rating;

		public EntityKey ReviewerEntity;

		public string ReviewId;

		public string ReviewText;

		public DateTime Submitted;

		public string Title;
	}
}
