using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using VH.Core.Configuration;
using System.Collections.Generic;

namespace TestApi
{
    public class Program
    {
        protected Program()
        {
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var keyVaults=new List<string> (){
                "vh-infra-core",
                "vh-test-api",
                "vh-bookings-api",
                "vh-video-api",
                "vh-user-api"
            };

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((configBuilder) =>
                {
                    foreach (var keyVault in keyVaults)
                    {
                        configBuilder.AddAksKeyVaultSecretProvider($"/mnt/secrets/{keyVault}");
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}