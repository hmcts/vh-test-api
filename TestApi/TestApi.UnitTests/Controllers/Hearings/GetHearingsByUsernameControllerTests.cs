using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class GetHearingsByUsernameControllerTests : HearingsControllerTestsBase
    {
        [Test]
        public async Task Should_return_hearing()
        {
            var hearingDetailsResponse = CreateHearingDetailsResponse();
            var listOfHearingSResponse = new List<HearingDetailsResponse> {hearingDetailsResponse};

            BookingsApiClient
                .Setup(x => x.GetHearingsByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(listOfHearingSResponse);

            var response =
                await Controller.GetHearingsByUsernameAsync(hearingDetailsResponse.Participants.First().Username);
            response.Should().NotBeNull();

            var result = (OkObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var hearingDetails = (List<HearingDetailsResponse>) result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(listOfHearingSResponse);
        }

        [Test]
        public async Task Should_return_empty_list_for_non_existent_username()
        {
            const string USERNAME = DefaultData.NON_EXISTENT_USERNAME;

            BookingsApiClient
                .Setup(x => x.GetHearingsByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HearingDetailsResponse>());

            var response = await Controller.GetHearingsByUsernameAsync(USERNAME);
            response.Should().NotBeNull();

            var result = (OkObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var hearingDetails = (List<HearingDetailsResponse>) result.Value;
            hearingDetails.Count.Should().Be(0);
        }

        [Test]
        public async Task Should_throw_error_with_empty_username()
        {
            BookingsApiClient
                .Setup(x => x.GetHearingsByUsernameAsync(string.Empty))
                .ThrowsAsync(CreateBookingsApiException("Username cannot be empty", HttpStatusCode.BadRequest));

            var response = await Controller.GetHearingsByUsernameAsync(string.Empty);
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}
