using System.Text.Json;
using System.Threading.Channels;
using RulesSystem;

namespace UnoGame;

public class SaveToJsonFile: ISaveLoadGame
{
    public string Location = "/Users/di/RiderProjects/icd0008-23f/UNO/SaveLoadGameJson/SavedGames";
    public void SaveGame(Guid id, GameState gameState)
    {
        string json = JsonSerializer.Serialize(gameState, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText($"{Location}/{id.ToString()}.json", json);
    }
    
    public GameState LoadGame(Guid id)
    {
        string fileToLoad = Path.ChangeExtension(id.ToString(), ".json");
        string jsonContent = File.ReadAllText(Location + $"/{fileToLoad}");
       try
       {
           GameState loadedGameState = JsonSerializer.Deserialize<GameState>(jsonContent) ?? new GameState();
           loadedGameState.FileToSave = Path.GetFileName(fileToLoad.Replace(".json",""));
           JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);

           string? ruleDictionaryType = jsonDocument.RootElement
               .GetProperty("GameConfigurations")
               .GetProperty("RuleDictionaryType").GetString();
           loadedGameState.GameConfigurations.RuleDictionary = ruleDictionaryType == "OfficialRules"
               ? OfficialRules.UnoRules
               : CustomRules.UnoRules;
           loadedGameState.Dealer.GameConfigurations.RuleDictionary = ruleDictionaryType == "OfficialRules"
               ? OfficialRules.UnoRules
               : CustomRules.UnoRules;
           Console.Clear();
           return loadedGameState;
       }
       catch(JsonException e)
       {
           Console.Clear();
           Console.WriteLine(jsonContent);
           Thread.Sleep(10000);
       }
       return new GameState();
    }
    
    public List<(Guid id, DateTime dateTime)> GetSavedGames()
    { 
        return Directory.EnumerateFiles("/Users/di/RiderProjects/icd0008-23f/UNO/SaveLoadGameJson/SavedGames")
            .Select(path => (Guid.Parse(Path.GetFileNameWithoutExtension(path)), File.GetLastWriteTime(path)))
            .ToList();
    }
}