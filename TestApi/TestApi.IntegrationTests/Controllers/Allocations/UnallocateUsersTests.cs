using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestApi.Domain.Enums;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Allocations
{
    public class UnallocateUsersTests : ControllerTestsBase
    {
        [Test]
        public async Task Should_unallocate_list_of_users()
        {
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

            var usernames = new List<string>()
            {
                judgeUser.Username, individualUser.Username, representativeUser.Username, caseAdminUser.Username
            };

            var request = new UnallocateUsersRequest() { Usernames = usernames };
            var uri = ApiUriFactory.AllocationEndpoints.UnallocateUsers;

            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<AllocationDetailsResponse>>(Json);

            response.Should().NotBeNull();
            Verify.AllocationDetailsResponse(response, usernames);
        }

        [Test]
        public async Task Should_not_throw_error_if_attempting_to_unallocate_unallocated_list_of_users()
        {
            var judgeUser = await Context.Data.SeedUser();
            await Context.Data.SeedAllocation(judgeUser.Id);

            var individualUser = await Context.Data.SeedUser(UserType.Individual);
            await Context.Data.SeedAllocation(individualUser.Id);

            var representativeUser = await Context.Data.SeedUser(UserType.Representative);
            await Context.Data.SeedAllocation(representativeUser.Id);

            var caseAdminUser = await Context.Data.SeedUser(UserType.CaseAdmin);
            await Context.Data.SeedAllocation(caseAdminUser.Id);

            var usernames = new List<string>()
            {
                judgeUser.Username, individualUser.Username, representativeUser.Username, caseAdminUser.Username
            };

            var request = new UnallocateUsersRequest() { Usernames = usernames };
            var uri = ApiUriFactory.AllocationEndpoints.UnallocateUsers;

            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<AllocationDetailsResponse>>(Json);

            response.Should().NotBeNull();
            Verify.AllocationDetailsResponse(response, usernames);
        }
    }
}
