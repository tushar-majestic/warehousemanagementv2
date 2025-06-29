using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DocumentFormat.OpenXml.Spreadsheet;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LabMaterials.Pages
{
    public class Rep_ItemCardsModel : BasePageModel
    {
        public DateTime? FromDate, ToDate;

        public string lblTotalItem, lblItemGroups, lblSearch, lblItems, lblItemName,
        lblStateofMatter, lblRiskRating, lblChemical, lblSubmit, lblUnitCode, lblHazardType, lblItemCode, lblGroupName, lblStores;

        public List<string> SelectedColumns { get; set; } = new List<string>();
        public List<ItemCardViewModels> ItemCardView { get; set; } = new List<ItemCardViewModels>();
        public List<ItemCardViewModels> ItemCardViewAll { get; set; } = new List<ItemCardViewModels>();
        public int? UserId;
        [BindProperty]
        public string ItemName { get; set; }
        public string StoreName { get; set; }
        private readonly LabDBContext _context;
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }

        
        public string lblInventory, lblStoreNumber, lblStoreName, lblShelveNumber, lblAvailableQuantity, lblMaterialsReceived, lblDate, 
            lblHazardousMaterials, lblUserActivity, lblDistributedMaterials, lblExpiryDate, lblDamagedItems, lblUserReport, lblExport, lblFromDate, lblToDate, lblPrint;
        public Rep_ItemCardsModel(LabDBContext context)
        {
            _context = context;
        }
        public void OnGet(string? ItemName,string? StoreName, DateTime? FromDate, DateTime? ToDate, int page = 1)
        {
            base.ExtractSessionData();
            FillLables();
            LoadSelectedColumns();
            if (this.CanManageItems)
            {
                if (HttpContext.Request.Query.ContainsKey("page"))
                {
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.ItemName = ItemName;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    FillData(ItemName,StoreName, FromDate, ToDate, page);

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
                    string pageName = "itemsCardsReport";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        // SelectedColumns = new List<string>();
                        string selectedColumns = "itemCode,itemName";
                        SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    }
                }
            }
        }

        public IActionResult OnPostAction(string ItemName,string StoreName, DateTime? FromDate, DateTime? ToDate, string action, List<string> columns)
        {
            base.ExtractSessionData();
            FillLables();

            if (action == "search")
            {
                CurrentPage = 1;
                this.ItemName = ItemName;
                this.StoreName = StoreName;
                this.FromDate = FromDate;
                this.ToDate = ToDate;
                if (CanManageItems)
                {
                    FillData(ItemName,StoreName, FromDate, ToDate);
                    LoadSelectedColumns();
                }

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "itemsCardsReport";
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {
                    string selectedColumns = string.Join(",", columns);
                    this.ItemName = ItemName;
                    this.StoreName = StoreName;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    FillData(ItemName,StoreName, FromDate, ToDate);

                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "itemsCardsReport";
                    SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    LoadSelectedColumns();
                }
            }

            return Page();
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


        private void FillData(string ItemName,string StoreName, DateTime? StartDate, DateTime? EndDate, int page = 1)
        {
            FillLables();

            this.UserId = HttpContext.Session.GetInt32("UserId");

            var query = (from item in _context.ItemCards
                         join store in _context.Stores on item.StoreId equals store.StoreId into storeGroup
                         from store in storeGroup.DefaultIfEmpty()
                         select new ItemCardViewModels
                         {
                             GroupCode = item.GroupCode,
                             ItemCardId = item.Id,
                             ItemCode = item.ItemCode,
                             ItemName = item.ItemName,
                             ItemDescription = item.ItemDescription,
                             UnitOfMeasure = item.UnitOfmeasure,
                             Chemical = item.Chemical,
                             HazardTypeName = item.HazardTypeName,
                             QuantityAvailable = item.QuantityAvailable,
                             DateOfEntry = item.CreatedAt,
                             WarehouseName = store.StoreName
                         });
            if (!string.IsNullOrWhiteSpace(ItemName))
                query = query.Where(i => i.ItemName.Contains(ItemName));

            if (!string.IsNullOrWhiteSpace(StoreName))
                query = query.Where(i => i.WarehouseName.Contains(StoreName));

            if (StartDate is not null && EndDate is not null)
            {
                query = query.Where(e => e.DateOfEntry.Value.Date >= StartDate && e.DateOfEntry.Value.Date <= EndDate);
            }

            TotalItems = query.Count();
            TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
            var list = query.ToList();
            ItemCardView = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            ItemCardViewAll = list;
            CurrentPage = page;
        }

        private void FillLables()
        {

            this.Lang = HttpContext.Session.GetString("Lang");
            this.lblMaterialsReceived = (Program.Translations["MaterialsReceived"])[Lang];
            this.lblInventory = (Program.Translations["Inventory"])[Lang];
            this.lblHazardousMaterials = (Program.Translations["HazardousMaterials"])[Lang];
            this.lblUserActivity = (Program.Translations["UserActivity"])[Lang];
            this.lblDistributedMaterials = (Program.Translations["DistributedMaterials"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblAvailableQuantity = (Program.Translations["AvailableQuantity"])[Lang];
            this.lblExpiryDate = (Program.Translations["ExpiryDate"])[Lang];
            this.lblUserReport = (Program.Translations["UserReport"])[Lang];
            this.lblExport = (Program.Translations["Export"])[Lang];
            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
            this.lblPrint = (Program.Translations["Print"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];
            this.lblItemGroups = (Program.Translations["ItemGroups"])[Lang];
            this.lblItemCards = (Program.Translations["ItemCards"])[Lang];
            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblHazardType = (Program.Translations["HazardType"])[Lang];
            this.lblUnitCode = (Program.Translations["UnitCode"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblChemical = (Program.Translations["Chemical"])[Lang];
            this.lblRiskRating = (Program.Translations["RiskRating"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];
            this.lblStateofMatter = (Program.Translations["StateofMatter"])[Lang];
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblDate = (Program.Translations["Date"])[Lang];
        }
    }
}
