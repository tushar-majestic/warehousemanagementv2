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
        public string lblUsers, lblPrivileges, lblAddPrivileges, lblBack, lblPrivilegeName;
        public async Task<IActionResult> OnGetAsync()
        {
            FillLables();
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

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var privilege = await _context.Privileges.FindAsync(id);
            if (privilege != null)
            {
                _context.Privileges.Remove(privilege);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(); // Refresh the list
        }
        private void FillLables()
        {
            

            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblPrivileges = (Program.Translations["Privileges"])[Lang];
            this.lblAddPrivileges = (Program.Translations["AddPrivilege"])[Lang];
            this.lblBack = (Program.Translations["Back"])[Lang];
            this.lblPrivilegeName = (Program.Translations["PrivilegeName"])[Lang];
        }

    }
}
