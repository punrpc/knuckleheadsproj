using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class JoinLobbyAsServerRequest : PlayFabRequestCommon
	{
		public string ConnectionString;

		public Dictionary<string, string> CustomTags;

		public Dictionary<string, string> ServerData;

		public EntityKey ServerEntity;
	}
}
