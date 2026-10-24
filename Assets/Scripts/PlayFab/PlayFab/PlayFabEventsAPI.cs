using System;
using System.Collections.Generic;
using PlayFab.EventsModels;

namespace PlayFab
{
	public static class PlayFabEventsAPI
	{
		static PlayFabEventsAPI()
		{
		}

		public static bool IsEntityLoggedIn()
		{
			return false;
		}

		public static void ForgetAllCredentials()
		{
		}

		public static void CreateTelemetryKey(CreateTelemetryKeyRequest request, Action<CreateTelemetryKeyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void DeleteDataConnection(DeleteDataConnectionRequest request, Action<DeleteDataConnectionResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void DeleteTelemetryKey(DeleteTelemetryKeyRequest request, Action<DeleteTelemetryKeyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void GetDataConnection(GetDataConnectionRequest request, Action<GetDataConnectionResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void GetTelemetryKey(GetTelemetryKeyRequest request, Action<GetTelemetryKeyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void ListDataConnections(ListDataConnectionsRequest request, Action<ListDataConnectionsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void ListTelemetryKeys(ListTelemetryKeysRequest request, Action<ListTelemetryKeysResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void SetDataConnection(SetDataConnectionRequest request, Action<SetDataConnectionResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void SetDataConnectionActive(SetDataConnectionActiveRequest request, Action<SetDataConnectionActiveResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void SetTelemetryKeyActive(SetTelemetryKeyActiveRequest request, Action<SetTelemetryKeyActiveResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void WriteEvents(WriteEventsRequest request, Action<WriteEventsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void WriteTelemetryEvents(WriteEventsRequest request, Action<WriteEventsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public static void WriteTelemetryEventsWithTelemetryKey(WriteEventsRequest request, Action<WriteEventsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}
	}
}
