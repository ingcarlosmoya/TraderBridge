using TraderBridge.Interfaces;
using TraderBridge.Models;
namespace TraderBridge.Handlers;
public class SymbolValidationHandler : IOrderHandler
{
    public Task Handle(TradeContext tradeContext)
    {
		try
		{
            tradeContext.IsStepSuccesful = Task.FromResult(tradeContext != null && !string.IsNullOrWhiteSpace(tradeContext.Mt5Order.Symbol)); 
            return Task.CompletedTask;
        }
		catch (Exception)
		{
			throw;
		}
    }
}
