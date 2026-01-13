namespace DataLayer.Interfaces
{
    public interface ISettingsPersistence
    {
        Task SaveSettingsAsync();
        void SaveSettings();
    }
}
