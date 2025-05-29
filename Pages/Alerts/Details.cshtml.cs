using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages.Alerts
{
    public class DetailsModel : BasePageModel
    {
        private readonly LabMaterials.DB.LabDBContext _context;

        public DetailsModel(LabMaterials.DB.LabDBContext context)
        {
            _context = context;
        }

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
    }
}
