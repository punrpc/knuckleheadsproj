using System;
using PlayFab.SharedModels;

namespace PlayFab.EconomyModels
{
	[Serializable]
	public class StoreDetails : PlayFabBaseModel
	{
		public FilterOptions FilterOptions;

		public Permissions Permissions;

		public CatalogPriceOptionsOverride PriceOptionsOverride;
	}
}
