namespace LabMaterials.AppCode
{
    public class DefaultData
    {
        public void InsertDefaultData()
        {
            LogableTask task =  LogableTask.NewTask("Seed data");
            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, $"Init Started");
            try
            {
                var dbContext = new LabDBContext();

                if (dbContext.PrimaryKeys.Count() == 0)
                {
                    PrimaryKey primaryKey = new PrimaryKey() { NextId = 1000, TableName = "" };
                    dbContext.PrimaryKeys.Add(primaryKey);
                    dbContext.SaveChanges();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "PrimaryKeys added");
                }
                else
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "PrimaryKeys found");


                if (dbContext.Users.Count() == 0)
                {
                    var encriptor = new SimpleAES();
                    var adminUser = new User
                    {
                        UserId = 1,
                        UserName = "admin",
                        FullName = "Default administrator",
                        Email = "",
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        FailedPasswordAttemptCount = 0,
                        IsActiveDirectoryUser = false,
                        Locked = false,
                        CreatedById = 0,
                        UserGroupId = 1
                    };

                    adminUser.Password = Lib.Hash.GenerateSHA(System.Text.UTF8Encoding.UTF8.GetBytes("Lab1234" + adminUser.UserName));
                    dbContext.Users.Add(adminUser);

                    dbContext.SaveChanges();
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "admin user created");
                }
                else
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "admin user found");

                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, $"Init Complete");
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

    }
}
