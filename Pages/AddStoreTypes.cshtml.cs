using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class AddStoreTypesModel : BasePageModel
    {
        private readonly LabDBContext _context;

        public AddStoreTypesModel(LabDBContext context)
        {
            _context = context;
        }
        [BindProperty]
        public StoreTypes StoreTypeModel { get; set; } = new();
        public List<StoreTypes> StoreTypeList { get; set; } = new();

        public async Task OnGetAsync()
        {

            base.ExtractSessionData();
            StoreTypeList = await _context.StoreTypes.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.StoreTypes.Add(StoreTypeModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("ViewStoreTypes");
        }
    }
}
