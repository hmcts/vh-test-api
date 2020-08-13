using System;

namespace TestApi.Services.Exceptions
{
    public class UserDetailsMismatchException : Exception
    {
        public UserDetailsMismatchException(string userEmail, string requestEmail) : base(
            $"Contact email {userEmail} does not match {requestEmail}")
        {
        }
    }
}