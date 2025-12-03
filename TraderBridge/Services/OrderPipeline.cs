using TraderBridge.Interfaces;
using TraderBridge.Models;
namespace TraderBridge.Services;
public class OrderPipeline : IOrderPipeline
{
    public bool IsOrderExecuted { get;  private set; }
    private readonly IEnumerable<IOrderHandler> _handlers;
    public OrderPipeline(IEnumerable<IOrderHandler> handlers) { _handlers = handlers; }
    public async Task ExecuteAsync(Mt5TradeTransaction mt5Order)
    {
        var tradeContext = new TradeContext
        {
            Mt5Order = mt5Order
        };

        foreach (var h in _handlers)
        {
            await h.Handle(tradeContext);
            IsOrderExecuted =tradeContext.IsStepSuccesful.Result;
            if (!tradeContext.IsStepSuccesful.Result) break;
        }
    }
}
