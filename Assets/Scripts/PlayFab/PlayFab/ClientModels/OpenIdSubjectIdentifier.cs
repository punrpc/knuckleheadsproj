using System;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class OpenIdSubjectIdentifier : PlayFabBaseModel
	{
		public string Issuer;

		public string Subject;
	}
}
