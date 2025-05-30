using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using LabMaterials.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        public List<ReturnRequestItem> ReturnItems { get; set; } = new List<ReturnRequestItem>();
        public List<SelectListItem> StateOfMatters { get; set; }
        [BindProperty]
        public ReturnRequest ReturnRequest { get; set; }
        public int ReturnRequestId { get; set; }
        public int InboxId { get; set; }

        [BindProperty]
        public ReturnRequest Report { get; set; }



        public async Task OnGetAsync()
        {
            base.ExtractSessionData();
            LoadDropdowns();

            int? ReturnRequestId = HttpContext.Session.GetInt32("ReturnRequestId");
            this.ReturnRequestId = ReturnRequestId.Value;

            int? InboxId =  HttpContext.Session.GetInt32("InboxId");
             if (InboxId.HasValue)
            {
                this.InboxId = InboxId.Value;
            }



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
            
           ReturnItems = await _context.ReturnRequestItems
                .Where(r => r.ReturnRequestId == ReturnRequestId)
                .ToListAsync();

           

        }

        public async Task<IActionResult> OnPostAsync()
        {
            base.ExtractSessionData();
            int? ReturnRequestId = HttpContext.Session.GetInt32("ReturnRequestId");
            this.ReturnRequestId = ReturnRequestId.Value;


            int? InboxId = HttpContext.Session.GetInt32("InboxId");
            if (InboxId.HasValue)
            {
                this.InboxId = InboxId.Value;
            }

            #pragma warning disable CS8601 // Possible null reference assignment.

            Report = await _context.ReturnRequests
                .Include(r => r.Items) // important to include items if you want to modify them
                .FirstOrDefaultAsync(r => r.Id == ReturnRequestId.Value);
            #pragma warning restore CS8601 // Possible null reference assignment.

           

            try
            {

            
            if (UserId == Report.CreatedBy)
            {
                var orderDateStr = Request.Form["OrderDate"];
                var reason = Request.Form["ReasonForReturn"];

                DateTime.TryParse(orderDateStr, out DateTime orderDate);
                Report.OrderDate = orderDate;
                Report.Reason = reason;

                // Ensure all flags are false first
                Report.IsSurplus = false;
                Report.IsExpired = false;
                Report.IsInvalid = false;
                Report.IsDamaged = false;

                switch (reason)
                {
                    case "SurPlus":
                        Report.IsSurplus = true;
                        break;
                    case "Expired":
                        Report.IsExpired = true;
                        break;
                    case "Invalid":
                        Report.IsInvalid = true;
                        break;
                    case "Damaged":
                        Report.IsDamaged = true;
                        break;
                }
                // Delete old items
                var existingItems = _context.ReturnRequestItems.Where(ri => ri.ReturnRequestId == Report.Id).ToList();
                _context.ReturnRequestItems.RemoveRange(existingItems);
                await _context.SaveChangesAsync();

                
                // Add updated items
                foreach (var item in ReturnItems)
                {
                        // if (item.ItemId != 0 && item.Quantity > 0 && item.UnitPrice > 0)
                        // {
                        // item.ReturnRequestId = 15;
                        // item.ItemCardId = 1111;
                        // item.ItemCode = "1245";
                        // item.ItemNameArabic = "Calcium Oxide";
                        // item.ItemNameEnglish = "Calcium Oxide";
                        // item.RiskRating = "Explosive";
                        // item.StateOfMatter = "Solid";
                        // item.UnitOfMeasure = "Rack";
                        // item.ReturnedQuantity = 2;
                        item.ReturnRequestId = Report.Id;
                        item.ItemCardId = item.ItemCardId;

                       

                        _context.ReturnRequestItems.Add(item);
                    // }
                }
                await _context.SaveChangesAsync();

            }
        }
        catch (Exception ex)
        {
            ErrorMsg = ex.Message;
            //for seeing inner exception errors
            if (ex.InnerException != null)
            {
                    ErrorMsg += " Inner Exception: " + ex.InnerException.Message;
            }
            Console.WriteLine("Error saving: " + ex.Message);
        }
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

                Report.InspOffApprovalDate = DateTime.UtcNow;

                if (InboxId.HasValue && InboxId.Value != 0)
                {
                    var message = _context.Messages.FirstOrDefault(m => m.Id == this.InboxId);

                    if (message != null)
                    {
                        message.Type = "Assign Supervisor";
                    }
                }


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

                var message = _context.Messages.FirstOrDefault(m => m.Id == this.InboxId);

                if (message != null)
                {
                    message.Type = "Added";
                }

                //find the keeper message to update it to show the add order to item card button
                var keeperMessage = _context.Messages
                    .FirstOrDefault(m => m.RecipientId == Report.KeeperId && m.ReturnRequestId == Report.Id);

                if (keeperMessage != null)
                {
                    keeperMessage.Type = "Add Order";
                }

            }
            await _context.SaveChangesAsync();
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