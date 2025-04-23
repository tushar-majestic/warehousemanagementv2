using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class EditSupplierModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string SupplierName;
        public string SupplierPhoneNumber;
        public string SupplierType;
        public string SelectSupplierType;
        public string SupplierNotUpdated;
        public int SupplierId;
        
        public string lblSupplierName, lblSupplierNotUpdated, lblSupplierPhoneNumber, lblSelectSupplierType, lblSupplierType, lblUpdateSupplier, lblEdit, lblCancel;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            if (CanManageSupplies == false)
                RedirectToPage("./Index?lang=" + Lang);
            else
            {
                var dbContext = new LabDBContext();
                var supplier = dbContext.Suppliers.Single(s => s.SupplierId == HttpContext.Session.GetInt32("SupplierId") && s.Ended==null);
                SupplierName = supplier.SupplierName;
                SupplierPhoneNumber = supplier.SupplierContact;
                SupplierType = supplier.SupplierType;
                SupplierId = supplier.SupplierId;
            }
        }

        public IActionResult OnPost([FromForm] int SupplierId, [FromForm] string SupplierName,string SupplierPhoneNumber,string SupplierType)
        {
            LogableTask task = LogableTask.NewTask("EditSupplier");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageSupplies)
                {
                    FillLables();
                    this.SupplierName = SupplierName;
                    this.SupplierPhoneNumber = SupplierPhoneNumber;
                    this.SupplierType = SupplierType;

                    if (string.IsNullOrEmpty(SupplierName))
                        ErrorMsg = (Program.Translations["SupplierNameMissing"])[Lang];
                    else if (string.IsNullOrEmpty(SupplierPhoneNumber))
                    {
                        ErrorMsg = (Program.Translations["SupplierPhoneNumberMissing"])[Lang];
                    }
                    else if (string.IsNullOrEmpty(SupplierType))
                    {
                        ErrorMsg = (Program.Translations["SupplierTypeMissing"])[Lang];
                    }
                    else
                    {
                        var dbContext = new LabDBContext();
                        if (dbContext.Suppliers.Count(s => s.SupplierId != SupplierId && s.SupplierName == SupplierName) > 0)
                        {
                            ErrorMsg = string.Format((Program.Translations["SupplierNameExists"])[Lang], SupplierName);
                        }
                        else
                        {

                            var existingSupplier = dbContext.Suppliers.SingleOrDefault(s => s.SupplierId == SupplierId);
                            if(existingSupplier == null)
                            {
                                ErrorMsg = string.Format((Program.Translations["SupplierNotUpdated"])[Lang], SupplierName);
                                return RedirectToPage("./ManageSupplier");
                            }
                            
                            existingSupplier.SupplierName = SupplierName;
                            existingSupplier.SupplierContact = SupplierPhoneNumber;
                            existingSupplier.SupplierType = SupplierType;

                            dbContext.SaveChanges();

                            string message = string.Format("Supplier {0} updated", existingSupplier.SupplierName);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, message, "Update", Helper.ExtractIP(Request), dbContext, true);

                            task.LogInfo(MethodBase.GetCurrentMethod(), "Supplier updated");

                            return RedirectToPage("./ManageSupplier");
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
            

            this.lblSupplierName = (Program.Translations["SupplierName"])[Lang];
            this.lblSupplierPhoneNumber = (Program.Translations["SupplierPhoneNumber"])[Lang];
            this.lblSupplierType = (Program.Translations["SupplierType"])[Lang];
            this.lblSelectSupplierType = (Program.Translations["SelectSupplierType"])[Lang];
            this.lblSupplierNotUpdated = (Program.Translations["SupplierNotUpdated"])[Lang];
            this.lblUpdateSupplier = (Program.Translations["UpdateSupplier"])[Lang];

            this.lblEdit = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
        }
    }
}
