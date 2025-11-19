using IBKR_Service.Config;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace IBKR_Service.Handlers
{
    public class ConfirmationResponseHanlder : ResponseHandler
    {
        string baseUrl = "https://localhost:5000/v1/api";

        public ConfirmationResponseHanlder(ILogger<ResponseHandler> logger, ApiMessenger messenger) : base(logger, messenger)
        {
        }



        //public ConfirmationResponseHanlder(ILogger<ResponseHandler> logger, ApiMessenger messenger) : base(messenger)
        //{
        //    _logger = logger;
        //}


        public override async void Handle(string jsonResponse)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<List<ConfirmationResponse>>(jsonResponse);
                if (response != null && response.Any())
                {
                    var confirmation = response.FirstOrDefault();
                    if (confirmation != null && confirmation.message != null && confirmation.message.Any())
                    {
                        _logger.LogInformation(string.Join(",", confirmation.message));
                        if (!string.IsNullOrEmpty(confirmation.id))
                        {
                            var replyRequest = new ReplyRequest { confirmed = true };
                            var jsonRequest = JsonConvert.SerializeObject(replyRequest);
                            var replyResponse = await _messenger.PostAsync($"{baseUrl}/iserver/reply/{confirmation.id}", jsonRequest);

                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var _httpClient = SetHttpClient();
                            //var response2 = await _httpClient.PostAsync($"{baseUrl}/iserver/reply/{confirmation.id}", content);
                            //var responseBody = await replyResponse.Content.ReadAsStringAsync();
                            var succesfulResponseHandler = new SuccessfulResponseHanlder(_logger, _messenger);
                            succesfulResponseHandler.Handle(replyResponse);
                        }
                    }
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

