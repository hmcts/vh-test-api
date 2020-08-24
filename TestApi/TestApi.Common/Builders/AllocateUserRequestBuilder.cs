using TestApi.Contract.Requests;
using TestApi.Domain.Enums;

namespace TestApi.Common.Builders
{
    public class AllocateUserRequestBuilder
    {
        private readonly AllocateUserRequest _request;

        public AllocateUserRequestBuilder()
        {
            _request = new AllocateUserRequest
            {
                Application = Application.TestApi,
                ExpiryInMinutes = 1,
                IsProdUser = false,
                TestType = TestType.Automated,
                UserType = UserType.Individual
            };
        }

        public AllocateUserRequestBuilder WithUserType(UserType userType)
        {
            _request.UserType = userType;
            return this;
        }

        public AllocateUserRequestBuilder ForApplication(Application application)
        {
            _request.Application = application;
            return this;
        }

        public AllocateUserRequestBuilder WithExpiryInMinutes(int expiresInMinutes)
        {
            _request.ExpiryInMinutes = expiresInMinutes;
            return this;
        }

        public AllocateUserRequestBuilder IsProdUser()
        {
            _request.IsProdUser = true;
            return this;
        }

        public AllocateUserRequestBuilder ForTestType(TestType testType)
        {
            _request.TestType = testType;
            return this;
        }

        public AllocateUserRequest Build()
        {
            return _request;
        }
    }
}