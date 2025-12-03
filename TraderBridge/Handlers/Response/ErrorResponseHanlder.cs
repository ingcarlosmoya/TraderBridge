using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TraderBridge.Config;
using TraderBridge.Models.Response;
using TraderBridge.Services;
using TraderBridge.Workers;

namespace TraderBridge.Handlers.Response
{
    public class ErrorResponseHandler : ResponseHandler
    {
        public ErrorResponseHandler(ILogger<IbkrWorker> logger, ApiMessenger messenger, IOptions<IBKRSettings> bridgeSettings) : base(logger, messenger, bridgeSettings)
        {
        }

        public override Task Handle(string jsonResponse, ResponseHandler? middleWorkHandler = null)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<ErrorResponse>(jsonResponse);
                if (response != null)
                    _logger.LogError($"Order not created, error: {response.Error}");

            }
            catch (Exception)
            {
                if (_next != null)
                    _next.Handle(jsonResponse);
            }
            return Task.CompletedTask;
        }
    }
}

