using Microsoft.EntityFrameworkCore;
using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class ViewReturnRequestsModel : BasePageModel
    {
        private readonly LabDBContext _context;
        public List<string> SelectedColumns { get; set; } = new List<string>();
        public string Message { get; set; }
        public List<ReturnRequest> ReturnRequests { get; set; } = new();
        public List<ReturnRequest> ReturnRequestsAll { get; set; } = new();
        public int TotalItems { get; set; }
        public string OrderNumber { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public bool HasSearched { get; set; } = false;

        public string lblSearch, lblPrintTable, lblExportExcel, lblTotalItem;

        public ViewReturnRequestsModel(LabDBContext context)
        {
            _context = context;
        }

        public void OnGet(string? OrderNumber, int page = 1)
        {
            base.ExtractSessionData();
            if (this.CanManageItems)
            {
                FillLables();
                LoadSelectedColumns();

                if (HttpContext.Request.Query.ContainsKey("page"))
                {
                    this.OrderNumber = OrderNumber;
                    CurrentPage = page;
                    FillData(OrderNumber, page);
                }
            }
            else
            {
                RedirectToPage("./Index?lang=" + Lang);
            }
        }

        // public async Task OnGetAsync()
        // {
        //     base.ExtractSessionData();
        //     ReturnRequests = await _context.ReturnRequests
        //         .Include(r => r.Warehouse)
        //         .Include(r => r.FromSector)
        //         .OrderByDescending(r => r.OrderDate)
        //         .ToListAsync();
        // }
        private void LoadSelectedColumns()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                using (var db = new LabDBContext())
                {
                    string pageName = "returnRequest";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        // SelectedColumns = new List<string>();
                        string selectedColumns = "orderNumber,orderDate";
                        SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    }
                }
            }
        }
        // public IActionResult OnPostAction(string OrderNumber, string action, List<string> columns)
        // {
        //     base.ExtractSessionData();

        //     if (action == "search")
        //     {
        //         CurrentPage = 1;
        //         this.OrderNumber = OrderNumber;

        //         FillData(OrderNumber, CurrentPage);

        //         int? userId = HttpContext.Session.GetInt32("UserId");
        //         string pageName = "returnRequest";
        //         LoadSelectedColumns();
        //     }
        //     else if (action == "updateColumns")
        //     {
        //         if (columns != null && columns.Any())
        //         {

        //             string selectedColumns = string.Join(",", columns);

        //             int? userId = HttpContext.Session.GetInt32("UserId");
        //             string pageName = "returnRequest";
        //             this.OrderNumber = OrderNumber;
        //             FillData(OrderNumber, CurrentPage);
        //             SaveSelectedColumns(userId.Value, pageName, selectedColumns);
        //             LoadSelectedColumns();
        //         }

        //     }

        //     return Page();
        // }
        public IActionResult OnPostAction(string OrderNumber, string action, List<string> columns)
        {
            base.ExtractSessionData();

            if (action == "search")
            {
                CurrentPage = 1;
                this.OrderNumber = OrderNumber;
                HasSearched = true;

                FillData(OrderNumber, CurrentPage);
                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "returnRequest";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {
                    string selectedColumns = string.Join(",", columns);
                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "returnRequest";

                    this.OrderNumber = OrderNumber;
                    HasSearched = true;
                    FillData(OrderNumber, CurrentPage);
                    SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    LoadSelectedColumns();
                }
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

        // private void FillData(string? OrderNumber, int page = 1)
        // {
        //     base.ExtractSessionData();

        //     if (!this.CanManageItems)
        //     {
        //         return;
        //     }

        //     FillLables();
        //     var dbContext = new LabDBContext();

        //     var query = dbContext.ReturnRequests
        //         .Include(r => r.Warehouse)
        //         .Include(r => r.FromSector)
        //         .OrderByDescending(r => r.OrderDate)
        //         .AsQueryable();

        //     if (!string.IsNullOrEmpty(OrderNumber))
        //     {
        //         query = query.Where(i => i.OrderNumber.Contains(OrderNumber));
        //     }

        //     TotalItems = query.Count();
        //     TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
        //     CurrentPage = page;

        //     ReturnRequests = query.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
        //     ReturnRequestsAll = query.ToList(); 
        // }
        private void FillData(string? OrderNumber, int page = 1)
        {
            base.ExtractSessionData();
            FillLables();

            if (!this.CanManageItems)
                return;

            var query = _context.ReturnRequests
                .Include(r => r.Warehouse)
                .Include(r => r.FromSector)
                .OrderByDescending(r => r.OrderDate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(OrderNumber))
            {
                query = query.Where(i => i.OrderNumber.Contains(OrderNumber));
            }

            TotalItems = query.Count();
            TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
            CurrentPage = page;
            var list = query.ToList();

            ReturnRequests = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            ReturnRequestsAll = query.ToList();
        }


        private void FillLables()
        {

            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblPrintTable = (Program.Translations["PrintTable"])[Lang];
            this.lblExportExcel = (Program.Translations["ExportExcel"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
        }
    }
}



