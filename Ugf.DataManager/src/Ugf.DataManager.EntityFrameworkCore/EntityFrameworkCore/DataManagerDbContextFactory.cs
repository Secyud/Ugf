using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ugf.DataManager.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class DataManagerDbContextFactory : IDesignTimeDbContextFactory<DataManagerDbContext>
{
    public DataManagerDbContext CreateDbContext(string[] args)
    {
        DataManagerEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<DataManagerDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new DataManagerDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Ugf.DataManager.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}