using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;


namespace LabMaterials.Pages
{
    public class viewUserModel : BasePageModel
    {
        public UserInfo singleUser { get; set; }
        public string lblView, lblUsers, lblSearch, lblAddUser, lblManageUserGroups, lblUserName, lblFullName, lblEmail,
            lblUserEnabled, lblIsLocked, lblUserType, lblUserGroupName, lblEdit, lblUnlock, lblTotalItem;
        public int page { get; set; }

        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            this.page = (int)HttpContext.Session.GetInt32("page");
            if (CanManageUsers)
            {
                var dbContext = new LabDBContext();
                var user = dbContext.Users.Single(u => u.UserId == HttpContext.Session.GetInt32("ToUpdateUserId"));
                var query = from u in dbContext.Users
                            join g in dbContext.UserGroups on u.UserGroupId equals g.UserGroupId
                            select new UserInfo
                            {
                                UserID = u.UserId,
                                UserName = u.UserName,
                                FullName = u.FullName,
                                Email = u.Email,
                                IsActive = u.IsActive ? (Lang == "ar" ? "تمكين" : "Enabled") : (Lang == "ar" ? "تعطيل" : "Disabled"),
                                EnableBtnText = u.IsActive ? (Lang == "ar" ? "تعطيل" : "Disable") : (Lang == "ar" ? "تمكين" : "Enable"),
                                IsADUser = u.IsActiveDirectoryUser ? (Lang == "ar" ? "مستخدم المجال" : "Domain User") : (Lang == "ar" ? "مستخدم التطبيق" : "Application User"),
                                IsLocked = u.Locked ? (Lang == "ar" ? "نعم" : "Yes") : (Lang == "ar" ? "لا" : "No"),
                                GroupName = g.UserGroupName
                            };

                // UserInfo singleUser = null;


                singleUser = query.FirstOrDefault(s => s.UserID == user.UserId);

            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillLables()
        {
            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblAddUser = (Program.Translations["AddUser"])[Lang];
            this.lblManageUserGroups = (Program.Translations["ManageUserGroups"])[Lang];
            this.lblUserName = (Program.Translations["UserName"])[Lang];
            this.lblFullName = (Program.Translations["FullName"])[Lang];
            this.lblEmail = (Program.Translations["Email"])[Lang];
            this.lblUserEnabled = (Program.Translations["UserEnabled"])[Lang];
            this.lblIsLocked = (Program.Translations["IsLocked"])[Lang];
            this.lblUserType = (Program.Translations["UserType"])[Lang];
            this.lblUserGroupName = (Program.Translations["UserGroupName"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblView = (Program.Translations["View"])[Lang];
            this.lblUnlock = (Program.Translations["Unlock"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblView = (Program.Translations["View"])[Lang];
        }
    }
    }