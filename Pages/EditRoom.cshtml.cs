using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class EditRoomModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string RoomNumber, RoomName, RoomNameSearch;
        public int? RoomId, SelectedStoreId;
        public List<Store> Stores { get; set; }
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
                SelectedStoreId = room.StoreId;
                Stores = dbContext.Stores.ToList();
            }
        }

        public IActionResult OnPost([FromForm] int RoomId, [FromForm] string RoomNumber, [FromForm] string RoomName, int StoreID)
        {
            LogableTask task = LogableTask.NewTask("EditRoom");

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
                        ErrorMsg = (Program.Translations["StoreNumberMissing"])[Lang];
                    else if (string.IsNullOrEmpty(RoomName))
                        ErrorMsg = (Program.Translations["StoreNameMissing"])[Lang];
                    else
                    {
                        var dbContext = new LabDBContext();
                        var selectedStore = dbContext.Stores.FirstOrDefault(X => X.StoreId == StoreID);
                        var name = selectedStore.StoreName;
                        if (dbContext.Rooms.Count(s => s.RoomNo == RoomNumber && s.RoomId != RoomId) > 0)
                            ErrorMsg = string.Format((Program.Translations["StoreNumberExists"])[Lang], RoomNumber);
                        else if (dbContext.Rooms.Count(s => s.RoomNo == RoomName && s.RoomId != RoomId) > 0)
                            ErrorMsg = string.Format((Program.Translations["StoreNameExists"])[Lang], RoomName);
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
                            room.RoomName = RoomName;
                            room.RoomNo = RoomNumber;
                            room.StoreId = StoreID;
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
