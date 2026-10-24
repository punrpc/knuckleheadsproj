using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class CreateOrUpdateFacebookInstantGamesRequest : PlayFabRequestCommon
	{
		public string AppID;

		public string AppSecret;

		public Dictionary<string, string> CustomTags;

		public EntityKey Entity;

		public bool? ErrorIfExists;
	}
}
