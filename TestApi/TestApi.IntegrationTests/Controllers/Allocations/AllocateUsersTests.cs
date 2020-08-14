using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestApi.Domain.Enums;
using TestApi.IntegrationTests.Configuration;

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
            const UserType CASE_ADMIN_USER = UserType.CaseAdmin;
            const Application APPLICATION = Application.TestApi;

            var userTypes = new List<UserType>()
            {
                JUDGE_USER, INDIVIDUAL_USER, REPRESENTATIVE_USER, CASE_ADMIN_USER
            };

            var request = new AllocateUsersRequest() {Application = APPLICATION, UserTypes = userTypes};
            var uri = ApiUriFactory.AllocationEndpoints.AllocateUsers;

            await SendPatchRequest(uri, RequestHelper.Serialise(request));

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<UserDetailsResponse>>(Json);

            response.Should().NotBeNull();
            Verify.UsersDetailsResponse(response, userTypes);
        }
    }
}
