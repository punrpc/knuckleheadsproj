using System;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class GetAppleResponse : PlayFabResultCommon
	{
		public string AppBundleId;

		public bool Created;

		public bool? IgnoreExpirationDate;

		public bool? RequireSecureAuthentication;
	}
}
