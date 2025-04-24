using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class AddUnitModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string UnitCode;
        public string UnitDescription;
        public string GroupCode;
        public string WarehouseType;
        public string UnitsMeasure;
        public string ChemicalStatus;
        public string DocumentType;
        public string HazardType;
        public List<ItemGroup> ItemGroups { get; set; }
        public List<Store> WarehouseTypes { get; set; }
        public List<ItemGroup> UnitsMeasures { get; set; }
        public List<DamagedItem> ChemicalStatuss { get; set; }
        public List<ItemGroup> DocumentTypes { get; set; }
        public List<HazardType> HazardTypes { get; set; }

        public string lblUnitCode, lblUnitDescription, lblGroupName, lblAdd, lblCancel, lblAddUnits, lblWarehouseType, lblUnitsMeasure, lblChemicalStatus, lblDocumentType, lblHazardType;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            if (CanManageItems == false)
                RedirectToPage("./Index?lang=" + Lang);
            using (var dbContext = new LabDBContext())
            {
                ItemGroups = dbContext.ItemGroups.ToList();
                WarehouseTypes = dbContext.Stores.ToList();
                HazardTypes = dbContext.HazardTypes.ToList();
                ChemicalStatuss = dbContext.DamagedItems.ToList();
            }
        }

        public IActionResult OnPost([FromForm] string GroupCode, [FromForm] string UnitCode, [FromForm] string UnitDescription, [FromForm] string WarehouseType, [FromForm] string UnitsMeasure, [FromForm] string ChemicalStatus, [FromForm] string DocumentType, [FromForm] string HazardType)
        {
            LogableTask task = LogableTask.NewTask("AddUnit");

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
                    this.WarehouseType = WarehouseType;
                    this.UnitsMeasure = UnitsMeasure;
                    this.ChemicalStatus = ChemicalStatus;
                    this.DocumentType = DocumentType;
                    this.HazardType = HazardType;

                    var dbContext = new LabDBContext();
                    ItemGroups = dbContext.ItemGroups.ToList();

                    if (string.IsNullOrEmpty(UnitCode))
                        ErrorMsg = (Program.Translations["UnitCodeMissing"])[Lang];
                    else if (string.IsNullOrEmpty(UnitDescription))
                        ErrorMsg = (Program.Translations["UnitDescriptionMissing"])[Lang];
                    else
                    {
                        if (dbContext.Units.Count(s => s.UnitCode == UnitCode) > 0)
                            ErrorMsg = string.Format((Program.Translations["UnitCodeExists"])[Lang], UnitCode);
                        else if (dbContext.Units.Count(s => s.UnitDesc == UnitDescription) > 0)
                            ErrorMsg = string.Format((Program.Translations["UnitDescriptionExists"])[Lang], UnitDescription);
                        else
                        {
                            var unit = new Unit
                            {
                                UnitCode = UnitCode,
                                UnitDesc = UnitDescription,
                                GroupCode = GroupCode,
                                HazardType = HazardType,
                                WarehouseType = WarehouseType,
                                UnitsMeasure = UnitsMeasure,
                                ChemicalStatus = ChemicalStatus,
                                DocumentType = DocumentType
                            };
                            dbContext.Units.Add(unit);
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "Unit added");

                            string Message = string.Format("Unit {0} added", unit.UnitDesc);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
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
            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblAddUnits = (Program.Translations["AddUnit"])[Lang];
            this.lblChemicalStatus = (Program.Translations["ChemicalStatus"])[Lang];
            this.lblWarehouseType = (Program.Translations["WarehouseType"])[Lang];
            this.lblUnitsMeasure = (Program.Translations["UnitsMeasure"])[Lang];
            this.lblDocumentType = (Program.Translations["DocumentType"])[Lang];
            this.lblHazardType = (Program.Translations["HazardTypeName"])[Lang];
        }

    }
}
