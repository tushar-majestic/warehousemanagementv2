using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class ManageUserGroupsModel : BasePageModel
    {
        public List<UserGroupInfo> UserGroups { get; set; }
        public List<UserGroupInfo> UserGroupsAll { get; set; }
        public int TotalItems { get; set; }
        public string Message { get; set; }
        [BindProperty]
        public string UserGroupName { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        
        public string lblUserGroup, lblSearch, lblAddUserGroup, lblUserGroupName, lblPrivilages,  lblEdit, lblDelete, lblTotalItem, lblUsers;
        public void OnGet(string? UserGroupName, int page = 1)
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                FillLables();
                if (HttpContext.Request.Query.ContainsKey("page")){
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.UserGroupName = UserGroupName;
                    FillData(UserGroupName, page);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillData(string UserGroupName, int page = 1)
        {    if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }
            base.ExtractSessionData();
            if (this.CanManageUsers)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = (from g in dbContext.UserGroups
                             select new UserGroupInfo
                             {
                                 UserGroupID = g.UserGroupId,
                                 UserGroupName = g.UserGroupName
                             }); ;
                

                if (string.IsNullOrEmpty(UserGroupName) == false)
                    query = query.Where(i => i.UserGroupName.Contains(UserGroupName));

                // UserGroups = query.ToList();
                // TotalItems = UserGroups.Count();
                TotalItems = query.Count();
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
                var list = query.ToList();
                UserGroups = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();    
                UserGroupsAll = query.ToList();    
                CurrentPage = page; 
                foreach(var UG in UserGroups)
                {
                    UG.Privilages = String.Join(", ", (from p in dbContext.Privileges
                                                       join ugp in dbContext.UserGroupPrivileges on p.PrivilegeId equals ugp.PrivilegeId
                                                       where ugp.UserGroupId == UG.UserGroupID
                                                       select p.PrivilegeName).ToList());
                }

            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPostEdit([FromForm] int UserGroupID, [FromForm] int page, [FromForm] string UserGroupName)
        {
            HttpContext.Session.SetInt32("UserGroupID", UserGroupID);
            HttpContext.Session.SetInt32("page", page);
            HttpContext.Session.SetString("UserGroupName", string.IsNullOrEmpty(UserGroupName) ? "" : UserGroupName);
            return RedirectToPage("./EditUserGroup");
        }

        public void OnPostDelete([FromForm] int UserGroupID)
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                var dbContext = new LabDBContext();

                if (dbContext.Users.Count(s => s.UserGroupId == UserGroupID) == 0)
                {
                    var userGroup = dbContext.UserGroups.Single(s => s.UserGroupId == UserGroupID);
                    var userGroupPrivilages = dbContext.UserGroupPrivileges.Where(i => i.UserGroupId == UserGroupID).ToList();
                    dbContext.UserGroupPrivileges.RemoveRange(userGroupPrivilages);
                    dbContext.UserGroups.Remove(userGroup);
                    dbContext.SaveChanges();
                    FillData(null);
                    Message = string.Format((Program.Translations["UserGroupDeleted"])[Lang], userGroup.UserGroupName);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
                }
                else
                {
                    Message = (Program.Translations["UserGroupNotDeleted"])[Lang];
                    FillData(null);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }


        public void OnPostSearch([FromForm] string UserGroupName)
        {   CurrentPage = 1;
            this.UserGroupName = UserGroupName;
            FillData(UserGroupName, CurrentPage);
        }

        private void FillLables()
        {
            

            this.lblUserGroup = (Program.Translations["UserGroups"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblAddUserGroup = (Program.Translations["AddUserGroup"])[Lang];
            this.lblUserGroupName = (Program.Translations["UserGroupName"])[Lang];
            this.lblPrivilages = (Program.Translations["Privilages"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblUsers = (Program.Translations["Users"])[Lang];
        }

    }
}
