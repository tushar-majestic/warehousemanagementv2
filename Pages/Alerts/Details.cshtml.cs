using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LabMaterials.DB;

namespace LabMaterials.Pages.Alerts
{
    public class DetailsModel : PageModel
    {
        private readonly LabMaterials.DB.LabDBContext _context;

        public DetailsModel(LabMaterials.DB.LabDBContext context)
        {
            _context = context;
        }

        public ItemCard ItemCard { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
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
