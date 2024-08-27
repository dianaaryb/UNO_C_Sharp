using Microsoft.EntityFrameworkCore;

namespace UnoGame;
public class Menu
{
    public const string MenuSeparator = "--------------------------------------------";
    
    public void Draw()
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine("MAIN MENU");
        Console.WriteLine(MenuSeparator);
        var menuOptions = new List<string> { "NEW GAME", "CONTINUE", "LOAD GAME", "EXIT" };
        var menuCommands = new List<string> { "s", "c", "l", "x" };
        var menuNavigator = new MenuNavigatorArrows(menuOptions, menuCommands, 2);
        string selectedCommand = menuNavigator.NavigateAndSelect();
        UserInput(selectedCommand);
    }

    public void UserInput(string input)
    {
        var connectionString = "DataSource=<%temppath%>uno.db;Cache=Shared".Replace("<%temppath%>", Path.GetTempPath());
    
        var contextOptions = new DbContextOptionsBuilder<GameContext>()
            .UseSqlite(connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;
        using var db = new GameContext(contextOptions);
        // apply all the migrations
        db.Database.Migrate();
        
        switch (input)
        {
            case "s":
                Console.Clear();
                new StartMenu().DrawMenuPlayersCount(db);
                break;
            case "c":
                Console.Clear();
                LoadGameMenu.LoadingLastSavedGame(db);
                break;
            case "l":
                Console.Clear();
                LoadGameMenu.SaveToDataBase = new SaveToDataBase(db);
                LoadGameMenu.LoadingListOfGames(db);
                break;
            case "x":
                Console.Clear();
                Console.WriteLine("<<<<<<Goodbye!!!>>>>>>");
                Environment.Exit(0);
                break;
        }
    }
}