using Microsoft.AspNetCore.Mvc;

namespace LabMaterials.Pages
{
    public class EditItemGroupModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string GroupCode, GroupCodeID;
        public string GroupDesc;
        
        public string lblGroupCode, lblGroupName, lblUpdate, lblCancel, lblUpdateItemGroup, lblItems, lblItemGroups;
        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageItems == false)
                RedirectToPage("./Index?lang=" + Lang);
            using (var dbContext = new LabDBContext())
            { 
                var group = dbContext.ItemGroups.Single(g => g.GroupCode == HttpContext.Session.GetString("GroupCode"));
                this.GroupCode = group.GroupCode;
                this.GroupCodeID = group.GroupCode;
                this.GroupDesc = group.GroupDesc;  
            }
            FillLables();

        }

        public IActionResult OnPost([FromForm] string GroupCodeID, [FromForm] string GroupCode, [FromForm] string GroupDesc)
        {
            LogableTask task = LogableTask.NewTask("UpdateGroup");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageItems)
                {
                    FillLables();
                    this.GroupCode = GroupCode;
                    this.GroupCodeID = GroupCodeID;
                    this.GroupDesc = GroupDesc;

                    if (string.IsNullOrEmpty(GroupCode))
                        ErrorMsg = (Program.Translations["GroupCodeMissing"])[Lang];
                    else if (string.IsNullOrEmpty(GroupDesc))
                        ErrorMsg = (Program.Translations["GroupNameMissing"])[Lang];
                    else
                    {
                        var dbContext = new LabDBContext();
                        if (dbContext.ItemGroups.Count(s => s.GroupCode == GroupCode && GroupCode != GroupCodeID) > 0)
                            ErrorMsg = string.Format((Program.Translations["GroupCodeExists"])[Lang], GroupCode);
                        else if (dbContext.ItemGroups.Count(s => s.GroupDesc == GroupDesc && s.GroupCode != GroupCodeID) > 0)
                            ErrorMsg = string.Format((Program.Translations["GroupNameExists"])[Lang], GroupDesc);
                        else
                        {
                            dbContext.SaveChanges();

                            var group = dbContext.ItemGroups.Single(g => g.GroupCode == GroupCodeID);
                            group.GroupCode = GroupCode;
                            group.GroupDesc = GroupDesc;

                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "Group updated");

                            string Message = string.Format("Group {0} updated", group.GroupDesc);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "update",
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
            this.lblUpdate = (Program.Translations["Update"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblUpdateItemGroup = (Program.Translations["UpdateItemGroup"])[Lang];
            this.lblItems = (Program.Translations["Items"])[Lang];
            this.lblItemGroups = (Program.Translations["ItemGroups"])[Lang];
        }

    }
}
