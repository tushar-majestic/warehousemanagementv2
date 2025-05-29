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
        public string lblUsers, lblHazardTypeName, lblAction, lblHazardType;
        public async Task<IActionResult> OnGetAsync(string id)
        {
            FillLables();
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
         private void FillLables()
        {
            this.Lang = HttpContext.Session.GetString("Lang");
            this.lblHazardType = (Program.Translations["HazardType"])[Lang];
            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblHazardTypeName = (Program.Translations["HazardTypeName"])[Lang];
        }
        
    }
}
