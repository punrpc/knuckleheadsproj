using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class GetFriendLeaderboardForEntityRequest : PlayFabRequestCommon
	{
		public Dictionary<string, string> CustomTags;

		public EntityKey Entity;

		public ExternalFriendSources? ExternalFriendSources;

		public string LeaderboardName;

		public uint? Version;

		public string XboxToken;
	}
}
