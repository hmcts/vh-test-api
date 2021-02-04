using System;
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
    public class GetPersonByUsernameControllerTests : HearingsControllerTestsBase
    {
        [Test]
        public async Task Should_get_person()
        {
            var hearingDetailsResponse = CreateHearingDetailsResponse();
            var individual = hearingDetailsResponse.Participants.First(x => x.User_role_name.Equals("Individual"));

            var personResponse = new PersonResponse()
            {
                Contact_email = individual.Contact_email,
                First_name = individual.First_name,
                Id = Guid.NewGuid(),
                Last_name = individual.Last_name,
                Middle_names = individual.Middle_names,
                Organisation = individual.Organisation,
                Telephone_number = individual.Telephone_number,
                Title = individual.Title,
                Username = individual.Username
            };

            BookingsApiClient
                .Setup(x => x.GetPersonByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(personResponse);

            var response = await Controller.GetPersonByUsernameAsync(individual.Username);
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var answers = (PersonResponse) result.Value;
            answers.Should().NotBeNull();
            answers.Should().BeEquivalentTo(personResponse);
        }

        [Test]
        public async Task Should_return_not_found_for_a_non_existent_username()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            BookingsApiClient
                .Setup(x => x.GetPersonByUsernameAsync(It.IsAny<string>()))
                .ThrowsAsync(CreateBookingsApiException("Person not found", HttpStatusCode.NotFound));

            var response = await Controller.GetPersonByUsernameAsync(USERNAME);
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
