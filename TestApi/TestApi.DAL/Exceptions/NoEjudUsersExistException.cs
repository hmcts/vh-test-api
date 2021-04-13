using System;

namespace TestApi.DAL.Exceptions
{
    public class NoEjudUsersExistException : Exception
    {
        public NoEjudUsersExistException() : base($"No Ejud Users found. Are you sure the migrations have ran?")
        {
        }
    }
}