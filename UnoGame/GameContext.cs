using Microsoft.EntityFrameworkCore;

namespace UnoGame;

// dotnet ef migrations add --project DAL --startup-project ConsoleApp InitialCreate
public class GameContext : DbContext //represents a session with the database and is used for querying and saving data to the database.
{
    public DbSet<Domain.Database.Game> Games { get; set; } = default!;
    public DbSet<Domain.Database.PlayerToDb> Players { get; set; } = default!;

    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
    }
}