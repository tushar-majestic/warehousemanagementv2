using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class ManageDamagedModel : BasePageModel
    {
        public List<ItemInfo> Items { get; set; }
        public int TotalItems { get; set; }
        public string Message { get; set; }

        public string lblItems, lblItemName, lblGroupName, lblItemCode, lblAvailableQuantity, lblHazardType, lblTypeName,
            lblUnitCode, lblSearch, lblSubmit, lblDamageReason, lblDamagedQuantity, lblManageItemGroup, lblManageUnit, lblAddItem, lblEdit, lblDelete, lblTotalItem, lblExpiryDate, lblBatchNo, lblDamage, lblDamagedItems;
        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageItems)
            {
                FillLables();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void OnPost([FromForm] string ItemName, [FromForm] string Group)
        {
            base.ExtractSessionData();
            if (CanManageItems)
            {
                FillData(ItemName, Group);
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillData(string ItemName, string Group)
        {
            FillLables();
            var dbContext = new LabDBContext();
            var query = (from i in dbContext.Items
                         join g in dbContext.ItemGroups on i.GroupCode equals g.GroupCode
                         join t in dbContext.ItemTypes on i.ItemTypeCode equals t.ItemTypeCode
                         join u in dbContext.Units on i.UnitId equals u.Id
                         join d in dbContext.DamagedItems on i.ItemId equals d.ItemId

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
                             ItemTypeCode = t.ItemTypeCode,
                             TypeName = t.TypeName,
                             UnitCode = u.UnitCode,
                             UnitDesc = u.UnitDesc,
                             BatchNo = i.BatchNo,
                             ExpiryDate = i.ExpiryDate,
                             DamagedQuantity = (int)d.DamagedQuantity,
                             DamageReason = d.DamagedReason,
                         });

            if (string.IsNullOrEmpty(ItemName) == false)
                query = query.Where(i => i.ItemName.Contains(ItemName));

            if (string.IsNullOrEmpty(Group) == false)
                query = query.Where(i => i.GroupDesc.Contains(Group));

            Items = query.ToList();
            TotalItems = Items.Count();
        }

        public IActionResult OnPostEdit([FromForm] int ItemId)
        {
            HttpContext.Session.SetInt32("ItemId", ItemId);

            return RedirectToPage("./EditItem");
        }

        public IActionResult OnPostDamage([FromForm] int ItemId)
        {
            HttpContext.Session.SetInt32("ItemId", ItemId);

            return RedirectToPage("./DamageItem");
        }

        public void OnPostDelete([FromForm] int ItemId)
        {
            base.ExtractSessionData();
            if (CanManageItems)
            {
                var dbContext = new LabDBContext();

                if (dbContext.Supplies.Count(s => s.ItemId == ItemId) == 0)
                {
                    var Item = dbContext.Items.Single(s => s.ItemId == ItemId);
                    dbContext.Items.Remove(Item);
                    dbContext.SaveChanges();
                    FillData(null, null);
                    Message = string.Format((Program.Translations["ItemDeleted"])[Lang], Item.ItemName);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
                }
                else
                {
                    Message = (Program.Translations["ItemNotDeleted"])[Lang];
                    FillData(null, null);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillLables()
        {


            this.lblItems = (Program.Translations["Items"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblAvailableQuantity = (Program.Translations["AvailableQuantity"])[Lang];
            this.lblHazardType = (Program.Translations["HazardType"])[Lang];
            this.lblTypeName = (Program.Translations["TypeName"])[Lang];
            this.lblUnitCode = (Program.Translations["UnitCode"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblManageItemGroup = (Program.Translations["ManageItemGroups"])[Lang];
            this.lblManageUnit = (Program.Translations["ManageUnits"])[Lang];
            this.lblAddItem = (Program.Translations["AddItem"])[Lang];
            this.lblExpiryDate = (Program.Translations["ExpiryDate"])[Lang];
            this.lblBatchNo = (Program.Translations["BatchNo"])[Lang];
            this.lblDamage = (Program.Translations["Damage"])[Lang];
            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblDamagedQuantity = (Program.Translations["DamagedQuantity"])[Lang];
            this.lblDamageReason = (Program.Translations["DamageReason"])[Lang]; 
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];



        }
    }
}
