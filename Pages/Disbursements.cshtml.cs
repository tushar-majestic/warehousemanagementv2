using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class DisbursementsModel : BasePageModel
    {
        public List<DisbursementInfo> Disbursement { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }
        public DateTime? FromDate, ToDate;
        public void OnGet() 
        {
            base.ExtractSessionData();
            if (CanDisburseItems)
            {
                FillLables();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
        
        public string lblDisbursements, lblSearch, lblRequesterName, lblFromStore, lblSubmit, lblItemName, lblStoreName,lblDestination, lblItemType, lblQuantity, lblItemCode, lblAddDisbursement, lblRequestReceivedDate, lblRequestingPlace, lblComments,
            lblDisbursementStatus, lblInventoryBalanced, lblEdit, lblTotalItem, lblFromDate, lblToDate;

        public void OnPostSearch([FromForm] string RequesterName, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        {
            FillData(RequesterName,FromDate,ToDate);
        }

        public IActionResult OnPostEdit([FromForm] int DisbursementID)
        {
            HttpContext.Session.SetInt32("DisbursementID", DisbursementID);

            return RedirectToPage("./EditDisbursement");
        }

        private void FillData(string? RequesterName, DateTime? FromDate, DateTime? ToDate)
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = from d in dbContext.DisbursementRequests
                            join s in dbContext.Stores on d.StoreId equals s.StoreId
                            join i in dbContext.Items on d.Itemcode equals i.ItemCode
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

                if (string.IsNullOrEmpty(RequesterName) == false)
                    query = query.Where(s => s.RequesterName.Contains(RequesterName));

                if (FromDate != null && FromDate >= DateTime.MinValue && ToDate != null && ToDate >= DateTime.MinValue)
                    query = query.Where(e => e.ReqReceivedAt.Date >= FromDate.Value.Date && e.ReqReceivedAt.Date <= ToDate.Value.Date);

                Disbursement = query.ToList();

                Disbursement = query.ToList();
                TotalItems = Disbursement.Count();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillLables()
        {
            

            this.lblDisbursements = (Program.Translations["Disbursements"])[Lang];
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
            this.lblItemName = (Program.Translations["ItemName"])[Lang]; 
            this.lblSubmit = (Program.Translations["Submit"])[Lang];

            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];


        }
    }
}
