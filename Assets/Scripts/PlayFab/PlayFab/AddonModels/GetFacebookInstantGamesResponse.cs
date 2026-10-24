using System;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class GetFacebookInstantGamesResponse : PlayFabResultCommon
	{
		public string AppID;

		public bool Created;
	}
}
