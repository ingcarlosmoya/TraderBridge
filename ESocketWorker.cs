using IBApi;
using IBKR_Service.Config;
using IBKR_ServiceEWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBKR_Service
{
    internal class ESocketWorker : BackgroundService
    {

        private readonly ILogger<Worker> _logger;
        EWrapperImpl ibkrClient;

        public ESocketWorker(ILogger<Worker> logger)
        {
            _logger = logger;
            ibkrClient = new EWrapperImpl();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            

            while (!stoppingToken.IsCancellationRequested)
            {
                ConnectSocket();
                GetData();
                await Task.Delay(500, stoppingToken);
            }
        }

        private void ConnectSocket()
        {
            if (!ibkrClient.ClientSocket.IsConnected())
            {
                _logger.LogInformation("Trying to connect...");
                ibkrClient.ClientSocket.eConnect("", 5000, 0);
                ConnectSocket();
            }
            else
            {
                _logger.LogInformation("Connected!");
            }
        }

        private void GetData() {
            //Contract contract = new Contract();
            //var marketDataOptions = new List<TagValue>();

            //contract.Symbol = "AAPL";
            //contract.SecType = "STK";
            //contract.Exchange = "SMART";
            //contract.PrimaryExch = "ISLAND";
            //contract.Currency = "USD";
            //ibkrClient.ClientSocket.reqMktData(1, contract,string.Empty, false, false, marketDataOptions);
            ibkrClient.ClientSocket.reqPnL(1, "DUN296642", string.Empty);

        }

    }

}
