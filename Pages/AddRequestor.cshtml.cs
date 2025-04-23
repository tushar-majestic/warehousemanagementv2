using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class AddRequestorModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string DestinationName;
        public string RequestorName;
        public int DId;
        public List<Destination> Destinations { get; set; }

        public string lblAddDestination, lblDestinationName, lblContactNumber,lblRequestorName, lblAddRequestor, lblAdd
        , lblCancel, lblStores, lblDestinations, lblManageRequestor;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            var dbContext = new LabDBContext();
            Destinations = dbContext.Destinations.ToList();
        }

        public IActionResult OnPost(string RequestorName, string ContactNumber, int DId)
        {
            LogableTask task = LogableTask.NewTask("AddRequestor");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageStore)
                {
                    FillLables();
                    

                    
                        var dbContext = new LabDBContext();
                        var dest = dbContext.Destinations.FirstOrDefault(x=> x.DId ==  DId);
                        DestinationName = dest.DestinationName;
                        if (string.IsNullOrEmpty(ContactNumber))
                            ErrorMsg = (Program.Translations["ContactNumberMissing"])[Lang];
                        else if (string.IsNullOrEmpty(RequestorName))
                            ErrorMsg = (Program.Translations["RequestorNameMissing"])[Lang];
                        else
                        {
                            var requestor = new Requester
                            {
                                DestinationName = DestinationName,
                                ReqName = RequestorName,
                                ContactNo = ContactNumber,
                                DestinationId = DId
                            };
                            dbContext.Requesters.Add(requestor);
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "destination added");

                            string Message = string.Format("Destination {0} added", requestor.DestinationName);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                                Helper.ExtractIP(Request), dbContext, true);

                            return RedirectToPage("./ManageRequestor");
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


            this.lblAddDestination = (Program.Translations["AddDestination"])[Lang];
            this.lblDestinationName = (Program.Translations["DestinationName"])[Lang];
            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang]; 
            this.lblAddRequestor = (Program.Translations["AddRequestor"])[Lang]; 
            this.lblRequestorName = (Program.Translations["RequestorName"])[Lang]; 
            this.lblContactNumber = (Program.Translations["ContactNumber"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];
            this.lblDestinations = (Program.Translations["Destinations"])[Lang];
             this.lblManageRequestor = (Program.Translations["ManageRequestor"])[Lang]; 



        }
    }
}
