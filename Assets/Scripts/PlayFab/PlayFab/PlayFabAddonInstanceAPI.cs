using System;
using System.Collections.Generic;
using PlayFab.AddonModels;
using PlayFab.SharedModels;

namespace PlayFab
{
	public class PlayFabAddonInstanceAPI : IPlayFabInstanceApi
	{
		public readonly PlayFabApiSettings apiSettings;

		public readonly PlayFabAuthenticationContext authenticationContext;

		public PlayFabAddonInstanceAPI(PlayFabAuthenticationContext context)
		{
		}

		public PlayFabAddonInstanceAPI(PlayFabApiSettings settings, PlayFabAuthenticationContext context)
		{
		}

		public bool IsEntityLoggedIn()
		{
			return false;
		}

		public void ForgetAllCredentials()
		{
		}

		public void CreateOrUpdateApple(CreateOrUpdateAppleRequest request, Action<CreateOrUpdateAppleResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void CreateOrUpdateFacebook(CreateOrUpdateFacebookRequest request, Action<CreateOrUpdateFacebookResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void CreateOrUpdateFacebookInstantGames(CreateOrUpdateFacebookInstantGamesRequest request, Action<CreateOrUpdateFacebookInstantGamesResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void CreateOrUpdateGoogle(CreateOrUpdateGoogleRequest request, Action<CreateOrUpdateGoogleResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void CreateOrUpdateKongregate(CreateOrUpdateKongregateRequest request, Action<CreateOrUpdateKongregateResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void CreateOrUpdateNintendo(CreateOrUpdateNintendoRequest request, Action<CreateOrUpdateNintendoResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void CreateOrUpdatePSN(CreateOrUpdatePSNRequest request, Action<CreateOrUpdatePSNResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void CreateOrUpdateSteam(CreateOrUpdateSteamRequest request, Action<CreateOrUpdateSteamResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void CreateOrUpdateToxMod(CreateOrUpdateToxModRequest request, Action<CreateOrUpdateToxModResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void CreateOrUpdateTwitch(CreateOrUpdateTwitchRequest request, Action<CreateOrUpdateTwitchResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void DeleteApple(DeleteAppleRequest request, Action<DeleteAppleResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void DeleteFacebook(DeleteFacebookRequest request, Action<DeleteFacebookResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void DeleteFacebookInstantGames(DeleteFacebookInstantGamesRequest request, Action<DeleteFacebookInstantGamesResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void DeleteGoogle(DeleteGoogleRequest request, Action<DeleteGoogleResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void DeleteKongregate(DeleteKongregateRequest request, Action<DeleteKongregateResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void DeleteNintendo(DeleteNintendoRequest request, Action<DeleteNintendoResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void DeletePSN(DeletePSNRequest request, Action<DeletePSNResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void DeleteSteam(DeleteSteamRequest request, Action<DeleteSteamResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void DeleteToxMod(DeleteToxModRequest request, Action<DeleteToxModResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void DeleteTwitch(DeleteTwitchRequest request, Action<DeleteTwitchResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void GetApple(GetAppleRequest request, Action<GetAppleResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void GetFacebook(GetFacebookRequest request, Action<GetFacebookResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void GetFacebookInstantGames(GetFacebookInstantGamesRequest request, Action<GetFacebookInstantGamesResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void GetGoogle(GetGoogleRequest request, Action<GetGoogleResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void GetKongregate(GetKongregateRequest request, Action<GetKongregateResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void GetNintendo(GetNintendoRequest request, Action<GetNintendoResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void GetPSN(GetPSNRequest request, Action<GetPSNResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void GetSteam(GetSteamRequest request, Action<GetSteamResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void GetToxMod(GetToxModRequest request, Action<GetToxModResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void GetTwitch(GetTwitchRequest request, Action<GetTwitchResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}
	}
}
