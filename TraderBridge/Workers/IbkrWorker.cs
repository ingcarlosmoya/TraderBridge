using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Threading;
using TraderBridge.Config;
using TraderBridge.Interfaces;
using TraderBridge.Services;

namespace TraderBridge.Workers
{
    public interface IIbkrWorker
    {
        bool KeepCheckingTickleAndPnl { get; set; }
        Task ExecuteSync(CancellationToken stoppingToken);
    }
    public class IbkrWorker : IIbkrWorker
    {
        private readonly ILogger<BackgroundService> _logger;
        private readonly IIbkrClient _ibkrClient;
        public int StartClientPortalMaxRetries { get; set; } = 5;
        public bool IsStartClientPortalStarted { get; set; }
        public bool KeepCheckingTickleAndPnl { get; set; } = true;

        public IbkrWorker(IIbkrClient ibkrClient, ILogger<BackgroundService> logger)
        {
            _logger = logger;
            _ibkrClient = ibkrClient;
        }


        public async Task ExecuteSync(CancellationToken stoppingToken)
        {
            var startClientPortalCurrentAttempt = 0;

            while (!IsStartClientPortalStarted && startClientPortalCurrentAttempt < StartClientPortalMaxRetries)
            {
                IsStartClientPortalStarted = await _ibkrClient.StartClientPortal();
                startClientPortalCurrentAttempt++;
            }

            if (IsStartClientPortalStarted)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await _ibkrClient.Tickle();
                    if (_ibkrClient.IsConnected)
                        await _ibkrClient.CheckPnl();
                    await Task.Delay(500);
                    if (!KeepCheckingTickleAndPnl)
                        break;
                }
            }
        }
    }
}
