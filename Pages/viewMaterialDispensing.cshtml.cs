using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabMaterials.dtos;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class viewMaterialDispensingModel : BasePageModel
    {
        public List<DisbursementInfo> Disbursement { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }
        [BindProperty]
        public string RequesterName { get; set; }

        public string lblItemName, lblDisbursements, lblSearch, lblRequesterName, lblFromStore, lblSubmit, lblStoreName, lblDestination, lblItemType, lblQuantity, lblItemCode, lblAddDisbursement, lblRequestReceivedDate, lblRequestingPlace, lblComments,
            lblDisbursementStatus, lblInventoryBalanced, lblEdit, lblTotalItem;


        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
                var dbContext = new LabDBContext();
                int? DisbursementID = HttpContext.Session.GetInt32("DisbursementID");

                var query = from d in dbContext.DisbursementRequests
                            join s in dbContext.Stores on d.StoreId equals s.StoreId
                            join i in dbContext.Items on d.Itemcode equals i.ItemCode
                            where d.DisbursementRequestId == DisbursementID
                            select new DisbursementInfo
                            {
                                DisbursementRequestId = d.DisbursementRequestId,
                                RequesterName = d.RequesterName,
                                RequestingPlace = d.RequestingPlace,
                                Comments = d.Comments,
                                ReqReceivedAt = d.ReqReceivedAt,
                                Status = d.Status,
                                InventoryBalanced = d.InventoryBalanced ? "Yes" : "No",
                                ItemCode = d.Itemcode,
                                ItemTypeCode = d.Itemtypecode,
                                Quantity = d.ItemQuantity,
                                StoreName = s.StoreName,
                                ItemName = i.ItemName
                            };

                Disbursement = query.ToList();

            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
        private void FillLables()
        {

            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblRequesterName = (Program.Translations["RequesterName"])[Lang];
            this.lblAddDisbursement = (Program.Translations["AddDisbursement"])[Lang];
            this.lblRequestReceivedDate = (Program.Translations["RequestReceivedDate"])[Lang];
            this.lblRequestingPlace = (Program.Translations["RequestingPlace"])[Lang];
            this.lblComments = (Program.Translations["Comments"])[Lang];
            this.lblDisbursementStatus = (Program.Translations["DisbursementStatus"])[Lang];
            this.lblInventoryBalanced = (Program.Translations["InventoryBalanced"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblFromStore = (Program.Translations["FromStore"])[Lang];
            this.lblItemType = (Program.Translations["ItemType"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblDestination = (Program.Translations["Destinations"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblDisbursements = (Program.Translations["Disbursements"])[Lang];
        }
    }
}
