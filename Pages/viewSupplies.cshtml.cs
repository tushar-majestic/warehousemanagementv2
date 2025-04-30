using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class viewSuppliesModel : BasePageModel
    {
        public SupplyInfo Supply { get; set; } // Changed from List to single object
        public string lblSupplies, lbltypeCode, lblStoreName, lblExpiryDate, lblItemType, lblRoomName, lblShelfNumber, lblManageSuppliers, lblSearch, lblSupplierName, lblItemName, lblSubmit, lblAddSupplies,
            lblQuantityReceived, lblPurchaseOrderNo, lblInvoiceNumber, lblReceivedAt, lblInventoryBalanced, lblEdit, lblDelete,
            lblTotalItem, lblFromDate, lblToDate;

        public void OnGet()
        {
            base.ExtractSessionData();
            if (!this.CanManageSupplies)
            {
                Response.Redirect("./Index?lang=" + Lang);
                return;
            }

            FillLables();

            var dbContext = new LabDBContext();
            int? supplyId = HttpContext.Session.GetInt32("SupplyId");

            if (supplyId == null)
            {
                Supply = null;
                return;
            }

            var query = (from S in dbContext.Supplies
                         join SR in dbContext.Suppliers on S.SupplierId equals SR.SupplierId
                         join I in dbContext.Items on S.ItemId equals I.ItemId
                         join sg in dbContext.Storages on S.SupplyId equals sg.SupplyId
                         join st in dbContext.Stores on sg.StoreId equals st.StoreId
                         join sb in dbContext.Rooms on sg.RoomId equals sb.RoomId
                         where S.SupplyId == supplyId
                         select new SupplyInfo
                         {
                             SupplyId = S.SupplyId,
                             SupplierName = SR.SupplierName,
                             ItemName = I.ItemName,
                             ReceivedAt = S.ReceivedAt,
                             InvoiceNumber = S.InvoiceNumber,
                             InventoryBalanced = S.InventoryBalanced,
                             PurchaseOrderNo = S.PurchaseOrderNo,
                             ItemCode = S.ItemCode,
                             ItemType = S.ItemType,
                             StoreName = st.StoreName,
                             RoomName = sb.RoomName,
                             ShelfNumber = sg.ShelfNumber,
                             ExpiryDate = S.ExpiryDate,
                             QuantityReceived = S.QuantityReceived.ToString() + " " + I.Unit.UnitCode
                         }).FirstOrDefault();

            Supply = query;
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
