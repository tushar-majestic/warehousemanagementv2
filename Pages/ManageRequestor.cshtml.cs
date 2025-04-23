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
        public void OnGet() 
        {
            base.ExtractSessionData();
            if (CanManageSupplies)
            {
                FillLables();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public string lblDestinations, lblSearch, lblRequestorName, lblContactNumber, lblDestinationName, lblAddRequestor, lblManageRequestor, lblSubmit, lblAddDestination,
            lblEdit, lblDelete;

        public void OnPostSearch([FromForm] string DestinationName)
        {
            FillData(DestinationName);
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

        private void FillData(string? DestinationName)
        {
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
                    query = query.Where(s => s.RequestorName.Contains(DestinationName));

                Requestors = query.ToList();

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

        }
    }
}
