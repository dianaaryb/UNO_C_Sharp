using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.Database;

namespace WebApp.Pages.Players
{
    public class EditModel : PageModel
    {
        private readonly UnoGame.GameContext _context;

        public EditModel(UnoGame.GameContext context)
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

            var playertodb =  await _context.Players.FirstOrDefaultAsync(m => m.Id == id);
            if (playertodb == null)
            {
                return NotFound();
            }
            PlayerToDb = playertodb;
           ViewData["GameId"] = new SelectList(_context.Games, "Id", "GameState");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(PlayerToDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerToDbExists(PlayerToDb.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PlayerToDbExists(Guid id)
        {
          return (_context.Players?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
