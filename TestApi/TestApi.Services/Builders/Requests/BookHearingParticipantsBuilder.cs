using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.Requests;
using FluentAssertions;
using TestApi.Common.Data;
using TestApi.Contract.Dtos;
using TestApi.Contract.Enums;

namespace TestApi.Services.Builders.Requests
{
    public class BookHearingParticipantsBuilder
    {
        private readonly List<ParticipantRequest> _participants;
        private readonly List<UserDto> _users;
        private readonly bool _isCACDCaseType;

        public BookHearingParticipantsBuilder(List<UserDto> users, bool isCACDCaseType)
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
                        request.CaseRoleName = RoleData.CACD_CASE_ROLE_NAME;
                        request.HearingRoleName = RoleData.APPELLANT_CASE_ROLE_NAME;
                    }
                    else
                    {
                        request.CaseRoleName = indIndex == 0 ? RoleData.FIRST_CASE_ROLE_NAME : RoleData.SECOND_CASE_ROLE_NAME;
                        request.HearingRoleName = RoleData.INDV_HEARING_ROLE_NAME;
                        indIndex++;
                    }
                }

                if (user.UserType == UserType.Representative)
                {
                    if (_isCACDCaseType)
                    {
                        request.CaseRoleName = RoleData.CACD_CASE_ROLE_NAME;
                        request.HearingRoleName = RoleData.CACD_REP_HEARING_ROLE_NAME;
                    }
                    else
                    {
                        request.CaseRoleName = repIndex == 0 ? RoleData.FIRST_CASE_ROLE_NAME : RoleData.SECOND_CASE_ROLE_NAME;
                        request.HearingRoleName = RoleData.REPRESENTATIVE_HEARING_ROLE_NAME;
                        request.Representee = ChooseToRepresentIndividualIfPossible(individuals, repIndex);
                        repIndex++;
                    }

                    request.OrganisationName = UserData.ORGANISATION;
                }

                if (user.UserType == UserType.Interpreter || user.UserType == UserType.Witness)
                {
                    request.CaseRoleName = _isCACDCaseType ? RoleData.CACD_CASE_ROLE_NAME : RoleData.FIRST_CASE_ROLE_NAME;
                    request.HearingRoleName = AddSpacesToUserType(user.UserType);
                }

                if (user.UserType == UserType.Winger)
                {
                    request.CaseRoleName = RoleData.CACD_CASE_ROLE_NAME;
                    request.HearingRoleName = AddSpacesToUserType(user.UserType);
                }

                if (user.UserType != UserType.Individual &&
                    user.UserType != UserType.Representative &&
                    user.UserType != UserType.Winger &&
                    user.UserType != UserType.Witness &&
                    user.UserType != UserType.Interpreter)
                {
                    request.CaseRoleName = AddSpacesToUserType(user.UserType);
                    request.HearingRoleName = AddSpacesToUserType(user.UserType);
                }

                request.ContactEmail = user.ContactEmail;
                request.DisplayName = user.DisplayName;
                request.FirstName = user.FirstName;
                request.LastName = user.LastName;
                request.MiddleNames = UserData.MIDDLE_NAME;
                request.TelephoneNumber = UserData.TELEPHONE_NUMBER;
                request.Title = UserData.TITLE;
                request.Username = user.Username;

                _participants.Add(request);
            }

            return _participants;
        }

        private static string ChooseToRepresentIndividualIfPossible(IReadOnlyList<UserDto> individuals, int repIndex)
        {
            return repIndex + 1 <= individuals.Count ? individuals[repIndex].DisplayName : HearingData.REPRESENTEE;
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