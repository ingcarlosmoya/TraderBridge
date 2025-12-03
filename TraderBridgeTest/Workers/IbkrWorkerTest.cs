using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using TraderBridge.Interfaces;
using TraderBridge.Workers;

namespace TraderBridgeTest.Workers
{
    public class IbkrWorkerTest
    {
        private Mock<ILogger<BackgroundService>> _logger;
        private Mock<IIbkrClient> _ibkrClient;
        private Mock<CancellationTokenSource> _cancellationTokenSource;
        IbkrWorker worker;

        public IbkrWorkerTest()
        {
            _logger = new Mock<ILogger<BackgroundService>>();
            _ibkrClient = new Mock<IIbkrClient>();
            _cancellationTokenSource = new Mock<CancellationTokenSource>();
            worker = new IbkrWorker(_ibkrClient.Object, _logger.Object);
        }


        [Fact]
        public async Task ExecuteSync_ClientPortalIsNotStarted_ThenDoesNotRunEitherTicklerAndCheckPnl()
        {
            //Given
            worker.StartClientPortalMaxRetries = 10;

            //When
            await worker.ExecuteSync(_cancellationTokenSource.Object.Token);

            //Then
            _ibkrClient.Verify(x => x.StartClientPortal(), Times.AtLeast(worker.StartClientPortalMaxRetries));
            _ibkrClient.Verify(x => x.Tickle(), Times.Never);
            _ibkrClient.Verify(x => x.Tickle(), Times.Never);

        }

        [Fact]
        public async Task ExecuteSync_ClientPortalIsStartedIbkrClentConnected_ThenRunsEitherTicklerAndCheckPnl()
        {
            //Given
            _ibkrClient.Setup(x => x.IsConnected).Returns(true);
            _ibkrClient.Setup(x => x.StartClientPortal()).Returns(Task.FromResult(true));
            worker.KeepCheckingTickleAndPnl = false;

            //When
            await worker.ExecuteSync(_cancellationTokenSource.Object.Token);
            _cancellationTokenSource.Object.Cancel();

            //Then
            _ibkrClient.Verify(x => x.StartClientPortal(), Times.Once);
            _ibkrClient.Verify(x => x.Tickle(), Times.Once);
            _ibkrClient.Verify(x => x.CheckPnl(), Times.Once);

        }

        [Fact]
        public async Task ExecuteSync_ClientPortalIsStartedButIbkrClientIsNotConnected_ThenRunsTicklerButNotCheckPnl()
        {
            //Given
            _ibkrClient.Setup(x => x.IsConnected).Returns(false);
            _ibkrClient.Setup(x => x.StartClientPortal()).Returns(Task.FromResult(true));
            worker.KeepCheckingTickleAndPnl = false;

            //When
            await worker.ExecuteSync(_cancellationTokenSource.Object.Token);

            //Then
            _ibkrClient.Verify(x => x.StartClientPortal(), Times.Once);
            _ibkrClient.Verify(x => x.Tickle(), Times.Once);
            _ibkrClient.Verify(x => x.CheckPnl(), Times.Never);

        }

    }
}