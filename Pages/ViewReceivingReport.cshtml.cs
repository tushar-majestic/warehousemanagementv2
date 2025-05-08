using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LabMaterials.Pages
{
    public class ViewReceivingReportModel : BasePageModel
    {
        private readonly LabDBContext _context;
        public string ReportId { get; set; }

        public string ReceivingWarehouse;
        public ViewReceivingReportModel(LabDBContext context)
        {
            _context = context;
        }

        public List<ReceivingReport> ReceivingReports { get; set; }

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

            Report = await _context.ReceivingReports
                    .Include(r => r.Supplier)
                        .FirstOrDefaultAsync(r => r.Id == int.Parse(this.ReportId));

            #pragma warning disable CS8601
                        this.ReceivingWarehouse = dbContext.Stores.Where(s => s.StoreId == int.Parse(Report.ReceivingWarehouse)).Select(s => s.StoreName)
                        .FirstOrDefault();
            #pragma warning restore CS8601 

            // ReceivingReports = await _context.ReceivingReports
            //     .Include(r => r.Supplier)
            //     .ToListAsync();
        }
    }
}
