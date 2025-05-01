using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client.Extensions.Msal;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions;

namespace LabMaterials.Pages
{
    public class NotificationsModel : BasePageModel
    {
        public string lblNotifications, pagetype = "inbox", inboxClass = "btn-dark text-white", outboxClass = "btn-light";

        public void OnGet()
        {
            if (HttpContext.Request.Query.ContainsKey("type"))
            {
                pagetype = HttpContext.Request.Query["type"];

                if(pagetype == "inbox")
                {
                    inboxClass = "btn-dark text-white";
                    outboxClass = "btn-light";
                }
                else
                {
                    inboxClass = "btn-light";
                    outboxClass = "btn-dark text-white";
                }
            }

            base.ExtractSessionData();
            FillLables();
        }

        private void FillLables()
        {
            this.lblNotifications = (Program.Translations["Notifications"])[Lang];   
        }
    }
}
