using System;
using PlayFab.SharedModels;

namespace PlayFab.CloudScriptModels
{
	[Serializable]
	public class EventHubFunctionModel : PlayFabBaseModel
	{
		public string ConnectionString;

		public string EventHubName;

		public string FunctionName;
	}
}
