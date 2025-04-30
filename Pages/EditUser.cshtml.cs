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
        public string Username, UserFullName, Email, Password, EmpAffiliation,JobNumber, Transfer, ReTypePassword;
        public bool IsADUser, ChangePassword;
        public int UserGroupID, ToUpdateUserID;
        public int page { get; set; }
        public List<UserGroup> UserGroupsList {  get; set; }
        
        public string lblUpdateUser, lblUserName, lblFullName, lblEmail,
            lblUserEnabled, lblPassword, lblChangePassword, lblIsDomainUser, lblUserGroupName, lblUpdate, lblCancel, lblUsers, lblJobNumber, lblEmpAffiliation, lblTransfer, lblReTypePassword;

        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageUsers == false)
                RedirectToPage("./Index?lang=" + Lang);
            FillLables();
            this.page = (int)HttpContext.Session.GetInt32("page");
            var dbContext = new LabDBContext();

            var user = dbContext.Users.Single(u => u.UserId == HttpContext.Session.GetInt32("ToUpdateUserId"));
            this.ToUpdateUserID = user.UserId;
            Username = user.UserName;
            this.UserFullName = user.FullName;
            this.Email = user.Email;
            this.UserGroupID = user.UserGroupId;
            this.IsADUser = user.IsActiveDirectoryUser;
            this.Password = "************";
            this.JobNumber = user.JobNumber.ToString();
            this.EmpAffiliation = user.EmpAffiliation;
            this.Transfer = user.Transfer.ToString();
            UserGroupsList = dbContext.UserGroups.ToList();
        }

        public IActionResult OnPost([FromForm] int ToUpdateUserID, [FromForm] string UserName, [FromForm] string UserFullName, [FromForm] string Email, [FromForm] string Password, [FromForm] bool IsADUser, [FromForm] bool ChangePassword, [FromForm] int UserGroupID, string ReTypePassword,[FromForm] string JobNumber,[FromForm] string EmpAffiliation, [FromForm] string Transfer)
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
                    this.JobNumber = JobNumber;
                    this.EmpAffiliation = EmpAffiliation;
                    this.Transfer = Transfer;
                    if(ChangePassword){
                        this.Password = Password;
                        this.ReTypePassword = ReTypePassword;
                    }
                    UserGroupsList = dbContext.UserGroups.ToList();

                    // old code 
                    // if (string.IsNullOrEmpty(UserName))
                    //     ErrorMsg = (Program.Translations["UserNameMissing"])[Lang];
                    // else if (string.IsNullOrEmpty(UserFullName))
                    //     ErrorMsg = (Program.Translations["UserFullNameMissing"])[Lang];
                    // else if (string.IsNullOrEmpty(Email))
                    //     ErrorMsg = (Program.Translations["UserEmailMissing"])[Lang];
                    // else if (UserGroupID == 0)
                    //     ErrorMsg = (Program.Translations["UserGroupMissing"])[Lang];
                    //old code ends

                    // new Changes
                    if (UserGroupID == 0)
                        ErrorMsg = (Program.Translations["SelectUserGroup"])[Lang];
                    else if (string.IsNullOrEmpty(JobNumber))
                        ErrorMsg = (Program.Translations["JobNumberMissing"])[Lang];
                    else if (string.IsNullOrEmpty(UserName))
                        ErrorMsg = (Program.Translations["UserNameMissing"])[Lang];
                    else if (string.IsNullOrEmpty(UserFullName))
                        ErrorMsg = (Program.Translations["UserFullNameMissing"])[Lang];
                    else if (string.IsNullOrEmpty(EmpAffiliation))
                        ErrorMsg = (Program.Translations["EmpAffiliationMissing"])[Lang];
                    else if (string.IsNullOrEmpty(Transfer))
                        ErrorMsg = (Program.Translations["TransferMissing"])[Lang];
                    else if (string.IsNullOrEmpty(Email))
                        ErrorMsg = (Program.Translations["UserEmailMissing"])[Lang];
                    else if (ChangePassword && !IsADUser && string.IsNullOrEmpty(Password))
                        ErrorMsg = (Program.Translations["PasswordMissing"])[Lang];
                    else if (ChangePassword && !IsADUser && string.IsNullOrEmpty(ReTypePassword))
                        ErrorMsg = (Program.Translations["ReTypePasswordMissing"])[Lang];
                    else if (ChangePassword && !IsADUser && (Password != ReTypePassword))
                        ErrorMsg = (Program.Translations["PasswordMismatch"])[Lang];
                    // new Changes ends
                    else
                    {
                        var user = dbContext.Users.Single(u => u.UserId == ToUpdateUserID);
                        user.Email = Email;
                        user.UserName = UserName;  
                        user.FullName = UserFullName;
                        user.UserGroupId = UserGroupID;
                        user.IsActiveDirectoryUser = IsADUser;
                        //new changes
                        user.JobNumber = int.Parse(JobNumber);
                        user.EmpAffiliation = EmpAffiliation;
                        user.Transfer = int.Parse(Transfer);
                        //new changes ends
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
            this.lblUserGroupName = (Program.Translations["UserType"])[Lang];

            this.lblUpdate = (Program.Translations["Update"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblJobNumber = (Program.Translations["JobNumber"])[Lang];
            this.lblEmpAffiliation = (Program.Translations["EmpAffiliation"])[Lang];
            this.lblTransfer = (Program.Translations["Transfer"])[Lang];
            this.lblReTypePassword = (Program.Translations["ReTypePassword"])[Lang];

        }
    }
}
