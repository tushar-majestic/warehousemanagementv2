using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using LabMaterials.DB;
using LabMaterials.dtos;
using LabMaterials.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
        public ItemCardBatch ItemCardBatch { get; set; } = new ItemCardBatch();

        // public ItemCardBatch ItemCardBatch { get; set; } = new ItemCardBatch();

        public List<SelectListItem> ItemList { get; set; }
        public List<Shelf> Shelves { get; set; }
        public int? ReportId;
        public int? InboxId;
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
        [BindProperty]
        public List<LabMaterials.DB.ItemCardExtended> ItemCardsFromReport { get; set; }


        // public List<string> ItemCardsFromReport { get; set; } 

        public async Task<IActionResult> OnGetAsync()
        {
            base.ExtractSessionData();
            // await PopulateDropdownsAsync();
            ItemList = _context.Items
            .Select(i => new SelectListItem { Value = i.ItemCode, Text = i.ItemCode })
            .ToList();

            Shelves = _context.Shelves.ToList();
            // AllItems = _context.Items.ToList();
            this.ReportId = HttpContext.Session.GetInt32("ReportId");
            // this.MessageId = HttpContext.Session.GetInt32("MessageId");

            if (ReportId.HasValue)
            {
                var receivingItems = await _context.ReceivingItems
                .Include(ri => ri.Item)
                .Include(ri => ri.ReceivingReport)
                .Where(ri => ri.ReceivingReportId == ReportId.Value)
                .ToListAsync();

                ItemCardsFromReport = (from ri in receivingItems
                                       join unit in _context.Units on ri.Item.UnitId equals unit.Id
                                       select new LabMaterials.DB.ItemCardExtended
                                       {
                                           ItemCode = ri.Item.ItemCode,
                                           ItemName = ri.Item.ItemName,
                                           GroupCode = ri.Item.GroupCode,
                                           ItemTypeCode = ri.Item.ItemTypeCode,
                                           ItemDescription = ri.Item.ItemDescription,
                                           ItemId = ri.ItemId,
                                           HazardTypeName = ri.Item.HazardTypeName,
                                           ExpiryDate = ri.Item.ExpiryDate,
                                           QuantityReceived = ri.Quantity,
                                           UnitOfmeasure = unit.UnitCode,
                                           Chemical = (bool)ri.Item.Chemical ? "Yes" : "No"
                                       }).ToList();


                var ReceivingReport = _context.ReceivingReports
                .Where(ri => ri.Id == ReportId.Value).FirstOrDefault();

                if (ReceivingReport != null)
                {
                    ItemCardBatch = new ItemCardBatch
                    {
                        DateOfEntry = ReceivingReport.CreatedAt,
                        SupplierId = ReceivingReport.SupplierId,
                        DocumentType = ReceivingReport.BasedOnDocument,
                        ReceiptDocumentnumber = ReceivingReport.DocumentNumber
                    };

                    ItemCard = new ItemCard
                    {
                        StoreId = int.Parse(ReceivingReport.ReceivingWarehouse)
                    };


                }
            }

            await PopulateDropdownsAsync(ItemCard.StoreId);
            ViewData["ItemId"] = ItemList;
            // AllItemsDto = _context.Items.Select(i => new ItemDto
            // {
            //     ItemCode = i.ItemCode,
            //     ItemName = i.ItemName,
            //     UnitOfMeasure = i.UnitId,
            //     ItemId = i.ItemId,
            //     GroupCode = i.GroupCode,
            //     ItemTypeCode = i.ItemTypeCode,
            //     ItemDescription = i.ItemDescription,
            //     HazardTypeName = i.HazardTypeName
            // }).ToList();

            ViewData["ItemId"] = _context.Items
                .Select(i => new SelectListItem { Value = i.ItemCode, Text = i.ItemCode })
                .ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm] int StoreId, [FromForm] string DocumentType, [FromForm] string ReceiptDocumentnumber, [FromForm] int RoomId, [FromForm] int ShelfId, [FromForm] int SupplierId, [FromForm] DateTime DateOfEntry)
        {
            var reportId = HttpContext.Session.GetInt32("ReportId");
            this.InboxId = HttpContext.Session.GetInt32("InboxId");
            Shelves = _context.Shelves.ToList();


            // foreach (var extendedCard  in ItemCardsFromReport)
            // {
            //     var itemCard = new ItemCard
            //     {
            //         ItemId = extendedCard.ItemId,
            //         ItemCode = extendedCard.ItemCode,
            //         ItemName = extendedCard.ItemName,
            //         GroupCode = extendedCard.GroupCode,
            //         ItemTypeCode = extendedCard.ItemTypeCode,
            //         ItemDescription = extendedCard.ItemDescription,
            //         StoreId = StoreId,
            //         HazardTypeName = extendedCard.HazardTypeName,
            //         QuantityAvailable = extendedCard.QuantityReceived


            //     };

            //         _context.ItemCards.Add(itemCard);
            //         await _context.SaveChangesAsync();

            //     // Now add the corresponding ItemCardBatch
            //     var itemCardBatch = new ItemCardBatch
            //     {
            //         ItemCardId = itemCard.Id,
            //         DocumentType = DocumentType,
            //         ReceiptDocumentnumber = ReceiptDocumentnumber,
            //         RoomId = RoomId,
            //         ShelfId = ShelfId,
            //         QuantityReceived = extendedCard.QuantityReceived,
            //         SupplierId = SupplierId,
            //         Minimum = extendedCard.Minimum,
            //         ReorderLimit = extendedCard.ReorderLimit,
            //         ExpiryDate = extendedCard.ExpiryDate,
            //         TypeOfAsset = extendedCard.TypeOfAsset,
            //         Ceiling = extendedCard.Minimum * extendedCard.ReorderLimit,
            //         DateOfEntry = DateOfEntry   ,

            //     };

            //     _context.ItemCardBatches.Add(itemCardBatch);
            //     await _context.SaveChangesAsync();
            // }
            foreach (var extendedCard in ItemCardsFromReport)
            {
                // Try to find existing ItemCard based on ItemId 
                var existingItemCard = await _context.ItemCards
                    .FirstOrDefaultAsync(ic => ic.ItemId == extendedCard.ItemId && ic.StoreId == StoreId);

                int itemCardId;

                if (existingItemCard != null)
                {
                    Console.WriteLine("Item already exists");
                    // Item already exists : update QuantityAvailable
                    existingItemCard.QuantityAvailable += extendedCard.QuantityReceived;
                    _context.ItemCards.Update(existingItemCard);
                    await _context.SaveChangesAsync();
                    itemCardId = existingItemCard.Id;
                }
                else
                {
                    Console.WriteLine("Item does not exists");

                    // New item create ItemCard
                    var newItemCard = new ItemCard
                    {
                        ItemId = extendedCard.ItemId,
                        ItemCode = extendedCard.ItemCode,
                        ItemName = extendedCard.ItemName,
                        GroupCode = extendedCard.GroupCode,
                        ItemTypeCode = extendedCard.ItemTypeCode,
                        ItemDescription = extendedCard.ItemDescription,
                        StoreId = StoreId,
                        HazardTypeName = extendedCard.HazardTypeName,
                        UnitOfmeasure = extendedCard.UnitOfmeasure,
                        // Chemical = extendedCard.Chemical,
                        QuantityAvailable = extendedCard.QuantityReceived,
                        CreatedBy = HttpContext.Session.GetInt32("UserId").Value
                    };

                    _context.ItemCards.Add(newItemCard);
                    await _context.SaveChangesAsync();
                    itemCardId = newItemCard.Id; // Needed for batch
                }

                // Insert ItemCardBatch 
                var itemCardBatch = new ItemCardBatch
                {
                    ItemCardId = itemCardId,
                    DocumentType = DocumentType,
                    ReceiptDocumentnumber = ReceiptDocumentnumber,
                    RoomId = RoomId,
                    ShelfId = ShelfId,
                    QuantityReceived = extendedCard.QuantityReceived,
                    SupplierId = SupplierId,
                    Minimum = extendedCard.Minimum,
                    ReorderLimit = extendedCard.ReorderLimit,
                    ExpiryDate = extendedCard.ExpiryDate,
                    TypeOfAsset = extendedCard.TypeOfAsset,
                    Ceiling = extendedCard.Minimum * extendedCard.ReorderLimit,
                    DateOfEntry = DateOfEntry
                };

                _context.ItemCardBatches.Add(itemCardBatch);

                var existingItemShelf = await _context.ShelveItems
                    .FirstOrDefaultAsync(s => s.ItemCardId == itemCardId && s.ShelfId == ShelfId);

                // If shelf for that item exists
                if (existingItemShelf != null)
                {
                    existingItemShelf.QuantityAvailable += extendedCard.QuantityReceived;
                    _context.ShelveItems.Update(existingItemShelf);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var shelveItem = new ShelveItem
                    {
                        ShelfId = ShelfId,
                        ItemCardId = itemCardId,
                        QuantityAvailable = extendedCard.QuantityReceived

                    };
                    _context.ShelveItems.Add(shelveItem);

                    await _context.SaveChangesAsync();
                }


            }
            var dbContext = new LabDBContext();

            var message = dbContext.Messages.FirstOrDefault(m => m.Id == this.InboxId);

            if (message != null)
            {
                message.Type = "Added";
            }
            dbContext.SaveChanges();

            return RedirectToPage("/ManageItemCards");
        }

        // private async Task PopulateDropdownsAsync()
        // {
        //     ViewData["WarehouseId"] = new SelectList(await _context.Stores.ToListAsync(), "StoreId", "StoreName");
        //     ViewData["RoomId"] = new SelectList(await _context.Rooms.ToListAsync(), "RoomId", "RoomName");
        //     ViewData["ShelfId"] = new SelectList(await _context.Shelves.ToListAsync(), "ShelfId", "ShelfNo");
        //     ViewData["SupplierId"] = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "SupplierName");
        //     ViewData["HazardId"] = new SelectList(await _context.HazardTypes.ToListAsync(), "HazardTypeName", "HazardTypeName");
        //     ViewData["ItemGroupId"] = new SelectList(await _context.ItemGroups.ToListAsync(), "GroupCode", "GroupDesc");
        //     ViewData["ItemIds"] = new SelectList(await _context.Items.ToListAsync(), "ItemId", "ItemName");
        //     ViewData["Itemtype"] = new SelectList(await _context.ItemTypes.ToListAsync(), "ItemTypeCode", "TypeName");
        // }
        private async Task PopulateDropdownsAsync(int selectedStoreId)
        {
            ViewData["WarehouseId"] = new SelectList(await _context.Stores.ToListAsync(), "StoreId", "StoreName", selectedStoreId);

            var filteredRooms = await _context.Rooms
                .Where(r => r.StoreId == selectedStoreId)
                .ToListAsync();
            Console.WriteLine("rooms for store", filteredRooms, selectedStoreId);
            ViewData["RoomId"] = new SelectList(filteredRooms, "RoomId", "RoomName");

            ViewData["ShelfId"] = new SelectList(new List<Shelf>(), "ShelfId", "ShelfNo"); // initially empty

            ViewData["SupplierId"] = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "SupplierName");
            ViewData["HazardId"] = new SelectList(await _context.HazardTypes.ToListAsync(), "HazardTypeName", "HazardTypeName");
            ViewData["ItemGroupId"] = new SelectList(await _context.ItemGroups.ToListAsync(), "GroupCode", "GroupDesc");
            ViewData["ItemIds"] = new SelectList(await _context.Items.ToListAsync(), "ItemId", "ItemName");
            ViewData["Itemtype"] = new SelectList(await _context.ItemTypes.ToListAsync(), "ItemTypeCode", "TypeName");
        }

        public async Task<JsonResult> OnGetShelvesByRoom(int roomId)
        {
            var shelves = await _context.Shelves
                .Where(s => s.RoomId == roomId)
                .Select(s => new { value = s.ShelfId, text = s.ShelfNo })
                .ToListAsync();

            return new JsonResult(shelves);
        }


    }
}

