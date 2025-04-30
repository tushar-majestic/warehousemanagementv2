using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LabMaterials.Pages
{
    public class ChangePasswordModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string Username, Password, NewPassword, NewPasswordConfirm, EmpAffiliation,JobNumber, Transfer,UserFullName,Email;
        public int ToUpdateUserID;
        
        public string lblChangePassword, lblCurrentPassword, lblChangePasswords, lblNewPassword, lblConfirmNewPassword, lblUpdate, lblCancel, lblJobNumber, lblEmpAffiliation, lblTransfer, lblFullName,lblEmail,lblUserName;

        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
            FillLables();
            var dbContext = new LabDBContext();

            var user = dbContext.Users.Single(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            ToUpdateUserID = user.UserId;
            Username = user.UserName;
            Password = NewPasswordConfirm = NewPassword = "";
            JobNumber = user.JobNumber.ToString();
            UserFullName = user.FullName;
            Email = user.Email;
            EmpAffiliation = user.EmpAffiliation;
            Transfer = user.Transfer.ToString();

        }

        public IActionResult OnPost([FromForm] int ToUpdateUserID, [FromForm] string UserName, [FromForm] string NewPassword, [FromForm] string NewPasswordConfirm, [FromForm] string Password)
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
                    this.Password = Password;
                    this.NewPassword = NewPassword;
                    this.NewPasswordConfirm = NewPasswordConfirm;

                    if (string.IsNullOrEmpty(Password))
                        ErrorMsg = (Program.Translations["CurrentPasswordMissing"])[Lang];
                    else if (string.IsNullOrEmpty(NewPassword))
                        ErrorMsg = (Program.Translations["NewPasswordMissing"])[Lang];
                    else if (string.IsNullOrEmpty(NewPasswordConfirm))
                        ErrorMsg = (Program.Translations["NewPasswordConfirmMissing"])[Lang];
                    else if (NewPassword != NewPasswordConfirm)
                        ErrorMsg = (Program.Translations["PasswordNotMatched"])[Lang];
                    else if(!Lib.Hash.GenerateSHA(UTF8Encoding.UTF8.GetBytes(Password + UserName)).SequenceEqual(dbContext.Users.Single(u => u.UserId == ToUpdateUserID).Password))
                    {
                        ErrorMsg = (Program.Translations["InvalidCurrentPassword"])[Lang];
                    }
                    else
                    {
                        var user = dbContext.Users.Single(u => u.UserId == ToUpdateUserID);
                        user.Password = Lib.Hash.GenerateSHA(System.Text.UTF8Encoding.UTF8.GetBytes(NewPassword + UserName));

                        dbContext.SaveChanges();
                        task.LogInfo(MethodBase.GetCurrentMethod(), "User password updated");

                        string Message = string.Format("User {0} password updated", UserName);
                        Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Update",
                            Helper.ExtractIP(Request), dbContext, true);

                        return RedirectToPage("./Home");
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
            

            this.lblChangePassword = (Program.Translations["ChangePassword"])[Lang];
            this.lblCurrentPassword = (Program.Translations["CurrentPassword"])[Lang];
            this.lblNewPassword = (Program.Translations["NewPassword"])[Lang];
            this.lblConfirmNewPassword = (Program.Translations["ConfirmNewPassword"])[Lang];
            this.lblUpdate = (Program.Translations["Update"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang]; 
            this.lblChangePasswords = (Program.Translations["ChangePasswords"])[Lang];

            this.lblJobNumber = (Program.Translations["JobNumber"])[Lang];
            this.lblEmpAffiliation = (Program.Translations["EmpAffiliation"])[Lang];
            this.lblTransfer = (Program.Translations["Transfer"])[Lang];
            this.lblUserName = (Program.Translations["UserName"])[Lang];
            this.lblFullName = (Program.Translations["FullName"])[Lang];
            this.lblEmail = (Program.Translations["Email"])[Lang];

        }
    }
}
