using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace LabMaterials.Pages
{
    public class newreturnRequestModel : BasePageModel
    {

        private readonly LabDBContext _context;

        public newreturnRequestModel(LabDBContext context)
        {
            _context = context;
        }

        // --- Dropdown Data ---
        public List<ItemGroup> ItemGroups { get; set; } = new();
        public List<ItemCard> ItemCards { get; set; } = new();
        public List<Item> Items { get; set; } = new();
        public List<Store> Stores { get; set; } = new();
        public List<Requester> requesters { get; set; } = new();
        public List<ItemCardExtended> ItemsValue { get; set; } = new();



        // --- Metadata ---
        public DateTime CurrentDate => DateTime.Now;

        // --- Localized Labels (sample) ---
        public string lblHome => "Home";
        public string lblDamageItem => "New Return Request";
        public string lblItemName => "Item Name";
        public string lblArabicLanguage => "Arabic";
        public string lblEnglishLanguage => "English";
        public string lblTypeofContract => "Type of Contract";
        public string lblOrderNumber => "Order Number";

        public List<Item> AllItems { get; set; }
        [BindProperty]
        public List<ReturnRequestItem> ReturnItems { get; set; } = new();
        public List<SelectListItem> StateOfMatters { get; set; }
        [BindProperty]
        public ReturnRequest ReturnRequest { get; set; }



        public void OnGet()
        {
            base.ExtractSessionData();
            var dbContext = new LabDBContext();
            StateOfMatters = new List<SelectListItem>
            {
                new SelectListItem { Text = "Solid", Value = "Solid" },
                new SelectListItem { Text = "Liquid", Value = "Liquid" },
                new SelectListItem { Text = "Gas", Value = "Gas" }
            };
            // Ensure list is not null
            if (ReturnItems == null)
            {
                ReturnItems = new List<ReturnRequestItem>();
            }

            // Optional: add one blank row so page renders a row
            ReturnItems.Add(new ReturnRequestItem());

            // ItemsValue = (from ic in dbContext.ItemCards
            //               join i in dbContext.Items on ic.ItemId equals i.ItemId
            //               select new ItemCardExtended
            //               {
            //                   Id = ic.Id,
            //                   ItemCode = ic.ItemCode,
            //                   ItemName = ic.ItemName,
            //                   GroupCode = ic.GroupCode,
            //                   HazardTypeName = ic.HazardTypeName,
            //                   ItemDescription = ic.ItemDescription,
            //                   Chemical = ic.Chemical,
            //                   UnitOfmeasure = ic.UnitOfmeasure,
            //                   ExpiryDate = i.ExpiryDate
            //               }).ToList();
            ItemsValue = (from d in _context.DespensedItems
              join ic in _context.ItemCards on d.ItemCardId equals ic.Id
              join i in _context.Items on ic.ItemId equals i.ItemId
              select new ItemCardExtended
              {
                  Id = ic.Id,
                  ItemCode = ic.ItemCode,
                  ItemName = ic.ItemName,
                  GroupCode = ic.GroupCode,
                  HazardTypeName = ic.HazardTypeName,
                  ItemDescription = ic.ItemDescription,
                  Chemical = ic.Chemical,
                  UnitOfmeasure = ic.UnitOfmeasure,
                  ExpiryDate = i.ExpiryDate
              }) .Distinct()
              .ToList();

            LoadDropdowns();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var dbContext = new LabDBContext();
            // Parse main form values
            var orderNumber = "RR-" + DateTime.UtcNow.Ticks.ToString(); // or your format
            var orderDateStr = Request.Form["OrderDate"];
            var requestingSector = Request.Form["RequestingSector"];
            var applicantsSector = Convert.ToInt32(Request.Form["ApplicantsSector"]);
            var storeId = Convert.ToInt32(Request.Form["StoreId"]);
            var reason = Request.Form["ReasonForReturn"];
            var store = _context.Stores.FirstOrDefault(s => s.StoreId == storeId);

            int? managerId = store?.WarehouseManagerId;



            // ItemsValue = (from ic in _context.ItemCards
            //               join i in _context.Items on ic.ItemId equals i.ItemId
            //               select new ItemCardExtended
            //               {
            //                   Id = ic.Id,
            //                   ItemCode = ic.ItemCode,
            //                   ItemName = ic.ItemName,
            //                   GroupCode = ic.GroupCode,
            //                   HazardTypeName = ic.HazardTypeName,
            //                   ItemDescription = ic.ItemDescription,
            //                   Chemical = ic.Chemical,
            //                   UnitOfmeasure = ic.UnitOfmeasure,
            //                   ExpiryDate = i.ExpiryDate
            //               }).ToList();
            ItemsValue = (from d in _context.DespensedItems
              join ic in _context.ItemCards on d.ItemCardId equals ic.Id
              join i in _context.Items on ic.ItemId equals i.ItemId
              select new ItemCardExtended
              {
                  Id = ic.Id,
                  ItemCode = ic.ItemCode,
                  ItemName = ic.ItemName,
                  GroupCode = ic.GroupCode,
                  HazardTypeName = ic.HazardTypeName,
                  ItemDescription = ic.ItemDescription,
                  Chemical = ic.Chemical,
                  UnitOfmeasure = ic.UnitOfmeasure,
                  ExpiryDate = i.ExpiryDate
              })
              .Distinct()
              .ToList();



            DateTime.TryParse(orderDateStr, out DateTime orderDate);

            var request = new ReturnRequest
            {
                OrderNumber = orderNumber,
                OrderDate = orderDate,
                ToSector = requestingSector,
                FromSectorId = applicantsSector,
                WarehouseId = storeId,
                ManagerId = managerId,
                Reason = reason,
                CreatedAt = DateTime.Now,
                Items = ReturnItems,
                CreatedBy = HttpContext.Session.GetInt32("UserId"),

            };
            // Ensure all flags are false first
            request.IsSurplus = false;
            request.IsExpired = false;
            request.IsInvalid = false;
            request.IsDamaged = false;
            switch (reason)
            {
                case "SurPlus":
                    request.IsSurplus = true;
                    break;
                case "Expired":
                    request.IsExpired = true;
                    break;
                case "Invalid":
                    request.IsInvalid = true;
                    break;
                case "Damaged":
                    request.IsDamaged = true;
                    break;
            }

            // Parse multiple return items
            var itemGroups = Request.Form["itemGroup"];
            var itemCodes = Request.Form["ItemCode"];
            var arabicNames = Request.Form["itemnamearabic"];
            var englishNames = Request.Form["itemnameenglish"];
            var descriptions = Request.Form["ItemDescription"];
            var types = Request.Form["typeofAsset"];
            var chemicals = Request.Form["chemical"];
            var risks = Request.Form["RiskRating"];
            var states = Request.Form["stateofMatter"];
            var expiries = Request.Form["ExpiryDate"];
            var units = Request.Form["UnitofMeasure"];
            var quantities = Request.Form["ReturnedQuantity"];
            var notes = Request.Form["ReturnNotes"];

            for (int i = 0; i < itemCodes.Count; i++)
            {
                if (string.IsNullOrEmpty(itemCodes[i]))
                    continue;

                var item = new ReturnRequestItem
                {
                    ItemGroup = itemGroups[i],
                    ItemCode = itemCodes[i],
                    ItemNameArabic = arabicNames[i],
                    ItemNameEnglish = englishNames[i],
                    ItemDescription = descriptions[i],
                    TypeOfContract = types[i],
                    Chemical = chemicals[i],
                    RiskRating = risks[i],
                    StateOfMatter = states[i],
                    ExpiryDate = string.IsNullOrEmpty(expiries[i]) ? (DateTime?)null : DateTime.Parse(expiries[i], CultureInfo.InvariantCulture),
                    UnitOfMeasure = units[i],
                    ReturnedQuantity = int.TryParse(quantities[i], out var qty) ? qty : 0,
                    ReturnNotes = notes[i]
                };

                request.Items.Add(item);
            }

            _context.ReturnRequests.Add(request);
            await _context.SaveChangesAsync();
            //request.OrderNumber = $"RR-{DateTime.UtcNow:yyyyMMdd}-{ReturnRequest.Id:D4}";
            //await _context.SaveChangesAsync(); // update with real Id
            //                                   // Now that we have the ID, generate the OrderNumber
            request.OrderNumber = $"RR-{DateTime.UtcNow:yyyyMMdd}-{request.Id:D4}";

            // Save again to persist the OrderNumber
            _context.Attach(request).Property(r => r.OrderNumber).IsModified = true;
            await _context.SaveChangesAsync();

            string MessageEn = string.Format(Program.Translations["RetRequestSent"]["en"]);
            string MessageAr = string.Format(Program.Translations["RetRequestSent"]["ar"]);

            // string Message = string.Format("Sent Return Item Request Approve the request or add comments.");
            var msg = new Message
            {
                ReturnRequestId = request.Id,
                ReportType = "ReturnItems",
                SenderId = request.CreatedBy,
                RecipientId = request.ManagerId,
                Content = MessageEn,
                ArContent = MessageAr,
                Type = "",
                CreatedAt = DateTime.UtcNow
            };
            _context.Messages.Add(msg);
            _context.SaveChanges();
            return RedirectToPage("ViewReturnRequests"); // redirect as appropriate
        }

        private void LoadDropdowns()
        {
            ItemGroups = _context.ItemGroups.ToList();
            // ItemCards = _context.ItemCards.ToList();
            ItemCards = (from d in _context.DespensedItems
                        join i in _context.ItemCards on d.ItemCardId equals i.Id
                        select i)
                        .Distinct()
                        .ToList();


            Stores = _context.Stores.ToList();
            requesters = _context.Requesters.ToList();
            AllItems = _context.Items.ToList();
        }

        // Optional: for role-based display
        public bool IsMajesticUser()
        {
            // Add logic for role/user check
            return false;
        }
    }
}
