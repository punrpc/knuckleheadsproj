using System;
using System.Collections.Generic;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class GetPlayFabIDsFromOpenIdsRequest : PlayFabRequestCommon
	{
		public List<OpenIdSubjectIdentifier> OpenIdSubjectIdentifiers;
	}
}
