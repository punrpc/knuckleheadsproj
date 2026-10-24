using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class UpdatePlayerCustomPropertiesRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public int? ExpectedPropertiesVersion;

		public List<UpdateProperty> Properties;
	}
}
