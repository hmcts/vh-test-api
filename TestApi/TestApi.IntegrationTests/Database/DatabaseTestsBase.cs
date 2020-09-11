using System.Threading.Tasks;
using NUnit.Framework;
using TestApi.DAL;
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
            await Context.Data.DeleteRecentUsers();
        }

        [OneTimeTearDown]
        public void AfterTestRun()
        {
            Context.Server.Dispose();
        }
    }
}