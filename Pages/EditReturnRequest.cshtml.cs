using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace LabMaterials.Pages
{
    public class EditReturnRequestModel : BasePageModel
    {
        private readonly LabDBContext _context;

        public EditReturnRequestModel(LabDBContext context)
        {
            _context = context;
        }
        // --- Dropdown Data ---
        public List<ItemGroup> ItemGroups { get; set; } = new();
        public List<ItemCard> ItemCards { get; set; } = new();
        public List<Store> Stores { get; set; } = new();
        public List<Requester> requesters { get; set; } = new();
        public List<Item> AllItems { get; set; }
        [BindProperty]
        public List<ReturnRequestItem> ReturnItems { get; set; } = new();
        public List<SelectListItem> StateOfMatters { get; set; }
        [BindProperty]
        public ReturnRequest ReturnRequest { get; set; }
        public int ReturnRequestId { get; set; }

        [BindProperty]
        public ReturnRequest Report { get; set; }



        public void OnGet()
        {
            base.ExtractSessionData();
            int? ReturnRequestId = HttpContext.Session.GetInt32("ReturnRequestId");
            this.ReturnRequestId = ReturnRequestId.Value;

          


            StateOfMatters = new List<SelectListItem>
            {
                new SelectListItem { Text = "Solid", Value = "Solid" },
                new SelectListItem { Text = "Liquid", Value = "Liquid" },
                new SelectListItem { Text = "Gas", Value = "Gas" }
            };

            var dbContext = new LabDBContext();
            #pragma warning disable CS8601 // Possible null reference assignment.
                        Report = dbContext.ReturnRequests
                            .FirstOrDefault(r => r.Id == ReturnRequestId.Value);
            #pragma warning restore CS8601 // Possible null reference assignment.
            
            ReturnItems = dbContext.ReturnRequestItems
                        .Where(r => r.ReturnRequestId == ReturnRequestId.Value)
                        .ToList();

           

            LoadDropdowns();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            base.ExtractSessionData();
            int? ReturnRequestId = HttpContext.Session.GetInt32("ReturnRequestId");
            this.ReturnRequestId = ReturnRequestId.Value;

             var dbContext = new LabDBContext();
            #pragma warning disable CS8601 // Possible null reference assignment.
            Report = dbContext.ReturnRequests
                            .FirstOrDefault(r => r.Id == ReturnRequestId.Value);
            #pragma warning restore CS8601 // Possible null reference assignment.
           
            ReturnItems = dbContext.ReturnRequestItems
                        .Where(r => r.ReturnRequestId == ReturnRequestId.Value)
                        .ToList();

            foreach (var item in ReturnItems)
            {

                var itemToUpdate = ReturnItems.FirstOrDefault(x => x.Id == item.Id);
                if (itemToUpdate != null)
                {
                    itemToUpdate.RecommendedAction = item.RecommendedAction;
                    itemToUpdate.Notes = item.Notes;
                }
            }
            await dbContext.SaveChangesAsync();

            return RedirectToPage("/Requests");
        }

        private void LoadDropdowns()
        {
            ItemGroups = _context.ItemGroups.ToList();
            ItemCards = _context.ItemCards.ToList();
            Stores = _context.Stores.ToList();
            requesters = _context.Requesters.ToList();
            AllItems = _context.Items.ToList();
        }
    }
}