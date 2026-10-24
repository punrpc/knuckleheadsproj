using System;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class NintendoEnvironment : PlayFabBaseModel
	{
		public string ClientID;

		public string ClientSecret;

		public string ID;
	}
}
