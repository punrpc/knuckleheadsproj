using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class DataConnectionSettings : PlayFabBaseModel
	{
		public DataConnectionAzureBlobSettings AzureBlobSettings;

		public DataConnectionAzureDataExplorerSettings AzureDataExplorerSettings;

		public DataConnectionFabricKQLSettings AzureFabricKQLSettings;
	}
}
