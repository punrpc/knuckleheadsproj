using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class GetDataConnectionRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public string Name;
	}
}
