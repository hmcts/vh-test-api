﻿using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Contract.Responses;
using TestApi.Contract.Enums;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Allocations
{
    public class AllocateSingleUserTests : ControllerTestsBase
    {
        [TestCase(UserType.Judge)]
        [TestCase(UserType.Individual)]
        [TestCase(UserType.Representative)]
        [TestCase(UserType.Observer)]
        [TestCase(UserType.PanelMember)]
        [TestCase(UserType.Winger)]
        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.VideoHearingsOfficer)]
        [TestCase(UserType.Tester)]
        [TestCase(UserType.Witness)]
        [TestCase(UserType.Interpreter)]
        public async Task Should_allocate_single_user_no_users_exist(UserType userType)
        {
            var request = new AllocateUserRequestBuilder().WithUserType(userType).Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser;
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.UserDetailsResponse(response, userType);
        }

        [TestCase(UserType.Judge)]
        [TestCase(UserType.Individual)]
        [TestCase(UserType.Representative)]
        [TestCase(UserType.Observer)]
        [TestCase(UserType.PanelMember)]
        [TestCase(UserType.Winger)]
        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.VideoHearingsOfficer)]
        [TestCase(UserType.Tester)]
        [TestCase(UserType.Witness)]
        [TestCase(UserType.Interpreter)]
        public async Task Should_allocate_single_user_one_unallocated_user_exists(UserType userType)
        {
            var user = await Context.Data.SeedUser(userType);
            await Context.Data.SeedAllocation(user.Id);

            var request = new AllocateUserRequestBuilder()
                .WithUserType(user.UserType)
                .ForApplication(user.Application)
                .Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser;
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.UserDetailsResponse(response, user);
        }

        [TestCase(UserType.Judge)]
        [TestCase(UserType.Individual)]
        [TestCase(UserType.Representative)]
        [TestCase(UserType.Observer)]
        [TestCase(UserType.PanelMember)]
        [TestCase(UserType.Winger)]
        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.VideoHearingsOfficer)]
        [TestCase(UserType.Tester)]
        [TestCase(UserType.Witness)]
        [TestCase(UserType.Interpreter)]
        public async Task Should_allocate_single_user_one_allocated_user_exists(UserType userType)
        {
            var user = await Context.Data.SeedUser(userType);
            await Context.Data.SeedAllocation(user.Id);
            await Context.Data.AllocateUser(user.Id);

            var request = new AllocateUserRequestBuilder()
                .WithUserType(user.UserType)
                .ForApplication(user.Application)
                .Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser;
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            response.Should().NotBeEquivalentTo(user);
            Verify.UserDetailsResponse(response, userType);
        }

        [TestCase(UserType.Judge)]
        [TestCase(UserType.Individual)]
        [TestCase(UserType.Representative)]
        [TestCase(UserType.Observer)]
        [TestCase(UserType.PanelMember)]
        [TestCase(UserType.Winger)]
        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.VideoHearingsOfficer)]
        [TestCase(UserType.Tester)]
        [TestCase(UserType.Witness)]
        [TestCase(UserType.Interpreter)]
        public async Task Should_allocate_single_user_allocated_user_expired(UserType userType)
        {
            var user = await Context.Data.SeedUser(userType);
            await Context.Data.SeedAllocation(user.Id);
            await Context.Data.AllocateUser(user.Id, -11);

            var request = new AllocateUserRequestBuilder()
                .WithUserType(user.UserType)
                .ForApplication(user.Application)
                .Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser;
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.UserDetailsResponse(response, user);
        }

        [Test]
        public async Task Should_allocate_prod_user()
        {
            const bool IS_PROD_USER = UserData.IS_PROD_USER;
            var user = await Context.Data.SeedUser(UserType.Judge, IS_PROD_USER);
            await Context.Data.SeedAllocation(user.Id);

            var request = new AllocateUserRequestBuilder()
                .WithUserType(user.UserType)
                .ForApplication(user.Application)
                .IsProdUser()
                .Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser;
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.UserDetailsResponse(response, user);
        }

        [TestCase(TestType.Automated)]
        [TestCase(TestType.Demo)]
        [TestCase(TestType.ITHC)]
        [TestCase(TestType.Manual)]
        [TestCase(TestType.Performance)]
        public async Task Should_allocate_user_with_specified_test_type(TestType testType)
        {
            var user = await Context.Data.SeedUser(testType);
            await Context.Data.SeedAllocation(user.Id);

            var request = new AllocateUserRequestBuilder()
                .WithUserType(user.UserType)
                .ForApplication(user.Application)
                .ForTestType(testType)
                .Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser;
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.UserDetailsResponse(response, user);
        }

        [TestCase(UserType.Judge)]
        [TestCase(UserType.PanelMember)]
        [TestCase(UserType.Winger)]
        public async Task Should_allocate_single_ejud_user(UserType userType)
        {
            var request = new AllocateUserRequestBuilder()
                .WithUserType(userType)
                .IsEjud()
                .Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser;
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.EjudUserDetailsResponse(response, userType);
            response.ContactEmail.Should().EndWith(Context.Config.EjudUsernameStem);
            response.ContactEmail.Should().NotEndWith(Context.Config.UsernameStem);
            response.Username.Should().EndWith(Context.Config.EjudUsernameStem);
            response.Username.Should().NotEndWith(Context.Config.UsernameStem);
        }

        [Test]
        public async Task Should_allocate_none_joh_user_with_ejud_true()
        {
            const UserType USER_TYPE = UserType.Individual;

            var request = new AllocateUserRequestBuilder()
                .WithUserType(USER_TYPE)
                .IsEjud()
                .Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser;
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.UserDetailsResponse(response, USER_TYPE);
            response.ContactEmail.Should().NotEndWith(Context.Config.EjudUsernameStem);
            response.Username.Should().EndWith(Context.Config.UsernameStem);
            response.Username.Should().NotEndWith(Context.Config.EjudUsernameStem);
        }
    }
}
