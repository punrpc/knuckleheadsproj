using System;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class SecretSummary : PlayFabBaseModel
	{
		public DateTime? ExpirationDate;

		public string Name;

		public string Version;
	}
}
