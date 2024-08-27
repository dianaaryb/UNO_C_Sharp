using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoGame;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly GameContext _gameContext;
    public int Count { get; set; }

    public IndexModel(ILogger<IndexModel> logger, GameContext context)
    {
        _logger = logger;
        _gameContext = context;
    }

    public void OnGet()
    {
        Count = _gameContext.Games.Count();
    }
}