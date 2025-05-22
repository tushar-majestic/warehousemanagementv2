using LabMaterials.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages_HazardTypes
{
    public class DetailsModel : BasePageModel
    {
        private readonly LabMaterials.DB.LabDBContext _context;

        public DetailsModel(LabMaterials.DB.LabDBContext context)
        {
            _context = context;
        }

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
    }
}
