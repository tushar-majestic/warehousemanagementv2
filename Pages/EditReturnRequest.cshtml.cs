using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace LabMaterials.Pages
{
    public class EditReturnRequestModel : BasePageModel
    {
        private readonly LabDBContext _context;
        public string ErrorMsg { get; set; }


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
        public List<ReturnRequestItem> ReturnItems { get; set; } =  new List<ReturnRequestItem>();
        public List<SelectListItem> StateOfMatters { get; set; }
        [BindProperty]
        public ReturnRequest ReturnRequest { get; set; }
        public int ReturnRequestId { get; set; }
        public int InboxId { get; set; }

        [BindProperty]
        public ReturnRequest Report { get; set; }



        public void OnGet()
        {
            base.ExtractSessionData();
            int? ReturnRequestId = HttpContext.Session.GetInt32("ReturnRequestId");
            this.ReturnRequestId = ReturnRequestId.Value;

            int? InboxId =  HttpContext.Session.GetInt32("InboxId");
            this.InboxId = InboxId.Value;



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

            int? InboxId =  HttpContext.Session.GetInt32("InboxId");
            this.InboxId = InboxId.Value;

            var dbContext = new LabDBContext();
            #pragma warning disable CS8601 // Possible null reference assignment.
            Report = dbContext.ReturnRequests
                            .FirstOrDefault(r => r.Id == ReturnRequestId.Value);
            #pragma warning restore CS8601 // Possible null reference assignment.
           
            ReturnItems = dbContext.ReturnRequestItems
                        .Where(r => r.ReturnRequestId == ReturnRequestId.Value)
                        .ToList();

            // foreach (var item in ReturnItems)
            // {

            //     var itemToUpdate = ReturnItems.FirstOrDefault(x => x.Id == item.Id);
            //     if (itemToUpdate != null)
            //     {
            //         // itemToUpdate.RecommendedAction = item.RecommendedAction;
            //         itemToUpdate.Notes = item.Notes;
            //     }
            //     else
            //     {
            //         ErrorMsg = "Items to update is null";
            //         return Page();
            //     }
            // }
            for (int i = 0; i < ReturnItems.Count; i++)
            {
                var item = ReturnItems[i];
                var notesKey = $"ReturnItems[{i}].Notes";
                var actionKey = $"ReturnItems[{i}].RecommendedAction";

                var actionValue = Request.Form[actionKey];
                var notesValue = Request.Form[notesKey];

                if (!string.IsNullOrEmpty(notesValue))
                {
                    item.Notes = notesValue;
                }

                if (!string.IsNullOrEmpty(actionValue) &&
                    Enum.TryParse(typeof(LabMaterials.DB.ReturnRequestItem.ItemCondition), actionValue, out var parsedAction))
                {
                    item.RecommendedAction = (LabMaterials.DB.ReturnRequestItem.ItemCondition)parsedAction;
                }
            }

            await dbContext.SaveChangesAsync();
            Report.InspOffApprovalDate = DateTime.UtcNow;

          

            var message = dbContext.Messages.FirstOrDefault(m => m.Id == this.InboxId);

            if (message != null)
            {
                message.Type = "Assign Supervisor";
            }
            dbContext.SaveChanges();


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