using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Services.Clients.BookingsApiClient;

namespace TestApi.Services.Builders
{
    public class BookHearingParticipantsBuilder
    {
        private const string FIRST_CASE_ROLE_NAME = "Claimant";
        private const string SECOND_CASE_ROLE_NAME = "Defendant";
        private const string FIRST_INDV_HEARING_ROLE_NAME = "Claimant LIP";
        private const string SECOND_INDV_HEARING_ROLE_NAME = "Defendant LIP";
        private const string REPRESENTATIVE_HEARING_ROLE_NAME = "Representative";

        private readonly List<User> _users;
        private readonly List<ParticipantRequest> _participants;

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
                if (user.UserType == UserType.CaseAdmin)
                {
                    continue;
                }

                var request = new ParticipantRequest();

                if (user.UserType == UserType.Individual)
                {
                    request.Case_role_name = indIndex == 0 ? FIRST_CASE_ROLE_NAME : SECOND_CASE_ROLE_NAME;
                    request.Hearing_role_name = indIndex == 0 ? FIRST_INDV_HEARING_ROLE_NAME : SECOND_INDV_HEARING_ROLE_NAME;
                    indIndex++;
                }

                if (user.UserType == UserType.Representative)
                {
                    request.Case_role_name = repIndex == 0 ? FIRST_CASE_ROLE_NAME : SECOND_CASE_ROLE_NAME;
                    request.Hearing_role_name = REPRESENTATIVE_HEARING_ROLE_NAME;
                    request.Organisation_name = Faker.Company.Name();
                    request.Reference = "Reference";
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
                request.Middle_names = string.Empty;
                request.Telephone_number = $"+44(0)7{Faker.RandomNumber.Next(900000000, 999999999)}";
                request.Title = "Mrs";
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
            return string.Concat(userType.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }
    }
}
