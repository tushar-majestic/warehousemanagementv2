using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class ViewReceivingReportModel : BasePageModel
    {
        private readonly LabDBContext _context;

        public ViewReceivingReportModel(LabDBContext context)
        {
            _context = context;
        }

        public List<ReceivingReport> ReceivingReports { get; set; }

        public async Task OnGetAsync()
        {
            base.ExtractSessionData();
            ReceivingReports = await _context.ReceivingReports
                .Include(r => r.Supplier)
                .ToListAsync();
        }
    }
}
