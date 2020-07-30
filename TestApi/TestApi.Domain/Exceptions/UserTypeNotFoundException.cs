using System;

namespace TestApi.DAL.Exceptions
{
    public class UserTypeNotFoundException : Exception
    {
        public UserTypeNotFoundException(string text) : base($"User type not found from text {text}")
        {
        }
    }
}
