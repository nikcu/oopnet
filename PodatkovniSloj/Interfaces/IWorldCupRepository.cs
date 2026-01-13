namespace DataLayer.Interfaces
{
    public interface IWorldCupRepository : IDataService
    {
        string GetDataSource();
        Task<bool> IsDataSourceAvailableAsync();
    }
}
