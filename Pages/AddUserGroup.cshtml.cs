using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class AddUserGroupModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string UserGroupName;
        public List<int> SelectedPrivilages { get; set; }
        public SelectList Privilages { get; set; }
        
        public string lblAddUserGroup, lblUserGroupName, lblPrivilages, lblAdd, lblCancel;
        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageUsers == false)
                RedirectToPage("./Index?lang=" + Lang);
            FillLables();
            var dbContext = new LabDBContext();

            Privilages = new SelectList(dbContext.Privileges.ToList(), "PrivilegeId", "PrivilegeName");
        }

        public IActionResult OnPost([FromForm] string UserGroupName, List<int> SelectedPrivilages)
        {
            LogableTask task = LogableTask.NewTask("AddSupplier");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageUsers)
                {
                    FillLables();
                    this.UserGroupName = UserGroupName;
                    var dbContext = new LabDBContext();
                    Privilages = new SelectList(dbContext.Privileges.ToList(), "PrivilegeId", "PrivilegeName");

                    if (string.IsNullOrEmpty(UserGroupName))
                        ErrorMsg = (Program.Translations["UserGroupMissing"])[Lang];
                    else if(SelectedPrivilages.Count() < 1)
                        ErrorMsg = (Program.Translations["AtleastOnePrivilage"])[Lang];
                    else
                    {
                        if (dbContext.UserGroups.Count(s => s.UserGroupName == UserGroupName) > 0)
                            ErrorMsg = string.Format((Program.Translations["UserGroupExists"])[Lang], UserGroupName);
                        else
                        {
                            var userGroup = new UserGroup
                            {
                                UserGroupName = UserGroupName,
                                UserGroupId = PrimaryKeyManager.GetNextId()
                            };
                            dbContext.UserGroups.Add(userGroup);
                            dbContext.SaveChanges();

                            foreach (var privilage in SelectedPrivilages)
                            {
                                var userGroupPrivilage = new UserGroupPrivilege
                                {
                                    UserGroupId = userGroup.UserGroupId,
                                    PrivilegeId = privilage
                                };
                                dbContext.UserGroupPrivileges.Add(userGroupPrivilage);
                            }
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "User group added");

                            string Message = string.Format("User group {0} added", userGroup.UserGroupName);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                                Helper.ExtractIP(Request), dbContext, true);

                            return RedirectToPage("./ManageUserGroups");
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
            

            this.lblAddUserGroup = (Program.Translations["AddUserGroup"])[Lang];
            this.lblUserGroupName = (Program.Translations["UserGroupName"])[Lang];
            this.lblPrivilages = (Program.Translations["Privilages"])[Lang];

            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
        }
    }
}
