using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ViiaNordic.Services
{
    public class HttpClientHandler : IHttpHandler
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public Task<string> GetStringAsync(string uri)
        {
            return _httpClient.GetStringAsync(uri);
        }

        public async Task<T> GetJsonAsync<T>(string uri) where T : new()
        {
            try
            {
                var response = await _httpClient.GetAsync(uri);
                AssertStatusCode(response);
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async Task<byte[]> GetBytesAsync(string uri)
        {
            var response = await _httpClient.GetAsync(uri);
            AssertStatusCode(response);

            return await response.Content.ReadAsByteArrayAsync();
        }

        private static HttpContent GetJsonContent(object body)
        {
            var jsonBody = JsonConvert.SerializeObject(body);
            return new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        public async Task<string> PostAsync(string uri, HttpContent httpContent)
        {
            var requestContent = await httpContent.ReadAsStringAsync();
            var response = await _httpClient.PostAsync(uri, httpContent);
            AssertStatusCode(response, requestContent);
            return await response.Content.ReadAsStringAsync();
        }

        public Task PostJsonAsync(object body, string uri)
        {
            return PostAsync(uri, GetJsonContent(body));
        }

        public async Task<string> PostJsonAsync(string uri, object body, Dictionary<string, string> headers)
        {
            var httpContent = GetJsonContent(body);
            foreach (var header in headers)
            {
                httpContent.Headers.Add(header.Key, header.Value);
            }
            return await PostAsync(uri, httpContent);
        }

        public async Task<T> PostJsonAsync<T>(object body, string uri) where T : new()
        {
            var response = await PostAsync(uri, GetJsonContent(body));
            return JsonConvert.DeserializeObject<T>(response);
        }

        public Task PostAsync(string uri)
        {
            return PostAsync(uri, new StringContent(string.Empty));
        }

        public async Task<T> PostAsync<T>(string uri)
        {
            var response = await PostAsync(uri, new StringContent(string.Empty));
            return JsonConvert.DeserializeObject<T>(response);
        }


        private void AssertStatusCode(HttpResponseMessage response, string requestContent = "")
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception();
                }

                //gather error details
                var errorDetails = new StringBuilder();
                try
                {
                    errorDetails.AppendLine($"Request URL: {response.RequestMessage.RequestUri}");
                    errorDetails.AppendLine($"Request Content: {requestContent}");
                    errorDetails.AppendLine($"Response Message: {response.ReasonPhrase}");
                    errorDetails.AppendLine($"Response Content: {response.Content?.ReadAsStringAsync().Result ?? ""}");
                }
                catch
                {
                    // ignored
                }

                throw new Exception(errorDetails.ToString());
            }
        }
    }
}
