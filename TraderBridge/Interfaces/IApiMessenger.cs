namespace TraderBridge.Interfaces
{
    public interface IApiMessenger {
        Task<string> PostAsyncJsonResponse(string url, string jsonBody);
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostAsync(string url, string jsonBody);
    }
}
