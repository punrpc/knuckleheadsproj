using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class SetDataConnectionActiveRequest : PlayFabRequestCommon
	{
		public bool Active;

		public Dictionary<string, string> CustomTags;

		public string Name;
	}
}
