using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;

namespace LabMaterials.Pages
{
    public class ManageDamagedModel : BasePageModel
    {
        public List<ItemInfo> Items { get; set; }
        public List<ItemInfo> ItemsAll { get; set; }
        public int TotalItems { get; set; }
        public string Message { get; set; }
        [BindProperty]
        public string ItemName { get; set; }
        public List<string> UniqueItemNames { get; set; }
        public List<string> UniqueTypeNames { get; set; }
        public List<string> UniqueDamageReasons { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? TypeName { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? DamageReason { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public List<string> SelectedColumns { get; set; } = new List<string>();
        public string lblItems, lblItemName, lblGroupName, lblItemCode, lblAvailableQuantity, lblHazardType, lblTypeName,
            lblUnitCode, lblSearch, lblSubmit, lblDamageReason, lblDamagedQuantity, lblManageItemGroup, lblManageUnit,
            lblAddItem, lblEdit, lblDelete, lblTotalItem, lblExpiryDate, lblBatchNo, lblDamage, lblDamagedItems, lblExportExcel,
            lblPrintTable;
        public void OnGet(string? ItemName, string? Group, int page = 1)
        {
            base.ExtractSessionData();
            if (CanManageItems)
            {
                FillLables();
                LoadSelectedColumns();
                if (HttpContext.Request.Query.ContainsKey("page"))
                {
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.ItemName = ItemName;
                    FillData(ItemName, Group, page, TypeName, DamageReason);

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
                    string pageName = "manageDamaged";
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

        // public void OnPost([FromForm] string ItemName, [FromForm] string Group)
        // {
        //     CurrentPage = 1;
        //     base.ExtractSessionData();
        //     this.ItemName = ItemName;
        //     if (CanManageItems)
        //     {
        //         FillData(ItemName, Group, CurrentPage);
        //     }
        //     else
        //         RedirectToPage("./Index?lang=" + Lang);
        // }

        public IActionResult OnPostAction(string ItemName, string Group, string action, List<string> columns)
        {
            base.ExtractSessionData();

            if (action == "search")
            {
                CurrentPage = 1;
                this.ItemName = ItemName;

                FillData(ItemName, Group, CurrentPage);

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "manageDamaged";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {

                    string selectedColumns = string.Join(",", columns);

                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "manageDamaged";
                    this.ItemName = ItemName;
                    FillData(ItemName, Group, CurrentPage);
                    SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    LoadSelectedColumns();
                }

                // After updating, redirect back to ManageStore with the StoreNumber and StoreName
                // return RedirectToPage("/ManageDamaged", new { ItemName = ItemName, Group = Group });
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

        // function before pagination 
        /*private void FillData(string ItemName, string Group)
        {
            FillLables();
            var dbContext = new LabDBContext();
            var query = (from i in dbContext.Items
                         join g in dbContext.ItemGroups on i.GroupCode equals g.GroupCode
                         join t in dbContext.ItemTypes on i.ItemTypeCode equals t.ItemTypeCode
                         join u in dbContext.Units on i.UnitId equals u.Id
                         join d in dbContext.DamagedItems on i.ItemId equals d.ItemId

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
                             ItemTypeCode = t.ItemTypeCode,
                             TypeName = t.TypeName,
                             UnitCode = u.UnitCode,
                             UnitDesc = u.UnitDesc,
                             BatchNo = i.BatchNo,
                             ExpiryDate = i.ExpiryDate,
                             DamagedQuantity = (int)d.DamagedQuantity,
                             DamageReason = d.DamagedReason,
                         });

            if (string.IsNullOrEmpty(ItemName) == false)
                query = query.Where(i => i.ItemName.Contains(ItemName));

            if (string.IsNullOrEmpty(Group) == false)
                query = query.Where(i => i.GroupDesc.Contains(Group));

            Items = query.ToList();
            TotalItems = Items.Count();
        }*/

        private void FillData(string ItemName, string Group, int page = 1, string? typeName = null, string? damageReason = null)
        {
            FillLables();
            var dbContext = new LabDBContext();

            var query = (from i in dbContext.Items
                         join g in dbContext.ItemGroups on i.GroupCode equals g.GroupCode
                         join t in dbContext.ItemTypes on i.ItemTypeCode equals t.ItemTypeCode
                         join u in dbContext.Units on i.UnitId equals u.Id
                         join d in dbContext.DamagedItems on i.ItemId equals d.ItemId
                         //join r in dbContext.ReturnRequestItems on i.ItemCode equals r.ItemCode
                         select new ItemInfo
                         {
                             AvailableQuantity = i.AvailableQuantity,
                             GroupCode = g.GroupCode,
                             GroupDesc = g.GroupDesc,
                             HazardTypeName = i.HazardTypeName ?? "",
                             IsHazardous = i.IsHazardous,
                             ItemCode = i.ItemCode,
                             ItemId = i.ItemId,
                             ItemName = i.ItemName,
                             ItemTypeCode = t.ItemTypeCode,
                             TypeName = t.TypeName ?? "",
                             UnitCode = u.UnitCode ?? "",
                             UnitDesc = u.UnitDesc ?? "",
                             BatchNo = i.BatchNo ?? "",
                             ExpiryDate = i.ExpiryDate,
                             DamagedQuantity = d.DamagedQuantity ?? 0,
                             DamageReason = d.DamagedReason ?? "",
                         });

            if (!string.IsNullOrEmpty(ItemName))
                query = query.Where(i => i.ItemName.Contains(ItemName));

            if (!string.IsNullOrEmpty(Group))
                query = query.Where(i => i.GroupDesc.Contains(Group));

            if (!string.IsNullOrEmpty(typeName))
                query = query.Where(i => i.TypeName == typeName);

            if (!string.IsNullOrEmpty(damageReason))
                query = query.Where(i => i.DamageReason == damageReason);

            TotalItems = query.Count();
            TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

            var allItemsQuery = (from i in dbContext.Items
                                 join g in dbContext.ItemGroups on i.GroupCode equals g.GroupCode
                                 join t in dbContext.ItemTypes on i.ItemTypeCode equals t.ItemTypeCode
                                 join u in dbContext.Units on i.UnitId equals u.Id
                                 join d in dbContext.DamagedItems on i.ItemId equals d.ItemId
                                 select new ItemInfo
                                 {
                                     TypeName = t.TypeName ?? "",
                                     DamageReason = d.DamagedReason ?? ""
                                 });

            UniqueTypeNames = allItemsQuery.Select(i => i.TypeName).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            UniqueDamageReasons = allItemsQuery.Select(i => i.DamageReason).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

            var list = query.ToList();

            Items = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            ItemsAll = list;
            CurrentPage = page;
        }

        public IActionResult OnPostEdit([FromForm] int ItemId)
        {
            HttpContext.Session.SetInt32("ItemId", ItemId);

            return RedirectToPage("./EditItem");
        }

        public IActionResult OnPostDamage([FromForm] int ItemId)
        {
            HttpContext.Session.SetInt32("ItemId", ItemId);

            return RedirectToPage("./DamageItem");
        }

        public void OnPostDelete([FromForm] int ItemId)
        {
            base.ExtractSessionData();
            if (CanManageItems)
            {
                var dbContext = new LabDBContext();

                if (dbContext.Supplies.Count(s => s.ItemId == ItemId) == 0)
                {
                    var Item = dbContext.Items.Single(s => s.ItemId == ItemId);
                    dbContext.Items.Remove(Item);
                    dbContext.SaveChanges();
                    FillData(null, null);
                    Message = string.Format((Program.Translations["ItemDeleted"])[Lang], Item.ItemName);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
                }
                else
                {
                    Message = (Program.Translations["ItemNotDeleted"])[Lang];
                    FillData(null, null);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillLables()
        {


            this.lblItems = (Program.Translations["Items"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblAvailableQuantity = (Program.Translations["AvailableQuantity"])[Lang];
            this.lblHazardType = (Program.Translations["HazardType"])[Lang];
            this.lblTypeName = (Program.Translations["TypeName"])[Lang];
            this.lblUnitCode = (Program.Translations["UnitCode"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblManageItemGroup = (Program.Translations["ManageItemGroups"])[Lang];
            this.lblManageUnit = (Program.Translations["ManageUnits"])[Lang];
            this.lblAddItem = (Program.Translations["AddItem"])[Lang];
            this.lblExpiryDate = (Program.Translations["ExpiryDate"])[Lang];
            this.lblBatchNo = (Program.Translations["BatchNo"])[Lang];
            this.lblDamage = (Program.Translations["Damage"])[Lang];
            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblDamagedQuantity = (Program.Translations["DamagedQuantity"])[Lang];
            this.lblDamageReason = (Program.Translations["DamageReason"])[Lang];
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblExportExcel = (Program.Translations["ExportExcel"])[Lang];
            this.lblPrintTable = (Program.Translations["PrintTable"])[Lang];



        }
    }
}
