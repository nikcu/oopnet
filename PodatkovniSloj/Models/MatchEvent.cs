using System.Text.Json.Serialization;

namespace DataLayer.Models
{
    public class MatchEvent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type_of_event")]
        public string TypeOfEvent { get; set; } = string.Empty;

        [JsonPropertyName("player")]
        public string Player { get; set; } = string.Empty;

        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;
    }
}
