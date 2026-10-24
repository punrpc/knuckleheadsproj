using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class GetTelemetryKeyResponse : PlayFabResultCommon
	{
		public TelemetryKeyDetails KeyDetails;
	}
}
