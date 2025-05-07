using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class DamageItemModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string ItemName, DamageReason;
        public string ImageUrl { get; set; }
        public int ItemId { get; set; }
        /*public int ItemId;*/
        public int DamagedQuantity { get; set; }
        public List<DamageInfo> DamagedItem { get; set; }
        public List<Store> Stores { get; set; }
        public DateTime CurrentDate;

        public string lblItemName, lblDamagedQuantity, lblDamageReason, lblAdd, lblCancel, lblDamageItem, lblItems, lblItemCode, lblQuantity, lblStoreName, lblDamagedItems,
        lblManageItemGroups, lblEnglishLanguage, lblArabicLanguage;
        public void OnGet(int id)
        {
            base.ExtractSessionData();
            FillLables();
            CurrentDate = DateTime.Now;
            var dbContext = new LabDBContext();
            Stores = dbContext.Stores.ToList();
            if (CanManageStore == false)
            {
                RedirectToPage("./Index?lang=" + Lang);
            }
            else
            {
                ItemId = id;
                FillData(id);
            }
            
        }

        public IActionResult OnPost([FromForm] int itemId,[FromForm] int DamagedQuantity, [FromForm] string DamageReason)
        {
            LogableTask task = LogableTask.NewTask("DamageItem");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageStore)
                {
                    FillLables();
                    this.DamagedQuantity = DamagedQuantity;
                    this.DamageReason = DamageReason;

                    if (DamagedQuantity <= 0)
                        ErrorMsg = (Program.Translations["StoreNumberMissing"])[Lang];
                    else if (string.IsNullOrEmpty(DamageReason))
                        ErrorMsg = (Program.Translations["StoreNameMissing"])[Lang];
                    else
                    {
                        var dbContext = new LabDBContext();
                        var item = dbContext.Items.FirstOrDefault(item => item.ItemId == itemId);
                        if(item != null)
                        {
                            if (item.AvailableQuantity < DamagedQuantity)
                            {
                                ErrorMsg = (Program.Translations["DamagedQuantityExceeded"])[Lang];
                                return RedirectToPage("./ManageItems");
                            }
                            else
                            {
                                item.AvailableQuantity -= DamagedQuantity;
                                dbContext.SaveChanges();
                            }
                        }
                        
                        /*if (dbContext.DamagedItems.Count(s => s.DamagedQuantity == DamagedQuantity) > 0)
                            ErrorMsg = string.Format((Program.Translations["StoreNumberExists"])[Lang], DamagedQuantity);*/
                        /*else if (dbContext.DamagedItems.Count(s => s.DamagedReason == DamageReason) > 0)
                            ErrorMsg = string.Format((Program.Translations["StoreNameExists"])[Lang], DamageReason);*/
                        
                            var damage = new DamagedItem
                            {
                                ItemId = itemId,
                                DamagedQuantity = DamagedQuantity,
                                DamagedReason = DamageReason
                            };
                            dbContext.DamagedItems.Add(damage);
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "store added");

                            string Message = string.Format("Store added");
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                                Helper.ExtractIP(Request), dbContext, true);

                            return RedirectToPage("./ManageItems");
                        
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

        private void FillData(int ItemId)
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var selectedItem = dbContext.Items.FirstOrDefault(item => item.ItemId == ItemId);
                DamagedItem = new List<DamageInfo>
                {
                new DamageInfo
                {
                    ItemId = selectedItem.ItemId,
                    ItemName = selectedItem.ItemName
                }
                };


            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillLables()
        {


            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblDamagedQuantity = (Program.Translations["DamagedQuantity"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblDamageReason = (Program.Translations["DamageReason"])[Lang];
            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];
            this.lblDamageItem = (Program.Translations["DamageItem"])[Lang];
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblManageItemGroups = (Program.Translations["ManageItemGroups"])[Lang];
            this.lblArabicLanguage = (Program.Translations["ArabicLanguage"])[Lang];
            this.lblEnglishLanguage = (Program.Translations["EnglishLanguage"])[Lang];
        }
    }
}
