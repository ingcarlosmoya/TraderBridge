namespace TraderBridge.Models;

public class TradeContext
{
    public Mt5TradeTransaction Mt5Order { get; set; }
    public IbkrOrder IbkrOrder { get; set; }     // ? este se debe llenar
    public Task<bool> IsStepSuccesful { get; set; }
}
