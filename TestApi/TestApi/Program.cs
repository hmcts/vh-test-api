using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using VH.Core.Configuration;

namespace TestApi
{
    public class Program
    {
        protected Program()
        {
        }

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            const string vhInfraCore = "/mnt/secrets/vh-infra-core";
            const string vhTestApi = "/mnt/secrets/vh-test-api";

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((configBuilder) =>
                {
                    configBuilder.AddAksKeyVaultSecretProvider(vhInfraCore);
                    configBuilder.AddAksKeyVaultSecretProvider(vhTestApi);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureAppConfiguration(configBuilder =>
                    {
                        configBuilder.AddAksKeyVaultSecretProvider(vhInfraCore);
                        configBuilder.AddAksKeyVaultSecretProvider(vhTestApi);
                    });
                });
        }
    }
}