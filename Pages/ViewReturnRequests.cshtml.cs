using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class ViewReturnRequestsModel : BasePageModel
    {


        private readonly LabDBContext _context;

        public ViewReturnRequestsModel(LabDBContext context)
        {
            _context = context;
        }

        public List<ReturnRequest> ReturnRequests { get; set; }

        public async Task OnGetAsync()
        {
            base.ExtractSessionData();
            ReturnRequests = await _context.ReturnRequests
                .Include(r => r.Warehouse)
                .Include(r => r.FromSector)
                .OrderByDescending(r => r.OrderDate)
                .ToListAsync();
        }
    }
}



