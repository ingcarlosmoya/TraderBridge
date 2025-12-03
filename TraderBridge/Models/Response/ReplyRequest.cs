using System.Text.Json.Serialization;

namespace TraderBridge.Models.Response
{

    public class ReplyRequest() {
        [JsonPropertyName("confirmed")]
        public bool Confirmed { get; set; }
    }
}

