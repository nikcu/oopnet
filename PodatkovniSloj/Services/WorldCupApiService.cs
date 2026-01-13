using System.Text.Json;
using DataLayer.Interfaces;
using DataLayer.Models;

namespace DataLayer.Services
{
    public class WorldCupApiService : IDataService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://worldcup-vua.nullbit.hr";

        public WorldCupApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        /// <summary>
        /// Fetches team results for the specified championship
        /// </summary>
        /// <param name="championship">"m" for men's or "f" for women's</param>
        /// <returns>List of team results</returns>
        public async Task<List<TeamResult>> GetTeamResultsAsync(string championship)
        {
            ValidateChampionship(championship);

            string endpoint = $"/{championship switch
            {
                "m" => "men",
                "f" => "women",
                _ => throw new ArgumentException($"Invalid championship: {championship}")
            }}/teams/results";

            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var teams = JsonSerializer.Deserialize<List<TeamResult>>(jsonString);

                return teams ?? new List<TeamResult>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to fetch team results: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse team results: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Fetches all matches for the specified championship
        /// </summary>
        /// <param name="championship">"m" for men's or "f" for women's</param>
        /// <returns>List of all matches</returns>
        public async Task<List<Match>> GetAllMatchesAsync(string championship)
        {
            ValidateChampionship(championship);

            string endpoint = $"/{championship switch
            {
                "m" => "men",
                "f" => "women",
                _ => throw new ArgumentException($"Invalid championship: {championship}")
            }}/matches";

            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var matches = JsonSerializer.Deserialize<List<Match>>(jsonString);

                return matches ?? new List<Match>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to fetch matches: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse matches: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Fetches matches for a specific country
        /// </summary>
        /// <param name="championship">"m" for men's or "f" for women's</param>
        /// <param name="fifaCode">3-letter FIFA country code (e.g., "ENG", "FRA")</param>
        /// <returns>List of matches for the specified country</returns>
        public async Task<List<Match>> GetCountryMatchesAsync(string championship, string fifaCode)
        {
            ValidateChampionship(championship);

            if (string.IsNullOrWhiteSpace(fifaCode))
            {
                throw new ArgumentException("FIFA code cannot be empty", nameof(fifaCode));
            }

            string endpoint = $"/{championship switch
            {
                "m" => "men",
                "f" => "women",
                _ => throw new ArgumentException($"Invalid championship: {championship}")
            }}/matches/country?fifa_code={fifaCode}";

            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var matches = JsonSerializer.Deserialize<List<Match>>(jsonString);

                return matches ?? new List<Match>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to fetch country matches: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse country matches: {ex.Message}", ex);
            }
        }

        private void ValidateChampionship(string championship)
        {
            if (championship != "m" && championship != "f")
            {
                throw new ArgumentException($"Championship must be 'm' or 'f', got '{championship}'", nameof(championship));
            }
        }
    }
}
