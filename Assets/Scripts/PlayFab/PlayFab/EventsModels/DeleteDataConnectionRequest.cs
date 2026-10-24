using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class DeleteDataConnectionRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public string Name;
	}
}
