using System;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class OpenIdSubjectIdentifierPlayFabIdPair : PlayFabBaseModel
	{
		public OpenIdSubjectIdentifier OpenIdSubjectIdentifier;

		public string PlayFabId;
	}
}
