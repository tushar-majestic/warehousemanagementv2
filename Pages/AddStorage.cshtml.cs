using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace LabMaterials.Pages
{
    public class AddStorageModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public int StoreId, ItemId, Quantity;
        public string ShelfNumber;

        public List<Store> Stores { get; set; }
        public List<Item> Items { get; set; }
        
        public string lblItemName, lblStoreName, lblAddStorage, lblShelveNumber, lblQuantity, lblAdd, lblCancel, lblStorage;

        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);

            var dbContext = new LabDBContext();
            Stores = dbContext.Stores.ToList();
            Items = dbContext.Items.ToList();
        }

        public IActionResult OnPost([FromForm] int ItemId, [FromForm] int StoreId, [FromForm] string ShelfNumber, [FromForm] int Quantity)
        {
            LogableTask task = LogableTask.NewTask("AddStorage");

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

                    var storage = new Storage
                    {
                        ItemId = ItemId,
                        StoreId = StoreId,
                        ShelfNumber = ShelfNumber,
                        AvailableQuantity = Quantity
                    };
                    dbContext.Storages.Add(storage);
                    dbContext.SaveChanges();
                    task.LogInfo(MethodBase.GetCurrentMethod(), "storage added");

                    string Message = string.Format("Storage for item {0} added for the store {1}", storage.Item.ItemName, storage.Store.StoreName);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
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
            

            this.lblAddStorage = (Program.Translations["AddStorage"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblShelveNumber = (Program.Translations["ShelveNumber"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblStorage = (Program.Translations["Storages"])[Lang];
        }
    }
}
