using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class EditUserGroupModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string UserGroupName;
        public int UserGroupId;
        public List<int> SelectedPrivilages { get; set; }
        public SelectList Privilages { get; set; }
        public int page { get; set; }
        public string lblUpdateUserGroup, lblUserGroupName, lblPrivilages, lblUpdate, lblCancel, lblUserGroup, lblUsers, UserGroupNameSearch;
        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageUsers == false)
                RedirectToPage("./Index?lang=" + Lang);
            FillLables();
            this.page = (int)HttpContext.Session.GetInt32("page");
            this.UserGroupNameSearch = HttpContext.Session.GetString("UserGroupName");
            var dbContext = new LabDBContext();

            Privilages = new SelectList(dbContext.Privileges.ToList(), "PrivilegeId", "PrivilegeName");

            var userGroup = dbContext.UserGroups.Single(i => i.UserGroupId == HttpContext.Session.GetInt32("UserGroupID"));
            this.UserGroupName = userGroup.UserGroupName;
            this.UserGroupId = userGroup.UserGroupId;
            this.SelectedPrivilages = (from ugp in dbContext.UserGroupPrivileges 
                                       where ugp.UserGroupId == userGroup.UserGroupId
                                       select ugp.PrivilegeId).ToList();
        }

        public IActionResult OnPost([FromForm] int UserGroupId, [FromForm] string UserGroupName, List<int> SelectedPrivilages)
        {
            LogableTask task = LogableTask.NewTask("UpdateUserGroup");

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
                        if (dbContext.UserGroups.Count(s => s.UserGroupName == UserGroupName && s.UserGroupId != UserGroupId) > 0)
                            ErrorMsg = string.Format((Program.Translations["UserGroupExists"])[Lang], UserGroupName);
                        else
                        {
                            var userGroup = dbContext.UserGroups.Single(ug => ug.UserGroupId == UserGroupId);
                            userGroup.UserGroupName = UserGroupName;
                            var currentPrivilages = dbContext.UserGroupPrivileges.Where(ug => ug.UserGroupId == UserGroupId).ToList();
                            dbContext.UserGroupPrivileges.RemoveRange(currentPrivilages);
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
                            task.LogInfo(MethodBase.GetCurrentMethod(), "User group updated");

                            string Message = string.Format("User group {0} added", userGroup.UserGroupName);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Update",
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
            

            this.lblUpdateUserGroup = (Program.Translations["UpdateUserGroup"])[Lang];
            this.lblUserGroupName = (Program.Translations["UserGroupName"])[Lang];
            this.lblPrivilages = (Program.Translations["Privilages"])[Lang];

            this.lblUpdate = (Program.Translations["Update"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblUserGroup = (Program.Translations["UserGroups"])[Lang];
        }
    }
}
