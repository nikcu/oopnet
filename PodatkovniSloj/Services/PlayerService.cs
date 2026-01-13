using DataLayer.Interfaces;
using DataLayer.Models;
using Utils.Helpers;

namespace DataLayer.Services
{
    public class PlayerService
    {
        private readonly ISettingsService _settings;

        public PlayerService(ISettingsService settings)
        {
            _settings = settings;
        }

        public List<Player> GetPlayersForTeam(List<Match> matches, string fifaCode)
        {
            var players = new Dictionary<string, Player>(StringComparer.OrdinalIgnoreCase);

            foreach (var match in matches)
            {
                if (match.HomeTeam.Code == fifaCode)
                {
                    AddPlayersFromStatistics(match.HomeTeamStatistics, players);
                }

                if (match.AwayTeam.Code == fifaCode)
                {
                    AddPlayersFromStatistics(match.AwayTeamStatistics, players);
                }
            }

            return players.Values
                .OrderBy(p => p.ShirtNumber)
                .ToList();
        }

        public string GenerateIdentifier(Player player)
        {
            return PlayerImageHelper.GeneratePlayerIdentifier(player.Name, player.ShirtNumber);
        }

        public string? GetPlayerImagePath(Player player)
        {
            string identifier = GenerateIdentifier(player);
            return _settings.GetPlayerImagePath(identifier);
        }

        public bool IsPlayerFavourite(Player player)
        {
            string identifier = GenerateIdentifier(player);
            return _settings.IsFavouritePlayer(identifier);
        }

        public Dictionary<string, List<Player>> GroupPlayersByPosition(List<Player> players)
        {
            return players
                .GroupBy(p => p.Position ?? "Unknown")
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(p => p.ShirtNumber).ToList()
                );
        }

        public List<Player> GetStartingEleven(TeamStatistics? statistics)
        {
            return statistics?.StartingEleven ?? new List<Player>();
        }

        public List<Player> GetSubstitutes(TeamStatistics? statistics)
        {
            return statistics?.Substitutes ?? new List<Player>();
        }

        private static void AddPlayersFromStatistics(TeamStatistics? statistics, Dictionary<string, Player> players)
        {
            if (statistics == null) return;

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
    }
}
