using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using POSApplication.Data.Context;

namespace POSApplication.Data;

public class POSDbContextFactory : IDesignTimeDbContextFactory<POSDbContext>
{
    public POSDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<POSDbContext>();
        optionsBuilder.UseSqlite("Data Source=pos.db");
        
        return new POSDbContext(optionsBuilder.Options);
    }
}
