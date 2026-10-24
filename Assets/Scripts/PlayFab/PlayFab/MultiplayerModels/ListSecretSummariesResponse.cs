using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class ListSecretSummariesResponse : PlayFabResultCommon
	{
		public int PageSize;

		public List<SecretSummary> SecretSummaries;

		public string SkipToken;
	}
}
