using System;
using System.Collections.Generic;
using TestApi.Domain;
using TestApi.Services.Clients.UserApiClient;

namespace TestApi.Services.Builders.Responses
{
    public class UserProfileResponseBuilder
    {
        private readonly UserProfile _response;

        public UserProfileResponseBuilder(User user)
        {
            _response = new UserProfile
            {
                Case_type = new List<string>(),
                Display_name = user.DisplayName,
                Email = user.ContactEmail,
                First_name = user.FirstName,
                Last_name = user.LastName,
                User_id = Guid.NewGuid().ToString(),
                User_role = "",
                User_name = user.Username
            };
        }

        public UserProfile Build()
        {
            return _response;
        }
    }
}
