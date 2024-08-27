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
    public class DeleteModel : PageModel
    {
        private readonly UnoGame.GameContext _context;

        public DeleteModel(UnoGame.GameContext context)
        {
            _context = context;
        }

        [BindProperty]
      public PlayerToDb PlayerToDb { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var playertodb = await _context.Players.FirstOrDefaultAsync(m => m.Id == id);

            if (playertodb == null)
            {
                return NotFound();
            }
            else 
            {
                PlayerToDb = playertodb;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }
            var playertodb = await _context.Players.FindAsync(id);

            if (playertodb != null)
            {
                PlayerToDb = playertodb;
                _context.Players.Remove(PlayerToDb);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
