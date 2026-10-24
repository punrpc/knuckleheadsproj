using System;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class GetPlayerCustomPropertyResult : PlayFabResultCommon
	{
		public int PropertiesVersion;

		public CustomPropertyDetails Property;
	}
}
