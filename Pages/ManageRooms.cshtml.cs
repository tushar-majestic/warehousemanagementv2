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
        public List<Room> Rooms { get; set; }
        public string Message { get; set; }
        public string RoomName { get; set; }
        public int TotalItems { get; set; }
        public void OnGet() 
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public string lblStores, lblManageStorage, lblSearch, lblRoomName, lblRoomNumber, lblManageShelves, lblManageRooms, lblStoreNumber,
        lblAddRoom, lblAddShelf, lblStoreName, lblSubmit, lblAddStore, lblShelves, lblEdit, lblDelete, lblTotalItem, lblAddDestination,
        lblManageDestination;

        // public void OnPostSearch([FromForm] string StoreNumber, [FromForm] string StoreName)
        // {
        //     FillData(StoreNumber, StoreName);
        // }

        public void OnPostSearch([FromForm] string RoomName)
        {
            this.RoomName = RoomName; // Store the input RoomName in the model
            FillData(null, null);     // Load data again based on new RoomName
        }

        public void OnPostDelete([FromForm] int RoomId)
        {
            var dbContext = new LabDBContext();

            var itemsInstore = dbContext.Storages.Where(e => e.Ended == null).Count(s => s.RoomId == RoomId && s.AvailableQuantity > 0);
            if (itemsInstore == 0)
            {
                var room = dbContext.Rooms.Single(s => s.RoomId == RoomId);
                room.Ended = DateTime.Now;
                dbContext.Rooms.Update(room);
                dbContext.SaveChanges();
                FillData(null, null);
                Message = string.Format((Program.Translations["RoomDeleted"])[Lang], room.RoomName);
                Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
            }
            else
            {
                var itemId = dbContext.Storages.First(s => s.RoomId == RoomId && s.AvailableQuantity > 0).ItemId;
                Message = string.Format((Program.Translations["RoomNotDeleted"])[Lang], itemsInstore,
                    dbContext.Items.Single(i => i.ItemId == itemId).ItemName);
                FillData(null, null);
            }

        }


        public IActionResult OnPostEdit([FromForm] int RoomId)
        {
            HttpContext.Session.SetInt32("RoomId", RoomId);

            return RedirectToPage("./EditRoom");
        }

        public IActionResult OnPostManageShelves([FromForm] int RoomId)
        {
            /*var dbContext = new LabDBContext();

            var shelfDetails = (from sh in dbContext.Shelves
                                where sh.RoomId == RoomId
                                select sh).ToList();*/

            HttpContext.Session.SetInt32("RoomId", RoomId);

            return RedirectToPage("./ManageShelves");
        }

        private void FillData(string? StoreNumber, string? StoreName)
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

                var roomsQuery = dbContext.Rooms.AsQueryable();  // <- Start with all rooms

                if (!string.IsNullOrEmpty(RoomName))
                {
                    roomsQuery = roomsQuery.Where(r => r.RoomName.Contains(RoomName));
                }

                Rooms = roomsQuery.ToList();


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
            




        }
    }
}
