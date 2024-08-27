using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Domain.Database;

namespace WebApp.Pages.Players
{
    public class CreateModel : PageModel
    {
        private readonly UnoGame.GameContext _context;

        public CreateModel(UnoGame.GameContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["GameId"] = new SelectList(_context.Games, "Id", "GameState");
            return Page();
        }

        [BindProperty]
        public PlayerToDb PlayerToDb { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Players == null || PlayerToDb == null)
            {
                return Page();
            }

            _context.Players.Add(PlayerToDb);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
