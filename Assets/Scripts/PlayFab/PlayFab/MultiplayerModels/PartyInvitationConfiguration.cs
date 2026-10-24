using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class PartyInvitationConfiguration : PlayFabBaseModel
	{
		public List<EntityKey> EntityKeys;

		public string Identifier;

		public string Revocability;
	}
}
