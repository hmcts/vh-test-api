using System;
using System.Collections.Generic;
using System.Net;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Builders.Requests;
using TestApi.Services.Builders.Responses;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.UnitTests.Controllers
{
    public class ControllerTestsBase
    {
        protected static User CreateUser(UserType userType)
        {
            const string emailStem = EmailData.FAKE_EMAIL_STEM;
            const int number = 1;
            return new UserBuilder(emailStem, number)
                .WithUserType(userType)
                .ForApplication(Application.TestApi)
                .BuildUser();
        }

        protected CreateHearingRequest CreateHearingRequest()
        {
            var firstUser = CreateUser(UserType.Judge);
            var secondUser = CreateUser(UserType.Individual);
            var thirdUser = CreateUser(UserType.Representative);
            var fourthUser = CreateUser(UserType.CaseAdmin);

            var users = new List<User> { firstUser, secondUser, thirdUser, fourthUser };

            return new HearingBuilder(users).Build();
        }

        protected HearingDetailsResponse CreateHearingDetailsResponse()
        {
            var createHearingRequest = CreateHearingRequest();
            var bookHearingRequest = new BookHearingRequestBuilder(createHearingRequest).Build();
            return new HearingDetailsResponseBuilder(bookHearingRequest).Build();
        }

        protected BookingsHearingResponse CreateBookingDetailsResponse()
        {
            var response = CreateHearingDetailsResponse();
            return new BookingsHearingResponseBuilder(response).Build();
        }

        protected ConferenceDetailsResponse CreateConferenceDetailsResponse()
        {
            var response = CreateHearingDetailsResponse();
            return new ConferenceDetailsFromHearingResponseBuilder(response).Build();
        }

        protected BookingsApiException CreateBookingsApiException(string errorMessage, HttpStatusCode statusCode)
        {
            return new BookingsApiException(errorMessage, (int)statusCode, "Response",
                new Dictionary<string, IEnumerable<string>>(), new Exception("Message"));
        }

        protected VideoApiException CreateVideoApiException(string errorMessage, HttpStatusCode statusCode)
        {
            return new VideoApiException(errorMessage, (int)statusCode, "Response",
                new Dictionary<string, IEnumerable<string>>(), new Exception("Message"));
        }
    }
}
