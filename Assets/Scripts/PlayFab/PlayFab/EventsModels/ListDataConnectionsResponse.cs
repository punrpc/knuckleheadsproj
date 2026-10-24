using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class ListDataConnectionsResponse : PlayFabResultCommon
	{
		public List<DataConnectionDetails> DataConnections;
	}
}
