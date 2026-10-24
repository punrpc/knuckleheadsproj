using System;
using PlayFab.SharedModels;

namespace PlayFab.EconomyModels
{
	[Serializable]
	public class GetInventoryOperationStatusResponse : PlayFabResultCommon
	{
		public string OperationStatus;
	}
}
