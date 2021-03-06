﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Contract.Dtos;
using TestApi.Contract.Responses;
using TestApi.Contract.Enums;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Allocations
{
    public class AllocateUsersTests : ControllerTestsBase
    {
        [Test]
        public async Task Should_allocate_multiple_users_no_users_exist()
        {
            var userTypes = new List<UserType>()
            {
                UserType.Judge, 
                UserType.Individual, 
                UserType.Representative, 
                UserType.Observer, 
                UserType.PanelMember, 
                UserType.Winger, 
                UserType.CaseAdmin, 
                UserType.Witness, 
                UserType.Interpreter
            };

            var request = new AllocateUsersRequestBuilder()
                .WithUserTypes(userTypes)
                .ForApplication(Application.TestApi)
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
            var judgeUser = await Context.Data.SeedUser();
            var individualUser = await Context.Data.SeedUser(UserType.Individual);
            var representativeUser = await Context.Data.SeedUser(UserType.Representative);
            var caseAdminUser = await Context.Data.SeedUser(UserType.CaseAdmin);
            var users = new List<UserDto>(){ judgeUser, individualUser, representativeUser, caseAdminUser};

            var userTypes = new List<UserType>()
            {
                judgeUser.UserType, individualUser.UserType, representativeUser.UserType, caseAdminUser.UserType
            };

            var request = new AllocateUsersRequestBuilder()
                .WithUserTypes(userTypes)
                .ForApplication(Application.TestApi)
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

        [TestCase(TestType.Demo)]
        [TestCase(TestType.ITHC)]
        [TestCase(TestType.Manual)]
        [TestCase(TestType.Performance)]
        public async Task Should_allocate_multiple_users_allocated_users_exist_with_other_test_types(TestType testType)
        {
            const Application APPLICATION = Application.TestApi;

            var judgeUser = await Context.Data.SeedUser(testType);
            await Context.Data.SeedAllocation(judgeUser.Id);
            await Context.Data.AllocateUser(judgeUser.Id);

            var individualUser = await Context.Data.SeedUser(testType, UserType.Individual);
            await Context.Data.SeedAllocation(individualUser.Id);
            await Context.Data.AllocateUser(individualUser.Id);

            var representativeUser = await Context.Data.SeedUser(testType, UserType.Representative);
            await Context.Data.SeedAllocation(representativeUser.Id);
            await Context.Data.AllocateUser(representativeUser.Id);

            var caseAdminUser = await Context.Data.SeedUser(testType, UserType.CaseAdmin);
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

        [Test]
        public async Task Should_allocate_multiple_joh_ejud_users_and_none_ejud()
        {
            const Application APPLICATION = Application.TestApi;

            var individualUser = await Context.Data.SeedUser(UserType.Individual);
            await Context.Data.SeedAllocation(individualUser.Id);

            var representativeUser = await Context.Data.SeedUser(UserType.Representative);
            await Context.Data.SeedAllocation(representativeUser.Id);

            var userTypes = new List<UserType>()
            {
                UserType.Judge, UserType.PanelMember, UserType.Winger, individualUser.UserType, representativeUser.UserType
            };

            var request = new AllocateUsersRequestBuilder()
                .WithUserTypes(userTypes)
                .ForApplication(APPLICATION)
                .IsEjud()
                .Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateUsers;

            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<UserDetailsResponse>>(Json);

            response.Should().NotBeNull();

            var allocateJudge = response.Single(x => x.UserType == UserType.Judge);
            var allocatedPanelMember = response.Single(x => x.UserType == UserType.PanelMember);
            var allocatedWinger = response.Single(x => x.UserType == UserType.Winger);
            var allocatedIndividual = response.Single(x => x.UserType == UserType.Individual);
            var allocatedRepresentative = response.Single(x => x.UserType == UserType.Representative);

            allocateJudge.ContactEmail.Should().EndWith(Context.Config.EjudUsernameStem);
            allocateJudge.ContactEmail.Should().NotEndWith(Context.Config.UsernameStem);
            allocateJudge.Username.Should().EndWith(Context.Config.EjudUsernameStem);
            allocateJudge.Username.Should().NotEndWith(Context.Config.UsernameStem);

            allocatedPanelMember.ContactEmail.Should().EndWith(Context.Config.EjudUsernameStem);
            allocatedPanelMember.ContactEmail.Should().NotEndWith(Context.Config.UsernameStem);
            allocatedPanelMember.Username.Should().EndWith(Context.Config.EjudUsernameStem);
            allocatedPanelMember.Username.Should().NotEndWith(Context.Config.UsernameStem);

            allocatedWinger.ContactEmail.Should().EndWith(Context.Config.EjudUsernameStem);
            allocatedWinger.ContactEmail.Should().NotEndWith(Context.Config.UsernameStem);
            allocatedWinger.Username.Should().EndWith(Context.Config.EjudUsernameStem);
            allocatedWinger.Username.Should().NotEndWith(Context.Config.UsernameStem);

            allocatedIndividual.ContactEmail.Should().NotEndWith(Context.Config.EjudUsernameStem);
            allocatedIndividual.Username.Should().EndWith(Context.Config.UsernameStem);
            allocatedIndividual.Username.Should().NotEndWith(Context.Config.EjudUsernameStem);

            allocatedRepresentative.ContactEmail.Should().NotEndWith(Context.Config.EjudUsernameStem);
            allocatedRepresentative.Username.Should().EndWith(Context.Config.UsernameStem);
            allocatedRepresentative.Username.Should().NotEndWith(Context.Config.EjudUsernameStem);
        }
    }
}
