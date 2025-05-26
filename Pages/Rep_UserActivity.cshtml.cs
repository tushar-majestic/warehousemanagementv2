using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LabMaterials.Pages
{
    public class Rep_UserActivityModel : BasePageModel
    {
        public List<VActivityLog> UsersActivities { get; set; }
        public List<VActivityLog> UsersActivitiesAll { get; set; }
        public int TotalItems { get; set; }
        public DateTime? FromDate, ToDate;

        [BindProperty]
        public string UserName { get; set; }

        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 20;
        public int TotalPages { get; set; }
        public List<string> SelectedColumns { get; set; } = new List<string>();

        public string lblUserActivity, lblSearch, lblUserName, lblFromDate, lblToDate, lblSubmit,
            lblAction, lblActionDetails, lblRequestingIp, lblActionTime, lblTotalItem,
            lblMaterialsReceived, lblInventory, lblHazardousMaterials, lblDistributedMaterials,
            lblDamagedItems, lblUserReport, lblExport, lblPrint, lblItems;
        public void OnGet(string? UserName, DateTime? FromDate, DateTime? ToDate, int page = 1)
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                // this.FromDate = DateTime.Today;
                // this.ToDate = DateTime.Today;
                FillLables();
                LoadSelectedColumns();
                if (HttpContext.Request.Query.ContainsKey("page"))
                {
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.UserName = UserName;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    FillData(UserName, FromDate, ToDate, page);

                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void LoadSelectedColumns()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                using (var db = new LabDBContext())
                {
                    string pageName = "UserActivity";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        // SelectedColumns = new List<string>();
                        string selectedColumns = "userName,requestingIP";
                        SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    }
                }
            }
        }
        // function before pagination 
        /*public void FillData(string? UserName, DateTime? FromDate, DateTime? ToDate)
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
        }*/

        private void SaveSelectedColumns(int userId, string pageName, string selectedColumns)
        {
            base.ExtractSessionData();
            using (var db = new LabDBContext())
            {
                var existingRecord = db.Tablecolumns
                    .FirstOrDefault(c => c.UserId == userId && c.Page == pageName);

                if (existingRecord != null)
                {
                    existingRecord.DisplayColumns = selectedColumns;
                }
                else
                {
                    var newRecord = new Tablecolumn
                    {
                        UserId = userId,
                        Page = pageName,
                        DisplayColumns = selectedColumns
                    };
                    db.Tablecolumns.Add(newRecord);
                }

                db.SaveChanges();
            }
        }

        public void FillData(string? UserName, DateTime? FromDate, DateTime? ToDate, int page = 1)
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                FillLables();
                //this.FromDate = FromDate;
                //this.ToDate = ToDate.AddHours(24);
                var dbContext = new LabDBContext();
                var query = dbContext.VActivityLogs.ToList();
                if (FromDate is not null && FromDate != DateTime.MinValue && ToDate is not null && ToDate != DateTime.MinValue)
                    query = query.Where(u => u.Time.Date >= FromDate && u.Time.Date <= ToDate).ToList();


                if (string.IsNullOrEmpty(UserName) == false)
                    query = query.Where(u => u.UserName.ToLower() == UserName.ToLower()).ToList();

                //UsersActivities = UsersActivities.Where(u => u.Time >= FromDate && u.Time <= ToDate).ToList();

                TotalItems = query.Count();
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

                var list = query.ToList();
                //UsersActivities = list;
                UsersActivities = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
                UsersActivitiesAll = query.ToList();

                CurrentPage = page;

                // this.FromDate = DateTime.Today;
                // this.ToDate = DateTime.Today;
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        // public void OnPost([FromForm] string UserName, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        // {
        //     base.ExtractSessionData();
        //     this.UserName = UserName;
        //     CurrentPage = 1; 
        //     this.FromDate = FromDate;
        //     this.ToDate = ToDate;
        //     if (CanSeeReports)
        //     {
        //         FillData(UserName, FromDate, ToDate, CurrentPage);
        //     }
        //     else
        //         RedirectToPage("./Index?lang=" + Lang);
        // }
        public IActionResult OnPostAction(string UserName, DateTime? FromDate, DateTime? ToDate, string action, List<string> columns)
        {
            base.ExtractSessionData();

            if (action == "search")
            {
                this.UserName = UserName;
                CurrentPage = 1;
                this.FromDate = FromDate;
                this.ToDate = ToDate;
                if (CanSeeReports)
                {

                    FillData(UserName, FromDate, ToDate, CurrentPage);

                }

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "UserActivity";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {

                    string selectedColumns = string.Join(",", columns);

                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "UserActivity";
                    this.UserName = UserName;
                    CurrentPage = 1;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    {

                        FillData(UserName, FromDate, ToDate, CurrentPage);

                    }
                    SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    LoadSelectedColumns();
                }

            }

            return Page();
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
            this.lblPrint = (Program.Translations["Print"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];
        }
    }
}
