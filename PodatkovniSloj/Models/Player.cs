using System.Text.Json.Serialization;

namespace DataLayer.Models
{
    public class Player
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("captain")]
        public bool Captain { get; set; }

        [JsonPropertyName("shirt_number")]
        public int ShirtNumber { get; set; }

        [JsonPropertyName("position")]
        public string Position { get; set; } = string.Empty;
    }
}
