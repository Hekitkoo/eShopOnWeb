using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.PublicApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args)
            //.ConfigureAppConfiguration((hostingContext, config) =>
            //{
            //    var env = hostingContext.HostingEnvironment;
            //    if (env.IsDevelopment())
            //    {
            //        var assembly = Assembly.Load(new AssemblyName(env.ApplicationName));
            //        if (assembly != null)
            //        {
            //            config.AddUserSecrets(assembly, true);
            //        }
            //    }

            //    config.Build();
            //    config.AddAzureKeyVault("https://cloudxfinal.vault.azure.net/");
            //})
            .Build();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var catalogContext = services.GetRequiredService<CatalogContext>();
                var identityContext = services.GetRequiredService<AppIdentityDbContext>();

                await CatalogContextSeed.SeedAsync(catalogContext, loggerFactory);

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await AppIdentityDbContextSeed.SeedAsync(identityContext, userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred seeding the DB.");
            }
        }

        host.Run();
    }


    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
