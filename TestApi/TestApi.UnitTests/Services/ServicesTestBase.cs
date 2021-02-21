using System;
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
using TestApi.Services.Services;

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
        protected readonly UserApiException NotFoundError = new UserApiException("User not found", 404, "Response",
            new Dictionary<string, IEnumerable<string>>(), new Exception("Message"));
        protected readonly UserApiException InternalServerError = new UserApiException("Internal server error", 500, "Response",
            new Dictionary<string, IEnumerable<string>>(), new Exception("Message"));

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
            UserApiService = new TestApi.Services.Services.UserApiService(UserApiClient.Object, GroupsConfig.Object);
            AllocationService = new AllocationService(CommandHandler.Object, QueryHandler.Object, Logger.Object,
                Configuration.Object, MockUserApiService.Object);
        }

        private void SetMockGroups()
        {
            var groups = new UserGroupsConfiguration
            {
                JudgeGroups = GroupData.FAKE_JUDGE_GROUPS,
                IndividualGroups = GroupData.FAKE_INDIVIDUAL_GROUPS,
                RepresentativeGroups = GroupData.FAKE_REPRESENTATIVE_GROUPS,
                VideoHearingsOfficerGroups = GroupData.FAKE_VIDEO_HEARINGS_OFFICER_GROUPS,
                CaseAdminGroups = GroupData.FAKE_CASE_ADMIN_GROUPS,
                KinlyGroups = GroupData.FAKE_KINLY_GROUPS,
                KinlyProdGroups = GroupData.FAKE_KINLY_PROD_GROUPS,
                TestAccountGroups = GroupData.FAKE_TEST_GROUPS,
                TestWebGroups = GroupData.FAKE_TEST_WEB_GROUPS,
                PerformanceTestAccountGroups = GroupData.FAKE_PERFORMANCE_TEST_GROUPS,
                JudicialOfficeGroups = GroupData.FAKE_JOH_GROUPS,
                WitnessGroups = GroupData.FAKE_WITNESS_GROUPS,
            };

            GroupsConfig
                .Setup(x => x.Value)
                .Returns(groups);
        }

        private void SetMockConfig()
        {
            Configuration
                .Setup(x => x.GetSection("UsernameStem").Value)
                .Returns(EmailData.FAKE_EMAIL_STEM);
        }

        protected User CreateNewUser(UserType userType, int number, bool isProdUser = false)
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;
            return new UserBuilder(EMAIL_STEM, number)
                .WithUserType(userType)
                .ForApplication(Application.TestApi)
                .IsProdUser(isProdUser)
                .BuildUser();
        }

        protected User CreateNewUser(TestType testType, int number)
        {
            const string EMAIL_STEM = EmailData.FAKE_EMAIL_STEM;
            return new UserBuilder(EMAIL_STEM, number)
                .WithUserType(UserType.Individual)
                .ForApplication(Application.TestApi)
                .ForTestType(testType)
                .BuildUser();
        }

        protected Allocation CreateAllocation(User user)
        {
            return new Allocation(user);
        }

        protected List<User> CreateListOfUsers(UserType userType, int size, bool isProdUser = false)
        {
            var users = new List<User>();

            for (var i = 1; i <= size; i++) users.Add(CreateNewUser(userType, i, isProdUser));

            return users;
        }

        protected List<Allocation> CreateAllocations(List<User> users)
        {
            return users.Select(CreateAllocation).ToList();
        }

        protected void AllocateAllUsers(List<Allocation> allocations)
        {
            const int ALLOCATE_FOR_MINUTES = 1;
            foreach (var allocation in allocations) allocation.Allocate(ALLOCATE_FOR_MINUTES, EmailData.TEST_WEB_MANUAL_USER);
        }
    }
}