using System.Text.Json.Serialization;
using CardSystem;

namespace PlayerSystem;

public class Player
{
    [JsonInclude] 
    public Guid Id { get; set; } = Guid.NewGuid();
    [JsonInclude]
    public PlayerType PlayerType;
    [JsonInclude]
    public string? Name;
    [JsonInclude]
    public List<Card> PlayerHand = new();
    [JsonInclude]
    public int Score = 0;
    [JsonInclude]
    public bool MustSkip = false;

    public bool SaidUno = false;

    public Player()
    {
        
    }
    public Player(PlayerType playerType, string name)
    {
        PlayerType = playerType;
        Name = name;
    }

    public string ToString()
    {
        return $"{PlayerType} = {Name}";
    }
}