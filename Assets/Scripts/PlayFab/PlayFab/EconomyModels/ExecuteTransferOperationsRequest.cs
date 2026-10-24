using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EconomyModels
{
	[Serializable]
	public class ExecuteTransferOperationsRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public string GivingCollectionId;

		public EntityKey GivingEntity;

		public string GivingETag;

		public string IdempotencyId;

		public List<TransferInventoryItemsOperation> Operations;

		public string ReceivingCollectionId;

		public EntityKey ReceivingEntity;
	}
}
