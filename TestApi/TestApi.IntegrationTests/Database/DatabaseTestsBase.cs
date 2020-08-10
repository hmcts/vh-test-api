using System.Threading.Tasks;
using NUnit.Framework;
using TestApi.DAL;
using TestApi.IntegrationTests.Hooks;

namespace TestApi.IntegrationTests.Database
{
    public class DatabaseTestsBase
    {
        protected readonly TestContext _context;
        protected readonly TestApiDbContext _dbContext;

        protected DatabaseTestsBase()
        {
            _context = new TestContext();
            var configHooks = new ConfigHooks(_context);
            configHooks.RegisterSecrets(_context);
            _dbContext = new TestApiDbContext(_context.DbContextOptions);
        }

        [TearDown]
        protected async Task TearDown()
        {
            await RemoveData.RemoveDataCreatedDuringTest(_context);
            RemoveData.RemoveServer(_context);
        }
    }
}
