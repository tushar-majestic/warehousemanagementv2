using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace LabMaterials.Pages
{
    public class IndexModel : PageModel
    {
        public string errorMessage { get; set; } = "";
        public string LoginTypeMsg { get; set; } = "Use application account to login";
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string dir { get; set; }
        public string Lang { get; set; }

        public string lblUserName, lblPassword, lblLogin, lblLabMaterials;

        public IActionResult OnGet()
        {
            try
            {
                HttpContext.Session.Clear();
                LoadPage();
            }
            catch (Exception ex)
            {
                this.errorMessage = "Error in application: " + ex.Message;
            }
            return Page();
        }

        private void LoadPage()
        {
            if (Request.Query["lang"] == "en")
            {
                Lang = "en";
                dir = "ltr";
            }
            else
            {
                dir = "rtl";
                Lang = "ar";
            }
            FillLables();
        }

        private void FillLables()
        {
            if (Program.Configuration.GetValue<bool>("UseLdap"))
                LoginTypeMsg = (Program.Translations["LdapLogin"])[Lang];
            else
                LoginTypeMsg = (Program.Translations["AppLogin"])[Lang];

            this.lblUserName = (Program.Translations["UserName"])[Lang];
            this.lblPassword = (Program.Translations["Password"])[Lang];
            this.lblLogin = (Program.Translations["Login"])[Lang];
            this.lblLabMaterials = (Program.Translations["LabMaterials"])[Lang];
        }

        public IActionResult OnPost(string UserName, string Password)
        {
            LogableTask task = LogableTask.NewTask("Login");
            var dbContext = new LabDBContext();

            try
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Called");
                LoadPage();
                if (UserName == null)
                    throw new Exception((Program.Translations["UserNameMissing"])[Lang]);

                if (Password == null)
                    throw new Exception((Program.Translations["PasswordMissing"])[Lang]);

                var dbUser = dbContext.Users.SingleOrDefault(u => u.UserName.ToLower() == UserName.ToLower());

                if (dbUser == null)
                    throw new Exception((Program.Translations["InvalidLgoin"])[Lang]);

                if (dbUser.Locked)
                    throw new Exception((Program.Translations["UserLocked"])[Lang]);

                if (!dbUser.IsActive)
                    throw new Exception((Program.Translations["UserDeactive"])[Lang]);

                string sourceIp = Helper.ExtractIP(Request);

                if (Program.Configuration.GetValue<bool>("UseLdap") && UserName != "admin")
                {
                    if (dbUser.IsActiveDirectoryUser)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "starting domain user validation");
                        PrincipalContext pc = new PrincipalContext(ContextType.Domain, Program.Configuration.GetValue<string>("Domain"));
                        bool isValid = pc.ValidateCredentials(UserName, Password, ContextOptions.Negotiate);
                        pc.Dispose();

                        if (isValid == false)
                        {
                            dbUser.FailedPasswordAttemptCount++;

                            if (dbUser != null && dbUser.FailedPasswordAttemptCount >= Program.Configuration.GetValue<int>("MaxWrongPassAttempts"))
                                dbUser.Locked = true;

                            dbContext.SaveChanges();

                            Helper.AddActivityLog(dbUser.UserId, "LDAP Login failed", "Login", sourceIp, dbContext, false);
                            this.errorMessage = "User LoginName or password is not valid.";
                        }
                        else
                        {
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "valid user, logging-in");
                            SetSessionVars(dbContext, dbUser, sourceIp);

                            return RedirectToPage("./Home");
                        }
                    }
                    else
                        this.errorMessage = (Program.Translations["InvalidLgoin"])[Lang];
                }
                else
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "starting application user validation");

                    if (Program.Configuration.GetValue<bool>("UseLdap") && UserName.ToLower() == "admin" && Program.Configuration.GetValue<bool>("AllowAdminUserWithLdap") == false)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Login failed. Admin user not allowed with LDAP authentication");
                        Helper.AddActivityLog(dbUser.UserId, "Login failed. Admin user not allowed with LDAP authentication", "Login", sourceIp, dbContext, false);
                        errorMessage = (Program.Translations["InvalidLgoin"])[Lang];
                    }
                    else
                    {

                        var hash = Lib.Hash.GenerateSHA(UTF8Encoding.UTF8.GetBytes(Password + UserName.ToLower()));

                        if (hash.SequenceEqual(dbUser.Password))
                        //if (true)
                        {
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "valid user, logging-in");
                            SetSessionVars(dbContext, dbUser, sourceIp);

                            return RedirectToPage("./Home");
                        }
                        else
                        {
                            Helper.AddActivityLog(dbUser.UserId, "Login failed", "Login", sourceIp, dbContext, false);
                            this.errorMessage = (Program.Translations["InvalidLgoin"])[Lang];
                            dbUser.FailedPasswordAttemptCount++;
                            if (dbUser != null && dbUser.FailedPasswordAttemptCount >= Program.Configuration.GetValue<int>("MaxWrongPassAttempts"))
                                dbUser.Locked = true;

                            dbContext.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                errorMessage = ex.Message;

            }
            finally
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Completed");
                task.EndTask();
            }
            FillLables();
            return Page();
        }

        private void SetSessionVars(LabDBContext dbContext, User dbUser, string sourceIp)
        {
            Helper.AddActivityLog(dbUser.UserId, "User Logged in successfully", "Login", sourceIp, dbContext, true);


            List<string> Privileges = (from p in dbContext.Privileges
                                       join ugp in dbContext.UserGroupPrivileges on p.PrivilegeId equals ugp.PrivilegeId
                                       where ugp.UserGroupId == dbUser.UserGroupId
                                       select p.PrivilegeName).ToList();



            HttpContext.Session.SetInt32("UserId", dbUser.UserId);
            HttpContext.Session.SetString("UserName", dbUser.UserName);
            HttpContext.Session.SetString("FullName", dbUser.FullName);
            HttpContext.Session.SetString("UserGroup", dbUser.UserGroup.UserGroupName);
            HttpContext.Session.SetInt32("IsLDAP", dbUser.IsActiveDirectoryUser ? 1 : 0);
            HttpContext.Session.SetInt32("CanManageStore", Privileges.Contains("CanManageStore") ? 1 : 0);
            HttpContext.Session.SetInt32("CanManageUsers", Privileges.Contains("CanManageUsers") ? 1 : 0);
            HttpContext.Session.SetInt32("CanManageItems", Privileges.Contains("CanManageItems") ? 1 : 0);
            HttpContext.Session.SetInt32("CanManageSupplies", Privileges.Contains("CanReceiveSupplies") ? 1 : 0);
            HttpContext.Session.SetInt32("CanDisburseItems", Privileges.Contains("CanDisburseItems") ? 1 : 0);
            HttpContext.Session.SetInt32("CanSeeReports", Privileges.Contains("CanSeeReports") ? 1 : 0);
            HttpContext.Session.SetInt32("CanManageItemGroup", Privileges.Contains("CanManageItemGroup") ? 1 : 0);
            HttpContext.Session.SetInt32("CanManageItemCard", Privileges.Contains("CanManageItemCard") ? 1 : 0);
            HttpContext.Session.SetInt32("CanManageRequests", Privileges.Contains("CanManageRequests") ? 1 : 0);
            HttpContext.Session.SetInt32("CanGenerateReceivingRequest", Privileges.Contains("CanGenerateReceivingRequest") ? 1 : 0);
            HttpContext.Session.SetInt32("CanGenerateDispensingRequest", Privileges.Contains("CanGenerateDispensingRequest") ? 1 : 0);
            HttpContext.Session.SetInt32("CanReturnItems", Privileges.Contains("CanReturnItems") ? 1 : 0);
            HttpContext.Session.SetString("LastLogin", dbUser.LastLoginTime.HasValue ? dbUser.LastLoginTime.Value.ToString() : "");
            if (string.IsNullOrEmpty(dbUser.Lang))
            {
                HttpContext.Session.SetString("Lang", "ar");
                // update db
                string sessionLang = HttpContext.Session.GetString("Lang");

                if (!string.IsNullOrEmpty(sessionLang) && dbUser != null)
                {
                    dbUser.Lang = sessionLang;
                    dbContext.SaveChanges();
                }


            }
            else
            {
                HttpContext.Session.SetString("Lang", dbUser.Lang);
            }

            //string lang = Request.Query["lang"];
            //if (string.IsNullOrEmpty(lang))
            //    HttpContext.Session.SetString("Lang", "ar");
            //else
            //    HttpContext.Session.SetString("Lang", lang);


            if (dbUser.FailedPasswordAttemptCount > 0)
            {
                dbUser.FailedPasswordAttemptCount = 0;
                dbContext.SaveChanges();
            }
        }
    }
}
