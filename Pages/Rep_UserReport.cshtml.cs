using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class Rep_UserReportModel : BasePageModel
    {
        public DateTime? FromDate, ToDate;
        public List<UsersInfo> UserInfo;
        public string lblTotalUsers,lblUsers, lblFromDate, lblToDate, lblMaterialsReceived, lblInventory, 
            lblHazardousMaterials, lblUserActivity, lblDistributedMaterials, lblHazardTypeName, 
            lblItemCode, lblItemName, lblStoreName, lblGroupName, lblAvailableQuantity, lblHazardType, 
            lblTypeName, lblUnitCode, lblSearch, lblSubmit, lblTotalItem, lblDamagedItems, lblUserReport,
            lblUserId, lblUserName, lblUserGroup, lblCreatedBy, lblCreationDate, lblExport;
        public int TotalUsers { get; set; }

        [BindProperty]
        public string UserName { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public void OnGet(string? UserName, DateTime? FromDate, DateTime? ToDate, int page = 1)
        {
            base.ExtractSessionData();
            if (CanSeeReports)
            {
                // this.FromDate = DateTime.Today;
                // this.ToDate = DateTime.Today;
                FillLables();
                if (HttpContext.Request.Query.ContainsKey("page")){
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.UserName = UserName;
                    this.FromDate = FromDate;
                    this.ToDate = ToDate;
                    FillData(UserName, FromDate, ToDate, page);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public void OnPost([FromForm]string UserName, [FromForm] DateTime? FromDate, [FromForm] DateTime? ToDate)
        {
            base.ExtractSessionData();
            this.UserName = UserName;
            CurrentPage = 1; 
            this.FromDate = FromDate;
            this.ToDate = ToDate;
            if (CanSeeReports)
            {
                FillData(UserName, FromDate, ToDate, CurrentPage);
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
        // function before pagination 
        /*public void FillData(string UserName, DateTime? FromDate, DateTime? ToDate)
        {
            var dbContext = new LabDBContext();
            var query = (from u in dbContext.Users
                         join ug in dbContext.UserGroups on u.UserGroupId equals ug.UserGroupId
                         select new UsersInfo
                         {
                             UserID = u.UserId,
                             UserName = u.UserName,
                             CreatedBy = u.CreatedById,
                             CreationDate = u.CreatedDate,
                             UserGroup = ug.UserGroupName

                         });
            query = query.OrderBy(u => u.UserID);

            
            if (UserName is not null)
            {
                query = query.Where(e => e.UserName.Contains(UserName));
            }

            if (FromDate is not null && FromDate != DateTime.MinValue && ToDate is not null && ToDate != DateTime.MinValue)
            {
                query = query.Where(e => e.CreationDate.Date >= FromDate && e.CreationDate.Date <= ToDate);
            }
            UserInfo = query.ToList();
            TotalUsers = UserInfo.Count();

            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
            base.ExtractSessionData();
            FillLables();
        }*/

        public void FillData(string UserName, DateTime? FromDate, DateTime? ToDate, int page = 1)
        {   if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }
            var dbContext = new LabDBContext();
            var query = (from u in dbContext.Users
                         join ug in dbContext.UserGroups on u.UserGroupId equals ug.UserGroupId
                         select new UsersInfo
                         {
                             UserID = u.UserId,
                             UserName = u.UserName,
                             CreatedBy = u.CreatedById,
                             CreationDate = u.CreatedDate,
                             UserGroup = ug.UserGroupName

                         });
            query = query.OrderBy(u => u.UserID);

            
            if (UserName is not null)
            {
                query = query.Where(e => e.UserName.Contains(UserName));
            }

            if (FromDate is not null && FromDate != DateTime.MinValue && ToDate is not null && ToDate != DateTime.MinValue)
            {
                query = query.Where(e => e.CreationDate.Date >= FromDate && e.CreationDate.Date <= ToDate);
            }
            UserInfo = query.ToList();

            TotalUsers = query.Count();
            TotalPages = (int)Math.Ceiling((double)TotalUsers / ItemsPerPage);

            var list = query.ToList();
            UserInfo = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();     
            CurrentPage = page;

            // FromDate = DateTime.Now;
            // ToDate = DateTime.Now;
            base.ExtractSessionData();
            FillLables();
        }
        private void FillLables()
        {

            this.lblMaterialsReceived = (Program.Translations["MaterialsReceived"])[Lang];
            this.lblInventory = (Program.Translations["Inventory"])[Lang];
            this.lblHazardousMaterials = (Program.Translations["HazardousMaterials"])[Lang];
            this.lblUserActivity = (Program.Translations["UserActivity"])[Lang];
            this.lblDistributedMaterials = (Program.Translations["DistributedMaterials"])[Lang];
            this.lblHazardTypeName = (Program.Translations["HazardTypeName"])[Lang];
            this.lblItemCode = (Program.Translations["ItemCode"])[Lang];
            this.lblItemName = (Program.Translations["ItemName"])[Lang];
            this.lblStoreName = (Program.Translations["StoreName"])[Lang];
            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblAvailableQuantity = (Program.Translations["AvailableQuantity"])[Lang];
            this.lblHazardType = (Program.Translations["HazardType"])[Lang];
            this.lblTypeName = (Program.Translations["TypeName"])[Lang];
            this.lblUnitCode = (Program.Translations["UnitCode"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblSubmit = (Program.Translations["Submit"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblDamagedItems = (Program.Translations["DamagedItems"])[Lang];
            this.lblUserReport = (Program.Translations["UserReport"])[Lang];
            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblFromDate = (Program.Translations["FromDate"])[Lang];
            this.lblToDate = (Program.Translations["ToDate"])[Lang];
            this.lblTotalUsers = (Program.Translations["TotalUsers"])[Lang];
            this.lblUserId = (Program.Translations["UserId"])[Lang];
            this.lblUserName = (Program.Translations["UserName"])[Lang];
            this.lblUserGroup = (Program.Translations["UserGroups"])[Lang];
            this.lblCreatedBy = (Program.Translations["CreatedBy"])[Lang];
            this.lblCreationDate = (Program.Translations["CreationDate"])[Lang];
            this.lblExport = (Program.Translations["Export"])[Lang];

        }
    }
}
