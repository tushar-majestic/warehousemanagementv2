using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LabMaterials.DB;

namespace LabMaterials.Pages.Alerts
{
    public class CreateModel : PageModel
    {
        private readonly LabMaterials.DB.LabDBContext _context;

        public CreateModel(LabMaterials.DB.LabDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["GroupCode"] = new SelectList(_context.ItemGroups, "GroupCode", "GroupCode");
        ViewData["HazardTypeName"] = new SelectList(_context.HazardTypes, "HazardTypeName", "HazardTypeName");
        ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId");
        ViewData["ItemTypeCode"] = new SelectList(_context.ItemTypes, "ItemTypeCode", "ItemTypeCode");
        ViewData["StoreId"] = new SelectList(_context.Stores, "StoreId", "StoreId");
            return Page();
        }

        [BindProperty]
        public ItemCard ItemCard { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ItemCards.Add(ItemCard);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
