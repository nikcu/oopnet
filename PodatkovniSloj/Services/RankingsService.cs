using DataLayer.Interfaces;
using DataLayer.Models;

namespace DataLayer.Services
{
    public class RankingsService : IRankingsService
    {
        public List<PlayerStat> GetGoalScorers(List<Match> matches)
        {
            var goalScorers = new Dictionary<string, int>();

            foreach (var match in matches)
            {
                ProcessGoalEvents(match.HomeTeamEvents, goalScorers);
                ProcessGoalEvents(match.AwayTeamEvents, goalScorers);
            }

            return goalScorers
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key)
                .Select(kvp => new PlayerStat(kvp.Key, kvp.Value))
                .ToList();
        }

        public List<PlayerStat> GetYellowCardRecipients(List<Match> matches)
        {
            var yellowCards = new Dictionary<string, int>();

            foreach (var match in matches)
            {
                ProcessYellowCardEvents(match.HomeTeamEvents, yellowCards);
                ProcessYellowCardEvents(match.AwayTeamEvents, yellowCards);
            }

            return yellowCards
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key)
                .Select(kvp => new PlayerStat(kvp.Key, kvp.Value))
                .ToList();
        }

        public List<AttendanceRecord> GetAttendanceRanking(List<Match> matches)
        {
            return matches
                .Where(m => !string.IsNullOrEmpty(m.Attendance))
                .Select(m => new AttendanceRecord(
                    m.Location,
                    m.Venue,
                    ParseAttendance(m.Attendance),
                    m.HomeTeamCountry,
                    m.AwayTeamCountry
                ))
                .Where(a => a.Attendance > 0)
                .OrderByDescending(a => a.Attendance)
                .ToList();
        }

        public Dictionary<string, Player> BuildPlayerLookup(List<Match> matches)
        {
            var playerLookup = new Dictionary<string, Player>(StringComparer.OrdinalIgnoreCase);

            foreach (var match in matches)
            {
                AddPlayersToLookup(match.HomeTeamStatistics?.StartingEleven, playerLookup);
                AddPlayersToLookup(match.HomeTeamStatistics?.Substitutes, playerLookup);
                AddPlayersToLookup(match.AwayTeamStatistics?.StartingEleven, playerLookup);
                AddPlayersToLookup(match.AwayTeamStatistics?.Substitutes, playerLookup);
            }

            return playerLookup;
        }

        private static void ProcessGoalEvents(List<MatchEvent>? events, Dictionary<string, int> goalScorers)
        {
            if (events == null) return;

            foreach (var evt in events)
            {
                if (IsGoalEvent(evt.TypeOfEvent))
                {
                    string playerName = evt.Player;
                    if (!string.IsNullOrEmpty(playerName))
                    {
                        if (goalScorers.ContainsKey(playerName))
                            goalScorers[playerName]++;
                        else
                            goalScorers[playerName] = 1;
                    }
                }
            }
        }

        private static void ProcessYellowCardEvents(List<MatchEvent>? events, Dictionary<string, int> yellowCards)
        {
            if (events == null) return;

            foreach (var evt in events)
            {
                if (evt.TypeOfEvent.Equals("yellow-card", StringComparison.OrdinalIgnoreCase))
                {
                    string playerName = evt.Player;
                    if (!string.IsNullOrEmpty(playerName))
                    {
                        if (yellowCards.ContainsKey(playerName))
                            yellowCards[playerName]++;
                        else
                            yellowCards[playerName] = 1;
                    }
                }
            }
        }

        private static bool IsGoalEvent(string eventType)
        {
            return eventType.Equals("goal", StringComparison.OrdinalIgnoreCase) ||
                   eventType.Equals("goal-penalty", StringComparison.OrdinalIgnoreCase);
        }

        private static int ParseAttendance(string? attendance)
        {
            if (string.IsNullOrEmpty(attendance))
                return 0;

            string cleanedValue = new string(attendance.Where(char.IsDigit).ToArray());
            return int.TryParse(cleanedValue, out int result) ? result : 0;
        }

        private static void AddPlayersToLookup(List<Player>? players, Dictionary<string, Player> lookup)
        {
            if (players == null) return;

            foreach (var player in players)
            {
                if (!string.IsNullOrEmpty(player.Name) && !lookup.ContainsKey(player.Name))
                {
                    lookup[player.Name] = player;
                }
            }
        }
    }
}
