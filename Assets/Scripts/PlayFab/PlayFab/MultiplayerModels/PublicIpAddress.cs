using System;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class PublicIpAddress : PlayFabBaseModel
	{
		public string FQDN;

		public string IpAddress;

		public string RoutingType;
	}
}
