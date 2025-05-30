using DocumentFormat.OpenXml.Office.CustomUI;
using LabMaterials.DB;
using LabMaterials.dtos;
using LabMaterials.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LabMaterials.Pages
{
    public class DeductOrderModel : BasePageModel
    {
        public List<SelectListItem> ItemList { get; set; }
        private readonly LabDBContext _context;
        public MaterialRequest MaterialRequest { get; set; } = new MaterialRequest();

        public List<Room> Rooms { get; set; }
        public List<Shelf> Shelves { get; set; }
        public PendingDeduction Deduction { get; set; } = default!;


        public DeductOrderModel(LabDBContext context)
        {
            _context = context;
        }

        public int? ReportId;
        public int? InboxId;

        [BindProperty]
        public List<DeductionExtended> ItemCardsFromReport { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            base.ExtractSessionData();
            await PopulateDropdownsAsync();
            this.ReportId = HttpContext.Session.GetInt32("DisReportId");
            this.InboxId = HttpContext.Session.GetInt32("MsgId");



            if (ReportId.HasValue)
            {
                var dispensedItems = await _context.DespensedItems
                .Include(ri => ri.ItemCard)
                .Include(ri => ri.MaterialRequest)
                .Where(ri => ri.MaterialRequestId == ReportId.Value)
                .ToListAsync();

                // Rooms = await _context.Rooms.ToListAsync();
                var itemCardIds = dispensedItems.Select(d => d.ItemCardId).Distinct().ToList();

                var roomIds = await _context.ItemCardBatches
                    .Where(b => itemCardIds.Contains(b.ItemCardId))
                    .Select(b => b.RoomId)
                    .Distinct()
                    .ToListAsync();

                Rooms = await _context.Rooms
                    .Where(r => roomIds.Contains(r.RoomId))
                    .ToListAsync();


                ItemCardsFromReport = (from di in dispensedItems
                                       join ic in _context.ItemCards on di.ItemCardId equals ic.Id
                                       join unit in _context.Units on ic.Item.UnitId equals unit.Id

                                       select new DeductionExtended
                                       {
                                            
                                           ItemCode = ic.Item.ItemCode,
                                           ItemName = ic.Item.ItemName,
                                           GroupCode = ic.Item.GroupCode,
                                           ItemTypeCode = ic.Item.ItemTypeCode,
                                           ItemDescription = ic.Item.ItemDescription,
                                           ItemId = ic.Id,
                                           HazardTypeName = ic.Item.HazardTypeName,
                                           ExpiryDate = ic.Item.ExpiryDate,
                                           QuantityReceived = di.Quantity,
                                           UnitOfmeasure = unit.UnitCode,
                                           //Chemical = ri.Item.Chemical
                                       }).ToList();

                var DispensingReport = _context.MaterialRequests
                .Where(di => di.RequestId == ReportId.Value).FirstOrDefault();

                if (DispensingReport != null)
                {
                    MaterialRequest = new MaterialRequest
                    {
                        DocumentNumber = DispensingReport.DocumentNumber,
                        RequestingSector = DispensingReport.RequestingSector,
                        WarehouseId = DispensingReport.WarehouseId
                    };
                }

            }

            return Page();

        }

        public async Task<JsonResult> OnGetShelvesByRoom(int roomId)
        {
            Console.WriteLine("Getting Shelves");
            var shelves = await _context.Shelves
                .Where(s => s.RoomId == roomId)
                .Select(s => new { value = s.ShelfId, text = s.ShelfNo })
                .ToListAsync();

            return new JsonResult(shelves);
        }

        public async Task<IActionResult> OnPostAsync([FromForm] int StoreId, [FromForm] DateTime OutDate, [FromForm] int PartyId, [FromForm] string DocumentNumber)
        {
            base.ExtractSessionData();
            this.ReportId = HttpContext.Session.GetInt32("DisReportId");
            this.InboxId = HttpContext.Session.GetInt32("MsgId");
            await PopulateDropdownsAsync();
            var dbContext = new LabDBContext();

    

            foreach (var item in ItemCardsFromReport)
            {   
                // 1. Reduce from ItemCard table
                var itemCard = dbContext.ItemCards.FirstOrDefault(i => i.Id == item.ItemId);
                if (itemCard != null)
                {
                    itemCard.QuantityAvailable -= item.QuantityReceived;
                }

                // 2. Reduce from ItemCardBatches table (adjust logic as needed)
                var batch = dbContext.ItemCardBatches
                        .Where(b => b.ItemCardId == item.ItemId)
                        .Where(b => b.RoomId == item.RoomId)
                        .Where(b => b.ShelfId == item.ShelfId)
                        .FirstOrDefault();

                // if (batch != null && batch.QuantityReceived >= item.QuantityReceived)
                if (batch != null)
                {
                    batch.QuantityAvailable -= item.QuantityReceived;
                }

                // 3. Reduce from ShelveItems table
                var shelveItem = dbContext.ShelveItems
                                .FirstOrDefault(s => s.ItemCardId == item.ItemId &&
                                                    s.ShelfId == item.ShelfId );

                if (shelveItem != null)
                {
                    shelveItem.QuantityAvailable -= item.QuantityReceived;
                }

                // can be used in future if deduction order is Generated before
                var newdeduction = new PendingDeduction
                {
                    StoreId = StoreId,
                    ItemCardId = item.ItemId,
                    RoomId = item.RoomId,
                    ShelfId = item.ShelfId,
                    ReduceQty = item.QuantityReceived,
                    OutDate = OutDate,
                    PartyId = PartyId,
                    DocumentNumber = DocumentNumber,
                    Status = true,
                    DeductedBy = HttpContext.Session.GetInt32("UserId").Value,
                    MaterialRequestId = this.ReportId.Value,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PendingDeductions.Add(newdeduction);
                 dbContext.SaveChanges();
                 await _context.SaveChangesAsync();
            }


            var message = dbContext.Messages.FirstOrDefault(m => m.Id == this.InboxId);

            // if (message != null)
            // {
            //     message.Type = "Assign Supervisor";
            // }
            if (message != null)
            {
                message.Type = "Deduction Done";
            }
            dbContext.SaveChanges();

            return RedirectToPage("/Requests");
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
            ViewData["Destinations"] = new SelectList(await _context.Destinations.ToListAsync(), "DId", "DestinationName");
        }
    }
}
