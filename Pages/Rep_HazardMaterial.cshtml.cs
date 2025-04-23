using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class Rep_HazardMaterialModel : BasePageModel
    {
        public List<ItemInfo> Items { get; set; }
        public int TotalItems { get; set; }
        public List<HazardType> HazardTypes { get; set; }
        public string HazardTypeName;
        
        public string lblHazardousMaterials, lblHazardTypeName, lblSearch, lblSubmit, lblItemCode, lblItemName, lblGroupName, 
            lblAvailableQuantity, lblHazardType, lblTypeName, lblStoreName, lblUnitCode, lblTotalItem,
            lblMaterialsReceived, lblInventory, lblUserActivity, lblDistributedMaterials, lblDamagedItems,
            lblUserReport, lblExport, lblSelectHazardType;
        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                var dbContext = new LabDBContext();
                HazardTypes = dbContext.HazardTypes.Where(h => h.HazardTypeName != "NonHazarduos").ToList();
                HazardTypeName = null;
                FillLables();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillData(string ItemName, string HazardTypeName)
        {
            if (CanSeeReports)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = (from i in dbContext.Items
                             join g in dbContext.ItemGroups on i.GroupCode equals g.GroupCode
                             join t in dbContext.ItemTypes on i.ItemTypeCode equals t.ItemTypeCode
                             join u in dbContext.Units on i.UnitId equals u.Id
                             join st in dbContext.Storages on i.ItemId equals st.ItemId
                             join s in dbContext.Stores on st.StoreId equals s.StoreId

                             select new ItemInfo
                             {
                                 AvailableQuantity = i.AvailableQuantity,
                                 GroupCode = g.GroupCode,
                                 GroupDesc = g.GroupDesc,
                                 HazardTypeName = i.HazardTypeName,
                                 IsHazardous = i.IsHazardous,
                                 ItemCode = i.ItemCode,
                                 ItemId = i.ItemId,
                                 ItemName = i.ItemName,
                                 StoreName = s.StoreName,
                                 ItemTypeCode = t.ItemTypeCode,
                                 TypeName = t.TypeName,
                                 UnitCode = u.UnitCode,
                                 UnitDesc = u.UnitDesc,
                             });

                query = query.Where(i => i.HazardTypeName.ToLower() != "NonHazarduos");

                if (string.IsNullOrEmpty(ItemName) == false)
                    query = query.Where(i => i.ItemName.Contains(ItemName));

                if (string.IsNullOrEmpty(HazardTypeName) == false)
                    query = query.Where(i => i.HazardTypeName.Contains(HazardTypeName));

                Items = query.ToList();
                TotalItems = Items.Count();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void OnPost([FromForm] string ItemName, [FromForm] string HazardTypeName)
        {
            base.ExtractSessionData();
            var dbContext = new LabDBContext();
            HazardTypes = dbContext.HazardTypes.Where(h => h.HazardTypeName != "NonHazarduos").ToList();
            this.HazardTypeName = HazardTypeName;
            if (CanSeeReports)
            {
                FillData(ItemName, HazardTypeName);
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
            this.lblExport = (Program.Translations["Export"])[Lang];
            this.lblSelectHazardType = (Program.Translations["SelectHazardType"])[Lang];
        }
    }
}
