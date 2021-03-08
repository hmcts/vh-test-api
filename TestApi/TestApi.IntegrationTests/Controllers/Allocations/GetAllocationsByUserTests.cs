using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Contract.Responses;
using TestApi.Contract.Enums;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Allocations
{
    public class GetAllocationsByUserTests : ControllerTestsBase
    {
        [Test]
        public async Task Should_retrieve_allocations_by_user()
        {
            const string ALLOCATED_BY = EmailData.TEST_WEB_MANUAL_USER;
            var request = new AllocateUserRequestBuilder().WithUserType(UserType.Judge).WithAllocatedBy(ALLOCATED_BY).Build();

            var uri = ApiUriFactory.AllocationEndpoints.AllocateSingleUser;
            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);

            uri = ApiUriFactory.AllocationEndpoints.GetAllocatedUsers(ALLOCATED_BY);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var allocations = RequestHelper.Deserialise<List<AllocationDetailsResponse>>(Json);
            allocations.Should().NotBeEmpty();

            foreach (var allocation in allocations)
            {
                allocation.Allocated.Should().BeTrue();
                allocation.AllocatedBy.Should().Be(ALLOCATED_BY);
                allocation.ExpiresAt.Should().HaveValue();
                allocation.Id.Should().NotBeEmpty();
                allocation.UserId.Should().NotBeEmpty();
                allocation.Username.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Test]
        public async Task Should_not_return_error_with_an_empty_list()
        {
            const string ALLOCATED_BY = EmailData.TEST_WEB_MANUAL_USER;

            var uri = ApiUriFactory.AllocationEndpoints.GetAllocatedUsers(ALLOCATED_BY);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var allocations = RequestHelper.Deserialise<List<AllocationDetailsResponse>>(Json);
            allocations.Should().BeEmpty();
        }
    }
}
