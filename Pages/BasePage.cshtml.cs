using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class BasePageModel : PageModel
    {
        public string FullName { get; set; }
        public string UserGroupName { get; set; }
        public string Usergroup { get; set; }
        public int? UserId { get; set; }
        public bool IsLDAP { get; set; }
        public bool CanManageStore { get; set; }

        public bool CanReturnItems { get; set; }

        public bool CanManageUsers { get; set; }
        public bool CanManageItems { get; set; }
        public bool CanManageSupplies { get; set; }
        public bool CanDisburseItems { get; set; }
        public bool CanSeeReports { get; set; }
        public bool CanManageItemCard { get; set; }
        public bool CanManageRequests { get; set; }
        public bool CanGenerateReceivingRequest { get; set; }
        public bool CanManageItemGroup { get; set; }
        public bool CanGenerateDispensingRequest { get; set; }

        public bool CanDeleteItems { get; set; }
        public bool CanDeleteSupplier { get; set; }
        public bool CanAddStore { get; set; }



        public string dir { get; set; } = "rtl";
        public string Lang { get; set; } = "ar";


        public string lblView, lblRequests, lblLabMaterials, lblHome, lblNotifications, lblShowHideColumn, lblDisbursement, lblReports, lblReportsInquiries,
        lblManageUsers, lblManageItems, lblManageSupplies, lblManageStores, lblUserProfile, lblLogout, lblDamagedItems, lblLanguage,
        lblWarehouseType, lblManagerName, lblBuildingNumber, lblRoomDesc, lblStatus, lblRoomStatus, lblOpen, lblClosed, lblRoomNumber, lblRoomNameNumber,
        lblNoOfShelves, lblKeeperName, lblKeeperJobNum, lblWarehouseManagerName, lblItemCards, lblReceivingItems, lblSerialNo,
        lblSupplierName, lblRecipientSector, lblPageCount, lblSectorNo, lblDateOfReceipt, lblRecipientWarehouse, lblFiscalYear, lblReceivingDate, lblReceivingWarehouse, lblSupplierType, lblReceipient, lblGeneralSupervisor, lblItemName, lblItemQty,
        lblDocumentDate, lblDocumentNo, lblBasedOnDocument, lblComments, lblUnitPrice, lblTotalPrice, lblQuantity, lblUnitOfMeasure,
        lblItemNameDescription, lblItemNo, lblCount, lblSAR, lblRiyal, lblChiefResponsible, lblTechnicalMember, lblRecipient, lblSignature, lblName,
        lblDate, lblcreateItemCard, lbldeductItemCard, lblItemMovement, lblStores, lblItemCard, lblCeiling, lblReorderLimit, lblMinimum, lblItemNoCode, lblItemNameArabic, lblQuantityReceived, lblDateOfEntryInWarehouse, lblSourceSupplier, lblAmountSpent, lblPartyDirected, lblRemainingBalance, lblDispensingDocumentNo, lblPrint,
        lblViewReceivingReport, lblReceivingReport,lblBack, lblOk, lblRequestDate, lblViewMaterialDispensing, lblSubmit, lblOrderNumber, lblOrderDate, lblSector, lblStoreName, lblReason, lblAction, lblEdit, lblCreatedAt, lblFromSector, lblToSector, lblWarehouse, lblReasonForReturn, lblSurplus, lblExpired, lblInvalid, lblDamaged, lblAdditionalNotes, lblReturnedItems, lblItemNameEng, lblItemCode, lblDesc, lblQty, lblExpiry, lblRecommendedAction, lblCommitteeNotes, lblApplicantSectorName, lblRequestSentSector, lblInspCommitteeRecommendations, lblReturned, lblItemDescription, lblReturnRequestDetails, lblReturnItemRequest, lblFilter, lblUpdate, lblFilterBy, lblLStatus, lblRequestingPlace, lblClearFilters, lblSelectFilterLeft, lblItemGroup, lblChemical, lblHazardType, lblAvailableQuantity, lblManageHazardTypes, lblManageDocumentType, lblUsers, lblAddHazardType,lblDetails, lblAddDocumentType, lblAlerts, lblMaximum, lblReorder, lblNotmoved, lblLastActivity, lblAttachment,lblDownloadFile, lblUnits, lblDelete, lblCreate, lblDocumentTypes,lblDocumentType, lblCreateDocumentType, lblCancel, lblSave, lblId,lblAddStoreType, lblStoreType, lblAddMore, lblRemove, lblItem, lblSelectItemGroup,lblSelectFiscalYear,lblSelectReceivingWarehouse,lblSelectBasedOndocument, lblSelectSupplierName, lblRiskRating, lblCreateReturnRequest, lblStateofMatter, lblSelectState, lblReturnQuantity,lblSelectApplicantSector, lblSelectStore, lblEditReturnRequest, lblAddRecyclingNotesItems, lblSelectRecommendedAction, lblRecyclingNotes,lblAddRecommendations, lblSelectItem, lblReturnNotes, lblRequestingSector, lblApplicantsSector, lblAreYouSure, lblInternal, lblExternal, lblRoomName, lblSelectRoomByNum, lblSelectRoom, lblShelfNumber, lblSelectShelf, lblOutOfWarehouseDate, lblParty, lblDispensingDocumentTypeNumber, lblSelectRequestingSector, lblSelectRequestDocumentType, lblDeptManager, lblSelectDeptMember, lblSelectTechnicalMember, lblSelectGeneralSpervisor, lblSelectAssetType, lblSustainable, lblConsumable, lblTypeName, lblUnitCode, lblGroupName, lblStoreNumber, lblDownloadDesReport ;

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString("UserId")))
            {
                context.Result = new RedirectToPageResult("/Index");
            }
            base.OnPageHandlerExecuting(context);
        }

        public void ExtractSessionData()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                Redirect("/Index?lang=" + Lang);
            }
            if (HttpContext.Session.Keys.Contains("UserId"))
            {
                UserId = HttpContext.Session.GetInt32("UserId");
                FullName = HttpContext.Session.GetString("FullName");
                UserGroupName = HttpContext.Session.GetString("UserGroup");
                Usergroup = HttpContext.Session.GetString("UserGroup");
                IsLDAP = HttpContext.Session.GetInt32("IsLDAP") == 1;

                CanGenerateDispensingRequest = HttpContext.Session.GetInt32("CanGenerateDispensingRequest") == 1;
                CanReturnItems = HttpContext.Session.GetInt32("CanReturnItems") == 1;

                CanManageStore = HttpContext.Session.GetInt32("CanManageStore") == 1;
                CanManageUsers = HttpContext.Session.GetInt32("CanManageUsers") == 1;
                CanManageItems = HttpContext.Session.GetInt32("CanManageItems") == 1;
                CanManageSupplies = HttpContext.Session.GetInt32("CanManageSupplies") == 1;
                CanDisburseItems = HttpContext.Session.GetInt32("CanDisburseItems") == 1;
                CanSeeReports = HttpContext.Session.GetInt32("CanSeeReports") == 1;
                CanManageItemGroup = HttpContext.Session.GetInt32("CanManageItemGroup") == 1;
                CanManageRequests = HttpContext.Session.GetInt32("CanManageRequests") == 1;
                CanAddStore = HttpContext.Session.GetInt32("CanAddStore") == 1;
                CanGenerateReceivingRequest = HttpContext.Session.GetInt32("CanGenerateReceivingRequest") == 1;
                CanDeleteSupplier = HttpContext.Session.GetInt32("CanDeleteSupplier") == 1;
                CanManageItemCard = HttpContext.Session.GetInt32("CanManageItemCard") == 1;
                CanGenerateDispensingRequest = HttpContext.Session.GetInt32("CanGenerateDispensingRequest") == 1;
                CanDeleteItems = HttpContext.Session.GetInt32("CanDeleteItems") == 1;
                dir = HttpContext.Session.GetString("Lang") == "en" ? "ltr" : "rtl";
                Lang = HttpContext.Session.GetString("Lang") == "en" ? "en" : "ar";
                FillLables();

            }
            else
            {
                HttpContext.Response.Redirect("/Index?lang=" + Lang);
            }

        }

        public bool IsMajesticUser()
        {
            var fullName = HttpContext.Session.GetString("FullName") ?? "";
            return fullName.StartsWith("majestic", StringComparison.OrdinalIgnoreCase);
        }
        private void FillLables()
        {


            this.lblDisbursement = (Program.Translations["Disbursements"])[Lang];
            this.lblReports = (Program.Translations["Reports"])[Lang];
            this.lblReportsInquiries = (Program.Translations["ReportsInquiries"])[Lang];
            this.lblManageItems = (Program.Translations["ReceivingItems"])[Lang];
            this.lblManageStores = (Program.Translations["ManageStore"])[Lang];
            this.lblManageSupplies = (Program.Translations["Supplies"])[Lang];
            this.lblManageUsers = (Program.Translations["ManageUsers"])[Lang];
            this.lblHome = (Program.Translations["Home"])[Lang];
            this.lblNotifications = (Program.Translations["Notifications"])[Lang];
            this.lblView = (Program.Translations["View"])[Lang];
            this.lblUserProfile = (Program.Translations["UserProfile"])[Lang];
            this.lblLogout = (Program.Translations["Logout"])[Lang];
            this.lblLabMaterials = (Program.Translations["LabMaterials"])[Lang];
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblLanguage = (Program.Translations["Language"])[Lang];

            this.lblRequests = (Program.Translations["Requests"])[Lang];

            this.lblShowHideColumn = (Program.Translations["ShowHideColumn"])[Lang];
            this.lblWarehouseType = (Program.Translations["WarehouseType"])[Lang];
            this.lblManagerName = (Program.Translations["ManagerName"])[Lang];
            this.lblWarehouseManagerName = (Program.Translations["WarehouseManagerName"])[Lang];
            this.lblBuildingNumber = (Program.Translations["BuildingNumber"])[Lang];
            this.lblRoomDesc = (Program.Translations["RoomDesc"])[Lang];
            this.lblStatus = (Program.Translations["WarehouseStatus"])[Lang];

            this.lblOpen = (Program.Translations["Open"])[Lang];
            this.lblClosed = (Program.Translations["Closed"])[Lang];
            this.lblRoomStatus = (Program.Translations["RoomStatus"])[Lang];
            this.lblRoomNumber = (Program.Translations["RoomNumber"])[Lang];
            this.lblNoOfShelves = (Program.Translations["NoOfShelves"])[Lang];
            this.lblKeeperJobNum = (Program.Translations["KeeperJobNum"])[Lang];
            this.lblKeeperName = (Program.Translations["KeeperName"])[Lang];
            this.lblItemCards = (Program.Translations["ItemCards"])[Lang];
            this.lblItemCard = (Program.Translations["ItemCard"])[Lang];


            this.lblReceivingItems = (Program.Translations["ReceivingItems"])[Lang];
            this.lblSerialNo = (Program.Translations["SerialNo"])[Lang];
            this.lblSupplierName = (Program.Translations["SupplierName"])[Lang];
            this.lblRecipientSector = (Program.Translations["RecipientSector"])[Lang];
            this.lblPageCount = (Program.Translations["PageCount"])[Lang];
            this.lblSectorNo = (Program.Translations["SectorNo"])[Lang];
            this.lblDateOfReceipt = (Program.Translations["DateOfReceipt"])[Lang];
            this.lblRecipientWarehouse = (Program.Translations["RecipientWarehouse"])[Lang];
            this.lblFiscalYear = (Program.Translations["FiscalYear"])[Lang];
            this.lblDocumentDate = (Program.Translations["DocumentDate"])[Lang];
            this.lblDocumentNo = (Program.Translations["DocumentNo"])[Lang];
            this.lblBasedOnDocument = (Program.Translations["BasedOnDocument"])[Lang];
            this.lblComments = (Program.Translations["Comm"])[Lang];
            this.lblUnitPrice = (Program.Translations["UnitPrice"])[Lang];
            this.lblTotalPrice = (Program.Translations["TotalPrice"])[Lang];
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblUnitOfMeasure = (Program.Translations["UnitOfMeasure"])[Lang];
            this.lblItemNameDescription = (Program.Translations["ItemNameDescription"])[Lang];
            this.lblItemNo = (Program.Translations["ItemNo"])[Lang];
            this.lblCount = (Program.Translations["Count"])[Lang];
            this.lblSAR = (Program.Translations["SAR"])[Lang];
            this.lblChiefResponsible = (Program.Translations["ChiefRes"])[Lang];
            this.lblTechnicalMember = (Program.Translations["TechnicalMember"])[Lang];
            this.lblRecipient = (Program.Translations["Recipient"])[Lang];
            this.lblName = (Program.Translations["Name"])[Lang];
            this.lblSignature = (Program.Translations["Signature"])[Lang];
            this.lblDate = (Program.Translations["Date"])[Lang];
            this.lblcreateItemCard = (Program.Translations["CreateItemCard"][Lang]);
            this.lbldeductItemCard = (Program.Translations["DeductItemCard"][Lang]);

            this.lblStores = (Program.Translations["Stores"])[Lang];
            this.lblRiyal = (Program.Translations["Riyal"])[Lang];
            this.lblItemMovement = (Program.Translations["ItemMovement"][Lang]);
            this.lblReceivingDate = (Program.Translations["ReceivingDate"][Lang]);
            this.lblReceivingWarehouse = (Program.Translations["ReceivingWarehouse"][Lang]);
            this.lblSupplierType = (Program.Translations["SupplierType"][Lang]);
            this.lblReceipient = (Program.Translations["Receipient"][Lang]);
            this.lblGeneralSupervisor = (Program.Translations["GeneralSupervisor"][Lang]);
            this.lblItemName = (Program.Translations["ItemName"][Lang]);
            this.lblItemQty = (Program.Translations["ItemQuantity"][Lang]);
            this.lblRoomNameNumber = (Program.Translations["RoomNameNumber"][Lang]);
            this.lblCeiling = (Program.Translations["Ceiling"][Lang]);
            this.lblReorderLimit = (Program.Translations["ReorderLimit"][Lang]);
            this.lblMinimum = (Program.Translations["Minimum"][Lang]);
            this.lblItemNoCode = (Program.Translations["ItemNoCode"][Lang]);
            this.lblItemNameArabic = (Program.Translations["ItemNameArabic"][Lang]);
            this.lblQuantityReceived = (Program.Translations["QuantityReceived"][Lang]);
            this.lblDateOfEntryInWarehouse = (Program.Translations["DateOfEntry"][Lang]);
            this.lblSourceSupplier = (Program.Translations["SourceSupplier"][Lang]);
            this.lblAmountSpent = (Program.Translations["AmountSpent"][Lang]);
            this.lblPartyDirected = (Program.Translations["PartyDirected"][Lang]);
            this.lblRemainingBalance = (Program.Translations["RemainingBalance"][Lang]);
            this.lblDispensingDocumentNo = (Program.Translations["DispensingDocumentNo"][Lang]);
            this.lblPrint = (Program.Translations["Print"][Lang]);
            this.lblViewReceivingReport = (Program.Translations["ViewReceivingReport"][Lang]);
            this.lblReceivingReport = (Program.Translations["ReceivingReport"][Lang]);
            this.lblBack = (Program.Translations["Back"][Lang]);
            this.lblOk = (Program.Translations["Ok"][Lang]);
            this.lblRequestDate = (Program.Translations["RequestDate"][Lang]);
            this.lblViewMaterialDispensing = (Program.Translations["ViewMaterialDispensing"][Lang]);
            this.lblSubmit = (Program.Translations["Submit"][Lang]);
            this.lblOrderNumber = (Program.Translations["OrderNumber"][Lang]);
            this.lblOrderDate = (Program.Translations["OrderDate"][Lang]);
            this.lblSector = (Program.Translations["Sector"][Lang]);
            this.lblStoreName = (Program.Translations["StoreName"][Lang]);
            this.lblReason = (Program.Translations["Reason"][Lang]);
            this.lblAction = (Program.Translations["Action"][Lang]);
            this.lblEdit = (Program.Translations["Edit"][Lang]);
            this.lblCreatedAt = (Program.Translations["CreatedAt"][Lang]);
            this.lblFromSector = (Program.Translations["FromSector"][Lang]);
            this.lblToSector = (Program.Translations["ToSector"][Lang]);
            this.lblWarehouse = (Program.Translations["Warehouse"][Lang]);
            this.lblReasonForReturn = (Program.Translations["ReasonForReturn"][Lang]);
            this.lblSurplus = (Program.Translations["SurPlus"][Lang]);
            this.lblExpired = (Program.Translations["Expired"][Lang]);
            this.lblInvalid = (Program.Translations["Invalid"][Lang]);
            this.lblDamaged = (Program.Translations["Damaged"][Lang]);
            this.lblAdditionalNotes = (Program.Translations["AdditionalNotes"][Lang]);
            this.lblReturnedItems = (Program.Translations["ReturnedItems"][Lang]);
            this.lblItemNameEng = (Program.Translations["ItemNameEng"][Lang]);
            this.lblItemCode = (Program.Translations["ItemCode"][Lang]);
            this.lblDesc = (Program.Translations["Desc"][Lang]);
            this.lblQty = (Program.Translations["Qty"][Lang]);
            this.lblExpiry = (Program.Translations["ExpiryDate"][Lang]);
            this.lblRecommendedAction = (Program.Translations["RecommendedAction"][Lang]);
            this.lblCommitteeNotes = (Program.Translations["CommitteeNotes"][Lang]);
            this.lblApplicantSectorName = (Program.Translations["ApplicantSectorName"][Lang]);
            this.lblRequestSentSector = (Program.Translations["RequestSentSector"][Lang]);
            this.lblInspCommitteeRecommendations = (Program.Translations["InspCommitteeRecommendations"][Lang]);
            this.lblReturned = (Program.Translations["Returned"][Lang]);
            this.lblItemDescription = (Program.Translations["ItemDescription"][Lang]);
            this.lblReturnRequestDetails = (Program.Translations["ReturnRequestDetails"][Lang]);
            this.lblReturnItemRequest = (Program.Translations["ReturnItemRequest"][Lang]);
            this.lblFilter = (Program.Translations["Filter"][Lang]);
            this.lblUpdate = (Program.Translations["Update"][Lang]);
            this.lblFilterBy = (Program.Translations["FilterBy"][Lang]);
            this.lblLStatus = (Program.Translations["Status"][Lang]);
            this.lblRequestingPlace = (Program.Translations["RequestingPlace"][Lang]);
            this.lblClearFilters = (Program.Translations["ClearFilters"][Lang]);
            this.lblSelectFilterLeft = (Program.Translations["SelectFilterLeft"][Lang]);
            this.lblItemGroup = (Program.Translations["ItemGroup"][Lang]);
            this.lblChemical = (Program.Translations["Chemical"][Lang]);
            this.lblHazardType = (Program.Translations["HazardType"][Lang]);
            this.lblAvailableQuantity = (Program.Translations["AvailableQuantity"][Lang]);
            this.lblManageHazardTypes = (Program.Translations["ManageHazardTypes"][Lang]);
            this.lblManageDocumentType = (Program.Translations["ManageDocumentType"][Lang]);
            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblAddHazardType = (Program.Translations["AddHazardType"])[Lang];
            this.lblDetails = (Program.Translations["Details"])[Lang];
            this.lblAddDocumentType = (Program.Translations["AddDocumentType"])[Lang];
            this.lblAlerts = (Program.Translations["Alerts"])[Lang];
            this.lblMaximum = (Program.Translations["Maximum"])[Lang];
            this.lblReorder = (Program.Translations["Reorder"])[Lang];
            this.lblNotmoved = (Program.Translations["Notmoved"])[Lang];
            this.lblLastActivity = (Program.Translations["LastActivity"])[Lang];
            this.lblAttachment = (Program.Translations["Attachment"])[Lang];
            this.lblDownloadFile = (Program.Translations["DownloadFile"])[Lang];
            this.lblUnits = (Program.Translations["Units"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblCreate = (Program.Translations["Create"])[Lang];
            this.lblDocumentTypes = (Program.Translations["DocumentTypes"])[Lang];
            this.lblDocumentType = (Program.Translations["DocumentType"])[Lang];
            this.lblCreateDocumentType = (Program.Translations["CreateDocumentType"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblSave = (Program.Translations["Save"])[Lang];
            this.lblId = (Program.Translations["Id"])[Lang];
            this.lblAddStoreType = (Program.Translations["AddStoreType"])[Lang];
            this.lblStoreType = (Program.Translations["StoreType"])[Lang];
            this.lblAddMore = (Program.Translations["AddMore"])[Lang];
            this.lblItem = (Program.Translations["Item"])[Lang];
            this.lblRemove = (Program.Translations["Remove"])[Lang];
            this.lblSelectItemGroup = (Program.Translations["SelectItemGroup"])[Lang];
            this.lblSelectFiscalYear = (Program.Translations["SelectFiscalYear"])[Lang];
            this.lblSelectReceivingWarehouse = (Program.Translations["SelectReceivingWarehouse"])[Lang];
            this.lblSelectBasedOndocument = (Program.Translations["SelectBasedOndocument"])[Lang];
            this.lblSelectSupplierName = (Program.Translations["SelectSupplierName"])[Lang];
            this.lblRiskRating = (Program.Translations["RiskRating"])[Lang];
            this.lblCreateReturnRequest = (Program.Translations["CreateReturnRequest"])[Lang];
            this.lblStateofMatter = (Program.Translations["StateofMatter"])[Lang];
            this.lblSelectState = (Program.Translations["SelectState"])[Lang];
            this.lblReturnQuantity = (Program.Translations["ReturnQuantity"])[Lang];
            this.lblSelectApplicantSector = (Program.Translations["SelectApplicantSector"])[Lang];
            this.lblSelectStore = (Program.Translations["SelectStore"])[Lang];
            this.lblEditReturnRequest = (Program.Translations["EditReturnRequest"])[Lang];
            this.lblAddRecyclingNotesItems = (Program.Translations["AddRecyclingNotesItems"])[Lang];
            this.lblSelectRecommendedAction = (Program.Translations["SelectRecommendedAction"])[Lang];
            this.lblRecyclingNotes = (Program.Translations["RecyclingNotes"])[Lang];
            this.lblAddRecommendations = (Program.Translations["AddRecommendations"])[Lang];
            this.lblSelectItem = (Program.Translations["SelectItem"])[Lang];
            this.lblReturnNotes = (Program.Translations["ReturnNotes"])[Lang];
            this.lblRequestingSector = (Program.Translations["RequestingSector"])[Lang];
            this.lblApplicantsSector = (Program.Translations["ApplicantsSector"])[Lang];
            this.lblAreYouSure = (Program.Translations["AreYouSure"])[Lang];
            this.lblInternal = (Program.Translations["Internal"])[Lang];
            this.lblExternal = (Program.Translations["External"])[Lang];
            this.lblRoomName = (Program.Translations["RoomName"])[Lang];
            this.lblSelectRoomByNum = (Program.Translations["SelectRoomByNum"])[Lang];
            this.lblSelectRoom = (Program.Translations["SelectRoom"])[Lang];
            this.lblShelfNumber = (Program.Translations["ShelfNumber"])[Lang];
            this.lblSelectShelf = (Program.Translations["SelectShelf"])[Lang];
            this.lblOutOfWarehouseDate = (Program.Translations["OutOfWarehouseDate"])[Lang];
            this.lblParty = (Program.Translations["Party"])[Lang];
            this.lblDispensingDocumentTypeNumber = (Program.Translations["DispensingDocumentTypeNumber"])[Lang];
            this.lblSelectRequestingSector = (Program.Translations["SelectRequestingSector"])[Lang];
            this.lblSelectRequestDocumentType = (Program.Translations["SelectRequestDocumentType"])[Lang];
            this.lblDeptManager = (Program.Translations["DeptManager"])[Lang];
            this.lblSelectDeptMember = (Program.Translations["SelectDeptMember"])[Lang];
            this.lblSelectTechnicalMember = (Program.Translations["SelectTechnicalMember"])[Lang];
            this.lblSelectGeneralSpervisor = (Program.Translations["SelectGeneralSpervisor"])[Lang];
            this.lblSelectAssetType = (Program.Translations["SelectAssetType"])[Lang];
            this.lblSustainable = (Program.Translations["Sustainable"])[Lang];
            this.lblConsumable = (Program.Translations["Consumable"])[Lang];
            this.lblTypeName = (Program.Translations["TypeName"])[Lang];
            this.lblUnitCode = (Program.Translations["UnitCode"])[Lang];
            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblStoreNumber = (Program.Translations["StoreNumber"])[Lang];
            this.lblDownloadDesReport = (Program.Translations["DownloadDesReport"])[Lang];








        }
    }
}
