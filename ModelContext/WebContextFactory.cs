
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ModelContext;
using System;
using System.IO;

namespace MedRoom.Models.Models
{
    public class WebContextFactory : IDesignTimeDbContextFactory<WebContext>
    {
        public WebContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<WebContext> options = new DbContextOptionsBuilder<WebContext>();
            IConfiguration config = GetAppConfiguration();

            options.UseNpgsql(config.GetSection("ConnectionStrings").GetSection("Web").Value);

            return new WebContext(options.Options);
        }

        private IConfiguration GetAppConfiguration()
        {

            DirectoryInfo dir = Directory.GetParent(AppContext.BaseDirectory);
            do
            {
                dir = dir.Parent;
            }
            while (dir.Name != "bin");
            dir = dir.Parent;
            string path = dir.FullName;

            IConfigurationBuilder builder = new ConfigurationBuilder()
                    .SetBasePath(path)
                    .AddJsonFile("appsettings.json")
#if DEBUG
                    .AddJsonFile($"appsettings.Debug.json", true)
#endif
                    .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
