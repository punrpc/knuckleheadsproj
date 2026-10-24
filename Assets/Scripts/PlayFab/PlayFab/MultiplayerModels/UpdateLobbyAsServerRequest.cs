using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class UpdateLobbyAsServerRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public string LobbyId;

		public Dictionary<string, string> ServerData;

		public List<string> ServerDataToDelete;

		public EntityKey ServerEntity;
	}
}
