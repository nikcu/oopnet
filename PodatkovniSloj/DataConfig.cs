using System.Text.Json;
using System.Text.Json.Nodes;
using Utils;

namespace DataLayer
{
    /// <summary>
    /// Configuration for data source (API vs JSON files)
    /// </summary>
    public class DataConfig : Singleton<DataConfig>
    {
        private const string DataSourceDefault = Constant.DataSourceApi;
        private const string JsonFilesPathDefault = "Data";

        /// <summary>
        /// Data source type: "api" or "json"
        /// </summary>
        public string DataSource { get; set; } = DataSourceDefault;

        /// <summary>
        /// Path to JSON files directory (relative to executable)
        /// Only used when DataSource is "json"
        /// </summary>
        public string JsonFilesPath { get; set; } = JsonFilesPathDefault;

        private bool IsLoadedFromFile = false;

        private DataConfig()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (!File.Exists(Constant.pathDataConfig))
            {
                return;
            }

            try
            {
                using StreamReader reader = new(Constant.pathDataConfig);
                string json = reader.ReadToEnd();

                JsonNode configNode = JsonNode.Parse(json)!;

                DataSource = configNode[nameof(DataSource)]?.GetValue<string>() ?? DataSourceDefault;
                JsonFilesPath = configNode[nameof(JsonFilesPath)]?.GetValue<string>() ?? JsonFilesPathDefault;

                // Validate data source
                if (DataSource != Constant.DataSourceApi && DataSource != Constant.DataSourceJson)
                {
                    DataSource = DataSourceDefault;
                }

                IsLoadedFromFile = true;
            }
            catch (Exception ex)
            {
                // If config file is corrupted, use defaults
                Console.WriteLine($"Warning: Failed to load data config, using defaults: {ex.Message}");
                DataSource = DataSourceDefault;
                JsonFilesPath = JsonFilesPathDefault;
            }
        }

        public bool GetIsLoadedFromFile()
        {
            return IsLoadedFromFile;
        }

        public async Task SaveConfigAsync()
        {
            try
            {
                await using FileStream createStream = File.Create(Constant.pathDataConfig);
                await JsonSerializer.SerializeAsync(createStream, this, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save data config: {ex.Message}", ex);
            }
        }
    }
}
