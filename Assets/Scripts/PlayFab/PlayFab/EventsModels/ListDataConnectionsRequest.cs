using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class ListDataConnectionsRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;
	}
}
