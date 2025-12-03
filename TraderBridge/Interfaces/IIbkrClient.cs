using TraderBridge.Models;
namespace TraderBridge.Interfaces;
public interface IIbkrClient
{
    bool IsConnected { get; }
    Task CheckPnl();
    Task SendOrderAsync(IbkrOrder order);
    Task<bool> StartClientPortal();
    Task Tickle();
}
