using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
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
        public string Token { get; set; }
    }
}