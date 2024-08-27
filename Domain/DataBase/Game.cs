namespace Domain.Database;

public class Game
{
    public Guid Id { get; set; }
    public DateTime CreatedAtDt { get; set; } = DateTime.Now;
    public DateTime UpdatedAtDt { get; set; } = DateTime.Now;
    public string GameState { get; set; } = default!;
    
    public ICollection<PlayerToDb>? Players { get; set; }
}