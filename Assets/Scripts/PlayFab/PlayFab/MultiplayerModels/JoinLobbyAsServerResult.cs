using System;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class JoinLobbyAsServerResult : PlayFabResultCommon
	{
		public string LobbyId;
	}
}
