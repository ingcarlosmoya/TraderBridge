namespace TraderBridge.Commands;
public class PlaceOrderCommand
{
    public string Symbol { get; set; } = string.Empty;
    public int Quantity { get; set; }
}