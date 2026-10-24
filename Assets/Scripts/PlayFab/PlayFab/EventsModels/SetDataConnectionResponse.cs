using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class SetDataConnectionResponse : PlayFabResultCommon
	{
		public DataConnectionDetails DataConnection;
	}
}
