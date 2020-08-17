using System.Collections.Generic;
using System.Linq;
using Faker;
using FluentAssertions;
using TestApi.Common.Data;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.Services.Builders
{
    public class BookHearingParticipantsBuilder
    {
        private readonly List<ParticipantRequest> _participants;

        private readonly List<User> _users;

        public BookHearingParticipantsBuilder(List<User> users)
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
                    request.Case_role_name = indIndex == 0 ? DefaultData.FIRST_CASE_ROLE_NAME : DefaultData.SECOND_CASE_ROLE_NAME;
                    request.Hearing_role_name =
                        indIndex == 0 ? DefaultData.FIRST_INDV_HEARING_ROLE_NAME : DefaultData.SECOND_INDV_HEARING_ROLE_NAME;
                    indIndex++;
                }

                if (user.UserType == UserType.Representative)
                {
                    request.Case_role_name = repIndex == 0 ? DefaultData.FIRST_CASE_ROLE_NAME : DefaultData.SECOND_CASE_ROLE_NAME;
                    request.Hearing_role_name = DefaultData.REPRESENTATIVE_HEARING_ROLE_NAME;
                    request.Organisation_name = Company.Name();
                    request.Reference = DefaultData.REFERENCE;
                    request.Representee = individuals[repIndex].DisplayName;
                    repIndex++;
                }

                if (user.UserType != UserType.Individual && user.UserType != UserType.Representative)
                {
                    request.Case_role_name = AddSpacesToUserType(user.UserType);
                    request.Hearing_role_name = AddSpacesToUserType(user.UserType);
                }

                request.AdditionalProperties = null;
                request.Contact_email = user.ContactEmail;
                request.Display_name = user.DisplayName;
                request.First_name = user.FirstName;
                request.Last_name = user.LastName;
                request.Middle_names = DefaultData.MIDDLE_NAME;
                request.Telephone_number = DefaultData.TELEPHONE_NUMBER;
                request.Title = DefaultData.TITLE;
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

        private static string AddSpacesToUserType(UserType userType)
        {
            return string.Concat(userType.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString()))
                .TrimStart(' ');
        }
    }
}