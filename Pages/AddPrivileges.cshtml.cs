using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class AddPrivilegesModel : BasePageModel
    {
        private readonly LabDBContext _context;

        public AddPrivilegesModel(LabDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Privilege Privilege { get; set; } = new();
        public List<Privilege> PrivilegeList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            base.ExtractSessionData();
            PrivilegeList = await _context.Privileges.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Privileges.Add(Privilege);
            await _context.SaveChangesAsync();

            return RedirectToPage("ManageUsers"); // Or your preferred redirect
        }
    }
}
