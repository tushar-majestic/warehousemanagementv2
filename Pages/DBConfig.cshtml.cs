global using LabMaterials.DB;
global using LabMaterials.AppCode;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace LabMaterials.Pages
{
    public class DBConfigModel : PageModel
    {
        public string error;
        public string msg;

        [BindProperty]
        public string connString { get; set; }

        public IActionResult OnGet()
        {
            if (System.IO.File.Exists(Program.HostingEnv.ContentRootPath + "\\ConnectionString.config") == true)
            {
                if (Program.Configuration.GetSection("CanUpdateConnectionString").Value == "true")
                {
                    var sqlConnectionStringBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(DB.DBUtils.ConnectionsString);
                    sqlConnectionStringBuilder.Password = "******";
                    connString = sqlConnectionStringBuilder.ConnectionString;
                }
            }

            return Page();
        }

        public void OnPostSave()
        {
            LogableTask task = LogableTask.NewTask("DBConfig.OnPostSave");
            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                SqlConnection sqlConnection = new SqlConnection(connString);
                sqlConnection.Open();
                sqlConnection.Close();
                var aesUtil = new SimpleAES();

                System.IO.File.WriteAllText(Program.HostingEnv.ContentRootPath + "/ConnectionString.config",
                    aesUtil.EncryptToString(connString));

                (new DefaultData()).InsertDefaultData();

                msg = "Connection string updated successfully";
                task.LogInfo(MethodBase.GetCurrentMethod(), "Connection string updated");
            }
            catch (Exception ex)
            {
                task.LogError(MethodBase.GetCurrentMethod(), ex);
                error = "Error : Connection failed, Connection string was not updated.";
            }
            finally { task.EndTask(); }
        }

        public void OnPostUpdate()
        {
            LogableTask task = LogableTask.NewTask("DBConfig.OnPostUpdate");
            try
            {
                task.LogInfo(MethodBase.GetCurrentMethod(), "Called");
                SqlConnection sqlConnection = new SqlConnection(connString);
                sqlConnection.Open();
                sqlConnection.Close();
                var aesUtil = new SimpleAES();

                System.IO.File.WriteAllText(Program.HostingEnv.ContentRootPath + "/ConnectionString.config",
                    aesUtil.EncryptToString(connString));

                msg = "Connection string updated successfully";
                task.LogInfo(MethodBase.GetCurrentMethod(), "Connection string updated");

                try { DBUtils.ReloadConnectionString(); }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
                OnGet();
            }
            catch (Exception ex)
            {
                task.LogError(MethodBase.GetCurrentMethod(), ex);
                error = "Error : Connection failed, Connection string was not updated.";
            }
            finally { task.EndTask(); }
        }

        public void OnPostTest()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(connString);
                sqlConnection.Open();
                sqlConnection.Close();
                msg = "Connection string tested ok ";
                LogableTask.LogSingleActivity("DBConfig test", MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "Connection string tested ok ");
            }
            catch (Exception ex)
            {
                LogableTask.LogSingleActivity("DBConfig test", MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                error = "Error : Connection failed.";
            }
        }
    }
}
