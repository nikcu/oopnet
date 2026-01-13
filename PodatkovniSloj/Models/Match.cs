using System.Text.Json.Serialization;

namespace DataLayer.Models
{
    public class Match
    {
        [JsonPropertyName("venue")]
        public string Venue { get; set; } = string.Empty;

        [JsonPropertyName("location")]
        public string Location { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;

        [JsonPropertyName("fifa_id")]
        public string FifaId { get; set; } = string.Empty;

        [JsonPropertyName("weather")]
        public Weather? Weather { get; set; }

        [JsonPropertyName("attendance")]
        public string Attendance { get; set; } = string.Empty;

        [JsonPropertyName("officials")]
        public List<string> Officials { get; set; } = new();

        [JsonPropertyName("stage_name")]
        public string StageName { get; set; } = string.Empty;

        [JsonPropertyName("home_team_country")]
        public string HomeTeamCountry { get; set; } = string.Empty;

        [JsonPropertyName("away_team_country")]
        public string AwayTeamCountry { get; set; } = string.Empty;

        [JsonPropertyName("datetime")]
        public string Datetime { get; set; } = string.Empty;

        [JsonPropertyName("winner")]
        public string? Winner { get; set; }

        [JsonPropertyName("winner_code")]
        public string? WinnerCode { get; set; }

        [JsonPropertyName("home_team")]
        public TeamInMatch HomeTeam { get; set; } = new();

        [JsonPropertyName("away_team")]
        public TeamInMatch AwayTeam { get; set; } = new();

        [JsonPropertyName("home_team_events")]
        public List<MatchEvent> HomeTeamEvents { get; set; } = new();

        [JsonPropertyName("away_team_events")]
        public List<MatchEvent> AwayTeamEvents { get; set; } = new();

        [JsonPropertyName("home_team_statistics")]
        public TeamStatistics HomeTeamStatistics { get; set; } = new();

        [JsonPropertyName("away_team_statistics")]
        public TeamStatistics AwayTeamStatistics { get; set; } = new();

        [JsonPropertyName("last_event_update_at")]
        public string? LastEventUpdateAt { get; set; }

        [JsonPropertyName("last_score_update_at")]
        public string? LastScoreUpdateAt { get; set; }
    }
}
