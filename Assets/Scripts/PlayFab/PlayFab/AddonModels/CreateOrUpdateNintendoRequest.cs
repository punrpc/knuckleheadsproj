using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class CreateOrUpdateNintendoRequest : PlayFabRequestCommon
	{
		public string ApplicationID;

		public Dictionary<string, string> CustomTags;

		public EntityKey Entity;

		public List<NintendoEnvironment> Environments;

		public bool? ErrorIfExists;
	}
}
