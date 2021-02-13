using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Domain.Helpers;

namespace TestApi.Common.Builders
{
    public class UserBuilder
    {
        private readonly string _emailStem;
        private readonly int _number;
        private readonly string _numberText;
        private Application _application = Application.TestApi;
        private string _appShortName = AppShortName.FromApplication(Application.TestApi);
        private TestType _testType = TestType.Automated;
        private UserType _userType;
        private bool _isProdUser;

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

        public UserBuilder AddWinger()
        {
            _userType = UserType.Winger;
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

        public UserBuilder ForTestType(TestType testType)
        {
            _testType = testType;
            return this;
        }

        public UserBuilder IsProdUser(bool isProdUser)
        {
            _isProdUser = isProdUser;
            return this;
        }

        public CreateUserRequest BuildRequest()
        {
            var firstname = SetFirstName();
            var lastname = SetLastName();
            var username = SetUsername(firstname, lastname);
            var contactEmail = SetContactEmail(firstname, lastname);

            return new CreateUserRequest
            {
                Username = username,
                ContactEmail = contactEmail,
                FirstName = firstname,
                LastName = lastname,
                DisplayName = $"{firstname} {lastname}",
                TestType =_testType,
                UserType = _userType,
                Application = _application,
                Number = _number,
                IsProdUser = _isProdUser
            };
        }

        public User BuildUser()
        {
            var request = BuildRequest();
            return new User(request.Username, request.ContactEmail, request.FirstName, request.LastName,
                request.DisplayName, request.Number, request.TestType, request.UserType, request.Application, request.IsProdUser);
        }

        private string SetFirstName()
        {
            return _testType switch
            {
                TestType.ITHC => $"{UserData.ITHC_FIRST_NAME_PREFIX}{AddProdUserSuffix()}",
                TestType.Manual => $"{UserData.MANUAL_FIRST_NAME_PREFIX}{AddProdUserSuffix()}",
                TestType.Performance => $"{UserData.PERFORMANCE_FIRST_NAME_PREFIX}{AddProdUserSuffix()}",
                TestType.Demo => $"{UserData.DEMO_FIRST_NAME_PREFIX}{AddProdUserSuffix()}",
                _ => $"{UserData.AUTOMATED_FIRST_NAME_PREFIX}{_appShortName}{AddProdUserSuffix()}"
            };
        }

        private string AddProdUserSuffix()
        {
            return _isProdUser ? UserData.PROD_USER_SUFFIX : string.Empty;
        }

        private string SetLastName()
        {
            return $"{_userType} {_numberText}";
        }

        private string SetUsername(string firstname, string lastname)
        {
            return $"{ReplaceSpacesWithUnderscores(firstname)}.{ReplaceSpacesWithUnderscores(lastname)}@{_emailStem}"
                .ToLowerInvariant();
        }

        private string SetContactEmail(string firstname, string lastname)
        {
            return
                $"{ReplaceSpacesWithUnderscores(firstname)}.{ReplaceSpacesWithUnderscores(lastname)}@{ContactEmailStem(_emailStem)}"
                    .ToLowerInvariant();
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
            return emailStem[16..];
        }
    }
}