using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class ManageStorageModel : BasePageModel
    {
        public List<StorageInfo> Storages { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }
        [BindProperty]
        public string StoreNumber { get; set; }
        [BindProperty]
        public string StoreName { get; set; }
        [BindProperty]
        public string Item { get; set; }
        
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public void OnGet(string? StoreNumber, string? StoreName,string? Item , int page = 1) 
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
                if (HttpContext.Request.Query.ContainsKey("page")){
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.StoreNumber = StoreNumber;
                    this.StoreName = StoreName;
                    this.Item = Item;
                    FillData(StoreNumber, StoreName, Item, page);

                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
        
        public string lblStorage, lblSearch, lblStoreNumber, lblStoreName, lblItemName, lblSubmit, lblAddStorage,
            lblShelveNumber, lblAvailableQuantity, lblEdit, lblDelete, lblTotalItem, lblStores;

        public void OnPostSearch([FromForm] string StoreNumber, [FromForm] string StoreName, [FromForm] string Item)
        {   CurrentPage = 1;
            this.StoreNumber = StoreNumber;
            this.StoreName = StoreName;
            this.Item = Item;
            FillData(StoreNumber, StoreName, Item, CurrentPage);
        }

        public void OnPostDelete([FromForm] int StorageId)
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                var dbContext = new LabDBContext();

                var itemsInstore = dbContext.Storages.Single(s => s.StorageId == StorageId);
                if (itemsInstore.AvailableQuantity == 0)
                {
                    var storage = dbContext.Storages.Single(s => s.StorageId == StorageId);
                    dbContext.Storages.Remove(storage);
                    dbContext.SaveChanges();
                    FillData(null, null, null);
                    Message = string.Format((Program.Translations["StorageDeleted"])[Lang], storage.Item.ItemName);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
                }
                else
                {
                    Message = string.Format((Program.Translations["StorageNotDeleted"])[Lang], itemsInstore.Item.ItemName,
                        itemsInstore.Store.StoreName);
                    FillData(null, null, null);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPostEdit([FromForm] int StorageId, [FromForm] int page)
        {
            HttpContext.Session.SetInt32("StorageId", StorageId);
            HttpContext.Session.SetInt32("page", page);

            return RedirectToPage("./EditStorage");
        }

        private void FillData(string? StoreNumber, string? StoreName, string? Item, int page = 1)
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
                var query = from st in dbContext.Storages
                            join i in dbContext.Items on st.ItemId equals i.ItemId
                            join s in dbContext.Stores on st.StoreId equals s.StoreId
                            select new StorageInfo
                            {
                                StoreName = s.StoreName,
                                ItemName = i.ItemName,
                                ShelfNumber = st.ShelfNumber,
                                AvailableQuantity = st.AvailableQuantity.ToString() + " " + i.Unit.UnitCode,
                                StoreNumber = s.StoreNumber,
                                StorageId = st.StorageId
                            };

                if (string.IsNullOrEmpty(StoreNumber) == false)
                    query = query.Where(s => s.StoreNumber.Contains(StoreNumber));

                if (string.IsNullOrEmpty(StoreName) == false)
                    query = query.Where(s => s.StoreName.Contains(StoreName));
                if (string.IsNullOrEmpty(Item) == false)
                    query = query.Where(s => s.ItemName.Contains(Item));

               
                TotalItems = query.Count();
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
                var list = query.ToList();
                Storages = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();        
                CurrentPage = page;   
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }


        private void FillLables()
        {
            

            this.lblStorage = (Program.Translations["Storages"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblStoreNumber = (Program.Translations["StoreNumber"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblAddStorage = (Program.Translations["AddStorage"])[Lang];
            this.lblShelveNumber = (Program.Translations["ShelveNumber"])[Lang];
            this.lblAvailableQuantity = (Program.Translations["AvailableQuantity"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];
        }
    }
}