using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class SetDataConnectionActiveResponse : PlayFabResultCommon
	{
		public DataConnectionDetails DataConnection;

		public bool WasUpdated;
	}
}
