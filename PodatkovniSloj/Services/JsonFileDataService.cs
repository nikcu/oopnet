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

        public async Task<List<Match>> GetCountryMatchesAsync(string championship, string fifaCode)
        {
            if (string.IsNullOrWhiteSpace(fifaCode))
            {
                throw new ArgumentException("FIFA code cannot be empty", nameof(fifaCode));
            }

            var allMatches = await GetAllMatchesAsync(championship);

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
