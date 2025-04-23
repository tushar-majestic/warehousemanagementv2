using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OfficeOpenXml;
using System.IO;

namespace LabMaterials.Pages
{
    public class Rep_MaterialsReceivedModel : BasePageModel
    {
        public DateTime? FromDate, ToDate;
        public List<SupplyInfo> Supplies { get; set; }
        public int TotalItems { get; set; }

        public string lblMaterialsReceived, lblSearch, lblSupplierName, lblItemName, lblSubmit,
            lblQuantityReceived, lblReceivedAt, lblInventoryBalanced, lblTotalItem,
            lblInventory, lblHazardousMaterials, lblUserActivity, lblDistributedMaterials, lblDamagedItems,
            lblUserReport, lblExport, lblFromDate, lblToDate;
        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                this.FromDate = DateTime.Today;
                this.ToDate = DateTime.Today;
                FillLables();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void FillData(string SupplierName, string ItemName, DateTime? FromDate, DateTime? ToDate)
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = (from S in dbContext.Supplies
                             join SR in dbContext.Suppliers on S.SupplierId equals SR.SupplierId
                             join I in dbContext.Items on S.ItemId equals I.ItemId

                             select new SupplyInfo
                             {
                                 SupplyId = S.SupplyId,
                                 SupplierName = SR.SupplierName,
                                 ItemName = I.ItemName,
                                 ReceivedAt = S.ReceivedAt,
                                 InvoiceNumber = S.InvoiceNumber,
                                 InventoryBalanced = S.InventoryBalanced,
                                 PurchaseOrderNo = S.PurchaseOrderNo,
                                 QuantityReceived = S.QuantityReceived + " " + I.Unit.UnitCode
                             });
                if (string.IsNullOrEmpty(SupplierName) == false)
                    query = query.Where(i => i.SupplierName.Contains(SupplierName));
                if (string.IsNullOrEmpty(ItemName) == false)
                    query = query.Where(i => i.ItemName.Contains(ItemName));
                if (FromDate is not null && FromDate != DateTime.MinValue && ToDate is not null && ToDate != DateTime.MinValue)
                    query = query.Where(e => e.ReceivedAt >= FromDate && e.ReceivedAt <= ToDate);
                Supplies = query.ToList();

                TotalItems = Supplies.Count();

                
            }
            else
                RedirectToPage("./Index?lang=" + Lang);

            this.FromDate = FromDate;
            this.ToDate = ToDate;
        }

        public void OnPost([FromForm] string SupplierName, [FromForm] string ItemName, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                FillData(SupplierName, ItemName, FromDate, ToDate);
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
        public IActionResult OnPostExport()
        {
            MemoryStream stream = new MemoryStream();
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                FillData(null, null, null, null);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("MaterialsRecieved");

                    // Add column headers
                    worksheet.Cells[1, 1].Value = "SUPPLIER NAME";
                    worksheet.Cells[1, 2].Value = "ITEM NAME";
                    worksheet.Cells[1, 3].Value = "QUANTITY RECEIVED";
                    worksheet.Cells[1, 4].Value = "RECEIVED DATE";
                    worksheet.Cells[1, 5].Value = "INVENTORY BALANCED";

                    int row = 2;
                    foreach (var item in Supplies)
                    {
                        worksheet.Cells[row, 1].Value = item.SupplierName;
                        worksheet.Cells[row, 2].Value = item.ItemName;
                        worksheet.Cells[row, 3].Value = item.QuantityReceived;
                        worksheet.Cells[row, 4].Style.Numberformat.Format = "yyyy-MM-dd";
                        worksheet.Cells[row, 4].Value = item.ReceivedAt;
                        worksheet.Cells[row, 5].Value = item.InventoryBalanced;

                        row++;
                    }
                    worksheet.Cells.AutoFitColumns();
                    // Save the Excel package to a memory stream
                    package.SaveAs(stream);

                    // Set the position of the memory stream to 0
                    stream.Position = 0;

                    // Return the Excel file as a FileResult
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Material Recieved Report.xlsx");

                    /*
                    // Convert the MemoryStream to a byte array
                    byte[] byteArray = stream.ToArray();

                    // Convert the byte array to a Base64 string
                    string base64String = Convert.ToBase64String(byteArray);

                    return Content(base64String);
                    */
                }

            }
            else
                RedirectToPage("./Index?lang=" + Lang);

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Items.xlsx");
        }

        private void FillLables()
        {

            this.lblMaterialsReceived = (Program.Translations["MaterialsReceived"])[Lang];
            this.lblInventory = (Program.Translations["Inventory"])[Lang];
            this.lblHazardousMaterials = (Program.Translations["HazardousMaterials"])[Lang];
            this.lblUserActivity = (Program.Translations["UserActivity"])[Lang];
            this.lblDistributedMaterials = (Program.Translations["DistributedMaterials"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblSupplierName = (Program.Translations["SupplierName"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblQuantityReceived = (Program.Translations["QuantityReceived"])[Lang];
            this.lblReceivedAt = (Program.Translations["ReceivedAt"])[Lang];
            this.lblInventoryBalanced = (Program.Translations["InventoryBalanced"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblUserReport = (Program.Translations["UserReport"])[Lang];
            this.lblExport = (Program.Translations["Export"])[Lang];
            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
        }
    }
}
