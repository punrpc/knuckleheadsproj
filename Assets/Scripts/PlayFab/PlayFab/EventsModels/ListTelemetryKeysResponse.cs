using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class ListTelemetryKeysResponse : PlayFabResultCommon
	{
		public List<TelemetryKeyDetails> KeyDetails;
	}
}
