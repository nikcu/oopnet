namespace DataLayer.Interfaces
{
    public interface IFavouritePlayersManager
    {
        IReadOnlyList<string> FavouritePlayersList { get; }
        bool AddFavouritePlayer(string playerIdentifier);
        bool RemoveFavouritePlayer(string playerIdentifier);
        bool IsFavouritePlayer(string playerIdentifier);
        void ClearFavouritePlayers();
    }
}
