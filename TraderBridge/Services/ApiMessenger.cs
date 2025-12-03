using Microsoft.Extensions.Options;
using System.Text;
using TraderBridge.Config;
using TraderBridge.Interfaces;

namespace TraderBridge.Services
{
    public class ApiMessenger : IApiMessenger
    {
        private readonly HttpClient _httpClient;
        private readonly IBKRSettings _bridgeSettings;

        public ApiMessenger(IOptions<IBKRSettings> bridgeSettings)
        {
            _bridgeSettings = bridgeSettings.Value;
            _httpClient = SetHttpClient();
        }

        private HttpClient SetHttpClient()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            var httpClient = new HttpClient(httpClientHandler);
            httpClient.BaseAddress = new Uri(_bridgeSettings.GatewayV1Api);

            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Console");
            //httpClient.DefaultRequestHeaders.Add("x-sess-uuid", "0.cb2f1402.1762295566.75ba690a");
            //httpClient.DefaultRequestHeaders.Add("Cookie", "x-sess-uuid=0.cb2f1402.1762295566.75ba690a");
            return httpClient;
        }

        public async Task<string> PostAsyncJsonResponse(string url, string jsonBody)
        {

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseJson = await response.Content.ReadAsStringAsync();
            return responseJson;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, string jsonBody)
        {
            try
            {
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                return response;
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                return response;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
