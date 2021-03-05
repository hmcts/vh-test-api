using System.Collections.Generic;
using System.Linq;
using TestApi.Contract.Dtos;
using TestApi.Contract.Responses;

namespace TestApi.Mappings
{
    public static class UserDetailsResponseToUserMapper
    {
        public static List<UserDto> Map(List<UserDetailsResponse> users)
        {
            return users.Select(user => new UserDto
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