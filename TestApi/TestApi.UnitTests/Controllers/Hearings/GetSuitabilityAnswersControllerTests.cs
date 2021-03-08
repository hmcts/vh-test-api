using System;
using System.Collections.Generic;
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
    public class GetSuitabilityAnswersControllerTests : HearingsControllerTestsBase
    {
        [Test]
        public async Task Should_get_suitability_answers()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            var answersResponse = new List<PersonSuitabilityAnswerResponse>()
            {
                new PersonSuitabilityAnswerResponse()
                {
                    Answers = new List<SuitabilityAnswerResponse>()
                    {
                        new SuitabilityAnswerResponse()
                        {
                            Answer =  "Answer",
                            Key = "Key",
                            ExtendedAnswer = "ExtendedAnswer"
                        }
                    },
                    HearingId = Guid.NewGuid(),
                    QuestionnaireNotRequired = false,
                    ParticipantId = Guid.NewGuid(),
                    ScheduledAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            BookingsApiClient
                .Setup(x => x.GetPersonSuitabilityAnswersAsync(It.IsAny<string>()))
                .ReturnsAsync(answersResponse);

            var response = await Controller.GetSuitabilityAnswersAsync(USERNAME);
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var answers = (List<PersonSuitabilityAnswerResponse>) result.Value;
            answers.Should().NotBeNull();
            answers.Should().BeEquivalentTo(answersResponse);
        }

        [Test]
        public async Task Should_return_empty_list_for_a_non_existent_username()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            BookingsApiClient
                .Setup(x => x.GetPersonSuitabilityAnswersAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<PersonSuitabilityAnswerResponse>());

            var response = await Controller.GetSuitabilityAnswersAsync(USERNAME);
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var answers = (List<PersonSuitabilityAnswerResponse>)result.Value;
            answers.Should().NotBeNull();
            answers.Should().BeEmpty();
        }

        [Test]
        public async Task Should_throw_api_error()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;

            BookingsApiClient
                .Setup(x => x.GetPersonSuitabilityAnswersAsync(It.IsAny<string>()))
                .ThrowsAsync(CreateBookingsApiException("Hearing error", HttpStatusCode.InternalServerError));

            var response = await Controller.GetSuitabilityAnswersAsync(USERNAME);
            response.Should().NotBeNull();

            var result = (ObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
