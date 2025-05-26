using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class ViewPrivilegesModel : BasePageModel
    {
        private readonly LabDBContext _context;

        public ViewPrivilegesModel(LabDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Privilege Privilege { get; set; } = new();
     
        [BindProperty]
        public int? EditId { get; set; } // Track which item is being edited
 
        public List<Privilege> PrivilegeList { get; set; } = new();
         public string lblUsers, lblPrivileges, lblAddPrivileges, lblBack, lblNoPrivilegesfound, lblPrivilegeName, lblID,lblActions;
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
            {
                PrivilegeList = await _context.Privileges.ToListAsync();
                return Page();
            }

            _context.Privileges.Add(Privilege);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var privilege = await _context.Privileges.FindAsync(id);
            if (privilege != null)
            {
                _context.Privileges.Remove(privilege);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditInitAsync()
        {
            base.ExtractSessionData();
            // Retain current list and set EditId for inline editing
            PrivilegeList = await _context.Privileges.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            base.ExtractSessionData();
            if (!ModelState.IsValid)
            {
                PrivilegeList = await _context.Privileges.ToListAsync();
                return Page();
            }

            var existing = await _context.Privileges.FindAsync(Privilege.PrivilegeId);
            if (existing != null)
            {
                existing.PrivilegeName = Privilege.PrivilegeName;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
           private void FillLables()
        {
            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblPrivileges = (Program.Translations["Privileges"])[Lang];
            this.lblAddPrivileges = (Program.Translations["AddPrivilege"])[Lang];
            this.lblBack = (Program.Translations["Back"])[Lang];
            this.lblNoPrivilegesfound = (Program.Translations["NoPrivilegesfound"])[Lang];
            this.lblPrivilegeName = (Program.Translations["PrivilegeName"])[Lang];
            this.lblID= (Program.Translations["ID"])[Lang];
            this.lblActions= (Program.Translations["Action"])[Lang];
        }
    }
}
