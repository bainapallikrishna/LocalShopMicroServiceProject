using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShop.Infrastructure
{
    using System.IO;
    using LocalShop.Infrastructure.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class LocalShopDbContextFactory : IDesignTimeDbContextFactory<LocalShopDbContext>
    {
        public LocalShopDbContext CreateDbContext(string[] args)
        {
            // Get the startup project directory (LocalShop)
            var startupProjectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "LocalShop"));
            
            var config = new ConfigurationBuilder()
                .SetBasePath(startupProjectPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // Check environment - for design-time, prefer local database
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            
            string connectionString;
            if (environment.Equals("Production", StringComparison.OrdinalIgnoreCase))
            {
                connectionString = config.GetConnectionString("AzureSqlConnection") 
                    ?? config.GetConnectionString("ConnLocalShopDb");
            }
            else
            {
                // For Development and design-time, use local database
                connectionString = config.GetConnectionString("ConnLocalShopDb");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"No connection string found for environment: {environment}");
            }

            var optionsBuilder = new DbContextOptionsBuilder<LocalShopDbContext>();
            optionsBuilder.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly("LocalShop.Infrastructure")
            );

            return new LocalShopDbContext(optionsBuilder.Options);
        }
    }
}



