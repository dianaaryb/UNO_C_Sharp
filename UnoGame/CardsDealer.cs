using System.Text.Json.Serialization;
using CardSystem;
using RulesSystem;

namespace UnoGame;

public class CardsDealer
{
    [JsonInclude]
    public int CountStatingHandSize = 0;
    [JsonInclude]
    public List<Card> DeckPile = new();
    [JsonInclude]
    public GameConfigurations GameConfigurations = new ();
    [JsonInclude]
    public List<Card> DiscardPile = new();
    [JsonInclude]
    public static Random R = new();

    public void UnoDeck()
    {
        foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
        {
            foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
            {
                if (value != CardValue.WildDrawFour && value != CardValue.Wild && value != CardValue.WildShuffleHands &&
                    value != CardValue.WildCustomizable && value != CardValue.Zero && color != CardColor.Multicolor)
                {
                    DeckPile.Add(new Card(color, value));
                    DeckPile.Add(new Card(color, value));
                }
                else if (value == CardValue.Zero && color != CardColor.Multicolor)
                {
                    DeckPile.Add(new Card(color, value));
                }
                else if ((value == CardValue.WildDrawFour || value == CardValue.Wild) && color == CardColor.Multicolor)
                {
                    DeckPile.Add(new Card(color, value));
                    DeckPile.Add(new Card(color, value));
                    DeckPile.Add(new Card(color, value));
                    DeckPile.Add(new Card(color, value));
                }
            }
        }
        DeckPile.Add(new Card(CardColor.Multicolor, CardValue.WildShuffleHands));
        DeckPile.Add(new Card(CardColor.Multicolor, CardValue.WildCustomizable));
        DeckPile.Add(new Card(CardColor.Multicolor, CardValue.WildCustomizable));
        DeckPile.Add(new Card(CardColor.Multicolor, CardValue.WildCustomizable));
        // foreach (var card in AllCards)
        // {
        //     Console.WriteLine(card.CardColor + " " + card.CardValue);
        // }
        //
        // Console.WriteLine(AllCards.Count);
    }

    public List<Card> ShuffleCards(List<Card> cards)
    {
        return cards.OrderBy(card => R.Next()).ToList();
    }

    public void DealCardsToPlayers()
    {
        UnoDeck();
        DeckPile = ShuffleCards(DeckPile);
        CountStatingHandSize = GameConfigurations?.RuleTypeInput == 1
            ? OfficialRules.UnoRules["GameRules"]["StartingHandSize"]
            : CustomRules.UnoRules["GameRules"]["StartingHandSize"];
        for (int i = 0; i < GameConfigurations?.Players.Count; i++)
        {
            for (int j = 0; j < CountStatingHandSize; j++)
            {
                GameConfigurations?.Players[i].PlayerHand.Add(DeckPile.Last());
                DeckPile.Remove(DeckPile.Last());
            }
        }
        // for (int i = 0; i < GameConfigurations?.Players.Count; i++)
        // {
        //     for (int j = 0; j < countStatingHandSize; j++)
        //     {
        //         Console.WriteLine(GameConfigurations?.Players[i].PlayerHand[j].CardColor);
        //         Console.WriteLine(GameConfigurations?.Players[i].PlayerHand[j].CardValue);
        //         // AllCards.Remove(AllCards.Last());
        //     }
        //     Console.WriteLine("--------------------------");
        // }
        // DiscardPile.Add(AllCards.Last());
    }
}