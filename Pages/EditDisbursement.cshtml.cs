using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace LabMaterials.Pages
{
    public class EditDisbursementModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
 
        public List<User> DeptManagerList {  get; set; }
        public List<User> SupervisorList {  get; set; }
        public List<User> KeeperList {  get; set; }
        public List<ItemCard> ItemsValue { get; set; }
        public List<Destination> Destinations { get; set; }
        public List<Store> Stores { get; set; }
        public List<Item> Items { get; set; }

        public List<ItemCard> ItemCards { get; set;}
        public List<Unit> Units { get; set; }
        public List<ItemGroup> ItemGroups { get; set; }
         [BindProperty]
        public MaterialRequest Report { get; set; }
        [BindProperty]
        public List<DespensedItem> ItemsForReport { get; set; } = new List<DespensedItem>();

        public int? SupervisorId, DeptManagerId, KeeperId;
        public int StoreId, Quantity;



        public string lblUpdateDisbursement, lblRequesterName, lblItemTypeCode, lblItemName, lblStoreName, lblEditDisbursement, lblItemCode, lblRequestReceivedDate, lblRequestingPlace, lblComments, lblQuantity,
            lblDisbursementStatus, lblInventoryBalanced, lblUpdate, lblCancel, lblDisbursements;

        public string lblAddDisbursement,
                 lblAdd, lblItemGroups, lblArabicLanguage, lblEnglishLanguage, lblItemDescription,
                    lblTypeofAsset, lblChemical, lblRiskRating, lblUnitOfMeasure, lblAmountSpent, lblUnitPrice, lblTotalPrice, lblRemove, lblAddMore, lblSerialNumber,
                    lblFiscalYear, lblOrderDate, lblRequestingSector, lblRequestDocumentType, lblRequestDocumentNumber, lblSector;

        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanGenerateDispensingRequest == false)
                RedirectToPage("./Index?lang=" + Lang);

            FillLables();
            var dbContext = new LabDBContext();
            Destinations = dbContext.Destinations.ToList();
            Stores = dbContext.Stores.ToList();
            ItemCards = dbContext.ItemCards.ToList();
            Units = dbContext.Units.ToList();
            Report ??= new MaterialRequest();
            ItemGroups = dbContext.ItemGroups.Where(g => g.Units.Count() > 0).ToList();
            // **Important**: seed one blank DespensedItem so index [0] exists
            ItemsForReport = new List<DespensedItem> { new DespensedItem() };

            //Department Manager list
            var DeptManagerId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Department Manager")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            DeptManagerList = dbContext.Users
                        .Where(u => u.UserGroupId == DeptManagerId)
                        .ToList();

            //General Supervisor list
            var SupervisorId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "General Supervisor")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            SupervisorList = dbContext.Users
                        .Where(u => u.UserGroupId == SupervisorId)
                        .ToList();

             //Keeper  list
            var KeeperId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Warehouse Keeper")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            KeeperList = dbContext.Users
                        .Where(u => u.UserGroupId == KeeperId)
                        .ToList();
            ItemsValue = dbContext.ItemCards
                            .Select(x => new ItemCard
                            {
                                Id = x.Id,
                                ItemId = x.ItemId,
                                ItemName = x.ItemName,
                                GroupCode = x.GroupCode,
                                ItemCode = x.ItemCode,
                                ItemDescription = x.ItemDescription,
                                Chemical = x.Chemical,
                                UnitOfmeasure = x.UnitOfmeasure
                            }).ToList();
        }
         private void FillLables()
        {


            this.lblAddDisbursement = (Program.Translations["AddDisbursement"])[Lang];
            this.lblRequesterName = (Program.Translations["RequesterName"])[Lang];
            this.lblRequestReceivedDate = (Program.Translations["RequestReceivedDate"])[Lang];
            this.lblRequestingPlace = (Program.Translations["RequestingPlace"])[Lang];
            this.lblComments = (Program.Translations["Comments"])[Lang];
            this.lblDisbursementStatus = (Program.Translations["DisbursementStatus"])[Lang];
            this.lblInventoryBalanced = (Program.Translations["InventoryBalanced"])[Lang];

            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblItemTypeCode = (Program.Translations["ItemTypeCode"])[Lang]; 
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblDisbursements = (Program.Translations["Disbursements"])[Lang];
            this.lblItemGroups = (Program.Translations["ItemGroups"])[Lang];
            this.lblArabicLanguage = (Program.Translations["ArabicLanguage"])[Lang];
            this.lblEnglishLanguage = (Program.Translations["EnglishLanguage"])[Lang];
            this.lblItemDescription = (Program.Translations["ItemDescription"])[Lang];
            this.lblTypeofAsset = (Program.Translations["TypeofAsset"])[Lang];
            this.lblChemical = (Program.Translations["Chemical"])[Lang];
            this.lblRiskRating = (Program.Translations["RiskRating"])[Lang];
            this.lblUnitOfMeasure = (Program.Translations["UnitOfMeasure"])[Lang];
            this.lblAmountSpent = (Program.Translations["AmountSpent"])[Lang];
            this.lblUnitPrice = (Program.Translations["UnitPrice"])[Lang];
            this.lblTotalPrice = (Program.Translations["TotalPrice"])[Lang];
            this.lblRemove = (Program.Translations["Remove"])[Lang];
            this.lblAddMore = (Program.Translations["AddMore"])[Lang];
            this.lblSerialNumber = (Program.Translations["SerialNumber"])[Lang];
            this.lblFiscalYear = (Program.Translations["FiscalYear"])[Lang];
            this.lblOrderDate = (Program.Translations["OrderDate"])[Lang];
            this.lblRequestingSector = (Program.Translations["RequestingSector"])[Lang];
            this.lblRequestDocumentType = (Program.Translations["RequestDocumentType"])[Lang];
            this.lblRequestDocumentNumber = (Program.Translations["RequestDocumentNumber"])[Lang];
            this.lblSector = (Program.Translations["Sector"])[Lang];
        }
    }
}
