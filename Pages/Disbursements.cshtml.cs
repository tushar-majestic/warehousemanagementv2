using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class DisbursementsModel : BasePageModel
    {
        public List<DisbursementInfo> Disbursement { get; set; }
        public List<DisbursementInfo> DisbursementAll { get; set; }
        public string Message { get; set; }
        public List<string> UniqueStoreName { get; set; }
        public List<string> UniqueStatus { get; set; }
        public List<string> UniqueRequestingPlace { get; set; }
        public int TotalItems { get; set; }
        public DateTime? FromDate, ToDate;
        [BindProperty]
        public string RequesterName { get; set; }

        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public int QuantityAvailable { get; set; }
        public List<string> SelectedColumns { get; set; } = new List<string>();
        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? StoreName { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? RequestingPlace { get; set; }
        public void OnGet(string? RequesterName, DateTime? FromDate, DateTime? ToDate, int page = 1) 
        {
            base.ExtractSessionData();
            if (CanDisburseItems)
            {
                FillLables();
                LoadSelectedColumns();
                    if (HttpContext.Request.Query.ContainsKey("page")){
                        string pagevalue = HttpContext.Request.Query["page"];
                        page = int.Parse(pagevalue);
                        this.RequesterName = RequesterName;
                        this.FromDate = FromDate;
                        this.ToDate = ToDate;
                        FillData(RequesterName, FromDate, ToDate, page, Status, StoreName, RequestingPlace);

                    }

                    

                    
            }
            
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void LoadSelectedColumns()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                using (var db = new LabDBContext())
                {
                    string pageName = "Disbursements";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        // SelectedColumns = new List<string>();
                        string selectedColumns = "requesterName,destination";
                        SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    }
                }
            }
        }

        public string lblDisbursements, lblSearch, lblRequesterName, lblFromStore, lblSubmit, lblItemName, lblStoreName, lblDestination, lblItemType, lblQuantity, lblItemCode, lblAddDisbursement, lblRequestReceivedDate, lblRequestingPlace, lblComments,
            lblDisbursementStatus, lblInventoryBalanced, lblEdit, lblTotalItem, lblFromDate, lblToDate, lblExportExcel, lblPrintTable, lblAvailableQuantity;

        // public void OnPostSearch([FromForm] string RequesterName, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        // {   CurrentPage = 1; 
        //       this.RequesterName = RequesterName;
        //         this.FromDate = FromDate;
        //         this.ToDate = ToDate;
        //     FillData(RequesterName,FromDate,ToDate, CurrentPage);
        // }

        public IActionResult OnPostAction(string RequesterName, DateTime? FromDate, DateTime? ToDate, string action, List<string> columns)
        {
            base.ExtractSessionData();
            FillLables();
            if (action == "search")
            {
                CurrentPage = 1;
                this.RequesterName = RequesterName;
                this.FromDate = FromDate;
                this.ToDate = ToDate;

                FillData(RequesterName, FromDate, ToDate, CurrentPage);

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "Disbursements";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {
                    string selectedColumns = string.Join(",", columns);

                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "Disbursements";
                    this.RequesterName = RequesterName;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;

                    FillData(RequesterName, FromDate, ToDate, CurrentPage);
                    SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    LoadSelectedColumns();
                }

                // After updating, redirect back to ManageStore with the StoreNumber and StoreName
                // return RedirectToPage("/Disbursements", new { RequesterName = RequesterName, FromDate = FromDate, ToDate = ToDate });
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

        public IActionResult OnPostEdit([FromForm] int DisbursementID, [FromForm] string RequesterName, [FromForm] string FromDate, [FromForm] string ToDate, [FromForm] int page)
        {
            HttpContext.Session.SetInt32("DisbursementID", DisbursementID);
            HttpContext.Session.SetInt32("DisbursementID", DisbursementID);
            HttpContext.Session.SetString("RequesterName", string.IsNullOrEmpty(RequesterName) ? "" : RequesterName);
            HttpContext.Session.SetString("FromDate", string.IsNullOrEmpty(FromDate) ? "" : FromDate);
            HttpContext.Session.SetString("ToDate", string.IsNullOrEmpty(ToDate) ? "" : ToDate);
            HttpContext.Session.SetInt32("page", page);

            return RedirectToPage("./EditDisbursement");
        }
        public IActionResult OnPostView([FromForm] int DisbursementID, [FromForm] string RequesterName, [FromForm] string FromDate, [FromForm] string ToDate, [FromForm] int page)
        {
            // HttpContext.Session.SetInt32("DisbursementID", DisbursementID);
            // HttpContext.Session.SetString("RequesterName", string.IsNullOrEmpty(RequesterName) ? "" : RequesterName);
            // HttpContext.Session.SetString("FromDate", string.IsNullOrEmpty(FromDate) ? "" : FromDate);
            // HttpContext.Session.SetString("ToDate", string.IsNullOrEmpty(ToDate) ? "" : ToDate);
            // HttpContext.Session.SetInt32("page", page);

            HttpContext.Session.SetString("MaterialRequestId", DisbursementID.ToString());


            return RedirectToPage("./ViewDispensedReport");
        }

        // Function without pagination 
        /*private void FillData(string? RequesterName, DateTime? FromDate, DateTime? ToDate)
        {
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = from d in dbContext.DisbursementRequests
                            join s in dbContext.Stores on d.StoreId equals s.StoreId
                            join i in dbContext.Items on d.Itemcode equals i.ItemCode
                            select new DisbursementInfo
                            {
                                DisbursementRequestId = d.DisbursementRequestId,
                                RequesterName = d.RequesterName,
                                RequestingPlace = d.RequestingPlace,
                                Comments = d.Comments,
                                ReqReceivedAt = d.ReqReceivedAt,
                                Status = d.Status,
                                InventoryBalanced = d.InventoryBalanced ? "Yes" : "No",
                                ItemCode = d.Itemcode,
                                ItemTypeCode = d.Itemtypecode,
                                Quantity = d.ItemQuantity,
                                StoreName = s.StoreName,
                                ItemName = i.ItemName
                            };

                if (string.IsNullOrEmpty(RequesterName) == false)
                    query = query.Where(s => s.RequesterName.Contains(RequesterName));

                if (FromDate != null && FromDate >= DateTime.MinValue && ToDate != null && ToDate >= DateTime.MinValue)
                    query = query.Where(e => e.ReqReceivedAt.Date >= FromDate.Value.Date && e.ReqReceivedAt.Date <= ToDate.Value.Date);

                Disbursement = query.ToList();

                Disbursement = query.ToList();
                TotalItems = Disbursement.Count();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }*/

        private void FillData(string? RequesterName, DateTime? FromDate, DateTime? ToDate, int page = 1, string? Status = null, string? StoreName = null, string? RequestingPlace = null)
        {
            if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }
            base.ExtractSessionData();
            if (CanManageStore)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = from d in dbContext.MaterialRequests
                            join u in dbContext.Users on d.RequestedByUserId equals u.UserId
                            join s in dbContext.Destinations on d.RequestingSector equals s.DId
                            join i in dbContext.DespensedItems on d.RequestId equals i.MaterialRequestId
                            join ic in dbContext.ItemCards on i.ItemCardId equals ic.Id
                            join wh in dbContext.Stores on d.WarehouseId equals wh.StoreId
                            select new DisbursementInfo
                            {
                                DisbursementRequestId = d.RequestId,
                                RequesterName = u.FullName,
                                RequestingPlace = s.DestinationName,
                                Comments = i.Comments,
                                ReqReceivedAt = d.OrderDate,
                                ReqDate = d.CreatedAt,
                                Status = d.SupervisorApproval ? "Approved" : "Pending",
                                InventoryBalanced = ic.QuantityAvailable.ToString(),
                                ItemCode = ic.ItemCode,
                                ItemTypeCode = ic.ItemTypeCode,
                                Quantity = i.Quantity,
                                StoreName = wh.StoreName,
                                ItemName = ic.ItemName
                            };

                // Apply filtering if needed
                if (!string.IsNullOrEmpty(RequesterName))
                    query = query.Where(s => s.RequesterName.Contains(RequesterName));

                // if (FromDate != null && FromDate >= DateTime.MinValue && ToDate != null && ToDate >= DateTime.MinValue)
                //     query = query.Where(e => e.ReqDate.Date >= FromDate.Value.Date && e.ReqDate.Date <= ToDate.Value.Date);

                if (FromDate is not null && ToDate is not null)
                {
                    query = query.Where(e => e.ReqDate.Value.Date >= FromDate && e.ReqDate.Value.Date <= ToDate);
                }

                if (string.IsNullOrEmpty(Status) == false)
                    query = query.Where(i => i.Status.Contains(Status));

                if (string.IsNullOrEmpty(StoreName) == false)
                    query = query.Where(i => i.StoreName.Contains(StoreName));

                if (string.IsNullOrEmpty(RequestingPlace) == false)
                    query = query.Where(i => i.RequestingPlace.Contains(RequestingPlace));

                var allItemsQuery = from d in dbContext.MaterialRequests
                                    join u in dbContext.Users on d.RequestedByUserId equals u.UserId
                                    join s in dbContext.Destinations on d.RequestingSector equals s.DId
                                    join i in dbContext.DespensedItems on d.RequestId equals i.MaterialRequestId
                                    join wh in dbContext.Stores on d.WarehouseId equals wh.StoreId
                                    select new DisbursementInfo
                                    {
                                        RequestingPlace = s.DestinationName,
                                        StoreName = wh.StoreName
                                    };

                UniqueRequestingPlace = allItemsQuery.Select(i => i.RequestingPlace).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                UniqueStatus = ["Approved", "Pending"];
                UniqueStoreName = allItemsQuery.Select(i => i.StoreName).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                // Get the total count of items
                TotalItems = query.Count();

                // Calculate total pages
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

                var list = query.ToList();

                Disbursement = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
                DisbursementAll = query.ToList();

                CurrentPage = page;
            }
            else
            {
                RedirectToPage("./Index?lang=" + Lang);
            }
        }   

        private void FillLables()
        {
            

            this.lblDisbursements = (Program.Translations["Disbursements"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblRequesterName = (Program.Translations["RequesterName"])[Lang];
            this.lblAddDisbursement = (Program.Translations["AddDisbursement"])[Lang];
            this.lblRequestReceivedDate = (Program.Translations["RequestReceivedDate"])[Lang];
            this.lblRequestingPlace = (Program.Translations["RequestingPlace"])[Lang];
            this.lblComments = (Program.Translations["Comments"])[Lang];
            this.lblDisbursementStatus = (Program.Translations["DisbursementStatus"])[Lang];
            this.lblInventoryBalanced = (Program.Translations["InventoryBalanced"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblFromStore = (Program.Translations["FromStore"])[Lang];
            this.lblItemType = (Program.Translations["ItemType"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblAvailableQuantity = (Program.Translations["AvailableQuantity"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang]; 
            this.lblDestination = (Program.Translations["Destinations"])[Lang]; 
            this.lblStoreName = (Program.Translations["StoreName"])[Lang]; 
            this.lblItemName = (Program.Translations["ItemName"])[Lang]; 
            this.lblSubmit = (Program.Translations["Submit"])[Lang];

            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
            this.lblExportExcel = (Program.Translations["ExportExcel"])[Lang];
            this.lblPrintTable = (Program.Translations["PrintTable"])[Lang];


        }
    }
}
