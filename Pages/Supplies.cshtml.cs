using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class SuppliesModel : BasePageModel
    {
        public List<SupplyInfo> Supplies { get; set; }
        public List<SupplyInfo> SuppliesAll { get; set; }
        public int TotalItems { get; set; }
        public string Message { get; set; }
        [BindProperty]
        public string SupplierName { get; set; }
        public List<string> UniqueStoreName { get; set; }
        public List<string> UniqueSupplierName { get; set; }

        [BindProperty]
        public string ItemName { get; set; }
        public DateTime? FromDate, ToDate;
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public List<string> SelectedColumns { get; set; } = new List<string>();



        public string lblSupplies, lbltypeCode, lblStoreName, lblExpiryDate, lblItemType, lblRoomName, lblShelfNumber, lblManageSuppliers, lblSearch, lblSupplierName, lblItemName, lblSubmit, lblAddSupplies,
            lblQuantityReceived, lblPurchaseOrderNo, lblInvoiceNumber, lblReceivedAt, lblInventoryBalanced, lblEdit, lblDelete,
            lblTotalItem, lblFromDate, lblToDate, lblNewReceivingReport;



        private void LoadSelectedColumns()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                using (var db = new LabDBContext())
                {
                    string pageName = "supplies";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        string selectedColumns = "SupplierName,itemName";
                        SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    }
                }
            }
        }
        public void OnGet(string? SupplierName, string? ItemName, DateTime? FromDate, DateTime? ToDate, int page = 1)
        {
            base.ExtractSessionData();
            if (this.CanManageSupplies)
            {
                FillLables();
                LoadSelectedColumns();
                if (HttpContext.Request.Query.ContainsKey("page"))
                {
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.SupplierName = SupplierName;
                    this.ItemName = ItemName;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    FillData(SupplierName, ItemName, FromDate, ToDate, page);

                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
        /*private void FillData(string SupplierName, string ItemName, DateTime? FromDate, DateTime? ToDate)
        {
            base.ExtractSessionData();
            if (this.CanManageSupplies)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = (from S in dbContext.Supplies
                             join SR in dbContext.Suppliers on S.SupplierId equals SR.SupplierId
                             join I in dbContext.Items on S.ItemId equals I.ItemId
                             join sg in dbContext.Storages on S.SupplyId equals sg.SupplyId
                             join st in dbContext.Stores on sg.StoreId equals st.StoreId
                             join sb in dbContext.Rooms on sg.RoomId equals sb.RoomId
                             select new SupplyInfo
                             {
                                 SupplyId = S.SupplyId,
                                 SupplierName = SR.SupplierName,
                                 ItemName = I.ItemName,
                                 ReceivedAt = S.ReceivedAt,
                                 InvoiceNumber = S.InvoiceNumber,
                                 InventoryBalanced = S.InventoryBalanced,
                                 PurchaseOrderNo = S.PurchaseOrderNo,
                                 ItemCode=S.ItemCode,
                                 ItemType=S.ItemType,
                                 StoreName=st.StoreName,
                                 RoomName=sb.RoomName,
                                 ShelfNumber=sg.ShelfNumber,
                                 ExpiryDate=S.ExpiryDate,
                                 
                                 QuantityReceived = S.QuantityReceived.ToString() + " " + I.Unit.UnitCode
                             });
                if (!string.IsNullOrEmpty(SupplierName) && !string.IsNullOrEmpty(ItemName))
                {
                    query = query.Where(i => i.SupplierName.Contains(SupplierName) && i.ItemName.Contains(ItemName));
                }
                if (!string.IsNullOrEmpty(SupplierName))
                {
                    query = query.Where(i => i.SupplierName.Contains(SupplierName));
                }
                if (!string.IsNullOrEmpty(ItemName))
                {
                    query = query.Where(i => i.ItemName.Contains(ItemName));
                }

                if (FromDate != null && FromDate >= DateTime.MinValue && ToDate != null && ToDate >= DateTime.MinValue)
                    query = query.Where(e => e.ExpiryDate.Date >= FromDate.Value.Date && e.ExpiryDate.Date <= ToDate.Value.Date);

                Supplies = query.ToList();

                TotalItems = Supplies.Count();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }*/

        private void FillData(string SupplierName, string ItemName, DateTime? FromDate, DateTime? ToDate, int page = 1)
        {
            base.ExtractSessionData();
            if (this.CanManageSupplies)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = (from S in dbContext.Supplies
                             join SR in dbContext.Suppliers on S.SupplierId equals SR.SupplierId
                             join I in dbContext.Items on S.ItemId equals I.ItemId
                             join sg in dbContext.Storages on S.SupplyId equals sg.SupplyId
                             join st in dbContext.Stores on sg.StoreId equals st.StoreId
                             join sb in dbContext.Rooms on sg.RoomId equals sb.RoomId
                             select new SupplyInfo
                             {
                                 SupplyId = S.SupplyId,
                                 SupplierName = SR.SupplierName,
                                 ItemName = I.ItemName,
                                 ReceivedAt = S.ReceivedAt,
                                 InvoiceNumber = S.InvoiceNumber,
                                 InventoryBalanced = S.InventoryBalanced,
                                 PurchaseOrderNo = S.PurchaseOrderNo,
                                 ItemCode = S.ItemCode,
                                 ItemType = S.ItemType,
                                 StoreName = st.StoreName,
                                 RoomName = sb.RoomName,
                                 ShelfNumber = sg.ShelfNumber,
                                 ExpiryDate = S.ExpiryDate,

                                 QuantityReceived = S.QuantityReceived.ToString() + " " + I.Unit.UnitCode
                             });
                if (!string.IsNullOrEmpty(SupplierName) && !string.IsNullOrEmpty(ItemName))
                {
                    query = query.Where(i => i.SupplierName.Contains(SupplierName) && i.ItemName.Contains(ItemName));
                }
                if (!string.IsNullOrEmpty(SupplierName))
                {
                    query = query.Where(i => i.SupplierName.Contains(SupplierName));
                }
                if (!string.IsNullOrEmpty(ItemName))
                {
                    query = query.Where(i => i.ItemName.Contains(ItemName));
                }

                if (FromDate != null && FromDate >= DateTime.MinValue && ToDate != null && ToDate >= DateTime.MinValue)
                    query = query.Where(e => e.ExpiryDate.Date >= FromDate.Value.Date && e.ExpiryDate.Date <= ToDate.Value.Date);


                UniqueSupplierName = query.Select(i => i.SupplierName).Distinct().ToList();
                UniqueStoreName = query.Select(i => i.StoreName).Distinct().ToList();


                TotalItems = query.Count();
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

                var list = query.ToList();

                Supplies = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
                SuppliesAll = query.ToList();

                CurrentPage = page;
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void OnPost([FromForm] string SupplierName, [FromForm] string ItemName, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        {   CurrentPage = 1; 
            this.FromDate = FromDate;
            this.ToDate = ToDate;
            this.SupplierName = SupplierName;
            this.ItemName = ItemName;
            base.ExtractSessionData();
            if (CanManageSupplies)
            {
                FillData(SupplierName, ItemName, FromDate, ToDate, CurrentPage);
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPostAction(string SupplierName, string ItemName, DateTime? FromDate, DateTime? ToDate,string action, List<string> columns)
        {
            base.ExtractSessionData();

            if (action == "search")
            {
                CurrentPage = 1;
                this.FromDate = FromDate;
                this.ToDate = ToDate;
                this.SupplierName = SupplierName;
                this.ItemName = ItemName;
                if (CanManageSupplies)
                {
                    FillData(SupplierName, ItemName, FromDate, ToDate, CurrentPage);
                }

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "supplies";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {
                    string selectedColumns = string.Join(",", columns);

                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "supplies";
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    this.SupplierName = SupplierName;
                    this.ItemName = ItemName;
                    FillData(SupplierName, ItemName, FromDate, ToDate, CurrentPage);
                    SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    LoadSelectedColumns();
                }

                // After updating, redirect back to ManageStore with the StoreNumber and StoreName
                // return RedirectToPage("/supplies", new { SupplierName = SupplierName, ItemName = ItemName, FromDate= FromDate, ToDate= ToDate });
            }

            return Page();
        }

        private void SaveSelectedColumns(int userId, string pageName, string selectedColumns)
        {
            base.ExtractSessionData();
            using (var db = new LabDBContext())
            {
                var existingRecord = db.Tablecolumns
                    .FirstOrDefault(c => c.UserId == userId && c.Page == pageName);

                if (existingRecord != null)
                {
                    existingRecord.DisplayColumns = selectedColumns;
                }
                else
                {
                    var newRecord = new Tablecolumn
                    {
                        UserId = userId,
                        Page = pageName,
                        DisplayColumns = selectedColumns
                    };
                    db.Tablecolumns.Add(newRecord);
                }

                db.SaveChanges();
            }
        }


        public IActionResult OnPostEdit([FromForm] int SupplyId, [FromForm] string FromDate, [FromForm] string ToDate, [FromForm] int page, [FromForm] string ItemName, [FromForm] string SupplierName)
        {
            HttpContext.Session.SetString("FromDate", string.IsNullOrEmpty(FromDate) ? "" : FromDate);
            HttpContext.Session.SetString("ToDate", string.IsNullOrEmpty(ToDate) ? "" : ToDate);
            HttpContext.Session.SetInt32("page", page);
             HttpContext.Session.SetString("ItemName", string.IsNullOrEmpty(ItemName) ? "" : ItemName);
             HttpContext.Session.SetString("SupplierName", string.IsNullOrEmpty(SupplierName) ? "" : SupplierName);
            HttpContext.Session.SetInt32("SupplyId", SupplyId);

            return RedirectToPage("./EditSupply");
        }
        public IActionResult OnPostView([FromForm] int SupplyId, [FromForm] string FromDate, [FromForm] string ToDate, [FromForm] int page)
        {
            HttpContext.Session.SetString("FromDate", string.IsNullOrEmpty(FromDate) ? "" : FromDate);
            HttpContext.Session.SetString("ToDate", string.IsNullOrEmpty(ToDate) ? "" : ToDate);
            HttpContext.Session.SetInt32("page", page);
            HttpContext.Session.SetInt32("SupplyId", SupplyId);

            return RedirectToPage("./viewSupplies");
        }

        public void OnPostDelete([FromForm] int SupplyId)
        {
            base.ExtractSessionData();
            if (CanManageSupplies)
            {
                var dbContext = new LabDBContext();

                
                var Supply = dbContext.Supplies.Single(s => s.SupplyId == SupplyId);
                dbContext.Supplies.Remove(Supply);
                
                var Storag = dbContext.Storages.Single(s => s.SupplyId == SupplyId);
                dbContext.Storages.Remove(Storag);
                dbContext.SaveChanges();
                FillData(null, null,null,null);
                Message = "Record has been deleted successfully";
                Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);

            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillLables()
        {
            

            this.lblSupplies = (Program.Translations["Supplies"])[Lang];
            this.lblManageSuppliers = (Program.Translations["ManageSuppliers"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblNewReceivingReport = (Program.Translations["NewReceivingReport"])[Lang];
            this.lblAddSupplies = (Program.Translations["AddSupplies"])[Lang];
            this.lblSupplierName = (Program.Translations["SupplierName"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lbltypeCode = (Program.Translations["ItemCode"])[Lang];
            this.lblItemType = (Program.Translations["ItemType"])[Lang];
            this.lblQuantityReceived = (Program.Translations["QuantityReceived"])[Lang];
            this.lblPurchaseOrderNo = (Program.Translations["PurchaseOrderNo"])[Lang];
            this.lblInvoiceNumber = (Program.Translations["InvoiceNumber"])[Lang];
            this.lblReceivedAt = (Program.Translations["ReceivedAt"])[Lang];
            this.lblInventoryBalanced = (Program.Translations["InventoryBalanced"])[Lang];
            this.lblRoomName = (Program.Translations["RoomName"])[Lang];
            this.lblShelfNumber = (Program.Translations["ShelfNumber"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblExpiryDate = (Program.Translations["ExpiryDate"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];

            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
        }
    }
}
