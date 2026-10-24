using System;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class UpdatePlayerCustomPropertiesResult : PlayFabResultCommon
	{
		public int PropertiesVersion;
	}
}
