using TraderBridge.Interfaces;
using TraderBridge.Models;
namespace TraderBridge.Handlers;
public class SendToIbkrHandler : IOrderHandler
{
    private readonly IIbkrClient _ibkrClient;
    private readonly IOrderRepository _repo;
    public SendToIbkrHandler(IIbkrClient ibkrClient, IOrderRepository repo)
    {
        _ibkrClient = ibkrClient; _repo = repo;
    }

    Task IOrderHandler.Handle(TradeContext tradeContext)
    {
        try
        {
            if (tradeContext != null && tradeContext.IbkrOrder != null) {
                _ibkrClient.SendOrderAsync(tradeContext.IbkrOrder);
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