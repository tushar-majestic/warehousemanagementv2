using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class ManageDestinationsModel : BasePageModel
    {
        public List<DestinationsInfo> Destinations { get; set; }
        public string Message { get; set; }
        [BindProperty]
        public int TotalItems { get; set; }
        [BindProperty]
        public string DestinationName { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
      
        public void OnGet(string? DestinationName, int page = 1) 
        {
            base.ExtractSessionData();
            if (CanManageSupplies)
            {
                FillLables();
                if (HttpContext.Request.Query.ContainsKey("page")){
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.DestinationName = DestinationName;
                    FillData(DestinationName, page);

                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public string lblDestinations, lblSearch, lblDestinationName, lblManageRequestor, lblSubmit, lblAddDestination,
            lblEdit, lblDelete, lblStores;

        public void OnPostSearch([FromForm] string DestinationName)
        {   CurrentPage = 1;
            this.DestinationName = DestinationName;
            FillData(DestinationName, CurrentPage);
        }

        public void OnPostDelete([FromForm] int DestinationId)
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                var dbContext = new LabDBContext();

                /*var itemsInstore = dbContext.Destinations.Single(s => s.DId == DestinationId);*/
                /*if (itemsInstore.AvailableQuantity == 0)
                {*/
                var destination = dbContext.Destinations.Single(s => s.DId == DestinationId);
                dbContext.Destinations.Remove(destination);
                dbContext.SaveChanges();
                dbContext.Destinations.OrderBy(d => d.DId).ToList();
                dbContext.SaveChanges();

                FillData(null);
                Message = string.Format((Program.Translations["DestinationDeleted"])[Lang], destination.DestinationName);
                Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
                /*}
                else
                {
                    Message = string.Format((Program.Translations["StorageNotDeleted"])[Lang], itemsInstore.Item.ItemName,
                        itemsInstore.Store.StoreName);
                    FillData(null);
                }*/
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPostEdit([FromForm] int DestinationId)
        {
            HttpContext.Session.SetInt32("DId", DestinationId);

            return RedirectToPage("./EditDestinations");
        }

        private void FillData(string? DestinationName, int page = 1)
        {   
            if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = from dst in dbContext.Destinations
                            select new DestinationsInfo
                            {
                                DestinationName = dst.DestinationName,
                                DestinationId = dst.DId
                            };

                if (string.IsNullOrEmpty(DestinationName) == false)
                    query = query.Where(s => s.DestinationName.Contains(DestinationName));

                TotalItems = query.Count();
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
                var list = query.ToList();
                Destinations = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();        
                CurrentPage = page;   

                /*Storages = query.ToList();
                TotalItems = Storages.Count();*/
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }


        private void FillLables()
        {

            this.lblDestinations = (Program.Translations["Destinations"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblDestinationName = (Program.Translations["DestinationName"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblAddDestination = (Program.Translations["AddDestination"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang]; 
            this.lblManageRequestor = (Program.Translations["ManageRequestor"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];

        }
    }
}
