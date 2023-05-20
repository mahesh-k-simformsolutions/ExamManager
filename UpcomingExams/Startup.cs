using ExamManagementSystem.Data.DbContext;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

[assembly: FunctionsStartup(typeof(UpcomingExams.Startup))]
namespace UpcomingExams
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = BuildConfiguration(builder.GetContext().ApplicationRootPath);
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(
              options => SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionString));

            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFile($"Logs/{Assembly.GetExecutingAssembly().GetName().Name}.log");
            });
        }
        private IConfiguration BuildConfiguration(string applicationRootPath)
        {
            var config =
                new ConfigurationBuilder()
                    .SetBasePath(applicationRootPath)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            return config;
        }
    }
}