using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.CloudScriptModels
{
	[Serializable]
	public class ListEventHubFunctionsResult : PlayFabResultCommon
	{
		public List<EventHubFunctionModel> Functions;
	}
}
