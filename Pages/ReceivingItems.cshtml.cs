using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LabMaterials.DB;
using System.Linq;
using LabMaterials.DB;
using LabMaterials.dtos;
using System.DirectoryServices.Protocols;


namespace LabMaterials.Pages
{
    public class ReceivingItemsModel : BasePageModel
    {
        public List<Message> InboxList { get; set; }
        public string UserFullName;
        public string UserGroupName;
        public int? UserId;
        public int InboxCount;

        public List<ReceivingReport> RequestSent { get; set; }
        public List<ReceivingReport> AllRequest { get; set; }

        public List<MaterialRequest> AllDispenseRequest { get; set; }
        public IList<Store> Warehouses { get; set; }
        public List<User> AllUsers { get; set; }
        public List<UserGroup> UserGroups { get; set; }
        public string DocumentNumber { get; set; }
        public List<Store> Stores { get; set; }

        public int CurrentPage { get; set; }
        public bool HasSearched { get; set; } = false;
        public List<string> SelectedColumns { get; set; } = new List<string>();
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }





        public string lblNewReceivingReport, lblSearch, lblPrintTable, lblExportExcel, lblTotalItem;

        public void OnGet(string? DocumentNumber, int page = 1)
        {
            var dbContext = new LabDBContext();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");

            this.UserId = HttpContext.Session.GetInt32("UserId");

            AllRequest = dbContext.ReceivingReports.ToList();
            AllDispenseRequest = dbContext.MaterialRequests.ToList();
            AllUsers = dbContext.Users.ToList();
            UserGroups = dbContext.UserGroups.ToList();
            Warehouses = dbContext.Stores.ToList();
            base.ExtractSessionData();

            if (CanManageItems)
            {
                FillLables();
                 LoadSelectedColumns();

                if (HttpContext.Request.Query.ContainsKey("page"))
                {
                    this.DocumentNumber = DocumentNumber;
                     CurrentPage = page;
                    FillData(DocumentNumber, page);
                }
                else
                {   
                    Console.WriteLine("page not available");
                }
            }
            else
            {
                Console.WriteLine("right not available");
                RedirectToPage("./Index?lang=" + Lang);
            }

            

        }

        private void FillData(string? DocumentNumber, int page = 1)
        {
            base.ExtractSessionData();
            FillLables();

           

            var dbContext = new LabDBContext();
             this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            Warehouses = dbContext.Stores.ToList();


            this.UserId = HttpContext.Session.GetInt32("UserId");

            var query = dbContext.ReceivingReports
                // .Where(r => r.CreatedBy == UserId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            if (!string.IsNullOrEmpty(DocumentNumber))
            {
                var lowerSearch = DocumentNumber.ToLower();

                 query = query
                .Where(r =>
                    !string.IsNullOrEmpty(r.DocumentNumber) &&
                    r.DocumentNumber.ToLower().Contains(lowerSearch)
                ).ToList();

            }

            TotalItems = query.Count();
            TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
            CurrentPage = page;
            var list = query.ToList();

            RequestSent = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();

            
        }

      public IActionResult OnPostAction(string DocumentNumber, string action, List<string> columns)
        {
            base.ExtractSessionData();

            if (action == "search")
            {
                CurrentPage = 1;
                this.DocumentNumber = DocumentNumber;
                HasSearched = true;

                FillData(DocumentNumber, CurrentPage);
                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "receivingRequest";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {
                    string selectedColumns = string.Join(",", columns);
                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "receivingRequest";

                    this.DocumentNumber = DocumentNumber;
                    HasSearched = true;
                    FillData(DocumentNumber, CurrentPage);
                    SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    LoadSelectedColumns();
                }
            }

            return Page();
        }

             public IActionResult OnPostView( [FromForm] int? RequestReportId)
        {
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            var dbContext = new LabDBContext();

            if ( RequestReportId.HasValue)
            {
                HttpContext.Session.SetString("ReportId", RequestReportId.Value.ToString());
                return RedirectToPage("./ViewReceivingReport");
            }
          

            return RedirectToPage("./Index", new { lang = Lang });
        }
        private void LoadSelectedColumns()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                using (var db = new LabDBContext())
                {
                    string pageName = "receivingRequest";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        // SelectedColumns = new List<string>();
                        string selectedColumns = "documentNumber,fiscalYear";
                        SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    }
                }
            }
        }

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
        private void FillLables()
        {
            this.lblNewReceivingReport = (Program.Translations["NewReceivingReport"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblPrintTable = (Program.Translations["PrintTable"])[Lang];
            this.lblExportExcel = (Program.Translations["ExportExcel"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];

        }
    }
}