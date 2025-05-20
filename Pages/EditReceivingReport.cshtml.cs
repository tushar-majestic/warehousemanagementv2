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
            this.lblNewReceivingReport = (Program.Translations["NewReceivingReport"])[Lang];
            this.lblEditReceivingReport = (Program.Translations["EditReceivingReport"])[Lang];



           
        }

    }
}