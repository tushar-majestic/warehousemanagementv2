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
        public string RequesterName, Status, Comments, RequestingPlace, StoreName, ItemName, ItemCode, ItemTypeCode;
        public int? Quantity, StoreId, ItemId;
        public int DId, rId;
        public List<DisbursementInfo> Disbursement { get; set; }
        public List<Destination> Destinations { get; set; }
        public bool InventoryBalanced;
        public DateTime RequestRecievedAt;
        public List<Store> Stores { get; set; }
        public int DisbursementID;
        public List<SelectListItem> StatusList;
        public List<ItemInfoByStoreId> ItemInfoByStore { get; set; }

        public string lblUpdateDisbursement, lblRequesterName, lblItemTypeCode, lblItemName, lblStoreName, lblEditDisbursement, lblItemCode, lblRequestReceivedDate, lblRequestingPlace, lblComments, lblQuantity,
            lblDisbursementStatus, lblInventoryBalanced, lblUpdate, lblCancel, lblDisbursements;

        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            FillLables();
            StatusList = (new[] { "NewRequest", "InPreparation", "Dispatched", "Dilivered" }).ToList().Select(x => new SelectListItem() { Text = x, Value= x}).ToList();
            RequestRecievedAt = DateTime.Now;
            var dbContext = new LabDBContext();
            Destinations = dbContext.Destinations.ToList();
            var query = from d in dbContext.DisbursementRequests
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
                            StoreName = d.Store.StoreName
                        };
            var disbursement = dbContext.DisbursementRequests.Single(d => d.DisbursementRequestId == HttpContext.Session.GetInt32("DisbursementID"));
            this.RequesterName = disbursement.RequesterName;
            this.Status = disbursement.Status;
            this.Comments = disbursement.Comments;
            this.RequestingPlace = disbursement.RequestingPlace;
            this.RequestRecievedAt = disbursement.ReqReceivedAt;
            this.InventoryBalanced = disbursement.InventoryBalanced;
            this.DisbursementID = disbursement.DisbursementRequestId;
            this.ItemCode = disbursement.Itemcode;
            this.ItemTypeCode = disbursement.Itemtypecode;
            Quantity = disbursement.ItemQuantity;
            this.StoreId = disbursement.StoreId;
            this.StoreName = disbursement.Store.StoreName;
            RequesterName = disbursement.RequesterName;
            var test = from d in dbContext.DisbursementRequests
                       join dest in dbContext.Destinations on d.RequestingPlace.ToLower() equals dest.DestinationName.ToLower()
                       select new
                       {
                           destName = dest.DestinationName,
                           destID = dest.DId,
                       };
            test = test.Where(e => e.destName == RequestingPlace);

            DId = test.Select(e => e.destID).FirstOrDefault();

            var req = from d in dbContext.DisbursementRequests
                       join reqq in dbContext.Requesters on d.RequesterName.ToLower() equals reqq.ReqName.ToLower()
                       select new
                       {
                           rName = reqq.ReqName,
                           rID = reqq.ReqId,
                       };
            req = req.Where(e => e.rName == RequesterName);

            rId = req.Select(e => e.rID).FirstOrDefault();


            var item = dbContext.Items.SingleOrDefault(st => st.ItemCode == ItemCode);

            ItemId = item.ItemId;
            ItemName = item.ItemName;
            ViewData["ItemId"] = ItemId;
            ViewData["RequesterName"] = RequesterName;
            ViewData["RequestingPlace"] = RequestingPlace;
            ViewData["DId"] = DId;
            ViewData["rId"] = rId;



            Disbursement = query.ToList();
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

        public IActionResult OnPost([FromForm]  int DisbursementID,[FromForm] string RequesterName, 
                                    [FromForm] string Status, [FromForm] string Comments,
                                    [FromForm] int DId, [FromForm] bool InventoryBalanced, 
                                    [FromForm] DateTime RequestRecievedAt, [FromForm] int StoreId,
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
                    this.RequestingPlace = RequestingPlace;
                    this.RequestRecievedAt = RequestRecievedAt;
                    this.InventoryBalanced = InventoryBalanced;
                    this.DisbursementID = DisbursementID;
                    StatusList = (new[] { "NewRequest", "InPreparation", "Dispatched", "Dilivered" }).ToList().Select(x => new SelectListItem() { Text = x, Value = x }).ToList();
                    
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
                        
                        var disbIdParam = new SqlParameter("@PDISBURSE_ID", SqlDbType.Int) { Value = DisbursementID };
                        var itemIdParam = new SqlParameter("@PNEW_ITEMID", SqlDbType.Int) { Value = ItemId };
                        var storeIdParam = new SqlParameter("@PSTORE_ID", SqlDbType.Int) { Value = StoreId };
                        var quantityParam = new SqlParameter("@PNEW_QTY", SqlDbType.Int) { Value = Quantity };
                        var disbDate = new SqlParameter("@PDIS_DATE", SqlDbType.DateTime) { Value = RequestRecievedAt };
                        var disbStatus = new SqlParameter("@PDIS_STATUS", SqlDbType.VarChar) { Value = Status };
                        var disbComment = new SqlParameter("@PDIS_COMMENT", SqlDbType.VarChar) { Value = string.IsNullOrEmpty(Comments) ? "" : Comments };
                        var disbInvBalance = new SqlParameter("@PINV_BALANCE", SqlDbType.Bit) { Value = InventoryBalanced };
                        var disbReqName = new SqlParameter("@PREQ_NAME", SqlDbType.VarChar) { Value = RequesterName };
                        var disbReqPlace = new SqlParameter("@PREQ_PLACE", SqlDbType.VarChar) { Value = this.RequestingPlace };
                        var disbItemCode = new SqlParameter("@PNEW_ITEMCODE", SqlDbType.VarChar) { Value = ItemCode };
                        var disbItemTypeCode = new SqlParameter("@PITEM_TYPE_CODE", SqlDbType.VarChar) { Value = this.ItemTypeCode };

                        disbIdParam.Direction = ParameterDirection.Input;
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
                            .ExecuteSqlRaw("EXEC PRC_UPDATE_DISBURSEMENT @PDISBURSE_ID, @PNEW_ITEMID, @PSTORE_ID,@PNEW_QTY, @PDIS_DATE, @PDIS_STATUS, @PDIS_COMMENT, @PINV_BALANCE, @PREQ_NAME, @PREQ_PLACE, @PNEW_ITEMCODE, @PITEM_TYPE_CODE, @PCODE OUTPUT, @PDESC OUTPUT, @PMSG OUTPUT",
                                        disbIdParam,itemIdParam, storeIdParam, quantityParam, disbDate, disbStatus, disbComment, disbInvBalance, disbReqName, disbReqPlace, disbItemCode, disbItemTypeCode, codeParam, descParam, msgParam);


                        var code = codeParam.Value.ToString();
                        var msg = msgParam.Value.ToString();
                        var desc = descParam.Value.ToString();

                        task.LogInfo(MethodBase.GetCurrentMethod(), "Disbursement Updated");

                        string Message = string.Format("disbursement for {0} added", RequesterName);
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
            

            this.lblUpdateDisbursement = (Program.Translations["UpdateDisbursement"])[Lang];
            this.lblRequesterName = (Program.Translations["RequesterName"])[Lang];
            this.lblRequestReceivedDate = (Program.Translations["RequestReceivedDate"])[Lang];
            this.lblRequestingPlace = (Program.Translations["RequestingPlace"])[Lang];
            this.lblComments = (Program.Translations["Comments"])[Lang];
            this.lblDisbursementStatus = (Program.Translations["DisbursementStatus"])[Lang];
            this.lblInventoryBalanced = (Program.Translations["InventoryBalanced"])[Lang];

            this.lblUpdate = (Program.Translations["Update"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblEditDisbursement = (Program.Translations["EditDisbursement"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblItemTypeCode = (Program.Translations["ItemTypeCode"])[Lang]; 
            this.lblQuantity = (Program.Translations["Quantity"])[Lang];
            this.lblDisbursements = (Program.Translations["Disbursements"])[Lang];


        }
    }
}
