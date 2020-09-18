using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ViiaNordic.Services.Interfaces
{
    public interface IHttpClientHandler
    {
        Task<string> GetStringAsync(string url);
        Task<T> GetJsonAsync<T>(string uri) where T : new();
        Task<string> PostAsync(string uri, HttpContent httpContent);
        Task<string> PostJsonAsync(string uri, object body, Dictionary<string, string> headers);
    }
}
