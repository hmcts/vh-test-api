using TestApi.Contract.Requests;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Domain.Helpers;

namespace TestApi.Contract.Builders
{
    public class UserBuilder
    {
        private readonly string _emailStem;
        private readonly string _numberText;
        private string _appShortName;
        private readonly int _number;
        private UserType _userType;
        private Application _application;

        public UserBuilder(string emailStem, int number)
        {
            _emailStem = emailStem;
            _number = number;
            _numberText = AddZerosBeforeNumber(number);
        }

        public UserBuilder AddJudge()
        {
            _userType = UserType.Judge;
            return this;
        }

        public UserBuilder AddIndividual()
        {
            _userType = UserType.Individual;
            return this;
        }

        public UserBuilder AddRepresentative()
        {
            _userType = UserType.Representative;
            return this;
        }

        public UserBuilder AddCaseAdmin()
        {
            _userType = UserType.CaseAdmin;
            return this;
        }

        public UserBuilder AddVideoHearingsOfficer()
        {
            _userType = UserType.VideoHearingsOfficer;
            return this;
        }

        public UserBuilder AddObserver()
        {
            _userType = UserType.Observer;
            return this;
        }

        public UserBuilder AddPanelMember()
        {
            _userType = UserType.PanelMember;
            return this;
        }

        public UserBuilder WithUserType(UserType userType)
        {
            _userType = userType;
            return this;
        }

        public UserBuilder ForApplication(Application application)
        {
            _application = application;
            _appShortName = AppShortName.FromApplication(_application);
            return this;
        }

        public CreateUserRequest BuildRequest()
        {
            var firstname = SetFirstName();
            var lastname = SetLastName();
            var username = SetUsername(firstname, lastname);
            var contactEmail = SetContactEmail(firstname, lastname);

            return new CreateUserRequest()
            {
                Username = username,
                ContactEmail = contactEmail,
                FirstName = firstname,
                LastName = lastname,
                DisplayName = $"{firstname} {lastname}",
                UserType = _userType,
                Application = _application,
                Number = _number
            };
        }

        public User BuildUser()
        {
            var request = BuildRequest();
            return new User(request.Username, request.ContactEmail, request.FirstName, request.LastName,
                request.DisplayName, request.Number, request.UserType, request.Application);
        }

        private string SetFirstName()
        {
            return $"Auto_{_appShortName}";
        }

        private string SetLastName()
        {
            return $"{_userType}_{_numberText}";
        }

        private string SetUsername(string firstname, string lastname)
        {
            return $"{ReplaceSpacesWithUnderscores(firstname)}.{ReplaceSpacesWithUnderscores(lastname)}@{_emailStem}".ToLowerInvariant();
        }

        private string SetContactEmail(string firstname, string lastname)
        {
            return $"{ReplaceSpacesWithUnderscores(firstname)}.{ReplaceSpacesWithUnderscores(lastname)}@{ContactEmailStem(_emailStem)}".ToLowerInvariant();
        }

        private static string ReplaceSpacesWithUnderscores(string text)
        {
            return text.Replace(" ", "_");
        }

        private static string AddZerosBeforeNumber(int number)
        {
            return number.ToString("D2");
        }

        private static string ContactEmailStem(string emailStem)
        {
            return emailStem.Substring(16);
        }
    }
}
