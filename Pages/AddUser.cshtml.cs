using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Tls;

namespace LabMaterials.Pages
{
    public class AddUserModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string Username, UserFullName, Email, Password,EmpAffiliation,JobNumber, Transfer, ReTypePassword;
        public bool IsADUser;
        public int UserGroupID;
        public List<UserGroup> UserGroupsList {  get; set; }
        
        public string lblAddUser, lblUserName, lblFullName, lblEmail,
            lblUserEnabled, lblPassword, lblIsDomainUser, lblUserGroupName, lblAdd, lblCancel, lblUsers, lblJobNumber, lblEmpAffiliation, lblTransfer, lblReTypePassword;

        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageUsers == false)
                RedirectToPage("./Index?lang=" + Lang);
            FillLables();
            var dbContext = new LabDBContext();
            UserGroupsList = dbContext.UserGroups.ToList();
        }

        public IActionResult OnPost([FromForm] string UserName, [FromForm] string UserFullName, [FromForm] string Email, [FromForm] string Password, [FromForm] string ReTypePassword,[FromForm] string JobNumber,[FromForm] string EmpAffiliation, [FromForm] string Transfer,[FromForm] bool IsADUser, [FromForm] int UserGroupID)
        {
            LogableTask task = LogableTask.NewTask("AddUser");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageUsers)
                {
                    FillLables();
                    var dbContext = new LabDBContext();
                    Username = UserName;
                    this.UserFullName = UserFullName;
                    this.Email = Email;
                    this.UserGroupID = UserGroupID;
                    this.IsADUser = IsADUser;
                    this.Password = Password;
                    this.JobNumber = JobNumber;
                    this.EmpAffiliation = EmpAffiliation;
                    this.Transfer = Transfer;
                    this.ReTypePassword = ReTypePassword;
                    UserGroupsList = dbContext.UserGroups.ToList();

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
                    else if (!IsADUser && string.IsNullOrEmpty(Password))
                        ErrorMsg = (Program.Translations["PasswordMissing"])[Lang];
                    else if (!IsADUser && string.IsNullOrEmpty(ReTypePassword))
                        ErrorMsg = (Program.Translations["ReTypePasswordMissing"])[Lang];
                    else if (!IsADUser && (Password != ReTypePassword))
                        ErrorMsg = (Program.Translations["PasswordMismatch"])[Lang];
                    // new Changes ends
                   
                    
                   
                    else
                    {
                        if (dbContext.Users.Count(s => s.UserName == UserName.ToLower()) > 0)
                            ErrorMsg = string.Format((Program.Translations["UserNameExist"])[Lang], UserName);
                        else
                        {
                            var user = new User
                            {
                                UserName = UserName.ToLower(),
                                FullName = UserFullName,
                                Email = Email,
                                IsActive = true,
                                IsActiveDirectoryUser = IsADUser,
                                Locked = false,
                                FailedPasswordAttemptCount = 0,
                                UserGroupId = UserGroupID,
                                CreatedById = HttpContext.Session.GetInt32("UserId").Value,
                                CreatedDate = DateTime.Now,
                                Password = Lib.Hash.GenerateSHA(System.Text.UTF8Encoding.UTF8.GetBytes(Password + UserName.ToLower())),
                                //new changes
                                JobNumber = int.Parse(JobNumber),
                                EmpAffiliation = EmpAffiliation,
                                Transfer = int.Parse(Transfer),
                                //new changes ends



                            };
                            dbContext.Users.Add(user);
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "User added");

                            string Message = string.Format("User {0} added", UserName);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                                Helper.ExtractIP(Request), dbContext, true);

                            return RedirectToPage("./ManageUsers");
                        }
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
            

            this.lblAddUser = (Program.Translations["AddUser"])[Lang];
            this.lblUserName = (Program.Translations["UserName"])[Lang];
            this.lblFullName = (Program.Translations["FullName"])[Lang];
            this.lblEmail = (Program.Translations["Email"])[Lang];
            this.lblUserEnabled = (Program.Translations["UserEnabled"])[Lang];
            this.lblPassword = (Program.Translations["Password"])[Lang];
            this.lblIsDomainUser = (Program.Translations["IsDomainUser"])[Lang];
            this.lblUserGroupName = (Program.Translations["UserType"])[Lang];

            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblJobNumber = (Program.Translations["JobNumber"])[Lang];
            this.lblEmpAffiliation = (Program.Translations["EmpAffiliation"])[Lang];
            this.lblTransfer = (Program.Translations["Transfer"])[Lang];
            this.lblReTypePassword = (Program.Translations["ReTypePassword"])[Lang];


        }
    }
}
