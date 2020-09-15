using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Clients.UserApiClient;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.Tests.Common
{
    public static class Verify
    {
        public static void UserDetailsResponse(UserDetailsResponse response, UserType userType)
        {
            response.Application.Should().Be(Application.TestApi);
            response.ContactEmail.Should().Contain(userType.ToString().ToLowerInvariant());
            response.CreatedDate.Should().NotBe(DateTime.MinValue);
            response.DisplayName.Should().Contain(userType.ToString());
            response.FirstName.Should().NotBeNullOrWhiteSpace();
            response.Id.Should().NotBeEmpty();
            response.LastName.Should().Contain(userType.ToString());
            response.Number.Should().BeGreaterThan(0);
            response.UserType.Should().Be(userType);
            response.Username.Should().Contain(userType.ToString().ToLowerInvariant());
        }

        public static void UserDetailsResponse(UserDetailsResponse response, User user)
        {
            response.Should().BeEquivalentTo(user);
        }

        public static void UsersDetailsResponse(List<UserDetailsResponse> responses, List<UserType> userTypes)
        {
            responses.Count.Should().Be(userTypes.Count);
            foreach (var userType in userTypes)
            {
                var response = responses.First(x => x.UserType.Equals(userType));
                response.Should().NotBeNull();
                UserDetailsResponse(response, userType);
            }
        }

        public static void UsersDetailsResponse(List<UserDetailsResponse> responses, List<User> users)
        {
            responses.Count.Should().Be(users.Count);
            foreach (var user in users)
            {
                var response = responses.First(x => x.Username.Equals(user.Username));
                response.Should().NotBeNull();
                response.Should().BeEquivalentTo(user);
            }
        }

        public static void HearingDetailsResponse(HearingDetailsResponse response, CreateHearingRequest request)
        {
            response.AdditionalProperties.Count.Should().Be(0);
            response.Audio_recording_required.Should().Be(request.AudioRecordingRequired);
            response.Cancel_reason.Should().BeNull();
            response.Case_type_name.Should().Be(HearingData.CASE_TYPE_NAME);
            response.Cases.First().AdditionalProperties.Count.Should().Be(0);
            response.Cases.First().Name.Should().Contain(request.TestType.ToString());
            response.Cases.First().Number.Should().NotBeNullOrWhiteSpace();
            response.Cases.First().Is_lead_case.Should().Be(HearingData.IS_LEAD_CASE);
            response.Confirmed_by.Should().BeNull();
            response.Confirmed_date.Should().BeNull();
            response.Created_by.Should().Be(request.Users.First(x => x.UserType == UserType.CaseAdmin).Username);
            response.Created_date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
            response.Hearing_room_name.Should().Be(HearingData.HEARING_ROOM_NAME);
            response.Hearing_type_name.Should().Be(HearingData.HEARING_TYPE_NAME);
            response.Hearing_venue_name.Should().Be(request.Venue);
            response.Id.Should().NotBeEmpty();
            response.Other_information.Should().Be(HearingData.OTHER_INFORMATION);
            response.Participants.Count.Should().Be(request.Users.Count - 1);
            response.Questionnaire_not_required.Should().Be(request.QuestionnaireNotRequired);
            response.Scheduled_date_time.Should().Be(request.ScheduledDateTime);
            response.Scheduled_duration.Should().Be(HearingData.SCHEDULED_DURATION);
            response.Status.Should().Be(BookingStatus.Booked);
            response.Updated_by.Should().BeNull();
            response.Updated_date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
            VerifyHearingParticipants(response.Participants, request.Users);
        }

        private static void VerifyHearingParticipants(IEnumerable<ParticipantResponse> participants, IReadOnlyCollection<User> users)
        {
            foreach (var participant in participants)
            {
                var user = users.First(x => x.LastName.Equals(participant.Last_name));
                participant.AdditionalProperties.Should().BeEmpty();
                participant.Case_role_name.Should().NotBeNullOrWhiteSpace();
                participant.Contact_email.Should().Be(user.ContactEmail);
                participant.Display_name.Should().Be(user.DisplayName);
                participant.First_name.Should().Be(user.FirstName);
                participant.Hearing_role_name.Should().NotBeNullOrWhiteSpace();
                participant.Middle_names.Should().Be(UserData.MIDDLE_NAME);
                participant.Last_name.Should().Be(user.LastName);
                participant.Id.Should().NotBeEmpty();

                if (user.UserType == UserType.Representative)
                {
                    participant.Organisation.Should().NotBeNullOrWhiteSpace();
                    participant.Reference.Should().NotBeNullOrWhiteSpace();
                    participant.Representee.Should().NotBeNullOrWhiteSpace();
                }

                participant.Telephone_number.Should().Be(UserData.TELEPHONE_NUMBER);
                participant.Title.Should().Be(UserData.TITLE);
                participant.Username.Should().Be(user.Username);
            }
        }

        public static void ConferenceDetailsResponse(ConferenceDetailsResponse response, HearingDetailsResponse hearing)
        {
            response.Audio_recording_required.Should().Be(hearing.Audio_recording_required);
            response.Case_name.Should().Be(hearing.Cases.First().Name);
            response.Case_number.Should().Be(hearing.Cases.First().Number);
            response.Case_type.Should().Be(hearing.Case_type_name);
            response.Closed_date_time.Should().BeNull();
            response.Current_status.Should().Be(ConferenceState.NotStarted);
            response.Hearing_id.Should().Be(hearing.Id);
            response.Hearing_venue_name.Should().Be(hearing.Hearing_venue_name);
            response.Id.Should().NotBeEmpty();
            response.Scheduled_date_time.Should().Be(hearing.Scheduled_date_time);
            response.Scheduled_duration.Should().Be(hearing.Scheduled_duration);
            response.Started_date_time.Should().BeNull();
            VerifyConferenceParticipants(response.Participants, hearing.Participants);
        }

        public static void ConferenceDetailsResponse(ConferenceDetailsResponse response, BookNewConferenceRequest request)
        {
            response.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        }

        private static void VerifyConferenceParticipants(IReadOnlyCollection<ParticipantDetailsResponse> hearingParticipants,
            IReadOnlyCollection<ParticipantResponse> conferenceParticipants)
        {
            hearingParticipants.Count.Should().Be(conferenceParticipants.Count);
            foreach (var hearingParticipant in hearingParticipants)
            {
                var conferenceParticipant =
                    conferenceParticipants.First(x => x.Last_name.Equals(hearingParticipant.Last_name));

                conferenceParticipant.AdditionalProperties.Should().BeEmpty();
                conferenceParticipant.Case_role_name.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Contact_email.Should().Be(hearingParticipant.Contact_email);
                conferenceParticipant.Display_name.Should().Be(hearingParticipant.Display_name);
                conferenceParticipant.First_name.Should().Be(hearingParticipant.First_name);
                conferenceParticipant.Hearing_role_name.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Id.Should().NotBeEmpty();
                conferenceParticipant.Middle_names.Should().BeNullOrWhiteSpace();
                conferenceParticipant.Last_name.Should().Be(hearingParticipant.Last_name);
                conferenceParticipant.Telephone_number.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Title.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Username.Should().Be(hearingParticipant.Username);

                if (!conferenceParticipant.User_role_name.Equals("Representative")) continue;
                conferenceParticipant.Organisation.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Reference.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Representee.Should().NotBeNullOrWhiteSpace();
            }
        }

        public static void AllocationDetailsResponse(List<AllocationDetailsResponse> allocations, List<string> usernames)
        {
            foreach (var allocation in allocations)
            {
                allocation.Allocated.Should().BeFalse();
                allocation.ExpiresAt.Should().BeNull();
                allocation.Id.Should().NotBeEmpty();
                allocation.UserId.Should().NotBeEmpty();
                usernames.Any(x => x.Equals(allocation.Username)).Should().BeTrue();
            }
        }

        public static void UpdatedHearing(HearingDetailsResponse hearingDetails, UpdateHearingRequest request)
        {
            hearingDetails.Should().BeEquivalentTo(request);
        }

        public static void UpdatedConference(ConferenceDetailsResponse conferenceDetails, UpdateHearingRequest request)
        {
            conferenceDetails.Should().BeEquivalentTo(request, options => options
                .Excluding(x => x.AdditionalProperties)
                .Excluding(x => x.Cases)
                .Excluding(x => x.Hearing_room_name)
                .Excluding(x => x.Other_information)
                .Excluding(x => x.Questionnaire_not_required)
                .Excluding(x => x.Updated_by)
            );

            conferenceDetails.Case_name.Should().Be(request.Cases.First().Name);
            conferenceDetails.Case_number.Should().Be(request.Cases.First().Number);
        }

        public static void ParticipantDetails(ParticipantDetailsResponse participant, AddParticipantsToHearingRequest request)
        {
            participant.Should().BeEquivalentTo(request.Participants.First(), 
                options => options.ExcludingMissingMembers().Excluding(x => x.Representee));
        }

        public static void ConferencesForJudgeResponses(List<ConferenceForJudgeResponse> response, BookNewConferenceRequest request)
        {
            response.Count.Should().BeGreaterThan(0);
            var conference = response.First(x => x.Case_name.Equals(request.Case_name));
            conference.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        }

        public static void ConferencesForVhoResponses(List<ConferenceForAdminResponse> response, BookNewConferenceRequest request)
        {
            response.Count.Should().BeGreaterThan(0);
            var conference = response.First(x => x.Hearing_ref_id.Equals(request.Hearing_ref_id));
            conference.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        }

        public static void Tasks(TaskResponse response, ConferenceEventRequest request)
        {
            response.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        }

        public static void UserProfileResponse(UserProfile response, User user)
        {
            response.Should().BeEquivalentTo(user, options => options.ExcludingMissingMembers());
        }

        public static void AdminWebDropdownUserResponse(UserDetailsResponse response)
        {
            response.Application.Should().Be(Application.AdminWeb);
            response.ContactEmail.Should().Be(AdminWebJudgeData.CONTACT_EMAIL);
            response.DisplayName.Should().Be(AdminWebJudgeData.DISPLAY_NAME);
            response.FirstName.Should().Be(AdminWebJudgeData.FIRST_NAME);
            response.IsProdUser.Should().BeTrue();
            response.LastName.Should().Be(AdminWebJudgeData.LAST_NAME);
            response.TestType.Should().Be(TestType.Automated);
            response.UserType.Should().Be(UserType.Judge);
            response.Username.Should().StartWith(AdminWebJudgeData.USERNAME);
        }
    }
}
