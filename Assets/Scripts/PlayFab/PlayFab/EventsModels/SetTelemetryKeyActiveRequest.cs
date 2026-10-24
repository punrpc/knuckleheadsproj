using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class SetTelemetryKeyActiveRequest : PlayFabRequestCommon
	{
		public bool Active;

		public Dictionary<string, string> CustomTags;

		public EntityKey Entity;

		public string KeyName;
	}
}
