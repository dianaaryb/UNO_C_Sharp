using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain.Database;

namespace WebApp.Pages.Players
{
    public class IndexModel : PageModel
    {
        private readonly UnoGame.GameContext _context;

        public IndexModel(UnoGame.GameContext context)
        {
            _context = context;
        }

        public IList<PlayerToDb> PlayerToDb { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Players != null)
            {
                PlayerToDb = await _context.Players
                .Include(p => p.Game).ToListAsync();
            }
        }
    }
}
