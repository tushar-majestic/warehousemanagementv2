using HarfBuzzSharp;
using LabMaterials.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace LabMaterials.Pages
{
    public class AddDisbursementModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string RequesterName, Status, Comments, RequestingPlace, StoreName, ItemName, ItemCode, ItemTypeCode;
        public int DId;
        public int StoreId, Quantity;
        public bool InventoryBalanced;
        public DateTime RequestRecievedAt;
        public List<SelectListItem> StatusList;
        public List<Destination> Destinations { get; set; }
        public List<Store> Stores { get; set; }
        public List<ItemInfoByStoreId> ItemInfoByStore { get; set; }

        public string lblAddDisbursement, lblRequesterName, lblRequestReceivedDate, lblQuantity, lblItemCode, lblItemTypeCode, lblItemName, lblStoreName, lblRequestingPlace, lblComments,
            lblDisbursementStatus, lblInventoryBalanced, lblAdd, lblCancel;

        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            FillLables();
            StatusList = (new[] { "NewRequest", "InPreparation", "Dispatched", "Delivered" }).ToList().Select(x => new SelectListItem() { Text = x, Value = x }).ToList();
            RequestRecievedAt = DateTime.Now;
            var dbContext = new LabDBContext();
            Destinations = dbContext.Destinations.ToList();
            Stores = dbContext.Stores.ToList();




        }

        public IActionResult OnGetItemsFromStore(int storeId)
        {
            var dbContext = new LabDBContext();
            var storeIdParam = new SqlParameter("@PSTORE_ID", SqlDbType.Int) { Value = storeId };
            storeIdParam.Direction = ParameterDirection.Input;
            var codeParam = new SqlParameter("@PCODE", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };
            var msgParam = new SqlParameter("@PMSG", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Output };
            var descParam = new SqlParameter("@PDESC", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };

            ItemInfoByStore = dbContext.ItemInfoByStoreIds
                .FromSqlRaw("EXEC PRC_GET_ITEM_INFO_BY_STOREID @PSTORE_ID, @PCODE OUTPUT, @PDESC OUTPUT, @PMSG OUTPUT",
                            storeIdParam, codeParam, descParam, msgParam)
                .ToList();


            return new JsonResult(ItemInfoByStore);

        }

        public IActionResult OnGetRequesterName(int DId)
        {
            var dbContext = new LabDBContext();


            var requesterName = dbContext.Requesters.Where(r => r.DestinationId == DId && r.Ended == null)
                        .Select(r => new { r.ReqName, r.ReqId })
                        .ToList();

            return new JsonResult(requesterName);
        }

        public IActionResult OnPost([FromForm] string RequesterName, [FromForm] string Status, [FromForm] string Comments,
            [FromForm] int DId, [FromForm] bool InventoryBalanced, [FromForm] DateTime RequestRecievedAt, [FromForm] int StoreId,
            [FromForm] int ItemId, [FromForm] int Quantity)
        {
            LogableTask task = LogableTask.NewTask("AddDisbursement");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanDisburseItems)
                {
                    FillLables();
                    this.RequesterName = RequesterName;
                    this.Status = Status;
                    this.Comments = Comments;
                    
                    StatusList = (new[] { "NewRequest", "InPreparation", "Dispatched", "Delivered" }).ToList().Select(x => new SelectListItem() { Text = x, Value = x }).ToList();

                    var dbContext = new LabDBContext();
                    var destination = dbContext.Destinations.SingleOrDefault(dst => dst.DId == DId);
                    this.RequestingPlace = destination.DestinationName;
                    var store = dbContext.Stores.SingleOrDefault(st => st.StoreId == StoreId);
                    this.StoreName = store.StoreName;
                    var item = dbContext.Items.SingleOrDefault(st => st.ItemId == ItemId);
                    this.ItemName = item.ItemName;
                    this.ItemCode = item.ItemCode;
                    this.ItemTypeCode = item.ItemTypeCode;


                    if (string.IsNullOrEmpty(RequesterName))
                        ErrorMsg = (Program.Translations["RequesterNameMissing"])[Lang];
                    else if (string.IsNullOrEmpty(RequestingPlace))
                        ErrorMsg = (Program.Translations["RequestingPlaceMissing"])[Lang];
                    else
                    {

                        /*var disbursement = new DisbursementRequest
                        {
                            DisbursementRequestId = PrimaryKeyManager.GetNextId(),
                            RequesterName = RequesterName,
                            Comments = Comments,
                            RequestingPlace = this.RequestingPlace,
                            Status = Status,
                            InventoryBalanced = InventoryBalanced,
                            ReqReceivedAt = RequestRecievedAt,
                            StoreId = StoreId,
                            Itemcode = this.ItemCode,
                            Itemtypecode = this.ItemTypeCode,
                            ItemQuantity = Quantity
                        };
                        dbContext.DisbursementRequests.Add(disbursement);*/

                        var itemIdParam = new SqlParameter("@PITEM_ID", SqlDbType.Int) { Value = ItemId };
                        var storeIdParam = new SqlParameter("@PSTORE_ID", SqlDbType.Int) { Value = StoreId };
                        var quantityParam = new SqlParameter("@PQUANTITY", SqlDbType.Int) { Value = Quantity };
                        var disbDate = new SqlParameter("@PDIS_DATE", SqlDbType.DateTime) { Value = RequestRecievedAt };
                        var disbStatus = new SqlParameter("@PDIS_STATUS", SqlDbType.VarChar) { Value = Status };
                        var disbComment = new SqlParameter("@PDIS_COMMENT", SqlDbType.VarChar) { Value = string.IsNullOrEmpty(Comments) ? "" : Comments };
                        var disbInvBalance = new SqlParameter("@PINV_BALANCE", SqlDbType.Bit) { Value = InventoryBalanced };
                        var disbReqName = new SqlParameter("@PREQ_NAME", SqlDbType.VarChar) { Value = RequesterName };
                        var disbReqPlace = new SqlParameter("@PREQ_PLACE", SqlDbType.VarChar) { Value = this.RequestingPlace };
                        var disbItemCode = new SqlParameter("@PITEM_CODE", SqlDbType.VarChar) { Value = ItemCode };
                        var disbItemTypeCode = new SqlParameter("@PITEM_TYPE_CODE", SqlDbType.VarChar) { Value = this.ItemTypeCode };

                        itemIdParam.Direction = ParameterDirection.Input;
                        storeIdParam.Direction = ParameterDirection.Input;
                        quantityParam.Direction = ParameterDirection.Input;
                        disbDate.Direction = ParameterDirection.Input;
                        disbStatus.Direction = ParameterDirection.Input;
                        disbComment.Direction = ParameterDirection.Input;
                        disbInvBalance.Direction = ParameterDirection.Input;
                        disbReqName.Direction = ParameterDirection.Input;
                        disbReqPlace.Direction = ParameterDirection.Input;
                        disbItemCode.Direction = ParameterDirection.Input;
                        disbItemTypeCode.Direction = ParameterDirection.Input;

                        var codeParam = new SqlParameter("@PCODE", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };
                        var msgParam = new SqlParameter("@PMSG", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Output };
                        var descParam = new SqlParameter("@PDESC", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };

                        var st = dbContext.Database
                            .ExecuteSqlRaw("EXEC PRC_ADD_DISBURSEMENT @PITEM_ID, @PSTORE_ID,@PQUANTITY, @PDIS_DATE, @PDIS_STATUS, @PDIS_COMMENT, @PINV_BALANCE, @PREQ_NAME, @PREQ_PLACE, @PITEM_CODE, @PITEM_TYPE_CODE, @PCODE OUTPUT, @PDESC OUTPUT, @PMSG OUTPUT",
                                        itemIdParam,storeIdParam, quantityParam, disbDate, disbStatus, disbComment, disbInvBalance, disbReqName, disbReqPlace, disbItemCode, disbItemTypeCode, codeParam, descParam, msgParam);
                        
                        
                        task.LogInfo(MethodBase.GetCurrentMethod(), "Disbursement added");

                        string Message = string.Format("disbursement for added");
                        Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                            Helper.ExtractIP(Request), dbContext, true);

                        return RedirectToPage("./Disbursements");
                    }
                    return Page();
                }
                else
                    return RedirectToPage("./Index?lang=" + Lang);
            }
            catch (Exception ex)
            {
                task.LogError(MethodBase.GetCurrentMethod(), ex);
                ErrorMsg = ex.Message;
                return Page();
            }
            finally { task.EndTask(); }
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
        }
    }
}
