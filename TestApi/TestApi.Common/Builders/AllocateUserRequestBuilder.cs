using TestApi.Common.Data;
using TestApi.Contract.Requests;
using TestApi.Contract.Enums;

namespace TestApi.Common.Builders
{
    public class AllocateUserRequestBuilder
    {
        private readonly AllocateUserRequest _request;

        public AllocateUserRequestBuilder()
        {
            _request = new AllocateUserRequest
            {
                AllocatedBy = EmailData.TEST_WEB_MANUAL_USER,
                Application = Application.TestApi,
                ExpiryInMinutes = 1,
                IsEjud = false,
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

        public AllocateUserRequestBuilder IsEjud()
        {
            _request.IsEjud = true;
            return this;
        }

        public AllocateUserRequestBuilder IsEjud(bool isEjud)
        {
            _request.IsEjud = isEjud;
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

        public AllocateUserRequestBuilder WithAllocatedBy(string allocatedBy)
        {
            _request.AllocatedBy = allocatedBy;
            return this;
        }

        public AllocateUserRequest Build()
        {
            return _request;
        }
    }
}