using System;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class GetToxModResponse : PlayFabResultCommon
	{
		public string AccountId;

		public string AccountKey;

		public bool Created;

		public bool Enabled;
	}
}
