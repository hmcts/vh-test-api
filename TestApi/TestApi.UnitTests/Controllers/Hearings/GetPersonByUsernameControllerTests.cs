using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;

namespace TestApi.UnitTests.Controllers.Hearings
{
    public class GetPersonByUsernameControllerTests : HearingsControllerTestsBase
    {
        [Test]
        public async Task Should_get_person()
        {
            var hearingDetailsResponse = CreateHearingDetailsResponse();
            var individual = hearingDetailsResponse.Participants.First(x => x.UserRoleName.Equals("Individual"));

            var personResponse = new PersonResponse()
            {
                ContactEmail = individual.ContactEmail,
                FirstName = individual.FirstName,
                Id = Guid.NewGuid(),
                LastName = individual.LastName,
                MiddleNames = individual.MiddleNames,
                Organisation = individual.Organisation,
                TelephoneNumber = individual.TelephoneNumber,
                Title = individual.Title,
                Username = individual.Username
            };

            BookingsApiClient
                .Setup(x => x.GetPersonByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(personResponse);

            var response = await Controller.GetPersonByUsername(individual.Username);
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

            var response = await Controller.GetPersonByUsername(USERNAME);
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
