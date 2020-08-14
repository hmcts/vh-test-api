using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Builders;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class CreateHearingControllerTests : HearingsControllerTestBase
    {
        [Test]
        public async Task Should_return_hearing()
        {
            var firstUser = CreateUser(UserType.Judge);
            var secondUser = CreateUser(UserType.Individual);
            var thirdUser = CreateUser(UserType.Representative);
            var fourthUser = CreateUser(UserType.CaseAdmin);

            var users = new List<User> {firstUser, secondUser, thirdUser, fourthUser};

            var createHearingRequest = new HearingBuilder(users).BuildRequest();
            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearingAsync(createHearingRequest);

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse) result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(hearingDetailsResponse);
        }

        [Test]
        public async Task Should_return_bad_request_with_invalid_request()
        {
            var firstUser = CreateUser(UserType.Judge);
            var secondUser = CreateUser(UserType.Individual);
            var thirdUser = CreateUser(UserType.Representative);
            var fourthUser = CreateUser(UserType.CaseAdmin);

            var users = new List<User> {firstUser, secondUser, thirdUser, fourthUser};

            var createHearingRequest = new HearingBuilder(users).BuildRequest();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ThrowsAsync(GetBookingsApiException("Hearing not created", HttpStatusCode.BadRequest));

            var response = await Controller.CreateHearingAsync(createHearingRequest);

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }
    }
}