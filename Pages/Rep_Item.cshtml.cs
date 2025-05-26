using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class Rep_ItemModel : BasePageModel
    {
        public List<ItemInfo> Items { get; set; }
        public List<ItemInfo> ItemsAll { get; set; }
        public int TotalItems { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public string Group { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public List<string> SelectedColumns { get; set; } = new List<string>();
        [BindProperty(SupportsGet = true)]
        public string? TypeName { get; set; }

        public string lblInventory, lblSearch, lblStoreNumber, lblStoreName, lblItemName, lblSubmit,
            lblShelveNumber, lblAvailableQuantity, lblTotalItem, lblMaterialsReceived,
            lblHazardousMaterials, lblUserActivity, lblDistributedMaterials, lblExpiryDate,
            lblDamagedItems, lblUserReport, lblExport, lblPrint, lblItemslblItems, lblGroupName, lblItemCode, lblQuantity, lblHazardType, lblTypeName,
            lblUnitCode, lblBatchNo, lblDamage, lblItems, lblChemical, lblRiskRating, lblStateofMatter;

        public void OnGet(string? ItemName, string? Group, int page = 1)
        {
            base.ExtractSessionData();
            if (this.CanManageItems)
            {
                FillLables();
                LoadSelectedColumns();
                if (HttpContext.Request.Query.ContainsKey("page"))
                {
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.ItemName = ItemName;
                    this.Group = Group;
                    FillData(ItemName, Group, page);

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
                    string pageName = "ItemReport";
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

        public IActionResult OnPostAction(string ItemName, string Group, string action, List<string> columns)
        {
            base.ExtractSessionData();

            if (action == "search")
            {
                CurrentPage = 1;
                this.ItemName = ItemName;
                this.Group = Group;
                if (CanManageItems)
                {
                    FillData(ItemName, Group);
                }

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "ItemReport";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {
                    string selectedColumns = string.Join(",", columns);
                    this.ItemName = ItemName;
                    this.Group = Group;
                    FillData(ItemName, Group);

                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "ItemReport";

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

        private void FillData(string ItemName, string Group, int page = 1)
        {
            if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }

            FillLables();
            var dbContext = new LabDBContext();

            var disbursements = dbContext.DisbursementRequests.ToList();

            var query = (from i in dbContext.Items
                         join g in dbContext.ItemGroups on i.GroupCode equals g.GroupCode
                         join t in dbContext.ItemTypes on i.ItemTypeCode equals t.ItemTypeCode
                         join u in dbContext.Units on i.UnitId equals u.Id
                         where i.Ended == null
                         select new ItemInfo
                         {
                             ItemId = i.ItemId,
                             ItemCode = i.ItemCode,
                             ItemName = i.ItemName,
                             ItemNameAr = i.ItemNameAr ?? "",
                             IsHazardous = i.IsHazardous,
                             Chemical = i.Chemical,
                             HazardTypeName = i.HazardTypeName ?? "",
                             RiskRating = i.RiskRating ?? "",
                             GroupCode = g.GroupCode,
                             GroupDesc = g.GroupDesc,
                             ItemTypeCode = t.ItemTypeCode,
                             TypeName = t.TypeName ?? "",
                             StateofMatter = i.StateofMatter ?? "",
                             UnitId = i.UnitId,
                             UnitCode = u.UnitCode ?? "",
                             UnitDesc = u.UnitDesc ?? "",
                             AvailableQuantity = i.AvailableQuantity,
                             ItemDescription = i.ItemDescription ?? "",
                             BatchNo = i.BatchNo ?? "",
                             ExpiryDate = i.ExpiryDate,
                             Ended = Convert.ToString(i.Ended),
                         });

            // Apply filters
            if (!string.IsNullOrEmpty(ItemName))
                query = query.Where(i => i.ItemName.Contains(ItemName));

            if (!string.IsNullOrEmpty(Group))
                query = query.Where(i => i.GroupDesc.Contains(Group));

            // Pagination
            TotalItems = query.Count();
            TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
            var list = query.ToList();
            Items = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            ItemsAll = list;
            CurrentPage = page;
        }

        private void FillLables()
        {

            this.lblMaterialsReceived = (Program.Translations["MaterialsReceived"])[Lang];
            this.lblInventory = (Program.Translations["Inventory"])[Lang];
            this.lblHazardousMaterials = (Program.Translations["HazardousMaterials"])[Lang];
            this.lblUserActivity = (Program.Translations["UserActivity"])[Lang];
            this.lblDistributedMaterials = (Program.Translations["DistributedMaterials"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblStoreNumber = (Program.Translations["StoreNumber"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblShelveNumber = (Program.Translations["ShelveNumber"])[Lang];
            this.lblAvailableQuantity = (Program.Translations["AvailableQuantity"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblExpiryDate = (Program.Translations["ExpiryDate"])[Lang];
            this.lblUserReport = (Program.Translations["UserReport"])[Lang];
            this.lblExport = (Program.Translations["Export"])[Lang];
            this.lblPrint = (Program.Translations["Print"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];
            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblHazardType = (Program.Translations["HazardType"])[Lang];
            this.lblTypeName = (Program.Translations["TypeName"])[Lang];
            this.lblUnitCode = (Program.Translations["UnitCode"])[Lang];
            this.lblBatchNo = (Program.Translations["BatchNo"])[Lang];
            this.lblChemical = (Program.Translations["Chemical"])[Lang];
            this.lblRiskRating = (Program.Translations["RiskRating"])[Lang];
            this.lblStateofMatter = (Program.Translations["StateofMatter"])[Lang];
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
        }
    }
}
