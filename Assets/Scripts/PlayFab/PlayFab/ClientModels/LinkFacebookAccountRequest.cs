using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class LinkFacebookAccountRequest : PlayFabRequestCommon
	{
		public string AccessToken;

		public string AuthenticationToken;

		public Dictionary<string, string> CustomTags;

		public bool? ForceLink;
	}
}
