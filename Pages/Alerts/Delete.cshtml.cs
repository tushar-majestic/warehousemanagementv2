using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages.Alerts
{
    public class DeleteModel : BasePageModel
    {
        private readonly LabMaterials.DB.LabDBContext _context;

        public DeleteModel(LabMaterials.DB.LabDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ItemCard ItemCard { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            base.ExtractSessionData();
            if (id == null)
            {
                return NotFound();
            }

            var itemcard = await _context.ItemCards.FirstOrDefaultAsync(m => m.Id == id);

            if (itemcard == null)
            {
                return NotFound();
            }
            else
            {
                ItemCard = itemcard;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemcard = await _context.ItemCards.FindAsync(id);
            if (itemcard != null)
            {
                ItemCard = itemcard;
                _context.ItemCards.Remove(ItemCard);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
