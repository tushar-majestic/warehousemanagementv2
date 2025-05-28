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

            //if Inspection Commitee Officer is logged in than he can only add the recommended action and additional notes
            if (UserGroupName == "Return Inspection Committee Officer")
            {
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
            }
            //if Recycling Officer is logged in than he can only Recyling Notes for the specific items.
            else if (UserGroupName == "Recycling Officer")
            {
                for (int i = 0; i < ReturnItems.Count; i++)
                {
                    var item = ReturnItems[i];
                    var notesKey = $"ReturnItems[{i}].RecyclingNotes";

                    var notesValue = Request.Form[notesKey];

                    if (!string.IsNullOrEmpty(notesValue))
                    {
                        item.RecyclingNotes = notesValue;
                    }


                }

                await dbContext.SaveChangesAsync();
                var message = dbContext.Messages.FirstOrDefault(m => m.Id == this.InboxId);

                if (message != null)
                {
                    message.Type = "Added";
                }
                dbContext.SaveChanges();

                //find the keeper message to update it to show the add order to item card button
                var keeperMessage = dbContext.Messages
                    .FirstOrDefault(m => m.RecipientId == Report.KeeperId && m.ReturnRequestId == Report.Id);

                if (keeperMessage != null)
                {
                    keeperMessage.Type = "Add Order";
                }
                dbContext.SaveChanges();

            }

           


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