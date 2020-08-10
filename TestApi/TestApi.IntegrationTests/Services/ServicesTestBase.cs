using System.Threading.Tasks;
using NUnit.Framework;
using TestApi.DAL;
using TestApi.DAL.Commands;
using TestApi.IntegrationTests.Hooks;
using TestApi.Services.Contracts;

namespace TestApi.IntegrationTests.Services
{
    public abstract class ServicesTestBase
    {
        protected readonly TestContext _context;
        protected readonly TestApiDbContext _dbContext;

        protected ServicesTestBase()
        {
            _context = new TestContext();
            var configHooks = new ConfigHooks(_context);
            configHooks.RegisterSecrets(_context);
            _dbContext = new TestApiDbContext(_context.DbContextOptions);
        }

        //protected ServicesTestBase() : this()
        //{
        //    _allocationService = allocationService;
        //}

        //protected ServicesTestBase(IUserApiService userApiService) : this()
        //{
        //    _userApiService = userApiService;
        //}

        [TearDown]
        protected async Task TearDown()
        {
            await RemoveData.RemoveDataCreatedDuringTest(_context);
            RemoveData.RemoveServer(_context);
        }
    }


}
