using Microsoft.Extensions.Options;
using TraderBridge.Config;
using TraderBridge.Interfaces;
using TraderBridge.Services;
using TraderBridge.Workers;

namespace TraderBridge.Handlers.Response
{


    public abstract class ResponseHandler :IResponseHandler
    {
        protected IResponseHandler? _next;
        protected ILogger<IbkrWorker> _logger;
        protected IApiMessenger _apiMessenger;
        protected IBKRSettings _bridgeSettings;

        public ResponseHandler _middleReponseHandler { get; set; }

        public abstract Task Handle(string jsonResponse, ResponseHandler? middleWorkHandler = null);

        public void SetNext(IResponseHandler handler)
        {
            _next = handler;
        }

        public ResponseHandler(ILogger<IbkrWorker> logger, ApiMessenger messenger, IOptions<IBKRSettings> bridgeSettings) { 
            _logger = logger;
            _apiMessenger = messenger;
            _bridgeSettings= bridgeSettings.Value;
        }
    }
}

