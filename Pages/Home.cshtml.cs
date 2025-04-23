using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LabMaterials.Pages
{
    public class HomeModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public List<DestinationsInfo> MostRequestingDestination { get; set; }
        public List<ItemInfo> FastMovingItem { get; set; }
        public List<ItemInfo> LowInventoryItem { get; set; }
        public int countItems, countUsers, countStores, countSupplies, countdisbursement,
            totalQuantity, MostRequestingDestCount;
        int? totalDisburse;
        //public string lblHome, lblDisbursement, lblReports, lblManageUsers, lblManageItems, lblManageSupplies, lblManageStores;
        //public void OnGet(DateTime? startDate, DateTime? endDate)

        public string lblHome, lblDisbursement, lblReports, lblManageUsers, lblManageItems,
            lblManageSupplies, lblManageStores, lblDamagedItems, Data1Values,
            Data1Colors, Data2Values, Data2Colors, LabelValues, LabelValues1,
            itemName, itemPercentage, lblLowInventoryItem, lblFastMovingItem,
            lblMostRequestingDestination, LineChartTitle, LineChartLabels, LineChartData,
            lblDestinationName, countDestination, lblCountDestination, lblItemName, lblCount,
            SuppliesData, DisbursementData, lblFromDate, lblToDate, lblTotalItems, lblTotalUsers, lblTotalStores,
            lblDisbursements, lblSupplies, lblSuppliesAndDisbursements, lblItems;

        public DateTime? FromDate = DateTime.Now;
        public DateTime? ToDate = DateTime.Now;
        public void OnGet(DateTime? startDate, DateTime? endDate)
        {
            LoadPage();
            if (!HttpContext.Session.Keys.Contains("UserId"))
            {

                RedirectToPage("./Index?lang=" + Lang);
            }
            else
            {
                var dbContext = new LabDBContext();
                totalQuantity = dbContext.Items.Sum(e => e.AvailableQuantity);
                totalDisburse = dbContext.DisbursementRequests.Sum(e => e.ItemQuantity);

                countItems = dbContext.Items.Where(e => e.Ended == null).Count();
                countUsers = dbContext.Users.Where(e => e.Ended == null).Count();
                countStores = dbContext.Stores.Where(e => e.Ended == null).Count();
                startDate = startDate is null ? DateTime.Now : startDate;
                endDate = endDate is null ? DateTime.Now : endDate;
                if (startDate > endDate)
                {
                    ErrorMsg = string.Format((Program.Translations["StartDateGreaterThanEndDate"])[Lang]);
                }
                countSupplies = dbContext.Supplies.Where(e => e.ReceivedAt >= startDate && e.ReceivedAt <= endDate).Count();
                countdisbursement = dbContext.DisbursementRequests.Where(e => e.ReqReceivedAt >= startDate && e.ReqReceivedAt <= endDate).Count();

                //var itemsList = dbContext.Items.Select(item => new
                //{
                //    Name = item.ItemName,
                //    Percentage = (double)item.AvailableQuantity / totalQuantity * 100
                //}).OrderByDescending(item => item.Percentage);

                var itemsList = (from i in dbContext.Items
                                 where i.Ended == null
                                 join cc in dbContext.ColorCodes on i.ItemId equals cc.ItemId
                                select new
                                {
                                    Name = i.ItemName,
                                    Percentage = (double)i.AvailableQuantity / totalQuantity * 100,
                                    COLOR_CODE = cc.ColorCode1,
                                })
                        .GroupBy(item => item.Name)
                        .Select(group => new
                        {
                            ItemCode = group.Key,
                            Name = group.First().Name,
                            COLOR_CODE = group.First().COLOR_CODE,
                            Percentage = group.Sum(item => item.Percentage),
                            Count = group.Count() // Count of occurrences for each item code
                        })
                        .OrderByDescending(item => item.Percentage);

                string nameList = string.Join(", ", itemsList.Select(item => item.Name));
                string percentageList = string.Join(", ", itemsList.Select(item => item.Percentage));
                string cc1 = string.Join(", ", itemsList.Select(item => item.COLOR_CODE));


                //var disbursementList = (from i in dbContext.Items
                //            join di in dbContext.DisbursementRequests on i.ItemCode equals di.Itemcode
                //            select new
                //            {
                //                DISBURSE_Name = i.ItemName,
                //                DISBURSE_Percentage = (double)di.ItemQuantity / totalDisburse * 100,
                //                ItemCode = i.ItemCode,
                //            }).OrderByDescending(item => item.DISBURSE_Percentage);

                var disbursementList = (from i in dbContext.Items
                                        join di in dbContext.DisbursementRequests on i.ItemCode equals di.Itemcode
                                        join cc in dbContext.ColorCodes on i.ItemId equals cc.ItemId
                                        select new
                                        {
                                            ItemCode = i.ItemCode,
                                            DISBURSE_Name = i.ItemName,
                                            COLOR_CODE = cc.ColorCode1,
                                            DISBURSE_Percentage = (double)di.ItemQuantity / totalDisburse * 100,
                                        })
                        .GroupBy(item => item.ItemCode)
                        .Select(group => new
                        {
                            ItemCode = group.Key,
                            DISBURSE_Name = group.First().DISBURSE_Name,
                            COLOR_CODE = group.First().COLOR_CODE,
                            DISBURSE_Percentage = group.Sum(item => item.DISBURSE_Percentage),
                            Count = group.Count() // Count of occurrences for each item code
                        })
                        .OrderByDescending(item => item.DISBURSE_Percentage);

                //var disbursementList = dbContext.DisbursementRequests.Select(disburseItem => new
                //{
                //    DISBURSE_Name = disburseItem.Itemcode,
                //    DISBURSE_Percentage = (double)disburseItem.ItemQuantity / totalDisburse * 100
                //}).OrderByDescending(item => item.DISBURSE_Percentage);

                string DisburseList = string.Join(", ", disbursementList.Select(disburseItem => disburseItem.DISBURSE_Name));
                string DisbursePercentageList = string.Join(", ", disbursementList.Select(disburseItem => disburseItem.DISBURSE_Percentage));
                string ColorLists = string.Join(", ", disbursementList.Select(disburseItem => disburseItem.COLOR_CODE));

                LabelValues = DisburseList;
                LabelValues1 = nameList;
                Data1Values = DisbursePercentageList;
                Data2Values = percentageList;
                Data1Colors = ColorLists;
                Data2Colors = cc1;

                /*
                var dest = (from dr in dbContext.DisbursementRequests
                           join de in dbContext.Destinations on dr.RequestingPlace.ToLower() 
                                        equals de.DestinationName.ToLower()
                            select new DestinationsInfo
                           {
                               DestinationName = de.DestinationName,
                               DestinationId = de.DId,
                               // Count = 
                           });
                */

                var destWithCount = (from dr in dbContext.DisbursementRequests
                                     group dr by dr.RequestingPlace.ToLower() into g
                                     orderby g.Count() descending
                                     select new DestinationsInfo
                                     {
                                         DestinationName = g.Key,
                                         Count = g.Count()
                                     }).Take(1);

                MostRequestingDestination = destWithCount.ToList();

                var fastMovingItem = (from dr in dbContext.DisbursementRequests
                                      join i in dbContext.Items on dr.Itemcode equals i.ItemCode
                                     group i by i.ItemName into g
                                      orderby g.Count() descending
                                      select new ItemInfo
                                     {
                                         ItemCode = g.Key,
                                         Count = g.Count()
                                     }).Take(1);
                FastMovingItem = fastMovingItem.ToList();

                var lowInventory = (from i in dbContext.Items
                                   group i by i.ItemName into g
                                   select new ItemInfo
                                   {
                                       ItemName = g.Key,
                                       Count = g.Sum(item => item.AvailableQuantity),
                                   });
                var res = lowInventory.Where(e => e.Count <= 10);

                LowInventoryItem = res.ToList();

                var disbDates = new SqlParameter("@PDISBURSE_DATES", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                var suppDates = new SqlParameter("@PSUPPLY_DATES", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                var disbCount = new SqlParameter("@PDISBURSE_COUNT", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                var suppCount = new SqlParameter("@PSUPPLY_COUNT", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                var code = new SqlParameter("@PCODE", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };
                var desc = new SqlParameter("@PMSG", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Output };
                var msg = new SqlParameter("@PDESC", SqlDbType.VarChar, 2) { Direction = ParameterDirection.Output };

                var sss = dbContext.Database.ExecuteSqlRaw("PRC_GET_DATE_WISE_DATA @PDISBURSE_DATES  OUT, @PSUPPLY_DATES OUT," +
                                                "@PDISBURSE_COUNT OUT, @PSUPPLY_COUNT OUT, @PCODE OUT," +
                                                "@PDESC OUT, @PMSG  OUT",disbDates, suppDates, disbCount, suppCount,
                                                code,desc,msg );

                var disburseDates = disbDates.Value.ToString();
                var supplyDates = suppDates.Value.ToString();
                var disburseCount = disbCount.Value.ToString();
                var supplyCount = suppCount.Value.ToString();



                var lineTitle = "Monthly Sales";
                var lineLabels = disburseDates;
                var SuppliesD = supplyCount;
                var DisbursementD = disburseCount;
                LineChartTitle = lineTitle;
                LineChartLabels = lineLabels;
                SuppliesData = SuppliesD;
                DisbursementData = DisbursementD;

                FromDate = startDate;
                ToDate = endDate;
                ExtractSessionData();
                FillLables();
            }
        }
        private void FillLables()
        {


            this.lblDisbursement = (Program.Translations["Disbursements"])[Lang];
            this.lblReports = (Program.Translations["Reports"])[Lang];
            this.lblManageItems = (Program.Translations["ManageItems"])[Lang];
            this.lblManageStores = (Program.Translations["ManageStore"])[Lang];
            this.lblManageSupplies = (Program.Translations["Supplies"])[Lang];
            this.lblManageUsers = (Program.Translations["ManageUsers"])[Lang];
            this.lblHome = (Program.Translations["Home"])[Lang];
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblLowInventoryItem = (Program.Translations["LowInventoryItem"])[Lang];
            this.lblFastMovingItem = (Program.Translations["FastMovingItem"])[Lang];
            this.lblMostRequestingDestination = (Program.Translations["MostRequestingDestination"])[Lang];
            this.lblDestinationName = (Program.Translations["DestinationName"])[Lang];
            this.lblCountDestination = (Program.Translations["CountDestination"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblCount = (Program.Translations["Count"])[Lang];
            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
            this.lblTotalItems = (Program.Translations["TotalItems"])[Lang];
            this.lblTotalUsers = (Program.Translations["TotalUsers"])[Lang];
            this.lblTotalStores = (Program.Translations["TotalStores"])[Lang];
            this.lblDisbursements = (Program.Translations["Disbursements"])[Lang];
            this.lblSupplies = (Program.Translations["Supplies"])[Lang];
            this.lblSuppliesAndDisbursements = (Program.Translations["SuppliesAndDisbursements"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];

        }

        private void LoadPage()
        {
            dir = HttpContext.Session.GetString("Lang") == "en" ? "ltr" : "rtl";
            Lang = HttpContext.Session.GetString("Lang") == "en" ? "en" : "ar";
            if (Request.Query["lang"] == "en")
            {
                Lang = "en";
                dir = "ltr";
                HttpContext.Session.SetString("Lang", "en");
            }
            else
            {
                dir = "rtl";
                Lang = "ar";
                HttpContext.Session.SetString("Lang", "ar");
            }
            FillLables();
        }
    }
}
