using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LabMaterials.Pages
{
    public class ManageShelvesModel : BasePageModel
    {
        public List<StoreDataResult> Stores { get; set; }
        public List<Shelf> StoresAll { get; set; }
        public List<Shelf> shelfDetails { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }
        public string ShelfNumber { get; set; }

        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }

        public void OnGet(string? ShelfNumber, int page = 1) 
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
                 if (HttpContext.Request.Query.ContainsKey("page")){
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.ShelfNumber = ShelfNumber;
                    FillData(ShelfNumber, CurrentPage);

                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public string lblStores, lblManageStorage, lblSearch, lblRoomName, lblAddShelves, lblRoomNumber, lblShelfNumber, 
        lblManageShelves, lblManageRooms, lblStoreNumber, lblAddRoom, lblAddShelf, lblStoreName, lblSubmit, lblAddStore, 
        lblShelves, lblEdit, lblDelete, lblTotalItem, lblAddDestination, lblManageDestination;

        public void OnPostSearch([FromForm] string ShelfNumber)
        {   CurrentPage = 1;
            this.ShelfNumber = ShelfNumber;
            FillData(ShelfNumber, CurrentPage);
        }

        public void OnPostDelete([FromForm] int ShelfId)
        {
            var dbContext = new LabDBContext();

            var itemsInstore = dbContext.Storages.Count(s => s.ShelfId == ShelfId && s.AvailableQuantity > 0);
            if (itemsInstore == 0)
            {
                var shelf = dbContext.Shelves.Single(s => s.ShelfId == ShelfId);
                shelf.Ended = DateTime.Now;
                dbContext.Shelves.Update(shelf);
                dbContext.SaveChanges();
                FillData(null);
                Message = string.Format("Shelve {0} deleted", shelf.ShelfNo);
                Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
            }
            else
            {
                var itemId = dbContext.Storages.First(s => s.ShelfId == ShelfId && s.AvailableQuantity > 0).ItemId;
                Message = string.Format((Program.Translations["ShelveNotDeleted"])[Lang], itemsInstore,
                    dbContext.Items.Single(i => i.ItemId == itemId).ItemName);
                FillData(null);
            }

        }


        public IActionResult OnPostEdit([FromForm] int ShelfId, [FromForm] int page, [FromForm] string ShelfNumber)
        {
            HttpContext.Session.SetInt32("ShelfId", ShelfId);
            HttpContext.Session.SetInt32("page", page);
            HttpContext.Session.SetString("ShelfNumber", string.IsNullOrEmpty(ShelfNumber) ? "" : ShelfNumber);
            return RedirectToPage("./EditShelves");
        }

        private void FillData(string? ShelfNumber, int page = 1)
        {   if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
                var dbContext = new LabDBContext();

                var roomId  = HttpContext.Session.GetInt32("RoomId");

                var query = (from sh in dbContext.Shelves
                                    where sh.RoomId == roomId
                                    orderby sh.ShelfId // Order by shelf ID or any other property if needed
                                    select sh).ToList();


                /*if (string.IsNullOrEmpty(StoreNumber) == false)
                    query = query.Where(s => s.StoreNumber.Contains(StoreNumber));

                if (string.IsNullOrEmpty(StoreName) == false)
                    query = query.Where(s => s.StoreName.Contains(StoreName));

                Stores = query.ToList();
                foreach (var s in Stores)
                    s.Shelves = string.IsNullOrEmpty(s.ShelfNumbers) ? new string[0] : s.StoreNumber.Split(',');

                Stores = query.ToList();*/

                if (string.IsNullOrEmpty(ShelfNumber) == false)
                    query = query.Where(s => s.ShelfNo.Contains(ShelfNumber)).ToList();

                TotalItems = query.Count();
                // shelfDetails = query;
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
                var list = query.ToList();
                shelfDetails = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
                StoresAll = query.ToList();
                CurrentPage = page; 
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
            this.lblAddShelves = (Program.Translations["AddShelves"])[Lang]; 
            this.lblShelfNumber = (Program.Translations["ShelfNumber"])[Lang];



        }
    }
}
