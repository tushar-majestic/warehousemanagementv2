using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LabMaterials.DB;
using System.Linq;
using LabMaterials.DB;
using LabMaterials.dtos;


namespace LabMaterials.Pages
{
    public class ReceivingReportsModel : BasePageModel
    {
        private readonly LabDBContext _context;
        private readonly IWebHostEnvironment _environment;
        public string lblCreateReport, lblSerialNumber, lblFiscalYear, lblReceivingDate, lblSectorNumber, lblReceivingWarehouse, lblBasedOnDocument, lblDocumentNumber, lblDocumentDate, lblAddAttachment, lblSupplierType, lblSupplierName, lblItemGroup, lblItemNo, lblItemName, lblItemDescription, lblUnitOfMeasure, lblQuantity, lblUnitPrice, lblTotalPrice, lblComments, lblRecipientID, lblRecipientName, lblTechnicalMember, lblChiefResponsible, lblSubmitReport, lblRecipientSector;
        public ReceivingReportsModel(LabDBContext context, IWebHostEnvironment environment)
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

        public string ItemNo;
        [BindProperty]
        public ReceivingReport Report { get; set; }  // <- change name from NewReport
        public string SupplierType => Report.Supplier?.SupplierType;

        [BindProperty]
        public List<ReceivingItem> ItemsForReport { get; set; } = new List<ReceivingItem>();

