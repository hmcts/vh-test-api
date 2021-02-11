using AcceptanceTests.Common.Configuration;
using AcceptanceTests.Common.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TestApi.Common.Configuration;
using TestApi.Common.Security;

namespace TestApi.AcceptanceTests.Helpers
{
    public class Setup
    {
        private IConfigurationRoot _configRoot;
        private TestContext _context;

        public TestContext RegisterSecrets(TestContext context)
        {
            _context = context;
            _configRoot = ConfigurationManager.BuildConfig("04df59fe-66aa-4fb2-8ac5-b87656f7675a", "A5262BF7-401F-4BAE-93D8-59A0D59CFE70");
            var azure = RegisterAzureSecrets();
            RegisterServices();
            RegisterTestData();
            RegisterUsernameStem();
            GenerateBearerToken(azure);
            return context;
        }

        private IOptions<AzureAdConfiguration> RegisterAzureSecrets()
        {
            var azureOptions = Options.Create(_configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
            _context.Config.AzureAdConfiguration = azureOptions.Value;
            ConfigurationManager.VerifyConfigValuesSet(_context.Config.AzureAdConfiguration);
            return azureOptions;
        }

        private void RegisterServices()
        {
            _context.Config.Services = GetTargetTestEnvironment() == string.Empty ? Options.Create(_configRoot.GetSection("Services").Get<ServicesConfiguration>()).Value
                : Options.Create(_configRoot.GetSection($"Testing.{GetTargetTestEnvironment()}.Services").Get<ServicesConfiguration>()).Value;
            if (_context.Config.Services == null && GetTargetTestEnvironment() != string.Empty)
            {
                throw new TestSecretsFileMissingException(GetTargetTestEnvironment());
            }

            _context.Config.Services.TestApiUrl.Should().NotBeNullOrWhiteSpace();
            _context.Config.Services.TestApiResourceId.Should().NotBeNullOrWhiteSpace();
        }

        private void RegisterTestData()
        {
            _context.TestData = new TestData();
        }

        private void RegisterUsernameStem()
        {
            _context.Config.UsernameStem = _configRoot.GetValue<string>("UsernameStem");
        }

        private void GenerateBearerToken(IOptions<AzureAdConfiguration> azureOptions)
        {
            _context.Token = new AzureTokenProvider(azureOptions).GetClientAccessToken(
                azureOptions.Value.ClientId, azureOptions.Value.ClientSecret,
                _context.Config.Services.TestApiResourceId);
            _context.Token.Should().NotBeNullOrEmpty();
        }

        private static string GetTargetTestEnvironment()
        {
            return NUnit.Framework.TestContext.Parameters["TargetTestEnvironment"] ?? string.Empty;
        }
    }
}
