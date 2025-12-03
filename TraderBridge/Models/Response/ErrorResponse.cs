using System.Text.Json.Serialization;

namespace TraderBridge.Models.Response
{
    public class ErrorResponse 
    {
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;
    }
}

