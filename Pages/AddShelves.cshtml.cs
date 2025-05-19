using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace LabMaterials.Pages
{
    public class AddShelvesModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string StoreNumber, StoreName, RoomNo, RoomName, ShelfNumber;
        public int StoreId;
        public List<Store> Stores { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Shelf> Shelves { get; set; }

        public string lblAddStore, lblRoomNumber, lblStoreName, lblAdd, lblCancel, lblRoomName, lblStoreNumber, lblAddRoom, 
        lblShelves, lblShelfNumber, lblStores, lblManageRooms, lblManageShelves;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            var RoomId = HttpContext.Session.GetInt32("RoomId");
            var StoreId = HttpContext.Session.GetInt32("StoreId");
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            var dbContext = new LabDBContext();
            Stores = dbContext.Stores.Where(s => s.StoreId == StoreId).ToList();
            Rooms = dbContext.Rooms.Where(r=> r.RoomId == RoomId && r.Ended == null).ToList();
            Shelves = dbContext.Shelves.ToList();
        }
        public IActionResult OnGetRoomsForStore(int storeId)
        {
            var dbContext = new LabDBContext();
            var rooms = dbContext.Rooms.Where(r => r.StoreId == storeId && r.Ended == null)
                        .Select(r => new { r.RoomId, r.RoomName, r.RoomNo })
                        .ToList();
            return new JsonResult(rooms);

        }

        public IActionResult OnPost([FromForm] int StoreId, [FromForm] int RoomId, [FromForm] string ShelfNumber, [FromForm] string RoomName)
        {
            LogableTask task = LogableTask.NewTask("AddShelve");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageStore)
                {
                    FillLables();
                    

                    if (string.IsNullOrEmpty(ShelfNumber))
                        ErrorMsg = (Program.Translations["RoomNumberMissing"])[Lang];
                    else
                    {
                        var dbContext = new LabDBContext();
                        var rm = dbContext.Rooms.Single(r=>r.RoomId == RoomId);
                        this.RoomName = rm.RoomName;
                        this.ShelfNumber = ShelfNumber;
                        /*if (dbContext.Shelves.Count(s => s.ShelfNo == ShelfNumber) > 0)
                            ErrorMsg = string.Format((Program.Translations["RoomNumberExists"])[Lang], ShelfNumber);*/


                        var shelf = new Shelf
                            {
                                ShelfNo = ShelfNumber,
                                StoreId = StoreId,
                                /*RoomNumber = RoomNumber,*/
                                RoomId = RoomId,
                            };
                            dbContext.Shelves.Add(shelf);
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "room added");

                            // string Message = string.Format("Room {0} added", shelf.ShelfNo);
                            string Message = string.Format("Shelve {0} added", shelf.ShelfNo,this.RoomName );

                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                                Helper.ExtractIP(Request), dbContext, true);

                            return RedirectToPage("./ManageShelves");
                        
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


            this.lblAddStore = (Program.Translations["AddStore"])[Lang];
            this.lblStoreNumber = (Program.Translations["StoreNumber"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblRoomName = (Program.Translations["RoomName"])[Lang];
            this.lblRoomNumber = (Program.Translations["RoomNumber"])[Lang];
            this.lblAddRoom = (Program.Translations["AddRoom"])[Lang];
            this.lblShelfNumber = (Program.Translations["ShelfNumber"])[Lang];
            this.lblShelves = (Program.Translations["Shelves"])[Lang];
            this.lblManageShelves = (Program.Translations["ManageShelves"])[Lang];
            this.lblManageRooms = (Program.Translations["ManageRooms"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];
            this.lblManageShelves = (Program.Translations["ManageShelves"])[Lang];
            


        }
    }
}
