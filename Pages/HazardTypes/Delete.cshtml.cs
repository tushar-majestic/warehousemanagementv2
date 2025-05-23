using LabMaterials.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages_HazardTypes
{
    public class DeleteModel : BasePageModel
    {
        private readonly LabMaterials.DB.LabDBContext _context;

        public DeleteModel(LabMaterials.DB.LabDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HazardType HazardType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            base.ExtractSessionData();
            if (id == null)
            {
                return NotFound();
            }

            var hazardtype = await _context.HazardTypes.FirstOrDefaultAsync(m => m.HazardTypeName == id);

            if (hazardtype == null)
            {
                return NotFound();
            }
            else
            {
                HazardType = hazardtype;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hazardtype = await _context.HazardTypes.FindAsync(id);
            if (hazardtype != null)
            {
                HazardType = hazardtype;
                _context.HazardTypes.Remove(HazardType);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
