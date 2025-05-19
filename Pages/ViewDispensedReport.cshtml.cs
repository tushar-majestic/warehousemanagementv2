using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using LabMaterials.DB;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using System.Security.AccessControl;

namespace LabMaterials.Pages
{
    public class ViewDispensedReportModel : BasePageModel
    {

        public string lblView, lblUsers, lblSearch, lblAddUser, lblManageUserGroups, lblUserName, lblFullName, lblEmail,
        lblUserEnabled, lblIsLocked, lblUserType, lblUserGroupName, lblEdit, lblUnlock, lblTotalItem, lblJobNumber,
        lblEmpAffiliation, lblTransfer;
        public void OnGet()
        {

            base.ExtractSessionData();
            FillLables();
        }

        private void FillLables()
        {

            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblAddUser = (Program.Translations["AddUser"])[Lang];
            this.lblManageUserGroups = (Program.Translations["ManageUserGroups"])[Lang];
            this.lblUserName = (Program.Translations["UserName"])[Lang];
            this.lblFullName = (Program.Translations["FullName"])[Lang];
            this.lblEmail = (Program.Translations["Email"])[Lang];
            this.lblUserEnabled = (Program.Translations["UserEnabled"])[Lang];
            this.lblIsLocked = (Program.Translations["IsLocked"])[Lang];
            this.lblUserType = (Program.Translations["UserType"])[Lang];
            this.lblUserGroupName = (Program.Translations["UserGroupName"])[Lang];

            this.lblEdit = (Program.Translations["Edit"])[Lang];
            this.lblView = (Program.Translations["View"])[Lang];
            this.lblUnlock = (Program.Translations["Unlock"])[Lang];
            this.lblTotalItem = (Program.Translations["TotalItem"])[Lang];
            this.lblView = (Program.Translations["View"])[Lang];
            this.lblJobNumber = (Program.Translations["JobNumber"])[Lang];
            this.lblEmpAffiliation = (Program.Translations["EmpAffiliation"])[Lang];
            this.lblTransfer = (Program.Translations["Transfer"])[Lang];

        }
    }
    
}


