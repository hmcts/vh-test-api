using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Services.Clients.BookingsApiClient;

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
                            Extended_answer = "ExtendedAnswer"
                        }
                    },
                    Hearing_id = Guid.NewGuid(),
                    Questionnaire_not_required = false,
                    Participant_id = Guid.NewGuid(),
                    Scheduled_at = DateTime.UtcNow,
                    Updated_at = DateTime.UtcNow
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
    }
}
