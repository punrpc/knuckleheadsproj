using System;
using System.Collections.Generic;
using PlayFab.ProgressionModels;

namespace PlayFab
{
	public static class PlayFabProgressionAPI
	{
		static PlayFabProgressionAPI()
		{
		}

		public static bool IsEntityLoggedIn()
		{
			return false;
		}

		public static void ForgetAllCredentials()
		{
		}

		public static void CreateLeaderboardDefinition(CreateLeaderboardDefinitionRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void CreateStatisticDefinition(CreateStatisticDefinitionRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void DeleteLeaderboardDefinition(DeleteLeaderboardDefinitionRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void DeleteLeaderboardEntries(DeleteLeaderboardEntriesRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void DeleteStatisticDefinition(DeleteStatisticDefinitionRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void DeleteStatistics(DeleteStatisticsRequest request, Action<DeleteStatisticsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void GetFriendLeaderboardForEntity(GetFriendLeaderboardForEntityRequest request, Action<GetEntityLeaderboardResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void GetLeaderboard(GetEntityLeaderboardRequest request, Action<GetEntityLeaderboardResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void GetLeaderboardAroundEntity(GetLeaderboardAroundEntityRequest request, Action<GetEntityLeaderboardResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void GetLeaderboardDefinition(GetLeaderboardDefinitionRequest request, Action<GetLeaderboardDefinitionResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void GetLeaderboardForEntities(GetLeaderboardForEntitiesRequest request, Action<GetEntityLeaderboardResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void GetStatisticDefinition(GetStatisticDefinitionRequest request, Action<GetStatisticDefinitionResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void GetStatistics(GetStatisticsRequest request, Action<GetStatisticsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void GetStatisticsForEntities(GetStatisticsForEntitiesRequest request, Action<GetStatisticsForEntitiesResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void IncrementLeaderboardVersion(IncrementLeaderboardVersionRequest request, Action<IncrementLeaderboardVersionResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void IncrementStatisticVersion(IncrementStatisticVersionRequest request, Action<IncrementStatisticVersionResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void ListLeaderboardDefinitions(ListLeaderboardDefinitionsRequest request, Action<ListLeaderboardDefinitionsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void ListStatisticDefinitions(ListStatisticDefinitionsRequest request, Action<ListStatisticDefinitionsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void UnlinkAggregationSourceFromStatistic(UnlinkAggregationSourceFromStatisticRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void UnlinkLeaderboardFromStatistic(UnlinkLeaderboardFromStatisticRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void UpdateLeaderboardDefinition(UpdateLeaderboardDefinitionRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void UpdateLeaderboardEntries(UpdateLeaderboardEntriesRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void UpdateStatisticDefinition(UpdateStatisticDefinitionRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void UpdateStatistics(UpdateStatisticsRequest request, Action<UpdateStatisticsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}
	}
}
