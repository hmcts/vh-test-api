using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TestApi.Common.Data;
using TestApi.Contract.Dtos;
using TestApi.Contract.Enums;
using TestApi.Services.Clients.VideoApiClient;

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

                var request = new ParticipantRequest();

                if (user.UserType == UserType.Individual)
                {
                    if (_isCACDCaseType)
                    {
                        request.Case_type_group = RoleData.CACD_CASE_ROLE_NAME;
                        request.Hearing_role = RoleData.APPELLANT_CASE_ROLE_NAME;
                    }
                    else
                    {
                        request.Case_type_group = indIndex == 0 ? RoleData.FIRST_CASE_ROLE_NAME : RoleData.SECOND_CASE_ROLE_NAME;
                        request.Hearing_role = RoleData.INDV_HEARING_ROLE_NAME;
                        indIndex++;
                    }
                }

                if (user.UserType == UserType.Representative)
                {
                    if (_isCACDCaseType)
                    {
                        request.Case_type_group = RoleData.CACD_CASE_ROLE_NAME;
                        request.Hearing_role = RoleData.CACD_REP_HEARING_ROLE_NAME;
                    }
                    else
                    {
                        request.Case_type_group = repIndex == 0 ? RoleData.FIRST_CASE_ROLE_NAME : RoleData.SECOND_CASE_ROLE_NAME;
                        request.Hearing_role = RoleData.REPRESENTATIVE_HEARING_ROLE_NAME;
                        request.Representee = ChooseToRepresentIndividualIfPossible(individuals, repIndex);
                        repIndex++;
                    }
                }

                if (user.UserType == UserType.Interpreter || user.UserType == UserType.Witness)
                {
                    request.Case_type_group = _isCACDCaseType ? RoleData.CACD_CASE_ROLE_NAME : RoleData.FIRST_CASE_ROLE_NAME;
                    request.Hearing_role = AddSpacesToUserType(user.UserType);
                }

                if (user.UserType == UserType.Winger)
                {
                    request.Case_type_group = RoleData.CACD_CASE_ROLE_NAME;
                    request.Hearing_role = AddSpacesToUserType(user.UserType);
                }

                if (user.UserType != UserType.Individual && 
                    user.UserType != UserType.Representative && 
                    user.UserType != UserType.Winger && 
                    user.UserType != UserType.Witness &&
                    user.UserType != UserType.Interpreter)
                {
                    request.Case_type_group = AddSpacesToUserType(user.UserType);
                    request.Hearing_role = AddSpacesToUserType(user.UserType);
                }

                request.Contact_email = user.ContactEmail;
                request.Contact_telephone = UserData.TELEPHONE_NUMBER;
                request.Display_name = user.DisplayName;
                request.First_name = user.FirstName;
                request.Last_name = user.LastName;
                request.Linked_participants = new List<LinkedParticipantRequest>();
                request.Name = $"{UserData.TITLE} {user.FirstName} {user.LastName}";
                request.Participant_ref_id = Guid.NewGuid();
                request.User_role = GetUserRoleFromUserType(user.UserType);
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
