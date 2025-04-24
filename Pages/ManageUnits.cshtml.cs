using LabMaterials.dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace LabMaterials.Pages
{
    public class ManageUnitsModel : BasePageModel
    {
        public List<UnitInfo> Units { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }
        
        public string lblUnitCode, lblUnitDescription, lblGroupName, lblEdit, lblDelete, lblTotalItem, lblAddUnit, 
        lblUnits, lblSearch, lblItems;
        public void OnGet() 
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void OnPostSearch([FromForm] string UnitDesc)
        {
            FillData(UnitDesc);
        }

        public void OnPostDelete([FromForm] int ID)
        {
            base.ExtractSessionData();
            if (CanManageItems)
            {
                var dbContext = new LabDBContext();

                var items = dbContext.Items.Count(s => s.UnitId == ID);
                if (items == 0)
                {
                    var unit = dbContext.Units.Single(s => s.Id == ID);
                    dbContext.Units.Remove(unit);
                    dbContext.SaveChanges();
                    FillData(null);
                    Message = string.Format((Program.Translations["UnitDeleted"])[Lang], unit.UnitDesc);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
                }
                else
                {
                    var item = dbContext.Items.First(s => s.UnitId == ID).ItemName;
                    var unit = dbContext.Units.First(s => s.Id == ID).UnitDesc;
                    Message = string.Format((Program.Translations["UnitNotDeleted"])[Lang], unit, item);
                    FillData(null);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);

        }


        public IActionResult OnPostEdit([FromForm] int ID)
        {
            HttpContext.Session.SetInt32("ID", ID);

            return RedirectToPage("./EditUnit");
        }

        private void FillData(string? UnitDesc)
        {
            base.ExtractSessionData();
            if (CanManageItems)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = from u in dbContext.Units
                            join g in dbContext.ItemGroups on u.GroupCode equals g.GroupCode
                            select new UnitInfo
                            {
                               ID = u.Id,
                               UnitCode = u.UnitCode,
                               UnitDescription = u.UnitDesc,
                               GroupName = g.GroupDesc
                            };

                if (string.IsNullOrEmpty(UnitDesc) == false)
                    query = query.Where(s => s.UnitDescription.Contains(UnitDesc));


                Units = query.ToList();

                Units = query.ToList();
                TotalItems = Units.Count();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillLables()
        {
            

            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblUnitCode = (Program.Translations["UnitCode"])[Lang];
            this.lblUnitDescription = (Program.Translations["UnitDescription"])[Lang];
            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblAddUnit= (Program.Translations["AddUnit"])[Lang];
            this.lblUnits = (Program.Translations["Units"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];
        }

    }
}