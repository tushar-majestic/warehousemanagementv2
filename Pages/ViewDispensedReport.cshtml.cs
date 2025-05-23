using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using LabMaterials.DB;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using System.Security.AccessControl;

namespace LabMaterials.Pages
{
    public class ViewDispensedReportModel : BasePageModel
    {
        private readonly LabDBContext _context;
        public string MaterialRequestId { get; set; }

        public string ReceivingWarehouse;
        public string RequestingSector;

        public string DeptManager;
        public string WarehouseManager;
        public string Keeper;
        public string GeneralSupervisor;

        public ViewDispensedReportModel(LabDBContext context)
        {
            _context = context;
        }

        public List<ReceivingReport> ReceivingReports { get; set; }
        public List<ReceivingItem> ReceivingItems { get; set; }
        public List<DespensedItem> DispensedItems { get; set; }
        public List<ItemCard> ItemCards { get; set; }
        public List<ItemCard> ItemCardsNew { get; set; }
        public List<Item> Item { get; set; }
        public List<Unit> Units { get; set; }

        public List<Store> Stores { get; set; }
        public MaterialRequest? MaterialRequest { get; set; }

        public string lblView, lblEdit, lblTotalItem, lblJobNumber, lblEmpAffiliation, lblTransfer, lblMaterialDispensing, lblPrint,
        lblSerialNumber, lblOrderDate, lblRequestingSector, lblRequestDocumentType, lblRequestDocumentNumber, lblSector, lblStoreName,
        lblManagerName, lblItemName, lblItemNo, lblCount, lblSAR, lblPageCount;
        // public void OnGet()
        // {

        //     base.ExtractSessionData();
        //     FillLables();
        // }

        public async Task OnGetAsync(int id)
        {
            base.ExtractSessionData();
            FillLables();

            this.MaterialRequestId = HttpContext.Session.GetString("MaterialRequestId");

            var dbContext = new LabDBContext();

            Stores = await dbContext.Stores.ToListAsync();
            ItemCards = await dbContext.ItemCards.ToListAsync();
            Units = await dbContext.Units.ToListAsync();

            MaterialRequest = await dbContext.MaterialRequests.FirstOrDefaultAsync(r => r.RequestId == int.Parse(this.MaterialRequestId));

            if (MaterialRequest != null)
            {
                DispensedItems = await dbContext.DespensedItems
                    .Where(r => r.MaterialRequestId == MaterialRequest.RequestId)
                    .ToListAsync();
                this.RequestingSector = dbContext.Destinations.Where(d => d.DId == MaterialRequest.RequestingSector).Select(s => s.DestinationName).FirstOrDefault();
                this.DeptManager = dbContext.Users.Where(u => u.UserId == MaterialRequest.DeptManagerId).Select(s => s.FullName).FirstOrDefault();
                this.WarehouseManager = dbContext.Users.Where(u => u.UserId == MaterialRequest.RequestedByUserId).Select(s => s.FullName).FirstOrDefault();
                this.Keeper = dbContext.Users.Where(u => u.UserId == MaterialRequest.KeeperId).Select(s => s.FullName).FirstOrDefault();
                this.GeneralSupervisor = dbContext.Users.Where(u => u.UserId == MaterialRequest.SupervisorId).Select(s => s.FullName).FirstOrDefault();
            }
            else
            {
                DispensedItems = new List<DespensedItem>();
            }


        }

        public IActionResult OnPostEdit([FromForm] int serialNumber, [FromForm] int ReceivingReportId)
        {
            HttpContext.Session.SetInt32("SerialNo", serialNumber);
            HttpContext.Session.SetInt32("ReceivingReportId", ReceivingReportId);

            return RedirectToPage("./EditDisbursement");
        }

        private void FillLables()
        {

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblView = (Program.Translations["View"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblJobNumber = (Program.Translations["JobNumber"])[Lang];
            this.lblEmpAffiliation = (Program.Translations["EmpAffiliation"])[Lang];
            this.lblTransfer = (Program.Translations["Transfer"])[Lang];
            this.lblMaterialDispensing = (Program.Translations["Disbursements"])[Lang];
            this.lblPrint = (Program.Translations["Print"])[Lang];
            this.lblSerialNumber = (Program.Translations["SerialNumber"])[Lang];
            this.lblFiscalYear = (Program.Translations["FiscalYear"])[Lang];
            this.lblOrderDate = (Program.Translations["OrderDate"])[Lang];
            this.lblRequestingSector = (Program.Translations["RequestingSector"])[Lang];
            this.lblRequestDocumentType = (Program.Translations["RequestDocumentType"])[Lang];
            this.lblRequestDocumentNumber = (Program.Translations["RequestDocumentNumber"])[Lang];
            this.lblSector = (Program.Translations["Sector"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblManagerName = (Program.Translations["ManagerName"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblItemNo = (Program.Translations["ItemNo"])[Lang];
            this.lblCount = (Program.Translations["Count"])[Lang];
            this.lblSAR = (Program.Translations["SAR"])[Lang];
            this.lblPageCount = (Program.Translations["PageCount"])[Lang];

        }
    }
    
}


