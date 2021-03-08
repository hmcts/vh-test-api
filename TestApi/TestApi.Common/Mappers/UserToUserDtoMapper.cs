using TestApi.Common.Extensions;
using TestApi.Contract.Dtos;
using TestApi.Domain;

namespace TestApi.Common.Mappers
{
    public static class UserToUserDtoMapper
    {
        public static UserDto Map(User user)
        {
            return new UserDto
            {
                Application = user.Application.MapToContractEnum(),
                ContactEmail = user.ContactEmail,
                CreatedDate = user.CreatedDate,
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                Id = user.Id,
                IsProdUser = user.IsProdUser,
                LastName = user.LastName,
                Number = user.Number,
                TestType = user.TestType.MapToContractEnum(),
                UserType = user.UserType.MapToContractEnum(),
                Username = user.Username
            };
        }
    }
}
