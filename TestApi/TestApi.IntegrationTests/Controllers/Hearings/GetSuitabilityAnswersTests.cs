using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using NUnit.Framework;
using TestApi.Common.Data;
using TestApi.Domain.Helpers;
using TestApi.Tests.Common.Configuration;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class GetSuitabilityAnswersTests : HearingsTestsBase
    {
        [Test]
        public async Task Should_get_suitability_answers()
        {
            var hearingRequest = CreateHearingRequestWithQuestionnaireEnabled();
            var hearingResponse = await CreateHearing(hearingRequest);
            var individual = hearingResponse.Participants.First(x => x.UserRoleName == UserTypeName.Individual.Name);

            var answersRequest = CreateAnswersRequest();
            await CreateSuitabilityAnswers(answersRequest, hearingResponse.Id, individual.Id);
            VerifyResponse(HttpStatusCode.NoContent, true);

            var uri = ApiUriFactory.HearingEndpoints.GetSuitabilityAnswers(individual.Username);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<PersonSuitabilityAnswerResponse>>(Json);

            response.Should().NotBeNull();
            response.Count.Should().Be(1);
            response.First().HearingId.Should().Be(hearingResponse.Id);
            response.First().ParticipantId.Should().Be(individual.Id);
            response.First().QuestionnaireNotRequired.Should().BeFalse();
            response.First().Answers.Count.Should().Be(1);
            response.First().Answers.First().Answer.Should().Be(answersRequest.First().Answer);
            response.First().Answers.First().ExtendedAnswer.Should().Be(answersRequest.First().ExtendedAnswer);
            response.First().Answers.First().Key.Should().Be(answersRequest.First().Key);
        }

        [Test]
        public async Task Should_return_empty_list_for_non_existent_username()
        {
            const string USERNAME = EmailData.NON_EXISTENT_USERNAME;
            var uri = ApiUriFactory.HearingEndpoints.GetSuitabilityAnswers(USERNAME);
            await SendGetRequest(uri);

            VerifyResponse(HttpStatusCode.OK, true);
            var response = RequestHelper.Deserialise<List<PersonSuitabilityAnswerResponse>>(Json);
            response.Should().BeEmpty();
        }
    }
}
