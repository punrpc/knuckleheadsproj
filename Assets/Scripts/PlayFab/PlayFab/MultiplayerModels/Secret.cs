using System;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class Secret : PlayFabBaseModel
	{
		public DateTime? ExpirationDate;

		public string Name;

		public string Value;
	}
}
