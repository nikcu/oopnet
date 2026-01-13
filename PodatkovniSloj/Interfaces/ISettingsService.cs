namespace DataLayer.Interfaces
{
    /// <summary>
    /// Interface for application settings management.
    /// Provides access to user preferences and application configuration.
    /// </summary>
    public interface ISettingsService
    {
        // Basic application settings
        string SelectedChampionship { get; set; }
        string SelectedLanguage { get; set; }

        // Favourite team (stored as FIFA code)
        string? FavouriteTeamFifaCode { get; set; }

        // WPF display mode settings
        bool WpfIsFullscreen { get; set; }
        int WpfResolutionWidth { get; set; }
        int WpfResolutionHeight { get; set; }

        // Favourite players (read-only list, use Add/Remove methods to modify)
        IReadOnlyList<string> FavouritePlayersList { get; }

        /// <summary>
        /// Checks if settings were loaded from an existing file
        /// </summary>
        bool GetIsLoadedFromFile();

        /// <summary>
        /// Saves settings to the settings file asynchronously
        /// </summary>
        Task SaveSettingsAsync();

        /// <summary>
        /// Saves settings to the settings file synchronously
        /// </summary>
        void SaveSettings();

        // Favourite player management
        /// <summary>
        /// Adds a player to favourites (max 3)
        /// </summary>
        /// <param name="playerIdentifier">Player identifier in "name_shirtnumber" format</param>
        /// <returns>True if added, false if already exists or max reached</returns>
        bool AddFavouritePlayer(string playerIdentifier);

        /// <summary>
        /// Removes a player from favourites
        /// </summary>
        /// <param name="playerIdentifier">Player identifier</param>
        /// <returns>True if removed, false if not found</returns>
        bool RemoveFavouritePlayer(string playerIdentifier);

        /// <summary>
        /// Checks if a player is in favourites
        /// </summary>
        /// <param name="playerIdentifier">Player identifier</param>
        /// <returns>True if player is a favourite</returns>
        bool IsFavouritePlayer(string playerIdentifier);

        /// <summary>
        /// Clears all favourite players
        /// </summary>
        void ClearFavouritePlayers();

        // Player image path management
        /// <summary>
        /// Sets the image path for a player
        /// </summary>
        /// <param name="playerIdentifier">Player identifier</param>
        /// <param name="imagePath">Full path to the image file</param>
        void SetPlayerImagePath(string playerIdentifier, string imagePath);

        /// <summary>
        /// Gets the image path for a player
        /// </summary>
        /// <param name="playerIdentifier">Player identifier</param>
        /// <returns>Image path or null if not set</returns>
        string? GetPlayerImagePath(string playerIdentifier);

        /// <summary>
        /// Removes the image path for a player
        /// </summary>
        /// <param name="playerIdentifier">Player identifier</param>
        /// <returns>True if removed, false if not found</returns>
        bool RemovePlayerImagePath(string playerIdentifier);
    }
}
