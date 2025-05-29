using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        [BindProperty]
        public List<ReturnRequestItem> ReturnRequestItems { get; set; } = new();

        public List<SelectListItem> ItemConditionList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            base.ExtractSessionData();
            ReturnRequest = await _context.ReturnRequests
                .Include(r => r.Warehouse)
                .Include(r => r.Items)
                .Include(r => r.FromSector)
                .FirstOrDefaultAsync(r => r.Id == id);



            ItemConditionList = Enum.GetValues(typeof(ReturnRequestItem.ItemCondition))
            .Cast<ReturnRequestItem.ItemCondition>()
            .Select(e => new SelectListItem
            {
                Text = e.ToString(),
                Value = ((int)e).ToString()
            }).ToList();


            if (ReturnRequest == null)
                return NotFound();

            ReturnRequestItems = ReturnRequest.Items.ToList();
            return Page();
        }
        
         public IActionResult OnPostEditReturnRequest([FromForm] int ReturnRequestId)
        {



            HttpContext.Session.SetInt32("ReturnRequestId", ReturnRequestId);
            HttpContext.Session.SetInt32("InboxId", 0);

            return RedirectToPage("./EditReturnRequest");




        }
    }
}



