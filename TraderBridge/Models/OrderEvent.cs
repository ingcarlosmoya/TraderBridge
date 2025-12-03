namespace TraderBridge.Models;
public class OrderEvent
{
    public Mt5TradeTransaction Transaction { get; set; } = new Mt5TradeTransaction();
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}
