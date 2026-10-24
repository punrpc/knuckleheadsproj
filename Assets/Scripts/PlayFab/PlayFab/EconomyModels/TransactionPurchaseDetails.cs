using System;
using PlayFab.SharedModels;

namespace PlayFab.EconomyModels
{
	[Serializable]
	public class TransactionPurchaseDetails : PlayFabBaseModel
	{
		public string ItemFriendlyId;

		public string ItemId;

		public string StoreFriendlyId;

		public string StoreId;
	}
}
