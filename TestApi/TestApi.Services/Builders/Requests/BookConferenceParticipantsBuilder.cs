using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TestApi.Common.Data;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.VideoApiClient;

namespace TestApi.Services.Builders.Requests
{
    public class BookConferenceParticipantsBuilder
    {
        private readonly List<ParticipantRequest> _participants;

        private readonly List<User> _users;

        public BookConferenceParticipantsBuilder(List<User> users)
        {
            _users = users;
            _participants = new List<ParticipantRequest>();
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
                    request.Case_type_group = indIndex == 0 ? RoleData.FIRST_CASE_ROLE_NAME : RoleData.SECOND_CASE_ROLE_NAME;
                    indIndex++;
                }

                if (user.UserType == UserType.Representative)
                {
                    request.Case_type_group = repIndex == 0 ? RoleData.FIRST_CASE_ROLE_NAME : RoleData.SECOND_CASE_ROLE_NAME;
                    request.Representee = individuals[repIndex].DisplayName;
                    repIndex++;
                }

                if (user.UserType != UserType.Individual && user.UserType != UserType.Representative)
                {
                    request.Case_type_group = AddSpacesToUserType(user.UserType);
                }

                request.Contact_email = user.ContactEmail;
                request.Contact_telephone = UserData.TELEPHONE_NUMBER;
                request.Display_name = user.DisplayName;
                request.First_name = user.FirstName;
                request.Last_name = user.LastName;
                request.Name = $"{UserData.TITLE} {user.FirstName} {user.LastName}";
                request.Participant_ref_id = Guid.NewGuid();
                request.User_role = GetUserRoleFromUserType(user.UserType);
                request.Username = user.Username;

                _participants.Add(request);
            }

            return _participants;
        }

        private void ValidateUsers(int totalIndividuals, int totalRepresentatives)
        {
            var totalJudges = _users.Count(x => x.UserType == UserType.Judge);
            totalJudges.Should().Be(1);
            totalIndividuals.Should().BeGreaterThan(0);
            totalRepresentatives.Should().BeGreaterThan(0);
            totalIndividuals.Should().Be(totalRepresentatives);
        }

        private static UserRole GetUserRoleFromUserType(UserType userType)
        {
            if (userType == UserType.Observer || userType == UserType.PanelMember)
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
