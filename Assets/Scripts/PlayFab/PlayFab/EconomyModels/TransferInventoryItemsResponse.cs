using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EconomyModels
{
	[Serializable]
	public class TransferInventoryItemsResponse : PlayFabResultCommon
	{
		public string GivingETag;

		public List<string> GivingTransactionIds;

		public string IdempotencyId;

		public string OperationStatus;

		public string OperationToken;

		public List<string> ReceivingTransactionIds;
	}
}
