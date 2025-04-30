using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client.Extensions.Msal;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions;

namespace LabMaterials.Pages
{
    public class ManageItemsModel : BasePageModel
    {
        public List<ItemInfo> Items { get; set; }
        public int TotalItems { get; set; }
        public string Message { get; set; }
        public DateTime? FromDate, ToDate;
        [BindProperty]
        public string ItemName { get; set; }
        [BindProperty]
        public string Group { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }


        public string lblItems, lblItemName, lblGroupName, lblItemCode, lblQuantity, lblHazardType, lblTypeName,
            lblUnitCode, lblSearch, lblSubmit, lblManageItemGroup, lblManageUnit, lblAddItem, lblEdit,
            lblDelete, lblTotalItem, lblExpiryDate, lblBatchNo, lblDamage, lblDamagedItems,
            lblImport, lblDonwloadSampleFile, lblFromDate, lblToDate;
        public void OnGet(string? ItemName, string? Group, DateTime? FromDate, DateTime? ToDate, int page = 1)
        {
            base.ExtractSessionData();
            if (this.CanManageItems)
            {
                FillLables();
                if (HttpContext.Request.Query.ContainsKey("page")){
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.ItemName = ItemName;
                    this.Group = Group;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    Console.WriteLine("FromDate", this.FromDate);
                    FillData(ItemName, Group, FromDate, ToDate, page);

                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void OnPost([FromForm] string ItemName, [FromForm] string Group, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        {
            
            base.ExtractSessionData();
            this.ItemName = ItemName;
            this.Group = Group;
            this.FromDate = FromDate;
            this.ToDate = ToDate;
            if (CanManageItems)
            {
                FillData(ItemName, Group, FromDate, ToDate);
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        // Function without pagination 
        /*private void FillData(string ItemName, string Group, DateTime? FromDate, DateTime? ToDate)
        {

            FillLables();
            var dbContext = new LabDBContext();
            var disbursements = dbContext.DisbursementRequests.ToList();
            var query = (from i in dbContext.Items
                         join g in dbContext.ItemGroups on i.GroupCode equals g.GroupCode
                         join t in dbContext.ItemTypes on i.ItemTypeCode equals t.ItemTypeCode
                         join u in dbContext.Units on i.UnitId equals u.Id
                         where i.Ended == null
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
                             Ended = Convert.ToString(i.Ended),
                         });

            if (string.IsNullOrEmpty(ItemName) == false)
                query = query.Where(i => i.ItemName.Contains(ItemName));

            if (string.IsNullOrEmpty(Group) == false)
                query = query.Where(i => i.GroupDesc.Contains(Group));

            if (string.IsNullOrEmpty(ItemName) == false && string.IsNullOrEmpty(Group) == false)
                query = query.Where(i => i.ItemName.Contains(ItemName) && i.GroupDesc.Contains(Group));

            if (FromDate != null && FromDate >= DateTime.MinValue  && ToDate != null && ToDate >= DateTime.MinValue)
                query = query.Where(e => e.ExpiryDate.Value.Date >= FromDate.Value.Date && e.ExpiryDate.Value.Date <= ToDate.Value.Date);
            
            Items = query.ToList();
            TotalItems = Items.Count();
        }*/

        private void FillData(string ItemName, string Group, DateTime? FromDate, DateTime? ToDate, int page = 1)
        {
            if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }
            FillLables();
            var dbContext = new LabDBContext();
            var disbursements = dbContext.DisbursementRequests.ToList();
            var query = (from i in dbContext.Items
                         join g in dbContext.ItemGroups on i.GroupCode equals g.GroupCode
                         join t in dbContext.ItemTypes on i.ItemTypeCode equals t.ItemTypeCode
                         join u in dbContext.Units on i.UnitId equals u.Id
                         where i.Ended == null
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
                             Ended = Convert.ToString(i.Ended),
                         });

            if (string.IsNullOrEmpty(ItemName) == false)
                query = query.Where(i => i.ItemName.Contains(ItemName));

            if (string.IsNullOrEmpty(Group) == false)
                query = query.Where(i => i.GroupDesc.Contains(Group));

            if (string.IsNullOrEmpty(ItemName) == false && string.IsNullOrEmpty(Group) == false)
                query = query.Where(i => i.ItemName.Contains(ItemName) && i.GroupDesc.Contains(Group));

            if (FromDate != null && FromDate >= DateTime.MinValue  && ToDate != null && ToDate >= DateTime.MinValue)
                query = query.Where(e => e.ExpiryDate.Value.Date >= FromDate.Value.Date && e.ExpiryDate.Value.Date <= ToDate.Value.Date);
            
            TotalItems = query.Count();
            TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
            var list = query.ToList();
            Items = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();        
            CurrentPage = page;       
        }
        public IActionResult OnPostEdit([FromForm] int ItemId, [FromForm] string FromDate, [FromForm] string ToDate, [FromForm] 
        int page, [FromForm] string ItemName, [FromForm] string Group)
        {
            HttpContext.Session.SetInt32("ItemId", ItemId);
            HttpContext.Session.SetString("ItemName", string.IsNullOrEmpty(ItemName) ? "" : ItemName);
            HttpContext.Session.SetString("Group", string.IsNullOrEmpty(Group) ? "" : Group);
            HttpContext.Session.SetString("FromDate", string.IsNullOrEmpty(FromDate) ? "" : FromDate);
            HttpContext.Session.SetString("ToDate", string.IsNullOrEmpty(ToDate) ? "" : ToDate);
            HttpContext.Session.SetInt32("page", page);
         

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
                    var item = dbContext.Items.Single(s => s.ItemId == ItemId);

                    item.Ended = DateTime.Now;

                    //dbContext.Items.Remove(Item);
                    dbContext.SaveChanges();
                    FillData(null, null, null, null);
                    Message = string.Format((Program.Translations["ItemDeleted"])[Lang], item.ItemName);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
                }
                else
                {
                    Message = (Program.Translations["ItemNotDeleted"])[Lang];
                    FillData(null, null, null, null);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public async Task<IActionResult> OnPostImport([FromForm] IFormFile file)
        {
            var dbContext = new LabDBContext();
            base.ExtractSessionData();
            if (CanManageItems)
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        var worksheet = package.Workbook.Worksheets[0];

                        int rowCount = worksheet.Dimension.End.Row;
                        int columnCount = worksheet.Dimension.End.Column;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            string Unit = worksheet.Cells[row, 8].Value.ToString();
                            var uId = dbContext.Units.FirstOrDefault(e => e.UnitCode == Unit).Id;

                            string itemTypeCode = string.Empty;
                            string itemType = worksheet.Cells[row, 6].Value.ToString();
                            if (itemType.ToLower().Equals("liquid"))
                                itemTypeCode = "LIQ";
                            else if (itemType.ToLower().Equals("solid"))
                                itemTypeCode = "SOL";
                            else if (itemType.ToLower().Equals("lab"))
                                itemTypeCode = "LAB";

                            string groupCode = string.Empty;
                            string groupName = Convert.ToString(worksheet.Cells[row, 7].Value);
                            if (groupName.ToLower().Equals("chemical"))
                                groupCode = "CH";
                            else if (groupName.ToLower().Equals("glass"))
                                groupCode = "GL";

                            var record = new Item()
                            {
                                ItemId = PrimaryKeyManager.GetNextId(),
                                ItemCode = Convert.ToString(worksheet.Cells[row, 1].Value),
                                ItemName = Convert.ToString(worksheet.Cells[row, 2].Value),
                                ItemDescription = Convert.ToString(worksheet.Cells[row, 3].Value),
                                IsHazardous = worksheet.Cells[row, 4].Value.ToString().ToLower() == "true" ? true : false,
                                HazardTypeName = Convert.ToString(worksheet.Cells[row, 5].Value),
                                ItemTypeCode = itemTypeCode,
                                GroupCode = groupCode,
                                UnitId = uId,
                                AvailableQuantity = Convert.ToInt32(worksheet.Cells[row, 9].Value),
                                BatchNo = Convert.ToString(worksheet.Cells[row, 10].Value),
                                ExpiryDate = Convert.ToDateTime(worksheet.Cells[row, 11].Value),
                                Created = DateTime.Now,

                            };

                            dbContext.Items.Add(record);

                            await dbContext.SaveChangesAsync();



                        }
                        Message = (Program.Translations["FileUploaded"])[Lang];
                        return RedirectToPage();

                    }

                }

            }
            else
                RedirectToPage("./Index?lang=" + Lang);

            return Page();
        }

        public IActionResult OnPostLoadSampleFile()
        {
            MemoryStream stream = new MemoryStream();
            base.ExtractSessionData();

            // Set the content type and file name for the response
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "ITEM_SAMPLE_FILE.xlsx";
            string filePath = "";

            if (CanSeeReports)
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ITEM_SAMPLE_FILE.xlsx");

                // Check if the file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Sample Excel file not found.");
                }


            }
            else
                RedirectToPage("./Index?lang=" + Lang);

            return File(System.IO.File.OpenRead(filePath), contentType, fileName);
        }
        private void FillLables()
        {


            this.lblItems = (Program.Translations["Items"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
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
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblImport = (Program.Translations["Import"])[Lang];
            this.lblDonwloadSampleFile = (Program.Translations["DonwloadSampleFile"])[Lang];
            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
        }

    }
}
