using System;
using PlayFab.SharedModels;

namespace PlayFab.EconomyModels
{
	[Serializable]
	public class RedemptionSuccess : PlayFabBaseModel
	{
		public DateTime? ExpirationTimestamp;

		public string MarketplaceAlternateId;

		public string MarketplaceTransactionId;

		public DateTime SuccessTimestamp;
	}
}
