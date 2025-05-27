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
        public bool CanDeleteSupplier { get; set; }


        public string dir { get; set; } = "rtl";
        public string Lang { get; set; } = "ar";


        public string lblView, lblRequests, lblLabMaterials, lblHome, lblNotifications, lblShowHideColumn, lblDisbursement, lblReports, lblReportsInquiries,
        lblManageUsers, lblManageItems, lblManageSupplies, lblManageStores, lblUserProfile, lblLogout, lblDamagedItems, lblLanguage,
        lblWarehouseType, lblManagerName, lblBuildingNumber, lblRoomDesc, lblStatus, lblRoomStatus, lblOpen, lblClosed, lblRoomNumber,
        lblNoOfShelves, lblKeeperName, lblKeeperJobNum, lblWarehouseManagerName, lblItemCards, lblReceivingItems, lblSerialNo,
        lblSupplierName, lblRecipientSector, lblPageCount, lblSectorNo, lblDateOfReceipt, lblRecipientWarehouse, lblFiscalYear,
        lblDocumentDate, lblDocumentNo, lblBasedOnDocument, lblComments, lblUnitPrice, lblTotalPrice, lblQuantity, lblUnitOfMeasure,
        lblItemNameDescription, lblItemNo, lblCount, lblSAR, lblChiefResponsible, lblTechnicalMember, lblRecipient, lblSignature, lblName,
        lblDate, lblcreateItemCard, lbldeductItemCard, lblItemMovement, lblStores;



        public void ExtractSessionData()
        {

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
                CanGenerateReceivingRequest = HttpContext.Session.GetInt32("CanGenerateReceivingRequest") == 1;
                CanDeleteSupplier = HttpContext.Session.GetInt32("CanDeleteSupplier") == 1;
                CanManageItemCard = HttpContext.Session.GetInt32("CanManageItemCard") == 1;
                CanGenerateDispensingRequest = HttpContext.Session.GetInt32("CanGenerateDispensingRequest") == 1;
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

            this.lblItemMovement = (Program.Translations["ItemMovement"][Lang]);


        }
    }
}
