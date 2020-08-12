using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Configuration;
using TestApi.DAL.Commands;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.UserApiClient;
using TestApi.Services.Contracts;

namespace TestApi.UnitTests.Services
{
    public abstract class ServicesTestBase
    {
        protected Mock<ICommandHandler> CommandHandler;
        protected Mock<IQueryHandler> QueryHandler;
        protected Mock<ILogger<AllocationService>> Logger;
        protected Mock<Microsoft.Extensions.Configuration.IConfiguration> Configuration;
        protected Mock<IUserApiService> MockUserApiService;
        protected Mock<IUserApiClient> UserApiClient;
        protected Mock<IOptions<UserGroupsConfiguration>> GroupsConfig;
        protected IUserApiService UserApiService;
        protected IAllocationService AllocationService;

        [SetUp]
        public void Setup()
        {
            CommandHandler = new Mock<ICommandHandler>();
            QueryHandler = new Mock<IQueryHandler>();
            Logger = new Mock<ILogger<AllocationService>>();
            Configuration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            GroupsConfig = new Mock<IOptions<UserGroupsConfiguration>>();
            SetMockGroups();
            SetMockConfig();
            MockUserApiService = new Mock<IUserApiService>();
            UserApiClient = new Mock<IUserApiClient>();
            UserApiService = new UserApiService(UserApiClient.Object, GroupsConfig.Object);
            AllocationService = new AllocationService(CommandHandler.Object, QueryHandler.Object, Logger.Object, Configuration.Object, MockUserApiService.Object);
        }

        private void SetMockGroups()
        {
            var groups = new UserGroupsConfiguration()
            {
                JudgeGroups = new List<string>() {"Group1", "Group2"},
                IndividualGroups = new List<string>() {"Group1", "Group2"},
                RepresentativeGroups = new List<string>() {"Group1", "Group2"},
                VideoHearingsOfficerGroups = new List<string>() {"Group1", "Group2"},
                CaseAdminGroups = new List<string>() {"Group1", "Group2"},
                KinlyGroups = new List<string>() {"Group1", "Group2"},
                TestAccountGroup = "Group1",
                PerformanceTestAccountGroup = "Group1"
            };

            GroupsConfig
                .Setup(x => x.Value)
                .Returns(groups);
        }

        private void SetMockConfig()
        {
            Configuration
                .Setup(x => x.GetSection("UsernameStem").Value)
                .Returns("made_up_email_stem.com");
        }

        protected User CreateNewUser(UserType userType, int number)
        {
            const string EMAIL_STEM = "made_up_email_stem.com";
            return new UserBuilder(EMAIL_STEM, number)
                .WithUserType(userType)
                .ForApplication(Application.TestApi)
                .BuildUser();
        }

        protected Allocation CreateAllocation(User user)
        {
            return new Allocation(user);
        }

        protected List<User> CreateListOfUsers(UserType userType, int size)
        {
            var users = new List<User>();

            for (var i = 1; i <= size; i++)
            {
                users.Add(CreateNewUser(userType, i));
            }

            return users;
        }

        protected List<Allocation> CreateAllocations(List<User> users)
        {
            return users.Select(CreateAllocation).ToList();
        }

        protected void AllocateAllUsers(List<Allocation> allocations)
        {
            const int ALLOCATE_FOR_MINUTES = 1;
            foreach (var allocation in allocations)
            {
                allocation.Allocate(ALLOCATE_FOR_MINUTES);
            }
        }
    }
}
