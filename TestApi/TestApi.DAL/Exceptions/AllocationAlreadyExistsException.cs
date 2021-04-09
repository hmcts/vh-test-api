using System;

namespace TestApi.DAL.Exceptions
{
    public class AllUsersAreAllocatedException : Exception
    {
        public AllUsersAreAllocatedException() : base(
            "All JOH users have been allocated")
        {
        }
    }
}