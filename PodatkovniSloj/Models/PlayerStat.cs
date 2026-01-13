namespace DataLayer.Models
{
    /// <summary>
    /// Represents a player statistic (goals, yellow cards, etc.)
    /// </summary>
    public record PlayerStat(
        string PlayerName,
        int Count,
        string? TeamCode = null,
        int? ShirtNumber = null
    );
}
