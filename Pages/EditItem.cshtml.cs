using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class EditItemModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string ItemCode, ItemName, GroupCode, ItemTypeCode, HazardTypeName, ItemDescription, BatchNo, ItemNameSearch, GroupSearch;
        public DateTime ExpiryDate;
        public bool IsHazardous;
        public int AvailableQuantity, UnitId, ItemID;
        public List<ItemGroup> ItemGroups { get; set; }
        public List<ItemType> ItemTypes { get; set; }
        public List<HazardType> HazardTypes { get; set; }
        public List<Unit> UnitTypes { get; set; }
        public string FromDate, ToDate;
        public int page { get; set; }
        
        public string lblItemName, lblGroupName, lblItemCode, lblItemDescription, lblAvailableQuantity, lblHazardType, lblTypeName,
            lblUnit, lblUpdateItem, lblUpdate, lblCancel, lblIsHazardous, lblItemType, lblBatchNo, lblExpiryDate, lblItems;

        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            this.page = (int)HttpContext.Session.GetInt32("page");
            this.FromDate = HttpContext.Session.GetString("FromDate");
            this.ToDate = HttpContext.Session.GetString("ToDate");
            this.ItemNameSearch = HttpContext.Session.GetString("ItemName");
            this.GroupSearch = HttpContext.Session.GetString("Group");
            if (CanManageItems == false)
                RedirectToPage("./Index?lang=" + Lang);
            else
            {
                using (var dbContext = new LabDBContext())
                {
                    ItemGroups = dbContext.ItemGroups.Where(g => g.Units.Count() > 0).ToList();
                    ItemTypes = dbContext.ItemTypes.ToList();
                    HazardTypes = dbContext.HazardTypes.ToList();
                    UnitTypes = dbContext.Units.ToList();
                    
                    var item = dbContext.Items.Single(i => i.ItemId == HttpContext.Session.GetInt32("ItemId"));

                    ItemID = item.ItemId;
                    ItemCode = item.ItemCode; 
                    ItemName = item.ItemName;
                    GroupCode = item.GroupCode;
                    ItemTypeCode = item.ItemTypeCode;
                    HazardTypeName = item.HazardTypeName;
                    IsHazardous = item.IsHazardous;
                    AvailableQuantity = item.AvailableQuantity;
                    UnitId = item.UnitId;
                    ItemDescription = item.ItemDescription;
                    BatchNo = item.BatchNo;
                    ExpiryDate = (DateTime)item.ExpiryDate;
                }
               
            }
        }

        public IActionResult OnPost([FromForm] int ItemId, [FromForm] string ItemCode, [FromForm] string ItemName, [FromForm] string GroupCode,
            [FromForm] string ItemTypeCode, [FromForm] bool IsHazardous, [FromForm] string HazardTypeName,
            [FromForm] int UnitId, [FromForm] int AvailableQuantity, [FromForm] string ItemDescription,
            [FromForm] string BatchNo, [FromForm] DateTime ExpiryDate)
        {
            LogableTask task = LogableTask.NewTask("EditSupplier");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageItems)
                {
                    FillLables();
                    this.ItemID = ItemId;
                    this.ItemCode = ItemCode;
                    this.ItemName = ItemName;
                    this.IsHazardous = IsHazardous;
                    this.GroupCode = GroupCode;
                    this.ItemTypeCode = ItemTypeCode;
                    this.HazardTypeName = HazardTypeName;
                    this.UnitId = UnitId;
                    this.AvailableQuantity = AvailableQuantity;
                    this.ItemDescription = ItemDescription;
                    this.BatchNo = BatchNo;
                    this.ExpiryDate = ExpiryDate;
                    var dbContext = new LabDBContext();

                    ItemGroups = dbContext.ItemGroups.Where(g => g.Units.Count() > 0).ToList();
                    ItemTypes = dbContext.ItemTypes.ToList();
                    HazardTypes = dbContext.HazardTypes.ToList();
                    UnitTypes = dbContext.Units.ToList();

                    if (string.IsNullOrEmpty(ItemName))
                        ErrorMsg = (Program.Translations["ItemNameMissing"])[Lang];
                    else if(string.IsNullOrEmpty(ItemCode))
                        ErrorMsg = (Program.Translations["ItemCodeMissing"])[Lang];
                    else
                    {
                        var item = dbContext.Items.Single(i => i.ItemId == ItemID);
                        if (dbContext.Items.Count(s => s.ItemName == ItemName && s.ItemId != item.ItemId) > 0)
                            ErrorMsg = string.Format((Program.Translations["ItemNameExists"])[Lang], ItemName);
                        else if (dbContext.Items.Count(s => s.ItemCode == ItemCode && s.ItemId != item.ItemId) > 0)
                            ErrorMsg = string.Format((Program.Translations["ItemCodeExists"])[Lang], ItemCode);
                        else
                        {       
                            item.ItemCode = ItemCode;
                            item.ItemName = ItemName;
                            item.IsHazardous = IsHazardous;
                            item.HazardTypeName = IsHazardous ? HazardTypeName : "NonHazarduos";
                            item.GroupCode = GroupCode;
                            item.ItemTypeCode = ItemTypeCode;
                            item.UnitId = UnitId;
                            item.AvailableQuantity = AvailableQuantity;
                            item.ItemDescription = ItemDescription;
                            item.BatchNo = BatchNo;
                            item.ExpiryDate = ExpiryDate;
                            dbContext.SaveChanges();

                            string Message = string.Format("Item {0} updated", item.ItemName);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Update",
                                Helper.ExtractIP(Request), dbContext, true);

                            task.LogInfo(MethodBase.GetCurrentMethod(), "Item updated");
                            return RedirectToPage("./ManageItems");
                        }
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
            

            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblItemDescription = (Program.Translations["ItemDescription"])[Lang];
            this.lblAvailableQuantity = (Program.Translations["AvailableQuantity"])[Lang];
            this.lblHazardType = (Program.Translations["HazardType"])[Lang];
            this.lblTypeName = (Program.Translations["TypeName"])[Lang];
            this.lblUnit = (Program.Translations["Units"])[Lang];
            this.lblUpdate = (Program.Translations["Update"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblUpdateItem = (Program.Translations["UpdateItem"])[Lang];
            this.lblIsHazardous = (Program.Translations["IsHazardous"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];

        }
    }
}
