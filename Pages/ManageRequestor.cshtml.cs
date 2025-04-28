using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class ManageRequestorModel : BasePageModel
    {
        public List<DestinationsInfo> Destinations { get; set; }
        public List<RequestorInfo> Requestors { get; set; }
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

        public string lblDestinations, lblSearch, lblRequestorName, lblContactNumber, lblDestinationName, lblAddRequestor, lblManageRequestor, lblSubmit, lblAddDestination,
            lblEdit, lblDelete, lblStores;

        public void OnPostSearch([FromForm] string DestinationName)
        {   CurrentPage = 1;
            this.DestinationName = DestinationName;
            FillData(DestinationName, CurrentPage);
        }

        public void OnPostDelete([FromForm] int ReqId)
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                var dbContext = new LabDBContext();

                /*var itemsInstore = dbContext.Destinations.Single(s => s.DId == DestinationId);*/
                /*if (itemsInstore.AvailableQuantity == 0)
                {*/
                var req = dbContext.Requesters.Single(s => s.ReqId == ReqId);
                req.Ended = DateTime.Now;
                dbContext.Requesters.Update(req);
                dbContext.SaveChanges();
                dbContext.Requesters.OrderBy(d => d.ReqId).ToList();
                dbContext.SaveChanges();

                FillData(null);
                Message = string.Format((Program.Translations["RequesterDeleted"])[Lang], req.DestinationName);
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

        public IActionResult OnPostEdit([FromForm] int ReqId)
        {
            HttpContext.Session.SetInt32("ReqId", ReqId);

            return RedirectToPage("./EditRequestor");
        }

        private void FillData(string? DestinationName, int page = 1)
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
                var query = from req in dbContext.Requesters
                            where req.Ended == null
                            select new RequestorInfo
                            {
                                DestinationName = req.DestinationName,
                                RequestorName = req.ReqName,
                                ContactNo = req.ContactNo,
                                ReqId = req.ReqId,
                            };
                if (string.IsNullOrEmpty(DestinationName) == false)
                    query = query.Where(s => s.RequestorName.Contains(DestinationName)||
                                        s.DestinationName.Contains(DestinationName) ||
                                        s.ContactNo.Contains(DestinationName) ||
                                        s.ReqId.ToString().Contains(DestinationName));


                TotalItems = query.Count();
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
                var list = query.ToList();
                Requestors = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();        
                CurrentPage = page;   

                /*Storages = query.ToList();
                TotalItems = Requestors.Count();*/
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }


        private void FillLables()
        {


            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblDestinationName = (Program.Translations["DestinationName"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblAddDestination = (Program.Translations["AddDestination"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblManageRequestor = (Program.Translations["ManageRequestor"])[Lang]; 
            this.lblAddRequestor = (Program.Translations["AddRequestor"])[Lang];
            this.lblRequestorName = (Program.Translations["RequestorName"])[Lang];
            this.lblContactNumber = (Program.Translations["ContactNumber"])[Lang];
            this.lblDestinations = (Program.Translations["Destinations"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];

        }
    }
}
