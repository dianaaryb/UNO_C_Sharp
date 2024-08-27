using PlayerSystem;
using RulesSystem;

namespace UnoGame;

public class StartMenu
{
    public GameConfigurations Configurations = new();

    public void DrawMenuPlayersCount(GameContext context)
    {
        Console.WriteLine("Settings");
        Console.WriteLine(Menu.MenuSeparator);
        RulesTypeInput();
        Console.Clear();
        PlayersInput();
        Console.Clear();
        HumanPlayersInput();
        Console.WriteLine("Your choice:" + "\n" + Menu.MenuSeparator + "\n" + Configurations.TotalPlayers + " players\nHuman players: " + Configurations.HumanPlayers + "\n"
                          + "AI players: " + (Int32.Parse(Configurations.TotalPlayers) - Int32.Parse(Configurations.HumanPlayers)));
        Thread.Sleep(500);
        NamesOfHumanPlayers();
        Thread.Sleep(500);
        NamesOfAiPlayers();
        Thread.Sleep(500);
        Thread.Sleep(500);
        Console.WriteLine(Menu.MenuSeparator);
        Action(context);
    }

    public void HumanPlayersInput()
    {
        Console.WriteLine("Settings");
        Console.WriteLine(Menu.MenuSeparator);
        int maxPlayers = int.Parse(Configurations.TotalPlayers);

        Console.WriteLine($"Choose the number of human players (1 - {maxPlayers} included): ");
        var humanPlayerOptions = Enumerable.Range(1, maxPlayers).Select(p => p.ToString()).ToList();
        humanPlayerOptions.AddRange(new[] { "BACK", "EXIT" });
        var humanPlayerCommands = Enumerable.Range(1, maxPlayers).Select(p => p.ToString()).ToList();
        humanPlayerCommands.AddRange(new[] { "b", "x" });
        string selectedCommand = new MenuNavigatorArrows(humanPlayerOptions, humanPlayerCommands, 3).NavigateAndSelect();
        switch (selectedCommand)
        {
            case "b":
                new Menu().Draw();
                break;
            case "x":
                Console.Clear();
                Console.WriteLine("<<<<<<Goodbye!!!>>>>>>");
                Environment.Exit(0);
                break;
            default:
                if (int.TryParse(selectedCommand, out int humanPlayersCount))
                {
                    Configurations.HumanPlayers = selectedCommand;
                    Console.Clear();
                }
                break;
        }
    }
    
    public void PlayersInput()
    {
        Console.WriteLine("Settings");
        Console.WriteLine(Menu.MenuSeparator);
        int maxPlayers = Configurations.RuleTypeInput == 1
            ? OfficialRules.UnoRules["GameRules"]["MaximumPlayers"] : CustomRules.UnoRules["GameRules"]["MaximumPlayers"];
        Console.WriteLine($"Choose number of players (2 - {maxPlayers} included): ");
        var playerOptions = Enumerable.Range(2, maxPlayers - 1).Select(p => p.ToString()).ToList();
        playerOptions.AddRange(new[] { "BACK", "EXIT" });
        var playerCommands = playerOptions.ToList();
        playerCommands[playerCommands.IndexOf("BACK")] = "b";
        playerCommands[playerCommands.IndexOf("EXIT")] = "x";

        string selectedCommand = new MenuNavigatorArrows(playerOptions, playerCommands, 3).NavigateAndSelect();
        switch (selectedCommand)
        {
            case "b":
                new Menu().Draw();
                break;
            case "x":
                Console.Clear();
                Console.WriteLine("<<<<<<Goodbye!!!>>>>>>");
                Environment.Exit(0);
                break;
            default:
                if (int.TryParse(selectedCommand, out int playersCount))
                {
                    Configurations.TotalPlayers = selectedCommand;
                    Console.Clear();
                }
                break;
        }
    }
    
    int AiPlayersCount()
    {
        return Int32.Parse(Configurations.TotalPlayers) - Int32.Parse(Configurations.HumanPlayers);
    }

    void RulesTypeInput()
    {
        Console.WriteLine("Choose type of rules or another option:\n");
        var ruleOptions = new List<string> { "Official rules", "Custom rules", "BACK", "EXIT" };
        var ruleCommands = new List<string> { "1", "2", "b", "x" };
        var ruleNavigator = new MenuNavigatorArrows(ruleOptions, ruleCommands, 3);
        string selectedCommand = ruleNavigator.NavigateAndSelect();
        switch (selectedCommand)
        {
            case "1":
                SetRuleTypeAndDisplay(1, "Official rules", OfficialRules.UnoRules);
                Configurations.HaveToShowPoints = false;
                break;
            case "2":
                SetRuleTypeAndDisplay(2, "Custom rules", CustomRules.UnoRules);
                Configurations.HaveToShowPoints = true;
                break;
            case "b":
                new Menu().Draw();
                break;
            case "x":
                Console.WriteLine("<<<<<<Goodbye!!!>>>>>>");
                Environment.Exit(0);
                break;
        }
    }
    
    void SetRuleTypeAndDisplay(int type, string ruleName, Dictionary<string, object> ruleDictionary)
    {
        Configurations.RuleTypeInput = type;
        Configurations.RuleDictionary = ruleDictionary;
        Console.WriteLine($"Chosen Rule type: {ruleName}\n");
        Thread.Sleep(1000);
    }


    public void Action(GameContext context)
    {
        Console.Clear();
        Console.WriteLine("Select an action: ");
        Console.WriteLine(Menu.MenuSeparator);
        var actionOptions = new List<string> { "START GAME", "BACK", "EXIT" };
        var actionCommands = new List<string> { "s", "b", "x" };
        var actionNavigator = new MenuNavigatorArrows(actionOptions, actionCommands, 2);

        string selectedCommand = actionNavigator.NavigateAndSelect();
        Console.Clear();
        if (selectedCommand == "s")
        {
            GameState gameState = new GameState();
            gameState.GameConfigurations = Configurations;
            gameState.RunGame(context);
        }
        else if (selectedCommand == "b")
        {
            new Menu().Draw();
        }
        else if (selectedCommand == "x")
        {
            Console.WriteLine("<<<<<<Goodbye!!!>>>>>>");
            Environment.Exit(0);
        }
    }
    
    public void NamesOfHumanPlayers()
    {
        Console.CursorVisible = true;
        for (int i = 1; i <= Int32.Parse(Configurations.HumanPlayers); i++)
        {
            Console.WriteLine($"Enter the name for Player{i}: ");
            string? playerName = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(playerName))
            {
                Console.WriteLine("<<<<<<Invalid name of player...>>>>>>");
                Thread.Sleep(2000);
                Console.Clear();
                NamesOfHumanPlayers();
            }
            
            if (!string.IsNullOrEmpty(playerName))
            {
                Configurations.Players.Add(new Player(PlayerType.Human, playerName));
            }
        }
        Console.CursorVisible = false;
    }

    public void NamesOfAiPlayers()
    {
        for (int i = 0; i < AiPlayersCount(); i++)
        {
            Configurations.Players.Add(new Player(PlayerType.Ai, $"AiPlayer{i + 1}"));
        }
        Configurations.Players.ForEach(item => Console.WriteLine(item.ToString()));
    }
}