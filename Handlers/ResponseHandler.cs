using IBKR_Service.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace IBKR_Service.Handlers
{


    public abstract class ResponseHandler
    {
        protected ResponseHandler? _next;
        protected ILogger<ResponseHandler> _logger;
        protected ApiMessenger _messenger;


        public abstract void Handle(string jsonResponse);

        public void SetNext(ResponseHandler handler)
        {
            _next = handler;
        }

        public ResponseHandler(ILogger<ResponseHandler> logger, ApiMessenger messenger) { 
            _logger = logger;
            _messenger = messenger;
        }
    }
}

