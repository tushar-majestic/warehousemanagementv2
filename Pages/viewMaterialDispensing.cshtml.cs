using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabMaterials.dtos;

namespace LabMaterials.Pages
{
    public class viewMaterialDispensingModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public int DId, StoreId, ItemId, Quantity;
        public string ShelfNumber, DestinationName;

        public List<Destination> Destinations { get; set; }
        public List<Item> Items { get; set; }

        public string lblItemName, lblDestinationName, lblUpdateDestination, lblShelveNumber, lblQuantity, lblUpdate, lblCancel, lblDestinations, lblStores;


        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
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
