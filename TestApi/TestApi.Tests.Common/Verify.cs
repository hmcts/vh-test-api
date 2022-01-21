using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.Enums;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using TestApi.Common.Data;
using TestApi.Contract.Dtos;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestApi.Contract.Enums;
using UserApi.Contract.Responses;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

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

        public static void UserDetailsResponse(UserDetailsResponse response, UserDto user)
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

        public static void UsersDetailsResponse(List<UserDetailsResponse> responses, List<UserDto> users)
        {
            responses.Count.Should().Be(users.Count);
            foreach (var user in users)
            {
                var response = responses.First(x => x.Username.Equals(user.Username));
                response.Should().NotBeNull();
                response.Should().BeEquivalentTo(user);
            }
        }

        public static void EjudUserDetailsResponse(UserDetailsResponse response, UserType userType)
        {
            response.Application.Should().Be(Application.Ejud);
            response.ContactEmail.Should().Contain(UserType.Judge.ToString());
            response.CreatedDate.Should().NotBe(DateTime.MinValue);
            response.DisplayName.Should().Contain(UserType.Judge.ToString());
            response.FirstName.Should().NotBeNullOrWhiteSpace();
            response.Id.Should().NotBeEmpty();
            response.LastName.Should().Contain(UserType.Judge.ToString());
            response.Number.Should().BeGreaterThan(0);
            response.UserType.Should().Be(userType);
            response.Username.Should().Contain(UserType.Judge.ToString());
        }

        public static void HearingDetailsResponse(HearingDetailsResponse response, CreateHearingRequest request)
        {
            response.AudioRecordingRequired.Should().Be(request.AudioRecordingRequired);
            response.CancelReason.Should().BeNull();
            response.CaseTypeName.Should().Be(request.CaseType);
            response.Cases.First().Name.Should().Contain(request.TestType.ToString());
            response.Cases.First().Number.Should().NotBeNullOrWhiteSpace();
            response.Cases.First().IsLeadCase.Should().Be(HearingData.IS_LEAD_CASE);
            response.ConfirmedBy.Should().BeNull();
            response.ConfirmedDate.Should().BeNull();
            VerifyCreatedBy(response, request);
            response.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
            response.HearingRoomName.Should().Be(HearingData.HEARING_ROOM_NAME);

            response.HearingTypeName.Should().Be(request.CaseType.Equals(HearingData.CASE_TYPE_NAME)
                ? HearingData.HEARING_TYPE_NAME
                : HearingData.CACD_HEARING_TYPE_NAME);

            response.HearingVenueName.Should().Be(request.Venue);
            response.Id.Should().NotBeEmpty();
            response.OtherInformation.Should().Be(HearingData.OTHER_INFORMATION);
            var expectedCount = UsersIncludeCaseAdminOrVho(request.Users) ? request.Users.Count - 1 : request.Users.Count;
            response.Participants.Count.Should().Be(expectedCount);
            response.QuestionnaireNotRequired.Should().Be(request.QuestionnaireNotRequired);
            response.ScheduledDateTime.Should().Be(request.ScheduledDateTime);
            response.ScheduledDuration.Should().Be(HearingData.SCHEDULED_DURATION);
            response.Status.Should().Be(BookingStatus.Booked);
            response.UpdatedBy.Should().BeNull();
            response.UpdatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
            VerifyHearingParticipants(response.Participants, request.Users);
        }

        private static void VerifyCreatedBy(HearingDetailsResponse response, CreateHearingRequest request)
        {
            response.CreatedBy.Should().Be(
                UsersIncludeCaseAdminOrVho(request.Users)
                    ? request.Users.First(x =>
                        x.UserType == UserType.CaseAdmin || x.UserType == UserType.VideoHearingsOfficer).Username
                    : UserData.DEFAULT_CREATED_BY_USER);
        }

        private static bool UsersIncludeCaseAdminOrVho(IEnumerable<UserDto> users)
        {
            return users.Any(x => x.UserType == UserType.CaseAdmin || x.UserType == UserType.VideoHearingsOfficer);
        }

        private static void VerifyHearingParticipants(IEnumerable<ParticipantResponse> participants, IReadOnlyCollection<UserDto> users)
        {
            foreach (var participant in participants)
            {
                var user = users.First(x => x.DisplayName.Equals(participant.DisplayName));
                participant.CaseRoleName.Should().NotBeNullOrWhiteSpace();

                if (user.UserType == UserType.Judge)
                {
                    participant.ContactEmail.Should().BeOneOf(user.ContactEmail, user.Username);
                }
                else
                {
                    participant.ContactEmail.Should().Be(user.ContactEmail);
                    participant.LastName.Should().Be(user.LastName);
                    participant.TelephoneNumber.Should().Be(UserData.TELEPHONE_NUMBER);
                    participant.Title.Should().Be(UserData.TITLE);
                }

                participant.DisplayName.Should().Be(user.DisplayName);
                participant.FirstName.Should().Be(user.FirstName);
                participant.HearingRoleName.Should().NotBeNullOrWhiteSpace();
                participant.Id.Should().NotBeEmpty();

                if (user.UserType == UserType.Representative)
                {
                    participant.Organisation.Should().NotBeNullOrWhiteSpace();
                    participant.Representee.Should().NotBeNullOrWhiteSpace();
                }

                participant.Username.Should().Be(user.Username);
            }
        }

        public static void ConferenceDetailsResponse(ConferenceDetailsResponse response, HearingDetailsResponse hearing)
        {
            response.AudioRecordingRequired.Should().Be(hearing.AudioRecordingRequired);
            response.CaseName.Should().Be(hearing.Cases.First().Name);
            response.CaseNumber.Should().Be(hearing.Cases.First().Number);
            response.CaseType.Should().Be(hearing.CaseTypeName);
            response.ClosedDateTime.Should().BeNull();
            response.CurrentStatus.Should().Be(ConferenceState.NotStarted);
            response.HearingId.Should().Be(hearing.Id);
            response.HearingVenueName.Should().Be(hearing.HearingVenueName);
            response.Id.Should().NotBeEmpty();
            response.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
            response.ScheduledDuration.Should().Be(hearing.ScheduledDuration);
            response.StartedDateTime.Should().BeNull();
            VerifyConferenceParticipants(response.Participants, hearing.Participants);
        }

        public static void ConferenceDetailsResponse(ConferenceDetailsResponse response, BookNewConferenceRequest request)
        {
            AssignParticipantResponseIdsToRequest(response.Participants, request);
            response.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        }

        private static void VerifyConferenceParticipants(IReadOnlyCollection<ParticipantDetailsResponse> hearingParticipants,
            IReadOnlyCollection<ParticipantResponse> conferenceParticipants)
        {
            hearingParticipants.Count.Should().Be(conferenceParticipants.Count);
            foreach (var hearingParticipant in hearingParticipants)
            {
                var conferenceParticipant =
                    conferenceParticipants.First(x => x.LastName.Equals(hearingParticipant.LastName));

                conferenceParticipant.CaseRoleName.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.ContactEmail.Should().Be(hearingParticipant.ContactEmail);
                conferenceParticipant.DisplayName.Should().Be(hearingParticipant.DisplayName);
                conferenceParticipant.FirstName.Should().Be(hearingParticipant.FirstName);
                conferenceParticipant.HearingRoleName.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Id.Should().NotBeEmpty();
                conferenceParticipant.MiddleNames.Should().BeNullOrWhiteSpace();
                conferenceParticipant.LastName.Should().Be(hearingParticipant.LastName);
                conferenceParticipant.TelephoneNumber.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Title.Should().NotBeNullOrWhiteSpace();
                conferenceParticipant.Username.Should().Be(hearingParticipant.Username);

                if (!conferenceParticipant.UserRoleName.Equals("Representative")) continue;
                conferenceParticipant.Organisation.Should().NotBeNullOrWhiteSpace();
                if (conferenceParticipant.HearingRoleName != RoleData.CACD_REP_HEARING_ROLE_NAME)
                {
                    conferenceParticipant.Representee.Should().NotBeNullOrWhiteSpace();
                }
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

        public static void ConferencesForJudgeResponses(List<ConferenceForHostResponse> responses, BookNewConferenceRequest request)
        {
            foreach (var response in responses)
                AssignParticipantResponseIdsToRequest(response.Participants, request);

            responses.Count.Should().BeGreaterThan(0);
            var conference = responses.First(x => x.CaseName.Equals(request.CaseName));
            conference.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        }

        public static void ConferencesForVhoResponses(List<ConferenceForAdminResponse> responses, BookNewConferenceRequest request)
        {
            foreach (var response in responses)
                AssignParticipantResponseIdsToRequest(response.Participants, request);

            responses.Count.Should().BeGreaterThan(0);
            var conference = responses.First(x => x.HearingRefId.Equals(request.HearingRefId));
            conference.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        }

        public static void Tasks(TaskResponse response, ConferenceEventRequest request)
        {
            response.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        }

        public static void UserProfileResponse(UserProfile response, UserDto user)
        {
            response.Should().BeEquivalentTo(user, options => options.ExcludingMissingMembers());
        }

        private static void AssignParticipantResponseIdsToRequest<T>(List<T> participants, BookNewConferenceRequest request)
        {
            foreach (dynamic participant in participants)
            {
                var foundParticipant = request.Participants.FirstOrDefault(req => req.DisplayName == participant.DisplayName);

                if (foundParticipant != null)
                {
                    foundParticipant.Id = participant.Id;
                }
            }
        }
    }
}
