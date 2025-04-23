using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LabMaterials.Pages
{
    public class Rep_UserActivityModel : BasePageModel
    {
        public List<VActivityLog> UsersActivities { get; set; }
        public int TotalItems { get; set; }
        public DateTime? FromDate, ToDate;

        public string lblUserActivity, lblSearch, lblUserName, lblFromDate, lblToDate, lblSubmit,
            lblAction, lblActionDetails, lblRequestingIp, lblActionTime, lblTotalItem,
            lblMaterialsReceived, lblInventory, lblHazardousMaterials, lblDistributedMaterials,
            lblDamagedItems, lblUserReport, lblExport;
        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                this.FromDate = DateTime.Today;
                this.ToDate = DateTime.Today;
                FillLables();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void FillData(string? UserName, DateTime? FromDate, DateTime? ToDate)
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                FillLables();
                //this.FromDate = FromDate;
                //this.ToDate = ToDate.AddHours(24);
                var dbContext = new LabDBContext();
                UsersActivities = dbContext.VActivityLogs.ToList();
                if (FromDate is not null && FromDate != DateTime.MinValue && ToDate is not null && ToDate != DateTime.MinValue)
                    UsersActivities = UsersActivities.Where(u => u.Time.Date >= FromDate && u.Time.Date <= ToDate).ToList();


                if (string.IsNullOrEmpty(UserName) == false)
                    UsersActivities = UsersActivities.Where(u => u.UserName.ToLower() == UserName.ToLower()).ToList();

                //UsersActivities = UsersActivities.Where(u => u.Time >= FromDate && u.Time <= ToDate).ToList();

                TotalItems = UsersActivities.Count();

                this.FromDate = DateTime.Today;
                this.ToDate = DateTime.Today;
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void OnPost([FromForm] string UserName, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                FillData(UserName, FromDate, ToDate);
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
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblUserName = (Program.Translations["UserName"])[Lang];
            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
            this.lblAction = (Program.Translations["Action"])[Lang];
            this.lblActionDetails = (Program.Translations["ActionDetails"])[Lang];
            this.lblRequestingIp = (Program.Translations["RequestingIp"])[Lang];
            this.lblActionTime = (Program.Translations["ActionTime"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblUserReport = (Program.Translations["UserReport"])[Lang];
            this.lblExport = (Program.Translations["Export"])[Lang];
        }
    }
}
