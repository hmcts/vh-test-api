using System.Net;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Services.Builders.Requests;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common;
using TestApi.Tests.Common.Configuration;

namespace TestApi.AcceptanceTests.Controllers
{
    public class HearingTests : TestsBase
    {
        [Test]
        public void ConfirmHearing()
        {
            var hearing = CreateHearing();
            var body = new UpdateBookingRequestBuilder().UpdatedBy(UserData.DEFAULT_CREATED_BY_USER).Build();
            var uri = ApiUriFactory.HearingEndpoints.ConfirmHearing(hearing.Id);
            var request = RequestHandler.Patch(uri, body);
            var response = SendRequest(request);
            var conference = RequestHelper.Deserialise<ConferenceDetailsResponse>(response.Content);
            response.Should().NotBeNull();
            Context.TestData.Conference = conference;
            Verify.ConferenceDetailsResponse(conference, hearing);
        }

        [Test]
        public void DeleteHearing()
        {
            var hearing = CreateHearing();
            var response = DeleteHearing(hearing.Id);
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
