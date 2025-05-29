using LabMaterials.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages_HazardTypes
{
    public class EditModel : BasePageModel
    {
        private readonly LabMaterials.DB.LabDBContext _context;

        public EditModel(LabMaterials.DB.LabDBContext context)
        {
            _context = context;
        }

        [BindProperty]
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
            HazardType = hazardtype;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(HazardType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HazardTypeExists(HazardType.HazardTypeName))
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

        private bool HazardTypeExists(string id)
        {
            return _context.HazardTypes.Any(e => e.HazardTypeName == id);
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
