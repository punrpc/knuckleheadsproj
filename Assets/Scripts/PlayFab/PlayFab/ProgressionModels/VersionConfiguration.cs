using System;
using PlayFab.SharedModels;

namespace PlayFab.ProgressionModels
{
	[Serializable]
	public class VersionConfiguration : PlayFabBaseModel
	{
		public int MaxQueryableVersions;

		public ResetInterval ResetInterval;
	}
}
