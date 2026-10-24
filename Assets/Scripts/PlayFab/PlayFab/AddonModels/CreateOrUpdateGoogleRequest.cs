using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class CreateOrUpdateGoogleRequest : PlayFabRequestCommon
	{
		public string AppLicenseKey;

		public string AppPackageID;

		public Dictionary<string, string> CustomTags;

		public EntityKey Entity;

		public bool? ErrorIfExists;

		public string OAuthClientID;

		public string OAuthClientSecret;

		public string OAuthCustomRedirectUri;

		public string ServiceAccountKey;
	}
}
