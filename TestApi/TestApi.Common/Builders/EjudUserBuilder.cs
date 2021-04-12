using System;
using TestApi.Common.Data;
using TestApi.Contract.Dtos;
using TestApi.Contract.Enums;

namespace TestApi.Common.Builders
{
    public class EjudUserBuilder
    {
        private readonly string _emailStem;
        private readonly int _number;
        private TestType _testType = TestType.Automated;
        private UserType _userType;

        public EjudUserBuilder(string emailStem, int number)
        {
            _emailStem = emailStem;
            _number = number;
        }

        public EjudUserBuilder AddJudge()
        {
            _userType = UserType.Judge;
            return this;
        }

        public EjudUserBuilder WithUserType(UserType userType)
        {
            _userType = userType;
            return this;
        }

        public UserDto BuildUserDto()
        {
            var firstName = SetFirstName();
            var lastName = EjudUserData.LAST_NAME(_number);
            var displayName = EjudUserData.DISPLAY_NAME(firstName, lastName);
            var contactEmail = EjudUserData.CONTACT_EMAIL(firstName, lastName, _emailStem);
            var username = EjudUserData.USERNAME(firstName, lastName, _emailStem);

            return new UserDto()
            {
                Application = Application.Ejud,
                ContactEmail = contactEmail,
                CreatedDate = DateTime.UtcNow,
                DisplayName = displayName,
                FirstName = firstName,
                IsProdUser = false,
                LastName = lastName,
                Number = _number,
                TestType = _testType,
                Username = username,
                UserType = _userType
            };
        }

        private string SetFirstName()
        {
            return _testType switch
            {
                TestType.Automated => EjudUserData.AUTOMATED_FIRST_NAME_PREFIX,
                TestType.Demo => EjudUserData.MANUAL_FIRST_NAME_PREFIX,
                TestType.ITHC => EjudUserData.MANUAL_FIRST_NAME_PREFIX,
                TestType.Manual => EjudUserData.MANUAL_FIRST_NAME_PREFIX,
                TestType.Performance => EjudUserData.MANUAL_FIRST_NAME_PREFIX,
                _ => EjudUserData.AUTOMATED_FIRST_NAME_PREFIX
            };
        }
    }
}