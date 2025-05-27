using Microsoft.AspNetCore.Mvc;

namespace LabMaterials.Pages
{
    public class AddItemGroupModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string GroupCode;
        public string GroupDesc;
        
        public string lblGroupCode, lblGroupName, lblAdd, lblCancel, lblAddItemGroup, lblItemGroups, lblItems, lblUsers;

        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            if (CanManageItems == false)
                RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPost([FromForm] string GroupCode, [FromForm] string GroupDesc)
        {
            LogableTask task = LogableTask.NewTask("AddGroup");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageItems)
                {
                    FillLables();
                    this.GroupCode = GroupCode;
                    this.GroupDesc = GroupDesc;

                    if (string.IsNullOrEmpty(GroupCode))
                        ErrorMsg = (Program.Translations["GroupCodeMissing"])[Lang];
                    else if (string.IsNullOrEmpty(GroupDesc))
                        ErrorMsg = (Program.Translations["GroupNameMissing"])[Lang];
                    else
                    {
                        var dbContext = new LabDBContext();
                        if (dbContext.ItemGroups.Count(s => s.GroupCode == GroupCode) > 0)
                            ErrorMsg = string.Format((Program.Translations["GroupCodeExists"])[Lang], GroupCode);
                        else if (dbContext.ItemGroups.Count(s => s.GroupDesc == GroupDesc) > 0)
                            ErrorMsg = string.Format((Program.Translations["GroupNameExists"])[Lang], GroupDesc);
                        else
                        {
                            var group = new ItemGroup
                            {
                                GroupCode = GroupCode,
                                GroupDesc = GroupDesc
                            };
                            dbContext.ItemGroups.Add(group);
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "Group added");

                            string Message = string.Format("Group {0} added", group.GroupDesc);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                                Helper.ExtractIP(Request), dbContext, true);

                            return RedirectToPage("./ManageItemGroups");
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
            

            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblGroupCode = (Program.Translations["GroupCode"])[Lang];
            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblAddItemGroup = (Program.Translations["AddItemGroup"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];
            this.lblItemGroups = (Program.Translations["ItemGroups"])[Lang];
            this.lblUsers = (Program.Translations["Users"])[Lang];
        }

    }
}
