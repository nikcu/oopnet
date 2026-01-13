using DataLayer.Interfaces;
using Utils;

namespace DataLayer.Services
{
    public class DataServiceFactory : IDataServiceFactory
    {
        private readonly DataConfig _config;

        public DataServiceFactory() : this(DataConfig.Instance)
        {
        }

        public DataServiceFactory(DataConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IDataService CreateDataService()
        {
            return _config.DataSource switch
            {
                Constant.DataSourceApi => new WorldCupApiService(),
                Constant.DataSourceJson => CreateJsonService(),
                _ => throw new InvalidOperationException(
                    $"Unknown data source: {_config.DataSource}. " +
                    $"Valid values are '{Constant.DataSourceApi}' or '{Constant.DataSourceJson}'.")
            };
        }

        public string GetDataSourceType() => _config.DataSource;

        private IDataService CreateJsonService()
        {
            try
            {
                return new JsonFileDataService(_config.JsonFilesPath);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to initialize JSON data service. " +
                    $"Directory '{_config.JsonFilesPath}' not found. " +
                    $"Please ensure the data files are in the correct location or switch to API mode.", ex);
            }
        }
    }
}
