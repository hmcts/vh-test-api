using System;
using TestApi.Domain.Enums;

namespace TestApi.Common.Exceptions
{
    public class ApplicationNotPermittedException : Exception
    {
        public ApplicationNotPermittedException(Application application) : base(
            $"Application type: {application} not permitted")
        {
        }
    }
}