using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace LabMaterials.Pages
{
    public class Rep_InventoryModel : BasePageModel
    {
        public List<StorageInfo> Storages { get; set; }
        public List<StorageInfo> StoragesAll { get; set; }
        public DateTime? FromDate, ToDate;
        public int TotalItems { get; set; }

        [BindProperty]
        public string StoreNumber { get; set; }

        [BindProperty]
        public string StoreName { get; set; }

        [BindProperty]
        public string Item { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public List<string> SelectedColumns { get; set; } = new List<string>();

        public string lblInventory, lblSearch, lblStoreNumber, lblStoreName, lblItemName, lblSubmit,
            lblShelveNumber, lblAvailableQuantity, lblTotalItem, lblMaterialsReceived,
            lblHazardousMaterials, lblUserActivity, lblDistributedMaterials, lblExpiryDate,
            lblDamagedItems, lblUserReport, lblExport, lblFromDate, lblToDate, lblPrint, lblItems;
        public void OnGet(string? Item,string? StoreNumber, string? StoreName, DateTime? FromDate, DateTime? ToDate, int page = 1)
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
                    this.Item = Item;
                    this.StoreNumber = StoreNumber;
                    this.StoreName = StoreName;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    // this.FromDate = DateTime.Today;
                    // this.ToDate = DateTime.Today;
                    FillData(StoreNumber, StoreName, Item,FromDate,ToDate, page);
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
                    string pageName = "InventoryReport";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        // SelectedColumns = new List<string>();
                        string selectedColumns = "itemName,warehouseName";
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

        private void FillData(string? StoreNumber, string? StoreName, string? Item, DateTime? FromDate, DateTime? ToDate,  int page = 1)
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
                var query = from ic in dbContext.ItemCards
                            join i in dbContext.Items on ic.ItemId equals i.ItemId
                            join s in dbContext.Stores on ic.StoreId equals s.StoreId
                            select new StorageInfo
                            {
                                StoreName = s.StoreName,
                                ItemName = i.ItemName,
                                AvailableQuantity = ic.QuantityAvailable.ToString(),
                                StoreNumber = s.StoreNumber,
                                ExpiryDate = i.ExpiryDate,
                            };
                // var query = from st in dbContext.Storages
                //             join i in dbContext.Items on st.ItemId equals i.ItemId
                //             join s in dbContext.Stores on st.StoreId equals s.StoreId
                //             select new StorageInfo
                //             {
                //                 StoreName = s.StoreName,
                //                 ItemName = i.ItemName,
                //                 ShelfNumber = st.ShelfNumber,
                //                 AvailableQuantity = st.AvailableQuantity.ToString() + " " + i.Unit.UnitCode,
                //                 StoreNumber = s.StoreNumber,
                //                 StorageId = st.StorageId,
                //                 ExpiryDate = i.ExpiryDate,
                //             };

                if (string.IsNullOrEmpty(StoreNumber) == false)
                    query = query.Where(s => s.StoreNumber.Contains(StoreNumber));

                if (string.IsNullOrEmpty(StoreName) == false)
                    query = query.Where(s => s.StoreName.Contains(StoreName));
                if (string.IsNullOrEmpty(Item) == false)
                    query = query.Where(s => s.ItemName.Contains(Item));

                if (FromDate is not null && FromDate != DateTime.MinValue && ToDate is not null && ToDate != DateTime.MinValue)
                    query = query.Where(e => e.ExpiryDate >= FromDate && e.ExpiryDate <= ToDate);

                // Storages = query.ToList();

                // Storages = query.ToList();
                // TotalItems = Storages.Count();

                // this.FromDate = DateTime.Today;
                // this.ToDate = DateTime.Today;
                TotalItems = query.Count();
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

                var list = query.ToList();
                Storages = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();     
                StoragesAll = query.ToList();     
                CurrentPage = page;
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void OnPostSearch([FromForm] string StoreNumber, [FromForm] string StoreName, [FromForm] string Item, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        {   CurrentPage = 1;
            this.Item = Item;
            this.StoreNumber = StoreNumber;
            this.StoreName = StoreName;
            this.FromDate = FromDate;
            this.ToDate = ToDate;
            // this.FromDate = DateTime.Today;
            // this.ToDate = DateTime.Today;
            FillData(StoreNumber, StoreName, Item,FromDate,ToDate, CurrentPage);
        }

        public IActionResult OnPostAction(string StoreNumber, string StoreName, string Item, DateTime? FromDate, DateTime? ToDate, string action, List<string> columns)
        {
            base.ExtractSessionData();

            if (action == "search")
            {
                this.Item = Item;
                this.StoreNumber = StoreNumber;
                this.StoreName = StoreName;
                CurrentPage = 1;
                this.FromDate = FromDate;
                this.ToDate = ToDate;
                if (CanSeeReports)
                {

                    FillData(StoreNumber, StoreName, Item, FromDate, ToDate, CurrentPage);

                }

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "InventoryReport";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {

                    string selectedColumns = string.Join(",", columns);

                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "InventoryReport";
                    this.Item = Item;
                    this.StoreNumber = StoreNumber;
                    this.StoreName = StoreName;
                    CurrentPage = 1;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    {

                        FillData(StoreNumber, StoreName, Item, FromDate, ToDate, CurrentPage);

                    }
                    SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    LoadSelectedColumns();
                }

            }

            return Page();
        }
        public IActionResult OnPostExport()
        {
            MemoryStream stream = new MemoryStream();
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                FillData(null, null, null, null, null);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("MaterialsRecieved");

                    // Add column headers
                    worksheet.Cells[1, 1].Value = "ITEM NAME";
                    worksheet.Cells[1, 2].Value = "STORE NUMBER";
                    worksheet.Cells[1, 3].Value = "STORE NAME";
                    worksheet.Cells[1, 4].Value = "SHELVE NUMBER";
                    worksheet.Cells[1, 5].Value = "AVAILABLE QUANTITY";
                    worksheet.Cells[1, 6].Value = "EXPIRY DATE";

                    int row = 2;
                    foreach (var item in Storages)
                    {
                        worksheet.Cells[row, 1].Value = item.ItemName;
                        worksheet.Cells[row, 2].Value = item.StoreNumber;
                        worksheet.Cells[row, 3].Value = item.StoreName;
                        worksheet.Cells[row, 4].Value = item.ShelfNumber;
                        worksheet.Cells[row, 5].Value = item.AvailableQuantity;
                        worksheet.Cells[row, 6].Style.Numberformat.Format = "yyyy-MM-dd";
                        worksheet.Cells[row, 6].Value = item.ExpiryDate;

                        row++;
                    }
                    worksheet.Cells.AutoFitColumns();
                    // Save the Excel package to a memory stream
                    package.SaveAs(stream);

                    // Set the position of the memory stream to 0
                    stream.Position = 0;

                    // Return the Excel file as a FileResult
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Inventory Report.xlsx");

                    /*
                    // Convert the MemoryStream to a byte array
                    byte[] byteArray = stream.ToArray();

                    // Convert the byte array to a Base64 string
                    string base64String = Convert.ToBase64String(byteArray);

                    return Content(base64String);
                    */
                }

            }
            else
                RedirectToPage("./Index?lang=" + Lang);

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Items.xlsx");
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
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblUserReport = (Program.Translations["UserReport"])[Lang];
            this.lblExport = (Program.Translations["Export"])[Lang];
            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
            this.lblPrint = (Program.Translations["Print"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];
        }
    }
}
