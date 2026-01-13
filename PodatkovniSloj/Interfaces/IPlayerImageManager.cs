namespace DataLayer.Interfaces
{
    public interface IPlayerImageManager
    {
        void SetPlayerImagePath(string playerIdentifier, string imagePath);
        string? GetPlayerImagePath(string playerIdentifier);
        bool RemovePlayerImagePath(string playerIdentifier);
    }
}
