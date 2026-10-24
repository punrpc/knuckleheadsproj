using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.CloudScriptModels
{
	[Serializable]
	public class RegisterEventHubFunctionRequest : PlayFabRequestCommon
	{
		public string ConnectionString;

		public Dictionary<string, string> CustomTags;

		public string EventHubName;

		public string FunctionName;
	}
}
