using Microsoft.Extensions.Options;
using TraderBridge.Config;
using TraderBridge.Interfaces;
using TraderBridge.Models;
namespace TraderBridge.Handlers;
public class BuildIbkrOrderHandler : IOrderHandler
{
    private readonly IBKRSettings _bridgeSettings;
    public BuildIbkrOrderHandler(IOptions<IBKRSettings> bridgeSettings)
    {
        _bridgeSettings = bridgeSettings.Value;
    }
    public Task Handle(TradeContext tradeContext)
    {
        try
        {
            if (tradeContext != null && tradeContext.Mt5Order != null)
            {
                var mt5Order = tradeContext.Mt5Order;
                tradeContext.IbkrOrder = new IbkrOrder()
                {
                    AcctId = _bridgeSettings.AccountId,
                    Conid = mt5Order.ConId,
                    OrderType = "MKT",
                    Quantity = mt5Order.Volume,
                    Side = mt5Order.Action,
                    Ticker = mt5Order.Symbol,
                    Tif = "GTC"
                };
                tradeContext.IsStepSuccesful = Task.FromResult(true);
            }

            return Task.CompletedTask;
        }
        catch (Exception)
        {

            throw;
        }

    }
}
