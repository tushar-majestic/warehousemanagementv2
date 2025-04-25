using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class viewUserModel : BasePageModel
    {
        public UserInfo singleUser { get; set; }

        public void OnGet()
        {
            base.ExtractSessionData();
            if (CanManageUsers)
            {
                var dbContext = new LabDBContext();
                var user = dbContext.Users.Single(u => u.UserId == HttpContext.Session.GetInt32("ToUpdateUserId"));
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

               // UserInfo singleUser = null;

                
                    singleUser = query.FirstOrDefault(s => s.UserID == user.UserId);
               
            }
            else
                RedirectToPage("./Index?lang=" + Lang);
        }
    }
}
