using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using TestApi.Domain.Helpers;

namespace TestApi.IntegrationTests.Controllers.Hearings
{
    public class UpdateSuitabilityAnswersTests : HearingsTestsBase
    {
        [Test]
        public async Task Should_add_suitability_answers()
        {
            var hearingRequest = CreateHearingRequestWithQuestionnaireEnabled();
            var hearingResponse = await CreateHearing(hearingRequest);
            var individual = hearingResponse.Participants.First(x => x.User_role_name == UserTypeName.Individual.Name);
            var answersRequest = CreateAnswersRequest();
            await CreateSuitabilityAnswers(answersRequest, hearingResponse.Id, individual.Id);

            VerifyResponse(HttpStatusCode.NoContent, true);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_hearing_id()
        {
            var hearingRequest = CreateHearingRequestWithQuestionnaireEnabled();
            var hearingResponse = await CreateHearing(hearingRequest);
            var individual = hearingResponse.Participants.First(x => x.User_role_name == UserTypeName.Individual.Name);
            var answersRequest = CreateAnswersRequest();

            await CreateSuitabilityAnswers(answersRequest, Guid.NewGuid(), individual.Id);
            VerifyResponse(HttpStatusCode.NotFound, false);
        }

        [Test]
        public async Task Should_return_not_found_for_non_existent_participant_id()
        {
            var hearingRequest = CreateHearingRequestWithQuestionnaireEnabled();
            var hearingResponse = await CreateHearing(hearingRequest);
            var answersRequest = CreateAnswersRequest();

            await CreateSuitabilityAnswers(answersRequest, hearingResponse.Id, Guid.NewGuid());
            VerifyResponse(HttpStatusCode.NotFound, false);
        }
    }
}
