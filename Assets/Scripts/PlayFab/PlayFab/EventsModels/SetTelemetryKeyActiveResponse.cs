using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class SetTelemetryKeyActiveResponse : PlayFabResultCommon
	{
		public TelemetryKeyDetails KeyDetails;

		public bool WasKeyUpdated;
	}
}
