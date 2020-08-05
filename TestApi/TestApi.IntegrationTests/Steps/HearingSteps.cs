using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestApi.Common.Helpers;
using TestApi.Contract.Builders;
using TestApi.Contract.Requests;
using TestApi.Domain.Enums;
using TestApi.IntegrationTests.Configuration;
using TestApi.IntegrationTests.Helpers;
using TestApi.Services.Clients.BookingsApiClient;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.IntegrationTests.Steps
{
    [Binding]
    public class HearingSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly CommonSteps _commonSteps;
        private CreateHearingRequest _request;

        public HearingSteps(TestContext context, CommonSteps commonSteps)
        {
            _context = context;
            _commonSteps = commonSteps;
        }

        [Given(@"I have a create hearing request")]
        public void GivenIHaveACreateHearingRequest()
        {
            BulkAddParticipants();

            _request = new HearingBuilder(_context.Test.Users).BuildRequest();
            BuildRequest(_request);
        }

        [Given(@"I have a create hearing request in (.*) minutes time")]
        public void GivenIHaveACreateHearingRequestInMinutesTime(int minutes)
        {
            BulkAddParticipants();

            _request = new HearingBuilder(_context.Test.Users)
                .ScheduledDateTime(DateTime.UtcNow.AddMinutes(minutes))
                .BuildRequest();

            BuildRequest(_request);
        }

        [Given(@"I have a create hearing request in (.*) days time")]
        public void GivenIHaveACreateHearingRequestInDaysTime(int days)
        {
            BulkAddParticipants();

            _request = new HearingBuilder(_context.Test.Users)
                .ScheduledDateTime(DateTime.UtcNow.AddDays(days))
                .BuildRequest();

            BuildRequest(_request);
        }

        [Given(@"I have a create hearing request in location")]
        public void GivenIHaveACreateHearingRequestInLocation()
        {
            const string MANCHESTER = "Manchester Civil and Family Justice Centre";

            BulkAddParticipants();

            _request = new HearingBuilder(_context.Test.Users)
                .HearingVenue(MANCHESTER)
                .BuildRequest();

            BuildRequest(_request);
        }

        [Given(@"I have a create hearing request with a list of participants (.*)")]
        public void GivenIHaveACreateHearingRequestWithAListOfParticipants(string participants)
        {
            var individualsCount = CountUserType(participants, UserType.Individual);
            var representativesCount = CountUserType(participants, UserType.Representative);
            var observersCount = CountUserType(participants, UserType.Observer);
            var panelMembersCount = CountUserType(participants, UserType.PanelMember);

            BulkAddParticipants(individualsCount, representativesCount, observersCount, panelMembersCount);

            _request = new HearingBuilder(_context.Test.Users).BuildRequest();
            BuildRequest(_request);
        }

        [Given(@"I have a create hearing request with specific participants")]
        public void GivenIHaveACreateHearingRequestWithSpecificParticipants()
        {
            _request = new HearingBuilder(_context.Test.Users).BuildRequest();
            BuildRequest(_request);
        }

        [Given(@"I have a create hearing request with audio recording enabled")]
        public void GivenIHaveACreateHearingRequestWithAudioRecordingEnabled()
        {
            BulkAddParticipants();

            _request = new HearingBuilder(_context.Test.Users)
                .AudioRecordingRequired()
                .BuildRequest();

            BuildRequest(_request);
        }

        [Given(@"I have a create hearing request with questionnaires enabled")]
        public void GivenIHaveACreateHearingRequestWithQuestionnairesEnabled()
        {
            BulkAddParticipants();

            _request = new HearingBuilder(_context.Test.Users)
                .QuestionnairesRequired()
                .BuildRequest();

            BuildRequest(_request);
        }

        [Given(@"I have a hearing")]
        public async Task GivenIHaveAHearing()
        {
            BulkAddParticipants();
            _request = new HearingBuilder(_context.Test.Users).BuildRequest();
            BuildRequest(_request);
            await _commonSteps.WhenISendTheRequestToTheEndpoint();
            _commonSteps.ThenTheResponseShouldHaveStatus(HttpStatusCode.Created, true);
            var response = await Response.GetResponses<HearingDetailsResponse>(_context.Response.Content);
            response.Should().NotBeNull();
            _context.Test.Hearing = response;
        }

        [Given(@"I have a get hearing request")]
        public void GivenIHaveAGetHearingRequest()
        {
            _context.Uri = ApiUriFactory.HearingEndpoints.GetHearing(_context.Test.Hearing.Id);
            _context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a confirm hearing request")]
        public void GivenIHaveAConfirmHearingRequest()
        {
            var request = new UpdateBookingStatusRequest()
            {
                Status = UpdateBookingStatus.Created,
                Updated_by = _request.Users.First(x => x.UserType == UserType.CaseAdmin).Username
            };

            _context.Uri = ApiUriFactory.HearingEndpoints.ConfirmHearing(_context.Test.Hearing.Id); ;
            _context.HttpMethod = HttpMethod.Patch;

            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a delete hearing request")]
        public void GivenIHaveADeleteHearingRequest()
        {
            _context.Uri = ApiUriFactory.HearingEndpoints.DeleteHearing(_context.Test.Hearing.Id);
            _context.HttpMethod = HttpMethod.Delete;
        }

        [Then(@"the hearing details should be retrieved")]
        public async Task ThenTheHearingDetailsShouldBeRetrieved()
        {
            var response = await Response.GetResponses<HearingDetailsResponse>(_context.Response.Content);
            response.Should().NotBeNull();

            var caseAdmin = _request.Users.First(x => x.UserType == UserType.CaseAdmin);
            VerifyParticipants(response.Participants);

            response.AdditionalProperties.Should().BeEmpty();
            response.Audio_recording_required.Should().Be(_request.AudioRecordingRequired);
            response.Cancel_reason.Should().BeNull();
            response.Case_type_name.Should().Be("Civil Money Claims");
            response.Cases.First().AdditionalProperties.Should().BeEmpty();
            response.Cases.First().Is_lead_case.Should().BeFalse();
            response.Cases.First().Name.Should().NotBeNullOrWhiteSpace();
            response.Cases.First().Number.Should().NotBeNullOrWhiteSpace();
            response.Confirmed_by.Should().BeNull();
            response.Confirmed_date.Should().BeNull();
            response.Created_by.Should().Be(caseAdmin.Username);
            response.Created_date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
            response.Hearing_room_name.Should().Be("Room 1");
            response.Hearing_type_name.Should().Be("Application to Set Judgment Aside");
            response.Hearing_venue_name.Should().Be(_request.Venue);
            response.Id.Should().NotBeEmpty();
            response.Other_information.Should().Be("Other information");
            response.Questionnaire_not_required.Should().Be(_request.QuestionnaireNotRequired);
            response.Scheduled_date_time.Should().BeCloseTo(_request.ScheduledDateTime, TimeSpan.FromSeconds(30));
            response.Scheduled_duration.Should().Be(60);
            response.Status.Should().Be(BookingStatus.Booked);
            response.Updated_by.Should().BeNull();
            response.Updated_date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(30));
        }

        [Then(@"the conference details should be retrieved")]
        public async Task ThenTheConferenceDetailsShouldBeRetrieved()
        {
            var response = await Response.GetResponses<ConferenceDetailsResponse>(_context.Response.Content);
            response.Should().NotBeNull();

            var hearing = _context.Test.Hearing;
            VerifyParticipants(hearing.Participants);

            response.Participants.Count.Should().Be(_request.Users.Count - 1);

            response.Audio_recording_required.Should().Be(hearing.Audio_recording_required);
            response.Case_name.Should().Be(hearing.Cases.First().Name);
            response.Case_number.Should().Be(hearing.Cases.First().Number);
            response.Case_type.Should().Be(hearing.Case_type_name);
            response.Closed_date_time.Should().BeNull();
            response.Current_status.Should().Be(ConferenceState.NotStarted);
            response.Hearing_id.Should().Be(hearing.Id);
            response.Hearing_venue_name.Should().Be(hearing.Hearing_venue_name);
            response.Id.Should().NotBeEmpty();
            response.Meeting_room.Admin_uri.Should().NotBeNullOrWhiteSpace();
            response.Meeting_room.Judge_uri.Should().NotBeNullOrWhiteSpace();
            response.Meeting_room.Participant_uri.Should().NotBeNullOrWhiteSpace();
            response.Meeting_room.Pexip_node.Should().NotBeNullOrWhiteSpace();
            response.Meeting_room.Pexip_self_test_node.Should().NotBeNullOrWhiteSpace();
            response.Scheduled_date_time.Should().Be(hearing.Scheduled_date_time);
            response.Scheduled_duration.Should().Be(hearing.Scheduled_duration);
            response.Started_date_time.Should().BeNull();
        }

        private void VerifyParticipants(IReadOnlyCollection<ParticipantResponse> responseParticipants)
        {
            responseParticipants.Count.Should().Be(_request.Users.Count - 1);

            foreach (var participant in responseParticipants)
            {
                var user = _request.Users.First(x => x.Username.Equals(participant.Username));

                participant.AdditionalProperties.Should().BeEmpty();

                if (user.UserType == UserType.Individual)
                {
                    participant.Case_role_name.Should().BeOneOf("Claimant", "Defendant");
                    participant.Hearing_role_name.Should().BeOneOf("Claimant LIP", "Defendant LIP");
                    participant.Organisation.Should().BeNull();
                    participant.Reference.Should().BeNull();
                    participant.Representee.Should().BeNull();
                }

                if (user.UserType == UserType.Representative)
                {
                    participant.Case_role_name.Should().BeOneOf("Claimant", "Defendant");
                    participant.Hearing_role_name.Should().Be("Representative");
                    participant.Organisation.Should().NotBeNullOrWhiteSpace();
                    participant.Reference.Should().Be("Reference");
                    participant.Representee.Should().NotBeNullOrWhiteSpace();
                }

                if (user.UserType != UserType.Individual && user.UserType != UserType.Representative)
                {
                    participant.Case_role_name.Should().Be(UserTypeName.FromUserType(user.UserType));
                    participant.Hearing_role_name.Should().Be(UserTypeName.FromUserType(user.UserType));
                    participant.Organisation.Should().BeNull();
                    participant.Reference.Should().BeNull();
                    participant.Representee.Should().BeNull();
                }

                participant.Contact_email.Should().Be(user.ContactEmail);
                participant.Display_name.Should().Be(user.DisplayName);
                participant.First_name.Should().Be(user.FirstName);
                participant.Id.Should().NotBeEmpty();
                participant.Last_name.Should().Be(user.LastName);
                participant.Middle_names.Should().BeEmpty();
                participant.Telephone_number.Should().NotBeNullOrWhiteSpace();
                participant.Title.Should().Be("Mrs");
                participant.Username.Should().Be(user.Username);

                var expectedUserRole = (user.UserType == UserType.Observer || user.UserType == UserType.PanelMember)
                    ? UserTypeName.FromUserType(UserType.Individual)
                    : UserTypeName.FromUserType(user.UserType);
                participant.User_role_name.Should().Be(expectedUserRole);
            }
        }

        private static int CountUserType(string text, UserType userType)
        {
            return text.Split(", ").Count(x => x.Contains(UserTypeName.FromUserType(userType)));
        }

        private void BulkAddParticipants(int individuals = 2, int representatives = 2, int observers = 0, int panelMembers = 0)
        {
            AddUsers(1, UserType.Judge);
            AddUsers(individuals, UserType.Individual);
            AddUsers(representatives, UserType.Representative);
            AddUsers(observers, UserType.Observer);
            AddUsers(panelMembers, UserType.PanelMember);
            AddUsers(1, UserType.CaseAdmin);
        }

        private void AddUsers(int numberOfNewUsers, UserType userType)
        {
            for (var i = 1; i <= numberOfNewUsers; i++)
            {
                var user = new UserBuilder(_context.Config.UsernameStem, i)
                    .WithUserType(userType)
                    .ForApplication(Application.TestApi)
                    .BuildUser();

                _context.Test.Users.Add(user);
            }
        }

        private void BuildRequest(CreateHearingRequest request)
        {
            _context.Uri = ApiUriFactory.HearingEndpoints.CreateHearing;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
    }
}
