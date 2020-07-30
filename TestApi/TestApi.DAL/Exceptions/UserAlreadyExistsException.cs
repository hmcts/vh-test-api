using System;
using TestApi.Domain.Enums;

namespace TestApi.DAL.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string username, Application application) : base($"User {username} in {application} already exists")
        {
        }

        public UserAlreadyExistsException(string username) : base($"User {username} already exists in AAD")
        {
        }
    }
}
