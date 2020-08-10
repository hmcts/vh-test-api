using System;

namespace TestApi.DAL.Exceptions
{
    public class AllocationAlreadyExistsException : Exception
    {
        public AllocationAlreadyExistsException(Guid userId) : base($"Allocation already exists for user with id {userId}")
        {
        }
    }
}
