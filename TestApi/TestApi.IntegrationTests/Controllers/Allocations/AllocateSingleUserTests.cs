using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Contract.Responses;
using TestApi.Domain.Enums;
using TestApi.IntegrationTests.Configuration;

namespace TestApi.IntegrationTests.Controllers.Allocations
{
    public class AllocateSingleUserTests : ControllerTestsBase
    {
        [Test]
        public async Task Should_allocate_single_user_no_users_exist()
        {
            const UserType USER_TYPE = UserType.Individual;
            const Application APPLICATION = Application.TestApi;

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser(USER_TYPE, APPLICATION);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.UserDetailsResponse(response, USER_TYPE);
        }

        [Test]
        public async Task Should_allocate_single_user_one_unallocated_user_exists()
        {
            var user = await Context.Data.SeedUser(UserType.Individual);
            await Context.Data.SeedAllocation(user.Id);

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser(user.UserType, user.Application);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.UserDetailsResponse(response, user);
        }

        [Test]
        public async Task Should_allocate_single_user_one_allocated_user_exists()
        {
            const UserType USER_TYPE = UserType.Individual;
            var user = await Context.Data.SeedUser(USER_TYPE);
            await Context.Data.SeedAllocation(user.Id);
            await Context.Data.AllocateUser(user.Id);

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser(user.UserType, user.Application);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            response.Should().NotBeEquivalentTo(user);
            Verify.UserDetailsResponse(response, USER_TYPE);
        }
    }
}
