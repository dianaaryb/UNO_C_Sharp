namespace UnoGame;

public interface ISaveLoadGame
{
    void SaveGame(Guid id, GameState gameState);

    GameState LoadGame(Guid id);
    
    List<(Guid id, DateTime dateTime)> GetSavedGames();
}