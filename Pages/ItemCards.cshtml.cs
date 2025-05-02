using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class ItemCardsModel : BasePageModel
    {
        private readonly LabDBContext _context;
        public ItemCardsModel(LabDBContext context)
        {
            _context = context;
        }
        [BindProperty]
        public ItemCard ItemCard { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            base.ExtractSessionData();
            await PopulateDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return Page();
            }

            // Attach related entities if necessary
            if (ItemCard.Item != null && ItemCard.Item.ItemId != 0)
            {
                ItemCard.Item = await _context.Items.FindAsync(ItemCard.Item.ItemId);
            }

            if (ItemCard.Store != null && ItemCard.Store.StoreId != 0)
            {
                ItemCard.Store = await _context.Stores.FindAsync(ItemCard.Store.StoreId);
            }

            _context.ItemCards.Add(ItemCard);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task PopulateDropdownsAsync()
        {
            ViewData["WarehouseId"] = new SelectList(await _context.Stores.ToListAsync(), "StoreId", "StoreName");
            ViewData["RoomId"] = new SelectList(await _context.Rooms.ToListAsync(), "RoomId", "RoomName");
            ViewData["ShelfId"] = new SelectList(await _context.Shelves.ToListAsync(), "ShelfId", "ShelfNo");
            ViewData["SupplierId"] = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "SupplierName");
            ViewData["HazardId"] = new SelectList(await _context.HazardTypes.ToListAsync(), "HazardTypeName", "HazardTypeName");
            ViewData["ItemGroupId"] = new SelectList(await _context.ItemGroups.ToListAsync(), "GroupCode", "GroupDesc");
            ViewData["ItemId"] = new SelectList(await _context.Items.ToListAsync(), "ItemId", "ItemCode");
            ViewData["Itemtype"] = new SelectList(await _context.ItemTypes.ToListAsync(), "ItemTypeCode", "TypeName");
        }
    }
}

