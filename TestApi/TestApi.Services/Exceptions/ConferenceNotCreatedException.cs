using System;

namespace TestApi.Services.Exceptions
{
    public class ConferenceNotCreatedException : Exception
    {
        public ConferenceNotCreatedException(Guid hearingRefId) : base($"Hearing created, but Conference with hearing ref id {hearingRefId} does not exist after 60 seconds.")
        {
        }
    }
}
