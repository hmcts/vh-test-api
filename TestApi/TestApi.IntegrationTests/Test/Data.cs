using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Common.Extensions;
using TestApi.Common.Mappers;
using TestApi.Contract.Dtos;
using TestApi.DAL;
using TestApi.Domain;
using TestApi.Contract.Enums;

namespace TestApi.IntegrationTests.Test
{
    public class Data
    {
        private readonly TestContext _context;
        private readonly DbContextOptions _dbContextOptions;

        public Data(TestContext context, DbContextOptions dbContextOptions)
        {
            _context = context;
            _dbContextOptions = dbContextOptions;
        }

        public async Task<UserDto> SeedUser(UserType userType = UserType.Judge, bool isProdUser = false)
        {
            await using var db = new TestApiDbContext(_dbContextOptions);

            const Application application = Application.TestApi;
            const TestType testType = TestType.Automated;

            var number = await IterateUserNumber(userType, application, isProdUser, testType);

            var user = new UserBuilder(_context.Config.UsernameStem, number)
                .WithUserType(userType)
                .ForApplication(application)
                .IsProdUser(isProdUser)
                .BuildUser();

            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return UserToUserDtoMapper.Map(user);
        }

        public async Task<UserDto> SeedUser(TestType testType, UserType userType = UserType.Judge)
        {
            await using var db = new TestApiDbContext(_dbContextOptions);

            const Application APPLICATION = Application.TestApi;
            const bool IS_PROD_USER = false;

            var number = await IterateUserNumber(userType, APPLICATION, IS_PROD_USER, testType);

            var user = new UserBuilder(_context.Config.UsernameStem, number)
                .WithUserType(userType)
                .ForApplication(APPLICATION)
                .ForTestType(testType)
                .BuildUser();

            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return UserToUserDtoMapper.Map(user);
        }

        public async Task<Allocation> SeedAllocation(Guid userId)
        {
            await using var db = new TestApiDbContext(_dbContextOptions);

            var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == userId);
            var allocation = new Allocation(user);
            await db.Allocations.AddAsync(allocation);
            await db.SaveChangesAsync();
            return allocation;
        }

        public async Task<RecentUser> SeedRecentUser(string username)
        {
            await using var db = new TestApiDbContext(_dbContextOptions);
            var recentUser = new RecentUser(username);
            await db.RecentUsers.AddAsync(recentUser);
            await db.SaveChangesAsync();
            return recentUser;
        }

        public async Task<int> IterateUserNumber(UserType userType, Application application, bool isProdUser, TestType testType)
        {
            await using var db = new TestApiDbContext(_dbContextOptions);

            List<User> users;

            if (testType == TestType.Automated)
            {
                users = await db.Users
                    .Where(x => x.UserType == userType.MapToContractEnum() && x.Application == application.MapToContractEnum() && x.IsProdUser == isProdUser)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
                users = await db.Users
                    .Where(x => x.UserType == userType.MapToContractEnum() && x.IsProdUser == isProdUser)
                    .AsNoTracking()
                    .ToListAsync();
            }

            if (users.Count.Equals(0)) return 1;

            return users.Select(user => user.Number).ToList().Max() + 1;
        }

        public async Task<Allocation> AllocateUser(Guid userId, int expiresInMinutes = 1)
        {
            await using var db = new TestApiDbContext(_dbContextOptions);

            var allocation = await db.Allocations
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            allocation.Allocate(expiresInMinutes);
            db.Allocations.Update(allocation);
            await db.SaveChangesAsync();
            return allocation;
        }

        public async Task DeleteUsers()
        {
            await using var db = new TestApiDbContext(_dbContextOptions);

            var users = await db.Users
                .Where(x => x.Application == Application.TestApi.MapToContractEnum())
                .AsNoTracking()
                .ToListAsync();

            foreach (var user in users)
            {
                db.Remove(user);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteRecentUsers()
        {
            await using var db = new TestApiDbContext(_dbContextOptions);

            var recentUsers = await db.RecentUsers
                .Where(x => x.Username == EmailData.RECENT_USER_USERNAME)
                .AsNoTracking()
                .ToListAsync();

            foreach (var user in recentUsers)
            {
                db.Remove(user);
                await db.SaveChangesAsync();
            }
        }

        public async Task<Allocation> GetAllocationByUserId(Guid userId)
        {
            await using var db = new TestApiDbContext(_dbContextOptions);

            return await db.Allocations
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public async Task<User> GetUserById(Guid userId)
        {
            await using var db = new TestApiDbContext(_dbContextOptions);

            return await db.Users
                .Where(x => x.Id == userId)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public async Task<RecentUser> GetRecentUserByUsername(string username)
        {
            await using var db = new TestApiDbContext(_dbContextOptions);

            return await db.RecentUsers
                .Where(x => x.Username == username)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }
    }
}