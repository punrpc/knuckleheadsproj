using System;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class CustomPropertyDetails : PlayFabBaseModel
	{
		public string Name;

		public object Value;
	}
}
