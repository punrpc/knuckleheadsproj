using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class CreateOrUpdateSteamRequest : PlayFabRequestCommon
	{
		public string ApplicationId;

		public Dictionary<string, string> CustomTags;

		public bool? EnforceServiceSpecificTickets;

		public EntityKey Entity;

		public bool? ErrorIfExists;

		public string SecretKey;

		public bool? UseSandbox;
	}
}
