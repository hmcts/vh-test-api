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
        [TestCase(UserType.Judge)]
        [TestCase(UserType.Individual)]
        [TestCase(UserType.Representative)]
        [TestCase(UserType.Observer)]
        [TestCase(UserType.PanelMember)]
        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.VideoHearingsOfficer)]
        public async Task Should_allocate_single_user_no_users_exist(UserType userType)
        {
            const Application APPLICATION = Application.TestApi;

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser(userType, APPLICATION);
            await SendGetRequest(uri);

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
        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.VideoHearingsOfficer)]
        public async Task Should_allocate_single_user_one_unallocated_user_exists(UserType userType)
        {
            var user = await Context.Data.SeedUser(userType);
            await Context.Data.SeedAllocation(user.Id);

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser(user.UserType, user.Application);
            await SendGetRequest(uri);

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
        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.VideoHearingsOfficer)]
        public async Task Should_allocate_single_user_one_allocated_user_exists(UserType userType)
        {
            var user = await Context.Data.SeedUser(userType);
            await Context.Data.SeedAllocation(user.Id);
            await Context.Data.AllocateUser(user.Id);

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser(user.UserType, user.Application);
            await SendGetRequest(uri);

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
        [TestCase(UserType.CaseAdmin)]
        [TestCase(UserType.VideoHearingsOfficer)]
        public async Task Should_allocate_single_user_allocated_user_expired(UserType userType)
        {
            var user = await Context.Data.SeedUser(userType);
            await Context.Data.SeedAllocation(user.Id);
            await Context.Data.AllocateUser(user.Id, -11);

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser(user.UserType, user.Application);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<UserDetailsResponse>(Json);

            response.Should().NotBeNull();
            Verify.UserDetailsResponse(response, user);
        }
    }
}
