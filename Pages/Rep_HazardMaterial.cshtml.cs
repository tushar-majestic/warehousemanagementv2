using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class Rep_HazardMaterialModel : BasePageModel
    {
        public List<ItemInfo> Items { get; set; }
        public List<ItemInfo> ItemsAll { get; set; }
        public int TotalItems { get; set; }
        public List<HazardType> HazardTypes { get; set; }
        public string HazardTypeName;

        [BindProperty]
        public string ItemName { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public List<string> SelectedColumns { get; set; } = new List<string>();

        public string lblHazardousMaterials, lblHazardTypeName, lblSearch, lblSubmit, lblItemCode, lblItemName, lblGroupName, 
            lblAvailableQuantity, lblHazardType, lblTypeName, lblStoreName, lblUnitCode, lblTotalItem,
            lblMaterialsReceived, lblInventory, lblUserActivity, lblDistributedMaterials, lblDamagedItems,
            lblUserReport, lblExport, lblSelectHazardType, lblPrint, lblItems;
        public void OnGet(string? ItemName,string? HazardTypeName, int page = 1)
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                var dbContext = new LabDBContext();
                HazardTypes = dbContext.HazardTypes.Where(h => h.HazardTypeName != "NonHazarduos").ToList();
                HazardTypeName = null;
                FillLables();
                LoadSelectedColumns();
                if (HttpContext.Request.Query.ContainsKey("page")){
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.ItemName = ItemName;
                    this.HazardTypeName = HazardTypeName;
                   
                    FillData(ItemName, HazardTypeName, page);
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
                    string pageName = "HazardousMaterialReport";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        // SelectedColumns = new List<string>();
                        string selectedColumns = "itemName,itemCode,warehouseName";
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

        private void FillData(string ItemName, string HazardTypeName, int page = 1)
        {   if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }
            if (CanSeeReports)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = (from i in dbContext.Items
                             join g in dbContext.ItemGroups on i.GroupCode equals g.GroupCode
                             join t in dbContext.ItemTypes on i.ItemTypeCode equals t.ItemTypeCode
                             join u in dbContext.Units on i.UnitId equals u.Id
                             join st in dbContext.Storages on i.ItemId equals st.ItemId
                             join s in dbContext.Stores on st.StoreId equals s.StoreId

                             select new ItemInfo
                             {
                                 AvailableQuantity = i.AvailableQuantity,
                                 GroupCode = g.GroupCode,
                                 GroupDesc = g.GroupDesc,
                                 HazardTypeName = i.HazardTypeName,
                                 IsHazardous = i.IsHazardous,
                                 ItemCode = i.ItemCode,
                                 ItemId = i.ItemId,
                                 ItemName = i.ItemName,
                                 StoreName = s.StoreName,
                                 ItemTypeCode = t.ItemTypeCode,
                                 TypeName = t.TypeName,
                                 UnitCode = u.UnitCode,
                                 UnitDesc = u.UnitDesc,
                             });

                query = query.Where(i => i.HazardTypeName.ToLower() != "NonHazarduos");

                if (string.IsNullOrEmpty(ItemName) == false)
                    query = query.Where(i => i.ItemName.Contains(ItemName));

                if (string.IsNullOrEmpty(HazardTypeName) == false)
                    query = query.Where(i => i.HazardTypeName.Contains(HazardTypeName));

                // Items = query.ToList();
                // TotalItems = Items.Count();
                TotalItems = query.Count();
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

                var list = query.ToList();
                Items = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();     
                ItemsAll = query.ToList();     
                CurrentPage = page;
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        // public void OnPost([FromForm] string ItemName, [FromForm] string HazardTypeName)
        // {
        //     base.ExtractSessionData();
        //     CurrentPage = 1;
        //     this.ItemName = ItemName;
        //     var dbContext = new LabDBContext();
        //     HazardTypes = dbContext.HazardTypes.Where(h => h.HazardTypeName != "NonHazarduos").ToList();
        //     this.HazardTypeName = HazardTypeName;
        //     if (CanSeeReports)
        //     {
        //         FillData(ItemName, HazardTypeName, CurrentPage);
        //     }
        //     else
        //         RedirectToPage("./Index?lang=" + Lang);
        // }
        public IActionResult OnPostAction(string ItemName, string HazardTypeName, string action, List<string> columns)
        {
            base.ExtractSessionData();

            if (action == "search")
            {
                this.ItemName = ItemName;
                this.HazardTypeName = HazardTypeName;
                CurrentPage = 1;
                if (CanSeeReports)
                {

                    FillData(ItemName, HazardTypeName, CurrentPage);

                }

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "HazardousMaterialReport";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {

                    string selectedColumns = string.Join(",", columns);

                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "HazardousMaterialReport";
                    this.ItemName = ItemName;
                    this.HazardTypeName = HazardTypeName;
                    CurrentPage = 1;
                    {

                        FillData(ItemName, HazardTypeName, CurrentPage);

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
            this.lblExport = (Program.Translations["Export"])[Lang];
            this.lblSelectHazardType = (Program.Translations["SelectHazardType"])[Lang];
            this.lblPrint = (Program.Translations["Print"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];
        }
    }
}
