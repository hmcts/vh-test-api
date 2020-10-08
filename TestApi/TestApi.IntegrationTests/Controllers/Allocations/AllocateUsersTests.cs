using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Contract.Responses;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Allocations
{
    public class AllocateUsersTests : ControllerTestsBase
    {
        [Test]
        public async Task Should_allocate_multiple_users_no_users_exist()
        {
            const UserType JUDGE_USER = UserType.Judge;
            const UserType INDIVIDUAL_USER = UserType.Individual;
            const UserType REPRESENTATIVE_USER = UserType.Representative;
            const UserType OBSERVER_USER = UserType.Observer;
            const UserType PANEL_MEMBER_USER = UserType.PanelMember;
            const UserType WINGER_USER = UserType.Winger;
            const UserType CASE_ADMIN_USER = UserType.CaseAdmin;

            const Application APPLICATION = Application.TestApi;

            var userTypes = new List<UserType>()
            {
                JUDGE_USER, INDIVIDUAL_USER, REPRESENTATIVE_USER, OBSERVER_USER, PANEL_MEMBER_USER, WINGER_USER, CASE_ADMIN_USER
            };

            var request = new AllocateUsersRequestBuilder()
                .WithUserTypes(userTypes)
                .ForApplication(APPLICATION)
                .Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateUsers;

            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<UserDetailsResponse>>(Json);

            response.Should().NotBeNull();
            Verify.UsersDetailsResponse(response, userTypes);
        }

        [Test]
        public async Task Should_allocate_multiple_users_unallocated_users_exist()
        {
            const Application APPLICATION = Application.TestApi;

            var judgeUser = await Context.Data.SeedUser();
            var individualUser = await Context.Data.SeedUser(UserType.Individual);
            var representativeUser = await Context.Data.SeedUser(UserType.Representative);
            var caseAdminUser = await Context.Data.SeedUser(UserType.CaseAdmin);
            var users = new List<User>(){ judgeUser, individualUser, representativeUser, caseAdminUser};

            var userTypes = new List<UserType>()
            {
                judgeUser.UserType, individualUser.UserType, representativeUser.UserType, caseAdminUser.UserType
            };

            var request = new AllocateUsersRequestBuilder()
                .WithUserTypes(userTypes)
                .ForApplication(APPLICATION)
                .Build();
            
            var uri = ApiUriFactory.AllocationEndpoints.AllocateUsers;

            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<UserDetailsResponse>>(Json);

            response.Should().NotBeNull();
            Verify.UsersDetailsResponse(response, users);
        }

        [Test]
        public async Task Should_allocate_multiple_users_allocated_users_exist()
        {
            const Application APPLICATION = Application.TestApi;

            var judgeUser = await Context.Data.SeedUser();
            await Context.Data.SeedAllocation(judgeUser.Id);
            await Context.Data.AllocateUser(judgeUser.Id);

            var individualUser = await Context.Data.SeedUser(UserType.Individual);
            await Context.Data.SeedAllocation(individualUser.Id);
            await Context.Data.AllocateUser(individualUser.Id);

            var representativeUser = await Context.Data.SeedUser(UserType.Representative);
            await Context.Data.SeedAllocation(representativeUser.Id);
            await Context.Data.AllocateUser(representativeUser.Id);

            var caseAdminUser = await Context.Data.SeedUser(UserType.CaseAdmin);
            await Context.Data.SeedAllocation(caseAdminUser.Id);
            await Context.Data.AllocateUser(caseAdminUser.Id);

            var userTypes = new List<UserType>()
            {
                judgeUser.UserType, individualUser.UserType, representativeUser.UserType, caseAdminUser.UserType
            };

            var request = new AllocateUsersRequestBuilder()
                .WithUserTypes(userTypes)
                .ForApplication(APPLICATION)
                .Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateUsers;

            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<UserDetailsResponse>>(Json);

            response.Should().NotBeNull();
            Verify.UsersDetailsResponse(response, userTypes);
        }

        [TestCase(TestType.ITHC)]
        [TestCase(TestType.Manual)]
        [TestCase(TestType.Performance)]
        public async Task Should_allocate_multiple_users_allocated_users_exist_with_other_test_types(TestType testType)
        {
            const Application APPLICATION = Application.TestApi;

            var judgeUser = await Context.Data.SeedUser();
            await Context.Data.SeedAllocation(judgeUser.Id);
            await Context.Data.AllocateUser(judgeUser.Id);

            var individualUser = await Context.Data.SeedUser(UserType.Individual);
            await Context.Data.SeedAllocation(individualUser.Id);
            await Context.Data.AllocateUser(individualUser.Id);

            var representativeUser = await Context.Data.SeedUser(UserType.Representative);
            await Context.Data.SeedAllocation(representativeUser.Id);
            await Context.Data.AllocateUser(representativeUser.Id);

            var caseAdminUser = await Context.Data.SeedUser(UserType.CaseAdmin);
            await Context.Data.SeedAllocation(caseAdminUser.Id);
            await Context.Data.AllocateUser(caseAdminUser.Id);

            var userTypes = new List<UserType>()
            {
                judgeUser.UserType, individualUser.UserType, representativeUser.UserType, caseAdminUser.UserType
            };

            var request = new AllocateUsersRequestBuilder()
                .WithUserTypes(userTypes)
                .ForApplication(APPLICATION)
                .WithTestType(testType)
                .Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateUsers;

            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<UserDetailsResponse>>(Json);

            response.Should().NotBeNull();
            Verify.UsersDetailsResponse(response, userTypes);
        }
    }
}
