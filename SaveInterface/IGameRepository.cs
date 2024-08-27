using Domain;
using UnoGame;

namespace SaveInterface;

public interface IGameRepository
{
    void Save(Guid id, GameState state);
    List<(Guid id, DateTime dt)> GetSaveGames();

    GameState LoadGame(Guid id);
}