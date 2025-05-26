using LabMaterials.Pages;
using Microsoft.AspNetCore.Mvc;

namespace LabMaterials.Pages_HazardTypes
{
    public class CreateModel : BasePageModel
    {
        private readonly LabMaterials.DB.LabDBContext _context;

        public CreateModel(LabMaterials.DB.LabDBContext context)
        {
            _context = context;
        }
        public string lblUsers, lblHazardTypeName, lblAction, lblHazardType;
        public IActionResult OnGet()
        {
            FillLables();
            base.ExtractSessionData();
            return Page();
        }

        [BindProperty]
        public HazardType HazardType { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.HazardTypes.Add(HazardType);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        private void FillLables()
        {
            this.lblHazardType = (Program.Translations["HazardType"])[Lang];
            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblHazardTypeName = (Program.Translations["HazardTypeName"])[Lang];
        }
        
    }
}
