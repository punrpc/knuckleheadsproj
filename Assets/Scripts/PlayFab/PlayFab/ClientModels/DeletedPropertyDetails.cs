using System;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class DeletedPropertyDetails : PlayFabBaseModel
	{
		public string Name;

		public bool WasDeleted;
	}
}
