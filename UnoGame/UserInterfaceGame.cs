using CardSystem;
using PlayerSystem;

namespace UnoGame;

public class UserInterfaceGame
{
    public static void DisplayStartGameMessage()
    {
        Console.WriteLine("GAME IS LOADING....." + "\n");
        Thread.Sleep(2000);
        Console.Clear();
        Console.WriteLine("--------------------GAME-----------------" + "\n");
        Thread.Sleep(1000);
    }
    
    public void AmountOfCardsInEveryRound(GameConfigurations gameConfig)
    {
        string playerInfo = "";
        Console.WriteLine("Cards in the game:");
        Console.WriteLine();
        foreach (Player player in gameConfig.Players)
        {
            playerInfo += $"{player.Name}: {player.PlayerHand.Count}        ";
        }
        Console.WriteLine(playerInfo);
        Console.WriteLine();
    }

    public void DisplayInitialRoundMessage(CardsDealer dealer, List<Player> players)
    {
        Console.WriteLine($"DECK SIZE: {dealer.DeckPile.Count}");
        Console.WriteLine($"Discard SIZE: {dealer.DiscardPile.Count}");
        players.ForEach(each => Console.WriteLine($"{each.Name} : {each.Score}"));
    }
    
    public void DisplayCardInTheGameFirstRound(GameState gameState)
    {
        ConsoleColor defaultBackgroundColor = Console.BackgroundColor;
        Random random = new Random();
        int randomIndex = random.Next(0, gameState.Dealer.DeckPile.Count);
        Card randomCard = gameState.Dealer.DeckPile[randomIndex];
        gameState.CardOnTheTable = randomCard;
        Console.Write("Card on the table: ");
        Console.BackgroundColor = gameState.CardOnTheTable.CardColor == CardColor.Red ? ConsoleColor.DarkRed
            : gameState.CardOnTheTable.CardColor == CardColor.Blue ? ConsoleColor.Blue
            : gameState.CardOnTheTable.CardColor == CardColor.Green ? ConsoleColor.DarkGreen
            : gameState.CardOnTheTable.CardColor == CardColor.Yellow ? ConsoleColor.Yellow
            : ConsoleColor.Magenta;
        Console.WriteLine(gameState.CardOnTheTable.CardColor + " " + gameState.CardOnTheTable.CardValue + "\n", Console.BackgroundColor);
        Console.BackgroundColor = defaultBackgroundColor;
    }
    
    public void DisplayCardInTheGame(Card card)
    {
        ConsoleColor defaultBackgroundColor = Console.BackgroundColor;
        Console.Write("Card on the table: ");
        Console.BackgroundColor = card.CardColor == CardColor.Red ? ConsoleColor.DarkRed
            : card.CardColor == CardColor.Blue ? ConsoleColor.Blue
            : card.CardColor == CardColor.Green ? ConsoleColor.DarkGreen
            : card.CardColor == CardColor.Yellow ? ConsoleColor.Yellow
            : ConsoleColor.Magenta;
        Console.WriteLine(card.CardColor + " " + card.CardValue + "\n", Console.BackgroundColor);
        Console.BackgroundColor = defaultBackgroundColor;
    }
    
    public void DisplayPlayerHand(Player player)
    {
        ConsoleColor defaultBackgroundColor = Console.BackgroundColor;
        Console.WriteLine($"{player.Name}'s Hand: " + "\n");
        for (int i = 0; i < player.PlayerHand.Count; i++)
        {
            Console.BackgroundColor = player.PlayerHand[i].CardColor == CardColor.Red ? ConsoleColor.DarkRed
                : player.PlayerHand[i].CardColor == CardColor.Blue ? ConsoleColor.Blue
                : player.PlayerHand[i].CardColor == CardColor.Green ? ConsoleColor.Green
                : player.PlayerHand[i].CardColor == CardColor.Yellow ? ConsoleColor.Yellow
                : ConsoleColor.Magenta;
            Console.WriteLine((i + 1) +  ". " + player.PlayerHand[i].CardColor + " " + player.PlayerHand[i].CardValue, Console.BackgroundColor);
        }
        Console.BackgroundColor = defaultBackgroundColor;
        Console.WriteLine();
    }
    
    public void ReverseMessage()
    {
        Console.WriteLine("Players will be reversed...");
        Thread.Sleep(5000);
    }

    public void ChooseColorForFirstRoundMessage()
    {
        Console.WriteLine("Choose color you want to play (Red, Blue, Green, Yellow): ");
    }
    public void DrawFourMessageFirstRound()
    {
        Console.WriteLine(
            "MultiColor WildDrawFour card is put back into deck of cards, and new card is put on the table..");
        Thread.Sleep(5000);
    }
    
    public void DrawTwoActionMessage()
    {
        Console.WriteLine("Card on the table is DrawTwo card, so you must take 2 cards and your turn is skipped...");
        Thread.Sleep(5000);
    }
    
    public void SkipActionMessage()
    {
        Console.WriteLine("Card on the table is SkipCard, so your turn is skipped...");
        Thread.Sleep(5000);
    }
    
    public void InvalidMoveMessage()
    {
        Console.WriteLine("<<<<<<Invalid move, try again...>>>>>>");
    }
    
    public void TurnToTheNextPlayerMessage()
    {
        Console.WriteLine("Turn goes to the next player...");
        Thread.Sleep(3000);
    }
    
    public void InvalidSelectionCardMessage()
    {
        Console.WriteLine("<<<<<<Invalid selection. Please choose a valid card.>>>>>>");
        Console.WriteLine();
    }
    
    public void InvalidColorMessage()
    {
        Console.WriteLine("<<<<<<Invalid color choice. Please try again.>>>>>>");
    }
    
    public void ChooseColorMessage()
    {
        Console.WriteLine("Please choose color for next player (Red, Blue, Yellow, Green): ");
    }
    
    public void SelectCardOrSkipMessage()
    {
        Console.WriteLine("Select a card to play or press: s to skip your turn...");
    }
    
    public void DrawFourActionMessage(Player player1, Player player2)
    {
        Console.WriteLine($"Player: {player1.Name} chose WildDrawFour card, so {player2.Name} must take 4 cards and his turn is skipped...");
        Thread.Sleep(5000);
    }
    
    public void ScoreWinnerMessage(Player currentPlayer)
    {
        Console.WriteLine($"Player: {currentPlayer.Name} reached 30 points and won the game!");
    }
    
    public void FirstCardOutWinnerMessage(GameState gameState)
    {
        Console.WriteLine($"{gameState.Winner} won the game! Congratulations!");
        Thread.Sleep(5000);
    }
    
    public void PlayerCalledUnoMessage(Player player)
    {
        Console.WriteLine($"Player: {player.Name} called UNO");
        Thread.Sleep(5000);
    }
    
    public void CardsToTakeForPenaltyMessage(int cardsToTakeForPenalty)
    {
        Console.WriteLine($"You have not called UNO, so you take {cardsToTakeForPenalty}...");
        Thread.Sleep(2000);
    }
    
    public void TakeAdditionalCardMessage(Player player)
    {
        Console.WriteLine(
            $"If you, {player.Name}, want to take an additional card from deck of cards, press: t or...\n" +
            $"Select a card to play:");
    }

    public void CardsShuffleMessage()
    {
        Console.WriteLine("Cards will be shuffled...");
        Thread.Sleep(3000);
    }

    public void ClearAll()
    {
        Console.Clear();
    }
}