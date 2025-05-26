using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class Rep_DamagedItemsModel : BasePageModel
    {
        public DateTime? FromDate, ToDate;
        public List<DamagedItemsInfo> DamagedItems;
        public List<DamagedItemsInfo> DamagedItemsAll;
        public string lblHazardousMaterials, lblHazardTypeName, lblSearch, lblSubmit, lblItemCode, lblItemName, lblGroupName,
            lblAvailableQuantity, lblHazardType, lblTypeName, lblStoreName, lblUnitCode, lblTotalItem,
            lblMaterialsReceived, lblInventory, lblUserActivity, lblDistributedMaterials, lblDamagedItems, lblUserReport,
            lblFromDate, lblToDate, lblDamageditems, lblTotalDamagedItem, lblItems,
            lblDamageDate, lblDamageQuantity, lblDamageReason, lblExport, lblPrint;
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }

        [BindProperty]
        public string ItemName { get; set; }
        public List<string> SelectedColumns { get; set; } = new List<string>();
        public void OnGet(string? ItemName, DateTime? FromDate, DateTime? ToDate, int page = 1)
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                // this.FromDate = DateTime.Today;
                // this.ToDate = DateTime.Today;
                FillLables();
                LoadSelectedColumns();
                if (HttpContext.Request.Query.ContainsKey("page")){
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.ItemName = ItemName;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    FillData(ItemName, FromDate, ToDate, page);
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
                    string pageName = "ReturnItemReport";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        // SelectedColumns = new List<string>();
                        string selectedColumns = "itemName,itemCode";
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
        public void FillData(string ItemNAme, DateTime? StartDate, DateTime? EndDate, int page = 1)
        {
            if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }
            var dbContext = new LabDBContext();
            var query = (from i in dbContext.Items
                         join di in dbContext.DamagedItems on i.ItemId equals di.ItemId

                         select new DamagedItemsInfo
                         {
                             ItemCode = i.ItemCode,
                             ItemName = i.ItemName,
                             DamageQuantity = di.DamagedQuantity,
                             DamageDate = di.DamagedDate,
                             DamageReason = di.DamagedReason

                         });

            if (ItemNAme != null)
            {
                query = query.Where(e => e.ItemName.Contains(ItemNAme));
            }

            if (StartDate is not null && EndDate is not null)
            {
                query = query.Where(e => e.DamageDate.Value.Date >= StartDate && e.DamageDate.Value.Date <= EndDate);
            }
            // DamagedItems = query.ToList();
            // TotalItems = DamagedItems.Count();

            // FromDate = DateTime.Now;
            // ToDate = DateTime.Now;
            TotalItems = query.Count();
            TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

            var list = query.ToList();
            DamagedItems = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            DamagedItemsAll = query.ToList();
            CurrentPage = page;
            base.ExtractSessionData();
            FillLables();
        }
        // public void OnPost([FromForm] string ItemName, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        // {
        //     base.ExtractSessionData();
        //     CurrentPage = 1;
        //     this.FromDate = FromDate;
        //     this.ToDate = ToDate;
        //     this.ItemName = ItemName;
        //     if (CanSeeReports)
        //     {
        //         FillData(ItemName, FromDate, ToDate, CurrentPage);
        //     }
        //     else
        //         RedirectToPage("./Index?lang=" + Lang);
        // }
        public IActionResult OnPostAction(string ItemName, DateTime? FromDate, DateTime? ToDate, string action, List<string> columns)
        {
            base.ExtractSessionData();

            if (action == "search")
            {
                this.ItemName = ItemName;
                CurrentPage = 1;
                this.FromDate = FromDate;
                this.ToDate = ToDate;
                if (CanSeeReports)
                {

                    FillData(ItemName, FromDate, ToDate, CurrentPage);

                }

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "ReturnItemReport";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {

                    string selectedColumns = string.Join(",", columns);

                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "ReturnItemReport";
                    this.ItemName = ItemName;
                    CurrentPage = 1;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    {

                        FillData(ItemName, FromDate, ToDate, CurrentPage);

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
            this.lblHazardTypeName = (Program.Translations["HazardTypeName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblAvailableQuantity = (Program.Translations["AvailableQuantity"])[Lang];
            this.lblHazardType = (Program.Translations["HazardType"])[Lang];
            this.lblTypeName = (Program.Translations["TypeName"])[Lang];
            this.lblUnitCode = (Program.Translations["UnitCode"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblUserReport = (Program.Translations["UserReport"])[Lang];
            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
            this.lblDamageditems = (Program.Translations["DamagedItems"])[Lang];
            this.lblTotalDamagedItem = (Program.Translations["TotalDamagedItem"])[Lang];
            this.lblDamageDate = (Program.Translations["DamageDate"])[Lang];
            this.lblDamageQuantity = (Program.Translations["DamageQuantity"])[Lang];
            this.lblDamageReason = (Program.Translations["DamageReason"])[Lang];
            this.lblExport = (Program.Translations["Export"])[Lang];
            this.lblPrint = (Program.Translations["Print"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];
        }
    }
}
