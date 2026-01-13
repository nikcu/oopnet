namespace DataLayer.Interfaces
{
    public interface ISettingsProvider
    {
        string SelectedChampionship { get; }
        string SelectedLanguage { get; }
        string? FavouriteTeamFifaCode { get; }
        bool WpfIsFullscreen { get; }
        int WpfResolutionWidth { get; }
        int WpfResolutionHeight { get; }
        bool GetIsLoadedFromFile();
    }
}
