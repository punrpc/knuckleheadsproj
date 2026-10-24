using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class GetPlayFabIDsFromOpenIdsResult : PlayFabResultCommon
	{
		public List<OpenIdSubjectIdentifierPlayFabIdPair> Data;
	}
}
