using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Builders.Requests;
using TestApi.Services.Builders.Responses;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.UnitTests.Controllers.Conferences
{
    public class CreateConferenceControllerTests : ConferencesControllerTestsBase
    {
        [TestCase(TestType.Automated)]
        [TestCase(TestType.Manual)]
        [TestCase(TestType.Performance)]
        public async Task Should_create_conference(TestType testType)
        {
            var firstUser = CreateUser(UserType.Judge);
            var secondUser = CreateUser(UserType.Individual);
            var thirdUser = CreateUser(UserType.Representative);
            var fourthUser = CreateUser(UserType.CaseAdmin);

            var users = new List<User> { firstUser, secondUser, thirdUser, fourthUser };

            var request = new BookConferenceRequestBuilder(users, testType).BuildRequest();

            var conferenceDetailsResponse = new ConferenceDetailsResponseBuilder(request).BuildResponse();

            VideoApiClient
                .Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()))
                .ReturnsAsync(conferenceDetailsResponse);

            var response = await Controller.BookNewConferenceAsync(request);

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            var hearingDetails = (ConferenceDetailsResponse)result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(conferenceDetailsResponse);
        }

        [Test]
        public async Task Should_throw_bad_request_with_invalid_request()
        {
            var firstUser = CreateUser(UserType.Judge);
            var secondUser = CreateUser(UserType.Individual);
            var thirdUser = CreateUser(UserType.Representative);
            var fourthUser = CreateUser(UserType.CaseAdmin);

            var users = new List<User> { firstUser, secondUser, thirdUser, fourthUser };

            var request = new BookConferenceRequestBuilder(users, TestType.Automated).BuildRequest();

            VideoApiClient
                .Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()))
                .ThrowsAsync(CreateVideoApiException("Conference details missing", HttpStatusCode.BadRequest));

            var response = await Controller.BookNewConferenceAsync(request);

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}
