using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.MultiplayerModels
{
	[Serializable]
	public class ListContainerImageTagsResponse : PlayFabResultCommon
	{
		public int PageSize;

		public string SkipToken;

		public List<string> Tags;
	}
}
