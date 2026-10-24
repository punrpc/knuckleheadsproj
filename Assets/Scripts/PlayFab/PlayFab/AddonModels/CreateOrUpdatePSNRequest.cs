using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class CreateOrUpdatePSNRequest : PlayFabRequestCommon
	{
		public string ClientID;

		public string ClientSecret;

		public Dictionary<string, string> CustomTags;

		public EntityKey Entity;

		public bool? ErrorIfExists;

		public string NextGenClientID;

		public string NextGenClientSecret;
	}
}
