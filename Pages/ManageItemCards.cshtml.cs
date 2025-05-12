using LabMaterials.dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace LabMaterials.Pages
{
    public class ManageItemCardsModel : BasePageModel
    {
        public string    lblTotalItem, lblAddItemCard, lblItemGroups, lblSearch, lblItems, lblExportExcel, lblPrintTable;

        public List<string> SelectedColumns { get; set; } = new List<string>();


        public void OnGet() 
        {   base.ExtractSessionData();
            FillLables();
        }

        public IActionResult OnPostView([FromForm] string ReportId)
        {
            var dbContext = new LabDBContext();

            // HttpContext.Session.SetString("ReportId", ReportId);


            return RedirectToPage("/viewItemCards");
        }

        public void OnPostSearch([FromForm] string itemcard)
        {   
            base.ExtractSessionData();
        }

         private void FillLables()
        {
            this.Lang =  HttpContext.Session.GetString("Lang");

         
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblAddItemCard = (Program.Translations["AddItemCard"])[Lang];
            this.lblItemGroups = (Program.Translations["ItemGroups"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblItemCards = (Program.Translations["ItemCards"])[Lang];
            this.lblExportExcel = (Program.Translations["ExportExcel"])[Lang];
            this.lblPrintTable = (Program.Translations["PrintTable"])[Lang];
        }
    }

}