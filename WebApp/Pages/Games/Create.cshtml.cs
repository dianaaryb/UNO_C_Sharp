using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RulesSystem;
using UnoGame;

namespace WebApp.Pages.Games
{
    public class CreateModel : PageModel
    {
        private readonly GameContext _context;
        [BindProperty] public Game GameToDb { get; set; } = default!;
        [BindProperty] public string PlayerNames { get; set; } = default!;
        private ISaveLoadGame _saveLoadGame = default!;

        public CreateModel(GameContext context)
        {
            _context = context;
        }

        [BindProperty] public GameConfigurations GameConfiguration { get; set; } = new GameConfigurations();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            GameConfiguration.RuleDictionary = GameConfiguration.RuleDictionaryType switch
            {
                "Official" => OfficialRules.UnoRules,
                "Custom" => CustomRules.UnoRules,
                _ => GameConfiguration.RuleDictionary
            };
            _saveLoadGame = GameConfiguration.SaveLocation switch
            {
                SaveLocation.Database => new SaveToDataBase(_context),
                SaveLocation.FileSystem => new SaveToJsonFile(),
                _ => _saveLoadGame
            };
            var playerNamesList = PlayerNames.Split(',')
                .Select(name => name.Trim())
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();
            playerNamesList.ForEach(each => GameConfiguration.PlayersList.Add(each));
            GameState gameState = new GameState();
            gameState.Dealer.GameConfigurations = GameConfiguration;
            gameState.GameConfigurations = GameConfiguration;
            GameStateInitializer initializer = new();
            gameState.PlayerIndex = 0;
            initializer.CreatePlayers(gameState.GameConfigurations);
            gameState.Dealer.DealCardsToPlayers();
            gameState.SaveLoadGame = _saveLoadGame;
            gameState.CardOnTheTable = gameState.Dealer.DeckPile[0];
            gameState.Dealer.DiscardPile.Add(gameState.CardOnTheTable);
            gameState.Dealer.DeckPile.Remove(gameState.CardOnTheTable);
            _saveLoadGame.SaveGame(gameState.Id, gameState);
            return RedirectToPage("/Play/Index", new { gameId = gameState.Id, playerId = gameState.GameConfigurations.Players.FirstOrDefault()!.Id, Saver = GameConfiguration.SaveLocation.ToString() });
        }
    }
}
