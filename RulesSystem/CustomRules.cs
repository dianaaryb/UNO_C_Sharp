using UnoGame.Rules;

namespace RulesSystem;

public class CustomRules
{
            // public RuleType RuleType = RuleType.CustomRules;
            public static Dictionary<string, dynamic> UnoRules = new Dictionary<string, dynamic>
            {
                ["GameRules"] = new Dictionary<string, dynamic>
                {
                    ["RuleType"] = RuleType.CustomRules,
                    ["StartingHandSize"] = 5,
                    ["MaximumPlayers"] = 6,
                    ["SkipAfterReverse"] = false,
                    ["DrawTwoStackable"] = false,
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
                        ["Skip"] = 10,
                        ["Reverse"] = 10,
                        ["DrawTwo"] = 10,
                        ["Wild"] = 25,
                        ["WildDrawFour"] = 25,
                        ["WildShuffleHands"] = 20,
                        ["WildCustomizable"] = 20
                    }
                },
                ["CardColors"] = new List<string> { "Red", "Yellow", "Green", "Blue", "Multicolor" },
                ["WinningCondition"] = "FirstPlayerToReach100Points",
                ["Scoring"] = "Points-based scoring",
                ["PenaltyForNotCallingUno"] = 4
            };
}