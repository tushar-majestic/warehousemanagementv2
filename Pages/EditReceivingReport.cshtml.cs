using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LabMaterials.DB;
using System.Linq;
using LabMaterials.DB;
using LabMaterials.dtos;
using Org.BouncyCastle.Cms;


namespace LabMaterials.Pages
{
    public class EditReceivingReportsModel : BasePageModel
    {
        private readonly LabDBContext _context;
        private readonly IWebHostEnvironment _environment;
        public string lblCreateReport, lblSerialNumber, lblFiscalYear, lblReceivingDate, lblSectorNumber, lblReceivingWarehouse, lblBasedOnDocument, lblDocumentNumber, lblDocumentDate, lblAddAttachment, lblSupplierType, lblSupplierName, lblItemGroup, lblItemNo, lblItemName, lblItemDescription, lblUnitOfMeasure, lblQuantity, lblUnitPrice, lblTotalPrice, lblComments, lblRecipientID, lblRecipientName, lblTechnicalMember, lblChiefResponsible, lblSubmitReport, lblRecipientSector, lblNewReceivingReport, lblEditReceivingReport;
        public EditReceivingReportsModel(LabDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        [BindProperty]
        public IFormFile AttachmentFile { get; set; }
        public IList<Supplier> Suppliers { get; set; }
        public IList<Store> Warehouses { get; set; }
        public List<ReceivingReport> Reports { get; set; }
        public List<Item> Items { get; set; }
        public List<User> GeneralSupervisorList {  get; set; }

        public List<User> Users {  get; set; }

        public List<User> TechnicalMemberList {  get; set; }

        public List<Unit> Units { get; set; } 
        public List<ItemGroup> ItemGroupList { get; set; } 


        public int?  GeneralSupervisorId, TechnicalMemberId ;
        public int? ItemId;
        public string ErrorMsg { get; set; }
        public string RecipientEmployeeName;

        public int? RecipientJobNumber { get; set; }

        public string ItemNo;
        [BindProperty]
        public ReceivingReport Report { get; set; }  // <- change name from NewReport
        public string SupplierType => Report.Supplier?.SupplierType;

        [BindProperty]
        public List<ReceivingItem> ItemsForReport { get; set; } = new List<ReceivingItem>();
        public int serialNo { get; set; }
        public int ReceivingReportId { get; set; }
        public int reportId { get; set; }


        public async Task OnGetAsync()
        {
            base.ExtractSessionData();
            FillLables();

            int? serialNo = HttpContext.Session.GetInt32("SerialNo");
            int? ReceivingReportId = HttpContext.Session.GetInt32("ReceivingReportId");

            this.serialNo = serialNo.Value;
            this.ReceivingReportId = ReceivingReportId.Value;

            // ReportDet = await _context.ReceivingReports
            // .Include(r => r.Supplier)
            // .FirstOrDefaultAsync(r => r.Id == ReceivingReportId.Value);


            
            var dbContext = new LabDBContext();
            Report = dbContext.ReceivingReports
                .Include(r => r.Supplier) 
                .FirstOrDefault(r => r.Id == ReceivingReportId.Value);
            Suppliers = _context.Suppliers.ToList();  // Fetch suppliers
            Warehouses = _context.Stores.ToList();  // Fetch suppliers
            // Reports = await _context.ReceivingReports.Include(r => r.Items).ToListAsync();
            Items = await _context.Items.ToListAsync();
            // Report ??= new ReceivingReport();
            // ItemsForReport = new List<ReceivingItem> { new ReceivingItem() };
            ItemsForReport = dbContext.ReceivingItems
                        .Where(r => r.ReceivingReportId == ReceivingReportId.Value)
                        .ToList();
            // var userName = HttpContext.Session.GetString("UserName");
            // Report.CreatedBy = string.IsNullOrEmpty(userName) ? "Unknown" : userName;


            Units = dbContext.Units.ToList();
            ItemGroupList = dbContext.ItemGroup.ToList();
            Users = dbContext.Users.ToList();


            //General Supervisor list
            var GeneralSupervisorId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "General Supervisor")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            GeneralSupervisorList = dbContext.Users
                        .Where(u => u.UserGroupId == GeneralSupervisorId)
                        .ToList();

            //Technical Member list
            var TechnicalMemberId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Technical Member")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            var Receipient = dbContext.Users.Where(u => u.UserId == Report.RecipientEmployeeId)
                            .FirstOrDefault();

            this.RecipientEmployeeName = Receipient.FullName;
            this.RecipientJobNumber = Receipient.JobNumber;

            
            TechnicalMemberList = dbContext.Users
                        .Where(u => u.UserGroupId == TechnicalMemberId)
                        .ToList();

            // Debugging: log item count and details
            Console.WriteLine($"Item count: {Items?.Count}");
            foreach (var item in Items)
            {
                Console.WriteLine($"Item ID: {item.ItemId}, Name: {item.ItemName}");
            }
        }


