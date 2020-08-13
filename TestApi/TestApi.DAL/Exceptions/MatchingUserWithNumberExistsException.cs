using System;
using TestApi.Domain.Enums;

namespace TestApi.DAL.Exceptions
{
    public class MatchingUserWithNumberExistsException : Exception
    {
        public MatchingUserWithNumberExistsException(UserType userType, int number) : base(
            $"User of type {userType} with number {number} already exists")
        {
        }
    }
}