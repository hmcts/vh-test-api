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
using TestApi.Common.Data;
using TestApi.Services.Builders.Requests;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Clients.VideoApiClient;
using TestApi.Tests.Common;

namespace TestApi.BQSTests.Subscriber
{
    public class HearingsSubscriberTests : TestsBase
    {
        [Test]
        public async Task Should_update_conference_when_hearing_updated()
        {
            var updateUri = BookingsApiUriFactory.HearingsEndpoints.UpdateHearingDetails(Hearing.Id);

            var caseRequests = new List<CaseRequest>()
            {
                new CaseRequest()
                {
                    AdditionalProperties = Hearing.Cases.First().AdditionalProperties,
                    Is_lead_case = Hearing.Cases.First().Is_lead_case,
                    Name = $"{Hearing.Cases.First().Name} {HearingData.UPDATED_TEXT}",
                    Number = $"{Hearing.Cases.First().Number} {HearingData.UPDATED_TEXT}"
                }
            };

            var request = new UpdateHearingRequest()
            {
                AdditionalProperties = Hearing.AdditionalProperties,
                Audio_recording_required = !Hearing.Audio_recording_required,
                Cases = caseRequests,
                Hearing_room_name = $"{Hearing.Hearing_room_name} {HearingData.UPDATED_TEXT}",
                Hearing_venue_name = Hearing.Hearing_venue_name.Equals(HearingData.VENUE_NAME) ? HearingData.VENUE_NAME_ALTERNATIVE : HearingData.VENUE_NAME,
                Other_information = $"{Hearing.Other_information} {HearingData.UPDATED_TEXT}",
                Questionnaire_not_required = !Hearing.Questionnaire_not_required,
                Scheduled_date_time = Hearing.Scheduled_date_time.AddMinutes(10),
                Scheduled_duration = Hearing.Scheduled_duration / 2,
                Updated_by = Hearing.Updated_by
            };

            await SendPutRequest(updateUri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.OK, true);

            var hearingDetails = RequestHelper.Deserialise<HearingDetailsResponse>(Json);
            hearingDetails.Should().NotBeNull();
            Verify.UpdatedHearing(hearingDetails, request);

            var response = await GetUpdatedConferenceDetailsPollingAsync(Hearing.Id);
            response.Should().NotBeNull();
            Verify.UpdatedConference(response, request);
        }

        private async Task<ConferenceDetailsResponse> GetUpdatedConferenceDetailsPollingAsync(Guid hearingRefId)
        {
            var uri = $"{Context.Config.Services.VideoApiUrl}{VideoApiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(hearingRefId)}";
            CreateNewVideoApiClient(Context.Tokens.VideoApiBearerToken);

            var policy = Policy
                .HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
                .OrResult(message => !message.Content.ReadAsStringAsync().Result.Contains(HearingData.UPDATED_TEXT))
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
        public async Task Should_delete_conference_when_hearing_deleted()
        {
            var deleteUri = BookingsApiUriFactory.HearingsEndpoints.RemoveHearing(Hearing.Id);
            await SendDeleteRequest(deleteUri);
            VerifyResponse(HttpStatusCode.NoContent, true);

            var result = await PollForConferenceDeleted(Hearing.Id);
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private async Task<HttpResponseMessage> PollForConferenceDeleted(Guid hearingRefId)
        {
            var uri = $"{Context.Config.Services.VideoApiUrl}{VideoApiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(hearingRefId)}";
            CreateNewVideoApiClient(Context.Tokens.VideoApiBearerToken);

            var policy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.OK)
                .WaitAndRetryAsync(RETRIES, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            try
            {
                var result = await policy.ExecuteAsync(async () => await Client.GetAsync(uri));
                return result;
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered error '{e.Message}' after {RETRIES ^ 2} seconds.");
            }
        }

        [Test]
        public async Task Should_delete_conference_when_hearing_cancelled()
        {
            var uri = BookingsApiUriFactory.HearingsEndpoints.UpdateHearingStatus(Hearing.Id);
            const string UPDATED_BY = "updated_by_user@email.com";

            var request = new UpdateBookingRequestBuilder()
                .WithStatus(UpdateBookingStatus.Cancelled)
                .UpdatedBy(UPDATED_BY)
                .Build();

            await SendPatchRequest(uri, RequestHelper.Serialise(request));
            VerifyResponse(HttpStatusCode.NoContent, true);

            var result = await PollForConferenceDeleted(Hearing.Id);
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}