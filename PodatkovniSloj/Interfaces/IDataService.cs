using DataLayer.Models;

namespace DataLayer.Interfaces
{
    /// <summary>
    /// Interface for data services that provide World Cup data.
    /// Implemented by WorldCupApiService and JsonFileDataService.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Gets team results for the specified championship
        /// </summary>
        /// <param name="championship">"m" for men's or "f" for women's</param>
        /// <returns>List of team results</returns>
        Task<List<TeamResult>> GetTeamResultsAsync(string championship);

        /// <summary>
        /// Gets all matches for the specified championship
        /// </summary>
        /// <param name="championship">"m" for men's or "f" for women's</param>
        /// <returns>List of all matches</returns>
        Task<List<Match>> GetAllMatchesAsync(string championship);

        /// <summary>
        /// Gets matches for a specific country
        /// </summary>
        /// <param name="championship">"m" for men's or "f" for women's</param>
        /// <param name="fifaCode">3-letter FIFA country code (e.g., "ENG", "FRA")</param>
        /// <returns>List of matches for the specified country</returns>
        Task<List<Match>> GetCountryMatchesAsync(string championship, string fifaCode);
    }
}
