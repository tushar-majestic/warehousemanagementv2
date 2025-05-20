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
        public List<Item> Item { get; set; }
        public List<Unit> Units { get; set; }

        public List<Store> Stores { get; set; }
        public MaterialRequest? MaterialRequest { get; set; }

        public string lblView, lblUsers, lblSearch, lblAddUser, lblManageUserGroups, lblUserName, lblFullName, lblEmail,
        lblUserEnabled, lblIsLocked, lblUserType, lblUserGroupName, lblEdit, lblUnlock, lblTotalItem, lblJobNumber,
        lblEmpAffiliation, lblTransfer;
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
                this.Keeper = dbContext.Users.Where(u => u.UserId == MaterialRequest.KeeperId).Select(s => s.FullName).FirstOrDefault();
                this.GeneralSupervisor = dbContext.Users.Where(u => u.UserId == MaterialRequest.SupervisorId).Select(s => s.FullName).FirstOrDefault();
            }
            else
            {
                DispensedItems = new List<DespensedItem>();
            }


        }

        private void FillLables()
        {

            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblAddUser = (Program.Translations["AddUser"])[Lang];
            this.lblManageUserGroups = (Program.Translations["ManageUserGroups"])[Lang];
            this.lblUserName = (Program.Translations["UserName"])[Lang];
            this.lblFullName = (Program.Translations["FullName"])[Lang];
            this.lblEmail = (Program.Translations["Email"])[Lang];
            this.lblUserEnabled = (Program.Translations["UserEnabled"])[Lang];
            this.lblIsLocked = (Program.Translations["IsLocked"])[Lang];
            this.lblUserType = (Program.Translations["UserType"])[Lang];
            this.lblUserGroupName = (Program.Translations["UserGroupName"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblView = (Program.Translations["View"])[Lang];
            this.lblUnlock = (Program.Translations["Unlock"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblView = (Program.Translations["View"])[Lang];
            this.lblJobNumber = (Program.Translations["JobNumber"])[Lang];
            this.lblEmpAffiliation = (Program.Translations["EmpAffiliation"])[Lang];
            this.lblTransfer = (Program.Translations["Transfer"])[Lang];

        }
    }
    
}


