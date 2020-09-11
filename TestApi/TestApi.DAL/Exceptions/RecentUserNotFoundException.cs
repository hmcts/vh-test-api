using System;

namespace TestApi.DAL.Exceptions
{
    public class RecentUserNotFoundException : Exception
    {
        public RecentUserNotFoundException(string username) : base(
            $"Recent user with username {username} could not be found")
        {
        }
    }
}
