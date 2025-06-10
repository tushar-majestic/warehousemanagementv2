using HarfBuzzSharp;
using LabMaterials.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace LabMaterials.Pages
{
    public class AddDisbursementModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string RequesterName, Status, Comments, RequestingPlace, StoreName, ItemName, ItemCode, ItemTypeCode;
        public int DId;
        private readonly LabDBContext _context;
        private readonly IWebHostEnvironment _environment;
         public AddDisbursementModel(LabDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public int StoreId, Quantity;
        public bool InventoryBalanced;
        public DateTime RequestRecievedAt;
        public List<SelectListItem> StatusList;
        public List<Destination> Destinations { get; set; }
        public List<Store> Stores { get; set; }
        public List<Item> Items { get; set; }

        public List<ItemCard> ItemCards { get; set;}
        public List<Unit> Units { get; set; }
        public List<ItemGroup> ItemGroups { get; set; }
        public List<ItemInfoByStoreId> ItemInfoByStore { get; set; }
        [BindProperty]
        public MaterialRequest Report { get; set; }
        [BindProperty]
        public List<DespensedItem> ItemsForReport { get; set; } = new List<DespensedItem>();
        public List<User> DeptManagerList {  get; set; }
        public List<User> SupervisorList {  get; set; }
        public List<User> KeeperList {  get; set; }
        public List<ItemCard> ItemsValue { get; set; }
        public List<DocumentType> DocumentList { get; set; }

        
        
        public int? SupervisorId, DeptManagerId, KeeperId;



        public string lblAddDisbursement, lblRequesterName, lblRequestReceivedDate, lblQuantity, lblItemCode, lblItemTypeCode, lblItemName, lblStoreName, lblRequestingPlace, lblComments,
            lblDisbursementStatus, lblInventoryBalanced, lblAdd, lblCancel, lblDisbursements, lblItemGroups, lblArabicLanguage, lblEnglishLanguage, lblItemDescription,
            lblTypeofAsset, lblChemical, lblRiskRating, lblUnitOfMeasure, lblAmountSpent, lblUnitPrice, lblTotalPrice, lblRemove, lblAddMore, lblSerialNumber,
            lblFiscalYear, lblOrderDate, lblRequestingSector, lblRequestDocumentType, lblRequestDocumentNumber, lblSector;

        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanGenerateDispensingRequest == false)
                RedirectToPage("./Index?lang=" + Lang);
            FillLables();
            RequestRecievedAt = DateTime.Now;
            var dbContext = new LabDBContext();
            Destinations = dbContext.Destinations.ToList();
            Stores = dbContext.Stores.ToList();
            var query = from ic in dbContext.ItemCards
                        join i in dbContext.Items on ic.ItemId equals i.ItemId
                        select new ItemCard
                        {
                            ItemCode = ic.ItemCode,
                            ItemTypeCode = ic.ItemTypeCode,
                            ItemName = ic.ItemName,
                            ItemNameAr = i.ItemNameAr,
                            ItemDescription = ic.ItemDescription,
                            UnitOfmeasure = ic.UnitOfmeasure,
                            Id = ic.Id,
                            ItemId = ic.ItemId,
                            GroupCode = ic.GroupCode,
                            Chemical = ic.Chemical,
                            HazardTypeName = ic.HazardTypeName,
                        };
            ItemCards = query.ToList();
            Units = dbContext.Units.ToList();
            Report ??= new MaterialRequest();
            ItemGroups = dbContext.ItemGroups.Where(g => g.Units.Count() > 0).ToList();
            // **Important**: seed one blank DespensedItem so index [0] exists
            ItemsForReport = new List<DespensedItem> { new DespensedItem() };

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
            ItemsValue = query.ToList();
            DocumentList = dbContext.DocumentTypes
                        .ToList();

        }
        public void OnGetOld()
        {
            base.ExtractSessionData();
            if (CanGenerateDispensingRequest == false)
                RedirectToPage("./Index?lang=" + Lang);
            FillLables();
            StatusList = (new[] { "NewRequest", "InPreparation", "Dispatched", "Delivered" }).ToList().Select(x => new SelectListItem() { Text = x, Value = x }).ToList();
            RequestRecievedAt = DateTime.Now;
            var dbContext = new LabDBContext();
            Destinations = dbContext.Destinations.ToList();
            Stores = dbContext.Stores.ToList();
            Items = dbContext.Items
                .Include(i => i.Unit)
                .Where(i => i.Ended == null)
                .ToList();

            ItemCards = dbContext.ItemCards.ToList();
            Units = dbContext.Units.ToList();
            ItemGroups = dbContext.ItemGroups.Where(g => g.Units.Count() > 0).ToList();




        }

        public IActionResult OnGetItemsFromStore(int storeId)
        {
            var dbContext = new LabDBContext();
            var storeIdParam = new SqlParameter("@PSTORE_ID", SqlDbType.Int) { Value = storeId };
            storeIdParam.Direction = ParameterDirection.Input;
            var codeParam = new SqlParameter("@PCODE", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };
            var msgParam = new SqlParameter("@PMSG", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Output };
            var descParam = new SqlParameter("@PDESC", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };

            ItemInfoByStore = dbContext.ItemInfoByStoreIds
                .FromSqlRaw("EXEC PRC_GET_ITEM_INFO_BY_STOREID @PSTORE_ID, @PCODE OUTPUT, @PDESC OUTPUT, @PMSG OUTPUT",
                            storeIdParam, codeParam, descParam, msgParam)
                .ToList();


            return new JsonResult(ItemInfoByStore);

        }

        public async Task<IActionResult> OnGetGetNextSerialNumberAsync(string fiscalYear)
        {
            
            int lastSerial = await _context.MaterialRequests
                .Where(r => r.FiscalYear == fiscalYear)
                .MaxAsync(r => (int?)r.SerialNumber) ?? 0;

            return new JsonResult(new { serial = lastSerial + 1 });
        }

        public IActionResult OnGetRequesterName(int DId)
        {
            var dbContext = new LabDBContext();


            var requesterName = dbContext.Requesters.Where(r => r.DestinationId == DId && r.Ended == null)
                        .Select(r => new { r.ReqName, r.ReqId })
                        .ToList();

            return new JsonResult(requesterName);
        }

        public async Task<IActionResult> OnPostAsync([FromForm] DateTime OrderDate, [FromForm] int SerialNumber, [FromForm] string FiscalYear, [FromForm] string RequestDocumentType, [FromForm] int RequestingSector, [FromForm] string Sector, [FromForm] int DeptManagerId)
        {
            LogableTask task = LogableTask.NewTask("AddDisbursement");
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
                DocumentList = dbContext.DocumentTypes
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
                    Report.OrderDate = OrderDate;
                    int userId = HttpContext.Session.GetInt32("UserId").Value;
                    var user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
                    if (user == null)
                    {
                        ErrorMsg = "User not found.";
                        return Page();
                    }
                 
                    Report.SerialNumber = SerialNumber;
                    Report.OrderDate = OrderDate.Date;
                    // Report.RequestedByUser = user;
                     Report.RequestedByUserId = user.UserId;
                    Report.FiscalYear = FiscalYear;
                    Report.RequestDocumentType = RequestDocumentType;
                    Report.RequestingSector = RequestingSector;
                    Report.Sector = Sector;
                    // Report.KeeperId = KeeperId;
                    Report.DeptManagerId = DeptManagerId;
                    Report.CreatedAt = DateTime.UtcNow;
                    // Report.SupervisorId = SupervisorId;
                    // Report.DocumentNumber = DocumentNumber;
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
                    else if (Report.RequestingSector == 0)
                    {
                        ErrorMsg = (Program.Translations["RecipientSectorMissing"])[Lang];
                        return Page();
                    }
                    if (string.IsNullOrEmpty(Report.DocumentNumber)){
                        ErrorMsg = (Program.Translations["DocumentNumberMissing"])[Lang];
                        return Page();
                    }
                    if (string.IsNullOrEmpty(Report.Sector)){
                        ErrorMsg = (Program.Translations["SectorNumberMissing"])[Lang];
                        return Page();
                    }
                    if (string.IsNullOrEmpty(Report.WarehouseId.ToString())){
                        ErrorMsg = (Program.Translations["WarehouseNameMissing"])[Lang];
                        return Page();
                    }
                    
                    if (Report.RequestingSector == 0)
                    {
                        ErrorMsg = (Program.Translations["ReceivingWarehouseMissing"])[Lang];
                        return Page();
                    }
                    if (Report.DeptManagerId == 0)
                    {
                        ErrorMsg = (Program.Translations["DeptManagerMissing"])[Lang];
                        return Page();
                    }


                    if (!ItemsForReport.Any(item => item.ItemCardId != 0 && item.Quantity > 0 && item.UnitPrice > 0))
                    {
                        ErrorMsg = "At least one item must have the required fields filled (Item Group, Quantity, Unit Price, Item Name).";
                        return Page();
                    }

                    

                    _context.MaterialRequests.Add(Report);
                    await _context.SaveChangesAsync();

 
                    foreach (var item in ItemsForReport)
                    {
                        item.MaterialRequestId = Report.RequestId;
                        item.ItemCardId = item.ItemCardId;
                        if (item.Comments == null || item.Comments == "0")
                            item.Comments = "";


                        _context.DespensedItems.Add(item); // Add the item to the context
                        //reduce quantity when request is approved
                        // var itemCard = await _context.ItemCards.FindAsync(item.ItemCardId);
                        // if (itemCard == null)
                        // {   //ItemCard not found
                        //     return Page();
                        // }
                        // itemCard.QuantityAvailable -= item.Quantity;
                    }
                    await _context.SaveChangesAsync();
                }

                string Message = string.Format("Sent Material Dispensing Request Approve the request or add comments.");
                var msg = new  Message
                {
                    MaterialRequestId = Report.RequestId,
                    ReportType = "Dispensing",
                    SenderId = Report.RequestedByUserId,
                    RecipientId = Report.DeptManagerId,
                    Content = Message,
                    Type = "",
                    CreatedAt = DateTime.UtcNow
                };
                dbContext.Messages.Add(msg);
                dbContext.SaveChanges();
                return RedirectToPage("/Disbursements");


            }
            catch (Exception ex)
            {
                task.LogError(MethodBase.GetCurrentMethod(), ex);
                ErrorMsg = ex.Message ;
                //for seeing inner exception errors
                if (ex.InnerException != null)
                {
                    ErrorMsg += " Inner Exception: " + ex.InnerException.Message;
                }

                // if (ItemsForReport == null || !ItemsForReport.Any())
                // {
                //     ItemsForReport = new List<DespensedItem> { new DespensedItem() };
                // }
                return Page();
            }
            finally { task.EndTask(); }

        }
        public IActionResult OnPostold([FromForm] string RequesterName, [FromForm] string Status, [FromForm] string Comments,
            [FromForm] int DId, [FromForm] bool InventoryBalanced, [FromForm] DateTime RequestRecievedAt, [FromForm] int StoreId,
            [FromForm] int ItemId, [FromForm] int Quantity)
        {
            LogableTask task = LogableTask.NewTask("AddDisbursement");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanDisburseItems)
                {
                    FillLables();
                    this.RequesterName = RequesterName;
                    this.Status = Status;
                    this.Comments = Comments;

                    StatusList = (new[] { "NewRequest", "InPreparation", "Dispatched", "Delivered" }).ToList().Select(x => new SelectListItem() { Text = x, Value = x }).ToList();

                    var dbContext = new LabDBContext();
                    var destination = dbContext.Destinations.SingleOrDefault(dst => dst.DId == DId);
                    this.RequestingPlace = destination.DestinationName;
                    var store = dbContext.Stores.SingleOrDefault(st => st.StoreId == StoreId);
                    this.StoreName = store.StoreName;
                    var item = dbContext.Items.SingleOrDefault(st => st.ItemId == ItemId);
                    this.ItemName = item.ItemName;
                    this.ItemCode = item.ItemCode;
                    this.ItemTypeCode = item.ItemTypeCode;


                    if (string.IsNullOrEmpty(RequesterName))
                        ErrorMsg = (Program.Translations["RequesterNameMissing"])[Lang];
                    else if (string.IsNullOrEmpty(RequestingPlace))
                        ErrorMsg = (Program.Translations["RequestingPlaceMissing"])[Lang];
                    else
                    {

                        /*var disbursement = new DisbursementRequest
                        {
                            DisbursementRequestId = PrimaryKeyManager.GetNextId(),
                            RequesterName = RequesterName,
                            Comments = Comments,
                            RequestingPlace = this.RequestingPlace,
                            Status = Status,
                            InventoryBalanced = InventoryBalanced,
                            ReqReceivedAt = RequestRecievedAt,
                            StoreId = StoreId,
                            Itemcode = this.ItemCode,
                            Itemtypecode = this.ItemTypeCode,
                            ItemQuantity = Quantity
                        };
                        dbContext.DisbursementRequests.Add(disbursement);*/

                        var itemIdParam = new SqlParameter("@PITEM_ID", SqlDbType.Int) { Value = ItemId };
                        var storeIdParam = new SqlParameter("@PSTORE_ID", SqlDbType.Int) { Value = StoreId };
                        var quantityParam = new SqlParameter("@PQUANTITY", SqlDbType.Int) { Value = Quantity };
                        var disbDate = new SqlParameter("@PDIS_DATE", SqlDbType.DateTime) { Value = RequestRecievedAt };
                        var disbStatus = new SqlParameter("@PDIS_STATUS", SqlDbType.VarChar) { Value = Status };
                        var disbComment = new SqlParameter("@PDIS_COMMENT", SqlDbType.VarChar) { Value = string.IsNullOrEmpty(Comments) ? "" : Comments };
                        var disbInvBalance = new SqlParameter("@PINV_BALANCE", SqlDbType.Bit) { Value = InventoryBalanced };
                        var disbReqName = new SqlParameter("@PREQ_NAME", SqlDbType.VarChar) { Value = RequesterName };
                        var disbReqPlace = new SqlParameter("@PREQ_PLACE", SqlDbType.VarChar) { Value = this.RequestingPlace };
                        var disbItemCode = new SqlParameter("@PITEM_CODE", SqlDbType.VarChar) { Value = ItemCode };
                        var disbItemTypeCode = new SqlParameter("@PITEM_TYPE_CODE", SqlDbType.VarChar) { Value = this.ItemTypeCode };

                        itemIdParam.Direction = ParameterDirection.Input;
                        storeIdParam.Direction = ParameterDirection.Input;
                        quantityParam.Direction = ParameterDirection.Input;
                        disbDate.Direction = ParameterDirection.Input;
                        disbStatus.Direction = ParameterDirection.Input;
                        disbComment.Direction = ParameterDirection.Input;
                        disbInvBalance.Direction = ParameterDirection.Input;
                        disbReqName.Direction = ParameterDirection.Input;
                        disbReqPlace.Direction = ParameterDirection.Input;
                        disbItemCode.Direction = ParameterDirection.Input;
                        disbItemTypeCode.Direction = ParameterDirection.Input;

                        var codeParam = new SqlParameter("@PCODE", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };
                        var msgParam = new SqlParameter("@PMSG", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Output };
                        var descParam = new SqlParameter("@PDESC", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };

                        var st = dbContext.Database
                            .ExecuteSqlRaw("EXEC PRC_ADD_DISBURSEMENT @PITEM_ID, @PSTORE_ID,@PQUANTITY, @PDIS_DATE, @PDIS_STATUS, @PDIS_COMMENT, @PINV_BALANCE, @PREQ_NAME, @PREQ_PLACE, @PITEM_CODE, @PITEM_TYPE_CODE, @PCODE OUTPUT, @PDESC OUTPUT, @PMSG OUTPUT",
                                        itemIdParam, storeIdParam, quantityParam, disbDate, disbStatus, disbComment, disbInvBalance, disbReqName, disbReqPlace, disbItemCode, disbItemTypeCode, codeParam, descParam, msgParam);


                        task.LogInfo(MethodBase.GetCurrentMethod(), "Disbursement added");

                        string Message = string.Format("Disbursement  added");
                        Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                            Helper.ExtractIP(Request), dbContext, true);

                        return RedirectToPage("./Disbursements");
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
