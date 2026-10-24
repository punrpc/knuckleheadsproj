using System;
using PlayFab.SharedModels;

namespace PlayFab.AddonModels
{
	[Serializable]
	public class GetFacebookResponse : PlayFabResultCommon
	{
		public string AppID;

		public bool Created;

		public string NotificationEmail;
	}
}
