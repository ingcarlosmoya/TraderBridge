using System.Text.Json.Serialization;

namespace TraderBridge.Models
{
    public class SuccessfulResponse 
    {
        [JsonPropertyName("order_id")]
        public string Order_id { get; set; } = string.Empty;
        [JsonPropertyName("order_status")]
        public string Order_status { get; set; } = string.Empty;
    }
}

