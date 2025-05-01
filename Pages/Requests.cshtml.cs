using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LabMaterials.Pages
{
    public class RequestsModel : BasePageModel
    {
        private readonly LabDBContext _context;

        public RequestsModel(LabDBContext context)
        {
            _context = context;
        }

        // Properties for Inbox and Outbox
        public List<MaterialRequest> PendingRequests { get; set; } = new();
        public List<MaterialRequest> UserRequests { get; set; } = new();

        public List<Item> materials { get; set; }
        public SelectList ItemList { get; set; }
        [BindProperty]
        public MaterialRequest NewRequest { get; set; } = new MaterialRequest();  // To hold data for Create
        public MaterialRequest EditRequest { get; set; } = new MaterialRequest(); // For Edit

        public int MaterialId { get; set; }
        [BindProperty]
        public int RequestedByUserId { get; set; }
        public string RequestedByUserI { get; set; }

        public Item Material { get; set; }  // Navigation property (optional)

        public async Task OnGetAsync()
        {
            base.ExtractSessionData();

            // Populate the item list for dropdown
            var items = await _context.Items.OrderBy(i => i.ItemName).ToListAsync();
            ItemList = new SelectList(items, "ItemName", "ItemName");

            // Retrieve user ID from session and store it in the property
            var userId = HttpContext.Session.GetInt32("UserId");
            RequestedByUserId = userId ?? 0;
            RequestedByUserI = HttpContext.Session.GetString("UserName");
            // Fetch pending requests for approval (Inbox)
            PendingRequests = await _context.MaterialRequests
                .Where(r => r.CurrentApproverUserId == userId && r.Status == "Pending")
                .Include(r => r.RequestedByUser)
                .ToListAsync();

            // Fetch all requests initiated by the user (Outbox)
            UserRequests = await _context.MaterialRequests
                .Where(r => r.RequestedByUserId == userId)
                .ToListAsync();

            if (CanManageStore)
            {
                // Additional logic for users who can manage the store (if needed)
            }
            else
            {
                // Redirect to another page if the user doesn't have permission
                RedirectToPage("./Index?lang=" + Lang);
            }
        }


        // Create - Post Request
        public async Task<IActionResult> OnPostCreateAsync()
        {
            base.ExtractSessionData();
            //if (!ModelState.IsValid)
            //{
            //    // Log validation errors for debugging
            //    foreach (var modelState in ModelState)
            //    {
            //        foreach (var error in modelState.Value.Errors)
            //        {
            //            Console.WriteLine($"Field: {modelState.Key}, Error: {error.ErrorMessage}");
            //        }
            //    }

            //    // Re-populate the dropdown and return the page
            //    var items = await _context.Items.OrderBy(i => i.ItemName).ToListAsync();
            //    ItemList = new SelectList(items, "ItemName", "ItemName");
            //    return Page();
            //}

            // Retrieve user ID from session and set the RequestedByUserId for the new request
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // Handle error (redirect to login page, etc.)
                return RedirectToPage("/Login");
            }

            NewRequest.RequestedByUserId = userId.Value;
            NewRequest.RequestedDate = DateTime.Now;

            // Optionally load the user object to ensure RequestedByUser is assigned
            var user = await _context.Users.FindAsync(userId.Value);
            if (user != null)
            {
                NewRequest.RequestedByUser = user;  // Ensure the RequestedByUser navigation property is set
            }

            // Add new request to the database
            _context.MaterialRequests.Add(NewRequest);
            await _context.SaveChangesAsync();

            // Redirect to the page after saving the request
            return RedirectToPage();
        }




        // Action to approve a request
        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var request = await _context.MaterialRequests.FindAsync(id);

            if (request != null)
            {
                // Approve the request and move it to the next approver
                request.Status = "Approved";
                request.CurrentApproverUserId = GetNextApprover(request);

                _context.MaterialRequests.Update(request);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(); // Refresh the page
        }

        // Action to reject a request
        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var request = await _context.MaterialRequests.FindAsync(id);

            if (request != null)
            {
                // Reject the request
                request.Status = "Rejected";

                _context.MaterialRequests.Update(request);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(); // Refresh the page
        }

        private int? GetUserIdByRole(string role)
        {
            return _context.Users
                .Where(u => u.UserGroup.UserGroupName == role)
                .Select(u => (int?)u.UserId)
                .FirstOrDefault(); // or .SingleOrDefault if only one exists
        }


        // Helper method to get the next approver
        private int? GetNextApprover(MaterialRequest request)
        {
            // Ensure RequestedByUser is loaded
            var requester = _context.Users.FirstOrDefault(u => u.UserId == request.RequestedByUserId);

            if (requester == null)
                return null;

            // Example logic based on user's role/group
            switch (requester.UserGroup.UserGroupName) // or requester.GroupId if that's what you use
            {
                case "Warehouse keeper":
                    return GetUserIdByRole("Technical Member");
                case "Technical Member":
                    return GetUserIdByRole("Warehouse Manager");
                case "Warehouse Manager":
                    return GetUserIdByRole("General Supervisor");
                default:
                    return null;
            }
        }

    }

}
