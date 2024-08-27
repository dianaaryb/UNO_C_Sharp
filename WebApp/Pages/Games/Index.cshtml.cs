using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain.Database;
using UnoGame;

namespace WebApp.Pages.Games
{
    public class IndexModel : PageModel
    {
        private readonly GameContext _context;
        public SaveToJsonFile SaveToJsonFile = new SaveToJsonFile();

        public IndexModel(GameContext context)
        {
            _context = context;
        }

        public IList<Game> Game { get;set; } = default!;
        public IList<GameState> GamesFromFileSystem { get; set; } = default!;
        public List<(Guid id, DateTime dateTime)>? GamesFromFileData { get; set; }

        public async Task OnGetAsync()
        {
                Game = await _context.Games
                    .Include(g => g.Players)
                    .OrderByDescending(g => g.UpdatedAtDt)
                    .ToListAsync();
                GamesFromFileData = SaveToJsonFile.GetSavedGames();
                GamesFromFileSystem = GamesFromFileData.Select(g => SaveToJsonFile.LoadGame(g.id)).ToList();
        }
    }
}
