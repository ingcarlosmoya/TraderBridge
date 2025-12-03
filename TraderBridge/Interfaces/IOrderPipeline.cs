using TraderBridge.Models;
namespace TraderBridge.Interfaces;
public interface IOrderPipeline
{
    bool IsOrderExecuted { get; }
    Task ExecuteAsync(Mt5TradeTransaction evt);
}

