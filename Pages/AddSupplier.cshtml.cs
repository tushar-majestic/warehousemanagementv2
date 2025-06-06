using LabMaterials.DB;
using Microsoft.AspNetCore.Mvc;

namespace LabMaterials.Pages
{
    public class AddSupplierModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        
        public string SupplierName;
        public string CoordinatorName;
        public string SupplierPhoneNumber;
        public string SupplierType;
        
        
        public string lblSupplierName, lbSupplierAdded,lblSupplierPhoneNumber, 
        lblSupplierType, lblAddSupplier, lblAdd, lblCancel, lblSupplies, lblSuppliers, lblCoordinatorName, lblExtensionNumber, lblCompanyName, lblStores;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            if (CanManageSupplies == false)
                RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPost([FromForm] string SupplierName, string CoordinatorName, string SupplierPhoneNumber, string SupplierType, [FromForm] int ExtensionNumber)
        {
            LogableTask task = LogableTask.NewTask("AddSupplier");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();

                if (CanManageSupplies)
                {
                    FillLables();
                    this.SupplierName = SupplierName;
                    this.CoordinatorName = CoordinatorName;
                    this.SupplierPhoneNumber = SupplierPhoneNumber;
                    this.SupplierType = SupplierType;
                    var dbContext = new LabDBContext();

                    // Validate fields

                    if (string.IsNullOrEmpty(SupplierName))
                    {
                        ErrorMsg = (Program.Translations["SupplierNameMissing"])[Lang];
                    }
                    else if (string.IsNullOrEmpty(SupplierPhoneNumber))
                    {
                        ErrorMsg = (Program.Translations["SupplierPhoneNumberMissing"])[Lang];
                    }
                    else if (string.IsNullOrEmpty(CoordinatorName))
                    {
                        ErrorMsg = (Program.Translations["CoordinatorNameMissing"])[Lang];
                    }
                    else if (string.IsNullOrEmpty(SupplierType))
                    {
                        ErrorMsg = (Program.Translations["SupplierTypeMissing"])[Lang];
                    }
                    else if (dbContext.Suppliers.Any(s => s.SupplierName == SupplierName))
                    {
                        ErrorMsg = string.Format((Program.Translations["SupplierNameExists"])[Lang], SupplierName);
                    }
                    else
                    {
                        // var dbContext = new LabDBContext();
                        if (dbContext.Suppliers.Any(s => s.SupplierName == SupplierName))
                        {
                            ErrorMsg = string.Format((Program.Translations["SupplierNameExists"])[Lang], SupplierName);
                        }
                        else
                        {
                            var supplier = new Supplier
                            {
                                SupplierName = SupplierName,
                                CoordinatorName = CoordinatorName,
                                SupplierType = SupplierType,
                                SupplierContact = SupplierPhoneNumber,
                                ExtensionNumber = ExtensionNumber,
                                SupplierId = PrimaryKeyManager.GetNextId()
                            };
                            dbContext.Suppliers.Add(supplier);

                           dbContext.SaveChanges();

     

                            task.LogInfo(MethodBase.GetCurrentMethod(), "Supplier added");
                            
                            string Messagee = string.Format("Supplier {0} added", supplier.SupplierName, supplier.SupplierContact, SupplierType);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Messagee, "Add",
                                Helper.ExtractIP(Request), dbContext, true);
                      
                            return RedirectToPage("./ManageSupplier");
                        }
                    }

                    // If there are any errors, return the page
                    return Page();
                }
                else
                {
                    return RedirectToPage("./Index?lang=" + Lang);
                }
            }
            catch (Exception ex)
            {
                task.LogError(MethodBase.GetCurrentMethod(), ex);
                ErrorMsg = ex.Message;
                return Page();
            }
            finally
            {
                task.EndTask();
            }
        }


        private void FillLables()
        {


            this.lblSupplierName = (Program.Translations["SupplierName"])[Lang];
            this.lblSupplierPhoneNumber = (Program.Translations["SupplierPhoneNumber"])[Lang];
            this.lblSupplierType = (Program.Translations["SupplierType"])[Lang];
            this.lblAddSupplier = (Program.Translations["AddSupplier"])[Lang];
            this.lbSupplierAdded = (Program.Translations["SupplierAdded"])[Lang];
            this.lblCompanyName = (Program.Translations["CompanyName"])[Lang];

            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblSupplies = (Program.Translations["Supplies"])[Lang];
            this.lblSuppliers = (Program.Translations["Suppliers"])[Lang];
            this.lblCoordinatorName = (Program.Translations["CoordinatorName"])[Lang];
            this.lblExtensionNumber = (Program.Translations["ExtensionNumber"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];

        }
    }
}
