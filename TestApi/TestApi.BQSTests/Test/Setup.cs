using AcceptanceTests.Common.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TestApi.Common.Configuration;
using TestApi.Common.Security;
using TestApi.Tests.Common.Configuration;

namespace TestApi.BQSTests.Test
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
            var services = RegisterServices();
            RegisterUsernameStem();
            GenerateBearerTokens(azureOptions, services);
            return _context;
        }

        private IOptions<AzureAdConfiguration> RegisterAzureSecrets()
        {
            var azureOptions = Options.Create(_configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            _context.Config.AzureAdConfiguration = azureOptions.Value;
            ConfigurationManager.VerifyConfigValuesSet(_context.Config.AzureAdConfiguration);
            return azureOptions;
        }

        private IOptions<ServicesConfiguration> RegisterServices()
        {
            var services = Options.Create(_configRoot.GetSection("Services").Get<ServicesConfiguration>());
            _context.Config.Services = services.Value;
            return services;
        }

        private void RegisterUsernameStem()
        {
            _context.Config.UsernameStem = _configRoot.GetValue<string>("UsernameStem");
        }

        private void GenerateBearerTokens(IOptions<AzureAdConfiguration> azureOptions, IOptions<ServicesConfiguration> services)
        {
            _context.Tokens.BookingsApiBearerToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.Value.ClientId, azureOptions.Value.ClientSecret,
                services.Value.BookingsApiResourceId);

            _context.Tokens.VideoApiBearerToken = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.Value.ClientId, azureOptions.Value.ClientSecret,
                services.Value.VideoApiResourceId);

            _context.Tokens.BookingsApiBearerToken.Should().NotBeNullOrEmpty();
            _context.Tokens.VideoApiBearerToken.Should().NotBeNullOrEmpty();
        }
    }
}