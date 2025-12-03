namespace TraderBridge.Models;
public class Mt5TradeTransaction
{
    public string Symbol { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "BUY"/"SELL"
    public double Volume { get; set; }
    public double Price { get; set; }
    public ulong Ticket { get; set; }
    public double Sl { get; set; }
    public double Tp { get; set; }
    public int ConId { get; set; }
}
