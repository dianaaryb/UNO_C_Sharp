using CardSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlayerSystem;
using UnoGame;

namespace WebApp.Pages.Play;

public class Index : PageModel
{
    private readonly GameContext _context;
    public ISaveLoadGame SaveLoadGame;
    [BindProperty(SupportsGet = true)] public Guid GameId { get; set; }
    [BindProperty(SupportsGet = true)] public Guid PlayerId { get; set; }
    public GameState GameState { get; set; } = default!;
    [BindProperty(SupportsGet = true)] public string? Saver { get; set; }
    public List<Card> PlayerCardsInGame = default!;

    public Index(GameContext context)
    {
        _context = context;
        SaveLoadGame = new SaveToDataBase(_context);
    }
    
    public void OnGet()
    {
        SaveLoadGame = Saver!.Equals("Database") ? new SaveToDataBase(_context) : new SaveToJsonFile();
        InitializeGameState();
        var currentPlayer = GameState.GameConfigurations.Players[GameState.PlayerIndex];
        ApplyActionCardForPlayer(currentPlayer, GameState.CardOnTheTable);
        if (currentPlayer.MustSkip)
        {
            GameState.PlayerIndex = (GameState.PlayerIndex + 1) % GameState.GameConfigurations.Players.Count;
            GameState.SaveLoadGame.SaveGame(GameState.Id, GameState);
            return;
        }
        if (currentPlayer.PlayerType == PlayerType.Ai)
        {
            if (currentPlayer.PlayerHand.Count == 1 && GameState.IsValidMove ||  currentPlayer.PlayerHand.Count == 0 )
            {
                GameState.GameIsOver = true;
            }
            GameState.HandleAiPlayerLogic(currentPlayer);
            PlayRound(currentPlayer);
            currentPlayer.MustSkip = false;
            GameState.PlayerIndex = (GameState.PlayerIndex + 1) % GameState.GameConfigurations.Players.Count;
        }
        GameState.SaveLoadGame.SaveGame(GameState.Id, GameState);
    }

    public void InitializeGameState()
    {
        SaveLoadGame = Saver!.Equals("Database") ? new SaveToDataBase(_context) : new SaveToJsonFile();
        GameState = SaveLoadGame.LoadGame(GameId);
        GameState.SaveLoadGame = SaveLoadGame;
        PlayerCardsInGame = GameState.GameConfigurations.Players.Find(p => p.Id == PlayerId)!.PlayerHand;
        GameState.SaveLoadGame.SaveGame(GameState.Id, GameState);
    }

    public IActionResult OnPostTake()
    {
        InitializeGameState();
        GameState.HandleTakeCardAction(GameState.GameConfigurations.Players[GameState.PlayerIndex]);
        GameState?.SaveLoadGame.SaveGame(GameState.Id, GameState);
        return RedirectToPage(new { GameId = GameId, PlayerId = PlayerId, Saver = Saver});
    }

    public IActionResult OnGetPlayCard(Guid gameId, Guid playerId, int cardIndex, string saver)
    {
        Saver = saver;
        GameId = gameId;
        PlayerId = playerId;
        InitializeGameState();
        Player currentPlayer = GameState.GameConfigurations.Players.Find(p => p.Id == playerId)!;
        if (!GameState.IsValidMove && !GameState.IsFirstRound)
        {
            GameState.SaveLoadGame.SaveGame(GameId, GameState);
            return RedirectToPage(new { GameId, PlayerId, Saver });
        }
        else{
            if (currentPlayer.PlayerHand.Count == 1 && GameState.IsValidMove || currentPlayer.PlayerHand.Count == 0)
            {
                GameState.GameIsOver = true;
            }
            GameState.CardOnTheTable = currentPlayer.PlayerHand[cardIndex];
            
            PlayRound(currentPlayer);
            currentPlayer.PlayerHand.RemoveAt(cardIndex);
            GameState.Dealer.DiscardPile.Add(GameState.CardOnTheTable);
            if (currentPlayer.PlayerHand.Count == 1 && GameState.IsValidMove || currentPlayer.PlayerHand.Count == 0)
            {
                GameState.GameIsOver = true;
            }
            currentPlayer.MustSkip = false;
            GameState.PlayerIndex = (GameState.PlayerIndex + 1) % GameState.GameConfigurations.Players.Count;
            GameState.SaveLoadGame.SaveGame(GameId, GameState);
            return RedirectToPage(new { GameId, PlayerId, Saver });
        }
    }
    
    public IActionResult OnPostSayUno()
    {
        InitializeGameState();
        GameState.GameConfigurations.Players.Find(p => p.Id == PlayerId)!.SaidUno = true;
        GameState.SaveLoadGame.SaveGame(GameId, GameState);
        return RedirectToPage(new { GameId = GameId, PlayerId = PlayerId, Saver });
    }
    
    public void PlayRound(Player player)
    {
        if (GameState.Dealer.DeckPile.Count == 0) GameState.ConvertDiscardPileIntoDeck();
        Card userSelectedCard = GameState.SelectedCard;
        Card activeCard = GameState.CardOnTheTable;
        switch (activeCard.CardValue)
        {
            case CardValue.Reverse:
                activeCard.WasPlayed = true;
                GameState.PlayerIndex = GameState.GameConfigurations.Players.IndexOf(player) - 1;
                GameState.UserInterface.ReverseMessage();
                GameState.GameConfigurations.Players.Reverse();
                break;
            case CardValue.Wild or CardValue.WildDrawFour or CardValue.WildCustomizable or CardValue.WildShuffleHands:
            {
                if (activeCard.CardValue is CardValue.WildShuffleHands)
                {
                    GameState.GameConfigurations.Players.ForEach(currentPlayer =>
                    {
                        GameState.AllPlayersCards.AddRange(currentPlayer.PlayerHand);
                        currentPlayer.PlayerHand.Clear();
                    
                    });
                    GameState.Dealer.ShuffleCards(GameState.AllPlayersCards);
                    GameState.DealCardsToPlayersAfterShuffle();
                }
                activeCard.WasPlayed = activeCard.CardValue is CardValue.Wild;
                var values = Enum.GetValues(typeof(CardColor))
                    .Cast<CardColor>()
                    .Where(c => c != CardColor.Multicolor)
                    .ToArray();
                activeCard.CardColor = (CardColor)values.GetValue(new Random().Next(values.Length))!;
                break;
            }
        }
        GameState.AddScoreToPlayerForPlayingCard(player, userSelectedCard);
        GameState.SaveLoadGame.SaveGame(GameId, GameState);
    }
    
    public void ApplyActionCardForPlayer(Player player, Card activeCard)
    {
        if(activeCard.WasPlayed) return;
        switch (activeCard.CardValue) 
        {
            case CardValue.Skip:
                GameState.PlayerIndex = (GameState.PlayerIndex + 1) % GameState.GameConfigurations.Players.Count;
                break;
            case CardValue.DrawTwo:
                GameState.DrawCardsToPlayer(player, 2);
                break;
            case CardValue.WildDrawFour:
                activeCard.WasPlayed = true;
                GameState.DrawCardsToPlayer(player, 4);
                break;
            case CardValue.WildShuffleHands:
                GameState.DealCardsToPlayersAfterShuffle();
                break;
        }
        activeCard.WasPlayed = true;
    }
}