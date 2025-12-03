using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using TraderBridge.Interfaces;
using TraderBridge.Workers;

namespace TraderBridgeTest.Workers
{
    public class ManagerWorkerTest
    {
        private Mock<ILogger<BackgroundService>> _logger;
        private Mock<IMt5Listener> _mt5Listener;
        private Mock<IIbkrClient> _ibkrClient;
        private Mock<CancellationTokenSource> _cancellationTokenSource;
        private ManagerWorker _managerWorker;
        Mock<IIbkrWorker> worker;

        public ManagerWorkerTest()
        {
            _logger = new Mock<ILogger<BackgroundService>>();
            _mt5Listener = new Mock<IMt5Listener>();
            _ibkrClient = new Mock<IIbkrClient>();
            _cancellationTokenSource = new Mock<CancellationTokenSource>();
            worker = new Mock<IIbkrWorker>();
            _managerWorker = new ManagerWorker(_logger.Object, _mt5Listener.Object, worker.Object);

        }


        [Fact]
        public async Task StartAsync_WhenMt5ReaderIsReady_ThenRunsMt5Worker()
        {
            //Given
            _mt5Listener.Setup(x => x.StartOrdersReader()).Returns(Task.FromResult(true));

            //When
            await _managerWorker.StartAsync(_cancellationTokenSource.Object.Token);

            //Then
            _mt5Listener.Verify(x => x.StartOrdersReader(), Times.AtLeastOnce);
            worker.Verify(x => x.ExecuteSync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);

        }

        [Fact]
        public async Task StartAsync_WhenMt5ReaderIsNotReady_ThenDoesNotRunsMt5Worker()
        {
            //Given

            //When
            await _managerWorker.StartAsync(_cancellationTokenSource.Object.Token);

            //Then
            _mt5Listener.Verify(x => x.StartOrdersReader(), Times.AtLeast(30));
            worker.Verify(x => x.ExecuteSync(It.IsAny<CancellationToken>()), Times.Never);

        }

    }
}