using System;
using System.Text;
using System.Threading.Tasks;

namespace IBKR_Service.Config
{
    public class IbkrSettings
    {
        public string GatewayUrl { get; set; } = string.Empty;
        public string SessionCookie { get; set; } = string.Empty;
    }

    public interface IApiMessenger {
        Task<string> PostAsync(string url, string jsonBody);
        Task<string> GetAsync(string url);
    }

    public class ApiMessenger : IApiMessenger { 
        private HttpClient _httpClient;
        private readonly string baseUrl = "https://localhost:5000/v1/api";

        public ApiMessenger()
        {
            _httpClient = SetHttpClient();
        }

        private HttpClient SetHttpClient()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            var httpClient = new HttpClient(httpClientHandler);
            httpClient.BaseAddress = new Uri(baseUrl);

            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Console");
            httpClient.DefaultRequestHeaders.Add("x-sess-uuid", "0.cb2f1402.1762295566.75ba690a");
            httpClient.DefaultRequestHeaders.Add("Cookie", "x-sess-uuid=0.cb2f1402.1762295566.75ba690a");
            return httpClient;
        }

        public async Task<string> PostAsync(string url, string jsonBody) {

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseJson = await response.Content.ReadAsStringAsync();
            return responseJson;
        }

        public async Task<string> GetAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            var responseJson = await response.Content.ReadAsStringAsync();
            return responseJson;
        }
    }
}
