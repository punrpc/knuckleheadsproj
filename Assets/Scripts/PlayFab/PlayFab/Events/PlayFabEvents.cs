using System.Runtime.CompilerServices;
using PlayFab.AddonModels;
using PlayFab.AuthenticationModels;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.DataModels;
using PlayFab.EconomyModels;
using PlayFab.EventsModels;
using PlayFab.ExperimentationModels;
using PlayFab.GroupsModels;
using PlayFab.InsightsModels;
using PlayFab.Internal;
using PlayFab.LocalizationModels;
using PlayFab.MultiplayerModels;
using PlayFab.ProfilesModels;
using PlayFab.ProgressionModels;
using PlayFab.SharedModels;

namespace PlayFab.Events
{
	public class PlayFabEvents
	{
		public delegate void PlayFabErrorEvent(PlayFabRequestCommon request, PlayFabError error);

		public delegate void PlayFabResultEvent<in TResult>(TResult result) where TResult : PlayFabResultCommon;

		public delegate void PlayFabRequestEvent<in TRequest>(TRequest request) where TRequest : PlayFabRequestCommon;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateOrUpdateAppleRequest> m_OnAddonCreateOrUpdateAppleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateOrUpdateAppleResponse> m_OnAddonCreateOrUpdateAppleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateOrUpdateFacebookRequest> m_OnAddonCreateOrUpdateFacebookRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateOrUpdateFacebookResponse> m_OnAddonCreateOrUpdateFacebookResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateOrUpdateFacebookInstantGamesRequest> m_OnAddonCreateOrUpdateFacebookInstantGamesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateOrUpdateFacebookInstantGamesResponse> m_OnAddonCreateOrUpdateFacebookInstantGamesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateOrUpdateGoogleRequest> m_OnAddonCreateOrUpdateGoogleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateOrUpdateGoogleResponse> m_OnAddonCreateOrUpdateGoogleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateOrUpdateKongregateRequest> m_OnAddonCreateOrUpdateKongregateRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateOrUpdateKongregateResponse> m_OnAddonCreateOrUpdateKongregateResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateOrUpdateNintendoRequest> m_OnAddonCreateOrUpdateNintendoRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateOrUpdateNintendoResponse> m_OnAddonCreateOrUpdateNintendoResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateOrUpdatePSNRequest> m_OnAddonCreateOrUpdatePSNRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateOrUpdatePSNResponse> m_OnAddonCreateOrUpdatePSNResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateOrUpdateSteamRequest> m_OnAddonCreateOrUpdateSteamRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateOrUpdateSteamResponse> m_OnAddonCreateOrUpdateSteamResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateOrUpdateToxModRequest> m_OnAddonCreateOrUpdateToxModRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateOrUpdateToxModResponse> m_OnAddonCreateOrUpdateToxModResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateOrUpdateTwitchRequest> m_OnAddonCreateOrUpdateTwitchRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateOrUpdateTwitchResponse> m_OnAddonCreateOrUpdateTwitchResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteAppleRequest> m_OnAddonDeleteAppleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteAppleResponse> m_OnAddonDeleteAppleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteFacebookRequest> m_OnAddonDeleteFacebookRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteFacebookResponse> m_OnAddonDeleteFacebookResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteFacebookInstantGamesRequest> m_OnAddonDeleteFacebookInstantGamesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteFacebookInstantGamesResponse> m_OnAddonDeleteFacebookInstantGamesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteGoogleRequest> m_OnAddonDeleteGoogleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteGoogleResponse> m_OnAddonDeleteGoogleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteKongregateRequest> m_OnAddonDeleteKongregateRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteKongregateResponse> m_OnAddonDeleteKongregateResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteNintendoRequest> m_OnAddonDeleteNintendoRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteNintendoResponse> m_OnAddonDeleteNintendoResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeletePSNRequest> m_OnAddonDeletePSNRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeletePSNResponse> m_OnAddonDeletePSNResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteSteamRequest> m_OnAddonDeleteSteamRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteSteamResponse> m_OnAddonDeleteSteamResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteToxModRequest> m_OnAddonDeleteToxModRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteToxModResponse> m_OnAddonDeleteToxModResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteTwitchRequest> m_OnAddonDeleteTwitchRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteTwitchResponse> m_OnAddonDeleteTwitchResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetAppleRequest> m_OnAddonGetAppleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetAppleResponse> m_OnAddonGetAppleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetFacebookRequest> m_OnAddonGetFacebookRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetFacebookResponse> m_OnAddonGetFacebookResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetFacebookInstantGamesRequest> m_OnAddonGetFacebookInstantGamesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetFacebookInstantGamesResponse> m_OnAddonGetFacebookInstantGamesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetGoogleRequest> m_OnAddonGetGoogleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetGoogleResponse> m_OnAddonGetGoogleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetKongregateRequest> m_OnAddonGetKongregateRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetKongregateResponse> m_OnAddonGetKongregateResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetNintendoRequest> m_OnAddonGetNintendoRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetNintendoResponse> m_OnAddonGetNintendoResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPSNRequest> m_OnAddonGetPSNRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPSNResponse> m_OnAddonGetPSNResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetSteamRequest> m_OnAddonGetSteamRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetSteamResponse> m_OnAddonGetSteamResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetToxModRequest> m_OnAddonGetToxModRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetToxModResponse> m_OnAddonGetToxModResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTwitchRequest> m_OnAddonGetTwitchRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTwitchResponse> m_OnAddonGetTwitchResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AuthenticateCustomIdRequest> m_OnAuthenticationAuthenticateGameServerWithCustomIdRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<AuthenticateCustomIdResult> m_OnAuthenticationAuthenticateGameServerWithCustomIdResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteRequest> m_OnAuthenticationDeleteRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.AuthenticationModels.EmptyResponse> m_OnAuthenticationDeleteResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetEntityTokenRequest> m_OnAuthenticationGetEntityTokenRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetEntityTokenResponse> m_OnAuthenticationGetEntityTokenResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ValidateEntityTokenRequest> m_OnAuthenticationValidateEntityTokenRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ValidateEntityTokenResponse> m_OnAuthenticationValidateEntityTokenResultEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LoginResult> m_OnLoginResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AcceptTradeRequest> m_OnAcceptTradeRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<AcceptTradeResponse> m_OnAcceptTradeResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AddFriendRequest> m_OnAddFriendRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<AddFriendResult> m_OnAddFriendResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AddGenericIDRequest> m_OnAddGenericIDRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<AddGenericIDResult> m_OnAddGenericIDResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AddOrUpdateContactEmailRequest> m_OnAddOrUpdateContactEmailRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<AddOrUpdateContactEmailResult> m_OnAddOrUpdateContactEmailResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AddSharedGroupMembersRequest> m_OnAddSharedGroupMembersRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<AddSharedGroupMembersResult> m_OnAddSharedGroupMembersResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AddUsernamePasswordRequest> m_OnAddUsernamePasswordRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<AddUsernamePasswordResult> m_OnAddUsernamePasswordResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AddUserVirtualCurrencyRequest> m_OnAddUserVirtualCurrencyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ModifyUserVirtualCurrencyResult> m_OnAddUserVirtualCurrencyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AndroidDevicePushNotificationRegistrationRequest> m_OnAndroidDevicePushNotificationRegistrationRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<AndroidDevicePushNotificationRegistrationResult> m_OnAndroidDevicePushNotificationRegistrationResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AttributeInstallRequest> m_OnAttributeInstallRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<AttributeInstallResult> m_OnAttributeInstallResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CancelTradeRequest> m_OnCancelTradeRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CancelTradeResponse> m_OnCancelTradeResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ConfirmPurchaseRequest> m_OnConfirmPurchaseRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ConfirmPurchaseResult> m_OnConfirmPurchaseResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ConsumeItemRequest> m_OnConsumeItemRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ConsumeItemResult> m_OnConsumeItemResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ConsumeMicrosoftStoreEntitlementsRequest> m_OnConsumeMicrosoftStoreEntitlementsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ConsumeMicrosoftStoreEntitlementsResponse> m_OnConsumeMicrosoftStoreEntitlementsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ConsumePS5EntitlementsRequest> m_OnConsumePS5EntitlementsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ConsumePS5EntitlementsResult> m_OnConsumePS5EntitlementsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ConsumePSNEntitlementsRequest> m_OnConsumePSNEntitlementsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ConsumePSNEntitlementsResult> m_OnConsumePSNEntitlementsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ConsumeXboxEntitlementsRequest> m_OnConsumeXboxEntitlementsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ConsumeXboxEntitlementsResult> m_OnConsumeXboxEntitlementsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateSharedGroupRequest> m_OnCreateSharedGroupRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateSharedGroupResult> m_OnCreateSharedGroupResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeletePlayerCustomPropertiesRequest> m_OnDeletePlayerCustomPropertiesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeletePlayerCustomPropertiesResult> m_OnDeletePlayerCustomPropertiesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ExecuteCloudScriptRequest> m_OnExecuteCloudScriptRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ClientModels.ExecuteCloudScriptResult> m_OnExecuteCloudScriptResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetAccountInfoRequest> m_OnGetAccountInfoRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetAccountInfoResult> m_OnGetAccountInfoResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetAdPlacementsRequest> m_OnGetAdPlacementsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetAdPlacementsResult> m_OnGetAdPlacementsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListUsersCharactersRequest> m_OnGetAllUsersCharactersRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListUsersCharactersResult> m_OnGetAllUsersCharactersResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetCatalogItemsRequest> m_OnGetCatalogItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetCatalogItemsResult> m_OnGetCatalogItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetCharacterDataRequest> m_OnGetCharacterDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetCharacterDataResult> m_OnGetCharacterDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetCharacterInventoryRequest> m_OnGetCharacterInventoryRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetCharacterInventoryResult> m_OnGetCharacterInventoryResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetCharacterLeaderboardRequest> m_OnGetCharacterLeaderboardRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetCharacterLeaderboardResult> m_OnGetCharacterLeaderboardResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetCharacterDataRequest> m_OnGetCharacterReadOnlyDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetCharacterDataResult> m_OnGetCharacterReadOnlyDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetCharacterStatisticsRequest> m_OnGetCharacterStatisticsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetCharacterStatisticsResult> m_OnGetCharacterStatisticsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetContentDownloadUrlRequest> m_OnGetContentDownloadUrlRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetContentDownloadUrlResult> m_OnGetContentDownloadUrlResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetFriendLeaderboardRequest> m_OnGetFriendLeaderboardRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetLeaderboardResult> m_OnGetFriendLeaderboardResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetFriendLeaderboardAroundPlayerRequest> m_OnGetFriendLeaderboardAroundPlayerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetFriendLeaderboardAroundPlayerResult> m_OnGetFriendLeaderboardAroundPlayerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetFriendsListRequest> m_OnGetFriendsListRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetFriendsListResult> m_OnGetFriendsListResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetLeaderboardRequest> m_OnGetLeaderboardRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetLeaderboardResult> m_OnGetLeaderboardResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetLeaderboardAroundCharacterRequest> m_OnGetLeaderboardAroundCharacterRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetLeaderboardAroundCharacterResult> m_OnGetLeaderboardAroundCharacterResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetLeaderboardAroundPlayerRequest> m_OnGetLeaderboardAroundPlayerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetLeaderboardAroundPlayerResult> m_OnGetLeaderboardAroundPlayerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetLeaderboardForUsersCharactersRequest> m_OnGetLeaderboardForUserCharactersRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetLeaderboardForUsersCharactersResult> m_OnGetLeaderboardForUserCharactersResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPaymentTokenRequest> m_OnGetPaymentTokenRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPaymentTokenResult> m_OnGetPaymentTokenResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPhotonAuthenticationTokenRequest> m_OnGetPhotonAuthenticationTokenRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPhotonAuthenticationTokenResult> m_OnGetPhotonAuthenticationTokenResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayerCombinedInfoRequest> m_OnGetPlayerCombinedInfoRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayerCombinedInfoResult> m_OnGetPlayerCombinedInfoResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayerCustomPropertyRequest> m_OnGetPlayerCustomPropertyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayerCustomPropertyResult> m_OnGetPlayerCustomPropertyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayerProfileRequest> m_OnGetPlayerProfileRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayerProfileResult> m_OnGetPlayerProfileResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayerSegmentsRequest> m_OnGetPlayerSegmentsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayerSegmentsResult> m_OnGetPlayerSegmentsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayerStatisticsRequest> m_OnGetPlayerStatisticsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayerStatisticsResult> m_OnGetPlayerStatisticsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayerStatisticVersionsRequest> m_OnGetPlayerStatisticVersionsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayerStatisticVersionsResult> m_OnGetPlayerStatisticVersionsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayerTagsRequest> m_OnGetPlayerTagsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayerTagsResult> m_OnGetPlayerTagsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayerTradesRequest> m_OnGetPlayerTradesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayerTradesResponse> m_OnGetPlayerTradesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromBattleNetAccountIdsRequest> m_OnGetPlayFabIDsFromBattleNetAccountIdsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromBattleNetAccountIdsResult> m_OnGetPlayFabIDsFromBattleNetAccountIdsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromFacebookIDsRequest> m_OnGetPlayFabIDsFromFacebookIDsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromFacebookIDsResult> m_OnGetPlayFabIDsFromFacebookIDsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromFacebookInstantGamesIdsRequest> m_OnGetPlayFabIDsFromFacebookInstantGamesIdsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromFacebookInstantGamesIdsResult> m_OnGetPlayFabIDsFromFacebookInstantGamesIdsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromGameCenterIDsRequest> m_OnGetPlayFabIDsFromGameCenterIDsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromGameCenterIDsResult> m_OnGetPlayFabIDsFromGameCenterIDsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromGenericIDsRequest> m_OnGetPlayFabIDsFromGenericIDsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromGenericIDsResult> m_OnGetPlayFabIDsFromGenericIDsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromGoogleIDsRequest> m_OnGetPlayFabIDsFromGoogleIDsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromGoogleIDsResult> m_OnGetPlayFabIDsFromGoogleIDsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromGooglePlayGamesPlayerIDsRequest> m_OnGetPlayFabIDsFromGooglePlayGamesPlayerIDsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromGooglePlayGamesPlayerIDsResult> m_OnGetPlayFabIDsFromGooglePlayGamesPlayerIDsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromKongregateIDsRequest> m_OnGetPlayFabIDsFromKongregateIDsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromKongregateIDsResult> m_OnGetPlayFabIDsFromKongregateIDsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromNintendoServiceAccountIdsRequest> m_OnGetPlayFabIDsFromNintendoServiceAccountIdsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromNintendoServiceAccountIdsResult> m_OnGetPlayFabIDsFromNintendoServiceAccountIdsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromNintendoSwitchDeviceIdsRequest> m_OnGetPlayFabIDsFromNintendoSwitchDeviceIdsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromNintendoSwitchDeviceIdsResult> m_OnGetPlayFabIDsFromNintendoSwitchDeviceIdsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromOpenIdsRequest> m_OnGetPlayFabIDsFromOpenIdSubjectIdentifiersRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromOpenIdsResult> m_OnGetPlayFabIDsFromOpenIdSubjectIdentifiersResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromPSNAccountIDsRequest> m_OnGetPlayFabIDsFromPSNAccountIDsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromPSNAccountIDsResult> m_OnGetPlayFabIDsFromPSNAccountIDsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromPSNOnlineIDsRequest> m_OnGetPlayFabIDsFromPSNOnlineIDsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromPSNOnlineIDsResult> m_OnGetPlayFabIDsFromPSNOnlineIDsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromSteamIDsRequest> m_OnGetPlayFabIDsFromSteamIDsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromSteamIDsResult> m_OnGetPlayFabIDsFromSteamIDsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromSteamNamesRequest> m_OnGetPlayFabIDsFromSteamNamesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromSteamNamesResult> m_OnGetPlayFabIDsFromSteamNamesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromTwitchIDsRequest> m_OnGetPlayFabIDsFromTwitchIDsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromTwitchIDsResult> m_OnGetPlayFabIDsFromTwitchIDsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPlayFabIDsFromXboxLiveIDsRequest> m_OnGetPlayFabIDsFromXboxLiveIDsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPlayFabIDsFromXboxLiveIDsResult> m_OnGetPlayFabIDsFromXboxLiveIDsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPublisherDataRequest> m_OnGetPublisherDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPublisherDataResult> m_OnGetPublisherDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetPurchaseRequest> m_OnGetPurchaseRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetPurchaseResult> m_OnGetPurchaseResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetSharedGroupDataRequest> m_OnGetSharedGroupDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetSharedGroupDataResult> m_OnGetSharedGroupDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetStoreItemsRequest> m_OnGetStoreItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetStoreItemsResult> m_OnGetStoreItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTimeRequest> m_OnGetTimeRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTimeResult> m_OnGetTimeResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTitleDataRequest> m_OnGetTitleDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTitleDataResult> m_OnGetTitleDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTitleNewsRequest> m_OnGetTitleNewsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTitleNewsResult> m_OnGetTitleNewsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTitlePublicKeyRequest> m_OnGetTitlePublicKeyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTitlePublicKeyResult> m_OnGetTitlePublicKeyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTradeStatusRequest> m_OnGetTradeStatusRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTradeStatusResponse> m_OnGetTradeStatusResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetUserDataRequest> m_OnGetUserDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetUserDataResult> m_OnGetUserDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetUserInventoryRequest> m_OnGetUserInventoryRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetUserInventoryResult> m_OnGetUserInventoryResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetUserDataRequest> m_OnGetUserPublisherDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetUserDataResult> m_OnGetUserPublisherDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetUserDataRequest> m_OnGetUserPublisherReadOnlyDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetUserDataResult> m_OnGetUserPublisherReadOnlyDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetUserDataRequest> m_OnGetUserReadOnlyDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetUserDataResult> m_OnGetUserReadOnlyDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GrantCharacterToUserRequest> m_OnGrantCharacterToUserRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GrantCharacterToUserResult> m_OnGrantCharacterToUserResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkAndroidDeviceIDRequest> m_OnLinkAndroidDeviceIDRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkAndroidDeviceIDResult> m_OnLinkAndroidDeviceIDResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkAppleRequest> m_OnLinkAppleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ClientModels.EmptyResult> m_OnLinkAppleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkBattleNetAccountRequest> m_OnLinkBattleNetAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> m_OnLinkBattleNetAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkCustomIDRequest> m_OnLinkCustomIDRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkCustomIDResult> m_OnLinkCustomIDResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkFacebookAccountRequest> m_OnLinkFacebookAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkFacebookAccountResult> m_OnLinkFacebookAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkFacebookInstantGamesIdRequest> m_OnLinkFacebookInstantGamesIdRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkFacebookInstantGamesIdResult> m_OnLinkFacebookInstantGamesIdResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkGameCenterAccountRequest> m_OnLinkGameCenterAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkGameCenterAccountResult> m_OnLinkGameCenterAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkGoogleAccountRequest> m_OnLinkGoogleAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkGoogleAccountResult> m_OnLinkGoogleAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkGooglePlayGamesServicesAccountRequest> m_OnLinkGooglePlayGamesServicesAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkGooglePlayGamesServicesAccountResult> m_OnLinkGooglePlayGamesServicesAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkIOSDeviceIDRequest> m_OnLinkIOSDeviceIDRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkIOSDeviceIDResult> m_OnLinkIOSDeviceIDResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkKongregateAccountRequest> m_OnLinkKongregateRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkKongregateAccountResult> m_OnLinkKongregateResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkNintendoServiceAccountRequest> m_OnLinkNintendoServiceAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ClientModels.EmptyResult> m_OnLinkNintendoServiceAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkNintendoSwitchDeviceIdRequest> m_OnLinkNintendoSwitchDeviceIdRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkNintendoSwitchDeviceIdResult> m_OnLinkNintendoSwitchDeviceIdResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkOpenIdConnectRequest> m_OnLinkOpenIdConnectRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ClientModels.EmptyResult> m_OnLinkOpenIdConnectResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkPSNAccountRequest> m_OnLinkPSNAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkPSNAccountResult> m_OnLinkPSNAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkSteamAccountRequest> m_OnLinkSteamAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkSteamAccountResult> m_OnLinkSteamAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkTwitchAccountRequest> m_OnLinkTwitchRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkTwitchAccountResult> m_OnLinkTwitchResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LinkXboxAccountRequest> m_OnLinkXboxAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LinkXboxAccountResult> m_OnLinkXboxAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListPlayerCustomPropertiesRequest> m_OnListPlayerCustomPropertiesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListPlayerCustomPropertiesResult> m_OnListPlayerCustomPropertiesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithAndroidDeviceIDRequest> m_OnLoginWithAndroidDeviceIDRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithAppleRequest> m_OnLoginWithAppleRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithBattleNetRequest> m_OnLoginWithBattleNetRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithCustomIDRequest> m_OnLoginWithCustomIDRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithEmailAddressRequest> m_OnLoginWithEmailAddressRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithFacebookRequest> m_OnLoginWithFacebookRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithFacebookInstantGamesIdRequest> m_OnLoginWithFacebookInstantGamesIdRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithGameCenterRequest> m_OnLoginWithGameCenterRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithGoogleAccountRequest> m_OnLoginWithGoogleAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithGooglePlayGamesServicesRequest> m_OnLoginWithGooglePlayGamesServicesRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithIOSDeviceIDRequest> m_OnLoginWithIOSDeviceIDRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithKongregateRequest> m_OnLoginWithKongregateRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithNintendoServiceAccountRequest> m_OnLoginWithNintendoServiceAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithNintendoSwitchDeviceIdRequest> m_OnLoginWithNintendoSwitchDeviceIdRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithOpenIdConnectRequest> m_OnLoginWithOpenIdConnectRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithPlayFabRequest> m_OnLoginWithPlayFabRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithPSNRequest> m_OnLoginWithPSNRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithSteamRequest> m_OnLoginWithSteamRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithTwitchRequest> m_OnLoginWithTwitchRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LoginWithXboxRequest> m_OnLoginWithXboxRequestEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<OpenTradeRequest> m_OnOpenTradeRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<OpenTradeResponse> m_OnOpenTradeResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<PayForPurchaseRequest> m_OnPayForPurchaseRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PayForPurchaseResult> m_OnPayForPurchaseResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<PurchaseItemRequest> m_OnPurchaseItemRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PurchaseItemResult> m_OnPurchaseItemResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RedeemCouponRequest> m_OnRedeemCouponRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RedeemCouponResult> m_OnRedeemCouponResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RefreshPSNAuthTokenRequest> m_OnRefreshPSNAuthTokenRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> m_OnRefreshPSNAuthTokenResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RegisterForIOSPushNotificationRequest> m_OnRegisterForIOSPushNotificationRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RegisterForIOSPushNotificationResult> m_OnRegisterForIOSPushNotificationResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RegisterPlayFabUserRequest> m_OnRegisterPlayFabUserRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RegisterPlayFabUserResult> m_OnRegisterPlayFabUserResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RemoveContactEmailRequest> m_OnRemoveContactEmailRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RemoveContactEmailResult> m_OnRemoveContactEmailResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RemoveFriendRequest> m_OnRemoveFriendRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RemoveFriendResult> m_OnRemoveFriendResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RemoveGenericIDRequest> m_OnRemoveGenericIDRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RemoveGenericIDResult> m_OnRemoveGenericIDResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RemoveSharedGroupMembersRequest> m_OnRemoveSharedGroupMembersRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RemoveSharedGroupMembersResult> m_OnRemoveSharedGroupMembersResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ReportAdActivityRequest> m_OnReportAdActivityRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ReportAdActivityResult> m_OnReportAdActivityResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeviceInfoRequest> m_OnReportDeviceInfoRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> m_OnReportDeviceInfoResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ReportPlayerClientRequest> m_OnReportPlayerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ReportPlayerClientResult> m_OnReportPlayerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RestoreIOSPurchasesRequest> m_OnRestoreIOSPurchasesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RestoreIOSPurchasesResult> m_OnRestoreIOSPurchasesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RewardAdActivityRequest> m_OnRewardAdActivityRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RewardAdActivityResult> m_OnRewardAdActivityResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SendAccountRecoveryEmailRequest> m_OnSendAccountRecoveryEmailRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SendAccountRecoveryEmailResult> m_OnSendAccountRecoveryEmailResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SetFriendTagsRequest> m_OnSetFriendTagsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SetFriendTagsResult> m_OnSetFriendTagsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SetPlayerSecretRequest> m_OnSetPlayerSecretRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SetPlayerSecretResult> m_OnSetPlayerSecretResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<StartPurchaseRequest> m_OnStartPurchaseRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<StartPurchaseResult> m_OnStartPurchaseResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SubtractUserVirtualCurrencyRequest> m_OnSubtractUserVirtualCurrencyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ModifyUserVirtualCurrencyResult> m_OnSubtractUserVirtualCurrencyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkAndroidDeviceIDRequest> m_OnUnlinkAndroidDeviceIDRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkAndroidDeviceIDResult> m_OnUnlinkAndroidDeviceIDResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkAppleRequest> m_OnUnlinkAppleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> m_OnUnlinkAppleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkBattleNetAccountRequest> m_OnUnlinkBattleNetAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> m_OnUnlinkBattleNetAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkCustomIDRequest> m_OnUnlinkCustomIDRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkCustomIDResult> m_OnUnlinkCustomIDResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkFacebookAccountRequest> m_OnUnlinkFacebookAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkFacebookAccountResult> m_OnUnlinkFacebookAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkFacebookInstantGamesIdRequest> m_OnUnlinkFacebookInstantGamesIdRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkFacebookInstantGamesIdResult> m_OnUnlinkFacebookInstantGamesIdResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkGameCenterAccountRequest> m_OnUnlinkGameCenterAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkGameCenterAccountResult> m_OnUnlinkGameCenterAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkGoogleAccountRequest> m_OnUnlinkGoogleAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkGoogleAccountResult> m_OnUnlinkGoogleAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkGooglePlayGamesServicesAccountRequest> m_OnUnlinkGooglePlayGamesServicesAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkGooglePlayGamesServicesAccountResult> m_OnUnlinkGooglePlayGamesServicesAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkIOSDeviceIDRequest> m_OnUnlinkIOSDeviceIDRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkIOSDeviceIDResult> m_OnUnlinkIOSDeviceIDResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkKongregateAccountRequest> m_OnUnlinkKongregateRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkKongregateAccountResult> m_OnUnlinkKongregateResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkNintendoServiceAccountRequest> m_OnUnlinkNintendoServiceAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> m_OnUnlinkNintendoServiceAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkNintendoSwitchDeviceIdRequest> m_OnUnlinkNintendoSwitchDeviceIdRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkNintendoSwitchDeviceIdResult> m_OnUnlinkNintendoSwitchDeviceIdResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkOpenIdConnectRequest> m_OnUnlinkOpenIdConnectRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> m_OnUnlinkOpenIdConnectResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkPSNAccountRequest> m_OnUnlinkPSNAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkPSNAccountResult> m_OnUnlinkPSNAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkSteamAccountRequest> m_OnUnlinkSteamAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkSteamAccountResult> m_OnUnlinkSteamAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkTwitchAccountRequest> m_OnUnlinkTwitchRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkTwitchAccountResult> m_OnUnlinkTwitchResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkXboxAccountRequest> m_OnUnlinkXboxAccountRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlinkXboxAccountResult> m_OnUnlinkXboxAccountResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlockContainerInstanceRequest> m_OnUnlockContainerInstanceRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlockContainerItemResult> m_OnUnlockContainerInstanceResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlockContainerItemRequest> m_OnUnlockContainerItemRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnlockContainerItemResult> m_OnUnlockContainerItemResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateAvatarUrlRequest> m_OnUpdateAvatarUrlRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> m_OnUpdateAvatarUrlResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateCharacterDataRequest> m_OnUpdateCharacterDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdateCharacterDataResult> m_OnUpdateCharacterDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateCharacterStatisticsRequest> m_OnUpdateCharacterStatisticsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdateCharacterStatisticsResult> m_OnUpdateCharacterStatisticsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdatePlayerCustomPropertiesRequest> m_OnUpdatePlayerCustomPropertiesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdatePlayerCustomPropertiesResult> m_OnUpdatePlayerCustomPropertiesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdatePlayerStatisticsRequest> m_OnUpdatePlayerStatisticsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdatePlayerStatisticsResult> m_OnUpdatePlayerStatisticsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateSharedGroupDataRequest> m_OnUpdateSharedGroupDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdateSharedGroupDataResult> m_OnUpdateSharedGroupDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateUserDataRequest> m_OnUpdateUserDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdateUserDataResult> m_OnUpdateUserDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateUserDataRequest> m_OnUpdateUserPublisherDataRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdateUserDataResult> m_OnUpdateUserPublisherDataResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateUserTitleDisplayNameRequest> m_OnUpdateUserTitleDisplayNameRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdateUserTitleDisplayNameResult> m_OnUpdateUserTitleDisplayNameResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ValidateAmazonReceiptRequest> m_OnValidateAmazonIAPReceiptRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ValidateAmazonReceiptResult> m_OnValidateAmazonIAPReceiptResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ValidateGooglePlayPurchaseRequest> m_OnValidateGooglePlayPurchaseRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ValidateGooglePlayPurchaseResult> m_OnValidateGooglePlayPurchaseResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ValidateIOSReceiptRequest> m_OnValidateIOSReceiptRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ValidateIOSReceiptResult> m_OnValidateIOSReceiptResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ValidateWindowsReceiptRequest> m_OnValidateWindowsStoreReceiptRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ValidateWindowsReceiptResult> m_OnValidateWindowsStoreReceiptResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<WriteClientCharacterEventRequest> m_OnWriteCharacterEventRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<WriteEventResponse> m_OnWriteCharacterEventResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<WriteClientPlayerEventRequest> m_OnWritePlayerEventRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<WriteEventResponse> m_OnWritePlayerEventResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<WriteTitleEventRequest> m_OnWriteTitleEventRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<WriteEventResponse> m_OnWriteTitleEventResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ExecuteEntityCloudScriptRequest> m_OnCloudScriptExecuteEntityCloudScriptRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.CloudScriptModels.ExecuteCloudScriptResult> m_OnCloudScriptExecuteEntityCloudScriptResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ExecuteFunctionRequest> m_OnCloudScriptExecuteFunctionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ExecuteFunctionResult> m_OnCloudScriptExecuteFunctionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetFunctionRequest> m_OnCloudScriptGetFunctionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetFunctionResult> m_OnCloudScriptGetFunctionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListFunctionsRequest> m_OnCloudScriptListEventHubFunctionsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListEventHubFunctionsResult> m_OnCloudScriptListEventHubFunctionsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListFunctionsRequest> m_OnCloudScriptListFunctionsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListFunctionsResult> m_OnCloudScriptListFunctionsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListFunctionsRequest> m_OnCloudScriptListHttpFunctionsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListHttpFunctionsResult> m_OnCloudScriptListHttpFunctionsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListFunctionsRequest> m_OnCloudScriptListQueuedFunctionsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListQueuedFunctionsResult> m_OnCloudScriptListQueuedFunctionsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<PostFunctionResultForEntityTriggeredActionRequest> m_OnCloudScriptPostFunctionResultForEntityTriggeredActionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> m_OnCloudScriptPostFunctionResultForEntityTriggeredActionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<PostFunctionResultForFunctionExecutionRequest> m_OnCloudScriptPostFunctionResultForFunctionExecutionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> m_OnCloudScriptPostFunctionResultForFunctionExecutionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<PostFunctionResultForPlayerTriggeredActionRequest> m_OnCloudScriptPostFunctionResultForPlayerTriggeredActionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> m_OnCloudScriptPostFunctionResultForPlayerTriggeredActionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<PostFunctionResultForScheduledTaskRequest> m_OnCloudScriptPostFunctionResultForScheduledTaskRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> m_OnCloudScriptPostFunctionResultForScheduledTaskResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RegisterEventHubFunctionRequest> m_OnCloudScriptRegisterEventHubFunctionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> m_OnCloudScriptRegisterEventHubFunctionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RegisterHttpFunctionRequest> m_OnCloudScriptRegisterHttpFunctionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> m_OnCloudScriptRegisterHttpFunctionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RegisterQueuedFunctionRequest> m_OnCloudScriptRegisterQueuedFunctionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> m_OnCloudScriptRegisterQueuedFunctionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnregisterFunctionRequest> m_OnCloudScriptUnregisterFunctionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> m_OnCloudScriptUnregisterFunctionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AbortFileUploadsRequest> m_OnDataAbortFileUploadsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<AbortFileUploadsResponse> m_OnDataAbortFileUploadsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteFilesRequest> m_OnDataDeleteFilesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteFilesResponse> m_OnDataDeleteFilesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<FinalizeFileUploadsRequest> m_OnDataFinalizeFileUploadsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<FinalizeFileUploadsResponse> m_OnDataFinalizeFileUploadsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetFilesRequest> m_OnDataGetFilesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetFilesResponse> m_OnDataGetFilesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetObjectsRequest> m_OnDataGetObjectsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetObjectsResponse> m_OnDataGetObjectsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<InitiateFileUploadsRequest> m_OnDataInitiateFileUploadsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<InitiateFileUploadsResponse> m_OnDataInitiateFileUploadsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SetObjectsRequest> m_OnDataSetObjectsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SetObjectsResponse> m_OnDataSetObjectsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AddInventoryItemsRequest> m_OnEconomyAddInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<AddInventoryItemsResponse> m_OnEconomyAddInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateDraftItemRequest> m_OnEconomyCreateDraftItemRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateDraftItemResponse> m_OnEconomyCreateDraftItemResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateUploadUrlsRequest> m_OnEconomyCreateUploadUrlsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateUploadUrlsResponse> m_OnEconomyCreateUploadUrlsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteEntityItemReviewsRequest> m_OnEconomyDeleteEntityItemReviewsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteEntityItemReviewsResponse> m_OnEconomyDeleteEntityItemReviewsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteInventoryCollectionRequest> m_OnEconomyDeleteInventoryCollectionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteInventoryCollectionResponse> m_OnEconomyDeleteInventoryCollectionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteInventoryItemsRequest> m_OnEconomyDeleteInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteInventoryItemsResponse> m_OnEconomyDeleteInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteItemRequest> m_OnEconomyDeleteItemRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteItemResponse> m_OnEconomyDeleteItemResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ExecuteInventoryOperationsRequest> m_OnEconomyExecuteInventoryOperationsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ExecuteInventoryOperationsResponse> m_OnEconomyExecuteInventoryOperationsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ExecuteTransferOperationsRequest> m_OnEconomyExecuteTransferOperationsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ExecuteTransferOperationsResponse> m_OnEconomyExecuteTransferOperationsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetCatalogConfigRequest> m_OnEconomyGetCatalogConfigRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetCatalogConfigResponse> m_OnEconomyGetCatalogConfigResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetDraftItemRequest> m_OnEconomyGetDraftItemRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetDraftItemResponse> m_OnEconomyGetDraftItemResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetDraftItemsRequest> m_OnEconomyGetDraftItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetDraftItemsResponse> m_OnEconomyGetDraftItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetEntityDraftItemsRequest> m_OnEconomyGetEntityDraftItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetEntityDraftItemsResponse> m_OnEconomyGetEntityDraftItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetEntityItemReviewRequest> m_OnEconomyGetEntityItemReviewRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetEntityItemReviewResponse> m_OnEconomyGetEntityItemReviewResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetInventoryCollectionIdsRequest> m_OnEconomyGetInventoryCollectionIdsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetInventoryCollectionIdsResponse> m_OnEconomyGetInventoryCollectionIdsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetInventoryItemsRequest> m_OnEconomyGetInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetInventoryItemsResponse> m_OnEconomyGetInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetInventoryOperationStatusRequest> m_OnEconomyGetInventoryOperationStatusRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetInventoryOperationStatusResponse> m_OnEconomyGetInventoryOperationStatusResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetItemRequest> m_OnEconomyGetItemRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetItemResponse> m_OnEconomyGetItemResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetItemContainersRequest> m_OnEconomyGetItemContainersRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetItemContainersResponse> m_OnEconomyGetItemContainersResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetItemModerationStateRequest> m_OnEconomyGetItemModerationStateRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetItemModerationStateResponse> m_OnEconomyGetItemModerationStateResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetItemPublishStatusRequest> m_OnEconomyGetItemPublishStatusRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetItemPublishStatusResponse> m_OnEconomyGetItemPublishStatusResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetItemReviewsRequest> m_OnEconomyGetItemReviewsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetItemReviewsResponse> m_OnEconomyGetItemReviewsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetItemReviewSummaryRequest> m_OnEconomyGetItemReviewSummaryRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetItemReviewSummaryResponse> m_OnEconomyGetItemReviewSummaryResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetItemsRequest> m_OnEconomyGetItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetItemsResponse> m_OnEconomyGetItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTransactionHistoryRequest> m_OnEconomyGetTransactionHistoryRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTransactionHistoryResponse> m_OnEconomyGetTransactionHistoryResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<PublishDraftItemRequest> m_OnEconomyPublishDraftItemRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PublishDraftItemResponse> m_OnEconomyPublishDraftItemResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<PurchaseInventoryItemsRequest> m_OnEconomyPurchaseInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PurchaseInventoryItemsResponse> m_OnEconomyPurchaseInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RedeemAppleAppStoreInventoryItemsRequest> m_OnEconomyRedeemAppleAppStoreInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RedeemAppleAppStoreInventoryItemsResponse> m_OnEconomyRedeemAppleAppStoreInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RedeemAppleAppStoreWithJwsInventoryItemsRequest> m_OnEconomyRedeemAppleAppStoreWithJwsInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RedeemAppleAppStoreWithJwsInventoryItemsResponse> m_OnEconomyRedeemAppleAppStoreWithJwsInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RedeemGooglePlayInventoryItemsRequest> m_OnEconomyRedeemGooglePlayInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RedeemGooglePlayInventoryItemsResponse> m_OnEconomyRedeemGooglePlayInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RedeemMicrosoftStoreInventoryItemsRequest> m_OnEconomyRedeemMicrosoftStoreInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RedeemMicrosoftStoreInventoryItemsResponse> m_OnEconomyRedeemMicrosoftStoreInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RedeemNintendoEShopInventoryItemsRequest> m_OnEconomyRedeemNintendoEShopInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RedeemNintendoEShopInventoryItemsResponse> m_OnEconomyRedeemNintendoEShopInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RedeemPlayStationStoreInventoryItemsRequest> m_OnEconomyRedeemPlayStationStoreInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RedeemPlayStationStoreInventoryItemsResponse> m_OnEconomyRedeemPlayStationStoreInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RedeemSteamInventoryItemsRequest> m_OnEconomyRedeemSteamInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RedeemSteamInventoryItemsResponse> m_OnEconomyRedeemSteamInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ReportItemRequest> m_OnEconomyReportItemRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ReportItemResponse> m_OnEconomyReportItemResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ReportItemReviewRequest> m_OnEconomyReportItemReviewRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ReportItemReviewResponse> m_OnEconomyReportItemReviewResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ReviewItemRequest> m_OnEconomyReviewItemRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ReviewItemResponse> m_OnEconomyReviewItemResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SearchItemsRequest> m_OnEconomySearchItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SearchItemsResponse> m_OnEconomySearchItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SetItemModerationStateRequest> m_OnEconomySetItemModerationStateRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SetItemModerationStateResponse> m_OnEconomySetItemModerationStateResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SubmitItemReviewVoteRequest> m_OnEconomySubmitItemReviewVoteRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SubmitItemReviewVoteResponse> m_OnEconomySubmitItemReviewVoteResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SubtractInventoryItemsRequest> m_OnEconomySubtractInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SubtractInventoryItemsResponse> m_OnEconomySubtractInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<TakedownItemReviewsRequest> m_OnEconomyTakedownItemReviewsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<TakedownItemReviewsResponse> m_OnEconomyTakedownItemReviewsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<TransferInventoryItemsRequest> m_OnEconomyTransferInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<TransferInventoryItemsResponse> m_OnEconomyTransferInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateCatalogConfigRequest> m_OnEconomyUpdateCatalogConfigRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdateCatalogConfigResponse> m_OnEconomyUpdateCatalogConfigResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateDraftItemRequest> m_OnEconomyUpdateDraftItemRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdateDraftItemResponse> m_OnEconomyUpdateDraftItemResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateInventoryItemsRequest> m_OnEconomyUpdateInventoryItemsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdateInventoryItemsResponse> m_OnEconomyUpdateInventoryItemsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateTelemetryKeyRequest> m_OnEventsCreateTelemetryKeyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateTelemetryKeyResponse> m_OnEventsCreateTelemetryKeyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteDataConnectionRequest> m_OnEventsDeleteDataConnectionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteDataConnectionResponse> m_OnEventsDeleteDataConnectionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteTelemetryKeyRequest> m_OnEventsDeleteTelemetryKeyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteTelemetryKeyResponse> m_OnEventsDeleteTelemetryKeyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetDataConnectionRequest> m_OnEventsGetDataConnectionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetDataConnectionResponse> m_OnEventsGetDataConnectionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTelemetryKeyRequest> m_OnEventsGetTelemetryKeyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTelemetryKeyResponse> m_OnEventsGetTelemetryKeyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListDataConnectionsRequest> m_OnEventsListDataConnectionsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListDataConnectionsResponse> m_OnEventsListDataConnectionsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListTelemetryKeysRequest> m_OnEventsListTelemetryKeysRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListTelemetryKeysResponse> m_OnEventsListTelemetryKeysResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SetDataConnectionRequest> m_OnEventsSetDataConnectionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SetDataConnectionResponse> m_OnEventsSetDataConnectionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SetDataConnectionActiveRequest> m_OnEventsSetDataConnectionActiveRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SetDataConnectionActiveResponse> m_OnEventsSetDataConnectionActiveResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SetTelemetryKeyActiveRequest> m_OnEventsSetTelemetryKeyActiveRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SetTelemetryKeyActiveResponse> m_OnEventsSetTelemetryKeyActiveResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<WriteEventsRequest> m_OnEventsWriteEventsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<WriteEventsResponse> m_OnEventsWriteEventsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<WriteEventsRequest> m_OnEventsWriteTelemetryEventsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<WriteEventsResponse> m_OnEventsWriteTelemetryEventsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateExclusionGroupRequest> m_OnExperimentationCreateExclusionGroupRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateExclusionGroupResult> m_OnExperimentationCreateExclusionGroupResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateExperimentRequest> m_OnExperimentationCreateExperimentRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateExperimentResult> m_OnExperimentationCreateExperimentResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteExclusionGroupRequest> m_OnExperimentationDeleteExclusionGroupRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ExperimentationModels.EmptyResponse> m_OnExperimentationDeleteExclusionGroupResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteExperimentRequest> m_OnExperimentationDeleteExperimentRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ExperimentationModels.EmptyResponse> m_OnExperimentationDeleteExperimentResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetExclusionGroupsRequest> m_OnExperimentationGetExclusionGroupsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetExclusionGroupsResult> m_OnExperimentationGetExclusionGroupsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetExclusionGroupTrafficRequest> m_OnExperimentationGetExclusionGroupTrafficRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetExclusionGroupTrafficResult> m_OnExperimentationGetExclusionGroupTrafficResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetExperimentsRequest> m_OnExperimentationGetExperimentsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetExperimentsResult> m_OnExperimentationGetExperimentsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetLatestScorecardRequest> m_OnExperimentationGetLatestScorecardRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetLatestScorecardResult> m_OnExperimentationGetLatestScorecardResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTreatmentAssignmentRequest> m_OnExperimentationGetTreatmentAssignmentRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTreatmentAssignmentResult> m_OnExperimentationGetTreatmentAssignmentResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<StartExperimentRequest> m_OnExperimentationStartExperimentRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ExperimentationModels.EmptyResponse> m_OnExperimentationStartExperimentResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<StopExperimentRequest> m_OnExperimentationStopExperimentRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ExperimentationModels.EmptyResponse> m_OnExperimentationStopExperimentResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateExclusionGroupRequest> m_OnExperimentationUpdateExclusionGroupRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ExperimentationModels.EmptyResponse> m_OnExperimentationUpdateExclusionGroupResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateExperimentRequest> m_OnExperimentationUpdateExperimentRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ExperimentationModels.EmptyResponse> m_OnExperimentationUpdateExperimentResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AcceptGroupApplicationRequest> m_OnGroupsAcceptGroupApplicationRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> m_OnGroupsAcceptGroupApplicationResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AcceptGroupInvitationRequest> m_OnGroupsAcceptGroupInvitationRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> m_OnGroupsAcceptGroupInvitationResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<AddMembersRequest> m_OnGroupsAddMembersRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> m_OnGroupsAddMembersResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ApplyToGroupRequest> m_OnGroupsApplyToGroupRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ApplyToGroupResponse> m_OnGroupsApplyToGroupResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<BlockEntityRequest> m_OnGroupsBlockEntityRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> m_OnGroupsBlockEntityResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ChangeMemberRoleRequest> m_OnGroupsChangeMemberRoleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> m_OnGroupsChangeMemberRoleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateGroupRequest> m_OnGroupsCreateGroupRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateGroupResponse> m_OnGroupsCreateGroupResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateGroupRoleRequest> m_OnGroupsCreateRoleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateGroupRoleResponse> m_OnGroupsCreateRoleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteGroupRequest> m_OnGroupsDeleteGroupRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> m_OnGroupsDeleteGroupResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteRoleRequest> m_OnGroupsDeleteRoleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> m_OnGroupsDeleteRoleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetGroupRequest> m_OnGroupsGetGroupRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetGroupResponse> m_OnGroupsGetGroupResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<InviteToGroupRequest> m_OnGroupsInviteToGroupRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<InviteToGroupResponse> m_OnGroupsInviteToGroupResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<IsMemberRequest> m_OnGroupsIsMemberRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<IsMemberResponse> m_OnGroupsIsMemberResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListGroupApplicationsRequest> m_OnGroupsListGroupApplicationsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListGroupApplicationsResponse> m_OnGroupsListGroupApplicationsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListGroupBlocksRequest> m_OnGroupsListGroupBlocksRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListGroupBlocksResponse> m_OnGroupsListGroupBlocksResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListGroupInvitationsRequest> m_OnGroupsListGroupInvitationsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListGroupInvitationsResponse> m_OnGroupsListGroupInvitationsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListGroupMembersRequest> m_OnGroupsListGroupMembersRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListGroupMembersResponse> m_OnGroupsListGroupMembersResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListMembershipRequest> m_OnGroupsListMembershipRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListMembershipResponse> m_OnGroupsListMembershipResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListMembershipOpportunitiesRequest> m_OnGroupsListMembershipOpportunitiesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListMembershipOpportunitiesResponse> m_OnGroupsListMembershipOpportunitiesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RemoveGroupApplicationRequest> m_OnGroupsRemoveGroupApplicationRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> m_OnGroupsRemoveGroupApplicationResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RemoveGroupInvitationRequest> m_OnGroupsRemoveGroupInvitationRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> m_OnGroupsRemoveGroupInvitationResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RemoveMembersRequest> m_OnGroupsRemoveMembersRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> m_OnGroupsRemoveMembersResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnblockEntityRequest> m_OnGroupsUnblockEntityRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> m_OnGroupsUnblockEntityResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateGroupRequest> m_OnGroupsUpdateGroupRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdateGroupResponse> m_OnGroupsUpdateGroupResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateGroupRoleRequest> m_OnGroupsUpdateRoleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdateGroupRoleResponse> m_OnGroupsUpdateRoleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<InsightsEmptyRequest> m_OnInsightsGetDetailsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<InsightsGetDetailsResponse> m_OnInsightsGetDetailsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<InsightsEmptyRequest> m_OnInsightsGetLimitsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<InsightsGetLimitsResponse> m_OnInsightsGetLimitsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<InsightsGetOperationStatusRequest> m_OnInsightsGetOperationStatusRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<InsightsGetOperationStatusResponse> m_OnInsightsGetOperationStatusResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<InsightsGetPendingOperationsRequest> m_OnInsightsGetPendingOperationsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<InsightsGetPendingOperationsResponse> m_OnInsightsGetPendingOperationsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<InsightsSetPerformanceRequest> m_OnInsightsSetPerformanceRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<InsightsOperationResponse> m_OnInsightsSetPerformanceResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<InsightsSetStorageRetentionRequest> m_OnInsightsSetStorageRetentionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<InsightsOperationResponse> m_OnInsightsSetStorageRetentionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetLanguageListRequest> m_OnLocalizationGetLanguageListRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetLanguageListResponse> m_OnLocalizationGetLanguageListResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CancelAllMatchmakingTicketsForPlayerRequest> m_OnMultiplayerCancelAllMatchmakingTicketsForPlayerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CancelAllMatchmakingTicketsForPlayerResult> m_OnMultiplayerCancelAllMatchmakingTicketsForPlayerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CancelAllServerBackfillTicketsForPlayerRequest> m_OnMultiplayerCancelAllServerBackfillTicketsForPlayerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CancelAllServerBackfillTicketsForPlayerResult> m_OnMultiplayerCancelAllServerBackfillTicketsForPlayerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CancelMatchmakingTicketRequest> m_OnMultiplayerCancelMatchmakingTicketRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CancelMatchmakingTicketResult> m_OnMultiplayerCancelMatchmakingTicketResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CancelServerBackfillTicketRequest> m_OnMultiplayerCancelServerBackfillTicketRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CancelServerBackfillTicketResult> m_OnMultiplayerCancelServerBackfillTicketResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateBuildAliasRequest> m_OnMultiplayerCreateBuildAliasRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<BuildAliasDetailsResponse> m_OnMultiplayerCreateBuildAliasResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateBuildWithCustomContainerRequest> m_OnMultiplayerCreateBuildWithCustomContainerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateBuildWithCustomContainerResponse> m_OnMultiplayerCreateBuildWithCustomContainerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateBuildWithManagedContainerRequest> m_OnMultiplayerCreateBuildWithManagedContainerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateBuildWithManagedContainerResponse> m_OnMultiplayerCreateBuildWithManagedContainerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateBuildWithProcessBasedServerRequest> m_OnMultiplayerCreateBuildWithProcessBasedServerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateBuildWithProcessBasedServerResponse> m_OnMultiplayerCreateBuildWithProcessBasedServerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateLobbyRequest> m_OnMultiplayerCreateLobbyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateLobbyResult> m_OnMultiplayerCreateLobbyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateMatchmakingTicketRequest> m_OnMultiplayerCreateMatchmakingTicketRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateMatchmakingTicketResult> m_OnMultiplayerCreateMatchmakingTicketResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateRemoteUserRequest> m_OnMultiplayerCreateRemoteUserRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateRemoteUserResponse> m_OnMultiplayerCreateRemoteUserResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateServerBackfillTicketRequest> m_OnMultiplayerCreateServerBackfillTicketRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateServerBackfillTicketResult> m_OnMultiplayerCreateServerBackfillTicketResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateServerMatchmakingTicketRequest> m_OnMultiplayerCreateServerMatchmakingTicketRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateMatchmakingTicketResult> m_OnMultiplayerCreateServerMatchmakingTicketResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateTitleMultiplayerServersQuotaChangeRequest> m_OnMultiplayerCreateTitleMultiplayerServersQuotaChangeRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<CreateTitleMultiplayerServersQuotaChangeResponse> m_OnMultiplayerCreateTitleMultiplayerServersQuotaChangeResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteAssetRequest> m_OnMultiplayerDeleteAssetRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerDeleteAssetResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteBuildRequest> m_OnMultiplayerDeleteBuildRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerDeleteBuildResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteBuildAliasRequest> m_OnMultiplayerDeleteBuildAliasRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerDeleteBuildAliasResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteBuildRegionRequest> m_OnMultiplayerDeleteBuildRegionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerDeleteBuildRegionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteCertificateRequest> m_OnMultiplayerDeleteCertificateRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerDeleteCertificateResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteContainerImageRequest> m_OnMultiplayerDeleteContainerImageRepositoryRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerDeleteContainerImageRepositoryResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteLobbyRequest> m_OnMultiplayerDeleteLobbyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LobbyEmptyResult> m_OnMultiplayerDeleteLobbyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteRemoteUserRequest> m_OnMultiplayerDeleteRemoteUserRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerDeleteRemoteUserResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteSecretRequest> m_OnMultiplayerDeleteSecretRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerDeleteSecretResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<EnableMultiplayerServersForTitleRequest> m_OnMultiplayerEnableMultiplayerServersForTitleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<EnableMultiplayerServersForTitleResponse> m_OnMultiplayerEnableMultiplayerServersForTitleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<FindFriendLobbiesRequest> m_OnMultiplayerFindFriendLobbiesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<FindFriendLobbiesResult> m_OnMultiplayerFindFriendLobbiesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<FindLobbiesRequest> m_OnMultiplayerFindLobbiesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<FindLobbiesResult> m_OnMultiplayerFindLobbiesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetAssetDownloadUrlRequest> m_OnMultiplayerGetAssetDownloadUrlRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetAssetDownloadUrlResponse> m_OnMultiplayerGetAssetDownloadUrlResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetAssetUploadUrlRequest> m_OnMultiplayerGetAssetUploadUrlRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetAssetUploadUrlResponse> m_OnMultiplayerGetAssetUploadUrlResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetBuildRequest> m_OnMultiplayerGetBuildRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetBuildResponse> m_OnMultiplayerGetBuildResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetBuildAliasRequest> m_OnMultiplayerGetBuildAliasRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<BuildAliasDetailsResponse> m_OnMultiplayerGetBuildAliasResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetContainerRegistryCredentialsRequest> m_OnMultiplayerGetContainerRegistryCredentialsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetContainerRegistryCredentialsResponse> m_OnMultiplayerGetContainerRegistryCredentialsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetLobbyRequest> m_OnMultiplayerGetLobbyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetLobbyResult> m_OnMultiplayerGetLobbyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetMatchRequest> m_OnMultiplayerGetMatchRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetMatchResult> m_OnMultiplayerGetMatchResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetMatchmakingQueueRequest> m_OnMultiplayerGetMatchmakingQueueRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetMatchmakingQueueResult> m_OnMultiplayerGetMatchmakingQueueResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetMatchmakingTicketRequest> m_OnMultiplayerGetMatchmakingTicketRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetMatchmakingTicketResult> m_OnMultiplayerGetMatchmakingTicketResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetMultiplayerServerDetailsRequest> m_OnMultiplayerGetMultiplayerServerDetailsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetMultiplayerServerDetailsResponse> m_OnMultiplayerGetMultiplayerServerDetailsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetMultiplayerServerLogsRequest> m_OnMultiplayerGetMultiplayerServerLogsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetMultiplayerServerLogsResponse> m_OnMultiplayerGetMultiplayerServerLogsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetMultiplayerSessionLogsBySessionIdRequest> m_OnMultiplayerGetMultiplayerSessionLogsBySessionIdRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetMultiplayerServerLogsResponse> m_OnMultiplayerGetMultiplayerSessionLogsBySessionIdResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetQueueStatisticsRequest> m_OnMultiplayerGetQueueStatisticsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetQueueStatisticsResult> m_OnMultiplayerGetQueueStatisticsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetRemoteLoginEndpointRequest> m_OnMultiplayerGetRemoteLoginEndpointRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetRemoteLoginEndpointResponse> m_OnMultiplayerGetRemoteLoginEndpointResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetServerBackfillTicketRequest> m_OnMultiplayerGetServerBackfillTicketRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetServerBackfillTicketResult> m_OnMultiplayerGetServerBackfillTicketResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTitleEnabledForMultiplayerServersStatusRequest> m_OnMultiplayerGetTitleEnabledForMultiplayerServersStatusRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTitleEnabledForMultiplayerServersStatusResponse> m_OnMultiplayerGetTitleEnabledForMultiplayerServersStatusResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTitleMultiplayerServersQuotaChangeRequest> m_OnMultiplayerGetTitleMultiplayerServersQuotaChangeRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTitleMultiplayerServersQuotaChangeResponse> m_OnMultiplayerGetTitleMultiplayerServersQuotaChangeResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTitleMultiplayerServersQuotasRequest> m_OnMultiplayerGetTitleMultiplayerServersQuotasRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTitleMultiplayerServersQuotasResponse> m_OnMultiplayerGetTitleMultiplayerServersQuotasResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<InviteToLobbyRequest> m_OnMultiplayerInviteToLobbyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LobbyEmptyResult> m_OnMultiplayerInviteToLobbyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<JoinArrangedLobbyRequest> m_OnMultiplayerJoinArrangedLobbyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<JoinLobbyResult> m_OnMultiplayerJoinArrangedLobbyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<JoinLobbyRequest> m_OnMultiplayerJoinLobbyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<JoinLobbyResult> m_OnMultiplayerJoinLobbyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<JoinLobbyAsServerRequest> m_OnMultiplayerJoinLobbyAsServerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<JoinLobbyAsServerResult> m_OnMultiplayerJoinLobbyAsServerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<JoinMatchmakingTicketRequest> m_OnMultiplayerJoinMatchmakingTicketRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<JoinMatchmakingTicketResult> m_OnMultiplayerJoinMatchmakingTicketResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LeaveLobbyRequest> m_OnMultiplayerLeaveLobbyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LobbyEmptyResult> m_OnMultiplayerLeaveLobbyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<LeaveLobbyAsServerRequest> m_OnMultiplayerLeaveLobbyAsServerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LobbyEmptyResult> m_OnMultiplayerLeaveLobbyAsServerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListMultiplayerServersRequest> m_OnMultiplayerListArchivedMultiplayerServersRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListMultiplayerServersResponse> m_OnMultiplayerListArchivedMultiplayerServersResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListAssetSummariesRequest> m_OnMultiplayerListAssetSummariesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListAssetSummariesResponse> m_OnMultiplayerListAssetSummariesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListBuildAliasesRequest> m_OnMultiplayerListBuildAliasesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListBuildAliasesResponse> m_OnMultiplayerListBuildAliasesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListBuildSummariesRequest> m_OnMultiplayerListBuildSummariesV2RequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListBuildSummariesResponse> m_OnMultiplayerListBuildSummariesV2ResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListCertificateSummariesRequest> m_OnMultiplayerListCertificateSummariesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListCertificateSummariesResponse> m_OnMultiplayerListCertificateSummariesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListContainerImagesRequest> m_OnMultiplayerListContainerImagesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListContainerImagesResponse> m_OnMultiplayerListContainerImagesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListContainerImageTagsRequest> m_OnMultiplayerListContainerImageTagsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListContainerImageTagsResponse> m_OnMultiplayerListContainerImageTagsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListMatchmakingQueuesRequest> m_OnMultiplayerListMatchmakingQueuesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListMatchmakingQueuesResult> m_OnMultiplayerListMatchmakingQueuesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListMatchmakingTicketsForPlayerRequest> m_OnMultiplayerListMatchmakingTicketsForPlayerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListMatchmakingTicketsForPlayerResult> m_OnMultiplayerListMatchmakingTicketsForPlayerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListMultiplayerServersRequest> m_OnMultiplayerListMultiplayerServersRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListMultiplayerServersResponse> m_OnMultiplayerListMultiplayerServersResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListPartyQosServersRequest> m_OnMultiplayerListPartyQosServersRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListPartyQosServersResponse> m_OnMultiplayerListPartyQosServersResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListQosServersForTitleRequest> m_OnMultiplayerListQosServersForTitleRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListQosServersForTitleResponse> m_OnMultiplayerListQosServersForTitleResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListSecretSummariesRequest> m_OnMultiplayerListSecretSummariesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListSecretSummariesResponse> m_OnMultiplayerListSecretSummariesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListServerBackfillTicketsForPlayerRequest> m_OnMultiplayerListServerBackfillTicketsForPlayerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListServerBackfillTicketsForPlayerResult> m_OnMultiplayerListServerBackfillTicketsForPlayerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListTitleMultiplayerServersQuotaChangesRequest> m_OnMultiplayerListTitleMultiplayerServersQuotaChangesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListTitleMultiplayerServersQuotaChangesResponse> m_OnMultiplayerListTitleMultiplayerServersQuotaChangesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListVirtualMachineSummariesRequest> m_OnMultiplayerListVirtualMachineSummariesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListVirtualMachineSummariesResponse> m_OnMultiplayerListVirtualMachineSummariesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RemoveMatchmakingQueueRequest> m_OnMultiplayerRemoveMatchmakingQueueRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RemoveMatchmakingQueueResult> m_OnMultiplayerRemoveMatchmakingQueueResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RemoveMemberFromLobbyRequest> m_OnMultiplayerRemoveMemberRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LobbyEmptyResult> m_OnMultiplayerRemoveMemberResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RequestMultiplayerServerRequest> m_OnMultiplayerRequestMultiplayerServerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RequestMultiplayerServerResponse> m_OnMultiplayerRequestMultiplayerServerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RequestPartyServiceRequest> m_OnMultiplayerRequestPartyServiceRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RequestPartyServiceResponse> m_OnMultiplayerRequestPartyServiceResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<RolloverContainerRegistryCredentialsRequest> m_OnMultiplayerRolloverContainerRegistryCredentialsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<RolloverContainerRegistryCredentialsResponse> m_OnMultiplayerRolloverContainerRegistryCredentialsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SetMatchmakingQueueRequest> m_OnMultiplayerSetMatchmakingQueueRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SetMatchmakingQueueResult> m_OnMultiplayerSetMatchmakingQueueResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ShutdownMultiplayerServerRequest> m_OnMultiplayerShutdownMultiplayerServerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerShutdownMultiplayerServerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SubscribeToLobbyResourceRequest> m_OnMultiplayerSubscribeToLobbyResourceRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SubscribeToLobbyResourceResult> m_OnMultiplayerSubscribeToLobbyResourceResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SubscribeToMatchResourceRequest> m_OnMultiplayerSubscribeToMatchmakingResourceRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SubscribeToMatchResourceResult> m_OnMultiplayerSubscribeToMatchmakingResourceResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnsubscribeFromLobbyResourceRequest> m_OnMultiplayerUnsubscribeFromLobbyResourceRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LobbyEmptyResult> m_OnMultiplayerUnsubscribeFromLobbyResourceResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnsubscribeFromMatchResourceRequest> m_OnMultiplayerUnsubscribeFromMatchmakingResourceRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UnsubscribeFromMatchResourceResult> m_OnMultiplayerUnsubscribeFromMatchmakingResourceResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UntagContainerImageRequest> m_OnMultiplayerUntagContainerImageRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerUntagContainerImageResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateBuildAliasRequest> m_OnMultiplayerUpdateBuildAliasRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<BuildAliasDetailsResponse> m_OnMultiplayerUpdateBuildAliasResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateBuildNameRequest> m_OnMultiplayerUpdateBuildNameRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerUpdateBuildNameResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateBuildRegionRequest> m_OnMultiplayerUpdateBuildRegionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerUpdateBuildRegionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateBuildRegionsRequest> m_OnMultiplayerUpdateBuildRegionsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerUpdateBuildRegionsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateLobbyRequest> m_OnMultiplayerUpdateLobbyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LobbyEmptyResult> m_OnMultiplayerUpdateLobbyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateLobbyAsServerRequest> m_OnMultiplayerUpdateLobbyAsServerRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<LobbyEmptyResult> m_OnMultiplayerUpdateLobbyAsServerResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UploadCertificateRequest> m_OnMultiplayerUploadCertificateRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerUploadCertificateResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UploadSecretRequest> m_OnMultiplayerUploadSecretRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> m_OnMultiplayerUploadSecretResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetGlobalPolicyRequest> m_OnProfilesGetGlobalPolicyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetGlobalPolicyResponse> m_OnProfilesGetGlobalPolicyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetEntityProfileRequest> m_OnProfilesGetProfileRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetEntityProfileResponse> m_OnProfilesGetProfileResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetEntityProfilesRequest> m_OnProfilesGetProfilesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetEntityProfilesResponse> m_OnProfilesGetProfilesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTitlePlayersFromMasterPlayerAccountIdsRequest> m_OnProfilesGetTitlePlayersFromMasterPlayerAccountIdsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTitlePlayersFromMasterPlayerAccountIdsResponse> m_OnProfilesGetTitlePlayersFromMasterPlayerAccountIdsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetTitlePlayersFromXboxLiveIDsRequest> m_OnProfilesGetTitlePlayersFromXboxLiveIDsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetTitlePlayersFromProviderIDsResponse> m_OnProfilesGetTitlePlayersFromXboxLiveIDsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SetDisplayNameRequest> m_OnProfilesSetDisplayNameRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SetDisplayNameResponse> m_OnProfilesSetDisplayNameResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SetGlobalPolicyRequest> m_OnProfilesSetGlobalPolicyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SetGlobalPolicyResponse> m_OnProfilesSetGlobalPolicyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SetProfileLanguageRequest> m_OnProfilesSetProfileLanguageRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SetProfileLanguageResponse> m_OnProfilesSetProfileLanguageResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<SetEntityProfilePolicyRequest> m_OnProfilesSetProfilePolicyRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<SetEntityProfilePolicyResponse> m_OnProfilesSetProfilePolicyResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateLeaderboardDefinitionRequest> m_OnProgressionCreateLeaderboardDefinitionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> m_OnProgressionCreateLeaderboardDefinitionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<CreateStatisticDefinitionRequest> m_OnProgressionCreateStatisticDefinitionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> m_OnProgressionCreateStatisticDefinitionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteLeaderboardDefinitionRequest> m_OnProgressionDeleteLeaderboardDefinitionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> m_OnProgressionDeleteLeaderboardDefinitionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteLeaderboardEntriesRequest> m_OnProgressionDeleteLeaderboardEntriesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> m_OnProgressionDeleteLeaderboardEntriesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteStatisticDefinitionRequest> m_OnProgressionDeleteStatisticDefinitionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> m_OnProgressionDeleteStatisticDefinitionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<DeleteStatisticsRequest> m_OnProgressionDeleteStatisticsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<DeleteStatisticsResponse> m_OnProgressionDeleteStatisticsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetFriendLeaderboardForEntityRequest> m_OnProgressionGetFriendLeaderboardForEntityRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetEntityLeaderboardResponse> m_OnProgressionGetFriendLeaderboardForEntityResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetEntityLeaderboardRequest> m_OnProgressionGetLeaderboardRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetEntityLeaderboardResponse> m_OnProgressionGetLeaderboardResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetLeaderboardAroundEntityRequest> m_OnProgressionGetLeaderboardAroundEntityRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetEntityLeaderboardResponse> m_OnProgressionGetLeaderboardAroundEntityResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetLeaderboardDefinitionRequest> m_OnProgressionGetLeaderboardDefinitionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetLeaderboardDefinitionResponse> m_OnProgressionGetLeaderboardDefinitionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetLeaderboardForEntitiesRequest> m_OnProgressionGetLeaderboardForEntitiesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetEntityLeaderboardResponse> m_OnProgressionGetLeaderboardForEntitiesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetStatisticDefinitionRequest> m_OnProgressionGetStatisticDefinitionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetStatisticDefinitionResponse> m_OnProgressionGetStatisticDefinitionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetStatisticsRequest> m_OnProgressionGetStatisticsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetStatisticsResponse> m_OnProgressionGetStatisticsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<GetStatisticsForEntitiesRequest> m_OnProgressionGetStatisticsForEntitiesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<GetStatisticsForEntitiesResponse> m_OnProgressionGetStatisticsForEntitiesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<IncrementLeaderboardVersionRequest> m_OnProgressionIncrementLeaderboardVersionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<IncrementLeaderboardVersionResponse> m_OnProgressionIncrementLeaderboardVersionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<IncrementStatisticVersionRequest> m_OnProgressionIncrementStatisticVersionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<IncrementStatisticVersionResponse> m_OnProgressionIncrementStatisticVersionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListLeaderboardDefinitionsRequest> m_OnProgressionListLeaderboardDefinitionsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListLeaderboardDefinitionsResponse> m_OnProgressionListLeaderboardDefinitionsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<ListStatisticDefinitionsRequest> m_OnProgressionListStatisticDefinitionsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<ListStatisticDefinitionsResponse> m_OnProgressionListStatisticDefinitionsResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkAggregationSourceFromStatisticRequest> m_OnProgressionUnlinkAggregationSourceFromStatisticRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> m_OnProgressionUnlinkAggregationSourceFromStatisticResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UnlinkLeaderboardFromStatisticRequest> m_OnProgressionUnlinkLeaderboardFromStatisticRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> m_OnProgressionUnlinkLeaderboardFromStatisticResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateLeaderboardDefinitionRequest> m_OnProgressionUpdateLeaderboardDefinitionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> m_OnProgressionUpdateLeaderboardDefinitionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateLeaderboardEntriesRequest> m_OnProgressionUpdateLeaderboardEntriesRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> m_OnProgressionUpdateLeaderboardEntriesResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateStatisticDefinitionRequest> m_OnProgressionUpdateStatisticDefinitionRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> m_OnProgressionUpdateStatisticDefinitionResultEvent;

