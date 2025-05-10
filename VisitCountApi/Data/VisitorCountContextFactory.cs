using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace VisitCountApi.Data
{
    public class VisitorCountContextFactory : IDesignTimeDbContextFactory<VisitorCountContext> 
    {
        public VisitorCountContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // ensures it reads from your project root
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<VisitorCountContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("VisitCountDB"));
            return new VisitorCountContext(optionsBuilder.Options);
        }
    }
}
