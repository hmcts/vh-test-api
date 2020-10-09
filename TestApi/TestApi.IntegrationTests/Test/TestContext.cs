using AcceptanceTests.Common.Api;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using TestApi.DAL;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Test
{
    public class TestContext
    {
        public Config Config { get; set; }
        public DbContextOptions<TestApiDbContext> DbContextOptions { get; set; }
        public TestServer Server { get; set; }
        public Data Data { get; set; }
        public Tokens Tokens { get; set; }

        public HttpClient CreateClient()
        {
            HttpClient client;
            if (Zap.SetupProxy)
            {
                var handler = new HttpClientHandler
                {
                    Proxy = Zap.WebProxy,
                    UseProxy = true,
                };

                Server.BaseAddress = new System.Uri(Config.Services.TestApiUrl);

                client = new HttpClient(handler)
                {
                    BaseAddress = Server.BaseAddress
                };
            }
            else
            {
                client = Server.CreateClient();
            }

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Tokens.TestApiBearerToken}");
            return client;
        }
    }
}