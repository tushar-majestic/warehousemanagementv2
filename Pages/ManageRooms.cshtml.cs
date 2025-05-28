using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System.Data;

namespace LabMaterials.Pages
{
    public class ManageRoomsModel : BasePageModel
    {
        public List<StoreDataResult> Stores { get; set; }
        public List<StoreDataResult> StoresAll { get; set; }
        public List<Room> Rooms { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }
         public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public string RoomName { get; set; }
        public string BuildingNnumber;
        public List<string> SelectedColumns { get; set; } = new List<string>();

        public void OnGet(string? RoomName, int page = 1) 
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
                LoadSelectedColumns();
                if (HttpContext.Request.Query.ContainsKey("page"))
                {
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.RoomName = RoomName;
                    FillData(RoomName, page);

                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public string lblStores, lblManageStorage, lblSearch, lblRoomName, lblRoomNumber, lblManageShelves, lblManageRooms, lblStoreNumber,
        lblAddRoom, lblAddShelf, lblStoreName, lblSubmit, lblAddStore, lblShelves, lblEdit, lblDelete, lblTotalItem, lblAddDestination,
        lblManageDestination, lblExportExcel, lblPrintTable;

        // public void OnPostSearch([FromForm] string RoomName)
        // {   CurrentPage = 1;
        //     this.RoomName = RoomName;
        //     FillData(RoomName, CurrentPage);
        // }
        private void LoadSelectedColumns()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                using (var db = new LabDBContext())
                {
                    string pageName = "manageRooms";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        // SelectedColumns = new List<string>();
                        string selectedColumns = "storeName,buildingNumber,roomNumber";
                        SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    }
                }
            }
        }
        public IActionResult OnPostAction(string RoomName, string action, List<string> columns)
        {
            base.ExtractSessionData();

            if (action == "search")
            {
                CurrentPage = 1;
                this.RoomName = RoomName;

                FillData(RoomName, CurrentPage);

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "manageRooms";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {
                    CurrentPage = 1;
                    string selectedColumns = string.Join(",", columns);
                    this.RoomName = RoomName;
                    FillData(RoomName, CurrentPage);
                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "manageRooms";

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

        public void OnPostDelete([FromForm] int RoomId)
        {   
            base.ExtractSessionData();
            var dbContext = new LabDBContext();

            // var itemsInstore = dbContext.Storages.Where(e => e.Ended == null).Count(s => s.RoomId == RoomId && s.AvailableQuantity > 0);

            var shelvesInRoom = dbContext.Shelves.Where(e => e.Ended == null).Count(s => s.RoomId == RoomId);

            bool hasItemsInRoom = dbContext.Shelves
            .Where(s => s.RoomId == RoomId && s.Ended == null)
            .Any(s => dbContext.ShelveItems.Any(si => si.ShelfId == s.ShelfId && si.QuantityAvailable > 0));

            if (!hasItemsInRoom)
            {
                var room = dbContext.Rooms.Single(s => s.RoomId == RoomId);
                room.Ended = DateTime.Now;
                dbContext.Rooms.Update(room);
                dbContext.SaveChanges();
                FillData(null);
                Message = string.Format((Program.Translations["RoomDeleted"])[Lang], room.RoomName);
                Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
            }
            else
            {
                // var itemId = dbContext.Storages.First(s => s.RoomId == RoomId && s.AvailableQuantity > 0).ItemId;
                // Message = string.Format((Program.Translations["RoomNotDeleted"])[Lang], itemsInstore,
                //     dbContext.Items.Single(i => i.ItemId == itemId).ItemName);
                 Message = string.Format((Program.Translations["RoomNotDeleted"])[Lang]);
                FillData(null);
            }
            LoadSelectedColumns();

        }


        public IActionResult OnPostEdit([FromForm] int RoomId, [FromForm] int page, [FromForm] string RoomName)
        {
            HttpContext.Session.SetInt32("RoomId", RoomId);
            HttpContext.Session.SetInt32("page", page);
            HttpContext.Session.SetString("RoomName", string.IsNullOrEmpty(RoomName) ? "" : RoomName);
            return RedirectToPage("./EditRoom");
        }

        public IActionResult OnPostManageShelves([FromForm] int RoomId, [FromForm] int StoreId)
        {
            /*var dbContext = new LabDBContext();

            var shelfDetails = (from sh in dbContext.Shelves
                                where sh.RoomId == RoomId
                                select sh).ToList();*/

            HttpContext.Session.SetInt32("RoomId", RoomId);
            HttpContext.Session.SetInt32("StoreId", StoreId);

            return RedirectToPage("./ManageShelves");
        }

       private void FillDataOld(string? StoreNumber, string? StoreName)
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



                /*if (string.IsNullOrEmpty(StoreNumber) == false)
                    query = query.Where(s => s.StoreNumber.Contains(StoreNumber));

                if (string.IsNullOrEmpty(StoreName) == false)
                    query = query.Where(s => s.StoreName.Contains(StoreName));

                Stores = query.ToList();
                foreach (var s in Stores)
                    s.Shelves = string.IsNullOrEmpty(s.ShelfNumbers) ? new string[0] : s.StoreNumber.Split(',');

                Stores = query.ToList();*/
                TotalItems = Rooms.Count(room => room.Ended == null);
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillData(string? RoomName, int page = 1)
        {
            if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }

            base.ExtractSessionData();

            if (!CanManageStore)
            {
                RedirectToPage("./Index?lang=" + Lang);
                return;
            }

            FillLables();
            int? userId = HttpContext.Session.GetInt32("UserId");
            var dbContext = new LabDBContext();

            var codeParam = new SqlParameter("@PCODE", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };
            var msgParam = new SqlParameter("@PMSG", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Output };
            var descParam = new SqlParameter("@PDESC", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };

            var allStores = dbContext.StoreDataResults
                                .FromSqlRaw("EXEC PRC_GET_STORE_DATA @PCODE OUTPUT, @PDESC OUTPUT, @PMSG OUTPUT",
                                            codeParam, descParam, msgParam)
                                .ToList();

                              

            var activeRooms = dbContext.Rooms
                                .Where(r => r.Ended == null)
                                .ToList();

            IQueryable<StoreDataResult> joinedDataQuery;

            if (HttpContext.Session.GetString("UserGroup") == "Warehouse Manager")
            {
                joinedDataQuery = (from store in allStores
                                   join room in activeRooms on store.RoomId equals room.RoomId
                                   join user in dbContext.Users on room.KeeperId equals user.UserId
                                   where store.WarehouseManagerID == userId
                                   select new StoreDataResult
                                   {
                                       StoreId = store.StoreId,
                                       StoreName = store.StoreName,
                                       ShelfNumber = store.ShelfNumber,
                                       RoomId = room.RoomId,
                                       RoomName = room.RoomName,
                                       KeeperName = user.FullName,
                                       KeeperJobNum = user.JobNumber,
                                       BuildingNumber = room.BuildingNumber,
                                       RoomNo = room.RoomNo,
                                       RoomDesc = room.RoomDesc,
                                       NoOfShelves = room.NoOfShelves,
                                       RoomStatus = room.RoomStatus
                                   }).AsQueryable();
            }
            else
            {
                joinedDataQuery = (from store in allStores
                                   join room in activeRooms on store.RoomId equals room.RoomId
                                   join user in dbContext.Users on room.KeeperId equals user.UserId
                                   select new StoreDataResult
                                   {
                                       StoreId = store.StoreId,
                                       StoreName = store.StoreName,
                                       ShelfNumber = store.ShelfNumber,
                                       RoomId = room.RoomId,
                                       RoomName = room.RoomName,
                                       KeeperName = user.FullName,
                                       KeeperJobNum = user.JobNumber,
                                       BuildingNumber = room.BuildingNumber,
                                       RoomNo = room.RoomNo,
                                       RoomDesc = room.RoomDesc,
                                       NoOfShelves = room.NoOfShelves,
                                       RoomStatus = room.RoomStatus
                                   }).AsQueryable();
            }

            if (!string.IsNullOrEmpty(RoomName))
            {
                joinedDataQuery = joinedDataQuery
                                  .Where(s => s.RoomDesc != null && s.RoomDesc.Contains(RoomName));
            }

            TotalItems = joinedDataQuery.Count();
            TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
            Stores = joinedDataQuery.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            StoresAll = joinedDataQuery.ToList();
            CurrentPage = page;
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
            this.lblRoomName = (Program.Translations["RoomName"])[Lang];
            this.lblExportExcel = (Program.Translations["ExportExcel"])[Lang];
            this.lblPrintTable = (Program.Translations["PrintTable"])[Lang];
            




        }
    }
}
