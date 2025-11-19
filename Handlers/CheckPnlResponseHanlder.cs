using IBKR_Service.Config;
using Newtonsoft.Json.Linq;

namespace IBKR_Service.Handlers
{
    public class CheckPnlResponseHanlder : ResponseHandler
    {
        public CheckPnlResponseHanlder(ILogger<ResponseHandler> logger, ApiMessenger messenger) : base(logger, messenger)
        {
        }

        public override void Handle(string jsonResponse)
        {
            try
            {
                var response = JObject.Parse(jsonResponse);
                string dpl = "$0";
                if (response["upnl"] != null && response["upnl"]["DUN296642.Core"] != null && response["upnl"]["DUN296642.Core"]["dpl"] != null)
                {
                    dpl = (string)response["upnl"]["DUN296642.Core"]["dpl"];
                    _logger.LogInformation($">>> Pnl: {dpl}");
                    //var responseLog = ($"Response for tickle endpoint: {response.StatusCode}");
                    //if (!response.IsSuccessStatusCode)
                    //    _logger.LogError(responseLog);
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

