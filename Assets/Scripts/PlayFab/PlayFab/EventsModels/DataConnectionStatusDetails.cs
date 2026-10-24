using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class DataConnectionStatusDetails : PlayFabBaseModel
	{
		public string Error;

		public string ErrorMessage;

		public DateTime? MostRecentErrorTime;

		public DataConnectionErrorState? State;
	}
}
