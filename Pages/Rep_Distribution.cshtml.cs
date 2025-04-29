using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class Rep_DistributionModel : BasePageModel
    {
        public DateTime? FromDate, ToDate;
        public List<DisbursementInfo> Disbursement { get; set; }
        public int TotalItems { get; set; }

        [BindProperty]
        public string RequesterName { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
    


        public string lblDistributedMaterials, lblSearch, lblRequesterName, lblRequestReceivedDate, lblRequestingPlace, lblComments,
            lblDisbursementStatus, lblInventoryBalanced, lblTotalItem,
            lblMaterialsReceived, lblInventory, lblHazardousMaterials, lblUserActivity, lblDamagedItems,
            lblUserReport, lblExport, lblFromDate, lblToDate;
        public void OnGet(string? RequesterName, DateTime? FromDate, DateTime? ToDate, int page = 1)
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                // this.FromDate = DateTime.Today;
                // this.ToDate = DateTime.Today;
                FillLables();
                if (HttpContext.Request.Query.ContainsKey("page")){
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.RequesterName = RequesterName;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    FillData(RequesterName, FromDate, ToDate, page);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillData(string? RequesterName, DateTime? FromDate, DateTime? ToDate, int page = 1)
        {   if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = from d in dbContext.DisbursementRequests
                            select new DisbursementInfo
                            {
                                DisbursementRequestId = d.DisbursementRequestId,
                                RequesterName = d.RequesterName,
                                RequestingPlace = d.RequestingPlace,
                                Comments = d.Comments,
                                ReqReceivedAt = d.ReqReceivedAt,
                                Status = d.Status,
                                InventoryBalanced = d.InventoryBalanced ? "Yes" : "No"
                            };

                if (string.IsNullOrEmpty(RequesterName) == false)
                    query = query.Where(s => s.RequesterName.Contains(RequesterName));
                if (FromDate is not null && FromDate != DateTime.MinValue && ToDate is not null && ToDate != DateTime.MinValue)
                    query = query.Where(e => e.ReqReceivedAt >= FromDate && e.ReqReceivedAt <= ToDate);

                // Disbursement = query.ToList();

                // Disbursement = query.ToList();
                // TotalItems = Disbursement.Count();

                // this.FromDate = DateTime.Today;
                // this.ToDate = DateTime.Today;

                TotalItems = query.Count();
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

                var list = query.ToList();
                Disbursement = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();     
                CurrentPage = page;
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void OnPost([FromForm] string RequesterName, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        {
            base.ExtractSessionData();
            this.RequesterName = RequesterName;
            this.FromDate = FromDate;
            this.ToDate = ToDate;
            CurrentPage = 1;
            if (CanSeeReports)
            {
                FillData(RequesterName, FromDate, ToDate, CurrentPage);
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillLables()
        {

            this.lblMaterialsReceived = (Program.Translations["MaterialsReceived"])[Lang];
            this.lblInventory = (Program.Translations["Inventory"])[Lang];
            this.lblHazardousMaterials = (Program.Translations["HazardousMaterials"])[Lang];
            this.lblUserActivity = (Program.Translations["UserActivity"])[Lang];
            this.lblDistributedMaterials = (Program.Translations["DistributedMaterials"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblRequesterName = (Program.Translations["RequesterName"])[Lang];
            this.lblRequestReceivedDate = (Program.Translations["RequestReceivedDate"])[Lang];
            this.lblRequestingPlace = (Program.Translations["RequestingPlace"])[Lang];
            this.lblComments = (Program.Translations["Comments"])[Lang];
            this.lblDisbursementStatus = (Program.Translations["DisbursementStatus"])[Lang];
            this.lblInventoryBalanced = (Program.Translations["InventoryBalanced"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblUserReport = (Program.Translations["UserReport"])[Lang];
            this.lblExport = (Program.Translations["Export"])[Lang];
            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
        }
    }
}
