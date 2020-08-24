using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using AcceptanceTests.Common.Api.Uris;
using FluentAssertions;
using NUnit.Framework;
using Polly;
using TestApi.Common.Builders;
using TestApi.Common.Data;
using TestApi.Domain.Enums;
using TestApi.Domain.Helpers;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common;
using ParticipantRequest = TestApi.Services.Clients.BookingsApiClient.ParticipantRequest;

namespace TestApi.BQSTests.Subscriber
{
    public class ParticipantSubscriberTests : TestsBase
    {
        [Test]
        public async Task Should_add_participant_to_hearing_and_conference()
        {
            var uri = BookingsApiUriFactory.HearingsParticipantsEndpoints.AddParticipantsToHearing(Hearing.Id);

            var user = new UserBuilder(Context.Config.UsernameStem, 1)
                .AddPanelMember()
                .ForApplication(Application.TestApi)
                .BuildUser();

            var request = new AddParticipantsToHearingRequest()
            {
                Participants = new List<ParticipantRequest>()
                {
                    new ParticipantRequest()
                    {
                        Case_role_name = UserTypeName.FromUserType(user.UserType),
                        Contact_email = user.ContactEmail,
                        Display_name = user.DisplayName,
                        First_name = user.FirstName,
                        Hearing_role_name = UserTypeName.FromUserType(user.UserType),
                        Middle_names = UserData.MIDDLE_NAME,
                        Last_name = user.LastName,
                        Telephone_number = UserData.TELEPHONE_NUMBER,
                        Title = UserData.TITLE,
                        Username = user.Username
                    }
                }
            };

            await SendPostRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.NoContent, true);

            var conferenceDetails = await PollForConferenceParticipantPresence(Hearing.Id, request.Participants.First().Username, true);
            var participant = conferenceDetails.Participants.First(x => x.Username == request.Participants.First().Username);
            Verify.ParticipantDetails(participant, request);
        }

        [Test]
        public async Task Should_remove_participant_from_hearing_and_conference()
        {
            var participant = Hearing.Participants.First(x => x.User_role_name.Equals(UserType.Individual.ToString()));
            var uri = BookingsApiUriFactory.HearingsParticipantsEndpoints.RemoveParticipantFromHearing(Hearing.Id, participant.Id);

            await SendDeleteRequest(uri);
            VerifyResponse(HttpStatusCode.NoContent, true);

            var conferenceDetails = await PollForConferenceParticipantPresence(Hearing.Id, participant.Username, false);
            conferenceDetails.Participants.Any(x => x.Username.Equals(participant.Username)).Should().BeFalse();
        }

        private async Task<ConferenceDetailsResponse> PollForConferenceParticipantPresence(Guid hearingRefId, string username, bool expected)
        {
            var uri = $"{Context.Config.Services.VideoApiUrl}{VideoApiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(hearingRefId)}";
            CreateNewVideoApiClient(Context.Tokens.VideoApiBearerToken);

            var policy = Policy
                .HandleResult<HttpResponseMessage>(r => r.Content.ReadAsStringAsync().Result.Contains(username).Equals(!expected))
                .WaitAndRetryAsync(RETRIES, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            try
            {
                var result = await policy.ExecuteAsync(async () => await Client.GetAsync(uri));
                var conferenceResponse = RequestHelper.Deserialise<ConferenceDetailsResponse>(await result.Content.ReadAsStringAsync());
                conferenceResponse.Case_name.Should().NotBeNullOrWhiteSpace();
                return conferenceResponse;
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered error '{e.Message}' after {RETRIES ^ 2} seconds.");
            }
        }

        [Test]
        public async Task Should_update_participant_in_hearing_and_conference()
        {
            var participant = Hearing.Participants.First(x => x.User_role_name.Equals(UserType.Representative.ToString()));
            var uri = BookingsApiUriFactory.HearingsParticipantsEndpoints.UpdateParticipantDetails(Hearing.Id, participant.Id);

            var request = new Services.Clients.BookingsApiClient.UpdateParticipantRequest()
            {
                Display_name = $"{participant.Display_name} {HearingData.UPDATED_TEXT}",
                Organisation_name = $"{participant.Organisation} {HearingData.UPDATED_TEXT}",
                Reference = $"{participant.Reference} {HearingData.UPDATED_TEXT}",
                Representee = $"{participant.Representee} {HearingData.UPDATED_TEXT}",
                Telephone_number = Faker.Phone.Number(),
                Title = $"{participant.Title} {HearingData.UPDATED_TEXT}"
            };

            await SendPutRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.OK, true);

            var conferenceDetails = await PollForConferenceParticipantUpdated(Hearing.Id, HearingData.UPDATED_TEXT);
            var updatedParticipant = conferenceDetails.Participants.First(x => x.Username.Equals(participant.Username));
            updatedParticipant.Display_name.Should().Be(request.Display_name);
            updatedParticipant.Representee.Should().Be(request.Representee);
            updatedParticipant.Contact_telephone.Should().Be(request.Telephone_number);
        }

        private async Task<ConferenceDetailsResponse> PollForConferenceParticipantUpdated(Guid hearingRefId, string updatedText)
        {
            var uri = $"{Context.Config.Services.VideoApiUrl}{VideoApiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(hearingRefId)}";
            CreateNewVideoApiClient(Context.Tokens.VideoApiBearerToken);

            var policy = Policy
                .HandleResult<HttpResponseMessage>(r => r.Content.ReadAsStringAsync().Result.Contains(updatedText).Equals(false))
                .WaitAndRetryAsync(RETRIES, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            try
            {
                var result = await policy.ExecuteAsync(async () => await Client.GetAsync(uri));
                var conferenceResponse = RequestHelper.Deserialise<ConferenceDetailsResponse>(await result.Content.ReadAsStringAsync());
                conferenceResponse.Case_name.Should().NotBeNullOrWhiteSpace();
                return conferenceResponse;
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered error '{e.Message}' after {RETRIES ^ 2} seconds.");
            }
        }
    }
}
