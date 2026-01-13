namespace DataLayer.Interfaces
{
    /// <summary>
    /// Interface for the World Cup repository.
    /// Extends IDataService with repository-specific functionality.
    /// </summary>
    public interface IWorldCupRepository : IDataService
    {
        /// <summary>
        /// Gets the current data source type
        /// </summary>
        /// <returns>"api" or "json"</returns>
        string GetDataSource();

        /// <summary>
        /// Checks if the data source is available
        /// </summary>
        /// <returns>True if data source is available, false otherwise</returns>
        Task<bool> IsDataSourceAvailableAsync();
    }
}
