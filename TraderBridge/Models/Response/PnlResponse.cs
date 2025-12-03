using System.Text.Json.Serialization;

namespace TraderBridge.Models.Response
{
    public class PnlResponse
    {
        [JsonPropertyName("order_id")]
        public string Order_id { get; set; } = string.Empty;
        [JsonPropertyName("order_status")]
        public string Order_status { get; set; } = string.Empty;
    }
}

