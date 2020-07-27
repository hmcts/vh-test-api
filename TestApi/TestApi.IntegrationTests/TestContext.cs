using System.Net.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL;
using TestApi.IntegrationTests.Configuration;
using TestApi.IntegrationTests.Data;

namespace TestApi.IntegrationTests
{
    public class TestContext
    {
        public Config Config { get; set; }
        public DbContextOptions<TestApiDbContext> DbContextOptions { get; set; }
        public HttpContent HttpContent { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public HttpResponseMessage Response { get; set; }
        public TestServer Server { get; set; }
        public TestData Test { get; set; }
        public TestDataManager TestDataManager { get; set; }
        public Tokens Tokens { get; set; }
        public string Uri { get; set; }
    }
}
