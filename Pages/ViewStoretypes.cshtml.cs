using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class ViewStoretypesModel : BasePageModel
    {
        private readonly LabDBContext _context;

        public ViewStoretypesModel(LabDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public StoreTypes StoreTypeModel { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; }

        public List<StoreTypes> StoreTypeList { get; set; } = new();
         public string lblUsers;
        public async Task OnGetAsync()
        {
            base.ExtractSessionData();
            FillLables();
            StoreTypeList = await _context.StoreTypes.ToListAsync();
        }

        public async Task<IActionResult> OnPostEditInitAsync()
        {
            base.ExtractSessionData();
            FillLables();
            StoreTypeList = await _context.StoreTypes.ToListAsync();
            StoreTypeModel = await _context.StoreTypes.FirstOrDefaultAsync(s => s.StoreTypeId == EditId);
            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync(bool? cancel)
        {
            base.ExtractSessionData();
            if (cancel == true)
            {
                return RedirectToPage();
            }

            if (!ModelState.IsValid)
            {
                StoreTypeList = await _context.StoreTypes.ToListAsync();
                return Page();
            }

            var existing = await _context.StoreTypes.FindAsync(StoreTypeModel.StoreTypeId);
            if (existing != null)
            {
                existing.StoreType = StoreTypeModel.StoreType;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var storeType = await _context.StoreTypes.FindAsync(id);
            if (storeType != null)
            {
                _context.StoreTypes.Remove(storeType);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
          private void FillLables()
        {
            

            this.lblUsers = (Program.Translations["Users"])[Lang];
        }
    }
}
