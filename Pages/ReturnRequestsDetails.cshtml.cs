using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class ReturnRequestsDetailsModel : BasePageModel
    {

        private readonly LabDBContext _context;

        public ReturnRequestsDetailsModel(LabDBContext context)
        {
            _context = context;
        }

        public ReturnRequest? ReturnRequest { get; set; }
        public List<ReturnRequestItem> ReturnRequestItems { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            base.ExtractSessionData();
            ReturnRequest = await _context.ReturnRequests
                .Include(r => r.Warehouse)
                .Include(r => r.Items)
                .Include(r => r.FromSector)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ReturnRequest == null)
                return NotFound();

            ReturnRequestItems = ReturnRequest.Items.ToList();
            return Page();
        }
    }
}



