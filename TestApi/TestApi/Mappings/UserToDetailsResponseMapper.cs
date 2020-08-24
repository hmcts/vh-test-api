using TestApi.Contract.Responses;
using TestApi.Domain;

namespace TestApi.Mappings
{
    public static class UserToDetailsResponseMapper
    {
        public static UserDetailsResponse MapToResponse(User user)
        {
            return new UserDetailsResponse
            {
                Id = user.Id,
                Username = user.Username,
                ContactEmail = user.ContactEmail,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                Number = user.Number,
                TestType = user.TestType,
                UserType = user.UserType,
                Application = user.Application,
                IsProdUser = user.IsProdUser,
                CreatedDate = user.CreatedDate
            };
        }
    }
}