using System.Data;
using LabMaterials.DB;
using LabMaterials.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class EditRoomModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string RoomNumber, RoomName, RoomNameSearch;
         public string StoreNumber, StoreName,KeeperName, StoreType, ManagerName, BuildingNumber, RoomDesc;
        public int? RoomId;

        public int? StoreId;
        public int? KeeperJobNum, NoOfShelves, KeeperId ;
        public int? WarehouseManagerID { get; set; }
        public string WarehouseManagerName { get; set; }
        public string Status { get; set; }
        // public List<Store> Stores { get; set; }
        public List<StoreDataResult> Stores { get; set; }
        public List<User> KeeperGroupsList {  get; set; }

        public int page { get; set; }
        public string lblUpdateStore, lblStoreNumber, lblStoreName, lblUpdateRoom, lblShelves, lblUpdate, lblCancel, lblRoomNumber, 
        lblRoomName, lblStores, lblManageRooms;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            this.page = (int)HttpContext.Session.GetInt32("page");
            this.RoomNameSearch = HttpContext.Session.GetString("RoomName");
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            else
            {
                var dbContext = new LabDBContext();
                var room = dbContext.Rooms.Single(s => s.RoomId == HttpContext.Session.GetInt32("RoomId"));

                RoomNumber = room.RoomNo;
                RoomName = room.RoomName;
                RoomId = room.RoomId;
                StoreId = room.StoreId;
                BuildingNumber = room.BuildingNumber;
                RoomDesc = room.RoomDesc;
                Status = room.RoomStatus;
                NoOfShelves = room.NoOfShelves;
                
                // KeeperId = room.KeeperID;

                var st = dbContext.Stores
                    .Where(s => s.StoreId == room.StoreId)
                    .FirstOrDefault();

                if (st != null)
                {
                     WarehouseManagerID = st.WarehouseManagerId;
                    StoreType = st.StoreType;
                    var keeper = dbContext.Users
                        .Where(u => u.UserId == KeeperId)
                        .FirstOrDefault();

                    if (keeper != null)
                        KeeperName = keeper.FullName;
                    else
                        KeeperName = string.Empty; 

                    var manager = dbContext.Users
                        .Where(u => u.UserId == WarehouseManagerID)
                        .FirstOrDefault();
                    
                    if (manager != null)
                        ManagerName = manager.FullName;
                    else
                        ManagerName = string.Empty; 
                }
                
                // Stores = dbContext.Stores.ToList();
                
                var codeParam = new SqlParameter("@PCODE", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };
                var msgParam = new SqlParameter("@PMSG", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Output };
                var descParam = new SqlParameter("@PDESC", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };
          
            var query =  dbContext.StoreDataResults
                                .FromSqlRaw("EXEC PRC_GET_STORE_DATA @PCODE OUTPUT, @PDESC OUTPUT, @PMSG OUTPUT",
                                            codeParam, descParam, msgParam)
                                .ToList();
            Stores = query.GroupBy(s => new { s.StoreId, s.StoreName })
                           .Select(g => g.First())
                           .ToList();

            var KeeperGroupId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Warehouse Keeper")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            KeeperGroupsList = dbContext.Users
                .Where(u => u.UserGroupId == KeeperGroupId)
                .ToList();
            }
        }

        public IActionResult OnPost([FromForm] int RoomId, [FromForm] string RoomNumber, [FromForm] string RoomName, int? StoreId, [FromForm] string StoreType, [FromForm] string ManagerName, [FromForm] string BuildingNumber, [FromForm] string RoomDesc, [FromForm] int NoOfShelves, [FromForm] int? KeeperJobNum, [FromForm] int? KeeperId,  [FromForm] string Status, [FromForm] string KeeperName)
        {
            LogableTask task = LogableTask.NewTask("EditRoom");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageStore)
                {
                    FillLables();
                    // this.RoomName = RoomName;
                    this.RoomNumber = RoomNumber;
                     this.StoreType = StoreType;
                    this.ManagerName = ManagerName;
                    this.BuildingNumber = BuildingNumber;
                    this.RoomDesc = RoomDesc;
                    this.NoOfShelves = NoOfShelves;
                    this.StoreId = StoreId;

                    this.KeeperId = KeeperId;
                    this.KeeperName = KeeperName;
                    this.Status = Status;

                    var codeParam = new SqlParameter("@PCODE", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };
                    var msgParam = new SqlParameter("@PMSG", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Output };
                    var descParam = new SqlParameter("@PDESC", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };
                    var dbContext = new LabDBContext();

                    var query =  dbContext.StoreDataResults
                                        .FromSqlRaw("EXEC PRC_GET_STORE_DATA @PCODE OUTPUT, @PDESC OUTPUT, @PMSG OUTPUT",
                                                    codeParam, descParam, msgParam)
                                        .ToList();
                    Stores = query.GroupBy(s => new { s.StoreId, s.StoreName })
                                .Select(g => g.First())
                                .ToList();

                    var KeeperGroupId = dbContext.UserGroups
                            .Where(g => g.UserGroupName == "Warehouse Keeper")
                            .Select(g => g.UserGroupId)
                            .FirstOrDefault();

                    KeeperGroupsList = dbContext.Users
                        .Where(u => u.UserGroupId == KeeperGroupId)
                        .ToList();

                    if (!StoreId.HasValue)
                        ErrorMsg = (Program.Translations["WarehouseMissing"])[Lang];
                    if(string.IsNullOrEmpty(BuildingNumber))
                        ErrorMsg = (Program.Translations["BuildingNumberMissing"])[Lang];
                    else if (string.IsNullOrEmpty(RoomNumber))
                        ErrorMsg = (Program.Translations["RoomNumberMissing"])[Lang];
                    if (!StoreId.HasValue)
                        ErrorMsg = (Program.Translations["StoreNumberMissing"])[Lang];
                    else if (!KeeperId.HasValue)
                        ErrorMsg = (Program.Translations["KeeperMissing"])[Lang];
                    else if( HttpContext.Session.GetString("UserGroup") == "Warehouse Manager")
                    {
                        if(HttpContext.Session.GetInt32("UserId").Value != WarehouseManagerID){
                            ErrorMsg = (Program.Translations["ManagerEditWarehouseOnly"])[Lang];

                        }
                    }
                    // else if (string.IsNullOrEmpty(RoomName))
                    //     ErrorMsg = (Program.Translations["StoreNameMissing"])[Lang];
                    else
                    {
                        var selectedStore = dbContext.Stores.FirstOrDefault(X => X.StoreId == StoreId);
                        var name = selectedStore.StoreName;
                        // if (dbContext.Rooms.Count(s => s.RoomNo == RoomNumber && s.RoomId != RoomId) > 0)
                        //     ErrorMsg = string.Format((Program.Translations["StoreNumberExists"])[Lang], RoomNumber);
                        // else if (dbContext.Rooms.Count(s => s.RoomNo == RoomName && s.RoomId != RoomId) > 0)
                        //     ErrorMsg = string.Format((Program.Translations["StoreNameExists"])[Lang], RoomName);
                        if (dbContext.Rooms.Any(r =>
                            r.RoomNo == RoomNumber &&
                            r.StoreId == StoreId &&
                            r.BuildingNumber == BuildingNumber &&
                            r.RoomId != RoomId)) // Exclude the current room being edited
                        {
                            ErrorMsg = string.Format((Program.Translations["RoomExists"])[Lang], RoomNumber);
                        }

                        else
                        {
                            var room = dbContext.Rooms.Single(s => s.RoomId == RoomId);

                            /*var oldShelves = store.ShelfNumbers.Split(',');
                            var newShelves = Shelves.Split(',');
                            var deletedShelves = oldShelves.Except(newShelves);

                            foreach (var deletedShelf in deletedShelves)
                            {
                                var firstItem = dbContext.Storages.Where(s => s.StoreId == store.StoreId
                                        && s.AvailableQuantity > 0 && s.ShelfNumber == deletedShelf)
                                    .FirstOrDefault();
                                if (firstItem != null)
                                {
                                    ErrorMsg = string.Format((Program.Translations["StoreNotUpdate"])[Lang], deletedShelf);
                                    return Page();
                                }
                            }*/

                            // room.RoomName = RoomName;
                            room.RoomNo = RoomNumber;
                            room.StoreId = StoreId;
                            room.BuildingNumber = BuildingNumber;
                            room.RoomDesc = RoomDesc;
                            room.NoOfShelves = NoOfShelves;
                            // room.KeeperID = KeeperId;
                            room.RoomStatus = Status;
                            dbContext.SaveChanges();

                            string Message = string.Format("Store {0} updated", room.RoomName);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Update",
                                Helper.ExtractIP(Request), dbContext, true);

                            task.LogInfo(MethodBase.GetCurrentMethod(), "store updated");
                            return RedirectToPage("./ManageStore");
                        }
                    }
                    return Page();
                }
                else
                    return RedirectToPage("./Index?lang=" + Lang);
            }
            catch (Exception ex)
            {
                task.LogError(MethodBase.GetCurrentMethod(), ex);
                ErrorMsg = ex.Message;
                return Page();
            }
            finally { task.EndTask(); }
        }

        private void FillLables()
        {


            this.lblUpdateStore = (Program.Translations["UpdateStore"])[Lang];
            this.lblStoreNumber = (Program.Translations["StoreNumber"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblShelves = (Program.Translations["Shelves"])[Lang];
            this.lblUpdate = (Program.Translations["Update"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang]; 
            this.lblRoomNumber = (Program.Translations["RoomNumber"])[Lang];
            this.lblRoomName = (Program.Translations["RoomName"])[Lang]; 
            this.lblUpdateRoom = (Program.Translations["UpdateRoom"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];
            this.lblManageRooms = (Program.Translations["ManageRooms"])[Lang];


        }
    }
}
