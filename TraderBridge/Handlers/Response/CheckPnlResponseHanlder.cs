using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using TraderBridge.Config;
using TraderBridge.Services;
using TraderBridge.Workers;

namespace TraderBridge.Handlers.Response
{
    public class CheckPnlResponseHandler : ResponseHandler
    {
        public CheckPnlResponseHandler(ILogger<IbkrWorker> logger, ApiMessenger messenger, IOptions<IBKRSettings> bridgeSettings) : base(logger, messenger, bridgeSettings)
        {
        }

        public override Task Handle(string jsonResponse, ResponseHandler? middleWorkHandler = null)
        {
            try
            {
                var response = JObject.Parse(jsonResponse);
                string dpl = "$0";
                if (response["upnl"] != null && response["upnl"]["DUN296642.Core"] != null && response["upnl"]["DUN296642.Core"]["dpl"] != null)
                {
                    dpl = (string)response["upnl"]["DUN296642.Core"]["dpl"];
                    _logger.LogInformation($">>> Pnl: {dpl}");
                }

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

