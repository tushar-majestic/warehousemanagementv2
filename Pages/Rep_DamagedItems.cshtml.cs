using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class Rep_DamagedItemsModel : BasePageModel
    {
        public DateTime? FromDate, ToDate;
        public List<DamagedItemsInfo> DamagedItems;
        public string lblHazardousMaterials, lblHazardTypeName, lblSearch, lblSubmit, lblItemCode, lblItemName, lblGroupName,
            lblAvailableQuantity, lblHazardType, lblTypeName, lblStoreName, lblUnitCode, lblTotalItem,
            lblMaterialsReceived, lblInventory, lblUserActivity, lblDistributedMaterials, lblDamagedItems, lblUserReport,
            lblFromDate, lblToDate, lblDamageditems, lblTotalDamagedItem,
            lblDamageDate, lblDamageQuantity, lblDamageReason, lblExport;
        public int TotalItems { get; set; }

        [BindProperty]
        public string ItemName { get; set; }
        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                this.FromDate = DateTime.Today;
                this.ToDate = DateTime.Today;
                FillLables();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
        public void FillData(string ItemNAme, DateTime? StartDate, DateTime? EndDate)
        {
            var dbContext = new LabDBContext();
            var query = (from i in dbContext.Items
                         join di in dbContext.DamagedItems on i.ItemId equals di.ItemId

                         select new DamagedItemsInfo
                         {
                             ItemCode = i.ItemCode,
                             ItemName = i.ItemName,
                             DamageQuantity = di.DamagedQuantity,
                             DamageDate = di.DamagedDate,
                             DamageReason = di.DamagedReason

                         });

            if(ItemNAme != null)
            {
                query = query.Where(e => e.ItemName.Contains(ItemNAme));
            }

            if(StartDate is not null && EndDate is not null )
            {
                query = query.Where(e => e.DamageDate.Value.Date >= StartDate && e.DamageDate.Value.Date <= EndDate);
            }
            DamagedItems = query.ToList();
            TotalItems = DamagedItems.Count();

            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
            base.ExtractSessionData();
            FillLables();
        }
        public void OnPost([FromForm] string ItemName, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        {
            base.ExtractSessionData();
            this.ItemName = ItemName;
            if (CanSeeReports)
            {
                FillData(ItemName, FromDate, ToDate);
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
        private void FillLables()
        {

            this.lblMaterialsReceived = (Program.Translations["MaterialsReceived"])[Lang];
            this.lblInventory = (Program.Translations["Inventory"])[Lang];
            this.lblHazardousMaterials = (Program.Translations["HazardousMaterials"])[Lang];
            this.lblUserActivity = (Program.Translations["UserActivity"])[Lang];
            this.lblDistributedMaterials = (Program.Translations["DistributedMaterials"])[Lang];
            this.lblHazardTypeName = (Program.Translations["HazardTypeName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblAvailableQuantity = (Program.Translations["AvailableQuantity"])[Lang];
            this.lblHazardType = (Program.Translations["HazardType"])[Lang];
            this.lblTypeName = (Program.Translations["TypeName"])[Lang];
            this.lblUnitCode = (Program.Translations["UnitCode"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblUserReport = (Program.Translations["UserReport"])[Lang];
            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
            this.lblDamageditems = (Program.Translations["DamagedItems"])[Lang];
            this.lblTotalDamagedItem = (Program.Translations["TotalDamagedItem"])[Lang];
            this.lblDamageDate = (Program.Translations["DamageDate"])[Lang];
            this.lblDamageQuantity = (Program.Translations["DamageQuantity"])[Lang];
            this.lblDamageReason = (Program.Translations["DamageReason"])[Lang];
            this.lblExport = (Program.Translations["Export"])[Lang];
        }
    }
}
