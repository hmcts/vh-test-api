using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestApi.Common.Builders;
using TestApi.Contract.Requests;
using TestApi.Controllers;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Builders;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Services.Contracts;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class HearingsControllerTestBase
    {
        protected Mock<IBookingsApiClient> BookingsApiClient;
        protected Mock<ICommandHandler> CommandHandler;
        protected HearingsController Controller;
        protected Mock<ILogger<HearingsController>> Logger;
        protected Mock<IQueryHandler> QueryHandler;
        protected Mock<IVideoApiService> VideoApiService;

        [SetUp]
        public void Setup()
        {
            QueryHandler = new Mock<IQueryHandler>();
            CommandHandler = new Mock<ICommandHandler>();
            Logger = new Mock<ILogger<HearingsController>>();
            BookingsApiClient = new Mock<IBookingsApiClient>();
            VideoApiService = new Mock<IVideoApiService>();
            Controller = new HearingsController(Logger.Object, BookingsApiClient.Object, VideoApiService.Object);
        }

        protected static User CreateUser(UserType userType)
        {
            const string emailStem = "made_up_email_stem";
            const int number = 1;
            return new UserBuilder(emailStem, number)
                .WithUserType(userType)
                .ForApplication(Application.TestApi)
                .BuildUser();
        }

        protected CreateHearingRequest GetCreateHearingRequest()
        {
            var firstUser = CreateUser(UserType.Judge);
            var secondUser = CreateUser(UserType.Individual);
            var thirdUser = CreateUser(UserType.Representative);
            var fourthUser = CreateUser(UserType.CaseAdmin);

            var users = new List<User> {firstUser, secondUser, thirdUser, fourthUser};

            return new HearingBuilder(users).BuildRequest();
        }

        protected HearingDetailsResponse GetHearingDetailsResponse()
        {
            var createHearingRequest = GetCreateHearingRequest();
            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            return new HearingDetailsResponseBuilder(bookHearingRequest).Build();
        }

        protected ConferenceDetailsResponse GetConferenceDetailsResponse()
        {
            var response = GetHearingDetailsResponse();
            return new ConferenceDetailsResponseBuilder(response).Build();
        }

        protected BookingsApiException GetException(string errorMessage, HttpStatusCode statusCode)
        {
            return new BookingsApiException(errorMessage, (int) statusCode, "Response",
                new Dictionary<string, IEnumerable<string>>(), new Exception("Message"));
        }
    }
}