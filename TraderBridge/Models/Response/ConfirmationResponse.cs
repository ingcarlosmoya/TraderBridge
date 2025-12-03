using System.Text.Json.Serialization;

namespace TraderBridge.Models.Response
{
    public class ConfirmationResponse 
    {
        [JsonPropertyName("message")]
        public IEnumerable<string> Message { get; set; } = new List<string>();

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}

