using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class ListPlayerCustomPropertiesResult : PlayFabResultCommon
	{
		public List<CustomPropertyDetails> Properties;

		public int PropertiesVersion;
	}
}
