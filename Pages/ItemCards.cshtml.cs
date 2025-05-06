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
        public List<SelectListItem> ItemList { get; set; }
        public List<Item> AllItems { get; set; }
        public class ItemDto
        {
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public int UnitOfMeasure { get; set; }
            public int ItemId { get; set; }
            public string GroupCode { get; set; }
            public string ItemTypeCode { get; set; }
            public string ItemDescription { get; set; }
            public string HazardTypeName { get; set; }
            // Add more fields as needed
        }
        public List<ItemDto> AllItemsDto { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            base.ExtractSessionData();
            await PopulateDropdownsAsync();
            ItemList = _context.Items
            .Select(i => new SelectListItem { Value = i.ItemCode, Text = i.ItemCode })
            .ToList();

            AllItems = _context.Items.ToList();

            ViewData["ItemId"] = ItemList;
            AllItemsDto = _context.Items.Select(i => new ItemDto
            {
                ItemCode = i.ItemCode,
                ItemName = i.ItemName,
                UnitOfMeasure = i.UnitId,
                ItemId = i.ItemId,
                GroupCode = i.GroupCode,
                ItemTypeCode = i.ItemTypeCode,
                ItemDescription = i.ItemDescription,
                HazardTypeName = i.HazardTypeName
            }).ToList();

            ViewData["ItemId"] = _context.Items
                .Select(i => new SelectListItem { Value = i.ItemCode, Text = i.ItemCode })
                .ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    base.ExtractSessionData();
            //    await PopulateDropdownsAsync();
            //    return Page();
            //}

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

            return RedirectToPage();
        }

        private async Task PopulateDropdownsAsync()
        {
            ViewData["WarehouseId"] = new SelectList(await _context.Stores.ToListAsync(), "StoreId", "StoreName");
            ViewData["RoomId"] = new SelectList(await _context.Rooms.ToListAsync(), "RoomId", "RoomName");
            ViewData["ShelfId"] = new SelectList(await _context.Shelves.ToListAsync(), "ShelfId", "ShelfNo");
            ViewData["SupplierId"] = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "SupplierName");
            ViewData["HazardId"] = new SelectList(await _context.HazardTypes.ToListAsync(), "HazardTypeName", "HazardTypeName");
            ViewData["ItemGroupId"] = new SelectList(await _context.ItemGroups.ToListAsync(), "GroupCode", "GroupDesc");
            ViewData["ItemIds"] = new SelectList(await _context.Items.ToListAsync(), "ItemId", "ItemName");
            ViewData["Itemtype"] = new SelectList(await _context.ItemTypes.ToListAsync(), "ItemTypeCode", "TypeName");
        }
    }
}

