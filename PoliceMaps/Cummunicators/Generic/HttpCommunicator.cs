using Newtonsoft.Json;
using RestSharp;
using System.Runtime;

namespace PoliceMaps.Cummunicators.Generic
{
    public class HttpCommunicator
    {
        protected RestClient _client;

        public HttpCommunicator(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }

        protected async Task<RestResponse?> ExecuteRequest(Method method, string resource, Func<RestRequest, RestRequest>? authenticator,
            object? body = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null)
        {
            var request = new RestRequest(resource, method);
            if (body != null)
            {
                if (body is string)
                {
                    request.AddStringBody((string)body, DataFormat.Json);
                }
                else
                {
                    request.AddJsonBody(body);
                }
            }

            if (queryParams != null)
            {
                foreach (var param in queryParams)
                {
                    request.AddQueryParameter(param.Key, param.Value);
                }
            }

            if (headers != null)
            {
                request.AddHeaders(headers);
            }

            if (authenticator != null)
            {
                authenticator(request);
            }

            return await _client.ExecuteAsync(request);
        }

        protected async Task<TResponse?> ExecuteRequest<TResponse>(Method method, string resource, Func<RestRequest, RestRequest>? authenticator,
            object? body = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null)
        {
            var response = await ExecuteRequest(method, resource, authenticator, body, queryParams, headers);

            if (response == null || response.Content == null)
            {
                return default;
            }

            try
            {
                return JsonConvert.DeserializeObject<TResponse>(response.Content);
            }
            catch (Exception) { }
            try
            {
                return (TResponse)Convert.ChangeType(response.Content, typeof(TResponse));
            }
            catch (Exception) { Console.WriteLine("Error: GenericHttpClient.ExecuteRequest: Deserialization failed."); }

            return default;
        }
    }
}
