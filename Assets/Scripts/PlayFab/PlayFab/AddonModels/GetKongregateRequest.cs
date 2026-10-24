using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class GetKongregateRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public EntityKey Entity;
	}
}
