using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class AddStoreModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string StoreNumber, StoreName, Shelves, StoreType,ManagerJobNumber ;
        public string Status { get; set; }

        public int? ManagerId;
        public List<User> ManagerGroupsList {  get; set; }

        public string lblStores, lblAddStore, lblStoreNumber, lblStoreName, lblShelves, lblAdd, lblCancel,lblWarehouseType, lblManagerName, lblManagerJobNumber,lblStatus, lblOpen, lblClosed ;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            var dbContext = new LabDBContext();
            var managerGroupId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Warehouse Manager")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            ManagerGroupsList = dbContext.Users
                .Where(u => u.UserGroupId == managerGroupId)
                .ToList();
                Console.WriteLine(ManagerGroupsList);

            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPost([FromForm] string StoreNumber, [FromForm] string StoreName, [FromForm] string StoreType, [FromForm] int? ManagerId, [FromForm] string ManagerJobNumber, [FromForm] string Status)
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
                    this.StoreType = StoreType;
                    this.ManagerId = ManagerId;
                    this.ManagerJobNumber = ManagerJobNumber;
                    int parsedManagerJobNumber = 0;
                    int.TryParse(ManagerJobNumber, out parsedManagerJobNumber);
                    this.Status = Status;

                    var dbContext = new LabDBContext();
                    var managerGroupId = dbContext.UserGroups
                                        .Where(g => g.UserGroupName == "Manager")
                                        .Select(g => g.UserGroupId)
                                        .FirstOrDefault();

                    ManagerGroupsList = dbContext.Users
                                    .Where(u => u.UserGroupId == managerGroupId)
                                    .ToList();

                    if(string.IsNullOrEmpty(StoreType))
                        ErrorMsg = (Program.Translations["StoreTypeMissing"])[Lang];
                    else if (string.IsNullOrEmpty(StoreName))
                        ErrorMsg = (Program.Translations["StoreNameMissing"])[Lang];
                    else if (string.IsNullOrEmpty(StoreNumber))
                        ErrorMsg = (Program.Translations["StoreNumberMissing"])[Lang];
                    else if (!ManagerId.HasValue)
                        ErrorMsg = (Program.Translations["ManagerNameMissing"])[Lang];
                    else if (string.IsNullOrEmpty(ManagerJobNumber))
                        ErrorMsg = (Program.Translations["ManagerJobNumberMissing"])[Lang];
                    else if (string.IsNullOrEmpty(Status))
                        ErrorMsg = (Program.Translations["WarehouseStatusMissing"])[Lang];
                    
                    else
                    {
                        // var dbContext = new LabDBContext();
                        if (dbContext.Stores.Count(s => s.StoreNumber == StoreNumber) > 0)
                            ErrorMsg = string.Format((Program.Translations["StoreNumberExists"])[Lang], StoreNumber);
                        else if (dbContext.Stores.Count(s => s.StoreName == StoreName) > 0)
                            ErrorMsg = string.Format((Program.Translations["StoreNameExists"])[Lang], StoreName);
                        else
                        {
                            var store = new Store
                            {   
                                StoreType = StoreType,
                                WarehouseManagerID = ManagerId,
                                // ManagerJobNum = parsedManagerJobNumber,
                                WarehouseStatus = Status,
                                ShelfNumbers = Shelves,
                                StoreName = StoreName,
                                StoreNumber = StoreNumber,
                                StoreId = PrimaryKeyManager.GetNextId(),
                               

                            };
                            dbContext.Stores.Add(store);
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "store added");

                            string Message = string.Format("Warehouse {0} added", store.StoreName);
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
            
            this.lblStores = (Program.Translations["Stores"])[Lang];
            this.lblAddStore = (Program.Translations["AddStore"])[Lang];
            this.lblStoreNumber = (Program.Translations["StoreNumber"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblShelves = (Program.Translations["Shelves"])[Lang];
            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblWarehouseType = (Program.Translations["WarehouseType"])[Lang];
            this.lblManagerName = (Program.Translations["ManagerName"])[Lang];
            this.lblManagerJobNumber = (Program.Translations["ManagerJobNumber"])[Lang];
            this.lblStatus = (Program.Translations["WarehouseStatus"])[Lang];
            
            this.lblOpen = (Program.Translations["Open"])[Lang];
            this.lblClosed = (Program.Translations["Closed"])[Lang];


        }
    }
}
