using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class BasePageModel : PageModel
    {
        public string FullName { get; set; }
        public int? UserId { get; set; }
        public bool IsLDAP { get; set; }
        public bool CanManageStore { get; set; }
        public bool CanManageUsers { get; set; }
        public bool CanManageItems { get; set; }
        public bool CanManageSupplies { get; set; }
        public bool CanDisburseItems { get; set; }
        public bool CanSeeReports { get; set; }
        public bool CanManageItemGroup {  get; set; }
        public string dir { get; set; } = "rtl";
        public string Lang { get; set; } = "ar";
        public string lblLabMaterials, lblHome, lblDisbursement, lblReports, lblManageUsers, lblManageItems, lblManageSupplies, lblManageStores, lblChangePassword, lblLogout, lblDamagedItems, lblLanguage;

        public void ExtractSessionData()
        {

            if (HttpContext.Session.Keys.Contains("UserId"))
            {
                UserId = HttpContext.Session.GetInt32("UserId");
                FullName = HttpContext.Session.GetString("FullName");
                IsLDAP = HttpContext.Session.GetInt32("IsLDAP") == 1;
                CanManageStore = HttpContext.Session.GetInt32("CanManageStore") == 1;
                CanManageUsers = HttpContext.Session.GetInt32("CanManageUsers") == 1;
                CanManageItems = HttpContext.Session.GetInt32("CanManageItems") == 1;
                CanManageSupplies = HttpContext.Session.GetInt32("CanManageSupplies") == 1;
                CanDisburseItems = HttpContext.Session.GetInt32("CanDisburseItems") == 1;
                CanSeeReports = HttpContext.Session.GetInt32("CanSeeReports") == 1;
                CanManageItemGroup = HttpContext.Session.GetInt32("CanManageItemGroup") == 1;
                dir = HttpContext.Session.GetString("Lang") == "en" ? "ltr" : "rtl";
                Lang = HttpContext.Session.GetString("Lang") == "en" ? "en" : "ar";
                FillLables();
                
            }
            else
            {
                HttpContext.Response.Redirect("/Index?lang=" + Lang);
            }
             
        }

        private void FillLables()
        {
            

            this.lblDisbursement = (Program.Translations["Disbursements"])[Lang];
            this.lblReports = (Program.Translations["Reports"])[Lang];
            this.lblManageItems = (Program.Translations["ManageItems"])[Lang];
            this.lblManageStores = (Program.Translations["ManageStore"])[Lang];
            this.lblManageSupplies = (Program.Translations["Supplies"])[Lang];
            this.lblManageUsers = (Program.Translations["ManageUsers"])[Lang];
            this.lblHome = (Program.Translations["Home"])[Lang];
            this.lblChangePassword = (Program.Translations["ChangePassword"])[Lang];
            this.lblLogout = (Program.Translations["Logout"])[Lang];
            this.lblLabMaterials = (Program.Translations["LabMaterials"])[Lang]; 
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblLanguage = (Program.Translations["Language"])[Lang];

        }
    }
}
