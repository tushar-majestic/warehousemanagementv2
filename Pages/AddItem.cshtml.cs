using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.Security.Cryptography;

namespace LabMaterials.Pages
{
    public class AddItemModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string ItemCode, ItemName, ItemNameAr, GroupCode, ItemTypeCode, HazardTypeName, ItemDescription, BatchNo;
        public bool IsHazardous;
        public DateTime ExpiryDate;
        public int AvailableQuantity, UnitId;
        public List<ItemGroup> ItemGroups { get; set; }
        public List<ItemType> ItemTypes { get; set; }
        public List<HazardType> HazardTypes { get; set; }
        public List<Unit> UnitTypes { get; set; }

        public string lblItemName, lblGroupName, lblItemCode, lblItemDescription, lblAvailableQuantity, lblHazardType, lblTypeName,
            lblUnit, lblAddItem, lblAdd, lblCancel, lblIsHazardous, lblExpiryDate, lblBatchNo, lblItems, lblEnglishLanguage, lblArabicLanguage,
            lblChemical, lblRiskRating, lblStateofMatter;

        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            if (CanManageItems == false)
                RedirectToPage("./Index?lang=" + Lang);
            using (var dbContext = new LabDBContext())
            {
                ItemGroups = dbContext.ItemGroups.Where(g => g.Units.Count() > 0).ToList();
                ItemTypes = dbContext.ItemTypes.ToList();
                HazardTypes = dbContext.HazardTypes.ToList();
                UnitTypes = dbContext.Units.ToList();
                ExpiryDate = DateTime.Today;
                //UnitTypes = dbContext.Units.Where(U => U.GroupCode == ItemGroups[0].GroupCode).ToList();
            }

        }

        public List<Unit> GetUnits(string GroupCode)
        {
            using (var dbContext = new LabDBContext())
            {
                UnitTypes = dbContext.Units.Where(U => U.GroupCode == GroupCode).ToList();
            }
            return UnitTypes;
        }

        public IActionResult OnPost([FromForm] string ItemCode, [FromForm] string ItemName, [FromForm] string ItemNameAr, [FromForm] string GroupCode,
            [FromForm] string ItemTypeCode, [FromForm] bool IsHazardous, [FromForm] string HazardTypeName,
            [FromForm] int UnitId, [FromForm] int? AvailableQuantity, [FromForm] string ItemDescription, [FromForm] string BatchNo, [FromForm] DateTime ExpiryDate)
        {
            LogableTask task = LogableTask.NewTask("AddStore");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageItems)
                {
                    FillLables();
                    this.ItemCode = ItemCode;
                    this.ItemName = ItemName;
                    this.ItemNameAr = ItemNameAr;
                    this.IsHazardous = IsHazardous;
                    this.GroupCode = GroupCode;
                    this.ItemTypeCode = ItemTypeCode;
                    this.HazardTypeName = HazardTypeName;
                    this.UnitId = UnitId;
                    this.AvailableQuantity = AvailableQuantity ?? 0;
                    this.ItemDescription = ItemDescription;
                    this.ExpiryDate = ExpiryDate;
                    this.BatchNo = BatchNo;

                    var dbContext = new LabDBContext();
                    ItemGroups = dbContext.ItemGroups.Where(g => g.Units.Count() > 0).ToList();
                    ItemTypes = dbContext.ItemTypes.ToList();
                    HazardTypes = dbContext.HazardTypes.ToList();
                    UnitTypes = dbContext.Units.ToList();

                    if (string.IsNullOrEmpty(ItemCode))
                        ErrorMsg = (Program.Translations["ItemCodeMissing"])[Lang];
                    else if (string.IsNullOrEmpty(ItemName))
                        ErrorMsg = (Program.Translations["ItemNameMissing"])[Lang];
                    else if (string.IsNullOrEmpty(ItemNameAr))
                        ErrorMsg = (Program.Translations["ItemNameMissing"])[Lang];
                    else
                    {
                        if (dbContext.Items.Count(s => s.ItemCode == ItemCode) > 0)
                            ErrorMsg = string.Format((Program.Translations["ItemCodeExists"])[Lang], ItemCode);
                        else if (dbContext.Items.Count(s => s.ItemName == ItemName) > 0)
                            ErrorMsg = string.Format((Program.Translations["ItemNameExists"])[Lang], ItemName);
                        else
                        {
                            var item = new Item
                            {
                                ItemId = PrimaryKeyManager.GetNextId(),
                                ItemCode = ItemCode,
                                ItemName = ItemName,
                                ItemNameAr = ItemNameAr,
                                IsHazardous = IsHazardous,
                                Chemical = IsHazardous,
                                HazardTypeName = IsHazardous ? HazardTypeName : "NonHazarduos",
                                RiskRating = IsHazardous ? HazardTypeName : "NonHazarduos",
                                GroupCode = GroupCode,
                                ItemTypeCode = ItemTypeCode,
                                StateofMatter = ItemTypeCode,
                                UnitId = UnitId,
                                AvailableQuantity = AvailableQuantity ?? 0,
                                ItemDescription = ItemDescription,
                                BatchNo = BatchNo,
                                ExpiryDate = ExpiryDate,
                            };
                            dbContext.Items.Add(item);
                            dbContext.SaveChanges();

                            Color color = new Color();
                            using (var rng = new RNGCryptoServiceProvider())
                            {
                                byte[] bytes = new byte[3];
                                rng.GetBytes(bytes);
                                color = Color.FromArgb(bytes[0], bytes[1], bytes[2]);
                            }
                            string colorCode = ColorTranslator.ToHtml(color);

                            var ColorCode = new ColorCode
                            {
                                ColorCode1 = colorCode,
                                ItemId = item.ItemId,
                                ItemCode = ItemCode
                            };

                            dbContext.ColorCodes.Add(ColorCode);
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "Item added");

                            string Message = string.Format("Item {0} added", item.ItemName);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                                Helper.ExtractIP(Request), dbContext, true);

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
            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblAddItem = (Program.Translations["AddItem"])[Lang];
            this.lblIsHazardous = (Program.Translations["IsHazardous"])[Lang];
            this.lblBatchNo = (Program.Translations["BatchNo"])[Lang];
            this.lblExpiryDate = (Program.Translations["ExpiryDate"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];
            this.lblArabicLanguage = (Program.Translations["ArabicLanguage"])[Lang];
            this.lblEnglishLanguage = (Program.Translations["EnglishLanguage"])[Lang];
            this.lblChemical = (Program.Translations["Chemical"])[Lang];
            this.lblRiskRating = (Program.Translations["RiskRating"])[Lang];
            this.lblStateofMatter = (Program.Translations["StateofMatter"])[Lang];

        }
    }
}
