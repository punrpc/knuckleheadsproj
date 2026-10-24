using System;
using System.Collections.Generic;
using PlayFab.EventsModels;
using PlayFab.SharedModels;

namespace PlayFab
{
	public class PlayFabEventsInstanceAPI : IPlayFabInstanceApi
	{
		public readonly PlayFabApiSettings apiSettings;

		public readonly PlayFabAuthenticationContext authenticationContext;

		public PlayFabEventsInstanceAPI(PlayFabAuthenticationContext context)
		{
		}

		public PlayFabEventsInstanceAPI(PlayFabApiSettings settings, PlayFabAuthenticationContext context)
		{
		}

		public bool IsEntityLoggedIn()
		{
			return false;
		}

		public void ForgetAllCredentials()
		{
		}

		public void CreateTelemetryKey(CreateTelemetryKeyRequest request, Action<CreateTelemetryKeyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void DeleteDataConnection(DeleteDataConnectionRequest request, Action<DeleteDataConnectionResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void DeleteTelemetryKey(DeleteTelemetryKeyRequest request, Action<DeleteTelemetryKeyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void GetDataConnection(GetDataConnectionRequest request, Action<GetDataConnectionResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void GetTelemetryKey(GetTelemetryKeyRequest request, Action<GetTelemetryKeyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void ListDataConnections(ListDataConnectionsRequest request, Action<ListDataConnectionsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void ListTelemetryKeys(ListTelemetryKeysRequest request, Action<ListTelemetryKeysResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void SetDataConnection(SetDataConnectionRequest request, Action<SetDataConnectionResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void SetDataConnectionActive(SetDataConnectionActiveRequest request, Action<SetDataConnectionActiveResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void SetTelemetryKeyActive(SetTelemetryKeyActiveRequest request, Action<SetTelemetryKeyActiveResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void WriteEvents(WriteEventsRequest request, Action<WriteEventsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void WriteTelemetryEvents(WriteEventsRequest request, Action<WriteEventsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}

		public void WriteTelemetryEventsWithTelemetryKey(WriteEventsRequest request, Action<WriteEventsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
		}
	}
}
