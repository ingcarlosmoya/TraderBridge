using TraderBridge.Interfaces;

namespace TraderBridge.Workers
{
    public class ManagerWorker : BackgroundService
    {
        private readonly ILogger<BackgroundService> _logger;
        private readonly IMt5Listener _mt5Listener;
        private readonly IIbkrWorker _ibkrWorker;
        private int mt5ReaderMaxRetries = 30;

        public ManagerWorker(ILogger<BackgroundService> logger, IMt5Listener mt5Listener, IIbkrWorker ibkrWorker)
        {
            _mt5Listener = mt5Listener;
            _ibkrWorker = ibkrWorker;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (await StartMt5Reader())
                await _ibkrWorker.ExecuteSync(stoppingToken);
        }

        private async Task<bool> StartMt5Reader()
        {
            int currentMt5ReaderAttempt = 0;
            bool isMt5Ready = false;
            while (!isMt5Ready && currentMt5ReaderAttempt < mt5ReaderMaxRetries)
            {
                isMt5Ready = await _mt5Listener.StartOrdersReader();
                currentMt5ReaderAttempt++;
            }
            return isMt5Ready;
        }
    }
}
