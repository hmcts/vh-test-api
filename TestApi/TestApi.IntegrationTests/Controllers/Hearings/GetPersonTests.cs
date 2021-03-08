using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Domain.Helpers;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class GetPersonTests : HearingsTestsBase
    {
        [Test]
        public async Task Should_get_person()
        {
            var hearingRequest = CreateHearingRequest();
            var hearingResponse = await CreateHearing(hearingRequest);
            var individual = hearingResponse.Participants.First(x => x.UserRoleName == UserTypeName.Individual.Name);

            var uri = ApiUriFactory.HearingEndpoints.GetPersonByUsername(individual.Username);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<PersonResponse>(Json);

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(individual,
                options => options.ExcludingMissingMembers().Excluding(x => x.Id));
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_username()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;
            var uri = ApiUriFactory.HearingEndpoints.GetPersonByUsername(USERNAME);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.NotFound, false);
        }
    }
}
