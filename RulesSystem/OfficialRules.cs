using UnoGame.Rules;

namespace RulesSystem;

public class OfficialRules
{
    // public RuleType RuleType = RuleType.OfficialRules;
        public static Dictionary<string, dynamic> UnoRules = new Dictionary<string, dynamic>
        {
            ["GameRules"] = new Dictionary<string, dynamic>
            {
                ["RuleType"] = RuleType.OfficialRules,
                ["StartingHandSize"] = 7,
                ["MaximumPlayers"] = 10,
                ["SkipAfterReverse"] = true,
                ["DrawTwoStackable"] = true,
                ["WildDrawFourStackable"] = true
            },
            ["CardValues"] = new Dictionary<string, dynamic>
            {
                ["NumberCards"] = new Dictionary<string, dynamic>
                {
                    ["Zero"] = 0,
                    ["One"] = 1,
                    ["Two"] = 2,
                    ["Three"] = 3,
                    ["Four"] = 4,
                    ["Five"] = 5,
                    ["Six"] = 6,
                    ["Seven"] = 7,
                    ["Eight"] = 8,
                    ["Nine"] = 9
                },
                ["SpecialCards"] = new Dictionary<string, dynamic>
                {
                    ["Skip"] = 20,
                    ["Reverse"] = 20,
                    ["DrawTwo"] = 20,
                    ["Wild"] = 50,
                    ["WildDrawFour"] = 50,
                    ["WildShuffleHands"] = 40,
                    ["WildCustomizable"] = 40
                }
            },
            ["CardColors"] = new List<string> { "Red", "Yellow", "Green", "Blue", "MultiColor" },
            ["WinningCondition"] = "FirstPlayerOut", 
            ["Scoring"] = "Standard",
            ["PenaltyForNotCallingUno"] = 2
        };
}


