using Microsoft.Extensions.Options;
using System.Text.Json;
using TraderBridge.Config;
using TraderBridge.Models.Response;
using TraderBridge.Services;
using TraderBridge.Workers;

namespace TraderBridge.Handlers.Response
{
    public class ConfirmationResponseHandler : ResponseHandler
    {

        public ConfirmationResponseHandler(ILogger<IbkrWorker> logger, ApiMessenger messenger, IOptions<IBKRSettings> bridgeSettings) : base(logger, messenger, bridgeSettings)
        {
            
        }

        public override async Task Handle(string jsonResponse, ResponseHandler? middleWorkHandler)
        {
            try
            {
                var response = JsonSerializer.Deserialize<List<ConfirmationResponse>>(jsonResponse);
                if (response != null && response.Any())
                {
                    var confirmation = response.FirstOrDefault();
                    if (confirmation != null && confirmation.Message != null && confirmation.Message.Any())
                    {
                        _logger.LogInformation(string.Join(",", confirmation.Message));
                        if (!string.IsNullOrEmpty(confirmation.Id))
                        {
                            var replyRequest = new ReplyRequest { Confirmed = true };
                            var options = new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = new LowerCaseNamingPolicy(),
                            };
                            var jsonRequest = JsonSerializer.Serialize(replyRequest, options);
                            var replyResponse = await _apiMessenger.PostAsyncJsonResponse($"{_bridgeSettings.GatewayV1Api}/iserver/reply/{confirmation.Id}", jsonRequest);
                            if (_middleReponseHandler != null)
                                await _middleReponseHandler.Handle(replyResponse, _middleReponseHandler);
                        }
                    }
                }

            }
            catch (Exception)
            {
                if (_next != null)
                    await _next.Handle(jsonResponse);
            }
        }
    }
}

