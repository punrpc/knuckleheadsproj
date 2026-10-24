using System;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class RequestPartyServiceResponse : PlayFabResultCommon
	{
		public string InvitationId;

		public string PartyId;

		public string SerializedNetworkDescriptor;
	}
}
