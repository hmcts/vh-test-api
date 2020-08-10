using System.Collections.Generic;
using System.Linq;
using TestApi.Contract.Responses;
using TestApi.Domain;

namespace TestApi.Mappings
{
    public static class UserDetailsResponseToUserMapper
    {
        public static List<User> Map(List<UserDetailsResponse> users)
        {
            return users.Select(user => new User()
                {
                    Application = user.Application,
                    ContactEmail = user.ContactEmail,
                    CreatedDate = user.CreatedDate,
                    DisplayName = user.DisplayName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Number = user.Number,
                    Username = user.Username,
                    UserType = user.UserType
                })
                .ToList();
        }
    }
}
