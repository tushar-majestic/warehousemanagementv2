using DocumentFormat.OpenXml.Spreadsheet;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LabMaterials.Pages
{
    public class ManageItemCardsModel : BasePageModel
    {
        public string lblTotalItem, lblAddItemCard, lblItemGroups, lblSearch, lblItems, lblExportExcel, lblPrintTable, lblItemName,
        lblStateofMatter, lblRiskRating, lblChemical, lblSubmit, lblUnitCode, lblHazardType, lblItemCode, lblGroupName, lblStores;

        public List<string> SelectedColumns { get; set; } = new List<string>();
        // public List<ItemCardViewModels> ItemCardView {get; set;}
        public List<ItemCardViewModels> ItemCardView { get; set; } = new List<ItemCardViewModels>();
        public List<ItemCardViewModels> ItemCardViewAll { get; set; } = new List<ItemCardViewModels>();
        public List<ItemCardViewModels> ItemCardViewPaged { get; set; }
        public int? UserId;
        [BindProperty]
        public string ItemName { get; set; }
        private readonly LabDBContext _context;
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }

        public ManageItemCardsModel(LabDBContext context)
        {
            _context = context;
        }


        // public async Task OnGetAsync() 
        // {   base.ExtractSessionData();
        //     this.UserId =  HttpContext.Session.GetInt32("UserId");
        //     var dbContext = new LabDBContext();

        //     ItemCardView = await (from item in _context.ItemCards
        //               join store in _context.Stores on item.StoreId equals store.StoreId into storeGroup
        //               from store in storeGroup.DefaultIfEmpty()
        //               select new ItemCardViewModels
        //               {
        //                   GroupCode = item.GroupCode,
        //                   ItemCode = item.ItemCode,
        //                   ItemName = item.ItemName,
        //                   ItemDescription = item.ItemDescription,
        //                   UnitOfMeasure = item.UnitOfmeasure,
        //                   Chemical = item.Chemical,
        //                   HazardTypeName = item.HazardTypeName,
        //                   QuantityAvailable = item.QuantityAvailable,
        //                   WarehouseName = store.StoreName
        //             }).ToListAsync();

        //     TotalItems = ItemCardView.Count();

        
            // Old query with batch 
            // ItemCardView = await (from item in _context.ItemCards
            //           join batch in _context.ItemCardBatches on item.Id equals batch.ItemCardId
            //           join room in _context.Rooms on batch.RoomId equals room.RoomId into roomGroup
            //           from room in roomGroup.DefaultIfEmpty()
            //           join shelf in _context.Shelves on batch.ShelfId equals shelf.ShelfId into shelfGroup
            //           from shelf in shelfGroup.DefaultIfEmpty()
            //           join supplier in _context.Suppliers on batch.SupplierId equals supplier.SupplierId into supplierGroup
            //           from supplier in supplierGroup.DefaultIfEmpty()
            //           join store in _context.Stores on item.StoreId equals store.StoreId into storeGroup
            //           from store in storeGroup.DefaultIfEmpty()
            //           select new ItemCardView
            //           {
            //               GroupCode = item.GroupCode,
            //               ItemCode = item.ItemCode,
            //               ItemName = item.ItemName,
            //               ItemDescription = item.ItemDescription,
            //               UnitOfMeasure = item.UnitOfmeasure,
            //               Chemical = item.Chemical,
            //               HazardTypeName = item.HazardTypeName,
            //               ExpiryDate = batch.ExpiryDate,

            //               TypeOfAsset = batch.TypeOfAsset,
            //                 Minimum = batch.Minimum,
            //                 ReorderLimit = batch.ReorderLimit,
            //               WarehouseName = store.StoreName,

            //               QuantityReceived = item.QuantityAvailable,
            //               DateOfEntry = batch.DateOfEntry,

            //               RoomName = room.RoomName,
            //                ShelfName = shelf.ShelfId.ToString(),
            //               SupplierName = supplier.SupplierName,
            //               DocumentType = batch.DocumentType,
            //               ReceiptDocumentNumber = batch.ReceiptDocumentnumber
            //           }).ToListAsync();


            // new 
            // ItemCardView = await (from item in _context.ItemCards
            //           join store in _context.Stores on item.StoreId equals store.StoreId into storeGroup
            //           from store in storeGroup.DefaultIfEmpty()
            //           select new ItemCardViewModels
            //           {
            //               GroupCode = item.GroupCode,
            //               ItemCardId = item.Id,
            //               ItemCode = item.ItemCode,
            //               ItemName = item.ItemName,
            //               ItemDescription = item.ItemDescription,
            //               UnitOfMeasure = item.UnitOfmeasure,
            //               Chemical = item.Chemical,
            //               HazardTypeName = item.HazardTypeName,
            //               QuantityAvailable = item.QuantityAvailable,
            //               WarehouseName = store.StoreName,
                          
            //         }).ToListAsync();

            // TotalItems = ItemCardView.Count();
        //     FillLables();
        // }

        public void OnGet(string? ItemName, int page = 1)
        {
            base.ExtractSessionData();
            FillLables();
            LoadSelectedColumns();
            if (this.CanManageItems)
            {
                if (HttpContext.Request.Query.ContainsKey("page"))
                {
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.ItemName = ItemName;
                    FillData(ItemName, page);

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
                    string pageName = "ManageItemsCards";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        // SelectedColumns = new List<string>();
                        string selectedColumns = "itemCode,itemName";
                        SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    }
                }
            }
        }
      
        public IActionResult OnPostView([FromForm] string ItemCardId)
        {
            var dbContext = new LabDBContext();
            HttpContext.Session.SetString("ItemCardId", ItemCardId);
            return RedirectToPage("./viewItemCards");
        }

        // public void OnPostSearch([FromForm] string ItemName)
        // {
        //     base.ExtractSessionData();
        //     FillLables();
        //     CurrentPage = 1;
        //     this.ItemName = ItemName;
        //     if (CanManageItems)
        //         {
        //             FillData(ItemName );
        //         }
            
        //         else
        //             RedirectToPage("./Index?lang=" + Lang);
        // }

        public IActionResult OnPostAction(string ItemName, string action, List<string> columns)
        {
            base.ExtractSessionData();
            FillLables();

            if (action == "search")
            {
                CurrentPage = 1;
                this.ItemName = ItemName;
                if (CanManageItems)
                {
                    FillData(ItemName);
                    LoadSelectedColumns();
                }

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "ManageItemCards";
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {
                    string selectedColumns = string.Join(",", columns);
                    this.ItemName = ItemName;
                    FillData(ItemName, 1);

                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "ManageItemsCards";
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


        private void FillData(string ItemName, int page = 1)
        {
            FillLables();

            this.UserId = HttpContext.Session.GetInt32("UserId");

            var query = (from item in _context.ItemCards
                         join store in _context.Stores on item.StoreId equals store.StoreId into storeGroup
                         from store in storeGroup.DefaultIfEmpty()
                         select new ItemCardViewModels
                         {
                             GroupCode = item.GroupCode,
                             ItemCode = item.ItemCode,
                             ItemName = item.ItemName,
                             ItemDescription = item.ItemDescription,
                             UnitOfMeasure = item.UnitOfmeasure,
                             Chemical = item.Chemical,
                             HazardTypeName = item.HazardTypeName,
                             QuantityAvailable = item.QuantityAvailable,
                             WarehouseName = store.StoreName
                         });
            if (!string.IsNullOrWhiteSpace(ItemName))
                query = query.Where(i => i.ItemName.Contains(ItemName));

            TotalItems = query.Count();
            TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
            var list = query.ToList();
            ItemCardView = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            ItemCardViewAll = list;
            CurrentPage = page;
        }


        private void FillLables()
        {
            this.Lang = HttpContext.Session.GetString("Lang");
            this.lblAddItemCard = (Program.Translations["AddItemCard"])[Lang];
            this.lblItemGroups = (Program.Translations["ItemGroups"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblItemCards = (Program.Translations["ItemCards"])[Lang];
            this.lblExportExcel = (Program.Translations["ExportExcel"])[Lang];
            this.lblPrintTable = (Program.Translations["PrintTable"])[Lang];


            this.lblItems = (Program.Translations["Items"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblHazardType = (Program.Translations["HazardType"])[Lang];
            this.lblUnitCode = (Program.Translations["UnitCode"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblChemical = (Program.Translations["Chemical"])[Lang];
            this.lblRiskRating = (Program.Translations["RiskRating"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];
            this.lblStateofMatter = (Program.Translations["StateofMatter"])[Lang];
        }
    }

   
}