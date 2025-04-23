using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LabMaterials.Pages
{
    public class AddDestinationModel : BasePageModel
    {
        public string ErrorMsg { get; set; }
        public string DestinationName;

        public string lblAddDestination, lblDestinationName, lblAdd, lblCancel, lblDestinations, lblStores;
        public void OnGet()
        {
            base.ExtractSessionData();
            FillLables();
            if (CanManageStore == false)
                RedirectToPage("./Index?lang=" + Lang);
        }

        public IActionResult OnPost([FromForm] string DestinationName)
        {
            LogableTask task = LogableTask.NewTask("AddDestination");

            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                base.ExtractSessionData();
                if (CanManageStore)
                {
                    FillLables();
                    this.DestinationName = DestinationName;
                   
                    if (string.IsNullOrEmpty(DestinationName))
                        ErrorMsg = (Program.Translations["DestinationNameMissing"])[Lang];
                    else
                    {
                        var dbContext = new LabDBContext();
                        if (dbContext.Destinations.Count(s => s.DestinationName == DestinationName) > 0)
                            ErrorMsg = string.Format((Program.Translations["DestinationNameExists"])[Lang], DestinationName);
                        else
                        {
                            var destination = new Destination
                            {
                                DestinationName = DestinationName
                            };
                            dbContext.Destinations.Add(destination);
                            dbContext.SaveChanges();
                            task.LogInfo(MethodBase.GetCurrentMethod(), "destination added");

                            string Message = string.Format("Destination {0} added", destination.DestinationName);
                            Helper.AddActivityLog(HttpContext.Session.GetInt32("UserId").Value, Message, "Add",
                                Helper.ExtractIP(Request), dbContext, true);

                            return RedirectToPage("./ManageDestinations");
                            return RedirectToPage("./ManageStore");
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


            this.lblAddDestination = (Program.Translations["AddDestination"])[Lang];
            this.lblDestinationName = (Program.Translations["DestinationName"])[Lang];
            this.lblAdd = (Program.Translations["Add"])[Lang];
            this.lblCancel = (Program.Translations["Cancel"])[Lang];
            this.lblDestinations = (Program.Translations["Destinations"])[Lang];
            this.lblStores = (Program.Translations["Stores"])[Lang];
        }
    }
}