        public async Task OnGetAsync()
        {
            base.ExtractSessionData();
            FillLables();
            var dbContext = new LabDBContext();
            Suppliers = _context.Suppliers.ToList();  // Fetch suppliers
            Warehouses = _context.Stores.ToList();  // Fetch suppliers
            // Reports = await _context.ReceivingReports.Include(r => r.Items).ToListAsync();
            Items = await _context.Items.ToListAsync();
            Report ??= new ReceivingReport();
            // **Important**: seed one blank ReceivingItem so index [0] exists
            ItemsForReport = new List<ReceivingItem> { new ReceivingItem() };
            // Ensure that session is available
            var userName = HttpContext.Session.GetString("UserName");

            // If the session is set, use it; otherwise, fallback to "Unknown"
            Report.CreatedBy = string.IsNullOrEmpty(userName) ? "Unknown" : userName;

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

       public async Task<IActionResult> OnGetGetNextSerialNumberAsync(string fiscalYear)
        {
            int lastSerial = await _context.ReceivingReports
                .Where(r => r.FiscalYear == fiscalYear)
                .MaxAsync(r => (int?)r.SerialNumber) ?? 0;

            return new JsonResult(new { serial = lastSerial + 1 });
        }


        public async Task<IActionResult> OnPostAsync(DateTime ReceivingDate, DateTime DocumentDate, [FromForm] int TechnicalMember, [FromForm] int ChiefResponsible,  [FromForm] string FiscalYear,  [FromForm] string BasedOnDocument, [FromForm] int SerialNumber, [FromForm] string RecipientEmployeeName)
        {   ModelState.Clear();

            base.ExtractSessionData();
            FillLables();
            Report.CreatedBy = HttpContext.Session.GetString("FullName") ?? "Unknown";

            var dbContext = new LabDBContext();

            this.ItemNo = ItemNo;
            this.ItemId = ItemId;
            Report.ReceivingDate = ReceivingDate;
            Warehouses = _context.Stores.ToList();
            Users = dbContext.Users.ToList(); 
            Suppliers = _context.Suppliers.ToList();  
            Items = await _context.Items.ToListAsync();
            Units = dbContext.Units.ToList(); 
            ItemGroupList = dbContext.ItemGroup.ToList(); 
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

            TechnicalMemberList = dbContext.Users
                        .Where(u => u.UserGroupId == TechnicalMemberId)
                        .ToList();

            Report.ReceivingDate = ReceivingDate.Date;
            Report.DocumentDate  = DocumentDate.Date;
            Report.TechnicalMemberId = TechnicalMember;
            Report.ChiefResponsibleId = ChiefResponsible;
            Report.FiscalYear = FiscalYear;
            Report.BasedOnDocument = BasedOnDocument;
            Report.SerialNumber = SerialNumber;

            Report.FiscalYear = FiscalYear;
            Report.KeeperApproval = true;

            this.RecipientEmployeeName = RecipientEmployeeName;
           
            var TechnicalMemberName = dbContext.Users
                    .Where(u => u.UserId == Report.TechnicalMemberId)
                    .Select(u => u.FullName)
                    .FirstOrDefault();


            if (string.IsNullOrEmpty(FiscalYear)){
                ErrorMsg = (Program.Translations["FiscalYearMissing"])[Lang];
                return Page();
            }
            else if (ReceivingDate == default(DateTime))
            {
                ErrorMsg = (Program.Translations["ReceivingDateMissing"])[Lang];
                return Page();
            }
            else if (string.IsNullOrEmpty(Report.RecipientSector))
            {
                ErrorMsg = (Program.Translations["RecipientSectorMissing"])[Lang];
                return Page();
            }
            else if (string.IsNullOrEmpty(Report.SectorNumber))
            {
                ErrorMsg = (Program.Translations["SectorNumberMissing"])[Lang];
                return Page();
            }
            else if (string.IsNullOrEmpty(Report.ReceivingWarehouse))
            {
                ErrorMsg = (Program.Translations["ReceivingWarehouseMissing"])[Lang];
                return Page();
            }
            else if (string.IsNullOrEmpty(Report.BasedOnDocument))
            {
                ErrorMsg = (Program.Translations["BasedOnDocumentMissing"])[Lang];
                return Page();
            }
            else if (string.IsNullOrEmpty(Report.DocumentNumber))
            {
                ErrorMsg = (Program.Translations["DocumentNumberMissing"])[Lang];
                return Page();
            }
            else if (DocumentDate == default(DateTime))
            {
                ErrorMsg = (Program.Translations["DocumentDateMissing"])[Lang];
                return Page();
            }
            else if(Report.SupplierId==0){
                ErrorMsg = (Program.Translations["SupplierMissing"])[Lang];
                return Page();
            }
            else if(Report.RecipientEmployeeId==0){
                ErrorMsg = (Program.Translations["ReceipientMissing"])[Lang];
                return Page();
            }
            
            else if(Report.TechnicalMemberId==0){
                ErrorMsg = (Program.Translations["TechnicalMemberMissing"])[Lang];
                return Page();
            }
           
           
           
            if (AttachmentFile != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder); // ensure it exists
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(AttachmentFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AttachmentFile.CopyToAsync(stream);
                }

                Report.AttachmentPath = "/uploads/" + uniqueFileName;
                ModelState.Remove("AttachmentPath"); // Removes the error for AttachmentPath

            }
               
            
                
            


            //if (!ModelState.IsValid)
            //{
            //    base.ExtractSessionData();
            //    Items = await _context.Items.ToListAsync();
            //    return Page();
            //}

            _context.ReceivingReports.Add(Report);
            await _context.SaveChangesAsync();


            foreach (var item in ItemsForReport)
            {
                // if (item.ItemId == 0 || item.Quantity <= 0 || item.UnitPrice <= 0)
                // {
                //     ErrorMsg = "Please ensure all item details are filled correctly.";
                //     return Page();
                // }
                item.ReceivingReportId = Report.Id; // Ensure the ReceivingReportId is set correctly
                item.ItemId = item.ItemId; // Ensure the ItemId is set correctly

                ModelState.Remove("AttachmentPath"); // Removes the error for AttachmentPath

                _context.ReceivingItems.Add(item); // Add the item to the context
            }
            await _context.SaveChangesAsync();

            string Message = string.Format("Sent Request for Items. Approve the request or add comments.");
            var msg = new  Message
            {
                ReceivingReportId = Report.Id,
                Sender = Report.CreatedBy,
                Recipient = TechnicalMemberName,
                Content = Message
            };
            dbContext.Messages.Add(msg);
            dbContext.SaveChanges();


            return RedirectToPage("/Requests");
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
            this.lblDocumentNumber= (Program.Translations["DocumentNumber"])[Lang];
            this.lblDocumentDate= (Program.Translations["DocumentDate"])[Lang];
            this.lblAddAttachment = (Program.Translations["AddAttachment"])[Lang];
            this.lblSupplierType = (Program.Translations["SupplierType"])[Lang];
            this.lblSupplierName = (Program.Translations["SupplierName"])[Lang];
            this.lblItemGroup = (Program.Translations["ItemGroup"])[Lang];
            this.lblItemNo = (Program.Translations["ItemNo"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblItemDescription =  (Program.Translations["ItemDescription"])[Lang];
            this.lblUnitOfMeasure = (Program.Translations["UnitOfMeasure"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblUnitPrice = (Program.Translations["UnitPrice"])[Lang];
            this.lblTotalPrice = (Program.Translations["TotalPrice"])[Lang];
            this.lblComments = (Program.Translations["Comments"])[Lang];
            this.lblRecipientID = (Program.Translations["RecipientID"])[Lang];
            this.lblRecipientName = (Program.Translations["RecipientName"])[Lang];
            this.lblTechnicalMember = (Program.Translations["TechnicalMember"])[Lang];
            this.lblChiefResponsible = (Program.Translations["ChiefResponsible"])[Lang];
            this.lblSubmitReport =  (Program.Translations["SubmitReport"])[Lang];
            this.lblRecipientSector =  (Program.Translations["RecipientSector"])[Lang];


           
        }
    
    }
}
