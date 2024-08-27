using System.Text.Json;
using RulesSystem;

namespace UnoGame;

public class LoadGameMenu
{
    public static SaveToDataBase SaveToDataBase = default!;
    public static string LoadFromDataBaseOrJsonFile()
    {
        Console.WriteLine("Please choose source of loading saved game: ");
        var loadOptions = new List<string> { "From JSON file", "From DataBase"};
        var loadCommands = new List<string> { "1", "2" };
        var loadNavigator = new MenuNavigatorArrows(loadOptions, loadCommands, 2);
        string selectedCommand = loadNavigator.NavigateAndSelect();
        return selectedCommand;
    }

    public static void LoadingLastSavedGame(GameContext context)
    {
        var selectedCommand = LoadFromDataBaseOrJsonFile();
        switch (selectedCommand)
        {
            case "1":
                Console.Clear();
                LoadLastSavedGameJsonFile(context);
                break;
            case "2":
                Console.Clear();
                LoadLastSavedGameDataBase(context);
                break;
        }
    }
    
    public static void LoadingListOfGames(GameContext context)
    {
        var selectedCommand = LoadFromDataBaseOrJsonFile();
        UserChoice(selectedCommand, context);
    }

    public static void UserChoice(string input, GameContext context)
    {
        switch (input)
        {
            case "1":
                Console.Clear();
                LoadSelectedGameFromJson(context);
                break;
            case "2":
                Console.Clear();
                LoadSelectedGameFromDataBase(context);
                break;
        }
    }

    public static void LoadSelectedGameFromDataBase(GameContext context)
    {
        Console.CursorVisible = true;
        Console.WriteLine("List of saved games:");
        Console.WriteLine(Menu.MenuSeparator);
        List<(Guid, DateTime)> savedGames = SaveToDataBase.GetSavedGames();
        for (int i = 0; i < savedGames.Count; i++)
        {
            Console.WriteLine($"{i + 1}. Game saved on {savedGames[i].Item2}");
        }
        Console.WriteLine("b. BACK");
        Console.WriteLine("x. EXIT");
        Console.Write("Enter your choice: ");
        string selectedCommand = Console.ReadLine()!;
        if (selectedCommand == "b")
        {
            Console.Clear();
            new Menu().Draw();
        }
        else if (selectedCommand == "x")
        {
            Console.Clear();
            Console.WriteLine("<<<<<<Goodbye!!!>>>>>>");
            Environment.Exit(0);
        }
        else if (int.TryParse(selectedCommand, out int fileNumber) && fileNumber > 0 && fileNumber <= savedGames.Count)
        {
            Console.WriteLine($"You selected file: {savedGames[fileNumber - 1].Item2}");
            Thread.Sleep(3000);
            GameState gameFromDb = SaveToDataBase.LoadGame(savedGames[fileNumber - 1].Item1);
            gameFromDb.PlayGame(context);
        }
        else
        {
            Console.WriteLine("Invalid selection. Please try again.");
        }
    }
    
    public static void LoadSelectedGameFromJson(GameContext context)
    {
        Console.CursorVisible = true;
        Console.WriteLine("List of saved games:");
        Console.WriteLine(Menu.MenuSeparator);
        SaveToJsonFile saveToJsonFile = new SaveToJsonFile();
        List<(Guid, DateTime)> savedGames = saveToJsonFile.GetSavedGames();
        for (int i = 0; i < savedGames.Count; i++)
        {
            Console.WriteLine($"{i + 1}. Game saved on {savedGames[i].Item2}");
        }
        Console.WriteLine("b. BACK");
        Console.WriteLine("x. EXIT");
        Console.Write("Enter your choice: ");
        string selectedCommand = Console.ReadLine()!;
        if (selectedCommand == "b")
        {
            Console.Clear();
            new Menu().Draw();
        }
        else if (selectedCommand == "x")
        {
            Console.Clear();
            Console.WriteLine("<<<<<<Goodbye!!!>>>>>>");
            Environment.Exit(0);
        }
        else if (int.TryParse(selectedCommand, out int fileNumber) && fileNumber > 0 && fileNumber <= savedGames.Count)
        {
            Console.WriteLine($"You selected file: {savedGames[fileNumber - 1].Item2}");
            Thread.Sleep(3000);
            GameState gameFromJson = saveToJsonFile.LoadGame(savedGames[fileNumber - 1].Item1);
            gameFromJson.PlayGame(context);
        }
        else
        {
            Console.WriteLine("Invalid selection. Please try again.");
        }
    }
    
    public static void LoadLastSavedGameDataBase(GameContext context)
    {
        Console.WriteLine("Last game is loading...");
        Thread.Sleep(3000);
        List<(Guid, DateTime)> savedGames = SaveToDataBase.GetSavedGames();
        var mostRecent = savedGames.OrderByDescending(g => g.Item2).First();
        SaveToDataBase.LoadGame(mostRecent.Item1);
    }
    
    public static void LoadLastSavedGameJsonFile(GameContext context)
    {
        Console.WriteLine("Last game is loading...");
        Thread.Sleep(3000);
        SaveToJsonFile saveToJsonFile = new SaveToJsonFile();
        List<(Guid, DateTime)> savedGames = saveToJsonFile.GetSavedGames();
        var mostRecent = savedGames.OrderByDescending(g => g.Item2).First();
        saveToJsonFile.LoadGame(mostRecent.Item1);
    }
}