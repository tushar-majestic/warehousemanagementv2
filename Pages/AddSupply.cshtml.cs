using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkiaSharp;

namespace LabMaterials.Pages
{
    public class AddSupplyModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string InvoiceNumber, PurchaseOrderNo, TypeName, itemTypeCode, Supplier_Name, Store_Name,
            Room_Name,ShelfNumber, ItemName;
        public bool InventoryBalanced;
        public int QuantityReceived, SupplierId, ItemId,StoreId,RoomId,shelfId;
        public DateTime ReceivedAt, ExpiryDate;
        public List<Supplier> Suppliers {  get; set; }
        public List<Item> Items { get; set; }
        public List<ItemType> ItemTypeCode { get; set; }
        public List<Item> ItemCode { get; set; }
        public List<Supplier> SupplierName { get; set; }
        public List<Store> StoreName { get; set; }
        public List<Room> rooms { get; set; }
        public List<Shelf> Shelf { get; set; }
        //public List<ItemCode> ItemCodes { get; set; }

        public string lblAddSupplies, lblSupplierName, lblExpiryDate, lblItemName, lblTypeName, lblItemCode, lblItemTypeCode, lblStoreName,lblRoomName, lblShelvesNumber,
            lblQuantityReceived, lblPurchaseOrderNo, lblInvoiceNumber, lblReceivedAt, lblInventoryBalanced, lblAdd, lblCancel;

