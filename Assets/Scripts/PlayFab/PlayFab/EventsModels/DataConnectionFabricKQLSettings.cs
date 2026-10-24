using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class DataConnectionFabricKQLSettings : PlayFabBaseModel
	{
		public string ClusterUri;

		public string Database;

		public string Table;
	}
}
