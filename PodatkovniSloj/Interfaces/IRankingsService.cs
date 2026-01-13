using DataLayer.Models;

namespace DataLayer.Interfaces
{
    public interface IRankingsService
    {
        List<PlayerStat> GetGoalScorers(List<Match> matches);
        List<PlayerStat> GetYellowCardRecipients(List<Match> matches);
        List<AttendanceRecord> GetAttendanceRanking(List<Match> matches);
        Dictionary<string, Player> BuildPlayerLookup(List<Match> matches);
    }
}
