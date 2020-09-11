using System;

namespace TestApi.DAL.Exceptions
{
    public class RecentUserAlreadyExistsException : Exception
    {
        public RecentUserAlreadyExistsException(string username) : base(
            $"Recent user already exists for username {username}")
        {
        }
    }
}
