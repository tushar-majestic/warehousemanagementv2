using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

public partial class LabDBContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.EnableSensitiveDataLogging(true);
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(DBUtils.ConnectionsString);
        }
    }

    public LabDBContext() { }
}
