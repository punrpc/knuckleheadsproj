using System;
using PlayFab.SharedModels;

namespace PlayFab.EventsModels
{
	[Serializable]
	public class DeleteDataConnectionResponse : PlayFabResultCommon
	{
		public bool WasDeleted;
	}
}
