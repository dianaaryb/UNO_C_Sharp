using PlayerSystem;

namespace Domain.Database;

public class PlayerToDb
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public PlayerType PlayerType { get; set; }
    public Guid GameId { get; set; }
    public Game? Game { get; set; }
}