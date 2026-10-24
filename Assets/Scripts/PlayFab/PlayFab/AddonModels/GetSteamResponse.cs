using System;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class GetSteamResponse : PlayFabResultCommon
	{
		public string ApplicationId;

		public bool Created;

		public bool? EnforceServiceSpecificTickets;

		public bool? UseSandbox;
	}
}
