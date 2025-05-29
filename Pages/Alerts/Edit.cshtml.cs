using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages.Alerts
{
    public class EditModel : BasePageModel
    {
        private readonly LabMaterials.DB.LabDBContext _context;

        public EditModel(LabMaterials.DB.LabDBContext context)
        {
            _context = context;
        }

        [BindProperty]
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
            ItemCard = itemcard;
            ViewData["GroupCode"] = new SelectList(_context.ItemGroups, "GroupCode", "GroupCode");
            ViewData["HazardTypeName"] = new SelectList(_context.HazardTypes, "HazardTypeName", "HazardTypeName");
            ViewData["ItemId"] = new SelectList(_context.Items, "ItemId", "ItemId");
            ViewData["ItemTypeCode"] = new SelectList(_context.ItemTypes, "ItemTypeCode", "ItemTypeCode");
            ViewData["StoreId"] = new SelectList(_context.Stores, "StoreId", "StoreId");
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

            _context.Attach(ItemCard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemCardExists(ItemCard.Id))
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

        private bool ItemCardExists(int id)
        {
            return _context.ItemCards.Any(e => e.Id == id);
        }
    }
}
