using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace LabMaterials.Pages
{
    public class ManageStoreModel : BasePageModel
    {
        public List<StoreDataResult> Stores { get; set; }
        public List<StoreDataResult> StoresAll { get; set; }
        public Store store { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }

        [BindProperty]
        public string StoreName { get; set; }

        [BindProperty]
        public string StoreNumber { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public List<string> SelectedColumns { get; set; } = new List<string>();
        public List<Room> Rooms { get; set; }

        public string lblStores, lblManageStorage, lblSearch, lblRoomName, lblManageShelves, lblUnlock, lblLock, lblRoomNumber,
        lblManageRooms, lblStoreNumber, lblAddRoom, lblAddShelf, lblStoreName, lblSubmit, lblAddStore, lblShelves, lblEdit,
        lblDelete, lblTotalItem, lblAddDestination, lblManageDestination, lblWarehouseType, lblManagerName, lblManagerJobNumber,
        lblStatus, lblExportExcel, lblPrintTable;

        private void LoadSelectedColumns()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                using (var db = new LabDBContext())
                {
                    string pageName = "manageStore";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        // SelectedColumns = new List<string>();
                        string selectedColumns = "storeNumber,warehouseType,storeName";
                        SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    }
                }
            }
        }

        public void OnGet(string? StoreNumber, string? StoreName, int page = 1)
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
                LoadSelectedColumns(); // 👉 Load columns here

                if (HttpContext.Request.Query.ContainsKey("page"))
                {
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.StoreNumber = StoreNumber;
                    this.StoreName = StoreName;

                    FillData(StoreNumber, StoreName, page);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        // public void OnPostSearch([FromForm] string StoreNumber, [FromForm] string StoreName)
        // {   CurrentPage = 1;
        //     this.StoreNumber = StoreNumber;
        //     this.StoreName = StoreName;

        //     FillData(StoreNumber, StoreName, CurrentPage);
        // }

        public IActionResult OnPostAction(string StoreNumber, string StoreName, string action, List<string> columns)
        {
            base.ExtractSessionData();

            if (action == "search")
            {
                CurrentPage = 1;
                this.StoreNumber = StoreNumber;
                this.StoreName = StoreName;

                FillData(StoreNumber, StoreName, CurrentPage);

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "manageStore";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {
                    CurrentPage = 1;
                    string selectedColumns = string.Join(",", columns);
                    this.StoreNumber = StoreNumber;
                    this.StoreName = StoreName;
                    FillData(StoreNumber, StoreName, CurrentPage);
                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "manageStore";

                    SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    LoadSelectedColumns();
                }

                // After updating, redirect back to ManageStore with the StoreNumber and StoreName
                // return RedirectToPage("/ManageStore", new { StoreNumber = StoreNumber, StoreName = StoreName });
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
                    // Update existing
                    existingRecord.DisplayColumns = selectedColumns;
                }
                else
                {
                    // Create new
                    var newRecord = new Tablecolumn()
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

        public void OnPostDelete([FromForm] int StoreId)
        {
            var dbContext = new LabDBContext();

            var itemsInstore = dbContext.Storages.Count(s => s.StoreId == StoreId && s.AvailableQuantity > 0);
            if (itemsInstore == 0)
            {
                var store = dbContext.Stores.Single(s => s.StoreId == StoreId);
                store.Ended = DateTime.Now;
                dbContext.Stores.Update(store);
                dbContext.SaveChanges();
                FillData(null, null);
                LoadSelectedColumns();
                Message = string.Format((Program.Translations["StoreDeleted"])[Lang], store.StoreName);
                Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
            }
            else
            {
                var itemId = dbContext.Storages.First(s => s.StoreId == StoreId && s.AvailableQuantity > 0).ItemId;
                Message = string.Format((Program.Translations["StoreNotDeleted"])[Lang], itemsInstore,
                    dbContext.Items.Single(i => i.ItemId == itemId).ItemName);
                FillData(null, null);
                LoadSelectedColumns();
            }

        }

        public async Task<IActionResult> OnPostAsync(int? StoreId, string lockToggle)
        {

            base.ExtractSessionData();
            FillLables();
            Console.WriteLine(StoreId);
            Console.WriteLine(lockToggle);
            if (StoreId == null)
            {
                return NotFound();
            }
            var dbContext = new LabDBContext();
            store = await dbContext.Stores.FindAsync(StoreId);

            if (store == null)
            {
                return NotFound();
            }

            if (lockToggle == "toggle")
            {
                store.IsActive = store.IsActive == 0 ? 1 : 0;
                Message = store.IsActive == 1 ? string.Format((Program.Translations["StoreUnlock"])[Lang], store.StoreName) :
                                string.Format((Program.Translations["StoreLocked"])[Lang], store.StoreName);

            }

            await dbContext.SaveChangesAsync();
            TempData["Message"] = Message;

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostLock(int? storeId, string lockToggle, int? page)
        {
            if (storeId == null)
            {
                return NotFound();
            }
            var dbContext = new LabDBContext();
            store = await dbContext.Stores.FindAsync(storeId);

            if (store == null)
            {
                return NotFound();
            }

            if (lockToggle == "toggle")
            {
                //store.IsActive = store.IsActive == 0 ? 1 : 0;
                //Message = store.IsActive == 1 ? string.Format((Program.Translations["StoreUnlock"])[Lang], store.StoreName) :
                //                string.Format((Program.Translations["StoreLocked"])[Lang], store.StoreName);

            }


            //await dbContext.SaveChangesAsync();
            //TempData["Message"] = Message;
            this.StoreNumber = StoreNumber;
            this.StoreName = StoreName;
            //return this.FillData(StoreNumber, StoreName,);
            return this.OnPostAction(StoreNumber, StoreName, "search", ["storeNumber", "warehouseType", "storeName"]);

            //return RedirectToPage();
        }



        public IActionResult OnPostEdit([FromForm] int StoreId, [FromForm] int page)
        {
            HttpContext.Session.SetInt32("StoreId", StoreId);
            HttpContext.Session.SetInt32("page", page);

            return RedirectToPage("./EditStore");
        }

        // Function without pagination 
        /*private void FillData(string? StoreNumber, string? StoreName)
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {

                FillLables();
                var dbContext = new LabDBContext();

                var codeParam = new SqlParameter("@PCODE", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };
                var msgParam = new SqlParameter("@PMSG", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Output };
                var descParam = new SqlParameter("@PDESC", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };

                
                Stores = dbContext.StoreDataResults
                                .FromSqlRaw("EXEC PRC_GET_STORE_DATA @PCODE OUTPUT, @PDESC OUTPUT, @PMSG OUTPUT",
                                            codeParam, descParam, msgParam)
                                .ToList();
                Rooms = dbContext.Rooms.ToList();



                var code = (string)codeParam.Value;
                var message = (string)msgParam.Value;
                var description = (string)descParam.Value;

                var distinctStores = Stores.GroupBy(s => new { s.StoreId, s.StoreName })
                           .Select(g => g.First())
                           .ToList();

                if (string.IsNullOrEmpty(StoreNumber) == false)
                    Stores = Stores.Where(s => s.StoreNumber.Contains(StoreNumber)).ToList();

                if (string.IsNullOrEmpty(StoreName) == false)
                    Stores = Stores.Where(s => s.StoreName.ToLower().Contains(StoreName.ToLower())).ToList();

                if (string.IsNullOrEmpty(StoreNumber) == false && string.IsNullOrEmpty(StoreName) == false)
                    Stores = Stores.Where(s => s.StoreNumber.Contains(StoreNumber) && s.StoreName.ToLower().Contains(StoreName.ToLower())).ToList();

                TotalItems = distinctStores.Count();

           
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }*/

        private void FillData(string? StoreNumber, string? StoreName, int page = 1)
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {

                FillLables();
                var dbContext = new LabDBContext();

                var codeParam = new SqlParameter("@PCODE", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };
                var msgParam = new SqlParameter("@PMSG", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Output };
                var descParam = new SqlParameter("@PDESC", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };


                var query = dbContext.StoreDataResults
                                .FromSqlRaw("EXEC PRC_GET_STORE_DATA @PCODE OUTPUT, @PDESC OUTPUT, @PMSG OUTPUT",
                                            codeParam, descParam, msgParam)
                                .ToList();
                Rooms = dbContext.Rooms.ToList();


                var code = (string)codeParam.Value;
                var message = (string)msgParam.Value;
                var description = (string)descParam.Value;
                TotalItems = query.Count();

                var distinctStores = query.GroupBy(s => new { s.StoreId, s.StoreName })
                           .Select(g => g.First())
                           .ToList();

                if (string.IsNullOrEmpty(StoreNumber) == false)
                    query = query.Where(s => s.StoreNumber.Contains(StoreNumber)).ToList();

                if (string.IsNullOrEmpty(StoreName) == false)
                    query = query.Where(s => s.StoreName.ToLower().Contains(StoreName.ToLower())).ToList();

                if (string.IsNullOrEmpty(StoreNumber) == false && string.IsNullOrEmpty(StoreName) == false)
                    query = query.Where(s => s.StoreNumber.Contains(StoreNumber) && s.StoreName.ToLower().Contains(StoreName.ToLower())).ToList();

                TotalItems = query.Count();
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
                var list = query.ToList();

                Stores = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
                StoresAll = query.ToList();


                CurrentPage = page;

                /*if (string.IsNullOrEmpty(StoreNumber) == false)
                    query = query.Where(s => s.StoreNumber.Contains(StoreNumber));

                if (string.IsNullOrEmpty(StoreName) == false)
                    query = query.Where(s => s.StoreName.Contains(StoreName));

                Stores = query.ToList();
                foreach (var s in Stores)
                    s.Shelves = string.IsNullOrEmpty(s.ShelfNumbers) ? new string[0] : s.StoreNumber.Split(',');

                Stores = query.ToList();*/
                /*TotalItems = Stores.Count();*/
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }


        private void FillLables()
        {


            this.lblStores = (Program.Translations["Stores"])[Lang];
            this.lblManageStorage = (Program.Translations["ManageStorage"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblStoreNumber = (Program.Translations["StoreNumber"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblAddStore = (Program.Translations["AddStore"])[Lang];
            this.lblShelves = (Program.Translations["Shelve"])[Lang];
            this.lblManageDestination = (Program.Translations["ManageDestinations"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblAddRoom = (Program.Translations["AddRoom"])[Lang];
            this.lblAddShelf = (Program.Translations["AddShelf"])[Lang];
            this.lblRoomNumber = (Program.Translations["RoomNumber"])[Lang];
            this.lblManageRooms = (Program.Translations["ManageRooms"])[Lang];
            this.lblManageShelves = (Program.Translations["ManageShelves"])[Lang];
            this.lblLock = (Program.Translations["Lock"])[Lang];
            this.lblRoomName = (Program.Translations["RoomName"])[Lang];
            this.lblUnlock = (Program.Translations["Unlock"])[Lang];
            this.lblWarehouseType = (Program.Translations["WarehouseType"])[Lang];
            this.lblManagerName = (Program.Translations["ManagerName"])[Lang];
            this.lblManagerJobNumber = (Program.Translations["ManagerJobNumber"])[Lang];
            this.lblStatus = (Program.Translations["WarehouseStatus"])[Lang];
            this.lblExportExcel = (Program.Translations["ExportExcel"])[Lang];
            this.lblPrintTable = (Program.Translations["PrintTable"])[Lang];



        }
    }
}