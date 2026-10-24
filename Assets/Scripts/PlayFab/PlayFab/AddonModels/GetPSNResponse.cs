using System;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class GetPSNResponse : PlayFabResultCommon
	{
		public string ClientID;

		public bool Created;

		public string NextGenClientID;
	}
}
