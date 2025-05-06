using LabMaterials.dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace LabMaterials.Pages
{
    public class ManageItemCardsModel : BasePageModel
    {
        public string lblGroupCode, lblGroupName, lblEdit, lblDelete, lblTotalItem, lblAddItemCard, lblItemGroups, lblSearch, lblItems;

        public void OnGet() 
        {   base.ExtractSessionData();
            FillLables();
        }

        public IActionResult OnPostView([FromForm] string ReportId)
        {
            var dbContext = new LabDBContext();

            // HttpContext.Session.SetString("ReportId", ReportId);


            return RedirectToPage();
        }

         private void FillLables()
        {
            this.Lang =  HttpContext.Session.GetString("Lang");

            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblGroupCode = (Program.Translations["GroupCode"])[Lang];
            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblAddItemCard = (Program.Translations["AddItemCard"])[Lang];
            this.lblItemGroups = (Program.Translations["ItemGroups"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblItemCards = (Program.Translations["ItemCards"])[Lang];
        }
    }

}