using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LabMaterials.Pages
{
    public class EditSupplyModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string InvoiceNumber, ShelfNo, itemTypeCode, storee, PurchaseOrderNo, Room_Name, ShelfNumber, ItemName, TypeName;
        public bool InventoryBalanced;
        public int QuantityReceived, SupplierId, ItemId, SupplyId, StoreId, RoomId, QuantityReceivedd;
        public DateTime ReceivedAt, ExpiryDate;

        public List<Supplier> Suppliers { get; set; }
        public List<Item> Items { get; set; }

        public List<ItemType> ItemTypeCode { get; set; }
        public List<Storage> storages { get; set; }

        public List<Store> StoreName { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Shelf> Shelf { get; set; }
        public int page { get; set; }
         public string FromDate, ToDate;
        public string lblUpdateSupplies, lblSupplierName, lblStoreName, lblTypeName, lblExpiryDate, lblRoomName, lblShelvesNumber, lblItemName, lblItemTypeCode,
            lblQuantityReceived, lblPurchaseOrderNo, lblInvoiceNumber, lblReceivedAt, lblInventoryBalanced, lblUpdate, lblCancel, lblSupplies;

        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageSupplies == false)
                RedirectToPage("./Index?lang=" + Lang);
            FillLables();
            this.page = (int)HttpContext.Session.GetInt32("page");
            this.FromDate = HttpContext.Session.GetString("FromDate");
            this.ToDate = HttpContext.Session.GetString("ToDate");
            using (var dbContext = new LabDBContext())
            {


                Suppliers = dbContext.Suppliers.ToList();
                Items = dbContext.Items.ToList();
                StoreName = dbContext.Stores.ToList();


                var supply = dbContext.Supplies.Single(i => i.SupplyId == HttpContext.Session.GetInt32("SupplyId"));

                this.SupplyId = supply.SupplyId;
                this.ItemId = supply.ItemId;
                this.SupplierId = supply.SupplierId;
                this.QuantityReceived = supply.QuantityReceived;
                this.InventoryBalanced = supply.InventoryBalanced;
                this.itemTypeCode = supply.ItemCode;
                this.TypeName = supply.ItemType;

                this.ReceivedAt = supply.ReceivedAt;
                this.ExpiryDate = supply.ExpiryDate;
                this.InvoiceNumber = supply.InvoiceNumber;
                this.PurchaseOrderNo = supply.PurchaseOrderNo;

                var aaa = dbContext.Storages.FirstOrDefault(r => r.SupplyId == SupplyId)?.StoreId;
                var bbb = dbContext.Storages.FirstOrDefault(r => r.SupplyId == SupplyId)?.RoomId;
                var ccc = dbContext.Storages.FirstOrDefault(r => r.SupplyId == SupplyId)?.ShelfNumber;
                ViewData["StoreId"] = aaa;
                ViewData["RoomId"] = bbb;
                ViewData["ShelfNo"] = ccc;

            }
        }

        public IActionResult OnGetItemTypeAndCode(int ItemId)
        {
            using (var dbContext = new LabDBContext())
            {
                var result = (from ty in dbContext.ItemTypes
                              join it in dbContext.Items on ty.ItemTypeCode equals it.ItemTypeCode
                              where it.ItemId == ItemId && it.Ended == null
                              select new { TypeName = ty.TypeName, ItemTypeCode = it.ItemCode }).ToList();
                return new JsonResult(result);
            }
        }

        public IActionResult OnGetRoomsForStore(int storeId)
        {
            var dbContext = new LabDBContext();
            var rooms = dbContext.Rooms.Where(r => r.StoreId == storeId && r.Ended == null)
                            .Select(r => new { r.RoomId, r.RoomName, r.RoomNo })
                            .ToList();

            //var stg= dbContext.Storages.Where(r => r.StoreId == storeId && r.Ended == null)
            //                 .Select(r => new { r.RoomId })
            //                 .ToList();
            // var query = (from S in dbContext.Storages
            //              join R in dbContext.Rooms on S.RoomId equals R.RoomId
            //              where R.Ended == null && S.StoreId == storeId
            //              select new { RoomName = R.RoomName });




            return new JsonResult(rooms);
        }
        public IActionResult OnGetShelvesForRoom(int roomId)
        {
            var dbContext = new LabDBContext();
            var shelves = dbContext.Shelves
                .Where(s => s.RoomId == roomId && s.Ended == null)
                .Select(s => s.ShelfNo)
                .ToList();


            return new JsonResult(shelves);
        }


        public IActionResult OnPost([FromForm] int RoomId, [FromForm] int SupplyId, [FromForm] int ItemId, [FromForm] int SupplierId, [FromForm] string ShelfNumber, [FromForm] string itemTypeCode, [FromForm] string TypeName, [FromForm] string InvoiceNumber,
            [FromForm] string PurchaseOrderNo, [FromForm] DateTime ReceivedAt, [FromForm] DateTime ExpiryDate, [FromForm] bool InventoryBalanced, [FromForm] int QuantityReceived)
        {
            LogableTask task = LogableTask.NewTask("EditSupply");

            try
            {

                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();

                if (CanManageSupplies)
                {
                    FillLables();
                    this.ItemId = ItemId;
                    this.SupplierId = SupplierId;
                    this.InvoiceNumber = InvoiceNumber;
                    this.TypeName = TypeName;
                    this.itemTypeCode = itemTypeCode;
                    this.StoreId = StoreId;
                    this.ShelfNumber = ShelfNumber;
                    this.PurchaseOrderNo = PurchaseOrderNo;
                    this.ReceivedAt = ReceivedAt;
                    this.ExpiryDate = ExpiryDate;
                    this.QuantityReceived = QuantityReceived;
                    this.InventoryBalanced = InventoryBalanced;
                    this.RoomId = RoomId;
                    var dbContext = new LabDBContext();
                    Suppliers = dbContext.Suppliers.ToList();
                    Items = dbContext.Items.ToList();

                    if (string.IsNullOrEmpty(InvoiceNumber))
                        ErrorMsg = (Program.Translations["InvoiceNumberMissing"])[Lang];
                    else if (string.IsNullOrEmpty(PurchaseOrderNo))
                        ErrorMsg = (Program.Translations["PurchaseOrderNoMissing"])[Lang];
                    else if (QuantityReceived <= 0)
                        ErrorMsg = (Program.Translations["QuantityReceivedMissing"])[Lang];
                    else if (ReceivedAt < Convert.ToDateTime("1/1/1753 12:00:00 AM"))
                        ErrorMsg = (Program.Translations["ReceivedAtMissing"])[Lang];
                    else
                    {

                        var supplyy = dbContext.Supplies.Single(i => i.SupplyId == HttpContext.Session.GetInt32("SupplyId"));


                        this.QuantityReceivedd = supplyy.QuantityReceived;


                        Suppliers = dbContext.Suppliers.ToList();
                        Items = dbContext.Items.ToList();
                        if (dbContext.Supplies.Count(s => s.InvoiceNumber == InvoiceNumber && s.SupplyId != SupplyId) > 0)
                            ErrorMsg = string.Format((Program.Translations["InvoiceNumberExists"])[Lang], InvoiceNumber);
                        else if (dbContext.Supplies.Count(s => s.PurchaseOrderNo == PurchaseOrderNo && s.SupplyId != SupplyId) > 0)
                            ErrorMsg = string.Format((Program.Translations["PurchaseOrderNoExists"])[Lang], PurchaseOrderNo);
                        else
                        {
                            int QuantityReceiveddDifference = 0;


                            var supply = dbContext.Supplies.Single(s => s.SupplyId == SupplyId);

                            if (!supply.InventoryBalanced && InventoryBalanced)
                            {

                                var item = dbContext.Items.Single(i => i.ItemId == ItemId);
                                item.AvailableQuantity += QuantityReceived;

                                dbContext.SaveChanges();
                            }
                            else if (supply.InventoryBalanced && !InventoryBalanced)
                            {
                                var item = dbContext.Items.Single(i => i.ItemId == ItemId);
                                item.AvailableQuantity -= QuantityReceived;

                                dbContext.SaveChanges();
                            }
                            else if (supply.InventoryBalanced && InventoryBalanced)
                            {
                                var item = dbContext.Items.Single(i => i.ItemId == ItemId);
                                if (QuantityReceived != QuantityReceivedd)
                                {
                                    QuantityReceiveddDifference = QuantityReceived - QuantityReceivedd;
                                    item.AvailableQuantity += QuantityReceiveddDifference;
                                }

                                dbContext.SaveChanges();

                            }
                            supply.ItemId = ItemId;
                            supply.SupplierId = SupplierId;
                            supply.InvoiceNumber = InvoiceNumber;
                            supply.PurchaseOrderNo = PurchaseOrderNo;
                            supply.ReceivedAt = ReceivedAt;
                            supply.QuantityReceived = QuantityReceived;
                            supply.InventoryBalanced = InventoryBalanced;
                            supply.ItemType = TypeName;
                            supply.ItemCode = itemTypeCode;
                            supply.ExpiryDate = ExpiryDate;

                            dbContext.SaveChanges();

                            var Storag = dbContext.Storages.Single(s => s.SupplyId == SupplyId);
                            Storag.ItemId = ItemId;

                            Storag.ShelfNumber = ShelfNumber;
                            Storag.ExpiryDate = ExpiryDate;
                            Storag.RoomId = RoomId;

                            dbContext.SaveChanges();

                            task.LogInfo(MethodBase.GetCurrentMethod(), "Supply Updated");

                            string Message = string.Format("Supply for item {0} from supplier {1} updated", Items.Single(i => i.ItemId == ItemId).ItemName, Suppliers.Single(s => s.SupplierId == SupplierId).SupplierName);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Update",
                                Helper.ExtractIP(Request), dbContext, true);

                            return RedirectToPage("./Supplies");
                        }
                    }
                    return Page();
                }
                else
                    return RedirectToPage("./Index?lang=" + Lang);
            }
            catch (Exception ex)
            {
                task.LogError(MethodBase.GetCurrentMethod(), ex);
                ErrorMsg = ex.Message;
                return Page();
            }
            finally { task.EndTask(); }
        }

        private void FillLables()
        {


            this.lblUpdateSupplies = (Program.Translations["UpdateSupplies"])[Lang];
            this.lblSupplierName = (Program.Translations["SupplierName"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblQuantityReceived = (Program.Translations["QuantityReceived"])[Lang];
            this.lblPurchaseOrderNo = (Program.Translations["PurchaseOrderNo"])[Lang];
            this.lblInvoiceNumber = (Program.Translations["InvoiceNumber"])[Lang];
            this.lblReceivedAt = (Program.Translations["ReceivedAt"])[Lang];
            this.lblInventoryBalanced = (Program.Translations["InventoryBalanced"])[Lang];

            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblRoomName = (Program.Translations["RoomName"])[Lang];
            this.lblShelvesNumber = (Program.Translations["ShelfNumber"])[Lang];
            this.lblTypeName = (Program.Translations["TypeName"])[Lang];
            this.lblItemTypeCode = (Program.Translations["ItemTypeCode"])[Lang];

            this.lblUpdate = (Program.Translations["Update"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblSupplies = (Program.Translations["Supplies"])[Lang];
        }
    }
}
