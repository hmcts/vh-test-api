using System.Collections.Generic;
using TestApi.Contract.Requests;
using TestApi.Contract.Enums;

namespace TestApi.Common.Builders
{
    public class AllocateUsersRequestBuilder
    {
        private const UserType JUDGE_USER_TYPE = UserType.Judge;
        private const UserType INDIVIDUAL_USER_TYPE = UserType.Individual;
        private const UserType REPRESENTATIVE_USER_TYPE = UserType.Representative;
        private const UserType CASE_ADMIN_USER_TYPE = UserType.CaseAdmin;
        private const UserType VIDEO_HEARINGS_OFFICER_USER_TYPE = UserType.VideoHearingsOfficer;
        private readonly AllocateUsersRequest _request;

        public AllocateUsersRequestBuilder()
        {
            _request = new AllocateUsersRequest {ExpiryInMinutes = 1, TestType = TestType.Automated};
        }

        public AllocateUsersRequestBuilder WithUserTypes(List<UserType> userTypes)
        {
            _request.UserTypes = userTypes;
            return this;
        }

        public AllocateUsersRequestBuilder WithDefaultTypes()
        {
            var userTypes = new List<UserType>
                {JUDGE_USER_TYPE, INDIVIDUAL_USER_TYPE, REPRESENTATIVE_USER_TYPE, CASE_ADMIN_USER_TYPE};
            _request.UserTypes = userTypes;
            return this;
        }

        public AllocateUsersRequestBuilder WithVideoHearingsOfficer()
        {
            var userTypes = new List<UserType>
                {JUDGE_USER_TYPE, INDIVIDUAL_USER_TYPE, REPRESENTATIVE_USER_TYPE, VIDEO_HEARINGS_OFFICER_USER_TYPE};
            _request.UserTypes = userTypes;
            return this;
        }

        public AllocateUsersRequestBuilder WithEmptyUsers()
        {
            _request.UserTypes = new List<UserType>();
            return this;
        }

        public AllocateUsersRequestBuilder WithMoreThanOneJudge()
        {
            WithDefaultTypes();
            _request.UserTypes.Add(UserType.Judge);
            return this;
        }

        public AllocateUsersRequestBuilder ForApplication(Application application)
        {
            _request.Application = application;
            return this;
        }

        public AllocateUsersRequestBuilder WithTestType(TestType testType)
        {
            _request.TestType = testType;
            return this;
        }

        public AllocateUsersRequestBuilder WithExpiryInMinutes(int expiryInMinutes)
        {
            _request.ExpiryInMinutes = expiryInMinutes;
            return this;
        }

        public AllocateUsersRequest Build()
        {
            return _request;
        }
    }
}