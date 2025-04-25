using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class viewSuppliesModel : BasePageModel
    {
        public SupplyInfo Supply { get; set; } // Changed from List to single object
        public string lblSupplies;

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
        }
    }
}
