using System.Data;
using LabMaterials.DB;
using LabMaterials.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace LabMaterials.Pages
{
    public class AddRoomsModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string StoreNumber, StoreName, RoomNumber,KeeperName, RoomName, StoreType, ManagerName, BuildingNumber, RoomDesc;
        public int? StoreId;
        public int? KeeperJobNum, NoOfShelves, KeeperId ;
        public int? WarehouseManagerID { get; set; }
        public string WarehouseManagerName { get; set; }
        public string Status { get; set; }
        // public List<Store> Stores { get; set; }

        public List<StoreDataResult> Stores { get; set; }

        public List<User> KeeperGroupsList {  get; set; }


        public string lblAddStore, lblRoomNumber, lblStoreName, lblAdd, lblCancel, lblRoomName, lblStoreNumber, 
        lblAddRoom, lblStores, lblManageRooms,lblNoOfShelves, lblKeeperJobNum, lblKeeperName;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            var dbContext = new LabDBContext();
            //   Stores = dbContext.Stores.ToList();

                
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

        public IActionResult OnPost([FromForm] int? StoreId, [FromForm] string RoomNumber,  [FromForm] string StoreType, [FromForm] string ManagerName, [FromForm] string BuildingNumber, [FromForm] string RoomDesc, [FromForm] int NoOfShelves, [FromForm] int? KeeperJobNum, [FromForm] int? KeeperId,  [FromForm] string Status, [FromForm] string KeeperName)
        {
            LogableTask task = LogableTask.NewTask("AddRoom");

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
                    // this.KeeperJobNum = KeeperJobNum;
                    this.KeeperId = KeeperId;
                    this.KeeperName = KeeperName;
                    this.Status = Status;
                    this.StoreId = StoreId; 

                    var dbContext = new LabDBContext();

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

                    // var managerId = dbContext.Stores
                    //     .Where(s => s.StoreId == StoreId)
                    //     .Select(s => s.WarehouseManagerID)
                    //     .FirstOrDefault();

                    var KeeperGroupId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Warehouse Keeper")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

                    KeeperGroupsList = dbContext.Users
                        .Where(u => u.UserGroupId == KeeperGroupId)
                        .ToList();

                    if (!StoreId.HasValue)
                        ErrorMsg = (Program.Translations["WarehouseMissing"])[Lang];
                    else if(string.IsNullOrEmpty(BuildingNumber))
                        ErrorMsg = (Program.Translations["BuildingNumberMissing"])[Lang];
                    else if (string.IsNullOrEmpty(RoomNumber))
                        ErrorMsg = (Program.Translations["RoomNumberMissing"])[Lang];
                    // else if (string.IsNullOrEmpty(RoomName))
                    //     ErrorMsg = (Program.Translations["RoomNameMissing"])[Lang];
                    else if (!KeeperId.HasValue)
                        ErrorMsg = (Program.Translations["KeeperMissing"])[Lang];
                    else if (string.IsNullOrEmpty(Status))
                        ErrorMsg = (Program.Translations["RoomStatusMissing"])[Lang];
                    // else if( HttpContext.Session.GetString("UserGroup") == "Warehouse Manager")
                    // {
                    //     if(HttpContext.Session.GetInt32("UserId").Value != managerId){
                    //         ErrorMsg = (Program.Translations["ManagerWarehouseOnly"])[Lang];

                    //     }
                    // }
                    else
                    {

                        if (dbContext.Rooms.Any(r => r.RoomNo == RoomNumber && r.StoreId == StoreId && r.BuildingNumber == BuildingNumber))
                            ErrorMsg = string.Format((Program.Translations["RoomExists"])[Lang], RoomNumber);
                       
                        else
                        {
                            var room = new Room
                            {
                                // RoomName = RoomName,
                                StoreId = StoreId,
                                /*RoomNumber = RoomNumber,*/
                                RoomNo = RoomNumber,
                                BuildingNumber = BuildingNumber,
                                RoomDesc = RoomDesc,
                                NoOfShelves = NoOfShelves,
                                KeeperId = KeeperId,
                                RoomStatus = Status,

                            };
                            dbContext.Rooms.Add(room);
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "room added");

                            string Message = string.Format("Room {0} added", room.RoomName);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                                Helper.ExtractIP(Request), dbContext, true);

                            return RedirectToPage("./ManageRooms");
                        }
                    }
                    // using ( dbContext = new LabDBContext())
                    //      { Stores = dbContext.Stores.ToList(); }

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


            this.lblAddStore = (Program.Translations["AddStore"])[Lang];
            this.lblStoreNumber = (Program.Translations["StoreNumber"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang]; 
            this.lblRoomName = (Program.Translations["RoomName"])[Lang]; 
            this.lblRoomNumber = (Program.Translations["RoomNumber"])[Lang]; 
            this.lblAddRoom = (Program.Translations["AddRoom"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];
            this.lblManageRooms = (Program.Translations["ManageRooms"])[Lang];
            this.lblNoOfShelves = (Program.Translations["NoOfShelves"])[Lang];
            this.lblKeeperJobNum = (Program.Translations["KeeperJobNum"])[Lang];
            this.lblKeeperName = (Program.Translations["KeeperName"])[Lang];



        }
    }
}
