using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TestApi.Common.Data;
using TestApi.Contract.Dtos;
using TestApi.Contract.Enums;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;

namespace TestApi.Services.Builders.Requests
{
    public class BookConferenceParticipantsBuilder
    {
        private readonly List<ParticipantRequest> _participants;
        private readonly List<UserDto> _users;
        private readonly bool _isCACDCaseType;

        public BookConferenceParticipantsBuilder(List<UserDto> users, bool isCACDCaseType)
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
                if (user.UserType == UserType.CaseAdmin) continue;

                var request = new ParticipantRequest {LinkedParticipants = new List<LinkedParticipantRequest>()};

                if (user.UserType == UserType.Individual)
                {
                    if (_isCACDCaseType)
                    {
                        request.CaseTypeGroup = RoleData.CACD_CASE_ROLE_NAME;
                        request.HearingRole = RoleData.APPELLANT_CASE_ROLE_NAME;
                    }
                    else
                    {
                        request.CaseTypeGroup = indIndex == 0 ? RoleData.FIRST_CASE_ROLE_NAME : RoleData.SECOND_CASE_ROLE_NAME;
                        request.HearingRole = RoleData.INDV_HEARING_ROLE_NAME;
                        indIndex++;
                    }
                }

                if (user.UserType == UserType.Representative)
                {
                    if (_isCACDCaseType)
                    {
                        request.CaseTypeGroup = RoleData.CACD_CASE_ROLE_NAME;
                        request.HearingRole = RoleData.CACD_REP_HEARING_ROLE_NAME;
                    }
                    else
                    {
                        request.CaseTypeGroup = repIndex == 0 ? RoleData.FIRST_CASE_ROLE_NAME : RoleData.SECOND_CASE_ROLE_NAME;
                        request.HearingRole = RoleData.REPRESENTATIVE_HEARING_ROLE_NAME;
                        request.Representee = ChooseToRepresentIndividualIfPossible(individuals, repIndex);
                        repIndex++;
                    }
                }

                if (user.UserType == UserType.Interpreter)
                {
                    request.CaseTypeGroup = _isCACDCaseType ? RoleData.CACD_CASE_ROLE_NAME : RoleData.FIRST_CASE_ROLE_NAME;
                    request.HearingRole = AddSpacesToUserType(user.UserType);
                }

                if (user.UserType == UserType.Witness)
                {
                    request.CaseTypeGroup = _isCACDCaseType ? RoleData.CACD_CASE_ROLE_NAME : RoleData.FIRST_CASE_ROLE_NAME;
                    request.HearingRole = AddSpacesToUserType(user.UserType);
                }

                if (user.UserType == UserType.Winger)
                {
                    request.CaseTypeGroup = RoleData.CACD_CASE_ROLE_NAME;
                    request.HearingRole = AddSpacesToUserType(user.UserType);
                }

                if (user.UserType != UserType.Individual && 
                    user.UserType != UserType.Representative && 
                    user.UserType != UserType.Winger && 
                    user.UserType != UserType.Witness &&
                    user.UserType != UserType.Interpreter)
                {
                    request.CaseTypeGroup = AddSpacesToUserType(user.UserType);
                    request.HearingRole = AddSpacesToUserType(user.UserType);
                }

                request.ContactEmail = user.ContactEmail;
                request.ContactTelephone = UserData.TELEPHONE_NUMBER;
                request.DisplayName = user.DisplayName;
                request.FirstName = user.FirstName;
                request.LastName = user.LastName;
                request.Name = $"{UserData.TITLE} {user.FirstName} {user.LastName}";
                request.ParticipantRefId = Guid.NewGuid();
                request.UserRole = GetUserRoleFromUserType(user.UserType);
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

        private static UserRole GetUserRoleFromUserType(UserType userType)
        {
            if (userType == UserType.PanelMember || userType == UserType.Winger)
            {
                return UserRole.JudicialOfficeHolder;
            }

            if (userType == UserType.Observer || userType == UserType.Witness || userType == UserType.Interpreter)
            {
                return UserRole.Individual;
            }

            return (UserRole) Enum.Parse(typeof(UserRole), userType.ToString(), true);
        }

        private static string AddSpacesToUserType(UserType userType)
        {
            return string.Concat(userType.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString()))
                .TrimStart(' ');
        }
    }
}
