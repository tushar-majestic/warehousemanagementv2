using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class SuppliesModel : BasePageModel
    {
        public List<SupplyInfo> Supplies { get; set; }
        public int TotalItems { get; set; }
        public string Message { get; set; }
        [BindProperty]
        public string SupplierName { get; set; }

        [BindProperty]
        public string ItemName { get; set; }
        public DateTime? FromDate, ToDate;




        public string lblSupplies, lbltypeCode, lblStoreName, lblExpiryDate, lblItemType, lblRoomName, lblShelfNumber, lblManageSuppliers, lblSearch, lblSupplierName, lblItemName, lblSubmit, lblAddSupplies,
            lblQuantityReceived, lblPurchaseOrderNo, lblInvoiceNumber, lblReceivedAt, lblInventoryBalanced, lblEdit, lblDelete,
            lblTotalItem, lblFromDate, lblToDate;
        public void OnGet()
        {
            base.ExtractSessionData();
            if (this.CanManageSupplies)
            {
                FillLables();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
        private void FillData(string SupplierName, string ItemName, DateTime? FromDate, DateTime? ToDate)
        {
            base.ExtractSessionData();
            if (this.CanManageSupplies)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = (from S in dbContext.Supplies
                             join SR in dbContext.Suppliers on S.SupplierId equals SR.SupplierId
                             join I in dbContext.Items on S.ItemId equals I.ItemId
                             join sg in dbContext.Storages on S.SupplyId equals sg.SupplyId
                             join st in dbContext.Stores on sg.StoreId equals st.StoreId
                             join sb in dbContext.Rooms on sg.RoomId equals sb.RoomId
                             select new SupplyInfo
                             {
                                 SupplyId = S.SupplyId,
                                 SupplierName = SR.SupplierName,
                                 ItemName = I.ItemName,
                                 ReceivedAt = S.ReceivedAt,
                                 InvoiceNumber = S.InvoiceNumber,
                                 InventoryBalanced = S.InventoryBalanced,
                                 PurchaseOrderNo = S.PurchaseOrderNo,
                                 ItemCode=S.ItemCode,
                                 ItemType=S.ItemType,
                                 StoreName=st.StoreName,
                                 RoomName=sb.RoomName,
                                 ShelfNumber=sg.ShelfNumber,
                                 ExpiryDate=S.ExpiryDate,
                                 
                                 QuantityReceived = S.QuantityReceived.ToString() + " " + I.Unit.UnitCode
                             });
                if (!string.IsNullOrEmpty(SupplierName) && !string.IsNullOrEmpty(ItemName))
                {
                    query = query.Where(i => i.SupplierName.Contains(SupplierName) && i.ItemName.Contains(ItemName));
                }
                if (!string.IsNullOrEmpty(SupplierName))
                {
                    query = query.Where(i => i.SupplierName.Contains(SupplierName));
                }
                if (!string.IsNullOrEmpty(ItemName))
                {
                    query = query.Where(i => i.ItemName.Contains(ItemName));
                }

                if (FromDate != null && FromDate >= DateTime.MinValue && ToDate != null && ToDate >= DateTime.MinValue)
                    query = query.Where(e => e.ExpiryDate.Date >= FromDate.Value.Date && e.ExpiryDate.Date <= ToDate.Value.Date);

                Supplies = query.ToList();

                TotalItems = Supplies.Count();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void OnPost([FromForm] string SupplierName, [FromForm] string ItemName, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        {
            this.SupplierName = SupplierName;
            this.ItemName = ItemName;
            base.ExtractSessionData();
            if (CanManageSupplies)
            {
                FillData(SupplierName, ItemName, FromDate, ToDate);
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }


        public IActionResult OnPostEdit([FromForm] int SupplyId)
        {
            HttpContext.Session.SetInt32("SupplyId", SupplyId);

            return RedirectToPage("./EditSupply");
        }
        public IActionResult OnPostView([FromForm] int SupplyId)
        {
            HttpContext.Session.SetInt32("SupplyId", SupplyId);

            return RedirectToPage("./viewSupplies");
        }

        public void OnPostDelete([FromForm] int SupplyId)
        {
            base.ExtractSessionData();
            if (CanManageSupplies)
            {
                var dbContext = new LabDBContext();

                
                var Supply = dbContext.Supplies.Single(s => s.SupplyId == SupplyId);
                dbContext.Supplies.Remove(Supply);
                
                var Storag = dbContext.Storages.Single(s => s.SupplyId == SupplyId);
                dbContext.Storages.Remove(Storag);
                dbContext.SaveChanges();
                FillData(null, null,null,null);
                Message = "Record has been deleted successfully";
                Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);

            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillLables()
        {
            

            this.lblSupplies = (Program.Translations["Supplies"])[Lang];
            this.lblManageSuppliers = (Program.Translations["ManageSuppliers"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblAddSupplies = (Program.Translations["AddSupplies"])[Lang];
            this.lblSupplierName = (Program.Translations["SupplierName"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lbltypeCode = (Program.Translations["ItemCode"])[Lang];
            this.lblItemType = (Program.Translations["ItemType"])[Lang];
            this.lblQuantityReceived = (Program.Translations["QuantityReceived"])[Lang];
            this.lblPurchaseOrderNo = (Program.Translations["PurchaseOrderNo"])[Lang];
            this.lblInvoiceNumber = (Program.Translations["InvoiceNumber"])[Lang];
            this.lblReceivedAt = (Program.Translations["ReceivedAt"])[Lang];
            this.lblInventoryBalanced = (Program.Translations["InventoryBalanced"])[Lang];
            this.lblRoomName = (Program.Translations["RoomName"])[Lang];
            this.lblShelfNumber = (Program.Translations["ShelfNumber"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblExpiryDate = (Program.Translations["ExpiryDate"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];

            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
        }
    }
}
