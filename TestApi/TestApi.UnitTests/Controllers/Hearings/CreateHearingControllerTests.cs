using System;
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
using TestApi.Services.Builders.Requests;
using TestApi.Services.Builders.Responses;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class CreateHearingControllerTests : HearingsControllerTestsBase
    {
        [Test]
        public async Task Should_create_hearing()
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
                .ThrowsAsync(CreateBookingsApiException("Hearing not created", HttpStatusCode.BadRequest));

            var response = await Controller.CreateHearingAsync(createHearingRequest);

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Should_create_hearing_with_audio_recording()
        {
            var firstUser = CreateUser(UserType.Judge);
            var secondUser = CreateUser(UserType.Individual);
            var thirdUser = CreateUser(UserType.Representative);
            var fourthUser = CreateUser(UserType.CaseAdmin);

            var users = new List<User> { firstUser, secondUser, thirdUser, fourthUser };

            var createHearingRequest = new HearingBuilder(users)
                .AudioRecordingRequired()
                .BuildRequest();

            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearingAsync(createHearingRequest);

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse)result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(hearingDetailsResponse);
        }

        [Test]
        public async Task Should_create_hearing_with_questionnaire_required()
        {
            var firstUser = CreateUser(UserType.Judge);
            var secondUser = CreateUser(UserType.Individual);
            var thirdUser = CreateUser(UserType.Representative);
            var fourthUser = CreateUser(UserType.CaseAdmin);

            var users = new List<User> { firstUser, secondUser, thirdUser, fourthUser };

            var createHearingRequest = new HearingBuilder(users)
                .QuestionnairesRequired()
                .BuildRequest();

            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearingAsync(createHearingRequest);

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse)result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(hearingDetailsResponse);
        }

        [Test]
        public async Task Should_create_hearing_with_specific_venue()
        {
            var firstUser = CreateUser(UserType.Judge);
            var secondUser = CreateUser(UserType.Individual);
            var thirdUser = CreateUser(UserType.Representative);
            var fourthUser = CreateUser(UserType.CaseAdmin);

            var users = new List<User> { firstUser, secondUser, thirdUser, fourthUser };

            var createHearingRequest = new HearingBuilder(users)
                .HearingVenue("Manchester Civil and Family Justice Centre")
                .BuildRequest();

            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearingAsync(createHearingRequest);

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse)result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(hearingDetailsResponse);
        }

        [Test]
        public async Task Should_create_hearing_with_specified_data_time()
        {
            var firstUser = CreateUser(UserType.Judge);
            var secondUser = CreateUser(UserType.Individual);
            var thirdUser = CreateUser(UserType.Representative);
            var fourthUser = CreateUser(UserType.CaseAdmin);

            var users = new List<User> { firstUser, secondUser, thirdUser, fourthUser };

            var createHearingRequest = new HearingBuilder(users)
                .ScheduledDateTime(DateTime.UtcNow.AddMinutes(1))
                .BuildRequest();

            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearingAsync(createHearingRequest);

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse)result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(hearingDetailsResponse);
        }
    }
}