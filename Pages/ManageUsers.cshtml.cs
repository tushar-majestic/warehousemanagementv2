using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class ManageUsersModel : BasePageModel
    {
        public List<UserInfo> Users { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }
        public void OnGet() 
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                FillLables();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
        //
        public string lblView, lblUsers, lblSearch, lblAddUser, lblManageUserGroups, lblUserName, lblFullName, lblEmail, 
            lblUserEnabled, lblIsLocked, lblUserType, lblUserGroupName, lblEdit, lblUnlock, lblTotalItem;

        public void OnPostSearch([FromForm] string UserName)
        {
            
            FillData(UserName);
        }

        public void OnPostEnable([FromForm] int UserId)
        {
            base.ExtractSessionData();
            if (HttpContext.Session.GetInt32("UserId").Value != UserId)
            {
                //
                var dbContext = new LabDBContext();
                string MessageText = Lang == "ar"? "تمكين" : "Enabled";
                var user = dbContext.Users.Single(s => s.UserId == UserId);
                if (user.IsActive)
                {
                    user.IsActive = false;
                    MessageText = Lang == "ar" ? "تعطيل" : "Disabled";
                }
                else
                    user.IsActive = true;

                dbContext.SaveChanges();
                FillData(null);
                Message = string.Format((Program.Translations["User"])[Lang] + " {0} " + MessageText, user.UserName);
                string MessageLog = string.Format((Program.Translations["User"])["en"] + " {0} " + MessageText, user.UserName);
                Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, MessageLog, MessageText, Helper.ExtractIP(Request), dbContext, true);
            }
            else
            {
                FillData(null);
                Message = string.Format((Program.Translations["UserSelfUpdateNotAllowed"])[Lang]);
            }
        }

        public void OnPostUnlock([FromForm] int UserId)
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                if (HttpContext.Session.GetInt32("UserId").Value != UserId)
                {
                    var dbContext = new LabDBContext();
                    var user = dbContext.Users.Single(s => s.UserId == UserId);
                    if (user.Locked)
                    {
                        user.Locked = false;
                        user.FailedPasswordAttemptCount = 0;
                        dbContext.SaveChanges();
                        Message = string.Format((Program.Translations["UserUnlocked"])[Lang], user.UserName);
                    }
                    else
                        Message = string.Format((Program.Translations["UserAlreadyUnlocked"])[Lang], user.UserName);


                    FillData(null);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Unlocked", Helper.ExtractIP(Request), dbContext, true);
                }
                else
                {
                    FillData(null);
                    Message = string.Format((Program.Translations["UserSelfUpdateNotAllowed"])[Lang]);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPostEdit([FromForm] int UserId)
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                if (HttpContext.Session.GetInt32("UserId").Value != UserId)
                {
                    HttpContext.Session.SetInt32("ToUpdateUserId", UserId);
                    return RedirectToPage("./EditUser");
                }
                else
                {
                    FillData(null);
                    Message = string.Format((Program.Translations["UserSelfUpdateNotAllowed"])[Lang]);
                    return Page();
                }
            }
            else
                return RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPostView([FromForm] int UserId)
        {
            HttpContext.Session.SetInt32("ToViewUserId", UserId);
            return RedirectToPage("./viewUser");
        }

        private void FillData(string? UserName)
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                FillLables();
                var dbContext = new LabDBContext();
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

                if (string.IsNullOrEmpty(UserName) == false)
                    query = query.Where(s => s.UserName.Contains(UserName));


                Users = query.ToList();

                TotalItems = Users.Count();
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
        }
    }
}
