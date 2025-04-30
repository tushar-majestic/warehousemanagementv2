using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class EditUnitModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public int UnitId;
        public string UnitCode;
        public string UnitDescription;
        public string GroupCode;
        public int page { get; set; }
        public List<ItemGroup> ItemGroups {  get; set; }
        
        public string lblUnitCode, lblAddUnit, lblUnitDescription, lblGroupName, lblUpdate, lblCancel, lblUpdateUnits, lblUnits, lblItems;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            this.page = (int)HttpContext.Session.GetInt32("page");
            if (CanDisburseItems == false)
                RedirectToPage("./Index?lang=" + Lang);
            using (var dbContext = new LabDBContext())
            {
                ItemGroups = dbContext.ItemGroups.ToList();

                var unit = dbContext.Units.Single(i => i.Id == HttpContext.Session.GetInt32("ID"));

                this.UnitCode = unit.UnitCode;
                this.UnitDescription = unit.UnitDesc;
                this.GroupCode = unit.GroupCode;
                this.UnitId = unit.Id;

            }
        }

        public IActionResult OnPost([FromForm] int UnitId, [FromForm] string GroupCode, [FromForm] string UnitCode, [FromForm] string UnitDescription)
        {
            LogableTask task = LogableTask.NewTask("EditUnit");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageItems)
                {
                    FillLables();
                    this.GroupCode = GroupCode;
                    this.UnitCode = UnitCode;
                    this.UnitDescription = UnitDescription;
                    this.UnitId = UnitId;
                    var dbContext = new LabDBContext();
                    ItemGroups = dbContext.ItemGroups.ToList();

                    if (string.IsNullOrEmpty(UnitCode))
                        ErrorMsg = (Program.Translations["UnitCodeMissing"])[Lang];
                    else if (string.IsNullOrEmpty(UnitDescription))
                        ErrorMsg = (Program.Translations["UnitDescriptionMissing"])[Lang];
                    else
                    {
                        if (dbContext.Units.Count(u => u.UnitCode == UnitCode && u.Id != UnitId) > 0)
                            ErrorMsg = string.Format((Program.Translations["UnitCodeExists"])[Lang], UnitCode);
                        else if (dbContext.Units.Count(u => u.UnitDesc == UnitDescription && u.Id != UnitId) > 0)
                            ErrorMsg = string.Format((Program.Translations["UnitDescriptionExists"])[Lang], UnitDescription);
                        else
                        {
                            var unit = dbContext.Units.Single(u => u.Id == UnitId);
                            unit.UnitCode = UnitCode;
                            unit.UnitDesc = UnitDescription;
                            unit.GroupCode = GroupCode;
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "Unit updated");

                            string Message = string.Format("Unit {0} updated", unit.UnitDesc);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Update",
                                Helper.ExtractIP(Request), dbContext, true);

                            return RedirectToPage("./ManageUnits");
                        }
                    }
                    return Page();
                }
                else
                    return RedirectToPage("./Index?lang=" + Lang);
            }
            catch (Exception ex)
            {
                task.LogError(MethodBase.GetCurrentMethod(), ex);
                ErrorMsg = ex.Message;
                return Page();
            }
            finally { task.EndTask(); }
        }

        private void FillLables()
        {
            

            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblUnitCode = (Program.Translations["UnitCode"])[Lang];
            this.lblUnitDescription = (Program.Translations["UnitDescription"])[Lang];
            this.lblUpdate = (Program.Translations["Update"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblUpdateUnits = (Program.Translations["UpdateUnit"])[Lang];
            this.lblAddUnit = (Program.Translations["AddUnit"])[Lang];
            this.lblUnits = (Program.Translations["Units"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];
        }
    }
}
