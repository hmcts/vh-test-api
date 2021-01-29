using AcceptanceTests.Common.Api;
using AcceptanceTests.Common.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TestApi.Common.Configuration;
using TestApi.Common.Security;
using TestApi.DAL;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Test
{
    public class Setup
    {
        private readonly TestContext _context;
        private readonly IConfigurationRoot _configRoot;

        public Setup()
        {
            _context = new TestContext();
            _configRoot = ConfigurationManager.BuildConfig("04df59fe-66aa-4fb2-8ac5-b87656f7675a");
            _context.Config = new Config();
        }

        public TestContext RegisterSecrets()
        {
            var azureOptions = RegisterAzureSecrets();
            RegisterDatabaseSettings();
            RegisterUsernameStem();
            RegisterServer();
            RegisterServices();
            RegisterWowzaSettings();
            GenerateBearerTokens(azureOptions);
            return _context;
        }

        private void RegisterServices()
        {
            _context.Config.Services = Options.Create(_configRoot.GetSection("Services").Get<ServicesConfiguration>()).Value;
            _context.Config.Services.TestApiUrl.Should().NotBeNullOrEmpty();
        }

        private IOptions<AzureAdConfiguration> RegisterAzureSecrets()
        {
            var azureOptions = Options.Create(_configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            _context.Config.AzureAdConfiguration = azureOptions.Value;
            ConfigurationManager.VerifyConfigValuesSet(_context.Config.AzureAdConfiguration);
            return azureOptions;
        }

        private void RegisterDatabaseSettings()
        {
            _context.Config.DbConnection =
                Options.Create(_configRoot.GetSection("ConnectionStrings").Get<DbConfig>()).Value;
            ConfigurationManager.VerifyConfigValuesSet(_context.Config.DbConnection);
            var db_contextOptionsBuilder = new DbContextOptionsBuilder<TestApiDbContext>();
            db_contextOptionsBuilder.EnableSensitiveDataLogging();
            db_contextOptionsBuilder.UseSqlServer(_context.Config.DbConnection.TestApi);
            _context.DbContextOptions = db_contextOptionsBuilder.Options;
            _context.Data = new Data(_context, _context.DbContextOptions);
        }

        private void RegisterUsernameStem()
        {
            _context.Config.UsernameStem = _configRoot.GetValue<string>("UsernameStem");
        }

        private void RegisterServer()
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                .UseEnvironment("Development")
                .UseKestrel(c => c.AddServerHeader = false)
                .UseStartup<Startup>();
            _context.Server = new TestServer(webHostBuilder);
        }

        private void RegisterWowzaSettings()
        {
            _context.Config.Wowza = Options.Create(_configRoot.GetSection("WowzaConfiguration").Get<WowzaConfiguration>()).Value;
            _context.Config.Wowza.StorageAccountKey.Should().NotBeNullOrEmpty();
            _context.Config.Wowza.StorageAccountName.Should().NotBeNullOrEmpty();
            _context.Config.Wowza.StorageContainerName.Should().NotBeNullOrEmpty();
            ConfigurationManager.VerifyConfigValuesSet(_context.Config.Wowza);
        }

        private void GenerateBearerTokens(IOptions<AzureAdConfiguration> azureOptions)
        {
            _context.Token = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.Value.ClientId, azureOptions.Value.ClientSecret,
                _context.Config.Services.TestApiResourceId);
            _context.Token.Should().NotBeNullOrEmpty();

            Zap.SetAuthToken(_context.Token);
        }
    }
}