using System.Text.Json.Serialization;
namespace TraderBridge.Models;
public class IbkrOrder
{
    public string Symbol { get; set; } = string.Empty;
    //public int Quantity { get; set; }
    public string Action { get; set; } = string.Empty;
    //public string OrderType { get; set; } = "MKT";
    public double? AuxPrice { get; set; }
    public double? LimitPrice { get; set; }

    public string AcctId { get; set; } = string.Empty;
    public int Conid { get; set; }

    [JsonPropertyName("orderType")]
    public string OrderType { get; set; } = "MKT";
    public string Side { get; set; } = string.Empty;
    public string Ticker { get; set; } = string.Empty;
    public string Tif { get; set; } = string.Empty;
    public double Quantity { get; set; }

    public List<IbkrOrder> Orders  { get; set; }
}
