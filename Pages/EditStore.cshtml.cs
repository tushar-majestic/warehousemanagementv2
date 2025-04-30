using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class EditStoreModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string StoreNumber, StoreName, Shelves, StoreNameSearch, StoreNumberSearch;
        public int StoreId;
        public int page { get; set; }
        
        public string lblUpdateStore, lblStoreNumber, lblStoreName, lblShelves, lblUpdate, lblCancel, lblStores;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            this.page = (int)HttpContext.Session.GetInt32("page");
            this.StoreNumberSearch = HttpContext.Session.GetString("StoreNumber");
            this.StoreNameSearch = HttpContext.Session.GetString("StoreName");
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            else
            {
                var dbContext = new LabDBContext();
                var store = dbContext.Stores.Single(s => s.StoreId == HttpContext.Session.GetInt32("StoreId"));

                StoreNumber = store.StoreNumber;
                StoreName = store.StoreName;
                StoreId = store.StoreId;
            }
        }

        public IActionResult OnPost([FromForm] int StoreId, [FromForm] string StoreNumber, [FromForm] string StoreName, [FromForm] string Shelves)
        {
            LogableTask task = LogableTask.NewTask("EditStore");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageStore)
                {
                    FillLables();
                    this.StoreName = StoreName;
                    this.StoreNumber = StoreNumber;
                    this.StoreId = StoreId;

                    if (string.IsNullOrEmpty(StoreNumber))
                        ErrorMsg = (Program.Translations["StoreNumberMissing"])[Lang];
                    else if (string.IsNullOrEmpty(StoreName))
                        ErrorMsg = (Program.Translations["StoreNameMissing"])[Lang];
                    else
                    {
                        var dbContext = new LabDBContext();
                        if (dbContext.Stores.Count(s => s.StoreNumber == StoreNumber && s.StoreId != StoreId) > 0)
                            ErrorMsg = string.Format((Program.Translations["StoreNumberExists"])[Lang], StoreNumber);
                        else if (dbContext.Stores.Count(s => s.StoreName == StoreName && s.StoreId != StoreId) > 0)
                            ErrorMsg = string.Format((Program.Translations["StoreNameExists"])[Lang], StoreName);
                        else
                        {
                            var store = dbContext.Stores.Single(s => s.StoreId == StoreId);

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
                            store.StoreName = StoreName;
                            store.StoreNumber = StoreNumber;
                            dbContext.SaveChanges();

                            string Message = string.Format("Store {0} updated", store.StoreName);
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
            this.lblStores = (Program.Translations["Stores"])[Lang];
        }
    }
}