        public void OnGet(int storeId)
        {
            base.ExtractSessionData();
            if (CanManageSupplies == false)
                RedirectToPage("./Index?lang=" + Lang);
            FillLables();
            using (var dbContext = new LabDBContext())
            {
                Suppliers = dbContext.Suppliers.ToList();
                Items = dbContext.Items.ToList();
                ItemTypeCode = dbContext.ItemTypes.ToList();
                ItemCode = dbContext.Items.ToList();
                SupplierName = dbContext.Suppliers.ToList();
                StoreName = dbContext.Stores.ToList();
                rooms = dbContext.Rooms.ToList();
                Shelf = dbContext.Shelves.ToList();
                //RoomName = dbContext.Rooms.Where(r => r.StoreId == storeId).ToList();
            }
        }
        public IActionResult OnGetRoomsForStore(int storeId,int roomId)
        {
            var dbContext = new LabDBContext();
            var rooms = dbContext.Rooms.Where(r => r.StoreId == storeId && r.Ended==null)
                        .Select(r => new { r.RoomId, r.RoomName, r.RoomNo })
                        .ToList();
            
            return new JsonResult(rooms);

        }
        public IActionResult OnGetShelvesForRoom(int roomId)
        {
            var dbContext = new LabDBContext();
            var shelves = dbContext.Shelves
                .Where(s => s.RoomId == roomId && s.Ended == null)
                .Select(s =>  s.ShelfNo  )
                .ToList();

            return new JsonResult(shelves);
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
        public IActionResult OnPost([FromForm] int ItemId, [FromForm] int SupplierId, [FromForm] string InvoiceNumber, [FromForm] string TypeName, [FromForm] string itemTypeCode, [FromForm] int StoreId, [FromForm] int RoomId, [FromForm] string ShelfNumber, 
          [FromForm] string PurchaseOrderNo, [FromForm] DateTime ReceivedAt,[FromForm] DateTime ExpiryDate, [FromForm] bool InventoryBalanced, [FromForm] int QuantityReceived)
             {
            LogableTask task = LogableTask.NewTask("AddSupply");

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
                    this.ExpiryDate= ExpiryDate;
                    this.RoomId= RoomId;
                    this.ShelfNumber= ShelfNumber;
                    this.QuantityReceived = QuantityReceived;
                    this.InventoryBalanced = InventoryBalanced;

                   
                    
                     if (StoreId==null)
                        ErrorMsg = (Program.Translations["SupplierNameisMissing"])[Lang];
                     
                    else if (string.IsNullOrEmpty(ShelfNumber))
                        ErrorMsg = (Program.Translations["ShelfIsMissing"])[Lang];
                    else if (string.IsNullOrEmpty(itemTypeCode))
                        ErrorMsg = (Program.Translations["ItemCodeIsMissing"])[Lang];
                    else if (string.IsNullOrEmpty(TypeName))
                        ErrorMsg = (Program.Translations["TypeNameIsMissing"])[Lang];
                    else if (string.IsNullOrEmpty(InvoiceNumber))
                        ErrorMsg = (Program.Translations["InvoiceNumberMissing"])[Lang];
                    else if (string.IsNullOrEmpty(PurchaseOrderNo))
                        ErrorMsg = (Program.Translations["PurchaseOrderNumberIsMissing"])[Lang];
                    else if (ReceivedAt < Convert.ToDateTime("1/1/1753 12:00:00 AM"))
                        ErrorMsg = (Program.Translations["ReceivedAtMissing"])[Lang];
                    else if (ExpiryDate < Convert.ToDateTime("1/1/1753 12:00:00 AM"))
                        ErrorMsg = (Program.Translations["ExpiryDateMissing"])[Lang];
                   
                    else if (QuantityReceived <= 0)
                        ErrorMsg = (Program.Translations["QuantityReceivedMissing"])[Lang];
                    else
                    {
                        var dbContext = new LabDBContext();
                        if (dbContext.Supplies.Count(s => s.InvoiceNumber == InvoiceNumber) > 0)
                            ErrorMsg = string.Format((Program.Translations["InvoiceNumberExists"])[Lang], InvoiceNumber);
                        else if (dbContext.Supplies.Count(s => s.PurchaseOrderNo == PurchaseOrderNo) > 0)
                            ErrorMsg = string.Format((Program.Translations["PurchaseOrderNoExists"])[Lang], PurchaseOrderNo);
                        else
                        {

                            var supply = new Supply
                            {
                                SupplyId = PrimaryKeyManager.GetNextId(),
                                ItemId = ItemId,
                                SupplierId = SupplierId,
                                ItemCode = itemTypeCode,
                                ItemType = TypeName,
                                InvoiceNumber = InvoiceNumber,
                                PurchaseOrderNo = PurchaseOrderNo,
                                ReceivedAt = ReceivedAt,
                                ExpiryDate = ExpiryDate,
                                QuantityReceived = QuantityReceived,
                                InventoryBalanced = InventoryBalanced
                            };
                            dbContext.Supplies.Add(supply);
                            dbContext.SaveChanges();

                            var storage = new Storage()
                            {
                                SupplyId= supply.SupplyId,
                                ItemId= ItemId,
                                StoreId= StoreId,
                                ShelfNumber = ShelfNumber,
                                ExpiryDate = ExpiryDate,
                                RoomId=RoomId,
                                AvailableQuantity = QuantityReceived
                        };

                            dbContext.Storages.Add(storage);
                            dbContext.SaveChanges();

                            if (InventoryBalanced)
                            {
                                var item = dbContext.Items.Single(i => i.ItemId == ItemId);
                                item.AvailableQuantity += QuantityReceived;
                                dbContext.SaveChanges();
                            }
                           //string successMessage= string.Format((Program.Translations["PurchaseOrderNoExists"])[Lang], PurchaseOrderNo);
                            task.LogInfo(MethodBase.GetCurrentMethod(), "Supply added");
                          
                            // string Message = string.Format("Supply for item {0} from supplier {1} added", Items.Single(i => i.ItemId == ItemId).ItemName, Suppliers.Single(s => s.SupplierId == SupplierId).SupplierName);
                            string Messagee = "Supply for item " + ItemName + " from supplier " + SupplierName +" added successfully on" + DateTime.Now.ToString();
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Messagee, "Add",
                                Helper.ExtractIP(Request), dbContext, true);
                            //TempData["SuccessMessage"] = successMessage;
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
            

            this.lblAddSupplies = (Program.Translations["AddSupplies"])[Lang];
            this.lblSupplierName = (Program.Translations["SupplierName"])[Lang];
            this.lblTypeName = (Program.Translations["TypeName"])[Lang];
            
            this.lblItemTypeCode = (Program.Translations["ItemTypeCode"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblShelvesNumber = (Program.Translations["ShelfNumber"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblRoomName = (Program.Translations["RoomName"])[Lang];
            this.lblQuantityReceived = (Program.Translations["QuantityReceived"])[Lang];
            this.lblPurchaseOrderNo = (Program.Translations["PurchaseOrderNo"])[Lang];
            this.lblInvoiceNumber = (Program.Translations["InvoiceNumber"])[Lang];
            this.lblReceivedAt = (Program.Translations["ReceivedAt"])[Lang];
            this.lblExpiryDate = (Program.Translations["ExpiryDate"])[Lang];
            this.lblInventoryBalanced = (Program.Translations["InventoryBalanced"])[Lang];

            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
        }
    }
}
