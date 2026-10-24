using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EconomyModels
{
	[Serializable]
	public class RealMoneyPriceDetails : PlayFabBaseModel
	{
		public Dictionary<string, int> AppleAppStorePrices;

		public Dictionary<string, int> GooglePlayPrices;

		public Dictionary<string, int> MicrosoftStorePrices;

		public Dictionary<string, int> NintendoEShopPrices;

		public Dictionary<string, int> PlayStationStorePrices;

		public Dictionary<string, int> SteamPrices;
	}
}
