using System;
using TestApi.Domain.Enums;

namespace TestApi.DAL.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(Guid userId) : base($"User with id {userId} does not exist")
        {}

        public UserNotFoundException(string username) : base($"User {username} does not exist")
        {}

        public UserNotFoundException(UserType userType, Application application, int number)
            : base($"No users found matching user type {userType}, application {application} and number {number}")
        {}
    }
}
