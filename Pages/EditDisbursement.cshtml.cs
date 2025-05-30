using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace LabMaterials.Pages
{
    public class EditDisbursementModel : BasePageModel
    {   
        private readonly LabDBContext _context;
        private readonly IWebHostEnvironment _environment;
        public EditDisbursementModel(LabDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public string ErrorMsg { get; set; }
 
        public List<User> DeptManagerList {  get; set; }
        public List<User> SupervisorList {  get; set; }
        public List<User> KeeperList {  get; set; }
        public List<ItemCard> ItemsValue { get; set; }
        public List<Destination> Destinations { get; set; }
        public List<Store> Stores { get; set; }
        public List<Item> Items { get; set; }

        public List<ItemCard> ItemCards { get; set;}
        public List<Unit> Units { get; set; }
        public List<ItemGroup> ItemGroups { get; set; }
         [BindProperty]
        public MaterialRequest Report { get; set; }
        [BindProperty]
        public List<DespensedItem> ItemsForReport { get; set; } = new List<DespensedItem>();

        public int? SupervisorId, DeptManagerId, KeeperId;
        public int StoreId, Quantity;

        public int MaterialRequestId { get; set; }
        public int reportId { get; set; }
        public int serialNo { get; set; }


        public string lblUpdateDisbursement, lblRequesterName, lblItemTypeCode, lblItemName, lblStoreName, lblEditDisbursement, lblItemCode, lblRequestReceivedDate, lblRequestingPlace, lblComments, lblQuantity,
            lblDisbursementStatus, lblInventoryBalanced, lblUpdate, lblCancel, lblDisbursements;

        public string lblAddDisbursement,
                 lblAdd, lblItemGroups, lblArabicLanguage, lblEnglishLanguage, lblItemDescription,
                    lblTypeofAsset, lblChemical, lblRiskRating, lblUnitOfMeasure, lblAmountSpent, lblUnitPrice, lblTotalPrice, lblRemove, lblAddMore, lblSerialNumber,
                    lblFiscalYear, lblOrderDate, lblRequestingSector, lblRequestDocumentType, lblRequestDocumentNumber, lblSector;

        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanGenerateDispensingRequest == false)
                RedirectToPage("./Index?lang=" + Lang);

            FillLables();
            int? ReceivingReportId = HttpContext.Session.GetInt32("ReceivingReportId");
            int? serialNo = HttpContext.Session.GetInt32("SerialNo");
            this.serialNo = serialNo.Value;
            this.MaterialRequestId = ReceivingReportId.Value;


            var dbContext = new LabDBContext();



            Destinations = dbContext.Destinations.ToList();
            Stores = dbContext.Stores.ToList();
            ItemCards = dbContext.ItemCards.ToList();
            Units = dbContext.Units.ToList();
            Report ??= new MaterialRequest();
            ItemGroups = dbContext.ItemGroups.Where(g => g.Units.Count() > 0).ToList();

            ItemsForReport = dbContext.DespensedItems
                        .Where(r => r.MaterialRequestId == ReceivingReportId.Value)
                        .ToList();

            //Department Manager list
            var DeptManagerId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Department Manager")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            DeptManagerList = dbContext.Users
                        .Where(u => u.UserGroupId == DeptManagerId)
                        .ToList();

            //General Supervisor list
            var SupervisorId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "General Supervisor")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            SupervisorList = dbContext.Users
                        .Where(u => u.UserGroupId == SupervisorId)
                        .ToList();

            //Keeper  list
            var KeeperId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Warehouse Keeper")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            KeeperList = dbContext.Users
                        .Where(u => u.UserGroupId == KeeperId)
                        .ToList();
            ItemsValue = dbContext.ItemCards
                            .Select(x => new ItemCard
                            {
                                Id = x.Id,
                                ItemId = x.ItemId,
                                ItemName = x.ItemName,
                                GroupCode = x.GroupCode,
                                ItemCode = x.ItemCode,
                                ItemDescription = x.ItemDescription,
                                Chemical = x.Chemical,
                                UnitOfmeasure = x.UnitOfmeasure
                            }).ToList();

            Report = dbContext.MaterialRequests
                .FirstOrDefault(r => r.RequestId == ReceivingReportId.Value);
           
        }

        public async Task<IActionResult> OnPostAsync([FromForm] DateTime OrderDate, [FromForm] int SerialNumber, [FromForm] string FiscalYear, [FromForm] string RequestDocumentType, [FromForm] int RequestingSector, [FromForm] string Sector, [FromForm] int DeptManagerId)
        {
            LogableTask task = LogableTask.NewTask("EditDisbursement");
            try
            { 
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                var dbContext = new LabDBContext();
                //Department Manager list
                var DeptManager = dbContext.UserGroups
                        .Where(g => g.UserGroupName == "Department Manager")
                        .Select(g => g.UserGroupId)
                        .FirstOrDefault();

                DeptManagerList = dbContext.Users
                            .Where(u => u.UserGroupId == DeptManager)
                            .ToList();
                Destinations = dbContext.Destinations.ToList();
                Stores = dbContext.Stores.ToList();
                ItemCards = dbContext.ItemCards.ToList();
                Units = dbContext.Units.ToList();
                ItemGroups = dbContext.ItemGroups.Where(g => g.Units.Count() > 0).ToList();

                ItemsValue = dbContext.ItemCards
                    .Select(x => new ItemCard
                    {
                        Id = x.Id,
                        ItemId = x.ItemId,
                        ItemName = x.ItemName,
                        GroupCode = x.GroupCode,
                        ItemCode = x.ItemCode,
                        ItemDescription = x.ItemDescription,
                        Chemical = x.Chemical,
                        UnitOfmeasure = x.UnitOfmeasure
                    }).ToList();

                if (ItemsForReport == null || !ItemsForReport.Any())
                {
                    ItemsForReport = new List<DespensedItem> { new DespensedItem() };
                }

                if (CanGenerateDispensingRequest)
                {
                    FillLables();
                    int userId = HttpContext.Session.GetInt32("UserId").Value;
                    var user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
                    if (user == null)
                    {
                        ErrorMsg = "User not found.";
                        return Page();
                    }
                    int? ReceivingReportId = HttpContext.Session.GetInt32("ReceivingReportId");
                    this.MaterialRequestId = ReceivingReportId.Value;
                    var report = await _context.MaterialRequests.FindAsync(this.MaterialRequestId);
                    if (report == null)
                    {
                        return NotFound("Receiving report not found.");
                    }
                    report.SerialNumber = SerialNumber;
                    report.OrderDate = OrderDate.Date;
                    // report.RequestedByUser = user;
                    report.RequestedByUserId = user.UserId;
                    report.FiscalYear = FiscalYear;
                    report.RequestDocumentType = RequestDocumentType;
                    report.RequestingSector = RequestingSector;
                    report.Sector = Sector;
                    // report.KeeperId = KeeperId;
                    report.DeptManagerId = DeptManagerId;
                    report.CreatedAt = DateTime.UtcNow;
                    // report.SupervisorId = SupervisorId;
                    // report.DocumentNumber = DocumentNumber;

                    if (string.IsNullOrEmpty(FiscalYear))
                    {
                        ErrorMsg = (Program.Translations["FiscalYearMissing"])[Lang];
                        return Page();
                    }
                    else if (OrderDate == default(DateTime))
                    {
                        ErrorMsg = (Program.Translations["ReceivingDateMissing"])[Lang];
                        return Page();
                    }
                    else if (report.RequestingSector == 0)
                    {
                        ErrorMsg = (Program.Translations["RecipientSectorMissing"])[Lang];
                        return Page();
                    }
                    if (string.IsNullOrEmpty(report.DocumentNumber))
                    {
                        ErrorMsg = (Program.Translations["DocumentNumberMissing"])[Lang];
                        return Page();
                    }
                    if (string.IsNullOrEmpty(report.Sector))
                    {
                        ErrorMsg = (Program.Translations["SectorNumberMissing"])[Lang];
                        return Page();
                    }
                    if (string.IsNullOrEmpty(report.WarehouseId.ToString()))
                    {
                        ErrorMsg = (Program.Translations["WarehouseNameMissing"])[Lang];
                        return Page();
                    }

                    if (report.RequestingSector == 0)
                    {
                        ErrorMsg = (Program.Translations["ReceivingWarehouseMissing"])[Lang];
                        return Page();
                    }
                    if (report.DeptManagerId == 0)
                    {
                        ErrorMsg = (Program.Translations["DeptManagerMissing"])[Lang];
                        return Page();
                    }


                    if (!ItemsForReport.Any(item => item.ItemCardId != 0 && item.Quantity > 0 && item.UnitPrice > 0))
                    {
                        ErrorMsg = "At least one item must have the required fields filled (Item Group, Quantity, Unit Price, Item Name).";
                        return Page();
                    }

                    await _context.SaveChangesAsync();
                    
                    // Delete old items
                    var existingItems = _context.DespensedItems.Where(ri => ri.MaterialRequestId == report.RequestId).ToList();
                    _context.DespensedItems.RemoveRange(existingItems);
                    await _context.SaveChangesAsync();

                    // Add updated items
                    foreach (var item in ItemsForReport)
                    {
                        // if (item.ItemId != 0 && item.Quantity > 0 && item.UnitPrice > 0)
                        // {
                            item.MaterialRequestId = report.RequestId;
                            item.ItemCardId = item.ItemCardId; // Ensure the ItemId is set correctly

                            if (item.Comments == null || item.Comments == "0")
                            item.Comments = "";

                            _context.DespensedItems.Add(item);
                        // }
                    }

                }
                await _context.SaveChangesAsync();
                return RedirectToPage("/Requests");
            }
            catch (Exception ex)
            {
                task.LogError(MethodBase.GetCurrentMethod(), ex);
                ErrorMsg = ex.Message;
                //for seeing inner exception errors
                if (ex.InnerException != null)
                {
                    ErrorMsg += " Inner Exception: " + ex.InnerException.Message;
                }


                return Page();
            }
            finally { task.EndTask(); }

        }


         private void FillLables()
        {


            this.lblAddDisbursement = (Program.Translations["AddDisbursement"])[Lang];
            this.lblRequesterName = (Program.Translations["RequesterName"])[Lang];
            this.lblRequestReceivedDate = (Program.Translations["RequestReceivedDate"])[Lang];
            this.lblRequestingPlace = (Program.Translations["RequestingPlace"])[Lang];
            this.lblComments = (Program.Translations["Comments"])[Lang];
            this.lblDisbursementStatus = (Program.Translations["DisbursementStatus"])[Lang];
            this.lblInventoryBalanced = (Program.Translations["InventoryBalanced"])[Lang];

            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblItemTypeCode = (Program.Translations["ItemTypeCode"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblDisbursements = (Program.Translations["Disbursements"])[Lang];
            this.lblItemGroups = (Program.Translations["ItemGroups"])[Lang];
            this.lblArabicLanguage = (Program.Translations["ArabicLanguage"])[Lang];
            this.lblEnglishLanguage = (Program.Translations["EnglishLanguage"])[Lang];
            this.lblItemDescription = (Program.Translations["ItemDescription"])[Lang];
            this.lblTypeofAsset = (Program.Translations["TypeofAsset"])[Lang];
            this.lblChemical = (Program.Translations["Chemical"])[Lang];
            this.lblRiskRating = (Program.Translations["RiskRating"])[Lang];
            this.lblUnitOfMeasure = (Program.Translations["UnitOfMeasure"])[Lang];
            this.lblAmountSpent = (Program.Translations["AmountSpent"])[Lang];
            this.lblUnitPrice = (Program.Translations["UnitPrice"])[Lang];
            this.lblTotalPrice = (Program.Translations["TotalPrice"])[Lang];
            this.lblRemove = (Program.Translations["Remove"])[Lang];
            this.lblAddMore = (Program.Translations["AddMore"])[Lang];
            this.lblSerialNumber = (Program.Translations["SerialNumber"])[Lang];
            this.lblFiscalYear = (Program.Translations["FiscalYear"])[Lang];
            this.lblOrderDate = (Program.Translations["OrderDate"])[Lang];
            this.lblRequestingSector = (Program.Translations["RequestingSector"])[Lang];
            this.lblRequestDocumentType = (Program.Translations["RequestDocumentType"])[Lang];
            this.lblRequestDocumentNumber = (Program.Translations["RequestDocumentNumber"])[Lang];
            this.lblSector = (Program.Translations["Sector"])[Lang];
        }
    }
}
