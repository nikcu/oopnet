using DataLayer.Interfaces;
using DataLayer.Models;
using Utils.Helpers;

namespace DataLayer.Services
{
    /// <summary>
    /// Service for player-related operations.
    /// Provides methods for retrieving and managing player data.
    /// </summary>
    public class PlayerService
    {
        private readonly ISettingsService _settings;

        public PlayerService(ISettingsService settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Gets all players for a specific team from match data
        /// </summary>
        /// <param name="matches">List of matches</param>
        /// <param name="fifaCode">Team FIFA code</param>
        /// <returns>List of unique players from the team</returns>
        public List<Player> GetPlayersForTeam(List<Match> matches, string fifaCode)
        {
            var players = new Dictionary<string, Player>(StringComparer.OrdinalIgnoreCase);

            foreach (var match in matches)
            {
                // Check home team
                if (match.HomeTeam.Code == fifaCode)
                {
                    AddPlayersFromStatistics(match.HomeTeamStatistics, players);
                }

                // Check away team
                if (match.AwayTeam.Code == fifaCode)
                {
                    AddPlayersFromStatistics(match.AwayTeamStatistics, players);
                }
            }

            return players.Values
                .OrderBy(p => p.ShirtNumber)
                .ToList();
        }

        /// <summary>
        /// Generates a unique identifier for a player
        /// </summary>
        /// <param name="player">Player object</param>
        /// <returns>Player identifier string</returns>
        public string GenerateIdentifier(Player player)
        {
            return PlayerImageHelper.GeneratePlayerIdentifier(player.Name, player.ShirtNumber);
        }

        /// <summary>
        /// Gets the image path for a player, or null if not set
        /// </summary>
        /// <param name="player">Player object</param>
        /// <returns>Image path or null</returns>
        public string? GetPlayerImagePath(Player player)
        {
            string identifier = GenerateIdentifier(player);
            return _settings.GetPlayerImagePath(identifier);
        }

        /// <summary>
        /// Checks if a player is marked as favourite
        /// </summary>
        /// <param name="player">Player object</param>
        /// <returns>True if player is a favourite</returns>
        public bool IsPlayerFavourite(Player player)
        {
            string identifier = GenerateIdentifier(player);
            return _settings.IsFavouritePlayer(identifier);
        }

        /// <summary>
        /// Groups players by their position
        /// </summary>
        /// <param name="players">List of players</param>
        /// <returns>Dictionary mapping position to list of players</returns>
        public Dictionary<string, List<Player>> GroupPlayersByPosition(List<Player> players)
        {
            return players
                .GroupBy(p => p.Position ?? "Unknown")
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(p => p.ShirtNumber).ToList()
                );
        }

        /// <summary>
        /// Gets starting eleven players from team statistics
        /// </summary>
        /// <param name="statistics">Team statistics</param>
        /// <returns>List of starting players</returns>
        public List<Player> GetStartingEleven(TeamStatistics? statistics)
        {
            return statistics?.StartingEleven ?? new List<Player>();
        }

        /// <summary>
        /// Gets substitute players from team statistics
        /// </summary>
        /// <param name="statistics">Team statistics</param>
        /// <returns>List of substitute players</returns>
        public List<Player> GetSubstitutes(TeamStatistics? statistics)
        {
            return statistics?.Substitutes ?? new List<Player>();
        }

        #region Private Helpers

        private static void AddPlayersFromStatistics(TeamStatistics? statistics, Dictionary<string, Player> players)
        {
            if (statistics == null) return;

            // Add starting eleven
            if (statistics.StartingEleven != null)
            {
                foreach (var player in statistics.StartingEleven)
                {
                    if (!string.IsNullOrEmpty(player.Name) && !players.ContainsKey(player.Name))
                    {
                        players[player.Name] = player;
                    }
                }
            }

            // Add substitutes
            if (statistics.Substitutes != null)
            {
                foreach (var player in statistics.Substitutes)
                {
                    if (!string.IsNullOrEmpty(player.Name) && !players.ContainsKey(player.Name))
                    {
                        players[player.Name] = player;
                    }
                }
            }
        }

        #endregion
    }
}
