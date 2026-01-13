namespace DataLayer.Models
{
    /// <summary>
    /// Represents match attendance data
    /// </summary>
    public record AttendanceRecord(
        string Location,
        string? Venue,
        int Attendance,
        string HomeTeam,
        string AwayTeam
    )
    {
        public string MatchDescription => $"{HomeTeam} vs {AwayTeam}";
    }
}
