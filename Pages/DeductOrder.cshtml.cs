using DocumentFormat.OpenXml.Office.CustomUI;
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
    public class DeductOrderModel : BasePageModel
    {
        public List<SelectListItem> ItemList { get; set; }
        private readonly LabDBContext _context;
        public MaterialRequest MaterialRequest { get; set; } = new MaterialRequest();

        public List<Room> Rooms { get; set; }
        public List<Shelf> Shelves { get; set; }

        public DeductOrderModel(LabDBContext context)
        {
            _context = context;
        }

        public int? ReportId;
        public List<LabMaterials.DB.ItemCardExtended> ItemCardsFromReport { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            base.ExtractSessionData();
            await PopulateDropdownsAsync();
            this.ReportId = HttpContext.Session.GetInt32("DisReportId");


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
                                       
                                       select new LabMaterials.DB.ItemCardExtended
                                       {
                                           ItemCode = ic.Item.ItemCode,
                                           ItemName = ic.Item.ItemName,
                                           GroupCode = ic.Item.GroupCode,
                                           ItemTypeCode = ic.Item.ItemTypeCode,
                                           ItemDescription = ic.Item.ItemDescription,
                                           ItemId = ic.ItemId,
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
                    MaterialRequest =  new MaterialRequest
                    {
                        DocumentNumber = DispensingReport.DocumentNumber,
                        RequestingSector = DispensingReport.RequestingSector,
                        WarehouseName = DispensingReport.WarehouseName
                    };
                }
              
            }

            return Page();

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
