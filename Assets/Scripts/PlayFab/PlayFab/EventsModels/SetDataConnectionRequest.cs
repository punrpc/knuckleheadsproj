using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class SetDataConnectionRequest : PlayFabRequestCommon
	{
		public DataConnectionSettings ConnectionSettings;

		public Dictionary<string, string> CustomTags;

		public bool IsActive;

		public string Name;

		public DataConnectionType Type;
	}
}
