using System;

namespace TestApi.DAL.Exceptions
{
    public class UserUnavailableException : Exception
    {
        public UserUnavailableException() : base("User is already allocated")
        {
        }
    }
}