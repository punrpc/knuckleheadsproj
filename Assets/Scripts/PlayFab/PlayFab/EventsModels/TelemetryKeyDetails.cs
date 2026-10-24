using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class TelemetryKeyDetails : PlayFabBaseModel
	{
		public DateTime CreateTime;

		public bool IsActive;

		public string KeyValue;

		public DateTime LastUpdateTime;

		public string Name;
	}
}