        public async Task<IActionResult> OnPostAsync(DateTime ReceivingDate, DateTime DocumentDate, [FromForm] int TechnicalMember, [FromForm] int ChiefResponsible, [FromForm] string FiscalYear, [FromForm] string BasedOnDocument, [FromForm] int SerialNumber, [FromForm] string RecipientEmployeeName, [FromForm] int RecipientJobNumber)
        {
            base.ExtractSessionData();
            FillLables();
            Suppliers = _context.Suppliers.ToList();  
            Warehouses = _context.Stores.ToList();
            Items = await _context.Items.ToListAsync();
            var dbContext = new LabDBContext();

            Units = dbContext.Units.ToList();
            ItemGroupList = dbContext.ItemGroup.ToList();
            Users = dbContext.Users.ToList();


            //General Supervisor list
            var GeneralSupervisorId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "General Supervisor")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            GeneralSupervisorList = dbContext.Users
                        .Where(u => u.UserGroupId == GeneralSupervisorId)
                        .ToList();

            //Technical Member list
            var TechnicalMemberId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Technical Member")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();



            int? ReceivingReportId = HttpContext.Session.GetInt32("ReceivingReportId");

            this.ReceivingReportId = ReceivingReportId.Value;

            var report = await _context.ReceivingReports.FindAsync(this.ReceivingReportId);
            if (report == null)
            {
                return NotFound("Receiving report not found.");
            }
            report.ReceivingDate = ReceivingDate;
            report.CreatedBy = HttpContext.Session.GetInt32("UserId");
            report.DocumentDate  = DocumentDate.Date;
            report.TechnicalMemberId = TechnicalMember;
            report.ChiefResponsibleId = ChiefResponsible;
            report.FiscalYear = FiscalYear;
            report.BasedOnDocument = BasedOnDocument;
            report.SerialNumber = SerialNumber;

            report.FiscalYear = FiscalYear;
            report.KeeperApproval = true;

            if (string.IsNullOrEmpty(FiscalYear)){
                ErrorMsg = (Program.Translations["FiscalYearMissing"])[Lang];
                return Page();
            }
            else if (ReceivingDate == default(DateTime))
            {
                ErrorMsg = (Program.Translations["ReceivingDateMissing"])[Lang];
                return Page();
            }
            else if (string.IsNullOrEmpty(report.RecipientSector))
            {
                ErrorMsg = (Program.Translations["RecipientSectorMissing"])[Lang];
                return Page();
            }
            else if (string.IsNullOrEmpty(report.SectorNumber))
            {
                ErrorMsg = (Program.Translations["SectorNumberMissing"])[Lang];
                return Page();
            }
            else if (string.IsNullOrEmpty(report.ReceivingWarehouse))
            {
                ErrorMsg = (Program.Translations["ReceivingWarehouseMissing"])[Lang];
                return Page();
            }
            else if (string.IsNullOrEmpty(report.BasedOnDocument))
            {
                ErrorMsg = (Program.Translations["BasedOnDocumentMissing"])[Lang];
                return Page();
            }
            else if (string.IsNullOrEmpty(report.DocumentNumber))
            {
                ErrorMsg = (Program.Translations["DocumentNumberMissing"])[Lang];
                return Page();
            }
            else if (DocumentDate == default(DateTime))
            {
                ErrorMsg = (Program.Translations["DocumentDateMissing"])[Lang];
                return Page();
            }
            else if(report.SupplierId==0){
                ErrorMsg = (Program.Translations["SupplierMissing"])[Lang];
                return Page();
            }
            else if(report.RecipientEmployeeId==0){
                ErrorMsg = (Program.Translations["ReceipientMissing"])[Lang];
                return Page();
            }
            if (!ItemsForReport.Any(item => item.ItemId != 0 && item.Quantity > 0 && item.UnitPrice > 0))
            {
                ErrorMsg = "At least one item must have the required fields filled (Item Group, Quantity, Unit Price, Item Name).";
                return Page();
            }
            else if(report.TechnicalMemberId==0){
                ErrorMsg = (Program.Translations["TechnicalMemberMissing"])[Lang];
                return Page();
            }
          
           
           
