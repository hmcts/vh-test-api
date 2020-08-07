using System.Linq;
using AcceptanceTests.Common.Model.Case;
using AcceptanceTests.Common.Model.Hearing;
using FluentAssertions;
using TestApi.Contract.Requests;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.Common.Builders
{
    public class ADUserBuilder
    {
        private readonly CreateUserRequest _userDetails;
        private readonly CreateADUserRequest _request;

        public ADUserBuilder(CreateUserRequest userDetails)
        {
            _request = new CreateADUserRequest();
            _userDetails = userDetails;
        }

        public CreateADUserRequest BuildRequest()
        {
            AddDefaultArguments();

            _request.FirstName = _userDetails.FirstName;
            _request.MiddleNames = string.Empty;
            _request.LastName = _userDetails.LastName;
            _request.DisplayName = _userDetails.DisplayName;
            _request.ContactEmail = _userDetails.ContactEmail;
            _request.Username = _userDetails.Username;

            ValidateArguments();

            return _request;
        }

        public ADUser BuildUser()
        {
            var request = BuildRequest();
            return new ADUser(request.Title, request.FirstName, request.MiddleNames, request.LastName, request.DisplayName,
                request.Username, request.ContactEmail, request.CaseRoleName, request.HearingRoleName, request.Reference,
            request.Representee, request.OrganisationName, request.TelephoneNumber);
        }

        private void AddDefaultArguments()
        {
            if (_userDetails.UserType != UserType.Individual && _userDetails.UserType != UserType.Representative)
            {
                var role = ConvertUserTypeToHearingRoleName(_userDetails.UserType);
                _request.CaseRoleName = role;
                _request.HearingRoleName = role;
            }

            if (_userDetails.UserType == UserType.Individual)
            {
                _request.CaseRoleName = CaseRole.Claimant.ToString();
                _request.HearingRoleName = HearingRole.ClaimantLip.ToString();
                _request.Reference = Faker.RandomNumber.Next(1, 100).ToString();
            }

            if (_userDetails.UserType == UserType.Representative)
            {
                _request.CaseRoleName = CaseRole.Claimant.ToString();
                _request.HearingRoleName = HearingRole.Representative.ToString();
                _request.OrganisationName = Faker.Company.Name();
                _request.Representee = "Individual";
            }

            _request.Title = "Mrs";
            _request.TelephoneNumber = Faker.Phone.Number();
        }

        private static string ConvertUserTypeToHearingRoleName(UserType userType)
        {
            return string.Concat(userType.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }

        private void ValidateArguments()
        {
            _request.CaseRoleName.Should().NotBeNullOrWhiteSpace();
            _request.HearingRoleName.Should().NotBeNullOrWhiteSpace();

            if (_userDetails.UserType == UserType.Representative)
            {
                _request.Representee.Should().NotBeNullOrWhiteSpace();
            }

            _request.Title.Should().NotBeNullOrWhiteSpace();
            _request.FirstName.Should().NotBeNullOrWhiteSpace();
            _request.MiddleNames.Should().BeNullOrEmpty();
            _request.LastName.Should().NotBeNullOrWhiteSpace();
            _request.DisplayName.Should().NotBeNullOrWhiteSpace();
            _request.ContactEmail.Should().NotBeNullOrWhiteSpace();
            _request.Username.Should().NotBeNullOrWhiteSpace();
            _request.TelephoneNumber.Should().NotBeNullOrWhiteSpace();
        }
    }
}
