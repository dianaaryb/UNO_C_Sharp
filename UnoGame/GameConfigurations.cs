using System.Text.Json.Serialization;
using PlayerSystem;
using RulesSystem;

namespace UnoGame;

public class GameConfigurations
{
    [JsonInclude]
    public string? TotalPlayers { get; set; }
    [JsonInclude]
    public string? HumanPlayers { get; set;}
    [JsonInclude]
    public string? AiPlayers { get; set; }
    [JsonInclude]
    public int? RuleTypeInput;
    [JsonInclude]
    public List<Player> Players = new ();
    [JsonIgnore] 
    public Dictionary<string, dynamic> RuleDictionary = RulesSystem.OfficialRules.UnoRules;

    [JsonInclude] public bool HaveToShowPoints;
    public SaveLocation SaveLocation { get; set; }
    public List<string> PlayersList = new ();
    
    [JsonInclude]
    public string RuleDictionaryType
    {
    get => RuleDictionary["GameRules"]["RuleType"].ToString();
    }
}