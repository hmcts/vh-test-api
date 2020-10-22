using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TestApi.Common.Data;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.Services.Builders.Requests
{
    public class BookHearingParticipantsBuilder
    {
        private readonly List<ParticipantRequest> _participants;
        private readonly List<User> _users;
        private readonly bool _isCACDCaseType;

        public BookHearingParticipantsBuilder(List<User> users, bool isCACDCaseType)
        {
            _users = users;
            _participants = new List<ParticipantRequest>();
            _isCACDCaseType = isCACDCaseType;
        }

        public List<ParticipantRequest> Build()
        {
            var individuals = _users.Where(x => x.UserType == UserType.Individual).ToList();
            var representatives = _users.Where(x => x.UserType == UserType.Representative).ToList();

            ValidateUsers(individuals.Count, representatives.Count);

            var indIndex = 0;
            var repIndex = 0;

            foreach (var user in _users)
            {
                if (user.UserType == UserType.CaseAdmin || user.UserType == UserType.VideoHearingsOfficer) continue;

                var request = new ParticipantRequest();

                if (user.UserType == UserType.Individual)
                {
                    if (_isCACDCaseType)
                    {
                        request.Case_role_name = RoleData.CACD_CASE_ROLE_NAME;
                        request.Hearing_role_name = RoleData.APPELLANT_CASE_ROLE_NAME;
                    }
                    else
                    {
                        request.Case_role_name = indIndex == 0 ? RoleData.FIRST_CASE_ROLE_NAME : RoleData.SECOND_CASE_ROLE_NAME;
                        request.Hearing_role_name = indIndex == 0 ? RoleData.FIRST_INDV_HEARING_ROLE_NAME : RoleData.SECOND_INDV_HEARING_ROLE_NAME;
                        indIndex++;
                    }
                }

                if (user.UserType == UserType.Representative)
                {
                    if (_isCACDCaseType)
                    {
                        request.Case_role_name = RoleData.CACD_CASE_ROLE_NAME;
                        request.Hearing_role_name = RoleData.CACD_REP_HEARING_ROLE_NAME;
                    }
                    else
                    {
                        request.Case_role_name = repIndex == 0 ? RoleData.FIRST_CASE_ROLE_NAME : RoleData.SECOND_CASE_ROLE_NAME;
                        request.Hearing_role_name = RoleData.REPRESENTATIVE_HEARING_ROLE_NAME;
                        request.Representee = individuals.Count == 0 ? "An individual" : individuals[repIndex].DisplayName;
                        repIndex++;
                    }

                    request.Organisation_name = UserData.ORGANISATION;
                }

                if (user.UserType == UserType.Winger)
                {
                    request.Case_role_name = RoleData.CACD_CASE_ROLE_NAME;
                    request.Hearing_role_name = AddSpacesToUserType(user.UserType);
                }

                if (user.UserType != UserType.Individual && user.UserType != UserType.Representative && user.UserType != UserType.Winger)
                {
                    request.Case_role_name = AddSpacesToUserType(user.UserType);
                    request.Hearing_role_name = AddSpacesToUserType(user.UserType);
                }

                request.AdditionalProperties = null;
                request.Contact_email = user.ContactEmail;
                request.Display_name = user.DisplayName;
                request.First_name = user.FirstName;
                request.Last_name = user.LastName;
                request.Middle_names = UserData.MIDDLE_NAME;
                request.Telephone_number = UserData.TELEPHONE_NUMBER;
                request.Title = UserData.TITLE;
                request.Username = user.Username;

                _participants.Add(request);
            }

            return _participants;
        }

        private void ValidateUsers(int totalIndividuals, int totalRepresentatives)
        {
            var totalJudges = _users.Count(x => x.UserType == UserType.Judge);
            totalJudges.Should().Be(1);
            (totalIndividuals + totalRepresentatives).Should().BeGreaterThan(0);
        }

        private static string AddSpacesToUserType(UserType userType)
        {
            return string.Concat(userType.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString()))
                .TrimStart(' ');
        }
    }
}