using System.Text.Json.Serialization;

namespace AI.MAF.AgentHostings.RestAPI
{
    public class GenerateResponse
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = string.Empty;

        [JsonPropertyName("totalOptions")]
        public int TotalOptions { get; set; }

        [JsonPropertyName("hasLuggage")]
        public bool HasLuggage { get; set; }
    }
}
