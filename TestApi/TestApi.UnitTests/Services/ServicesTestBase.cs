using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Configuration;
using TestApi.Common.Data;
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
        protected IAllocationService AllocationService;
        protected Mock<ICommandHandler> CommandHandler;
        protected Mock<IConfiguration> Configuration;
        protected Mock<IOptions<UserGroupsConfiguration>> GroupsConfig;
        protected Mock<ILogger<AllocationService>> Logger;
        protected Mock<IUserApiService> MockUserApiService;
        protected Mock<IQueryHandler> QueryHandler;
        protected Mock<IUserApiClient> UserApiClient;
        protected IUserApiService UserApiService;

        [SetUp]
        public void Setup()
        {
            CommandHandler = new Mock<ICommandHandler>();
            QueryHandler = new Mock<IQueryHandler>();
            Logger = new Mock<ILogger<AllocationService>>();
            Configuration = new Mock<IConfiguration>();
            GroupsConfig = new Mock<IOptions<UserGroupsConfiguration>>();
            SetMockGroups();
            SetMockConfig();
            MockUserApiService = new Mock<IUserApiService>();
            UserApiClient = new Mock<IUserApiClient>();
            UserApiService = new UserApiService(UserApiClient.Object, GroupsConfig.Object);
            AllocationService = new AllocationService(CommandHandler.Object, QueryHandler.Object, Logger.Object,
                Configuration.Object, MockUserApiService.Object);
        }

        private void SetMockGroups()
        {
            var groups = new UserGroupsConfiguration
            {
                JudgeGroups = new List<string> { DefaultData.FAKE_JUDGE_GROUP_1, DefaultData.FAKE_JUDGE_GROUP_2 },
                IndividualGroups = new List<string> { DefaultData.FAKE_INDIVIDUAL_GROUP_1, DefaultData.FAKE_INDIVIDUAL_GROUP_2 },
                RepresentativeGroups = new List<string> { DefaultData.FAKE_REPRESENTATIVE_GROUP_1, DefaultData.FAKE_REPRESENTATIVE_GROUP_2 },
                VideoHearingsOfficerGroups = new List<string> { DefaultData.FAKE_VIDEO_HEARINGS_OFFICER_GROUP_1, DefaultData.FAKE_VIDEO_HEARINGS_OFFICER_GROUP_2 },
                CaseAdminGroups = new List<string> { DefaultData.FAKE_CASE_ADMIN_GROUP_1, DefaultData.FAKE_CASE_ADMIN_GROUP_2 },
                KinlyGroups = new List<string> { DefaultData.FAKE_PEXIP_GROUP_1, DefaultData.FAKE_PEXIP_GROUP_2 },
                TestAccountGroup = DefaultData.FAKE_TEST_GROUP,
                PerformanceTestAccountGroup = DefaultData.FAKE_PERFORMANCE_TEST_GROUP
            };

            GroupsConfig
                .Setup(x => x.Value)
                .Returns(groups);
        }

        private void SetMockConfig()
        {
            Configuration
                .Setup(x => x.GetSection("UsernameStem").Value)
                .Returns(DefaultData.FAKE_EMAIL_STEM);
        }

        protected User CreateNewUser(UserType userType, int number)
        {
            const string EMAIL_STEM = DefaultData.FAKE_EMAIL_STEM;
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

            for (var i = 1; i <= size; i++) users.Add(CreateNewUser(userType, i));

            return users;
        }

        protected List<Allocation> CreateAllocations(List<User> users)
        {
            return users.Select(CreateAllocation).ToList();
        }

        protected void AllocateAllUsers(List<Allocation> allocations)
        {
            const int ALLOCATE_FOR_MINUTES = 1;
            foreach (var allocation in allocations) allocation.Allocate(ALLOCATE_FOR_MINUTES);
        }
    }
}