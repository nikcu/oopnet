using DataLayer.Models;

namespace DataLayer.Interfaces
{
    public interface IDataService
    {
        Task<List<TeamResult>> GetTeamResultsAsync(string championship);
        Task<List<Match>> GetAllMatchesAsync(string championship);
        Task<List<Match>> GetCountryMatchesAsync(string championship, string fifaCode);
    }
}
