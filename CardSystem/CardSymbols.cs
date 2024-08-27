namespace CardSystem;

public static class CardSymbols
{
    private static readonly Dictionary<CardValue, string> CardSymbolMap = new Dictionary<CardValue, string>
    {
        { CardValue.Zero, "0" },
        { CardValue.One, "1" },
        { CardValue.Two, "2" },
        { CardValue.Three, "3" },
        { CardValue.Four, "4" },
        { CardValue.Five, "5" },
        { CardValue.Six, "6" },
        { CardValue.Seven, "7" },
        { CardValue.Eight, "8" },
        { CardValue.Nine, "9" },
        { CardValue.Skip, "S" },
        { CardValue.Reverse, "R" },
        { CardValue.DrawTwo, "+2" },
        { CardValue.Wild, "W" },
        { CardValue.WildDrawFour, "W4" },
        { CardValue.WildShuffleHands, "WS" },
        { CardValue.WildCustomizable, "WC"}
    };

    public static string GetSymbol(CardValue value)
    {
        return CardSymbolMap.TryGetValue(value, out var symbol) ? symbol : string.Empty;
    }
}