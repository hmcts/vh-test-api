using System.Threading.Tasks;
using NUnit.Framework;
using TestApi.DAL;
using TestApi.IntegrationTests.Controllers;
using TestApi.IntegrationTests.Test;
using TestContext = TestApi.IntegrationTests.Test.TestContext;

namespace TestApi.IntegrationTests.Database
{
    public class DatabaseTestsBase
    {
        protected TestContext Context;
        protected readonly TestApiDbContext DbContext;

        protected DatabaseTestsBase()
        {
            Context = new Setup().RegisterSecrets();
            DbContext = new TestApiDbContext(Context.DbContextOptions);
        }

        [TearDown]
        public async Task AfterEveryTest()
        {
            await Context.Data.DeleteUsers();
        }

        [OneTimeTearDown]
        public void AfterTestRun()
        {
            Context.Server.Dispose();
        }
    }
}