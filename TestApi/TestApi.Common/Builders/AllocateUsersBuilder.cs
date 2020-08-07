using System.Collections.Generic;
using TestApi.Contract.Requests;
using TestApi.Domain.Enums;

namespace TestApi.Common.Builders
{
    public class AllocateUsersBuilder
    {
        private readonly AllocateUsersRequest _request;

        public AllocateUsersBuilder()
        {
            _request = new AllocateUsersRequest();
        }

        public AllocateUsersBuilder WithUserTypes(List<UserType> userTypes)
        {
            _request.UserTypes = userTypes;
            return this;
        }

        public AllocateUsersBuilder ForApplication(Application application)
        {
            _request.Application = application;
            return this;
        }

        public AllocateUsersRequest Build()
        {
            return _request;
        }
    }
}
