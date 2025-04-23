using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class ManageDestinationsModel : BasePageModel
    {
        public List<DestinationsInfo> Destinations { get; set; }
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

        public string lblDestinations, lblSearch, lblDestinationName, lblManageRequestor, lblSubmit, lblAddDestination,
            lblEdit, lblDelete;

        public void OnPostSearch([FromForm] string DestinationName)
        {
            FillData(DestinationName);
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

        private void FillData(string? DestinationName)
        {
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

                Destinations = query.ToList();

                /*Storages = query.ToList();
                TotalItems = Storages.Count();*/
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

        }
    }
}
