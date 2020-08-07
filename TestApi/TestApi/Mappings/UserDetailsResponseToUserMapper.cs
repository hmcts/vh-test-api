using System.Collections.Generic;
using TestApi.Contract.Responses;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.Mappings
{
    public static class UserDetailsResponseToUserMapper
    {
        public static List<User> Map(List<UserDetailsResponse> users, Application application)
        {
            var userList = new List<User>();

            foreach (var user in users)
            {
                var newUser = new User()
                {
                    Application = application,
                    ContactEmail = user.ContactEmail,
                    CreatedDate = user.CreatedDate,
                    DisplayName = user.DisplayName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Number = user.Number,
                    Username = user.Username,
                    UserType = user.UserType
                };
                userList.Add(newUser);
            }

            return userList;
        }
    }
}
