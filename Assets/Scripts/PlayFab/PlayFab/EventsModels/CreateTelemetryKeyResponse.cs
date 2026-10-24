using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class CreateTelemetryKeyResponse : PlayFabResultCommon
	{
		public TelemetryKeyDetails NewKeyDetails;
	}
}
