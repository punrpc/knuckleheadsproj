using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class LobbyServer : PlayFabBaseModel
	{
		public string PubSubConnectionHandle;

		public Dictionary<string, string> ServerData;

		public EntityKey ServerEntity;
	}
}
