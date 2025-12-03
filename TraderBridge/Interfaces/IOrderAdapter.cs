using TraderBridge.Models;
namespace TraderBridge.Interfaces;
public interface IOrderAdapter
{
    Task<bool> ApplyMapping(Mt5TradeTransaction mt5Order);
}
