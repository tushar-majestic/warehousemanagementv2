global using Lib;
global using LabMaterials.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Text;
using LabMaterials.AppCode;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Claims;


namespace LabMaterials.Pages
{
    public class LoginModel : PageModel
    {
        public string errorMessage { get; set; }
        public string LoginTypeMsg { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public IActionResult OnGet()
        {
            try
            {
                if (Program.Configuration.GetValue<bool>("UseLdap"))
                    LoginTypeMsg = "Use your domain account to login";
                else
                    LoginTypeMsg = "Use application account to login";
            }
            catch (Exception ex)
            {
                this.errorMessage = "Error in application: " + ex.Message;
            }
            return Page();
        }

        public IActionResult OnPost(string UserName, string Password)
        {
            LogableTask task = LogableTask.NewTask("Login");
            User dbUser = null;
            var dbContext = new LabDBContext();

            try
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Called");

                if (UserName == null)
                    throw new Exception("User Name is required");

                if (Password == null)
                    throw new Exception("Password is required");


                dbUser = dbContext.Users.SingleOrDefault(u => u.UserName.ToLower() == UserName.ToLower());

                if (dbUser == null)
                    throw new Exception("User LoginName or password is not valid.");

                if (dbUser.Locked)
                    throw new Exception("User is locked, contact administrator");

                if (!dbUser.IsActive)
                    throw new Exception("User is deactivated, contact administrator");

                dbUser.FailedPasswordAttemptCount++;
                string sourceIp = Helper.ExtractIP(Request);

                if (Program.Configuration.GetValue<bool>("UseLdap"))
                    LoginTypeMsg = "Use your domain account to login";
                else
                    LoginTypeMsg = "Use application account to login";


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

                            Helper.AddActivityLog(dbUser.UserId, "LDAP Login failed", "Login", sourceIp, dbContext, false);
                            this.errorMessage = "User LoginName or password is not valid.";
                        }
                        else
                        {
                            Helper.AddActivityLog(dbUser.UserId, "User Logged in successfully", "Login", sourceIp, dbContext, false);
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "valid user, logging-in");
                            
                            HttpContext.Session.SetInt32("UserId", dbUser.UserId);
                            HttpContext.Session.SetString("UserName", dbUser.UserName);
                            HttpContext.Session.SetString("FullName", dbUser.FullName);
                            HttpContext.Session.SetString("LastLogin", dbUser.LastLoginTime.HasValue ? dbUser.LastLoginTime.ToString() : "");
                            HttpContext.Session.SetInt32("CanManageStore", dbUser.CanManageStore ? 1 : 0);
                            HttpContext.Session.SetInt32("CanManageUsers", dbUser.CanManageUsers ? 1 : 0);
                            HttpContext.Session.SetInt32("CanManageItems", dbUser.CanManageItems ? 1 : 0);
                            HttpContext.Session.SetInt32("CanReceiveSupplies", dbUser.CanReceiveSupplies ? 1 : 0);
                            HttpContext.Session.SetInt32("CanDisburseItems", dbUser.CanDisburseItems ? 1 : 0);

                            HttpContext.Session.SetString("LastLogin", dbUser.LastLoginTime.HasValue ? dbUser.LastLoginTime.ToString() : "");

                            return RedirectToPage("./Home");
                        }
                    }
                    else
                        this.errorMessage = "User LoginName or password is not valid.";
                }
                else
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "starting application user validation");

                    if (Program.Configuration.GetValue<bool>("UseLdap") && UserName == "admin" && Program.Configuration.GetValue<bool>("AllowAdminUserWithLdap") == false)
                    {
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Login failed. Admin user not allowed with LDAP authentication");
                        Helper.AddActivityLog(dbUser.UserId, "Login failed. Admin user not allowed with LDAP authentication", "Login", sourceIp, dbContext, false);
                        errorMessage = "User LoginName or password is not valid.";
                    }

                    var hash = Lib.Hash.GenerateSHA(UTF8Encoding.UTF8.GetBytes(Password + UserName));

                    if (hash.SequenceEqual(dbUser.Password))
                    {
                        Helper.AddActivityLog(dbUser.UserId, "User Logged in successfully", "Login", sourceIp, dbContext, false);
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "valid user, logging-in");

                        HttpContext.Session.SetInt32("UserId", dbUser.UserId);
                        HttpContext.Session.SetString("UserName", dbUser.UserName);
                        HttpContext.Session.SetString("FullName", dbUser.FullName);
                        HttpContext.Session.SetString("LastLogin", dbUser.LastLoginTime.HasValue ? dbUser.LastLoginTime.ToString() : "");
                        HttpContext.Session.SetInt32("CanManageStore", dbUser.CanManageStore?1:0);
                        HttpContext.Session.SetInt32("CanManageUsers", dbUser.CanManageUsers ? 1:0);
                        HttpContext.Session.SetInt32("CanManageItems", dbUser.CanManageItems ? 1 :0);
                        HttpContext.Session.SetInt32("CanReceiveSupplies", dbUser.CanReceiveSupplies ? 1 :0);
                        HttpContext.Session.SetInt32("CanDisburseItems", dbUser.CanDisburseItems ? 1 :0);


                        HttpContext.Session.SetString("LastLogin", dbUser.LastLoginTime.HasValue ? dbUser.LastLoginTime.ToString() : "");


                        return RedirectToPage("./Home");
                    }
                    else
                    {
                        Helper.AddActivityLog(dbUser.UserId, "Login failed", "Login", sourceIp, dbContext, false);
                        this.errorMessage = "User LoginName or password is not valid.";
                    }
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                errorMessage = ex.Message;

            }
            finally
            {
                if (dbUser != null && dbUser.FailedPasswordAttemptCount >= Program.Configuration.GetValue<int>("MaxWrongPassAttempts"))
                    dbUser.Locked = true;


                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Completed");
                task.EndTask();
            }

            return Page();
        }

    }
}
