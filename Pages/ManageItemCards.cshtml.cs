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
        public string    lblTotalItem, lblAddItemCard, lblItemGroups, lblSearch, lblItems, lblExportExcel, lblPrintTable;

        public List<string> SelectedColumns { get; set; } = new List<string>();
        public List<ItemCardView> ItemCardView {get; set;}
        public int? UserId;
        private readonly LabDBContext _context;
        public int TotalItems { get; set; }

         public ManageItemCardsModel(LabDBContext context)
        {
            _context = context;
        }


        public async Task OnGetAsync() 
        {   base.ExtractSessionData();
            this.UserId =  HttpContext.Session.GetInt32("UserId");
            var dbContext = new LabDBContext();
            ItemCardView = await (from item in _context.ItemCards
                      join batch in _context.ItemCardBatches on item.Id equals batch.ItemCardId
                      join room in _context.Rooms on batch.RoomId equals room.RoomId into roomGroup
                      from room in roomGroup.DefaultIfEmpty()
                      join shelf in _context.Shelves on batch.ShelfId equals shelf.ShelfId into shelfGroup
                      from shelf in shelfGroup.DefaultIfEmpty()
                      join supplier in _context.Suppliers on batch.SupplierId equals supplier.SupplierId into supplierGroup
                      from supplier in supplierGroup.DefaultIfEmpty()
                      join store in _context.Stores on item.StoreId equals store.StoreId into storeGroup
                      from store in storeGroup.DefaultIfEmpty()
                      select new ItemCardView
                      {
                          GroupCode = item.GroupCode,
                          ItemCode = item.ItemCode,
                          ItemName = item.ItemName,
                          ItemDescription = item.ItemDescription,
                          UnitOfMeasure = item.UnitOfmeasure,
                          Chemical = item.Chemical,
                          HazardTypeName = item.HazardTypeName,
                          ExpiryDate = item.ExpiryDate,

                        //   TypeOfAsset = batch.TypeOfAsset,
                          Minimum = batch.Minimum,
                          ReorderLimit = batch.ReorderLimit,
                          WarehouseName = store.StoreName,
                          QuantityReceived = batch.QuantityReceived,
                          DateOfEntry = batch.DateOfEntry,

                          RoomName = room.RoomName,
                           ShelfName = shelf.ShelfId.ToString(),
                          SupplierName = supplier.SupplierName,
                          DocumentType = batch.DocumentType,
                          ReceiptDocumentNumber = batch.ReceiptDocumentnumber
                      }).ToListAsync();

                TotalItems = ItemCardView.Count();

            FillLables();
        }

        public IActionResult OnPostView([FromForm] string ReportId)
        {
            var dbContext = new LabDBContext();

            // HttpContext.Session.SetString("ReportId", ReportId);


            return RedirectToPage("/viewItemCards");
        }

        public void OnPostSearch([FromForm] string itemcard)
        {   
            base.ExtractSessionData();
        }

         private void FillLables()
        {
            this.Lang =  HttpContext.Session.GetString("Lang");

         
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblAddItemCard = (Program.Translations["AddItemCard"])[Lang];
            this.lblItemGroups = (Program.Translations["ItemGroups"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblItemCards = (Program.Translations["ItemCards"])[Lang];
            this.lblExportExcel = (Program.Translations["ExportExcel"])[Lang];
            this.lblPrintTable = (Program.Translations["PrintTable"])[Lang];
        }
    }

   
}