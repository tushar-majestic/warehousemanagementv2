using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class AddStoreModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string StoreNumber, StoreName, Shelves;

        public string lblAddStore, lblStoreNumber, lblStoreName, lblShelves, lblAdd, lblCancel;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPost([FromForm] string StoreNumber, [FromForm] string StoreName)
        {
            LogableTask task = LogableTask.NewTask("AddStore");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageStore)
                {
                    FillLables();
                    this.StoreName = StoreName;
                    this.StoreNumber = StoreNumber;
                    this.Shelves = "";

                    if (string.IsNullOrEmpty(StoreNumber))
                        ErrorMsg = (Program.Translations["StoreNumberMissing"])[Lang];
                    else if (string.IsNullOrEmpty(StoreName))
                        ErrorMsg = (Program.Translations["StoreNameMissing"])[Lang];
                    else
                    {
                        var dbContext = new LabDBContext();
                        if (dbContext.Stores.Count(s => s.StoreNumber == StoreNumber) > 0)
                            ErrorMsg = string.Format((Program.Translations["StoreNumberExists"])[Lang], StoreNumber);
                        else if (dbContext.Stores.Count(s => s.StoreName == StoreName) > 0)
                            ErrorMsg = string.Format((Program.Translations["StoreNameExists"])[Lang], StoreName);
                        else
                        {
                            var store = new Store
                            {
                                ShelfNumbers = Shelves,
                                StoreName = StoreName,
                                StoreNumber = StoreNumber,
                                StoreId = PrimaryKeyManager.GetNextId()
                            };
                            dbContext.Stores.Add(store);
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "store added");

                            string Message = string.Format("Store {0} added", store.StoreName);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                                Helper.ExtractIP(Request), dbContext, true);

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
            

            this.lblAddStore = (Program.Translations["AddStore"])[Lang];
            this.lblStoreNumber = (Program.Translations["StoreNumber"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblShelves = (Program.Translations["Shelves"])[Lang];
            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
        }
    }
}
