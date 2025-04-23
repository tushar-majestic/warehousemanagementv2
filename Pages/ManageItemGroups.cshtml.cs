using LabMaterials.dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace LabMaterials.Pages
{
    public class ManageItemGroupsModel : BasePageModel
    {
        public List<GroupInfo> Groups { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }
        public void OnGet() 
        {
            base.ExtractSessionData();
            if (CanManageItemGroup)
            {
                FillLables();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
        
        public string lblGroupCode, lblGroupName, lblEdit, lblDelete, lblTotalItem, lblAddItemGroup, lblItemGroups, lblSearch;

        public void OnPostSearch([FromForm] string GroupName)
        {
            FillData(GroupName);
        }

        public void OnPostDelete([FromForm] string GroupCode)
        {
            base.ExtractSessionData();
            if (CanManageItemGroup)
            {
                var dbContext = new LabDBContext();

                var units = dbContext.Units.Count(s => s.GroupCode == GroupCode);
                var items = dbContext.Items.Count(s => s.GroupCode == GroupCode);
                if (units == 0 && items == 0)
                {
                    var group = dbContext.ItemGroups.Single(s => s.GroupCode == GroupCode);
                    dbContext.ItemGroups.Remove(group);
                    dbContext.SaveChanges();
                    FillData(null);
                    Message = string.Format((Program.Translations["ItemGroupDeleted"])[Lang], group.GroupDesc);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Delete", Helper.ExtractIP(Request), dbContext, true);
                }
                else
                {
                    string text = "";
                    if (units != 0)
                        text = dbContext.Units.First(s => s.GroupCode == GroupCode).UnitDesc;
                    else if (items != 0)
                        text = dbContext.Items.First(i => i.GroupCode == GroupCode).ItemName;

                    var groupName = dbContext.ItemGroups.First(s => s.GroupCode == GroupCode).GroupDesc;
                    Message = string.Format((Program.Translations["ItemGroupNotDeleted"])[Lang], groupName, text);
                    FillData(null);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);

        }


        public IActionResult OnPostEdit([FromForm] string GroupCode)
        {
            HttpContext.Session.SetString("GroupCode", GroupCode);

            return RedirectToPage("./EditItemGroup");
        }

        private void FillData(string? GroupName)
        {
            base.ExtractSessionData();
            if (CanManageItemGroup)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = from s in dbContext.ItemGroups
                            select new GroupInfo
                            {
                                GroupCode = s.GroupCode,
                                GroupDesc  = s.GroupDesc
                            };

                if (string.IsNullOrEmpty(GroupName) == false)
                    query = query.Where(s => s.GroupDesc.Contains(GroupName));


                Groups = query.ToList();

                Groups = query.ToList();
                TotalItems = Groups.Count();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        private void FillLables()
        {
            this.Lang =  HttpContext.Session.GetString("Lang");

            this.lblGroupName = (Program.Translations["GroupName"])[Lang];
            this.lblGroupCode = (Program.Translations["GroupCode"])[Lang];
            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblDelete = (Program.Translations["Delete"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblAddItemGroup = (Program.Translations["AddItemGroup"])[Lang];
            this.lblItemGroups = (Program.Translations["ItemGroups"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
        }
    }
}