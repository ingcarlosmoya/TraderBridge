using IBKR_Service.Config;
using Newtonsoft.Json;

namespace IBKR_Service.Handlers
{
    public class SuccessfulResponseHanlder : ResponseHandler
    {
        public SuccessfulResponseHanlder(ILogger<ResponseHandler> logger, ApiMessenger messenger) : base(logger, messenger)
        {
        }

        public override void Handle(string jsonResponse)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<List<SuccessfulResponse>>(jsonResponse);
                if (response != null && response.Any() && !string.IsNullOrEmpty(response.FirstOrDefault().order_id))
                    _logger.LogInformation($"Order created, ID: {response.FirstOrDefault().order_id}");
                else {
                    _next.Handle(jsonResponse);
                }

            }
            catch (Exception)
            {
                if (_next != null)
                    _next.Handle(jsonResponse);
            }
        }
    }
}

