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
using TestApi.IntegrationTests.Configuration;

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
            _context.Tokens = new Tokens();
        }

        public TestContext RegisterSecrets()
        {
            var azureOptions = RegisterAzureSecrets();
            RegisterDatabaseSettings();
            RegisterUsernameStem();
            RegisterServer();
            GenerateBearerTokens(azureOptions);
            return _context;
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
                .UseStartup<Startup>();
            _context.Server = new TestServer(webHostBuilder);
        }

        private void GenerateBearerTokens(IOptions<AzureAdConfiguration> azureOptions)
        {
            _context.Tokens.TestApiBearerToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.Value.ClientId, azureOptions.Value.ClientSecret,
                azureOptions.Value.ValidAudience);
            _context.Tokens.TestApiBearerToken.Should().NotBeNullOrEmpty();
        }
    }
}