using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProfilesModels
{
	[Serializable]
	public class SetDisplayNameRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public string DisplayName;

		public EntityKey Entity;

		public int? ExpectedVersion;
	}
}
