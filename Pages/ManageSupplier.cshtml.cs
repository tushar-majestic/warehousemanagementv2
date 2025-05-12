using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class ManageSupplierModel : BasePageModel
    {
        public List<SupplierInfo> Suppliers { get; set; }
        public List<SupplierInfo> SuppliersAll { get; set; }
        public int TotalItems { get; set; }
        public string Message { get; set; }

        [BindProperty]
        public string SupplierName { get; set; }
        public string CoordinatorName { get; set; }
        public string SupplierType { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        
        public string lblSuppliers, lblSearch, lblSuplierName, lblSubmit, lblSupplierName, lblConatctNumber, lblSupplierType, 
        lblAddSupplier, lblEdit, lblDelete, lblTotalItem, lblSupplies, lblCoordinatorName, lblExportExcel, lblPrintTable;
        public void OnGet(string? SupplierName, string? CoordinatorName, string? SupplierType, int page = 1)
        {
            base.ExtractSessionData();
            if (CanManageSupplies)
            {
                FillLables();
                if (HttpContext.Request.Query.ContainsKey("page")){
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.SupplierName = SupplierName;
                    this.CoordinatorName = CoordinatorName;
                    this.SupplierType = SupplierType;
                    FillData(SupplierName, CoordinatorName, SupplierType, page);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillData(string SupplierName, string CoordinatorName, string SupplierType, int page = 1)
        {   if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }
            base.ExtractSessionData();
            if (this.CanManageSupplies)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = (from i in dbContext.Suppliers

                             select new SupplierInfo
                             {
                                 SupplierId = i.SupplierId,
                                 SupplierName = i.SupplierName,
                                 CoordinatorName = i.CoordinatorName,
                                 ConatctNumber=i.SupplierContact,
                                 SupplierType=i.SupplierType,
                                 
                             });

                if (string.IsNullOrEmpty(SupplierName) == false)
                    query = query.Where(i => i.SupplierName.Contains(SupplierName));

                if (string.IsNullOrEmpty(CoordinatorName) == false)
                    query = query.Where(i => i.CoordinatorName.Contains(CoordinatorName));

                if (string.IsNullOrEmpty(SupplierType) == false)
                    query = query.Where(i => i.SupplierType.Contains(SupplierType));

                // Suppliers = query.ToList();

                TotalItems = query.Count();
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
                var list = query.ToList();
                Suppliers = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();       
                SuppliersAll = query.ToList();   
                CurrentPage = page; 
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPostEdit([FromForm] int SupplierId, [FromForm] int page, [FromForm] string SupplierName)
        {
            HttpContext.Session.SetInt32("SupplierId", SupplierId);
            HttpContext.Session.SetInt32("page", page);
            HttpContext.Session.SetString("SupplierName", string.IsNullOrEmpty(SupplierName) ? "" : SupplierName);
            HttpContext.Session.SetString("CoordinatorName", string.IsNullOrEmpty(CoordinatorName) ? "" : CoordinatorName);
            return RedirectToPage("./EditSupplier");
        }

        public void OnPostDelete([FromForm] int SupplierId)
        {
            base.ExtractSessionData();
            if (CanManageSupplies)
            {
                var dbContext = new LabDBContext();

                if (dbContext.Supplies.Count(s => s.SupplierId == SupplierId) == 0)
                {
                    var supplier = dbContext.Suppliers.Single(s => s.SupplierId == SupplierId);
                    dbContext.Suppliers.Remove(supplier);
                    dbContext.SaveChanges();
                    FillData(null, null, null);
                    Message = string.Format((Program.Translations["SupplierDeleted"])[Lang], supplier.SupplierName);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
                }
                else
                {
                    Message = (Program.Translations["SupplierNotDeleted"])[Lang];
                    FillData(null, null, null);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void OnPostSearch([FromForm] string SupplierName, [FromForm] string CoordinatorName, [FromForm] string SupplierType)
        {   CurrentPage = 1;
            this.SupplierName = SupplierName;
            this.CoordinatorName = CoordinatorName;
            this.SupplierType = SupplierType;
            FillData(SupplierName, CoordinatorName, SupplierType, CurrentPage);
        }

        private void FillLables()
        {
            

            this.lblSuppliers = (Program.Translations["Suppliers"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblSupplierName = (Program.Translations["SupplierName"])[Lang];
            this.lblCoordinatorName = (Program.Translations["CoordinatorName"])[Lang];
            this.lblConatctNumber = (Program.Translations["SupplierContactNumber"])[Lang];
            this.lblSupplierType = (Program.Translations["SupplierType"])[Lang];
            this.lblAddSupplier = (Program.Translations["AddSupplier"])[Lang];
            this.lblSuplierName = (Program.Translations["SupplierName"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblSupplies = (Program.Translations["Supplies"])[Lang];
            this.lblExportExcel = (Program.Translations["ExportExcel"])[Lang];
            this.lblPrintTable = (Program.Translations["PrintTable"])[Lang];
        }
    }
}
