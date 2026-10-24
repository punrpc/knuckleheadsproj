using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class DeleteTelemetryKeyResponse : PlayFabResultCommon
	{
		public bool WasKeyDeleted;
	}
}
