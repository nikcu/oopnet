using DataLayer.Interfaces;
using DataLayer.Models;
using DataLayer.Services;
using Utils;

namespace DataLayer.Repositories
{
    /// <summary>
    /// Main repository for World Cup data access.
    /// Automatically switches between API and JSON file data sources based on configuration.
    /// </summary>
    public class WorldCupRepository : Singleton<WorldCupRepository>, IWorldCupRepository
    {
        private WorldCupApiService? _apiService;
        private JsonFileDataService? _jsonService;

        private WorldCupRepository()
        {
            InitializeServices();
        }

        private void InitializeServices()
        {
            var config = DataConfig.Instance;

            if (config.DataSource == Constant.DataSourceApi)
            {
                _apiService = new WorldCupApiService();
            }
            else if (config.DataSource == Constant.DataSourceJson)
            {
                try
                {
                    _jsonService = new JsonFileDataService(config.JsonFilesPath);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new Exception($"Failed to initialize JSON data service. " +
                        $"Directory '{config.JsonFilesPath}' not found. " +
                        $"Please ensure the data files are in the correct location or switch to API mode.", ex);
                }
            }
            else
            {
                throw new Exception($"Unknown data source: {config.DataSource}");
            }
        }

        /// <summary>
        /// Gets team results for the specified championship
        /// </summary>
        /// <param name="championship">"m" for men's or "f" for women's</param>
        /// <returns>List of team results</returns>
        public async Task<List<TeamResult>> GetTeamResultsAsync(string championship)
        {
            try
            {
                if (_apiService != null)
                {
                    return await _apiService.GetTeamResultsAsync(championship);
                }
                else if (_jsonService != null)
                {
                    return await _jsonService.GetTeamResultsAsync(championship);
                }
                else
                {
                    throw new InvalidOperationException("No data service is initialized");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get team results: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all matches for the specified championship
        /// </summary>
        /// <param name="championship">"m" for men's or "f" for women's</param>
        /// <returns>List of all matches</returns>
        public async Task<List<Match>> GetAllMatchesAsync(string championship)
        {
            try
            {
                if (_apiService != null)
                {
                    return await _apiService.GetAllMatchesAsync(championship);
                }
                else if (_jsonService != null)
                {
                    return await _jsonService.GetAllMatchesAsync(championship);
                }
                else
                {
                    throw new InvalidOperationException("No data service is initialized");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get all matches: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets matches for a specific country
        /// </summary>
        /// <param name="championship">"m" for men's or "f" for women's</param>
        /// <param name="fifaCode">3-letter FIFA country code (e.g., "ENG", "FRA")</param>
        /// <returns>List of matches for the specified country</returns>
        public async Task<List<Match>> GetCountryMatchesAsync(string championship, string fifaCode)
        {
            try
            {
                if (_apiService != null)
                {
                    return await _apiService.GetCountryMatchesAsync(championship, fifaCode);
                }
                else if (_jsonService != null)
                {
                    return await _jsonService.GetCountryMatchesAsync(championship, fifaCode);
                }
                else
                {
                    throw new InvalidOperationException("No data service is initialized");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get country matches: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the current data source type
        /// </summary>
        /// <returns>"api" or "json"</returns>
        public string GetDataSource()
        {
            return DataConfig.Instance.DataSource;
        }

        /// <summary>
        /// Checks if the data source is available
        /// </summary>
        /// <returns>True if data source is available, false otherwise</returns>
        public async Task<bool> IsDataSourceAvailableAsync()
        {
            try
            {
                // Try to fetch a small amount of data to check connectivity
                var championship = Settings.Instance.SelectedChampionship;
                var teams = await GetTeamResultsAsync(championship);
                return teams != null && teams.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
