using DataLayer.Interfaces;
using DataLayer.Models;
using DataLayer.Services;
using Utils;

namespace DataLayer.Repositories
{
    public class WorldCupRepository : Singleton<WorldCupRepository>, IWorldCupRepository
    {
        private readonly IDataService _dataService;
        private readonly IDataServiceFactory _factory;

        private WorldCupRepository() : this(new DataServiceFactory())
        {
        }

        internal WorldCupRepository(IDataServiceFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _dataService = _factory.CreateDataService();
        }

        public async Task<List<TeamResult>> GetTeamResultsAsync(string championship)
        {
            try
            {
                return await _dataService.GetTeamResultsAsync(championship);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to get team results", ex);
            }
        }

        public async Task<List<Match>> GetAllMatchesAsync(string championship)
        {
            try
            {
                return await _dataService.GetAllMatchesAsync(championship);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Failed to get all matches", ex);
            }
        }

        public async Task<List<Match>> GetCountryMatchesAsync(string championship, string fifaCode)
        {
            try
            {
                return await _dataService.GetCountryMatchesAsync(championship, fifaCode);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"Failed to get matches for {fifaCode}", ex);
            }
        }

        public string GetDataSource() => _factory.GetDataSourceType();

        public async Task<bool> IsDataSourceAvailableAsync()
        {
            try
            {
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

    public class DataAccessException : Exception
    {
        public DataAccessException(string message) : base(message) { }
        public DataAccessException(string message, Exception innerException) : base(message, innerException) { }
    }
}
