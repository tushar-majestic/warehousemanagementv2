using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace LabMaterials.Pages
{
    public class EditStoreModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string StoreNumber, StoreName, Shelves, ManagerJobNumber, StoreNameSearch, StoreNumberSearch;

        public int StoreId;
        public int? ManagerId;

        public string Status { get; set; }
        public string StoreType ;
        public List<User> ManagerGroupsList {  get; set; }
        public List<StoreTypes> StoreTypeList { get; set; }

        public string lblUpdateStore, lblStoreNumber, lblStoreName, lblShelves, lblUpdate, lblCancel, lblStores, lblWarehouseType, lblManagerName, lblManagerJobNumber, lblStatus, lblOpen, lblClosed;

        public int page { get; set; }
        

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
                // StoreTypeList = new List<SelectListItem>
                // {
                //     new SelectListItem { Value = "Central", Text = "Central" },
                //     new SelectListItem { Value = "Branch", Text = "Branch" },
                //     new SelectListItem { Value = "SupplyRoom", Text = "Supply Room" },
                //     new SelectListItem { Value = "EmergencyRoom", Text = "Emergency Room" },
                // };


                var dbContext = new LabDBContext();
                StoreTypeList = dbContext.StoreTypes.ToList();
                var store = dbContext.Stores.Single(s => s.StoreId == HttpContext.Session.GetInt32("StoreId"));
                var managerGroupId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Warehouse Manager")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

                ManagerGroupsList = dbContext.Users
                    .Where(u => u.UserGroupId == managerGroupId)
                    .ToList();

                StoreNumber = store.StoreNumber;
                StoreName = store.StoreName;
                StoreId = store.StoreId;
                ManagerId = store.WarehouseManagerId;

                StoreType = store.StoreType;
                Status = store.WarehouseStatus;

                var manager = dbContext.Users
                    .Where(u => u.UserId == ManagerId)
                    .FirstOrDefault();

                if (manager != null)
                {
                    ManagerJobNumber = manager.JobNumber.ToString();
                }
                else
                {
                    ManagerJobNumber = string.Empty; // or handle as needed
                }
                                
                // ManagerJobNumber = store.ManagerJobNum.ToString();

            }
        }

        public IActionResult OnPost([FromForm] int StoreId, [FromForm] string StoreNumber, [FromForm] string StoreName, [FromForm] string Shelves, [FromForm] string StoreType, [FromForm] int? ManagerId, [FromForm] string ManagerJobNumber, [FromForm] string Status)
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
                     this.StoreType = StoreType;
                    this.ManagerId = ManagerId;
                    this.ManagerJobNumber = ManagerJobNumber;
                    int parsedManagerJobNumber = 0;
                    int.TryParse(ManagerJobNumber, out parsedManagerJobNumber);
                    this.Status = Status;

                    if(string.IsNullOrEmpty(StoreType))
                        ErrorMsg = (Program.Translations["StoreTypeMissing"])[Lang];
                    else if (string.IsNullOrEmpty(StoreNumber))
                        ErrorMsg = (Program.Translations["StoreNumberMissing"])[Lang];
                    else if (string.IsNullOrEmpty(StoreName))
                        ErrorMsg = (Program.Translations["StoreNameMissing"])[Lang];
                    else if (!ManagerId.HasValue)
                        ErrorMsg = (Program.Translations["ManagerNameMissing"])[Lang];
                   
                    else
                    {
                        var dbContext = new LabDBContext();
                        if (dbContext.Stores.Count(s => s.StoreNumber == StoreNumber && s.StoreId != StoreId) > 0)
                            ErrorMsg = string.Format((Program.Translations["StoreNumberExists"])[Lang], StoreNumber);
                        else if (dbContext.Stores.Count(s => s.StoreName == StoreName && s.StoreId != StoreId) > 0)
                            ErrorMsg = string.Format((Program.Translations["StoreNameExists"])[Lang], StoreName);
                        else
                        {   
                             int Active;
                            if (Status == "Closed")
                            {
                                Active = 0;
                            }
                            else
                            {
                                Active = 1;
                            }
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
                            store.StoreType = StoreType;
                            store.WarehouseManagerId = ManagerId;
                            store.WarehouseStatus = Status;
                            store.StoreName = StoreName;
                            store.StoreNumber = StoreNumber;
                            store.IsActive = Active;
                            dbContext.SaveChanges();

                            if (Status == "Closed")
                            {
                                // Update all rooms associated with this store to "Closed"
                                var relatedRooms = dbContext.Rooms.Where(r => r.StoreId == store.StoreId).ToList();
                                foreach (var room in relatedRooms)
                                {
                                    room.RoomStatus = "Closed"; 
                                }

                                dbContext.SaveChanges();
                            }

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
            this.lblWarehouseType = (Program.Translations["WarehouseType"])[Lang];
            this.lblManagerName = (Program.Translations["ManagerName"])[Lang];
            this.lblManagerJobNumber = (Program.Translations["ManagerJobNumber"])[Lang];
            this.lblStatus = (Program.Translations["WarehouseStatus"])[Lang];
            
            this.lblOpen = (Program.Translations["Open"])[Lang];
            this.lblClosed = (Program.Translations["Closed"])[Lang];
        }
    }
}
