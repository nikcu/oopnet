namespace DataLayer.Interfaces
{
    public interface ISettingsService : ISettingsProvider, IFavouritePlayersManager, IPlayerImageManager, ISettingsPersistence
    {
        new string SelectedChampionship { get; set; }
        new string SelectedLanguage { get; set; }
        new string? FavouriteTeamFifaCode { get; set; }
        new bool WpfIsFullscreen { get; set; }
        new int WpfResolutionWidth { get; set; }
        new int WpfResolutionHeight { get; set; }
    }
}
