namespace PlayFab.MultiplayerModels
{
	public enum DirectPeerConnectivityOptions
	{
		None = 0,
		SamePlatformType = 1,
		DifferentPlatformType = 2,
		AnyPlatformType = 3,
		SameEntityLoginProvider = 4,
		DifferentEntityLoginProvider = 5,
		AnyEntityLoginProvider = 6,
		AnyPlatformTypeAndEntityLoginProvider = 7,
		OnlyServers = 8
	}
}
