using System.Text.Json.Serialization;

namespace DataLayer.Models
{
    public class Weather
    {
        [JsonPropertyName("humidity")]
        public string Humidity { get; set; } = string.Empty;

        [JsonPropertyName("temp_celsius")]
        public string TempCelsius { get; set; } = string.Empty;

        [JsonPropertyName("temp_farenheit")]
        public string TempFarenheit { get; set; } = string.Empty;

        [JsonPropertyName("wind_speed")]
        public string WindSpeed { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}
