using System.Text.Json.Serialization;

namespace CardSystem;

public class Card
{
    [JsonInclude]
    public CardColor CardColor;
    [JsonInclude]
    public CardValue CardValue;
    [JsonInclude] public bool WasPlayed;

    public Card()
    {
        
    }
    public Card(CardColor cardColor, CardValue cardValue)
    {
        CardColor = cardColor;
        CardValue = cardValue;
    }
}