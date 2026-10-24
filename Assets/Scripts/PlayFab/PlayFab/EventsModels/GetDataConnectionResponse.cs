using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class GetDataConnectionResponse : PlayFabResultCommon
	{
		public DataConnectionDetails DataConnection;
	}
}
