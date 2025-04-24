using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class viewUserModel : BasePageModel
    {
        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
               
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
    }
}
