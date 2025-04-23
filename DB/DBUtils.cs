using Microsoft.AspNetCore.Hosting;

namespace LabMaterials.DB
{
    public class DBUtils
    {
        static string connectionsStr;

        public static string ConnectionsString
        {
            get
            {
                if (connectionsStr == null)
                    ReloadConnectionString();
                return connectionsStr;
            }
        }

        /// <summary>
        /// it is public but not supposed to be used from out-side other than force reload senarios.
        /// Use ConnectionsString property instead
        /// </summary>
        public static void ReloadConnectionString()
        {
            string str = File.ReadAllText(Program.HostingEnv.ContentRootPath + "/ConnectionString.config");
            SimpleAES enc = new SimpleAES();
            connectionsStr = str;
        }

        public static void TestDBConnection()
        {
            using var dbContext = new LabDBContext();
            dbContext.HazardTypes.Count();
        }
    }
}
