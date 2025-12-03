using TraderBridge.Interfaces;
using TraderBridge.Models;
namespace TraderBridge.Handlers;
public class LoggingHandler : IOrderHandler
{
    Task IOrderHandler.Handle(TradeContext tradeContext)
    {
        return Task.CompletedTask;
    }
}
