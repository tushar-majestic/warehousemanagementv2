using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class ViewReceivingReportModel : BasePageModel
    {
        private readonly LabDBContext _context;
        public string ReportId { get; set; }


        public ViewReceivingReportModel(LabDBContext context)
        {
            _context = context;
        }

        public List<ReceivingReport> ReceivingReports { get; set; }
        public ReceivingReport? Report { get; set; }
        public async Task OnGetAsync(int id)
        {
            base.ExtractSessionData();
            this.ReportId = HttpContext.Session.GetString("ReportId");
            if(ReportId == null){
                Report = null;
                return;
            }

               Report = await _context.ReceivingReports
                    .Include(r => r.Supplier)
                        .FirstOrDefaultAsync(r => r.Id == int.Parse(this.ReportId));

            // ReceivingReports = await _context.ReceivingReports
            //     .Include(r => r.Supplier)
            //     .ToListAsync();
        }
    }
}
