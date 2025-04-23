using LabMaterials.DB;
using Microsoft.Extensions.Primitives;

namespace LabMaterials.AppCode
{
    public class Helper
    {
        public static void AddActivityLog(int userID, string desc, string type, string IPAddress, LabDBContext dbContext = null, bool submitDBChanges = true)
        {
            ActivityLog activity = new ActivityLog()
            {
                ActivityTime = DateTime.Now,
                Description = desc,
                UserId = userID,
                SourceIp = IPAddress,
                Type = type,
            };
            if (dbContext == null)
                dbContext = new LabDBContext();

            dbContext.ActivityLogs.Add(activity);

            if (submitDBChanges)
                dbContext.SaveChanges();
        }


        public static string ExtractIP(HttpRequest request)
        {
            string ip = request.HttpContext.Connection.RemoteIpAddress.ToString();
            if (request.Headers.TryGetValue("X-Forwarded-For", out StringValues headerValues))
                ip = headerValues[0];

            return ip;
        }
    }
}
