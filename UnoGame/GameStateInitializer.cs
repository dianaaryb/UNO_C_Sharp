using PlayerSystem;

namespace UnoGame;

public class GameStateInitializer
{
    public void CreatePlayers(GameConfigurations gameConfigurations)
    {
        for (int i = 0; i < Int32.Parse(gameConfigurations.HumanPlayers!); i++)
        {
            gameConfigurations.Players.Add(new Player(PlayerType.Human, gameConfigurations.PlayersList[i]));
        }
        for (int i = Int32.Parse(gameConfigurations.HumanPlayers!); i < Int32.Parse(gameConfigurations.TotalPlayers!); i++)
        {
            gameConfigurations.Players.Add(new Player(PlayerType.Ai, gameConfigurations.PlayersList[i]));
        }
    }
}