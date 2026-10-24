using System;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class GetGoogleResponse : PlayFabResultCommon
	{
		public string AppPackageID;

		public bool Created;

		public string OAuthClientID;

		public string OauthCustomRedirectUri;
	}
}
