using System;

namespace TestApi.DAL.Exceptions
{
    public class IncrementedUsernameException : Exception
    {
        public IncrementedUsernameException(string username, string incrementedUsername) : base(
            $"Expected user to be created with username {username} but username is {incrementedUsername} probably due to the user already existing and the user api incrementing the username.")
        {
        }
    }
}
