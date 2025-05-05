using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client.Extensions.Msal;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions;

namespace LabMaterials.Pages
{
    public class RequestsModel : BasePageModel
    {
        public string lblRequests, lblNewReceivingReport, pagetype = "inbox", inboxClass = "btn-dark text-white", outboxClass = "btn-light";

        public List<ReceivingReport> RequestSent { get; set; }
        public IList<Store> Warehouses { get; set; }

        public void OnGet()
        {
            if (HttpContext.Request.Query.ContainsKey("type"))
            {
                pagetype = HttpContext.Request.Query["type"];

                if (pagetype == "inbox")
                {
                    inboxClass = "btn-dark text-white";
                    outboxClass = "btn-light";
                }
                else
                {
                    inboxClass = "btn-light";
                    outboxClass = "btn-dark text-white";
                }
            }
            var dbContext = new LabDBContext();
            RequestSent = dbContext.ReceivingReports.ToList(); 
            Warehouses = dbContext.Stores.ToList();  // Fetch suppliers
 
            base.ExtractSessionData();
            FillLables();
        }


         public IActionResult OnPostView([FromForm] string ReportId)
        {
                   var dbContext = new LabDBContext();

            HttpContext.Session.SetString("ReportId", ReportId);
            RequestSent = dbContext.ReceivingReports.ToList(); 


            return RedirectToPage("./ViewReceivingReport");
        }


        private void FillLables()
        {
            this.lblRequests = (Program.Translations["Requests"])[Lang];
            this.lblNewReceivingReport = (Program.Translations["NewReceivingReport"])[Lang];
        }
    }
}
