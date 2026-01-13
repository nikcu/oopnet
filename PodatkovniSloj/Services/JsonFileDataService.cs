using System.Text.Json;
using DataLayer.Interfaces;
using DataLayer.Models;

namespace DataLayer.Services
{
    public class JsonFileDataService : IDataService
    {
        private readonly string _dataPath;

        public JsonFileDataService(string dataPath)
        {
            _dataPath = dataPath ?? throw new ArgumentNullException(nameof(dataPath));

            if (!Directory.Exists(_dataPath))
            {
                throw new DirectoryNotFoundException($"Data directory not found: {_dataPath}");
            }
        }

        /// <summary>
        /// Loads team results from a JSON file
        /// </summary>
        /// <param name="championship">"m" for men's or "f" for women's</param>
        /// <returns>List of team results</returns>
        public async Task<List<TeamResult>> GetTeamResultsAsync(string championship)
        {
            ValidateChampionship(championship);

            string fileName = championship == "m" ? "men_teams.json" : "women_teams.json";
            string filePath = Path.Combine(_dataPath, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Team results file not found: {filePath}");
            }

            try
            {
                await using FileStream openStream = File.OpenRead(filePath);
                var teams = await JsonSerializer.DeserializeAsync<List<TeamResult>>(openStream);

                return teams ?? new List<TeamResult>();
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse team results from file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads all matches from a JSON file
        /// </summary>
        /// <param name="championship">"m" for men's or "f" for women's</param>
        /// <returns>List of all matches</returns>
        public async Task<List<Match>> GetAllMatchesAsync(string championship)
        {
            ValidateChampionship(championship);

            string fileName = championship == "m" ? "men_matches.json" : "women_matches.json";
            string filePath = Path.Combine(_dataPath, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Matches file not found: {filePath}");
            }

            try
            {
                await using FileStream openStream = File.OpenRead(filePath);
                var matches = await JsonSerializer.DeserializeAsync<List<Match>>(openStream);

                return matches ?? new List<Match>();
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse matches from file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets matches for a specific country by filtering all matches
        /// </summary>
        /// <param name="championship">"m" for men's or "f" for women's</param>
        /// <param name="fifaCode">3-letter FIFA country code (e.g., "ENG", "FRA")</param>
        /// <returns>List of matches for the specified country</returns>
        public async Task<List<Match>> GetCountryMatchesAsync(string championship, string fifaCode)
        {
            if (string.IsNullOrWhiteSpace(fifaCode))
            {
                throw new ArgumentException("FIFA code cannot be empty", nameof(fifaCode));
            }

            var allMatches = await GetAllMatchesAsync(championship);

            // Filter matches where the country is either home or away team
            var countryMatches = allMatches
                .Where(m => m.HomeTeam.Code == fifaCode || m.AwayTeam.Code == fifaCode)
                .ToList();

            return countryMatches;
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
