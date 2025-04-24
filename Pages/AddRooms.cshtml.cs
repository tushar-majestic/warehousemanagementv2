using LabMaterials.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class AddRoomsModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string StoreNumber, StoreName, RoomNumber, RoomName;
        public int StoreId;
        public List<Store> Stores { get; set; }

        public string lblAddStore, lblRoomNumber, lblStoreName, lblAdd, lblCancel, lblRoomName, lblStoreNumber, 
        lblAddRoom, lblStores, lblManageRooms;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            var dbContext = new LabDBContext();
            Stores = dbContext.Stores.ToList();
        }

        public IActionResult OnPost([FromForm] int StoreId, [FromForm] string RoomNumber, [FromForm] string RoomName)
        {
            LogableTask task = LogableTask.NewTask("AddRoom");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageStore)
                {
                    FillLables();
                    this.RoomName = RoomName;
                    this.RoomNumber = RoomNumber;

                    if (string.IsNullOrEmpty(RoomNumber))
                        ErrorMsg = (Program.Translations["RoomNumberMissing"])[Lang];
                    else if (string.IsNullOrEmpty(RoomName))
                        ErrorMsg = (Program.Translations["RoomNameMissing"])[Lang];
                    else
                    {
                        var dbContext = new LabDBContext();
                        if (dbContext.Rooms.Any(r => r.RoomNo == RoomNumber && r.StoreId == StoreId))
                            ErrorMsg = string.Format((Program.Translations["RoomNumberExists"])[Lang], RoomNumber);
                        else if (dbContext.Rooms.Any(r => r.RoomName == RoomName && r.StoreId == StoreId))
                            ErrorMsg = string.Format((Program.Translations["RoomNameExists"])[Lang], RoomName);
                        else
                        {
                            var room = new Room
                            {
                                RoomName = RoomName,
                                StoreId = StoreId,
                                /*RoomNumber = RoomNumber,*/
                                RoomNo = RoomNumber,
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
                    using (var dbContext = new LabDBContext())
                        { Stores = dbContext.Stores.ToList(); }

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



        }
    }
}
