using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TraderBridge.Config;
using TraderBridge.Interfaces;
using TraderBridge.Models;
using TraderBridge.Services;
using TraderBridge.Workers;

namespace TraderBridge.Handlers.Response
{
    public class SuccessfulResponseHandler : ResponseHandler, IResponseHandler
    {
        public ResponseHandler MiddleworkHandler { get; set; }
        public SuccessfulResponseHandler(ILogger<IbkrWorker> logger, ApiMessenger messenger, IOptions<IBKRSettings> bridgeSettings) : base(logger, messenger, bridgeSettings)
        {
        }

        public override Task Handle(string jsonResponse, ResponseHandler? middleWorkHandler = null)
        {
            try
            {
                var succesfulResponseList = JsonConvert.DeserializeObject<List<SuccessfulResponse>>(jsonResponse);
                if (succesfulResponseList != null && succesfulResponseList.Any() && !string.IsNullOrEmpty(succesfulResponseList.FirstOrDefault().Order_id))
                {
                    var succesfulResponse = succesfulResponseList.FirstOrDefault();
                    if (succesfulResponse != null && string.IsNullOrEmpty(succesfulResponse.Order_id))
                        _logger.LogInformation($"Order created, ID: {succesfulResponse.Order_id}");
                }
                else
                {
                    _next.Handle(jsonResponse, middleWorkHandler);
                }
            }
            catch (Exception)
            {
                if (_next != null)
                    _next.Handle(jsonResponse, this);
            }
            return Task.CompletedTask;
        }
    }
}

