using System.Text.Json.Serialization;

namespace DataLayer.Models
{
    /// <summary>
    /// Data Transfer Object for settings persistence.
    /// Contains only data with JSON attributes, no business logic.
    /// </summary>
    public class SettingsData
    {
        [JsonPropertyName("selected_championship")]
        public string SelectedChampionship { get; set; } = "m";

        [JsonPropertyName("selected_language")]
        public string SelectedLanguage { get; set; } = "en";

        [JsonPropertyName("favourite_team_fifa_code")]
        public string? FavouriteTeamFifaCode { get; set; }

        [JsonPropertyName("favourite_players")]
        public List<string> FavouritePlayers { get; set; } = new();

        [JsonPropertyName("player_image_paths")]
        public Dictionary<string, string> PlayerImagePaths { get; set; } = new();

        [JsonPropertyName("wpf_is_fullscreen")]
        public bool WpfIsFullscreen { get; set; } = false;

        [JsonPropertyName("wpf_resolution_width")]
        public int WpfResolutionWidth { get; set; } = 1280;

        [JsonPropertyName("wpf_resolution_height")]
        public int WpfResolutionHeight { get; set; } = 720;
    }
}
