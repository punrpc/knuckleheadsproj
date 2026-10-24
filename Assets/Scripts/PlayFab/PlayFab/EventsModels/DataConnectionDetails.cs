using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class DataConnectionDetails : PlayFabBaseModel
	{
		public DataConnectionSettings ConnectionSettings;

		public bool IsActive;

		public string Name;

		public DataConnectionStatusDetails Status;

		public DataConnectionType Type;
	}
}