		[CompilerGenerated]
		private PlayFabRequestEvent<UpdateStatisticsRequest> m_OnProgressionUpdateStatisticsRequestEvent;

		[CompilerGenerated]
		private PlayFabResultEvent<UpdateStatisticsResponse> m_OnProgressionUpdateStatisticsResultEvent;

		[CompilerGenerated]
		private PlayFabErrorEvent m_OnGlobalErrorEvent;

		private static PlayFabEvents _instance;

		public event PlayFabRequestEvent<CreateOrUpdateAppleRequest> OnAddonCreateOrUpdateAppleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateOrUpdateAppleResponse> OnAddonCreateOrUpdateAppleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateOrUpdateFacebookRequest> OnAddonCreateOrUpdateFacebookRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateOrUpdateFacebookResponse> OnAddonCreateOrUpdateFacebookResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateOrUpdateFacebookInstantGamesRequest> OnAddonCreateOrUpdateFacebookInstantGamesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateOrUpdateFacebookInstantGamesResponse> OnAddonCreateOrUpdateFacebookInstantGamesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateOrUpdateGoogleRequest> OnAddonCreateOrUpdateGoogleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateOrUpdateGoogleResponse> OnAddonCreateOrUpdateGoogleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateOrUpdateKongregateRequest> OnAddonCreateOrUpdateKongregateRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateOrUpdateKongregateResponse> OnAddonCreateOrUpdateKongregateResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateOrUpdateNintendoRequest> OnAddonCreateOrUpdateNintendoRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateOrUpdateNintendoResponse> OnAddonCreateOrUpdateNintendoResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateOrUpdatePSNRequest> OnAddonCreateOrUpdatePSNRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateOrUpdatePSNResponse> OnAddonCreateOrUpdatePSNResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateOrUpdateSteamRequest> OnAddonCreateOrUpdateSteamRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateOrUpdateSteamResponse> OnAddonCreateOrUpdateSteamResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateOrUpdateToxModRequest> OnAddonCreateOrUpdateToxModRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateOrUpdateToxModResponse> OnAddonCreateOrUpdateToxModResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateOrUpdateTwitchRequest> OnAddonCreateOrUpdateTwitchRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateOrUpdateTwitchResponse> OnAddonCreateOrUpdateTwitchResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteAppleRequest> OnAddonDeleteAppleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteAppleResponse> OnAddonDeleteAppleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteFacebookRequest> OnAddonDeleteFacebookRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteFacebookResponse> OnAddonDeleteFacebookResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteFacebookInstantGamesRequest> OnAddonDeleteFacebookInstantGamesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteFacebookInstantGamesResponse> OnAddonDeleteFacebookInstantGamesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteGoogleRequest> OnAddonDeleteGoogleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteGoogleResponse> OnAddonDeleteGoogleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteKongregateRequest> OnAddonDeleteKongregateRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteKongregateResponse> OnAddonDeleteKongregateResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteNintendoRequest> OnAddonDeleteNintendoRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteNintendoResponse> OnAddonDeleteNintendoResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeletePSNRequest> OnAddonDeletePSNRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeletePSNResponse> OnAddonDeletePSNResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteSteamRequest> OnAddonDeleteSteamRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteSteamResponse> OnAddonDeleteSteamResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteToxModRequest> OnAddonDeleteToxModRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteToxModResponse> OnAddonDeleteToxModResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteTwitchRequest> OnAddonDeleteTwitchRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteTwitchResponse> OnAddonDeleteTwitchResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetAppleRequest> OnAddonGetAppleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetAppleResponse> OnAddonGetAppleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetFacebookRequest> OnAddonGetFacebookRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetFacebookResponse> OnAddonGetFacebookResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetFacebookInstantGamesRequest> OnAddonGetFacebookInstantGamesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetFacebookInstantGamesResponse> OnAddonGetFacebookInstantGamesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetGoogleRequest> OnAddonGetGoogleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetGoogleResponse> OnAddonGetGoogleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetKongregateRequest> OnAddonGetKongregateRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetKongregateResponse> OnAddonGetKongregateResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetNintendoRequest> OnAddonGetNintendoRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetNintendoResponse> OnAddonGetNintendoResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPSNRequest> OnAddonGetPSNRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPSNResponse> OnAddonGetPSNResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetSteamRequest> OnAddonGetSteamRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetSteamResponse> OnAddonGetSteamResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetToxModRequest> OnAddonGetToxModRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetToxModResponse> OnAddonGetToxModResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTwitchRequest> OnAddonGetTwitchRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTwitchResponse> OnAddonGetTwitchResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AuthenticateCustomIdRequest> OnAuthenticationAuthenticateGameServerWithCustomIdRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<AuthenticateCustomIdResult> OnAuthenticationAuthenticateGameServerWithCustomIdResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteRequest> OnAuthenticationDeleteRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.AuthenticationModels.EmptyResponse> OnAuthenticationDeleteResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetEntityTokenRequest> OnAuthenticationGetEntityTokenRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetEntityTokenResponse> OnAuthenticationGetEntityTokenResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ValidateEntityTokenRequest> OnAuthenticationValidateEntityTokenRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ValidateEntityTokenResponse> OnAuthenticationValidateEntityTokenResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LoginResult> OnLoginResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AcceptTradeRequest> OnAcceptTradeRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<AcceptTradeResponse> OnAcceptTradeResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AddFriendRequest> OnAddFriendRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<AddFriendResult> OnAddFriendResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AddGenericIDRequest> OnAddGenericIDRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<AddGenericIDResult> OnAddGenericIDResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AddOrUpdateContactEmailRequest> OnAddOrUpdateContactEmailRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<AddOrUpdateContactEmailResult> OnAddOrUpdateContactEmailResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AddSharedGroupMembersRequest> OnAddSharedGroupMembersRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<AddSharedGroupMembersResult> OnAddSharedGroupMembersResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AddUsernamePasswordRequest> OnAddUsernamePasswordRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<AddUsernamePasswordResult> OnAddUsernamePasswordResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AddUserVirtualCurrencyRequest> OnAddUserVirtualCurrencyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ModifyUserVirtualCurrencyResult> OnAddUserVirtualCurrencyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AndroidDevicePushNotificationRegistrationRequest> OnAndroidDevicePushNotificationRegistrationRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<AndroidDevicePushNotificationRegistrationResult> OnAndroidDevicePushNotificationRegistrationResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AttributeInstallRequest> OnAttributeInstallRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<AttributeInstallResult> OnAttributeInstallResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CancelTradeRequest> OnCancelTradeRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CancelTradeResponse> OnCancelTradeResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ConfirmPurchaseRequest> OnConfirmPurchaseRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ConfirmPurchaseResult> OnConfirmPurchaseResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ConsumeItemRequest> OnConsumeItemRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ConsumeItemResult> OnConsumeItemResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ConsumeMicrosoftStoreEntitlementsRequest> OnConsumeMicrosoftStoreEntitlementsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ConsumeMicrosoftStoreEntitlementsResponse> OnConsumeMicrosoftStoreEntitlementsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ConsumePS5EntitlementsRequest> OnConsumePS5EntitlementsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ConsumePS5EntitlementsResult> OnConsumePS5EntitlementsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ConsumePSNEntitlementsRequest> OnConsumePSNEntitlementsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ConsumePSNEntitlementsResult> OnConsumePSNEntitlementsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ConsumeXboxEntitlementsRequest> OnConsumeXboxEntitlementsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ConsumeXboxEntitlementsResult> OnConsumeXboxEntitlementsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateSharedGroupRequest> OnCreateSharedGroupRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateSharedGroupResult> OnCreateSharedGroupResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeletePlayerCustomPropertiesRequest> OnDeletePlayerCustomPropertiesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeletePlayerCustomPropertiesResult> OnDeletePlayerCustomPropertiesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ExecuteCloudScriptRequest> OnExecuteCloudScriptRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ClientModels.ExecuteCloudScriptResult> OnExecuteCloudScriptResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetAccountInfoRequest> OnGetAccountInfoRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetAccountInfoResult> OnGetAccountInfoResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetAdPlacementsRequest> OnGetAdPlacementsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetAdPlacementsResult> OnGetAdPlacementsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListUsersCharactersRequest> OnGetAllUsersCharactersRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListUsersCharactersResult> OnGetAllUsersCharactersResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetCatalogItemsRequest> OnGetCatalogItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetCatalogItemsResult> OnGetCatalogItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetCharacterDataRequest> OnGetCharacterDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetCharacterDataResult> OnGetCharacterDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetCharacterInventoryRequest> OnGetCharacterInventoryRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetCharacterInventoryResult> OnGetCharacterInventoryResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetCharacterLeaderboardRequest> OnGetCharacterLeaderboardRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetCharacterLeaderboardResult> OnGetCharacterLeaderboardResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetCharacterDataRequest> OnGetCharacterReadOnlyDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetCharacterDataResult> OnGetCharacterReadOnlyDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetCharacterStatisticsRequest> OnGetCharacterStatisticsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetCharacterStatisticsResult> OnGetCharacterStatisticsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetContentDownloadUrlRequest> OnGetContentDownloadUrlRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetContentDownloadUrlResult> OnGetContentDownloadUrlResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetFriendLeaderboardRequest> OnGetFriendLeaderboardRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetLeaderboardResult> OnGetFriendLeaderboardResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetFriendLeaderboardAroundPlayerRequest> OnGetFriendLeaderboardAroundPlayerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetFriendLeaderboardAroundPlayerResult> OnGetFriendLeaderboardAroundPlayerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetFriendsListRequest> OnGetFriendsListRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetFriendsListResult> OnGetFriendsListResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetLeaderboardRequest> OnGetLeaderboardRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetLeaderboardResult> OnGetLeaderboardResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetLeaderboardAroundCharacterRequest> OnGetLeaderboardAroundCharacterRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetLeaderboardAroundCharacterResult> OnGetLeaderboardAroundCharacterResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetLeaderboardAroundPlayerRequest> OnGetLeaderboardAroundPlayerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetLeaderboardAroundPlayerResult> OnGetLeaderboardAroundPlayerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetLeaderboardForUsersCharactersRequest> OnGetLeaderboardForUserCharactersRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetLeaderboardForUsersCharactersResult> OnGetLeaderboardForUserCharactersResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPaymentTokenRequest> OnGetPaymentTokenRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPaymentTokenResult> OnGetPaymentTokenResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPhotonAuthenticationTokenRequest> OnGetPhotonAuthenticationTokenRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPhotonAuthenticationTokenResult> OnGetPhotonAuthenticationTokenResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayerCombinedInfoRequest> OnGetPlayerCombinedInfoRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayerCombinedInfoResult> OnGetPlayerCombinedInfoResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayerCustomPropertyRequest> OnGetPlayerCustomPropertyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayerCustomPropertyResult> OnGetPlayerCustomPropertyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayerProfileRequest> OnGetPlayerProfileRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayerProfileResult> OnGetPlayerProfileResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayerSegmentsRequest> OnGetPlayerSegmentsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayerSegmentsResult> OnGetPlayerSegmentsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayerStatisticsRequest> OnGetPlayerStatisticsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayerStatisticsResult> OnGetPlayerStatisticsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayerStatisticVersionsRequest> OnGetPlayerStatisticVersionsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayerStatisticVersionsResult> OnGetPlayerStatisticVersionsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayerTagsRequest> OnGetPlayerTagsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayerTagsResult> OnGetPlayerTagsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayerTradesRequest> OnGetPlayerTradesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayerTradesResponse> OnGetPlayerTradesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromBattleNetAccountIdsRequest> OnGetPlayFabIDsFromBattleNetAccountIdsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromBattleNetAccountIdsResult> OnGetPlayFabIDsFromBattleNetAccountIdsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromFacebookIDsRequest> OnGetPlayFabIDsFromFacebookIDsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromFacebookIDsResult> OnGetPlayFabIDsFromFacebookIDsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromFacebookInstantGamesIdsRequest> OnGetPlayFabIDsFromFacebookInstantGamesIdsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromFacebookInstantGamesIdsResult> OnGetPlayFabIDsFromFacebookInstantGamesIdsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromGameCenterIDsRequest> OnGetPlayFabIDsFromGameCenterIDsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromGameCenterIDsResult> OnGetPlayFabIDsFromGameCenterIDsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromGenericIDsRequest> OnGetPlayFabIDsFromGenericIDsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromGenericIDsResult> OnGetPlayFabIDsFromGenericIDsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromGoogleIDsRequest> OnGetPlayFabIDsFromGoogleIDsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromGoogleIDsResult> OnGetPlayFabIDsFromGoogleIDsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromGooglePlayGamesPlayerIDsRequest> OnGetPlayFabIDsFromGooglePlayGamesPlayerIDsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromGooglePlayGamesPlayerIDsResult> OnGetPlayFabIDsFromGooglePlayGamesPlayerIDsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromKongregateIDsRequest> OnGetPlayFabIDsFromKongregateIDsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromKongregateIDsResult> OnGetPlayFabIDsFromKongregateIDsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromNintendoServiceAccountIdsRequest> OnGetPlayFabIDsFromNintendoServiceAccountIdsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromNintendoServiceAccountIdsResult> OnGetPlayFabIDsFromNintendoServiceAccountIdsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromNintendoSwitchDeviceIdsRequest> OnGetPlayFabIDsFromNintendoSwitchDeviceIdsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromNintendoSwitchDeviceIdsResult> OnGetPlayFabIDsFromNintendoSwitchDeviceIdsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromOpenIdsRequest> OnGetPlayFabIDsFromOpenIdSubjectIdentifiersRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromOpenIdsResult> OnGetPlayFabIDsFromOpenIdSubjectIdentifiersResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromPSNAccountIDsRequest> OnGetPlayFabIDsFromPSNAccountIDsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromPSNAccountIDsResult> OnGetPlayFabIDsFromPSNAccountIDsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromPSNOnlineIDsRequest> OnGetPlayFabIDsFromPSNOnlineIDsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromPSNOnlineIDsResult> OnGetPlayFabIDsFromPSNOnlineIDsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromSteamIDsRequest> OnGetPlayFabIDsFromSteamIDsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromSteamIDsResult> OnGetPlayFabIDsFromSteamIDsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromSteamNamesRequest> OnGetPlayFabIDsFromSteamNamesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromSteamNamesResult> OnGetPlayFabIDsFromSteamNamesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromTwitchIDsRequest> OnGetPlayFabIDsFromTwitchIDsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromTwitchIDsResult> OnGetPlayFabIDsFromTwitchIDsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPlayFabIDsFromXboxLiveIDsRequest> OnGetPlayFabIDsFromXboxLiveIDsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPlayFabIDsFromXboxLiveIDsResult> OnGetPlayFabIDsFromXboxLiveIDsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPublisherDataRequest> OnGetPublisherDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPublisherDataResult> OnGetPublisherDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetPurchaseRequest> OnGetPurchaseRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetPurchaseResult> OnGetPurchaseResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetSharedGroupDataRequest> OnGetSharedGroupDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetSharedGroupDataResult> OnGetSharedGroupDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetStoreItemsRequest> OnGetStoreItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetStoreItemsResult> OnGetStoreItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTimeRequest> OnGetTimeRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTimeResult> OnGetTimeResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTitleDataRequest> OnGetTitleDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTitleDataResult> OnGetTitleDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTitleNewsRequest> OnGetTitleNewsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTitleNewsResult> OnGetTitleNewsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTitlePublicKeyRequest> OnGetTitlePublicKeyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTitlePublicKeyResult> OnGetTitlePublicKeyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTradeStatusRequest> OnGetTradeStatusRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTradeStatusResponse> OnGetTradeStatusResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetUserDataRequest> OnGetUserDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetUserDataResult> OnGetUserDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetUserInventoryRequest> OnGetUserInventoryRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetUserInventoryResult> OnGetUserInventoryResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetUserDataRequest> OnGetUserPublisherDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetUserDataResult> OnGetUserPublisherDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetUserDataRequest> OnGetUserPublisherReadOnlyDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetUserDataResult> OnGetUserPublisherReadOnlyDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetUserDataRequest> OnGetUserReadOnlyDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetUserDataResult> OnGetUserReadOnlyDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GrantCharacterToUserRequest> OnGrantCharacterToUserRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GrantCharacterToUserResult> OnGrantCharacterToUserResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkAndroidDeviceIDRequest> OnLinkAndroidDeviceIDRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkAndroidDeviceIDResult> OnLinkAndroidDeviceIDResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkAppleRequest> OnLinkAppleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResult> OnLinkAppleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkBattleNetAccountRequest> OnLinkBattleNetAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> OnLinkBattleNetAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkCustomIDRequest> OnLinkCustomIDRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkCustomIDResult> OnLinkCustomIDResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkFacebookAccountRequest> OnLinkFacebookAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkFacebookAccountResult> OnLinkFacebookAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkFacebookInstantGamesIdRequest> OnLinkFacebookInstantGamesIdRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkFacebookInstantGamesIdResult> OnLinkFacebookInstantGamesIdResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkGameCenterAccountRequest> OnLinkGameCenterAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkGameCenterAccountResult> OnLinkGameCenterAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkGoogleAccountRequest> OnLinkGoogleAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkGoogleAccountResult> OnLinkGoogleAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkGooglePlayGamesServicesAccountRequest> OnLinkGooglePlayGamesServicesAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkGooglePlayGamesServicesAccountResult> OnLinkGooglePlayGamesServicesAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkIOSDeviceIDRequest> OnLinkIOSDeviceIDRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkIOSDeviceIDResult> OnLinkIOSDeviceIDResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkKongregateAccountRequest> OnLinkKongregateRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkKongregateAccountResult> OnLinkKongregateResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkNintendoServiceAccountRequest> OnLinkNintendoServiceAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResult> OnLinkNintendoServiceAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkNintendoSwitchDeviceIdRequest> OnLinkNintendoSwitchDeviceIdRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkNintendoSwitchDeviceIdResult> OnLinkNintendoSwitchDeviceIdResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkOpenIdConnectRequest> OnLinkOpenIdConnectRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResult> OnLinkOpenIdConnectResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkPSNAccountRequest> OnLinkPSNAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkPSNAccountResult> OnLinkPSNAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkSteamAccountRequest> OnLinkSteamAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkSteamAccountResult> OnLinkSteamAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkTwitchAccountRequest> OnLinkTwitchRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkTwitchAccountResult> OnLinkTwitchResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LinkXboxAccountRequest> OnLinkXboxAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LinkXboxAccountResult> OnLinkXboxAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListPlayerCustomPropertiesRequest> OnListPlayerCustomPropertiesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListPlayerCustomPropertiesResult> OnListPlayerCustomPropertiesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithAndroidDeviceIDRequest> OnLoginWithAndroidDeviceIDRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithAppleRequest> OnLoginWithAppleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithBattleNetRequest> OnLoginWithBattleNetRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithCustomIDRequest> OnLoginWithCustomIDRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithEmailAddressRequest> OnLoginWithEmailAddressRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithFacebookRequest> OnLoginWithFacebookRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithFacebookInstantGamesIdRequest> OnLoginWithFacebookInstantGamesIdRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithGameCenterRequest> OnLoginWithGameCenterRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithGoogleAccountRequest> OnLoginWithGoogleAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithGooglePlayGamesServicesRequest> OnLoginWithGooglePlayGamesServicesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithIOSDeviceIDRequest> OnLoginWithIOSDeviceIDRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithKongregateRequest> OnLoginWithKongregateRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithNintendoServiceAccountRequest> OnLoginWithNintendoServiceAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithNintendoSwitchDeviceIdRequest> OnLoginWithNintendoSwitchDeviceIdRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithOpenIdConnectRequest> OnLoginWithOpenIdConnectRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithPlayFabRequest> OnLoginWithPlayFabRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithPSNRequest> OnLoginWithPSNRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithSteamRequest> OnLoginWithSteamRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithTwitchRequest> OnLoginWithTwitchRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LoginWithXboxRequest> OnLoginWithXboxRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<OpenTradeRequest> OnOpenTradeRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<OpenTradeResponse> OnOpenTradeResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<PayForPurchaseRequest> OnPayForPurchaseRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PayForPurchaseResult> OnPayForPurchaseResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<PurchaseItemRequest> OnPurchaseItemRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PurchaseItemResult> OnPurchaseItemResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RedeemCouponRequest> OnRedeemCouponRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RedeemCouponResult> OnRedeemCouponResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RefreshPSNAuthTokenRequest> OnRefreshPSNAuthTokenRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> OnRefreshPSNAuthTokenResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RegisterForIOSPushNotificationRequest> OnRegisterForIOSPushNotificationRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RegisterForIOSPushNotificationResult> OnRegisterForIOSPushNotificationResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RegisterPlayFabUserRequest> OnRegisterPlayFabUserRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RegisterPlayFabUserResult> OnRegisterPlayFabUserResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RemoveContactEmailRequest> OnRemoveContactEmailRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RemoveContactEmailResult> OnRemoveContactEmailResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RemoveFriendRequest> OnRemoveFriendRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RemoveFriendResult> OnRemoveFriendResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RemoveGenericIDRequest> OnRemoveGenericIDRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RemoveGenericIDResult> OnRemoveGenericIDResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RemoveSharedGroupMembersRequest> OnRemoveSharedGroupMembersRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RemoveSharedGroupMembersResult> OnRemoveSharedGroupMembersResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ReportAdActivityRequest> OnReportAdActivityRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ReportAdActivityResult> OnReportAdActivityResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeviceInfoRequest> OnReportDeviceInfoRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> OnReportDeviceInfoResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ReportPlayerClientRequest> OnReportPlayerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ReportPlayerClientResult> OnReportPlayerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RestoreIOSPurchasesRequest> OnRestoreIOSPurchasesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RestoreIOSPurchasesResult> OnRestoreIOSPurchasesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RewardAdActivityRequest> OnRewardAdActivityRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RewardAdActivityResult> OnRewardAdActivityResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SendAccountRecoveryEmailRequest> OnSendAccountRecoveryEmailRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SendAccountRecoveryEmailResult> OnSendAccountRecoveryEmailResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SetFriendTagsRequest> OnSetFriendTagsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SetFriendTagsResult> OnSetFriendTagsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SetPlayerSecretRequest> OnSetPlayerSecretRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SetPlayerSecretResult> OnSetPlayerSecretResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<StartPurchaseRequest> OnStartPurchaseRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<StartPurchaseResult> OnStartPurchaseResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SubtractUserVirtualCurrencyRequest> OnSubtractUserVirtualCurrencyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ModifyUserVirtualCurrencyResult> OnSubtractUserVirtualCurrencyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkAndroidDeviceIDRequest> OnUnlinkAndroidDeviceIDRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkAndroidDeviceIDResult> OnUnlinkAndroidDeviceIDResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkAppleRequest> OnUnlinkAppleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> OnUnlinkAppleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkBattleNetAccountRequest> OnUnlinkBattleNetAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> OnUnlinkBattleNetAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkCustomIDRequest> OnUnlinkCustomIDRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkCustomIDResult> OnUnlinkCustomIDResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkFacebookAccountRequest> OnUnlinkFacebookAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkFacebookAccountResult> OnUnlinkFacebookAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkFacebookInstantGamesIdRequest> OnUnlinkFacebookInstantGamesIdRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkFacebookInstantGamesIdResult> OnUnlinkFacebookInstantGamesIdResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkGameCenterAccountRequest> OnUnlinkGameCenterAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkGameCenterAccountResult> OnUnlinkGameCenterAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkGoogleAccountRequest> OnUnlinkGoogleAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkGoogleAccountResult> OnUnlinkGoogleAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkGooglePlayGamesServicesAccountRequest> OnUnlinkGooglePlayGamesServicesAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkGooglePlayGamesServicesAccountResult> OnUnlinkGooglePlayGamesServicesAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkIOSDeviceIDRequest> OnUnlinkIOSDeviceIDRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkIOSDeviceIDResult> OnUnlinkIOSDeviceIDResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkKongregateAccountRequest> OnUnlinkKongregateRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkKongregateAccountResult> OnUnlinkKongregateResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkNintendoServiceAccountRequest> OnUnlinkNintendoServiceAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> OnUnlinkNintendoServiceAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkNintendoSwitchDeviceIdRequest> OnUnlinkNintendoSwitchDeviceIdRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkNintendoSwitchDeviceIdResult> OnUnlinkNintendoSwitchDeviceIdResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkOpenIdConnectRequest> OnUnlinkOpenIdConnectRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> OnUnlinkOpenIdConnectResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkPSNAccountRequest> OnUnlinkPSNAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkPSNAccountResult> OnUnlinkPSNAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkSteamAccountRequest> OnUnlinkSteamAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkSteamAccountResult> OnUnlinkSteamAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkTwitchAccountRequest> OnUnlinkTwitchRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkTwitchAccountResult> OnUnlinkTwitchResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkXboxAccountRequest> OnUnlinkXboxAccountRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlinkXboxAccountResult> OnUnlinkXboxAccountResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlockContainerInstanceRequest> OnUnlockContainerInstanceRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlockContainerItemResult> OnUnlockContainerInstanceResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlockContainerItemRequest> OnUnlockContainerItemRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnlockContainerItemResult> OnUnlockContainerItemResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateAvatarUrlRequest> OnUpdateAvatarUrlRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> OnUpdateAvatarUrlResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateCharacterDataRequest> OnUpdateCharacterDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdateCharacterDataResult> OnUpdateCharacterDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateCharacterStatisticsRequest> OnUpdateCharacterStatisticsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdateCharacterStatisticsResult> OnUpdateCharacterStatisticsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdatePlayerCustomPropertiesRequest> OnUpdatePlayerCustomPropertiesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdatePlayerCustomPropertiesResult> OnUpdatePlayerCustomPropertiesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdatePlayerStatisticsRequest> OnUpdatePlayerStatisticsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdatePlayerStatisticsResult> OnUpdatePlayerStatisticsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateSharedGroupDataRequest> OnUpdateSharedGroupDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdateSharedGroupDataResult> OnUpdateSharedGroupDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateUserDataRequest> OnUpdateUserDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdateUserDataResult> OnUpdateUserDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateUserDataRequest> OnUpdateUserPublisherDataRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdateUserDataResult> OnUpdateUserPublisherDataResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateUserTitleDisplayNameRequest> OnUpdateUserTitleDisplayNameRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdateUserTitleDisplayNameResult> OnUpdateUserTitleDisplayNameResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ValidateAmazonReceiptRequest> OnValidateAmazonIAPReceiptRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ValidateAmazonReceiptResult> OnValidateAmazonIAPReceiptResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ValidateGooglePlayPurchaseRequest> OnValidateGooglePlayPurchaseRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ValidateGooglePlayPurchaseResult> OnValidateGooglePlayPurchaseResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ValidateIOSReceiptRequest> OnValidateIOSReceiptRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ValidateIOSReceiptResult> OnValidateIOSReceiptResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ValidateWindowsReceiptRequest> OnValidateWindowsStoreReceiptRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ValidateWindowsReceiptResult> OnValidateWindowsStoreReceiptResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<WriteClientCharacterEventRequest> OnWriteCharacterEventRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<WriteEventResponse> OnWriteCharacterEventResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<WriteClientPlayerEventRequest> OnWritePlayerEventRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<WriteEventResponse> OnWritePlayerEventResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<WriteTitleEventRequest> OnWriteTitleEventRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<WriteEventResponse> OnWriteTitleEventResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ExecuteEntityCloudScriptRequest> OnCloudScriptExecuteEntityCloudScriptRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.CloudScriptModels.ExecuteCloudScriptResult> OnCloudScriptExecuteEntityCloudScriptResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ExecuteFunctionRequest> OnCloudScriptExecuteFunctionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ExecuteFunctionResult> OnCloudScriptExecuteFunctionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetFunctionRequest> OnCloudScriptGetFunctionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetFunctionResult> OnCloudScriptGetFunctionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListFunctionsRequest> OnCloudScriptListEventHubFunctionsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListEventHubFunctionsResult> OnCloudScriptListEventHubFunctionsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListFunctionsRequest> OnCloudScriptListFunctionsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListFunctionsResult> OnCloudScriptListFunctionsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListFunctionsRequest> OnCloudScriptListHttpFunctionsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListHttpFunctionsResult> OnCloudScriptListHttpFunctionsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListFunctionsRequest> OnCloudScriptListQueuedFunctionsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListQueuedFunctionsResult> OnCloudScriptListQueuedFunctionsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<PostFunctionResultForEntityTriggeredActionRequest> OnCloudScriptPostFunctionResultForEntityTriggeredActionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> OnCloudScriptPostFunctionResultForEntityTriggeredActionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<PostFunctionResultForFunctionExecutionRequest> OnCloudScriptPostFunctionResultForFunctionExecutionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> OnCloudScriptPostFunctionResultForFunctionExecutionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<PostFunctionResultForPlayerTriggeredActionRequest> OnCloudScriptPostFunctionResultForPlayerTriggeredActionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> OnCloudScriptPostFunctionResultForPlayerTriggeredActionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<PostFunctionResultForScheduledTaskRequest> OnCloudScriptPostFunctionResultForScheduledTaskRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> OnCloudScriptPostFunctionResultForScheduledTaskResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RegisterEventHubFunctionRequest> OnCloudScriptRegisterEventHubFunctionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> OnCloudScriptRegisterEventHubFunctionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RegisterHttpFunctionRequest> OnCloudScriptRegisterHttpFunctionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> OnCloudScriptRegisterHttpFunctionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RegisterQueuedFunctionRequest> OnCloudScriptRegisterQueuedFunctionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> OnCloudScriptRegisterQueuedFunctionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnregisterFunctionRequest> OnCloudScriptUnregisterFunctionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.CloudScriptModels.EmptyResult> OnCloudScriptUnregisterFunctionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AbortFileUploadsRequest> OnDataAbortFileUploadsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<AbortFileUploadsResponse> OnDataAbortFileUploadsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteFilesRequest> OnDataDeleteFilesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteFilesResponse> OnDataDeleteFilesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<FinalizeFileUploadsRequest> OnDataFinalizeFileUploadsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<FinalizeFileUploadsResponse> OnDataFinalizeFileUploadsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetFilesRequest> OnDataGetFilesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetFilesResponse> OnDataGetFilesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetObjectsRequest> OnDataGetObjectsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetObjectsResponse> OnDataGetObjectsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<InitiateFileUploadsRequest> OnDataInitiateFileUploadsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<InitiateFileUploadsResponse> OnDataInitiateFileUploadsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SetObjectsRequest> OnDataSetObjectsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SetObjectsResponse> OnDataSetObjectsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AddInventoryItemsRequest> OnEconomyAddInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<AddInventoryItemsResponse> OnEconomyAddInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateDraftItemRequest> OnEconomyCreateDraftItemRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateDraftItemResponse> OnEconomyCreateDraftItemResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateUploadUrlsRequest> OnEconomyCreateUploadUrlsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateUploadUrlsResponse> OnEconomyCreateUploadUrlsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteEntityItemReviewsRequest> OnEconomyDeleteEntityItemReviewsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteEntityItemReviewsResponse> OnEconomyDeleteEntityItemReviewsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteInventoryCollectionRequest> OnEconomyDeleteInventoryCollectionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteInventoryCollectionResponse> OnEconomyDeleteInventoryCollectionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteInventoryItemsRequest> OnEconomyDeleteInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteInventoryItemsResponse> OnEconomyDeleteInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteItemRequest> OnEconomyDeleteItemRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteItemResponse> OnEconomyDeleteItemResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ExecuteInventoryOperationsRequest> OnEconomyExecuteInventoryOperationsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ExecuteInventoryOperationsResponse> OnEconomyExecuteInventoryOperationsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ExecuteTransferOperationsRequest> OnEconomyExecuteTransferOperationsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ExecuteTransferOperationsResponse> OnEconomyExecuteTransferOperationsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetCatalogConfigRequest> OnEconomyGetCatalogConfigRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetCatalogConfigResponse> OnEconomyGetCatalogConfigResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetDraftItemRequest> OnEconomyGetDraftItemRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetDraftItemResponse> OnEconomyGetDraftItemResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetDraftItemsRequest> OnEconomyGetDraftItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetDraftItemsResponse> OnEconomyGetDraftItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetEntityDraftItemsRequest> OnEconomyGetEntityDraftItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetEntityDraftItemsResponse> OnEconomyGetEntityDraftItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetEntityItemReviewRequest> OnEconomyGetEntityItemReviewRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetEntityItemReviewResponse> OnEconomyGetEntityItemReviewResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetInventoryCollectionIdsRequest> OnEconomyGetInventoryCollectionIdsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetInventoryCollectionIdsResponse> OnEconomyGetInventoryCollectionIdsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetInventoryItemsRequest> OnEconomyGetInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetInventoryItemsResponse> OnEconomyGetInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetInventoryOperationStatusRequest> OnEconomyGetInventoryOperationStatusRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetInventoryOperationStatusResponse> OnEconomyGetInventoryOperationStatusResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetItemRequest> OnEconomyGetItemRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetItemResponse> OnEconomyGetItemResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetItemContainersRequest> OnEconomyGetItemContainersRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetItemContainersResponse> OnEconomyGetItemContainersResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetItemModerationStateRequest> OnEconomyGetItemModerationStateRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetItemModerationStateResponse> OnEconomyGetItemModerationStateResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetItemPublishStatusRequest> OnEconomyGetItemPublishStatusRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetItemPublishStatusResponse> OnEconomyGetItemPublishStatusResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetItemReviewsRequest> OnEconomyGetItemReviewsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetItemReviewsResponse> OnEconomyGetItemReviewsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetItemReviewSummaryRequest> OnEconomyGetItemReviewSummaryRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetItemReviewSummaryResponse> OnEconomyGetItemReviewSummaryResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetItemsRequest> OnEconomyGetItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetItemsResponse> OnEconomyGetItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTransactionHistoryRequest> OnEconomyGetTransactionHistoryRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTransactionHistoryResponse> OnEconomyGetTransactionHistoryResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<PublishDraftItemRequest> OnEconomyPublishDraftItemRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PublishDraftItemResponse> OnEconomyPublishDraftItemResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<PurchaseInventoryItemsRequest> OnEconomyPurchaseInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PurchaseInventoryItemsResponse> OnEconomyPurchaseInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RedeemAppleAppStoreInventoryItemsRequest> OnEconomyRedeemAppleAppStoreInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RedeemAppleAppStoreInventoryItemsResponse> OnEconomyRedeemAppleAppStoreInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RedeemAppleAppStoreWithJwsInventoryItemsRequest> OnEconomyRedeemAppleAppStoreWithJwsInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RedeemAppleAppStoreWithJwsInventoryItemsResponse> OnEconomyRedeemAppleAppStoreWithJwsInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RedeemGooglePlayInventoryItemsRequest> OnEconomyRedeemGooglePlayInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RedeemGooglePlayInventoryItemsResponse> OnEconomyRedeemGooglePlayInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RedeemMicrosoftStoreInventoryItemsRequest> OnEconomyRedeemMicrosoftStoreInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RedeemMicrosoftStoreInventoryItemsResponse> OnEconomyRedeemMicrosoftStoreInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RedeemNintendoEShopInventoryItemsRequest> OnEconomyRedeemNintendoEShopInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RedeemNintendoEShopInventoryItemsResponse> OnEconomyRedeemNintendoEShopInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RedeemPlayStationStoreInventoryItemsRequest> OnEconomyRedeemPlayStationStoreInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RedeemPlayStationStoreInventoryItemsResponse> OnEconomyRedeemPlayStationStoreInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RedeemSteamInventoryItemsRequest> OnEconomyRedeemSteamInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RedeemSteamInventoryItemsResponse> OnEconomyRedeemSteamInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ReportItemRequest> OnEconomyReportItemRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ReportItemResponse> OnEconomyReportItemResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ReportItemReviewRequest> OnEconomyReportItemReviewRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ReportItemReviewResponse> OnEconomyReportItemReviewResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ReviewItemRequest> OnEconomyReviewItemRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ReviewItemResponse> OnEconomyReviewItemResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SearchItemsRequest> OnEconomySearchItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SearchItemsResponse> OnEconomySearchItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SetItemModerationStateRequest> OnEconomySetItemModerationStateRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SetItemModerationStateResponse> OnEconomySetItemModerationStateResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SubmitItemReviewVoteRequest> OnEconomySubmitItemReviewVoteRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SubmitItemReviewVoteResponse> OnEconomySubmitItemReviewVoteResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SubtractInventoryItemsRequest> OnEconomySubtractInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SubtractInventoryItemsResponse> OnEconomySubtractInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<TakedownItemReviewsRequest> OnEconomyTakedownItemReviewsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<TakedownItemReviewsResponse> OnEconomyTakedownItemReviewsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<TransferInventoryItemsRequest> OnEconomyTransferInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<TransferInventoryItemsResponse> OnEconomyTransferInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateCatalogConfigRequest> OnEconomyUpdateCatalogConfigRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdateCatalogConfigResponse> OnEconomyUpdateCatalogConfigResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateDraftItemRequest> OnEconomyUpdateDraftItemRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdateDraftItemResponse> OnEconomyUpdateDraftItemResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateInventoryItemsRequest> OnEconomyUpdateInventoryItemsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdateInventoryItemsResponse> OnEconomyUpdateInventoryItemsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateTelemetryKeyRequest> OnEventsCreateTelemetryKeyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateTelemetryKeyResponse> OnEventsCreateTelemetryKeyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteDataConnectionRequest> OnEventsDeleteDataConnectionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteDataConnectionResponse> OnEventsDeleteDataConnectionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteTelemetryKeyRequest> OnEventsDeleteTelemetryKeyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteTelemetryKeyResponse> OnEventsDeleteTelemetryKeyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetDataConnectionRequest> OnEventsGetDataConnectionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetDataConnectionResponse> OnEventsGetDataConnectionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTelemetryKeyRequest> OnEventsGetTelemetryKeyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTelemetryKeyResponse> OnEventsGetTelemetryKeyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListDataConnectionsRequest> OnEventsListDataConnectionsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListDataConnectionsResponse> OnEventsListDataConnectionsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListTelemetryKeysRequest> OnEventsListTelemetryKeysRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListTelemetryKeysResponse> OnEventsListTelemetryKeysResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SetDataConnectionRequest> OnEventsSetDataConnectionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SetDataConnectionResponse> OnEventsSetDataConnectionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SetDataConnectionActiveRequest> OnEventsSetDataConnectionActiveRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SetDataConnectionActiveResponse> OnEventsSetDataConnectionActiveResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SetTelemetryKeyActiveRequest> OnEventsSetTelemetryKeyActiveRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SetTelemetryKeyActiveResponse> OnEventsSetTelemetryKeyActiveResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<WriteEventsRequest> OnEventsWriteEventsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<WriteEventsResponse> OnEventsWriteEventsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<WriteEventsRequest> OnEventsWriteTelemetryEventsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<WriteEventsResponse> OnEventsWriteTelemetryEventsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateExclusionGroupRequest> OnExperimentationCreateExclusionGroupRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateExclusionGroupResult> OnExperimentationCreateExclusionGroupResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateExperimentRequest> OnExperimentationCreateExperimentRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateExperimentResult> OnExperimentationCreateExperimentResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteExclusionGroupRequest> OnExperimentationDeleteExclusionGroupRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ExperimentationModels.EmptyResponse> OnExperimentationDeleteExclusionGroupResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteExperimentRequest> OnExperimentationDeleteExperimentRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ExperimentationModels.EmptyResponse> OnExperimentationDeleteExperimentResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetExclusionGroupsRequest> OnExperimentationGetExclusionGroupsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetExclusionGroupsResult> OnExperimentationGetExclusionGroupsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetExclusionGroupTrafficRequest> OnExperimentationGetExclusionGroupTrafficRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetExclusionGroupTrafficResult> OnExperimentationGetExclusionGroupTrafficResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetExperimentsRequest> OnExperimentationGetExperimentsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetExperimentsResult> OnExperimentationGetExperimentsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetLatestScorecardRequest> OnExperimentationGetLatestScorecardRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetLatestScorecardResult> OnExperimentationGetLatestScorecardResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTreatmentAssignmentRequest> OnExperimentationGetTreatmentAssignmentRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTreatmentAssignmentResult> OnExperimentationGetTreatmentAssignmentResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<StartExperimentRequest> OnExperimentationStartExperimentRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ExperimentationModels.EmptyResponse> OnExperimentationStartExperimentResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<StopExperimentRequest> OnExperimentationStopExperimentRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ExperimentationModels.EmptyResponse> OnExperimentationStopExperimentResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateExclusionGroupRequest> OnExperimentationUpdateExclusionGroupRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ExperimentationModels.EmptyResponse> OnExperimentationUpdateExclusionGroupResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateExperimentRequest> OnExperimentationUpdateExperimentRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ExperimentationModels.EmptyResponse> OnExperimentationUpdateExperimentResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AcceptGroupApplicationRequest> OnGroupsAcceptGroupApplicationRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsAcceptGroupApplicationResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AcceptGroupInvitationRequest> OnGroupsAcceptGroupInvitationRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsAcceptGroupInvitationResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<AddMembersRequest> OnGroupsAddMembersRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsAddMembersResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ApplyToGroupRequest> OnGroupsApplyToGroupRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ApplyToGroupResponse> OnGroupsApplyToGroupResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<BlockEntityRequest> OnGroupsBlockEntityRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsBlockEntityResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ChangeMemberRoleRequest> OnGroupsChangeMemberRoleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsChangeMemberRoleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateGroupRequest> OnGroupsCreateGroupRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateGroupResponse> OnGroupsCreateGroupResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateGroupRoleRequest> OnGroupsCreateRoleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateGroupRoleResponse> OnGroupsCreateRoleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteGroupRequest> OnGroupsDeleteGroupRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsDeleteGroupResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteRoleRequest> OnGroupsDeleteRoleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsDeleteRoleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetGroupRequest> OnGroupsGetGroupRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetGroupResponse> OnGroupsGetGroupResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<InviteToGroupRequest> OnGroupsInviteToGroupRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<InviteToGroupResponse> OnGroupsInviteToGroupResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<IsMemberRequest> OnGroupsIsMemberRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<IsMemberResponse> OnGroupsIsMemberResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListGroupApplicationsRequest> OnGroupsListGroupApplicationsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListGroupApplicationsResponse> OnGroupsListGroupApplicationsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListGroupBlocksRequest> OnGroupsListGroupBlocksRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListGroupBlocksResponse> OnGroupsListGroupBlocksResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListGroupInvitationsRequest> OnGroupsListGroupInvitationsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListGroupInvitationsResponse> OnGroupsListGroupInvitationsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListGroupMembersRequest> OnGroupsListGroupMembersRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListGroupMembersResponse> OnGroupsListGroupMembersResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListMembershipRequest> OnGroupsListMembershipRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListMembershipResponse> OnGroupsListMembershipResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListMembershipOpportunitiesRequest> OnGroupsListMembershipOpportunitiesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListMembershipOpportunitiesResponse> OnGroupsListMembershipOpportunitiesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RemoveGroupApplicationRequest> OnGroupsRemoveGroupApplicationRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsRemoveGroupApplicationResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RemoveGroupInvitationRequest> OnGroupsRemoveGroupInvitationRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsRemoveGroupInvitationResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RemoveMembersRequest> OnGroupsRemoveMembersRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsRemoveMembersResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnblockEntityRequest> OnGroupsUnblockEntityRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsUnblockEntityResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateGroupRequest> OnGroupsUpdateGroupRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdateGroupResponse> OnGroupsUpdateGroupResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateGroupRoleRequest> OnGroupsUpdateRoleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdateGroupRoleResponse> OnGroupsUpdateRoleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<InsightsEmptyRequest> OnInsightsGetDetailsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<InsightsGetDetailsResponse> OnInsightsGetDetailsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<InsightsEmptyRequest> OnInsightsGetLimitsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<InsightsGetLimitsResponse> OnInsightsGetLimitsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<InsightsGetOperationStatusRequest> OnInsightsGetOperationStatusRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<InsightsGetOperationStatusResponse> OnInsightsGetOperationStatusResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<InsightsGetPendingOperationsRequest> OnInsightsGetPendingOperationsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<InsightsGetPendingOperationsResponse> OnInsightsGetPendingOperationsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<InsightsSetPerformanceRequest> OnInsightsSetPerformanceRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<InsightsOperationResponse> OnInsightsSetPerformanceResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<InsightsSetStorageRetentionRequest> OnInsightsSetStorageRetentionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<InsightsOperationResponse> OnInsightsSetStorageRetentionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetLanguageListRequest> OnLocalizationGetLanguageListRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetLanguageListResponse> OnLocalizationGetLanguageListResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CancelAllMatchmakingTicketsForPlayerRequest> OnMultiplayerCancelAllMatchmakingTicketsForPlayerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CancelAllMatchmakingTicketsForPlayerResult> OnMultiplayerCancelAllMatchmakingTicketsForPlayerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CancelAllServerBackfillTicketsForPlayerRequest> OnMultiplayerCancelAllServerBackfillTicketsForPlayerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CancelAllServerBackfillTicketsForPlayerResult> OnMultiplayerCancelAllServerBackfillTicketsForPlayerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CancelMatchmakingTicketRequest> OnMultiplayerCancelMatchmakingTicketRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CancelMatchmakingTicketResult> OnMultiplayerCancelMatchmakingTicketResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CancelServerBackfillTicketRequest> OnMultiplayerCancelServerBackfillTicketRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CancelServerBackfillTicketResult> OnMultiplayerCancelServerBackfillTicketResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateBuildAliasRequest> OnMultiplayerCreateBuildAliasRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<BuildAliasDetailsResponse> OnMultiplayerCreateBuildAliasResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateBuildWithCustomContainerRequest> OnMultiplayerCreateBuildWithCustomContainerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateBuildWithCustomContainerResponse> OnMultiplayerCreateBuildWithCustomContainerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateBuildWithManagedContainerRequest> OnMultiplayerCreateBuildWithManagedContainerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateBuildWithManagedContainerResponse> OnMultiplayerCreateBuildWithManagedContainerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateBuildWithProcessBasedServerRequest> OnMultiplayerCreateBuildWithProcessBasedServerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateBuildWithProcessBasedServerResponse> OnMultiplayerCreateBuildWithProcessBasedServerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateLobbyRequest> OnMultiplayerCreateLobbyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateLobbyResult> OnMultiplayerCreateLobbyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateMatchmakingTicketRequest> OnMultiplayerCreateMatchmakingTicketRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateMatchmakingTicketResult> OnMultiplayerCreateMatchmakingTicketResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateRemoteUserRequest> OnMultiplayerCreateRemoteUserRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateRemoteUserResponse> OnMultiplayerCreateRemoteUserResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateServerBackfillTicketRequest> OnMultiplayerCreateServerBackfillTicketRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateServerBackfillTicketResult> OnMultiplayerCreateServerBackfillTicketResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateServerMatchmakingTicketRequest> OnMultiplayerCreateServerMatchmakingTicketRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateMatchmakingTicketResult> OnMultiplayerCreateServerMatchmakingTicketResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateTitleMultiplayerServersQuotaChangeRequest> OnMultiplayerCreateTitleMultiplayerServersQuotaChangeRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<CreateTitleMultiplayerServersQuotaChangeResponse> OnMultiplayerCreateTitleMultiplayerServersQuotaChangeResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteAssetRequest> OnMultiplayerDeleteAssetRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerDeleteAssetResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteBuildRequest> OnMultiplayerDeleteBuildRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerDeleteBuildResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteBuildAliasRequest> OnMultiplayerDeleteBuildAliasRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerDeleteBuildAliasResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteBuildRegionRequest> OnMultiplayerDeleteBuildRegionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerDeleteBuildRegionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteCertificateRequest> OnMultiplayerDeleteCertificateRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerDeleteCertificateResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteContainerImageRequest> OnMultiplayerDeleteContainerImageRepositoryRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerDeleteContainerImageRepositoryResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteLobbyRequest> OnMultiplayerDeleteLobbyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LobbyEmptyResult> OnMultiplayerDeleteLobbyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteRemoteUserRequest> OnMultiplayerDeleteRemoteUserRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerDeleteRemoteUserResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteSecretRequest> OnMultiplayerDeleteSecretRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerDeleteSecretResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<EnableMultiplayerServersForTitleRequest> OnMultiplayerEnableMultiplayerServersForTitleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<EnableMultiplayerServersForTitleResponse> OnMultiplayerEnableMultiplayerServersForTitleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<FindFriendLobbiesRequest> OnMultiplayerFindFriendLobbiesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<FindFriendLobbiesResult> OnMultiplayerFindFriendLobbiesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<FindLobbiesRequest> OnMultiplayerFindLobbiesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<FindLobbiesResult> OnMultiplayerFindLobbiesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetAssetDownloadUrlRequest> OnMultiplayerGetAssetDownloadUrlRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetAssetDownloadUrlResponse> OnMultiplayerGetAssetDownloadUrlResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetAssetUploadUrlRequest> OnMultiplayerGetAssetUploadUrlRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetAssetUploadUrlResponse> OnMultiplayerGetAssetUploadUrlResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetBuildRequest> OnMultiplayerGetBuildRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetBuildResponse> OnMultiplayerGetBuildResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetBuildAliasRequest> OnMultiplayerGetBuildAliasRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<BuildAliasDetailsResponse> OnMultiplayerGetBuildAliasResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetContainerRegistryCredentialsRequest> OnMultiplayerGetContainerRegistryCredentialsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetContainerRegistryCredentialsResponse> OnMultiplayerGetContainerRegistryCredentialsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetLobbyRequest> OnMultiplayerGetLobbyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetLobbyResult> OnMultiplayerGetLobbyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetMatchRequest> OnMultiplayerGetMatchRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetMatchResult> OnMultiplayerGetMatchResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetMatchmakingQueueRequest> OnMultiplayerGetMatchmakingQueueRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetMatchmakingQueueResult> OnMultiplayerGetMatchmakingQueueResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetMatchmakingTicketRequest> OnMultiplayerGetMatchmakingTicketRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetMatchmakingTicketResult> OnMultiplayerGetMatchmakingTicketResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetMultiplayerServerDetailsRequest> OnMultiplayerGetMultiplayerServerDetailsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetMultiplayerServerDetailsResponse> OnMultiplayerGetMultiplayerServerDetailsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetMultiplayerServerLogsRequest> OnMultiplayerGetMultiplayerServerLogsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetMultiplayerServerLogsResponse> OnMultiplayerGetMultiplayerServerLogsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetMultiplayerSessionLogsBySessionIdRequest> OnMultiplayerGetMultiplayerSessionLogsBySessionIdRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetMultiplayerServerLogsResponse> OnMultiplayerGetMultiplayerSessionLogsBySessionIdResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetQueueStatisticsRequest> OnMultiplayerGetQueueStatisticsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetQueueStatisticsResult> OnMultiplayerGetQueueStatisticsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetRemoteLoginEndpointRequest> OnMultiplayerGetRemoteLoginEndpointRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetRemoteLoginEndpointResponse> OnMultiplayerGetRemoteLoginEndpointResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetServerBackfillTicketRequest> OnMultiplayerGetServerBackfillTicketRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetServerBackfillTicketResult> OnMultiplayerGetServerBackfillTicketResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTitleEnabledForMultiplayerServersStatusRequest> OnMultiplayerGetTitleEnabledForMultiplayerServersStatusRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTitleEnabledForMultiplayerServersStatusResponse> OnMultiplayerGetTitleEnabledForMultiplayerServersStatusResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTitleMultiplayerServersQuotaChangeRequest> OnMultiplayerGetTitleMultiplayerServersQuotaChangeRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTitleMultiplayerServersQuotaChangeResponse> OnMultiplayerGetTitleMultiplayerServersQuotaChangeResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTitleMultiplayerServersQuotasRequest> OnMultiplayerGetTitleMultiplayerServersQuotasRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTitleMultiplayerServersQuotasResponse> OnMultiplayerGetTitleMultiplayerServersQuotasResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<InviteToLobbyRequest> OnMultiplayerInviteToLobbyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LobbyEmptyResult> OnMultiplayerInviteToLobbyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<JoinArrangedLobbyRequest> OnMultiplayerJoinArrangedLobbyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<JoinLobbyResult> OnMultiplayerJoinArrangedLobbyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<JoinLobbyRequest> OnMultiplayerJoinLobbyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<JoinLobbyResult> OnMultiplayerJoinLobbyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<JoinLobbyAsServerRequest> OnMultiplayerJoinLobbyAsServerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<JoinLobbyAsServerResult> OnMultiplayerJoinLobbyAsServerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<JoinMatchmakingTicketRequest> OnMultiplayerJoinMatchmakingTicketRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<JoinMatchmakingTicketResult> OnMultiplayerJoinMatchmakingTicketResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LeaveLobbyRequest> OnMultiplayerLeaveLobbyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LobbyEmptyResult> OnMultiplayerLeaveLobbyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<LeaveLobbyAsServerRequest> OnMultiplayerLeaveLobbyAsServerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LobbyEmptyResult> OnMultiplayerLeaveLobbyAsServerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListMultiplayerServersRequest> OnMultiplayerListArchivedMultiplayerServersRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListMultiplayerServersResponse> OnMultiplayerListArchivedMultiplayerServersResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListAssetSummariesRequest> OnMultiplayerListAssetSummariesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListAssetSummariesResponse> OnMultiplayerListAssetSummariesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListBuildAliasesRequest> OnMultiplayerListBuildAliasesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListBuildAliasesResponse> OnMultiplayerListBuildAliasesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListBuildSummariesRequest> OnMultiplayerListBuildSummariesV2RequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListBuildSummariesResponse> OnMultiplayerListBuildSummariesV2ResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListCertificateSummariesRequest> OnMultiplayerListCertificateSummariesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListCertificateSummariesResponse> OnMultiplayerListCertificateSummariesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListContainerImagesRequest> OnMultiplayerListContainerImagesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListContainerImagesResponse> OnMultiplayerListContainerImagesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListContainerImageTagsRequest> OnMultiplayerListContainerImageTagsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListContainerImageTagsResponse> OnMultiplayerListContainerImageTagsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListMatchmakingQueuesRequest> OnMultiplayerListMatchmakingQueuesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListMatchmakingQueuesResult> OnMultiplayerListMatchmakingQueuesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListMatchmakingTicketsForPlayerRequest> OnMultiplayerListMatchmakingTicketsForPlayerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListMatchmakingTicketsForPlayerResult> OnMultiplayerListMatchmakingTicketsForPlayerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListMultiplayerServersRequest> OnMultiplayerListMultiplayerServersRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListMultiplayerServersResponse> OnMultiplayerListMultiplayerServersResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListPartyQosServersRequest> OnMultiplayerListPartyQosServersRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListPartyQosServersResponse> OnMultiplayerListPartyQosServersResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListQosServersForTitleRequest> OnMultiplayerListQosServersForTitleRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListQosServersForTitleResponse> OnMultiplayerListQosServersForTitleResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListSecretSummariesRequest> OnMultiplayerListSecretSummariesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListSecretSummariesResponse> OnMultiplayerListSecretSummariesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListServerBackfillTicketsForPlayerRequest> OnMultiplayerListServerBackfillTicketsForPlayerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListServerBackfillTicketsForPlayerResult> OnMultiplayerListServerBackfillTicketsForPlayerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListTitleMultiplayerServersQuotaChangesRequest> OnMultiplayerListTitleMultiplayerServersQuotaChangesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListTitleMultiplayerServersQuotaChangesResponse> OnMultiplayerListTitleMultiplayerServersQuotaChangesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListVirtualMachineSummariesRequest> OnMultiplayerListVirtualMachineSummariesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListVirtualMachineSummariesResponse> OnMultiplayerListVirtualMachineSummariesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RemoveMatchmakingQueueRequest> OnMultiplayerRemoveMatchmakingQueueRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RemoveMatchmakingQueueResult> OnMultiplayerRemoveMatchmakingQueueResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RemoveMemberFromLobbyRequest> OnMultiplayerRemoveMemberRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LobbyEmptyResult> OnMultiplayerRemoveMemberResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RequestMultiplayerServerRequest> OnMultiplayerRequestMultiplayerServerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RequestMultiplayerServerResponse> OnMultiplayerRequestMultiplayerServerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RequestPartyServiceRequest> OnMultiplayerRequestPartyServiceRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RequestPartyServiceResponse> OnMultiplayerRequestPartyServiceResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<RolloverContainerRegistryCredentialsRequest> OnMultiplayerRolloverContainerRegistryCredentialsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<RolloverContainerRegistryCredentialsResponse> OnMultiplayerRolloverContainerRegistryCredentialsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SetMatchmakingQueueRequest> OnMultiplayerSetMatchmakingQueueRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SetMatchmakingQueueResult> OnMultiplayerSetMatchmakingQueueResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ShutdownMultiplayerServerRequest> OnMultiplayerShutdownMultiplayerServerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerShutdownMultiplayerServerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SubscribeToLobbyResourceRequest> OnMultiplayerSubscribeToLobbyResourceRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SubscribeToLobbyResourceResult> OnMultiplayerSubscribeToLobbyResourceResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SubscribeToMatchResourceRequest> OnMultiplayerSubscribeToMatchmakingResourceRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SubscribeToMatchResourceResult> OnMultiplayerSubscribeToMatchmakingResourceResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnsubscribeFromLobbyResourceRequest> OnMultiplayerUnsubscribeFromLobbyResourceRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LobbyEmptyResult> OnMultiplayerUnsubscribeFromLobbyResourceResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnsubscribeFromMatchResourceRequest> OnMultiplayerUnsubscribeFromMatchmakingResourceRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UnsubscribeFromMatchResourceResult> OnMultiplayerUnsubscribeFromMatchmakingResourceResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UntagContainerImageRequest> OnMultiplayerUntagContainerImageRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerUntagContainerImageResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateBuildAliasRequest> OnMultiplayerUpdateBuildAliasRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<BuildAliasDetailsResponse> OnMultiplayerUpdateBuildAliasResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateBuildNameRequest> OnMultiplayerUpdateBuildNameRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerUpdateBuildNameResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateBuildRegionRequest> OnMultiplayerUpdateBuildRegionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerUpdateBuildRegionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateBuildRegionsRequest> OnMultiplayerUpdateBuildRegionsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerUpdateBuildRegionsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateLobbyRequest> OnMultiplayerUpdateLobbyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LobbyEmptyResult> OnMultiplayerUpdateLobbyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateLobbyAsServerRequest> OnMultiplayerUpdateLobbyAsServerRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<LobbyEmptyResult> OnMultiplayerUpdateLobbyAsServerResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UploadCertificateRequest> OnMultiplayerUploadCertificateRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerUploadCertificateResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UploadSecretRequest> OnMultiplayerUploadSecretRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerUploadSecretResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetGlobalPolicyRequest> OnProfilesGetGlobalPolicyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetGlobalPolicyResponse> OnProfilesGetGlobalPolicyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetEntityProfileRequest> OnProfilesGetProfileRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetEntityProfileResponse> OnProfilesGetProfileResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetEntityProfilesRequest> OnProfilesGetProfilesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetEntityProfilesResponse> OnProfilesGetProfilesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTitlePlayersFromMasterPlayerAccountIdsRequest> OnProfilesGetTitlePlayersFromMasterPlayerAccountIdsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTitlePlayersFromMasterPlayerAccountIdsResponse> OnProfilesGetTitlePlayersFromMasterPlayerAccountIdsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetTitlePlayersFromXboxLiveIDsRequest> OnProfilesGetTitlePlayersFromXboxLiveIDsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetTitlePlayersFromProviderIDsResponse> OnProfilesGetTitlePlayersFromXboxLiveIDsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SetDisplayNameRequest> OnProfilesSetDisplayNameRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SetDisplayNameResponse> OnProfilesSetDisplayNameResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SetGlobalPolicyRequest> OnProfilesSetGlobalPolicyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SetGlobalPolicyResponse> OnProfilesSetGlobalPolicyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SetProfileLanguageRequest> OnProfilesSetProfileLanguageRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SetProfileLanguageResponse> OnProfilesSetProfileLanguageResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<SetEntityProfilePolicyRequest> OnProfilesSetProfilePolicyRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<SetEntityProfilePolicyResponse> OnProfilesSetProfilePolicyResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateLeaderboardDefinitionRequest> OnProgressionCreateLeaderboardDefinitionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> OnProgressionCreateLeaderboardDefinitionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<CreateStatisticDefinitionRequest> OnProgressionCreateStatisticDefinitionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> OnProgressionCreateStatisticDefinitionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteLeaderboardDefinitionRequest> OnProgressionDeleteLeaderboardDefinitionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> OnProgressionDeleteLeaderboardDefinitionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteLeaderboardEntriesRequest> OnProgressionDeleteLeaderboardEntriesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> OnProgressionDeleteLeaderboardEntriesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteStatisticDefinitionRequest> OnProgressionDeleteStatisticDefinitionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> OnProgressionDeleteStatisticDefinitionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<DeleteStatisticsRequest> OnProgressionDeleteStatisticsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<DeleteStatisticsResponse> OnProgressionDeleteStatisticsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetFriendLeaderboardForEntityRequest> OnProgressionGetFriendLeaderboardForEntityRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetEntityLeaderboardResponse> OnProgressionGetFriendLeaderboardForEntityResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetEntityLeaderboardRequest> OnProgressionGetLeaderboardRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetEntityLeaderboardResponse> OnProgressionGetLeaderboardResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetLeaderboardAroundEntityRequest> OnProgressionGetLeaderboardAroundEntityRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetEntityLeaderboardResponse> OnProgressionGetLeaderboardAroundEntityResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetLeaderboardDefinitionRequest> OnProgressionGetLeaderboardDefinitionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetLeaderboardDefinitionResponse> OnProgressionGetLeaderboardDefinitionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetLeaderboardForEntitiesRequest> OnProgressionGetLeaderboardForEntitiesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetEntityLeaderboardResponse> OnProgressionGetLeaderboardForEntitiesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetStatisticDefinitionRequest> OnProgressionGetStatisticDefinitionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetStatisticDefinitionResponse> OnProgressionGetStatisticDefinitionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetStatisticsRequest> OnProgressionGetStatisticsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetStatisticsResponse> OnProgressionGetStatisticsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<GetStatisticsForEntitiesRequest> OnProgressionGetStatisticsForEntitiesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<GetStatisticsForEntitiesResponse> OnProgressionGetStatisticsForEntitiesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<IncrementLeaderboardVersionRequest> OnProgressionIncrementLeaderboardVersionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<IncrementLeaderboardVersionResponse> OnProgressionIncrementLeaderboardVersionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<IncrementStatisticVersionRequest> OnProgressionIncrementStatisticVersionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<IncrementStatisticVersionResponse> OnProgressionIncrementStatisticVersionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListLeaderboardDefinitionsRequest> OnProgressionListLeaderboardDefinitionsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListLeaderboardDefinitionsResponse> OnProgressionListLeaderboardDefinitionsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<ListStatisticDefinitionsRequest> OnProgressionListStatisticDefinitionsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<ListStatisticDefinitionsResponse> OnProgressionListStatisticDefinitionsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkAggregationSourceFromStatisticRequest> OnProgressionUnlinkAggregationSourceFromStatisticRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> OnProgressionUnlinkAggregationSourceFromStatisticResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UnlinkLeaderboardFromStatisticRequest> OnProgressionUnlinkLeaderboardFromStatisticRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> OnProgressionUnlinkLeaderboardFromStatisticResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateLeaderboardDefinitionRequest> OnProgressionUpdateLeaderboardDefinitionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> OnProgressionUpdateLeaderboardDefinitionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateLeaderboardEntriesRequest> OnProgressionUpdateLeaderboardEntriesRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> OnProgressionUpdateLeaderboardEntriesResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateStatisticDefinitionRequest> OnProgressionUpdateStatisticDefinitionRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<PlayFab.ProgressionModels.EmptyResponse> OnProgressionUpdateStatisticDefinitionResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabRequestEvent<UpdateStatisticsRequest> OnProgressionUpdateStatisticsRequestEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabResultEvent<UpdateStatisticsResponse> OnProgressionUpdateStatisticsResultEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		public event PlayFabErrorEvent OnGlobalErrorEvent
		{
			[CompilerGenerated]
			add
			{
			}
			[CompilerGenerated]
			remove
			{
			}
		}

		private PlayFabEvents()
		{
		}

		public static PlayFabEvents Init()
		{
			return null;
		}

		public void UnregisterInstance(object instance)
		{
		}

		private void OnProcessingErrorEvent(PlayFabRequestCommon request, PlayFabError error)
		{
		}

		private void OnProcessingEvent(ApiProcessingEventArgs e)
		{
		}
	}
}
