using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LabMaterials.DB;
using System.Linq;
using LabMaterials.DB;
using LabMaterials.dtos;
using System.DirectoryServices.Protocols;


namespace LabMaterials.Pages
{
    public class ReceivingItemsModel : BasePageModel
    {
        public List<Message> InboxList { get; set; }
        public string UserFullName;
        public string UserGroupName;
        public int? UserId;
        public int InboxCount;

        public List<ReceivingReport> RequestSent { get; set; }
        public List<ReceivingReport> AllRequest { get; set; }

        public List<MaterialRequest> AllDispenseRequest { get; set; }
        public IList<Store> Warehouses { get; set; }
        public List<User> AllUsers { get; set; }
        public List<UserGroup> UserGroups { get; set; }

        public string lblNewReceivingReport;

        public void OnGet(string? searchTerm = null)
        {
            var dbContext = new LabDBContext();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");

            this.UserId = HttpContext.Session.GetInt32("UserId");

            AllRequest = dbContext.ReceivingReports.ToList();
            AllDispenseRequest = dbContext.MaterialRequests.ToList();
            AllUsers = dbContext.Users.ToList();
            UserGroups = dbContext.UserGroups.ToList();
            Warehouses = dbContext.Stores.ToList();


            if (this.UserGroupName == "Warehouse Keeper")
            {
                RequestSent = dbContext.ReceivingReports
                    .Where(r => r.CreatedBy == UserId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var lowerSearch = searchTerm.ToLower();

                    RequestSent = RequestSent
                        .Where(r =>
                        {
                            var store = Warehouses.FirstOrDefault(u => u.StoreId == int.Parse(r.ReceivingWarehouse));
                            var storename = store?.StoreName?.ToLower() ?? "";

                            return storename.Contains(lowerSearch);

                        }).ToList();


                }
            }

            base.ExtractSessionData();
            FillLables();
        }


        private void FillLables()
        {
            this.lblNewReceivingReport = (Program.Translations["NewReceivingReport"])[Lang];
        }
    }
}