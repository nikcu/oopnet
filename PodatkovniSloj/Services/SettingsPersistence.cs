using System.Text.Json;
using System.Text.Json.Serialization;
using DataLayer.Models;
using Utils.Interfaces;

namespace DataLayer.Services
{
    /// <summary>
    /// Handles settings file persistence (loading and saving).
    /// Separates I/O concerns from Settings business logic.
    /// </summary>
    public class SettingsPersistence
    {
        private readonly ILogger? _logger;

        public SettingsPersistence(ILogger? logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// Loads settings from a JSON file
        /// </summary>
        /// <param name="filePath">Path to the settings file</param>
        /// <returns>SettingsData object, or null if file doesn't exist or is invalid</returns>
        public SettingsData? LoadFromFile(string filePath)
        {
            try
            {
                string fullPath = Path.GetFullPath(filePath);

                if (!File.Exists(fullPath))
                {
                    _logger?.Info($"Settings file not found at: {fullPath}");
                    return null;
                }

                _logger?.Info($"Loading settings from: {fullPath}");

                string json = File.ReadAllText(fullPath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger?.Warning("Settings file is empty");
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = JsonSerializer.Deserialize<SettingsData>(json, options);

                if (data != null)
                {
                    // Validate and clean up image paths
                    data.PlayerImagePaths = data.PlayerImagePaths
                        .Where(kvp => File.Exists(kvp.Value))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    _logger?.Info($"Settings loaded: Championship={data.SelectedChampionship}, " +
                                  $"Language={data.SelectedLanguage}, " +
                                  $"FavouriteTeam={data.FavouriteTeamFifaCode ?? "None"}");
                }

                return data;
            }
            catch (JsonException ex)
            {
                _logger?.Error($"JSON parsing error in settings file", ex);
                return null;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error loading settings", ex);
                return null;
            }
        }

        /// <summary>
        /// Saves settings to a JSON file
        /// </summary>
        /// <param name="filePath">Path to the settings file</param>
        /// <param name="data">Settings data to save</param>
        public async Task SaveToFileAsync(string filePath, SettingsData data)
        {
            string fullPath = Path.GetFullPath(filePath);

            // Ensure directory exists
            string? directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger?.Info($"Created settings directory: {directory}");
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            };

            _logger?.Info($"Saving settings to: {fullPath}");

            await using FileStream stream = File.Create(fullPath);
            await JsonSerializer.SerializeAsync(stream, data, options);
            await stream.FlushAsync();

            _logger?.Info("Settings saved successfully");
        }

        /// <summary>
        /// Validates settings values
        /// </summary>
        /// <param name="data">Settings data to validate</param>
        /// <param name="validChampionships">Valid championship values</param>
        /// <param name="validLanguages">Valid language values</param>
        /// <returns>True if valid, throws exception if invalid</returns>
        public static bool Validate(SettingsData data, string[] validChampionships, string[] validLanguages)
        {
            if (!validChampionships.Contains(data.SelectedChampionship))
            {
                throw new InvalidOperationException($"Invalid championship value: {data.SelectedChampionship}");
            }

            if (!validLanguages.Contains(data.SelectedLanguage))
            {
                throw new InvalidOperationException($"Invalid language value: {data.SelectedLanguage}");
            }

            return true;
        }
    }
}
