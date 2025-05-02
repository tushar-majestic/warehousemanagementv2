using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LabMaterials.DB;
using System.Linq;

namespace LabMaterials.Pages
{
    public class ReceivingReportsModel : BasePageModel
    {
        private readonly LabDBContext _context;
        private readonly IWebHostEnvironment _environment;
        public ReceivingReportsModel(LabDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        [BindProperty]
        public IFormFile AttachmentFile { get; set; }
        public IList<Supplier> Suppliers { get; set; }
        public IList<Store> Warehouses { get; set; }
        public List<ReceivingReport> Reports { get; set; }
        public List<Item> Items { get; set; }
        public List<User> GeneralSupervisorList {  get; set; }

        public List<User> Users {  get; set; }

        public List<User> TechnicalMemberList {  get; set; }

        public List<Unit> Units { get; set; } 
        public List<ItemGroup> ItemGroupList { get; set; } 


        public int?  GeneralSupervisorId, TechnicalMemberId ;
        public int? ItemId;

        public string ItemNo;
        [BindProperty]
        public ReceivingReport Report { get; set; }  // <- change name from NewReport
        public string SupplierType => Report.Supplier?.SupplierType;

        [BindProperty]
        public List<ReceivingItem> ItemsForReport { get; set; } = new List<ReceivingItem>();

        public async Task OnGetAsync()
        {
            base.ExtractSessionData();
            var dbContext = new LabDBContext();
            Suppliers = _context.Suppliers.ToList();  // Fetch suppliers
            Warehouses = _context.Stores.ToList();  // Fetch suppliers
            // Reports = await _context.ReceivingReports.Include(r => r.Items).ToListAsync();
            Items = await _context.Items.ToListAsync();
            Report ??= new ReceivingReport();
            // **Important**: seed one blank ReceivingItem so index [0] exists
            ItemsForReport = new List<ReceivingItem> { new ReceivingItem() };
            // Ensure that session is available
            var userName = HttpContext.Session.GetString("UserName");

            // If the session is set, use it; otherwise, fallback to "Unknown"
            Report.CreatedBy = string.IsNullOrEmpty(userName) ? "Unknown" : userName;

            Units = dbContext.Units.ToList(); 
            ItemGroupList = dbContext.ItemGroup.ToList(); 
            Users = dbContext.Users.ToList(); 


            //General Supervisor of educational services list
            var GeneralSupervisorId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "General Supervisor of Educational Services")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            GeneralSupervisorList = dbContext.Users
                        .Where(u => u.UserGroupId == GeneralSupervisorId)
                        .ToList();

            //General Supervisor of educational services list
            var TechnicalMemberId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Technical Member")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            TechnicalMemberList = dbContext.Users
                        .Where(u => u.UserGroupId == TechnicalMemberId)
                        .ToList();

            // Debugging: log item count and details
            Console.WriteLine($"Item count: {Items?.Count}");
            foreach (var item in Items)
            {
                Console.WriteLine($"Item ID: {item.ItemId}, Name: {item.ItemName}");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Ensure CreatedBy is populated, for example, from the session or user context
            Report.CreatedBy = HttpContext.Session.GetString("UserName") ?? "Unknown";

           
            this.ItemNo = ItemNo;
            this.ItemId = ItemId;

            if (AttachmentFile != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder); // ensure it exists
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(AttachmentFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AttachmentFile.CopyToAsync(stream);
                }

                Report.AttachmentPath = "/uploads/" + uniqueFileName;
                ModelState.Remove("AttachmentPath"); // Removes the error for AttachmentPath

            }
            else
            {
                ModelState.AddModelError("AttachmentPath", "Attachment is required.");
                return Page();
            }


            //if (!ModelState.IsValid)
            //{
            //    base.ExtractSessionData();
            //    Items = await _context.Items.ToListAsync();
            //    return Page();
            //}

            _context.ReceivingReports.Add(Report);
            await _context.SaveChangesAsync();


            foreach (var item in ItemsForReport)
            {
                item.ReceivingReportId = Report.Id; // Ensure the ReceivingReportId is set correctly
                item.ItemId = item.ItemId; // Ensure the ItemId is set correctly

                ModelState.Remove("AttachmentPath"); // Removes the error for AttachmentPath

                _context.ReceivingItems.Add(item); // Add the item to the context
            }
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }


    }
}
