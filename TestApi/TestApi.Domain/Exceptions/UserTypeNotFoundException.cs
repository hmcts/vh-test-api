using System;

namespace TestApi.Domain.Exceptions
{
    public class UserTypeNotFoundException : Exception
    {
        public UserTypeNotFoundException(string text) : base($"User type not found from text {text}")
        {
        }
    }
}