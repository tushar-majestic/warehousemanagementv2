using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LabMaterials.Pages
{
    public class ViewReceivingReportModel : BasePageModel
    {
        private readonly LabDBContext _context;
        public string ReportId { get; set; }

        public string ReceivingWarehouse;
        public string ReceipientManager;

        public string TechnicalMember;
        public string GeneralSupervisor;

        public ViewReceivingReportModel(LabDBContext context)
        {
            _context = context;
        }

        public List<ReceivingReport> ReceivingReports { get; set; }
        public List<ReceivingItem> ReceivingItems {get; set;}
        public List<Item> Items {get; set;}
        public List<Unit> Units {get; set;}

        public List<Store> Stores {get; set;}
        public ReceivingReport? Report { get; set; }
        public async Task OnGetAsync(int id)
        {
            base.ExtractSessionData();
            this.ReportId = HttpContext.Session.GetString("ReportId");
            if(ReportId == null){
                Report = null;
                return;
            }
            var dbContext = new LabDBContext();
            Stores = dbContext.Stores.ToList();
            Items = dbContext.Items.ToList();
            Units =  dbContext.Units.ToList();

            Report = await _context.ReceivingReports
                    .Include(r => r.Supplier)
                        .FirstOrDefaultAsync(r => r.Id == int.Parse(this.ReportId));

            ReceivingItems = dbContext.ReceivingItems.Where(r => r.ReceivingReportId == Report.Id).ToList();

            #pragma warning disable CS8601
                this.ReceivingWarehouse = dbContext.Stores.Where(s => s.StoreId == int.Parse(Report.ReceivingWarehouse)).Select(s => s.StoreName)
                        .FirstOrDefault();
           
                this.ReceipientManager = dbContext.Users.Where(u => u.JobNumber == Report.RecipientEmployeeId).Select(s => s.FullName).FirstOrDefault();

                this.TechnicalMember = dbContext.Users.Where(u => u.UserId == Report.TechnicalMemberId).Select(s => s.FullName).FirstOrDefault();

                var GeneralSupervisor = dbContext.Users.FirstOrDefault(u => u.UserId == Report.ChiefResponsibleId);

                if(GeneralSupervisor != null){
                    this.GeneralSupervisor = GeneralSupervisor.FullName;
                }
            #pragma warning restore CS8601 


            // ReceivingReports = await _context.ReceivingReports
            //     .Include(r => r.Supplier)
            //     .ToListAsync();
        }
    }
}
