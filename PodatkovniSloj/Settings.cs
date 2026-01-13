using System.Text.Json.Serialization;
using DataLayer.Interfaces;
using DataLayer.Models;
using DataLayer.Services;
using Utils;
using Utils.Interfaces;
using Utils.Logging;

namespace DataLayer
{
    /// <summary>
    /// Application settings management.
    /// Implements ISettingsService for dependency inversion.
    /// Uses SettingsPersistence for file I/O (SRP).
    /// </summary>
    public class Settings : Singleton<Settings>, ISettingsService
    {
        // Constants for available options
        public static readonly string[] Championship = ["m", "f"];
        public static readonly string[] Languages = ["en", "hr"];

        // Persistence helper
        private readonly SettingsPersistence _persistence;
        private readonly ILogger _logger;

        // Settings data
        private SettingsData _data;

        // Tracks if settings were loaded from file
        [JsonIgnore]
        private bool _isLoadedFromFile = false;

        private Settings()
        {
            _logger = new ConsoleLogger("Settings");
            _persistence = new SettingsPersistence(_logger);
            _data = new SettingsData();
            LoadSettings();
        }

        #region ISettingsService Properties

        public string SelectedChampionship
        {
            get => _data.SelectedChampionship;
            set => _data.SelectedChampionship = value;
        }

        public string SelectedLanguage
        {
            get => _data.SelectedLanguage;
            set => _data.SelectedLanguage = value;
        }

        public string? FavouriteTeamFifaCode
        {
            get => _data.FavouriteTeamFifaCode;
            set => _data.FavouriteTeamFifaCode = value;
        }

        public bool WpfIsFullscreen
        {
            get => _data.WpfIsFullscreen;
            set => _data.WpfIsFullscreen = value;
        }

        public int WpfResolutionWidth
        {
            get => _data.WpfResolutionWidth;
            set => _data.WpfResolutionWidth = value;
        }

        public int WpfResolutionHeight
        {
            get => _data.WpfResolutionHeight;
            set => _data.WpfResolutionHeight = value;
        }

        // Direct access to FavouritePlayers list (for JSON serialization compatibility)
        [JsonPropertyName("favourite_players")]
        public List<string> FavouritePlayers
        {
            get => _data.FavouritePlayers;
            set => _data.FavouritePlayers = value;
        }

        // ISettingsService implementation - read-only view
        [JsonIgnore]
        public IReadOnlyList<string> FavouritePlayersList => _data.FavouritePlayers.AsReadOnly();

        // Direct access to PlayerImagePaths (for JSON serialization compatibility)
        [JsonPropertyName("player_image_paths")]
        public Dictionary<string, string> PlayerImagePaths
        {
            get => _data.PlayerImagePaths;
            set => _data.PlayerImagePaths = value;
        }

        #endregion

        #region Persistence

        private void LoadSettings()
        {
            var loadedData = _persistence.LoadFromFile(Constant.pathSettings);

            if (loadedData != null)
            {
                _data = loadedData;
                _isLoadedFromFile = true;
                _logger.Info("Settings loaded from file");
            }
            else
            {
                _data = new SettingsData();
                _isLoadedFromFile = false;
                _logger.Info("Using default settings");
            }
        }

        public bool GetIsLoadedFromFile()
        {
            return _isLoadedFromFile;
        }

        public async Task SaveSettingsAsync()
        {
            try
            {
                // Validate before saving
                SettingsPersistence.Validate(_data, Championship, Languages);

                await _persistence.SaveToFileAsync(Constant.pathSettings, _data);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(
                    $"Permission denied when saving settings. " +
                    $"Try running as administrator or check file permissions.", ex);
            }
            catch (IOException ex)
            {
                throw new IOException(
                    $"IO error when saving settings. " +
                    $"File may be in use or disk may be full.", ex);
            }
        }

        public void SaveSettings()
        {
            SaveSettingsAsync().GetAwaiter().GetResult();
        }

        #endregion

        #region Favourite Player Management

        public bool AddFavouritePlayer(string playerIdentifier)
        {
            if (_data.FavouritePlayers.Count >= 3)
            {
                return false; // Max 3 favourite players
            }

            if (!_data.FavouritePlayers.Contains(playerIdentifier))
            {
                _data.FavouritePlayers.Add(playerIdentifier);
                return true;
            }

            return false;
        }

        public bool RemoveFavouritePlayer(string playerIdentifier)
        {
            return _data.FavouritePlayers.Remove(playerIdentifier);
        }

        public bool IsFavouritePlayer(string playerIdentifier)
        {
            return _data.FavouritePlayers.Contains(playerIdentifier);
        }

        public void ClearFavouritePlayers()
        {
            _data.FavouritePlayers.Clear();
        }

        #endregion

        #region Player Image Path Management

        public void SetPlayerImagePath(string playerIdentifier, string imagePath)
        {
            _data.PlayerImagePaths[playerIdentifier] = imagePath;
        }

        public string? GetPlayerImagePath(string playerIdentifier)
        {
            return _data.PlayerImagePaths.TryGetValue(playerIdentifier, out string? path) ? path : null;
        }

        public bool RemovePlayerImagePath(string playerIdentifier)
        {
            return _data.PlayerImagePaths.Remove(playerIdentifier);
        }

        #endregion
    }
}
