using System.Text.Json.Serialization;
using CardSystem;
using PlayerSystem;
using RulesSystem;

namespace UnoGame;

public class GameState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [JsonInclude] public CardsDealer Dealer = new();
    //sozdaem objekt, u kotorogo est Dealer, no on pustoj i emu prisvaivaem konfiguracii vnizu
    [JsonInclude] public GameConfigurations GameConfigurations = new();
    [JsonInclude] public Card SelectedCard = new();
    [JsonInclude] public bool IsValidMove;
    [JsonInclude] public int PlayerIndex = -1;
    [JsonInclude] public int NextPlayerIndex;
    [JsonInclude] public List<Card> AllPlayersCards = new();
    [JsonInclude] public static Random Random = new();
    [JsonInclude] public bool IsFirstRound = true;
    [JsonInclude] public Card CardOnTheTable = new();
    [JsonInclude] public string? Winner;
    [JsonInclude] public bool GameIsOver;
    [JsonIgnore] public UserInterfaceGame UserInterface = new();
    [JsonInclude]public string FileToSave = DateTime.Now.ToString("dddd'-'dd'-'MMMM'-'yyyy'-'HH'-'mm'-'ss");
    [JsonIgnore]public ISaveLoadGame SaveLoadGame = default!;
    
    
    public void RunGame(GameContext context)
    {
        Dealer.GameConfigurations = GameConfigurations;
        Dealer.DealCardsToPlayers();
        UserInterfaceGame.DisplayStartGameMessage();
        PlayGame(context);
    }
    
    public void PlayGame(GameContext context)
    {
        CardOnTheTable = Dealer.DeckPile[0];
        if (!IsActionCard(CardOnTheTable))
        {
            IsFirstRound = false;
        }
        Dealer.DiscardPile.Add(CardOnTheTable);
        Dealer.DeckPile.Remove(CardOnTheTable);
        while (true)
        {
            PlayerIndex = (PlayerIndex + 1) % GameConfigurations.Players.Count;
            Player currentPlayer = GameConfigurations.Players[PlayerIndex];
            if (Dealer.DeckPile.Count == 0)
            {
                 ConvertDiscardPileIntoDeck();
            }
            SaveToJsonFile saveToJsonFile = new SaveToJsonFile();
            saveToJsonFile.SaveGame(Id, this);
            SaveToDataBase saveToDataBase = new SaveToDataBase(context);
            saveToDataBase.SaveGame(Id, this);
            DisplayGameState(currentPlayer, CardOnTheTable);
            if (IsFirstRound && IsActionCard(CardOnTheTable))
            {
                ApplyActionForActionCard(CardOnTheTable, currentPlayer);
                IsFirstRound = false;
                if (currentPlayer.MustSkip)
                {
                    currentPlayer.MustSkip = false;
                    PlayerIndex = (PlayerIndex + 1) % GameConfigurations.Players.Count;
                    continue;
                }
            }
            AskUserToSelectCardToPlay(CardOnTheTable, currentPlayer);
            IsValidMove = CheckIfValidMove(SelectedCard, CardOnTheTable);
            while (!IsValidMove)
            {
                DisplayGameState(currentPlayer, CardOnTheTable);
                UserInterface.InvalidMoveMessage();
                AskUserToSelectCardToPlay(CardOnTheTable, currentPlayer);
                IsValidMove = CheckIfValidMove(SelectedCard, CardOnTheTable);
            }
            if (currentPlayer.PlayerHand.Count == 1)
            {
                GameIsOver = true;
                Winner = currentPlayer.Name;
                break;
            }
            if (IsActionCard(SelectedCard))
            {
                ApplyActionForActionCard(SelectedCard, currentPlayer);
                CardOnTheTable = SelectedCard;
                if (currentPlayer.MustSkip)
                {
                    currentPlayer.MustSkip = false;
                    PlayerIndex = (PlayerIndex + 1) % GameConfigurations.Players.Count;
                }
            }
            CardOnTheTable = SelectedCard;
            AddScoreToPlayerForPlayingCard(currentPlayer, SelectedCard);
            CheckingScoreWinningCondition(currentPlayer);
            currentPlayer.PlayerHand.Remove(SelectedCard);
            Dealer.DiscardPile.Add(SelectedCard);
            IsFirstRound = false;
            UserInterface.ClearAll();
        }
        GameIsOver = true;
        UserInterface.FirstCardOutWinnerMessage(this);
    }

    public void AddScoreToPlayerForPlayingCard(Player currentPlayer, Card selectedCard)
    {
        currentPlayer.Score += GameConfigurations.RuleDictionary?["Scoring"].Equals("Points-based scoring")
            ? GetPointsForCard(selectedCard)
            : 0;
    }

    public void DisplayGameState(Player currentPlayer, Card card)
    {
        UserInterface.DisplayInitialRoundMessage(Dealer, GameConfigurations.Players);
        UserInterface.AmountOfCardsInEveryRound(GameConfigurations);
        UserInterface.DisplayCardInTheGame(card);
        UserInterface.DisplayPlayerHand(currentPlayer);
    }

    public bool CheckIfValidMove(Card selectedCard, Card cardOnTheTable)
    {
        return IsWildCard(selectedCard) || selectedCard.CardColor == cardOnTheTable.CardColor || selectedCard.CardValue == cardOnTheTable.CardValue;
    }

    public bool IsWildCard(Card card)
    {
        return card.CardValue == CardValue.Wild || card.CardValue == CardValue.WildCustomizable || card.CardValue == CardValue.WildDrawFour || card.CardValue == CardValue.WildShuffleHands;
    }

    public bool IsActionCard(Card card)
    {
        return IsWildCard(card) || card.CardValue == CardValue.Skip || card.CardValue == CardValue.Reverse || card.CardValue == CardValue.DrawTwo;
    }
    
    public void AskUserToSelectCardToPlay(Card card, Player player)
    {
        Console.CursorVisible = true;
        if (player.PlayerType is PlayerType.Ai)
        {
            HandleAiPlayerLogic(player);
            return;
        }
        while (true)
        {
            UserInterface.TakeAdditionalCardMessage(player);
            string? input = Console.ReadLine();
            UserInterface.ClearAll();
            if (input?.Equals("t") == true)
            {
                HandleTakeCardAction(player);
                continue;
            }
            if (TryParseInputForCardIndex(input, player, out int cardIndex))
            {
                if (player.PlayerHand.Count == 2)
                {
                    if (input != null && input.Contains("UNO"))
                    {
                        UserInterface.PlayerCalledUnoMessage(player);
                    }
                    else
                    {
                        HandlePenaltyForNotCallingUno(player);
                    }
                }
                SelectedCard = player.PlayerHand[cardIndex];
                break;
            }
            if (input?.Equals("s") == true)
            {
                UserInterface.TurnToTheNextPlayerMessage();
                UserInterface.ClearAll();
                break;
            }
            DisplayGameState(player, card);
            UserInterface.InvalidSelectionCardMessage();
        }
    }
    
    public void HandleTakeCardAction(Player player)
    {
        if (Dealer.DeckPile.Count == 0)
        {
            ConvertDiscardPileIntoDeck();
        }
        player.PlayerHand.Add(Dealer.DeckPile.Last());
        Dealer.DeckPile.RemoveAt(Dealer.DeckPile.Count - 1);
        DisplayGameState(player, CardOnTheTable);
    }

    public bool TryParseInputForCardIndex(string? input, Player player, out int cardIndex)
    {
        if (input != null && int.TryParse(input.Split(' ')[0], out cardIndex) && cardIndex > 0 && cardIndex <= player.PlayerHand.Count)
        {
            cardIndex--;
            return true;
        }
        cardIndex = -1;
        return false;
    }

    public void HandlePenaltyForNotCallingUno(Player player)
    {
        int cardsToTakeForPenalty = GameConfigurations.RuleTypeInput == 1 ? OfficialRules.UnoRules["PenaltyForNotCallingUno"] : CustomRules.UnoRules["PenaltyForNotCallingUno"];
        for (int i = 0; i < cardsToTakeForPenalty; i++)
        {
            if (Dealer.DeckPile.Count == 0)
            {
                ConvertDiscardPileIntoDeck();
            }
            player.PlayerHand.Add(Dealer.DeckPile[0]);
            Dealer.DeckPile.RemoveAt(0);
        }
        Dealer.DiscardPile.Add(SelectedCard);
        UserInterface.CardsToTakeForPenaltyMessage(cardsToTakeForPenalty);
    }

    public void ConvertDiscardPileIntoDeck()
    {
        Dealer.DiscardPile.ForEach(each =>
        {
            each.CardColor = (each.CardValue == CardValue.WildShuffleHands || each.CardValue == CardValue.WildCustomizable ||
                each.CardValue == CardValue.WildDrawFour || each.CardValue == CardValue.Wild)
                ? CardColor.Multicolor
                : each.CardColor;
        });
        Dealer.DiscardPile = Dealer.ShuffleCards(Dealer.DiscardPile);
        Dealer.DeckPile = new List<Card>(Dealer.DiscardPile);
        Dealer.DiscardPile.Clear();
    }

    public void ApplyActionForActionCard(Card card, Player player)
    {
        int nextPlayerIndex = (PlayerIndex + 1) % GameConfigurations.Players.Count;
        Player nextPlayer = GameConfigurations.Players[nextPlayerIndex];
        switch (card.CardValue)
        {
            case CardValue.Skip:
                UserInterface.SkipActionMessage();
                if (IsFirstRound)
                {
                    PlayerIndex = GameConfigurations.Players.IndexOf(player) - 1;
                }
                player.MustSkip = true;
                break;
            case CardValue.Reverse:
                PlayerIndex = GameConfigurations.Players.IndexOf(player) - 1;
                UserInterface.ReverseMessage();
                GameConfigurations.Players.Reverse();
                break;
            case CardValue.DrawTwo:
                if (IsFirstRound)
                {
                    DrawCardsToPlayer(player, 2);
                    PlayerIndex = GameConfigurations.Players.IndexOf(player) - 1;
                }
                else
                {
                    DrawCardsToPlayer(nextPlayer, 2);
                }
                UserInterface.DrawTwoActionMessage();
                player.MustSkip = true;
                break;
            case CardValue.Wild:
            case CardValue.WildCustomizable:
                AskUserForNextCardColor(card, player);
                break;
            case CardValue.WildDrawFour:
                if (IsFirstRound)
                {
                    UserInterface.DrawFourMessageFirstRound();
                    Dealer.DeckPile.Add(CardOnTheTable);
                    Dealer.DiscardPile.Remove(CardOnTheTable);
                    CardOnTheTable = Dealer.DeckPile[0];
                    Dealer.DeckPile.Remove(CardOnTheTable);
                    Dealer.DiscardPile.Add(CardOnTheTable);
                    UserInterface.ClearAll();
                    UserInterface.DisplayInitialRoundMessage(Dealer, GameConfigurations.Players);
                    UserInterface.AmountOfCardsInEveryRound(GameConfigurations);
                    UserInterface.DisplayCardInTheGameFirstRound(this);
                    UserInterface.DisplayPlayerHand(player);
                    IsFirstRound = false;
                }
                else
                {
                    AskUserForNextCardColor(card, player);
                    DrawCardsToPlayer(nextPlayer, 4);
                    UserInterface.DrawFourActionMessage(player, nextPlayer);
                    player.MustSkip = true;
                }
                break;
            case CardValue.WildShuffleHands:
                AskUserForNextCardColor(card, player);
                UserInterface.CardsShuffleMessage();
                GameConfigurations.Players.ForEach(currentPlayer =>
                {
                    AllPlayersCards.AddRange(currentPlayer.PlayerHand);
                    currentPlayer.PlayerHand.Clear();
                    
                });
                AllPlayersCards = Dealer.ShuffleCards(AllPlayersCards);
                DealCardsToPlayersAfterShuffle();
                UserInterface.ClearAll();
                DisplayGameState(player, CardOnTheTable);
                break;
        }
    }
    
     public void DealCardsToPlayersAfterShuffle()
     {
         while (AllPlayersCards.Count > 0)
         {
             foreach (Player gamer in GameConfigurations.Players)
             {
                 if (AllPlayersCards.Count == 0)
                 {
                     break;
                 }
                 gamer.PlayerHand.Add(AllPlayersCards[0]);
                 AllPlayersCards.RemoveAt(0);
             }
         }
         AllPlayersCards.ForEach(c => c.WasPlayed = false);
     }
    
    public void AskUserForNextCardColor(Card card, Player player)
    {
        if (player.PlayerType is PlayerType.Ai)
        {
            ChooseColorForAi(card);
            return;
        }
        if (IsFirstRound)
        {
            UserInterface.ChooseColorForFirstRoundMessage();
        }
        else
        {
            UserInterface.ChooseColorMessage();
        }
        
        while (true)
        {
            string? input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input) && Enum.IsDefined(typeof(CardColor), input))
            {
                card.CardColor = (CardColor)Enum.Parse(typeof(CardColor), input);
                if (IsFirstRound)
                {
                    UserInterface.ClearAll();
                    DisplayGameState(player, CardOnTheTable);
                    IsFirstRound = false;
                }
                break;
            }
            {
                UserInterface.InvalidColorMessage();
            }
        }
    }

    public void ChooseColorForAi(Card card)
    {
        List<CardColor> colors = new List<CardColor> { CardColor.Red, CardColor.Blue, CardColor.Green, CardColor.Yellow };
        int randomIndex = Random.Next(colors.Count);
        card.CardColor = colors[randomIndex];
    }
    
    public void DrawCardsToPlayer(Player player, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (Dealer.DeckPile.Count == 0)
            {
                ConvertDiscardPileIntoDeck();
            }
            player.PlayerHand.Add(Dealer.DeckPile.Last());
            Dealer.DeckPile.Remove(Dealer.DeckPile.Last());
        }
    }
    
    public void CheckingScoreWinningCondition(Player currentPlayer)
    {
        if (currentPlayer.Score >= 30)
        {
            GameIsOver = true;
            UserInterface.ScoreWinnerMessage(currentPlayer);
        }
    }

    public int GetPointsForCard(Card card)
    {
        return IsActionCard(card)
            ? GameConfigurations.RuleDictionary?["CardValues"]["SpecialCards"][card.CardValue.ToString()]
            : GameConfigurations.RuleDictionary?["CardValues"]["NumberCards"][card.CardValue.ToString()];
    }
    
    public void HandleAiPlayerLogic(Player aiPlayer)
    {
        List<Card> validCards = new();
        foreach (var aiPlayerCard in aiPlayer.PlayerHand)
        {
            if (CheckIfValidMove(aiPlayerCard, CardOnTheTable))
            {
                validCards.Add(aiPlayerCard);
            }
        }
        if (validCards.Count > 0)
        {
            SelectedCard = validCards[0];
            CardOnTheTable = SelectedCard;
            if (IsActionCard(SelectedCard))
            {
                ApplyActionForActionCard(SelectedCard, aiPlayer);
            }
            aiPlayer.PlayerHand.Remove(SelectedCard);
            
            if (aiPlayer.PlayerHand.Count == 1)
            {
                GameIsOver = true;
                Winner = aiPlayer.Name;
                UserInterface.FirstCardOutWinnerMessage(this);
            }
        }
        else
        {
            HandleTakeCardAction(aiPlayer);
        }
    }
}