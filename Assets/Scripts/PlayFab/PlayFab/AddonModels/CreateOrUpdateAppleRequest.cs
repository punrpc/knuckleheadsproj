using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class CreateOrUpdateAppleRequest : PlayFabRequestCommon
	{
		public bool? AllowProduction;

		public bool? AllowSandbox;

		public string AppBundleId;

		public string AppId;

		public string AppSharedSecret;

		public Dictionary<string, string> CustomTags;

		public EntityKey Entity;

		public bool? ErrorIfExists;

		public bool? IgnoreExpirationDate;

		public string IssuerId;

		public string KeyId;

		public string PrivateKey;

		public bool? RequireSecureAuthentication;
	}
}
