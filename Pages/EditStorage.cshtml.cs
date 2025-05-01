using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace LabMaterials.Pages
{
    public class EditStorageModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public int StorageId, StoreId, ItemId, Quantity;
        public string ShelfNumber, StoreNumberSearch, StoreNameSearch, Item;
        public int page { get; set; }
        public List<Store> Stores { get; set; }
        public List<Item> Items { get; set; }
        
        public string lblItemName, lblStoreName, lblUpdateStorage, lblShelveNumber, lblQuantity, lblUpdate, lblCancel, lblStorage, lblStores;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            this.page = (int)HttpContext.Session.GetInt32("page");
            this.StoreNumberSearch = HttpContext.Session.GetString("StoreNumber");
            this.StoreNameSearch = HttpContext.Session.GetString("StoreName");
             this.Item = HttpContext.Session.GetString("Item");
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            var dbContext = new LabDBContext();
            var storage = dbContext.Storages.Single(st => st.StorageId == HttpContext.Session.GetInt32("StorageId"));

            this.StorageId = storage.StorageId;
            this.StoreId = storage.StoreId;
            this.ItemId = storage.ItemId;
            this.ShelfNumber = storage.ShelfNumber;
            this.Quantity = storage.AvailableQuantity;

            Stores = dbContext.Stores.ToList();
            Items = dbContext.Items.ToList();
        }

        public IActionResult OnPost([FromForm] int StorageId, [FromForm] int ItemId, [FromForm] int StoreId, [FromForm] string ShelfNumber, [FromForm] int Quantity)
        {
            LogableTask task = LogableTask.NewTask("UpdateStorage");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageStore)
                {
                    FillLables();
                    var dbContext = new LabDBContext();
                    this.StoreId = StoreId;
                    this.ItemId = ItemId;
                    this.ShelfNumber = ShelfNumber;
                    this.Quantity = Quantity;
                    Stores = dbContext.Stores.ToList();
                    Items = dbContext.Items.ToList();

                    var store = dbContext.Stores.Single(s => s.StoreId == StoreId);
                    var Shelves = store.ShelfNumbers.Split(',');
                    if (!Shelves.Contains(ShelfNumber))
                    {
                        ErrorMsg = string.Format((Program.Translations["ShelfNumberNotExists"])[Lang], store.StoreName, ShelfNumber);
                        return Page();
                    }
                    var storage = dbContext.Storages.Single(st => st.StorageId == StorageId);
                    storage.StoreId = StoreId;
                    storage.ItemId = ItemId;
                    storage.ShelfNumber = ShelfNumber;
                    storage.AvailableQuantity = Quantity;

                    dbContext.SaveChanges();
                    task.LogInfo(MethodBase.GetCurrentMethod(), "storage Updated");

                    string Message = string.Format("Storage for item {0} update for the store {1}", storage.Item.ItemName, storage.Store.StoreName);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Update",
                        Helper.ExtractIP(Request), dbContext, true);

                    return RedirectToPage("./ManageStorage");

                  
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
            

            this.lblUpdateStorage = (Program.Translations["UpdateStorage"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblShelveNumber = (Program.Translations["ShelveNumber"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblUpdate = (Program.Translations["Update"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblStorage = (Program.Translations["Storages"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];
        }
    }
}
