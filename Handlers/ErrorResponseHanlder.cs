using IBKR_Service.Config;
using Newtonsoft.Json;

namespace IBKR_Service.Handlers
{
    public class ErrorResponseHanlder : ResponseHandler
    {
        public ErrorResponseHanlder(ILogger<ResponseHandler> logger, ApiMessenger messenger) : base(logger, messenger)
        {
        }

        public override void Handle(string jsonResponse)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<ErrorResponse>(jsonResponse);
                if (response != null)
                    _logger.LogError($"Order not created, error: {response.error}");

            }
            catch (Exception)
            {
                if (_next != null)
                    _next.Handle(jsonResponse);
            }
        }
    }
}