           if (AttachmentFile != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(AttachmentFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AttachmentFile.CopyToAsync(stream);
                }

                report.AttachmentPath = "/uploads/" + uniqueFileName;
                ModelState.Remove("AttachmentPath");
            }


            if (report.ChiefResponsibleId == 0)
            {
                report.ChiefResponsibleId = null;
            }
            await _context.SaveChangesAsync();

            // Delete old items
            var existingItems = _context.ReceivingItems.Where(ri => ri.ReceivingReportId == report.Id).ToList();
            _context.ReceivingItems.RemoveRange(existingItems);
            await _context.SaveChangesAsync();

            // Add updated items
            foreach (var item in ItemsForReport)
            {
                // if (item.ItemId != 0 && item.Quantity > 0 && item.UnitPrice > 0)
                // {
                    item.ReceivingReportId = report.Id;
                    item.ItemId = item.ItemId; // Ensure the ItemId is set correctly

                    if (item.Comments == null)
                    item.Comments = "";

                    _context.ReceivingItems.Add(item);
                // }
            }

            await _context.SaveChangesAsync();

          
            return RedirectToPage("/Requests");

        }

        public async Task<IActionResult> OnGetGetNextSerialNumberAsync(string fiscalYear)
        {
            int lastSerial = await _context.ReceivingReports
                .Where(r => r.FiscalYear == fiscalYear)
                .MaxAsync(r => (int?)r.SerialNumber) ?? 0;

            return new JsonResult(new { serial = lastSerial + 1 });
        }

        private void FillLables()
        {

            this.lblCreateReport = (Program.Translations["CreateReport"])[Lang];
            this.lblFiscalYear = (Program.Translations["FiscalYear"])[Lang];
            this.lblSerialNumber = (Program.Translations["SerialNumber"])[Lang];
            this.lblReceivingDate = (Program.Translations["ReceivingDate"])[Lang];
            this.lblSectorNumber = (Program.Translations["SectorNumber"])[Lang];
            this.lblReceivingWarehouse = (Program.Translations["ReceivingWarehouse"])[Lang];
            this.lblBasedOnDocument = (Program.Translations["BasedOnDocument"])[Lang];
            this.lblDocumentNumber = (Program.Translations["DocumentNumber"])[Lang];
            this.lblDocumentDate = (Program.Translations["DocumentDate"])[Lang];
            this.lblAddAttachment = (Program.Translations["AddAttachment"])[Lang];
            this.lblSupplierType = (Program.Translations["SupplierType"])[Lang];
            this.lblSupplierName = (Program.Translations["SupplierName"])[Lang];
            this.lblItemGroup = (Program.Translations["ItemGroup"])[Lang];
            this.lblItemNo = (Program.Translations["ItemNo"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblItemDescription = (Program.Translations["ItemDescription"])[Lang];
            this.lblUnitOfMeasure = (Program.Translations["UnitOfMeasure"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblUnitPrice = (Program.Translations["UnitPrice"])[Lang];
            this.lblTotalPrice = (Program.Translations["TotalPrice"])[Lang];
            this.lblComments = (Program.Translations["Comments"])[Lang];
            this.lblRecipientID = (Program.Translations["RecipientID"])[Lang];
            this.lblRecipientName = (Program.Translations["RecipientName"])[Lang];
            this.lblTechnicalMember = (Program.Translations["TechnicalMember"])[Lang];
            this.lblChiefResponsible = (Program.Translations["ChiefResponsible"])[Lang];
            this.lblSubmitReport = (Program.Translations["SubmitReport"])[Lang];
            this.lblRecipientSector = (Program.Translations["RecipientSector"])[Lang];
            this.lblNewReceivingReport = (Program.Translations["NewReceivingReport"])[Lang];
            this.lblEditReceivingReport = (Program.Translations["EditReceivingReport"])[Lang];




        }

    }
}