using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class DeletePlayerCustomPropertiesResult : PlayFabResultCommon
	{
		public List<DeletedPropertyDetails> DeletedProperties;

		public int PropertiesVersion;
	}
}
