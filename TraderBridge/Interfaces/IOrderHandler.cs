using TraderBridge.Models;
namespace TraderBridge.Interfaces;
public interface IOrderHandler
{
    Task Handle(TradeContext tradeContext);
}
