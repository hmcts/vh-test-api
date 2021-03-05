using TestApi.Common.Extensions;
using TestApi.Contract.Dtos;
using TestApi.Domain;

namespace TestApi.UnitTests.Helpers
{
    public static class UserDtoToUserMapper
    {
        public static User Map(UserDto user) 
        {
            return new User
            {
                Application = user.Application.MapToContractEnum(),
                ContactEmail = user.ContactEmail,
                CreatedDate = user.CreatedDate,
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                IsProdUser = user.IsProdUser,
                LastName = user.LastName,
                Number = user.Number,
                TestType = user.TestType.MapToContractEnum(),
                Username = user.Username,
                UserType = user.UserType.MapToContractEnum()
            };
        }
    }
}
