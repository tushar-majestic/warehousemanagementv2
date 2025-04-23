using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LabMaterials.Pages
{
    public class EditUserModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string Username, UserFullName, Email, Password;
        public bool IsADUser, ChangePassword;
        public int UserGroupID, ToUpdateUserID;
        public List<UserGroup> UserGroupsList {  get; set; }
        
        public string lblUpdateUser, lblUserName, lblFullName, lblEmail,
            lblUserEnabled, lblPassword, lblChangePassword, lblIsDomainUser, lblUserGroupName, lblUpdate, lblCancel;

        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageUsers == false)
                RedirectToPage("./Index?lang=" + Lang);
            FillLables();
            var dbContext = new LabDBContext();

            var user = dbContext.Users.Single(u => u.UserId == HttpContext.Session.GetInt32("ToUpdateUserId"));
            this.ToUpdateUserID = user.UserId;
            Username = user.UserName;
            this.UserFullName = user.FullName;
            this.Email = user.Email;
            this.UserGroupID = user.UserGroupId;
            this.IsADUser = user.IsActiveDirectoryUser;
            this.Password = "************";
            UserGroupsList = dbContext.UserGroups.ToList();
        }

        public IActionResult OnPost([FromForm] int ToUpdateUserID, [FromForm] string UserName, [FromForm] string UserFullName, [FromForm] string Email, [FromForm] string Password, [FromForm] bool IsADUser, [FromForm] bool ChangePassword, [FromForm] int UserGroupID)
        {
            LogableTask task = LogableTask.NewTask("EditUser");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageUsers)
                {
                    FillLables();
                    var dbContext = new LabDBContext();
                    this.ToUpdateUserID = ToUpdateUserID;
                    Username = UserName;
                    this.UserFullName = UserFullName;
                    this.Email = Email;
                    this.UserGroupID = UserGroupID;
                    this.IsADUser = IsADUser;
                    this.ChangePassword = ChangePassword;
                    if(ChangePassword)
                        this.Password = Password;
                    UserGroupsList = dbContext.UserGroups.ToList();


                    if (string.IsNullOrEmpty(UserName))
                        ErrorMsg = (Program.Translations["UserNameMissing"])[Lang];
                    else if (string.IsNullOrEmpty(UserFullName))
                        ErrorMsg = (Program.Translations["UserFullNameMissing"])[Lang];
                    else if (string.IsNullOrEmpty(Email))
                        ErrorMsg = (Program.Translations["UserEmailMissing"])[Lang];
                    else if (UserGroupID == 0)
                        ErrorMsg = (Program.Translations["UserGroupMissing"])[Lang];
                    else
                    {
                        var user = dbContext.Users.Single(u => u.UserId == ToUpdateUserID);
                        user.Email = Email;
                        user.UserName = UserName;  
                        user.FullName = UserFullName;
                        user.UserGroupId = UserGroupID;
                        user.IsActiveDirectoryUser = IsADUser;
                        if(ChangePassword)
                            user.Password = Lib.Hash.GenerateSHA(System.Text.UTF8Encoding.UTF8.GetBytes(Password + UserName.ToLower()));

                        dbContext.SaveChanges();
                        task.LogInfo(MethodBase.GetCurrentMethod(), "User Updated");

                        string Message = string.Format("User {0} Updated", UserName);
                        Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                            Helper.ExtractIP(Request), dbContext, true);

                        return RedirectToPage("./ManageUsers");
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
            

            this.lblUpdateUser = (Program.Translations["UpdateUser"])[Lang];
            this.lblUserName = (Program.Translations["UserName"])[Lang];
            this.lblFullName = (Program.Translations["FullName"])[Lang];
            this.lblEmail = (Program.Translations["Email"])[Lang];
            this.lblUserEnabled = (Program.Translations["UserEnabled"])[Lang];
            this.lblChangePassword = (Program.Translations["ChangePassword"])[Lang];
            this.lblPassword = (Program.Translations["Password"])[Lang];
            this.lblIsDomainUser = (Program.Translations["IsDomainUser"])[Lang];
            this.lblUserGroupName = (Program.Translations["UserGroupName"])[Lang];

            this.lblUpdate = (Program.Translations["Update"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
        }
    }
}
