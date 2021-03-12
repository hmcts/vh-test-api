using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Contract.Dtos;
using TestApi.Contract.Enums;
using TestApi.Services.Builders.Requests;
using TestApi.Services.Builders.Responses;

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

            var users = new List<UserDto> {firstUser, secondUser, thirdUser, fourthUser};

            var createHearingRequest = new HearingBuilder(users).Build();
            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearing(createHearingRequest);

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

            var users = new List<UserDto> {firstUser, secondUser, thirdUser, fourthUser};

            var createHearingRequest = new HearingBuilder(users).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ThrowsAsync(CreateBookingsApiException("Hearing not created", HttpStatusCode.BadRequest));

            var response = await Controller.CreateHearing(createHearingRequest);

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

            var users = new List<UserDto> {firstUser, secondUser, thirdUser, fourthUser};

            var createHearingRequest = new HearingBuilder(users)
                .AudioRecordingRequired()
                .Build();

            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearing(createHearingRequest);

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse) result.Value;
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

            var users = new List<UserDto> {firstUser, secondUser, thirdUser, fourthUser};

            var createHearingRequest = new HearingBuilder(users)
                .QuestionnairesRequired()
                .Build();

            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearing(createHearingRequest);

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse) result.Value;
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

            var users = new List<UserDto> {firstUser, secondUser, thirdUser, fourthUser};

            var createHearingRequest = new HearingBuilder(users)
                .HearingVenue("Manchester Civil and Family Justice Centre")
                .Build();

            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearing(createHearingRequest);

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse) result.Value;
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

            var users = new List<UserDto> {firstUser, secondUser, thirdUser, fourthUser};

            var createHearingRequest = new HearingBuilder(users)
                .ScheduledDateTime(DateTime.UtcNow.AddMinutes(1))
                .Build();

            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearing(createHearingRequest);

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse) result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(hearingDetailsResponse);
        }

        [TestCase(TestType.Automated)]
        [TestCase(TestType.Demo)]
        [TestCase(TestType.ITHC)]
        [TestCase(TestType.Manual)]
        [TestCase(TestType.Performance)]
        public async Task Should_create_hearing_with_specified_test_type(TestType testType)
        {
            var firstUser = CreateUser(UserType.Judge);
            var secondUser = CreateUser(UserType.Individual);
            var thirdUser = CreateUser(UserType.Representative);
            var fourthUser = CreateUser(UserType.CaseAdmin);

            var users = new List<UserDto> {firstUser, secondUser, thirdUser, fourthUser};

            var createHearingRequest = new HearingBuilder(users)
                .TypeOfTest(testType)
                .Build();

            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearing(createHearingRequest);

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse) result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(hearingDetailsResponse);
        }

        [Test]
        public async Task Should_create_hearing_without_specifying_the_case_type()
        {
            var judge = CreateUser(UserType.Judge);
            var individual = CreateUser(UserType.Individual);

            var users = new List<UserDto> {judge, individual};

            var createHearingRequest = new HearingBuilder(users).WithoutACaseType().Build();
            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearing(createHearingRequest);

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse) result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(hearingDetailsResponse);
        }

        [Test]
        public async Task Should_create_hearings_for_other_applications()
        {
            var judge = CreateUser(UserType.Judge);
            var individual = CreateUser(UserType.Individual);

            var users = new List<UserDto> {judge, individual};

            var createHearingRequest = new HearingBuilder(users)
                .TypeOfTest(TestType.Automated)
                .ForApplication(Application.QueueSubscriber)
                .Build();

            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearing(createHearingRequest);

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse) result.Value;
            hearingDetails.Should().NotBeNull();
            hearingDetails.Should().BeEquivalentTo(hearingDetailsResponse);
        }

        [Test]
        public async Task Should_create_hearing_with_all_user_types()
        {
            var judge = CreateUser(UserType.Judge);
            var observer = CreateUser(UserType.Observer);
            var panelMember = CreateUser(UserType.PanelMember);
            var caseAdmin = CreateUser(UserType.CaseAdmin);
            var vho = CreateUser(UserType.VideoHearingsOfficer);
            var individual = CreateUser(UserType.Individual);

            var users = new List<UserDto> { judge, observer, panelMember, caseAdmin, vho, individual };

            var createHearingRequest = new HearingBuilder(users).Build();
            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            var hearingDetailsResponse = new HearingDetailsResponseBuilder(bookHearingRequest).Build();

            BookingsApiClient
                .Setup(x => x.BookNewHearingAsync(It.IsAny<BookNewHearingRequest>()))
                .ReturnsAsync(hearingDetailsResponse);

            var response = await Controller.CreateHearing(createHearingRequest);

            var result = (ObjectResult) response;
            result.StatusCode.Should().Be((int) HttpStatusCode.Created);

            var hearingDetails = (HearingDetailsResponse) result.Value;
            hearingDetails.Should().NotBeNull();
        }
    }
}