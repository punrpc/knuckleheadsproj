using System;
using PlayFab.SharedModels;

namespace PlayFab.ClientModels
{
	[Serializable]
	public class UpdateProperty : PlayFabBaseModel
	{
		public string Name;

		public object Value;
	}
}
