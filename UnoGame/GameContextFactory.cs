using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UnoGame;

public class GameContextFactory : IDesignTimeDbContextFactory<GameContext>
{
    public GameContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<GameContext> optionsBuilder = new DbContextOptionsBuilder<GameContext>();
        optionsBuilder.UseSqlite("");
        
        return new GameContext(optionsBuilder.Options);
    }
}