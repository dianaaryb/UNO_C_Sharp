using System.Text.Json;
using Domain.Database;
using Helpers;
using Microsoft.EntityFrameworkCore;
using RulesSystem;

namespace UnoGame;

public class SaveToDataBase: ISaveLoadGame
{
    private readonly GameContext _gameContext = default!;

    public SaveToDataBase(GameContext gameContext)
    {
        _gameContext = gameContext;
    }

    public void SaveGame(Guid id, GameState state)
    {
        // Check if the game is already in the database
        var game = _gameContext.Games.FirstOrDefault(g => g.Id == id);
        if (game == null)
        {
            // If the game is not found, create a new Game entity
            game = new Game()
            {
                Id = id,
                GameState = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions),
                Players = state.GameConfigurations.Players.Select(each => new PlayerToDb()
                {
                    Id = each.Id,
                    Name = each.Name,
                    PlayerType = each.PlayerType
                }).ToList()
                // Assuming the Dealer object contains the list of players
            };
            _gameContext.Games.Add(game);
        }
        else
        {
            // If the game is found, update the existing entity
            game.UpdatedAtDt = DateTime.Now;
            game.GameState = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);
            // Update the players list if necessary, this would involve more complex logic
            // to compare current players with the new list and update accordingly.
        }
        _gameContext.SaveChanges();
    }

    public List<(Guid id, DateTime dateTime)> GetSavedGames()
    {
        return _gameContext.Games
            .OrderByDescending(g => g.UpdatedAtDt)
            .ToList()
            .Select(g => (g.Id, g.UpdatedAtDt))
            .ToList();
    }

    public GameState LoadGame(Guid id)
    {
        var loadedGame = _gameContext.Games.FirstOrDefault(game => game.Id == id);
        if (loadedGame == null)
        {
            Console.WriteLine("Game not found.");
            return null!;
        }

        if (string.IsNullOrWhiteSpace(loadedGame.GameState))
        {
            Console.WriteLine("Game state data is null or empty.");
            return null!;
        }

        var loadedGameState = JsonSerializer.Deserialize<GameState>(loadedGame.GameState, JsonHelpers.JsonSerializerOptions);
        if (loadedGameState?.GameConfigurations == null)
        {
            Console.WriteLine("Failed to load game state or game configurations.");
            return null!;
        }

        try
        {
            var jsonDocument = JsonDocument.Parse(loadedGame.GameState);
            var ruleType = jsonDocument.RootElement.GetProperty("gameConfigurations").GetProperty("ruleDictionaryType").GetString();
            loadedGameState.GameConfigurations.RuleDictionary = ruleType!.Equals("Official") ? OfficialRules.UnoRules : CustomRules.UnoRules;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while parsing the game state JSON: {ex.Message}");
            return null!;
        }
        return loadedGameState;
    }
}