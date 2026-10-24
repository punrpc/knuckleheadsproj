using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class CreateOrUpdateToxModRequest : PlayFabRequestCommon
	{
		public string AccountId;

		public string AccountKey;

		public Dictionary<string, string> CustomTags;

		public bool Enabled;

		public EntityKey Entity;

		public bool? ErrorIfExists;
	}
}
