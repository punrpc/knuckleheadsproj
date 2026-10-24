using System;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class PartyNetworkConfiguration : PlayFabBaseModel
	{
		public string DirectPeerConnectivityOptions;

		public uint MaxDevices;

		public uint MaxDevicesPerUser;

		public uint MaxEndpointsPerDevice;

		public uint MaxUsers;

		public uint MaxUsersPerDevice;

		public PartyInvitationConfiguration PartyInvitationConfiguration;
	}
}
