using System;
using PlayFab.SharedModels;

namespace PlayFab.EconomyModels
{
	[Serializable]
	public class TransactionClawbackDetails : PlayFabBaseModel
	{
		public string TransactionIdClawedback;
	}
}
