using AcceptanceTests.Common.Api;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using TestApi.Common.Configuration;

namespace TestApi.IntegrationTests
{
    [SetUpFixture]
    public class TestSetupFixture
    {
        private ServicesConfiguration ServicesConfiguration => new ConfigurationBuilder()
                                                            .AddJsonFile("appsettings.json")
                                                            .Build()
                                                            .GetSection("Services")
                                                            .Get<ServicesConfiguration>();

        [OneTimeSetUp]
        public void ZapStart()
        {
            Zap.Start();
        }

        [OneTimeTearDown]
        public void ZapReport()
        {
            Zap.ReportAndShutDown("TestApi - Integration", ServicesConfiguration.TestApiUrl);
        }
    }
}
