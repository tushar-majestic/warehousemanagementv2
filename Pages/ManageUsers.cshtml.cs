using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class ManageUsersModel : BasePageModel
    {
        public List<UserInfo> Users { get; set; }
        public List<UserInfo> UsersAll { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }
        [BindProperty]
        public string UserName { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public List<string> SelectedColumns { get; set; } = new List<string>();

        public void OnGet(string? UserName, int page = 1) 
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                FillLables();
                LoadSelectedColumns();
                //FillData("");
                if (HttpContext.Request.Query.ContainsKey("page"))
                {
                    string pagevalue = HttpContext.Request.Query["page"];
                    page = int.Parse(pagevalue);
                    this.UserName = UserName;
                    FillData(UserName, page);

                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
        //

        private void LoadSelectedColumns()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                using (var db = new LabDBContext())
                {
                    string pageName = "manageUsers";
                    var existingRecord = db.Tablecolumns.FirstOrDefault(c => c.UserId == userId.Value && c.Page == pageName);
                    if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.DisplayColumns))
                    {
                        SelectedColumns = existingRecord.DisplayColumns.Split(',').ToList();
                    }
                    else
                    {
                        // SelectedColumns = new List<string>();
                        string selectedColumns = "userName,fullName,email";
                        SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                    }
                }
            }
        }

        public string lblView, lblUsers, lblSearch, lblAddUser, lblManageUserGroups, lblUserName, lblFullName, lblEmail, lblIsDomainUser, lblJobNumber, lblEmpAffiliation, lblTransfer,
            lblUserEnabled, lblIsLocked, lblUserType, lblUserGroupName, lblEdit, lblUnlock, lblTotalItem;


        public void OnPostSearch([FromForm] string UserName)
        {
            CurrentPage = 1;
            this.UserName = UserName;
            FillData(this.UserName, CurrentPage);

        }

        public IActionResult OnPostAction(string UserName, string action, List<string> columns)
        {
            base.ExtractSessionData();

            if (action == "search")
            {
                CurrentPage = 1;
                this.UserName = UserName;

                FillData(UserName, CurrentPage);

                int? userId = HttpContext.Session.GetInt32("UserId");
                string pageName = "manageUsers";
                LoadSelectedColumns();
            }
            else if (action == "updateColumns")
            {
                if (columns != null && columns.Any())
                {

                    string selectedColumns = string.Join(",", columns);

                    int? userId = HttpContext.Session.GetInt32("UserId");
                    string pageName = "manageUsers";
                    this.UserName = UserName;
                    FillData(UserName, CurrentPage);
                    LoadSelectedColumns();

                    SaveSelectedColumns(userId.Value, pageName, selectedColumns);
                }

                // After updating, redirect back to ManageStore with the StoreNumber and StoreName
                // return RedirectToPage("/ManageDamaged", new { ItemName = ItemName, Group = Group });
            }

            return Page();
        }

        private void SaveSelectedColumns(int userId, string pageName, string selectedColumns)
        {
            base.ExtractSessionData();
            using (var db = new LabDBContext())
            {
                var existingRecord = db.Tablecolumns
                    .FirstOrDefault(c => c.UserId == userId && c.Page == pageName);

                if (existingRecord != null)
                {
                    existingRecord.DisplayColumns = selectedColumns;
                }
                else
                {
                    var newRecord = new Tablecolumn
                    {
                        UserId = userId,
                        Page = pageName,
                        DisplayColumns = selectedColumns
                    };
                    db.Tablecolumns.Add(newRecord);
                }

                db.SaveChanges();
            }
        }

        public void OnPostEnable([FromForm] int UserId)
        {
            base.ExtractSessionData();
            if (HttpContext.Session.GetInt32("UserId").Value != UserId)
            {
                //
                var dbContext = new LabDBContext();
                string MessageText = Lang == "ar" ? "تمكين" : "Enabled";
                var user = dbContext.Users.Single(s => s.UserId == UserId);
                if (user.IsActive)
                {
                    user.IsActive = false;
                    MessageText = Lang == "ar" ? "تعطيل" : "Disabled";
                }
                else
                    user.IsActive = true;

                dbContext.SaveChanges();
                FillData(null);
                Message = string.Format((Program.Translations["User"])[Lang] + " {0} " + MessageText, user.UserName);
                string MessageLog = string.Format((Program.Translations["User"])["en"] + " {0} " + MessageText, user.UserName);
                Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, MessageLog, MessageText, Helper.ExtractIP(Request), dbContext, true);
            }
            else
            {
                FillData(null);
                Message = string.Format((Program.Translations["UserSelfUpdateNotAllowed"])[Lang]);
            }
        }

        public void OnPostUnlock([FromForm] int UserId)
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                if (HttpContext.Session.GetInt32("UserId").Value != UserId)
                {
                    var dbContext = new LabDBContext();
                    var user = dbContext.Users.Single(s => s.UserId == UserId);
                    if (user.Locked)
                    {
                        user.Locked = false;
                        user.FailedPasswordAttemptCount = 0;
                        dbContext.SaveChanges();
                        Message = string.Format((Program.Translations["UserUnlocked"])[Lang], user.UserName);
                    }
                    else
                        Message = string.Format((Program.Translations["UserAlreadyUnlocked"])[Lang], user.UserName);


                    FillData(null);
                    Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Unlocked", Helper.ExtractIP(Request), dbContext, true);
                }
                else
                {
                    FillData(null);
                    Message = string.Format((Program.Translations["UserSelfUpdateNotAllowed"])[Lang]);
                }
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPostEdit([FromForm] int UserId, [FromForm] int page, [FromForm] string UserName)
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                if (HttpContext.Session.GetInt32("UserId").Value != UserId)
                {
                    HttpContext.Session.SetInt32("page", page);
                    HttpContext.Session.SetInt32("ToUpdateUserId", UserId);
                     HttpContext.Session.SetString("UserName", string.IsNullOrEmpty(UserName) ? "" : UserName);
                    return RedirectToPage("./EditUser");
                }
                else
                {
                    FillData(null);
                    Message = string.Format((Program.Translations["UserSelfUpdateNotAllowed"])[Lang]);
                    return Page();
                }
            }
            else
                return RedirectToPage("./Index?lang=" + Lang);
        }  
        
        public IActionResult OnPostView([FromForm] int UserId, [FromForm] int page)
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                //if (HttpContext.Session.GetInt32("UserId").Value != UserId)
                {
                    HttpContext.Session.SetInt32("page", page);
                    HttpContext.Session.SetInt32("ToUpdateUserId", UserId);
                    return RedirectToPage("./viewUser");
                }
                //else
                //{
                //    FillData(null);
                //    Message = string.Format((Program.Translations["UserSelfUpdateNotAllowed"])[Lang]);
                //    return Page();
                //}
            }
            else
                return RedirectToPage("./Index?lang=" + Lang);
        }

       
        // function before pagination 
        /*private void FillData(string? UserName)
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = from u in dbContext.Users
                            join g in dbContext.UserGroups on u.UserGroupId equals g.UserGroupId
                            select new UserInfo
                            {
                                UserID = u.UserId,
                                UserName = u.UserName,
                                FullName = u.FullName,
                                Email = u.Email,
                                IsActive = u.IsActive ? (Lang == "ar" ? "تمكين" : "Enabled") : (Lang == "ar" ? "تعطيل" : "Disabled"),
                                EnableBtnText = u.IsActive ? (Lang == "ar" ? "تعطيل" : "Disable") : (Lang == "ar" ? "تمكين" : "Enable"),
                                IsADUser = u.IsActiveDirectoryUser ? (Lang == "ar" ? "مستخدم المجال" : "Domain User") : (Lang == "ar" ? "مستخدم التطبيق" : "Application User"),
                                IsLocked = u.Locked ? (Lang == "ar" ? "نعم" : "Yes") : (Lang == "ar" ? "لا" : "No"),
                                GroupName = g.UserGroupName
                            };

                if (string.IsNullOrEmpty(UserName) == false)
                    query = query.Where(s => s.UserName.Contains(UserName));


                Users = query.ToList();

                TotalItems = Users.Count();
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }*/

        private void FillData(string? UserName, int page = 1)
        {    if (HttpContext.Request.Query.ContainsKey("page"))
            {
                string pagevalue = HttpContext.Request.Query["page"];
                page = int.Parse(pagevalue);
            }
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                FillLables();
                var dbContext = new LabDBContext();
                var query = from u in dbContext.Users
                            join g in dbContext.UserGroups on u.UserGroupId equals g.UserGroupId
                            select new UserInfo
                            {
                                UserID = u.UserId,
                                UserName = u.UserName,
                                FullName = u.FullName,
                                Email = u.Email,
                                EmpAffiliation = u.EmpAffiliation,
                                JobNumber = u.JobNumber.ToString(),
                                Transfer = u.Transfer.ToString(),

                                IsActive = u.IsActive ? (Lang == "ar" ? "تمكين" : "Enabled") : (Lang == "ar" ? "تعطيل" : "Disabled"),
                                EnableBtnText = u.IsActive ? (Lang == "ar" ? "تعطيل" : "Disable") : (Lang == "ar" ? "تمكين" : "Enable"),
                                IsADUser = u.IsActiveDirectoryUser ? (Lang == "ar" ? "مستخدم المجال" : "Domain User") : (Lang == "ar" ? "مستخدم التطبيق" : "Application User"),
                                IsLocked = u.Locked ? (Lang == "ar" ? "نعم" : "Yes") : (Lang == "ar" ? "لا" : "No"),
                                GroupName = g.UserGroupName
                            };

                if (string.IsNullOrEmpty(UserName) == false)
                    query = query.Where(s => s.UserName.Contains(UserName));

                TotalItems = query.Count();

                // Calculate total pages
                TotalPages = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);

                var list = query.ToList();

                Users = list.Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();   
                UsersAll = query.ToList();  
                
                CurrentPage = page;
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
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
            this.lblIsDomainUser = (Program.Translations["IsDomainUser"])[Lang];
            this.lblJobNumber = (Program.Translations["JobNumber"])[Lang];
            this.lblEmpAffiliation = (Program.Translations["EmpAffiliation"])[Lang];
            this.lblTransfer = (Program.Translations["Transfer"])[Lang];

        }
    }
}
