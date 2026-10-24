using System;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class GetPlayerCustomPropertyRequest : PlayFabRequestCommon
	{
		public string PropertyName;
	}
}
