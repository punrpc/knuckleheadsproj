using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class GetNintendoResponse : PlayFabResultCommon
	{
		public string ApplicationID;

		public bool Created;

		public List<NintendoEnvironment> Environments;
	}
}
