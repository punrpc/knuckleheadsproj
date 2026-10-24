using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class DataConnectionAzureBlobSettings : PlayFabBaseModel
	{
		public string AccountName;

		public string ContainerName;

		public string TenantId;
	}
}
