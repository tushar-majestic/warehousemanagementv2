using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class EditShelvesModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string RoomNumber, RoomName, ShelfNumber;
        public int? RoomId, ShelfId;
        public int page { get; set; }
        public string lblUpdateStore, lblStoreNumber, lblUpdateShelf, lblStoreName, lblShelfNumber, 
        lblShelves, lblUpdate, lblCancel, lblRoomNumber, lblRoomName, lblStores, lblManageRooms, lblManageShelves;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
             this.page = (int)HttpContext.Session.GetInt32("page");
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            else
            {
                var dbContext = new LabDBContext();
                var shelf = dbContext.Shelves.Single(s => s.ShelfId == HttpContext.Session.GetInt32("ShelfId"));

                ShelfNumber = shelf.ShelfNo;
                ShelfId = shelf.ShelfId;
            }
        }

        public IActionResult OnPost([FromForm] int ShelfId, [FromForm] string ShelfNumber)
        {
            LogableTask task = LogableTask.NewTask("EditShelf");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageStore)
                {
                    FillLables();
                    
                    this.ShelfNumber = ShelfNumber;

                    
                        var dbContext = new LabDBContext();
                        
                            var shelf = dbContext.Shelves.Single(s => s.ShelfId == ShelfId);

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
                            shelf.ShelfNo = ShelfNumber;
                            dbContext.SaveChanges();

                            string Message = string.Format("Shelf {0} updated", shelf.ShelfNo);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Update",
                                Helper.ExtractIP(Request), dbContext, true);

                            task.LogInfo(MethodBase.GetCurrentMethod(), "Shelf updated");
                            return RedirectToPage("./ManageShelves");
                        
                    
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
            this.lblShelfNumber = (Program.Translations["ShelfNumber"])[Lang]; 
            this.lblUpdateShelf = (Program.Translations["UpdateShelf"])[Lang];
            this.lblManageShelves = (Program.Translations["ManageShelves"])[Lang];
            this.lblManageRooms = (Program.Translations["ManageRooms"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];
            


        }
    }
}
