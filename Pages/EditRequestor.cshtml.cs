using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class EditRequestorModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public int StoreId, ItemId, Quantity, ReqId;
        public int? DId;
        public string ShelfNumber, DestinationName, RequestorName, ContactNo, DestinationNameSearch;

        public List<Destination> Destinations { get; set; }
        public List<Requester> Requestors { get; set; }
        public List<Item> Items { get; set; }
        public int page { get; set; }
        public string lblItemName, lblDestinationName, lblUpdateDestination, lblUpdateRequestor, lblContactNumber, 
        lblRequestorName, lblShelveNumber, lblQuantity, lblUpdate, lblCancel, lblStores, lblDestinations, lblManageRequestor;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            this.page = (int)HttpContext.Session.GetInt32("page");
            this.DestinationNameSearch = HttpContext.Session.GetString("DestinationName");
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            var dbContext = new LabDBContext();
            var req = dbContext.Requesters.Single(st => st.ReqId == HttpContext.Session.GetInt32("ReqId"));
            if (req != null)
            {
                this.ReqId = req.ReqId;
                DId = req.DestinationId;
                ViewData["ReqId"] = this.ReqId;
                RequestorName = req.ReqName;
                DestinationName = req.DestinationName;
                ContactNo = req.ContactNo;

                Requestors = dbContext.Requesters.ToList();
                Destinations = dbContext.Destinations.ToList();
            }
        }

        public IActionResult OnPost(string RequestorName, string ContactNumber, int ReqId, int DId)
        {
            LogableTask task = LogableTask.NewTask("UpdateRequestor");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageStore)
                {
                    FillLables();
                    var dbContext = new LabDBContext();

                    Requestors = dbContext.Requesters.ToList();
                    var dest = dbContext.Destinations.FirstOrDefault(d => d.DId == DId);
                    var req = dbContext.Requesters.Single(s => s.ReqId == ReqId);

                    req.ReqId = ReqId;
                    req.DestinationName = dest.DestinationName;
                    req.ReqName = RequestorName;
                    req.ContactNo = ContactNumber;

                    dbContext.SaveChanges();
                    task.LogInfo(MethodBase.GetCurrentMethod(), "destination Updated");

                    string Message = string.Format("Destination agains {0} has been updated", req.ReqId);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Update",
                        Helper.ExtractIP(Request), dbContext, true);

                    return RedirectToPage("./ManageRequestor");


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


            this.lblUpdateDestination = (Program.Translations["UpdateDestination"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblDestinationName = (Program.Translations["DestinationName"])[Lang];
            this.lblShelveNumber = (Program.Translations["ShelveNumber"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblUpdate = (Program.Translations["Update"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblContactNumber = (Program.Translations["ContactNumber"])[Lang];
            this.lblRequestorName = (Program.Translations["RequestorName"])[Lang]; 
            this.lblUpdateRequestor = (Program.Translations["UpdateRequestor"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];
            this.lblDestinations = (Program.Translations["Destinations"])[Lang];
            this.lblManageRequestor = (Program.Translations["ManageRequestor"])[Lang];


        }
    }
}
