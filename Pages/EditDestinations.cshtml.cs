using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class EditDestinationsModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public int DId, StoreId, ItemId, Quantity;
        public string ShelfNumber, DestinationName, DestinationSearch;

        public List<Destination> Destinations { get; set; }
        public List<Item> Items { get; set; }
        public int page { get; set; }
        public string lblItemName, lblDestinationName, lblUpdateDestination, lblShelveNumber, lblQuantity, lblUpdate, lblCancel, lblDestinations, lblStores;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            this.page = (int)HttpContext.Session.GetInt32("page");
            this.DestinationSearch = HttpContext.Session.GetString("DestinationName");
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            var dbContext = new LabDBContext();
            var destination = dbContext.Destinations.Single(st => st.DId == HttpContext.Session.GetInt32("DId"));
            if (destination != null)
            {
                this.DId = destination.DId;
                ViewData["DId"] = this.DId;
                DestinationName = destination.DestinationName;

                Destinations = dbContext.Destinations.ToList();
            }
        }

        public IActionResult OnPost([FromForm] int DId,[FromForm] string DestinationName)
        {
            LogableTask task = LogableTask.NewTask("UpdateDestination");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageStore)
                {
                    FillLables();
                    var dbContext = new LabDBContext();

                    Destinations = dbContext.Destinations.ToList();

                    var destination = dbContext.Destinations.Single(s => s.DId == DId);

                    destination.DId = DId;
                    destination.DestinationName = DestinationName;

                    dbContext.SaveChanges();
                    task.LogInfo(MethodBase.GetCurrentMethod(), "destination Updated");

                    string Message = string.Format("Destination agains {0} has been updated", destination.DId);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Update",
                        Helper.ExtractIP(Request), dbContext, true);

                    return RedirectToPage("./ManageDestinations");


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
            this.lblDestinations = (Program.Translations["Destinations"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];
        }
    }
}
