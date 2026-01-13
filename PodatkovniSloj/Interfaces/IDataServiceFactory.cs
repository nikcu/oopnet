namespace DataLayer.Interfaces
{
    public interface IDataServiceFactory
    {
        IDataService CreateDataService();
        string GetDataSourceType();
    }
}
